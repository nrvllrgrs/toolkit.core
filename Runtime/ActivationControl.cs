using UnityEngine;

namespace ToolkitEngine
{
	public class ActivationControl : MonoBehaviour
	{
		#region Fields

		[SerializeField]
		[Tooltip("If TRUE: count tracks deactivation (0 = active, >0 = inactive).\n" +
				 "If FALSE: count tracks activation (>0 = active, 0 = inactive).")]
		private bool m_useDeactivateCount = false;

		// Tracks activation or deactivation pressure depending on mode:
		// - DeactivateCount mode (default): 0 = active, >0 = inactive
		// - ActivateCount mode: >0 = active, 0 = inactive
		private int m_count = 0;

		#endregion

		#region Methods

		/// <summary>
		/// Adjusts the internal count and updates the GameObject's active state if needed.
		/// </summary>
		private void UpdateCount(int delta)
		{
			m_count += delta;

			// Determine what "inactive" means for the current mode:
			// - DeactivateCount mode: inactive when count > 0
			// - ActivateCount mode: inactive when count == 0
			if (gameObject.activeSelf == m_useDeactivateCount)
			{
				// In both modes, a positive count pushes toward the "active" state
				if (m_count > 0)
				{
					gameObject.SetActive(!m_useDeactivateCount);
				}
			}
			else if (m_count == 0)
			{
				// Returning to neutral count restores the default state for the mode
				gameObject.SetActive(m_useDeactivateCount);
			}
		}

		public void SetActive(bool value)
		{
			if (value)
			{
				Activate();
			}
			else
			{
				Deactivate();
			}
		}

		/// <summary>
		/// Applies activation pressure.
		/// Behavior depends on whether we're using activate or deactivate count.
		/// </summary>
		public void Activate() => UpdateCount(m_useDeactivateCount ? -1 : 1);

		/// <summary>
		/// Applies deactivation pressure.
		/// Behavior depends on whether we're using activate or deactivate count.
		/// </summary>
		public void Deactivate() => UpdateCount(m_useDeactivateCount ? 1 : -1);

		/// <summary>
		/// Forces the object into the active state when using activate-count mode.
		/// (In deactivate-count mode, active is already the default at count == 0.)
		/// </summary>
		public void ForceActivate()
		{
			if (!m_useDeactivateCount)
				return;

			// Reset to neutral state (count = 0)
			UpdateCount(-m_count);
		}

		/// <summary>
		/// Forces the object into the inactive state when using deactivate-count mode.
		/// (In activate-count mode, inactive is already the default at count == 0.)
		/// </summary>
		public void ForceDeactivate()
		{
			if (m_useDeactivateCount)
				return;

			// Reset to neutral state (count = 0)
			UpdateCount(-m_count);
		}

		#endregion
	}
}