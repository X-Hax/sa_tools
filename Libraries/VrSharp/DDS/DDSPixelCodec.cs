using System;
using VrSharp.Gvr;

namespace VrSharp.DDS
{
	public abstract class DDSPixelCodec : VrPixelCodec
	{
		#region Rgb565
		// Rgb565
		public class RGB : DDSPixelCodec
		{
			public override bool CanEncode
			{
				get { return true; }
			}

			public override int Bpp
			{
				get { return 16; }
			}

			public override int NumPixels
			{
				get { return 1; }
			}

			public override void DecodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
			{
				ushort pixel = BitConverter.ToUInt16(source, sourceIndex);

				destination[destinationIndex + 3] = 0xFF;
				destination[destinationIndex + 2] = (byte)(((pixel >> 11) & 0x1F) * 0xFF / 0x1F);
				destination[destinationIndex + 1] = (byte)(((pixel >> 5) & 0x3F) * 0xFF / 0x3F);
				destination[destinationIndex + 0] = (byte)(((pixel >> 0) & 0x1F) * 0xFF / 0x1F);
			}

			public override void EncodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
			{
				ushort pixel = 0x0000;
				pixel |= (ushort)((source[sourceIndex + 2] >> 3) << 11);
				pixel |= (ushort)((source[sourceIndex + 1] >> 2) << 5);
				pixel |= (ushort)((source[sourceIndex + 0] >> 3) << 0);

				destination[destinationIndex + 1] = (byte)(pixel & 0xFF);
				destination[destinationIndex + 0] = (byte)((pixel >> 8) & 0xFF);
			}
		}
		#endregion

		#region Rgb5a3
		// Rgb5a3
		public class RGBA : DDSPixelCodec
		{
			public override bool CanEncode
			{
				get { return true; }
			}

			public override int NumPixels
			{
				get { return 1; }
			}

			public override int Bpp
			{
				get { return 16; }
			}

			public override void DecodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
			{
				ushort pixel = BitConverter.ToUInt16(source, sourceIndex);

				if ((pixel & 0x8000) != 0) // Rgb565
				{
					destination[destinationIndex + 3] = 0xFF;
					destination[destinationIndex + 2] = (byte)(((pixel >> 11) & 0x1F) * 0xFF / 0x1F);
					destination[destinationIndex + 1] = (byte)(((pixel >> 5) & 0x3F) * 0xFF / 0x3F);
					destination[destinationIndex + 0] = (byte)(((pixel >> 0) & 0x1F) * 0xFF / 0x1F);
				}
				else // Argb4444
				{
					destination[destinationIndex + 3] = (byte)(((pixel >> 12) & 0x0F) * 0xFF / 0x0F);
					destination[destinationIndex + 2] = (byte)(((pixel >> 8) & 0x0F) * 0xFF / 0x0F);
					destination[destinationIndex + 1] = (byte)(((pixel >> 4) & 0x0F) * 0xFF / 0x0F);
					destination[destinationIndex + 0] = (byte)(((pixel >> 0) & 0x0F) * 0xFF / 0x0F);
				}
			}

			public override void EncodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
			{
				ushort pixel = 0x0000;

				if (source[sourceIndex + 3] <= 0xDA) // Argb3444
				{
					pixel |= (ushort)((source[sourceIndex + 3] >> 5) << 12);
					pixel |= (ushort)((source[sourceIndex + 2] >> 4) << 8);
					pixel |= (ushort)((source[sourceIndex + 1] >> 4) << 4);
					pixel |= (ushort)((source[sourceIndex + 0] >> 4) << 0);
				}
				else // Rgb555
				{
					pixel |= 0x8000;
					pixel |= (ushort)((source[sourceIndex + 2] >> 3) << 10);
					pixel |= (ushort)((source[sourceIndex + 1] >> 3) << 5);
					pixel |= (ushort)((source[sourceIndex + 0] >> 3) << 0);
				}

				destination[destinationIndex + 1] = (byte)(pixel & 0xFF);
				destination[destinationIndex + 0] = (byte)((pixel >> 8) & 0xFF);
			}
		}
		#endregion

		#region Get Codec
		public static DDSPixelCodec GetPixelCodec(DDSPixelFormat format)
		{
			switch (format)
			{
				case DDSPixelFormat.RGB:
					return new RGB();
				case DDSPixelFormat.RGBA:
					return new RGBA();
			}

			return null;
		}
		#endregion
	}
}
