using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
    [AddComponentMenu("")]
    public class OnTimedCurvePlayedMessageListener : MessageListener
    {
        private void Start() => GetComponent<TimedCurve>()?.OnPlayed.AddListener((value) =>
        {
            EventBus.Trigger(EventHooks.OnTimedCurvePlayed, gameObject, value);
        });
    }
}