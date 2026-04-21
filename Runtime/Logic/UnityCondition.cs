using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ToolkitEngine
{
	[System.Serializable]
	public class UnityCondition
	{
		#region Enumerators

		public enum ConditionType
		{
			All,
			Any,
		}

		public enum CompareOperatorType
		{
			Equal,
			NotEqual,
			GreaterThan,
			GreaterOrEqual,
			LessThan,
			LessOrEqual,
		}

		#endregion

		#region Fields

		[SerializeField]
		private List<Argument> m_arguments = new();

		private ConditionType m_conditionType;
		private bool m_validOnEmpty;

		#endregion

		#region Properties

		public bool isEmpty => m_arguments.Count == 0;
		public bool isTrue
		{
			get
			{
				if (isEmpty)
					return m_validOnEmpty;

				switch (m_conditionType)
				{
					case ConditionType.All:
						return m_arguments.All(x => x.isTrue);

					case ConditionType.Any:
						return m_arguments.Any(x => x.isTrue);
				}
				return false;
			}
		}

		public bool isTrueAndEnabled
		{
			get
			{
				if (isEmpty)
					return m_validOnEmpty;

				switch (m_conditionType)
				{
					case ConditionType.All:
						return m_arguments.All(x => x.isTrueAndEnabled);

					case ConditionType.Any:
						return m_arguments.Any(x => x.isTrueAndEnabled);
				}
				return false;
			}
		}

		public bool isTrueOrDisabled
		{
			get
			{
				if (isEmpty)
					return m_validOnEmpty;

				switch (m_conditionType)
				{
					case ConditionType.All:
						return m_arguments.All(x => x.isTrueOrDisabled);

					case ConditionType.Any:
						return m_arguments.Any(x => x.isTrueOrDisabled);
				}
				return false;
			}
		}

		#endregion

		#region Constructors

		public UnityCondition()
			: this(ConditionType.All)
		{ }

		public UnityCondition(ConditionType conditionType)
		{
			m_conditionType = conditionType;

			switch (m_conditionType)
			{
				case ConditionType.All:
					m_validOnEmpty = true;
					break;

				case ConditionType.Any:
					m_validOnEmpty = false;
					break;
			}
		}

		#endregion

		#region Structures

		[System.Serializable]
		internal class Argument
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
			private CompareOperatorType m_conditionType;

			[SerializeField]
			private bool m_boolArgument;

			[SerializeField]
			private int m_intArgument;

			[SerializeField]
			private float m_floatArgument;

			private MemberInfo m_memberInfo;

			#endregion

			#region Properties

			public bool isTrue
			{
				get
				{
					if (m_component == null || string.IsNullOrEmpty(m_memberName))
						return false;

					if (!TryGetMemberInfo(out var memberType, out var memberValue))
						return false;

					if (memberType == typeof(int))
						return Evaluate((int)memberValue, m_intArgument);

					if (memberType == typeof(float))
						return Evaluate((float)memberValue, m_floatArgument);

					if (memberType == typeof(bool))
						return Evaluate((bool)memberValue, m_boolArgument);

					return false;
				}
			}

			public bool isTrueAndEnabled
			{
				get
				{
					if (!TryGetBehaviour(out var behaviour))
						return false;

					return behaviour.enabled && isTrue;
				}
			}

			public bool isTrueOrDisabled
			{
				get
				{
					if (!TryGetBehaviour(out var behaviour))
						return false;

					return !behaviour.enabled || isTrue;
				}
			}

			#endregion

			#region Methods

			private bool TryGetMemberInfo(out System.Type memberType, out object memberValue)
			{
#if UNITY_EDITOR
				m_memberInfo = null;
#endif
				if (m_memberInfo == null)
				{
					m_memberInfo = !m_isProperty
						? m_component.GetType().GetField(m_memberName, BindingFlags.Public | BindingFlags.Instance)
						: m_component.GetType().GetProperty(m_memberName, BindingFlags.Public | BindingFlags.Instance);
				}

				memberType = default;
				memberValue = default;

				if (m_memberInfo == null)
					return false;

				if (m_memberInfo is FieldInfo fieldInfo)
				{
					memberType = fieldInfo.FieldType;
					memberValue = fieldInfo.GetValue(m_component);
				}
				else if (m_memberInfo is PropertyInfo propertyInfo)
				{
					memberType = propertyInfo.PropertyType;
					memberValue = propertyInfo.GetValue(m_component);
				}
				return true;
			}

			private bool TryGetBehaviour(out Behaviour behaviour)
			{
				behaviour = null;
				if (m_component == null)
					return false;

				behaviour = m_component as Behaviour;
				return behaviour != null;
			}

			private bool Evaluate(System.IComparable value, System.IComparable test)
			{
				switch (m_conditionType)
				{
					case CompareOperatorType.Equal:
						return value.CompareTo(test) == 0;

					case CompareOperatorType.NotEqual:
						return value.CompareTo(test) != 0;

					case CompareOperatorType.GreaterThan:
						return value.CompareTo(test) > 0;

					case CompareOperatorType.GreaterOrEqual:
						return value.CompareTo(test) >= 0;

					case CompareOperatorType.LessThan:
						return value.CompareTo(test) < 0;

					case CompareOperatorType.LessOrEqual:
						return value.CompareTo(test) <= 0;
				}

				return false;
			}

			#endregion
		}

		#endregion
	}
}
