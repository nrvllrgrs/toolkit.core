using UnityEngine;

namespace ToolkitEngine
{
	public sealed class Comment : MonoBehaviour
    {
#if UNITY_EDITOR
		#region Fields

		[SerializeField, Multiline(9)]
		private string m_text;

		[SerializeField]
		private string m_author;

		[SerializeField]
		private long m_date;

		[SerializeField]
		private bool m_showInScene = true;

		#endregion

		#region Properties

		public string text => m_text;
		public string author => m_author;
		public bool showInScene => m_showInScene;

		#endregion
#endif
	}
}