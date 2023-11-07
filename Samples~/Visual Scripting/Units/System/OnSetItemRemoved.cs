using System;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
    [UnitTitle("On Item Removed"), UnitSurtitle("Set")]
    public class OnSetItemRemove : BaseSetEventUnit
    {
        public override Type MessageListenerType => typeof(OnSetItemRemovedMessageListener);
    }
}