using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ToolkitEditor
{
	public static class AssetUtility
    {
		#region Methods

		public static T[] GetAssetsOfType<T>()
			where T : Object
		{
			// Get assets in database
			var guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T).Name));
			var assets = new List<T>();

			foreach (var guid in guids)
			{
				assets.Add(AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)));
			}

			return assets.ToArray();
		}

		#endregion
	}
}