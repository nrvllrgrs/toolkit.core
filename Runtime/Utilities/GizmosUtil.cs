using UnityEngine;
using static UnityEngine.UI.Image;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ToolkitEngine
{
	public static class GizmosUtil
    {
		public static void DrawArrow(Vector3 position, Quaternion rotation, float scale = 1f)
		{
			DrawArrow(position, rotation, Color.green, scale);
		}

		public static void DrawArrow(Vector3 position, Quaternion rotation, Color color, float scale = 1f)
		{
#if UNITY_EDITOR
			Handles.color = color;
			Handles.DrawLines(new[]
			{
				position + rotation * (new Vector3(0f, 0f, 0.5f) * scale),
				position + rotation * (new Vector3(0.5f, 0f, 0.1667f) * scale),
				position + rotation * (new Vector3(0.25f, 0f, 0.1667f) * scale),
				position + rotation * (new Vector3(0.25f, 0f, -0.5f) * scale),
				position + rotation * (new Vector3(-0.25f, 0f, -0.5f) * scale),
				position + rotation * (new Vector3(-0.25f, 0f, 0.1667f) * scale),
				position + rotation * (new Vector3(-0.5f, 0f, 0.1667f) * scale),
			},
			new[] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 0 });

			// Draw UP vector
			Handles.DrawLine(position, position + rotation * Vector3.up * 0.25f * scale);
#endif
		}

		public static void DrawArrow(Transform transform, float scale = 1f)
		{
			DrawArrow(transform.position, transform.rotation, scale);
		}

		public static void DrawArrow(Transform transform, Color color, float scale = 1f)
		{
			DrawArrow(transform.position, transform.rotation, color, scale);
		}

		public static void DrawCone(Vector3 vertex, float angle, float height, Vector3 direction)
		{
#if UNITY_EDITOR
			var center = vertex + direction * height;
			var radius = Mathf.Tan(angle * 0.5f * Mathf.Deg2Rad) * height;

			Quaternion rotation = Quaternion.LookRotation(direction);
			Vector3 up = rotation * Vector3.up;
			Vector3 right = rotation * Vector3.right;

			Handles.DrawWireDisc(center, direction, radius);
			Gizmos.DrawLine(vertex, center + up * radius);
			Gizmos.DrawLine(vertex, center + -up * radius);
			Gizmos.DrawLine(vertex, center + right * radius);
			Gizmos.DrawLine(vertex, center + -right * radius);
#endif
		}

		public static void DrawCone(Vector3 vertex, float angle, float height, Vector3 direction, Color color)
		{
#if UNITY_EDITOR
			Gizmos.color = Handles.color = color;
			DrawCone(vertex, angle, height, direction);
#endif
		}

		public static void DrawCylinder(Transform transform, float radius, float height)
		{
			DrawCylinder(transform, radius, height, Color.white);
		}

		public static void DrawCylinder(Transform transform, float radius, float height, Color color)
		{
			DrawCylinder(transform.position, transform.up, radius, height, color);
		}

		public static void DrawCylinder(Vector3 origin, float radius, float height)
		{
			DrawCylinder(origin, radius, height, Color.white);
		}

		public static void DrawCylinder(Vector3 origin, float radius, float height, Color color)
		{
			DrawCylinder(origin, Vector3.up, radius, height , color);
		}

		private static void DrawCylinder(Vector3 origin, Vector3 up, float radius, float height, Color color)
		{
#if UNITY_EDITOR
			Gizmos.color = Handles.color = color;
			var offset = up * (height * 0.5f);
			Handles.DrawWireDisc(origin - offset, up, radius);
			Handles.DrawWireDisc(origin + offset, up, radius);

			Gizmos.DrawLine(origin - offset + Vector3.forward * radius, origin + offset + Vector3.forward * radius);
			Gizmos.DrawLine(origin - offset - Vector3.forward * radius, origin + offset - Vector3.forward * radius);
			Gizmos.DrawLine(origin - offset + Vector3.right * radius, origin + offset + Vector3.right * radius);
			Gizmos.DrawLine(origin - offset - Vector3.right * radius, origin + offset - Vector3.right * radius);
#endif
		}

		public static void DrawEllipse(Transform transform, float xRadius, float zRadius, int segments)
		{
			var theta = 360f / segments * Mathf.Deg2Rad;
			var prev = transform.rotation * new Vector3(
				xRadius * Mathf.Cos(theta),
				0f,
				zRadius * Mathf.Sin(theta));

			for (int i = 1; i <= segments; ++i)
			{
				var curr = transform.rotation * new Vector3(
					xRadius * Mathf.Cos(theta * (i + 1).Mod(segments)),
					0f,
					zRadius * Mathf.Sin(theta * (i + 1).Mod(segments)));

				Gizmos.DrawLine(transform.position + prev, transform.position + curr);
				prev = curr;
			}
		}
	}
}