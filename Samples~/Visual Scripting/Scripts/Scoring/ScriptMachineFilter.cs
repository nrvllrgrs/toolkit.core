using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[System.Serializable]
	public class ScriptMachineFilter : BaseFilter
    {
		#region Fields

		[SerializeField]
		private ScriptMachine m_scriptMachine;

		[SerializeField]
		private string m_eventName;

		#endregion

		#region Properties

		public bool isIncluded { get; set; }

		#endregion

		#region Methods

		protected override bool IsIncluded(GameObject actor, GameObject target, Vector3 position)
		{
			if (string.IsNullOrWhiteSpace(m_eventName))
				return false;

			EventBus.Trigger(nameof(ScriptMachineFilter), m_scriptMachine.gameObject, new EventArgs(m_eventName, this, actor, target, position));
			return isIncluded;
		}

		#endregion

		#region Structures

		public class EventArgs : System.EventArgs
		{
			public string name { get; private set; }
			public ScriptMachineFilter filter { get; private set; }
			public GameObject actor { get; private set; }
			public GameObject target { get; private set; }
			public Vector3 position { get; private set; }

			public EventArgs(string name, ScriptMachineFilter filter, GameObject actor, GameObject target, Vector3 position)
			{
				this.name = name;
				this.filter = filter;
				this.actor = actor;
				this.target = target;
				this.position = position;
			}
		}

		#endregion
	}
}