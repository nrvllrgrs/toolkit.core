using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[UnitCategory("Events")]
	[UnitTitle("Filter Event"), UnitSurtitle("Script Machine Filter")]
	public class ScriptMachineFilterEvent : EventUnit<ScriptMachineFilter.EventArgs>
	{
		#region Fields

		[DoNotSerialize, PortLabelHidden]
		public ValueInput _name { get; private set; }

		[DoNotSerialize, PortLabel("Filter")]
		public ValueOutput _filter { get; private set; }

		[DoNotSerialize, PortLabel("Actor")]
		public ValueOutput _actor { get; private set; }

		[DoNotSerialize, PortLabel("Target")]
		public ValueOutput _target { get; private set; }

		[DoNotSerialize, PortLabel("Postion")]
		public ValueOutput _position { get; private set; }

		#endregion

		#region Properties

		protected override bool register => true;

		#endregion

		#region Methods

		protected override void Definition()
		{
			base.Definition();

			_name = ValueInput(nameof(_name), string.Empty);

			_filter = ValueOutput<ScriptMachineFilter>(nameof(_filter));
			_actor = ValueOutput<GameObject>(nameof(_actor));
			_target = ValueOutput<GameObject>(nameof(_target));
			_position = ValueOutput<Vector3>(nameof(_position));
		}

		protected override void AssignArguments(Flow flow, ScriptMachineFilter.EventArgs args)
		{
			flow.SetValue(_filter, args.filter);
			flow.SetValue(_actor, args.actor);
			flow.SetValue(_target, args.target);
			flow.SetValue(_position, args.position);
		}

		protected override bool ShouldTrigger(Flow flow, ScriptMachineFilter.EventArgs args)
		{
			return CompareNames(flow, _name, args.name);
		}

		public override EventHook GetHook(GraphReference reference)
		{
			return new EventHook(nameof(ScriptMachineFilter), reference.self);
		}

		#endregion
	}
}