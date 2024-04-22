using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[AddComponentMenu("")]
	public class OnSpawnerDespawnedMessageListener : MessageListener
	{
		private void Start() => GetComponent<ObjectSpawner>()?.onDespawned.AddListener((value) =>
		{
			EventBus.Trigger(EventHooks.OnSpawnerDespawned, gameObject, value);
		});
	}
}