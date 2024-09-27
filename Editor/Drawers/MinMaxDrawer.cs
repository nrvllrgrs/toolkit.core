using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MinMaxAttribute))]
public class MinMaxDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var minMaxAttr = attribute as MinMaxAttribute;
		label.tooltip = property.tooltip;

		if (property.propertyType == SerializedPropertyType.Vector2)
		{
			Vector2 value = property.vector2Value;
			EditorGUIRectLayout.LabelField(ref position, label);

			++EditorGUI.indentLevel;

			EditorGUI.BeginChangeCheck();
			value.x = EditorGUIRectLayout.FloatField(ref position, minMaxAttr.minLabel, value.x);

			if (EditorGUI.EndChangeCheck())
			{
				value.x = Mathf.Max(Mathf.Min(value.x, value.y), minMaxAttr.minValue);
			}

			EditorGUI.BeginChangeCheck();
			value.y = EditorGUIRectLayout.FloatField(ref position, minMaxAttr.maxLabel, value.y);

			if (EditorGUI.EndChangeCheck())
			{
				value.y = Mathf.Min(Mathf.Max(value.x, value.y), minMaxAttr.maxValue);
			}

			--EditorGUI.indentLevel;

			property.vector2Value = value;
		}
		else if (property.propertyType == SerializedPropertyType.Vector2Int)
		{
			Vector2Int value = property.vector2IntValue;
			EditorGUIRectLayout.LabelField(ref position, label);

			++EditorGUI.indentLevel;

			EditorGUI.BeginChangeCheck();
			value.x = EditorGUIRectLayout.IntField(ref position, minMaxAttr.minLabel, value.x);

			if (EditorGUI.EndChangeCheck())
			{
				value.x = (int)Mathf.Max(Mathf.Min(value.x, value.y), minMaxAttr.minValue);
			}

			EditorGUI.BeginChangeCheck();
			value.y = EditorGUIRectLayout.IntField(ref position, minMaxAttr.maxLabel, value.y);

			if (EditorGUI.EndChangeCheck())
			{
				value.y = (int)Mathf.Min(Mathf.Max(value.x, value.y), minMaxAttr.maxValue);
			}

			--EditorGUI.indentLevel;

			property.vector2IntValue = value;
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return (EditorGUIUtility.singleLineHeight * 3f)
			+ (EditorGUIUtility.standardVerticalSpacing * 3f);
	}
}