using UnityEngine;

namespace ToolkitEngine
{
	public class ConfigurableSubsystem<T, TConfig> : Subsystem<T>
        where T : class, ISubsystem, new()
		where TConfig : ScriptableObject
    {
		#region Fields

		private TConfig m_config;

		#endregion

		#region Properties

		public TConfig Config
		{
			get
			{
				if (m_config == null)
				{
					ConfigManager.TryGet(out m_config);
				}
				return m_config;
			}
		}

		#endregion
	}
}