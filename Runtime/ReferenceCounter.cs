using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine
{
    public class ReferenceCounter : MonoBehaviour
    {
		#region Fields

		private int m_count = 0;

		#endregion

		#region Events

		[SerializeField, Foldout("Events")]
		[Tooltip("Invoked when count is greater than 0.")]
		private UnityEvent m_onThresholdEntered;

		[SerializeField, Foldout("Events")]
		[Tooltip("Invoked when count is equals 0.")]
		private UnityEvent m_onThresholdExited;

		#endregion

		#region Properties

		public int count
		{
			get => m_count;
			private set
			{
				value = Mathf.Max(value, 0);

				// No change, skip
				if (m_count == value)
					return;

				bool wasZero = m_count == 0;
				m_count = value;

				if (m_count > 0)
				{
					if (wasZero)
					{
						m_onThresholdEntered?.Invoke();
					}
				}
				else
				{
					m_onThresholdExited?.Invoke();
				}
			}
		}

		#endregion

		#region Methods

		public void Increment() => ++count;
		public void Decrement() => --count;

		#endregion
	}
}