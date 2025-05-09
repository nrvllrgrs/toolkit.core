using UnityEngine;

namespace ToolkitEngine
{
	public abstract class SubsystemInstance<T, TConfig> : MonoBehaviour
		where T : class, IInstantiableSubsystem, new()
		where TConfig : ScriptableObject
	{
		#region Methods

		protected virtual void Awake()
		{
			DontDestroyOnLoad(gameObject);
			//InstantiableSubsystem<T, TConfig>.CastInstance.SetInstance(gameObject);
		}

		#endregion
	}
}