using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[UnitCategory("Events/Spawning/Pool Item")]
	public abstract class BasePoolItemEventUnit : BaseEventUnit<PoolItem>
	{
		#region Properties

		protected override bool showEventArgs => false;

		#endregion
	}
}