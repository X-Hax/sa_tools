using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

// Other formats: PNG (1 pixel height), Adobe ACT, JASC PAL, Microsoft PAL, PaintShopPro TXT, GIMP GPL

namespace TextureLib
{
    public class TexturePalette
    {
        private const uint Magic_PVPL = 0x4C505650; // PVPL
        private const uint Magic_GVPL = 0x4C505647; // GVPL
        private const uint Magic_PVRP = 0x50525650; // PVRP

        public int StartBank;
        public int StartColor;

        public byte[] DecodedData; // ARGB8888 color data
        public byte[] RawData; // Encoded data without header

        private PixelCodec paletteCodec; // Codec used for the palette format

        private bool isBigEndian; // Whether the data in RawData is Big Endian or not 

        public int GetNumColors() => DecodedData.Length / 4;

        /// <summary>
        /// Creates a palette from a byte array containing a header (PVP or GVP) and data.
        /// </summary>
        /// <param name="data">Loaded byte array.</param>
        /// <param name="saCompatible">Use ARGB1555 instead of IntensityA8 and ARGB4444 instead of RGB5A3 (for SADX and SA2B).</param>
        public TexturePalette(byte[] data, bool saCompatible = true)
        {
            int numColors = 0; // Number of colors in the palette
            int sectionSize = 0; // Size stored in the header
            int colorsSize = 0; // Size of the section that contains color data
            int colorsStartOffset = 0; // Offset of the section that contains color data
            byte paletteFmt; // Palette format ID
            bool isGvr = false; // Whether or not the palette was loaded from a GVR file (values are Big Endian)

            ByteConverter.BackupBigEndian();

            switch (BitConverter.ToUInt32(data, 0))
            {
                case Magic_PVRP: // Some format used in old Kamui demos
                    sectionSize = BitConverter.ToInt32(data, 4);
                    colorsSize = sectionSize - 4;
                    paletteFmt = data[8]; // 9 is unknown
                    paletteCodec = PixelCodec.GetPixelCodec((PvrPixelFormat)paletteFmt);
                    numColors = colorsSize / paletteCodec.BytesPerPixel;
                    colorsStartOffset = 0xC;
                    break;
                case Magic_PVPL:
                    sectionSize = BitConverter.ToInt32(data, 4);
                    colorsSize = sectionSize - 8;
                    paletteFmt = data[8]; // 9 is unknown
                    StartBank = BitConverter.ToInt16(data, 0xA);
                    StartColor = BitConverter.ToInt16(data, 0xC);
                    numColors = BitConverter.ToInt16(data, 0xE);
                    colorsStartOffset = 0x10;
                    paletteCodec = PixelCodec.GetPixelCodec((PvrPixelFormat)paletteFmt);
                    break;
                case Magic_GVPL:
                    ByteConverter.BigEndian = true;
                    sectionSize = BitConverter.ToInt32(data, 4); // Section size is Little Endian, everything else is Big Endian
                    colorsSize = sectionSize - 8;
                    paletteFmt = data[9]; // 8 is unknown
                    StartBank = ByteConverter.ToInt16(data, 0xA);
                    StartColor = ByteConverter.ToInt16(data, 0xC);
                    numColors = ByteConverter.ToInt16(data, 0xE);
                    colorsStartOffset = 0x10;
                    paletteCodec = PixelCodec.GetPixelCodec((GvrPaletteFormat)paletteFmt, saCompatible);
                    isBigEndian = isGvr = true;
                    break;
                default:
                    throw new NotImplementedException("TexturePalette: The data is not supported.");
            }
#if DEBUG
            Console.WriteLine("\nPALETTE INFO");
            Console.Write("Pixel format: {0}", paletteFmt.ToString());
            Console.Write(" ({0})\n", isGvr ? (GvrPaletteFormat)paletteFmt : (PvrPixelFormat)paletteFmt);
            Console.WriteLine("Codec: {0}", paletteCodec.ToString());
            Console.WriteLine("Num colors: {0}", numColors);
            Console.WriteLine("Start bank: {0}", StartBank.ToString());
            Console.WriteLine("Start color: {0}", StartColor.ToString());
#endif
            RawData = data[colorsStartOffset..];
            int srcAddress = 0;
            int dstAddress = 0;
            DecodedData = new byte[numColors * 4];
            ReadOnlySpan<byte> srcData = RawData.AsSpan();
            Span<byte> dest = DecodedData[dstAddress..];
            for (int index = 0; index < numColors; index++)
            {
                paletteCodec.DecodePixel(srcData[srcAddress..],
                   dest[dstAddress..], ByteConverter.BigEndian);

                srcAddress += paletteCodec.BytesPerPixel;
                dstAddress += 4 * paletteCodec.Pixels;
            }
            DecodedData = dest.ToArray();

            ByteConverter.RestoreBigEndian();
        }

        /// <summary>
        /// Creates a palette from a byte array containing headerless encoded data.
        /// </summary>
        /// <param name="rawEncodedData">Loaded byte array.</param>
        /// <param name="codec">The pixel codec for the format of the data.</param>
        /// <param name="numColors">Number of entries in the palette.</param>
        /// <param name="colorsStartOffset">Start offset of the encoded data.</param>
        /// <param name="bigEndian">Whether the data is Big Endian or not.</param>
        public TexturePalette(byte[] rawEncodedData, PixelCodec codec, int numColors, int colorsStartOffset = 0, bool bigEndian = false)
        {
            bool bigEndianBk = ByteConverter.BigEndian;
            ByteConverter.BigEndian = bigEndian;
            ReadOnlySpan<byte> srcData = rawEncodedData[colorsStartOffset..];
            int srcAddress = 0;
            int dstAddress = 0;
            DecodedData = new byte[numColors * 4];
            Span<byte> dest = DecodedData[dstAddress..];

            for (int index = 0; index < numColors; index++)
            {
                //Console.WriteLine("Color {0}", index);
                codec.DecodePixel(srcData[srcAddress..],
                   dest[dstAddress..], ByteConverter.BigEndian);

                srcAddress += codec.BytesPerPixel;
                dstAddress += 4 * codec.Pixels;
            }
            ByteConverter.BigEndian = bigEndianBk;
            DecodedData = dest.ToArray();
            //File.WriteAllBytes("pal_dec", DecodedData);
            paletteCodec = codec;
        }

        /// <summary>
        /// Creates a palette from a Bitmap containing colors for the palette (width = number of colors, height = 1).
        /// </summary>
        /// <param name="bitmap">Loaded Bitmap.</param>
        /// <param name="codec">The pixel codec for the format of the data.</param>
        /// <param name="startBank">The "starting bank" of the palette.</param>
        /// <param name="startColor">The "starting color" on the palette bank.</param>
        /// <param name="bigEndian">Whether the data will be encoded in Big Endian or not.</param>
        public TexturePalette(Bitmap bitmap, PixelCodec codec, int startBank = 0, int startColor = 0, bool bigEndian = false)
        {
            StartBank = startBank;
            StartColor = startColor;
            paletteCodec = codec;
            TextureFunctions.BitmapToRaw(bitmap, DecodedData);
			//TextureFunctions.RGBAtoBGRA(DecodedData);
            Encode(codec, bigEndian);
        }

        /// <summary>
        /// Encodes the color data using the currently selected pixel codec, or optionally with a different codec.
        /// </summary>
        /// <param name="codec">The pixel codec for the format of the data.</param>
        /// <param name="encodeAsBigEndian">Whether the data will be encoded in Big Endian or not.</param>
        public void Encode(PixelCodec codec = null, bool encodeAsBigEndian = false)
        {
            if (codec != null)
                paletteCodec = codec;
            isBigEndian = encodeAsBigEndian;
            int numColors = GetNumColors();
            RawData = new byte[paletteCodec.BytesPerPixel * numColors];
            Span<byte> dest = RawData.AsSpan();
            int srcAddress = 0;
            int dstAddress = 0;
            for (int index = 0; index < numColors; index++)
            {
                paletteCodec.EncodePixel(DecodedData[srcAddress..],
                   dest[dstAddress..], encodeAsBigEndian);
                srcAddress += 4;
                dstAddress += paletteCodec.BytesPerPixel;
            }
        }

        public byte[] GetBytes(bool gvp)
        {
            ByteConverter.SetBigEndian(gvp);
            List<byte> result = new List<byte>();
            result.AddRange(gvp ? BitConverter.GetBytes(Magic_GVPL) : BitConverter.GetBytes(Magic_PVPL));
            int sectionSize = RawData.Length + 8;
            result.AddRange(BitConverter.GetBytes(sectionSize)); // This part is Little Endian even in GVP
            ushort format = 0;
            switch (paletteCodec)
            {
                case IntensityA8PixelCodec:
                case ARGB1555PixelCodec:
                    format = 0;
                    break;
                case RGB565PixelCodec:
                    format = 1;
                    break;
                case ARGB4444PixelCodec:
                case RGB5A3PixelCodec:
                    format = 2;
                    break;
                case ARGB8888PixelCodec:
                    format = 6;
                    break;
            }
            result.AddRange(ByteConverter.GetBytes(format));
            result.AddRange(ByteConverter.GetBytes((ushort)StartBank));
            result.AddRange(ByteConverter.GetBytes((ushort)StartColor));
            result.AddRange(ByteConverter.GetBytes((ushort)GetNumColors()));
            byte[] writeData = new byte[RawData.Length];
            RawData.CopyTo(writeData, 0);
            if ((!isBigEndian && gvp) || (isBigEndian && !gvp))
                ByteConverter.SwapEndianArray(writeData, paletteCodec.BytesPerPixel);
            result.AddRange(writeData);
            ByteConverter.RestoreBigEndian();
            return result.ToArray();
        }

        public void SavePVP(string outputPath)
        {
            File.WriteAllBytes(outputPath, GetBytes(false));
        }

        public void SaveGVP(string outputPath)
        {
            File.WriteAllBytes(outputPath, GetBytes(true));
        }

		public void Save(string outputPath, bool gvp)
		{
			File.WriteAllBytes(outputPath, GetBytes(gvp));
		}

		public void SavePNG(string outputPath)
        {
            byte[] colorsBitmap = new byte[DecodedData.Length];
            Array.Copy(DecodedData, 0, colorsBitmap, 0, DecodedData.Length);
            //TextureFunctions.RGBAtoBGRA(colorsBitmap);
            Bitmap img = new Bitmap(GetNumColors(), 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            TextureFunctions.RawToBitmap(img, colorsBitmap);
            img.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
        }

        public static TexturePalette CreateDefaultPalette(bool index8, PixelCodec codec = null)
        {
            if (codec == null)
                codec = new ARGB8888PixelCodec();
            int numColors = index8 ? 256 : 16;
            byte[] rawData = new byte[numColors * 4];

            for (int index = 0; index < numColors; index++)
            {
                if (index8)
                {
                    rawData[index * 4 + 0] = (byte)index;
                    rawData[index * 4 + 1] = (byte)index;
                    rawData[index * 4 + 2] = (byte)index;
                }
                else
                {
                    rawData[index * 4 + 0] = (byte)(index * 16);
                    rawData[index * 4 + 1] = (byte)(index * 16);
                    rawData[index * 4 + 2] = (byte)(index * 16);
                }
                rawData[index * 4 + 3] = 0xFF;
            }
            return new TexturePalette(rawData, codec, numColors);
        }

        public Color[] ToColorArray()
        {
            List<Color> colors = new List<Color>();
            for (int entry = 0; entry < GetNumColors(); entry++)
            {
                int argb = BitConverter.ToInt32(DecodedData, 4 * entry);
				// Might need BGRA conversion?
                colors.Add(Color.FromArgb(argb));
            }
            return colors.ToArray();
        }

        public static TexturePalette FromIndexedBitmap(Bitmap bitmap, bool forceIndex8 = false)
        {
            int numColors = bitmap.Palette.Entries.Length;
            bool index8 = forceIndex8 || numColors > 16;
            byte[] colorData = new byte[(index8 ? 256 : 16) * 4];
			for (int i = 0; i < bitmap.Palette.Entries.Length; i++)
			{
				byte[] color = BitConverter.GetBytes(bitmap.Palette.Entries[i].ToArgb());
				// Convert from BGRA because Windows bullshit!
				byte temp = color[i];     // Store R
				color[i] = color[i + 2]; // R becomes B
				color[i + 2] = temp;     // B becomes original R
				Array.Copy(color, 0, colorData, i * 4, 4);
			}
            return new TexturePalette(colorData, new ARGB8888PixelCodec(), index8 ? 256 : 16);
        }

        /// <summary>
        /// Creates a palette quantizer that can be used to convert a color image to an indexed image.
        /// </summary>
        /// <param name="palette">The palette to match the colors against.</param>
        /// <param name="width">The number of colors from the palette to use.</param>
        /// <param name="offset">The offset at which to start using colors from the palette.</param>
        /// <param name="dither">Whether to allow dithering when quantizing.</param>
        /// <returns>The quantizer.</returns>
        public static PaletteQuantizer CreatePaletteQuantizer(TexturePalette palette, int width, int offset, bool dither)
        {
            SixLabors.ImageSharp.Color[] paletteColors = new SixLabors.ImageSharp.Color[width];
            ReadOnlySpan<byte> colorData = palette.DecodedData;

            for (int i = 0; i < width; i++)
            {
                ReadOnlySpan<byte> color = colorData.Slice((offset + i) * 4, 4);
                paletteColors[i] = new Rgba32(color[0], color[1], color[2], color[3]);
            }

            return new PaletteQuantizer(
                new(paletteColors),
                new QuantizerOptions()
                {
                    MaxColors = width,
                    Dither = dither ? QuantizerConstants.DefaultDither : null
                });
        }

		/// <summary>
		/// Sorts the colors in a palette by luminance.
		/// </summary>
		public void SortByLuminance()
		{
			(int, byte)[] luminanceLUT = new (int, byte)[GetNumColors()];
			ReadOnlySpan<byte> data = DecodedData;

			for (int i = 0; i < luminanceLUT.Length; i++)
			{
				ReadOnlySpan<byte> color = data[(i * 4)..];
				luminanceLUT[i] = (i, TextureFunctions.GetLuminance(color));
			}

			Array.Sort(luminanceLUT, (a, b) => a.Item2.CompareTo(b.Item2));

			byte[] newPalette = new byte[data.Length];
			Span<byte> destination = newPalette;
			for (int i = 0; i < luminanceLUT.Length; i++)
			{
				int dstIndex = luminanceLUT[i].Item1;
				data.Slice(luminanceLUT[i].Item1 * 4, 4).CopyTo(destination[(i * 4)..]);
			}

			DecodedData = newPalette;
		}
	}
}