using UnityEditor;
using UnityEngine;

namespace ToolkitEditor
{
	/// <summary>
	/// Enforces two rules whenever an <see cref="AppBootstrapperConfig"/> asset is
	/// created, renamed, or moved inside the Unity Editor:
	///
	///   1. The asset name must be exactly <see cref="ToolkitEngine.AppBootstrapperConfig.RESOURCE_NAME"/>
	///      so that Resources.Load can find it. If renamed, it is silently corrected.
	///
	///   2. The asset must live inside a folder named "Resources" somewhere in the
	///      project. If it doesn't, a warning is emitted (we don't auto-move it since
	///      the correct Resources folder is project-specific).
	/// </summary>
	internal class AppBootstrapperConfigPostprocessor : AssetPostprocessor
	{
		private static void OnPostprocessAllAssets(
			string[] importedAssets,
			string[] deletedAssets,
			string[] movedAssets,
			string[] movedFromAssetPaths)
		{
			// Check both newly imported and moved assets.
			foreach (var path in importedAssets)
			{
				Validate(path);
			}

			foreach (var path in movedAssets)
			{
				Validate(path);
			}
		}

		private static void Validate(string assetPath)
		{
			var obj = AssetDatabase.LoadAssetAtPath<ToolkitEngine.AppBootstrapperConfig>(assetPath);
			if (obj == null)
				return;

			EnforceName(assetPath, obj);
			WarnIfNotInResources(assetPath);
		}

		/// <summary>
		/// Renames the asset file back to <see cref="ToolkitEngine.AppBootstrapperConfig.RESOURCE_NAME"/>
		/// if it was changed, so Resources.Load always finds it.
		/// </summary>
		private static void EnforceName(string assetPath, Object obj)
		{
			if (obj.name == ToolkitEngine.AppBootstrapperConfig.RESOURCE_NAME) return;

			Debug.LogWarning(
				$"[AppBootstrapper] Config asset was renamed to '{obj.name}'. " +
				$"Reverting to '{ToolkitEngine.AppBootstrapperConfig.RESOURCE_NAME}' " +
				$"so Resources.Load can find it.",
				obj);

			AssetDatabase.RenameAsset(assetPath, ToolkitEngine.AppBootstrapperConfig.RESOURCE_NAME);
			AssetDatabase.SaveAssets();
		}

		/// <summary>
		/// Warns when the asset is not inside any Resources folder.
		/// We warn rather than auto-move because the correct Resources folder is
		/// project-specific.
		/// </summary>
		private static void WarnIfNotInResources(string assetPath)
		{
			if (IsInsideResourcesFolderPublic(assetPath))
				return;

			Debug.LogWarning(
				$"[AppBootstrapper] '{assetPath}' is not inside a Resources folder. " +
				$"Resources.Load will not find it at runtime. " +
				$"Move it to any folder named 'Resources'.");
		}

		internal static bool IsInsideResourcesFolderPublic(string assetPath)
		{
			// assetPath uses forward slashes: "Assets/Foo/Resources/AppBootstrapperConfig.asset"
			// Split and check for a segment literally named "Resources".
			var parts = assetPath.Split('/');
			for (int i = 0; i < parts.Length - 1; ++i) // exclude filename segment
			{
				if (parts[i] == "Resources")
					return true;
			}
			return false;
		}
	}

	/// <summary>
	/// Surfaces asset name / location issues directly in the Inspector so the
	/// problem is visible even if the postprocessor warning was missed.
	/// </summary>
	[CustomEditor(typeof(ToolkitEngine.AppBootstrapperConfig))]
	internal class AppBootstrapperConfigEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			var assetPath = AssetDatabase.GetAssetPath(target);

			if (target.name != ToolkitEngine.AppBootstrapperConfig.RESOURCE_NAME)
			{
				EditorGUILayout.HelpBox(
					$"This asset must be named '{ToolkitEngine.AppBootstrapperConfig.RESOURCE_NAME}'. " +
					$"Rename it or it will not be found at runtime.",
					MessageType.Error);
			}
			else if (!string.IsNullOrEmpty(assetPath) &&
					 !AppBootstrapperConfigPostprocessor.IsInsideResourcesFolderPublic(assetPath))
			{
				EditorGUILayout.HelpBox(
					"This asset is not inside a Resources folder and will not be " +
					"found at runtime. Move it to any folder named 'Resources'.",
					MessageType.Warning);
			}

			DrawDefaultInspector();
		}
	}
}