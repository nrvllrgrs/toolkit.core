using System;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
    [UnitTitle("On Played"), UnitSurtitle("Timed Curve")]
    public class OnTimedCurvePlayed : BaseTimedCurveEventUnit
    {
        public override Type MessageListenerType => typeof(OnTimedCurvePlayedMessageListener);
    }
}