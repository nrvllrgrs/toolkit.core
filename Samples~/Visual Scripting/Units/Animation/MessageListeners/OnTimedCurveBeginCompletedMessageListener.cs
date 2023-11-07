using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
    [AddComponentMenu("")]
    public class OnTimedCurveBeginCompletedMessageListener : MessageListener
    {
        private void Start() => GetComponent<TimedCurve>()?.OnBeginCompleted.AddListener((value) =>
        {
            EventBus.Trigger(EventHooks.OnTimedCurveBeginCompleted, gameObject, value);
        });
    }
}