using UnityEngine;
using UnityEditor;
using ToolkitEngine;

namespace ToolkitEditor
{
	[CustomPropertyDrawer(typeof(NestedAttribute))]
	public class NestedDrawer : PropertyDrawer
	{
		#region Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.ObjectReference)
			{
				return;
			}

			EditorGUI.BeginProperty(position, label, property);

			var data = property.objectReferenceValue as ScriptableObject;
			if (EditorGUIRectLayout.Foldout(ref position, property, data?.name ?? label.text))
			{
				EditorGUI.BeginChangeCheck();
				EditorGUIRectLayout.PropertyField(ref position, property, new GUIContent("Type"));

				if (EditorGUI.EndChangeCheck())
				{
					data = property.objectReferenceValue as ScriptableObject;
				}

				if (data != null)
				{
					var editor = Editor.CreateEditor(data);
					if (editor is INestableEditor nestableEditor)
					{
						nestableEditor.OnNestedGUI(ref position);

						if (GUI.changed)
						{
							editor.serializedObject.ApplyModifiedProperties();
						}
					}
				}
			}

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			if (property.isExpanded)
			{
				height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

				var data = property.objectReferenceValue as ScriptableObject;
				if (data != null)
				{
					var editor = Editor.CreateEditor(data);
					if (editor is INestableEditor nestableEditor)
					{
						height += nestableEditor.GetNestedHeight();
					}
				}
			}

			return height;
		}

		#endregion
	}
}