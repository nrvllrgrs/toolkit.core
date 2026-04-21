using DG.Tweening;
using UnityEngine;
using TMPro;
using NaughtyAttributes;
using UnityEngine.Events;

namespace ToolkitEngine.UI
{
	public class TextMeshTicker : MonoBehaviour
    {
		#region Fields

		[SerializeField]
		private TMP_Text m_text;

		[SerializeField]
		private int m_defaultValue = 0;

		[SerializeField]
		private string m_format = "{0}";

		[SerializeField, Min(0f)]
		private float m_duration = 1f;

		[SerializeField]
		private Ease m_ease = Ease.OutQuad;

		private int m_value;
		private Tweener m_tweener;

		#endregion

		#region Properties

		public string format { get => m_format; set => m_format = value; }
		public int value { get => m_value; set => SetValue(value); }

		public bool isPlaying => m_tweener?.IsPlaying() ?? false;

		#endregion

		#region Events

		[SerializeField, Foldout("Events")]
		private UnityEvent<int> m_onValueChanged;

		[SerializeField, Foldout("Events")]
		private UnityEvent m_onCompleted;

		#endregion

		#region Methods

		private void Awake()
		{
			if (m_text == null)
			{
				m_text = GetComponent<TMP_Text>();
			}

			m_value = m_defaultValue;
			UpdateText(m_value);
		}

		private void OnDisable()
		{
			m_tweener?.Kill(false);
		}

		public void SetValue(int value, bool tick = true)
		{
			// No change, skip
			if (m_value == value)
				return;

			int prevValue = m_value;
			m_value = value;

			if (tick)
			{
				Tick(prevValue, value, m_duration);
			}
			else
			{
				UpdateText(value);
				m_onCompleted?.Invoke();
			}
		}

		private void Tick(int from, int to, float duration)
		{
			m_tweener?.Kill(false);

			m_value = from;
			UpdateText(m_value);
			m_onValueChanged?.Invoke(value);

			m_tweener = DOTween.To(
					() => m_value,
					x =>
					{
						m_value = x;
						UpdateText(x);
						m_onValueChanged?.Invoke(value);
					},
					to,
					duration)
				.SetEase(m_ease)
				.OnComplete(() => m_onCompleted?.Invoke());
		}

		public void Kill(bool complete = false)
		{
			m_tweener?.Kill(complete);
		}

		private void UpdateText(int value)
		{
			if (m_text == null)
				return;

			m_text.text = string.Format(m_format, value);
		}

		#endregion
	}
}