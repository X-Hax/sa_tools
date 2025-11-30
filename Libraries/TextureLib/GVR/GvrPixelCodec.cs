using System;
using System.Collections.Generic;

namespace TextureLib
{
	internal abstract class GvrPixelCodec: DataCodec
	{
		private static readonly Dictionary<GvrDataFormat, GvrPixelCodec> _codecs = new()
		{
			{ GvrDataFormat.Intensity4, new GvrIntensity4PixelCodec() },
			{ GvrDataFormat.Intensity8, new GvrIntensity8PixelCodec() },
			{ GvrDataFormat.IntensityA4, new GvrIntensityA4PixelCodec() },
			{ GvrDataFormat.IntensityA8, new GvrIntensityA8PixelCodec() },
			{ GvrDataFormat.Rgb565, new GvrRGB565PixelCodec() },
			{ GvrDataFormat.Rgb5a3, new GvrRGB5A3PixelCodec() },
			{ GvrDataFormat.Argb8888, new GvrARGB8PixelCodec() },
			{ GvrDataFormat.Index4, new GvrIndex4PixelCodec() },
			{ GvrDataFormat.Index8, new GvrIndex8PixelCodec() },
			{ GvrDataFormat.Index14, new GvrIndex14PixelCodec() },
			{ GvrDataFormat.Dxt1, new GvrDXT1PixelCodec() },
		};

		/// <summary>
		/// Gets the maximum number of entries the palette allows for, or 0 if this pixel format doesn't use a palette.
		/// </summary>
		public virtual int PaletteEntries => 0;

		public int CalculateTextureSize(int width, int height)
		{
			return Math.Max(32, InternalCalculateTextureSize(width, height));
		}

		protected abstract int InternalCalculateTextureSize(int width, int height);

		public override byte[] Decode(ReadOnlySpan<byte> src, int width, int height, ReadOnlySpan<byte> palette)
		{
			byte[] result = new byte[width * height * (PaletteEntries > 0 ? 1 : 4)];
			InternalDecode(src, width, height, result);
			return result;
		}

		protected abstract void InternalDecode(ReadOnlySpan<byte> src, int width, int height, Span<byte> dst);

		public override byte[] Encode(ReadOnlySpan<byte> src, int width, int height)
		{
			byte[] result = new byte[CalculateTextureSize(width, height)];
			InternalEncode(src, width, height, result);
			return result;
		}

		protected abstract void InternalEncode(ReadOnlySpan<byte> src, int width, int height, Span<byte> dst);

		public static GvrPixelCodec GetPixelCodec(GvrDataFormat pixelFormat)
		{
			if(_codecs.TryGetValue(pixelFormat, out GvrPixelCodec? result))
			{
				return result;
			}

			throw new NotImplementedException($"Pixel format \"{pixelFormat}\" is not implemented");
		}

		public static GvrPixelCodec GetPixelCodecForPalette(GvrPaletteFormat paletteFormat, bool saCompatible = true)
		{
			switch (paletteFormat)
			{
				case GvrPaletteFormat.IntensityA8orArgb1555:
					return saCompatible ? new GvrRGBA1555PixelCodec() : new GvrIntensityA8PixelCodec();
                case GvrPaletteFormat.Rgb5A3orArgb4444:
                    return saCompatible ? new GvrRGBA4444PixelCodec() : new GvrRGB5A3PixelCodec();
                case GvrPaletteFormat.Argb8888:
                    return new GvrARGB8PixelCodec();
				case GvrPaletteFormat.Rgb565:
                    return new GvrRGB565PixelCodec();
				default:
					throw new NotImplementedException();
            }
		}

	}
}
