using UnityEngine;

public class VectorLabelAttribute : PropertyAttribute
{
	public string[] labels { get; private set; }

	public VectorLabelAttribute(params string[] args)
	{
		this.labels = args;
	}
}
