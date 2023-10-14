using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace VrSharp.Gvr
{
    public class GvrTexture : VrTexture
    {
        #region Texture Properties
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
        private GvrDataFlags dataFlags;

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

                return (dataCodec.PaletteEntries == 0 && (dataFlags & GvrDataFlags.Mipmaps) != 0);
            }
        }
        #endregion

        #region Constructors & Initalizers
        /// <summary>
        /// Open a GVR texture from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        public GvrTexture(string file) : base(file) { }

        /// <summary>
        /// Open a GVR texture from a byte array.
        /// </summary>
        /// <param name="source">Byte array that contains the texture data.</param>
        public GvrTexture(byte[] source) : base(source) { }

        /// <summary>
        /// Open a GVR texture from a byte array.
        /// </summary>
        /// <param name="source">Byte array that contains the texture data.</param>
        /// <param name="offset">Offset of the texture in the array.</param>
        /// <param name="length">Number of bytes to read.</param>
        public GvrTexture(byte[] source, int offset, int length) : base(source, offset, length) { }

        /// <summary>
        /// Open a GVR texture from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        public GvrTexture(Stream source) : base(source) { }

        /// <summary>
        /// Open a GVR texture from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        /// <param name="length">Number of bytes to read.</param>
        public GvrTexture(Stream source, int length) : base(source, length) { }

        protected override void Initalize()
        {
            // Check to see if what we are dealing with is a GVR texture
            if (!Is(encodedData))
            {
                throw new NotAValidTextureException("This is not a valid GVR texture.");
            }

            // Determine the offsets of the GBIX/GCIX (if present) and GCIX header chunks.
            if (PTMethods.Contains(encodedData, 0, Encoding.UTF8.GetBytes("GBIX")) ||
                PTMethods.Contains(encodedData, 0, Encoding.UTF8.GetBytes("GCIX")))
            {
                gbixOffset = 0x00;
                pvrtOffset = 0x10;
            }
            else
            {
                gbixOffset = -1;
                pvrtOffset = 0x00;
            }

            // Read the global index (if it is present). If it is not present, just set it to 0.
            if (gbixOffset != -1)
            {
                globalIndex = PTMethods.ToUInt32BE(encodedData, gbixOffset + 0x08);
            }
            else
            {
                globalIndex = 0;
            }

            // Read information about the texture
            textureWidth  = PTMethods.ToUInt16BE(encodedData, pvrtOffset + 0x0C);
            textureHeight = PTMethods.ToUInt16BE(encodedData, pvrtOffset + 0x0E);

            pixelFormat = (GvrPixelFormat)(encodedData[pvrtOffset + 0x0A] >> 4); // Only the first 4 bits matter
            dataFlags   = (GvrDataFlags)(encodedData[pvrtOffset + 0x0A] & 0x0F); // Only the last 4 bits matter
            dataFormat  = (GvrDataFormat)encodedData[pvrtOffset + 0x0B];

            // Get the codecs and make sure we can decode using them
            dataCodec = GvrDataCodec.GetDataCodec(dataFormat);

            // We need a pixel codec if this is a palettized texture
            if (dataCodec != null && dataCodec.PaletteEntries != 0)
            {
                pixelCodec = GvrPixelCodec.GetPixelCodec(pixelFormat);

                if (pixelCodec != null)
                {
                    dataCodec.PixelCodec = pixelCodec;
                    canDecode = true;
                }
            }
            else
            {
                pixelFormat = GvrPixelFormat.NonIndexed;

                if (dataCodec != null)
                {
                    canDecode = true;
                }
            }

            // Set the palette and data offsets
            paletteEntries = dataCodec.PaletteEntries;
            if (!canDecode || paletteEntries == 0 || (paletteEntries != 0 && (dataFlags & GvrDataFlags.ExternalPalette) != 0))
            {
                paletteOffset = -1;
                dataOffset = pvrtOffset + 0x10;
            }
            else
            {
                paletteOffset = pvrtOffset + 0x10;
                dataOffset = paletteOffset + (paletteEntries * (pixelCodec.Bpp >> 3));
            }

            // If the texture contains mipmaps, gets the offsets of them
            if (canDecode && paletteEntries == 0 && (dataFlags & GvrDataFlags.Mipmaps) != 0)
            {
                mipmapOffsets = new int[(int)Math.Log(textureWidth, 2) + 1];

                int mipmapOffset = 0;
                for (int i = 0, size = textureWidth; i < mipmapOffsets.Length; i++, size >>= 1)
                {
                    mipmapOffsets[i] = mipmapOffset;
                    mipmapOffset += Math.Max(size * size * (dataCodec.Bpp >> 3), 32);
                }
            }

            initalized = true;
        }
		#endregion

		#region Palette
		/// <summary>
		/// Set the palette data from an external palette file.
		/// </summary>
		/// <param name="clut">A GvpPalette object</param>
		/// <param name="bank">Palette bank ID</param>
		public void SetPalette(GvpPalette palette, int bank = 0)
        {
            SetPalette((VpPalette)palette, bank);
        }

        /// <summary>
        /// Returns if the texture needs an external clut file.
        /// </summary>
        /// <returns></returns>
        public override bool NeedsExternalPalette
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return (paletteEntries != 0 && (dataFlags & GvrDataFlags.ExternalPalette) != 0);
            }
        }
        #endregion

        #region Texture Check
        /// <summary>
        /// Determines if this is a GVR texture.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="offset">The offset in the byte array to start at.</param>
        /// <param name="length">Length of the data (in bytes).</param>
        /// <returns>True if this is a GVR texture, false otherwise.</returns>
        public static bool Is(byte[] source, int offset, int length)
        {
            // GBIX and GVRT
            if (length >= 32 &&
                PTMethods.Contains(source, offset + 0x00, Encoding.UTF8.GetBytes("GBIX")) &&
                PTMethods.Contains(source, offset + 0x10, Encoding.UTF8.GetBytes("GVRT")) &&
                BitConverter.ToUInt32(source, offset + 0x14) == length - 24)
                return true;

            // GCIX and GVRT
            else if (length >= 32 &&
                PTMethods.Contains(source, offset + 0x00, Encoding.UTF8.GetBytes("GCIX")) &&
                PTMethods.Contains(source, offset + 0x10, Encoding.UTF8.GetBytes("GVRT")) &&
                BitConverter.ToUInt32(source, offset + 0x14) == length - 24)
                return true;

            // GVRT (and no GBIX or GCIX chunk)
            else if (length > 16 &&
                PTMethods.Contains(source, offset + 0x00, Encoding.UTF8.GetBytes("GVRT")) &&
                BitConverter.ToUInt32(source, offset + 0x04) == length - 8)
                return true;

            return false;
        }

        /// <summary>
        /// Determines if this is a GVR texture.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <returns>True if this is a GVR texture, false otherwise.</returns>
        public static bool Is(byte[] source)
        {
            return Is(source, 0, source.Length);
        }

        /// <summary>
        /// Determines if this is a GVR texture.
        /// </summary>
        /// <param name="source">The stream to read from. The stream position is not changed.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <returns>True if this is a GVR texture, false otherwise.</returns>
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
        /// Determines if this is a GVR texture.
        /// </summary>
        /// <param name="source">The stream to read from. The stream position is not changed.</param>
        /// <returns>True if this is a GVR texture, false otherwise.</returns>
        public static bool Is(Stream source)
        {
            return Is(source, (int)(source.Length - source.Position));
        }

        /// <summary>
        /// Determines if this is a GVR texture.
        /// </summary>
        /// <param name="file">Filename of the file that contains the data.</param>
        /// <returns>True if this is a GVR texture, false otherwise.</returns>
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