using UnityEngine;

namespace ToolkitEngine
{
	[AddComponentMenu("Animation/Vector2 Remapper")]
	public class Vector2Remapper : BaseRemapper<Vector2>
	{
		#region Methods

		protected override Vector2 GetValue(float t) => Vector2.Lerp(m_dstMinValue, m_dstMaxValue, t);

		#endregion
	}
}