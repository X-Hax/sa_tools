using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SonicRetro.SAModel.GC
{
	[Serializable]
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

		public void Write(BinaryWriter writer, VertexAttribute attrib)
		{
			switch (attrib.DataType)
			{
				case GXDataType.Float32:
					writer.Write(X);
					writer.Write(Y);
					writer.Write(Z);
					break;
				case GXDataType.Unsigned16:
					writer.Write((ushort)(X * (1 << attrib.FractionalBitCount)));
					writer.Write((ushort)(Y * (1 << attrib.FractionalBitCount)));
					writer.Write((ushort)(Z * (1 << attrib.FractionalBitCount)));
					break;
				case GXDataType.Signed16:
					writer.Write((short)(X * (1 << attrib.FractionalBitCount)));
					writer.Write((short)(Y * (1 << attrib.FractionalBitCount)));
					writer.Write((short)(Z * (1 << attrib.FractionalBitCount)));
					break;
				case GXDataType.Unsigned8:
					writer.Write((byte)(X * (1 << attrib.FractionalBitCount)));
					writer.Write((byte)(Y * (1 << attrib.FractionalBitCount)));
					writer.Write((byte)(Z * (1 << attrib.FractionalBitCount)));
					break;
				case GXDataType.Signed8:
					writer.Write((sbyte)(X * (1 << attrib.FractionalBitCount)));
					writer.Write((sbyte)(Y * (1 << attrib.FractionalBitCount)));
					writer.Write((sbyte)(Z * (1 << attrib.FractionalBitCount)));
					break;
			}
		}
	}

	[Serializable]
	public struct Vector2
	{
		public float X { get; set; }
		public float Y { get; set; }

		public Vector2(float x, float y)
		{
			X = x;
			Y = y;
		}

		public void Write(BinaryWriter writer, VertexAttribute attrib)
		{
			switch (attrib.DataType)
			{
				case GXDataType.Float32:
					writer.Write(X);
					writer.Write(Y);
					break;
				case GXDataType.Unsigned16:
					writer.Write((ushort)(X * (1 << attrib.FractionalBitCount)));
					writer.Write((ushort)(Y * (1 << attrib.FractionalBitCount)));
					break;
				case GXDataType.Signed16:
					writer.Write((short)(X * 255f)); //(1 << attrib.FractionalBitCount)));
					writer.Write((short)(Y * 255f)); //(1 << attrib.FractionalBitCount)));
					break;
				case GXDataType.Unsigned8:
					writer.Write((byte)(X * (1 << attrib.FractionalBitCount)));
					writer.Write((byte)(Y * (1 << attrib.FractionalBitCount)));
					break;
				case GXDataType.Signed8:
					writer.Write((sbyte)(X * (1 << attrib.FractionalBitCount)));
					writer.Write((sbyte)(Y * (1 << attrib.FractionalBitCount)));
					break;
			}
		}
	}

	[Serializable]
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

		public void Write(BinaryWriter writer, VertexAttribute attrib)
		{
			switch (attrib.DataType)
			{
				case GXDataType.RGB8:
					writer.Write((byte)R);
					writer.Write((byte)G);
					writer.Write((byte)B);
					writer.Write((byte)255);
					break;
				case GXDataType.RGBA8:
					writer.Write((byte)R);
					writer.Write((byte)G);
					writer.Write((byte)B);
					writer.Write((byte)A);
					break;
			}
		}
	}
}
