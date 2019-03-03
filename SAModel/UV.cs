using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace SonicRetro.SAModel
{
	[TypeConverter(typeof (UVConverter))]
	[Serializable]
	public class UV : IEquatable<UV>, ICloneable
	{
		public float U { get; set; }
		public float V { get; set; }

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

		public UV(byte[] file, int address, bool UVH)
		{
			U = ByteConverter.ToInt16(file, address) / (UVH ? 1023f : 255f);
			V = ByteConverter.ToInt16(file, address + 2) / (UVH ? 1023f : 255f);
		}

		public UV(string data)
		{
			string[] uv = data.Split(',');
			U = short.Parse(uv[0], NumberStyles.Float, NumberFormatInfo.InvariantInfo);
			V = short.Parse(uv[1], NumberStyles.Float, NumberFormatInfo.InvariantInfo);
		}

		public byte[] GetBytes()
		{
			return GetBytes(false);
		}

		public byte[] GetBytes(bool UVH)
		{
			List<byte> result = new List<byte>();
			result.AddRange(ByteConverter.GetBytes((short)(U * (UVH ? 1023f : 255f))));
			result.AddRange(ByteConverter.GetBytes((short)(V * (UVH ? 1023f : 255f))));
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
			return "{ " + (short)(U * 255f) + ", " + (short)(V * 255f) + " }";
		}

		public string ToNJA()
		{
			if (U == 0 && V == 0)
				return "{ 0 }";
			return "UV ( " + (short)(U * 255f) + ", " + (short)(V * 255f) + " )";
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