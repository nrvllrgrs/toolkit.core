using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
    [AddComponentMenu("")]
    public class OnTimedCurvePausedMessageListener : MessageListener
    {
        private void Start() => GetComponent<TimedCurve>()?.OnPaused.AddListener((value) =>
        {
            EventBus.Trigger(EventHooks.OnTimedCurvePaused, gameObject, value);
        });
    }
}