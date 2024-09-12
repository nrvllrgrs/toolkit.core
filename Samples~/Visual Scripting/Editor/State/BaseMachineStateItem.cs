using System;
using System.Linq;
using System.Reflection;
using ToolkitEngine.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace ToolkitEditor.VisualScripting
{
	[CustomPropertyDrawer(typeof(BaseMachineStateItem<>), true)]
    public class BaseMachineStateItem : PropertyDrawer
    {
		#region Fields

		private const float k_enabledWidth = 20f;

		#endregion

		#region Properties

		protected virtual string[] excludedProperties => new string[]
		{
			"m_enabled",
			"m_name",
			"m_events"
		};

		#endregion

		#region Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			Rect rect = new Rect(position);
			rect.height = EditorGUIUtility.singleLineHeight;

			property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, string.Empty, EditorStyles.foldout);

			// Draw the enabled property
			rect.width = k_enabledWidth;
			rect.x += 4f;

			var enabledProperty = property.FindPropertyRelative("m_enabled");
			if (enabledProperty != null)
			{
				EditorGUI.PropertyField(rect, enabledProperty, GUIContent.none);
			}
			rect.x += rect.width + 2f;

			// Draw the type name
			var width = position.width - 2f;
			rect.width = width - k_enabledWidth;

			var nameProp = property.FindPropertyRelative("m_name");
			EditorGUI.LabelField(rect, !string.IsNullOrWhiteSpace(nameProp.stringValue) ? nameProp.stringValue : property.displayName);

			EditorGUI.EndProperty();

			if (property.isExpanded)
			{
				rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
				EditorGUIRectLayout.PropertyField(ref rect, nameProp);

				// Draw properties
				DrawCustomProperties(ref rect, property);
				DrawProperties(ref rect, property);

				EditorGUIRectLayout.PropertyField(ref rect, property.FindPropertyRelative("m_events"));
			}
		}

		protected virtual void DrawCustomProperties(ref Rect position, SerializedProperty property)
		{ }

		private void DrawProperties(ref Rect position, SerializedProperty property)
		{
			var excludedFieldNames = this.excludedProperties;
			var fields = property.GetValue().GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (var field in fields)
			{
				if (excludedFieldNames.Contains(field.Name))
					continue;

				EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative(field.Name));
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float height = base.GetPropertyHeight(property, label);
			if (property.isExpanded)
			{
				height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_name"))
					+ EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_events"))
					+ (EditorGUIUtility.standardVerticalSpacing * 3f);

				var excludedFieldNames = this.excludedProperties;
				var fields = property.GetValue().GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (var field in fields)
				{
					if (excludedFieldNames.Contains(field.Name))
						continue;

					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative(field.Name))
						+ EditorGUIUtility.standardVerticalSpacing;
				}
			}

			return height;
		}

		#endregion
	}
}