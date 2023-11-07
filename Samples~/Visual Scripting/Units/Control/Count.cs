namespace Unity.VisualScripting
{
    /// <summary>
    /// Executes an action only once, and a different action afterwards.
    /// </summary>
    [UnitCategory("Control")]
    [UnitOrder(14)]
    public sealed class Count : Unit, IGraphElementWithData
    {
        public sealed class Data : IGraphElementData
        {
            public int value;
            public bool executed;
        }

        /// <summary>
        /// The entry point for the action.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }

        /// <summary>
        /// Trigger to reset the once check.
        /// </summary>
        [DoNotSerialize]
        public ControlInput reset { get; private set; }

        [DoNotSerialize]
        public ValueInput value { get; private set; }

        /// <summary>
        /// The action to execute the once node is entered target times.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput count { get; private set; }

        /// <summary>
        /// The action to execute subsequently.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput after { get; private set; }

        private int m_remaining;

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), Enter);
            reset = ControlInput(nameof(reset), Reset);
            count = ControlOutput(nameof(count));
            after = ControlOutput(nameof(after));
            value = ValueInput(nameof(value), 2);

            Succession(enter, count);
            Succession(enter, after);
        }

        public IGraphElementData CreateData()
        {
            return new Data();
        }

        public ControlOutput Enter(Flow flow)
        {
            var data = flow.stack.GetElementData<Data>(this);
            ++data.value;

            var target = flow.GetValue<int>(value);

            // Already triggered
            if (data.executed)
            {
                return after;
            }
            else if (data.value >= target)
            {
                data.executed = true;
                return count;
            }

            return null;
        }

        public ControlOutput Reset(Flow flow)
        {
            var data = flow.stack.GetElementData<Data>(this);
            data.executed = false;
            data.value = 0;

            return null;
        }
    }
}
