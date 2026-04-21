using UnityEngine;

namespace ToolkitEngine
{
	[CreateAssetMenu(menuName = "Toolkit/Config/PersistentSubsystem Config", order = 10)]
	public class PersistentSubsystemConfig : ScriptableObject
    {
		#region Fields

		[SerializeField]
		private GameObjectMap m_objects = new();

		#endregion

		#region Properties

		public GameObjectMap objects => m_objects;

		#endregion

		#region Structures

		[System.Serializable]
		public class GameObjectMap : SerializableDictionary<string, GameObject>
		{ }

		#endregion
	}
}