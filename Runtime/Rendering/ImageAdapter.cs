using UnityEngine;
using UnityEngine.UI;

namespace ToolkitEngine.Rendering
{
	public class ImageAdapter : MonoBehaviour, ISpriteDisplay
	{
		[SerializeField]
		private Image m_image;

		public new bool enabled
		{
			get => m_image.enabled;
			set => base.enabled = m_image.enabled = value;
		}

		public Sprite sprite
		{
			get => m_image.sprite;
			set => m_image.sprite = value;
		}

		public Color color
		{
			get => m_image.color;
			set => m_image.color = value;
		}
	}
}
