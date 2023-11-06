using UnityEngine;
using UnityEditor;
using ToolkitEngine;

namespace ToolkitEditor
{
    [CustomEditor(typeof(TimedCurve))]
    public class TimedCurveEditor : Editor
    {
        #region Fields

        protected TimedCurve m_timedCurve;

        protected SerializedProperty m_curve;
        protected SerializedProperty m_duration;
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

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_curve);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_duration);

            // Add check in editor to update tweener time scale
            if (EditorGUI.EndChangeCheck())
            {
                m_timedCurve.duration = m_duration.floatValue;
            }

            EditorGUILayout.PropertyField(m_amplitude);
            EditorGUILayout.PropertyField(m_verticalShift);
            EditorGUILayout.PropertyField(m_playOnAwake);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(m_time);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Separator();

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

            if (EditorGUILayoutUtility.Foldout(m_onPlayed, "Events"))
            {
                EditorGUILayout.PropertyField(m_onPlayed);
                EditorGUILayout.PropertyField(m_onPaused);
                EditorGUILayout.PropertyField(m_onTimeChanged);
                EditorGUILayout.PropertyField(m_onBeginCompleted);
                EditorGUILayout.PropertyField(m_onEndCompleted);
                EditorGUILayout.PropertyField(m_onValueChanged);
            }

            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}