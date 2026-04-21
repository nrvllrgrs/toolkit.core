using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ToolkitEngine
{
	[AddComponentMenu("Events/Pointer Action Events")]
	public class PointerActionEvents : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
	{
		#region Fields

		private bool m_isPressed = false;

		#endregion

		#region Properties

		public bool isPressed
		{
			get => m_isPressed;
			set
			{
				// No change, skip
				if (m_isPressed == value)
					return;

				m_isPressed = value;

				if (value)
				{
					m_onPointerDown?.Invoke();
				}
				else
				{
					m_onPointerUp?.Invoke();
				}
			}
		}

		#endregion

		#region Events

		[SerializeField, Foldout("Events")]
		private UnityEvent m_onPointerClick;

		[SerializeField, Foldout("Events")]
		private UnityEvent m_onPointerDown;

		[SerializeField, Foldout("Events")]
		private UnityEvent m_onPointerUp;

		#endregion

		#region Methods

		private void OnDisable()
		{
			isPressed = false;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			m_onPointerClick?.Invoke();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			isPressed = true;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			isPressed = false;
		}

		#endregion
	}
}