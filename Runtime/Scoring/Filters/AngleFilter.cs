using UnityEngine;

namespace ToolkitEngine
{
    public class AngleFilter : BaseFilter
	{
		#region Fields

		[SerializeField, Range(0f, 180f), Tooltip("Maximum degrees from actor's forward vector.")]
		private float m_angle = 10f;

		#endregion

		#region Methods

		protected override bool IsIncluded(GameObject actor, GameObject target, Vector3 position)
		{
			float angle = Vector3.Angle(actor.transform.forward, position - actor.transform.position);
			return angle <= m_angle;
		}

		#endregion
	}
}