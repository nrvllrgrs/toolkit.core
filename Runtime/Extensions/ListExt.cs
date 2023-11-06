using System.Collections.Generic;
using UnityEngine;

public static class ListExt
{
	public static T Random<T>(this IList<T> list, bool remove = false)
	{
		int index = UnityEngine.Random.Range(0, list.Count);
		T value = list[index];

		if (remove)
		{
			list.RemoveAt(index);
		}

		return value;
	}

    public static IList<T> Shuffle<T>(this IList<T> list)
    {
		IList<T> result = new List<T>(list);

		int index = result.Count;
		while (index > 1)
		{
			--index;
			int randomIndex = UnityEngine.Random.Range(0, index + 1);

			T value = result[randomIndex];
			result[randomIndex] = result[index];
			result[index] = value;
		}

		return result;
	}

	public static void SetActive(this IEnumerable<GameObject> list, bool value)
	{
		foreach (var item in list)
		{
			item.SetActive(value);
		}
	}

	public static void SetEnabled(this IEnumerable<Behaviour> list, bool value)
    {
		foreach (var item in list)
        {
			item.enabled = value;
        }
    }
}