using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[System.Serializable]
	public class VisualScriptingBridge<T> : IVisualScriptingBridge, IDisposable
	{
		#region Fields

		private UniTaskCompletionSource<T> m_completionSource;
		private CancellationTokenSource m_cancellationTokenSource;

		private bool m_isRunning;
		private bool m_isDisposed;

		#endregion

		#region Properties

		public UniTask<T> task => m_completionSource.Task;
		public T result { get; private set; }

		#endregion

		#region Methods

		public void Dispose()
		{
			if (m_isDisposed)
				return;

			m_isDisposed = true;
			m_isRunning = false;

			m_cancellationTokenSource?.Cancel();
			m_cancellationTokenSource?.Dispose();
			m_completionSource?.TrySetCanceled();
		}

		public bool CanRunGraph(ScriptMachine machine, string eventName)
		{
			return HasMatchingEvent(machine.graph, eventName);
		}

		public async UniTask<T> RunGraph(
			ScriptMachine machine,
			string eventName,
			object payload = null,
			CancellationToken cancellationToken = default,
			int timeoutMilliseconds = 30000)
		{
			if (m_isDisposed)
				throw new ObjectDisposedException(nameof(VisualScriptingBridge<T>));

			if (m_isRunning)
				throw new InvalidOperationException("Bridge is already running");

			if (machine == null || machine.graph == null)
			{
				throw new ArgumentNullException("machine");
			}

			m_isRunning = true;
			m_completionSource = new();
			m_cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

			try
			{
				var args = new BridgeEventArgs(eventName, this)
				{
					payload = payload,
				};
				EventBus.Trigger(nameof(OnGraphCall), machine.gameObject, args);

				// Wait for either completion, timeout, or cancellation
				var taskWithTimeout = task.AttachExternalCancellation(m_cancellationTokenSource.Token);
				if (timeoutMilliseconds > 0)
				{
					var timeoutTask = UniTask.Delay(timeoutMilliseconds, cancellationToken: m_cancellationTokenSource.Token);
					var (hasResult, _) = await UniTask.WhenAny(taskWithTimeout, timeoutTask);
					if (!hasResult)
					{
						throw new TimeoutException($"Bridge '{eventName}' timed out after {timeoutMilliseconds}ms");
					}
				}

				return await taskWithTimeout;
			}
			finally
			{
				m_isRunning = false;
				m_cancellationTokenSource?.Dispose();
				m_cancellationTokenSource = null;
			}
		}

		private bool HasMatchingEvent(FlowGraph graph, string eventName)
		{
			if (graph == null)
				return false;

			// Check units in this graph
			foreach (var unit in graph.units)
			{
				// Direct match
				if (unit is OnGraphCall bridge && bridge.key == eventName)
					return true;

				// Check subgraphs
				if (unit is SubgraphUnit subgraphUnit)
				{
					var nestedGraph = subgraphUnit.nest?.graph;
					if (nestedGraph != null && HasMatchingEvent(nestedGraph, eventName))
						return true;
				}
			}

			return false;
		}


		public void SetResult(T value)
		{
			if (m_isDisposed)
				return;

			result = value;
			m_completionSource.TrySetResult(value);
		}

		// Interface implementation for Visual Scripting
		void IVisualScriptingBridge.SetResult(object value)
		{
			if (value is T typedValue)
			{
				SetResult(typedValue);
			}
			else if (value != null && typeof(T).IsAssignableFrom(value.GetType()))
			{
				SetResult((T)value);
			}
			else
			{
				try
				{
					SetResult((T)Convert.ChangeType(value, typeof(T)));
				}
				catch (Exception e)
				{
					Debug.LogError($"Type mismatch: cannot convert {value?.GetType().Name ?? "null"} to {typeof(T).Name}: {e.Message}");
				}
			}
		}

		#endregion
	}

	public interface IVisualScriptingBridge
	{
		void SetResult(object value);
	}

	public class BridgeEventArgs
	{
		#region Fields

		private string m_key;
		private object m_bridge;
		public object payload;

		#endregion

		#region Properties

		public string key => m_key;
		public object bridge => m_bridge;

		#endregion

		#region Constructors

		public BridgeEventArgs(string key, object bridge)
		{
			m_key = key;
			m_bridge = bridge;
		}

		#endregion
	}
}