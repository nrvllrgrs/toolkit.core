using Sirenix.Utilities;
using System.Reflection;
using UnityEngine;

namespace ToolkitEngine
{
	[System.Serializable]
	public abstract class UnityValue<T>
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
		private ValueType m_type;

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
						if (m_memberInfo == null)
						{
							m_memberInfo = !m_isProperty
								? m_component.GetType().GetField(m_memberName, BindingFlags.Public | BindingFlags.Instance)
								: m_component.GetType().GetProperty(m_memberName, BindingFlags.Public | BindingFlags.Instance);
						}

						if (m_memberInfo != null)
							return (T)m_memberInfo.GetMemberValue(m_component);

						break;
				}
				return default;
			}
		}

		#endregion

		#region Construtors

		public UnityValue(T value)
		{
			m_value = value;
		}

		#endregion
	}
}