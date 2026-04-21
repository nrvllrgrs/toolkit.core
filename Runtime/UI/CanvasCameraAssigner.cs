using UnityEngine;
using UnityEngine.SceneManagement;

namespace ToolkitEngine
{
	[RequireComponent(typeof(Canvas))]
	public class CanvasCameraAssigner : MonoBehaviour
	{
		#region Fields

		[SerializeField, Tag]
		private string m_tag = MAIN_CAMERA_TAG;

		private Canvas m_canvas;
		private const string MAIN_CAMERA_TAG = "MainCamera";

		#endregion

		#region Methods

		private void Awake()
		{
			m_canvas = GetComponent<Canvas>();
			SceneManager.sceneLoaded += OnSceneLoaded;
			AssignCamera();
		}

		private void OnDestroy()
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (mode == LoadSceneMode.Additive)
				return;

			AssignCamera();
		}

		private void AssignCamera()
		{
			if (m_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
				return;

			if (string.Equals(m_tag, MAIN_CAMERA_TAG))
			{
				m_canvas.worldCamera = Camera.main;
				return;
			}

			var cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
			foreach (var camera in cameras)
			{
				if (camera.CompareTag(m_tag))
				{
					m_canvas.worldCamera = camera;
					return;
				}
			}
		}

		#endregion
	}
}