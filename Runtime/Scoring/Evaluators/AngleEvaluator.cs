using UnityEngine;

namespace ToolkitEngine.Scoring
{
	public class AngleEvaluator : BaseEvaluator
    {
		#region Fields

		[SerializeField, Tooltip("Origin of the angle check. If empty, use this gameObject's transform.")]
		private Transform m_origin;

		[SerializeField, MinMax(0f, 180f), Tooltip("Minimum and maximum angles (degrees) relative to gameObject's forward vector. Value less than minimum returns 0; value greater than maximum returns 1.")]
		private Vector2 m_range = new Vector2(0f, 60f);

		[SerializeField, Tooltip("Indicates whether angle should only be evaluated on X-Z plane.")]
		private bool m_horizontalOnly = false;

		#endregion

		#region Properties

		public float minAngle => m_range.x;
		public float maxAngle => m_range.y;

		#endregion

		#region Methods

		protected override float CalculateNormalizedScore(GameObject actor, GameObject target, Vector3 position)
		{
			var origin = GetOrigin(actor, ref m_origin);

			float angle;
			if (!m_horizontalOnly)
			{
				angle = Vector3.Angle(origin.forward, position - origin.position);
			}
			else
			{
				var forward2D = new Vector2(origin.forward.x, origin.forward.z);
				var position2D = new Vector2(position.x, position.z);
				var actorPosition2D = new Vector2(origin.position.x, origin.position.z);
				angle = Vector2.Angle(forward2D, position2D - actorPosition2D);
			}

			return MathUtil.GetPercent(angle, minAngle, maxAngle);
		}

		#endregion
	}
}