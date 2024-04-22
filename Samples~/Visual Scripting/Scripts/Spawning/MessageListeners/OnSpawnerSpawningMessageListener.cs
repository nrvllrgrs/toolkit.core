using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[AddComponentMenu("")]
	public class OnSpawnerSpawningMessageListener : MessageListener
	{
		private void Start() => GetComponent<ObjectSpawner>()?.onSpawning.AddListener((value) =>
		{
			EventBus.Trigger(EventHooks.OnSpawnerSpawning, gameObject, value);
		});
	}
}