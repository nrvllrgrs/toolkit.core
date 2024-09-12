using System;
using UnityEngine;

namespace ToolkitEngine.VisualScripting
{
	public abstract class BaseMachineStateBehaviour<T> : MonoBehaviour, IMachineState<T>
		where T : struct, IConvertible
	{
		#region Fields

		[SerializeField]
		private MachineStateEvent<T> m_events;

		#endregion

		#region Properties

		public T value { get; private set; }

		#endregion

		#region Methods

		public abstract void Continue();

		protected void SetValue(T value, Action action)
		{
			this.value = value;
			m_events.Evaluate(value, IMachineState<T>.GetEventName(value), action);
		}

		#endregion
	}
}