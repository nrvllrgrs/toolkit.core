using System.Linq;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

namespace ToolkitEditor.VisualScripting
{
	public static class VisualScriptingVariablesUtil
    {
        [MenuItem("CONTEXT/ScriptMachine/Initialize Variables")]
        public static void InitializeVariables(MenuCommand cmd)
        {
            if (cmd.context is not ScriptMachine scriptMachine)
                return;

            var graph = scriptMachine.graph;
            if (graph == null)
                return;

            var variableTuples = graph.units.Where(x => x is UnifiedVariableUnit)
                .Cast<UnifiedVariableUnit>()
                .Where(x => x.kind == VariableKind.Object && !x.name.hasAnyConnection)
                .Select(x => new
                {
                    name = x.defaultValues[nameof(x.name)].ToString(),
                    value = default(object), // Want to eventually add type
				});

            var variableDeclaration = Variables.Object(scriptMachine.gameObject);

            foreach (var tuple in variableTuples)
            {
                if (!variableDeclaration.IsDefined(tuple.name))
                {
                    variableDeclaration.Set(tuple.name, tuple.value);
                }
            }
        }
    }
}