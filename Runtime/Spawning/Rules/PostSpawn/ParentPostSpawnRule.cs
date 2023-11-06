using UnityEngine;

namespace ToolkitEngine
{
    [AddComponentMenu("Spawner/Post-Spawn Rules/Parent Post-Spawn Rule")]
    public class ParentPostSpawnRule : BasePostSpawnRule
    {
        #region Fields

        [SerializeField]
        private Transform m_parent;

        [SerializeField]
        private bool m_worldPositionStays = true;

        #endregion

        #region Methods

        public override void Process(Transform transform, Spawner spawner, GameObject spawnedObject)
		{
            spawnedObject.transform.SetParent(m_parent != null ? m_parent : transform, m_worldPositionStays);
        }

        #endregion
    }
}