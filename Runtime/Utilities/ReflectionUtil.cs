using System;
using System.Reflection;

public static class ReflectionUtil
{
	public static bool TryGetField(object obj, string name, out FieldInfo info)
	{
		info = obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		return info != null;
	}

	public static bool TryGetFieldValue<T>(object obj, string name, out FieldInfo info, out T value)
	{
		if (!TryGetField(obj, name, out info))
		{
			value = default;
			return false;
		}

		value = (T)Convert.ChangeType(info.GetValue(obj), typeof(T));
		return true;
	}

	public static bool TrySetFieldValue(object obj, string name, object value)
	{
		if (!TryGetField(obj, name, out var info))
			return false;

		info.SetValue(obj, value);
		return true;
	}

	public static bool TryGetProperty(object obj, string name, out PropertyInfo info)
	{
		info = obj.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		return info != null;
	}

	public static bool TryGetPropertyValue<T>(object obj, string name, out PropertyInfo info, out T value)
	{
		if (!TryGetProperty(obj, name, out info))
		{
			value = default;
			return false;
		}

		value = (T)Convert.ChangeType(info.GetValue(obj), typeof(T));
		return true;
	}

	public static bool TrySetPropertyValue(object obj, string name, object value)
	{
		if (!TryGetProperty(obj, name, out var info))
			return false;

		info.SetValue(obj, value);
		return true;
	}
}
