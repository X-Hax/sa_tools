using BCnEncoder.Encoder;
using BCnEncoder.ImageSharp;
using BCnEncoder.Shared;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using static VrSharp.Xvr.DirectXTexUtility;

namespace VrSharp.Xvr
{
	public class XvrTextureEncoder : VrTextureEncoder
	{
		#region Texture Properties
		/// <summary>
		/// The texture's pixel format. This only applies to palettized textures.
		/// </summary>
		public DXGIFormat PixelFormat
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
		private DXGIFormat pixelFormat;

		/// <summary>
		/// The texture's data format.
		/// </summary>
		public DXGIFormat DataFormat
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
		private DXGIFormat dataFormat;

		/// <summary>
		/// Gets or sets if this texture has mipmaps.
		/// </summary>
		public new bool HasMipmaps
		{
			get
			{
				if (!initalized)
				{
					throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
				}

				return useMipmaps;
			}
			set
			{
				if (!initalized)
				{
					throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
				}

				useMipmaps = value;
			}
		}

		private bool useMipmaps;

		/// <summary>
		/// Gets or sets if this texture uses alpha channels
		/// </summary>
		public bool HasAlpha
		{
			get
			{
				if (!initalized)
				{
					throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
				}

				return useAlpha;
			}
			set
			{
				if (!initalized)
				{
					throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
				}

				useAlpha = value;
			}
		}

		private bool useAlpha;
		#endregion

		#region Constructors & Initalizers
		/// <summary>
		/// Opens a texture to encode from a file.
		/// </summary>
		/// <param name="file">Filename of the file that contains the texture data.</param>
		/// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
		/// <param name="dataFormat">Data format to encode the texture to.</param>
		public XvrTextureEncoder(string file, DXGIFormat pixelFormat, DXGIFormat dataFormat) : base(file)
		{
			if (decodedBitmap != null)
			{
				initalized = Initalize(pixelFormat, dataFormat);
			}
		}

		/// <summary>
		/// Opens a texture to encode from a byte array.
		/// </summary>
		/// <param name="source">Byte array that contains the texture data.</param>
		/// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
		/// <param name="dataFormat">Data format to encode the texture to.</param>
		public XvrTextureEncoder(byte[] source, DXGIFormat pixelFormat, DXGIFormat dataFormat)
			: base(source)
		{
			if (decodedBitmap != null)
			{
				initalized = Initalize(pixelFormat, dataFormat);
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
		public XvrTextureEncoder(byte[] source, int offset, int length, DXGIFormat pixelFormat, DXGIFormat dataFormat)
			: base(source, offset, length)
		{
			if (decodedBitmap != null)
			{
				initalized = Initalize(pixelFormat, dataFormat);
			}
		}

		/// <summary>
		/// Opens a texture to encode from a stream.
		/// </summary>
		/// <param name="source">Stream that contains the texture data.</param>
		/// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
		/// <param name="dataFormat">Data format to encode the texture to.</param>
		public XvrTextureEncoder(Stream source, DXGIFormat pixelFormat, DXGIFormat dataFormat)
			: base(source)
		{
			if (decodedBitmap != null)
			{
				initalized = Initalize(pixelFormat, dataFormat);
			}
		}

		/// <summary>
		/// Opens a texture to encode from a stream.
		/// </summary>
		/// <param name="source">Stream that contains the texture data.</param>
		/// <param name="length">Number of bytes to read.</param>
		/// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
		/// <param name="dataFormat">Data format to encode the texture to.</param>
		public XvrTextureEncoder(Stream source, int length, DXGIFormat pixelFormat, DXGIFormat dataFormat)
			: base(source, length)
		{
			if (decodedBitmap != null)
			{
				initalized = Initalize(pixelFormat, dataFormat);
			}
		}

		/// <summary>
		/// Opens a texture to encode from a bitmap.
		/// </summary>
		/// <param name="source">Bitmap to encode.</param>
		/// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
		/// <param name="dataFormat">Data format to encode the texture to.</param>
		public XvrTextureEncoder(Bitmap source, DXGIFormat pixelFormat, DXGIFormat dataFormat)
			: base(source)
		{
			if (decodedBitmap != null)
			{
				initalized = Initalize(pixelFormat, dataFormat);
			}
		}

		private bool Initalize(DXGIFormat pixelFormat, DXGIFormat dataFormat)
		{
			// Set the default values
			hasGlobalIndex = true;
			globalIndex = 0;
			useAlpha = true;
			useMipmaps = false;

			// Set the data format and pixel format and load the appropiate codecs
			this.dataFormat = dataFormat;
			this.pixelFormat = pixelFormat;

			return true;
		}
		#endregion

		#region Palette
		/// <summary>
		/// Gets if the texture needs an external palette file. This only applies to palettized textures.
		/// </summary>
		/// <returns></returns>
		public override bool NeedsExternalPalette => false;
		#endregion

		#region Encode Texture
		protected override MemoryStream EncodeTexture()
		{
			int type;
			List<byte> xvr = new List<byte>(GetDdsData(decodedBitmap, new byte[0], decodedBitmap.Width, decodedBitmap.Height, out type));

			if (HasMipmaps)
			{
				for (int size = textureWidth >> 1, hSize = textureHeight >> 1; size > 1 && hSize > 1; size >>= 1, hSize >>= 1)
				{
					byte[] mipmapDecodedData = BitmapToRawResized(decodedBitmap, size, 4);
					byte[] mipmapTextureData = GetDdsData(null, mipmapDecodedData, size, hSize, out int nope);
					xvr.AddRange(mipmapTextureData);
				}
			}

			xvr.InsertRange(0, GenerateXvrHeader(xvr.Count + 0x40, type));

			MemoryStream destination = new(xvr.ToArray());
			xvr = null;

			return destination;
		}

		private byte[] GetDdsData(Bitmap bmap, byte[] bp, int width, int height, out int type)
		{
			Image<Rgba32> image;
			if (bmap != null)
			{
				MemoryStream ms = new MemoryStream();
				bmap.Save(ms, ImageFormat.Png);
				image = SixLabors.ImageSharp.Image.Load<Rgba32>(ms.ToArray());
				ms.Dispose();
			} else {
				image = SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(bp, width, height);
			}
			BcEncoder encoder = new BcEncoder();

			encoder.OutputOptions.GenerateMipMaps = useMipmaps;
			encoder.OutputOptions.Quality = CompressionQuality.BestQuality;

			type = 0;
			if (HasAlpha)
			{
				encoder.OutputOptions.Format = CompressionFormat.Bc3;
				type = 0xa;
			}
			else
			{
				encoder.OutputOptions.Format = CompressionFormat.Bc1;
				type = 0x6;
			}
			encoder.OutputOptions.FileFormat = OutputFileFormat.Dds;

			MemoryStream ddsData = new MemoryStream();
			encoder.EncodeToStream(image, ddsData);

			byte[] xvr = new byte[(ddsData.Length - 0x80)];
			Array.Copy(ddsData.ToArray(), 0x80, xvr, 0, ddsData.Length - 0x80);
			ddsData.Dispose();

			return xvr;
		}

		private byte[] GenerateXvrHeader(int length, int type)
		{
			List<byte> outBytes = new();
			outBytes.AddRange(XvrTexture.xvrtFourCC);
			outBytes.AddRange(BitConverter.GetBytes(length - 0x8));
			outBytes.AddRange(BitConverter.GetBytes(GetFlags()));
			outBytes.AddRange(BitConverter.GetBytes(type));
			outBytes.AddRange(BitConverter.GetBytes(globalIndex));
			outBytes.AddRange(BitConverter.GetBytes(TextureWidth));
			outBytes.AddRange(BitConverter.GetBytes(TextureHeight));
			outBytes.AddRange(BitConverter.GetBytes(length - 0x40));
			outBytes.AddRange(new byte[0x24]);

			return outBytes.ToArray();
		}

		private int GetFlags()
		{
			int flags = 0;
			if (HasMipmaps)
			{
				flags += 0x1;
			}
			if (HasAlpha)
			{
				flags += 0x2;
			}

			return flags;
		}
		#endregion
	}
}
