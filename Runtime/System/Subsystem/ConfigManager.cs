using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ToolkitEngine
{
	public static class ConfigManager
    {
		#region Fields

		private static ConfigCollection s_collection;
		private static Dictionary<Type, ScriptableObject> s_map;

		#endregion

		#region Properties

		public static ScriptableObject[] configs => s_collection.configs;

		#endregion

		#region Methods

		internal static void Initialize()
		{
			s_collection = Resources.LoadAll<ConfigCollection>(string.Empty).FirstOrDefault();

			s_map = new();
			if (s_collection != null)
			{
				foreach (var config in s_collection.configs)
				{
					s_map.Add(config.GetType(), config);
				}
			}
		}

		public static bool TryGet<T>(out T value)
			where T : ScriptableObject
		{
			if (s_map.TryGetValue(typeof(T), out var config))
			{
				value = config as T;
				return true;
			}
			
			value = default;
			return false;
		}

		#endregion
	}
}