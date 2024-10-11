using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ToolkitEngine
{
	public class PoolItemManager : ConfigurableSubsystem<PoolItemManager, PoolItemManagerConfig>
    {
		#region Fields

		private Dictionary<PoolItem, PoolItemSpawner<PoolItem>> m_map = new();

		#endregion

		#region Methods

		protected override void Initialize()
		{
			base.Initialize();

			foreach (var spawner in Config.spawners)
			{
				m_map.Add(spawner.template, spawner);
			}
		}

		protected override void Terminate()
		{
			foreach (var spawner in Config.spawners)
			{
				spawner.Clear();
			}
		}

		public bool TryGet(PoolItem template, out PoolItem item)
		{
			Assert.IsNotNull(template);

			if (m_map.TryGetValue(template, out PoolItemSpawner<PoolItem> spawner))
			{
				return spawner.TryGet(out item);
			}

			item = null;
			return false;
		}

		public bool Release(PoolItem item)
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