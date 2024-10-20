using System;
using UnityEngine;

namespace ToolkitEngine
{
	[Serializable]
	public abstract class BaseFilter : IEvaluable
    {
		#region Fields

		[SerializeField, Tooltip("Indicates whether evaluator is processed.")]
		protected bool m_enabled = true;

		[SerializeField, Tooltip("Indicates whether true condition forces score of 1; whereas a false condition is skipped.")]
		protected bool m_overrideOrSkip;

		[SerializeField, Tooltip("Indicates whether filter result is inverted.")]
		protected bool m_inverted;

		[SerializeField, Min(0f), Tooltip("Bonus weight multiplied to valid targets. Should only be used in rare circumstances.")]
		protected float m_bonusWeight = 1f;

		#endregion

		#region Properties

		public bool enabled { get => m_enabled; internal set => m_enabled = value; }
		public bool overrideOrSkip => m_overrideOrSkip;
		public float bonusWeight => m_bonusWeight;

		#endregion

		#region Methods

		public float Evaluate(GameObject actor, GameObject target)
		{
			return Evaluate(actor, target, target.transform.position);
		}

		public float Evaluate(GameObject actor, GameObject target, Vector3 position)
		{
			return Convert.ToSingle(IsIncluded(actor, target, position) != m_inverted);
		}

		protected abstract bool IsIncluded(GameObject actor, GameObject target, Vector3 position);

		protected static void UpdateOrigin(GameObject actor, ref Transform origin)
		{
			if (origin == null)
			{
				origin = actor.transform;
			}
		}

		#endregion
	}
}