using Unity.VisualScripting;
using System;

namespace ToolkitEngine.VisualScripting
{
    [UnitTitle("On Empty"), UnitSurtitle("Set")]
    public class OnSetEmpty : BaseSetEventUnit
    {
        public override Type MessageListenerType => typeof(OnSetEmptyMessageListener);
    }
}