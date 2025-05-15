using UnityEngine;

namespace ToolkitEngine
{
    [System.Serializable]
    public class AttachPoint
    {
		#region Fields

		[SerializeField]
		private Transform m_transform;

		[SerializeField]
		private Vector3 m_offset;

		[SerializeField]
		private bool m_invertFacing;

		#endregion

		#region Properties

		public Transform transform => m_transform;
		public Vector3 position => m_transform.position + localPosition;
		public Vector3 localPosition => m_transform.localRotation * m_offset;

		#endregion

		#region Methods

		public void Initalize(Transform transform)
		{
			if (m_transform == null)
			{
				m_transform = transform;
			}
		}

		public void Attach(Transform child)
		{
			child.SetParent(m_transform, false);
			child.SetLocalPositionAndRotation(localPosition, Quaternion.identity);
		}

		public Vector3 GetPosition(Transform transform)
		{
			var attachPoint = m_transform != null
				? m_transform
				: transform;

			return attachPoint.position + attachPoint.rotation * m_offset;
		}

		#endregion
	}
}