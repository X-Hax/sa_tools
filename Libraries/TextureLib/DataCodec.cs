using System;

namespace TextureLib
{
	// Generic class for PVR and GVR data codecs
	public abstract class DataCodec
	{
		public abstract byte[] Encode(ReadOnlySpan<byte> src, int width, int height);

		public abstract byte[] Decode(ReadOnlySpan<byte> src, int width, int height, ReadOnlySpan<byte> palette);
	}
}
