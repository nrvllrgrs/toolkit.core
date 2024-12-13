using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine
{
	[RequireComponent(typeof(Animator))]
    public class AnimatorStack : MonoBehaviour
    {
		#region Fields

		private Animator m_animator;
		private RuntimeAnimatorController m_controller;

		private List<RuntimeAnimatorController> m_stack = new();

		#endregion

		#region Properties

		public Animator animator
		{
			get
			{
				if (m_animator == null)
				{
					m_animator = GetComponent<Animator>();
				}
				return m_animator;
			}
		}

		public RuntimeAnimatorController defaultController
		{
			get
			{
				if (m_controller == null)
				{
					m_animator = GetComponent<Animator>();
					m_controller = animator.runtimeAnimatorController;
				}
				return m_controller;
			}
		}

		#endregion

		#region Methods

		private void Awake()
		{
			m_animator ??= GetComponent<Animator>();
			m_controller ??= m_animator.runtimeAnimatorController;
		}

		#endregion

		#region Stack Methods

		public RuntimeAnimatorController Peek() => m_stack[m_stack.Count - 1];

		public bool TryPeek(out RuntimeAnimatorController result)
		{
			if (m_stack.Count == 0)
			{
				result = null;
				return false;
			}

			result = Peek();
			return true;
		}

		public void Push(RuntimeAnimatorController item)
		{
			m_stack.Add(item);
			m_animator.runtimeAnimatorController = item;
		}

		public RuntimeAnimatorController Pop()
		{
			var result = Peek();
			m_stack.RemoveAt(m_stack.Count - 1);
			CheckEmpty();

			return result;
		}

		public bool TryPop(out RuntimeAnimatorController result)
		{
			if (m_stack.Count == 0)
			{
				result = null;
				return false;
			}

			result = Pop();
			return true;
		}

		public void Remove(RuntimeAnimatorController item)
		{
			m_stack.Remove(item);
			CheckEmpty();
		}

		private void CheckEmpty()
		{
			if (m_stack.Count == 0)
			{
				m_animator.runtimeAnimatorController = m_controller;
			}
		}

		public void Clear()
		{
			m_stack.Clear();
			m_animator.runtimeAnimatorController = m_controller;
		}

		#endregion
	}
}