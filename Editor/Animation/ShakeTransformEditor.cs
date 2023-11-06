using UnityEngine;
using UnityEditor;
using ToolkitEngine;
using Sirenix.Utilities;
using System.Linq;

namespace ToolkitEditor
{
	[CustomEditor(typeof(ShakeTransform))]
	public class TransformShakeEditor : BaseToolkitEditor
    {
		#region Fields

		protected SerializedProperty m_transform;
		protected SerializedProperty m_shakeOnStart;
		protected SerializedProperty m_blockers;
		protected ShakeSettingsEditor m_positionSettings;
		protected ShakeSettingsEditor m_rotationSettings;
		protected ShakeSettingsEditor m_scaleSettings;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_transform = serializedObject.FindProperty(nameof(m_transform));
			m_shakeOnStart = serializedObject.FindProperty(nameof(m_shakeOnStart));
			m_blockers = serializedObject.FindProperty(nameof(m_blockers));

			m_positionSettings = new();
			m_positionSettings.Initialize(serializedObject.FindProperty("m_positionSettings"), true);

			m_rotationSettings = new();
			m_rotationSettings.Initialize(serializedObject.FindProperty("m_rotationSettings"), false);

			m_scaleSettings = new();
			m_scaleSettings.Initialize(serializedObject.FindProperty("m_scaleSettings"), false);
		}

		protected override void InitializeKnownSerializedPropertyNames()
		{
			base.InitializeKnownSerializedPropertyNames();
			knownSerializedPropertyNames = knownSerializedPropertyNames.Concat(new[]
			{
				"m_positionSettings",
				"m_rotationSettings",
				"m_scaleSettings"
			}).ToArray();
		}

		protected override void DrawProperties()
		{
			EditorGUILayout.PropertyField(m_transform);
			EditorGUILayout.PropertyField(m_shakeOnStart);
			EditorGUILayout.PropertyField(m_blockers);

			m_positionSettings.DrawProperties("Shake Position");
			m_rotationSettings.DrawProperties("Shake Rotation");
			m_scaleSettings.DrawProperties("Shake Scale");
		}

		#endregion

		#region Structures

		protected class ShakeSettingsEditor
		{
			#region Fields

			public SerializedProperty m_shake;
			public SerializedProperty m_duration;
			public SerializedProperty m_strength;
			public SerializedProperty m_vibrato;
			public SerializedProperty m_randomness;
			public SerializedProperty m_snapping = null;
			public SerializedProperty m_fadeOut;
			public SerializedProperty m_randomnessMode;

			#endregion

			#region Methods

			public void Initialize(SerializedProperty property, bool includeSnapping)
			{
				m_shake = property.FindPropertyRelative(nameof(m_shake));
				m_duration = property.FindPropertyRelative(nameof(m_duration));
				m_strength = property.FindPropertyRelative(nameof(m_strength));
				m_vibrato = property.FindPropertyRelative(nameof(m_vibrato));
				m_randomness = property.FindPropertyRelative(nameof(m_randomness));
				m_fadeOut = property.FindPropertyRelative(nameof(m_fadeOut));
				m_randomnessMode = property.FindPropertyRelative(nameof(m_randomnessMode));

				if (includeSnapping)
				{
					m_snapping = property.FindPropertyRelative(nameof(m_snapping));
				}
			}

			public void DrawProperties(string label)
			{
				EditorGUILayout.PropertyField(m_shake, new GUIContent(label));
				if (m_shake.boolValue)
				{
					++EditorGUI.indentLevel;

					EditorGUILayout.PropertyField(m_duration);
					EditorGUILayout.PropertyField(m_strength);
					EditorGUILayout.PropertyField(m_vibrato);
					EditorGUILayout.PropertyField(m_randomness);

					if (m_snapping != null)
					{
						EditorGUILayout.PropertyField(m_snapping);
					}

					EditorGUILayout.PropertyField(m_fadeOut);
					EditorGUILayout.PropertyField(m_randomnessMode);

					--EditorGUI.indentLevel;
				}
			}

			#endregion
		}

		#endregion
	}
}