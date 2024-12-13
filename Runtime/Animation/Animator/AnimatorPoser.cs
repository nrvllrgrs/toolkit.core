using UnityEngine;

namespace ToolkitEngine
{
	[RequireComponent(typeof(Animator))]
    public class AnimatorPoser : MonoBehaviour
    {
		#region Fields

		[SerializeField]
		private string m_stateName;

		[SerializeField]
		private string m_motion;

		[SerializeField]
		private AnimationClip m_clip;

		private Animator m_animator;
		private AnimatorOverrideController m_overrideController;
		private RuntimeAnimatorController m_controller;

		#endregion

		#region Methods

		private void Awake()
		{
			m_animator = GetComponent<Animator>();
			m_controller = m_animator.runtimeAnimatorController;

			m_overrideController = new AnimatorOverrideController();
			m_overrideController.name = $"{m_clip.name} Override";

			m_overrideController.runtimeAnimatorController = m_animator.runtimeAnimatorController;
			m_overrideController[m_motion] = m_clip;
		}

		private void OnEnable()
		{
			m_animator.runtimeAnimatorController = m_overrideController;
			m_animator.Play(m_stateName);
		}

		private void OnDisable()
		{
			m_animator.runtimeAnimatorController = m_controller;
		}

		#endregion
	}
}