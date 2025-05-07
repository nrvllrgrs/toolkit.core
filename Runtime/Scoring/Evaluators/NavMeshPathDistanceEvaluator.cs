using UnityEngine;
using UnityEngine.AI;

namespace ToolkitEngine.Scoring
{
	public class NavMeshPathDistanceEvaluator : BaseEvaluator
    {
		#region Fields

		[SerializeField, MinMax(0f, float.PositiveInfinity)]
		private Vector2 m_range = new Vector2(0f, 100f);

		[SerializeField]
		private bool m_sampleActorPosition = true;

		[SerializeField]
		private bool m_sampleTargetPosition = true;

		[SerializeField, Min(0f)]
		private float m_maxSampleDistance = 5f;

		[SerializeField, NavMeshAreaMask]
		private int m_navMeshArea;

		#endregion

		#region Properties

		public float minDistance => m_range.x;
		public float maxDistance => m_range.y;

		#endregion

		#region Methods

		protected override float CalculateNormalizedScore(GameObject actor, GameObject target, Vector3 position)
		{
			var actorPosition = actor.transform.position;
			if (m_sampleActorPosition && NavMesh.SamplePosition(actorPosition, out NavMeshHit hit, m_maxSampleDistance, m_navMeshArea))
			{
				actorPosition = hit.position;
			}

			var targetPosition = position;
			if (m_sampleTargetPosition && NavMesh.SamplePosition(targetPosition, out hit, m_maxSampleDistance, m_navMeshArea))
			{
				targetPosition = hit.position;
			}

			NavMeshPath path = new NavMeshPath();
			if (NavMesh.CalculatePath(actorPosition, targetPosition, m_navMeshArea, path))
			{
				return MathUtil.GetPercent(path.GetDistance(), minDistance, maxDistance);
			}

			return 0f;
		}

		#endregion
	}
}