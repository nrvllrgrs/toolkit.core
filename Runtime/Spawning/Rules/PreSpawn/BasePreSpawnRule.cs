using UnityEngine;

namespace ToolkitEngine
{
    public abstract class BasePreSpawnRule : MonoBehaviour
    {
        public abstract bool Process(ObjectSpawner spawner, GameObject template, ref Vector3 position, ref Quaternion rotation);
    }
}