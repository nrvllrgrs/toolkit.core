using System.Collections.Generic;
using System.Linq;

namespace UnityEngine
{
	public static class RandomUtil
	{
		public static Vector3 Noise(Vector3 variance)
		{
			return new Vector3(
				Random.Range(-variance.x, variance.x),
				Random.Range(-variance.y, variance.y),
				Random.Range(-variance.z, variance.z));
		}

		public static Vector2 RandomInSector(float half)
		{
			var u = Random.Range(-half, half) * Mathf.Deg2Rad;
			return new Vector2(Mathf.Sin(u), Mathf.Cos(u));
		}

		public static Vector3 RandomInSphericalSector(float halfHorizonal, float halfVertical)
		{
			// Calculate angles
			// ...where u is between -180 and 180 and v is between -90 and 90
			var u = Random.Range(-halfHorizonal, halfHorizonal) * Mathf.Deg2Rad;
			var v = Random.Range(-halfVertical, halfVertical) * Mathf.Deg2Rad;

			// Calculate position
			return new Vector3(
				Mathf.Sin(u) * Mathf.Sin(v),
				Mathf.Sin(v),
				Mathf.Cos(u) * Mathf.Sin(v));
		}

		public static int WeightedRandomIndex<T>(this IEnumerable<T> items, System.Func<T, float> getWeight)
		{
			float totalWeights = items.Select(x => getWeight(x)).Sum();
			float value = Random.Range(0f, totalWeights);

			int count = items.Count();
			for (int i = 0; i < count - 1; ++i)
			{
				var weight = getWeight(items.ElementAt(i));
				if (value < weight)
					return i;

				value -= weight;
			}
			return items.Count() - 1;
		}

		public static int WeightedRandomIndex(this IEnumerable<float> weights)
		{
			return WeightedRandomIndex(weights, (x) => x);
		}

		public static T WeightedRandom<T>(this IEnumerable<T> items, System.Func<T, float> getWeight)
		{
			int index = items.Select(x => getWeight(x)).WeightedRandomIndex();
			if (!index.Between(0, items.Count() - 1))
				return default;

			return items.ElementAt(index);
		}

		public static T WeightedRandom<T>(this IEnumerable<IWeightedItem<T>> items)
		{
			return WeightedRandom(items, (x) => x.weight).item;
		}

		public static T[] WeightedRandom<T>(this IEnumerable<IWeightedItem<T>> items, int count)
		{
			var result = new List<T>();
			var modifiedItems = new List<IWeightedItem<T>>(items);
			for (int i = 0; i < count; ++i)
			{
				int index = modifiedItems.Select(x => x.weight).WeightedRandomIndex();

				// Add selected item to result
				result.Add(modifiedItems[index].item);

				// Remove item at index so it is not selected again
				modifiedItems.RemoveAt(index);
			}

			return result.ToArray();
		}
	}

	public interface IWeightedItem<T>
	{
		T item { get; }
		float weight { get; }
	}
}