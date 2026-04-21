using Unity.VisualScripting;
using System.Collections.Generic;
using System.Collections;

namespace ToolkitEngine.VisualScripting
{
	[UnitCategory("Collections/Lists")]
	public class IndexOf : Unit
    {
		#region Ports

		[DoNotSerialize, PortLabelHidden]
		public ControlInput enter { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput exit { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ValueInput list { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ValueInput item { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ValueOutput index { get; set; }

		#endregion

		#region Methods

		protected override void Definition()
		{
			enter = ControlInput(nameof(enter), (flow) => exit);
			exit = ControlOutput(nameof(exit));
			Succession(enter, exit);

			list = ValueInput<IList<object>>(nameof(list));
			item = ValueInput<object>(nameof(item));
			Requirement(list, enter);

			index = ValueOutput(nameof(index), (flow) =>
			{
				var collection = flow.GetValue<IList>(list);
				var target = flow.GetValue<object>(item);

				return collection != null
					? collection.IndexOf(target)
					: -1;
			});
		}

		#endregion
	}
}