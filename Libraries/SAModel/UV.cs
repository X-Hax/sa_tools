using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace SAModel
{
	[TypeConverter(typeof (UVConverter))]
	[Serializable]
	public class UV : IEquatable<UV>, ICloneable
	{
		public double U { get; set; }
		public double V { get; set; }

		public static int Size
		{
			get { return 4; }
		}

		public UV()
		{
		}

		public UV(byte[] file, int address)
			: this(file, address, false)
		{
		}

		public UV(byte[] file, int address, bool UVH, bool chunk = false, bool xj = false)
		{
			// XJ uses actual floats as UVs
			if (xj)
			{
				U = ByteConverter.ToSingle(file, address);
				V = ByteConverter.ToSingle(file, address + 4);
			} 
			// "Reverse" is for the order used in SADX Gamecube
			else if (ByteConverter.Reverse || !ByteConverter.BigEndian || chunk)
			{
				U = ByteConverter.ToInt16(file, address) / (UVH ? 1024.0 : 256.0);
				V = ByteConverter.ToInt16(file, address + 2) / (UVH ? 1024.0 : 256.0);
			}
			else
			{
				V = ByteConverter.ToInt16(file, address) / (UVH ? 1024.0 : 256.0);
				U = ByteConverter.ToInt16(file, address + 2) / (UVH ? 1024.0 : 256.0);
			}
		}

		public UV(string data)
		{
			string[] uv = data.Split(',');
			U = short.Parse(uv[0], NumberStyles.Float, NumberFormatInfo.InvariantInfo);
			V = short.Parse(uv[1], NumberStyles.Float, NumberFormatInfo.InvariantInfo);
		}

		public UV(double u, double v)
		{
			U = u;
			V = v;
		}

		public byte[] GetBytes()
		{
			return GetBytes(false);
		}

		public byte[] GetBytes(bool UVH)
		{
			List<byte> result = new List<byte>();
			result.AddRange(ByteConverter.GetBytes((short)(U * (UVH ? 1024.0 : 256.0))));
			result.AddRange(ByteConverter.GetBytes((short)(V * (UVH ? 1024.0 : 256.0))));
			return result.ToArray();
		}

		public byte[] GetBytesXJ()
		{
			List<byte> result = new List<byte>();
			result.AddRange(ByteConverter.GetBytes(U));
			result.AddRange(ByteConverter.GetBytes(V));
			return result.ToArray();
		}

		public override string ToString()
		{
			return U.ToString(NumberFormatInfo.InvariantInfo) + ", " + V.ToString(NumberFormatInfo.InvariantInfo);
		}

		public string ToStruct()
		{
			if (U == 0 && V == 0)
				return "{ 0 }";
			return "{ " + (short)(U * 256.0) + ", " + (short)(V * 256.0) + " }";
		}

		public string ToNJA()
		{
			return "UV ( " + (short)(U * 256.0) + ", " + (short)(V * 256.0) + " )";
		}

		public override bool Equals(object obj)
		{
			if (obj is UV)
				return Equals((UV)obj);
			return false;
		}

		public override int GetHashCode()
		{
			return U.GetHashCode() ^ V.GetHashCode();
		}

		public bool Equals(UV other)
		{
			if (other == null)
				return false;
			return U == other.U && V == other.V;
		}

		object ICloneable.Clone() => Clone();

		public UV Clone() => (UV)MemberwiseClone();
	}

	public class UVConverter : ExpandableObjectConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(UV))
				return true;
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string) && value is UV)
				return ((UV)value).ToString();
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
				return new UV((string)value);
			return base.ConvertFrom(context, culture, value);
		}
	}
}