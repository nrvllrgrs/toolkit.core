using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

namespace ToolkitEngine
{
	[Serializable]
	public abstract class UnityElector
	{
		#region Enumerators

		public enum SelectionMode
		{
			Highest,
			Count,
			Percent,
		}

		public enum RandomnessMode
		{
			Random,
			Weighted,
		}

		#endregion
	}

	[Serializable]
	public class UnityElector<T> : UnityElector
	{
		#region Fields

		[SerializeField]
		private SelectionMode m_mode;

		[SerializeField, Min(0), Tooltip("Number of items with highest scores to be randomly selected from.")]
		private int m_count = 3;

		[SerializeField, Range(0f, 1f), Tooltip("Percent of items with highest scored to be randomly selected from.")]
		private float m_percent = 0.05f;

		[SerializeField]
		private RandomnessMode m_randomnessMode;

		[SerializeField]
		private UnityEvaluator m_score = new();

		#endregion

		#region Methods

		public bool TryElect(IEnumerable<T> items, GameObject actor, Func<T, GameObject> getTarget, out T value, out float score)
		{
			return TryElect(items, actor, getTarget, GetWeight, out value, out score);
		}

		public bool TryElect(IEnumerable<T> items, GameObject actor, Func<T, GameObject> getTarget, Func<T, float> getWeight, out T value, out float score)
		{
			if (items != null && items.Count() > 0)
			{
				if (m_mode == SelectionMode.Highest)
				{
					score = default;
					value = default;

					foreach (var item in items)
					{
						float t = m_score.Evaluate(actor, getTarget(item)) * getWeight(item);
						if (t > score)
						{
							score = t;
							value = item;
						}
					}

					if (score != float.NegativeInfinity)
						return true;
				}
				else
				{
					return TryEvaluateByScoredItems(items, actor, getTarget, getWeight, out value, out score);
				}
			}

			value = default;
			score = default;
			return false;
		}

		private bool TryEvaluateByScoredItems(IEnumerable<T> items, GameObject actor, Func<T, GameObject> getTarget, Func<T, float> getWeight, out T value, out float score)
		{
			var scoredItems = new List<Tuple<T, float>>();
			foreach (var item in items)
			{
				scoredItems.Add(new Tuple<T, float>(
					item,
					m_score.Evaluate(actor, getTarget(item)) * getWeight(item)));
			}

			if (scoredItems.Count > 0)
			{
				int count = 0;
				switch (m_mode)
				{
					case SelectionMode.Count:
						count = Mathf.Min(m_count, scoredItems.Count - 1);
						break;

					case SelectionMode.Percent:
						count = Mathf.RoundToInt(m_percent * scoredItems.Count);
						break;
				}

				var orderedItems = scoredItems.OrderByDescending(x => x.Item2)
					.Take(Mathf.Max(1, count))
					.ToArray();

				if (m_randomnessMode == RandomnessMode.Random)
				{
					var tuple = orderedItems[Random.Range(0, orderedItems.Length)];
					value = tuple.Item1;
					score = tuple.Item2;
					return true;
				}
				else if (m_randomnessMode == RandomnessMode.Weighted)
				{
					var tuple = orderedItems.WeightedRandom(x => x.Item2);
					value = tuple.Item1;
					score = tuple.Item2;
					return true;
				}
			}

			value = default;
			score = default;
			return false;
		}

		private float GetWeight(T item) => 1f;

		#endregion
	}
}