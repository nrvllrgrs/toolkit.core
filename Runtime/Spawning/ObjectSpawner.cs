using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

#if UNITY_ADDRESSABLES
using UnityEngine.AddressableAssets;
#endif

namespace ToolkitEngine
{
	public class ObjectSpawner : MonoBehaviour
	{
		#region Enumerators

		public enum SpawnSpace
		{
			SpawnAtPoint,
			SpawnInLocalSpace,
			SpawnInWorldSpace,
			PositionAndRotation,
		}

		public enum OrderMode
		{
			Sequence,
			Random,
			RandomWithoutRepeating,
			Indexed,
		};

		#endregion

		#region Fields

		[SerializeField]
		private Spawner m_spawner;

		[SerializeField]
		private Transform[] m_points = new Transform[] { };

		[SerializeField]
		private bool m_zeroRotation;

		[SerializeField]
		private OrderMode m_order;

		[SerializeField]
		private Transform m_parent;

		[SerializeField]
		private SpawnSpace m_spawnSpace = SpawnSpace.SpawnAtPoint;

		[SerializeField]
		private Vector3 m_position, m_rotation;

		[SerializeField, Tooltip("Indicates whether spawns on start.")]
		protected bool m_spawnOnStart;

		[SerializeField]
		private BasePreSpawnRule[] m_preSpawnRules;

		[SerializeField]
		private BasePostSpawnRule[] m_postSpawnRules;

		private int m_index;
		private IList<Transform> m_availablePoints = new List<Transform>();

		#endregion

		#region Properties

		/// <summary>
		/// Indicates whether template is defined
		/// </summary>
		public bool isDefined => m_spawner.isDefined;

		/// <summary>
		/// Template to spawn
		/// </summary>
		public GameObject template => m_spawner.template;

		/// <summary>
		/// Number of objects currently spawned
		/// </summary>
		public int count => m_spawner.count;

		public UnityEvent<SpawnerEventArgs> onSpawning => m_spawner.onSpawning;
		public UnityEvent<SpawnerEventArgs> onSpawned => m_spawner.onSpawned;
		public UnityEvent<SpawnerEventArgs> onDespawned => m_spawner.onDespawned;

		public bool hasPoints => m_spawnSpace == SpawnSpace.SpawnAtPoint && (m_points?.Length ?? 0) > 0;
		public Transform[] points
		{
			get
			{
				if (m_spawnSpace != SpawnSpace.SpawnAtPoint)
					return new Transform[] { };

				return m_points != null && m_points.Length > 0
					? m_points
					: new Transform[] { transform };
			}
		}

		internal ObjectPool<PoolItem> objectPool => m_spawner.objectPool;

		#endregion

		#region Methods

		protected virtual void Awake()
		{
			m_availablePoints = new List<Transform>(m_points);

			m_spawner.spawningConditionFunc = _SpawningCondition;
			m_spawner.actionOnSpawned = _OnSpawned;
		}

		protected virtual void Start()
		{
			if (m_spawnOnStart)
			{
				QuickSpawn();
			}
		}

		private bool _SpawningCondition(GameObject template, Vector3 position, Quaternion rotation)
		{
			foreach (var rule in m_preSpawnRules)
			{
				if (rule != null && !rule.Process(this, template, ref position, ref rotation))
					return false;
			}
			return true;
		}

		private void _OnSpawned(GameObject spawnedObject)
		{
			foreach (var rule in m_postSpawnRules)
			{
				if (rule != null)
				{
					rule.Process(transform, m_spawner, spawnedObject);
				}
			}
		}

		#endregion

		#region Spawner Methods

		public void Set(GameObject template)
		{
			m_spawner.Set(template);
		}

		public void Set(PoolItem poolItem)
		{
			m_spawner.Set(poolItem);
		}

#if UNITY_ADDRESSABLES
		public void Set(AssetReferenceGameObject assetReference)
		{
			m_spawner.Set(assetReference);
		}
#endif
#endregion

		#region Spawn Methods

		[ContextMenu("Quick Spawn")]
		public void QuickSpawn()
		{
			Spawn();
		}

		public bool Spawn()
		{
			switch (m_spawnSpace)
			{
				case SpawnSpace.SpawnAtPoint:
					return InstantiateNext(null);

				case SpawnSpace.SpawnInLocalSpace:
					return Instantiate(m_parent, false);

				case SpawnSpace.SpawnInWorldSpace:
					return Instantiate(m_parent, true);

				case SpawnSpace.PositionAndRotation:
					return Instantiate(m_position, Quaternion.Euler(m_rotation), m_parent);
			}
			return false;
		}

		public bool InstantiateNext(SpawnedAction spawnedAction, params object[] args)
		{
			if (m_spawnSpace != SpawnSpace.SpawnAtPoint)
				return false;

			Transform point = null;
			if (!hasPoints)
			{
				point = transform;
			}
			else
			{
				switch (m_order)
				{
					case OrderMode.Sequence:
						point = m_points[m_index];
						m_index = (m_index + 1).Mod(m_points.Length);
						break;

					case OrderMode.Random:
						point = m_points[Random.Range(0, m_points.Length)];
						break;

					case OrderMode.RandomWithoutRepeating:
						if (m_index == 0)
						{
							m_availablePoints = m_availablePoints.Shuffle();
						}
						point = m_availablePoints[m_index];
						m_index = (m_index + 1).Mod(m_points.Length);
						break;
				}
			}

			if (point != null)
			{
				return Instantiate(point.position, !m_zeroRotation ? point.rotation : Quaternion.identity, spawnedAction, args);
			}
			return false;
		}

		public bool Instantiate(int index)
		{
			return Instantiate(index, null);
		}

		public bool Instantiate(int index, SpawnedAction spawnedAction, params object[] args)
		{
			if (m_order != OrderMode.Indexed)
				return false;

			if (!index.Between(0, m_points.Length - 1))
				return false;

			var point = m_points[index];
			return Instantiate(point.position, point.rotation, spawnedAction, args);
		}

		public bool Instantiate()
		{
			return Instantiate(null as Transform);
		}

		public bool Instantiate(SpawnedAction spawnedAction, params object[] args)
		{
			return Instantiate(null, spawnedAction, args, spawnedAction, args);
		}

		public bool Instantiate(Transform parent)
		{
			return Instantiate(Vector3.zero, Quaternion.identity, parent);
		}

		public bool Instantiate(Transform parent, SpawnedAction spawnedAction, params object[] args)
		{
			return Instantiate(Vector3.zero, Quaternion.identity, parent, spawnedAction, args);
		}

		public bool Instantiate(Transform parent, bool spawnInWorldSpace)
		{
			return Instantiate(parent, spawnInWorldSpace, null);
		}

		public bool Instantiate(Transform parent, bool spawnInWorldSpace, SpawnedAction spawnedAction, params object[] args)
		{
			if (spawnInWorldSpace)
			{
				return Instantiate(parent, spawnedAction, args, spawnedAction, args);
			}
			else
			{
				return Instantiate(parent.position, parent.rotation, parent, spawnedAction, args);
			}
		}

		public bool Instantiate(Vector3 position, Quaternion rotation)
		{
			return Instantiate(position, rotation, null as Transform);
		}

		public bool Instantiate(Vector3 position, Quaternion rotation, SpawnedAction spawnedAction, params object[] args)
		{
			return Instantiate(position, rotation, null, spawnedAction, args);
		}

		public bool Instantiate(Vector3 position, Quaternion rotation, Transform parent)
		{
			return Instantiate(position, rotation, parent, null);
		}

		public bool Instantiate(Vector3 position, Quaternion rotation, Transform parent, SpawnedAction spawnedAction, params object[] args)
		{
			return m_spawner.Instantiate(position, rotation, parent, spawnedAction, args);
		}

		[ContextMenu("Despawn All")]
		public void DestroyAll()
		{
			m_spawner.DestroyAll();
		}

		#endregion

		#region Editor-Only
#if UNITY_EDITOR

		private void OnValidate()
		{
			if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
				return;

			UnityEditor.EditorApplication.delayCall += () =>
			{
				switch (m_spawnSpace)
				{
					case SpawnSpace.SpawnAtPoint:
						Spawner.RefreshProxies(GetInstanceID(), m_spawner, points);
						break;

					case SpawnSpace.SpawnInLocalSpace when m_parent != null:
					case SpawnSpace.SpawnInWorldSpace when m_parent != null:
						Spawner.RefreshProxies(GetInstanceID(), m_spawner, m_parent);
						break;

					case SpawnSpace.PositionAndRotation:
						Spawner.DestroyProxies(GetInstanceID());
						break;
				}
			};
		}

		private void OnDrawGizmos()
		{
			if (Application.isPlaying)
				return;

			switch (m_spawnSpace)
			{
				case SpawnSpace.SpawnAtPoint:
					foreach (var point in points)
					{
						GizmosUtil.DrawArrow(point);
					}
					break;

				case SpawnSpace.SpawnInLocalSpace:
				case SpawnSpace.SpawnInWorldSpace:
					if (m_parent != null)
					{
						GizmosUtil.DrawArrow(m_parent);
					}
					break;

				case SpawnSpace.PositionAndRotation:
					GizmosUtil.DrawArrow(m_position, Quaternion.Euler(m_rotation));
					break;
			}
		}

#endif
		#endregion
	}
}