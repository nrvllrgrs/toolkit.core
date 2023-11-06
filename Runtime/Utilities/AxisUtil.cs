using UnityEngine;

namespace ToolkitEngine
{
	public enum Axis
	{
		Right,
		Left,
		Up,
		Down,
		Forward,
		Back,
	}

	public static class AxisUtil
    {
		public static Vector3 GetDirection(Axis axis)
		{
			switch (axis)
			{
				case Axis.Right:
					return Vector3.right;

				case Axis.Left:
					return Vector3.left;

				case Axis.Up:
					return Vector3.up;

				case Axis.Down:
					return Vector3.down;

				case Axis.Forward:
					return Vector3.forward;

				case Axis.Back:
					return Vector3.back;
			}
			return Vector3.zero;
		}

		public static Vector3 GetDirection(this Transform transform, Axis axis)
		{
			switch (axis)
			{
				case Axis.Right:
					return transform.right;

				case Axis.Left:
					return -transform.right;

				case Axis.Up:
					return transform.up;

				case Axis.Down:
					return -transform.up;

				case Axis.Forward:
					return transform.forward;

				case Axis.Back:
					return -transform.forward;
			}
			return Vector3.zero;
		}
    }
}