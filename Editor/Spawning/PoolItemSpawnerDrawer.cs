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
			var sourceProp = property.FindPropertyRelative("m_source");
			var sourceValue = (PoolItemSpawner.SourceType)sourceProp.intValue;

			if (property.serializedObject.targetObject is Component)
			{
				EditorGUIRectLayout.PropertyField(ref position, sourceProp);
			}

			if (sourceValue != PoolItemSpawner.SourceType.Direct)
			{
				var templateProp = property.FindPropertyRelative("m_template");
				if (templateProp.objectReferenceValue == null)
				{
					EditorGUIRectLayout.HelpBox(ref position, "Template is required!", MessageType.Error);
				}
				EditorGUIRectLayout.PropertyField(ref position, templateProp);
			}

			if (property.serializedObject.targetObject is Component)
			{
				switch (sourceValue)
				{
					case PoolItemSpawner.SourceType.Internal:
						EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_collectionCheck"));
						EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_capacity"));
						EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_capacityMode"));
						break;

					case PoolItemSpawner.SourceType.Direct:
						EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_spawner"));
						break;
				}
			}
			// PoolItemSpawner on ScriptableObjects must be global
			else
			{
				sourceProp.intValue = (int)PoolItemSpawner.SourceType.Global;

				EditorGUI.BeginDisabledGroup(true);
				EditorGUIRectLayout.PropertyField(ref position, sourceProp);
				EditorGUI.EndDisabledGroup();
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float height = 0f;
			var sourceProp = property.FindPropertyRelative("m_source");
			var sourceValue = (PoolItemSpawner.SourceType)sourceProp.intValue;

			SerializedProperty templateProp = null;
			if (sourceValue != PoolItemSpawner.SourceType.Direct)
			{
				templateProp = property.FindPropertyRelative("m_template");
				height += EditorGUI.GetPropertyHeight(templateProp)
					+ EditorGUIUtility.standardVerticalSpacing;
			}

			height += EditorGUI.GetPropertyHeight(sourceProp)
					+ EditorGUIUtility.standardVerticalSpacing;

			switch ((PoolItemSpawner.SourceType)sourceProp.intValue)
			{
				case PoolItemSpawner.SourceType.Internal:
					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_collectionCheck"))
						+ EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_capacity"))
						+ EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_capacityMode"))
						+ (EditorGUIUtility.standardVerticalSpacing * 3f);
					break;

				case PoolItemSpawner.SourceType.Direct:
					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_spawner"))
						+ EditorGUIUtility.standardVerticalSpacing;
					break;
			}

			if (templateProp != null && templateProp.objectReferenceValue == null)
			{
				height += EditorGUIRectLayout.GetHelpboxHeight("Template is required!");
			}
			return height;
		}

		#endregion
	}
}