using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	public abstract class FilteredManagerEventUnit<TArgs, TFilter> : ManagerEventUnit<TArgs>
	{
		#region Fields

		[UnitHeaderInspectable("Filtered")]
		public bool filtered;

		[DoNotSerialize]
		public ValueInput filter { get; set; }

		#endregion

		#region Methods

		protected override void Definition()
		{
			base.Definition();

			if (filtered)
			{
				filter = ValueInput(nameof(filter), default(TFilter));
			}
		}

		protected override bool ShouldTrigger(Flow flow, TArgs args)
		{
			if (!filtered)
				return true;

			return Equals(flow.GetValue<TFilter>(filter), GetFilterValue(args));
		}

		protected abstract TFilter GetFilterValue(TArgs args);

		#endregion
	}
}