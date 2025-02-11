using UnityEngine;

namespace ToolkitEngine
{
    [CreateAssetMenu(menuName = "Animation/Animation Set")]
    public class AnimationSet : ScriptableObject
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

		#region Structures

		[System.Serializable]
		public class AnimationClipMap : SerializableDictionary<string, AnimationClip>
		{ }

		#endregion
	}
}