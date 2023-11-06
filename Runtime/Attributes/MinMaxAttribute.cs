using UnityEngine;

public class MinMaxAttribute : PropertyAttribute
{
	public float minValue { get; private set; }
	public float maxValue { get; private set; }
	public string minLabel { get; private set; }
	public string maxLabel { get; private set; }

	public MinMaxAttribute()
		: this(float.MinValue, float.MaxValue)
	{ }

	public MinMaxAttribute(float minValue, float maxValue)
		: this(minValue, maxValue, "Min", "Max")
	{ }

	public MinMaxAttribute(float minValue, float maxValue, string minLabel, string maxLabel)
	{
		this.minValue = minValue;
		this.maxValue = maxValue;
		this.minLabel = minLabel;
		this.maxLabel = maxLabel;
	}
}
