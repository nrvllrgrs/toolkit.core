using System.Collections;
using UnityEngine;

namespace ToolkitEngine
{
    public class Lifetime : MonoBehaviour
    {
        #region Fields

        [SerializeField, Min(0f)]
        private float m_value;

        private Coroutine m_thread = null;
        private PoolItem m_poolItem;

        #endregion

        #region Methods

        private void Awake()
        {
            m_poolItem = GetComponent<PoolItem>();
        }

        private void OnEnable()
        {
            this.RestartCoroutine(AsyncTerminiate(), ref m_thread);
        }

        private void OnDisable()
        {
            this.CancelCoroutine(ref m_thread);
        }

        private IEnumerator AsyncTerminiate()
        {
            yield return new WaitForSeconds(m_value);

            if (m_poolItem == null)
            {
                Destroy(gameObject);
            }
            else
            {
                m_poolItem.AttemptRelease();
            }
        }

        #endregion
    }
}