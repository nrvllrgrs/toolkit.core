using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using ToolkitEngine.Rendering;
using TMPro;

namespace ToolkitEditor.Rendering
{
	[CustomEditor(typeof(BaseMaterialModifier), true)]
	public class BaseMaterialModifierEditor : Editor
	{
		#region Fields

		protected BaseMaterialModifier m_materialModifier;

		protected SerializedProperty m_enableKeywords;
		protected SerializedProperty m_mode;
		protected SerializedProperty m_renderers;
		protected SerializedProperty m_shared;
		protected SerializedProperty m_indices;
		protected SerializedProperty m_materials;
		protected SerializedProperty m_graphics;
		protected SerializedProperty m_propertyName;
		protected SerializedProperty m_applyOnEnable;

		#endregion

		#region Methods

		protected virtual void OnEnable()
		{
			m_materialModifier = target as BaseMaterialModifier;

			m_enableKeywords = serializedObject.FindProperty(nameof(m_enableKeywords));
			m_mode = serializedObject.FindProperty(nameof(m_mode));
			m_renderers = serializedObject.FindProperty(nameof(m_renderers));
			m_shared = serializedObject.FindProperty(nameof(m_shared));
			m_indices = serializedObject.FindProperty(nameof(m_indices));
			m_materials = serializedObject.FindProperty(nameof(m_materials));
			m_graphics = serializedObject.FindProperty(nameof(m_graphics));
			m_propertyName = serializedObject.FindProperty(nameof(m_propertyName));
			m_applyOnEnable = serializedObject.FindProperty(nameof(m_applyOnEnable));
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			using (new EditorGUI.DisabledScope(true))
			{
				if (target is MonoBehaviour behaviour)
				{
					EditorGUILayout.ObjectField(EditorGUIUtility.TrTempContent("Script"), MonoScript.FromMonoBehaviour(behaviour), typeof(MonoBehaviour), false);
				}
			}

			EditorGUILayout.PropertyField(m_enableKeywords);

			var mode = (BaseMaterialModifier.Mode)m_mode.intValue;

			HashSet<string> propertyNames = new();
			switch (mode)
			{
				case BaseMaterialModifier.Mode.Renderer:
					HashSet<int> validIndices = new();
					int indexCount = m_indices.arraySize;
					for (int i = 0; i < indexCount; ++i)
					{
						validIndices.Add(m_indices.GetArrayElementAtIndex(i).intValue);
					}

					int rendererCount = m_renderers.arraySize;
					for (int i = 0; i < rendererCount; ++i)
					{
						var renderer = m_renderers.GetArrayElementAtIndex(i)?.objectReferenceValue as Renderer;
						if (renderer == null)
							continue;

						// In editor, only need to get shared materials
						List<Material> materials = new();
						renderer.GetSharedMaterials(materials);

						foreach (var material in materials)
						{
							if (validIndices.Count > 0 && !validIndices.Contains(i))
								continue;

							ProcessMaterial(material, propertyNames);
						}
					}
					break;

				case BaseMaterialModifier.Mode.Material:
					int materialCount = m_materials.arraySize;
					for (int i = 0; i < materialCount; ++i)
					{
						var material = m_materials.GetArrayElementAtIndex(i)?.objectReferenceValue as Material;
						if (material == null)
							continue;

						ProcessMaterial(material, propertyNames);
					}
					break;

				case BaseMaterialModifier.Mode.Graphic:
					int graphicCount = m_graphics.arraySize;
					for (int i = 0; i < graphicCount; ++i)
					{
						var graphic = m_graphics.GetArrayElementAtIndex(i)?.objectReferenceValue as Graphic;
						if (graphic == null)
							continue;

						Material mat;
						if (graphic is TMP_Text tmpText)
						{
							// TMP_Text overrides material handling
							mat = tmpText.fontSharedMaterial;
						}
						else
						{
							// Use the assigned material if set, otherwise fall back to the default material
							mat = graphic.material != null ? graphic.material : graphic.defaultMaterial;
						}

						ProcessMaterial(mat, propertyNames);
					}
					break;
			}

			// No property names exist (probably undefined renderers, materials, or graphics)
			if (!propertyNames.Any())
			{
				EditorGUILayout.PropertyField(m_propertyName);
			}
			else
			{
				var propertyNameList = new List<string>(propertyNames.OrderBy(x => x));

				int propertyNameIndex = 0;
				if (!string.IsNullOrWhiteSpace(m_propertyName.stringValue))
				{
					propertyNameIndex = propertyNameList.IndexOf(m_propertyName.stringValue);
				}

				if (propertyNameIndex >= 0)
				{
					propertyNameIndex = EditorGUILayout.Popup(m_propertyName.displayName, propertyNameIndex, propertyNameList.ToArray());
					m_propertyName.stringValue = propertyNameList[propertyNameIndex];
				}
				else
				{
					EditorGUILayout.HelpBox("Selected Property Name does not exist in shaders.", MessageType.Warning);
					EditorGUILayout.PropertyField(m_propertyName);
				}
			}

			EditorGUILayout.PropertyField(m_mode);

			switch (mode)
			{
				case BaseMaterialModifier.Mode.Renderer:
					EditorGUILayout.PropertyField(m_renderers);
					EditorGUILayout.PropertyField(m_indices);
					EditorGUILayout.PropertyField(m_shared);
					break;

				case BaseMaterialModifier.Mode.Material:
					EditorGUILayout.PropertyField(m_materials);
					break;

				case BaseMaterialModifier.Mode.Graphic:
					EditorGUILayout.PropertyField(m_graphics);
					DrawSharedWarning();
					EditorGUILayout.PropertyField(m_shared);
					break;
			}

			EditorGUILayout.PropertyField(m_applyOnEnable);
			DrawProperties();

			serializedObject.ApplyModifiedProperties();
		}

		/// <summary>
		/// Warns the user when Shared is enabled in Graphic mode, since writing to the
		/// defaultMaterial affects every Graphic that references it.
		/// </summary>
		private void DrawSharedWarning()
		{
			if (m_shared.boolValue)
			{
				EditorGUILayout.HelpBox(
					"Shared is enabled: changes will be written to the Graphic's default (shared) material " +
					"and will affect every UI element that uses the same material asset.",
					MessageType.Warning);
			}
		}

		private void ProcessMaterial(Material material, HashSet<string> propertyNames)
		{
			if (material == null || material.shader == null)
				return;

			var propertyCount = material.shader.GetPropertyCount();
			for (int i = 0; i < propertyCount; ++i)
			{
				if (material.shader.GetPropertyType(i) == m_materialModifier.ShaderPropertyType)
				{
					propertyNames.Add(material.shader.GetPropertyName(i));
				}
			}
		}

		protected virtual void DrawProperties()
		{ }

		#endregion
	}
}