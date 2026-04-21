using UnityEngine;
using UnityEngine.Rendering;

namespace ToolkitEngine.Rendering
{
	public class IntMaterialModifier : BaseMaterialModifier<int>
	{
		#region Fields

		[SerializeField]
		private int m_source = 0;

		[SerializeField]
		private int m_destination = 1;

		#endregion

		#region Properties

#if UNITY_EDITOR
		public override ShaderPropertyType ShaderPropertyType => ShaderPropertyType.Int;
#endif

		#endregion

		#region Methods

		protected override int GetValue(float t)
		{
			return (int)Mathf.Lerp(m_source, m_destination, t);
		}

		protected override void Set(Material material, int value)
		{
#if UNITY_EDITOR
			material.SetInt(m_propertyName, value);
#else
            material.SetInt(m_nameId, value);
#endif
		}

		#endregion
	}
}