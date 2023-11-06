using UnityEditor;
using ToolkitEngine;

namespace ToolkitEditor
{
	[CustomPropertyDrawer(typeof(UnityFloat))]
	public class UnityFloatDrawer : UnityValueDrawer<float>
    { }
}