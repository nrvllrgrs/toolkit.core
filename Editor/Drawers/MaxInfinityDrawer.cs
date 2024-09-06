using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MaxInfinityAttribute))]
public class MaxInfinityDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		float value = 0f;
		switch (property.propertyType)
		{
			case SerializedPropertyType.Float:
				value = property.floatValue;
				if (value < 0f)
				{
					value = float.PositiveInfinity;
				}
				property.floatValue = EditorGUIRectLayout.FloatField(ref position, label, value);
				break;
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property);
	}
}