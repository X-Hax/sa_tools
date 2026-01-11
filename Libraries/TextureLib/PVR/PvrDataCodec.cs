using System;

namespace TextureLib
{
	public abstract class PvrDataCodec: DataCodec
	{
		public PixelCodec PixelCodec { get; }

		/// <summary>
		/// Gets if this data format requires an external palette file.
		/// </summary>
		public virtual bool NeedsExternalPalette => false;

		/// <summary>
		/// Gets if this data format has mipmaps.
		/// </summary>
		public virtual bool HasMipmaps => false;


		protected PvrDataCodec(PixelCodec pixelCodec)
		{
			PixelCodec = pixelCodec;
		}

		public static PvrDataCodec Create(PvrDataFormat format, PixelCodec pixelCodec)
		{
			return format switch
			{
				PvrDataFormat.SquareTwiddled => new SquareTwiddledDataCodec(pixelCodec),
                PvrDataFormat.SquareTwiddledMipmaps => new SquareTwiddledMipmapsDataCodec(pixelCodec),
                PvrDataFormat.Vq => new VqDataCodec(pixelCodec),
                PvrDataFormat.VqMipmaps => new VqMipmapsDataCodec(pixelCodec),
                PvrDataFormat.Index4 => new Index4DataCodec(pixelCodec),
                PvrDataFormat.Index4Mipmaps => new Index4MipmapsDataCodec(pixelCodec),
                PvrDataFormat.Index8 => new Index8DataCodec(pixelCodec),
                PvrDataFormat.Index8Mipmaps => new Index8MipmapsDataCodec(pixelCodec),
                PvrDataFormat.Rectangle => new RectangleDataCodec(pixelCodec),
                PvrDataFormat.RectangleStride => new StrideDataCodec(pixelCodec),
                PvrDataFormat.RectangleTwiddled => new RectangleTwiddledDataCodec(pixelCodec),
                PvrDataFormat.SmallVq => new SmallVqDataCodec(pixelCodec),
                PvrDataFormat.SmallVqMipmaps => new SmallVqMipmapsDataCodec(pixelCodec),
                PvrDataFormat.Bitmap => new BitmapDataCodec(pixelCodec),
				// Duplicate/unsupported
				PvrDataFormat.SquareTwiddledMipmapsDma => new SquareTwiddledMipmapsDataCodec(pixelCodec),
				PvrDataFormat.RectangleMipmaps => new RectangleDataCodec(pixelCodec),
				PvrDataFormat.RectangleStrideMipmaps => new StrideDataCodec(pixelCodec),
				// BitmapMipmaps
				_ => throw new ArgumentException(string.Format("No codec for PVR data format {0} ({1}) is currently implemented. Please share this texture with SA Tools developers.", format.ToString(), format))
			};
		}

		public virtual bool CheckDimensionsValid(int width, int height)
		{
			return width == height
				&& width is >= 8 and <= 1024
				&& TextureFunctions.IsPow2(width);
		}

		/// <summary>
		/// Gets the maximum number of entries the palette allows for, or 0 if this pixel format doesn't use a palette.
		/// </summary>
		public virtual int GetPaletteEntries(int? width)
		{
			return 0;
		}

		public abstract int CalculateTextureSize(int width, int height);
		public override byte[] Decode(ReadOnlySpan<byte> source, int width, int height, ReadOnlySpan<byte> palette)
		{
			byte[] result = new byte[width * height * (NeedsExternalPalette ? 1 : 4)];
			Span<byte> destination = result;

			InternalDecode(source, width, height, palette, destination);

			return result;
		}

		protected abstract void InternalDecode(ReadOnlySpan<byte> source, int width, int height, ReadOnlySpan<byte> palette, Span<byte> destination);


		public override byte[] Encode(ReadOnlySpan<byte> source, int width, int height)
		{
			byte[] result = new byte[CalculateTextureSize(width, height)];
			Span<byte> destination = result;

			InternalEncode(source, width, height, destination);

			return result;
		}

		protected abstract void InternalEncode(ReadOnlySpan<byte> source, int width, int height, Span<byte> destination);

	}
}