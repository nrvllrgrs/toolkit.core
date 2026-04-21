using System;
using System.Collections.Generic;
using System.Linq;
using ToolkitEngine;
using UnityEditor;
using UnityEngine;

namespace ToolkitEditor
{
	[CustomPropertyDrawer(typeof(SubsystemEntry))]
	public class SubsystemEntryDrawer : PropertyDrawer
	{
		#region Static Type Cache

		private static List<Type> s_subsystemTypes;
		private static string[] s_displayNames;   // shown in the popup (None + all types)
		private static string[] s_assemblyNames;  // parallel array of AssemblyQualifiedName

		private static void EnsureTypeCache()
		{
			if (s_subsystemTypes != null)
				return;

			s_subsystemTypes = new List<Type>();
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				try
				{
					foreach (var type in assembly.GetTypes())
					{
						if (type.IsAbstract || type.IsGenericTypeDefinition)
							continue;

						if (IsConcreteSubsystem(type))
						{
							s_subsystemTypes.Add(type);
						}
					}
				}
				catch { /* skip assemblies that throw on GetTypes() */ }
			}

			s_subsystemTypes.Sort((a, b) =>
				string.Compare(a.FullName, b.FullName, StringComparison.Ordinal));

			// Index 0 is always the sentinel "(None)" entry.
			s_displayNames = s_subsystemTypes
				.Select(t => FormatTypeName(t))
				.Prepend("(None)")
				.ToArray();

			s_assemblyNames = s_subsystemTypes
				.Select(t => t.AssemblyQualifiedName ?? string.Empty)
				.Prepend(string.Empty)
				.ToArray();
		}

		/// <summary>
		/// Returns true when <paramref name="type"/> has Subsystem&lt;T&gt; somewhere in its
		/// base-type chain.
		/// </summary>
		private static bool IsConcreteSubsystem(Type type)
		{
			var t = type.BaseType;
			while (t != null)
			{
				if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ToolkitEngine.Subsystem<>))
					return true;

				t = t.BaseType;
			}
			return false;
		}

		/// <summary>
		/// Converts "My.Namespace.FooSubsystem" → "My/Namespace/FooSubsystem" so that the
		/// Popup renders a nested folder-style menu.
		/// </summary>
		private static string FormatTypeName(Type type) => (type.FullName ?? type.Name).Replace('.', '/').Replace('+', '/');

		#endregion

		#region Height

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			EnsureTypeCache();
			int rows = HasConfigRow(property) ? 3 : 2; // type + [config] + mode
			return rows * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing)
				   - EditorGUIUtility.standardVerticalSpacing;
		}

		private static bool HasConfigRow(SerializedProperty property)
		{
			var typeNameProp = property.FindPropertyRelative("m_typeAssemblyQualifiedName");
			if (string.IsNullOrEmpty(typeNameProp?.stringValue))
				return false;

			var type = Type.GetType(typeNameProp.stringValue);
			return type != null
				&& SubsystemEntry.GetConfigurableSubsystemConfigType(type) != null;
		}

		#endregion

		#region OnGUI

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EnsureTypeCache();

			EditorGUI.BeginProperty(position, label, property);

			var typeNameProp = property.FindPropertyRelative("m_typeAssemblyQualifiedName");
			var configProp = property.FindPropertyRelative("config");
			var modeProp = property.FindPropertyRelative("mode");

			var row = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

			// Type picker
			int currentIndex = Array.IndexOf(s_assemblyNames, typeNameProp.stringValue);
			if (currentIndex < 0)
			{
				currentIndex = 0;
			}

			EditorGUI.BeginChangeCheck();
			int newIndex = EditorGUIRectLayout.Popup(ref row, "Subsystem", currentIndex, s_displayNames);
			if (EditorGUI.EndChangeCheck())
			{
				typeNameProp.stringValue = newIndex == 0
					? string.Empty
					: s_assemblyNames[newIndex];

				// Clear any stale config reference when the type changes
				if (newIndex == 0)
				{
					configProp.objectReferenceValue = null;
				}
				else
				{
					var newType = Type.GetType(s_assemblyNames[newIndex]);
					var expectedCfgType = SubsystemEntry.GetConfigurableSubsystemConfigType(newType);

					// Clear config if it no longer matches the new type
					if (configProp.objectReferenceValue != null && expectedCfgType != null
						&& !expectedCfgType.IsInstanceOfType(configProp.objectReferenceValue))
					{
						configProp.objectReferenceValue = null;
					}
				}
			}

			// Config (only when the type is a ConfigurableSubsystem)
			// EditorGUIRectLayout.ObjectField<T> requires a compile-time type, so we
			// call EditorGUI directly here and advance row manually — the runtime
			// configType is only known after resolving the serialized type string.
			if (!string.IsNullOrEmpty(typeNameProp.stringValue))
			{
				var resolvedType = Type.GetType(typeNameProp.stringValue);
				if (resolvedType != null)
				{
					var configType = SubsystemEntry.GetConfigurableSubsystemConfigType(resolvedType);
					if (configType != null)
					{
						var tooltip = new GUIContent("Config", $"Must be of type: {configType.Name}");
						configProp.objectReferenceValue = EditorGUI.ObjectField(
							row, tooltip,
							configProp.objectReferenceValue,
							configType,
							allowSceneObjects: false);

						row.y += row.height + EditorGUIUtility.standardVerticalSpacing;
					}
				}
			}

			// Game Mode
			EditorGUIRectLayout.PropertyField(ref row, modeProp, new GUIContent("Mode"));

			EditorGUI.EndProperty();
		}

		#endregion
	}
}