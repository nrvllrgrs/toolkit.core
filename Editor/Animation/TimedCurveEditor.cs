using UnityEngine;
using UnityEditor;
using ToolkitEngine;

namespace ToolkitEditor
{
    [CustomEditor(typeof(TimedCurve))]
    public class TimedCurveEditor : BaseToolkitEditor
    {
        #region Fields

        protected TimedCurve m_timedCurve;

        protected SerializedProperty m_curve;
        protected SerializedProperty m_duration;
        protected SerializedProperty m_durationFactor;
        protected SerializedProperty m_amplitude;
        protected SerializedProperty m_verticalShift;
        protected SerializedProperty m_playOnAwake;
        protected SerializedProperty m_time;

        // Events
        protected SerializedProperty m_onPlayed;
        protected SerializedProperty m_onPaused;
        protected SerializedProperty m_onTimeChanged;
        protected SerializedProperty m_onBeginCompleted;
        protected SerializedProperty m_onEndCompleted;
        protected SerializedProperty m_onValueChanged;

        #endregion

        #region Methods

        private void OnEnable()
        {
            m_timedCurve = (TimedCurve)target;

            m_curve = serializedObject.FindProperty(nameof(m_curve));
            m_duration = serializedObject.FindProperty(nameof(m_duration));
            m_durationFactor = serializedObject.FindProperty(nameof(m_durationFactor));
            m_amplitude = serializedObject.FindProperty(nameof(m_amplitude));
            m_verticalShift = serializedObject.FindProperty(nameof(m_verticalShift));
            m_playOnAwake = serializedObject.FindProperty(nameof(m_playOnAwake));
            m_time = serializedObject.FindProperty(nameof(m_time));

            // Events
            m_onPlayed = serializedObject.FindProperty("OnPlayed");
            m_onPaused = serializedObject.FindProperty("OnPaused");
            m_onTimeChanged = serializedObject.FindProperty("OnTimeChanged");
            m_onBeginCompleted = serializedObject.FindProperty("OnBeginCompleted");
            m_onEndCompleted = serializedObject.FindProperty("OnEndCompleted");
            m_onValueChanged = serializedObject.FindProperty("OnValueChanged");
        }

		protected override void DrawProperties()
		{
			EditorGUILayout.PropertyField(m_curve);

			EditorGUI.BeginChangeCheck();
			{
				EditorGUILayout.PropertyField(m_duration);
				EditorGUILayout.PropertyField(m_durationFactor);
			}
			// Add check in editor to update tweener time scale
			if (EditorGUI.EndChangeCheck() && Application.isPlaying)
			{
				m_timedCurve.duration = m_duration.floatValue;
			}

			EditorGUI.BeginDisabledGroup(true);
			{
				EditorGUILayout.PropertyField(m_time);
			}
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.PropertyField(m_playOnAwake);

			EditorGUILayout.Separator();

			EditorGUILayout.PropertyField(m_amplitude);
			EditorGUILayout.PropertyField(m_verticalShift);

			EditorGUILayout.Separator();
			DrawCommands();
		}

		protected void DrawCommands()
		{
			if (EditorGUILayoutUtility.Foldout(m_curve, "Commands"))
			{
				EditorGUI.BeginDisabledGroup(!Application.isPlaying);

				if (!m_timedCurve.isPlaying)
				{
					if (GUILayout.Button("Play"))
					{
						m_timedCurve.Play();
					}
				}
				else if (GUILayout.Button("Pause"))
				{
					m_timedCurve.Pause();
				}

				if (GUILayout.Button("Play Forward"))
				{
					m_timedCurve.PlayForward();
				}

				if (GUILayout.Button("Play Backwards"))
				{
					m_timedCurve.PlayBackwards();
				}

				if (GUILayout.Button("Reverse"))
				{
					m_timedCurve.Reverse();
				}

				if (GUILayout.Button("Restart"))
				{
					m_timedCurve.Restart();
				}

				if (GUILayout.Button("Flip"))
				{
					m_timedCurve.Flip();
				}

				EditorGUI.EndDisabledGroup();
			}
		}

		protected override void DrawEvents()
		{
			if (EditorGUILayoutUtility.Foldout(m_onPlayed, "Events"))
			{
				EditorGUILayout.PropertyField(m_onPlayed);
				EditorGUILayout.PropertyField(m_onPaused);
				EditorGUILayout.PropertyField(m_onTimeChanged);
				EditorGUILayout.PropertyField(m_onBeginCompleted);
				EditorGUILayout.PropertyField(m_onEndCompleted);
				EditorGUILayout.PropertyField(m_onValueChanged);
			}
		}

        #endregion
    }
}