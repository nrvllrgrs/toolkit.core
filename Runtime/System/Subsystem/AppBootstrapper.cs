using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ToolkitEngine
{
	[Flags]
	public enum GameMode
	{
		None = 0,
		Play = 1 << 1,
		Edit = 1 << 2,
	}

	public static class AppBootstrapper
	{
		#region Fields

		// Tracks PlayerLoop insertions per phase so we can cleanly remove them on teardown.
		private static readonly Dictionary<Type, Dictionary<Type, PlayerLoopSystem>> s_subsystemsByPhase = new();

		// Subsystems initialized at runtime, in initialization order.
		// Kept so we can dispose in reverse order on shutdown.
		private static readonly List<ISubsystem> s_runtimeInstances = new();

#if UNITY_EDITOR
		private static readonly List<ISubsystem> s_editorInstances = new();
#endif

		#endregion

		#region Runtime

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		internal static void RuntimeInitializePlayerLoop()
		{
			var loop = PlayerLoop.GetCurrentPlayerLoop();

			InsertSubsystem<Update>(typeof(LifecycleSubsystem), ref loop, 0, LifecycleSubsystem.Update);
			InsertSubsystem<FixedUpdate>(typeof(LifecycleSubsystem), ref loop, 0, LifecycleSubsystem.FixedUpdate);
			InsertSubsystem<PostLateUpdate>(typeof(LifecycleSubsystem), ref loop, 0, LifecycleSubsystem.LateUpdate);

			PlayerLoop.SetPlayerLoop(loop);
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		internal static void RuntimeInitialize()
		{
			var config = Resources.Load<AppBootstrapperConfig>(AppBootstrapperConfig.RESOURCE_NAME);
			if (config == null)
			{
				Debug.LogWarning($"[AppBootstrapper] No {nameof(AppBootstrapperConfig)} found in Resources.");
				return;
			}

			var ordered = BuildInitializationOrder(config.entries, GameMode.Play);
			foreach (var entry in ordered)
			{
				var instance = InitializeSubsystem(entry);
				if (instance != null)
				{
					s_runtimeInstances.Add(instance);

					if (instance is IInstantiableSubsystem instantiable)
					{
						instantiable.Instantiate();
					}
				}
			}

			Application.quitting += TeardownRuntime;
		}

		private static void TeardownRuntime()
		{
			Application.quitting -= TeardownRuntime;
			DisposeInReverseOrder(s_runtimeInstances);
			s_runtimeInstances.Clear();

			var loop = PlayerLoop.GetCurrentPlayerLoop();
			RemoveSubsystem<Update>(typeof(LifecycleSubsystem), ref loop);
			RemoveSubsystem<FixedUpdate>(typeof(LifecycleSubsystem), ref loop);
			RemoveSubsystem<PostLateUpdate>(typeof(LifecycleSubsystem), ref loop);
			PlayerLoop.SetPlayerLoop(loop);
		}

		#endregion

		#region Editor Methods

#if UNITY_EDITOR
		[InitializeOnLoadMethod]
		internal static void EditorInitialize()
		{
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

			// Fire immediately on domain reload when already in edit mode.
			if (!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				InitializeEditModeSubsystems();
			}
		}

		private static void OnPlayModeStateChanged(PlayModeStateChange state)
		{
			switch (state)
			{
				// Tear down edit-mode subsystems before entering play mode.
				case PlayModeStateChange.ExitingEditMode:
					TeardownEditorSubsystems();
					break;

				// Tear down play-mode subsystems when leaving play mode.
				// (Application.quitting does not fire when stopping play mode in-editor.)
				case PlayModeStateChange.ExitingPlayMode:
					DisposeInReverseOrder(s_runtimeInstances);
					s_runtimeInstances.Clear();

					var loop = PlayerLoop.GetCurrentPlayerLoop();
					RemoveSubsystem<Update>(typeof(LifecycleSubsystem), ref loop);
					RemoveSubsystem<FixedUpdate>(typeof(LifecycleSubsystem), ref loop);
					RemoveSubsystem<PostLateUpdate>(typeof(LifecycleSubsystem), ref loop);
					PlayerLoop.SetPlayerLoop(loop);
					break;

				// Re-initialize edit-mode subsystems once play mode has fully exited.
				case PlayModeStateChange.EnteredEditMode:
					InitializeEditModeSubsystems();
					break;
			}
		}

		private static void InitializeEditModeSubsystems()
		{
			var config = Resources.Load<AppBootstrapperConfig>(AppBootstrapperConfig.RESOURCE_NAME);
			if (config == null) return;

			var ordered = BuildInitializationOrder(config.entries, GameMode.Edit);
			foreach (var entry in ordered)
			{
				var instance = InitializeSubsystem(entry);
				if (instance != null)
				{
					s_editorInstances.Add(instance);
				}
			}
		}

		private static void TeardownEditorSubsystems()
		{
			DisposeInReverseOrder(s_editorInstances);
			s_editorInstances.Clear();
		}
#endif

		#endregion

		#region Initialization / Teardown Methods

		/// <summary>
		/// Returns <paramref name="entries"/> filtered to <paramref name="mode"/> and sorted
		/// so every subsystem comes after all of its <see cref="SubsystemDependencyAttribute"/>
		/// dependencies. Emits warnings for missing dependencies and errors for cycles.
		/// </summary>
		private static List<SubsystemEntry> BuildInitializationOrder(
			SubsystemEntry[] entries,
			GameMode mode)
		{
			// 1. Collect entries that apply to this mode
			var relevant = new List<SubsystemEntry>();
			var typeToEntry = new Dictionary<Type, SubsystemEntry>();

			foreach (var entry in entries)
			{
				if (entry == null)
					continue;

				if ((entry.mode & mode) == 0)
					continue;

				var type = entry.SubsystemType;
				if (type == null)
					continue;

				relevant.Add(entry);
				typeToEntry[type] = entry;
			}

			// 2. Build in-degree map and reverse-adjacency list
			var inDegree = new Dictionary<Type, int>();
			var dependents = new Dictionary<Type, List<Type>>(); // dep → types that need it

			foreach (var entry in relevant)
			{
				inDegree[entry.SubsystemType] = 0;
			}

			// Snapshot before iterating — synthetic entries for missing dependencies are
			// appended to relevant during this loop, which would cause an
			// InvalidOperationException if we iterated relevant directly.
			var snapshot = new List<SubsystemEntry>(relevant);
			foreach (var entry in snapshot)
			{
				var type = entry.SubsystemType;

				foreach (var attr in type.GetCustomAttributes<SubsystemDependencyAttribute>(inherit: true))
				{
					var dependency = attr.DependencyType;
					if (!typeToEntry.ContainsKey(dependency))
					{
						Debug.LogWarning(
							$"[AppBootstrapper] '{type.Name}' depends on '{dependency.Name}' but it is not " +
							$"registered in {nameof(AppBootstrapperConfig)} for mode '{mode}'. " +
							$"It will be auto-initialized without a config.");

						// Auto-register a minimal entry so the sort still works.
						var synthetic = new SubsystemEntry { mode = mode };
						synthetic.SubsystemType = dependency;
						relevant.Add(synthetic);
						typeToEntry[dependency] = synthetic;
						inDegree[dependency] = 0;
					}

					++inDegree[type];

					if (!dependents.TryGetValue(dependency, out var list))
					{
						dependents[dependency] = list = new List<Type>();
					}
					list.Add(type);
				}
			}

			// 3. Kahn's algorithm
			var queue = new Queue<Type>();
			var result = new List<SubsystemEntry>(relevant.Count);

			foreach (var kvp in inDegree)
			{
				if (kvp.Value == 0)
				{
					queue.Enqueue(kvp.Key);
				}
			}

			while (queue.Count > 0)
			{
				var current = queue.Dequeue();
				result.Add(typeToEntry[current]);

				if (!dependents.TryGetValue(current, out var children))
					continue;

				foreach (var child in children)
				{
					if (--inDegree[child] == 0)
					{
						queue.Enqueue(child);
					}
				}
			}

			// 4. Cycle detection
			if (result.Count != inDegree.Count)
			{
				var cycleTypes = new List<string>();
				foreach (var kvp in inDegree)
				{
					if (kvp.Value > 0)
					{
						cycleTypes.Add(kvp.Key.Name);
					}
				}
				Debug.LogError(
					$"[AppBootstrapper] Circular dependency detected among: {string.Join(", ", cycleTypes)}. " +
					$"These subsystems will not be initialized.");
			}

			return result;
		}

		private static ISubsystem InitializeSubsystem(SubsystemEntry entry)
		{
			var type = entry.SubsystemType;
			if (type == null)
			{
				Debug.LogWarning("[AppBootstrapper] Entry has an unresolved or missing type – skipping.");
				return null;
			}

			if (type.IsAbstract || type.IsGenericTypeDefinition)
			{
				Debug.LogWarning($"[AppBootstrapper] '{type.FullName}' is abstract or open-generic – skipping.");
				return null;
			}

			if (entry.config != null)
			{
				TryInjectConfig(type, entry.config);
			}

			try
			{
				var prop = type.GetProperty("CastInstance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				if (prop == null)
				{
					Debug.LogWarning($"[AppBootstrapper] Could not find 'CastInstance' on '{type.FullName}'.");
					return null;
				}

				return prop.GetValue(null) as ISubsystem;
			}
			catch (Exception e)
			{
				Debug.LogError($"[AppBootstrapper] Failed to initialize '{type.FullName}': {e}");
				return null;
			}
		}

		/// <summary>
		/// Disposes subsystems in reverse initialization order (LIFO), mirroring
		/// the way dependencies were built up.
		/// </summary>
		private static void DisposeInReverseOrder(List<ISubsystem> instances)
		{
			for (int i = instances.Count - 1; i >= 0; --i)
			{
				try
				{
					instances[i]?.Dispose();
				}
				catch (Exception e)
				{
					Debug.LogError($"[AppBootstrapper] Error during teardown of subsystem at index {i}: {e}");
				}
			}
		}

		/// <summary>
		/// Directly sets the private static s_config field on
		/// ConfigurableSubsystem&lt;T,TConfig&gt; before initialization runs,
		/// making the bootstrapper the sole source of config assignment.
		/// </summary>
		private static void TryInjectConfig(Type subsystemType, ScriptableObject config)
		{
			var t = subsystemType;
			while (t != null)
			{
				if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ConfigurableSubsystem<,>))
				{
					var field = t.GetField("s_config", BindingFlags.NonPublic | BindingFlags.Static);
					if (field != null)
					{
						field.SetValue(null, config);
						return;
					}

					Debug.LogWarning(
						$"[AppBootstrapper] Found ConfigurableSubsystem base on '{subsystemType.FullName}' " +
						"but could not locate s_config field.");
					return;
				}
				t = t.BaseType;
			}

			Debug.LogWarning(
				$"[AppBootstrapper] A config was provided for '{subsystemType.FullName}' " +
				"but it does not derive from ConfigurableSubsystem – config ignored.");
		}

		#endregion

		#region Player Loop Methods

		private static bool InsertSubsystem<TPhase>(
			Type subsystemType,
			ref PlayerLoopSystem loop,
			int index,
			PlayerLoopSystem.UpdateFunction update = null)
		{
			var phaseType = typeof(TPhase);
			if (!s_subsystemsByPhase.TryGetValue(phaseType, out var subsystems))
				s_subsystemsByPhase[phaseType] = subsystems = new Dictionary<Type, PlayerLoopSystem>();

			if (subsystems.ContainsKey(subsystemType))
				return false;

			var system = new PlayerLoopSystem
			{
				type = subsystemType,
				updateDelegate = update,
				subSystemList = null
			};

			if (PlayerLoopUtil.InsertSystem<TPhase>(ref loop, in system, index))
			{
				subsystems[subsystemType] = system;
				return true;
			}
			return false;
		}

		private static bool RemoveSubsystem<TPhase>(Type subsystemType, ref PlayerLoopSystem loop)
		{
			var phaseType = typeof(TPhase);
			if (!s_subsystemsByPhase.TryGetValue(phaseType, out var subsystems)
				|| !subsystems.TryGetValue(subsystemType, out var system))
				return false;

			PlayerLoopUtil.RemoveSystem<TPhase>(ref loop, in system);
			subsystems.Remove(subsystemType);
			return true;
		}

		#endregion
	}

	[Serializable]
	public class SubsystemEntry
	{
		#region Fields

		[SerializeField]
		internal string m_typeAssemblyQualifiedName;

		[SerializeField]
		public ScriptableObject config;

		[SerializeField]
		public GameMode mode;

		#endregion

		#region Properties

		/// <summary>
		/// The resolved Subsystem type. Stored as an assembly-qualified name because
		/// Unity cannot serialize System.Type directly.
		/// </summary>
		public Type SubsystemType
		{
			get
			{
				return !string.IsNullOrEmpty(m_typeAssemblyQualifiedName)
					? Type.GetType(m_typeAssemblyQualifiedName)
					: null;
			}
			set => m_typeAssemblyQualifiedName = value?.AssemblyQualifiedName ?? string.Empty;
		}

		/// <summary>
		/// Returns the TConfig type argument if SubsystemType derives from
		/// ConfigurableSubsystem&lt;T, TConfig&gt;, otherwise null.
		/// </summary>
		public Type ConfigType => GetConfigurableSubsystemConfigType(SubsystemType);

		#endregion

		#region Static Helpers

		/// <summary>
		/// Walks the base-type chain of <paramref name="subsystemType"/> looking for a
		/// closed ConfigurableSubsystem&lt;,&gt; and returns its second generic argument
		/// (the TConfig), or null if not found.
		/// </summary>
		internal static Type GetConfigurableSubsystemConfigType(Type subsystemType)
		{
			var t = subsystemType;
			while (t != null)
			{
				if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ConfigurableSubsystem<,>))
				{
					return t.GetGenericArguments()[1];
				}
				t = t.BaseType;
			}
			return null;
		}

		#endregion
	}
}