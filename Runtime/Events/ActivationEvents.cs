using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

namespace ToolkitEngine
{
	[AddComponentMenu("Events/Activation Events")]
    public class ActivationEvents : MonoBehaviour
    {
		#region Events

		[SerializeField, Foldout("Events")]
		private UnityEvent<GameObject> m_onEnabled;

		[SerializeField, Foldout("Events")]
		private UnityEvent<GameObject> m_onDisabled;

		#endregion

		#region Properties

		public UnityEvent<GameObject> OnEnabled => m_onEnabled;
		public UnityEvent<GameObject> OnDisabled => m_onDisabled;

		#endregion

		#region Methods

		private void OnEnable() => m_onEnabled?.Invoke(gameObject);
		private void OnDisable() => m_onDisabled?.Invoke(gameObject);

		#endregion
	}
}