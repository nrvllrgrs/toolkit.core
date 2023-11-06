using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ToolkitEngine
{
	[AddComponentMenu("Spawner/Pre-Spawn Rules/NavMesh Pre-Spawn Rule")]
	public class NavMeshPreSpawnRule : BasePreSpawnRule
	{
		#region Fields

		[SerializeField, Min(0f)]
		private float m_sampleDistance = 20f;

		[SerializeField, NavMeshAreaMask]
		private int m_areaMask = ~0;

		[SerializeField]
		private string m_agentType = "Humanoid";

		private static Dictionary<string, int> s_agentTypeMap;

		#endregion

		#region Methods

		public override bool Process(ObjectSpawner spawner, GameObject template, ref Vector3 position, ref Quaternion rotation)
		{
			NavMeshQueryFilter filter = new NavMeshQueryFilter()
			{
				agentTypeID = GetAgentTypeId(m_agentType),
				areaMask = m_areaMask,
			};

			if (NavMesh.SamplePosition(position, out NavMeshHit hit, m_sampleDistance, filter))
			{
				position = hit.position;
				return true;
			}

			Debug.LogFormat("Cannot spawn object! Valid NavMesh not found within range of {0}!", position);
			return false;
		}

		private static int GetAgentTypeId(string agentType)
		{
			if (string.IsNullOrWhiteSpace(agentType))
				return 0;

			if (s_agentTypeMap == null)
			{
				s_agentTypeMap = new Dictionary<string, int>();

				int count = NavMesh.GetSettingsCount();
				for (int i = 0; i < count; ++i)
				{
					int id = NavMesh.GetSettingsByIndex(i).agentTypeID;
					string name = NavMesh.GetSettingsNameFromID(id);
					s_agentTypeMap.Add(name, id);
				}
			}

			return s_agentTypeMap.TryGetValue(agentType, out int value)
				? value
				: 0;
		}

		#endregion
	}
}