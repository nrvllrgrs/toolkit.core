using UnityEngine.Rendering;

namespace ToolkitEngine.Rendering
{
    public class RangeMaterialModifier : FloatMaterialModifier
    {
        #region Properties

#if UNITY_EDITOR
        public override ShaderPropertyType ShaderPropertyType => ShaderPropertyType.Range;
#endif

        #endregion
    }
}