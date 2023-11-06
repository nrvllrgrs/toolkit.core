using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine.Rendering
{
	public class MaterialRecycler : MonoBehaviour, IPoolItemRecyclable
    {
		#region Fields

		/// <summary>
		/// Map material to default material properties
		/// </summary>
		private Dictionary<Material, Material> m_materialMap = new();

		#endregion

		#region Methods

		public void Recycle()
		{
			foreach (var p in m_materialMap)
			{
				p.Key.CopyPropertiesFromMaterial(p.Value);
			}
		}

		private void Awake()
		{
			foreach (var renderer in GetComponentsInChildren<Renderer>(true))
			{
				foreach (var material in renderer.materials)
				{
					if (m_materialMap.ContainsKey(material))
						continue;

					var copy = new Material(material.shader);
					copy.CopyPropertiesFromMaterial(material);
					m_materialMap.Add(material, copy);
				}
			}
		}

		#endregion
	}
}