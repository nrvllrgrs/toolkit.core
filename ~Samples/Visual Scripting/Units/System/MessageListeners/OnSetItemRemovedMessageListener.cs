using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
    [AddComponentMenu("")]
    public class OnSetItemRemovedMessageListener : MessageListener
    {
        private void Start() => GetComponent<Set>()?.onItemRemoved.AddListener((value) =>
        {
            EventBus.Trigger(EventHooks.OnSetItemRemoved, gameObject, value);
        });
    }
}