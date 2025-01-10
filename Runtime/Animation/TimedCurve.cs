using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace ToolkitEngine
{
	public sealed class TimedCurve : MonoBehaviour
	{
		#region Fields

		[SerializeField]
		private AnimationCurve m_curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		[SerializeField, Min(0f)]
		private float m_duration = 1f;

		[SerializeField]
		private float m_amplitude = 1f;

		[SerializeField]
		private float m_verticalShift = 0f;

		[SerializeField]
		private bool m_playOnAwake;

		[SerializeField]
		private float m_time;

		private bool m_isPlaying;
		private TweenerCore<float, float, FloatOptions> m_tweener;

		#endregion

		#region Events

		public UnityEvent<TimedCurve> OnPlayed = new UnityEvent<TimedCurve>();
		public UnityEvent<TimedCurve> OnPaused = new UnityEvent<TimedCurve>();
		public UnityEvent<TimedCurve> OnTimeChanged = new UnityEvent<TimedCurve>();
		public UnityEvent<TimedCurve> OnBeginCompleted = new UnityEvent<TimedCurve>();
		public UnityEvent<TimedCurve> OnEndCompleted = new UnityEvent<TimedCurve>();

		public UnityEvent<float> OnValueChanged = new();

		#endregion

		#region Properties

		public bool isPlaying
		{
			get => m_isPlaying;
			private set
			{
				// No change, skip
				if (m_isPlaying == value)
					return;

				m_isPlaying = value;

				if (value)
				{
					OnPlayed.Invoke(this);
				}
				else
				{
					OnPaused.Invoke(this);
				}
			}
		}

		public float time
		{
			get => m_time;
			set
			{
				value = Mathf.Clamp(value, 0f, m_duration);

				// No change, skip
				if (m_time == value)
					return;

				m_time = value;
				OnTimeChanged.Invoke(this);
				OnValueChanged?.Invoke(this.value);

				if (time == m_duration)
				{
					tweener.fullPosition = tweener.endValue;

					if (isPlaying && !tweener.isBackwards)
					{
						Wrap(m_curve.postWrapMode);
						OnEndCompleted.Invoke(this);
					}
				}
				else if (time == 0)
				{
					tweener.fullPosition = 0f;

					if (isPlaying && tweener.isBackwards)
					{
						Wrap(m_curve.preWrapMode);
						OnBeginCompleted.Invoke(this);
					}
				}
			}
		}

		public float normalizedTime
		{
			get => m_time / m_duration;
			set
			{
				value = Mathf.Clamp01(value);

				// No change, skip
				if (value == tweener.fullPosition)
					return;

				tweener.fullPosition = value;
				time = m_duration * value;
			}
		}

		public float value => m_curve.Evaluate(normalizedTime) * m_amplitude + m_verticalShift;

		public float duration
		{
			get => m_duration;
			set
			{
				m_duration = value;
				tweener.timeScale = 1f / m_duration;
			}
		}

		public bool isBackwards => tweener.IsBackwards();

		private TweenerCore<float, float, FloatOptions> tweener
		{
			get
			{
				if (m_tweener == null)
				{
					m_tweener = DOTween.To(() => time, SetTweenerValue, 1f, 1f)
						.SetAutoKill(false);

					if (!m_isPlaying)
					{
						m_tweener.Pause();
					}
					m_tweener.timeScale = 1f / m_duration;
				}
				return m_tweener;
			}
		}

		#endregion

		#region Methods

		private void SetTweenerValue(float value)
		{
			if (!m_isPlaying)
				return;

			time = value * m_duration;
		}

		private void Awake()
		{
			if (m_playOnAwake)
			{
				Play();
			}
		}

		private void OnDestroy()
		{
			if (m_tweener != null)
			{
				m_tweener.Kill(false);
			}
		}

		public void Play()
		{
			tweener.Play();
			isPlaying = true;
		}

		public void PlayForward()
		{
			tweener.PlayForward();
			isPlaying = true;
		}

		public void PlayBackwards()
		{
			tweener.PlayBackwards();
			isPlaying = true;
		}

		public void Play(bool value)
		{
			if (value)
			{
				if (!isPlaying)
				{
					Play();
				}
			}
			else if (isPlaying)
			{
				Pause();
			}
		}

		public void Pause()
		{
			isPlaying = false;
			tweener.Pause();
		}
		
		public void Stop()
		{
			if (!isBackwards)
			{
				StopAtBegin();
			}
			else
			{
				StopAtEnd();
			}
		}

		public void StopAtBegin()
		{
			Pause();
			time = 0f;
		}

		public void StopAtEnd()
		{
			Pause();
			time = duration;
		}

		public void Restart()
		{
			time = 0f;
			isPlaying = true;
			tweener.Restart(false);
		}

		public void Reverse()
		{
			time = m_duration;
			isPlaying = true;
			tweener.PlayBackwards();
		}

		public void Flip()
		{
			tweener.Flip();
		}

		private void Wrap(WrapMode wrapMode)
		{
			switch (wrapMode)
			{
				case WrapMode.Loop:
					if (!tweener.isBackwards)
					{
						Restart();
					}
					else
					{
						Reverse();
					}
					break;

				case WrapMode.PingPong:
					Flip();
					tweener.Play();
					break;

				default:
					isPlaying = false;
					break;
			}
		}

		#endregion
	}
}