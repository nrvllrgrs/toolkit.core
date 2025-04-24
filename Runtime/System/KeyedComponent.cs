using UnityEngine;
using UnityEngine.Assertions;

namespace ToolkitEngine
{
	public abstract class KeyedComponent<T> : MonoBehaviour
        where T : Component
    {
		#region Fields

		[SerializeField]
		private string m_key;

		[SerializeField]
		private T m_value;

		#endregion

		#region Properties

		public string key => m_key;
		public T value => m_value;

		#endregion

		#region Methods

		protected virtual void Awake()
		{
			Assert.IsTrue(!string.IsNullOrWhiteSpace(m_key));
			Assert.IsNotNull(m_value);
		}

		#endregion
	}
}