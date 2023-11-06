using UnityEngine;
using UnityEditor;
using ToolkitEngine;

namespace ToolkitEditor
{
    [CustomEditor(typeof(Set))]
    public class SetEditor : Editor
    {
        #region Fields

        protected SerializedProperty m_items;
        protected SerializedProperty m_removeOnDestroyed;
		protected SerializedProperty m_removeOnKilled;
        protected SerializedProperty m_delay;
        protected SerializedProperty m_releaseOnRemoved;

		// Events
		protected SerializedProperty m_onItemAdded;
        protected SerializedProperty m_onItemRemoved;

        #endregion

        #region Methods

        private void OnEnable()
        {
            m_items = serializedObject.FindProperty(nameof(m_items));
            m_removeOnDestroyed = serializedObject.FindProperty(nameof(m_removeOnDestroyed));
			m_removeOnKilled = serializedObject.FindProperty(nameof(m_removeOnKilled));
			m_delay = serializedObject.FindProperty(nameof(m_delay));
			m_releaseOnRemoved = serializedObject.FindProperty(nameof(m_releaseOnRemoved));

			// Events
			m_onItemAdded = serializedObject.FindProperty("onItemAdded");
            m_onItemRemoved = serializedObject.FindProperty("onItemRemoved");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUILayout.PropertyField(m_items);
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(m_removeOnDestroyed);

			EditorGUILayout.PropertyField(m_removeOnKilled);
            if (m_removeOnKilled.boolValue)
            {
                ++EditorGUI.indentLevel;
				EditorGUILayout.PropertyField(m_delay);
				--EditorGUI.indentLevel;
			}

			EditorGUILayout.PropertyField(m_releaseOnRemoved);

            EditorGUILayout.Separator();

            if (EditorGUILayoutUtility.Foldout(m_onItemAdded, "Events"))
            {
                EditorGUILayout.PropertyField(m_onItemAdded);
                EditorGUILayout.PropertyField(m_onItemRemoved);
            }

            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}