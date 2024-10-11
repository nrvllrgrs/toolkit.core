using UnityEngine;

namespace ToolkitEngine
{
	[CreateAssetMenu(menuName = "Toolkit/Config/Application Config", order = 0)]
	public class ConfigCollection : ScriptableObject
    {
		#region Fields

		[SerializeField]
		private ScriptableObject[] m_configs;

		#endregion

		#region Properties

		internal ScriptableObject[] configs => m_configs;

		#endregion
	}
}