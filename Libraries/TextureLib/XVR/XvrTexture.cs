using System;
using System.Collections.Generic;
using System.Drawing;
using static TextureLib.DirectXTexUtility;

// Class for XVR textures (PSO BB)

namespace TextureLib
{
	public class XvrTexture : DdsTexture
	{
		public int XvrType = -1;

		public XvrTexture(byte[] data, int offset = 0, string name = null) : base(data, offset, name)
		{
			XvrType = BitConverter.ToInt32(data, offset + 0xC);
#if DEBUG
			Console.WriteLine("\nXVR TEXTURE INFO");
			Console.WriteLine("GBIX: {0}", Gbix);
			Console.WriteLine("XVR TYPE: {0} ({1})", XvrType, GetDxgiFormatFromXvrPixelFormat((byte)XvrType).ToString());
#endif
		}

		public XvrTexture(Bitmap texture, int xvrFormat, bool mipmaps, uint gbix = 0) : base(texture, GetDdsFormatFromXvrPixelFormat(xvrFormat), mipmaps)
		{
			Gbix = gbix;
		}

		public override byte[] GetBytes()
		{
			const uint MagicXVRT = 0x54525658;
			List<byte> result = new List<byte>();
			result.AddRange(BitConverter.GetBytes(MagicXVRT));
			result.AddRange(BitConverter.GetBytes(HeaderlessData.Length + 64 - 8));
			XvrFlags flags = 0;
			if (UseAlpha)
				flags |= XvrFlags.Alpha;
			if (HasMipmaps)
				flags |= XvrFlags.Mips;
			result.AddRange(BitConverter.GetBytes((uint)flags));
			if (XvrType == -1)
				XvrType = GetXvrPixelFormatFromDdsFormat(DdsFormat);
			result.AddRange(BitConverter.GetBytes(XvrType));
			result.AddRange(BitConverter.GetBytes(Gbix));
			result.AddRange(BitConverter.GetBytes((ushort)Width));
			result.AddRange(BitConverter.GetBytes((ushort)Height));
			result.AddRange(BitConverter.GetBytes(HeaderlessData.Length));
			result.AddRange(new byte[0x24]);
			result.AddRange(HeaderlessData);
			return result.ToArray();
		}

		public static DXGIFormat GetDxgiFormatFromXvrPixelFormat(byte pixelFormat)
		{
			switch (pixelFormat)
			{
				case 11:
				case 1:
					return DXGIFormat.B8G8R8A8UNORM;
				case 12:
				case 2:
					return DXGIFormat.B5G6R5UNORM;
				case 13:
				case 3:
					return DXGIFormat.B5G5R5A1UNORM;
				case 14:
				case 4:
					return DXGIFormat.B4G4R4A4UNORM;
				case 5:
					return DXGIFormat.P8;
				case 6:
					return DXGIFormat.BC1UNORM;
				case 7:
				case 8:
					return DXGIFormat.BC2UNORM;
				case 9:
				case 10:
					return DXGIFormat.BC3UNORM;
				case 15:
					return DXGIFormat.YUY2;
				case 16:
					return DXGIFormat.R8G8SNORM;
				case 17:
					return DXGIFormat.A8UNORM;
				case 18: //D3DFMT_X1R5G5B5
				case 19: //D3DFMT_X8R8G8B8
					return DXGIFormat.UNKNOWN;
				default:
					return DXGIFormat.BC1UNORM;
			}
		}

		public int GetXvrPixelFormatFromDdsFormat(DdsFormat format)
		{
			switch (format)
			{
				case DdsFormat.Argb8888:
					return 1;
				case DdsFormat.Rgb565:
					return 2;
				case DdsFormat.Argb1555:
					return 3;
				case DdsFormat.Argb4444:
					return 4;
				case DdsFormat.Dxt1:
					return 6;
				case DdsFormat.Dxt3:
					return 7;
				case DdsFormat.Dxt5:
					return 9;
				case DdsFormat.Rgb888:
				default:
					throw new NotImplementedException(string.Format("XVR conversin from DDS format {0} is not implemented.", format));
			}
		}

		public static DdsFormat GetDdsFormatFromXvrPixelFormat(int xvrPixelFormat)
		{
			switch (xvrPixelFormat)
			{
				case 1:
				case 11:
					return DdsFormat.Argb8888;
				case 2:
				case 12:
					return DdsFormat.Rgb565;
				case 3:
				case 13:
					return DdsFormat.Argb1555;
				case 4:
				case 14:
					return DdsFormat.Argb4444;
				case 6:
					return DdsFormat.Dxt1;
				case 7:
				case 8:
					return DdsFormat.Dxt3;
				case 9:
				case 10:
					return DdsFormat.Dxt5;
				default:
					throw new NotImplementedException(string.Format("Conversion for XVR format {0} is not implemented.", xvrPixelFormat));
			}
		}

		[Flags]
		private enum XvrFlags : byte
		{
			Mips = 1,
			Alpha = 2,
		}

		public static DDSHeader GetDdsHeaderFromXvr(byte[] data, int offset)
		{
			int flags = BitConverter.ToInt32(data, offset + 8);
			bool useAlpha = (flags & (int)XvrFlags.Alpha) > 0;
			bool useMips = (flags & (int)XvrFlags.Mips) > 0;
			int pixelFormat = BitConverter.ToInt32(data, offset + 0xC);
			DXGIFormat fmt = GetDxgiFormatFromXvrPixelFormat((byte)pixelFormat);
			// GBIX at 0x10
			int textureWidth = BitConverter.ToUInt16(data, offset + 0x14);
			int textureHeight = BitConverter.ToUInt16(data, offset + 0x16);
			int mipCount = 0;
			if (useMips)
			{
				int heightMip = textureWidth;
				int widthMip = textureHeight;
				while (heightMip > 1 && widthMip > 1)
				{
					heightMip /= 2;
					widthMip /= 2;
					mipCount++;
				}
				mipCount--;
			}
			var meta = GenerateMataData(textureWidth, textureHeight, mipCount, fmt, false);
			DDSHeader ddsHeader;
			GenerateDDSHeader(meta, DDSFlags.NONE, out ddsHeader, out _);
			if (useAlpha)
				ddsHeader.PixelFormat.Flags |= PixelFormats.DDSALPHAPIXELS;
			return ddsHeader;
		}

		public static uint GetGbixFromXvr(byte[] data, int offset)
		{
			return BitConverter.ToUInt32(data, offset + 0x10);
		}
	}
}