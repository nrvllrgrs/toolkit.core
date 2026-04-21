using UnityEngine;

namespace ToolkitEngine
{
	[RequireComponent(typeof(Canvas))]
	public class WorldCanvasScaler : MonoBehaviour
	{
		[Header("Reference Size (at base aspect ratio)")]

		[SerializeField]
		private int m_baseWidth = 1920;

		[SerializeField]
		private int m_baseHeight = 1080;

		private RectTransform m_rectTransform;
		private float m_baseAspect;

		private void Awake()
		{
			if (GetComponent<Canvas>().renderMode != RenderMode.WorldSpace)
			{
				Debug.LogWarning($"WorldCanvasScaler on '{name}' requires a World Space canvas. Disabling.", this);
				enabled = false;
				return;
			}

			m_rectTransform = GetComponent<RectTransform>();
			m_baseAspect = m_baseWidth / m_baseHeight;
			UpdateCanvasSize();
		}

		private void OnRectTransformDimensionsChange()
		{
			if (m_rectTransform != null)
			{
				UpdateCanvasSize();
			}
		}

		public void UpdateCanvasSize()
		{
			float aspect = (float)Screen.width / Screen.height;

			float width, height;
			if (aspect >= m_baseAspect)
			{
				// Wider than base: fix height, expand width
				height = m_baseHeight;
				width = m_baseHeight * aspect;
			}
			else
			{
				// Taller than base: fix width, expand height
				width = m_baseWidth;
				height = m_baseWidth / aspect;
			}

			m_rectTransform.sizeDelta = new Vector2(width, height);
		}
	}
}