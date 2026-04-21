using UnityEngine;
using UnityEngine.Audio;

namespace ToolkitEngine
{
	[CreateAssetMenu(menuName = "Toolkit/Config/MusicManager Config", order = 10)]
	public class MusicManagerConfig : ScriptableObject
    {
		#region Fields

		[SerializeField]
		private AudioMixerGroup m_musicMixerGroup;

		[SerializeField]
		private float m_defaultFadeDuration = 1.5f;

		[SerializeField]
		private AudioClip m_defaultMusic = null;

		#endregion

		#region Properties

		public AudioMixerGroup musicMixerGroup => m_musicMixerGroup;
		public float defaultFadeDuration => m_defaultFadeDuration;
		public AudioClip defaultMusic => m_defaultMusic;

		#endregion
	}
}