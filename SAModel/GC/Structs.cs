using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonicRetro.SAModel.GC
{
	public struct Vector3
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }

		public Vector3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}
	}

	public struct Vector2
	{
		public float X { get; set; }
		public float Y { get; set; }

		public Vector2(float x, float y)
		{
			X = x;
			Y = y;
		}
	}

	public struct Color
	{
		public float R { get; set; }
		public float G { get; set; }
		public float B { get; set; }
		public float A { get; set; }

		public Color(float r, float g, float b, float a)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}
	}
}
