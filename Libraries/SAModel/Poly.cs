using System;
using System.Collections.Generic;
using System.Text;

namespace SAModel
{
	[Serializable]
	public sealed class Triangle : Poly
	{
		public Triangle()
		{
			Indexes = new ushort[3];
		}

		public Triangle(ushort a, ushort b, ushort c)
		{
			Indexes = new ushort[3];
			Indexes[0] = a;
			Indexes[1] = b;
			Indexes[2] = c;
		}

		public Triangle(byte[] file, int address)
			: this()
		{
			Indexes[0] = ByteConverter.ToUInt16(file, address);
			Indexes[1] = ByteConverter.ToUInt16(file, address + 2);
			Indexes[2] = ByteConverter.ToUInt16(file, address + 4);
		}

		public override Basic_PolyType PolyType => Basic_PolyType.Triangles;
	}

	[Serializable]
	public sealed class Quad : Poly
	{
		public Quad()
		{
			Indexes = new ushort[4];
		}

		public Quad(byte[] file, int address)
			: this()
		{
			Indexes[0] = ByteConverter.ToUInt16(file, address);
			Indexes[1] = ByteConverter.ToUInt16(file, address + 2);
			Indexes[2] = ByteConverter.ToUInt16(file, address + 4);
			Indexes[3] = ByteConverter.ToUInt16(file, address + 6);
		}

		public override Basic_PolyType PolyType => Basic_PolyType.Quads;
	}

	[Serializable]
	public sealed class Strip : Poly
	{
		public bool Reversed { get; private set; }

		public Strip(int NumVerts, bool Reverse)
		{
			Indexes = new ushort[NumVerts];
			Reversed = Reverse;
		}

		public Strip(ushort[] Verts, bool Reverse)
		{
			Indexes = Verts;
			Reversed = Reverse;
		}

		public Strip(byte[] file, int address)
		{
			Indexes = new ushort[ByteConverter.ToUInt16(file, address) & 0x7FFF];
			Reversed = (ByteConverter.ToUInt16(file, address) & 0x8000) == 0x8000;
			address += 2;
			for (int i = 0; i < Indexes.Length; i++)
			{
				Indexes[i] = ByteConverter.ToUInt16(file, address);
				address += 2;
			}
		}

		public override int Size => base.Size + 2;

		public override Basic_PolyType PolyType => Basic_PolyType.Strips;

		public override byte[] GetBytes()
		{
			List<byte> result = new List<byte>();
			result.AddRange(ByteConverter.GetBytes((ushort)(Indexes.Length | (Reversed ? 0x8000 : 0))));
			result.AddRange(base.GetBytes());
			return result.ToArray();
		}

		public override string ToStruct()
		{
			StringBuilder result = new StringBuilder();
			if (Reversed)
			{
				result.Append("0x8000u | ");
			}

			result.Append(Indexes.Length & 0x7FFF);
			result.Append(", ");
			result.Append(base.ToStruct());
			return result.ToString();
		}

		public override string ToNJA()
		{
			StringBuilder result = new StringBuilder();
			result.Append(base.ToNJA());
			return result.ToString();
		}
	}

	[Serializable]
	public abstract class Poly : ICloneable
	{
		public ushort[] Indexes { get; protected set; }

		internal Poly()
		{
		}

		public virtual int Size => Indexes.Length * 2;

		public abstract Basic_PolyType PolyType { get; }

		public virtual byte[] GetBytes()
		{
			List<byte> result = new List<byte>();
			foreach (ushort item in Indexes)
				result.AddRange(ByteConverter.GetBytes(item));
			return result.ToArray();
		}

		public virtual string ToStruct()
		{
			List<string> s = new List<string>(Indexes.Length);
			for (int i = 0; i < Indexes.Length; i++)
				s.Add(Indexes[i].ToString());
			return string.Join(", ", s.ToArray());
		}

		public virtual string ToNJA()
		{
			List<string> s = new List<string>(Indexes.Length);
			for (int i = 0; i < Indexes.Length; i++)
				s.Add(Indexes[i].ToString());
			return string.Join(", ", s.ToArray());
		}

		public static Poly CreatePoly(Basic_PolyType type)
		{
			return type switch
			{
				Basic_PolyType.Triangles => new Triangle(),
				Basic_PolyType.Quads => new Quad(),
				Basic_PolyType.NPoly or Basic_PolyType.Strips => throw new ArgumentException("Cannot create strip-type poly without additional information.\nUse Strip.Strip(int NumVerts, bool Reverse) instead.", nameof(type)),
				_ => throw new ArgumentException("Unknown poly type!", nameof(type))
			};
		}

		public static Poly CreatePoly(Basic_PolyType type, byte[] file, int address)
		{
			return type switch
			{
				Basic_PolyType.Triangles => new Triangle(file, address),
				Basic_PolyType.Quads => new Quad(file, address),
				Basic_PolyType.NPoly or Basic_PolyType.Strips => new Strip(file, address),
				_ => throw new ArgumentException("Unknown poly type!", nameof(type))
			};
		}

		object ICloneable.Clone() => Clone();

		public Poly Clone()
		{
			Poly result = (Poly)MemberwiseClone();
			Indexes = (ushort[])Indexes.Clone();
			return result;
		}
	}
}
