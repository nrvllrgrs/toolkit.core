using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine
{
	[CreateAssetMenu(menuName = "Toolkit/Config/PoolItem Manager Config")]
	public class PoolItemManagerConfig : ScriptableObject
    {
		#region Fields

		[SerializeField]
		private PoolItemSpawner<PoolItem>[] m_spawners;

		#endregion

		#region Properties

		public PoolItemSpawner<PoolItem>[] spawners => m_spawners;

		#endregion
	}
}