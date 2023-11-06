using System;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
    [UnitTitle("On End Completed"), UnitSurtitle("Timed Curve")]
    public class OnTimedCurveEndCompleted : BaseTimedCurveEventUnit
    {
        public override Type MessageListenerType => typeof(OnTimedCurveEndCompletedMessageListener);
    }
}