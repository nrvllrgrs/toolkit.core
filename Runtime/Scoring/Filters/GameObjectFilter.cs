using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine
{
	public class GameObjectFilter : BaseFilter
	{
		#region Fields

		[SerializeField, Tooltip("List of included gameObject. If empty, ALL are included.")]
		private List<GameObject> m_includedObjects = new();

		[SerializeField, Tooltip("List of excluded gameObject. If empty, NONE are excluded.")]
		private List<GameObject> m_excludedObjects = new();

		#endregion

		#region Methods

		protected override bool IsIncluded(GameObject actor, GameObject target, Vector3 position)
		{
			if (m_excludedObjects.Contains(target))
				return false;

			if (m_includedObjects.Count > 0)
				return m_includedObjects.Contains(target);

			return true;
		}

		#endregion
	}
}