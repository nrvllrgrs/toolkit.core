using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine
{
	public class TagFilter : BaseFilter
	{
		#region Fields

		[SerializeField, Tag, Tooltip("List of included tags. If empty, ALL are included.")]
		private List<string> m_includedTags = new();

		[SerializeField, Tag, Tooltip("List of excluded tags. If empty, NONE are excluded.")]
		private List<string> m_excludedTags = new();

		#endregion

		#region Methods

		protected override bool IsIncluded(GameObject actor, GameObject target, Vector3 position)
		{
			if (m_excludedTags.Contains(target.tag))
				return false;

			if (m_includedTags.Count > 0)
				return m_includedTags.Contains(target.tag);

			return true;
		}

		#endregion
	}
}