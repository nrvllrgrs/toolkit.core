using UnityEngine;

namespace ToolkitEngine
{
	public interface IInstantiableSubsystem : ISubsystem
	{
		GameObject GetInstance();
		void SetInstance(GameObject instance);
	}
}