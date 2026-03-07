using System;

namespace TextureLib
{
	internal abstract class DdsDataCodec : DataCodec
	{
		public PixelCodec PixelCodec { get; }

		protected DdsDataCodec(PixelCodec pixelCodec)
		{
			PixelCodec = pixelCodec;
		}

		public virtual bool CheckDimensionsValid(int width, int height)
		{
			return width is >= 4 and <= 8192
			&& height is >= 4 and <= 8192
			&& TextureFunctions.IsPow2(width)
			&& TextureFunctions.IsPow2(height);
		}

		public abstract int CalculateTextureSize(int width, int height);

		public override byte[] Decode(ReadOnlySpan<byte> source, int width, int height, ReadOnlySpan<byte> palette)
		{
			byte[] result = new byte[width * height * 4];
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

		public static DdsDataCodec GetDataCodec(DdsFormat fmt, PixelCodec px, bool useAlpha)
		{
			switch (fmt)
			{
				case DdsFormat.Dxt1:
					return new BCDataCodec(px) { CompressionFormat = useAlpha ? BCnEncoder.Shared.CompressionFormat.Bc1WithAlpha : BCnEncoder.Shared.CompressionFormat.Bc1 };
				case DdsFormat.Dxt3:
					return new BCDataCodec(px) { CompressionFormat = BCnEncoder.Shared.CompressionFormat.Bc2 };
				case DdsFormat.Dxt5:
					return new BCDataCodec(px) { CompressionFormat = BCnEncoder.Shared.CompressionFormat.Bc3 };
				default:
					return new LinearDataCodec(px);
			}
		}

	}
}