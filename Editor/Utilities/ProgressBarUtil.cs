using UnityEditor;
using UnityEngine;

namespace ToolkitEditor
{
	public static class ProgressBarUtil
	{
		#region Fields

		private static bool s_showProgressBar = false;
		private static string s_title;
		private static string s_info;
		private static float s_progress;

		#endregion

		#region Methods

		public static void DisplayProgressBar(string title, string info, float progress)
		{
			s_title = title ?? string.Empty;
			s_info = info ?? string.Empty;
			s_progress = Mathf.Clamp01(progress);

			if (!s_showProgressBar)
			{
				s_showProgressBar = true;
				EditorApplication.update += Update;
			}
		}

		public static void ClearProgressBar()
		{
			s_showProgressBar = false;
		}

		public static void Update()
		{
			if (!s_showProgressBar)
			{
				EditorApplication.update -= Update;
				s_showProgressBar = false;

				// Must clear after unsubscribing to update
				EditorUtility.ClearProgressBar();
				return;
			}

			EditorUtility.DisplayProgressBar(s_title, s_info, s_progress);
		}

		#endregion
	}
}