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
					SerializedObject serializedObject = new SerializedObject(data);

					// Iterate over all the values and draw them
					SerializedProperty nestedProp = serializedObject.GetIterator();
					if (nestedProp.NextVisible(true))
					{
						do
						{
							// Don't bother drawing the class file
							if (nestedProp.name == "m_Script")
								continue;

							EditorGUIRectLayout.PropertyField(ref position, nestedProp);
						}
						while (nestedProp.NextVisible(false));
					}

					if (GUI.changed)
					{
						serializedObject.ApplyModifiedProperties();
					}

					serializedObject.Dispose();
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
					SerializedObject serializedObject = new SerializedObject(data);

					// Iterate over all the values and draw them
					SerializedProperty nestedProp = serializedObject.GetIterator();
					if (nestedProp.NextVisible(true))
					{
						do
						{
							if (nestedProp.name == "m_Script")
								continue;

							height += EditorGUI.GetPropertyHeight(serializedObject.FindProperty(nestedProp.name), null, true) + EditorGUIUtility.standardVerticalSpacing;
						}
						while (nestedProp.NextVisible(false));
					}

					serializedObject.Dispose();
				}
			}

			return height;
		}

		#endregion
	}
}