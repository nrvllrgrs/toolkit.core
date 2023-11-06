using UnityEngine;
using CompareOperatorType = ToolkitEngine.UnityCondition.CompareOperatorType;

namespace ToolkitEngine
{
	public abstract class BaseCompareFilter<T> : BaseFilter
		where T : System.IComparable
    {
		#region Fields

		[SerializeField]
		protected CompareOperatorType m_compareType;

		[SerializeField]
		protected T m_compareTo;

		#endregion

		#region Methods

		protected bool CompareTo(T value)
		{
			return CompareTo(m_compareTo);	
		}

		protected bool CompareTo(T value, T compareTo)
		{
			int c = value.CompareTo(compareTo);
			switch (m_compareType)
			{
				case CompareOperatorType.LessThan:
					return c < 0;

				case CompareOperatorType.LessOrEqual:
					return c <= 0;

				case CompareOperatorType.Equal:
					return c == 0;

				case CompareOperatorType.NotEqual:
					return c != 0;

				case CompareOperatorType.GreaterOrEqual:
					return c >= 0;

				case CompareOperatorType.GreaterThan:
					return c > 0;
			}

			return false;
		}

		#endregion
	}
}
