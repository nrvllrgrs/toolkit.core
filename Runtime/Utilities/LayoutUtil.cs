using UnityEngine;
using UnityEngine.UI;

namespace ToolkitEngine
{
    public static class LayoutUtil
    {
        public static void ForceRebuildLayoutImmediateInChildren(RectTransform rectTransform)
        {
			foreach (var layoutGroup in rectTransform.GetComponentsInChildren<LayoutGroup>())
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
			}
		}

		public static void MarkLayoutForRebuildInChildren(RectTransform rectTransform)
		{
			foreach (var layoutGroup in rectTransform.GetComponentsInChildren<LayoutGroup>())
			{
				LayoutRebuilder.MarkLayoutForRebuild(layoutGroup.GetComponent<RectTransform>());
			}
		}
	}
}