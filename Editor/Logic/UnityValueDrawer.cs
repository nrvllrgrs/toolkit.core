using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using ToolkitEngine;
using Sirenix.Utilities;

namespace ToolkitEditor
{
	[CustomPropertyDrawer(typeof(UnityValue<>))]
	public class UnityValueDrawer<T> : PropertyDrawer
	{
		#region Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var tooltipAttr = fieldInfo.GetAttribute<TooltipAttribute>();
			label.tooltip = tooltipAttr?.tooltip;

			EditorGUI.BeginProperty(position, label, property);

			var typeProperty = property.FindPropertyRelative("m_type");
			var valueProperty = property.FindPropertyRelative("m_value");
			var objectProperty = property.FindPropertyRelative("m_object");
			var componentProperty = property.FindPropertyRelative("m_component");
			var memberNameProperty = property.FindPropertyRelative("m_memberName");

			EditorGUIRectLayout.LabelField(ref position, label);

			++EditorGUI.indentLevel;
			EditorGUIRectLayout.PropertyField(ref position, typeProperty);

			switch ((UnityValue<T>.ValueType)typeProperty.intValue)
			{
				case UnityValue<T>.ValueType.Value:
					EditorGUIRectLayout.PropertyField(ref position, valueProperty);
					break;

				case UnityValue<T>.ValueType.Member:
					EditorGUI.BeginChangeCheck();
					EditorGUIRectLayout.PropertyField(ref position, objectProperty, new GUIContent("Member"));

					// Changed object, reset component and member name
					if (EditorGUI.EndChangeCheck())
					{
						componentProperty.objectReferenceValue = null;
						memberNameProperty.stringValue = string.Empty;
					}

					string selectionLabel = string.Empty;
					if (componentProperty.objectReferenceValue != null && !string.IsNullOrWhiteSpace(memberNameProperty.stringValue))
					{
						selectionLabel = string.Format("{0}.{1}", componentProperty.objectReferenceValue.GetType().Name, memberNameProperty.stringValue);
					}

					var memberRect = new Rect(position);
					memberRect.x += EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;
					memberRect.width -= EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;
					memberRect.height = EditorGUIUtility.singleLineHeight;

					if (EditorGUI.DropdownButton(memberRect, new GUIContent(selectionLabel), FocusType.Keyboard) && objectProperty.objectReferenceValue != null)
					{
						var menu = new GenericMenu();

						var gameObject = objectProperty.objectReferenceValue as GameObject;
						if (gameObject != null)
						{
							PopulateDropDown(menu, gameObject.GetComponents<Component>(), property);
						}
						else
						{
							var component = objectProperty.objectReferenceValue as Component;
							if (component != null)
							{
								PopulateDropDown(menu, component.GetComponents<Component>(), property);
							}
						}

						menu.DropDown(memberRect);
					}
					break;
			}

			--EditorGUI.indentLevel;
		}

		private void PopulateDropDown(GenericMenu menu, Component[] components, SerializedProperty property)
		{
			foreach (var component in components)
			{
				AddMenuItems<FieldInfo>(component.GetType().GetFields().Where(x => x.IsPublic), (field) =>
				{
					return field.FieldType == typeof(T);
				}, menu, HandleMemberInfoClicked, property, component, false);

				AddMenuItems<PropertyInfo>(component.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance), (prop) =>
				{
					return prop.PropertyType == typeof(T);
				}, menu, HandleMemberInfoClicked, property, component, true);
			}
		}

		private void AddMenuItems<K>(IEnumerable<MemberInfo> items, System.Func<K, bool> predicate, GenericMenu menu, GenericMenu.MenuFunction2 clickedHandler, SerializedProperty property, Component component, bool isProperty)
			where K : MemberInfo
		{
			foreach (var item in items.OrderBy(x => x.Name))
			{
				if (!predicate((K)item))
					continue;

				menu.AddItem(new GUIContent(string.Format("{0}/{1}", component.GetType().Name, item.Name)), false, clickedHandler, new MenuEventArgs()
				{
					property = property,
					component = component,
					memberInfo = item,
					isProperty = isProperty
				});
			}
		}

		private void HandleMemberInfoClicked(object args)
		{
			var menuEventArgs = (MenuEventArgs)args;

			menuEventArgs.property.serializedObject.Update();
			menuEventArgs.property.FindPropertyRelative("m_component").objectReferenceValue = menuEventArgs.component;
			menuEventArgs.property.FindPropertyRelative("m_memberName").stringValue = menuEventArgs.memberInfo.Name;
			menuEventArgs.property.FindPropertyRelative("m_isProperty").boolValue = menuEventArgs.isProperty;
			menuEventArgs.property.serializedObject.ApplyModifiedProperties();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var typeProperty = property.FindPropertyRelative("m_type");
			switch ((UnityValue<T>.ValueType)typeProperty.intValue)
			{
				case UnityValue<T>.ValueType.Value:
					return (EditorGUIUtility.singleLineHeight * 3f)
						+ (EditorGUIUtility.standardVerticalSpacing * 3f);

				case UnityValue<T>.ValueType.Member:
					return (EditorGUIUtility.singleLineHeight * 4f)
						+ (EditorGUIUtility.standardVerticalSpacing * 4f);
			}

			return 0f;
		}

		#endregion

		#region Structures

		private struct MenuEventArgs
		{
			public SerializedProperty property;
			public Component component;
			public MemberInfo memberInfo;
			public bool isProperty;
		}

		#endregion
	}
}