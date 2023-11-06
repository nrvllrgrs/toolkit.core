using UnityEngine;

namespace ToolkitEngine
{
    public class FloatRemapper : BaseRemapper<float>
    {
        #region Methods

        protected override float GetValue(float t) => Mathf.Lerp(m_dstMinValue, m_dstMaxValue, t);

        #endregion
    }
}