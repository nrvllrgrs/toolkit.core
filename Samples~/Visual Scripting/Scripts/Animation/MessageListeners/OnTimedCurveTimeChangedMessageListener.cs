using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
    [AddComponentMenu("")]
    public class OnTimedCurveTimeChangedMessageListener : MessageListener
    {
        private void Start() => GetComponent<TimedCurve>()?.OnTimeChanged.AddListener((value) =>
        {
            EventBus.Trigger(EventHooks.OnTimedCurveTimeChanged, gameObject, value);
        });
    }
}