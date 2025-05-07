using UnityEngine;

namespace ToolkitEngine.Scoring
{
	public class ColliderAngleFilter : BaseFilter
	{
		#region Fields

		[SerializeField, Range(0f, 180f), Tooltip("Maximum degrees from actor's forward vector.")]
		private float m_angle = 10f;

		[SerializeField, Min(0f), Tooltip("Maximum distance to scan for collider.")]
		private float m_distance = 1f;

		[SerializeField, Tooltip("Indicates whether child colliders should be considered.")]
		private bool m_includeChildren = false;

		#endregion

		#region Methods

		protected override bool IsIncluded(GameObject actor, GameObject target, Vector3 position)
		{
			var colliders = !m_includeChildren
				? target.GetComponents<Collider>()
				: target.GetComponentsInChildren<Collider>();

			foreach (var collider in colliders)
			{
				var scanDirection = Vector3.RotateTowards(
					actor.transform.forward,
					(collider.bounds.center - actor.transform.position).normalized,
					m_angle * Mathf.Deg2Rad,
					0f);

				var ray = new Ray(actor.transform.position, scanDirection);
				if (collider.Raycast(ray, out RaycastHit hit, m_distance))
					return true;

			}

			return false;
		}

		#endregion
	}
}