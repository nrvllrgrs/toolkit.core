using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
    [AddComponentMenu("")]
    public class OnTimedCurveEndCompletedMessageListener : MessageListener
    {
        private void Start() => GetComponent<TimedCurve>()?.OnEndCompleted.AddListener((value) =>
        {
            EventBus.Trigger(EventHooks.OnTimedCurveEndCompleted, gameObject, value);
        });
    }
}