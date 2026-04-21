using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace ToolkitEngine
{
	public class PoolItemManager : ConfigurableSubsystem<PoolItemManager, PoolItemManagerConfig>
    {
		#region Fields

		private Dictionary<PoolItem, PoolItemSpawner<PoolItem>> m_map = new();

		private static Scene s_dontDestroyOnLoadScene;
		private const string DONTDESTROYONLOAD_SCENE_NAME = "DontDestroyOnLoad";

		#endregion

		#region Properties

		public static Scene DontDestroyOnLoadScene
		{
			get
			{
				if (s_dontDestroyOnLoadScene == null)
				{
					s_dontDestroyOnLoadScene = SceneManager.GetSceneByName(DONTDESTROYONLOAD_SCENE_NAME);
				}
				return s_dontDestroyOnLoadScene;
			}
		}

		#endregion

		#region Methods

		protected override void Initialize()
		{
			base.Initialize();

			foreach (var spawner in Config.spawners)
			{
				m_map.Add(spawner.template, spawner);
				spawner.onReleasePoolItem += PoolItemReleased;
			}
		}

		protected override void Terminate()
		{
			foreach (var spawner in Config.spawners)
			{
				spawner.onReleasePoolItem -= PoolItemReleased;
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

		private void PoolItemReleased(PoolItem item)
		{
			item.transform.SetParent(null);
			UnityEngine.Object.DontDestroyOnLoad(item.gameObject);
		}

		#endregion
	}
}