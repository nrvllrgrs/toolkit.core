using UnityEngine;

namespace ToolkitEngine
{
    public abstract class BasePostSpawnRule : MonoBehaviour
    {
        public abstract void Process(Transform transform, Spawner spawner, GameObject spawnedObject);
    }
}