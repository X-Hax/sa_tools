using BCnEncoder.Encoder;
using BCnEncoder.Decoder;
using BCnEncoder.Shared;
using BCnEncoder.ImageSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using static VrSharp.Xvr.DirectXTexUtility;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = System.Drawing.Rectangle;

namespace VrSharp.Xvr
{
	/// <summary>
	/// .xvr textures are .dds files encapsulated in formatting akin to Sega's other VrTexture formats.
	/// At runtime, the game will create a .dds header based on the .xvr header info.
	/// </summary>
	public class XvrTexture : VrTexture
	{
		#region Fields
		// FourCC for XVRT headers.
		public static readonly byte[] xvrtFourCC = { (byte)'X', (byte)'V', (byte)'R', (byte)'T' };
		#endregion

		#region Texture Properties
		public DXGIFormat DXGIPixelFormat
		{
			get
			{
				if (!initalized)
				{
					throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
				}
				return GetFormat();
			}
		}

		private PixelFormat bitmapPixelFormat;

		private DXGIFormat GetFormat()
		{
			DXGIFormat fmt = DXGIFormat.BC1UNORM;
			switch (pixelFormat)
			{
				case 11:
				case 1:
					fmt = DXGIFormat.B8G8R8A8UNORM;
					break;
				case 12:
				case 2:
					fmt = DXGIFormat.B5G6R5UNORM;
					break;
				case 13:
				case 3:
					fmt = DXGIFormat.B5G5R5A1UNORM;
					break;
				case 14:
				case 4:
					fmt = DXGIFormat.B4G4R4A4UNORM;
					break;
				case 5:
					fmt = DXGIFormat.P8;
					break;
				case 6:
					fmt = DXGIFormat.BC1UNORM;
					break;
				case 7:
					fmt = DXGIFormat.BC2UNORM;
					break;
				case 8:
					fmt = DXGIFormat.BC2UNORM;
					break;
				case 9:
					fmt = DXGIFormat.BC3UNORM;
					break;
				case 10:
					fmt = DXGIFormat.BC3UNORM;
					break;
				case 15:
					fmt = DXGIFormat.YUY2;
					break;
				case 16:
					fmt = DXGIFormat.R8G8SNORM;
					break;
				case 17:
					fmt = DXGIFormat.A8UNORM;
					break;
				case 18: //D3DFMT_X1R5G5B5
				case 19: //D3DFMT_X8R8G8B8
					fmt = DXGIFormat.UNKNOWN;
					break;
			}

			return fmt;
		}

		private int pixelFormat;

		public bool UseAlpha
		{
			get
			{
				if (!initalized)
				{
					throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
				}

				return useAlpha;
			}
		}
		private bool useAlpha;

		private bool useMips;
		#endregion

		#region Constructors & Initalizers
		/// <summary>
		/// Open a XVR texture from a file.
		/// </summary>
		/// <param name="file">Filename of the file that contains the texture data.</param>
		public XvrTexture(string file) : base(file) { }

		/// <summary>
		/// Open a XVR texture from a byte array.
		/// </summary>
		/// <param name="source">Byte array that contains the texture data.</param>
		public XvrTexture(byte[] source) : base(source) { }

		/// <summary>
		/// Open a XVR texture from a byte array.
		/// </summary>
		/// <param name="source">Byte array that contains the texture data.</param>
		/// <param name="offset">Offset of the texture in the array.</param>
		/// <param name="length">Number of bytes to read.</param>
		public XvrTexture(byte[] source, int offset, int length) : base(source, offset, length) { }

		/// <summary>
		/// Open a XVR texture from a stream.
		/// </summary>
		/// <param name="source">Stream that contains the texture data.</param>
		public XvrTexture(Stream source) : base(source) { }

		/// <summary>
		/// Open a XVR texture from a stream.
		/// </summary>
		/// <param name="source">Stream that contains the texture data.</param>
		/// <param name="length">Number of bytes to read.</param>
		public XvrTexture(Stream source, int length) : base(source, length) { }

		protected override void Initalize()
		{
			// Check to see if what we are dealing with is a PVR texture
			if (!Is(encodedData))
			{
				throw new NotAValidTextureException("This is not a valid XVR texture.");
			}
			pvrtOffset = 0;
			paletteOffset = -1;

			int flags = BitConverter.ToInt32(encodedData, 0x8);
			useAlpha = (flags & (int)XvrFormats.XvrFlags.Alpha) > 0;
			useMips = (flags & (int)XvrFormats.XvrFlags.Mips) > 0;
			pixelFormat = BitConverter.ToInt32(encodedData, 0xC);
			globalIndex = BitConverter.ToUInt32(encodedData, 0x10);
			textureWidth = BitConverter.ToUInt16(encodedData, 0x14);
			textureHeight = BitConverter.ToUInt16(encodedData, 0x16);

			canDecode = !(GetFormat() == DXGIFormat.UNKNOWN);
			initalized = true;
		}
		#endregion

		#region Texture Check
		/// <summary>
		/// Checks for the XVRT header and validates it.
		/// </summary>
		/// <param name="source">Byte array containing the data.</param>
		/// <param name="offset">The offset in the byte array to start at.</param>
		/// <param name="length">The expected length of the PVR data minus the preceding header sizes.</param>
		/// <returns>True if the header is XVRT and it passes validation, false otherwise.</returns>
		private static bool IsValidXvrt(byte[] source, int offset, int length)
		{
			return PTMethods.Contains(source, offset, xvrtFourCC)
				&& BitConverter.ToUInt32(source, offset + 0x04) == length - 8;
		}

		/// <summary>
		/// Determines if this is a XVR texture.
		/// </summary>
		/// <param name="source">Byte array containing the data.</param>
		/// <param name="offset">The offset in the byte array to start at.</param>
		/// <param name="length">Length of the data (in bytes).</param>
		/// <returns>True if this is a XVR texture, false otherwise.</returns>
		public static bool Is(byte[] source, int offset, int length)
		{
			// XVRT (GBIX is simply a value at 0x10 in the header bytes for XVRT)
			if (length >= 0x40 && IsValidXvrt(source, offset, length))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Determines if this is a XVR texture.
		/// </summary>
		/// <param name="source">Byte array containing the data.</param>
		/// <returns>True if this is a XVR texture, false otherwise.</returns>
		public static bool Is(byte[] source)
		{
			return Is(source, 0, source.Length);
		}

		/// <summary>
		/// Determines if this is a XVR texture.
		/// </summary>
		/// <param name="source">The stream to read from. The stream position is not changed.</param>
		/// <param name="length">Number of bytes to read.</param>
		/// <returns>True if this is a XVR texture, false otherwise.</returns>
		public static bool Is(Stream source, int length)
		{
			// If the length is < 0x40, then there is no way this is a valid texture.
			if (length < 0x40)
			{
				return false;
			}
			int amountToRead = 0x40;

			byte[] buffer = new byte[amountToRead];
			source.Read(buffer, 0, amountToRead);
			source.Position -= amountToRead;

			return Is(buffer, 0, length);
		}

		/// <summary>
		/// Determines if this is a XVR texture.
		/// </summary>
		/// <param name="source">The stream to read from. The stream position is not changed.</param>
		/// <returns>True if this is a XVR texture, false otherwise.</returns>
		public static bool Is(Stream source)
		{
			return Is(source, (int)(source.Length - source.Position));
		}

		/// <summary>
		/// Determines if this is a XVR texture.
		/// </summary>
		/// <param name="file">Filename of the file that contains the data.</param>
		/// <returns>True if this is a XVR texture, false otherwise.</returns>
		public static bool Is(string file)
		{
			using (FileStream stream = File.OpenRead(file))
			{
				return Is(stream);
			}
		}
		#endregion

		#region Texture Retrieval
		public byte[] GentDDSHeader(int size)
		{
			int mipCount = 0;
			if (HasMipmaps)
			{
				int heightMip = TextureHeight;
				int widthMip = TextureWidth;
				while (heightMip > 1 && widthMip > 1)
				{
					heightMip /= 2;
					widthMip /= 2;
					mipCount++;
				}
				mipCount--;
			}
			//Map to DXGIFormat based on XVRFormats list, recovered from a PSO1 executable. Redundancies prevent this from being an enum
			DXGIFormat fmt = GetFormat();
			var meta = GenerateMataData(TextureWidth, TextureHeight, mipCount, fmt, false);
			if (UseAlpha)
			{
				meta.MiscFlags2 = TexMiscFlags2.TEXMISC2ALPHAMODEMASK;
			}
			GenerateDDSHeader(meta, DDSFlags.NONE, out var ddsHeader, out var dx10Header);

			return ddsHeader.GetBytes();
		}

		//Overridden to accomodoate xvr conversion
		protected override byte[] DecodeTexture()
		{
			Pfim.IImage img = null;
			ToImage(ref img);

			if(img != null)
			{
				byte[] data = new byte[img.DataLen];
				Array.Copy(img.Data, 0, data, 0, img.DataLen);
				return data;
			}

			throw new CannotDecodeTextureException("Cannot decode texture. The pixel format and/or data format may not be supported.");
		}

		private void ToImage(ref Pfim.IImage img)
		{
			// Make sure we can decode this texture
			if (!canDecode)
			{
				throw new CannotDecodeTextureException("Cannot decode texture. The pixel format and/or data format may not be supported.");
			}

			//Convert to .dds
			int rawDataLength = encodedData.Length - 0x40;
			byte[] ddsHeader = GentDDSHeader(rawDataLength);
			byte[] ddsData = new byte[rawDataLength + 0x80];
			Array.Copy(ddsHeader, 0, ddsData, 0, 0x80);
			Array.Copy(encodedData, 0x40, ddsData, 0x80, rawDataLength);

			MemoryStream str = new MemoryStream(ddsData);
			uint check = BitConverter.ToUInt32(ddsData, 0);
			if (check == 0x20534444) // DDS header
			{
				img = Pfim.Pfimage.FromStream(str, new Pfim.PfimConfig());
				switch (img.Format)
				{
					case Pfim.ImageFormat.Rgba32:
						bitmapPixelFormat = PixelFormat.Format32bppArgb;
						break;
					case Pfim.ImageFormat.Rgb24:
						bitmapPixelFormat = PixelFormat.Format24bppRgb;
						break;
					case Pfim.ImageFormat.R5g5b5:
						bitmapPixelFormat = PixelFormat.Format16bppRgb555;
						break;
					case Pfim.ImageFormat.R5g5b5a1:
						bitmapPixelFormat = PixelFormat.Format16bppArgb1555;
						break;
					case Pfim.ImageFormat.R5g6b5:
						bitmapPixelFormat = PixelFormat.Format16bppRgb565;
						break;
					default:
						throw new CannotDecodeTextureException("Cannot decode texture. The pixel format and/or data format may not be supported.");
				}
			}
		}

		/// <summary>
		/// Returns the decoded texture as a bitmap.
		/// </summary>
		/// <returns></returns>
		//Overriden to ensure proper pixel format
		public override Bitmap ToBitmap()
		{
			if (!initalized)
			{
				throw new TextureNotInitalizedException("Cannot decode this texture as it is not initalized.");
			}

			byte[] data = DecodeTexture();
			Bitmap img = new Bitmap(textureWidth, textureHeight, bitmapPixelFormat);
			BitmapData bitmapData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, bitmapPixelFormat);
			Marshal.Copy(data, 0, bitmapData.Scan0, data.Length);
			img.UnlockBits(bitmapData);

			return img;
		}
		#endregion

		#region Mipmaps & Mipmap Retrieval
		/// <summary>
		/// Returns the mipmaps of a texture as an array of bitmaps. The first index will contain the largest, original sized texture and the last index will contain the smallest texture.
		/// </summary>
		/// <returns></returns>
		//Override to ensure proper pixel format
		public override Bitmap[] MipmapsToBitmap()
		{
			if (!initalized)
			{
				throw new TextureNotInitalizedException("Cannot decode this texture as it is not initalized.");
			}

			// If this texture does not contain mipmaps, just return the texture
			if (!HasMipmaps)
			{
				return new Bitmap[] { ToBitmap() };
			}

			byte[][] data = DecodeMipmaps();

			Bitmap[] img = new Bitmap[data.Length];
			for (int i = 0, size = textureWidth; i < img.Length; i++, size >>= 1)
			{
				img[i] = new Bitmap(size, size, bitmapPixelFormat);
				BitmapData bitmapData = img[i].LockBits(new Rectangle(0, 0, img[i].Width, img[i].Height), ImageLockMode.WriteOnly, img[i].PixelFormat);
				Marshal.Copy(data[i], 0, bitmapData.Scan0, data[i].Length);
				img[i].UnlockBits(bitmapData);
			}

			return img;
		}

		// Decodes mipmaps
		protected override byte[][] DecodeMipmaps()
		{
			Pfim.IImage img = null;
			ToImage(ref img);
			var pinnedArray = GCHandle.Alloc(img.Data, GCHandleType.Pinned);
			var addr = pinnedArray.AddrOfPinnedObject();

			byte[][] mipmaps = new byte[img.MipMaps.Length][];
			for (int i = 0, size = textureWidth; i < img.MipMaps.Length; i++, size >>= 1)
			{
				var mip = img.MipMaps[i];
				mipmaps[i] = new byte[mip.DataLen];
				Marshal.Copy(addr + mip.DataOffset, mipmaps[i], 0, mip.DataLen);
			}

			return mipmaps;
		}
		/// <summary>
		/// Returns if the texture has mipmaps.
		/// </summary>
		//This is overriden since XvrTexture makes no use of the usual datacodec
		public override bool HasMipmaps
		{
			get
			{
				if (!initalized)
				{
					throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
				}

				return useMips;
			}
		}
		#endregion

		#region
		/// <summary>
		/// Returns if the texture needs an external palette file.
		/// </summary>
		/// <returns></returns>
		public override bool NeedsExternalPalette
		{
			get
			{
				return false;
			}
		}
		#endregion
	}
}
