using System;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	public abstract class BaseMachineStateContinueUnit<T> : Unit
		where T : struct, IConvertible
	{
		#region Fields

		[DoNotSerialize, PortLabelHidden]
		public ControlInput enter;

		[DoNotSerialize]
		public ValueInput state;

		#endregion

		#region Methods

		protected override void Definition()
		{
			enter = ControlInput(nameof(enter), Trigger);
			state = ValueInput<IMachineState<T>>(nameof(state), null);
			Requirement(state, enter);
		}

		private ControlOutput Trigger(Flow flow)
		{
			flow.GetValue<IMachineState<T>>(state).Continue();

			// Turn off gameObject automatically after use for performance
			flow.stack.gameObject.SetActive(false);
			return null;
		}

		#endregion
	}
}