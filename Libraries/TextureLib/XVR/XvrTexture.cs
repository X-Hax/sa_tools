using System;
using System.Collections.Generic;
using System.Drawing;
using static TextureLib.DirectXTexUtility;

// Class for XVR textures (PSO BB)

namespace TextureLib
{
	public class XvrTexture : DdsTexture
	{
		public XvrFormat XvrType = XvrFormat.Invalid;

		/// <summary>
		/// Initializes an XVR texture from a byte array that contains XVR texture header and data.
		/// </summary>
		/// <param name="data">Byte array containing XVR texture header and data.</param>
		/// <param name="offset">Offset to the beginning of the GBIX or XVRT header.</param>
		/// <param name="name">Texture name, if applicable.</param>
		public XvrTexture(byte[] data, int offset = 0, string name = null) : base(data, offset, 0, name)
		{
			Gbix = BitConverter.ToUInt32(data, offset + 0x10);
			XvrType = (XvrFormat)BitConverter.ToInt32(data, offset + 0xC);
#if DEBUG
			Console.WriteLine("\nXVR TEXTURE INFO");
			Console.WriteLine("GBIX: {0}", Gbix);
			Console.WriteLine("XVR TYPE: {0} ({1})", XvrFormats.GetDxgiFormatFromXvrPixelFormat(XvrType).ToString(), XvrType);
#endif
		}

		public XvrTexture Clone()
		{
			return new XvrTexture(RawData, 0, Name);
		}

		/// <summary>
		/// Encodes an XVR texture from Bitmap.
		/// </summary>
		/// <param name="texture">Source Bitmap.</param>
		/// <param name="xvrFormat">Target XVR data format.</param>
		/// <param name="mipmaps">Encode mipmaps.</param>
		/// <param name="gbix">Global index (optional).</param>		
		/// <param name="name">Textre name (optional).</param>
		public XvrTexture(Bitmap texture, XvrFormat xvrFormat, bool mipmaps, uint gbix = 0, string name = null) : base(texture, XvrFormats.GetDdsFormatFromXvrPixelFormat(xvrFormat), mipmaps, gbix, name) { }

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
			if (XvrType == XvrFormat.Invalid)
				XvrType = XvrFormats.GetXvrPixelFormatFromDdsFormat(DdsFormat);
			result.AddRange(BitConverter.GetBytes((uint)XvrType));
			result.AddRange(BitConverter.GetBytes(Gbix));
			result.AddRange(BitConverter.GetBytes((ushort)Width));
			result.AddRange(BitConverter.GetBytes((ushort)Height));
			result.AddRange(BitConverter.GetBytes(HeaderlessData.Length));
			result.AddRange(new byte[0x24]);
			result.AddRange(HeaderlessData);
			return result.ToArray();
		}

		/// <summary>
		/// Reads the header of an XVR texture and creates a DDS texture header used by DirectXTexUtility.
		/// </summary>
		/// <param name="data">Souce byte array containing the XVR texture header.</param>
		/// <param name="offset">Offset where the header starts.</param>
		/// <returns>DirectXTexUtility's DDSHeader structure.</returns>
		public static DDSHeader GetDdsHeaderFromXvr(byte[] data, int offset)
		{
			XvrFlags flags = (XvrFlags)BitConverter.ToUInt32(data, offset + 8);
			bool useAlpha = flags.HasFlag(XvrFlags.Alpha);
			bool useMips = flags.HasFlag(XvrFlags.Mips);
			int pixelFormat = BitConverter.ToInt32(data, offset + 0xC);
			DXGIFormat fmt = XvrFormats.GetDxgiFormatFromXvrPixelFormat((XvrFormat)pixelFormat);
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

		[Flags]
		private enum XvrFlags
		{
			Mips = 1,
			Alpha = 2,
		}
	}
}