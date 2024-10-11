using System;
using UnityEngine;

namespace ToolkitEngine
{
	public interface IInstantiableSubsystemConfig
    {
		GameObject GetTemplate();
		Type GetManagerType();
	}
}