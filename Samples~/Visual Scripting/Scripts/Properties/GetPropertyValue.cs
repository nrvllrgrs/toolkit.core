using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[UnitCategory("Properties")]
	public abstract class GetPropertyValue<T> : Unit
    {
		#region Ports

		[DoNotSerialize, PortLabelHidden]
		public ControlInput enter { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput exit { get; set; }

		[DoNotSerialize]
		public ValueInput properties { get; set; }

		[DoNotSerialize]
		public ValueInput key { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ValueOutput value { get; set; }

		#endregion

		#region Methods

		protected override void Definition()
		{
			enter = ControlInput(nameof(enter), Trigger);
			exit = ControlOutput(nameof(exit));
			Succession(enter, exit);

			properties = ValueInput<PropertyCollection>(nameof(properties));
			key = ValueInput(nameof(key), string.Empty);
			Requirement(properties, enter);
			Requirement(key, enter);

			value = ValueOutput(nameof(value), GetValue);
		}

		protected virtual ControlOutput Trigger(Flow flow) => exit;

		protected virtual T GetValue(Flow flow)
		{
			return flow.GetValue<PropertyCollection>(properties)
				.GetPropertyValue<T>(flow.GetValue<string>(key));
		}

		#endregion
	}

	public class GetBoolPropertyValue : GetPropertyValue<bool>
	{ }

	public class GetColorPropertyValue : GetPropertyValue<Color>
	{ }

	public class GetFloatPropertyValue : GetPropertyValue<float>
	{ }

	public class GetIntPropertyValue : GetPropertyValue<int>
	{ }

	public class GetObjectPropertyValue : GetPropertyValue<Object>
	{ }

	public class GetStringPropertyValue : GetPropertyValue<string>
	{ }

	public class GetVector2PropertyValue : GetPropertyValue<Vector2>
	{ }

	public class GetVector2IntPropertyValue : GetPropertyValue<Vector2Int>
	{ }

	public class GetVector3PropertyValue : GetPropertyValue<Vector3>
	{ }

	public class GetVector3IntPropertyValue : GetPropertyValue<Vector3Int>
	{ }

	public class GetVector4PropertyValue : GetPropertyValue<Vector4>
	{ }
}