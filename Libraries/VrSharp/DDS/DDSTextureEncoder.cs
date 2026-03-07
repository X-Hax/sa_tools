using System;
using System.Drawing;
using System.IO;
using System.Linq;


namespace VrSharp.DDS
{
	public class DDSTextureEncoder : VrTextureEncoder
	{
		#region Texture Properties

		
		/// <summary>
		/// The texture's header flags. Can contain one or more of the following:
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
		protected DDSHeaderFlags dataFlags;

		/// <summary>
		/// The texture's pixel format. This only applies to palettized textures.
		/// </summary>
		//public GvrPixelFormat PixelFormat
		//{
		//	get
		//	{
		//		if (!initalized)
		//		{
		//			throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
		//		}

		//		return pixelFormat;
		//	}
		//}
		//private GvrPixelFormat pixelFormat;

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
		/// The texture's data format.
		/// </summary>
		public DDSCaps DataCapsFormat
		{
			get
			{
				if (!initalized)
				{
					throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
				}

				return dataCapsFormat;
			}
		}
		private DDSCaps dataCapsFormat;

		/// <summary>
		/// Gets or sets if this texture has mipmaps. This only applies to 4-bit or 16-bit non-palettized textures.
		/// </summary>
		public new bool HasMipmaps
		{
			get
			{
				if (!initalized)
				{
					throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
				}

				return ((dataFlags & DDSHeaderFlags.MipmapCount) != 0);
			}
			set
			{
				if (!initalized)
				{
					throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
				}

				// Mipmaps can only be used on 4-bit or 16-bit non-palettized textures
				if (dataCodec.PaletteEntries != 0 || (dataCodec.Bpp != 4 && dataCodec.Bpp != 16))
					return;

				if (value)
				{
					// Set mipmaps to true
					dataFlags |= DDSHeaderFlags.MipmapCount;
					dataCapsFormat |= DDSCaps.Complex | DDSCaps.Mipmap;
				}
				else
				{
					// Set mipmaps to false
					dataFlags &= ~DDSHeaderFlags.MipmapCount;
					dataCapsFormat &= ~DDSCaps.Complex | ~DDSCaps.Mipmap;

				}
			}
		}
		public System.Drawing.Drawing2D.InterpolationMode InterpolationMode
		{
			get
			{
				if (!initalized)
				{
					throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
				}
				return interpolationMode;
			}
			set 
			{
				if (!initalized)
				{
					throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
				}
				interpolationMode = value; 
			}

		}
		private System.Drawing.Drawing2D.InterpolationMode interpolationMode;
		
		#endregion

		#region Constructors & Initalizers
		/// <summary>
		/// Opens a texture to encode from a file.
		/// </summary>
		/// <param name="file">Filename of the file that contains the texture data.</param>
		/// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
		/// <param name="dataFormat">Data format to encode the texture to.</param>
		public DDSTextureEncoder(string file, DDSPixelBitFormat dataFormat) : base(file)
		{
			if (decodedBitmap != null)
			{
				initalized = Initalize(dataFormat);
			}
		}

		/// <summary>
		/// Opens a texture to encode from a byte array.
		/// </summary>
		/// <param name="source">Byte array that contains the texture data.</param>
		/// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
		/// <param name="dataFormat">Data format to encode the texture to.</param>
		public DDSTextureEncoder(byte[] source, DDSPixelBitFormat dataFormat)
			: base(source)
		{
			if (decodedBitmap != null)
			{
				initalized = Initalize(dataFormat);
			}
		}

		/// <summary>
		/// Opens a texture to encode from a byte array.
		/// </summary>
		/// <param name="source">Byte array that contains the texture data.</param>
		/// <param name="offset">Offset of the texture in the array.</param>
		/// <param name="length">Number of bytes to read.</param>
		/// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
		/// <param name="dataFormat">Data format to encode the texture to.</param>
		public DDSTextureEncoder(byte[] source, int offset, int length, DDSPixelBitFormat dataFormat)
			: base(source, offset, length)
		{
			if (decodedBitmap != null)
			{
				initalized = Initalize(dataFormat);
			}
		}

		/// <summary>
		/// Opens a texture to encode from a stream.
		/// </summary>
		/// <param name="source">Stream that contains the texture data.</param>
		/// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
		/// <param name="dataFormat">Data format to encode the texture to.</param>
		public DDSTextureEncoder(Stream source, DDSPixelBitFormat dataFormat)
			: base(source)
		{
			if (decodedBitmap != null)
			{
				initalized = Initalize(dataFormat);
			}
		}

		/// <summary>
		/// Opens a texture to encode from a stream.
		/// </summary>
		/// <param name="source">Stream that contains the texture data.</param>
		/// <param name="length">Number of bytes to read.</param>
		/// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
		/// <param name="dataFormat">Data format to encode the texture to.</param>
		public DDSTextureEncoder(Stream source, int length, DDSPixelBitFormat dataFormat)
			: base(source, length)
		{
			if (decodedBitmap != null)
			{
				initalized = Initalize(dataFormat);
			}
		}

		/// <summary>
		/// Opens a texture to encode from a bitmap.
		/// </summary>
		/// <param name="source">Bitmap to encode.</param>
		/// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
		/// <param name="dataFormat">Data format to encode the texture to.</param>
		public DDSTextureEncoder(Bitmap source, DDSPixelBitFormat dataFormat)
			: base(source)
		{
			if (decodedBitmap != null)
			{
				initalized = Initalize(dataFormat);
			}
		}
		public DDSTextureEncoder(Bitmap source, DDSPixelBitFormat dataFormat, Bitmap[] mipsource)
			: base(source, mipsource)
		{
			if (decodedMipBitmap == null)
			{
				decodedMipBitmap = mipsource;
			}
			if (decodedBitmap != null)
			{
				initalized = InitalizeWithMips(dataFormat, mipsource);
			}
		}

		private bool Initalize(DDSPixelBitFormat dataFormat)
		{
			// Set the default values
			dataFlags = DDSHeaderFlags.Caps | DDSHeaderFlags.PixelType | DDSHeaderFlags.Height | DDSHeaderFlags.Width | DDSHeaderFlags.Pitch;
			dataCapsFormat = DDSCaps.Texture;
			// Set the data format and pixel format and load the appropiate codecs
			this.dataFormat = dataFormat;
			dataCodec = DDSDataCodec.GetDataCodec(dataFormat);

			// Make sure the data codec exists and we can encode to it
			if (dataCodec == null || !dataCodec.CanEncode) return false;
				pixelCodec = null;

			// Convert the bitmap to an array
			decodedData = BitmapToRaw(decodedBitmap);

			return true;
		}
		private bool InitalizeWithMips(DDSPixelBitFormat dataFormat, Bitmap[] mipsource)
		{
			// Set the default values
			dataFlags = DDSHeaderFlags.Caps | DDSHeaderFlags.PixelType | DDSHeaderFlags.Height | DDSHeaderFlags.Width | DDSHeaderFlags.Pitch;
			dataCapsFormat = DDSCaps.Texture;
			// Set the data format and pixel format and load the appropiate codecs
			this.dataFormat = dataFormat;
			dataCodec = DDSDataCodec.GetDataCodec(dataFormat);

			// Make sure the data codec exists and we can encode to it
			if (dataCodec == null || !dataCodec.CanEncode) return false;

			pixelCodec = null;

			// Convert the bitmap to an array
			decodedData = BitmapToRaw(decodedBitmap);
			if (mipsource != null)
			{
				int n = 0;
				decodedMipData = new byte[mipsource.Length][];
				do
				{
					decodedMipData[n] = BitmapToRaw(decodedMipBitmap[n]);
					n++;
				}
				while (n < mipsource.Length);
			}

			return true;
		}
		#endregion

		#region Palette
		///// <summary>
		///// Gets or sets if the texture needs an external palette file. This only applies to palettized textures.
		///// </summary>
		///// <returns></returns>
		//public new bool NeedsExternalPalette
		//{
		//	get
		//	{
		//		if (!initalized)
		//		{
		//			throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
		//		}

		//		return ((dataFlags & GvrDataFlags.ExternalPalette) != 0);
		//	}
		//	set
		//	{
		//		if (!initalized)
		//		{
		//			throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
		//		}

		//		// If this is a non-palettized texture, don't set anything
		//		if (dataCodec.PaletteEntries == 0)
		//			return;

		//		if (value)
		//		{
		//			// Set external palette to true
		//			dataFlags &= ~GvrDataFlags.InternalPalette;
		//			dataFlags |= GvrDataFlags.ExternalPalette;

		//			// Initalize the palette encoder
		//			if (paletteEncoder == null)
		//			{
		//				paletteEncoder = new GvpPaletteEncoder(texturePalette, (ushort)dataCodec.PaletteEntries, pixelFormat, pixelCodec);
		//			}
		//		}
		//		else
		//		{
		//			// Set external palette to false
		//			dataFlags &= ~GvrDataFlags.ExternalPalette;
		//			dataFlags |= GvrDataFlags.InternalPalette;

		//			// Uninitalize the palette encoder
		//			if (paletteEncoder != null)
		//			{
		//				paletteEncoder = null;
		//			}
		//		}
		//	}
		//}
		#endregion

		#region Encode Texture
		protected override MemoryStream EncodeTexture()
		{
			// Calculate what the length of the texture will be
			int textureLength = 0x80 + (textureWidth * textureHeight * dataCodec.Bpp / 8);
			int mipmapcount = 1;
			uint rmask = 0;
			uint gmask = 0;
			uint bmask = 0;
			uint amask = 0;
			switch (dataFormat)
			{
				case DDSPixelBitFormat.ARGB8888:
				default:
					rmask = 16711680;   //0x0000FF00
					gmask = 65280;      //0x00FF0000
					bmask = 255;        //0xFF000000
					amask = 4278190080; //0x000000FF
					break;
				case DDSPixelBitFormat.BGRA8888:
					rmask = 255;        //0xFF000000
					gmask = 65280;      //0x00FF0000
					bmask = 16711680;   //0x0000FF00
					amask = 4278190080; //0x000000FF
					break;
				case DDSPixelBitFormat.RGB565:
					rmask = 63488; //0x00F80000
					gmask = 2016;  //0xE0070000
					bmask = 31;    //0x1F000000
					amask = 0;
					break;
				case DDSPixelBitFormat.ARGB1555:
					rmask = 31744;
					gmask = 992;
					bmask = 31;
					amask = 32768;
					break;
				case DDSPixelBitFormat.ARGB4444:
					rmask = 3840;
					gmask = 240;
					bmask = 15;
					amask = 61440;
					break;
			}
			if (HasMipmaps)
			{
				int sizew = textureWidth;
				int sizeh = textureHeight;
				do 
				{
					sizew = sizew / 2;
					sizeh = sizeh / 2;
					textureLength += (sizew * sizeh) * (dataCodec.Bpp / 8);
					mipmapcount++;
				}
				while (sizew > 1 || sizeh > 1);
			}

			MemoryStream destination = new MemoryStream(textureLength);
			destination.WriteByte((byte)'D');
			destination.WriteByte((byte)'D');
			destination.WriteByte((byte)'S');
			destination.WriteByte((byte)' ');
			PTStream.WriteUInt32(destination, 124);
			PTStream.WriteUInt32(destination, (uint)dataFlags);
			PTStream.WriteUInt32(destination, textureHeight);
			PTStream.WriteUInt32(destination, textureWidth);
			PTStream.WriteUInt32(destination, (uint)((textureWidth * dataCodec.Bpp + 7) / 8));
			PTStream.WriteInt32(destination, 1);
			PTStream.WriteUInt32(destination, (uint)mipmapcount);
			//Reserved Space
			do
			{
				PTStream.WriteInt32(destination, 0);
			}
			while (destination.Position != 0x4C);
			PTStream.WriteUInt32(destination, 32);
			PTStream.WriteUInt32(destination, (uint)(dataFormat == DDSPixelBitFormat.RGB565 ? DDSPixelFormat.RGB : DDSPixelFormat.RGBA));
			PTStream.WriteUInt32(destination, 0);
			PTStream.WriteUInt32(destination, (uint)dataCodec.Bpp);
			PTStream.WriteUInt32(destination, rmask);
			PTStream.WriteUInt32(destination, gmask);
			PTStream.WriteUInt32(destination, bmask);
			PTStream.WriteUInt32(destination, amask);
			PTStream.WriteUInt32(destination, (uint)dataCapsFormat);
			do
			{
				PTStream.WriteUInt32(destination, 0);
			}
			while (destination.Position != 0x80);

			if (HasMipmaps && decodedMipData != null)
			{
				int i = 0;
				int size = textureWidth, hSize = textureHeight;
				do
				{
					byte[] mipdataset = dataCodec.Encode(decodedMipData[i], size, hSize, null);
					size = size >> 1;
					hSize = hSize >> 1;
					destination.Write(mipdataset, 0, mipdataset.Length);
					i++;
				}
				while (i < decodedMipData.Length);
			}
			else 
			{
				// Write the texture data
				byte[] textureData = dataCodec.Encode(decodedData, textureWidth, textureHeight, null);
				destination.Write(textureData, 0, textureData.Length);
				if (HasMipmaps)
				{
					// Calculate the minimum size for each mipmap
					int minSize = 0;
					if (dataCodec.Bpp == 4)
					{
						// 8x8 blocks
						minSize = 8;
					}
					else if (dataCodec.Bpp == 16)
					{
						// 4x4 blocks
						minSize = 4;
					}
					
					for (int size = textureWidth >> 1, hSize = textureHeight >> 1; size > 1 && hSize > 1; size >>= 1, hSize >>= 1)
					{
						byte[] mipmapDecodedData = BitmapToRawResized(decodedBitmap, size, minSize);
						byte[] mipmapTextureData = dataCodec.Encode(mipmapDecodedData, 0, Math.Max(size, minSize), Math.Max(size, minSize));
						destination.Write(mipmapTextureData, 0, mipmapTextureData.Length);
					}
				}
			}
			return destination;
		}
		#endregion
	}
}

