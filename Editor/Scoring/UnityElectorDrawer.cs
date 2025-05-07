using UnityEditor;
using UnityEngine;
using ToolkitEngine.Scoring;

namespace ToolkitEditor
{
	[CustomPropertyDrawer(typeof(UnityElector), true)]
    public class UnityElectorDrawer : PropertyDrawer
    {
		#region Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			label.tooltip = property.tooltip;

			if (EditorGUIRectLayout.Foldout(ref position, property, label))
			{
				++EditorGUI.indentLevel;
				var modeProp = property.FindPropertyRelative("m_mode");
				EditorGUIRectLayout.PropertyField(ref position, modeProp);

				++EditorGUI.indentLevel;
				switch ((UnityElector.SelectionMode)modeProp.intValue)
				{
					case UnityElector.SelectionMode.Count:
						EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_count"));
						EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_randomnessMode"));
						break;

					case UnityElector.SelectionMode.Percent:
						EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_percent"));
						EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_randomnessMode"));
						break;
				}
				--EditorGUI.indentLevel;

				EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_score"));
				--EditorGUI.indentLevel;
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float height = EditorGUIUtility.singleLineHeight; // Property label

			if (property.isExpanded)
			{
				var modeProp = property.FindPropertyRelative("m_mode");
				height += EditorGUI.GetPropertyHeight(modeProp) // Mode
					+ EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_score")) // Score
					+ (EditorGUIUtility.standardVerticalSpacing * 2); // Spacing

				switch ((UnityElector.SelectionMode)modeProp.intValue)
				{
					case UnityElector.SelectionMode.Count:
						height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_count"))
							+ EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_randomnessMode"))
							+ (EditorGUIUtility.standardVerticalSpacing * 2);
						break;

					case UnityElector.SelectionMode.Percent:
						height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_percent"))
							+ EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_randomnessMode"))
							+ (EditorGUIUtility.standardVerticalSpacing * 2);
						break;
				}
			}

			return height;
		}

		#endregion
	}
}