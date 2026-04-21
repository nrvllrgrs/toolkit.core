using UnityEngine;

namespace ToolkitEngine.Rendering
{
	public interface ISpriteDisplay
	{
		bool enabled { get; set; }
		Sprite sprite { get; set; }
		Color color { get; set; }
	}
}