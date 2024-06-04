using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine
{
	[System.Serializable]
    public class UnityEvaluator
    {
		#region Fields

		[SerializeReference]
		private List<IEvaluable> m_evaluables = new();

		[SerializeField, Min(0f)]
		private float m_weight = 1f;

		#endregion

		#region Properties

		public float weight => m_weight;

#if UNITY_EDITOR
		internal List<IEvaluable> evaluables { get => m_evaluables; set => m_evaluables = value; }
#endif

		#endregion

		#region Methods

		public float Evaluate(GameObject actor, GameObject target)
		{
			return Evaluate(actor, target, target.transform.position);
		}

		public float Evaluate(GameObject actor, GameObject target, Vector3 position)
		{
			float score = m_weight;
			foreach (var evaluable in m_evaluables)
			{
  				if (!evaluable.enabled)
					continue;

				float s = evaluable.Evaluate(actor, target, position);
				if (evaluable is BaseFilter filter && filter.overrideOrSkip)
				{
					if (Equals(s, 0f))
						continue;

					score = evaluable.bonusWeight;
					break;
				}

				score *= s * evaluable.bonusWeight;

				if (Equals(score, 0f))
					break;
			}

			return GetCompensatedScore(score, m_evaluables.Count);
		}

		public static float GetCompensatedScore(float score, int count)
		{
			if (count > 0)
			{
				// High number of evaluables (even with high scores) can drive down score
				// Calculate compensation factor to adjust appropriately
				float compensationFactor = (1f - score) * (1f - (1f / count));
				score = score + (compensationFactor * score);
			}
			return score;
		}

		#endregion
	}
}