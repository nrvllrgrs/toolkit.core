using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine
{
    public class ScriptMachinePostSpawnRule : BasePostSpawnRule
    {
        #region Fields

        [SerializeField]
        private ScriptMachine m_scriptMachine;

        [SerializeField]
        private string m_customEvent = "ProcessPostSpawn";

        #endregion

        #region Methods

        public override void Process(Transform transform, Spawner spawner, GameObject spawnedObject)
		{
            CustomEvent.Trigger(m_scriptMachine.gameObject, m_customEvent, transform, spawner, spawnedObject);
        }

        #endregion
    }
}