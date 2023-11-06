using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine
{
	public class FacingEvents : MonoBehaviour
	{
		#region Fields

		[SerializeField]
		private Transform m_target;

		[SerializeField]
		private Axis m_axis = Axis.Forward;

		[SerializeField]
		private Vector3 m_direction = Vector3.forward;

		[SerializeField]
		private Space m_space = Space.Self;

		[SerializeField, Range(0f, 180f)]
		private float m_threshold = 10f;

		private bool m_facing;

		#endregion

		#region Events

		[SerializeField]
		private UnityEvent<bool> m_onFacingChanged;

		#endregion

		#region Properties

		public bool facing
		{
			get => m_facing;
			private set
			{
				// No change, skip
				if (m_facing == value)
					return;

				m_facing = value;
				m_onFacingChanged?.Invoke(value);
			}
		}

		public UnityEvent<bool> onFacing => m_onFacingChanged;

		#endregion

		#region Methods

		private void Awake()
		{
			if (m_target == null)
			{
				m_target = transform;
			}

			m_direction = m_direction.normalized;
		}

		private void Update()
		{
			var from = m_direction;
			if (m_space == Space.Self)
			{
				from = transform.rotation * from;
			}

			facing = Vector3.Angle(from, m_target.GetDirection(m_axis)) <= m_threshold;
		}

		#endregion

		#region Editor-Only
#if UNITY_EDITOR

		private void OnDrawGizmosSelected()
		{
			var target = m_target != null
				? m_target
				: transform;

			Gizmos.DrawRay(target.position, target.GetDirection(m_axis));

			Color color = Color.white;
			if (Application.isPlaying)
			{
				color = facing
					? Color.green
					: Color.red;
			}

			var from = m_direction;
			if (m_space == Space.Self)
			{
				from = transform.rotation * from;
			}
			GizmosUtil.DrawCone(target.position, m_threshold * 2f, 1f, from, color);
		}

#endif
		#endregion
	}
}