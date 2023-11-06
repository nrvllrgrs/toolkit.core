using UnityEngine;

namespace ToolkitEngine
{
	public class AngleEvaluator : BaseEvaluator
    {
		#region Fields

		[SerializeField, MinMax(0f, 180f), Tooltip("Minimum and maximum angles (degrees) relative to gameObject's forward vector. Value less than minimum returns 0; value greater than maximum returns 1.")]
		private Vector2 m_range = new Vector2(0f, 60f);

		#endregion

		#region Properties

		public float minAngle => m_range.x;
		public float maxAngle => m_range.y;

		#endregion

		#region Methods

		protected override float CalculateNormalizedScore(GameObject actor, GameObject target, Vector3 position)
		{
			float angle = Vector3.Angle(actor.transform.forward, position - actor.transform.position);
			return MathUtil.GetPercent(angle, minAngle, maxAngle);
		}

		#endregion
	}
}