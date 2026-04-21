using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[System.Serializable]
	[GenericMenuCategory("Visual Scripting")]
    public class ScriptGraphProperty : BaseProperty<ScriptGraphAsset>
    {
		public ScriptGraphProperty()
			: base() { }

		public ScriptGraphProperty(ScriptGraphAsset value)
			: base(value) { }
	}
}