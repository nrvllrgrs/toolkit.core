using UnityEngine;

namespace ToolkitEngine
{
	public class ConfigurableSubsystem<T, TConfig> : Subsystem<T>
        where T : class, ISubsystem, new()
		where TConfig : ScriptableObject
    {
		#region Fields

		private static TConfig s_config;

		#endregion

		#region Properties

		public static TConfig Config => s_config;

		#endregion
	}
}