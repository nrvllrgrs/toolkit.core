using System;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	public abstract class BaseMachineStateItemEventUnit<T> : EventUnit<BaseMachineStateItem<T>>
		where T : struct, IConvertible
	{
		#region Fields

		[PortLabelHidden, UnitHeaderInspectable]
		public T value;

		[DoNotSerialize, PortLabelHidden]
		public ValueInput key;

		[DoNotSerialize, PortLabelHidden]
		public ValueOutput state;

		private IMachineState<T> m_cachedState;

		#endregion

		#region Properties

		protected override bool register => true;

		#endregion

		#region Methods

		protected override void Definition()
		{
			base.Definition();

			key = ValueInput(nameof(key), string.Empty);
			state = ValueOutput(nameof(state), (flow) => m_cachedState);
		}

		protected override void AssignArguments(Flow flow, BaseMachineStateItem<T> args)
		{
			m_cachedState = args;
		}

		protected override bool ShouldTrigger(Flow flow, BaseMachineStateItem<T> args)
		{
			return Equals(flow.GetValue<string>(key), args.name);
		}

		public override EventHook GetHook(GraphReference reference)
		{
			return new EventHook(IMachineState<T>.GetEventName(value), reference.self);
		}

		#endregion
	}
}