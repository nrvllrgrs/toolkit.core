using UnityEngine;

namespace ToolkitEngine
{
	[System.Serializable]
	public class ObjectActivation
	{
		#region Fields

		[SerializeField]
		private Object m_object;

		[SerializeField]
		private bool m_active;

		#endregion

		#region Methods

		public void Set()
		{
			Set(m_active);
		}

		public void Invert()
		{
			Set(!m_active);
		}

		private void Set(bool value)
		{
			if (m_object is GameObject go)
			{
				if (GameObjectExt.IsNull(go))
					return;

				go.SetActive(value);
			}
			else if (m_object is Behaviour behaviour)
			{
				if (behaviour == null)
					return;

				behaviour.enabled = value;
			}
		}

		#endregion
	}
}