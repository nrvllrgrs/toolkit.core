using UnityEngine;
using DG.Tweening;

namespace ToolkitEngine
{
    public static class AudioSourceExt
    {
		public static Tween DOFade(this AudioSource audioSource, float targetVolume, float duration)
		{
			return DOTween.To(
				() => audioSource.volume,
				x => audioSource.volume = x,
				targetVolume,
				duration
			).SetTarget(audioSource);
		}
	}
}