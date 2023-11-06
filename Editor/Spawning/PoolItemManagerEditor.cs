using UnityEditor;
using ToolkitEngine;

namespace ToolkitEditor
{
	[CustomEditor(typeof(PoolItemManager))]
	public class PoolItemManagerEditor : BaseToolkitEditor
    {
		#region Fields

		protected SerializedProperty m_dontDestroyOnLoad;
		protected SerializedProperty m_spawners;

		#endregion

		#region Methods

		protected virtual void OnEnable()
		{
			m_dontDestroyOnLoad = serializedObject.FindProperty(nameof(m_dontDestroyOnLoad));
			m_spawners = serializedObject.FindProperty(nameof(m_spawners));
		}

		protected override void DrawProperties()
		{
			EditorGUILayout.PropertyField(m_dontDestroyOnLoad);
			EditorGUILayout.PropertyField(m_spawners);
		}

		#endregion
	}
}