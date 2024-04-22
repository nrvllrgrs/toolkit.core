using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
    [UnitCategory("Events/Animation/Timed Curve")]
    public abstract class BaseTimedCurveEventUnit : BaseEventUnit<TimedCurve>
    {
        #region Properties

        protected override bool showEventArgs => false;

        #endregion
    }
}