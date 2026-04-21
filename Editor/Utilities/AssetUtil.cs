using System.Collections.Generic;
using System.Drawing;
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

		public static string GetFirstAssetPath<T>(string name, DefaultAsset directory)
			where T : Object
		{
			var folderPath = AssetDatabase.GetAssetPath(directory);
			var guid = AssetDatabase.FindAssets($"t:{typeof(T).Name} {name}", new[] { folderPath }).FirstOrDefault();
			return guid != null
				? AssetDatabase.GUIDToAssetPath(guid)
				: null;
		}

		public static string GetFirstExactAssetPath<T>(string name)
			where T : Object
		{
			return AssetDatabase.FindAssets($"t:{typeof(T).Name} {name}")
				.Select(AssetDatabase.GUIDToAssetPath)
				.FirstOrDefault(path => System.IO.Path.GetFileNameWithoutExtension(path) == name);
		}

		public static string GetFirstExactAssetPath<T>(string name, DefaultAsset directory)
			where T : Object
		{
			var folderPath = AssetDatabase.GetAssetPath(directory);
			return AssetDatabase.FindAssets($"t:{typeof(T).Name} {name}", new[] { folderPath })
				.Select(AssetDatabase.GUIDToAssetPath)
				.FirstOrDefault(path => System.IO.Path.GetFileNameWithoutExtension(path) == name);
		}

		public static T LoadFirstAsset<T>(string name)
			where T : Object
		{
			var path = GetFirstAssetPath<T>(name);
			if (path == null)
				return null;

			var assets = AssetDatabase.LoadAllAssetsAtPath(path);
			return assets.FirstOrDefault(x => x.name == name) as T;
		}

		public static T LoadFirstAsset<T>(string name, DefaultAsset directory)
			where T : Object
		{
			var path = GetFirstAssetPath<T>(name, directory);
			if (path == null)
				return null;

			var assets = AssetDatabase.LoadAllAssetsAtPath(path);
			return assets.FirstOrDefault(x => x.name == name) as T;
		}

		public static T LoadFirstExactAsset<T>(string name)
			where T : Object
		{
			var path = GetFirstExactAssetPath<T>(name);
			if (path == null)
				return null;

			var assets = AssetDatabase.LoadAllAssetsAtPath(path);
			return assets.FirstOrDefault(x => x.name == name) as T;
		}

		public static T LoadFirstExactAsset<T>(string name, DefaultAsset directory)
			where T : Object
		{
			var path = GetFirstExactAssetPath<T>(name, directory);
			if (path == null)
				return null;

			var assets = AssetDatabase.LoadAllAssetsAtPath(path);
			return assets.FirstOrDefault(x => x.name == name) as T;
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

		public static string GetAssetPath(string path)
		{
			path = path.Replace("\\", "/");

			string projectPath = Application.dataPath.Replace("Assets", "");
			projectPath = projectPath.Replace("\\", "/");

			if (!path.StartsWith(projectPath))
			{
				Debug.LogError("Path is not inside this project.");
				return null;
			}

			return path.Substring(projectPath.Length);
		}

		public static string GetFullPath(Object asset)
		{
			return System.IO.Path.GetFullPath(AssetDatabase.GetAssetPath(asset));
		}

		public static string GetParentPath(Object asset)
		{
			string assetPath = AssetDatabase.GetAssetPath(asset);
			return System.IO.Path.GetDirectoryName(assetPath);
		}

		public static DefaultAsset GetParentAsset(Object asset)
		{
			return AssetDatabase.LoadAssetAtPath<DefaultAsset>(GetParentPath(asset));
		}

		public static void UpdatePrefabContent(Object obj, System.Action<GameObject> updater)
		{
			// Load prefab in edit mode
			string assetPath = AssetDatabase.GetAssetPath(obj);
			var prefab = PrefabUtility.LoadPrefabContents(assetPath);
			{
				updater?.Invoke(prefab);

				PrefabUtility.SaveAsPrefabAsset(prefab, assetPath);
				PrefabUtility.UnloadPrefabContents(prefab);
			}
		}

		#endregion
	}
}