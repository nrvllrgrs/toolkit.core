using System;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	public abstract class BaseMachineStateBehaviourEventUnit<T> : EventUnit<BaseMachineStateBehaviour<T>>
		where T : struct, IConvertible
    {
		#region Fields

		[PortLabelHidden, UnitHeaderInspectable]
		public T value;

		[DoNotSerialize, PortLabelHidden]
		public ValueInput target;

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

			target = ValueInput<BaseMachineStateBehaviour<T>>(nameof(target), null);
			state = ValueOutput(nameof(state), (flow) => m_cachedState);
		}

		protected override void AssignArguments(Flow flow, BaseMachineStateBehaviour<T> args)
		{
			m_cachedState = args;
		}

		protected override bool ShouldTrigger(Flow flow, BaseMachineStateBehaviour<T> args)
		{
			return Equals(flow.GetValue<BaseMachineStateBehaviour<T>>(target), args);
		}

		public override EventHook GetHook(GraphReference reference)
		{
			return new EventHook(IMachineState<T>.GetEventName(value), reference.self);
		}

		#endregion
	}
}