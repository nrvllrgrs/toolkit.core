using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(NavMeshAreaMaskAttribute))]
public class NavMeshAreaMaskDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position = EditorGUI.PrefixLabel(position, label);

        EditorGUI.BeginChangeCheck();

        string[] areaNames = GameObjectUtility.GetNavMeshAreaNames();
        string[] completedAreaNames = new string[areaNames.Length];

        foreach (var name in areaNames)
        {
            completedAreaNames[GameObjectUtility.GetNavMeshAreaFromName(name)] = name;
        }

        int mask = EditorGUI.MaskField(position, property.intValue, completedAreaNames);

        if (EditorGUI.EndChangeCheck())
        {
            property.intValue = mask;
        }
    }
}
