using System;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
    [UnitTitle("On Performed")]
    public class OnInputPerformed : BaseInputEventUnit
	{
        public override Type MessageListenerType => typeof(OnInputPerformedMessageListener);
    }
}