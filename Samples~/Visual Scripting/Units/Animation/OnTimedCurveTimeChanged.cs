using System;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
    [UnitTitle("On Time Changed"), UnitSurtitle("Timed Curve")]
    public class OnTimedCurveTimeChanged : BaseTimedCurveEventUnit
    {
        public override Type MessageListenerType => typeof(OnTimedCurveTimeChangedMessageListener);

        #region Fields

        [DoNotSerialize]
        public ValueOutput time;

        [DoNotSerialize]
        public ValueOutput normalizedTime;

        [DoNotSerialize]
        public ValueOutput value;

        protected float m_time, m_normalizedTime, m_value;

        #endregion

        #region Methods

        protected override void Definition()
        {
            base.Definition();

            time = ValueOutput<float>("time");
            normalizedTime = ValueOutput<float>("normalizedTime");
            value = ValueOutput<float>("value");
        }

        protected override void AssignArguments(Flow flow, TimedCurve args)
        {
            flow.SetValue(time, args.time);
            flow.SetValue(normalizedTime, args.normalizedTime);
            flow.SetValue(value, args.value);
        }

        #endregion
    }
}