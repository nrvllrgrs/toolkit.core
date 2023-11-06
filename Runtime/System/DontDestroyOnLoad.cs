namespace UnityEngine
{
	public class DontDestroyOnLoad : MonoBehaviour
	{
		#region Methods

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}

		#endregion
	}
}
