using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using Newtonsoft.Json;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ToolkitEngine
{
	public static class SubsystemBootstrapper
	{
		#region Fields

		private static Dictionary<Type, Dictionary<Type, PlayerLoopSystem>> s_subsystemsByPhase = new();

		private const string SUBSYSTEM_CONFIG_PATH = "";

		#endregion

		#region Methods

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		internal static void RuntimeInitialize()
		{
			PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();

			ConfigManager.Initialize();
			InsertSubsystem<Update>(typeof(LifecycleSubsystem), ref currentPlayerLoop, 0, LifecycleSubsystem.Update);
			InsertSubsystem<FixedUpdate>(typeof(LifecycleSubsystem), ref currentPlayerLoop, 0, LifecycleSubsystem.FixedUpdate);
			InsertSubsystem<PostLateUpdate>(typeof(LifecycleSubsystem), ref currentPlayerLoop, 0, LifecycleSubsystem.LateUpdate);

			var subsystemConfigs = Resources.LoadAll<TextAsset>(SUBSYSTEM_CONFIG_PATH);
			foreach (var config in subsystemConfigs)
			{
				try
				{
					// Instantiate each subsystem in config
					var types = JsonConvert.DeserializeObject<List<Type>>(config.text);
					foreach (var type in types)
					{
						Instantiate(type);
					}
				}
				catch { }
			}

			PlayerLoop.SetPlayerLoop(currentPlayerLoop);

#if UNITY_EDITOR
			EditorApplication.playModeStateChanged -= OnPlayModeState;
			EditorApplication.playModeStateChanged += OnPlayModeState;

			static void OnPlayModeState(PlayModeStateChange state)
			{
				if (state == PlayModeStateChange.ExitingPlayMode)
				{
					PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();

					RemoveSubsystem<Update>(typeof(LifecycleSubsystem), ref currentPlayerLoop);
					RemoveSubsystem<FixedUpdate>(typeof(LifecycleSubsystem), ref currentPlayerLoop);
					RemoveSubsystem<PostLateUpdate>(typeof(LifecycleSubsystem), ref currentPlayerLoop);
					LifecycleSubsystem.Dispose();

					PlayerLoop.SetPlayerLoop(currentPlayerLoop);
				}
			}
#endif
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		internal static void RuntimeInstantiate()
		{
			foreach (var config in ConfigManager.configs)
			{
				if (config is IInstantiableSubsystemConfig subsystemConfig)
				{
					GameObject template = subsystemConfig.GetTemplate();
					if (template == null)
						continue;

					GameObject clone = UnityEngine.Object.Instantiate(template);
					UnityEngine.Object.DontDestroyOnLoad(clone);

					// 'Instance' needs to be instantiated before reflection can get its property
					var type = subsystemConfig.GetManagerType();
					if (!Instantiate(type))
						continue;

					// Get Subsystem.Instance and set its instantiated gameObject
					var propertyInfo = type.GetProperty(nameof(ISubsystem.Instance), BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
					if (propertyInfo?.GetValue(null, null) is IInstantiableSubsystem subsystem)
					{
						clone.name = $"{type.Name} Instance";
						subsystem.SetInstance(clone);
					}
				}
			}
		}

		private static bool Instantiate(Type type)
		{
			if (type == null)
				return false;

			var methodInfo = type.GetMethod("Instantiate", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
			if (methodInfo != null)
			{
				methodInfo.Invoke(null, null);
				return true;
			}

			return false;
		}

#if UNITY_EDITOR
		[InitializeOnLoadMethod]
		internal static void Initialize()
		{
			ConfigManager.Initialize();

			EditorApplication.playModeStateChanged -= OnPlayModeState;
			EditorApplication.playModeStateChanged += OnPlayModeState;

			static void OnPlayModeState(PlayModeStateChange state)
			{
				if (state == PlayModeStateChange.ExitingEditMode)
				{
					LifecycleSubsystem.Dispose();
				}
			}
		}
#endif

		private static bool InsertSubsystem<T>(Type subsystemType, ref PlayerLoopSystem loop, int index, PlayerLoopSystem.UpdateFunction update = null)
		{
			var phaseType = typeof(T);
			if (!s_subsystemsByPhase.TryGetValue(phaseType, out var subsystems))
			{
				subsystems = new();
				s_subsystemsByPhase.Add(phaseType, subsystems);
			}

			if (subsystems.ContainsKey(subsystemType))
				return false;

			var system = new PlayerLoopSystem()
			{
				type = subsystemType,
				updateDelegate = update,
				subSystemList = null
			};

			if (PlayerLoopUtil.InsertSystem<T>(ref loop, in system, index))
			{
				subsystems.Add(subsystemType, system);
				return true;
			}
			return false;
		}

		private static bool RemoveSubsystem<T>(Type subsystemType, ref PlayerLoopSystem loop)
		{
			var phaseType = typeof(T);
			if (!s_subsystemsByPhase.TryGetValue(phaseType, out var subsystems)
				|| !subsystems.TryGetValue(subsystemType, out var system))
				return false;

			PlayerLoopUtil.RemoveSystem<T>(ref loop, in system);
			subsystems.Remove(subsystemType);
			return true;
		}

		#endregion
	}
}