using UnityEngine;
using UnityEditor;
using ToolkitEngine.Rendering;

namespace ToolkitEditor.Rendering
{
    [CustomEditor(typeof(ColorMaterialModifier))]
    public class ColorMaterialModifierEditor : BaseMaterialModifierEditor
    {
        #region Fields

        protected SerializedProperty m_hdr;
        protected SerializedProperty m_source;
        protected SerializedProperty m_destination;
        protected SerializedProperty m_hdrSource;
        protected SerializedProperty m_hdrDestination;

        #endregion

        #region Methods

        protected override void OnEnable()
        {
            base.OnEnable();

            m_hdr = serializedObject.FindProperty(nameof(m_hdr));
            m_source = serializedObject.FindProperty(nameof(m_source));
            m_destination = serializedObject.FindProperty(nameof(m_destination));
            m_hdrSource = serializedObject.FindProperty(nameof(m_hdrSource));
            m_hdrDestination = serializedObject.FindProperty(nameof(m_hdrDestination));
        }

        protected override void DrawProperties()
        {
            EditorGUILayout.Separator();

            if (m_hdr.boolValue)
            {
				EditorGUILayout.PropertyField(m_hdrSource, new GUIContent("Source"));
				EditorGUILayout.PropertyField(m_hdrDestination, new GUIContent("Destination"));
			}
            else
            {
				EditorGUILayout.PropertyField(m_source);
				EditorGUILayout.PropertyField(m_destination);
			}
			EditorGUILayout.PropertyField(m_hdr, new GUIContent("HDR"));
		}

        #endregion
    }
}