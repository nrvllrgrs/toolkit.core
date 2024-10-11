using UnityEngine;

namespace ToolkitEngine
{
	public class DistanceEvaluator : BaseEvaluator
    {
		#region Fields

		[SerializeField, MinMax(0f, float.PositiveInfinity)]
		private Vector2 m_range = new Vector2(0f, 100f);

		[SerializeField, Tooltip("Indicates whether angle should only be evaluated on X-Z plane.")]
		private bool m_horizontalOnly = false;

		#endregion

		#region Properties

		public float minDistance => m_range.x;
		public float maxDistance => m_range.y;

		#endregion

		#region Methods

		protected override float CalculateNormalizedScore(GameObject actor, GameObject target, Vector3 position)
		{
			float sqrDistance;
			if (!m_horizontalOnly)
			{
				sqrDistance = (actor.transform.position - position).sqrMagnitude;
			}
			else
			{
				var position2D = new Vector2(position.x, position.z);
				var actorPosition2D = new Vector2(actor.transform.position.x, actor.transform.position.z);
				sqrDistance = (actorPosition2D - position2D).sqrMagnitude;
			}

			return MathUtil.GetPercent(sqrDistance, minDistance * minDistance, maxDistance * maxDistance);
		}

		#endregion
	}
}