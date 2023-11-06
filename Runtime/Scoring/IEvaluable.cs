using UnityEngine;

namespace ToolkitEngine
{
    public interface IEvaluable
    {
        bool enabled { get; }
        float bonusWeight { get; }
        float Evaluate(GameObject actor, GameObject target);
        float Evaluate(GameObject actor, GameObject target, Vector3 position);
    }
}