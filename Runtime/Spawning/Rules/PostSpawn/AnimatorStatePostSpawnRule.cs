using UnityEngine;

#if UNITY_EDITOR
using System.Linq;
#endif

namespace ToolkitEngine
{
    [AddComponentMenu("Spawner/Post-Spawn Rules/Animator State Post-Spawn Rule")]
    public class AnimatorStatePostSpawnRule : BasePostSpawnRule
    {
        #region Fields

        [SerializeField, Tooltip("The state name.")]
        private string m_stateName;

        [SerializeField, Tooltip("The layer index. If layer is -1, it plays the first state with the given state name."), Min(-1)]
        private int m_layer = -1;

        [SerializeField, Tooltip("The time offset between zero and one."), Range(0f, 1f)]
        private float m_normalizedTime = 0f;

        #endregion

        #region Methods

        public override void Process(Transform transform, Spawner spawner, GameObject spawnedObject)
		{
            var animator = spawnedObject.GetComponent<Animator>();
            if (animator == null)
                return;

            if (Application.isPlaying)
            {
                animator.Play(m_stateName, m_layer, m_normalizedTime);
            }
#if UNITY_EDITOR
            else
            {
                animator.speed = 0f;
                animator.Play(m_stateName, m_layer, m_normalizedTime);
                animator.Update(Time.deltaTime);
            }
#endif
        }

        #endregion

        #region Editor-Only
#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            //if (!Spawner.PostSpawnRules.Contains(this))
            //    return;

            //foreach (var proxy in Spawner.m_editorProxies)
            //{
            //    Process(null, proxy);
            //}
        }

#endif
        #endregion
    }
}