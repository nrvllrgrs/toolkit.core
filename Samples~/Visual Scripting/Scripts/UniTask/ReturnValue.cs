using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[UnitTitle("Return Value")]
	public class ReturnValue : Unit
	{
		#region Fields

		[Serialize, Inspectable, UnitHeaderInspectable("Type")]
		//[TypeFilter(typeof(object), Matching = TypesMatching.AssignableToAll)]
		public System.Type returnType { get; set; }

		#endregion

		#region Ports

		[DoNotSerialize, PortLabelHidden]
		public ControlInput enter { get; set; }

		[DoNotSerialize]
		public ValueInput bridge { get; set; }

		[DoNotSerialize]
		public ValueInput value { get; set; }

		#endregion

		#region Methods

		protected override void Definition()
		{
			// Ensure returnType is never null
			if (returnType == null)
			{
				returnType = typeof(object);
			}

			enter = ControlInput(nameof(enter), Trigger);
			bridge = ValueInput<object>(nameof(bridge));
			value = ValueInput(returnType, nameof(value));

			Requirement(bridge, enter);
		}

		private ControlOutput Trigger(Flow flow)
		{
			if (flow.GetValue<object>(bridge) is IVisualScriptingBridge b)
			{
				b.SetResult(flow.GetValue(value));
			}
			else
			{
				Debug.LogError("Bridge type mismatch.");
			}
			return null;
		}

		#endregion
	}
}