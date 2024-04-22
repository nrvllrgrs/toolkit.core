using System.Reflection;
using UnityEngine;

namespace ToolkitEngine
{
	public abstract class UnityValue
	{
		#region Enumerators

		public enum ValueType
		{
			Value,
			Member,
		}

		#endregion

		#region Fields

		[SerializeField]
		protected ValueType m_type;

		#endregion
	}

	[System.Serializable]
	public abstract class UnityValue<T> : UnityValue
	{
		#region Fields

		[SerializeField]
		private T m_value;

		[SerializeField]
		private Object m_object;

		[SerializeField]
		private Component m_component;

		[SerializeField]
		private string m_memberName;

		[SerializeField]
		private bool m_isProperty;

		private MemberInfo m_memberInfo;

		#endregion

		#region Properties

		public T value
		{
			get
			{
				switch (m_type)
				{
					case ValueType.Value:
						return m_value;

					case ValueType.Member:
						InitMemberInfo();
						if (m_memberInfo != null)
						{
							return m_memberInfo.GetMemberValue<T>(m_component);
						}
						break;
				}
				return default;
			}
			set
			{
				switch (m_type)
				{
					case ValueType.Value:
						m_value = value;
						return;

					case ValueType.Member:
						InitMemberInfo();
						if (m_memberInfo != null)
						{
							m_memberInfo.SetMemberValue(m_component, value);
						}
						return;
				}
			}
		}

		#endregion

		#region Construtors

		public UnityValue(T value)
		{
			m_value = value;
		}

		#endregion

		#region Methods

		private void InitMemberInfo()
		{
			if (m_memberInfo == null)
			{
				m_memberInfo = !m_isProperty
					? m_component.GetType().GetField(m_memberName, BindingFlags.Public | BindingFlags.Instance)
					: m_component.GetType().GetProperty(m_memberName, BindingFlags.Public | BindingFlags.Instance);
			}
		}

		#endregion
	}
}