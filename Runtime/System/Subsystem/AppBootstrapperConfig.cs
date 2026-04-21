using UnityEngine;

namespace ToolkitEngine
{
	/// <summary>
	/// Holds the list of subsystems the AppBootstrapper should initialize at startup.
	/// Place one instance of this asset inside any Resources folder so it can be
	/// loaded with Resources.Load at both runtime and edit-time.
	///
	/// Recommended path: Assets/Resources/AppBootstrapperConfig.asset
	/// </summary>
	[CreateAssetMenu(menuName = "Toolkit/Config/AppBootstrapper Config", order = 1)]
	public class AppBootstrapperConfig : ScriptableObject
	{
		#region Fields

		[SerializeField]
		private SubsystemEntry[] m_entries = new SubsystemEntry[0];

		public const string RESOURCE_NAME = "AppBootstrapperConfig";

		#endregion

		#region Properties

		public SubsystemEntry[] entries => m_entries;

		#endregion
	}
}