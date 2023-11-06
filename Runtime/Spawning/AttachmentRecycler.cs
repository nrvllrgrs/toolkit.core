using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine
{
	public class AttachmentRecycler : MonoBehaviour, IPoolItemRecyclable
    {
		#region Fields

		[SerializeField]
		private GameObject[] m_attachments;

		private Dictionary<GameObject, Record> m_data = new();

		#endregion

		#region Properties

		public GameObject[] attachments => m_attachments;

		#endregion

		#region Methods

		public void Recycle()
		{
			foreach (var p in m_data)
			{
				if (p.Key.TryGetComponent(out Rigidbody rigidbody))
				{
					rigidbody.useGravity = p.Value.useGravity;
					rigidbody.isKinematic = p.Value.isKinematic;
				}

				p.Key.transform.SetParent(p.Value.parent);
				p.Key.transform.localPosition = p.Value.localPosition;
				p.Key.transform.localRotation = p.Value.localRotation;
				p.Key.transform.localScale = p.Value.localScale;

				// Attachment may not have been released, so manually recycle
				foreach (var recyclable in p.Key.GetComponentsInChildren<IPoolItemRecyclable>(true))
				{
					recyclable.Recycle();
				}
			}
		}

		private void Awake()
		{
			foreach (var child in m_attachments)
			{
				if (child.IsNull())
					continue;

				// Not an attachment, skip
				if (child.transform.parent == null)
					continue;

				var data = new Record()
				{
					parent = child.transform.parent,
					localPosition = child.transform.localPosition,
					localRotation = child.transform.localRotation,
					localScale = child.transform.localScale
				};

				if (child.TryGetComponent(out Rigidbody rigidbody))
				{
					data.useGravity = rigidbody.useGravity;
					data.isKinematic = rigidbody.isKinematic;
				}

				m_data.Add(child, data);
			}
		}

		#endregion

		#region Structures

		private struct Record
		{
			// Transform
			public Transform parent;
			public Vector3 localPosition;
			public Quaternion localRotation;
			public Vector3 localScale;

			// Rigidbody
			public bool useGravity;
			public bool isKinematic;
		}

		#endregion
	}
}