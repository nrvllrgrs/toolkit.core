using UnityEngine;
using UnityEditor;
using ToolkitEngine;

namespace ToolkitEditor
{
    [CustomPropertyDrawer(typeof(ObjectActivation))]
    public class ObjectActivationDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var objectRect = new Rect(position);
            objectRect.width -= EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            var activeRect = new Rect(position);
            activeRect.x = objectRect.x + objectRect.width + (EditorGUIUtility.standardVerticalSpacing * 2f);
			activeRect.width = EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(objectRect, property.FindPropertyRelative("m_object"), new GUIContent(string.Empty));
            EditorGUI.PropertyField(activeRect, property.FindPropertyRelative("m_active"), new GUIContent(string.Empty));
        }
    }
}