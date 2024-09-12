using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[Serializable]
	public sealed class MachineStateEvent<T>
		where T : struct, IConvertible
	{
		#region Fields

		[SerializeField]
		private List<Argument> m_arguments = new();

		#endregion

		#region Properties

		public bool isEmpty => m_arguments.Count == 0;

		#endregion

		#region Constructors

		public MachineStateEvent()
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type.");
			}
		}

		#endregion

		#region Methods

		public bool Evaluate(T value, string eventName, Action action)
		{
			if (isEmpty)
				return false;

			bool triggered = false;
			foreach (var arg in m_arguments)
			{
				triggered |= arg.Evaluate(value, eventName);
			}

			if (!triggered)
			{
				action?.Invoke();
			}
			return triggered;
		}

		#endregion

		#region Structures

		[Serializable]
		internal class Argument
		{
			#region Fields

			[SerializeField]
			private T m_state;

			[SerializeField]
			private ScriptMachine m_scriptMachine;

			#endregion

			#region Methods

			internal bool Evaluate(T value, string eventName)
			{
				if (!Equals(m_state, value))
					return false;

				if (m_scriptMachine != null)
				{
					m_scriptMachine.gameObject.SetActive(true);
					EventBus.Trigger(eventName, m_scriptMachine.gameObject, this);
					return true;
				}
				return false;
			}

			#endregion
		}

		#endregion
	}
}