using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ToolkitEngine.Scoring
{
	public class LabelFilter : BaseFilter
    {
		#region Fields

		[SerializeField, Tooltip("List of included labels. If empty, ALL are included.")]
		private List<LabelType> m_includedLabels = new();

		[SerializeField, Tooltip("List of excluded labels. If empty, NONE are excluded.")]
		private List<LabelType> m_excludedLabels = new();

		#endregion

		#region Methods

		protected override bool IsIncluded(GameObject actor, GameObject target, Vector3 position)
		{
			if (target.TryGetComponent(out Labels labels))
			{
				var list = labels.Select(x => x);
				if (m_excludedLabels.Intersect(list).Any())
					return false;

				if (m_includedLabels.Count > 0)
					return m_includedLabels.Intersect(list).Any();

				return true;
			}

			return m_includedLabels.Count == 0;
		}

		#endregion
	}
}