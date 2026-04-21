using System;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[UnitCategory("Properties")]
	public class GetGenericObjectPropertyValue : Unit
	{
		[DoNotSerialize, PortLabelHidden]
		public ControlInput enter { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput exit { get; set; }

		[DoNotSerialize]
		public ValueInput properties { get; set; }

		[DoNotSerialize]
		public ValueInput key { get; set; }

		[Inspectable, UnitHeaderInspectable("Type")]
		public Type objectType { get; set; }

		[DoNotSerialize, PortLabelHidden]
		public ValueOutput value { get; set; }

		protected override void Definition()
		{
			enter = ControlInput(nameof(enter), Trigger);
			exit = ControlOutput(nameof(exit));
			Succession(enter, exit);

			properties = ValueInput<PropertyCollection>(nameof(properties));
			key = ValueInput(nameof(key), string.Empty);
			Requirement(properties, enter);
			Requirement(key, enter);

			value = ValueOutput(nameof(value), (flow) =>
			{
				var propertyCollection = flow.GetValue<PropertyCollection>(properties);
				var propertyKey = flow.GetValue<string>(key);

				// Use reflection to call the generic method
				var method = typeof(PropertyCollection).GetMethod(nameof(PropertyCollection.GetPropertyValue));
				var genericMethod = method.MakeGenericMethod(objectType ?? typeof(UnityEngine.Object));
				return genericMethod.Invoke(propertyCollection, new object[] { propertyKey });
			});
		}

		protected virtual ControlOutput Trigger(Flow flow) => exit;
	}
}