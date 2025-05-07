using UnityEngine;

namespace ToolkitEngine.Scoring
{
	public class PresetEvaluator : BaseEvaluator
	{
		#region Fields

		[SerializeField]
		private PresetEvaluable m_evaluable;

		#endregion

		#region Properties
#if UNITY_EDITOR
		public override bool showCurve => false;
#endif
		#endregion

		#region Methods

		protected override float CalculateNormalizedScore(GameObject actor, GameObject target, Vector3 position)
		{
			return m_evaluable?.Evaluate(actor, target, position) ?? 0f;
		}

		#endregion
	}
}