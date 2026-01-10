using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using static TextureLib.DirectXTexUtility;

// Class for DDS textures

namespace TextureLib
{
	// Formats supported by this library
	public enum DdsFormat
	{
		Rgb888,
		Argb8888,
		Rgb565,
		Argb1555,
		Argb4444,
		Dxt1,
		Dxt3,
		Dxt5,
		Unsupported
	}

	/* Other formats for potential support in the future (probably useless):
	// D3DFMT_P8 // PVR and GVR Index8
	// D3DFMT_DXT2 // DDS DXT2 (DXT3 premultiplied by alpha)
	// D3DFMT_DXT4 // DDS DXT4 (DXT5 premultiplied by alpha)
	// D3DFMT_P8 // PVR and GVR Index8
	// D3DFMT_V8U8 // PVR Bump
	// D3DFMT_A4L4 // GVR IntensityA8
	// D3DFMT_L8 // GVR Intensity8
	// D3DFMT_UYVY or D3DFMT_YUY2? // PVR YUV422
	*/

	public partial class DdsTexture : GenericTexture
	{
		public DdsFormat DdsFormat;
		public byte[] HeaderlessData;
		public bool UseAlpha; // For XVR

		/// <summary>
		/// Initializes a DDS texture from a byte array that contains DDS texture header and data.
		/// </summary>
		/// <param name="data">Byte array containing GVR texture header and data.</param>
		/// <param name="offset">Offset to the beginning of the GVR texture header.</param>
		/// <param name="gbix">Global index, if applicable.</param>		
		/// <param name="name">Texture name, if applicable.</param>		
		public DdsTexture(byte[] data, int offset = 0, uint gbix = 0, string name = null)
		{
			InitTexture(data, offset, name);
			Gbix = gbix;
			Decode();
		}

		/// <summary>
		/// Encodes a DDS texture from a Bitmap.
		/// </summary>
		/// <param name="texture">Source Bitmap.</param>
		/// <param name="dataFormat">Target DDS data format.</param>
		/// <param name="mipmaps">Encode mipmaps.</param>
		/// <param name="gbix">Global Index (optional).</param>
		/// <param name="name">Texture name (optional).</param>
		public DdsTexture(Bitmap texture, DdsFormat dataFormat, bool mipmaps, uint gbix = 0, string name = null)
		{
			Image = new Bitmap(texture);
			DdsFormat = dataFormat;
			HasMipmaps = mipmaps;
			Name = name;
			Gbix = gbix;
			Encode();
		}

		public override void Decode()
		{
			// Check if dealing with DDS or XVR
			DdsFunctions.DdsTextureHeaderType type = DdsFunctions.CheckHeaderType(RawData, 0);
			DDSHeader header; // Standard DDS header
			switch (type)
			{
				// If this is a DDS, just load its header
				case DdsFunctions.DdsTextureHeaderType.Dds:
					header = DdsFunctions.GetDdsHeader(RawData, 0);
					break;
				// If this is an XVR, retrieve the header and GBIX
				case DdsFunctions.DdsTextureHeaderType.Xvr:
					header = XvrTexture.GetDdsHeaderFromXvr(RawData, 0);
					Gbix = BitConverter.ToUInt32(RawData, 0x10);
					break;
				default:
					throw new Exception("Texture not in DDS or XVR format");
			}
			// Read information about the texture
			DdsFormat = DdsFunctions.IdentifyPixelFormat(header.PixelFormat);
			Width = (int)header.Width;
			Height = (int)header.Height;
			HasMipmaps = header.Flags.HasFlag(DDSHeader.HeaderFlags.MIPMAP) && header.MipMapCount > 1;
			// Set pixel and data codec
			PixelCodec pixelCodec = PixelCodec.GetPixelCodec(DdsFormat);
			DdsDataCodec dataCodec = DdsDataCodec.GetDataCodec(DdsFormat, pixelCodec, true);
			// Set up headerless data
			int headerSize = type == DdsFunctions.DdsTextureHeaderType.Xvr ? 64 : 128;
			HeaderlessData = new byte[RawData.Length - headerSize];
			Array.Copy(RawData, headerSize, HeaderlessData, 0, HeaderlessData.Length);
			// Decode main texture
			int textureAddress = 0;
			int textureSize = dataCodec.CalculateTextureSize(Width, Height);
			ReadOnlySpan<byte> textureData = HeaderlessData[textureAddress..];
			byte[] result = dataCodec.Decode(textureData, Width, Height, null);
			Image = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			TextureFunctions.RawToBitmap(Image, result);
			UseAlpha = TextureFunctions.GetAlphaLevelFromBitmap(Image) > BitmapAlphaLevel.None;
			// Decode mipmaps if present
			if (HasMipmaps)
			{
				int mipCount = (int)header.MipMapCount;
				int mipmapOffset = 0;
				int mipWidth = Width;
				int mipHeight = Height;
				MipmapImages = new Bitmap[mipCount];
				for (int m = 0; m < mipCount; m++)
				{
					int mipDataSize = dataCodec.CalculateTextureSize(mipWidth, mipHeight);
					ReadOnlySpan<byte> mipData = HeaderlessData[mipmapOffset..];
					byte[] mipmapResult = dataCodec.Decode(mipData, mipWidth, mipHeight, null);
					MipmapImages[m] = new Bitmap(mipWidth, mipHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
					TextureFunctions.RawToBitmap(MipmapImages[m], mipmapResult);
					mipWidth = Math.Max(1, mipWidth >>= 1); // The Max check is necessary for rectangular mipmaps such as those in omocha.dds in SOC\tipsK5.PAK
					mipHeight = Math.Max(1, mipHeight >>= 1);
					mipmapOffset += mipDataSize;
				}
			}
		}

		public override void Encode()
		{
			Width = Image.Width;
			Height = Image.Height;
			UseAlpha = TextureFunctions.GetAlphaLevelFromBitmap(Image) > BitmapAlphaLevel.None;
			MemoryStream outputStream = new();
			// Set pixel and data codec
			PixelCodec pixelCodec = PixelCodec.GetPixelCodec(DdsFormat);
			DdsDataCodec dataCodec = DdsDataCodec.GetDataCodec(DdsFormat, pixelCodec, UseAlpha);
			// Encode main texture
			outputStream.Write(dataCodec.Encode(TextureFunctions.BitmapToRaw(Image), Width, Height));
			// Encode mipmaps
			if (HasMipmaps)
			{
				// Calculate the number of mip levels
				int mipLevels = (int)Math.Floor(Math.Log2(Math.Max(Width, Height))) + 1;
				MipmapImages = new Bitmap[mipLevels];
				MipmapImages[0] = new Bitmap(Image);
				int mipWidth = Math.Max(1, Width >> 1);
				int mipHeight = Math.Max(1, Height >> 1);
				// DDS mipmap order: from largest to smallest
				for (int mipLevel = 1; mipLevel < mipLevels; mipLevel++)
				{
					MipmapImages[mipLevel] = new Bitmap(Image, mipWidth, mipHeight);
					outputStream.Write(dataCodec.Encode(TextureFunctions.BitmapToRaw(MipmapImages[mipLevel]), mipWidth, mipHeight));
					mipWidth = Math.Max(1, mipWidth >>= 1);
					mipHeight = Math.Max(1, mipHeight >>= 1);
				}
			}
			// Set data arrays
			HeaderlessData = outputStream.ToArray();
			RawData = GetBytes();
		}

		public override byte[] GetBytes()
		{
			DirectXTexUtility.TexMetadata meta = new DirectXTexUtility.TexMetadata
			{
				Width = Width,
				Height = Height,
				Depth = 1, 
				ArraySize = 1, 
				MipLevels = MipmapImages == null ? 1: MipmapImages.Length,
				MiscFlags2 = 0,
				Format = DdsFunctions.GetDxgiFormat(DdsFormat),
				Dimension = TexDimension.TEXTURE2D
			};
			DirectXTexUtility.GenerateDDSHeader(meta, DDSFlags.NONE, out DDSHeader outHeader, out _);
			// There is no DXGI format for RGB888 so for this format R8G8B8G8UNORM is used as a workaround to generate the header.
			// If R8G8B8G8UNORM is written into the header, override it with RGB888 parameters.
			if (DdsFunctions.ComparePixelFormats(outHeader.PixelFormat, PixelFormats.R8G8B8G8))
			{
				// Fix pixel format
				outHeader.PixelFormat = PixelFormats.R8G8B8;
				// Fix pitch
				outHeader.PitchOrLinearSize = (outHeader.Width * 24 + 7) / 8; // 24 is bpp
			}
			// If there are no mipmaps, remove the mipmaps capability (for some reason DirectXTexUtility enables it but PaintShopPro doesn't)
			if (!HasMipmaps)
			{
				outHeader.Flags &= ~DDSHeader.HeaderFlags.MIPMAP;
				outHeader.Caps &= ~(uint)DDSHeader.SurfaceFlags.MIPMAP;
			}
			List<byte> result = new List<byte>();
			result.AddRange(outHeader.GetBytes());
			result.AddRange(HeaderlessData);
			return result.ToArray();
		}

		public DdsTexture Clone()
		{
			return new DdsTexture(RawData, 0, Gbix, Name) { PakMetadata = PakMetadata, PvmxOriginalDimensions = PvmxOriginalDimensions };
		}

		public static bool Identify(byte[] data, int offset = 0)
		{
			const uint Magic_DDS = 0x20534444;
			return (BitConverter.ToUInt32(data,offset) == Magic_DDS);
		}

		public override bool CanHaveMipmaps()
		{
			// Technically even rectangular DDS textures have mipmaps I think, but the games' main formats only allow for square mipmaps.
			return Width == Height;
		}

		public override string Info()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("DDS TEXTURE INFO");
			sb.AppendLine("Width: " + Width.ToString());
			sb.AppendLine("Height: " + Height.ToString());
			sb.AppendLine("Data format: " + DdsFormat.ToString());
			sb.AppendLine("Mipmaps: " + HasMipmaps.ToString());			
			return sb.ToString();
		}
	}
}