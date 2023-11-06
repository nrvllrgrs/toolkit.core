using UnityEditor;
using UnityEngine;
using ToolkitEngine;

namespace ToolkitEditor
{
	[CustomPropertyDrawer(typeof(PoolItemSpawner<>), true)]
    public class PoolItemSpawnerDrawer : PropertyDrawer
    {
		#region Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var templateProp = property.FindPropertyRelative("m_template");
			if (templateProp.objectReferenceValue == null)
			{
				EditorGUIRectLayout.HelpBox(ref position, "Template is required!", MessageType.Error);
			}
			EditorGUIRectLayout.PropertyField(ref position, templateProp);

			var globalProp = property.FindPropertyRelative("m_global");
			if (property.serializedObject.targetObject is Component)
			{
				// If component is PoolItemManager then must check locally (preventing infinite loop)
				if (property.serializedObject.targetObject is PoolItemManager)
				{
					globalProp.boolValue = false;
				}
				else
				{
					EditorGUIRectLayout.PropertyField(ref position, globalProp);
				}
				
				if (!globalProp.boolValue)
				{
					EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_collectionCheck"));
					EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_capacity"));
					EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_capacityMode"));
				}
			}
			// PoolItemSpawner on ScriptableObjects must be global
			else
			{
				globalProp.boolValue = true;

				EditorGUI.BeginDisabledGroup(true);
				EditorGUIRectLayout.PropertyField(ref position, globalProp);
				EditorGUI.EndDisabledGroup();
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var templateProp = property.FindPropertyRelative("m_template");
			float height = EditorGUI.GetPropertyHeight(templateProp)
				+ EditorGUIUtility.standardVerticalSpacing;

			var globalProp = property.FindPropertyRelative("m_global");
			if (!(property.serializedObject.targetObject is PoolItemManager))
			{
				height += EditorGUI.GetPropertyHeight(globalProp)
					+ EditorGUIUtility.standardVerticalSpacing;
			}

			if (property.serializedObject.targetObject is Component)
			{
				if (!globalProp.boolValue)
				{
					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_collectionCheck"))
						+ EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_capacity"))
						+ EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_capacityMode"))
						+ (EditorGUIUtility.standardVerticalSpacing * 3f);
				}
			}

			if (templateProp.objectReferenceValue == null)
			{
				height += EditorGUIRectLayout.GetHelpboxHeight("Template is required!");
			}
			return height;
		}

		#endregion
	}
}