using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
#if USE_UNITY_LOCALIZATION

	[UnitCategory("Localization")]
	public class GetLocalizedString : Unit
    {
		#region Fields

		[Inspectable]
		public SerializableLocalizedString localizedString;

		#endregion

		#region Ports

		[DoNotSerialize, PortLabelHidden]
		public ControlInput enter { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput exit { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ValueOutput value { get; set; }

		#endregion

		#region Methods

		protected override void Definition()
		{
			enter = ControlInput(nameof(enter), (flow) => exit);
			exit = ControlOutput(nameof(exit));
			Succession(enter, exit);

			value = ValueOutput(nameof(value), (flow) =>
			{
				return !localizedString.IsEmpty
					? localizedString.GetLocalizedString()
					: string.Empty;
			});
		}

		#endregion
	}

#endif
}