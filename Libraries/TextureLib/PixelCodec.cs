using System;

namespace TextureLib
{
    public abstract class PixelCodec
    {
        public abstract int BytesPerPixel { get; }

        public virtual int Pixels => 1;

        public abstract void DecodePixel(ReadOnlySpan<byte> source, Span<byte> destination, bool bigEndian);

        public abstract void EncodePixel(ReadOnlySpan<byte> source, Span<byte> destination, bool bigEndian);

        public static PixelCodec GetPixelCodec(PvrPixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PvrPixelFormat.Argb1555:
                    return new ARGB1555PixelCodec();
                case PvrPixelFormat.Rgb555:
                    return new RGB555PixelCodec();
                case PvrPixelFormat.Argb4444:
                    return new ARGB4444PixelCodec();
                case PvrPixelFormat.Argb8888orYUV420:
                    return new ARGB8888PixelCodec();
                case PvrPixelFormat.Argb8888:
                    return new ARGB8888PixelCodec();
                case PvrPixelFormat.Rgb565:
                    return new RGB565PixelCodec();
                case PvrPixelFormat.Yuv422:
                    return new YUV422PixelCodec();
                case PvrPixelFormat.Bump88:
                    return new Bump88PixelCodec();
            }
            throw new NotImplementedException(pixelFormat.ToString());
        }

        public static PixelCodec GetPixelCodec(GvrPaletteFormat paletteFormat, bool saCompatible = true)
        {
            switch (paletteFormat)
            {
                case GvrPaletteFormat.IntensityA8orArgb1555:
                    return saCompatible ? new ARGB1555PixelCodec() : new IntensityA8PixelCodec();
                case GvrPaletteFormat.Rgb565:
                    return new RGB565PixelCodec();
                case GvrPaletteFormat.Rgb5A3orArgb4444:
                    return saCompatible ? new ARGB4444PixelCodec() : new RGB5A3PixelCodec();
                case GvrPaletteFormat.Argb8888:
                    return new ARGB8888PixelCodec();
                default:
                    throw new NotImplementedException(paletteFormat.ToString());
            }
        }

		public static PixelCodec GetPixelCodec(DdsFormat ddsFormat)
		{
			switch (ddsFormat)
			{
				case DdsFormat.Argb8888:
					return new ABGR8888PixelCodec();
				case DdsFormat.Argb1555:
					return new ARGB1555PixelCodec();
				case DdsFormat.Argb4444:
					return new ARGB4444PixelCodec();
				case DdsFormat.Rgb565:
					return new RGB565PixelCodec();
				case DdsFormat.Dxt1:
				case DdsFormat.Dxt3:
				case DdsFormat.Dxt5:
					return null;
				case DdsFormat.Rgb888:
					return new RGB888PixelCodec();
				case DdsFormat.Unsupported:
				default:
					throw new NotImplementedException(ddsFormat.ToString());
			}
		}
	}
}