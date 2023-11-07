using System;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[UnitTitle("On Released"), UnitSurtitle("Pool Item")]
	public class OnPoolItemReleased : BasePoolItemEventUnit
	{
		public override Type MessageListenerType => typeof(OnPoolItemReleasedMessageListener);
	}
}
