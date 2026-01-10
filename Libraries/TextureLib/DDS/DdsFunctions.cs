using System;
using static TextureLib.DirectXTexUtility;
using static TextureLib.DirectXTexUtility.DDSHeader;

namespace TextureLib
{
	public static class DdsFunctions
	{
		public enum DdsTextureHeaderType
		{
			Dds,
			Xvr,
			Unknown
		}

		public static DDSHeader GetDdsHeader(byte[] data, int offset)
		{
			return new DDSHeader
			{
				Size = BitConverter.ToUInt32(data, offset + 4),
				Flags = (DDSHeader.HeaderFlags)BitConverter.ToUInt32(data, offset + 8),
				Height = BitConverter.ToUInt32(data, offset + 12),
				Width = BitConverter.ToUInt32(data, offset + 16),
				PitchOrLinearSize = BitConverter.ToUInt32(data, offset + 20),
				Depth = BitConverter.ToUInt32(data, offset + 24),
				MipMapCount = BitConverter.ToUInt32(data, offset + 28),
				PixelFormat = GetDDSPixelFormat(data, offset + 76),
				Caps = BitConverter.ToUInt32(data, offset + 108),
				Caps2 = BitConverter.ToUInt32(data, offset + 112),
				Caps3 = BitConverter.ToUInt32(data, offset + 116),
				Reserved2 = BitConverter.ToUInt32(data, offset + 120)
			};
		}

		public static DDSPixelFormat GetDDSPixelFormat(byte[] data, int offset)
		{
			return new DDSPixelFormat
			{
				Size = BitConverter.ToUInt32(data, offset),
				Flags = BitConverter.ToUInt32(data, offset + 4),
				FourCC = BitConverter.ToUInt32(data, offset + 8),
				RGBBitCount = BitConverter.ToUInt32(data, offset + 12),
				RBitMask = BitConverter.ToUInt32(data, offset + 16),
				GBitMask = BitConverter.ToUInt32(data, offset + 20),
				BBitMask = BitConverter.ToUInt32(data, offset + 24),
				ABitMask = BitConverter.ToUInt32(data, offset + 28)
			};
		}

		public static bool ComparePixelFormats(DDSPixelFormat src, DDSPixelFormat dst)
		{
			//if (src.Size != dst.Size) return false;
			//if (src.Flags != dst.Flags) return false;
			if (src.FourCC != dst.FourCC) return false;
			if (src.RGBBitCount != dst.RGBBitCount) return false;
			if (src.RBitMask != dst.RBitMask) return false;
			if (src.GBitMask != dst.GBitMask) return false;
			if (src.BBitMask != dst.BBitMask) return false;
			if (src.ABitMask != dst.ABitMask) return false;
			return true;
		}

		public static DdsFormat IdentifyPixelFormat(DDSPixelFormat fmt)
		{
			if (ComparePixelFormats(fmt, PixelFormats.R8G8B8))
				return DdsFormat.Rgb888;
			else if (ComparePixelFormats(fmt, PixelFormats.A8R8G8B8)) // R8G8B8A8UNORM
				return DdsFormat.Argb8888;
			else if (ComparePixelFormats(fmt, PixelFormats.R5G6B5)) // B5G6R5UNORM
				return DdsFormat.Rgb565;
			else if (ComparePixelFormats(fmt, PixelFormats.A1R5G5B5)) // B5G5R5A1UNORM
				return DdsFormat.Argb1555;
			if (ComparePixelFormats(fmt, PixelFormats.A4R4G4B4)) // B4G4R4A4UNORM
				return DdsFormat.Argb4444;
			else if (ComparePixelFormats(fmt, PixelFormats.DXT1)) // BC1UNORM
				return DdsFormat.Dxt1;
			else if (ComparePixelFormats(fmt, PixelFormats.DXT3)) // BC2UNORM
				return DdsFormat.Dxt3;
			else if (ComparePixelFormats(fmt, PixelFormats.DXT5)) // BC3UNORM
				return DdsFormat.Dxt5;
			else
				return DdsFormat.Dxt1;
		}

		public static DXGIFormat GetDxgiFormat(DdsFormat fmt)
		{
			switch (fmt)
			{
				case DdsFormat.Rgb888:
					return DXGIFormat.R8G8B8G8UNORM; // R8G8B8G8UNORM is wrong, this is just a workaround because the real R8G8B8 is not a DXGI format
				case DdsFormat.Argb8888:
					return DXGIFormat.B8G8R8A8UNORM;
				case DdsFormat.Rgb565:
					return DXGIFormat.B5G6R5UNORM;
				case DdsFormat.Argb1555:
					return DXGIFormat.B5G5R5A1UNORM;
				case DdsFormat.Argb4444:
					return DXGIFormat.B4G4R4A4UNORM;
				case DdsFormat.Dxt1:
					return DXGIFormat.BC1UNORM;
				case DdsFormat.Dxt3:
					return DXGIFormat.BC2UNORM;
				case DdsFormat.Dxt5:
					return DXGIFormat.BC3UNORM;
				default:
					return DXGIFormat.UNKNOWN;
			}
		}

		public static DdsTextureHeaderType CheckHeaderType(byte[] data, int offset)
		{
			const uint MagicXVRT = 0x54525658;
			const uint MagicDDS = 0x20534444;
			uint test = BitConverter.ToUInt32(data, offset);
			if (test == MagicDDS)
				return DdsTextureHeaderType.Dds;
			else if (test == MagicXVRT)
				return DdsTextureHeaderType.Xvr;
			return DdsTextureHeaderType.Unknown;
		}
	}
}