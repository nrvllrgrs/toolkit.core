using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

namespace Tourbus.VisualScripting
{
	[UnitCategory("Time/UniTask")]
	public class WaitForSeconds : Unit
	{
		#region Ports

		[DoNotSerialize, PortLabelHidden]
		public ControlInput enter { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput exit { get; set; }

		[DoNotSerialize]
		public ControlOutput after { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ValueInput time { get; set; }

		#endregion

		#region Methods

		protected override void Definition()
		{
			enter = ControlInput(nameof(enter), Trigger);
			exit = ControlOutput(nameof(exit));
			Succession(enter, exit);

			after = ControlOutput(nameof(after));
			Succession(enter, after);

			time = ValueInput(nameof(time), 0f);
		}

		protected ControlOutput Trigger(Flow flow)
		{
			AsyncTrigger(
				flow.stack.AsReference(),
				flow.GetValue<float>(time));
			return exit;
		}

		private async void AsyncTrigger(GraphReference reference, float time)
		{
			await UniTask.WaitForSeconds(time);
			using (var flow = Flow.New(reference))
			{
				flow.Invoke(after);
			}
		}

		#endregion
	}
}