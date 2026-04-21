using UnityEditor;
using ToolkitEngine.Rendering;

namespace ToolkitEditor.Rendering
{
	[CustomEditor(typeof(IntMaterialModifier))]
    public class IntMaterialModifierEditor : BaseMaterialModifierEditor
    {
        #region Fields

        protected SerializedProperty m_source;
        protected SerializedProperty m_destination;

        #endregion

        #region Methods

        protected override void OnEnable()
        {
            base.OnEnable();

            m_source = serializedObject.FindProperty(nameof(m_source));
            m_destination = serializedObject.FindProperty(nameof(m_destination));
        }

        protected override void DrawProperties()
        {
            EditorGUILayout.Separator();

			EditorGUILayout.PropertyField(m_source);
			EditorGUILayout.PropertyField(m_destination);
		}

        #endregion
    }
}