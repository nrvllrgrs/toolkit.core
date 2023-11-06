using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine
{
    public class LabelFilter : BaseFilter
    {
		#region Fields

		[SerializeField, Tooltip("List of included labels. If empty, ALL are included.")]
		private List<string> m_includedLabels = new();

		[SerializeField, Tooltip("List of excluded labels. If empty, NONE are excluded.")]
		private List<string> m_excludedLabels = new();

		#endregion

		#region Methods

		protected override bool IsIncluded(GameObject actor, GameObject target, Vector3 position)
		{
			if (m_excludedLabels.Contains(target.tag))
				return false;

			if (m_includedLabels.Count > 0)
				return m_includedLabels.Contains(target.tag);

			return true;
		}

		#endregion
	}
}