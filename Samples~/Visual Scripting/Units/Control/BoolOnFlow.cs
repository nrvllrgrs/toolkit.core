using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[UnitCategory("Control")]
    public class BoolOnFlow : Unit
    {
		#region Fields

		[DoNotSerialize]
		public ControlInput trueTrigger { get; private set; }

		[DoNotSerialize]
		public ControlInput falseTrigger { get; private set; }

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput outputTrigger { get; private set; }

		[DoNotSerialize, PortLabelHidden]
		public ValueOutput value;

		private bool m_trueFlow;

		#endregion

		#region Methods

		protected override void Definition()
		{
			trueTrigger = ControlInput("True", TriggerTrue);
			falseTrigger = ControlInput("False", TriggerFalse);

			value = ValueOutput(nameof(value), (x) => m_trueFlow);

			outputTrigger = ControlOutput(nameof(outputTrigger));
			Succession(trueTrigger, outputTrigger);
			Succession(falseTrigger, outputTrigger);
		}

		private ControlOutput TriggerTrue(Flow flow)
		{
			m_trueFlow = true;
			return outputTrigger;
		}

		private ControlOutput TriggerFalse(Flow flow)
		{
			m_trueFlow = false;
			return outputTrigger;
		}

		#endregion
	}
}