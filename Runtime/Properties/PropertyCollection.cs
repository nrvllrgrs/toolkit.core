using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ToolkitEngine
{
	[System.Serializable]
    public class PropertyCollection : Dictionary<string, BaseProperty>, ISerializationCallbackReceiver
	{
		#region Fields

		[SerializeField, HideInInspector]
		protected List<string> keys = new();

		[SerializeReference, HideInInspector]
		protected List<BaseProperty> values = new();

		#endregion

		#region Property Methods

		public bool ContainsProperty<T>(string key)
		{
			if (!TryGetValue(key, out var property))
				return false;

			if (property.valueType == typeof(T))
				return true;

			// Handles GenericObjectProperty polymorphism
			return property is BaseProperty<Object> objProperty
				&& objProperty.value is T;
		}

		public T GetPropertyValue<T>(string key)
		{
			if (TryGetPropertyValue(key, out T value))
			{
				return value;
			}
			throw new System.ArgumentException($"Key {key} not found!");
		}

		public bool TryGetPropertyValue<T>(string key, out T value)
		{
			if (TryGetValue(key, out var property))
			{
				// Fast path: exact type match (covers all concrete property types)
				if (property.valueType == typeof(T))
				{
					value = (property as BaseProperty<T>).value;
					return true;
				}

				// Fallback: handles GenericObjectProperty (and any ObjectProperty<>
				// subclass) where valueType is a base type but the stored instance
				// is a more derived type compatible with T.
				if (property is BaseProperty<Object> objProperty
					&& objProperty.value is T typedValue)
				{
					value = typedValue;
					return true;
				}
			}
			value = default;
			return false;
		}

		public List<T> GetPropertyList<T>(string key)
		{
			if (TryGetPropertyList(key, out List<T> value))
			{
				return value;
			}
			throw new System.ArgumentException($"Key {key} not found!");
		}

		public bool TryGetPropertyList<T>(string key, out List<T> value)
		{
			if (TryGetValue(key, out var property))
			{
				if (property.valueType == typeof(List<T>))
				{
					value = (property as BasePropertyList<T>).value;
					return true;
				}
			}
			value = default;
			return false;
		}

#if UNITY_EDITOR

		public void SetProperty<T>(string key, BaseProperty<T> property, Object owner = null)
		{
			if (TryGetValue(key, out var storedProp)
				&& storedProp.valueType == typeof(T))
			{
				(storedProp as BaseProperty<T>).value = property.value;
			}
			else
			{
				Add(key, property);
			}

			if (owner != null)
			{
				EditorUtility.SetDirty(owner);
			}
		}

		public void SetProperty<T>(string key, BasePropertyList<T> property, Object owner = null)
		{
			if (TryGetValue(key, out var storedProp)
				&& storedProp.valueType == typeof(List<T>))
			{
				(storedProp as BasePropertyList<T>).value = property.value;
			}
			else
			{
				Add(key, property);
			}

			if (owner != null)
			{
				EditorUtility.SetDirty(owner);
			}
		}

		public void SetPropertyValue<T>(string key, T value, Object owner = null)
		{
			if (TrySetPropertyValue(key, value, owner))
			{
				return;
			}
			throw new System.ArgumentException($"Key {key} not found!");
		}

		public bool TrySetPropertyValue<T>(string key, T value, Object owner = null)
		{
			if (TryGetValue(key, out var property)
				&& property.valueType == typeof(T))
			{
				(property as BaseProperty<T>).value = value;
				if (owner != null)
				{
					EditorUtility.SetDirty(owner);
				}

				return true;
			}
			return false;
		}

		public void SetPropertyList<T>(string key, List<T> value, Object owner = null)
		{
			if (TrySetPropertyList(key, value, owner))
			{
				return;
			}
			throw new System.ArgumentException($"Key {key} not found!");
		}

		public bool TrySetPropertyList<T>(string key, List<T> value, Object owner = null)
		{
			if (TryGetValue(key, out var property)
				&& property.valueType == typeof(List<T>))
			{
				(property as BasePropertyList<T>).value = value;
				if (owner != null)
				{
					EditorUtility.SetDirty(owner);
				}

				return true;
			}
			return false;
		}
#endif
		#endregion

		#region Serialization Methods

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

		#endregion
	}
}