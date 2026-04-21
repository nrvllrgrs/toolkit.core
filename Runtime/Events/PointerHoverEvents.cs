using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ToolkitEngine
{
	[AddComponentMenu("Events/Pointer Hover Events")]
	public class PointerHoverEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
		#region Fields

		private bool m_isHovering = false;

		#endregion

		#region Properties

		public bool isHovering
		{
			get => m_isHovering;
			set
			{
				// No change, skip
				if (m_isHovering == value)
					return;

				m_isHovering = value;

				if (value)
				{
					m_onPointerEnter?.Invoke();
				}
				else
				{
					m_onPointerExit?.Invoke();
				}
			}
		}

		#endregion

		#region Events

		[SerializeField, Foldout("Events")]
		private UnityEvent m_onPointerEnter;

		[SerializeField, Foldout("Events")]
		private UnityEvent m_onPointerExit;

		#endregion

		#region Methods

		private void OnDisable()
		{
			isHovering = false;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			isHovering = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			isHovering = false;
		}

		#endregion
	}
}