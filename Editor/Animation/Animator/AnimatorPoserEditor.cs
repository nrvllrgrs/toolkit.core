using ToolkitEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace ToolkitEditor
{
	[CustomEditor(typeof(AnimatorPoser))]
    public class AnimatorPoserEditor : BaseToolkitEditor
    {
		#region Fields

		protected Animator m_animator;

		protected SerializedProperty m_stateName;
		protected SerializedProperty m_motion;
		protected SerializedProperty m_clip;

		#endregion

		#region Methods

		protected void OnEnable()
		{
			var poser = target as AnimatorPoser;

			m_animator = poser.GetComponent<Animator>();
			m_stateName = serializedObject.FindProperty(nameof(m_stateName));
			m_motion = serializedObject.FindProperty(nameof(m_motion));
			m_clip = serializedObject.FindProperty(nameof(m_clip));
		}

		protected override void DrawProperties()
		{
			base.DrawProperties();

			EditorGUI.BeginDisabledGroup(m_animator.runtimeAnimatorController == null || Application.isPlaying);
			{
				AnimatorController controller = null;
				if (!Application.isPlaying)
				{
					controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(
						AssetDatabase.GetAssetPath(m_animator.runtimeAnimatorController));
				}

				GenericMenu menu = new GenericMenu();

				if (controller != null)
				{
					foreach (var layer in controller.layers)
					{
						foreach (var animState in layer.stateMachine.states)
						{
							menu.AddItem(new GUIContent($"{layer.name}/{animState.state.name}"), false, MenuSelected, animState);
						}
					}
				}

				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PrefixLabel(m_stateName.displayName);

					if (EditorGUILayout.DropdownButton(new GUIContent(m_stateName.stringValue), FocusType.Keyboard))
					{
						menu.ShowAsContext();
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.PropertyField(m_clip);
		}

		private void MenuSelected(object animState)
		{
			m_stateName.stringValue = ((ChildAnimatorState)animState).state.name;
			m_motion.stringValue = ((ChildAnimatorState)animState).state.motion.name;
			serializedObject.ApplyModifiedProperties();
		}

		#endregion
	}
}