using System;
using System.Collections.Generic;
using System.Linq;
using ToolkitEngine;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace ToolkitEditor
{
	[CustomPropertyDrawer(typeof(GenericObjectProperty))]
	public class GenericObjectPropertyDrawer : BasePropertyDrawer
	{
		#region Fields

		private static List<GUIContent> s_cachedDisplayNames = null;
		private static List<Type> s_cachedTypes = null;
		private static Dictionary<string, Assembly> s_assemblyInfoCache = null;

		#endregion

		#region Properties

		[InitializeOnLoadMethod]
		private static void Initialize()
		{
			CompilationPipeline.assemblyCompilationFinished += (outputPath, messages) =>
			{
				s_cachedDisplayNames = null;
				s_cachedTypes = null;
				s_assemblyInfoCache = null;
			};
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			// Get the m_value and m_requiredType properties
			var valueProp = property.FindPropertyRelative("m_value");
			var requiredTypeProp = property.FindPropertyRelative("m_requiredType");

			if (valueProp == null || requiredTypeProp == null)
			{
				EditorGUI.LabelField(position, "Required fields not found");
				EditorGUI.EndProperty();
				return;
			}

			// Split into two lines
			float lineHeight = EditorGUIUtility.singleLineHeight;
			float spacing = EditorGUIUtility.standardVerticalSpacing;

			Rect typeRect = new Rect(position.x, position.y, position.width, lineHeight);
			Rect valueRect = new Rect(position.x, position.y + lineHeight + spacing, position.width, lineHeight);

			// Draw type selector
			EditorGUI.BeginChangeCheck();

			Type requiredTypeValue = null;
			if (!string.IsNullOrEmpty(requiredTypeProp.stringValue))
			{
				requiredTypeValue = Type.GetType(requiredTypeProp.stringValue);
			}

			requiredTypeValue = DrawTypeField(typeRect, requiredTypeValue);

			if (EditorGUI.EndChangeCheck())
			{
				// Type changed - update the string and clear the value
				if (requiredTypeValue != null)
				{
					requiredTypeProp.stringValue = requiredTypeValue.AssemblyQualifiedName;
				}
				else
				{
					requiredTypeProp.stringValue = string.Empty;
				}

				// Clear the object field when type changes
				valueProp.objectReferenceValue = null;
			}

			// Determine the allowed type for the object field
			Type allowedType = typeof(UnityEngine.Object);
			if (requiredTypeValue != null && typeof(UnityEngine.Object).IsAssignableFrom(requiredTypeValue))
			{
				allowedType = requiredTypeValue;
			}

			// Draw the object field with type constraint
			EditorGUI.BeginChangeCheck();
			var newValue = EditorGUI.ObjectField(valueRect, GUIContent.none, valueProp.objectReferenceValue, allowedType, true);
			if (EditorGUI.EndChangeCheck())
			{
				valueProp.objectReferenceValue = newValue;
			}

			EditorGUI.EndProperty();
		}

		private Type DrawTypeField(Rect position, Type currentType)
		{
			if (s_cachedDisplayNames == null)
			{
				InitializeAssemblyInfoCache();
				s_cachedDisplayNames = new();
				s_cachedTypes = new();

				foreach (var p in from assembly in AppDomain.CurrentDomain.GetAssemblies()
								  let assemblyName = assembly.GetName().Name
								  where IsRuntimeAssembly(assemblyName)
								  from type in assembly.GetTypes()
								  where type.IsClass
								  && !type.IsAbstract
								  && typeof(UnityEngine.Object).IsAssignableFrom(type)
								  && !typeof(Component).IsAssignableFrom(type)
								  orderby assemblyName, type.Name
								  select new { displayName = $"{assemblyName}/{type.Name}", type })
				{
					s_cachedDisplayNames.Add(new GUIContent(p.displayName));
					s_cachedTypes.Add(p.type);
				}
			}
			// Find current index
			int index = currentType != null
				? s_cachedTypes.IndexOf(currentType)
				: -1;

			// Draw popup
			EditorGUI.BeginChangeCheck();
			index = EditorGUI.Popup(position, GUIContent.none, index, s_cachedDisplayNames.ToArray());

			if (EditorGUI.EndChangeCheck() && index >= 0 && index < s_cachedTypes.Count)
			{
				return s_cachedTypes[index];
			}

			return currentType;
		}

		private static void InitializeAssemblyInfoCache()
		{
			if (s_assemblyInfoCache != null)
				return;

			s_assemblyInfoCache = new Dictionary<string, Assembly>();

			// CALL THIS ONLY ONCE - this is the slow operation
			var allAssemblies = CompilationPipeline.GetAssemblies();

			// Cache all assembly info by name for fast lookup
			foreach (var asm in allAssemblies)
			{
				s_assemblyInfoCache[asm.name] = asm;
			}
		}

		private static bool IsRuntimeAssembly(string assemblyName)
		{
			// Ensure cache is initialized
			if (s_assemblyInfoCache == null)
				InitializeAssemblyInfoCache();

			// Check if we have assembly info
			if (s_assemblyInfoCache.TryGetValue(assemblyName, out Assembly assemblyInfo))
			{
				// Check the EditorAssembly flag
				return (assemblyInfo.flags & AssemblyFlags.EditorAssembly) == 0;
			}

			// No asmdef - check if it's a built-in Unity assembly
			if (assemblyName.StartsWith("UnityEditor") || assemblyName == "Assembly-CSharp-Editor")
				return false;

			// Built-in Unity runtime assemblies or default assemblies without asmdef
			return true;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			// Two lines: type field + value field
			return (EditorGUIUtility.singleLineHeight * 2)
				+ EditorGUIUtility.standardVerticalSpacing;
		}

		#endregion
	}
}