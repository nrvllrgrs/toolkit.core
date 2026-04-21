using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[UnitCategory("Collections/Lists")]
	public class Except : Unit
	{
		#region Ports

		[Inspectable, UnitHeaderInspectable("Type")]
		public Type type { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ControlInput enter { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput exit { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ValueInput a { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ValueInput b { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ValueOutput except { get; set; }

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

			var outputType = type != null
				? typeof(List<>).MakeGenericType(type)
				: typeof(List<object>);

			except = ValueOutput(outputType, nameof(except), (flow) =>
			{
				var value = flow.GetValue<List<object>>(a)
					.Except(flow.GetValue<List<object>>(b));

				if (type != null)
				{
					var list = (IList)Activator.CreateInstance(outputType);
					foreach (var item in value)
					{
						list.Add(item);
					}
					return list;
				}

				return value.ToList();
			});
		}

		#endregion
	}
}