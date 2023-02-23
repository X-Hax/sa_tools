using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace VrSharp.Svr
{
    public class SvrTexture : VrTexture
    {
        #region Texture Properties
        /// <summary>
        /// The texture's pixel format.
        /// </summary>
        public SvrPixelFormat PixelFormat
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
        private SvrPixelFormat pixelFormat;

        /// <summary>
        /// The texture's data format.
        /// </summary>
        public SvrDataFormat DataFormat
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
        private SvrDataFormat dataFormat;
        #endregion

        #region Constructors & Initalizers
        /// <summary>
        /// Open a SVR texture from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        public SvrTexture(string file) : base(file) { }

        /// <summary>
        /// Open a SVR texture from a byte array.
        /// </summary>
        /// <param name="source">Byte array that contains the texture data.</param>
        public SvrTexture(byte[] source) : base(source) { }

        /// <summary>
        /// Open a SVR texture from a byte array.
        /// </summary>
        /// <param name="source">Byte array that contains the texture data.</param>
        /// <param name="offset">Offset of the texture in the array.</param>
        /// <param name="length">Number of bytes to read.</param>
        public SvrTexture(byte[] source, int offset, int length) : base(source, offset, length) { }

        /// <summary>
        /// Open a SVR texture from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        public SvrTexture(Stream source) : base(source) { }

        /// <summary>
        /// Open a SVR texture from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        /// <param name="length">Number of bytes to read.</param>
        public SvrTexture(Stream source, int length) : base(source, length) { }

        protected override void Initalize()
        {
            // Check to see if what we are dealing with is a SVR texture
            if (!Is(encodedData))
            {
                throw new NotAValidTextureException("This is not a valid GVR texture.");
            }

            // Determine the offsets of the GBIX (if present) and PVRT header chunks.
            if (PTMethods.Contains(encodedData, 0, Encoding.UTF8.GetBytes("GBIX")))
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
                globalIndex = BitConverter.ToUInt32(encodedData, gbixOffset + 0x08);
            }
            else
            {
                globalIndex = 0;
            }

            // Read information about the texture
            textureWidth  = BitConverter.ToUInt16(encodedData, pvrtOffset + 0x0C);
            textureHeight = BitConverter.ToUInt16(encodedData, pvrtOffset + 0x0E);

            pixelFormat = (SvrPixelFormat)encodedData[pvrtOffset + 0x08];
            dataFormat  = (SvrDataFormat)encodedData[pvrtOffset + 0x09];

            // Get the codecs and make sure we can decode using them
            pixelCodec = SvrPixelCodec.GetPixelCodec(pixelFormat);
            dataCodec = SvrDataCodec.GetDataCodec(dataFormat);

            if (dataCodec != null && pixelCodec != null)
            {
                dataCodec.PixelCodec = pixelCodec;
                canDecode = true;
            }

            // Set the palette and data offsets
            paletteEntries = dataCodec.PaletteEntries;
            if (!canDecode || paletteEntries == 0 || dataCodec.NeedsExternalPalette)
            {
                paletteOffset = -1;
                dataOffset = pvrtOffset + 0x10;
            }
            else
            {
                paletteOffset = pvrtOffset + 0x10;
                dataOffset = paletteOffset + (paletteEntries * (pixelCodec.Bpp >> 3));
            }

            initalized = true;
        }
		#endregion

		#region Palette
		/// <summary>
		/// Set the palette data from an external palette file.
		/// </summary>
		/// <param name="palette">A SvpPalette object</param>
		/// <param name="bank">Palette bank ID</param>
		public void SetPalette(SvpPalette palette, int bank = 0)
        {
            SetPalette((VpPalette)palette, bank);
        }
        #endregion

        #region Texture Check
        /// <summary>
        /// Determines if this is a SVR texture.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="offset">The offset in the byte array to start at.</param>
        /// <param name="length">Length of the data (in bytes).</param>
        /// <returns>True if this is a SVR texture, false otherwise.</returns>
        public static bool Is(byte[] source, int offset, int length)
        {
            // GBIX and PVRT
            if (length >= 0x20 &&
                PTMethods.Contains(source, offset + 0x00, Encoding.UTF8.GetBytes("GBIX")) &&
                PTMethods.Contains(source, offset + 0x10, Encoding.UTF8.GetBytes("PVRT")) &&
                source[offset + 0x19] >= 0x60 && source[offset + 0x19] < 0x70)
            {
                // Some SVR files have an extra byte at the end for seemingly no reason.
                UInt32 expected_length = BitConverter.ToUInt32(source, offset + 0x14);
                if (expected_length == length - 24 || expected_length == length - 24 - 1)
                    return true;
            }

            // PVRT (and no GBIX chunk)
            else if (length >= 0x10 &&
                PTMethods.Contains(source, offset + 0x00, Encoding.UTF8.GetBytes("PVRT")) &&
                source[offset + 0x09] >= 0x60 && source[offset + 0x09] < 0x70)
            {
                // Some SVR files have an extra byte at the end for seemingly no reason.
                UInt32 expected_length = BitConverter.ToUInt32(source, offset + 0x04);
                if (expected_length == length - 8 || expected_length == length - 8 - 1)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if this is a SVR texture.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <returns>True if this is a SVR texture, false otherwise.</returns>
        public static bool Is(byte[] source)
        {
            return Is(source, 0, source.Length);
        }

        /// <summary>
        /// Determines if this is a SVR texture.
        /// </summary>
        /// <param name="source">The stream to read from. The stream position is not changed.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <returns>True if this is a SVR texture, false otherwise.</returns>
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
        /// Determines if this is a SVR texture.
        /// </summary>
        /// <param name="source">The stream to read from. The stream position is not changed.</param>
        /// <returns>True if this is a SVR texture, false otherwise.</returns>
        public static bool Is(Stream source)
        {
            return Is(source, (int)(source.Length - source.Position));
        }

        /// <summary>
        /// Determines if this is a SVR texture.
        /// </summary>
        /// <param name="file">Filename of the file that contains the data.</param>
        /// <returns>True if this is a SVR texture, false otherwise.</returns>
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
