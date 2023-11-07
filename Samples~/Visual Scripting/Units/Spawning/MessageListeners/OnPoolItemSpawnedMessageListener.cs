using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[AddComponentMenu("")]
	public class OnPoolItemSpawnedMessageListener : MessageListener
	{
		private void Start() => GetComponent<PoolItem>()?.OnGet.AddListener((value) =>
		{
			EventBus.Trigger(EventHooks.OnPoolItemSpawned, gameObject, value);
		});
	}
}