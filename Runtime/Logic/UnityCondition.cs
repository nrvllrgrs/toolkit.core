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

			#endregion

			#region Properties

			public bool isTrue
			{
				get
				{
					if (m_component == null || string.IsNullOrEmpty(m_memberName))
						return false;

					if (!m_isProperty)
					{
						var fieldInfo = m_component.GetType().GetField(m_memberName, BindingFlags.Public | BindingFlags.Instance);
						if (fieldInfo == null)
							return false;

						if (fieldInfo.FieldType == typeof(int))
							return Evaluate((int)fieldInfo.GetValue(m_component), m_intArgument);

						if (fieldInfo.FieldType == typeof(float))
							return Evaluate((float)fieldInfo.GetValue(m_component), m_floatArgument);

						if (fieldInfo.FieldType == typeof(bool))
							return Evaluate((bool)fieldInfo.GetValue(m_component), m_boolArgument);
					}
					else
					{
						var propertyInfo = m_component.GetType().GetProperty(m_memberName, BindingFlags.Public | BindingFlags.Instance);
						if (propertyInfo == null)
							return false;

						if (propertyInfo.PropertyType == typeof(int))
							return Evaluate((int)propertyInfo.GetValue(m_component), m_intArgument);

						if (propertyInfo.PropertyType == typeof(float))
							return Evaluate((float)propertyInfo.GetValue(m_component), m_floatArgument);

						if (propertyInfo.PropertyType == typeof(bool))
							return Evaluate((bool)propertyInfo.GetValue(m_component), m_boolArgument);
					}

					return false;
				}
			}

			public bool isTrueAndEnabled
			{
				get
				{
					if (m_component == null)
						return false;

					var behaviour = m_component as Behaviour;
					if (behaviour == null)
						return false;

					return behaviour.enabled && isTrue;
				}
			}

			#endregion

			#region Methods

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
