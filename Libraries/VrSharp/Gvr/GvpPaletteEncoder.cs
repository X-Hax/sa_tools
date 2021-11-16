using System;
using System.IO;

namespace VrSharp.Gvr
{
    public class GvpPaletteEncoder : VpPaletteEncoder
    {
        #region Fields
        private GvrPixelFormat pixelFormat; // Pixel format
        #endregion

        #region Constructors & Initalizers
        internal GvpPaletteEncoder(byte[][] palette, ushort numColors, GvrPixelFormat pixelFormat, VrPixelCodec pixelCodec)
            : base(palette, numColors, pixelCodec)
        {
            this.pixelFormat = pixelFormat;
        }
        #endregion

        #region Encode Palette
        protected override MemoryStream EncodePalette()
        {
            // Calculate what the length of the palette will be
            int paletteLength = 16 + (paletteEntries * pixelCodec.Bpp / 8);

            MemoryStream destination = new MemoryStream(paletteLength);

            // Write out the GVPL header
            destination.WriteByte((byte)'G');
            destination.WriteByte((byte)'V');
            destination.WriteByte((byte)'P');
            destination.WriteByte((byte)'L');

            PTStream.WriteInt32(destination, paletteLength - 8);

            destination.WriteByte(0);
            destination.WriteByte((byte)pixelFormat);

            PTStream.WriteUInt32(destination, 0);

            PTStream.WriteUInt16BE(destination, paletteEntries);

            // Write the palette data
            byte[] palette = pixelCodec.EncodePalette(decodedPalette, paletteEntries);
            destination.Write(palette, 0, palette.Length);

            return destination;
        }
        #endregion
    }
}