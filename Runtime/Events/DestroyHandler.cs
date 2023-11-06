using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine
{
    public class DestroyHandler : MonoBehaviour
    {
        public UnityEvent<GameObject> OnDestroyed = new UnityEvent<GameObject>();

        private void OnDestroy()
        {
            OnDestroyed.Invoke(gameObject);
        }

        #region Editor-Only
#if UNITY_EDITOR

        [ContextMenu("Destroy")]
        private void Destroy()
        {
            Destroy(gameObject);
        }

#endif
        #endregion
    }
}