using System;
using System.Collections.Generic;

namespace TextureLib
{
	internal abstract class GvrDataCodec: DataCodec
	{
		private static readonly Dictionary<GvrDataFormat, GvrDataCodec> _codecs = new()
		{
			{ GvrDataFormat.Intensity4, new GvrIntensity4DataCodec() },
			{ GvrDataFormat.Intensity8, new GvrIntensity8DataCodec() },
			{ GvrDataFormat.IntensityA4, new GvrIntensityA4DataCodec() },
			{ GvrDataFormat.IntensityA8, new GvrIntensityA8DataCodec() },
			{ GvrDataFormat.Rgb565, new GvrRGB565DataCodec() },
			{ GvrDataFormat.Rgb5a3, new GvrRGB5A3DataCodec() },
			{ GvrDataFormat.Argb8888, new GvrARGB8DataCodec() },
			{ GvrDataFormat.Index4, new GvrIndex4DataCodec() },
			{ GvrDataFormat.Index8, new GvrIndex8DataCodec() },
			{ GvrDataFormat.Index14, new GvrIndex14DataCodec() },
			{ GvrDataFormat.Dxt1, new GvrDXT1DataCodec() },
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

		public static GvrDataCodec GetDataCodec(GvrDataFormat pixelFormat)
		{
			if(_codecs.TryGetValue(pixelFormat, out GvrDataCodec? result))
			{
				return result;
			}

			throw new NotImplementedException($"Pixel format \"{pixelFormat}\" is not implemented");
		}

		public static GvrDataCodec GetGvrDataCodecForPalette(GvrPaletteFormat paletteFormat, bool saCompatible = true)
		{
			switch (paletteFormat)
			{
				case GvrPaletteFormat.IntensityA8orArgb1555:
					return saCompatible ? new GvrRGBA1555DataCodec() : new GvrIntensityA8DataCodec();
                case GvrPaletteFormat.Rgb5A3orArgb4444:
                    return saCompatible ? new GvrRGBA4444DataCodec() : new GvrRGB5A3DataCodec();
                case GvrPaletteFormat.Argb8888:
                    return new GvrARGB8DataCodec();
				case GvrPaletteFormat.Rgb565:
                    return new GvrRGB565DataCodec();
				default:
					throw new NotImplementedException();
            }
		}

	}
}
