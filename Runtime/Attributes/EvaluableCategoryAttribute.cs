using System;

namespace ToolkitEngine
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class EvaluableCategoryAttribute : Attribute
	{
		public string category { get; private set; }

		public EvaluableCategoryAttribute(string category)
		{
			this.category = category;
		}
	}
}
