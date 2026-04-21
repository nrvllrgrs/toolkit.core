using UnityEngine.Audio;

namespace ToolkitEngine
{
	[GenericMenuCategory("Audio")]
	public class AudioResourceProperty : ObjectProperty<AudioResource>
    {
		public AudioResourceProperty()
			: base() { }

		public AudioResourceProperty(AudioResource value)
			: base(value) { }
	}

	[GenericMenuCategory("Audio")]
	public class AudioMixerGroupProperty : ObjectProperty<AudioMixerGroup>
	{
		public AudioMixerGroupProperty()
			: base() { }

		public AudioMixerGroupProperty(AudioMixerGroup value)
			: base(value) { }
	}
}