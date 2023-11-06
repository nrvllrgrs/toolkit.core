using UnityEngine;
using UnityEngine.Rendering;

namespace ToolkitEngine.Rendering
{
    public class ColorMaterialModifier : BaseMaterialModifier<Color>
    {
        #region Fields

        [SerializeField]
        private bool m_hdr;

        [SerializeField]
        private Color m_source = Color.white;

        [SerializeField]
        private Color m_destination;

		[SerializeField, ColorUsage(true, true)]
		private Color m_hdrSource = Color.white;

		[SerializeField, ColorUsage(true, true)]
		private Color m_hdrDestination;

		#endregion

		#region Properties

#if UNITY_EDITOR
		public override ShaderPropertyType ShaderPropertyType => ShaderPropertyType.Color;
#endif

        #endregion

        #region Methods

        protected override Color GetValue(float t)
        {
            return !m_hdr
                ? Color.Lerp(m_source, m_destination, t)
                : Color.Lerp(m_hdrSource, m_hdrDestination, t);
        }

        protected override void Set(Material material, Color value)
        {
#if UNITY_EDITOR
			material.SetColor(m_propertyName, value);
#else
            material.SetColor(m_nameId, value);
#endif

		}

        #endregion
    }
}