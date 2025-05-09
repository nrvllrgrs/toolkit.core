using System;

namespace ToolkitEngine
{
	public interface IInstantiableSubsystemConfig
	{
		Type subsystemType { get; }
	}
}