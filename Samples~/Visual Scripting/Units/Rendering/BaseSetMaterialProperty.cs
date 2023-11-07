using UnityEngine;
using Unity.VisualScripting;
using ToolkitEngine.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace ToolkitEngine.VisualScripting
{
	[UnitCategory("Rendering")]
	public abstract class BaseSetMaterialProperty<T> : Unit
	{
		#region Fields

		[DoNotSerialize, PortLabelHidden]
		public ControlInput inputTrigger { get; private set; }

		[DoNotSerialize]
		public ValueInput propertyName;

		[UnitHeaderInspectable]
		public BaseMaterialModifier.Mode mode;

		[DoNotSerialize]
		public ValueInput renderers;

		[DoNotSerialize]
		public ValueInput shared;

		//[DoNotSerialize, AllowsNull]
		//public ValueInput indices;

		[DoNotSerialize]
		public ValueInput materials;

		[DoNotSerialize]
		public ValueInput value;

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput outputTrigger { get; private set; }

		#endregion

		#region Methods

		protected override void Definition()
		{
			inputTrigger = ControlInput(nameof(inputTrigger), Trigger);
			propertyName = ValueInput(nameof(propertyName), string.Empty);

			switch (mode)
			{
				case BaseMaterialModifier.Mode.Renderer:
					renderers = ValueInput<Renderer[]>(nameof(renderers));
					//indices = ValueInput<int[]>(nameof(indices), null);
					shared = ValueInput(nameof(shared), false);
					break;

				case BaseMaterialModifier.Mode.Material:
					materials = ValueInput<Material[]>(nameof(materials));
					break;
			}

			DefineValue();

			outputTrigger = ControlOutput(nameof(outputTrigger));
			Succession(inputTrigger, outputTrigger);
		}

		protected virtual void DefineValue()
		{
			value = ValueInput<T>(nameof(value));
		}

		private ControlOutput Trigger(Flow flow)
		{
			string _propertyName = flow.GetValue<string>(propertyName);
			T _value = flow.GetValue<T>(value);

			switch (mode)
			{
				case BaseMaterialModifier.Mode.Renderer:
					foreach (var renderer in flow.GetValue<List<Renderer>>(renderers))
					{
						//int[] _indices = flow.GetValue<List<int>>(indices)?.ToArray();
						//if (_indices == null)
						//{
						//	_indices = new int[] { };
						//}

						bool _shared = flow.GetValue<bool>(shared);

						if (/*_indices.Length == 0 &&*/ !_shared)
						{
							var matPropBlock = new MaterialPropertyBlock();
							Set(matPropBlock, _propertyName, _value);
							renderer.SetPropertyBlock(matPropBlock);
						}
						else
						{
							List<Material> materials = new();
							if (_shared)
							{
								renderer.GetSharedMaterials(materials);
							}
							else
							{
								renderer.GetMaterials(materials);
							}

							for (int i = 0; i < materials.Count; ++i)
							{
								//if (_indices.Length > 0 && !_indices.Contains(i))
								//	continue;

								Set(materials[i], _propertyName, _value);
							}
						}
					}
					break;

				case BaseMaterialModifier.Mode.Material:
					foreach (var material in flow.GetValue<List<Material>>(materials))
					{
						Set(material, _propertyName, _value);
					}
					break;
			}

			return outputTrigger;
		}

		protected abstract void Set(Material material, string propertyName, T value);
		protected abstract void Set(MaterialPropertyBlock propertyBlock, string propertyName, T value);

		#endregion
	}
}