using UnityEditor;
using ToolkitEngine;

namespace ToolkitEditor
{
	[CustomEditor(typeof(PoolItemManager))]
	public class PoolItemManagerEditor : BaseToolkitEditor
    {
		#region Fields

		protected SerializedProperty m_dontDestroyOnLoad;
		protected SerializedProperty m_config;

		#endregion

		#region Methods

		protected virtual void OnEnable()
		{
			m_dontDestroyOnLoad = serializedObject.FindProperty(nameof(m_dontDestroyOnLoad));
			m_config = serializedObject.FindProperty(nameof(m_config));
		}

		protected override void DrawProperties()
		{
			EditorGUILayout.PropertyField(m_dontDestroyOnLoad);
			EditorGUILayout.PropertyField(m_config);
		}

		#endregion
	}
}