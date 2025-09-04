using System;
using System.IO;
using System.Linq;
using System.Text;

namespace VrSharp.DDS
{
	public class DDSTexture : VrTexture
	{
		#region Texture Properties
		/// <summary>
		/// The texture's pixel format. This only applies to palettized textures.
		/// </summary>
		public DDSPixelFormat PixelFormat
		{
			get
			{
				if (!initalized)
				{
					throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
				}

				return pixelFormat;
			}
		}
		private DDSPixelFormat pixelFormat;

		/// <summary>
		/// The texture's data flags. Can contain one or more of the following:
		/// <para>- GvrDataFlags.Mipmaps</para>
		/// <para>- GvrDataFlags.ExternalPalette</para>
		/// <para>- GvrDataFlags.InternalPalette</para>
		/// </summary>
		public DDSHeaderFlags DataFlags
		{
			get
			{
				if (!initalized)
				{
					throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
				}

				return dataFlags;
			}
		}
		private DDSHeaderFlags dataFlags;

		/// <summary>
		/// The texture's data format.
		/// </summary>
		public DDSPixelBitFormat DataFormat
		{
			get
			{
				if (!initalized)
				{
					throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
				}

				return dataFormat;
			}
		}
		private DDSPixelBitFormat dataFormat;

		/// <summary>
		/// Returns if the texture contains mipmaps.
		/// </summary>
		public override bool HasMipmaps
		{
			get
			{
				if (!initalized)
				{
					throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
				}

				return (dataCodec.PaletteEntries == 0 && (dataFlags & DDSHeaderFlags.MipmapCount) != 0);
			}
		}
		#endregion

		#region Constructors & Initalizers
		/// <summary>
		/// Open a DDS texture from a file.
		/// </summary>
		/// <param name="file">Filename of the file that contains the texture data.</param>
		public DDSTexture(string file) : base(file) { }

		/// <summary>
		/// Open a DDS texture from a byte array.
		/// </summary>
		/// <param name="source">Byte array that contains the texture data.</param>
		public DDSTexture(byte[] source) : base(source) { }

		/// <summary>
		/// Open a DDS texture from a byte array.
		/// </summary>
		/// <param name="source">Byte array that contains the texture data.</param>
		/// <param name="offset">Offset of the texture in the array.</param>
		/// <param name="length">Number of bytes to read.</param>
		public DDSTexture(byte[] source, int offset, int length) : base(source, offset, length) { }

		/// <summary>
		/// Open a DDS texture from a stream.
		/// </summary>
		/// <param name="source">Stream that contains the texture data.</param>
		public DDSTexture(Stream source) : base(source) { }

		/// <summary>
		/// Open a DDS texture from a stream.
		/// </summary>
		/// <param name="source">Stream that contains the texture data.</param>
		/// <param name="length">Number of bytes to read.</param>
		public DDSTexture(Stream source, int length) : base(source, length) { }

		protected override void Initalize()
		{
			// Check to see if what we are dealing with is a DDS texture
			if (!Is(encodedData))
			{
				throw new NotAValidTextureException($"This is not a valid DDS texture.");
			}

			// Read information about the texture
			textureWidth = (ushort)BitConverter.ToUInt32(encodedData, 0xC);
			textureHeight = (ushort)BitConverter.ToUInt32(encodedData, 0x10);

			pixelFormat = (DDSPixelFormat)(encodedData[0x50]);
			dataFlags = (DDSHeaderFlags)BitConverter.ToUInt32(encodedData, 0x8);
			uint amask = BitConverter.ToUInt32(encodedData, 0x68);
			uint rmask = BitConverter.ToUInt32(encodedData, 0x5C);
			uint gmask = BitConverter.ToUInt32(encodedData, 0x60);
			uint bmask = BitConverter.ToUInt32(encodedData, 0x64);
			if (amask == 0)
				dataFormat = DDSPixelBitFormat.RGB565;
			else if (amask == 32768 && rmask == 31744)
				dataFormat = DDSPixelBitFormat.ARGB1555;
			else if (amask == 61440 && rmask == 3840)
				dataFormat = DDSPixelBitFormat.ARGB4444;
			else if (amask == 4278190080 && rmask == 16711680)
				dataFormat = DDSPixelBitFormat.ARGB8888;

				// Get the codecs and make sure we can decode using them
				dataCodec = DDSDataCodec.GetDataCodec(dataFormat);
			// We need a pixel codec if this is a palettized texture
			// Placeholder because palette data doesn't work properly yet
			//if (dataCodec != null && dataCodec.PaletteEntries != 0)
			//{
			pixelCodec = DDSPixelCodec.GetPixelCodec(pixelFormat);

			if (pixelCodec != null)
			{
				dataCodec.PixelCodec = pixelCodec;
				canDecode = true;
			}
			//}
			//else
			//{
			pixelFormat = DDSPixelFormat.Invalid;

			if (dataCodec != null)
			{
				canDecode = true;
			}
			//}

			// If the texture contains mipmaps, gets the offsets of them
			if (canDecode && paletteEntries == 0 && (dataFlags & DDSHeaderFlags.MipmapCount - 1) != 0)
			{
				//mipmapOffsets = new int[(int)Math.Log(textureWidth, 2)];
				mipmapOffsets = new int[2];
				int mipmapOffset = 0;
				for (int i = 0, size = textureWidth; i < mipmapOffsets.Length; i++, size >>= 1)
				{
					mipmapOffsets[i] = mipmapOffset;
					mipmapOffset += Math.Max(size * size * (dataCodec.Bpp >> 3), 32);
				}
			}
			dataOffset = 0x80;

			initalized = true;
		}
		#endregion

		#region Texture Check
		/// <summary>
		/// Determines if this is a DDS texture.
		/// </summary>
		/// <param name="source">Byte array containing the data.</param>
		/// <param name="offset">The offset in the byte array to start at.</param>
		/// <param name="length">Length of the data (in bytes).</param>
		/// <returns>True if this is a DDS texture, false otherwise.</returns>
		public static bool Is(byte[] source, int offset, int length)
		{
			// Fairly simple. Just check for DDS magic and no compression
			uint check = BitConverter.ToUInt32(source, 0);
			uint check2 = BitConverter.ToUInt32(source, 0x50);
			if (check == 0x20534444 && check2 != 0x00000004) // DDS header
				return true;

			return false;
		}

		/// <summary>
		/// Determines if this is a DDS texture.
		/// </summary>
		/// <param name="source">Byte array containing the data.</param>
		/// <returns>True if this is a DDS texture, false otherwise.</returns>
		public static bool Is(byte[] source)
		{
			return Is(source, 0, source.Length);
		}

		/// <summary>
		/// Determines if this is a DDS texture.
		/// </summary>
		/// <param name="source">The stream to read from. The stream position is not changed.</param>
		/// <param name="length">Number of bytes to read.</param>
		/// <returns>True if this is a DDS texture, false otherwise.</returns>
		public static bool Is(Stream source, int length)
		{
			// If the length is < 16, then there is no way this is a valid texture.
			if (length < 16)
			{
				return false;
			}

			// Let's see if we should check 16 bytes or 32 bytes
			int amountToRead = 0;
			if (length < 32)
			{
				amountToRead = 16;
			}
			else
			{
				amountToRead = 32;
			}

			byte[] buffer = new byte[amountToRead];
			source.Read(buffer, 0, amountToRead);
			source.Position -= amountToRead;

			return Is(buffer, 0, length);
		}

		/// <summary>
		/// Determines if this is a DDS texture.
		/// </summary>
		/// <param name="source">The stream to read from. The stream position is not changed.</param>
		/// <returns>True if this is a DDS texture, false otherwise.</returns>
		public static bool Is(Stream source)
		{
			return Is(source, (int)(source.Length - source.Position));
		}

		/// <summary>
		/// Determines if this is a DDS texture.
		/// </summary>
		/// <param name="file">Filename of the file that contains the data.</param>
		/// <returns>True if this is a DDS texture, false otherwise.</returns>
		public static bool Is(string file)
		{
			using (FileStream stream = File.OpenRead(file))
			{
				return Is(stream);
			}
		}
		#endregion
	}
}
