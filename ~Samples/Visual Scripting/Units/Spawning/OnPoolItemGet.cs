using System;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[UnitTitle("On Get"), UnitSurtitle("Pool Item")]
	public class OnPoolItemGet : BasePoolItemEventUnit
	{
		public override Type MessageListenerType => typeof(OnPoolItemGetMessageListener);
	}
}
