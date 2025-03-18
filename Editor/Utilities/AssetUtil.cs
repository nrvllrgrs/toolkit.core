using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ToolkitEditor
{
	public static class AssetUtil
    {
		#region Methods

		public static T LoadAsset<T>(string name)
			where T : Object
		{
			var guid = AssetDatabase.FindAssets($"t:{typeof(T).Name} {name}").FirstOrDefault();
			return guid != null
				? AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid))
				: null;
		}

		public static T LoadAsset<T>(Object asset)
			where T : Object
		{
			return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GetAssetPath(asset));
		}

		public static T LoadImporter<T>(Object asset)
			where T : AssetImporter
		{
			return AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(asset)) as T;
		}

		public static T LoadImporter<T>(string guid)
			where T : AssetImporter
		{
			return AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid)) as T;
		}

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