using System.Collections;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;

public static class SerializedPropertyExt
{
    public static bool IsOfType<T>(this SerializedProperty property)
    {
        return property.GetValue() is T;
    }

	public static object GetValue(this SerializedProperty property)
	{
        string path = property.propertyPath.Replace(".Array.data[", "[");
        object obj = property.serializedObject.targetObject;
        string[] elements = path.Split('.');

        foreach (string element in elements.Take(elements.Length))
        {
            if (element.Contains("["))
            {
                string elementName = element.Substring(0, element.IndexOf("["));
                int index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue(obj, elementName, index);
            }
            else
            {
                obj = GetValue(obj, element);
            }
        }
        return obj;
    }

    public static T GetValue<T>(this SerializedProperty property)
    {
        object value = property.GetValue();
        if (value == null)
            return default(T);

		return (T)value;
    }

    private static object GetValue(object source, string name)
    {
        if (source == null)
            return null;

        var type = source.GetType();

        var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        if (f == null)
        {
            var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p == null)
                return null;

            return p.GetValue(source, null);
        }
        return f.GetValue(source);
    }

    private static object GetValue(object source, string name, int index)
    {
        var enumerable = GetValue(source, name) as IEnumerable;
        if (enumerable == null)
            return null;

        var enm = enumerable.GetEnumerator();
        while (index-- >= 0)
        {
            enm.MoveNext();
        }

        return enm.Current;
    }

	public static int GetChildIndex<T>(this SerializedProperty property, string arrayName)
	{
		var item = property.GetValue<T>();
		var arrayProp = property.serializedObject.FindProperty(arrayName);

		for (int i = 0; i < arrayProp.arraySize; ++i)
		{
			if (Equals(item, arrayProp.GetArrayElementAtIndex(i).GetValue<T>()))
			{
				return i;
			}
		}

		return -1;
	}
}