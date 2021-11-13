using System;
using System.IO;
using System.Text;

namespace VrSharp.Gvr
{
    public class GvpPalette : VpPalette
    {
        #region Palette Properties
        /// <summary>
        /// The palette's pixel format.
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
        #endregion

        #region Constructors & Initalizers
        /// <summary>
        /// Open a GVP palette from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the palette data.</param>
        public GvpPalette(string file) : base(file) { }

        /// <summary>
        /// Open a GVP palette from a byte array.
        /// </summary>
        /// <param name="source">Byte array that contains the palette data.</param>
        public GvpPalette(byte[] source) : base(source) { }

        /// <summary>
        /// Open a GVP palette from a byte array.
        /// </summary>
        /// <param name="source">Byte array that contains the palette data.</param>
        /// <param name="offset">Offset of the palette in the array.</param>
        /// <param name="length">Number of bytes to read.</param>
        public GvpPalette(byte[] source, int offset, int length) : base(source, offset, length) { }

        /// <summary>
        /// Open a GVP palette from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the palette data.</param>
        public GvpPalette(Stream source) : base(source) { }

        /// <summary>
        /// Open a GVP palette from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the palette data.</param>
        /// <param name="length">Number of bytes to read.</param>
        public GvpPalette(Stream source, int length) : base(source, length) { }

        protected override bool Initalize()
        {
            // Check to see if what we are dealing with is a GVP palette
            if (!Is(encodedData))
                return false;

            // Get the pixel format and the codec and make sure we can decode using them
            pixelFormat = (GvrPixelFormat)encodedData[0x09];
            pixelCodec = GvrPixelCodec.GetPixelCodec(pixelFormat);
            if (pixelCodec == null) return false;

            // Get the number of colors contained in the palette
            paletteEntries = PTMethods.ToUInt16BE(encodedData, 0x0E);

            return true;
        }
        #endregion

        #region Palette Check
        /// <summary>
        /// Determines if this is a GVP palette.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="offset">The offset in the byte array to start at.</param>
        /// <param name="length">Length of the data (in bytes).</param>
        /// <returns>True if this is a GVP palette, false otherwise.</returns>
        public static bool Is(byte[] source, int offset, int length)
        {
            if (length >= 16 &&
                PTMethods.Contains(source, offset + 0x00, Encoding.UTF8.GetBytes("GVPL")) &&
                BitConverter.ToUInt32(source, offset + 0x04) == length - 8)
                return true;

            return false;
        }

        /// <summary>
        /// Determines if this is a GVP palette.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <returns>True if this is a GVP palette, false otherwise.</returns>
        public static bool Is(byte[] source)
        {
            return Is(source, 0, source.Length);
        }

        /// <summary>
        /// Determines if this is a GVP palette.
        /// </summary>
        /// <param name="source">The stream to read from. The stream position is not changed.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <returns>True if this is a GVP palette, false otherwise.</returns>
        public static bool Is(Stream source, int length)
        {
            // If the length is < 16, then there is no way this is a valid palette file.
            if (length < 16)
            {
                return false;
            }

            byte[] buffer = new byte[16];
            source.Read(buffer, 0, 16);
            source.Position -= 16;

            return Is(buffer, 0, length);
        }

        /// <summary>
        /// Determines if this is a GVP palette.
        /// </summary>
        /// <param name="source">The stream to read from. The stream position is not changed.</param>
        /// <returns>True if this is a GVP palette, false otherwise.</returns>
        public static bool Is(Stream source)
        {
            return Is(source, (int)(source.Length - source.Position));
        }

        /// <summary>
        /// Determines if this is a GVP palette.
        /// </summary>
        /// <param name="file">Filename of the file that contains the data.</param>
        /// <returns>True if this is a GVP palette, false otherwise.</returns>
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