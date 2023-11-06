using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[AddComponentMenu("")]
	public class OnPoolItemReleasedMessageListener : MessageListener
	{
		private void Start() => GetComponent<PoolItem>()?.OnGet.AddListener((value) =>
		{
			EventBus.Trigger(EventHooks.OnPoolItemReleased, gameObject, value);
		});
	}
}