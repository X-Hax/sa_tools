using System;
using System.IO;

namespace VrSharp
{
    public abstract class VpPalette
    {
        #region Fields
        protected bool initalized = false; // Is the palette initalized?

        protected ushort paletteEntries; // Number of palette entries

        protected byte[] encodedData; // Encoded palette data (VR data)

        protected VrPixelCodec pixelCodec; // Pixel codec
        #endregion

        #region Internal Methods
        internal ushort PaletteEntries
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the palette is not initalized.");
                }

                return paletteEntries;
            }
        }

        internal byte[] EncodedData
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the palette is not initalized.");
                }

                return encodedData;
            }
        }

        internal VrPixelCodec PixelCodec
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the palette is not initalized.");
                }

                return pixelCodec;
            }
        }
        #endregion

        #region Constructors & Initalizers
        // Open a texture from a file.
        public VpPalette(string file)
        {
            encodedData = File.ReadAllBytes(file);

            if (encodedData != null)
            {
                initalized = Initalize();
            }
        }

        // Open a texture from a byte array.
        public VpPalette(byte[] source) : this(source, 0, source.Length) { }

        public VpPalette(byte[] source, int offset, int length)
        {
            if (source == null || (offset == 0 && source.Length == length))
            {
                encodedData = source;
            }
            else if (source != null)
            {
                encodedData = new byte[length];
                Array.Copy(source, offset, encodedData, 0, length);
            }

            if (encodedData != null)
            {
                initalized = Initalize();
            }
        }

        // Open a texture from a stream.
        public VpPalette(Stream source) : this(source, (int)(source.Length - source.Position)) { }

        public VpPalette(Stream source, int length)
        {
            encodedData = new byte[length];
            source.Read(encodedData, 0, length);

            if (encodedData != null)
            {
                initalized = Initalize();
            }
        }

        protected abstract bool Initalize();

        /// <summary>
        /// Returns if the texture was loaded successfully.
        /// </summary>
        /// <returns></returns>
        public bool Initalized
        {
            get { return initalized; }
        }
        #endregion
    }
}