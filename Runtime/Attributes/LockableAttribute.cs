using UnityEngine;

namespace ToolkitEngine
{
	/// <summary>
	/// Displays a field in the Inspector with a lock toggle button.
	/// When locked (default), the field is read-only. When unlocked, it is editable.
	/// </summary>
	public class LockableAttribute : PropertyAttribute
	{ }
}