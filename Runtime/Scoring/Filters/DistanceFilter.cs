using UnityEngine;

namespace ToolkitEngine
{
	public class DistanceFilter : BaseCompareFilter<float>
    {
		protected override bool IsIncluded(GameObject actor, GameObject target, Vector3 position)
		{
			float sqrDistance = (actor.transform.position - position).sqrMagnitude;
			return CompareTo(sqrDistance, m_compareTo * m_compareTo);
		}
	}
}