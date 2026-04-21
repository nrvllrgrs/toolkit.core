using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	public static class ScriptMachineExt
    {
        public static void SetScriptGraph(this ScriptMachine machine, ScriptGraphAsset scriptGraph)
        {
			machine.enabled = false;
			machine.nest.macro = scriptGraph;
			machine.enabled = true;
		}
    }
}