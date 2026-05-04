using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine
{
	[Serializable, GenericMenuCategory("Lists")]
	public class BoolPropertyList : BasePropertyList<bool>
	{
		public BoolPropertyList()
			: base() { }

		public BoolPropertyList(IEnumerable<bool> value)
			: base(value) { }
	}

	[Serializable, GenericMenuCategory("Lists")]
	public class ColorPropertyList : BasePropertyList<Color>
	{
		public ColorPropertyList()
			: base() { }

		public ColorPropertyList(IEnumerable<Color> value)
			: base(value) { }
	}

	[Serializable, GenericMenuCategory("Lists")]
	public class FloatPropertyList : BasePropertyList<float>
	{
		public FloatPropertyList()
			: base() { }

		public FloatPropertyList(IEnumerable<float> value)
			: base(value) { }
	}

	[Serializable, GenericMenuCategory("Lists")]
	public class IntPropertyList : BasePropertyList<int>
	{
		public IntPropertyList()
			: base() { }

		public IntPropertyList(IEnumerable<int> value)
			: base(value) { }
	}

	[Serializable, GenericMenuCategory("Lists")]
	public class GameObjectPropertyList : ObjectPropertyList<GameObject>
	{
		public GameObjectPropertyList()
			: base() { }

		public GameObjectPropertyList(IEnumerable<GameObject> value)
			: base(value) { }
	}

	[Serializable, GenericMenuCategory("Lists")]
	public class ObjectPropertyList : ObjectPropertyList<UnityEngine.Object>
	{
		public ObjectPropertyList()
			: base() { }

		public ObjectPropertyList(IEnumerable<UnityEngine.Object> value)
			: base(value) { }
	}

	[Serializable, GenericMenuCategory("Lists")]
	public abstract class ObjectPropertyList<T> : BasePropertyList<T>
		where T : UnityEngine.Object
	{
		public ObjectPropertyList()
			: base() { }

		public ObjectPropertyList(IEnumerable<T> value)
			: base(value) { }
	}

	[Serializable, GenericMenuCategory("Lists")]
	public class StringPropertyList : BasePropertyList<string>
	{
		public StringPropertyList()
			: base() { }

		public StringPropertyList(IEnumerable<string> value)
			: base(value) { }
	}

	[Serializable, GenericMenuCategory("Lists")]
	public class Vector2PropertyList : BasePropertyList<Vector2>
	{
		public Vector2PropertyList()
			: base() { }

		public Vector2PropertyList(IEnumerable<Vector2> value)
			: base(value) { }
	}

	[Serializable, GenericMenuCategory("Lists")]
	public class Vector2IntPropertyList : BasePropertyList<Vector2Int>
	{
		public Vector2IntPropertyList()
			: base() { }

		public Vector2IntPropertyList(IEnumerable<Vector2Int> value)
			: base(value) { }
	}

	[Serializable, GenericMenuCategory("Lists")]
	public class Vector3PropertyList : BasePropertyList<Vector3>
	{
		public Vector3PropertyList()
			: base() { }

		public Vector3PropertyList(IEnumerable<Vector3> value)
			: base(value) { }
	}

	[Serializable, GenericMenuCategory("Lists")]
	public class Vector3IntPropertyList : BasePropertyList<Vector3Int>
	{
		public Vector3IntPropertyList()
	: base() { }

		public Vector3IntPropertyList(IEnumerable<Vector3Int> value)
			: base(value) { }
	}

	[Serializable, GenericMenuCategory("Lists")]
	public class Vector4PropertyList : BasePropertyList<Vector4>
	{
		public Vector4PropertyList()
			: base() { }

		public Vector4PropertyList(IEnumerable<Vector4> value)
			: base(value) { }
	}
}
