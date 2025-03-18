using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
    public class VisualScriptingManager : Subsystem<VisualScriptingManager>
    {
		#region Fields

		private Dictionary<string, HashSet<IMachine>> m_map = new();

		#endregion

		#region Methods

		public void Register(string key, IMachine machine)
		{
			if (!m_map.TryGetValue(key, out var set))
			{
				set = new HashSet<IMachine>();
				m_map.Add(key, set);
			}
			set.Add(machine);
		}

		public void Unregister(string key, IMachine machine)
		{
			if (!m_map.TryGetValue(key, out var set))
				return;

			set.Remove(machine);
			if (set.Count == 0)
			{
				m_map.Remove(key);
			}
		}

		public void Trigger(string key)
		{
			if (!m_map.TryGetValue(key, out var set))
				return;

			HashSet<GameObject> triggered = new();
			foreach (var machine in set)
			{
				if (triggered.Contains(machine.threadSafeGameObject))
					continue;

				EventBus.Trigger(key, machine.threadSafeGameObject);
				triggered.Add(machine.threadSafeGameObject);
			}
		}

		public static string GetEventHookName<T>(string key)
			where T : class
		{
			return $"{typeof(T).Name}:{key}";
		}

		#endregion
	}
}