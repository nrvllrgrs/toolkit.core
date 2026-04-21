using Unity.VisualScripting;
using UnityEngine.InputSystem;

namespace ToolkitEngine.VisualScripting
{
	[UnitCategory("Events/Input")]
	public abstract class BaseInputEventUnit : BaseEventUnit<InputAction.CallbackContext>
    { }
}