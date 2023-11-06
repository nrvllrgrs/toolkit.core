using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine
{
    [AddComponentMenu("Spawner/Post-Spawn Rules/Attachment Post-Spawn Rule")]
    public class AttachmentPostSpawnRule : BasePostSpawnRule
    {
        #region Fields

        [SerializeField]
        private ObjectSpawner m_spawner;

        [SerializeField]
        private HumanBodyBones m_bone;

        private HashSet<Spawner> m_registeredSpawners = new();
        private Dictionary<GameObject, PoolItem> m_map = new();

        #endregion

        #region Methods

        public override void Process(Transform transform, Spawner spawner, GameObject spawnedObject)
		{
            if (!m_registeredSpawners.Contains(spawner))
            {
                spawner.onDespawned.AddListener(Despawned);
            }

            var boneTargets = spawnedObject.GetComponent<HumanBodyTargets>();
            if (boneTargets != null && boneTargets.TryGetTarget(m_bone, out HumanBodyTargets.Target target))
            {
                //var item = m_spawner.Spawn(target.Point, false);
                //if (item != null)
                //{
                //    m_map.Add(spawnedObject, item);
                //}
            }
        }

        private void Despawned(SpawnerEventArgs e)
        {
            if (m_map.TryGetValue(e.spawnedObject, out PoolItem item))
            {
                item.AttemptRelease();
            }
        }

        #endregion
    }
}