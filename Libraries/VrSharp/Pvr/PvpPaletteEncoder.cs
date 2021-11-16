using System;
using System.IO;

namespace VrSharp.Pvr
{
    public class PvpPaletteEncoder : VpPaletteEncoder
    {
        #region Fields
        private PvrPixelFormat pixelFormat; // Pixel format
        #endregion

        #region Constructors & Initalizers
        internal PvpPaletteEncoder(byte[][] palette, ushort numColors, PvrPixelFormat pixelFormat, VrPixelCodec pixelCodec)
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

            // Write out the PVPL header
            destination.WriteByte((byte)'P');
            destination.WriteByte((byte)'V');
            destination.WriteByte((byte)'P');
            destination.WriteByte((byte)'L');

            PTStream.WriteInt32(destination, paletteLength - 8);

            destination.WriteByte((byte)pixelFormat);
            destination.WriteByte(0);

            PTStream.WriteUInt32(destination, 0);

            PTStream.WriteUInt16(destination, paletteEntries);

            // Write the palette data
            byte[] palette = pixelCodec.EncodePalette(decodedPalette, paletteEntries);
            destination.Write(palette, 0, palette.Length);

            return destination;
        }
        #endregion
    }
}