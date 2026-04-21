using System;
using Unity.VisualScripting;

namespace ToolkitEngine.VisualScripting
{
	[UnitTitle("On Canceled")]
	public class OnInputCanceled : BaseInputEventUnit
	{
		public override Type MessageListenerType => typeof(OnInputCanceledMessageListener);
	}
}