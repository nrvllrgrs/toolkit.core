using System;
using UnityEngine;

namespace UnityEditor
{
	public static class EditorGUIRectLayout
    {
		public static void BeginFieldOnly(ref Rect rect)
		{
			rect.x += EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;
			rect.width -= EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;
		}

		public static void EndFieldOnly(ref Rect rect)
		{
			rect.x -= EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;
			rect.width += EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;
		}

		public static void CurveField(ref Rect rect, SerializedProperty property, string label, int lineCount = 1)
		{
			CurveField(ref rect, property, Color.green, label, lineCount);
		}

		public static void CurveField(ref Rect rect, SerializedProperty property, Color color, string label, int lineCount = 1)
		{
			CurveField(ref rect, property, color, new GUIContent(label), lineCount);
		}

		public static void CurveField(ref Rect rect, SerializedProperty property, GUIContent label = null, int lineCount = 1)
		{
			CurveField(ref rect, property, Color.green, label, lineCount);
		}

		public static void CurveField(ref Rect rect, SerializedProperty property, Color color, GUIContent label = null, int lineCount = 1)
		{
			label = label ?? new GUIContent(property.displayName);

			rect.height = EditorGUIUtility.singleLineHeight * lineCount;
			EditorGUI.CurveField(rect, property, color, new Rect(0f, 0f, 1f, 1f), label);

			rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
		}

		public static void EnumPopup<T>(ref Rect rect, SerializedProperty property, string label)
			where T : Enum
		{
			EnumPopup<T>(ref rect, property, new GUIContent(label));
		}

		public static void EnumPopup<T>(ref Rect rect, SerializedProperty property, GUIContent label)
			where T : Enum
		{
			rect.height = EditorGUIUtility.singleLineHeight;

			var value = (T)Enum.GetValues(typeof(T)).GetValue(property.enumValueIndex);
			value = (T)EditorGUI.EnumPopup(rect, label, value);
			property.enumValueIndex = Convert.ToInt32(value);

			rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
		}

		public static float FloatField(ref Rect rect, string label, float value)
		{
			return FloatField(ref rect, new GUIContent(label), value);
		}

		public static float FloatField(ref Rect rect, GUIContent label, float value)
		{
			rect.height = EditorGUIUtility.singleLineHeight;
			value = EditorGUI.FloatField(rect, label, value);

			rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
			return value;
		}

		public static bool Foldout(ref Rect rect, SerializedProperty property, string label, GUIStyle style = null)
		{
			return Foldout(ref rect, property, new GUIContent(label), style);
		}

		public static bool Foldout(ref Rect rect, SerializedProperty property, GUIContent label, GUIStyle style = null)
		{
			rect.height = EditorGUIUtility.singleLineHeight;
			property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label, style ?? EditorStyles.foldout);

			rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
			return property.isExpanded;
		}

		public static float GetFoldoutHeight(SerializedProperty property)
		{
			return EditorGUIUtility.singleLineHeight
				+ (property.isExpanded ? EditorGUI.GetPropertyHeight(property) : 0f);
		}

		public static float GetHelpboxHeight(string message)
		{
			return Mathf.Max(
				EditorStyles.helpBox.CalcHeight(new GUIContent(message), EditorGUIUtility.currentViewWidth) + 4,
				38f);
		}

		public static float GetSpaceHeight() => EditorGUIUtility.standardVerticalSpacing * 2;

		public static void HelpBox(ref Rect rect, string message, MessageType messageType = MessageType.None)
		{
			rect.height = GetHelpboxHeight(message);
			EditorGUI.HelpBox(rect, message, messageType);

			rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
		}

		public static int IntField(ref Rect rect, string label, int value)
		{
			return IntField(ref rect, new GUIContent(label), value);
		}

		public static int IntField(ref Rect rect, GUIContent label, int value)
		{
			rect.height = EditorGUIUtility.singleLineHeight;
			value = EditorGUI.IntField(rect, label, value);

			rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
			return value;
		}

		public static void IntSlider(ref Rect rect, SerializedProperty property, int minValue, int maxValue)
		{
			rect.height = EditorGUI.GetPropertyHeight(property);
			EditorGUI.IntSlider(rect, property, minValue, maxValue);

			rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
		}

		public static void LabelField(ref Rect rect, string label, GUIStyle style = null)
		{
			LabelField(ref rect, new GUIContent(label), style);
		}

		public static void LabelField(ref Rect rect, GUIContent label, GUIStyle style = null)
		{
			rect.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.LabelField(rect, label, style ?? EditorStyles.label);

			rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
		}

		public static void ProgressBar(ref Rect rect, float value, string label)
		{
			rect.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.ProgressBar(rect, value, label);

			rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
		}

		public static void PropertyField(ref Rect rect, SerializedProperty property, GUIContent label = null)
		{
			label = label ?? new GUIContent(property.displayName);

			rect.height = EditorGUI.GetPropertyHeight(property);
			EditorGUI.PropertyField(rect, property, label);

			rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
		}

		public static void Slider(ref Rect rect, SerializedProperty property, float minValue, float maxValue)
		{
			rect.height = EditorGUI.GetPropertyHeight(property);
			EditorGUI.Slider(rect, property, minValue, maxValue);

			rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
		}

		public static void Space(ref Rect rect)
		{
			rect.y += GetSpaceHeight();
		}
	}
}