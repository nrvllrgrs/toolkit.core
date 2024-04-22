using System;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[UnitTitle("On Spawned"), UnitSurtitle("Object Spawner")]
	public class OnSpawnerSpawned : BaseSpawnerEventUnit
	{
		public override Type MessageListenerType => typeof(OnSpawnerSpawnedMessageListener);
	}
}
