using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace ToolkitEngine
{
    public sealed class PoolItemSpawner
    {
		#region Enumerators

		public enum SourceType
		{
			Internal,
			Direct,
			Global
		}

		#endregion
	}

	[System.Serializable]
    public sealed class PoolItemSpawner<T>
        where T : PoolItem
    {
		#region Enumerators

		public enum CapacityMode
        {
            Overflow,
            AutoRelease,
            Block,
        }

		#endregion

		#region Fields

		[SerializeField]
        private T m_template;

        [SerializeField]
        private PoolItemSpawner.SourceType m_source;

        [SerializeField]
        private ObjectSpawner m_spawner;

        /// <summary>
        /// Collection checks are performed when an instance is returned back to the pool. An exception will be thrown if the instance is already in the pool.
        /// </summary>
        [SerializeField, Tooltip("Collection checks are performed when an instance is returned back to the pool. An exception will be thrown if the instance is already in the pool.")]
        private bool m_collectionCheck = true;

        [SerializeField, Min(1), Tooltip("Maximum number of simultaneous pooled items.")]
        private int m_capacity = 1;

        [SerializeField]
        private CapacityMode m_capacityMode;

        private ObjectPool<T> m_objectPool;

        /// <summary>
        /// Use for limit type
        /// </summary>
        private List<T> m_queue = new List<T>();

        #endregion

        #region Properties

        public bool hasTemplate => m_template != null;
        public T template
        {
            get => m_template;
            set
            {
                // No change, skip
                if (m_template == value)
                    return;

                if (m_template != null)
                {
                    var items = m_queue.ToArray();
                    foreach (var item in items)
                    {
                        _OnDestroyPoolItem(item);
                    }
                    m_queue.Clear();
                }

                // Set new template
                m_template = value;
            }
        }

        internal ObjectPool<T> objectPool
        {
            get
            {
				if (m_objectPool == null)
				{
					if (m_source == PoolItemSpawner.SourceType.Internal)
					{
						m_objectPool = new ObjectPool<T>(_CreatePoolItem, _OnGetPoolItem, _OnReleasePoolItem, _OnDestroyPoolItem, m_collectionCheck, 1, m_capacity);
					}
                    else if (m_source == PoolItemSpawner.SourceType.Direct)
                    {
                        m_objectPool = m_spawner.objectPool as ObjectPool<T>;
                    }
				}
                return m_objectPool;
            }
        }

        private bool useGlobal => m_source == PoolItemSpawner.SourceType.Global;

		public System.Action<T> onCreatePoolItem { get; set; }
        public System.Action<T> onGetPoolItem { get; set; }
        public System.Action<T> onReleasePoolItem { get; set; }
        public System.Action<T> onDestroyPoolItem { get; set; }

        #endregion

        #region Methods

        public bool TryGet(out T item)
        {
            if (!useGlobal)
            {
				item = null;
				if (!hasTemplate)
					return false;

				switch (m_capacityMode)
				{
					case CapacityMode.Overflow:
						item = objectPool.Get();
						break;

					case CapacityMode.AutoRelease:
						if (m_queue.Count >= m_capacity)
						{
							// Release oldest item
							item = m_queue[0];
							m_queue.RemoveAt(0);

							objectPool.Release(item);
						}
						item = objectPool.Get();
						break;

					case CapacityMode.Block:
						if (m_queue.Count < m_capacity)
						{
							item = objectPool.Get();
						}
						break;
				}

				return item != null;
			}
            else if (PoolItemManager.Exists && PoolItemManager.TryGet(template, out PoolItem poolItem))
            {
				item = poolItem as T;
				return true;
            }

			item = null;
			return false;
		}

        public void Release(T item)
        {
            if (!useGlobal)
            {
				objectPool.Release(item);
			}
            else if (PoolItemManager.Exists)
            {
                PoolItemManager.Release(item);
            }
        }

        public void Clear()
        {
            if (!useGlobal)
            {
				objectPool.Clear();
			}
        }

        private T _CreatePoolItem()
        {
            var item = Object.Instantiate(m_template);
            item.ReleaseRequested += (sender, e) =>
            {
                if (!m_queue.Contains(sender as T))
                    return;

                objectPool.Release(sender as T);
            };

            onCreatePoolItem?.Invoke(item);
            item.CreatePoolItem();
            return item;
        }

        private void _OnGetPoolItem(T item)
        {
            foreach (var recyclable in item.GetComponentsInChildren<IPoolItemRecyclable>(true))
            {
                recyclable.Recycle();
            }

            // Remember order of items
            m_queue.Add(item);

            onGetPoolItem?.Invoke(item);
            item.GetPoolItem();
        }

        private void _OnReleasePoolItem(T item)
        {
            // Remove item from queue
            m_queue.Remove(item);

            onReleasePoolItem?.Invoke(item);
            item.ReleasePoolItem();
        }

        private void _OnDestroyPoolItem(T item)
        {
            if (item == null)
                return;

            onDestroyPoolItem?.Invoke(item);
            item.DestroyPoolItem();
        }

        #endregion
    }
}