using System;

namespace TextureLib
{
    public abstract class PixelCodec
    {
        public abstract int BytesPerPixel { get; }

        public virtual int Pixels => 1;

		public bool BigEndian;

		public abstract string Info();

        public abstract void DecodePixel(ReadOnlySpan<byte> source, Span<byte> destination);

        public abstract void EncodePixel(ReadOnlySpan<byte> source, Span<byte> destination);

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
                case PvrPixelFormat.Argb8888: // YUV420 not implemented
				case PvrPixelFormat.Argb8888Alt:
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
                    return saCompatible ? new ARGB1555PixelCodec() { BigEndian = true } : new IntensityA8PixelCodec();
                case GvrPaletteFormat.Rgb565:
                    return new RGB565PixelCodec { BigEndian = true };
                case GvrPaletteFormat.Rgb5A3orArgb4444:
                    return saCompatible ? new ARGB4444PixelCodec() { BigEndian = true } : new RGB5A3PixelCodec { BigEndian = true };
                case GvrPaletteFormat.Argb8888:
                    return new ARGB8888PixelCodec() { BigEndian = true };
                default:
                    throw new NotImplementedException(paletteFormat.ToString());
            }
        }

		public static PixelCodec GetPixelCodec(DdsFormat ddsFormat)
		{
			switch (ddsFormat)
			{
				case DdsFormat.Argb8888:
					return new ARGB8888PixelCodec() { BigEndian = false };
				case DdsFormat.Argb1555:
					return new ARGB1555PixelCodec() { BigEndian = false };
				case DdsFormat.Argb4444:
					return new ARGB4444PixelCodec() { BigEndian = false };
				case DdsFormat.Rgb565:
					return new RGB565PixelCodec() { BigEndian = false };
				case DdsFormat.Dxt1:
				case DdsFormat.Dxt3:
				case DdsFormat.Dxt5:
					return null;
				case DdsFormat.Rgb888:
					return new RGB888PixelCodec() { BigEndian = false };
				case DdsFormat.Unsupported:
				default:
					throw new NotImplementedException(ddsFormat.ToString());
			}
		}

		public PvrPixelFormat GetPvrPixelFormat()
		{
			switch (this)
			{
				case ARGB1555PixelCodec: return PvrPixelFormat.Argb1555;
				case ARGB4444PixelCodec: return PvrPixelFormat.Argb4444;
				case ARGB8888PixelCodec: return PvrPixelFormat.Argb8888;
				case RGB555PixelCodec: return PvrPixelFormat.Rgb555;
				case RGB565PixelCodec: return PvrPixelFormat.Rgb565;
				case YUV422PixelCodec: return PvrPixelFormat.Yuv422;
				case Bump88PixelCodec: return PvrPixelFormat.Bump88;
				default:
					throw new Exception("Unable to get PVR pixel format from pixel codec " + this.ToString());
			}
		}

		public GvrPaletteFormat GetGvrPaletteFormat()
		{
			switch (this)
			{
				case IntensityA8PixelCodec:
				case ARGB1555PixelCodec: return GvrPaletteFormat.IntensityA8orArgb1555;
				case ARGB8888PixelCodec: return GvrPaletteFormat.Argb8888;
				case RGB565PixelCodec: return GvrPaletteFormat.Rgb565;
				case RGB5A3PixelCodec:
				case ARGB4444PixelCodec: return GvrPaletteFormat.Rgb5A3orArgb4444;
				default:
					throw new Exception("Unable to get GVR palette format from pixel codec " + this.ToString());
			}
		}
	}
}