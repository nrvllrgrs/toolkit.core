using System;
using System.Collections.Generic;
using ToolkitEngine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityToolbarExtender;

namespace ToolkitEditor.VisualScripting
{
	[InitializeOnLoad]
    public static class Setup
    {
		static Setup()
		{
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

			Initialize("ToolkitEngine", types);
		}

		public static void Initialize(string assemblyName, IEnumerable<Type> types)
		{
			Initialize(new[] { assemblyName }, types);
		}

		public static void Initialize(IEnumerable<string> assemblyNames, IEnumerable<Type> types)
		{
			if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
				return;

			BoltCoreConfiguration config = BoltCore.Configuration;

			bool assemblyDirty = false;
			foreach (var name in assemblyNames)
			{
				if (!config.assemblyOptions.Contains(name))
				{
					config.assemblyOptions.Add(name);
					assemblyDirty = true;

					Debug.LogFormat("Adding {0} to Visual Scripting assembly options.", name);
				}
			}

			if (assemblyDirty)
			{
				var metadata = config.GetMetadata(nameof(config.assemblyOptions));
				metadata.Save();
			}

			bool typeDirty = false;
			foreach (var type in types)
			{
				if (!config.typeOptions.Contains(type))
				{
					config.typeOptions.Add(type);
					typeDirty = true;

					Debug.LogFormat("Adding {0} to Visual Scripting type options.", type.FullName);
				}
			}

			if (typeDirty)
			{
				var metadata = config.GetMetadata(nameof(config.typeOptions));
				metadata.Save();
			}

			if (assemblyDirty || typeDirty)
			{
				foreach (var settings in config.projectSettings)
				{
					settings.Save();
				}

				config.SaveProjectSettingsAsset(true);
				Codebase.UpdateSettings();
				UnitBase.Rebuild();
			}
		}
	}

	[InitializeOnLoad]
	public static class VisualScriptToolbar
	{
		private const string REGENERATE_NODES_ICON_PATH = "Assets/Samples/Core Toolkit/1.0.0/Visual Scripting/Editor/regenerateNodes_Icon.png";
		private static Texture2D s_regenerateNodesIcon;

		static VisualScriptToolbar()
		{
			s_regenerateNodesIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(REGENERATE_NODES_ICON_PATH);
			ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
		}

		private static void OnToolbarGUI()
		{
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(
				new GUIContent(s_regenerateNodesIcon, "Regenerate Nodes"),
				EditorStyles.toolbarButton,
				GUILayout.Width(32),
				GUILayout.Height(20)))
			{
				UnitBase.Rebuild();
			}
		}
	}
}