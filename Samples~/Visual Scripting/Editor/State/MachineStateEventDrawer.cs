using UnityEngine;
using UnityEditor;
using ToolkitEngine.VisualScripting;

namespace ToolkitEditor.VisualScripting
{
	[CustomPropertyDrawer(typeof(MachineStateEvent<>))]
    public sealed class MachineStateEventDrawer : PropertyDrawer
    {
		#region Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			label.tooltip = property.tooltip;

			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.PropertyField(position, property.FindPropertyRelative("m_arguments"), label);
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_arguments"));
		}

		#endregion
	}

	[CustomPropertyDrawer(typeof(MachineStateEvent<>.Argument))]
	public sealed class MachineStateEventArgumentDrawer : PropertyDrawer
	{
		#region Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var stateProperty = property.FindPropertyRelative("m_state");
			var scriptMachineProperty = property.FindPropertyRelative("m_scriptMachine");

			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.PropertyField(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), stateProperty, new GUIContent());

			var memberRect = new Rect(
				position.x + EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing,
				position.y,
				position.width - (EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing),
				EditorGUIUtility.singleLineHeight);

			EditorGUI.PropertyField(memberRect, scriptMachineProperty, new GUIContent());
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight(property, label);
		}

		#endregion
	}
}