using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace ToolkitEngine.Rendering
{
    public abstract class BaseMaterialModifier : MonoBehaviour
    {
        #region Enumerators

        public enum Mode
        {
            Renderer,
            Material,
			Graphic,
        }

        #endregion

        #region Fields

        [Header("Material Settings")]

        [SerializeField]
        protected string m_propertyName;

        [SerializeField]
        protected Mode m_mode = Mode.Renderer;

        [SerializeField]
        protected Renderer[] m_renderers;

        [SerializeField]
        protected bool m_shared;

        [SerializeField]
        protected int[] m_indices;

        [SerializeField]
        protected Material[] m_materials;

		[SerializeField]
		protected Graphic[] m_graphics;

		[SerializeField]
		protected bool m_applyOnEnable;

		protected int m_nameId;

		#endregion

		#region Properties

#if UNITY_EDITOR
		public abstract ShaderPropertyType ShaderPropertyType { get; }
#endif

        #endregion

        #region Methods

        protected virtual void Awake()
        {
            m_nameId = Shader.PropertyToID(m_propertyName);
        }

        #endregion
    }

    public abstract class BaseMaterialModifier<T> : BaseMaterialModifier
    {
		#region Fields

		[SerializeField]
		private string[] m_enableKeywords;

		Dictionary<Graphic, Material> m_graphicMaterials = new();

#if UNITY_EDITOR
		private List<Material> m_editorMaterials;
#endif
		#endregion

		#region Methods

		protected override void Awake()
		{
			base.Awake();

#if UNITY_EDITOR
			if (m_mode == Mode.Material)
			{
				m_editorMaterials = new();
				foreach (var material in m_materials)
				{
					m_editorMaterials.Add(new Material(material));
				}
			}
			UnityEditor.EditorApplication.playModeStateChanged += EditorApplication_PlayModeStateChanged;
#endif

			if (m_enableKeywords.Length > 0)
			{
				switch (m_mode)
				{
					case Mode.Renderer:
						foreach (var renderer in m_renderers)
						{
							List<Material> materials = new();
							if (m_shared)
							{
								renderer.GetSharedMaterials(materials);
							}
							else
							{
								renderer.GetMaterials(materials);
							}

							for (int i = 0; i < materials.Count; ++i)
							{
								if (m_indices.Length > 0 && !m_indices.Contains(i))
									continue;

								EnableKeywords(materials[i]);
							}
						}
						break;

					case Mode.Material:
						foreach (var material in m_materials)
						{
							EnableKeywords(material);
						}
						break;

					case Mode.Graphic:
						foreach (var graphic in m_graphics)
						{
							if (graphic == null)
								continue;

							// m_shared == true uses the shared defaultMaterial; false uses the instance material
							EnableKeywords(m_shared ? graphic.defaultMaterial : graphic.material);
						}
						break;
				}
			}
		}

		private void EnableKeywords(Material material)
		{
			foreach (var keyword in m_enableKeywords)
			{
				material.EnableKeyword(keyword);
			}
		}

		protected virtual void OnEnable()
		{
			if (m_applyOnEnable)
			{
                Set(0f);
			}
		}

		protected virtual void OnDestroy()
		{
			foreach (var material in m_graphicMaterials.Values)
			{
				Destroy(material);
			}

#if UNITY_EDITOR
			UnityEditor.EditorApplication.playModeStateChanged -= EditorApplication_PlayModeStateChanged;
#endif
		}

#if UNITY_EDITOR
		private void EditorApplication_PlayModeStateChanged(UnityEditor.PlayModeStateChange stateChange)
		{
			if (stateChange == UnityEditor.PlayModeStateChange.ExitingPlayMode)
			{
				for (int i = 0; i < m_materials.Length; ++i)
				{
					m_materials[i].CopyPropertiesFromMaterial(m_editorMaterials[i]);
				}
			}
		}
#endif

		protected Material GetGraphicMaterial(Graphic graphic)
		{
			if (m_shared)
			{
				if (graphic is TMPro.TMP_Text tmpText)
					return tmpText.fontSharedMaterial;

				return graphic.material;
			}

			if (!m_graphicMaterials.TryGetValue(graphic, out var material))
			{
				if (graphic is TMPro.TMP_Text tmpText)
				{
					// Copy from the clean font material, not the stencil-processed one
					material = new Material(tmpText.fontSharedMaterial);
					tmpText.fontMaterial = material;
				}
				else
				{
					material = new Material(graphic.materialForRendering);
					graphic.material = material;
				}
				m_graphicMaterials.Add(graphic, material);
			}
			return material;
		}

		public void Set(float t)
        {
			T value = GetValue(t);
			switch (m_mode)
			{
				case Mode.Renderer:
					foreach (var renderer in m_renderers)
					{
						List<Material> materials = new();
						if (m_shared)
						{
							renderer.GetSharedMaterials(materials);
						}
						else
						{
							renderer.GetMaterials(materials);
						}

						for (int i = 0; i < materials.Count; ++i)
						{
							if (m_indices.Length > 0 && !m_indices.Contains(i))
								continue;

							Set(materials[i], value);
						}
					}
					break;

				case Mode.Material:
					foreach (var material in m_materials)
					{
						Set(material, value);
					}
					break;

				case Mode.Graphic:
					foreach (var graphic in m_graphics)
					{
						if (graphic == null)
							continue;

						if (!SetGraphic(graphic, value))
						{
							var material = GetGraphicMaterial(graphic);
							Set(material, value);
							graphic.SetMaterialDirty();
						}
						graphic.SetMaterialDirty();
					}
					break;
			}
		}

		/// <summary>
		/// Called for each Graphic before falling through to material-level Set.
		/// Override to handle Graphic subclasses that don't use material properties
		/// Return true if the value was applied directly to the Graphic, 
		/// false to fall through to the normal material path.
		/// </summary>
		protected virtual bool SetGraphic(Graphic graphic, T value) => false;

		protected abstract T GetValue(float t);
        protected abstract void Set(Material material, T value);

        #endregion
    }
}