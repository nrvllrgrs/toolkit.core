using UnityEngine;
using Unity.VisualScripting;
using ToolkitEngine.InputSystem;

namespace ToolkitEngine.VisualScripting
{
	[AddComponentMenu("")]
	public class OnInputCanceledMessageListener : MessageListener
	{
		private void Start() => GetComponent<InputEvents>()?.onCanceled.AddListener(() =>
		{
			EventBus.Trigger(nameof(OnInputCanceled), gameObject);
		});
	}
}