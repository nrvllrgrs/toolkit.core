using ToolkitEngine.VisualScripting;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace ToolkitEditor.VisualScripting
{
	[CustomPropertyDrawer(typeof(ScriptGraphProperty), true)]
	public class ScriptGraphPropertyDrawer : BasePropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			var valueProperty = property.FindPropertyRelative("m_value");
			if (valueProperty == null)
			{
				EditorGUI.LabelField(position, "m_value field not found");
				EditorGUI.EndProperty();
				return;
			}

			var ownerObject = property.serializedObject.targetObject as ScriptableObject;
			EditorGUIRectLayout.ScriptableObjectField<ScriptGraphAsset>(ref position, valueProperty, ownerObject, label);

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}
	}
}