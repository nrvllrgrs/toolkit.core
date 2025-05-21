using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
    public abstract class BaseEventUnit<TArgs> : GameObjectEventUnit<TArgs>
    {
        #region Fields

        [DoNotSerialize, PortLabelHidden]
        public ValueOutput eventArgs;

        protected TArgs m_eventArgs;

        #endregion

        #region Properties

        protected virtual bool showEventArgs => true;
        protected override string hookName => GetType().Name;

        #endregion

        #region Methods

        protected override void Definition()
        {
            base.Definition();

            if (showEventArgs)
            {
                eventArgs = ValueOutput("eventArgs", (flow) => m_eventArgs);
            }
        }

        protected override void AssignArguments(Flow flow, TArgs args)
        {
            if (eventArgs != null)
            {
                flow.SetValue(eventArgs, args);
            }
        }

        #endregion
    }
}