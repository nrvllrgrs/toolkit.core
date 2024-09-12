using System;
using UnityEngine;

namespace ToolkitEngine.VisualScripting
{
	[Serializable]
    public abstract class BaseMachineStateItem<T> : IMachineState<T>
		where T : struct, IConvertible
	{
		#region Fields

		/// <summary>
		/// Indicates whether state is enabled.
		/// </summary>
		[SerializeField, Tooltip("Indicates whether state is enabled.")]
		protected bool m_enabled = true;

		/// <summary>
		/// Name of state
		/// </summary>
		[SerializeField, Tooltip("Name of state.")]
		protected string m_name;

		[SerializeField]
		protected MachineStateEvent<T> m_events = new MachineStateEvent<T>();

		#endregion

		#region Properties

		/// <summary>
		/// Indicates whether state is enabled.
		/// </summary>
		public bool enabled => m_enabled;

		/// <summary>
		/// Name of state -- used as key in visual scripting
		/// </summary>
		public string name => m_name;

		public T value { get; private set; }

		#endregion

		#region Methods

		protected void SetValue(T value, Action action)
		{
			this.value = value;
			m_events.Evaluate(value, IMachineState<T>.GetEventName(value), action);
		}

		public abstract void Continue();

		#endregion
	}

	public interface IMachineState<T>
		where T : struct, IConvertible
	{
		string name { get; }
		T value { get; }
		void Continue();

		public static string GetEventName(T state)
		{
			// Include '$' and type.Name prefix to filter from identical state names
			return $"${state.GetType().Name}.{state.ToString()}";
		}
	}
}