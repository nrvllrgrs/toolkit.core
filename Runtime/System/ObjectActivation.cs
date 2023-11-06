using UnityEngine;

namespace ToolkitEngine
{
	[System.Serializable]
	public class ObjectActivation
	{
		#region Fields

		[SerializeField]
		private GameObject m_object;

		[SerializeField]
		private bool m_active;

		#endregion

		#region Methods

		public void Set()
		{
			m_object.SetActive(m_active);
		}

		public void Invert()
		{
			m_object.SetActive(!m_active);
		}

		#endregion
	}
}