using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine
{
	public abstract class BaseRemapper<T> : MonoBehaviour
	{
		#region Fields

		[SerializeField]
		private Object m_object;

		[SerializeField]
		private Component m_component;

		[SerializeField]
		private string m_memberName;

		[SerializeField]
		private bool m_isProperty;

		[SerializeField]
		private AnimationCurve m_curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		[SerializeField]
		private float m_srcMinValue;

		[SerializeField]
		private float m_srcMaxValue;

		[SerializeField]
		protected T m_dstMinValue;

		[SerializeField]
		protected T m_dstMaxValue;

		private MemberInfo m_memberInfo = null; 

		#endregion

		#region Events

		[SerializeField]
		private UnityEvent<T> m_onRemapped;

		#endregion

		#region Methods

		private void Awake()
		{
			if (m_component != null && !string.IsNullOrWhiteSpace(m_memberName))
			{
				m_memberInfo = !m_isProperty
					? m_component.GetType().GetField(m_memberName, BindingFlags.Public | BindingFlags.Instance)
					: m_component.GetType().GetProperty(m_memberName, BindingFlags.Public | BindingFlags.Instance);
			}
		}

		public void Remap()
		{
			if (m_memberInfo == null)
				return;

			Remap((float)m_memberInfo.GetMemberValue(m_component));
		}

		public void Remap(float value)
		{
			if (m_onRemapped == null)
				return;

			var t = MathUtil.GetPercent(value, m_srcMinValue, m_srcMaxValue);
			t = m_curve.Evaluate(t);

			m_onRemapped.Invoke(GetValue(t));
		}

		protected abstract T GetValue(float t);

		#endregion
	}
}