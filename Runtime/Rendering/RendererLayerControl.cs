using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine.Rendering
{
	public class RendererLayerControl : MonoBehaviour
    {
		#region Fields

		[SerializeField, Layer]
		protected int m_layer;

		[SerializeField]
		private Transform m_target;

		[SerializeField]
		private List<Renderer> m_ignoredRenderers;

		private Dictionary<Renderer, int> m_defaultLayers = new();

		#endregion

		#region Methods

		private void Awake()
		{
			if (m_target == null)
			{
				m_target = transform;
			}

			foreach (var renderer in m_target.GetComponentsInChildren<Renderer>())
			{
				if (m_ignoredRenderers.Contains(renderer))
					continue;

				m_defaultLayers.Add(renderer, renderer.gameObject.layer);
			}
		}

		public void SetLayer()
		{
			SetLayer(m_layer);
		}

		public void SetLayer(int layer)
		{
			foreach (var renderer in m_defaultLayers.Keys)
			{
				renderer.gameObject.layer = layer;
			}
		}

		public void ResetLayer()
		{
			foreach (var p in m_defaultLayers)
			{
				p.Key.gameObject.layer = p.Value;
			}
		}

		#endregion
	}
}