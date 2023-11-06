using UnityEngine;
using UnityEditor;
using ToolkitEngine;

namespace ToolkitEditor
{
	[CustomEditor(typeof(FacingEvents))]
	public class FacingEventsEditor : BaseToolkitEditor
    {
		#region Fields

		protected SerializedProperty m_target;
		protected SerializedProperty m_axis;
		protected SerializedProperty m_direction;
		protected SerializedProperty m_space;

		protected SerializedProperty m_onFacingChanged;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_target = serializedObject.FindProperty(nameof(m_target));
			m_axis = serializedObject.FindProperty(nameof(m_axis));
			m_direction = serializedObject.FindProperty(nameof(m_direction));
			m_space = serializedObject.FindProperty(nameof(m_space));

			m_onFacingChanged = serializedObject.FindProperty(nameof(m_onFacingChanged));
		}

		protected override void DrawProperties()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(m_target);

			bool targetingSelf = m_target.objectReferenceValue == null || Equals(m_target.objectReferenceValue as Transform, (target as FacingEvents).transform);
			if (EditorGUI.EndChangeCheck() && targetingSelf)
			{
				m_space.intValue = (int)Space.World;
			}

			EditorGUILayout.PropertyField(m_axis);
			EditorGUILayout.PropertyField(m_direction);

			EditorGUI.BeginDisabledGroup(targetingSelf);
			EditorGUILayout.PropertyField(m_space);
			EditorGUI.EndDisabledGroup();
		}

		protected override void DrawEvents()
		{
			if (EditorGUILayoutUtility.Foldout(m_onFacingChanged, "Events"))
			{
				EditorGUILayout.PropertyField(m_onFacingChanged);
			}
		}

		#endregion
	}
}