using System;
using System.Collections.Generic;

namespace ToolkitEngine
{
	public static class LifecycleSubsystem
    {
		#region Enumerators

		public enum Phase
		{
			Update,
			FixedUpdate,
			LateUpdate
		}

		#endregion

		#region Fields

		private static readonly List<ISubsystem> s_subsystems = new();
		private static readonly Dictionary<Phase, List<ISubsystem>> s_phaseMap = new();

		private static readonly List<ISubsystem> s_sweep = new();

		#endregion

		#region Registration Methods

		public static void Register(ISubsystem subsystem) => s_subsystems.Add(subsystem);
		public static void Unregister(ISubsystem subsystem) => s_subsystems.Remove(subsystem);

		public static void Register(ISubsystem subsystem, Phase phase)
		{
			if (!s_phaseMap.TryGetValue(phase, out var list))
			{
				list = new List<ISubsystem>();
				s_phaseMap.Add(phase, list);
			}
			list.Add(subsystem);
		}

		public static void Unregister(ISubsystem subsystem, Phase phase)
		{
			if (s_phaseMap.TryGetValue(phase, out var list))
			{
				list.Remove(subsystem);
				if (list.Count == 0)
				{
					s_phaseMap.Remove(phase);
				}
			}
		}

		#endregion

		#region Callbacks

		public static void Update()
		{
			ProcessPhase(Phase.Update, (s) => s.Update());
		}

		public static void FixedUpdate()
		{
			ProcessPhase(Phase.FixedUpdate, (s) => s.FixedUpdate());
		}

		public static void LateUpdate()
		{
			ProcessPhase(Phase.LateUpdate, (s) => s.LateUpdate());
		}

		private static void ProcessPhase(Phase phase, Action<ISubsystem> action)
		{
			if (!s_phaseMap.TryGetValue(phase, out var list))
				return;
				
			s_sweep.RefreshWith(list);
			foreach (var susbsystem in s_sweep)
			{
				action.Invoke(susbsystem);
			}
		}

		internal static void Dispose()
		{
			s_sweep.RefreshWith(s_subsystems);
			foreach (var susbsystem in s_sweep)
			{
				susbsystem.Dispose();
			}

			s_subsystems.Clear();
			s_sweep.Clear();
		}

		#endregion
	}
}