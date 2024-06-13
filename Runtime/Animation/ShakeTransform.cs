using System;
using DG.Tweening;
using UnityEngine;

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

		#endregion

		#region Methods

		private void Awake()
		{
			if (m_transform == null)
			{
				m_transform = transform;
			}
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

		[ContextMenu("Shake")]
		public void Shake()
		{
			if (m_blockers.isTrueAndEnabled)
				return;

			m_positionSettings.Shake(m_transform);
			m_rotationSettings.Shake(m_transform);
			m_scaleSettings.Shake(m_transform);
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

		private float GetDuration(BaseSettings settings, float fallback) => settings.shake ? settings.duration : fallback;

		#endregion

		#region Structures

		[Serializable]
		protected abstract class BaseSettings
		{
			#region Fields

			[SerializeField]
			protected bool m_shake = false;

			[SerializeField, MaxInfinity]
			protected float m_duration = 1f;

			[SerializeField]
			protected Vector3 m_strength = Vector3.one;

			[SerializeField, Min(0)]
			protected int m_vibrato = 10;

			[SerializeField, Range(0f, 180f)]
			protected float m_randomness = 90f;

			[SerializeField]
			protected bool m_fadeOut = true;

			[SerializeField]
			protected ShakeRandomnessMode m_randomnessMode = ShakeRandomnessMode.Full;

			protected Tweener m_tweener = null;
			protected bool m_killed = false;

			#endregion

			#region Properties

			public bool shake => m_shake;
			public float duration => m_duration;

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

				if (m_duration < float.PositiveInfinity)
				{
					m_tweener = transform.DOShakePosition(m_duration, m_strength, m_vibrato, m_randomness, m_snapping, m_fadeOut, m_randomnessMode);
				}
				else
				{
					m_tweener = transform.DOShakePosition(1f, m_strength, m_vibrato, m_randomness, m_snapping, m_fadeOut, m_randomnessMode)
						.OnComplete(() => AttemptShake(transform));
				}
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

				if (m_duration < float.PositiveInfinity)
				{
					m_tweener = transform.DOShakeRotation(m_duration, m_strength, m_vibrato, m_randomness, m_fadeOut, m_randomnessMode);
				}
				else
				{
					m_tweener = transform.DOShakeRotation(1f, m_strength, m_vibrato, m_randomness, m_fadeOut, m_randomnessMode)
						.OnComplete(() => AttemptShake(transform));
				}
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

				if (m_duration < float.PositiveInfinity)
				{
					m_tweener = transform.DOShakeScale(m_duration, m_strength, m_vibrato, m_randomness, m_fadeOut, m_randomnessMode);
				}
				else
				{
					m_tweener = transform.DOShakeScale(1f, m_strength, m_vibrato, m_randomness, m_fadeOut, m_randomnessMode)
						.OnComplete(() => AttemptShake(transform));
				}
				return true;
			}

			#endregion
		}

		#endregion
	}
}