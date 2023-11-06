using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
    [AddComponentMenu("")]
    public class OnSetEmptyMessageListener : MessageListener
    {
        private Set m_set;

        private void Start()
        {
            m_set = GetComponent<Set>();
            if (m_set != null)
            {
                m_set.onItemRemoved.AddListener((value) =>
                {
                    if (m_set.IsEmpty)
                    {
                        EventBus.Trigger(EventHooks.OnSetEmpty, gameObject, value);
                    }
                });
            }
        }
    }
}