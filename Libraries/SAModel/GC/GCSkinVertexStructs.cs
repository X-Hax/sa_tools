using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SAModel.GC
{
	public class Vector3Short(short x, short y, short z)
	{
		public short X = x;
		public short Y = y;
		public short Z = z;

		public float XF = x / 255F;
		public float YF = y / 255F;
		public float ZF = z / 255F;

		public Vector3Short(byte[] file, int address) : this(ByteConverter.ToInt16(file, address), ByteConverter.ToInt16(file, address + 2), ByteConverter.ToInt16(file, address + 4))
		{
		}

		public byte[] GetBytes()
		{
			List<byte> result = [];

			result.AddRange(ByteConverter.GetBytes(X));
			result.AddRange(ByteConverter.GetBytes(Y));
			result.AddRange(ByteConverter.GetBytes(Z));

			return result.ToArray();
		}


		public string ToStruct()
		{
			var result = new StringBuilder("{ ");

			result.Append(X);
			result.Append(", ");
			result.Append(Y);
			result.Append(", ");
			result.Append(Z);
			result.Append(" }");

			return result.ToString();
		}

	}
	public class GCSkinVertexSetPosNrm
	{
		public Vector3Short pos;
		public Vector3Short nrm;

		public byte[] GetBytes()
		{
			List<byte> result = [];

			result.AddRange(pos.GetBytes());
			result.AddRange(nrm.GetBytes());

			return result.ToArray();
		}

		public string ToStruct()
		{
			var result = new StringBuilder("{ ");

			result.Append(pos.ToStruct());
			result.Append(", ");
			result.Append(nrm.ToStruct());
			result.Append(" }");

			return result.ToString();
		}

		public void ToNJA(TextWriter writer)
		{
			writer.WriteLine($"\tGJWPos( {pos.XF.ToNJA()}, {pos.YF.ToNJA()}, {pos.ZF.ToNJA()} ),");
			writer.WriteLine($"\tGJWNrm( {nrm.XF.ToNJA()}, {nrm.YF.ToNJA()}, {nrm.ZF.ToNJA()} ),");
		}
	}
	public class GCSkinVertexSetWeight
	{
		/// <summary>
		/// Only used if the elementType in the containing GCSkinVertexSet is 2, AKA PartialWeight. 
		/// </summary>
		public ushort vertIndex;
		public ushort weight;
		public byte[] GetBytes()
		{
			List<byte> result = [];

			result.AddRange(ByteConverter.GetBytes(vertIndex));
			result.AddRange(ByteConverter.GetBytes(weight));

			return result.ToArray();
		}

		public string ToStruct()
		{
			var result = new StringBuilder("{ ");

			result.Append(vertIndex);
			result.Append(", ");
			result.Append(weight);
			result.Append(" }");

			return result.ToString();
		}
		public void ToNJA(TextWriter writer, bool weightpower)
		{
			var translatedweight = weight / (weightpower? 65535.0F : 255.0F) * 100.0F;
			writer.WriteLine($"\tGJWIdx( {vertIndex}, {translatedweight.ToString("N6")} ),");
		}
	}
}
