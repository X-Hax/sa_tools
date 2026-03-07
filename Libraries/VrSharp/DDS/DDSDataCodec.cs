using System;
using VrSharp.Gvr;
using static VrSharp.Gvr.GvrDataCodec;

namespace VrSharp.DDS
{
	public abstract class DDSDataCodec : VrDataCodec
	{
		/// <summary>
		/// Parts adapted from DDS GIMP plugin. Rework if necessary.
		/// Link: https://code.google.com/archive/p/gimp-dds/
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		private int Multiply8BitValue(int a, int b)
		{
			int t = a * b + 128;
			return ((t + (t >> 8)) >> 8);
		}
		private ushort DDSEncodeRGB565(int r, int g, int b)
		{
			return (ushort)((Multiply8BitValue(r, 31) << 11) |
				   (Multiply8BitValue(g, 63) << 5) |
				   (Multiply8BitValue(b, 31)));
		}
		#region RGB565
		public class RGB565 : DDSDataCodec
		{
			public override bool CanEncode
			{
				get { return true; }
			}

			public override int Bpp
			{
				get { return 16; }
			}

			public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
			{
				byte[] output = new byte[width * height * 4];
				for (int y = 0; y < (uint)(width * height); y++)
				{
					ushort pixel = BitConverter.ToUInt16(input, offset);
					output[(y * 4) + 3] = 0xFF;
					output[(y * 4) + 2] = (byte)(((pixel >> 11) & 0x1F) * 0xFF / 0x1F);
					output[(y * 4) + 1] = (byte)(((pixel >> 5) & 0x3F) * 0xFF / 0x3F);
					output[(y * 4) + 0] = (byte)(((pixel >> 0) & 0x1F) * 0xFF / 0x1F);
					offset += 2;
				}
				return output;
			}
			public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
			{
				int offset = 0;
				byte[] output = new byte[width * height * 2];
				byte r = 0;
				byte g = 0;
				byte b = 0;
				for (int i = 0; i < width * height; i++)
				{
					ushort pixel = 0x0000;
					b = input[(4 * i) + 0];
					g = input[(4 * i) + 1];
					r = input[(4 * i) + 2];
					pixel = DDSEncodeRGB565(r, g, b);
					BitConverter.GetBytes(pixel).CopyTo(output, offset);
					offset += 2;
				}
				return output;
			}
		}
		#endregion
		private ushort DDSEncodeRGBA1555(int r, int g, int b, int a)
		{
			return (ushort)((((a >> 7) & 0x01) << 15) |
				   (Multiply8BitValue(r, 31) << 10) |
				   (Multiply8BitValue(g, 31) << 5) |
				   (Multiply8BitValue(b, 31)));
		}
		#region ARGB1555
		public class ARGB1555 : DDSDataCodec
		{
			public override bool CanEncode
			{
				get { return true; }
			}

			public override int Bpp
			{
				get { return 16; }
			}

			public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
			{
				byte[] output = new byte[width * height * 4];
				for (int y = 0; y < (uint)(width * height); y++)
				{
					ushort pixel = BitConverter.ToUInt16(input, offset);
					output[(y * 4) + 3] = (byte)(((pixel >> 15) & 0x01) * 0xFF);
					output[(y * 4) + 2] = (byte)(((pixel >> 10) & 0x1F) * 0xFF / 0x1F);
					output[(y * 4) + 1] = (byte)(((pixel >> 5) & 0x1F) * 0xFF / 0x1F);
					output[(y * 4) + 0] = (byte)(((pixel >> 0) & 0x1F) * 0xFF / 0x1F);
					offset += 2;
				}
				return output;
			}
			public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
			{
				int offset = 0;
				byte[] output = new byte[width * height * 2];
				byte r = 0;
				byte g = 0;
				byte b = 0;
				byte a = 0;
				for (int i = 0; i < width * height; i++)
				{
					ushort pixel = 0x0000;
					b = input[(4 * i) + 0];
					g = input[(4 * i) + 1];
					r = input[(4 * i) + 2];
					a = input[(4 * i) + 3];
					pixel = DDSEncodeRGBA1555(r, g, b, a);
					BitConverter.GetBytes(pixel).CopyTo(output, offset);
					offset += 2;
				}

				return output;
			}
		}
		#endregion
		private ushort DDSEncodeRGBA4444(int r, int g, int b, int a)
		{
			return (ushort)((Multiply8BitValue(a, 15) << 12) |
				   (Multiply8BitValue(r, 15) << 8) |
				   (Multiply8BitValue(g, 15) << 4) |
				   (Multiply8BitValue(b, 15)));
		}
		#region ARGB4444
		public class ARGB4444 : DDSDataCodec
		{
			public override bool CanEncode
			{
				get { return true; }
			}

			public override int Bpp
			{
				get { return 16; }
			}

			public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
			{
				byte[] output = new byte[width * height * 4];
				for (int y = 0; y < width * height; y++)
				{
					ushort pixel = BitConverter.ToUInt16(input, offset);
					output[(y * 4) + 3] = (byte)(((pixel >> 12) & 0xF) * 0xFF / 0xF);
					output[(y * 4) + 2] = (byte)(((pixel >> 8) & 0xF) * 0xFF / 0xF);
					output[(y * 4) + 1] = (byte)(((pixel >> 4) & 0xF) * 0xFF / 0xF);
					output[(y * 4) + 0] = (byte)(((pixel >> 0) & 0xF) * 0xFF / 0xF);
					offset += 2;
				}
				return output;
			}
			public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
			{
				int offset = 0;
				byte[] output = new byte[width * height * 2];

				byte r = 0;
				byte g = 0;
				byte b = 0;
				byte a = 0;
				for (int i = 0; i < width * height; i++)
				{
					ushort pixel = 0x0000;
					b = input[(4 * i) + 0];
					g = input[(4 * i) + 1];
					r = input[(4 * i) + 2];
					a = input[(4 * i) + 3];
					pixel = DDSEncodeRGBA4444(r, g, b, a);
					BitConverter.GetBytes(pixel).CopyTo(output, offset);
					offset += 2;
				}

				return output;
			}
		}
		#endregion
		#region ARGB8888
		// Argb8888
		public class ARGB8888 : DDSDataCodec
		{
			public override bool CanEncode
			{
				get { return true; }
			}

			public override int Bpp
			{
				get { return 32; }
			}

			public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
			{
				byte[] output = new byte[width * height * 4];
				for (int y = 0; y < width * height; y++)
				{
					uint pixel = BitConverter.ToUInt32(input, offset);
					output[(y * 4) + 3] = (byte)(pixel >> 24);
					output[(y * 4) + 2] = (byte)(pixel >> 16);
					output[(y * 4) + 1] = (byte)(pixel >> 8);
					output[(y * 4) + 0] = (byte)(pixel >> 0);
					offset += 4;
				}
				return output;
			}

			public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
			{
				int offset = 0;
				byte[] output = new byte[width * height * 4];
				for (int y = 0; y < width * height; y++)
				{
					uint pixel = BitConverter.ToUInt32(input, offset);
					output[offset + 0] = input[(y * 4) + 0];
					output[offset + 1] = input[(y * 4) + 1];
					output[offset + 2] = input[(y * 4) + 2];
					output[offset + 3] = input[(y * 4) + 3];
					offset += 4;
				}
				return output;
			}
		}
		#endregion
		#region Get Codec
		public static DDSDataCodec GetDataCodec(DDSPixelBitFormat format)
		{
			switch (format)
			{
				case DDSPixelBitFormat.RGB565:
					return new RGB565();
				case DDSPixelBitFormat.ARGB1555:
					return new ARGB1555();
				case DDSPixelBitFormat.ARGB4444:
					return new ARGB4444();
				case DDSPixelBitFormat.ARGB8888:
					return new ARGB8888();
			}

			return null;
		}
		#endregion
	}
}
