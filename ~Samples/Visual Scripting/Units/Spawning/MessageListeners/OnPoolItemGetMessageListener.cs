using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[AddComponentMenu("")]
	public class OnPoolItemGetMessageListener : MessageListener
	{
		private void Start() => GetComponent<PoolItem>()?.OnGet.AddListener((value) =>
		{
			EventBus.Trigger(EventHooks.OnPoolItemGet, gameObject, value);
		});
	}
}