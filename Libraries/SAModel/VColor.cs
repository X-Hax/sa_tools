using System;
using System.Drawing;

namespace SAModel
{
	public static class VColor
	{
		public static Color FromBytes(byte[] file, int address)
		{
			return FromBytes(file, address, ColorType.ARGB8888_32);
		}

		public static Color FromBytes(byte[] file, int address, ColorType type)
		{
			switch (type)
			{
				case ColorType.RGBA8888_32:
					if (address > file.Length - 4) return Color.FromArgb(0, 0, 0, 0);
					return Color.FromArgb(file[address + 3], file[address], file[address + 1], file[address + 2]);
				case ColorType.ARGB8888_32:
					if (address > file.Length - 4) return Color.FromArgb(0, 0, 0, 0);
					// "Reverse" mode is for SADX Gamecube/SA2B/SA2PC where the color order is ABGR
					if (ByteConverter.BigEndian)
					{
						if (ByteConverter.Reverse)
							return Color.FromArgb(file[address + 3], file[address], file[address + 1], file[address + 2]);
						else
							return Color.FromArgb(file[address], file[address + 1], file[address + 2], file[address + 3]);
					}
					else if (ByteConverter.Reverse)
						return Color.FromArgb(file[address], file[address + 3], file[address + 2], file[address + 1]);
					else
						return Color.FromArgb(file[address + 3], file[address + 2], file[address + 1], file[address]);
				case ColorType.XRGB8888_32:
					return Color.FromArgb(unchecked((int)(ByteConverter.ToUInt32(file, address) | 0xFF000000u)));
				case ColorType.ARGB8888_16:
						return Color.FromArgb((ByteConverter.ToUInt16(file, address + 2) << 16) | ByteConverter.ToUInt16(file, address));
				case ColorType.XRGB8888_16:
					return Color.FromArgb(unchecked((int)((uint)((ByteConverter.ToUInt16(file, address + 2) << 16) | ByteConverter.ToUInt16(file, address)) | 0xFF000000u)));
				case ColorType.ARGB4444:
					ushort value = ByteConverter.ToUInt16(file, address);
					int a = value >> 12;
					int r = (value >> 8) & 0xF;
					int g = (value >> 4) & 0xF;
					int b = value & 0xF;
					return Color.FromArgb(
						a | (a << 4),
						r | (r << 4),
						g | (g << 4),
						b | (b << 4)
						);
				case ColorType.RGB565:
					value = ByteConverter.ToUInt16(file, address);
					r = value >> 11;
					g = (value >> 5) & 0x3F;
					b = value & 0x1F;
					return Color.FromArgb(
						r << 3 | r >> 2,
						g << 2 | g >> 4,
						b << 3 | b >> 2
						);
			}
			throw new ArgumentOutOfRangeException("type");
		}

		public static byte[] GetBytes(Color Color)
		{
			return GetBytes(Color, ColorType.ARGB8888_32);
		}

		public static byte[] GetBytes(Color color, ColorType type)
		{
			switch (type)
			{
				case ColorType.RGBA8888_32:
					return new byte[] { color.R, color.G, color.B, color.A};
				case ColorType.ARGB8888_32:
					return ByteConverter.GetBytes(color.ToArgb());
				case ColorType.XRGB8888_32:
					color = Color.FromArgb(0, color);
					goto case ColorType.ARGB8888_32;
				case ColorType.ARGB8888_16:
				{
					byte[] result = new byte[4];
					int i = color.ToArgb();
					ByteConverter.GetBytes((ushort)(i & 0xFFFF)).CopyTo(result, 0);
					ByteConverter.GetBytes((ushort)((i >> 16) & 0xFFFF)).CopyTo(result, 2);
					return result;
				}
				case ColorType.XRGB8888_16:
					color = Color.FromArgb(0, color);
					goto case ColorType.ARGB8888_16;
				case ColorType.ARGB4444:
					return ByteConverter.GetBytes((ushort)(((color.A >> 4) << 12) | ((color.R >> 4) << 8) | ((color.G >> 4) << 4) | (color.B >> 4)));
				case ColorType.RGB565:
					return ByteConverter.GetBytes((ushort)(((color.R >> 3) << 11) | ((color.G >> 2) << 5) | (color.B >> 3)));
			}
			throw new ArgumentOutOfRangeException("type");
		}

		public static string ToStruct(this Color color)
		{
			if (color == Color.Empty)
				return "{ 0 }";
			return "{ 0x" + color.ToArgb().ToString("X8") + " }";
		}

		public static string ToNJA(this Color color)
		{
			if (color == Color.Empty)
				return "ARGB ( 0, 0, 0, 0)";
			return "ARGB ( " + color.A.ToString() + ", " + color.R.ToString() + ", " + color.G.ToString() + ", " + color.B.ToString() + ")";
		}

		public static int Size(ColorType type)
		{
			switch (type)
			{
				case ColorType.ARGB8888_32:
				case ColorType.RGBA8888_32:
				case ColorType.XRGB8888_32:
				case ColorType.ARGB8888_16:
				case ColorType.XRGB8888_16:
					return 4;
				case ColorType.ARGB4444:
				case ColorType.RGB565:
					return 2;
			}
			throw new ArgumentOutOfRangeException("type");
		}
	}

	public enum ColorType
	{
		RGBA8888_32,
		ARGB8888_32,
		XRGB8888_32,
		ARGB8888_16,
		XRGB8888_16,
		ARGB4444,
		RGB565
	}
}