using System;
using BCnEncoder.Decoder;
using BCnEncoder.Encoder;
using BCnEncoder.Shared;

namespace TextureLib
{
	// Codec for DXT formats that uses BCnEncoder
	internal class BCDataCodec : DdsDataCodec
	{
		public override int CalculateTextureSize(int width, int height)
		{
			// Minimum DXT block size is 4x4 so 2x2 and 1x1 maps take as much as 4x4
			width = Math.Max(width, 4);
			height = Math.Max(height, 4);
			switch (CompressionFormat)
			{
				case CompressionFormat.Bc1: // DXT1, 4 bpp				
					return width * height / 2;
				case CompressionFormat.Bc2: // DXT3, 8 bpp
				case CompressionFormat.Bc3: // DXT5, 8 bpp
				default:
					return width * height;
			}
		}

		public override bool CheckDimensionsValid(int width, int height)
		{
			return width is >= 4 and <= 8192
				&& height is >= 4 and <= 8192
				&& TextureFunctions.IsPow2(width)
				&& TextureFunctions.IsPow2(height);
		}

		public BCDataCodec(PixelCodec pixelCodec) : base(pixelCodec) { }

		public BCnEncoder.Shared.CompressionFormat CompressionFormat;

		protected override void InternalDecode(ReadOnlySpan<byte> source, int width, int height, ReadOnlySpan<byte> palette, Span<byte> destination)
		{
			BcDecoder decoder = new BcDecoder();
			ColorRgba32[] result = decoder.DecodeRaw(source.ToArray(), width, height, CompressionFormat);
			int dstAddress = 0;
			for (int c = 0; c < result.Length; c++)
			{
				destination[dstAddress] = result[c].r;
				destination[dstAddress + 1] = result[c].g;
				destination[dstAddress + 2] = result[c].b;
				destination[dstAddress + 3] = result[c].a;
				dstAddress += 4;
			}
		}

		protected override void InternalEncode(ReadOnlySpan<byte> source, int width, int height, Span<byte> destination)
		{
			BcEncoder encoder = new BcEncoder(CompressionFormat);			
			Span<byte> result = encoder.EncodeToRawBytes(source, width, height, PixelFormat.Rgba32, 0, out _, out _);
			for (int a = 0; a < result.Length; a++)
			{
				destination[a] = result[a];
			}			
		}
	}
}