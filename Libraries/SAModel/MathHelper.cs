namespace SonicRetro.SAModel
{
	public static class MathHelper
	{
		public static readonly float Pi = 3.14159265358979323846f;
		public static readonly float Deg2Rad = (Pi / 180.0f);
		public static readonly float Rad2Deg = (180.0f / Pi);

		/// <summary>
		/// Constrains a value to be within the specified range.
		/// </summary>
		/// <param name="value">Input Value.</param>
		/// <param name="min">Minimum Value.</param>
		/// <param name="max">Maximum Value.</param>
		/// <returns>Value, passed through the min/max filter.</returns>
		public static int Clamp(int value, int min, int max)
		{
			return (value < min) ? min : (value > max) ? max : value;
		}

		/// <summary>
		/// Constrains a value to be within the specified range.
		/// </summary>
		/// <param name="value">Input Value.</param>
		/// <param name="min">Minimum Value.</param>
		/// <param name="max">Maximum Value.</param>
		/// <returns>Value, passed through the min/max filter.</returns>
		public static float Clamp(float value, float min, float max)
		{
			return (value < min) ? min : (value > max) ? max : value;
		}

		/// <summary>
		/// Linear interpolation between two values
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <param name="tValue"></param>
		/// <returns></returns>
		public static float Lerp(float min, float max, float tValue)
		{
			if (tValue == 0)
				return min;
			if (tValue == 1)
				return max;

			return min + (max - min) * tValue;
		}
	}
}