using System;

namespace VrSharp.Svr
{
    public abstract class SvrPixelCodec : VrPixelCodec
    {
        #region Rgb5a3
        // Rgb5a3
        public class Rgb5a3 : SvrPixelCodec
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

                if ((pixel & 0x8000) != 0) // Rgb555
                {
                    destination[destinationIndex + 3] = 0xFF;
                    destination[destinationIndex + 2] = (byte)(((pixel >> 0)  & 0x1F) * 0xFF / 0x1F);
                    destination[destinationIndex + 1] = (byte)(((pixel >> 5)  & 0x1F) * 0xFF / 0x1F);
                    destination[destinationIndex + 0] = (byte)(((pixel >> 10) & 0x1F) * 0xFF / 0x1F);
                }
                else // Argb3444
                {
                    destination[destinationIndex + 3] = (byte)(((pixel >> 12) & 0x07) * 0xFF / 0x07);
                    destination[destinationIndex + 2] = (byte)(((pixel >> 0)  & 0x0F) * 0xFF / 0x0F);
                    destination[destinationIndex + 1] = (byte)(((pixel >> 4)  & 0x0F) * 0xFF / 0x0F);
                    destination[destinationIndex + 0] = (byte)(((pixel >> 8)  & 0x0F) * 0xFF / 0x0F);
                }
            }

            public override void EncodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
            {
                ushort pixel = 0x0000;

                if (source[sourceIndex + 3] <= 0xDA) // Argb3444
                {
                    pixel |= (ushort)((source[sourceIndex + 3] >> 5) << 12);
                    pixel |= (ushort)((source[sourceIndex + 2] >> 4) << 0);
                    pixel |= (ushort)((source[sourceIndex + 1] >> 4) << 4);
                    pixel |= (ushort)((source[sourceIndex + 0] >> 4) << 8);
                }
                else // Rgb555
                {
                    pixel |= 0x8000;
                    pixel |= (ushort)((source[sourceIndex + 2] >> 3) << 0);
                    pixel |= (ushort)((source[sourceIndex + 1] >> 3) << 5);
                    pixel |= (ushort)((source[sourceIndex + 0] >> 3) << 10);
                }

                destination[destinationIndex + 1] = (byte)((pixel >> 8) & 0xFF);
                destination[destinationIndex + 0] = (byte)(pixel & 0xFF);
            }
        }
        #endregion

        #region Argb8888
        // Argb8888
        public class Argb8888 : SvrPixelCodec
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
                uint pixel = BitConverter.ToUInt32(source, sourceIndex);

                if ((source[sourceIndex + 3] & 0x80) != 0) // Rgb888
                {
                    destination[destinationIndex + 3] = 0xFF;
                    destination[destinationIndex + 2] = source[sourceIndex + 0];
                    destination[destinationIndex + 1] = source[sourceIndex + 1];
                    destination[destinationIndex + 0] = source[sourceIndex + 2];
                }
                else // Argb7888
                {
                    destination[destinationIndex + 3] = (byte)((source[sourceIndex + 3] << 1) & 0xFF);
                    destination[destinationIndex + 2] = source[sourceIndex + 0];
                    destination[destinationIndex + 1] = source[sourceIndex + 1];
                    destination[destinationIndex + 0] = source[sourceIndex + 2];
                }
            }

            public override void EncodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
            {
                if (source[sourceIndex + 3] < 0xFF) // Argb7888
                {
                    destination[destinationIndex + 3] = (byte)((source[sourceIndex + 3] >> 1) & 0x7F);
                    destination[destinationIndex + 0] = source[sourceIndex + 2];
                    destination[destinationIndex + 1] = source[sourceIndex + 1];
                    destination[destinationIndex + 2] = source[sourceIndex + 0];
                }
                else // Rgb888
                {
                    destination[destinationIndex + 3] = 0x80;
                    destination[destinationIndex + 0] = source[sourceIndex + 2];
                    destination[destinationIndex + 1] = source[sourceIndex + 1];
                    destination[destinationIndex + 2] = source[sourceIndex + 0];
                }
            }
        }
        #endregion

        #region Get Codec
        public static SvrPixelCodec GetPixelCodec(SvrPixelFormat format)
        {
            switch (format)
            {
                case SvrPixelFormat.Rgb5a3:
                    return new Rgb5a3();
                case SvrPixelFormat.Argb8888:
                    return new Argb8888();
            }

            return null;
        }
        #endregion
    }
}