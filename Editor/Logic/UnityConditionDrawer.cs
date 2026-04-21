using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using ToolkitEngine;

#if USE_COMPONENT_NAMES
using Sisus.ComponentNames;
#endif

namespace ToolkitEditor
{
	[CustomPropertyDrawer(typeof(UnityCondition))]
	public class UnityConditionDrawer : PropertyDrawer
	{
		#region Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			label.tooltip = property.tooltip;

			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.PropertyField(position, property.FindPropertyRelative("m_arguments"), label);
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_arguments"));
		}

		#endregion
	}

	[CustomPropertyDrawer(typeof(UnityCondition.Argument))]
	public class UnityConditionArgumenDrawer : PropertyDrawer
	{
		#region Enumerators

		public enum MemberType
		{
			Boolean,
			Int32,
			Single,
		}

		#endregion

		#region Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var condition = property.GetValue() as UnityCondition;

			EditorGUI.BeginProperty(position, label, property);

			var objectProperty = property.FindPropertyRelative("m_object");
			var conditionTypeProperty = property.FindPropertyRelative("m_conditionType");
			var componentProperty = property.FindPropertyRelative("m_component");
			var memberNameProperty = property.FindPropertyRelative("m_memberName");
			var isPropertyProperty = property.FindPropertyRelative("m_isProperty");

			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), objectProperty, new GUIContent());

			// Changed object, reset component and member name
			if (EditorGUI.EndChangeCheck())
			{
				componentProperty.objectReferenceValue = null;
				memberNameProperty.stringValue = string.Empty;
			}

			EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), conditionTypeProperty, new GUIContent());

			var memberRect = new Rect(
				position.x + EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing,
				position.y,
				position.width - (EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing),
				EditorGUIUtility.singleLineHeight);

			string selectionLabel = string.Empty;
			if (componentProperty.objectReferenceValue != null && !string.IsNullOrWhiteSpace(memberNameProperty.stringValue))
			{
#if USE_COMPONENT_NAMES
				string componentDisplayName = ((Component)componentProperty.objectReferenceValue).GetName();
#else
				string componentDisplayName = componentProperty.objectReferenceValue.GetType().Name;
#endif
				selectionLabel = string.Format("{0}.{1}", componentDisplayName, memberNameProperty.stringValue);
			}

			if (EditorGUI.DropdownButton(memberRect, new GUIContent(selectionLabel), FocusType.Keyboard) && objectProperty.objectReferenceValue != null)
			{
				var menu = new GenericMenu();

				var gameObject = objectProperty.objectReferenceValue as GameObject;
				if (gameObject != null)
				{
					PopulateDropDown(menu, gameObject.GetComponents<Component>(), property);
				}
				else
				{
					var component = objectProperty.objectReferenceValue as Component;
					if (component != null)
					{
						PopulateDropDown(menu, component.GetComponents<Component>(), property);
					}
				}

				menu.DropDown(memberRect);
			}

			if (TryGetMemberType((Component)componentProperty.objectReferenceValue, memberNameProperty.stringValue, isPropertyProperty.boolValue, out MemberType memberType))
			{
				var argumentRect = new Rect(
					position.x + EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing,
					position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
					position.width - (EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing),
					EditorGUIUtility.singleLineHeight);

				switch (memberType)
				{
					case MemberType.Boolean:
						EditorGUI.PropertyField(argumentRect, property.FindPropertyRelative("m_boolArgument"), new GUIContent());
						break;

					case MemberType.Int32:
						EditorGUI.PropertyField(argumentRect, property.FindPropertyRelative("m_intArgument"), new GUIContent());
						break;

					case MemberType.Single:
						EditorGUI.PropertyField(argumentRect, property.FindPropertyRelative("m_floatArgument"), new GUIContent());
						break;
				}
			}

			EditorGUI.EndProperty();
		}

		private void PopulateDropDown(GenericMenu menu, Component[] components, SerializedProperty property)
		{
			// Group components by type to handle duplicates
			var componentGroups = components
				.Select((comp, index) => new { Component = comp, Index = index })
				.GroupBy(x => x.Component.GetType());

			foreach (var group in componentGroups)
			{
				var componentsOfType = group.ToList();
				bool hasMultiple = componentsOfType.Count > 1;

				for (int i = 0; i < componentsOfType.Count; i++)
				{
					var componentData = componentsOfType[i];
					var component = componentData.Component;

					string displayName;
#if USE_COMPONENT_NAMES
					string componentName = component.GetName();

					// Check if multiple components in this group have the same custom name
					var componentsWithSameName = componentsOfType
						.Where(x => string.Equals(x.Component.GetName(), componentName))
						.ToList();

					bool hasMultipleWithSameName = componentsWithSameName.Count > 1;

					if (hasMultipleWithSameName)
					{
						// Find the index within components that share the same name
						int nameIndex = componentsWithSameName.IndexOf(componentData);
						displayName = $"{componentName} [{nameIndex}]";
					}
					else
					{
						displayName = componentName;
					}
#else
					// Get component type name
					string componentTypeName = component.GetType().Name;

					// If multiple of same type, add index to differentiate
					displayName = hasMultiple
						? $"{componentTypeName} [{i}]"
						: componentTypeName;
#endif

					// Add fields
					AddMenuItems<FieldInfo>(
						component.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance),
						(field) => field.FieldType == typeof(int) || field.FieldType == typeof(float) || field.FieldType == typeof(bool),
						menu,
						HandleMemberInfoClicked,
						property,
						component,
						displayName,
						false);

					// Add properties
					AddMenuItems<PropertyInfo>(
						component.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance),
						(prop) => prop.PropertyType == typeof(int) || prop.PropertyType == typeof(float) || prop.PropertyType == typeof(bool),
						menu,
						HandleMemberInfoClicked,
						property,
						component,
						displayName,
						true);
				}
			}
		}

		private void AddMenuItems<T>(
			IEnumerable<MemberInfo> items,
			System.Func<T, bool> predicate,
			GenericMenu menu,
			GenericMenu.MenuFunction2 clickedHandler,
			SerializedProperty property,
			Component component,
			string componentDisplayName,
			bool isProperty)
			where T : MemberInfo
		{
			foreach (var item in items.OrderBy(x => x.Name))
			{
				if (!predicate((T)item))
					continue;

				menu.AddItem(
					new GUIContent($"{componentDisplayName}/{item.Name}"), false, clickedHandler, new MenuEventArgs()
					{
						property = property,
						component = component,
						memberInfo = item,
						isProperty = isProperty
					});
			}
		}

		private void HandleMemberInfoClicked(object args)
		{
			var menuEventArgs = (MenuEventArgs)args;

			menuEventArgs.property.serializedObject.Update();
			menuEventArgs.property.FindPropertyRelative("m_component").objectReferenceValue = menuEventArgs.component;
			menuEventArgs.property.FindPropertyRelative("m_memberName").stringValue = menuEventArgs.memberInfo.Name;
			menuEventArgs.property.FindPropertyRelative("m_isProperty").boolValue = menuEventArgs.isProperty;
			menuEventArgs.property.serializedObject.ApplyModifiedProperties();
		}

		public bool TryGetMemberType(Component component, string memberName, bool isProperty, out MemberType memberType)
		{
			memberType = 0;

			if (component == null || string.IsNullOrWhiteSpace(memberName))
				return false;

			if (!isProperty)
			{
				var fieldInfo = component.GetType().GetField(memberName, BindingFlags.Public | BindingFlags.Instance);
				if (fieldInfo != null)
				{
					if (fieldInfo.FieldType == typeof(int))
					{
						memberType = MemberType.Int32;
						return true;
					}
					if (fieldInfo.FieldType == typeof(float))
					{
						memberType = MemberType.Single;
						return true;
					}
					if (fieldInfo.FieldType == typeof(bool))
					{
						memberType = MemberType.Boolean;
						return true;
					}
				}
			}
			else
			{
				var propInfo = component.GetType().GetProperty(memberName, BindingFlags.Public | BindingFlags.Instance);
				if (propInfo != null)
				{
					if (propInfo.PropertyType == typeof(int))
					{
						memberType = MemberType.Int32;
						return true;
					}
					if (propInfo.PropertyType == typeof(float))
					{
						memberType = MemberType.Single;
						return true;
					}
					if (propInfo.PropertyType == typeof(bool))
					{
						memberType = MemberType.Boolean;
						return true;
					}
				}
			}

			return false;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight(property, label)
				+ EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
		}

		#endregion

		#region Structures

		private struct MenuEventArgs
		{
			public SerializedProperty property;
			public Component component;
			public MemberInfo memberInfo;
			public bool isProperty;
		}

		#endregion
	}
}