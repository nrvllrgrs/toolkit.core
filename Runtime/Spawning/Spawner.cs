using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

#if ADDRESSABLE_ASSETS
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ToolkitEngine
{
	public class SpawnerEventArgs : EventArgs
	{
		#region Properties

		public Spawner spawner { get; private set; }
		public GameObject spawnedObject { get; private set; }

		#endregion

		#region Constructors

		public SpawnerEventArgs(Spawner spawner)
			: this(spawner, null)
		{ }

		public SpawnerEventArgs(Spawner spawner, GameObject spawnedObject)
		{
			this.spawner = spawner;
			this.spawnedObject = spawnedObject;
		}

		#endregion
	}

	public delegate void SpawnedAction(GameObject spawnedObject, params object[] args);

	[System.Serializable]
    public class Spawner
    {
		#region Enumerators

		public enum SpawnType
		{
			Template,
			ObjectPool,
			Addressable,
		}

		#endregion

		#region Fields

		[SerializeField]
		protected SpawnType m_spawnType;

		[SerializeField]
		protected GameObject m_template;

		[SerializeField]
		protected PoolItemSpawner<PoolItem> m_pool;

#if ADDRESSABLE_ASSETS
		[SerializeField]
		protected AssetReferenceGameObject m_assetReference;
#endif

#if UNITY_EDITOR
		[SerializeField]
		private bool m_showInHierarchy = true;
#endif

		[SerializeField]
		private Set[] m_sets;

		private bool m_poolInitialized = false;
		private HashSet<GameObject> m_activeSpawns = new();

		#endregion

		#region Events

		[SerializeField]
		private UnityEvent<SpawnerEventArgs> m_onSpawning = new UnityEvent<SpawnerEventArgs>();

		[SerializeField]
		private UnityEvent<SpawnerEventArgs> m_onSpawned = new UnityEvent<SpawnerEventArgs>();

		[SerializeField]
		private UnityEvent<SpawnerEventArgs> m_onDespawned = new UnityEvent<SpawnerEventArgs>();

		#endregion

		#region Properties

		public int id { get; private set; }

		public SpawnType spawnType => m_spawnType;

		public bool isDefined
		{
			get
			{
				switch (m_spawnType)
				{
					case SpawnType.Template:
						return m_template != null;

					case SpawnType.ObjectPool:
						return m_pool.template?.gameObject != null;

#if ADDRESSABLE_ASSETS
					case SpawnType.Addressable:
						return m_assetReference.RuntimeKeyIsValid();
#endif
				}
				return false;
			}
		}

		public GameObject template
		{
			get
			{
				switch (m_spawnType)
				{
					case SpawnType.Template:
						return m_template;

					case SpawnType.ObjectPool:
						return m_pool.template?.gameObject;

					case SpawnType.Addressable:
						break;
				}
				return null;
			}
		}

		public Set[] sets => m_sets;
		public GameObject[] spawns => m_activeSpawns.ToArray();

		/// <summary>
		/// Number of objects currently spawned
		/// </summary>
		public int count => m_activeSpawns.Count;

		public Func<GameObject, Vector3, Quaternion, bool> spawningConditionFunc { get; set; }
		public Action<GameObject> actionOnSpawned { get; set; }
		public Action<GameObject> actionOnDespawned { get; set; }

		public UnityEvent<SpawnerEventArgs> onSpawning => m_onSpawning;
		public UnityEvent<SpawnerEventArgs> onSpawned => m_onSpawned;
		public UnityEvent<SpawnerEventArgs> onDespawned => m_onDespawned;

		#endregion

		#region Methods

		public void Set(GameObject template)
		{
			m_spawnType = SpawnType.Template;
			m_template = template;
		}

		public void Set(PoolItem poolItem)
		{
			m_spawnType = SpawnType.ObjectPool;
			m_pool.template = poolItem;
		}

#if ADDRESSABLE_ASSETS
		public void Set(AssetReferenceGameObject assetReference)
		{
			m_spawnType = SpawnType.Addressable;
			m_assetReference = assetReference;
		}
#endif
		#endregion

		#region Spawn Methods

		public bool Instantiate()
		{
			return Instantiate(null as Transform);
		}

		public bool Instantiate(SpawnedAction onSpawnedAction, params object[] args)
		{
			return Instantiate(null, onSpawnedAction, args);
		}

		public bool Instantiate(Transform parent)
		{
			return Instantiate(Vector3.zero, Quaternion.identity, parent);
		}

		public bool Instantiate(Transform parent, SpawnedAction onSpawnedAction, params object[] args)
		{
			return Instantiate(Vector3.zero, Quaternion.identity, parent, onSpawnedAction, args);
		}

		public bool Instantiate(Transform parent, bool spawnInWorldSpace)
		{
			return Instantiate(parent, spawnInWorldSpace, null);
		}

		public bool Instantiate(Transform parent, bool spawnInWorldSpace, SpawnedAction onSpawnedAction, params object[] args)
		{
			if (spawnInWorldSpace)
			{
				return Instantiate(parent, onSpawnedAction, args);
			}
			else
			{
				return Instantiate(parent.position, parent.rotation, parent, onSpawnedAction, args);
			}
		}

		public bool Instantiate(Vector3 position, Quaternion rotation)
		{
			return Instantiate(position, rotation, null as Transform);
		}

		public bool Instantiate(Vector3 position, Quaternion rotation, SpawnedAction onSpawnedAction, params object[] args)
		{
			return Instantiate(position, rotation, null, onSpawnedAction, args);
		}

		public bool Instantiate(Vector3 position, Quaternion rotation, Transform parent)
		{
			return Instantiate(position, rotation, parent, null);
		}

		public bool Instantiate(Vector3 position, Quaternion rotation, Transform parent, SpawnedAction onSpawnedAction, params object[] args)
		{
			if (!isDefined)
				return false;

			m_onSpawning?.Invoke(new SpawnerEventArgs(this));

			if (spawningConditionFunc != null && !spawningConditionFunc.Invoke(template, position, rotation))
				return false;

			switch (m_spawnType)
			{
				case SpawnType.Template:
					PostInstantiate(UnityEngine.Object.Instantiate(m_template, position, rotation, parent), position, onSpawnedAction, args);
					break;

				case SpawnType.ObjectPool:
					if (!m_poolInitialized)
					{
						m_pool.onReleasePoolItem = _OnReleasePoolItem;
						m_poolInitialized = true;
					}

					if (!m_pool.TryGet(out PoolItem poolItem))
					{
						Debug.LogFormat("Cannot spawn {0}! Object pool at capacity.", m_pool.template.name);
						return false;
					}

					var spawnedObject = poolItem.gameObject;
					spawnedObject.transform.SetParent(parent);
					spawnedObject.transform.SetPositionAndRotation(position, rotation);

					PostInstantiate(spawnedObject, position, onSpawnedAction, args);
					break;

#if ADDRESSABLE_ASSETS
				case SpawnType.Addressable:
					if (!m_assetReference.RuntimeKeyIsValid())
						return false;

					m_assetReference.InstantiateAsync(position, rotation, parent).Completed += (op) =>
					{
						if (op.Status != AsyncOperationStatus.Succeeded)
							return;

						PostInstantiate(op.Result, position, onSpawnedAction, args);
					};
					break;
#endif
			}

			return true;
		}

		protected virtual void PostInstantiate(GameObject spawnedObject, Vector3 position, SpawnedAction onSpawnedAction, params object[] args)
		{
#if UNITY_EDITOR
			if (!m_showInHierarchy)
			{
				spawnedObject.hideFlags |= HideFlags.HideInHierarchy;
			}
#endif

			WarpNavMeshAgent(spawnedObject, position);

			switch (m_spawnType)
			{
				case SpawnType.Template:
					spawnedObject.ReadyComponent<DestroyHandler>()
						.OnDestroyed.AddListener(SpawnedObject_Destroyed);
					break;

				case SpawnType.ObjectPool:
					// Decrementing simultaneous count handled in pool initialization
					break;

#if ADDRESSABLE_ASSETS
				case SpawnType.Addressable:
					spawnedObject.ReadyComponent<DestroyHandler>()
						.OnDestroyed.AddListener(AddressableDestroyed);
					break;
#endif
			}

			// Keep track of spawned objects
			m_activeSpawns.Add(spawnedObject);

			foreach (var set in m_sets)
			{
				set.Add(spawnedObject);
			}

			actionOnSpawned?.Invoke(spawnedObject);
			onSpawnedAction?.Invoke(spawnedObject, args);

			// Notify subscribers that object has spawned
			m_onSpawned?.Invoke(new SpawnerEventArgs(this, spawnedObject));
		}

		private void WarpNavMeshAgent(GameObject spawnedObject, Vector3 position)
		{
			var navMeshAgent = spawnedObject.GetComponent<NavMeshAgent>();
			if (navMeshAgent != null)
			{
				bool storedEnabled = false;

				var characterController = spawnedObject.GetComponent<CharacterController>();
				if (characterController != null)
				{
					storedEnabled = characterController.enabled;
					characterController.enabled = false;
				}

				navMeshAgent.Warp(position);

				if (characterController != null)
				{
					characterController.enabled = storedEnabled;
				}
			}
		}

		private void SpawnedObject_Destroyed(GameObject spawnedObject)
		{
			m_activeSpawns.Remove(spawnedObject);
			actionOnDespawned?.Invoke(spawnedObject);
			m_onDespawned?.Invoke(new SpawnerEventArgs(this, spawnedObject));
		}

		private void _OnReleasePoolItem(PoolItem poolItem)
		{
			SpawnedObject_Destroyed(poolItem.gameObject);
		}

#if ADDRESSABLE_ASSETS
		private void AddressableDestroyed(GameObject spawnedObject)
		{
			SpawnedObject_Destroyed(spawnedObject.gameObject);
			Addressables.Release(spawnedObject);
		}
#endif
		#endregion

		#region Despawn Methods

		public void DestroyAll()
		{
			var items = m_activeSpawns.ToArray();
			switch (m_spawnType)
			{
				case SpawnType.ObjectPool:
					for (int i = 0; i < items.Length; ++i)
					{
						m_pool.Release(items[i].GetComponent<PoolItem>());
					}
					break;

				default:
					for (int i = 0; i < items.Length; ++i)
					{
						UnityEngine.Object.Destroy(items[i]);
					}
					break;
			}
		}

		#endregion

		#region Editor-Only
#if UNITY_EDITOR

		public static void RefreshProxies(int instanceId, Spawner spawner, Transform point, Action<GameObject> postSpawnAction = null)
		{
			RefreshProxies(instanceId, spawner, new[] { point }, postSpawnAction);
		}

		public static void RefreshProxies(int instanceId, Spawner spawner, Transform[] points, Action<GameObject> postSpawnAction = null)
		{
			DestroyProxies(instanceId);

			foreach (var point in points)
			{
				SpawnProxy(instanceId, spawner, point, postSpawnAction); 
			}
		}

		public static void DestroyProxies(int instanceId)
		{
			if (!SpawnerProxyManager.instance.map.TryGetValue(instanceId, out var data))
				return;

			// Destroy all proxy objects
			foreach (var proxy in data.proxies)
			{
				if (!proxy.IsNull())
				{
					if (!data.addressableProxies)
					{
						UnityEngine.Object.DestroyImmediate(proxy);
					}
#if ADDRESSABLE_ASSETS
					else
					{
						Addressables.Release(proxy);
					}
#endif
				}
			}
			data.proxies.Clear();
		}

		public static void SpawnProxy(int instanceId, Spawner spawner, Transform point, Action<GameObject> postSpawnAction = null)
		{
			if (point == null)
				return;

			if (!SpawnerProxyManager.instance.map.TryGetValue(instanceId, out var data))
			{
				data = new SpawnerProxyManager.Data(); 
				SpawnerProxyManager.instance.map.Add(instanceId, data);
			}

			switch (spawner.spawnType)
			{
				case Spawner.SpawnType.Template:
				case Spawner.SpawnType.ObjectPool:
					var template = spawner.template;
					if (template == null)
						return;

					var obj = UnityEngine.Object.Instantiate(template, point.position, point.rotation, point);

					// Scale so proxy ignores parent's scale
					obj.transform.localScale = new Vector3(
						obj.transform.localScale.x / point.lossyScale.x,
						obj.transform.localScale.y / point.lossyScale.y,
						obj.transform.localScale.z / point.lossyScale.z);

					obj.SetActive(true);
					PostSpawnProxy(obj, data, postSpawnAction);

					data.addressableProxies = false;
					break;

				case Spawner.SpawnType.Addressable:
#if ADDRESSABLE_ASSETS
					if (!spawner.m_assetReference.RuntimeKeyIsValid())
						return;

					spawner.m_assetReference.InstantiateAsync(point.position, point.rotation, point).Completed += (op) =>
					{
						if (op.Result == null)
							return;

						// Scale so proxy ignores parent's scale
						op.Result.transform.localScale = new Vector3(
							op.Result.transform.localScale.x / point.lossyScale.x,
							op.Result.transform.localScale.y / point.lossyScale.y,
							op.Result.transform.localScale.z / point.lossyScale.z);

						op.Result.SetActive(true);
						PostSpawnProxy(op.Result, data, postSpawnAction);
					};
					data.addressableProxies = true;
#endif
					break;
			}
		}

		private static void PostSpawnProxy(GameObject proxy, SpawnerProxyManager.Data data, Action<GameObject> postSpawnAction)
		{
			data.proxies.Add(proxy);

			// Make sure children of proxy cannot be selected from scene
			foreach (var child in proxy.GetComponentsInChildren<Transform>())
			{
				child.gameObject.hideFlags |= HideFlags.NotEditable | HideFlags.DontSave | HideFlags.HideInHierarchy;
			}

			postSpawnAction?.Invoke(proxy);
		}

#endif
		#endregion
	}

#if UNITY_EDITOR

	public class SpawnerProxyManager : ScriptableSingleton<SpawnerProxyManager>
	{
		#region Fields

		[SerializeField]
		private SpawnerDataMap m_map = new();

		#endregion

		#region Properties

		public SpawnerDataMap map => m_map;

		#endregion

		#region Structures

		[Serializable]
		public class Data
		{
			public List<GameObject> proxies = new();
			public bool addressableProxies = false;
		}

		[Serializable]
		public class SpawnerDataMap : SerializableDictionary<int, Data>
		{ }

		#endregion
	}

#endif
}
