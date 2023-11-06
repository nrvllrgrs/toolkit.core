using System.Collections;
using UnityEngine;

namespace ToolkitEngine
{
	public class CoroutineUtil : Singleton<CoroutineUtil>
    {
		#region Methods

		public static Coroutine GlobalStartCoroutine(IEnumerator routine)
		{
			return Instance.StartCoroutine(routine);
		}

		public static void GlobalStopCoroutine(Coroutine routine)
		{
			if (routine == null)
				return;

			Instance.StopCoroutine(routine);
		}

		#endregion
	}
}