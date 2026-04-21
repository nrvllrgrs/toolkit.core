using UnityEngine;

namespace ToolkitEngine.Rendering
{
	public class SpriteRendererAdapter : MonoBehaviour, ISpriteDisplay
	{
		[SerializeField]
		private SpriteRenderer m_spriteRenderer;

		public new bool enabled
		{
			get => m_spriteRenderer.enabled;
			set => base.enabled = m_spriteRenderer.enabled = value;
		}

		public Sprite sprite
		{
			get => m_spriteRenderer.sprite;
			set => m_spriteRenderer.sprite = value;
		}

		public Color color
		{
			get => m_spriteRenderer.color;
			set => m_spriteRenderer.color = value;
		}
	}
}