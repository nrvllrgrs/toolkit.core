using System;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	public static class EditorGUIRectLayout
    {
		private const int BUTTON_WIDTH = 66;

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

		public static bool Button(ref Rect rect, string label)
		{
			return Button(ref rect, new GUIContent(label));
		}

		public static bool Button(ref Rect rect, GUIContent label)
		{
			bool r = GUI.Button(rect, label);
			rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;

			return r;
		}

		public static Color ColorField(ref Rect rect, string label, Color value) => GenericField(ref rect, label, value, EditorGUI.ColorField);
		public static Color ColorField(ref Rect rect, GUIContent label, Color value) => GenericField(ref rect, label, value, EditorGUI.ColorField);

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

		public static float FloatField(ref Rect rect, string label, float value) => GenericField(ref rect, label, value, EditorGUI.FloatField);
		public static float FloatField(ref Rect rect, GUIContent label, float value) => GenericField(ref rect, label, value, EditorGUI.FloatField);

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

		private static T GenericField<T>(ref Rect rect, string label, T value, Func<Rect, GUIContent, T, T> guiFieldFunc)
		{
			return GenericField(ref rect, new GUIContent(label), value, guiFieldFunc);
		}

		private static T GenericField<T>(ref Rect rect, GUIContent label, T value, Func<Rect, GUIContent, T, T> guiFieldFunc)
		{
			rect.height = EditorGUIUtility.singleLineHeight;
			value = guiFieldFunc(rect, label, value);

			rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
			return value;
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

		public static int IntField(ref Rect rect, string label, int value) => GenericField(ref rect, label, value, EditorGUI.IntField);

		public static int IntField(ref Rect rect, GUIContent label, int value) => GenericField(ref rect, label, value, EditorGUI.IntField);

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

		public static void ObjectField(ref Rect rect, SerializedProperty property, GUIContent label = null)
		{
			label = label ?? new GUIContent(property.displayName);

			rect.height = EditorGUI.GetPropertyHeight(property);
			EditorGUI.ObjectField(rect, property, label);

			rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
		}

		public static int Popup(ref Rect rect, string label, int selectedIndex, string[] displayedOptions)
		{
			return Popup(ref rect, new GUIContent(label), selectedIndex, displayedOptions.Select(x => new GUIContent(x)).ToArray());
		}

		public static int Popup(ref Rect rect, GUIContent label, int selectedIndex, GUIContent[] displayedOptions)
		{
			rect.height = EditorGUIUtility.singleLineHeight;
			int index = EditorGUI.Popup(rect, label, selectedIndex, displayedOptions);

			rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
			return index;
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

		public static void ScriptableObjectField<T>(ref Rect rect, SerializedProperty property, ScriptableObject scriptableObject, GUIContent label = null)
			where T : ScriptableObject
		{
			label = label ?? new GUIContent(property.displayName);

			if (property.objectReferenceValue == null)
			{
				var propertyRect = new Rect(rect.x, rect.y, rect.width - (BUTTON_WIDTH + EditorGUIUtility.standardVerticalSpacing), EditorGUIUtility.singleLineHeight);

				EditorGUI.BeginDisabledGroup(true);
				PropertyField(ref propertyRect, property, label);
				EditorGUI.EndDisabledGroup();

				var buttonRect = new Rect(rect.x + rect.width - BUTTON_WIDTH, rect.y, BUTTON_WIDTH, EditorGUIUtility.singleLineHeight);
				if (GUI.Button(buttonRect, "Create"))
				{
					var asset = ScriptableObject.CreateInstance<T>();
					asset.name = Guid.NewGuid().ToString();

					AssetDatabase.AddObjectToAsset(asset, scriptableObject);

					property.objectReferenceValue = asset;
					property.serializedObject.ApplyModifiedProperties();
					AssetDatabase.SaveAssets();
				}

				rect.y = propertyRect.y;
			}
			else
			{
				var propertyRect = new Rect(rect.x, rect.y, rect.width - (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing), EditorGUIUtility.singleLineHeight);

				EditorGUI.BeginDisabledGroup(true);
				PropertyField(ref propertyRect, property, label);
				EditorGUI.EndDisabledGroup();

				var content = EditorGUIUtility.IconContent("TreeEditor.Trash");
				var buttonRect = new Rect(rect.x + rect.width - EditorGUIUtility.singleLineHeight, rect.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
				if (GUI.Button(buttonRect, content, EditorStyles.iconButton))
				{
					var scriptGraph = property.objectReferenceValue;
					AssetDatabase.RemoveObjectFromAsset(scriptGraph);

					property.objectReferenceValue = null;
					property.serializedObject.ApplyModifiedProperties();
					AssetDatabase.SaveAssets();
				}

				rect.y = propertyRect.y;
			}
		}

		public static float Slider(ref Rect rect, SerializedProperty property, float minValue, float maxValue)
		{
			return Slider(ref rect, property.displayName, property.floatValue, minValue, maxValue, EditorGUI.GetPropertyHeight(property));
		}

		public static float Slider(ref Rect rect, string label, float value, float minValue, float maxValue)
		{
			return Slider(ref rect, label, value, minValue, maxValue, EditorGUIUtility.singleLineHeight);
		}

		private static float Slider(ref Rect rect, string label, float value, float minValue, float maxValue, float height)
		{
			rect.height = height;
			float r = EditorGUI.Slider(rect, label, value, minValue, maxValue);

			rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
			return r;
		}

		public static void Space(ref Rect rect)
		{
			rect.y += GetSpaceHeight();
		}

		public static string TextField(ref Rect rect, string label, string value, GUIStyle style = null)
		{
			return TextField(ref rect, new GUIContent(label), value, style);
		}

		public static string TextField(ref Rect rect, GUIContent label, string value, GUIStyle style = null)
		{
			style ??= EditorStyles.textField;

			rect.height = EditorGUIUtility.singleLineHeight;
			string s = EditorGUI.TextField(rect, label, value, style);

			rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
			return s;
		}

		public static bool ToggleField(ref Rect rect, string label, bool value) => GenericField(ref rect, label, value, EditorGUI.Toggle);
		public static bool ToggleField(ref Rect rect, GUIContent label, bool value) => GenericField(ref rect, label, value, EditorGUI.Toggle);
	}
}