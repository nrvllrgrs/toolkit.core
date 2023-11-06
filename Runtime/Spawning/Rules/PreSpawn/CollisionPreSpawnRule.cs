using UnityEngine;

namespace ToolkitEngine
{
    [AddComponentMenu("Spawner/Pre-Spawn Rules/Collision Pre-Spawn Rule")]
    /// <summary>
    /// Prevents spawn from occurring if template will spawn into collision.
    /// For example, an object could can run onto the spawner
    /// </summary>
    public class CollisionPreSpawnRule : BasePreSpawnRule
    {
        #region Fields

        [SerializeField]
        private LayerMask m_testLayers;

        [SerializeField]
        private QueryTriggerInteraction m_queryTrigger = QueryTriggerInteraction.Ignore;

        #endregion

        #region Methods

        public override bool Process(ObjectSpawner spawner, GameObject template, ref Vector3 position, ref Quaternion rotation)
        {
            if (template == null)
                return true;

            var bounds = template.GetColliderBounds();
            var center = new Vector3(position.x, position.y, position.z);
            center.y += bounds.extents.y;

            if (Physics.OverlapBox(center, bounds.extents, Quaternion.identity, m_testLayers, m_queryTrigger).Length > 0)
            {
                Debug.LogFormat("Cannot spawn object! Collision occurred at {0}!", position);
                return false;
            }

            return true;
        }

        #endregion
    }
}