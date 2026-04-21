using System;
using UnityEngine;

namespace ToolkitEngine
{
    public static class InitializationSystem
    {
		#region Enumerators

		public enum Stage
		{
			NotStarted,
			SubsystemsRegistered,
			AfterAssembliesLoaded,
			BeforeSplashScreen,
			BeforeSceneLoad,
			AfterSceneLoad,
		}

		#endregion

		#region Fields

		private static Stage s_stage = Stage.NotStarted;

		#endregion

		#region Events

		public static Action<Stage> StageChanged;

		#endregion

		#region Properties

		public static Stage stage
		{
			get => s_stage;
			set
			{
				// No change, skip
				if (s_stage == value)
					return;

				s_stage = value;
				StageChanged?.Invoke(value);
			}
		}

		#endregion

		#region Methods

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void OnSubsystemsRegistered() => stage = Stage.SubsystemsRegistered;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		static void OnAfterAssembliesLoaded() => stage = Stage.AfterAssembliesLoaded;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
		static void OnBeforeSplashScreen() => stage = Stage.BeforeSplashScreen;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void OnBeforeSceneLoaded() => stage = Stage.BeforeSceneLoad;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static void OnAfterSceneLoaded() => stage = Stage.AfterSceneLoad;

#if UNITY_EDITOR
		[UnityEditor.InitializeOnLoadMethod]
		static void OnEditorLoad()
		{
			// Reset stage when entering play mode or on domain reload
			stage = Stage.NotStarted;
		}
#endif
		#endregion
	}
}