using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using ToolkitEngine;

namespace ToolkitEditor
{
    [CustomEditor(typeof(BaseRemapper<>))]
    public class BaseRemapperEditor : Editor
    {
        #region Fields

        protected SerializedProperty m_object;
        protected SerializedProperty m_component;
        protected SerializedProperty m_memberName;
        protected SerializedProperty m_isProperty;

        protected SerializedProperty m_curve;
        protected SerializedProperty m_srcMinValue;
        protected SerializedProperty m_srcMaxValue;
        protected SerializedProperty m_dstMinValue;
        protected SerializedProperty m_dstMaxValue;

        protected SerializedProperty m_onRemapped;

        #endregion

        #region Methods

        private void OnEnable()
        {
            m_object = serializedObject.FindProperty(nameof(m_object));
            m_component = serializedObject.FindProperty(nameof(m_component));
            m_memberName = serializedObject.FindProperty(nameof(m_memberName));
            m_isProperty = serializedObject.FindProperty(nameof(m_isProperty));

            m_curve = serializedObject.FindProperty(nameof(m_curve));
            m_srcMinValue = serializedObject.FindProperty(nameof(m_srcMinValue));
            m_srcMaxValue = serializedObject.FindProperty(nameof(m_srcMaxValue));
            m_dstMinValue = serializedObject.FindProperty(nameof(m_dstMinValue));
            m_dstMaxValue = serializedObject.FindProperty(nameof(m_dstMaxValue));

            m_onRemapped = serializedObject.FindProperty(nameof(m_onRemapped));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_object, new GUIContent("Float Member"));

            // Changed object, reset component and member name
            if (EditorGUI.EndChangeCheck())
            {
                m_component.objectReferenceValue = null;
                m_memberName.stringValue = string.Empty;
            }

            string selectionLabel = string.Empty;
            if (m_component.objectReferenceValue != null && !string.IsNullOrWhiteSpace(m_memberName.stringValue))
            {
                selectionLabel = string.Format("{0}.{1}", m_component.objectReferenceValue.GetType().Name, m_memberName.stringValue);
            }

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(string.Empty, GUILayout.Width(EditorGUIUtility.labelWidth));

            if (EditorGUILayout.DropdownButton(new GUIContent(selectionLabel), FocusType.Keyboard, EditorStyles.popup) && m_object.objectReferenceValue != null)
            {
                var menu = new GenericMenu();

                var gameObject = m_object.objectReferenceValue as GameObject;
                if (gameObject != null)
                {
                    PopulateDropDown(menu, gameObject.GetComponents<Component>());
                }
                else
                {
                    var component = m_object.objectReferenceValue as Component;
                    if (component != null)
                    {
                        PopulateDropDown(menu, component.GetComponents<Component>());
                    }
                }

                menu.DropDown(GUILayoutUtility.GetLastRect());
            }

            EditorGUILayout.EndHorizontal();

            DrawMinMax("Source", DrawFloatMinMax, m_srcMinValue, m_srcMaxValue);
            DrawMinMax("Destination", DrawDestinationMinMax, m_dstMinValue, m_dstMaxValue, m_curve);

            EditorGUILayout.Separator();

            if (EditorGUILayoutUtility.Foldout(m_onRemapped, "Events"))
            {
                EditorGUILayout.PropertyField(m_onRemapped);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void PopulateDropDown(GenericMenu menu, Component[] components)
        {
            foreach (var component in components)
            {
                AddMenuItems<FieldInfo>(component.GetType().GetFields().Where(x => x.IsPublic), (field) =>
                {
                    return field.FieldType == typeof(float);
                }, menu, HandleMemberInfoClicked, component, false);

                AddMenuItems<PropertyInfo>(component.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance), (prop) =>
                {
                    return prop.PropertyType == typeof(float);
                }, menu, HandleMemberInfoClicked, component, true);
            }
        }

        private void AddMenuItems<T>(IEnumerable<MemberInfo> items, System.Func<T, bool> predicate, GenericMenu menu, GenericMenu.MenuFunction2 clickedHandler, Component component, bool isProperty)
            where T : MemberInfo
        {
            foreach (var item in items.OrderBy(x => x.Name))
            {
                if (!predicate((T)item))
                    continue;

                menu.AddItem(new GUIContent(string.Format("{0}/{1}", component.GetType().Name, item.Name)), false, clickedHandler, new MenuEventArgs()
                {
                    component = component,
                    memberInfo = item,
                    isProperty = isProperty
                });
            }
        }

        private void HandleMemberInfoClicked(object args)
        {
            serializedObject.Update();

            var menuEventArgs = (MenuEventArgs)args;
            m_component.objectReferenceValue = menuEventArgs.component;
            m_memberName.stringValue = menuEventArgs.memberInfo.Name;
            m_isProperty.boolValue = menuEventArgs.isProperty;

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawMinMax(string label, System.Action<SerializedProperty, SerializedProperty> drawMinMax, SerializedProperty minProp, SerializedProperty maxProp, params SerializedProperty[] args)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            ++EditorGUI.indentLevel;

            foreach (var arg in args)
            {
                EditorGUILayout.PropertyField(arg);
            }

            drawMinMax?.Invoke(minProp, maxProp);

			--EditorGUI.indentLevel;
        }

        protected void DrawFloatMinMax(SerializedProperty minProp, SerializedProperty maxProp)
        {
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(minProp, new GUIContent("Min"));

			if (EditorGUI.EndChangeCheck())
			{
				minProp.floatValue = Mathf.Min(minProp.floatValue, maxProp.floatValue);
			}

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(maxProp, new GUIContent("Max"));

			if (EditorGUI.EndChangeCheck())
			{
				maxProp.floatValue = Mathf.Max(minProp.floatValue, maxProp.floatValue);
			}
		}

        protected virtual void DrawDestinationMinMax(SerializedProperty minProp, SerializedProperty maxProp)
        {
            EditorGUILayout.PropertyField(m_dstMinValue, new GUIContent("Min"));
			EditorGUILayout.PropertyField(m_dstMaxValue, new GUIContent("Max"));
		}

        #endregion

        #region Structures

        private struct MenuEventArgs
        {
            public Component component;
            public MemberInfo memberInfo;
            public bool isProperty;
        }

        #endregion
    }
}