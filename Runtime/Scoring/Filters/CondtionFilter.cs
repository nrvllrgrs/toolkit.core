using UnityEngine;

namespace ToolkitEngine
{
	public class ConditionFilter : BaseFilter
	{
		#region Fields

		[SerializeField]
		private UnityCondition m_condition;

		#endregion

		#region Methods

		protected override bool IsIncluded(GameObject actor, GameObject target, Vector3 position)
		{
			return m_condition.isTrueAndEnabled;
		}

		#endregion
	}
}