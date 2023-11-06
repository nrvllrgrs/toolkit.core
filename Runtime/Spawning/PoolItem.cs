using System;
using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine
{
    public class PoolItem : MonoBehaviour
    {
        #region Events

        public UnityEvent<PoolItem> OnGet = new UnityEvent<PoolItem>();
        public UnityEvent<PoolItem> OnSpawned = new UnityEvent<PoolItem>();
        public UnityEvent<PoolItem> OnReleased = new UnityEvent<PoolItem>();

        internal event EventHandler ReleaseRequested;

        #endregion

        #region Methods

        [ContextMenu("Release")]
        public void AttemptRelease()
        {
            if (ReleaseRequested != null)
            {
                ReleaseRequested.Invoke(this, EventArgs.Empty);
            }
            else
            {
                UnityEngine.Object.Destroy(gameObject);
            }
        }

        internal void CreatePoolItem()
        { }

        internal void GetPoolItem()
        {
            gameObject.SetActive(true);
            OnGet.Invoke(this);
        }

        internal void ReleasePoolItem()
        {
            gameObject.SetActive(false);
            OnReleased.Invoke(this);
        }

        internal void DestroyPoolItem()
        {
            Destroy(gameObject);
        }

        public void Spawn()
        {
            OnSpawned?.Invoke(this);
        }

        public static void Destroy(GameObject gameObject)
        {
            var poolItem = gameObject.GetComponent<PoolItem>();
            if (poolItem != null)
            {
                poolItem?.AttemptRelease();
            }
            else
            {
                UnityEngine.Object.Destroy(gameObject);
            }
        }

        #endregion
    }
}