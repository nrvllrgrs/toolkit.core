using Cysharp.Threading.Tasks;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace ToolkitEngine.VisualScripting
{
	[UnitCategory("UniTask")]
	[UnitTitle("Call Graph")]
	public class CallGraph : Unit
	{
		#region Fields

		[Serialize, Inspectable, UnitHeaderInspectable("Event Name")]
		public string eventName { get; set; } = "GraphCall";

		[Serialize, Inspectable, UnitHeaderInspectable("Type")]
		public Type valueType { get; set; }

		#endregion

		#region Ports

		[DoNotSerialize, PortLabelHidden]
		public ControlInput enter { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput exit { get; set; }

		[DoNotSerialize, NullMeansSelf]
		public ValueInput target { get; set; }

		[DoNotSerialize, AllowsNull]
		public ValueInput payload { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ValueOutput result { get; set; }

		#endregion

		#region Methods

		protected override void Definition()
		{
			// Ensure valueType is never null
			if (valueType == null)
			{
				valueType = typeof(object);
			}

			enter = ControlInput(nameof(enter), Enter);
			exit = ControlOutput(nameof(exit));

			target = ValueInput<GameObject>(nameof(target));
			payload = ValueInput<object>(nameof(payload), null);
			result = ValueOutput(valueType, nameof(result));

			Requirement(target, enter);
			Succession(enter, exit);
			Assignment(enter, result);
		}

		private ControlOutput Enter(Flow flow)
		{
			CallGraphAsync(flow).Forget();
			return null; // Pause flow
		}

		private async UniTaskVoid CallGraphAsync(Flow flow)
		{
			var targetObj = flow.GetValue<GameObject>(target);

			var machines = targetObj?.GetComponents<ScriptMachine>();
			if (machines.Length == 0)
			{
				Debug.LogError($"No ScriptMachine found on {targetObj.name}");
				flow.SetValue(result, null);
				return;
			}

			VisualScriptingBridge<object> bridge = new();
			foreach (var machine in machines)
			{
				if (!bridge.CanRunGraph(machine, eventName))
					continue;

				try
				{
					// Check if payload port is connected before getting value
					object payloadValue = payload.hasValidConnection
						? flow.GetValue(payload, valueType)
						: null;

					object value = await bridge.RunGraph(machine, eventName, payloadValue);
					flow.SetValue(result, value);
					flow.Invoke(exit);
				}
				finally
				{
					bridge.Dispose();
				}
				break;
			}
		}

		#endregion
	}
}