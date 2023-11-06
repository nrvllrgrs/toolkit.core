using UnityEngine;
using UnityEngine.Rendering;

namespace ToolkitEngine.Rendering
{
    public class FloatMaterialModifier : BaseMaterialModifier<float>
    {
        #region Properties

#if UNITY_EDITOR
        public override ShaderPropertyType ShaderPropertyType => ShaderPropertyType.Float;
#endif

        #endregion

        #region Methods

        protected override float GetValue(float t)
        {
            return t;
        }

        protected override void Set(Material material, float value)
        {
#if UNITY_EDITOR
            material.SetFloat(m_propertyName, value);
#else
            material.SetFloat(m_nameId, value);
#endif

        }

        #endregion
    }
}