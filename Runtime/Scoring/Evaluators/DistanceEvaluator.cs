using UnityEngine;

namespace ToolkitEngine.Scoring
{
	public class DistanceEvaluator : BaseEvaluator
    {
		#region Fields

		[SerializeField, Tooltip("Origin of the distance check. If empty, use this gameObject's transform.")]
		private Transform m_origin;

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
			var origin = GetOrigin(actor, ref m_origin);

			float sqrDistance;
			if (!m_horizontalOnly)
			{
				sqrDistance = (origin.position - position).sqrMagnitude;
			}
			else
			{
				var position2D = new Vector2(position.x, position.z);
				var actorPosition2D = new Vector2(origin.position.x, origin.position.z);
				sqrDistance = (actorPosition2D - position2D).sqrMagnitude;
			}

			return MathUtil.GetPercent(sqrDistance, minDistance * minDistance, maxDistance * maxDistance);
		}

		#endregion
	}
}