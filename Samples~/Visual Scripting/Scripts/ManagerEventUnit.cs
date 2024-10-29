using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	public abstract class ManagerEventUnit<TArgs> : EventUnit<TArgs>
	{
		#region Fields

		private TArgs m_eventArgs;
		private GraphReference m_graph;

		#endregion

		#region Ports

		[DoNotSerialize]
		[PortLabelHidden]
		public ValueOutput eventArgs { get; private set; }

		#endregion

		#region Properties

		protected sealed override bool register => true;
		protected virtual string hookName => GetType().Name;
		protected virtual bool showEventArgs => true;

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

		protected void StartListening(GraphStack stack, bool updateTarget)
		{
			if (register && m_graph == null)
			{
				m_graph = stack.AsReference();
				StartListeningToManager();
			}
		}

		public override void StartListening(GraphStack stack)
		{
			StartListening(stack, true);
		}

		public override void StopListening(GraphStack stack)
		{
			if (register && m_graph != null)
			{
				StopListeningToManager();
				m_graph = null;
			}
		}

		protected abstract void StartListeningToManager();
		protected abstract void StopListeningToManager();

		protected void InvokeTrigger(object sender, TArgs e)
		{
			Trigger(m_graph, e);
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
