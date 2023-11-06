using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine
{
	public abstract class SerializableDictionary<TKey, TValue>
		: Dictionary<TKey, TValue>, ISerializationCallbackReceiver
	{
		[SerializeField, HideInInspector]
		private List<TKey> keys = new List<TKey>();

		[SerializeField, HideInInspector]
		private List<TValue> values = new List<TValue>();

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			keys.Clear();
			values.Clear();

			foreach (var item in this)
			{
				keys.Add(item.Key);
				values.Add(item.Value);
			}
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			Clear();

			if (keys.Count != values.Count)
			{
				throw new System.Exception(string.Format("Key-Value count mismatch!"));
			}

			for (int i = 0; i < keys.Count; ++i)
			{
				Add(keys[i], values[i]);
			}
		}
	}
}