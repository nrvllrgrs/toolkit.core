using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine
{
	public abstract class SerializableDictionary<TKey, TValue>
		: Dictionary<TKey, TValue>, ISerializationCallbackReceiver
	{
		[SerializeField, HideInInspector]
		protected List<TKey> keys = new List<TKey>();

		[SerializeField, HideInInspector]
		protected List<TValue> values = new List<TValue>();

		protected SerializableDictionary()
			: base()
		{ }

		protected SerializableDictionary(IEqualityComparer<TKey> comparer)
			: base(comparer)
		{ }

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