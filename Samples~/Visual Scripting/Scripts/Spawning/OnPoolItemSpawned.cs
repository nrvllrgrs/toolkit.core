using System;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[UnitTitle("On Spawned"), UnitSurtitle("Pool Item")]
	public class OnPoolItemSpawned : BasePoolItemEventUnit
	{
		public override Type MessageListenerType => typeof(OnPoolItemSpawnedMessageListener);
	}
}
