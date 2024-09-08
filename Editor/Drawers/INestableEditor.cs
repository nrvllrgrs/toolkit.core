using UnityEngine;
using UnityEditor;

namespace ToolkitEditor
{
	public interface INestableEditor
    {
        float GetNestedHeight();
        void OnNestedGUI(ref Rect position);
    }
}