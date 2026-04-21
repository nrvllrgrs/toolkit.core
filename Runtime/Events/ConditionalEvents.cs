using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine
{
    public class ConditionalEvents : MonoBehaviour
    {
		#region Fields

		[SerializeField]
		protected UnityCondition m_condition = new();

		#endregion

		#region Events

		[SerializeField, Foldout("Events")]
		protected UnityEvent m_onTrue;

		[SerializeField, Foldout("Events")]
		protected UnityEvent m_onFalse;

		#endregion

		#region Properties

		public UnityEvent onTrue => m_onTrue;
		public UnityEvent onFalse => m_onFalse;

		#endregion

		#region Methods

		public void Evaluate()
		{
			if (m_condition.isTrueAndEnabled)
			{
				m_onTrue?.Invoke();
			}
			else
			{
				m_onFalse?.Invoke();
			}
		}

		#endregion
	}
}