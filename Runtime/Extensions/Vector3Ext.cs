namespace UnityEngine
{
	public static class Vector3Ext
    {
		/// <summary>
		/// Inverts a scale vector by dividing 1 by each component
		/// </summary>
		public static Vector3 Inverse(this Vector3 v)
        {
            return new Vector3(1f / v.x, 1f / v.y, 1f / v.z);
        }

		/// <summary>
		/// Divides two vectors component-wise
		/// </summary>
		public static Vector3 InverseScale(this Vector3 a, Vector3 b)
		{
			return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
		}
    }
}