using System;
using System.Collections.Generic;
using System.Linq;
using ToolkitEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ToolkitEditor
{
	[CustomPropertyDrawer(typeof(PropertyCollection))]
	public class PropertyCollectionDrawer : PropertyDrawer
	{
		#region Fields

		private static IEnumerable<Type> s_cachedPropertyTypes = null;

		// Used to map (SerializedObject, propertyPath) to cached data (stable across repaints)
		private static Dictionary<(int, string), CachedData> s_dataLookup = new();

		// Used to map reorderable list to cached data
		private static Dictionary<ReorderableList, CachedData> s_listLookup = new();

		private const string PROPERTY_SUFFIX = "Property";

		#endregion

		#region Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			label.tooltip = property.tooltip;

			if (EditorGUIRectLayout.Foldout(ref position, property, label))
			{
				// Cache all BaseProperty types on first use
				if (s_cachedPropertyTypes == null)
				{
					s_cachedPropertyTypes = from a in AppDomain.CurrentDomain.GetAssemblies()
											from t in a.GetTypes()
											where t.IsClass && !t.IsAbstract && typeof(BaseProperty).IsAssignableFrom(t)
											let attr = t.GetAttribute<GenericMenuCategoryAttribute>()
											let menuPath = attr != null ? $"{attr.category}/{t.Name}" : t.Name
											orderby attr == null, GetMenuSortKey(menuPath)
											select t;
				}

				// Use SerializedObject instance ID + property path as unique key
				var cacheKey = (property.serializedObject.GetHashCode(), property.propertyPath);

				// Get or create cached data
				if (!s_dataLookup.TryGetValue(cacheKey, out CachedData data))
				{
					var keysProperty = property.FindPropertyRelative("keys");
					var valuesProperty = property.FindPropertyRelative("values");

					if (keysProperty == null || valuesProperty == null)
					{
						Debug.LogError($"Could not find keys or values in PropertyCollection at path: {property.propertyPath}");
						return;
					}

					data = new CachedData()
					{
						property = property,
						keys = keysProperty,
						values = valuesProperty,
						reorderableList = new ReorderableList(property.serializedObject, valuesProperty, true, true, true, true)
					};

					// Draw header with Key and Value labels
					data.reorderableList.drawHeaderCallback = (rect) =>
					{
						float keyWidth = rect.width * 0.35f;
						float valueWidth = rect.width * 0.65f;

						Rect keyRect = new Rect(rect.x, rect.y, keyWidth - 5f, rect.height);
						Rect valueRect = new Rect(rect.x + keyWidth, rect.y, valueWidth, rect.height);

						EditorGUI.LabelField(keyRect, "Key");
						EditorGUI.LabelField(valueRect, "Value");
					};

					// Draw each element
					data.reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
					{
						if (!index.Between(0, data.values.arraySize))
							return;

						SerializedProperty keyProp = data.keys.GetArrayElementAtIndex(index);
						SerializedProperty valueProp = data.values.GetArrayElementAtIndex(index);
						if (keyProp == null || valueProp == null)
							return;

						float keyWidth = rect.width * 0.35f;
						float valueWidth = rect.width * 0.65f;

						Rect keyRect = new Rect(rect.x, rect.y + 2f, keyWidth - 5f, EditorGUI.GetPropertyHeight(keyProp));

						bool isList = valueProp.managedReferenceValue is BasePropertyList;
						float offset = isList ? 15f : 0f;
						Rect valueRect = new Rect(rect.x + keyWidth + offset, rect.y, valueWidth - offset, EditorGUI.GetPropertyHeight(valueProp));

						// Draw key field
						EditorGUI.PropertyField(keyRect, keyProp, GUIContent.none);

						// Draw value field (delegates to BasePropertyDrawer)
						EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none, true);
					};

					// Calculate element height
					data.reorderableList.elementHeightCallback = (index) =>
					{
						if (!index.Between(0, data.values.arraySize))
							return EditorGUIUtility.singleLineHeight;

						SerializedProperty keyProp = data.keys.GetArrayElementAtIndex(index);
						SerializedProperty valueProp = data.values.GetArrayElementAtIndex(index);
						if (keyProp == null || valueProp == null)
							return EditorGUIUtility.singleLineHeight;

						return Mathf.Max(
							EditorGUI.GetPropertyHeight(keyProp),
							EditorGUI.GetPropertyHeight(valueProp, true));
					};

					// Setup callbacks
					data.reorderableList.onCanAddCallback = OnCanAddCallback;
					data.reorderableList.onAddDropdownCallback = OnAddDropdownCallback;
					data.reorderableList.onReorderCallbackWithDetails = OnReorderCallbackWithDetails;
					data.reorderableList.onCanRemoveCallback = OnCanRemoveCallback;
					data.reorderableList.onRemoveCallback = OnRemoveCallback;

					s_dataLookup.Add(cacheKey, data);
					s_listLookup.Add(data.reorderableList, data);
				}

				// Draw the list at the position after foldout (position was updated by ref)
				data.reorderableList.DoList(position);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float height = EditorGUIUtility.singleLineHeight; // Property label

			if (!property.isExpanded)
				return height;

			var cacheKey = (property.serializedObject.GetHashCode(), property.propertyPath);
			if (s_dataLookup.TryGetValue(cacheKey, out CachedData data))
			{
				try
				{
					height += data.reorderableList.GetHeight();
				}
				catch
				{
					// Clean up corrupted cache entries
					s_listLookup.Remove(data.reorderableList);
					s_dataLookup.Remove(cacheKey);
				}
			}

			return height;
		}

		[Obsolete]
		public override bool CanCacheInspectorGUI(SerializedProperty property) => true;

		private static string GetMenuSortKey(string menuPath)
		{
			var parts = menuPath.Split('/');
			var stringBuilder = new System.Text.StringBuilder();
			for (int i = 0; i < parts.Length; ++i)
			{
				bool isFolder = i < parts.Length - 1;

				stringBuilder.Append(isFolder ? '\x00' : '\x7f');
				stringBuilder.Append(parts[i]);
				if (isFolder)
				{
					stringBuilder.Append('/');
				}
			}
			return stringBuilder.ToString();
		}

		#endregion

		#region ReorderableList Callbacks

		private bool OnCanAddCallback(ReorderableList list)
		{
			return true;
		}

		private void OnAddDropdownCallback(Rect buttonRect, ReorderableList list)
		{
			var addMenu = new GenericMenu();

			foreach (var propertyType in s_cachedPropertyTypes)
			{
				string category = string.Empty;
				var attr = propertyType.GetAttribute<GenericMenuCategoryAttribute>();
				if (attr != null)
				{
					category += attr.category + "/";
				}

				// Remove "Property" suffix for cleaner menu
				string displayMenu = category + GetDisplayName(propertyType);
				addMenu.AddItem(new GUIContent(displayMenu), false, OnAddProperty, new MenuEventArgs()
				{
					reorderableList = list,
					type = propertyType,
				});
			}

			addMenu.ShowAsContext();
		}

		private void OnAddProperty(object parameter)
		{
			if (parameter == null)
				return;

			var args = (MenuEventArgs)parameter;
			if (!s_listLookup.TryGetValue(args.reorderableList, out CachedData data))
				return;

			data.property.serializedObject.Update();

			// Generate unique key name
			string uniqueKey = GetUniqueKey(data.keys, GetDisplayName(args.type));

			// Add new key
			data.keys.arraySize++;
			var newKey = data.keys.GetArrayElementAtIndex(data.keys.arraySize - 1);
			newKey.stringValue = uniqueKey;

			// Add new value
			data.values.arraySize++;
			var newValue = data.values.GetArrayElementAtIndex(data.values.arraySize - 1);
			newValue.managedReferenceValue = Activator.CreateInstance(args.type);

			data.property.serializedObject.ApplyModifiedProperties();

			// Explicitly mark dirty
			EditorUtility.SetDirty(data.property.serializedObject.targetObject);

			// Select the newly added element
			args.reorderableList.index = data.values.arraySize - 1;
		}

		private string GetDisplayName(Type propertyType)
		{
			// Remove "Property" suffix for cleaner menu
			string displayName = propertyType.Name;
			if (displayName.EndsWith(PROPERTY_SUFFIX))
			{
				displayName = displayName.Substring(0, displayName.Length - PROPERTY_SUFFIX.Length);
			}

			return displayName;
		}

		private string GetUniqueKey(SerializedProperty keys, string baseName)
		{
			HashSet<string> existingKeys = new HashSet<string>();
			for (int i = 0; i < keys.arraySize; i++)
			{
				existingKeys.Add(keys.GetArrayElementAtIndex(i).stringValue);
			}

			// If base name is available, use it
			if (!existingKeys.Contains(baseName))
				return baseName;

			// Otherwise, append numbers
			int counter = 1;
			string key;
			do
			{
				key = $"{baseName}{counter}";
				counter++;
			}
			while (existingKeys.Contains(key));

			return key;
		}

		private void OnReorderCallbackWithDetails(ReorderableList list, int oldIndex, int newIndex)
		{
			if (!s_listLookup.TryGetValue(list, out CachedData data))
				return;

			// ReorderableList automatically reordered the 'values' array,
			// but we need to manually reorder the 'keys' array to match
			if (oldIndex != newIndex && oldIndex >= 0 && oldIndex < data.keys.arraySize)
			{
				// Move the key to match the value reordering
				data.keys.MoveArrayElement(oldIndex, newIndex);
			}
			data.property.serializedObject.ApplyModifiedProperties();
		}

		private bool OnCanRemoveCallback(ReorderableList list)
		{
			if (!s_listLookup.TryGetValue(list, out CachedData data))
				return false;

			return data.values.arraySize > 0;
		}

		private void OnRemoveCallback(ReorderableList list)
		{
			if (!s_listLookup.TryGetValue(list, out CachedData data))
				return;

			if (list.index < 0 || list.index >= data.values.arraySize)
				return;

			data.property.serializedObject.Update();

			// Remove both key and value
			data.keys.DeleteArrayElementAtIndex(list.index);
			data.values.DeleteArrayElementAtIndex(list.index);

			data.property.serializedObject.ApplyModifiedProperties();

			// Clamp index to valid range
			list.index = Mathf.Clamp(list.index, 0, data.values.arraySize - 1);
		}

		#endregion

		#region Structures

		private class CachedData
		{
			public SerializedProperty property;
			public SerializedProperty keys;
			public SerializedProperty values;
			public ReorderableList reorderableList;
		}

		private struct MenuEventArgs
		{
			public ReorderableList reorderableList;
			public Type type;
		}

		#endregion
	}
}