using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[UnitCategory("Math/Scalar")]
    public class Negate : Unit
    {
		#region Ports

		[DoNotSerialize, PortLabel("X")]
		public ValueInput value { get; set; }

		[DoNotSerialize, PortLabel("~X")]
		public ValueOutput result { get; set; }

		#endregion

		#region Methods

		protected override void Definition()
		{
			value = ValueInput<float>(nameof(value));
			result = ValueOutput(nameof(result), (flow) =>
			{
				return -flow.GetValue<float>(value);
			});
		}

		#endregion
	}
}