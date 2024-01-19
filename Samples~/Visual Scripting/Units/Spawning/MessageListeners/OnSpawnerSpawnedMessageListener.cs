using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[AddComponentMenu("")]
	public class OnSpawnerSpawnedMessageListener : MessageListener
	{
		private void Start() => GetComponent<ObjectSpawner>()?.onSpawned.AddListener((value) =>
		{
			EventBus.Trigger(EventHooks.OnSpawnerSpawned, gameObject, value);
		});
	}
}