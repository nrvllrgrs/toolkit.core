using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEditor
{
	public static class SerializedPropertyUtility
	{
		public static List<string> GetDerivedSerializedPropertyNames(Editor editor)
		{
			if (editor == null)
				throw new ArgumentNullException(nameof(editor));

			var fields = editor.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			var propertyNames = new List<string> { "m_Script" };
			foreach (var field in fields)
			{
				var value = field.GetValue(editor);
				if (value is SerializedProperty serializedProperty)
				{
					propertyNames.Add(serializedProperty.name);
				}
			}

			return propertyNames;
		}
	}
}