using UnityEngine;
using UnityEditor;
using ToolkitEngine;

namespace ToolkitEditor
{
    [CustomPropertyDrawer(typeof(WaveSpawner.Wave))]
    public class WaveDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var wave = property.GetValue<WaveSpawner.Wave>();
            var wavesProp = property.serializedObject.FindProperty("m_waves");

            int index = -1;
            for (int i = 0; i < wavesProp.arraySize; ++i)
            {
                if (Equals(wave, wavesProp.GetArrayElementAtIndex(i).GetValue<WaveSpawner.Wave>()))
                {
                    index = i;
                    break;
                }
            }

			var overrideSpawnerProp = property.FindPropertyRelative("m_overrideSpawner");
            if (!EditorGUIRectLayout.Foldout(ref position, overrideSpawnerProp, string.Format("Wave {0}", index + 1)))
                return;

			var indicesProp = property.FindPropertyRelative("m_indices");
            var thresholdProp = property.FindPropertyRelative("m_threshold");

            EditorGUIRectLayout.PropertyField(ref position, overrideSpawnerProp);
            if (overrideSpawnerProp.boolValue)
            {
                EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_spawner"));
            }

            EditorGUI.BeginChangeCheck();
			EditorGUIRectLayout.PropertyField(ref position, indicesProp);

            if (EditorGUI.EndChangeCheck())
            {
                thresholdProp.intValue = Mathf.Min(thresholdProp.intValue, indicesProp.arraySize);
			}

			EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_preSpawnRules"));
			EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_postSpawnRules"));

            var nextModeProp = property.FindPropertyRelative("m_nextMode");
			EditorGUIRectLayout.PropertyField(ref position, nextModeProp);

            ++EditorGUI.indentLevel;

            if (0 != ((WaveSpawner.NextMode)nextModeProp.intValue & WaveSpawner.NextMode.Count))
            {
                EditorGUIRectLayout.IntSlider(ref position, thresholdProp, 0, indicesProp.arraySize);
			}

			if (0 != ((WaveSpawner.NextMode)nextModeProp.intValue & WaveSpawner.NextMode.Time))
			{
				EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_duration"));
                EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_despawnOnTimeout"));
			}

            --EditorGUI.indentLevel;
		}

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
			var overrideSpawnerProp = property.FindPropertyRelative("m_overrideSpawner");
            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (!overrideSpawnerProp.isExpanded)
                return height;

			var nextModeProp = property.FindPropertyRelative("m_nextMode");
            height += EditorGUI.GetPropertyHeight(overrideSpawnerProp)
                + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_indices"))
                + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_preSpawnRules"))
                + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_postSpawnRules"))
                + EditorGUI.GetPropertyHeight(nextModeProp)
                + (EditorGUIUtility.standardVerticalSpacing * 5);

            if (overrideSpawnerProp.boolValue)
            {
                height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_spawner"))
                    + EditorGUIUtility.standardVerticalSpacing;
			}

			if (0 != ((WaveSpawner.NextMode)nextModeProp.intValue & WaveSpawner.NextMode.Count))
            {
				height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_threshold"))
					+ EditorGUIUtility.standardVerticalSpacing;
			}

			if (0 != ((WaveSpawner.NextMode)nextModeProp.intValue & WaveSpawner.NextMode.Time))
            {
				height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_duration"))
					+ EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_despawnOnTimeout"))
					+ (EditorGUIUtility.standardVerticalSpacing * 2f);
			}

            return height;
		}
    }
}