using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ToolkitEngine
{
	public class TargetingPoints : MonoBehaviour, IEnumerable<Transform>
	{
		#region Fields

		[SerializeField]
		private Transform[] m_points;

		#endregion

		#region Methods

		#endregion

		#region IEnumerable Methods

		public IEnumerator<Transform> GetEnumerator()
		{
			return m_points.Cast<Transform>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_points.GetEnumerator();
		}

		#endregion
	}
}