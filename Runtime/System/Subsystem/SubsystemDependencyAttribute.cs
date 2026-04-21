using System;

namespace ToolkitEngine
{
	/// <summary>
	/// Declares that this subsystem requires another subsystem to be initialized first.
	/// Apply multiple times to express multiple dependencies.
	///
	/// Example:
	///   [SubsystemDependency(typeof(AudioSubsystem))]
	///   [SubsystemDependency(typeof(PoolingSubsystem))]
	///   public class MusicSubsystem : ConfigurableSubsystem<MusicSubsystem, MusicConfig> { }
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public sealed class SubsystemDependencyAttribute : Attribute
	{
		public Type DependencyType { get; }

		public SubsystemDependencyAttribute(Type dependencyType)
		{
			DependencyType = dependencyType;
		}
	}
}