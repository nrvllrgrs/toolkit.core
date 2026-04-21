using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine
{
	public class ShakeTransform : MonoBehaviour
	{
		#region Fields

		[SerializeField]
		private Transform m_transform;

		[SerializeField]
		private bool m_shakeOnStart = false;

		[SerializeField]
		private UnityCondition m_blockers = new UnityCondition(UnityCondition.ConditionType.Any);

		[SerializeField]
		private PositionSettings m_positionSettings;

		[SerializeField]
		private RotationSettings m_rotationSettings;

		[SerializeField]
		private ScaleSettings m_scaleSettings;

		#endregion

		#region Events

		[SerializeField]
		private UnityEvent m_onShake;

		[SerializeField]
		private UnityEvent m_onCompleted;

		#endregion

		#region Properties

		public bool shaking => m_positionSettings.shaking || m_rotationSettings.shaking || m_scaleSettings.shaking;

		public float minDuration => Mathf.Min(
			GetDuration(m_positionSettings, float.PositiveInfinity),
			GetDuration(m_rotationSettings, float.PositiveInfinity),
			GetDuration(m_scaleSettings, float.PositiveInfinity));

		public float maxDuration => Mathf.Max(
			GetDuration(m_positionSettings, float.NegativeInfinity),
			GetDuration(m_rotationSettings, float.NegativeInfinity),
			GetDuration(m_scaleSettings, float.NegativeInfinity));

		public float duration
		{
			set
			{
				SetDuration(m_positionSettings, value);
				SetDuration(m_rotationSettings, value);
				SetDuration(m_scaleSettings, value);
			}
		}

		public UnityEvent onShake => m_onShake;
		public UnityEvent onCompleted => m_onCompleted;

		#endregion

		#region Methods

		private void Awake()
		{
			if (m_transform == null)
			{
				m_transform = transform;
			}

			m_positionSettings.Completed += Completed;
			m_rotationSettings.Completed += Completed;
			m_scaleSettings.Completed += Completed;
		}

		private void Start()
		{
			if (m_shakeOnStart)
			{
				Shake();
			}
		}

		private void OnDisable()
		{
			Kill();
		}

		private void OnDestroy()
		{
			m_positionSettings.Completed -= Completed;
			m_rotationSettings.Completed -= Completed;
			m_scaleSettings.Completed -= Completed;
		}

		[ContextMenu("Shake")]
		public void Shake()
		{
			if (m_blockers.isTrueAndEnabled)
				return;

			m_positionSettings.Shake(m_transform);
			m_rotationSettings.Shake(m_transform);
			m_scaleSettings.Shake(m_transform);
			m_onShake?.Invoke();
		}

		[ContextMenu("Kill")]
		public void Kill()
		{
			Kill(true);
		}

		public void Kill(bool complete)
		{
			m_positionSettings.Kill(complete);
			m_rotationSettings.Kill(complete);
			m_scaleSettings.Kill(complete);
		}

		private void Completed()
		{
			if (!shaking)
			{
				m_onCompleted?.Invoke();
			}
		}

		private float GetDuration(BaseSettings settings, float fallback) => settings.shake ? settings.duration : fallback;

		private void SetDuration(BaseSettings settings, float value)
		{
			if (!settings.shake)
				return;

			settings.duration = value;
		}

		#endregion

		#region Structures

		[Serializable]
		protected abstract class BaseSettings
		{
			#region Fields

			[SerializeField]
			protected bool m_shake = false;

			[SerializeField]
			protected bool m_continuous = false;

			[SerializeField, Min(0)]
			protected float m_duration = 1f;

			[SerializeField]
			protected Vector3 m_strength = Vector3.one;

			[SerializeField]
			protected ReflectedFloat m_strengthMultiplier = new ReflectedFloat(1f);

			[SerializeField, Min(0)]
			protected int m_vibrato = 10;

			[SerializeField]
			protected ReflectedInt m_vibratoMultiplier = new ReflectedInt(1);

			[SerializeField, Range(0f, 180f)]
			protected float m_randomness = 90f;

			[SerializeField]
			protected bool m_fadeOut = true;

			[SerializeField]
			protected ShakeRandomnessMode m_randomnessMode = ShakeRandomnessMode.Full;

			protected Tweener m_tweener = null;
			protected bool m_killed = false;

			#endregion

			#region Events

			public event Action Completed;

			#endregion

			#region Properties

			public bool shake => m_shake;
			public float duration { get => m_duration; set => m_duration = value; }
			public float intensity => m_strengthMultiplier.value;
			protected Vector3 scaledStrength => m_strength * intensity;
			protected int scaledVibrato => m_vibrato * m_vibratoMultiplier.value;

			public bool shaking => m_tweener?.IsPlaying() ?? false;

			#endregion

			#region Methods

			public abstract bool Shake(Transform transform);

			public void Kill(bool complete)
			{
				if (m_tweener == null)
					return;

				m_killed = true;
				m_tweener.Kill(complete);
			}

			protected void AttemptShake(Transform transform)
			{
				if (m_killed)
					return;

				Shake(transform);
			}

			protected void CheckContinuous(Transform transform)
			{
				if (m_continuous)
				{
					m_tweener.OnComplete(() => AttemptShake(transform));
				}
				else
				{
					m_tweener.OnComplete(() => Completed?.Invoke());
				}
			}

			#endregion
		}

		[Serializable]
		protected class PositionSettings : BaseSettings
		{
			#region Fields

			[SerializeField]
			protected bool m_snapping = false;

			#endregion

			#region Methods

			public override bool Shake(Transform transform)
			{
				if (!m_shake)
					return false;

				// Reset killed
				m_killed = false;

				m_tweener = transform.DOShakePosition(m_duration, scaledStrength, scaledVibrato, m_randomness, m_snapping, m_fadeOut, m_randomnessMode);
				CheckContinuous(transform);

				return true;
			}

			#endregion
		}

		[Serializable]
		protected class RotationSettings : BaseSettings
		{
			#region Methods

			public override bool Shake(Transform transform)
			{
				if (!m_shake)
					return false;

				// Reset killed
				m_killed = false;

				m_tweener = transform.DOShakeRotation(m_duration, scaledStrength, scaledVibrato, m_randomness, m_fadeOut, m_randomnessMode);
				CheckContinuous(transform);

				return true;
			}

			#endregion
		}

		[Serializable]
		protected class ScaleSettings : BaseSettings
		{
			#region Methods

			public override bool Shake(Transform transform)
			{
				if (!m_shake)
					return false;

				// Reset killed
				m_killed = false;

				m_tweener = transform.DOShakeScale(m_duration, scaledStrength, scaledVibrato, m_randomness, m_fadeOut, m_randomnessMode);
				CheckContinuous(transform);

				return true;
			}

			#endregion
		}

		#endregion
	}
}