using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine
{
    public class CounterEvents : MonoBehaviour
    {
		#region Fields

		[SerializeField, Min(1)]
		private int m_count = 1;

		[SerializeField]
		private bool m_resetOnCount = true;

		private int m_value;

		#endregion

		#region Events

		[SerializeField, Foldout("Events")]
		private UnityEvent m_onCount;

		[SerializeField, Foldout("Events")]
		private UnityEvent m_onAfter;

		#endregion

		#region Properties

		public int value => m_value;
		public UnityEvent onCount => m_onCount;
		public UnityEvent onAfter => m_onAfter;

		#endregion

		#region Methods

		public void Increment()
		{
			++m_value;

			if (m_value == m_count)
			{
				m_onCount?.Invoke();
				if (m_resetOnCount)
				{
					ResetValue();
				}
			}
			else if (m_value >= m_count)
			{
				m_onAfter?.Invoke();
			}
		}

		public void ResetValue()
		{
			m_value = 0;
		}

		#endregion
	}
}