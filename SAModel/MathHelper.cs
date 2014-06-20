using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonicRetro.SAModel
{
	public static class MathHelper
	{
		/// <summary>
		/// Constrains a value to be within the specified range.
		/// </summary>
		/// <param name="value">Input Value.</param>
		/// <param name="min">Minimum Value.</param>
		/// <param name="max">Maxiumum Value.</param>
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
		/// <param name="max">Maxiumum Value.</param>
		/// <returns>Value, passed through the min/max filter.</returns>
		public static float Clamp(float value, float min, float max)
		{
			return (value < min) ? min : (value > max) ? max : value;
		}
	}
}
