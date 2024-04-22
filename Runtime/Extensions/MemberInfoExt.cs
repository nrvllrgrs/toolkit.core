using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

public static class MemberInfoExt
{
	public static void SetMemberValue(this MemberInfo member, object obj, object value)
	{
		if (member is FieldInfo fieldInfo)
		{
			fieldInfo.SetValue(obj, value);
		}
		else if (member is PropertyInfo propertyInfo)
		{
			propertyInfo.SetValue(obj, value);
		}

		throw new ArgumentException("Can't set the value of a " + member.GetType().Name);
	}

	public static object GetMemberValue(this MemberInfo member, object obj)
	{
		if (member is FieldInfo fieldInfo)
		{
			return fieldInfo.GetValue(obj);
		}
		else if (member is PropertyInfo propertyInfo)
		{
			return propertyInfo.GetGetMethod(nonPublic: true).Invoke(obj, null);
		}

		throw new ArgumentException("Can't get the value of a " + member.GetType().Name);
	}

	public static T GetMemberValue<T>(this MemberInfo memberInfo, object obj)
	{
		var value = GetMemberValue(memberInfo, obj);
		return (T)Convert.ChangeType(value, typeof(T));
	}

	public static T GetAttribute<T>(this ICustomAttributeProvider member, bool inherit)
		where T : Attribute
	{
		T[] array = member.GetAttributes<T>(inherit).ToArray();
		if (array != null && array.Length != 0)
		{
			return array[0];
		}

		return null;
	}

	public static T GetAttribute<T>(this ICustomAttributeProvider member)
		where T : Attribute
	{
		return member.GetAttribute<T>(false);
	}

	public static IEnumerable<T> GetAttributes<T>(this ICustomAttributeProvider member)
		where T : Attribute
	{
		return member.GetAttributes<T>(false);
	}

	public static IEnumerable<T> GetAttributes<T>(this ICustomAttributeProvider member, bool includeAncestors)
		where T : Attribute
	{
		try
		{
			return member.GetCustomAttributes(typeof(T), includeAncestors).Cast<T>();
		}
		catch
		{
			return new T[0];
		}
	}

	public static Attribute[] GetAttributes(this ICustomAttributeProvider member)
	{
		try
		{
			return member.GetAttributes<Attribute>().ToArray();
		}
		catch
		{
			return new Attribute[0];
		}
	}

	public static Attribute[] GetAttributes(this ICustomAttributeProvider member, bool includeAncestors)
	{
		try
		{
			return member.GetAttributes<Attribute>(includeAncestors).ToArray();
		}
		catch
		{
			return new Attribute[0];
		}
	}
}