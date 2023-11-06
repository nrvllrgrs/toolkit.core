using UnityEngine;

namespace ToolkitEngine
{
	public class Vector3Remapper : BaseRemapper<Vector3>
	{
		#region Methods

		protected override Vector3 GetValue(float t) => Vector3.Lerp(m_dstMinValue, m_dstMaxValue, t);

		#endregion
	}
}