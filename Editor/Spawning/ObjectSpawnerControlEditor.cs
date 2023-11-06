using UnityEditor;
using ToolkitEngine;

namespace ToolkitEditor
{
	[CustomEditor(typeof(ObjectSpawnerControl)), CanEditMultipleObjects]
    public class ObjectSpawnerControlEditor : Editor
    {
		#region Fields

		protected SerializedProperty m_autoSpawn;
		protected SerializedProperty m_isOn;
		protected SerializedProperty m_queueable;
		protected SerializedProperty m_objectSpawner;

		// Rate settings
		protected SerializedProperty m_delayTime;
		protected SerializedProperty m_minDelayTime;
		protected SerializedProperty m_maxDelayTime;

		// Limit settings
		protected SerializedProperty m_isInfinite;
		protected SerializedProperty m_maxCount;
		protected SerializedProperty m_isSimultaneousInfinite;
		protected SerializedProperty m_maxSimultaneousCount;
		protected SerializedProperty m_useDelayForVacancy;

		protected SerializedProperty m_blockers;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_autoSpawn = serializedObject.FindProperty(nameof(m_autoSpawn));
			m_isOn = serializedObject.FindProperty(nameof(m_isOn));
			m_queueable = serializedObject.FindProperty(nameof(m_queueable));
			m_objectSpawner = serializedObject.FindProperty(nameof(m_objectSpawner));

			m_delayTime = serializedObject.FindProperty(nameof(m_delayTime));

			m_isInfinite = serializedObject.FindProperty(nameof(m_isInfinite));
			m_maxCount = serializedObject.FindProperty(nameof(m_maxCount));
			m_isSimultaneousInfinite = serializedObject.FindProperty(nameof(m_isSimultaneousInfinite));
			m_maxSimultaneousCount = serializedObject.FindProperty(nameof(m_maxSimultaneousCount));
			m_useDelayForVacancy = serializedObject.FindProperty(nameof(m_useDelayForVacancy));

			m_blockers = serializedObject.FindProperty(nameof(m_blockers));
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(m_autoSpawn);
			if (m_autoSpawn.boolValue)
			{
				++EditorGUI.indentLevel;
				EditorGUILayout.PropertyField(m_isOn);
				--EditorGUI.indentLevel;
			}

			EditorGUILayout.PropertyField(m_queueable);

			EditorGUILayout.Separator();

			// Rate Settings
			EditorGUILayout.PropertyField(m_delayTime);

			EditorGUILayout.Separator();

			// Limit Settings
			EditorGUILayout.PropertyField(m_isInfinite);
			if (!m_isInfinite.boolValue)
			{
				++EditorGUI.indentLevel;
				EditorGUILayout.PropertyField(m_maxCount);
				--EditorGUI.indentLevel;
			}

			EditorGUILayout.PropertyField(m_isSimultaneousInfinite);
			if (!m_isSimultaneousInfinite.boolValue)
			{
				++EditorGUI.indentLevel;
				if (m_isInfinite.boolValue)
				{
					EditorGUILayout.PropertyField(m_maxSimultaneousCount);
				}
				else
				{
					EditorGUILayout.IntSlider(m_maxSimultaneousCount, 1, m_maxCount.intValue);
				}
				--EditorGUI.indentLevel;
			}

			if (!m_isSimultaneousInfinite.boolValue && (m_isInfinite.boolValue || m_maxCount.intValue != m_maxSimultaneousCount.intValue))
			{
				EditorGUILayout.PropertyField(m_useDelayForVacancy);
			}

			EditorGUILayout.Separator();

			EditorGUILayout.PropertyField(m_blockers);

			serializedObject.ApplyModifiedProperties();
		}

		#endregion
	}
}