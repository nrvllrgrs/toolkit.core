using System;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[UnitTitle("On Spawning"), UnitSurtitle("Object Spawner")]
	public class OnObjectSpawnerSpawning : BaseSpawnerEventUnit
	{
		public override Type MessageListenerType => typeof(OnSpawnerSpawningMessageListener);
	}
}
