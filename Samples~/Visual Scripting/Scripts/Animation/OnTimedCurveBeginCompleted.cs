using System;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
    [UnitTitle("On Begin Completed"), UnitSurtitle("Timed Curve")]
    public class OnTimedCurveBeginCompleted : BaseTimedCurveEventUnit
    {
        public override Type MessageListenerType => typeof(OnTimedCurveBeginCompletedMessageListener);
    }
}