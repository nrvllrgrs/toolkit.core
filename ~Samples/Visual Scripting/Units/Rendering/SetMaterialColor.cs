using UnityEngine;

namespace ToolkitEngine.VisualScripting
{
	public class SetMaterialColor : BaseSetMaterialProperty<Color>
    {
		#region Methods

		protected override void DefineValue()
		{
			value = ValueInput(nameof(value), Color.white);
		}

		protected override void Set(Material material, string propertyName, Color color)
		{
			material.SetColor(propertyName, color);
		}

		protected override void Set(MaterialPropertyBlock propertyBlock, string propertyName, Color value)
		{
			propertyBlock.SetColor(propertyName, value);
		}

		#endregion
	}
}