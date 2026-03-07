using System;

namespace TextureLib
{
	internal class VqMipmapsDataCodec : VqDataCodec
	{
		public override bool HasMipmaps => true;

		public VqMipmapsDataCodec(PixelCodec pixelCodec) : base(pixelCodec) { }

		protected override void InternalDecode(ReadOnlySpan<byte> source, int width, int height, ReadOnlySpan<byte> palette, Span<byte> destination)
		{
			if(width == 1 && height == 1)
			{
				palette[..4].CopyTo(destination);
			}
			else
			{
				base.InternalDecode(source, width, height, palette, destination);
			}
		}

		protected override void InternalEncode(ReadOnlySpan<byte> source, int width, int height, Span<byte> destination)
		{
			if(width == 1 && height == 1)
			{
				destination[0] = source[0];
			}
			else
			{
				base.InternalEncode(source, width, height, destination);
			}
		}

	}
}