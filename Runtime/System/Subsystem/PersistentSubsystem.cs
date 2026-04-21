using UnityEngine;

namespace ToolkitEngine
{
    public class PersistentSubsystem : ConfigurableSubsystem<PersistentSubsystem, PersistentSubsystemConfig>
    {
		#region Fields

		private PersistentSubsystemConfig.GameObjectMap m_objects = new();

		#endregion

		#region Methods

		protected override void Initialize()
		{
			base.Initialize();

			foreach (var p in Config.objects)
			{
				if (p.Value == null)
					continue;

				var clone = Object.Instantiate(p.Value);
				clone.name = p.Value.name;
				Object.DontDestroyOnLoad(clone);

				m_objects.Add(p.Key, clone);
			}
		}

		protected override void Terminate()
		{
			base.Terminate();

			foreach (var p in m_objects)
			{
				if (p.Value == null)
					continue;

				Object.Destroy(p.Value);
			}
		}

		public static bool ContainsKey(string key) => CastInstance.m_objects.ContainsKey(key);
		public static bool TryGetGameObject(string key, out GameObject obj) => CastInstance.m_objects.TryGetValue(key, out obj);

		#endregion
	}
}