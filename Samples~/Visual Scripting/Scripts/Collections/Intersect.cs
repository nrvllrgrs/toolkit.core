using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[UnitCategory("Collections/Lists")]
    public class Intersect : Unit
    {
		#region Ports

		[DoNotSerialize, PortLabelHidden]
		public ControlInput enter { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput exit { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ValueInput a { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ValueInput b { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ValueOutput intersect { get; set; }

		#endregion

		#region Methods

		protected override void Definition()
		{
			enter = ControlInput(nameof(enter), (flow) => exit);
			exit = ControlOutput(nameof(exit));
			Succession(enter, exit);

			a = ValueInput<List<object>>(nameof(a));
			b = ValueInput<List<object>>(nameof(b));
			Requirement(a, enter);
			Requirement(b, enter);

			intersect = ValueOutput<List<object>>(nameof(intersect), (flow) =>
			{
				var value = flow.GetValue<List<object>>(a)
					.Intersect(flow.GetValue<List<object>>(b)).ToList();

				flow.SetValue(intersect, value);
				return value;
			});
		}

		#endregion
	}
}