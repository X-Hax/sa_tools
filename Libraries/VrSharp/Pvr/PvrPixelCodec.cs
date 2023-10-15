using System;
using System.Drawing;

namespace VrSharp.Pvr
{
    public abstract class PvrPixelCodec : VrPixelCodec
    {
        #region Argb1555
        // Argb1555
        public class Argb1555 : PvrPixelCodec
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

                destination[destinationIndex + 3] = (byte)(((pixel >> 15) & 0x01) * 0xFF);
                destination[destinationIndex + 2] = (byte)(((pixel >> 10) & 0x1F) * 0xFF / 0x1F);
                destination[destinationIndex + 1] = (byte)(((pixel >> 5) & 0x1F) * 0xFF / 0x1F);
                destination[destinationIndex + 0] = (byte)(((pixel >> 0) & 0x1F) * 0xFF / 0x1F);
            }

            public override void EncodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
            {
                ushort pixel = 0x0000;
                pixel |= (ushort)((source[sourceIndex + 3] >> 7) << 15);
                pixel |= (ushort)((source[sourceIndex + 2] >> 3) << 10);
                pixel |= (ushort)((source[sourceIndex + 1] >> 3) << 5);
                pixel |= (ushort)((source[sourceIndex + 0] >> 3) << 0);

                destination[destinationIndex + 1] = (byte)((pixel >> 8) & 0xFF);
                destination[destinationIndex + 0] = (byte)(pixel & 0xFF);
            }
        }
        #endregion

        #region Rgb565
        // Rgb565
        public class Rgb565 : PvrPixelCodec
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
				// In some textures (sky2.pvr in KATANA SDK 2.00J) the PVRT chunk size is incorrect
				//if (sourceIndex > source.Length - 4)
					//sourceIndex -= 4;
				ushort pixel = BitConverter.ToUInt16(source, sourceIndex);

                destination[destinationIndex + 3] = 0xFF;
                destination[destinationIndex + 2] = (byte)(((pixel >> 11) & 0x1F) * 0xFF / 0x1F);
                destination[destinationIndex + 1] = (byte)(((pixel >> 5)  & 0x3F) * 0xFF / 0x3F);
                destination[destinationIndex + 0] = (byte)(((pixel >> 0)  & 0x1F) * 0xFF / 0x1F);
            }

            public override void EncodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
            {
                ushort pixel = 0x0000;
                pixel |= (ushort)((source[sourceIndex + 2] >> 3) << 11);
                pixel |= (ushort)((source[sourceIndex + 1] >> 2) << 5);
                pixel |= (ushort)((source[sourceIndex + 0] >> 3) << 0);

                destination[destinationIndex + 1] = (byte)((pixel >> 8) & 0xFF);
                destination[destinationIndex + 0] = (byte)(pixel & 0xFF);
            }
        }
        #endregion

        #region Argb4444
        // Argb4444
        public class Argb4444 : PvrPixelCodec
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
				// In some textures (yhappa.pvr in KATANA SDK 2.00J) the PVRT chunk size is incorrect
				//if (sourceIndex > source.Length - 4)
					//sourceIndex -= 4;
                ushort pixel = BitConverter.ToUInt16(source, sourceIndex);

                destination[destinationIndex + 3] = (byte)(((pixel >> 12) & 0x0F) * 0xFF / 0x0F);
                destination[destinationIndex + 2] = (byte)(((pixel >> 8)  & 0x0F) * 0xFF / 0x0F);
                destination[destinationIndex + 1] = (byte)(((pixel >> 4)  & 0x0F) * 0xFF / 0x0F);
                destination[destinationIndex + 0] = (byte)(((pixel >> 0)  & 0x0F) * 0xFF / 0x0F);
            }

            public override void EncodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
            {
                ushort pixel = 0x0000;
                pixel |= (ushort)((source[sourceIndex + 3] >> 4) << 12);
                pixel |= (ushort)((source[sourceIndex + 2] >> 4) << 8);
                pixel |= (ushort)((source[sourceIndex + 1] >> 4) << 4);
                pixel |= (ushort)((source[sourceIndex + 0] >> 4) << 0);

                destination[destinationIndex + 1] = (byte)((pixel >> 8) & 0xFF);
                destination[destinationIndex + 0] = (byte)(pixel & 0xFF);
            }
        }
		#endregion

		// Stolen from Shenmue Modding SDK https://github.com/Shenmue-Mods/VrSharp/blob/master/Files/Images/_PVRT/PvrFormats.cs
		#region Yuv422
		public class Yuv422 : PvrPixelCodec
		{
			public override bool CanEncode
			{
				get { return true; }
			}

			public override int Bpp
			{
				get { return 32; } //Using 32 Bpp because YUV requires two pixels to decode one pixel
			}

			public override int NumPixels
			{
				get { return 2; }
			}

			public override void DecodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
			{
				ushort pixel1 = BitConverter.ToUInt16(source, sourceIndex);
				ushort pixel2 = BitConverter.ToUInt16(source, sourceIndex + 2);

				int Y0 = (pixel1 & 0xFF00) >> 8, U = (pixel1 & 0x00FF);
				int Y1 = (pixel2 & 0xFF00) >> 8, V = (pixel2 & 0x00FF);

				byte r1 = MathExtensions.ClampByte((int)(Y0 + 1.375 * (V - 128.0)));
				byte g1 = MathExtensions.ClampByte((int)(Y0 - 0.6875 * (V - 128.0) - 0.34375 * (U - 128.0)));
				byte b1 = MathExtensions.ClampByte((int)(Y0 + 1.71875 * (U - 128.0)));

				byte r2 = MathExtensions.ClampByte((int)(Y1 + 1.375 * (V - 128.0)));
				byte g2 = MathExtensions.ClampByte((int)(Y1 - 0.6875 * (V - 128.0) - 0.34375 * (U - 128.0)));
				byte b2 = MathExtensions.ClampByte((int)(Y1 + 1.71875 * (U - 128.0)));

				destination[destinationIndex + 3] = 0xFF;
				destination[destinationIndex + 2] = r1;
				destination[destinationIndex + 1] = g1;
				destination[destinationIndex + 0] = b1;

				destination[destinationIndex + 4 + 3] = 0xFF;
				destination[destinationIndex + 4 + 2] = r2;
				destination[destinationIndex + 4 + 1] = g2;
				destination[destinationIndex + 4 + 0] = b2;
			}

			public override void EncodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
			{
				byte r1 = source[sourceIndex + 2];
				byte g1 = source[sourceIndex + 1];
				byte b1 = source[sourceIndex];

				byte r2 = source[sourceIndex + 4 + 2];
				byte g2 = source[sourceIndex + 4 + 1];
				byte b2 = source[sourceIndex + 4];

				uint Y0 = (uint)(0.299 * r1 + 0.587 * r2 + 0.114 * b1);
				uint Y1 = (uint)(0.299 * r2 + 0.587 * g2 + 0.114 * b2);

				byte r = (byte)((r2 + r1) / 2);
				byte g = (byte)((g2 + g1) / 2);
				byte b = (byte)((b2 + b1) / 2);

				uint U = (uint)(128.0f - 0.14f * r - 0.29f * g + 0.43f * b);
				uint V = (uint)(128.0f + 0.36f * r - 0.29f * g - 0.07f * b);

				ushort pixel1 = (ushort)((Y0 << 8) | U);
				ushort pixel2 = (ushort)((Y1 << 8) | V);

				destination[destinationIndex + 3] = (byte)((pixel2 >> 8) & 0xFF);
				destination[destinationIndex + 2] = (byte)(pixel2 & 0xFF);
				destination[destinationIndex + 1] = (byte)((pixel1 >> 8) & 0xFF);
				destination[destinationIndex] = (byte)(pixel1 & 0xFF);
			}
		}
		#endregion

		#region Bump
		public class Bump88 : PvrPixelCodec
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
				byte r = source[sourceIndex];
				byte s = source[sourceIndex + 1];

				double rAngle = r / 255.0 * 360.0;
				double sAngle = s / 255.0 * 90.0;

				double rRadian = VrSharp.MathHelper.DegreesToRadians(rAngle);
				double sRadian = VrSharp.MathHelper.DegreesToRadians(sAngle);

				double x = Math.Cos(sRadian) * Math.Cos(rRadian);
				double y = Math.Cos(sRadian) * Math.Sin(rRadian);
				double z = Math.Sin(sRadian);

				double colorR = 0.5 * x + 0.5;
				double colorG = 0.5 * y + 0.5;
				double colorB = 0.5 * z + 0.5;

				destination[destinationIndex + 3] = 0xFF;
				destination[destinationIndex + 2] = (byte)(colorR * 255.0);
				destination[destinationIndex + 1] = (byte)(colorG * 255.0);
				destination[destinationIndex + 0] = (byte)(colorB * 255.0);
			}

			public override void EncodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
			{
				byte colorR = source[sourceIndex + 2];
				byte colorG = source[sourceIndex + 1];
				byte colorB = source[sourceIndex];

				double x = colorR / 255.0 * 2.0 - 1.0;
				double y = colorG / 255.0 * 2.0 - 1.0;
				double z = colorB / 255.0 * 2.0 - 1.0;

				double radius = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
				double rRadian = Math.Atan2(y, x);
				double sRadian = Math.Asin(z / radius);

				double rAngle = VrSharp.MathHelper.RadiansToDegrees(rRadian);
				rAngle = rAngle < 0.0 ? rAngle + 360.0 : rAngle;
				double sAngle = VrSharp.MathHelper.RadiansToDegrees(sRadian);

				rAngle = MathExtensions.Clamp(rAngle, 0.0f, 360.0f);
				sAngle = MathExtensions.Clamp(sAngle, 0.0f, 90.0f);

				byte r = (byte)Math.Round(rAngle / 360.0f * 255.0f);
				byte s = (byte)Math.Round(sAngle / 90.0f * 255.0f);

				destination[destinationIndex + 1] = s;
				destination[destinationIndex + 0] = r;
			}
		}
		#endregion

		#region Rgb555
		public class Rgb555 : PvrPixelCodec
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

				destination[destinationIndex + 3] = 0xFF;
				destination[destinationIndex + 2] = (byte)(((pixel >> 10) & 0x1F) * 0xFF / 0x1F);
				destination[destinationIndex + 1] = (byte)(((pixel >> 5) & 0x1F) * 0xFF / 0x1F);
				destination[destinationIndex + 0] = (byte)(((pixel >> 0) & 0x1F) * 0xFF / 0x1F);
			}

			public override void EncodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
			{
				ushort pixel = 0x8000;
				pixel |= (ushort)((source[sourceIndex + 2] >> 3) << 10);
				pixel |= (ushort)((source[sourceIndex + 1] >> 3) << 5);
				pixel |= (ushort)((source[sourceIndex + 0] >> 3) << 0);

				destination[destinationIndex + 1] = (byte)((pixel >> 8) & 0xFF);
				destination[destinationIndex + 0] = (byte)(pixel & 0xFF);
			}
		}
		#endregion

		#region Argb8888
		public class Argb8888 : PvrPixelCodec
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
				get { return 32; }
			}

			public override void DecodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
			{
				ushort pixel = BitConverter.ToUInt16(source, sourceIndex);

				destination[destinationIndex + 3] = source[sourceIndex + 3];
				destination[destinationIndex + 2] = source[sourceIndex + 2];
				destination[destinationIndex + 1] = source[sourceIndex + 1];
				destination[destinationIndex + 0] = source[sourceIndex + 0];
			}

			public override void EncodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
			{
				destination[destinationIndex + 3] = source[sourceIndex + 3];
				destination[destinationIndex + 2] = source[sourceIndex + 2];
				destination[destinationIndex + 1] = source[sourceIndex + 1];
				destination[destinationIndex + 0] = source[sourceIndex + 0];
			}
		}
		#endregion

		#region Get Codec
		public static PvrPixelCodec GetPixelCodec(PvrPixelFormat format)
        {
            switch (format)
            {
                case PvrPixelFormat.Argb1555:
                    return new Argb1555();
                case PvrPixelFormat.Rgb565:
                    return new Rgb565();
                case PvrPixelFormat.Argb4444:
                    return new Argb4444();
				case PvrPixelFormat.Rgb555:
					return new Rgb555();
				case PvrPixelFormat.Yuv422:
					return new Yuv422();
				case PvrPixelFormat.Bump88:
					return new Bump88();
				case PvrPixelFormat.Argb8888:
					return new Argb8888();
            }

            return null;
        }
        #endregion
    }
}