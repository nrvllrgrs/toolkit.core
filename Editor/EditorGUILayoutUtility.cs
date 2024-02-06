using System;
using System.Reflection;
using UnityEngine;

namespace UnityEditor
{
	public static class EditorGUILayoutUtility
	{
		public static bool Foldout(SerializedProperty property, string label)
		{
			property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, label);
			return property.isExpanded;
		}

		public static void MinMaxSlider(SerializedProperty property, float minValue, float maxValue)
		{
			var rect = EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(property.displayName);

			var vector = property.vector2Value;

			rect.x += EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;
			rect.width -= EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;
			Rect[] splittedRect = SplitRect(rect, 3);

			EditorGUI.BeginChangeCheck();
			vector.x = EditorGUI.FloatField(splittedRect[0], vector.x);

			if (EditorGUI.EndChangeCheck())
			{
				vector.x = Mathf.Max(Mathf.Min(vector.x, vector.y), minValue);
				property.vector2Value = vector;
			}

			EditorGUI.BeginChangeCheck();
			vector.y = EditorGUI.FloatField(splittedRect[2], vector.y);

			if (EditorGUI.EndChangeCheck())
			{
				vector.y = Mathf.Min(Mathf.Max(vector.x, vector.y), maxValue);
				property.vector2Value = vector;
			}

			EditorGUI.BeginChangeCheck();
			EditorGUI.MinMaxSlider(splittedRect[1], ref vector.x, ref vector.y, minValue, maxValue);

			if (EditorGUI.EndChangeCheck())
			{
				property.vector2Value = vector;
			}

			EditorGUILayout.EndHorizontal();
		}

		private static Rect[] SplitRect(Rect rect, int n)
		{
			Rect[] split = new Rect[n];
			for (int i = 0; i < n; i++)
			{
				split[i] = new Rect(
					rect.position.x + (i * rect.width / n),
					rect.position.y,
					rect.width / n,
					rect.height);
			}

			int padding = (int)split[0].width - 40;
			int space = 5;

			split[0].width -= padding + space;
			split[2].width -= padding + space;

			split[1].x -= padding;
			split[1].width += padding * 2;

			split[2].x += padding + space;

			return split;
		}

		public static void ScriptableObjectField<T>(SerializedProperty property, ScriptableObject scriptableObject, GUIContent label = null)
			where T : ScriptableObject
		{
			label = label ?? new GUIContent(property.displayName);

			// Create faux label to get rect size
			EditorGUILayout.LabelField(new GUIContent(string.Empty));

			var rect = GUILayoutUtility.GetLastRect();
			EditorGUIRectLayout.ScriptableObjectField<T>(ref rect, property, scriptableObject, label);
		}
	}
}