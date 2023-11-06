namespace UnityEngine
{
	public static class MathUtil
	{
		public static float GetPercent(float value, float min, float max)
		{
			return Mathf.Clamp01((value - min) / (max - min));
		}

		public static float Remap01(float value, float newMin, float newMax)
		{
			return newMin + value * (newMax - newMin);
		}

		public static float Remap(float value, float oldMin, float oldMax, float newMin, float newMax)
		{
			return newMin + (GetPercent(value, oldMin, oldMax) * (newMax - newMin));
		}

		public static float Mod(this float a, float b)
		{
			return (a % b + b) % b;
		}

		public static int Mod(this int a, int b)
		{
			if (b == 0)
				return -1;

			return (a % b + b) % b;
		}

		public static bool Between(this float value, float min, float max)
		{
			return min <= value && value <= max;
		}

		public static bool Between(this int value, int min, int max)
		{
			return min <= value && value <= max;
		}

		public static float Wrap(this float value, float maxDouble)
		{
			value = Mod(value, maxDouble);
			if (value > maxDouble * 0.5f)
			{
				value -= maxDouble;
			}
			return value;
		}

		public static float WrapEulerAngle(this float value)
		{
			return value.Wrap(360f);
		}

		public static Vector3 WrapEulerAngles(this Vector3 vector)
		{
			return new Vector3(vector.x.WrapEulerAngle(), vector.y.WrapEulerAngle(), vector.z.WrapEulerAngle());
		}

		public static float NextGaussian()
		{
			float v1, v2, s;
			do
			{
				v1 = 2.0f * Random.Range(0f, 1f) - 1.0f;
				v2 = 2.0f * Random.Range(0f, 1f) - 1.0f;
				s = v1 * v1 + v2 * v2;
			} while (s >= 1.0f || s == 0f);

			s = Mathf.Sqrt(-2.0f * Mathf.Log(s) / s);
			return v1 * s;
		}

		public static float NextGaussian(float mean, float stdDeviation)
		{
			return mean + NextGaussian() * stdDeviation;
		}
	}
}