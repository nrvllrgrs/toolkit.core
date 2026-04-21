using UnityEngine;
using Unity.VisualScripting;
using ToolkitEngine.InputSystem;

namespace ToolkitEngine.VisualScripting
{
	[AddComponentMenu("")]
	public class OnInputPerformedMessageListener : MessageListener
	{
		private void Start() => GetComponent<InputEvents>()?.onPerformed.AddListener(() =>
		{
			EventBus.Trigger(nameof(OnInputPerformed), gameObject);
		});
	}
}