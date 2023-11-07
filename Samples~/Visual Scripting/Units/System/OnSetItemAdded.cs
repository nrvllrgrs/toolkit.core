using System;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
    [UnitTitle("On Item Added"), UnitSurtitle("Set")]
    public class OnSetItemAdded : BaseSetEventUnit
    {
        public override Type MessageListenerType => typeof(OnSetItemAddedMessageListener);
    }
}