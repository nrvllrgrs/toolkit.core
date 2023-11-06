using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

[CustomEditor(typeof(BaseSetAnimatorParameter), true)]
public class BaseSetAnimatorParameterEditor : Editor
{
    #region Fields

    protected BaseSetAnimatorParameter m_setter;

    protected SerializedProperty m_animator;
    protected SerializedProperty m_parameterName;

    #endregion

    #region Methods

    protected virtual void OnEnable()
    {
        m_setter = target as BaseSetAnimatorParameter;

        m_animator = serializedObject.FindProperty(nameof(m_animator));
        m_parameterName = serializedObject.FindProperty(nameof(m_parameterName));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_animator);

        var animator = m_animator.objectReferenceValue as Animator;
        if (animator == null)
        {
            EditorGUILayout.PropertyField(m_parameterName);
        }
        else
        {
            List<string> parameterNameList = new();

            var controller = animator?.runtimeAnimatorController as AnimatorController;
            if (controller != null)
            {
                foreach (var parameter in animator.parameters)
                {
                    if (parameter.type != m_setter.AnimatorControllerParameterType)
                        continue;

                    parameterNameList.Add(parameter.name);
                }
            }

            if (!parameterNameList.Any())
            {
                EditorGUILayout.HelpBox(string.Format("Selected animator has no {0} parameters.", m_setter.AnimatorControllerParameterType.ToString()), MessageType.Warning);
                EditorGUILayout.PropertyField(m_parameterName);
            }
            else
            {
                parameterNameList = parameterNameList.OrderBy(x => x).ToList();

                int parameterNameIndex = 0;
                if (!string.IsNullOrWhiteSpace(m_parameterName.stringValue))
                {
                    parameterNameIndex = parameterNameList.IndexOf(m_parameterName.stringValue);
                }

                if (parameterNameIndex >= 0)
                {
                    parameterNameIndex = EditorGUILayout.Popup(m_parameterName.displayName, parameterNameIndex, parameterNameList.ToArray());
                    m_parameterName.stringValue = parameterNameList[parameterNameIndex];
                }
                else
                {
                    EditorGUILayout.HelpBox("Selected Parameter Name does not exist in animator.", MessageType.Warning);
                    EditorGUILayout.PropertyField(m_parameterName);
                }
            }
        }

        DrawProperties();

        serializedObject.ApplyModifiedProperties();
    }

    protected virtual void DrawProperties() { }

    #endregion
}

[CustomEditor(typeof(BaseSetAnimatorParameter<>), true)]
public class BaseGenericSetAnimatorParameterEditor : BaseSetAnimatorParameterEditor
{
    #region Fields

    protected SerializedProperty m_value;

    #endregion

    #region Methods

    protected override void OnEnable()
    {
        base.OnEnable();
        m_value = serializedObject.FindProperty(nameof(m_value));
    }

    protected override void DrawProperties()
    {
        EditorGUILayout.PropertyField(m_value);
    }

    #endregion
}