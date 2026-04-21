using System;

namespace ToolkitEngine
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class GenericMenuCategoryAttribute : Attribute
	{
		public string category { get; private set; }

		public GenericMenuCategoryAttribute(string category)
		{
			this.category = category;
		}
	}
}