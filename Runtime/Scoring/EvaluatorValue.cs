using UnityEngine;

namespace ToolkitEngine
{
	public sealed class EvaluatorValue : MonoBehaviour
    {
		#region Fields

		[SerializeField]
		private UnityEvaluator m_evaluator = new UnityEvaluator();

		[SerializeField]
		private GameObject m_target;

		#endregion

		#region Properties

		public bool isTrue => value != 0f;
		public float value => m_evaluator.Evaluate(gameObject, m_target);

		#endregion

		#region Methods

		private void Awake()
		{
			if (m_target == null)
			{
				m_target = gameObject;
			}
		}

		#endregion
	}
}