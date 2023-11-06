namespace UnityEngine
{
	public static class GameObjectExt
	{
		public static T GetComponent<T>(this GameObject obj, ref T value)
			where T : Component
		{
#if !UNITY_EDITOR
			if (value == null)
			{
				value = obj.GetComponent<T>();
			}
			return value;
#else
			if (value != null)
			{
				return value;
			}
			return obj.GetComponent<T>();
#endif
		}

		public static T[] GetComponentsInChildren<T>(this GameObject obj, ref T[] value, bool includeInactive = false)
			where T : Component
		{
#if !UNITY_EDITOR
			if (value == null)
			{
				value = obj.GetComponentsInChildren<T>(includeInactive);
			}
			return value;
#else
			if (value != null)
			{
				return value;
			}
			return obj.GetComponentsInChildren<T>(includeInactive);
#endif
		}

		public static T GetComponentInParent<T>(this GameObject obj, ref T value)
			where T : Component
		{
#if !UNITY_EDITOR
			if (value == null)
			{
				value = obj.GetComponentInParent<T>();
			}
			return value;
#else
			if (value != null)
			{
				return value;
			}
			return obj.GetComponentInParent<T>();
#endif
		}

		public static T ReadyComponent<T>(this GameObject obj)
			where T : Component
		{
			if (!obj.TryGetComponent(out T component))
			{
				component = obj.AddComponent<T>();
			}
			return component;
		}

		public static bool CompareTagInParent(this GameObject obj, string tag)
		{
			return CompareTagInParent(obj.transform, tag);
		}

		public static bool CompareTagInParent(this Transform transform, string tag)
		{
			while (transform != null)
			{
				if (transform.CompareTag(tag))
					return true;

				transform = transform.parent;
			}

			return false;
		}

		public static bool IsAncestorOf(this GameObject ancestor, GameObject obj)
		{
			if (obj == null)
				return false;

			return obj.IsDescendantOf(ancestor);
		}

		public static bool IsDescendantOf(this GameObject descendant, GameObject obj)
		{
			if (obj == null)
				return false;

			Transform parent = descendant.transform.parent;
			while (parent != null)
			{
				if (parent == obj.transform)
					return true;

				parent = parent.parent;
			}

			return false;
		}

		public static bool IsNull(this GameObject obj)
		{
			return obj == null || obj.Equals(null);
		}

		public static void ToggleActive(this GameObject obj)
		{
			obj.SetActive(!obj.activeInHierarchy);
		}

		public static Bounds GetLocalRendererBounds(this GameObject obj)
		{
			// Remember rotation to be restored after finding bounds
			var position = obj.transform.position;
			var rotation = obj.transform.rotation;
			obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

			Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
			foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
			{
				bounds.Encapsulate(renderer.localBounds);
			}

			// Restore previous position / rotation
			obj.transform.SetPositionAndRotation(position, rotation);
			return bounds;
		}

		public static Bounds GetColliderBounds(this GameObject obj, bool ignoreTriggers = true)
		{
			bool first = true;
			Bounds bounds = new();

			foreach (var collider in obj.GetComponentsInChildren<Collider>())
			{
				if (ignoreTriggers && collider.isTrigger)
					continue;

				if (first)
				{
					bounds = collider.bounds;
					first = false;
				}
				else
				{
					bounds.Encapsulate(collider.bounds);
				}
			}

			return bounds;
		}

		public static Bounds GetLocalColliderBounds(this GameObject obj, bool ignoreTriggers = true)
		{
			// Remember current position / rotation
			Vector3 position = obj.transform.position;
			Quaternion rotation = obj.transform.rotation;

			// Set position / rotation for bounds calculation
			obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

			Bounds bounds = new Bounds();
			foreach (var collider in obj.GetComponentsInChildren<Collider>())
			{
				if (ignoreTriggers && collider.isTrigger)
					continue;

				bounds.Encapsulate(collider.bounds);
			}

			// Restore previous position / rotation
			obj.transform.SetPositionAndRotation(position, rotation);
			return bounds;
		}
	}
}