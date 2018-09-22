using System;
using SharpDX;

namespace SonicRetro.SAModel
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
				case ColorType.ARGB8888_32:
					return Color.FromRgba(ByteConverter.ToInt32(file, address));
				case ColorType.XRGB8888_32:
					return Color.FromRgba(unchecked((int)(ByteConverter.ToUInt32(file, address) | 0xFF000000u)));
				case ColorType.ARGB8888_16:
					return Color.FromRgba((ByteConverter.ToUInt16(file, address + 2) << 16) | ByteConverter.ToUInt16(file, address));
				case ColorType.XRGB8888_16:
					return Color.FromRgba(unchecked((int)((uint)((ByteConverter.ToUInt16(file, address + 2) << 16) | ByteConverter.ToUInt16(file, address)) | 0xFF000000u)));
				case ColorType.ARGB4444:
					ushort value = ByteConverter.ToUInt16(file, address);
					int a = value >> 12;
					int r = (value >> 8) & 0xF;
					int g = (value >> 4) & 0xF;
					int b = value & 0xF;
					return new Color(
						r | (r << 4),
						g | (g << 4),
						b | (b << 4),
						a | (a << 4)
						);
				case ColorType.RGB565:
					value = ByteConverter.ToUInt16(file, address);
					r = value >> 11;
					g = (value >> 5) & 0x3F;
					b = value & 0x1F;
					return new Color(
						r << 3 | r >> 2,
						g << 2 | g >> 4,
						b << 3 | b >> 2
						);
			}
			throw new ArgumentOutOfRangeException(nameof(type));
		}

		public static byte[] GetBytes(Color Color)
		{
			return GetBytes(Color, ColorType.ARGB8888_32);
		}

		public static byte[] GetBytes(Color color, ColorType type)
		{
			switch (type)
			{
				case ColorType.ARGB8888_32:
					return ByteConverter.GetBytes(color.ToRgba());
				case ColorType.XRGB8888_32:
					color.A = 0;
					goto case ColorType.ARGB8888_32;
				case ColorType.ARGB8888_16:
				{
					byte[] result = new byte[4];
					int i = color.ToRgba();
					ByteConverter.GetBytes((ushort)(i & 0xFFFF)).CopyTo(result, 0);
					ByteConverter.GetBytes((ushort)((i >> 16) & 0xFFFF)).CopyTo(result, 2);
					return result;
				}
				case ColorType.XRGB8888_16:
					color.A = 0;
					goto case ColorType.ARGB8888_16;
				case ColorType.ARGB4444:
					return ByteConverter.GetBytes((ushort)(((color.A >> 4) << 12) | ((color.R >> 4) << 8) | ((color.G >> 4) << 4) | (color.B >> 4)));
				case ColorType.RGB565:
					return ByteConverter.GetBytes((ushort)(((color.R >> 3) << 11) | ((color.G >> 2) << 5) | (color.B >> 3)));
			}
			throw new ArgumentOutOfRangeException(nameof(type));
		}

		public static string ToStruct(this Color color)
		{
			if (color == Color.Zero)
				return "{ 0 }";
			return "{ 0x" + color.ToRgba().ToString("X8") + " }";
		}

		public static int Size(ColorType type)
		{
			switch (type)
			{
				case ColorType.ARGB8888_32:
				case ColorType.XRGB8888_32:
				case ColorType.ARGB8888_16:
				case ColorType.XRGB8888_16:
					return 4;
				case ColorType.ARGB4444:
				case ColorType.RGB565:
					return 2;
			}
			throw new ArgumentOutOfRangeException(nameof(type));
		}
	}

	public enum ColorType
	{
		ARGB8888_32,
		XRGB8888_32,
		ARGB8888_16,
		XRGB8888_16,
		ARGB4444,
		RGB565
	}
}