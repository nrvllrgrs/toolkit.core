using ToolkitEngine;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace ToolkitEditor
{
	[CustomPropertyDrawer(typeof(BaseProperty), true)]
	public class BasePropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			// Get the actual BaseProperty instance
			var baseProperty = property.managedReferenceValue as BaseProperty;
			if (baseProperty == null)
			{
				EditorGUI.LabelField(position, "Property is null");
				EditorGUI.EndProperty();
				return;
			}

			// Try to find the m_value property through SerializedProperty
			// This will work even for private fields if they're serializable
			var valueProperty = property.FindPropertyRelative("m_value");
			if (valueProperty != null)
			{
				// Let Unity's property drawer system handle it
				// This will automatically use custom property drawers
				EditorGUI.BeginChangeCheck();
				EditorGUI.PropertyField(position, valueProperty, GetValueLabel(baseProperty), true);

				if (EditorGUI.EndChangeCheck())
				{
					EditorUtility.SetDirty(property.serializedObject.targetObject);
				}
			}
			else
			{
				// Get the m_value field through reflection
				var valueField = baseProperty.GetType().BaseType.GetField("m_value", BindingFlags.NonPublic | BindingFlags.Instance);
				if (valueField == null)
				{
					EditorGUI.LabelField(position, "m_value field not found via reflection");
					EditorGUI.EndProperty();
					return;
				}

				// Get current value
				var currentValue = valueField.GetValue(baseProperty);

				// Draw field based on type
				EditorGUI.BeginChangeCheck();
				var newValue = DrawValueField(position, baseProperty.valueType, currentValue);

				// Set new value if changed
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(property.serializedObject.targetObject, "Change Property Value");
					valueField.SetValue(baseProperty, newValue);
					EditorUtility.SetDirty(property.serializedObject.targetObject);
				}
			}

			EditorGUI.EndProperty();
		}

		protected virtual object DrawValueField(Rect position, System.Type type, object value)
		{
			if (type == typeof(bool))
			{
				return EditorGUI.Toggle(position, (bool)(value ?? false));
			}
			else if (type == typeof(int))
			{
				return EditorGUI.IntField(position, (int)(value ?? 0));
			}
			else if (type == typeof(float))
			{
				return EditorGUI.FloatField(position, (float)(value ?? 0f));
			}
			else if (type == typeof(string))
			{
				return EditorGUI.TextField(position, (string)(value ?? string.Empty));
			}
			else if (type == typeof(Vector2))
			{
				return EditorGUI.Vector2Field(position, GUIContent.none, (Vector2)(value ?? Vector2.zero));
			}
			else if (type == typeof(Vector3))
			{
				return EditorGUI.Vector3Field(position, GUIContent.none, (Vector3)(value ?? Vector3.zero));
			}
			else if (type == typeof(Vector2Int))
			{
				return EditorGUI.Vector2IntField(position, GUIContent.none, (Vector2Int)(value ?? Vector2Int.zero));
			}
			else if (type == typeof(Vector3Int))
			{
				return EditorGUI.Vector3IntField(position, GUIContent.none, (Vector3Int)(value ?? Vector3Int.zero));
			}
			else if (type == typeof(Vector4))
			{
				return EditorGUI.Vector4Field(position, GUIContent.none, (Vector4)(value ?? Vector4.zero));
			}
			else if (type == typeof(Color))
			{
				return EditorGUI.ColorField(position, (Color)(value ?? Color.white));
			}
			else if (typeof(Object).IsAssignableFrom(type))
			{
				return EditorGUI.ObjectField(position, (Object)value, type, true);
			}
			else
			{
				EditorGUI.LabelField(position, $"Unsupported type: {type.Name}");
			}

			return value;
		}

		private GUIContent GetValueLabel(BaseProperty baseProperty)
		{
			var valueType = baseProperty.valueType;
			if (!valueType.IsGenericType || valueType.GetGenericTypeDefinition() != typeof(List<>))
				return GUIContent.none;

			string elementName = valueType.GetGenericArguments()[0].Name;
			return new GUIContent($"List<{elementName}>");
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			// Prefer asking Unity directly — handles lists, arrays, and all nested types correctly
			var valueProperty = property.FindPropertyRelative("m_value");
			if (valueProperty != null)
				return EditorGUI.GetPropertyHeight(valueProperty, true);

			var baseProperty = property.GetValue<BaseProperty>();
			if (baseProperty == null)
				return EditorGUIUtility.singleLineHeight;

			// Vector types need more height
			if (baseProperty.valueType == typeof(Vector3)
				|| baseProperty.valueType == typeof(Vector3Int)
				|| baseProperty.valueType == typeof(Vector4))
			{
				return EditorGUIUtility.singleLineHeight * 2;
			}

			return EditorGUIUtility.singleLineHeight;
		}
	}
}