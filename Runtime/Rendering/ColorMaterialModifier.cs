using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace ToolkitEngine.Rendering
{
	public class ColorMaterialModifier : BaseMaterialModifier<Color>
    {
		#region Enumerators

		public enum ColorMode
		{
			Standard,
			HDR,
			Vertex,
		}

		#endregion

		#region Fields

		[SerializeField]
        private ColorMode m_colorMode;

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
            return m_colorMode != ColorMode.HDR
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

		protected override bool SetGraphic(Graphic graphic, Color value)
		{
			if (m_colorMode != ColorMode.Vertex)
				return false;

			graphic.color = value;
			return true;
		}

		#endregion
	}
}