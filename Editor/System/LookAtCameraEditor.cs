using UnityEditor;
using ToolkitEngine;

namespace ToolkitEditor
{
	[CustomEditor(typeof(LookAtCamera))]
    public class LookAtCameraEditor : BaseToolkitEditor
    {
		#region Fields

		protected SerializedProperty lookStyle;
		protected SerializedProperty axis;
		protected SerializedProperty reverse;

		#endregion

		#region Methods

		private void OnEnable()
		{
			lookStyle = serializedObject.FindProperty(nameof(lookStyle));
			axis = serializedObject.FindProperty(nameof(axis));
			reverse = serializedObject.FindProperty(nameof(reverse));
		}

		protected override void DrawProperties()
		{
			EditorGUILayout.PropertyField(lookStyle);
			
			if ((LookAtCamera.LookStyle)lookStyle.intValue == LookAtCamera.LookStyle.Axis)
			{
				++EditorGUI.indentLevel;
				EditorGUILayout.PropertyField(axis);
				--EditorGUI.indentLevel;
			}

			EditorGUILayout.PropertyField(reverse);
		}

		#endregion
	}
}