using UnityEngine;
using Unity.VisualScripting;
using ToolkitEngine.Scoring;

namespace ToolkitEngine.VisualScripting
{
	[System.Serializable]
	public class ScriptMachineEvaluator : BaseEvaluator
    {
		#region Fields

		[SerializeField]
		private ScriptMachine m_scriptMachine;

		[SerializeField]
		private string m_eventName;

		#endregion

		#region Properties

		public float value { get; set; }

		#endregion

		#region Methods

		protected override float CalculateNormalizedScore(GameObject actor, GameObject target, Vector3 position)
		{
			if (string.IsNullOrWhiteSpace(m_eventName))
				return 0f;

			EventBus.Trigger(nameof(ScriptMachineEvaluator), m_scriptMachine.gameObject, new EventArgs(m_eventName, this, actor, target, position));
			return Mathf.Clamp01(value);
		}

		#endregion

		#region Structures

		public class EventArgs : System.EventArgs
		{
			public string name { get; private set; }
			public ScriptMachineEvaluator evaluator { get; private set; }
			public GameObject actor { get; private set; }
			public GameObject target { get; private set; }
			public Vector3 position { get; private set; }

			public EventArgs(string name, ScriptMachineEvaluator evaluator, GameObject actor, GameObject target, Vector3 position)
			{
				this.name = name;
				this.evaluator = evaluator;
				this.actor = actor;
				this.target = target;
				this.position = position;
			}
		}

		#endregion
	}
}