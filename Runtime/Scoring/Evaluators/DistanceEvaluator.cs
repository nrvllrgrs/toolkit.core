using UnityEngine;

namespace ToolkitEngine
{
	public class DistanceEvaluator : BaseEvaluator
    {
		#region Fields

		[SerializeField, MinMax(0f, float.PositiveInfinity)]
		private Vector2 m_range = new Vector2(0f, 100f);

		#endregion

		#region Properties

		public float minDistance => m_range.x;
		public float maxDistance => m_range.y;

		#endregion

		#region Methods

		protected override float CalculateNormalizedScore(GameObject actor, GameObject target, Vector3 position)
		{
			return MathUtil.GetPercent(
				(actor.transform.position - position).sqrMagnitude,
				minDistance * minDistance,
				maxDistance * maxDistance);
		}

		#endregion
	}
}