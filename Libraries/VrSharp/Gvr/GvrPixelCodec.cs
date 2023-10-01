using System;

namespace VrSharp.Gvr
{
    public abstract class GvrPixelCodec : VrPixelCodec
    {
        #region Intensity 8-bit with Alpha
        // Intensity 8-bit with Alpha
        public class IntensityA8 : GvrPixelCodec
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
                destination[destinationIndex + 3] = source[sourceIndex];
                destination[destinationIndex + 2] = source[sourceIndex + 1];
                destination[destinationIndex + 1] = source[sourceIndex + 1];
                destination[destinationIndex + 0] = source[sourceIndex + 1];
            }

            public override void EncodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
            {
                destination[destinationIndex + 0] = source[sourceIndex + 3];
                destination[destinationIndex + 0] = (byte)((0.30 * source[sourceIndex + 2]) + (0.59 * source[sourceIndex + 1]) + (0.11 * source[sourceIndex + 0]));
            }
        }
        #endregion

        #region Rgb565
        // Rgb565
        public class Rgb565 : GvrPixelCodec
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
                ushort pixel = PTMethods.ToUInt16BE(source, sourceIndex);

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

                destination[destinationIndex + 1] = (byte)(pixel & 0xFF);
                destination[destinationIndex + 0] = (byte)((pixel >> 8) & 0xFF);
            }
        }
        #endregion

        #region Rgb5a3
        // Rgb5a3
        public class Rgb5a3 : GvrPixelCodec
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
                ushort pixel = PTMethods.ToUInt16BE(source, sourceIndex);

                if ((pixel & 0x8000) != 0) // Rgb555
                {
                    destination[destinationIndex + 3] = 0xFF;
                    destination[destinationIndex + 2] = (byte)(((pixel >> 10) & 0x1F) * 0xFF / 0x1F);
                    destination[destinationIndex + 1] = (byte)(((pixel >> 5)  & 0x1F) * 0xFF / 0x1F);
                    destination[destinationIndex + 0] = (byte)(((pixel >> 0)  & 0x1F) * 0xFF / 0x1F);
                }
                else // Argb3444
                {
                    destination[destinationIndex + 3] = (byte)(((pixel >> 12) & 0x07) * 0xFF / 0x07);
                    destination[destinationIndex + 2] = (byte)(((pixel >> 8)  & 0x0F) * 0xFF / 0x0F);
                    destination[destinationIndex + 1] = (byte)(((pixel >> 4)  & 0x0F) * 0xFF / 0x0F);
                    destination[destinationIndex + 0] = (byte)(((pixel >> 0)  & 0x0F) * 0xFF / 0x0F);
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
        public static GvrPixelCodec GetPixelCodec(GvrPixelFormat format)
        {
            switch (format)
            {
                case GvrPixelFormat.IntensityA8:
                    return new IntensityA8();
                case GvrPixelFormat.Rgb565:
                    return new Rgb565();
                case GvrPixelFormat.Rgb5a3:
                    return new Rgb5a3();
            }

            return null;
        }
        #endregion
    }
}