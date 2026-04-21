using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ToolkitEngine.EventSystems
{
	[AddComponentMenu("Event/Priority Physics Raycaster")]
	[RequireComponent(typeof(Camera))]
	public class PriorityPhysicsRaycaster : BaseRaycaster
	{
		[SerializeField]
		private LayerMask m_eventMask = ~0;

		public override Camera eventCamera => Camera.main;

		public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			// If any GraphicsRaycaster has already found a UI hit, don't
			// submit any physics results — UI always wins.
			foreach (var result in resultAppendList)
			{
				if (result.module is GraphicRaycaster)
					return;
			}

			Ray ray = eventCamera.ScreenPointToRay(eventData.position);
			if (!Physics.Raycast(ray, out RaycastHit hit))
				return;

			if ((m_eventMask & (1 << hit.collider.gameObject.layer)) == 0)
				return;

			resultAppendList.Add(new RaycastResult
			{
				gameObject = hit.collider.gameObject,
				module = this,
				distance = hit.distance,
				index = resultAppendList.Count,
				worldPosition = hit.point,
				worldNormal = hit.normal
			});
		}
	}
}