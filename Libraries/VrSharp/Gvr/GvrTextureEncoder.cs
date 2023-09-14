using System;
using System.Drawing;
using System.IO;

namespace VrSharp.Gvr
{
    public class GvrTextureEncoder : VrTextureEncoder
    {
        #region Texture Properties
        /// <summary>
        /// Indicates the magic code used for the GBIX header. This only matters if IncludeGbixHeader is true. The default value is GvrGbixType.Gbix.
        /// </summary>
        public GvrGbixType GbixType
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return gbixType;
            }
            set
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                gbixType = value;
            }
        }
        protected GvrGbixType gbixType;

        /// <summary>
        /// The texture's data flags. Can contain one or more of the following:
        /// <para>- GvrDataFlags.Mipmaps</para>
        /// <para>- GvrDataFlags.ExternalPalette</para>
        /// <para>- GvrDataFlags.InternalPalette</para>
        /// </summary>
        public GvrDataFlags DataFlags
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
        protected GvrDataFlags dataFlags;

        /// <summary>
        /// The texture's pixel format. This only applies to palettized textures.
        /// </summary>
        public GvrPixelFormat PixelFormat
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
        private GvrPixelFormat pixelFormat;

        /// <summary>
        /// The texture's data format.
        /// </summary>
        public GvrDataFormat DataFormat
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
        private GvrDataFormat dataFormat;

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

                return ((dataFlags & GvrDataFlags.Mipmaps) != 0);
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
                    dataFlags |= GvrDataFlags.Mipmaps;
                }
                else
                {
                    // Set mipmaps to false
                    dataFlags &= ~GvrDataFlags.Mipmaps;
                }
            }
        }
        #endregion

        #region Constructors & Initalizers
        /// <summary>
        /// Opens a texture to encode from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GvrTextureEncoder(string file, GvrPixelFormat pixelFormat, GvrDataFormat dataFormat) : base(file)
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
        public GvrTextureEncoder(byte[] source, GvrPixelFormat pixelFormat, GvrDataFormat dataFormat)
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
        public GvrTextureEncoder(byte[] source, int offset, int length, GvrPixelFormat pixelFormat, GvrDataFormat dataFormat)
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
        public GvrTextureEncoder(Stream source, GvrPixelFormat pixelFormat, GvrDataFormat dataFormat)
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
        public GvrTextureEncoder(Stream source, int length, GvrPixelFormat pixelFormat, GvrDataFormat dataFormat)
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
        public GvrTextureEncoder(Bitmap source, GvrPixelFormat pixelFormat, GvrDataFormat dataFormat)
            : base(source)
        {
            if (decodedBitmap != null)
            {
                initalized = Initalize(pixelFormat, dataFormat);
            }
        }

        private bool Initalize(GvrPixelFormat pixelFormat, GvrDataFormat dataFormat)
        {
            // Set the default values
            dataFlags = GvrDataFlags.None;
            hasGlobalIndex = true;
            gbixType = GvrGbixType.Gbix;
            globalIndex = 0;

            // Set the data format and pixel format and load the appropiate codecs
            this.dataFormat = dataFormat;
            dataCodec = GvrDataCodec.GetDataCodec(dataFormat);

            // Make sure the data codec exists and we can encode to it
            if (dataCodec == null || !dataCodec.CanEncode) return false;

            // Only palettized formats require a pixel codec.
            if (dataCodec.PaletteEntries != 0)
            {
                this.pixelFormat = pixelFormat;
                pixelCodec = GvrPixelCodec.GetPixelCodec(pixelFormat);

                // Make sure the pixel codec exists and we can encode to it
                if (pixelCodec == null || !pixelCodec.CanEncode) return false;

                dataCodec.PixelCodec = pixelCodec;

                dataFlags |= GvrDataFlags.InternalPalette;

                // Convert the bitmap to an array containing indicies.
                decodedData = BitmapToRawIndexed(decodedBitmap, dataCodec.PaletteEntries, out texturePalette);
            }
            else
            {
                this.pixelFormat = GvrPixelFormat.NonIndexed;
                pixelCodec = null;

                // Convert the bitmap to an array
                decodedData = BitmapToRaw(decodedBitmap);
            }

            return true;
        }
        #endregion

        #region Palette
        /// <summary>
        /// Gets or sets if the texture needs an external palette file. This only applies to palettized textures.
        /// </summary>
        /// <returns></returns>
        public new bool NeedsExternalPalette
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return ((dataFlags & GvrDataFlags.ExternalPalette) != 0);
            }
            set
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                // If this is a non-palettized texture, don't set anything
                if (dataCodec.PaletteEntries == 0)
                    return;

                if (value)
                {
                    // Set external palette to true
                    dataFlags &= ~GvrDataFlags.InternalPalette;
                    dataFlags |= GvrDataFlags.ExternalPalette;

                    // Initalize the palette encoder
                    if (paletteEncoder == null)
                    {
                        paletteEncoder = new GvpPaletteEncoder(texturePalette, (ushort)dataCodec.PaletteEntries, pixelFormat, pixelCodec);
                    }
                }
                else
                {
                    // Set external palette to false
                    dataFlags &= ~GvrDataFlags.ExternalPalette;
                    dataFlags |= GvrDataFlags.InternalPalette;

                    // Uninitalize the palette encoder
                    if (paletteEncoder != null)
                    {
                        paletteEncoder = null;
                    }
                }
            }
        }
        #endregion

        #region Encode Texture
        protected override MemoryStream EncodeTexture()
        {
            // Calculate what the length of the texture will be
            int textureLength = 16 + (textureWidth * textureHeight * dataCodec.Bpp / 8);
            if (hasGlobalIndex)
            {
                textureLength += 16;
            }
            if ((dataFlags & GvrDataFlags.InternalPalette) != 0)
            {
                textureLength += (dataCodec.PaletteEntries * pixelCodec.Bpp / 8);
            }
            if ((dataFlags & GvrDataFlags.Mipmaps) != 0)
            {
                for (int size = 1; size < textureWidth; size <<= 1)
                {
                    textureLength += Math.Max((size * size * dataCodec.Bpp) >> 3, 32);
                }
            }

            MemoryStream destination = new MemoryStream(textureLength);

            // Write out the GBIX header (if we are including one)
            if (hasGlobalIndex)
            {
                if (gbixType == GvrGbixType.Gcix)
                {
                    destination.WriteByte((byte)'G');
                    destination.WriteByte((byte)'C');
                    destination.WriteByte((byte)'I');
                    destination.WriteByte((byte)'X');
                }
                else
                {
                    destination.WriteByte((byte)'G');
                    destination.WriteByte((byte)'B');
                    destination.WriteByte((byte)'I');
                    destination.WriteByte((byte)'X');
                }

                PTStream.WriteUInt32(destination, 8);
                PTStream.WriteUInt32BE(destination, globalIndex);
                PTStream.WriteUInt32(destination, 0);
            }

            // Write out the GVRT header
            destination.WriteByte((byte)'G');
            destination.WriteByte((byte)'V');
            destination.WriteByte((byte)'R');
            destination.WriteByte((byte)'T');

            if (hasGlobalIndex)
            {
                PTStream.WriteInt32(destination, textureLength - 24);
            }
            else
            {
                PTStream.WriteInt32(destination, textureLength - 8);
            }

            PTStream.WriteUInt16(destination, 0);
            if (PixelFormat != GvrPixelFormat.NonIndexed)
            {
                destination.WriteByte((byte)(((byte)pixelFormat << 4) | ((byte)dataFlags & 0x0F)));
            }
            else
            {
                destination.WriteByte((byte)((byte)dataFlags & 0xF));
            }
            destination.WriteByte((byte)dataFormat);

            PTStream.WriteUInt16BE(destination, textureWidth);
            PTStream.WriteUInt16BE(destination, textureHeight);

            // If we have an internal palette, write it
            if ((dataFlags & GvrDataFlags.InternalPalette) != 0)
            {
                byte[] palette = pixelCodec.EncodePalette(texturePalette, dataCodec.PaletteEntries);
                destination.Write(palette, 0, palette.Length);
            }

            // Write the texture data
            byte[] textureData = dataCodec.Encode(decodedData, textureWidth, textureHeight, null);
            destination.Write(textureData, 0, textureData.Length);

            // Write out any mipmaps
            if ((dataFlags & GvrDataFlags.Mipmaps) != 0)
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

                for (int size = textureWidth >> 1; size > 0; size >>= 1)
                {
                    byte[] mipmapDecodedData = BitmapToRawResized(decodedBitmap, size, minSize);
                    byte[] mipmapTextureData = dataCodec.Encode(mipmapDecodedData, 0, Math.Max(size, minSize), Math.Max(size, minSize));
                    destination.Write(mipmapTextureData, 0, mipmapTextureData.Length);
                }
            }

            return destination;
        }
        #endregion
    }
}