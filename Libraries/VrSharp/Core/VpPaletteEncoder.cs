using System;
using System.IO;

namespace VrSharp
{
    public abstract class VpPaletteEncoder
    {
        #region Fields
        protected byte[][] decodedPalette; // Decoded palette data (32-bit RGBA)

        protected ushort paletteEntries; // Number of palette entries

        protected VrPixelCodec pixelCodec; // Pixel codec
        #endregion

        #region Constructors & Initalizers
        internal VpPaletteEncoder(byte[][] palette, ushort numColors, VrPixelCodec pixelCodec)
        {
            decodedPalette = palette;
            paletteEntries = numColors;
            this.pixelCodec = pixelCodec;
        }
        #endregion

        #region Palette Retrieval
        /// <summary>
        /// Returns the encoded palette as a byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            return EncodePalette().ToArray();
        }

        /// <summary>
        /// Returns the encoded palette as a stream.
        /// </summary>
        /// <returns></returns>
        public MemoryStream ToStream()
        {
            MemoryStream paletteStream = EncodePalette();
            paletteStream.Position = 0;
            return paletteStream;
        }

        /// <summary>
        /// Saves the encoded palette to the specified path.
        /// </summary>
        /// <param name="path">Name of the file to save the data to.</param>
        public void Save(string path)
        {
            using (FileStream destination = File.Create(path))
            {
                MemoryStream paletteStream = EncodePalette();
                paletteStream.Position = 0;
                PTStream.CopyTo(paletteStream, destination);
            }
        }

        /// <summary>
        /// Saves the encoded palette to the specified stream.
        /// </summary>
        /// <param name="destination">The stream to save the texture to.</param>
        public void Save(Stream destination)
        {
            MemoryStream paletteStream = EncodePalette();
            paletteStream.Position = 0;
            PTStream.CopyTo(paletteStream, destination);
        }
        #endregion

        #region Private Methods
        protected abstract MemoryStream EncodePalette();
        #endregion
    }
}