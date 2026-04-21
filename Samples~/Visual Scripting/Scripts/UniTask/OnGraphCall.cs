using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[UnitCategory("Events")]
	[UnitTitle("On Graph Call")]
	public class OnGraphCall : EventUnit<BridgeEventArgs>
	{
		#region Ports

		[UnitHeaderInspectable]
		public string key { get; set; }

		[DoNotSerialize]
		public ValueOutput bridge { get; set; }

		[DoNotSerialize]
		public ValueOutput payload { get; set; }

		#endregion

		#region Properties

		protected override bool register => true;

		#endregion

		#region Methods

		protected override void Definition()
		{
			base.Definition();

			bridge = ValueOutput<object>(nameof(bridge));
			payload = ValueOutput<object>(nameof(payload));
		}

		protected override void AssignArguments(Flow flow, BridgeEventArgs data)
		{
			flow.SetValue(bridge, data.bridge);
			flow.SetValue(payload, data.payload);
		}

		protected override bool ShouldTrigger(Flow flow, BridgeEventArgs args)
		{
			return Equals(args.key, key);
		}

		public override EventHook GetHook(GraphReference reference)
		{
			return new EventHook(GetType().Name, reference.self);
		}

		#endregion
	}
}