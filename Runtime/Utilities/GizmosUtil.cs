using UnityEditor;
using UnityEngine;

namespace ToolkitEngine
{
	public static class GizmosUtil
    {
		public static void DrawArrow(Transform transform, float scale = 1f)
		{
			DrawArrow(transform, Color.green, scale);
		}

		public static void DrawArrow(Transform transform, Color color, float scale = 1f)
		{
#if UNITY_EDITOR
			Handles.color = color;
			Handles.DrawLines(new[]
			{
				transform.position + transform.rotation * (new Vector3(0f, 0f, 0.5f) * scale),
				transform.position + transform.rotation * (new Vector3(0.5f, 0f, 0.1667f) * scale),
				transform.position + transform.rotation * (new Vector3(0.25f, 0f, 0.1667f) * scale),
				transform.position + transform.rotation * (new Vector3(0.25f, 0f, -0.5f) * scale),
				transform.position + transform.rotation * (new Vector3(-0.25f, 0f, -0.5f) * scale),
				transform.position + transform.rotation * (new Vector3(-0.25f, 0f, 0.1667f) * scale),
				transform.position + transform.rotation * (new Vector3(-0.5f, 0f, 0.1667f) * scale),
			},
			new[] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 0 });

			// Draw UP vector
			Handles.DrawLine(transform.position, transform.position + transform.up * 0.25f * scale);
#endif
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