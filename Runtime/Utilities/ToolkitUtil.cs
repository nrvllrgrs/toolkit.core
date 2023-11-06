namespace ToolkitEngine
{
	public static class ToolkitUtil
    {
		public static bool IsNull(object value)
		{
			return value == null || value.Equals(null);
		}
	}
}