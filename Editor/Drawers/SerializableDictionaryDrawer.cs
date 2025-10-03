using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace ToolkitEngine
{
	[CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
	public class SerializableDictionaryDrawer : PropertyDrawer
	{
		#region Fields

		private IDictionary m_dictionary;
		private Type m_keyType;
		private Type m_valueType;

		private dynamic m_keyValue;

		private static GUIStyle s_buttonStyle;
		private static GUIContent s_addEntryContent;
		private static GUIContent s_removeEntryContent;
		private const float BUTTON_WIDTH = 18f;

		#endregion

		#region Properties

		private static GUIStyle BUTTON_STYLE
		{
			get
			{
				if (s_buttonStyle == null)
				{
					s_buttonStyle = new GUIStyle(EditorStyles.miniButton);
					s_buttonStyle.padding = new RectOffset(2, 2, 2, 2);
				}
				return s_buttonStyle;
			}
		}

		private static GUIContent ADD_ENTRY_CONTENT
		{
			get
			{
				if (s_addEntryContent == null)
				{
					s_addEntryContent = new GUIContent("Add Item");
					s_addEntryContent.tooltip = "Add Item";
				}
				return s_addEntryContent;
			}
		}

		private static GUIContent REMOVE_ENTRY_CONTENT
		{
			get
			{
				if (s_removeEntryContent == null)
				{
					s_removeEntryContent = new GUIContent(EditorGUIUtility.IconContent("Close"));
					s_removeEntryContent.tooltip = "Remove Item";
				}
				return s_removeEntryContent;

			}
		}

		#endregion

		#region Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			label.tooltip = property.tooltip;

			if (m_dictionary == null)
			{
				m_dictionary = property.GetValue<IDictionary>();

				var arguments = m_dictionary.Keys.GetType().GetGenericArguments();
				m_keyType = arguments[0];
				m_valueType = arguments[1];
			}

			if (!EditorGUIRectLayout.Foldout(ref position, property, label))
				return;

			var keysProp = property.FindPropertyRelative("keys");
			var valuesProp = property.FindPropertyRelative("values");

			UpdateKey(ref position);

			EditorGUI.BeginDisabledGroup(m_keyValue == null || m_dictionary.Contains(m_keyValue));
			if (EditorGUIRectLayout.Button(ref position, ADD_ENTRY_CONTENT))
			{
				m_dictionary.Add(m_keyValue, m_valueType.IsValueType
					? Activator.CreateInstance(m_valueType)
					: null);
			}
			EditorGUI.EndDisabledGroup();

			EditorGUIRectLayout.Space(ref position);

			for (int i = 0; i < keysProp.arraySize; ++i)
			{
				var keyProp = keysProp.GetArrayElementAtIndex(i);
				var valueProp = valuesProp.GetArrayElementAtIndex(i);

				var keyRect = position;
				keyRect.width = EditorGUIUtility.labelWidth - EditorGUIUtility.standardVerticalSpacing;

				var valueRect = position;
				valueRect.x = keyRect.xMax + EditorGUIUtility.standardVerticalSpacing;
				valueRect.width = position.width - (EditorGUIUtility.labelWidth + BUTTON_WIDTH + EditorGUIUtility.standardVerticalSpacing * 2f);

				EditorGUI.BeginDisabledGroup(true);
				EditorGUI.PropertyField(keyRect, keyProp, new GUIContent(string.Empty));
				EditorGUI.EndDisabledGroup();

				EditorGUI.PropertyField(valueRect, valueProp, new GUIContent(string.Empty));

				var removeRect = valueRect;
				removeRect.x = valueRect.xMax + EditorGUIUtility.standardVerticalSpacing;
				removeRect.width = BUTTON_WIDTH;

				// Shift vertical position before potential break
				position.y += Mathf.Max(EditorGUI.GetPropertyHeight(keyProp), EditorGUI.GetPropertyHeight(valueProp))
					+ EditorGUIUtility.standardVerticalSpacing;

				if (GUI.Button(removeRect, REMOVE_ENTRY_CONTENT, BUTTON_STYLE))
				{
					m_dictionary.Remove(keyProp.GetValue());
					break;
				}
			}
		}

		private void UpdateKey(ref Rect position)
		{
			if (typeof(UnityObject).IsAssignableFrom(m_keyType))
			{
				m_keyValue = EditorGUI.ObjectField(position, m_keyValue, m_keyType, true);
				position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
				return;
			}

			switch (m_keyType)
			{
				case null:
					EditorGUIRectLayout.LabelField(ref position, "null");
					return;

				//case long:
				//	m_keyValue = EditorGUI.LongField(rect, m_keyValue);
				case Type intType when intType == typeof(int):
					m_keyValue = EditorGUIRectLayout.IntField(ref position, "Key", m_keyValue);
					return;

				case Type floatType when floatType == typeof(float):
					m_keyValue = EditorGUIRectLayout.FloatField(ref position, "Key", m_keyValue);
					return;

				//case float: return (T)(object)EditorGUI.FloatField(rect, (float)(object)value);
				//case double: return (T)(object)EditorGUI.DoubleField(rect, (double)(object)value);
				case Type stringType when stringType == typeof(string):
					m_keyValue = EditorGUIRectLayout.TextField(ref position, "Key", m_keyValue);
					return;

				//case bool: return (T)(object)EditorGUI.Toggle(rect, (bool)(object)value);
				//case Vector2Int: return (T)(object)EditorGUI.Vector2IntField(rect, GUIContent.none, (Vector2Int)(object)value);
				//case Vector3Int: return (T)(object)EditorGUI.Vector3IntField(rect, GUIContent.none, (Vector3Int)(object)value);
				//case Vector2: return (T)(object)EditorGUI.Vector2Field(rect, GUIContent.none, (Vector2)(object)value);
				//case Vector3: return (T)(object)EditorGUI.Vector3Field(rect, GUIContent.none, (Vector3)(object)value);
				//case Vector4: return (T)(object)EditorGUI.Vector4Field(rect, GUIContent.none, (Vector4)(object)value);
				//case BoundsInt: return (T)(object)EditorGUI.BoundsIntField(rect, (BoundsInt)(object)value);
				//case Bounds: return (T)(object)EditorGUI.BoundsField(rect, (Bounds)(object)value);
				//case RectInt: return (T)(object)EditorGUI.RectIntField(rect, (RectInt)(object)value);
				//case Rect: return (T)(object)EditorGUI.RectField(rect, (Rect)(object)value);
				//case Color: return (T)(object)EditorGUI.ColorField(rect, (Color)(object)value);
				//case AnimationCurve: return (T)(object)EditorGUI.CurveField(rect, (AnimationCurve)(object)value);
				//case Gradient: return (T)(object)EditorGUI.GradientField(rect, (Gradient)(object)value);
				//case UnityObject: return (T)(object)EditorGUI.ObjectField(rect, (UnityObject)(object)value, type, true);
			}

			if (m_keyType.IsEnum)
			{
				List<string> values = new();
				foreach (var enumValue in Enum.GetValues(m_keyType))
				{
					values.Add(enumValue.ToString());
				}

				m_keyValue ??= 0;
				m_keyValue = EditorGUIRectLayout.Popup(ref position, "Key", m_keyValue, values.ToArray());
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float height = EditorGUIRectLayout.GetFoldoutHeight(property);
			if (property.isExpanded)
			{
				height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2f;
				height += EditorGUIRectLayout.GetSpaceHeight();

				var keysProp = property.FindPropertyRelative("keys");
				var valuesProp = property.FindPropertyRelative("values");

				for (int i = 0; i < keysProp.arraySize; ++i)
				{
					var keyProp = keysProp.GetArrayElementAtIndex(i);
					var valueProp = valuesProp.GetArrayElementAtIndex(i);

					height += Mathf.Max(EditorGUI.GetPropertyHeight(keyProp), EditorGUI.GetPropertyHeight(valueProp))
						+ EditorGUIUtility.standardVerticalSpacing;
				}
			}

			return height;
		}

		#endregion
	}
}