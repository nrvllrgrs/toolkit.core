using System.Collections;
using System.Reflection;
using UnityEngine;

public static class ComponentExt
{
	public static T GetComponent<T>(this Component component, ref T value)
		where T : Component
	{
		return component.gameObject.GetComponent(ref value);
	}

	public static T[] GetComponentsInChildren<T>(this Component component, ref T[] value, bool includeInactive = false)
		where T : Component
	{
		return component.gameObject.GetComponentsInChildren(ref value, includeInactive);
	}

	public static T GetComponentInParent<T>(this Component component, ref T value)
		where T : Component
	{
		return component.gameObject.GetComponentInParent<T>(ref value);
	}

	public static T ReadyComponent<T>(this Component component)
		where T : Component
    {
		return component.gameObject.ReadyComponent<T>();
    }

	public static T CopyTo<T>(this T component, GameObject destination)
		where T : Component
	{
		var type = component.GetType();
		var copy = destination.AddComponent(type);

		FieldInfo[] fields = type.GetFields();
		foreach (var field in fields)
		{
			field.SetValue(copy, field.GetValue(component));
		}

		return copy as T;
	}

	public static void CancelCoroutine<T>(this T component, ref Coroutine coroutine)
		where T : MonoBehaviour
    {
		if (coroutine != null)
		{
			component.StopCoroutine(coroutine);
			coroutine = null;
		}
	}

	public static void RestartCoroutine<T>(this T component, IEnumerator routine, ref Coroutine coroutine)
		where T : MonoBehaviour
    {
		component.CancelCoroutine(ref coroutine);

		if (component.isActiveAndEnabled)
		{
			coroutine = component.StartCoroutine(routine);
		}
    }
}