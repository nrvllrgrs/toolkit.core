using UnityEngine;
using UnityEditor;
using ToolkitEngine;
using static ToolkitEngine.ObjectSpawner;

namespace ToolkitEditor
{
	[CustomEditor(typeof(ObjectSpawner)), CanEditMultipleObjects]
    public class ObjectSpawnerEditor : Editor
    {
		#region Fields

		protected ObjectSpawner m_objectSpawner;

		protected SerializedProperty m_spawner;

		protected SerializedProperty m_spawnOnStart;

		protected SerializedProperty m_points;
		protected SerializedProperty m_zeroRotation;
		protected SerializedProperty m_order;
		protected SerializedProperty m_parent;
		protected SerializedProperty m_spawnSpace;
		protected SerializedProperty m_position;
		protected SerializedProperty m_rotation;

		protected SerializedProperty m_preSpawnRules;
		protected SerializedProperty m_postSpawnRules;

		protected SerializedProperty m_onSpawning;
		protected SerializedProperty m_onSpawned;
		protected SerializedProperty m_onDespawned;

		#endregion

		#region Methods

		protected virtual void OnEnable()
		{
			m_objectSpawner = target as ObjectSpawner;

			m_spawner = serializedObject.FindProperty(nameof(m_spawner));

			m_spawnOnStart = serializedObject.FindProperty(nameof(m_spawnOnStart));

			m_points = serializedObject.FindProperty(nameof(m_points));
			m_zeroRotation = serializedObject.FindProperty(nameof(m_zeroRotation));
			m_order = serializedObject.FindProperty(nameof(m_order));
			m_parent = serializedObject.FindProperty(nameof(m_parent));
			m_spawnSpace = serializedObject.FindProperty(nameof(m_spawnSpace));
			m_position = serializedObject.FindProperty(nameof(m_position));
			m_rotation = serializedObject.FindProperty(nameof(m_rotation));

			m_preSpawnRules = serializedObject.FindProperty(nameof(m_preSpawnRules));
			m_postSpawnRules = serializedObject.FindProperty(nameof(m_postSpawnRules));

			m_onSpawning = m_spawner.FindPropertyRelative(nameof(m_onSpawning));
			m_onSpawned = m_spawner.FindPropertyRelative(nameof(m_onSpawned));
			m_onDespawned = m_spawner.FindPropertyRelative(nameof(m_onDespawned));
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(m_spawner);

			EditorGUILayout.Separator();

			EditorGUILayout.PropertyField(m_spawnOnStart);
			EditorGUILayout.PropertyField(m_spawnSpace);

			++EditorGUI.indentLevel;
			switch ((SpawnSpace)m_spawnSpace.intValue)
			{
				case SpawnSpace.SpawnAtPoint:
					if (m_points.arraySize == 0)
					{
						EditorGUILayout.HelpBox("\"Points\" are undefined. Using local transform.", MessageType.Info);
					}
					EditorGUILayout.PropertyField(m_points);

					if (m_points.arraySize > 1)
					{
						EditorGUILayout.PropertyField(m_order);
					}
					EditorGUILayout.PropertyField(m_zeroRotation);
					break;

				case SpawnSpace.SpawnInLocalSpace:
				case SpawnSpace.SpawnInWorldSpace:
					if (m_parent.objectReferenceValue == null)
					{
						EditorGUILayout.HelpBox("Parent is required!", MessageType.Error);
					}
					EditorGUILayout.PropertyField(m_parent);
					break;

				case SpawnSpace.PositionAndRotation:
					EditorGUILayout.PropertyField(m_position);
					EditorGUILayout.PropertyField(m_rotation);
					break;
			}
			--EditorGUI.indentLevel;

			EditorGUILayout.Separator();

			EditorGUILayout.PropertyField(m_preSpawnRules);
			EditorGUILayout.PropertyField(m_postSpawnRules);

			EditorGUILayout.Separator();

			if (EditorGUILayoutUtility.Foldout(m_onSpawning, "Events"))
			{
				EditorGUILayout.PropertyField(m_onSpawning);
				EditorGUILayout.PropertyField(m_onSpawned);
				EditorGUILayout.PropertyField(m_onDespawned);
			}

			serializedObject.ApplyModifiedProperties();
		}

		protected virtual void OnSceneGUI()
		{
			if (m_objectSpawner == null)
				return;

			var orderMode = (OrderMode)m_order.enumValueIndex;
			if (m_objectSpawner.hasPoints && (orderMode == OrderMode.Sequence || orderMode == OrderMode.Indexed))
			{
				GUIStyle style = new GUIStyle();
				style.normal.textColor = Color.green;

				for (int i = 0; i < m_objectSpawner.points.Length; ++i)
				{
					var point = m_objectSpawner.points[i];
					if (point == null)
						continue;

					Handles.Label(point.position, i.ToString(), style);
				}
			}
		}

		#endregion
	}
}