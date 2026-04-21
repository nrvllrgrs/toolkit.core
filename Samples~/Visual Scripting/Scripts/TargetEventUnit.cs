using System;
using UnityEngine;
using Unity.VisualScripting;
using System.Collections.Generic;

namespace ToolkitEngine.VisualScripting
{
	public abstract class TargetEventUnit<TArgs> : EventUnit<TArgs>, IGameObjectEventUnit
	{
		#region Enumerators

		public enum TargetType
		{
			GameObject,
			Global
		}

		#endregion

		#region Fields

		[UnitHeaderInspectable]
		public TargetType type;

		private TArgs m_eventArgs;
		private readonly HashSet<GraphReference> m_graphs = new();

		#endregion

		#region Ports

		/// <summary>
		/// The game object that listens for the event.
		/// </summary>
		[DoNotSerialize]
		[NullMeansSelf]
		[PortLabel("Target")]
		[PortLabelHidden]
		public ValueInput target { get; protected set; }

		[DoNotSerialize]
		[PortLabelHidden]
		public ValueOutput eventArgs { get ; private set; }

		#endregion

		#region Properties

		protected sealed override bool register => true;
		protected virtual string hookName => GetType().Name;
		protected virtual bool showEventArgs => true;
		public abstract Type MessageListenerType { get; }

		#endregion

		#region Methods

		protected override void Definition()
		{
			base.Definition();

			if (type == TargetType.GameObject)
			{
				target = ValueInput<GameObject>(nameof(target), null).NullMeansSelf();
			}

			if (showEventArgs)
			{
				eventArgs = ValueOutput("eventArgs", (flow) => m_eventArgs);
			}
		}

		public override EventHook GetHook(GraphReference reference)
		{
			if (!reference.hasData)
			{
				return hookName;
			}

			var data = reference.GetElementData<Data>(this);
			return new EventHook(hookName, data.target);
		}

		private void UpdateTarget(GraphStack stack)
		{
			var data = stack.GetElementData<Data>(this);
			var wasListening = data.isListening;

			var newTarget = Flow.FetchValue<GameObject>(target, stack.ToReference());
			if (newTarget != data.target)
			{
				if (wasListening)
				{
					StopListening(stack);
				}

				data.target = newTarget;

				if (wasListening)
				{
					StartListening(stack, false);
				}
			}
		}

		protected void StartListening(GraphStack stack, bool updateTarget)
		{
			switch (type)
			{
				case TargetType.GameObject:
					if (updateTarget)
					{
						UpdateTarget(stack);
					}

					var data = stack.GetElementData<Data>(this);
					if (data.target == null)
					{
						return;
					}

					// can be null. CustomEvent doesn't need a message listener
					if (UnityThread.allowsAPI && MessageListenerType != null)
					{
						MessageListener.AddTo(MessageListenerType, data.target);
					}

					base.StartListening(stack);
					break;

				case TargetType.Global:
					var reference = stack.AsReference();
					if (!m_graphs.Contains(reference))
					{
						if (m_graphs.Count == 0)
						{
							StartListeningToManager();
						}
						m_graphs.Add(reference);
					}
					break;
			}
		}

		public override void StartListening(GraphStack stack)
		{
			StartListening(stack, true);
		}

		public override void StopListening(GraphStack stack)
		{
			switch (type)
			{
				case TargetType.GameObject:
					base.StopListening(stack);
					break;

				case TargetType.Global:
					if (register)
					{
						var reference = stack.AsReference();
						m_graphs.Remove(reference);
						if (m_graphs.Count == 0)
						{
							StopListeningToManager(); // unsubscribe only when all machines are gone
						}
					}
					break;
			}
		}

		protected abstract void StartListeningToManager();
		protected abstract void StopListeningToManager();

		protected void InvokeTrigger(TArgs e)
		{
			var cachedGraphs = new HashSet<GraphReference>(m_graphs);
			foreach (var graph in cachedGraphs)
			{
				Trigger(graph, e);
			}
		}

		protected void InvokeTrigger(object sender, TArgs e)
		{
			var cachedGraphs = new HashSet<GraphReference>(m_graphs);
			foreach (var graph in cachedGraphs)
			{
				Trigger(graph, e);
			}
		}

		protected override void AssignArguments(Flow flow, TArgs args)
		{
			if (eventArgs != null)
			{
				flow.SetValue(eventArgs, args);
			}
		}

		public override IGraphElementData CreateData()
		{
			return new Data();
		}

		#endregion

		#region Structures

		public new class Data : EventUnit<TArgs>.Data
		{
			public GameObject target;
		}

		#endregion
	}
}
