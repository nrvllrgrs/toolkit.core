using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
    [AddComponentMenu("")]
    public class OnSetItemAddedMessageListener : MessageListener
    {
        private void Start() => GetComponent<Set>()?.onItemAdded.AddListener((value) =>
        {
            EventBus.Trigger(EventHooks.OnSetItemAdded, gameObject, value);
        });
    }
}