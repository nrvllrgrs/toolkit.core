using UnityEngine;

namespace ToolkitEngine
{
	public abstract class InstantiableSubsystem<T, TConfig> : ConfigurableSubsystem<T, TConfig>, IInstantiableSubsystem
		where T : class, IInstantiableSubsystem, new()
		where TConfig : ScriptableObject
	{
		#region Fields

		private GameObject m_gameObjectInstance = null;

		#endregion

		#region Methods

		public GameObject GetInstance() => m_gameObjectInstance;
		public void SetInstance(GameObject instance) => m_gameObjectInstance = instance;

		#endregion
	}
}