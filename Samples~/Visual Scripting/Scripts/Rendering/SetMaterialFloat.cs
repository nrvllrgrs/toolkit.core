using UnityEngine;

namespace ToolkitEngine.VisualScripting
{
	public class SetMaterialFloat : BaseSetMaterialProperty<float>
	{
		#region Methods

		protected override void DefineValue()
		{
			value = ValueInput(nameof(value), 0f);
		}

		protected override void Set(Material material, string propertyName, float value)
		{
			material.SetFloat(propertyName, value);
		}

		protected override void Set(MaterialPropertyBlock propertyBlock, string propertyName, float value)
		{
			propertyBlock.SetFloat(propertyName, value);
		}

		#endregion
	}
}