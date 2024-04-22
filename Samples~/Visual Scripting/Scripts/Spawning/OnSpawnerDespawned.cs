using System;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[UnitTitle("On Despawned"), UnitSurtitle("Object Spawner")]
	public class OnObjectSpawnerDespawned : BaseSpawnerEventUnit
	{
		public override Type MessageListenerType => typeof(OnSpawnerDespawnedMessageListener);
	}
}
