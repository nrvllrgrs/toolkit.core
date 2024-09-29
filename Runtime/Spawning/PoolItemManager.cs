using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ToolkitEngine
{
	public class PoolItemManager : Singleton<PoolItemManager>
    {
		#region Fields

		[SerializeField]
		private PoolItemManagerConfig m_config;

		private static Dictionary<PoolItem, PoolItemSpawner<PoolItem>> m_map = new();

		#endregion

		#region Methods

		protected override void Initialize()
		{
			foreach (var spawner in m_config.spawners)
			{
				m_map.Add(spawner.template, spawner);
			}
		}

		protected override void Terminate()
		{
			foreach (var spawner in m_config.spawners)
			{
				spawner.Clear();
			}
		}

		public static bool TryGet(PoolItem template, out PoolItem item)
		{
			Assert.IsNotNull(template);

			if (m_map.TryGetValue(template, out PoolItemSpawner<PoolItem> spawner))
			{
				return spawner.TryGet(out item);
			}

			item = null;
			return false;
		}

		public static bool Release(PoolItem item)
		{
			if (m_map.TryGetValue(item, out PoolItemSpawner<PoolItem> spawner))
			{
				spawner.Release(item);
				return true;
			}

			return false;
		}

		#endregion
	}
}