using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine
{
    public class Set : MonoBehaviour, ISet<GameObject>
    {
        #region Fields

        [SerializeField]
        private List<GameObject> m_items = new();

        [SerializeField]
        private bool m_removeOnDestroyed = true;

        [SerializeField]
        private bool m_removeOnKilled = true;

        [SerializeField, Min(0f), Tooltip("Seeconds to wait after \"Killed\" to remove gameObject.")]
        private float m_delay = 0f;

        [SerializeField, Tooltip("Indicates whether \"Release\" is attempted on removed gameObject.")]
        private bool m_releaseOnRemoved = true;

        private HashSet<GameObject> m_set = new();
        private Dictionary<GameObject, Coroutine> m_pendingRemovals = new();

        #endregion

        #region Events

        public UnityEvent<GameObject> onItemAdded = new UnityEvent<GameObject>();
        public UnityEvent<GameObject> onItemRemoved = new UnityEvent<GameObject>();

        #endregion

        #region Properties

        public GameObject[] Items => m_set.ToArray();

        public int Count => m_set.Count;

        /// <summary>
        /// Indicates whether set is currently empty
        /// </summary>
        public bool IsEmpty => m_set.Count == 0;

        public bool IsReadOnly => false;

        #endregion

        #region Methods

        private void Awake()
        {
            m_set = new HashSet<GameObject>(m_items);
        }

        private void OnDisable()
        {
            foreach (var p in m_pendingRemovals)
            {
                Remove(p.Key);
                StopCoroutine(p.Value);
            }
            m_pendingRemovals.Clear();
        }

        private void PoolItem_Released(PoolItem poolItem)
        {
            Remove(poolItem.gameObject);
        }

        private void Item_Destroyed(GameObject obj)
        {
			Remove(obj);
		}

		private void Killable_Killed(object sender, System.EventArgs e)
		{
            var obj = sender as GameObject;
            if (obj == null)
                return;

            m_pendingRemovals.Add(obj, StartCoroutine(AsyncRemoveKilled(obj)));
		}

        private IEnumerator AsyncRemoveKilled(GameObject obj)
        {
            yield return new WaitForSeconds(m_delay);
			m_pendingRemovals.Remove(obj);

			Remove(obj);
		}

		#endregion

		#region ISet Methods

		public bool Add(GameObject item)
        {
            if (item == null)
                return false;

            if (m_set.Add(item))
            {
#if UNITY_EDITOR
                m_items.Add(item);
#endif

                if (m_removeOnDestroyed)
                {
                    var poolItem = item.GetComponent<PoolItem>();
                    if (poolItem != null)
                    {
                        poolItem.OnReleased.AddListener(PoolItem_Released);
                    }
                    else
                    {
						var handler = item.ReadyComponent<DestroyHandler>();
						handler.OnDestroyed.AddListener(Item_Destroyed);
					}
                }

                if (m_removeOnKilled)
                {
                    var killable = item.GetComponent<IKillable>();
                    if (killable != null)
                    {
                        killable.Killed += Killable_Killed;
                    }
                }

                onItemAdded.Invoke(item);
                return true;
            }
            return false;
        }

        public bool Remove(GameObject item)
        {
			if (item == null)
				return false;

			if (m_set.Remove(item))
            {
#if UNITY_EDITOR
                m_items.Remove(item);
#endif

				if (m_removeOnDestroyed)
				{
					var poolItem = item.GetComponent<PoolItem>();
					if (poolItem != null)
					{
						poolItem.OnReleased.RemoveListener(PoolItem_Released);
					}
                    else
                    {
						item.GetComponent<DestroyHandler>()?.OnDestroyed.RemoveListener(Item_Destroyed);
					}
				}

				if (m_removeOnKilled)
				{
					var killable = item.GetComponent<IKillable>();
					if (killable != null)
					{
						killable.Killed -= Killable_Killed;
					}
				}

                if (m_releaseOnRemoved)
                {
                    var poolItem = item.GetComponent<PoolItem>();
                    if (poolItem != null)
                    {
                        poolItem.AttemptRelease();
                    }
                }

				onItemRemoved.Invoke(item);
                return true;
            }
            return false;
        }

        void ICollection<GameObject>.Add(GameObject item)
        {
            Add(item);
        }

        public void Clear()
        {
            m_set.Clear();

#if UNITY_EDITOR
            m_items.Clear();
#endif
        }

        public bool Contains(GameObject item)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return m_set.Contains(item);

            return m_items.Contains(item);
#else
            return m_set.Contains(item);
#endif
        }

        public void CopyTo(GameObject[] array, int arrayIndex)
        {
            m_set.CopyTo(array, arrayIndex);
        }

        public void ExceptWith(IEnumerable<GameObject> other)
        {
            m_set.ExceptWith(other);
        }

        public void IntersectWith(IEnumerable<GameObject> other)
        {
            m_set.IntersectWith(other);
        }

        public bool IsProperSubsetOf(IEnumerable<GameObject> other)
        {
            return m_set.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<GameObject> other)
        {
            return m_set.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<GameObject> other)
        {
            return m_set.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<GameObject> other)
        {
            return m_set.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<GameObject> other)
        {
            return m_set.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<GameObject> other)
        {
            return m_set.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<GameObject> other)
        {
            m_set.SymmetricExceptWith(other);
        }

        public void UnionWith(IEnumerable<GameObject> other)
        {
            m_set.UnionWith(other);
        }

        public IEnumerator<GameObject> GetEnumerator()
        {
            return m_set.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_set.GetEnumerator();
        }

		#endregion 
	}
}