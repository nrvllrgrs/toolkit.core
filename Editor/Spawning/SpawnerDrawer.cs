using UnityEngine;
using UnityEditor;
using ToolkitEngine;

namespace ToolkitEditor
{
    [CustomPropertyDrawer(typeof(Spawner))]
    public class SpawnerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var spawnTypeProp = property.FindPropertyRelative("m_spawnType");
            EditorGUIRectLayout.PropertyField(ref position, spawnTypeProp);

            ++EditorGUI.indentLevel;
            switch ((Spawner.SpawnType)spawnTypeProp.intValue)
            {
                case Spawner.SpawnType.Template:
                    var templateProp = property.FindPropertyRelative("m_template");
                    if (templateProp.objectReferenceValue == null)
                    {
                        EditorGUIRectLayout.HelpBox(ref position, "Template is required!", MessageType.Error);
                    }
                    EditorGUIRectLayout.PropertyField(ref position, templateProp);
                    break;

                case Spawner.SpawnType.ObjectPool:
                    EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_pool"));
                    break;
		    
#if USE_UNITY_ADDRESSABLES
                case Spawner.SpawnType.Addressable:
                    EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_assetReference"));
                    break;
#endif
            }
            --EditorGUI.indentLevel;

            EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_showInHierarchy"));

            if (property.serializedObject.targetObject is Component)
            {
                EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_sets"));
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
	    var spawnTypeProp = property.FindPropertyRelative("m_spawnType");
            float height = EditorGUI.GetPropertyHeight(spawnTypeProp)
                + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_showInHierarchy"))
                + (EditorGUIUtility.standardVerticalSpacing * 2f);

            if (property.serializedObject.targetObject is Component)
            {
                height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_sets"))
                    + EditorGUIUtility.standardVerticalSpacing;
			}

			switch ((Spawner.SpawnType)spawnTypeProp.intValue)
			{
				case Spawner.SpawnType.Template:
					var templateProp = property.FindPropertyRelative("m_template");
					if (templateProp.objectReferenceValue == null)
                    {
                        height += EditorGUIRectLayout.GetHelpboxHeight("Template is required!")
                            + EditorGUIUtility.standardVerticalSpacing;
                    }
                    height += EditorGUI.GetPropertyHeight(templateProp)
                        + EditorGUIUtility.standardVerticalSpacing;
                    break;

                case Spawner.SpawnType.ObjectPool:
                    height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_pool"))
		            + EditorGUIUtility.standardVerticalSpacing;
		        break;
      
#if USE_UNITY_ADDRESSABLES
                case Spawner.SpawnType.Addressable:
                    height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_assetReference"))
                        + EditorGUIUtility.standardVerticalSpacing;
		        break;
#endif
            }

            return height;
        }
    }
}
