using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(VectorLabelAttribute))]
public class VectorLabelDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var vectorLabelAttr = attribute as VectorLabelAttribute;

		if (property.propertyType == SerializedPropertyType.Vector2)
		{
			var value = property.vector2Value;
			EditorGUIRectLayout.LabelField(ref position, property.displayName);

			++EditorGUI.indentLevel;

			value.x = EditorGUIRectLayout.FloatField(ref position, vectorLabelAttr.labels[0], value.x);
			value.y = EditorGUIRectLayout.FloatField(ref position, vectorLabelAttr.labels[1], value.y);
			property.vector2Value = value;

			--EditorGUI.indentLevel;
		}
		else if (property.propertyType == SerializedPropertyType.Vector3)
		{
			var value = property.vector3Value;
			EditorGUIRectLayout.LabelField(ref position, property.displayName);

			++EditorGUI.indentLevel;

			value.x = EditorGUIRectLayout.FloatField(ref position, vectorLabelAttr.labels[0], value.x);
			value.y = EditorGUIRectLayout.FloatField(ref position, vectorLabelAttr.labels[1], value.y);
			value.z = EditorGUIRectLayout.FloatField(ref position, vectorLabelAttr.labels[2], value.z);
			property.vector3Value = value;

			--EditorGUI.indentLevel;
		}
		else if (property.propertyType == SerializedPropertyType.Vector4)
		{
			var value = property.vector4Value;
			EditorGUIRectLayout.LabelField(ref position, property.displayName);

			++EditorGUI.indentLevel;

			value.x = EditorGUIRectLayout.FloatField(ref position, vectorLabelAttr.labels[0], value.x);
			value.y = EditorGUIRectLayout.FloatField(ref position, vectorLabelAttr.labels[1], value.y);
			value.z = EditorGUIRectLayout.FloatField(ref position, vectorLabelAttr.labels[2], value.z);
			value.w = EditorGUIRectLayout.FloatField(ref position, vectorLabelAttr.labels[3], value.w);
			property.vector4Value = value;

			--EditorGUI.indentLevel;
		}
		else if (property.propertyType == SerializedPropertyType.Vector2Int)
		{
			var value = property.vector2IntValue;
			EditorGUIRectLayout.LabelField(ref position, property.displayName);

			++EditorGUI.indentLevel;

			value.x = EditorGUIRectLayout.IntField(ref position, vectorLabelAttr.labels[0], value.x);
			value.y = EditorGUIRectLayout.IntField(ref position, vectorLabelAttr.labels[1], value.y);
			property.vector2Value = value;

			--EditorGUI.indentLevel;
		}
		else if (property.propertyType == SerializedPropertyType.Vector3Int)
		{
			var value = property.vector3IntValue;
			EditorGUIRectLayout.LabelField(ref position, property.displayName);

			++EditorGUI.indentLevel;

			value.x = EditorGUIRectLayout.IntField(ref position, vectorLabelAttr.labels[0], value.x);
			value.y = EditorGUIRectLayout.IntField(ref position, vectorLabelAttr.labels[1], value.y);
			value.z = EditorGUIRectLayout.IntField(ref position, vectorLabelAttr.labels[2], value.z);
			property.vector3Value = value;

			--EditorGUI.indentLevel;
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if (property.propertyType == SerializedPropertyType.Vector2 || property.propertyType == SerializedPropertyType.Vector2Int)
		{
			return (EditorGUIUtility.singleLineHeight * 3f)
				+ (EditorGUIUtility.standardVerticalSpacing * 3f);
		}
		else if (property.propertyType == SerializedPropertyType.Vector3 || property.propertyType == SerializedPropertyType.Vector3Int)
		{
			return (EditorGUIUtility.singleLineHeight * 4f)
				+ (EditorGUIUtility.standardVerticalSpacing * 4f);
		}
		else if (property.propertyType == SerializedPropertyType.Vector4)
		{
			return (EditorGUIUtility.singleLineHeight * 5f)
				+ (EditorGUIUtility.standardVerticalSpacing * 5f);
		}

		return EditorGUI.GetPropertyHeight(property);
	}
}