using System;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
    [UnitTitle("On Paused"), UnitSurtitle("Timed Curve")]
    public class OnTimedCurvePaused : BaseTimedCurveEventUnit
    {
        public override Type MessageListenerType => typeof(OnTimedCurvePausedMessageListener);
    }
}