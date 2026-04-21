namespace ToolkitEngine
{
	[System.Serializable]
    public class ReflectedFloat : ReflectedValue<float>
    {
        public ReflectedFloat(float value)
            : base(value) { }
    }

	[System.Serializable]
	public class ReflectedInt : ReflectedValue<int>
    {
        public ReflectedInt(int value)
            : base(value) { }
    }
}