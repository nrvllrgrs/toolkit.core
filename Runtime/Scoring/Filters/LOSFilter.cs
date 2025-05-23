using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine.Scoring
{
	public class LOSFilter : BaseFilter
    {
		#region Fields

		[SerializeField, Tooltip("Origin of the LOS check. If empty, use this gameObject's transform.")]
		private Transform m_origin;

		[SerializeField]
		private LayerMask m_layers = ~0;

		[SerializeField]
		private QueryTriggerInteraction m_queryTrigger = QueryTriggerInteraction.Ignore;

		[SerializeField, Tooltip("Indicates whether target is required to have TargetingPoints component.")]
		private bool m_requireTargetingPoints = false;

		[SerializeField, Tooltip("Indicates whether collider can be a child of target.")]
		private bool m_includeChildren;

		[SerializeField, Tooltip("Colliders that are ignored for line-of-sight check.")]
		private Collider[] m_ignoredColliders;

		private HashSet<Collider> m_ignoredSet = null;

		#endregion

		#region Methods

		protected override bool IsIncluded(GameObject actor, GameObject target, Vector3 position)
		{
			if (m_ignoredSet == null)
			{
				m_ignoredSet = new HashSet<Collider>(m_ignoredColliders);
			}

			var targetingPoints = target.GetComponent<TargetingPoints>();
			if (targetingPoints == null)
			{
				if (m_requireTargetingPoints)
					return false;

				return HasLineOfSight(actor, target, position);
			}
			else
			{
				foreach (var point in targetingPoints)
				{
					if (HasLineOfSight(actor, target, point.position))
						return true;
				}
				return false;
			}
		}

		private bool HasLineOfSight(GameObject actor, GameObject target, Vector3 end)
		{
			UpdateOrigin(actor, ref m_origin);

			bool hasCharacterController = target.GetComponent<CharacterController>() != null;
			if (m_ignoredSet.Count == 0)
			{
				if (Physics.Linecast(m_origin.position, end, out RaycastHit hit, m_layers, m_queryTrigger))
					return CheckRaycastHit(hit, target);

				return hasCharacterController;
			}
			else
			{
				Vector3 direction = (end - m_origin.position).normalized;
				float distance = Vector3.Distance(m_origin.position, end);

				foreach (var hit in Physics.RaycastAll(m_origin.position, direction, distance, m_layers, m_queryTrigger))
				{
					// Ignored collier, skip
					if (m_ignoredSet.Contains(hit.collider))
						continue;

					return CheckRaycastHit(hit, target);
				}

				return hasCharacterController;
			}
		}

		private bool CheckRaycastHit(RaycastHit hit, GameObject target)
		{
			if (!m_includeChildren)
				return Equals(hit.collider.gameObject, target);

			return hit.collider.gameObject.IsDescendantOf(target);
		}

		#endregion
	}
}