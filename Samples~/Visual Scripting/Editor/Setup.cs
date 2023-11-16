using System;
using System.Collections.Generic;
using ToolkitEngine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;

namespace ToolkitEditor.VisualScripting
{
    [InitializeOnLoad]
    public static class Setup
    {
        static Setup()
        {
			bool dirty = false;
			var config = BoltCore.Configuration;

			var assemblyName = new LooseAssemblyName("ToolkitEngine");
			if (!config.assemblyOptions.Contains(assemblyName))
			{
				config.assemblyOptions.Add(assemblyName);
				dirty = true;
			}

			var types = new List<Type>()
			{
				typeof(TargetingPoints),
				typeof(TimedCurve),
				typeof(ShakeTransform),
				typeof(UnityCondition),
				typeof(UnityEvaluator),
				typeof(UnityElector),
				typeof(ObjectSpawner),
				typeof(ObjectSpawnerControl),
				typeof(PoolItem),
				typeof(WaveSpawner),
				typeof(WaveSpawner.Wave),
				typeof(Labels),
				typeof(LabelType),
				typeof(Set),
				typeof(MathUtil),
				typeof(PhysicsUtil),
				typeof(RandomUtil),
				typeof(ToolkitEngine.ShaderUtil),
			};

			foreach (var type in types)
			{
				if (!config.typeOptions.Contains(type))
				{
					config.typeOptions.Add(type);
					dirty = true;

					Debug.LogFormat("Adding {0} to Visual Scripting type options.", type.FullName);
				}
			}

			if (dirty)
			{
				var metadata = config.GetMetadata(nameof(config.typeOptions));
				metadata.Save();
				Codebase.UpdateSettings();
				UnitBase.Rebuild();
			}
		}
    }
}