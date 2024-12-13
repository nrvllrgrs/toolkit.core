using UnityEngine;

namespace ToolkitEngine
{
	[RequireComponent(typeof(AnimatorStack))]
    public class AnimatorPoser : MonoBehaviour
    {
		#region Fields

		[SerializeField]
		private string m_stateName;

		[SerializeField]
		private string m_motion;

		[SerializeField]
		private AnimationClip m_clip;

		private AnimatorStack m_animatorStack;
		private AnimatorOverrideController m_overrideController;
		private RuntimeAnimatorController m_controller;

		#endregion

		#region Methods

		private void Awake()
		{
			m_animatorStack = GetComponent<AnimatorStack>();

			m_overrideController = new AnimatorOverrideController();
			m_overrideController.name = $"{m_clip.name} Override";
			m_overrideController.runtimeAnimatorController = m_animatorStack.defaultController;
			m_overrideController[m_motion] = m_clip;
		}

		private void OnEnable()
		{
			m_animatorStack.Push(m_overrideController);
			m_animatorStack.animator.Play(m_stateName);
		}

		private void OnDisable()
		{
			m_animatorStack.Remove(m_overrideController);
		}

		#endregion
	}
}