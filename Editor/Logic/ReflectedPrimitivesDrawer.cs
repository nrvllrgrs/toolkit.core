using UnityEditor;
using ToolkitEngine;

namespace ToolkitEditor
{
	[CustomPropertyDrawer(typeof(ReflectedInt))]
	public class ReflectedIntDrawer : ReflectedValueDrawer<int>
	{ }

	[CustomPropertyDrawer(typeof(ReflectedFloat))]
	public class ReflectedFloatDrawer : ReflectedValueDrawer<float>
	{ }
}