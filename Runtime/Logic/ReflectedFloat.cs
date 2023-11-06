using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine
{
    [System.Serializable]
    public class UnityFloat : UnityValue<float>
    {
        public UnityFloat(float value)
            : base(value)
        { }
    }
}