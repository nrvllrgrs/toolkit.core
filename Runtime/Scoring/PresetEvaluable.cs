using UnityEngine;

namespace ToolkitEngine.Scoring
{
	[CreateAssetMenu(menuName = "Toolkit/Preset Evaluable")]
	public class PresetEvaluable : ScriptableObject
    {
		#region Fields

		[SerializeField]
		private UnityEvaluator m_evalator = new();

		#endregion

		#region Methods

		public float Evaluate(GameObject actor, GameObject target)
		{
			return m_evalator.Evaluate(actor, target);
		}

		public float Evaluate(GameObject actor, GameObject target, Vector3 position)
		{
			return m_evalator.Evaluate(actor, target, position);
		}

		#endregion
	}
}