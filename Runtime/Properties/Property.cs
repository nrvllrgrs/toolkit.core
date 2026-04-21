using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ToolkitEngine
{
	[Serializable]
	public abstract class BaseProperty
	{
		public abstract Type valueType { get; }

		public BaseProperty()
		{ }
	}

	[Serializable]
	public abstract class BaseProperty<T> : BaseProperty
	{
		[SerializeField]
		protected T m_value;

		public T value
		{
			get => m_value;
			set => m_value = value;
		}
		public override Type valueType => typeof(T);

		public BaseProperty()
			: base()
		{ }

		public BaseProperty(T value)
		{
			m_value = value;
		}
	}

	[Serializable]
	public abstract class BasePropertyList : BaseProperty
	{ }

	[Serializable]
	public abstract class BasePropertyList<T> : BasePropertyList
	{
		[SerializeField]
		protected List<T> m_value;

		public List<T> value
		{
			get => m_value;
			set => m_value = value;
		}
		public override Type valueType => typeof(List<T>);

		public BasePropertyList()
			: base()
		{ }

		public BasePropertyList(IEnumerable<T> value)
		{
			m_value = value?.ToList() ?? new List<T>();
		}
	}

	[Serializable]
	public class ColorProperty : BaseProperty<Color>
	{
		public ColorProperty()
			: base() { }

		public ColorProperty(Color value)
			: base(value) { }
	}

	[Serializable]
	public class BoolProperty : BaseProperty<bool>
	{
		public BoolProperty()
			: base() { }

		public BoolProperty(bool value)
			: base(value) { }
	}

	[Serializable]
	public class FloatProperty : BaseProperty<float>
	{
		public FloatProperty()
			: base() { }

		public FloatProperty(float value)
			: base(value) { }
	}

	[Serializable]
	public class IntProperty : BaseProperty<int>
	{
		public IntProperty()
			: base() { }

		public IntProperty(int value)
			: base(value) { }
	}

	[Serializable]
	public class GameObjectProperty : ObjectProperty<GameObject>
	{
		public GameObjectProperty()
			: base() { }

		public GameObjectProperty(GameObject value)
			: base(value) { }
	}

	[Serializable]
	public class ObjectProperty : ObjectProperty<UnityEngine.Object>
	{
		public ObjectProperty()
			: base() { }

		public ObjectProperty(UnityEngine.Object value)
			: base(value) { }
	}

	[Serializable]
	public abstract class ObjectProperty<T> : BaseProperty<T>
		where T : UnityEngine.Object
	{
		public ObjectProperty()
			: base() { }

		public ObjectProperty(T value)
			: base(value) { }
	}

	[Serializable]
	public class StringProperty : BaseProperty<string>
	{
		public StringProperty()
			: base() { }

		public StringProperty(string value)
			: base(value) { }
	}

	[Serializable]
	public class Vector2Property : BaseProperty<Vector2>
	{
		public Vector2Property()
			: base() { }

		public Vector2Property(Vector2 value)
			: base(value) { }
	}

	[Serializable]
	public class Vector2IntProperty : BaseProperty<Vector2Int>
	{
		public Vector2IntProperty()
			: base() { }

		public Vector2IntProperty(Vector2Int value)
			: base(value) { }
	}

	[Serializable]
	public class Vector3Property : BaseProperty<Vector3>
	{
		public Vector3Property()
			: base() { }

		public Vector3Property(Vector3 value)
			: base(value) { }
	}

	[Serializable]
	public class Vector3IntProperty : BaseProperty<Vector3Int>
	{
		public Vector3IntProperty()
	:		 base() { }

		public Vector3IntProperty(Vector3Int value)
			: base(value) { }
	}

	[Serializable]
	public class Vector4Property : BaseProperty<Vector4>
	{
		public Vector4Property()
			: base() { }

		public Vector4Property(Vector4 value)
			: base(value) { }
	}

	[Serializable]
	public class GenericObjectProperty : ObjectProperty<UnityEngine.Object>
	{
		[SerializeField]
		private string m_requiredType; // AssemblyQualifiedName

		public GenericObjectProperty()
			: base() { }

		public GenericObjectProperty(UnityEngine.Object value)
		{ }
	}
}