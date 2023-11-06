using System;

namespace ToolkitEngine
{
	public interface IKillable
    {
		bool isDead { get; }
		event EventHandler Killed;
	}
}