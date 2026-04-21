using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

namespace ToolkitEngine
{
    public class MusicManager : ConfigurableSubsystem<MusicManager, MusicManagerConfig>
    {
		#region Fields

		private AudioSource[] m_sources = new AudioSource[2];
		private int m_activeSourceIndex = 0;

		private CancellationTokenSource crossfadeCts;

		#endregion

		#region Properties

		private static AudioSource activeSource => CastInstance.m_sources[CastInstance.m_activeSourceIndex];

		#endregion

		#region Methods

		protected override void Initialize()
		{
			base.Initialize();

			var obj = new GameObject("Music Tracks");
			Object.DontDestroyOnLoad(obj);

			for (int i = 0; i < 2; i++)
			{
				m_sources[i] = obj.AddComponent<AudioSource>();
				m_sources[i].loop = true;
				m_sources[i].priority = 0;
				m_sources[i].outputAudioMixerGroup = Config.musicMixerGroup;
			}

			if (Config.defaultMusic != null)
			{
				Play(Config.defaultMusic);
			}
		}

		protected override void Terminate()
		{
			CancelCurrent();
			m_sources[0].DOKill();
			m_sources[1].DOKill();

			// Cleanup "Music Tracks" gameObject
			Object.Destroy(m_sources[0].gameObject);
		}

		public static void Play(AudioClip clip, float? fadeDuration = null)
		{
			// If the same track is already playing, do nothing
			if (activeSource.clip == clip && activeSource.isPlaying)
				return;

			CancelCurrent();
			CastInstance.crossfadeCts = new CancellationTokenSource();
			CrossfadeAsync(clip, fadeDuration ?? Config.defaultFadeDuration, CastInstance.crossfadeCts.Token).Forget();
		}

		public static void Stop(float? fadeDuration = null)
		{
			CancelCurrent();
			CastInstance.crossfadeCts = new CancellationTokenSource();
			FadeOutAsync(activeSource, fadeDuration ?? Config.defaultFadeDuration, CastInstance.crossfadeCts.Token).Forget();
		}

		// Awaitable version for callers that need to know when the transition finishes
		public static async UniTask PlayAsync(AudioClip clip, float? fadeDuration = null, CancellationToken externalCt = default)
		{
			if (activeSource.clip == clip && activeSource.isPlaying)
				return;

			CancelCurrent();
			CastInstance.crossfadeCts = new CancellationTokenSource();

			// Link our internal token with any external one (e.g. from a scene loading system)
			var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(CastInstance.crossfadeCts.Token, externalCt);

			await CrossfadeAsync(clip, fadeDuration ?? Config.defaultFadeDuration, linkedCts.Token);
		}

		// --- Internal Logic ---

		private static async UniTask CrossfadeAsync(AudioClip newClip, float duration, CancellationToken ct)
		{
			int incomingIndex = 1 - CastInstance.m_activeSourceIndex;
			var incomingSource = CastInstance.m_sources[incomingIndex];

			incomingSource.clip = newClip;
			incomingSource.volume = 0f;
			incomingSource.Play();

			// DOTween handles the actual volume animation
			// Kill any lingering tweens on these sources first
			activeSource.DOKill();
			incomingSource.DOKill();

			activeSource.DOFade(0f, duration)
				.SetEase(Ease.InOutSine)
				.SetUpdate(true);
			incomingSource.DOFade(1f, duration)
				.SetEase(Ease.InOutSine)
				.SetUpdate(true);

			// UniTask waits for the fade to finish, respecting cancellation
			await UniTask.Delay(
				System.TimeSpan.FromSeconds(duration),
				ignoreTimeScale: true,
				cancellationToken: ct
			);

			// Cleanup after successful completion (not cancelled)
			activeSource.Stop();
			activeSource.clip = null;
			CastInstance.m_activeSourceIndex = incomingIndex;
		}

		private static async UniTask FadeOutAsync(AudioSource source, float duration, CancellationToken ct)
		{
			source.DOKill();
			source.DOFade(0f, duration)
				.SetEase(Ease.InOutSine)
				.SetUpdate(true);

			await UniTask.Delay(
				System.TimeSpan.FromSeconds(duration),
				ignoreTimeScale: true,
				cancellationToken: ct
			);

			source.Stop();
			source.volume = 1f; // reset for next use
		}

		private static void CancelCurrent()
		{
			if (CastInstance.crossfadeCts == null)
				return;

			// Kill DOTween tweens immediately when cancelled so volume doesn't hang mid-fade
			CastInstance.m_sources[0].DOKill();
			CastInstance.m_sources[1].DOKill();

			CastInstance.crossfadeCts.Cancel();
			CastInstance.crossfadeCts.Dispose();
			CastInstance.crossfadeCts = null;
		}

		#endregion
	}
}