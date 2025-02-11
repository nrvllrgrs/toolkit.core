using UnityEngine;
using static ToolkitEngine.AnimationSet;

namespace ToolkitEngine
{
    public class AnimationSetOverride : MonoBehaviour
    {
		#region Fields

		[SerializeField]
		private AnimationClipMap m_clips;

		#endregion

		#region Methods

		public bool HasClip(string key) => m_clips.ContainsKey(key);
		public AnimationClip GetClip(string key) => m_clips[key];
		public bool TryGetClip(string key, out AnimationClip clip) => m_clips.TryGetValue(key, out clip);

		#endregion
	}
}