using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ToolkitEngine
{
    public class Labels : MonoBehaviour, ISet<LabelType>
    {
        #region Fields

        [SerializeField]
        private LabelType[] m_labels;

        private HashSet<LabelType> m_set;

		#endregion

		#region Properties

		public int Count => throw new System.NotImplementedException();

		public bool IsReadOnly => throw new System.NotImplementedException();

        #endregion

        #region Methods

        private void Awake()
        {
            m_set = new HashSet<LabelType>(m_labels);
        }

        #endregion

        #region Set Methods

        public bool Add(LabelType item)
		{
#if UNITY_EDITOR
			if (m_set.Add(item))
            {
				m_labels = m_set.ToArray();
                return true;
			}
            return false;
#else
            return m_set.Add(item);
#endif
		}

		public void Clear()
        {
#if UNITY_EDITOR
            m_labels = new LabelType[] { };
#endif
            m_set.Clear();
		}

		public bool Contains(LabelType item)
		{
            return m_set.Contains(item);
		}

		public void CopyTo(LabelType[] array, int arrayIndex)
		{
			m_set.CopyTo(array, arrayIndex);
		}

		public void ExceptWith(IEnumerable<LabelType> other)
		{
            m_set.ExceptWith(other);
		}

		public IEnumerator<LabelType> GetEnumerator()
        {
            return m_set.GetEnumerator();
        }

        public void IntersectWith(IEnumerable<LabelType> other)
        {
            m_set.IntersectWith(other);
        }

        public bool IsProperSubsetOf(IEnumerable<LabelType> other)
        {
			return m_set.IsProperSubsetOf(other);
		}

        public bool IsProperSupersetOf(IEnumerable<LabelType> other)
        {
            return m_set.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<LabelType> other)
        {
            return m_set.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<LabelType> other)
        {
            return m_set.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<LabelType> other)
        {
            return m_set.Overlaps(other);
        }

        public bool Remove(LabelType item)
        {
#if UNITY_EDITOR
			if (m_set.Remove(item))
			{
				m_labels = m_set.ToArray();
				return true;
			}
			return false;
#else
            return m_set.Remove(item);
#endif
		}

		public bool SetEquals(IEnumerable<LabelType> other)
        {
            return m_set.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<LabelType> other)
        {
            m_set.SymmetricExceptWith(other);
        }

        public void UnionWith(IEnumerable<LabelType> other)
        {
            m_set.UnionWith(other);
        }

        void ICollection<LabelType>.Add(LabelType item)
        {
            m_set.Add(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_set.GetEnumerator();
        }

        #endregion
    }
}