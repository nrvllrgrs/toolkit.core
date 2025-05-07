using System;
using UnityEngine;

namespace ToolkitEngine.Scoring
{
	[Serializable]
    public abstract class BaseEvaluator : IEvaluable
    {
		#region Fields

		[SerializeField, Tooltip("Indicates whether evaluator is processed.")]
		protected bool m_enabled = true;

		[SerializeField, Tooltip("Weight curve of this evaluator. Use this curve to calculate score.")]
		protected AnimationCurve m_curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		[SerializeField, Min(0f), Tooltip("Bonus weight multiplied to valid targets. Should only be used in rare circumstances.")]
		protected float m_bonusWeight = 1f;

		#endregion

		#region Properties

		public bool enabled { get => m_enabled; internal set => m_enabled = value; }
		public AnimationCurve curve { get => m_curve; internal set => m_curve = value; }

#if UNITY_EDITOR
		public virtual bool showCurve => true;
#endif

		public float bonusWeight => m_bonusWeight;

		#endregion

		#region Methods

		public virtual float Evaluate(GameObject actor, GameObject target)
		{
			return Evaluate(actor, target, target.transform.position);
		}

		public virtual float Evaluate(GameObject actor, GameObject target, Vector3 position)
		{
			return m_curve.Evaluate(CalculateNormalizedScore(actor, target, position));
		}

		protected abstract float CalculateNormalizedScore(GameObject actor, GameObject target, Vector3 position);

		protected static Transform GetOrigin(GameObject actor, ref Transform origin) => origin != null ? origin : actor.transform;

		#endregion
	}
}
