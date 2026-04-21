using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ToolkitEngine.InputSystem
{
	public class InputEvents : MonoBehaviour
    {
		#region Fields

		[SerializeField]
		private InputActionProperty m_action;

		[SerializeField]
		private UnityCondition m_predicate = new UnityCondition(UnityCondition.ConditionType.All);

		protected bool m_performing;

		private static Dictionary<InputAction, uint> s_actionRefCount = new();

		#endregion

		#region Events

		[SerializeField, Foldout("Events")]
		private UnityEvent m_onPerformed;

		[SerializeField, Foldout("Events")]
		private UnityEvent m_onCanceled;

		#endregion

		#region Properties

		public bool canPerform => m_predicate.isTrueAndEnabled;

		public bool performing => m_performing;

		public UnityEvent onPerformed => m_onPerformed;
		public UnityEvent onCanceled => m_onCanceled;

		#endregion

		#region Methods

		protected virtual void OnEnable()
		{
			Register(m_action, Performed, Canceled);
		}

		protected virtual void OnDisable()
		{
			Unregister(m_action, Performed, Canceled);
		}

		protected virtual void Performed(InputAction.CallbackContext obj)
		{
			if (!canPerform)
				return;

			SetPerforming(true, obj);
		}

		protected virtual void Canceled(InputAction.CallbackContext obj)
		{
			SetPerforming(false, obj);
		}

		protected void SetPerforming(bool value, InputAction.CallbackContext obj)
		{
			// No change, skip
			if (m_performing == value)
				return;

			m_performing = value;

			if (value)
			{
				m_onPerformed?.Invoke();
			}
			else
			{
				m_onCanceled?.Invoke();
			}
		}

		#endregion

		#region Editor-Only
#if UNITY_EDITOR

		[ContextMenu("Perform - Click")]
		protected void EditorPerformClick()
		{
			SetPerforming(true, default);
			SetPerforming(false, default);
		}

		[ContextMenu("Perform - Press")]
		protected void EditorPerformPress()
		{
			SetPerforming(true, default);
		}

		[ContextMenu("Cancel")]
		protected void EditorCanel()
		{
			SetPerforming(false, default);
		}

#endif
		#endregion

		#region Static Methods

		public static void Register(InputActionProperty inputAction, Action<InputAction.CallbackContext> onPerformed, Action<InputAction.CallbackContext> onCanceled)
		{
			if (inputAction == null || inputAction.action?.actionMap?.asset == null)
				return;

			InputAction action = inputAction.action;
			if (!s_actionRefCount.TryGetValue(action, out uint count))
			{
				s_actionRefCount.Add(action, 0);
				action.Enable();
			}
			++s_actionRefCount[action];

			action.performed += onPerformed;
			action.canceled += onCanceled;
		}

		public static void Unregister(InputActionProperty inputAction, Action<InputAction.CallbackContext> onPerformed, Action<InputAction.CallbackContext> onCanceled)
		{
			if (inputAction == null || inputAction.action == null)
				return;

			InputAction action = inputAction.action;
			if (s_actionRefCount.TryGetValue(action, out uint count))
			{
				if (--count == 0)
				{
					s_actionRefCount.Remove(action);
					action.Disable();
				}
				else
				{
					s_actionRefCount[action] = count;
				}
			}

			action.performed -= onPerformed;
			action.canceled -= onCanceled;
		}

		#endregion
	}
}