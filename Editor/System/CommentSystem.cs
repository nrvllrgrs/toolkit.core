using UnityEngine;
using UnityEditor;
using ToolkitEngine;

namespace ToolkitEditor
{
	[InitializeOnLoad]
	public static class CommentSystem
	{
		#region Fields

		private static Texture s_icon;
		private static bool s_isOn;

		public const string ACTIVE_SAVE_VAR = "CommentSystem.isOn";

		#endregion

		#region Properties

		public static bool isOn
		{
			get => s_isOn;
			set
			{
				// No change, skip
				if (s_isOn == value)
					return;

				s_isOn = value;
				EditorPrefs.SetBool(ACTIVE_SAVE_VAR, value);
			}
		}

		#endregion

		#region Constructors

		static CommentSystem()
		{
			s_icon = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath("98bdf8483626f6d40afc368adbb5c7dd"));
			SceneView.duringSceneGui += SceneView_DuringSceneGui;
		}

		private static void SceneView_DuringSceneGui(SceneView sceneView)
		{
			if (!isOn)
				return;

			var comments = Object.FindObjectsOfType<Comment>();
			foreach (var comment in comments)
			{
				if (!comment.showInScene || string.IsNullOrWhiteSpace(comment.text))
					continue;

				Handles.Label(comment.transform.position, new GUIContent(s_icon, comment.text));
			}
		}

		#endregion
	}
}