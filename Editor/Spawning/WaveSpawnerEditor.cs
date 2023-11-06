using UnityEngine;
using UnityEditor;
using ToolkitEngine;

namespace ToolkitEditor
{
	[CustomEditor(typeof(WaveSpawner))]
    public class WaveSpawnerEditor : BaseToolkitEditor
    {
		#region Fields

		protected WaveSpawner m_waveSpawner;

		protected SerializedProperty m_spawner;
		protected SerializedProperty m_waves;
		protected SerializedProperty m_thresholdSet;
		protected SerializedProperty m_points;
		protected SerializedProperty m_spawnOnStart;
		protected SerializedProperty m_despawnOnTimeout;

		protected SerializedProperty m_onSpawning;
		protected SerializedProperty m_onSpawned;
		protected SerializedProperty m_onDespawned;
		protected SerializedProperty m_onCompleted;
		protected SerializedProperty m_onTimeout;
		protected SerializedProperty m_onExhausted;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_waveSpawner = target as WaveSpawner;

			m_spawner = serializedObject.FindProperty(nameof(m_spawner));
			m_waves = serializedObject.FindProperty(nameof(m_waves));
			m_thresholdSet = serializedObject.FindProperty(nameof(m_thresholdSet));
			m_points = serializedObject.FindProperty(nameof(m_points));
			m_spawnOnStart = serializedObject.FindProperty(nameof(m_spawnOnStart));

			m_onSpawning = serializedObject.FindProperty(nameof(m_onSpawning));
			m_onSpawned = serializedObject.FindProperty(nameof(m_onSpawned));
			m_onDespawned = serializedObject.FindProperty(nameof(m_onDespawned));
			m_onCompleted = serializedObject.FindProperty(nameof(m_onCompleted));
			m_onTimeout = serializedObject.FindProperty(nameof(m_onTimeout));
			m_onExhausted = serializedObject.FindProperty(nameof(m_onExhausted));
		}

		protected override void DrawProperties()
		{
			EditorGUILayout.PropertyField(m_spawner);

			EditorGUILayout.Separator();

			EditorGUILayout.PropertyField(m_waves);
			EditorGUILayout.PropertyField(m_points);

			if (m_thresholdSet.objectReferenceValue == null)
			{
				EditorGUILayout.HelpBox("Set is required!", MessageType.Error);
			}
			EditorGUILayout.PropertyField(m_thresholdSet);

			EditorGUILayout.Separator();

			EditorGUILayout.PropertyField(m_spawnOnStart);
		}

		protected override void DrawEvents()
		{
			if (EditorGUILayoutUtility.Foldout(m_onSpawning, "Spawner Events"))
			{
				EditorGUILayout.PropertyField(m_onSpawning);
				EditorGUILayout.PropertyField(m_onSpawned);
				EditorGUILayout.PropertyField(m_onDespawned);

				DrawNestedEvents();
			}

			EditorGUILayout.Separator();

			if (EditorGUILayoutUtility.Foldout(m_onCompleted, "Wave Events"))
			{
				EditorGUILayout.PropertyField(m_onCompleted);
				EditorGUILayout.PropertyField(m_onTimeout);
				EditorGUILayout.PropertyField(m_onExhausted);
			}
		}

		protected virtual void OnSceneGUI()
		{
			if (m_waveSpawner == null)
				return;

			GUIStyle style = new GUIStyle();
			style.normal.textColor = Color.green;

			for (int i = 0; i < m_waveSpawner.points.Length; ++i)
			{
				var point = m_waveSpawner.points[i];
				if (point == null)
					continue;

				Handles.Label(point.position, i.ToString(), style);
			}
		}

		#endregion
	}
}