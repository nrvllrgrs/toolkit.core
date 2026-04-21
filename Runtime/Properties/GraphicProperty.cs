using UnityEngine;

namespace ToolkitEngine
{
	[GenericMenuCategory("Graphics")]
	public class SpriteProperty : ObjectProperty<Sprite>
	{
		public SpriteProperty()
			: base() { }

		public SpriteProperty(Sprite value)
			: base(value) { }
	}

	[GenericMenuCategory("Graphics")]
	public class MaterialProperty : ObjectProperty<Material>
	{
		public MaterialProperty()
			: base() { }

		public MaterialProperty(Material value)
			: base(value) { }
	}

	[GenericMenuCategory("Graphics")]
	public class ShaderProperty : ObjectProperty<Shader>
	{
		public ShaderProperty()
			: base() { }

		public ShaderProperty(Shader value)
			: base(value) { }
	}
}