using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ToolkitEditor
{
	public static class AssetUtil
    {
		#region Methods

		public static string GetGUID(Object asset)
		{
			return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset));
		}

		public static string GetFirstAssetPath<T>(string name)
			where T : Object
		{
			var guid = AssetDatabase.FindAssets($"t:{typeof(T).Name} {name}").FirstOrDefault();
			return guid != null
				? AssetDatabase.GUIDToAssetPath(guid)
				: null;
		}

		public static T LoadFirstAsset<T>(string name)
			where T : Object
		{
			var path = GetFirstAssetPath<T>(name);
			return path != null
				? AssetDatabase.LoadAssetAtPath<T>(path)
				: null;
		}

		public static T LoadAsset<T>(Object asset)
			where T : Object
		{
			return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GetAssetPath(asset));
		}

		public static T LoadAssetByGUID<T>(string guid)
			where T : Object
		{
			return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
		}

		public static AssetImporter LoadImporter(Object asset)
		{
			return AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(asset));
		}

		public static AssetImporter LoadImporter(string guid)
		{
			return AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid));
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

		public static IEnumerable<T> GetAssetsOfType<T>()
			where T : Object
		{
			return AssetDatabase.FindAssets($"t:{typeof(T).Name} a:assets")
				.Select(x => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(x)));
		}

		public static void SaveSubAsset(Object target)
		{
			if (!AssetDatabase.IsSubAsset(target))
				return;

			var path = AssetDatabase.GetAssetPath(target);
			var assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
			foreach (var asset in assets)
			{
				if (asset != target)
					continue;

				LoadImporter(target)?.SaveAndReimport();
			}
		}


		#endregion
	}
}