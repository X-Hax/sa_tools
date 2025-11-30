using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace TextureLib
{
    // Class for Gamecube and Wii GVR textures
    public class GvrTexture : GenericTexture
    {
        const uint Magic_GBIX = 0x58494247;
        const uint Magic_GCIX = 0x58494347;
        const uint Magic_GVRT = 0x54525647;

        public GvrPaletteFormat GvrPaletteFormat;
        public GvrDataFormat GvrDataFormat;
        private GvrDataFlags GvrDataFlags;
        private byte[] HeaderlessData;

        public byte[] GetBytes()
        {
            ByteConverter.SetBigEndian(true);
            List<byte> result = new();
            if (Gbix != 0)
            {
                result.AddRange(BitConverter.GetBytes(Magic_GBIX));
                result.AddRange(BitConverter.GetBytes((uint)8));
                result.AddRange(ByteConverter.GetBytes(Gbix));
                result.AddRange(ByteConverter.GetBytes((uint)0));
            }
            result.AddRange(BitConverter.GetBytes(Magic_GVRT));
            result.AddRange(BitConverter.GetBytes((uint)(RawData.Length + 8)));
            result.Add((byte)PaletteBank);
            result.Add((byte)PaletteStartIndex);
            int flag = ((byte)GvrPaletteFormat << 4) | (byte)GvrDataFlags;            
            result.Add((byte)flag);
            result.Add((byte)GvrDataFormat);
            result.AddRange(ByteConverter.GetBytes((ushort)Width));
            result.AddRange(ByteConverter.GetBytes((ushort)Height));
            result.AddRange(RawData);
            ByteConverter.RestoreBigEndian();
            return result.ToArray();
        }

        public GvrTexture(byte[] data, int offset = 0, string name = null, TexturePalette extPalette = null)
        {
            ByteConverter.SetBigEndian(true);
            InitTexture(data, offset, name);
            int currentOffset = offset;
            // Get global index
            if (BitConverter.ToUInt32(RawData, offset) == Magic_GBIX || BitConverter.ToUInt32(RawData, offset) == Magic_GCIX)
            {
                Gbix = ByteConverter.ToUInt32(RawData, currentOffset + 0x8);
                currentOffset += 0x10;
            }
            // Parse header
            if (BitConverter.ToUInt32(RawData, currentOffset) == Magic_GVRT)
            {
                int chunksize = ByteConverter.ToInt32(RawData, currentOffset + 0x4);
                PaletteBank = RawData[currentOffset + 0x8]; // Not confirmed
                PaletteStartIndex = RawData[currentOffset + 0x9]; // Not confirmed
                GvrPaletteFormat = (GvrPaletteFormat)(RawData[currentOffset + 0x0A] & 0xF0); // Only the first 4 bits matter
                GvrDataFlags = (GvrDataFlags)(RawData[currentOffset + 0x0A] & 0x0F); // Only the last 4 bits matter
                GvrDataFormat = (GvrDataFormat)RawData[currentOffset + 0x0B];
                Width = ByteConverter.ToUInt16(RawData, currentOffset + 0xC);
                Height = ByteConverter.ToUInt16(RawData, currentOffset + 0xE);
            }
            // Set texture properties
            switch (GvrDataFormat)
            {
                case GvrDataFormat.Index4:
                    Indexed = true;
                    break;
                case GvrDataFormat.Index8:
                    Indexed = true;
                    break;
                default:
                    break;
            }
            // Parse flags
            if (GvrDataFlags.HasFlag(GvrDataFlags.Mipmaps))
                HasMipmaps = true;
            if (GvrDataFlags.HasFlag(GvrDataFlags.InternalPalette) && (GvrDataFormat == GvrDataFormat.Index4 || GvrDataFormat == GvrDataFormat.Index8 || GvrDataFormat == GvrDataFormat.Index14))
            {
                // Sometimes this flag is there even when the texture isn't indexed...
                Indexed = true;
            }
            if (GvrDataFlags.HasFlag(GvrDataFlags.ExternalPalette))
            {
                Indexed = true;
                RequiresPaletteFile = true;
            }

            currentOffset += 0x10;
            GvrPixelCodec pixelCodec = GvrPixelCodec.GetPixelCodec(GvrDataFormat);

            Console.WriteLine("\nTEXTURE INFO");
            Console.WriteLine("Width: " + Width.ToString());
            Console.WriteLine("Height: " + Height.ToString());
            Console.WriteLine("Gbix: " + Gbix.ToString());
            Console.WriteLine("Data format: " + GvrDataFormat.ToString());
            Console.WriteLine("Palette format: " + (RequiresPaletteFile ? "External" : GvrPaletteFormat.ToString()));
            Console.WriteLine("Mipmaps: " + HasMipmaps.ToString());
            Console.WriteLine("Indexed: " + Indexed.ToString());
            Console.WriteLine("Requires palette file: " + RequiresPaletteFile.ToString());
            //Console.WriteLine("Palette entries: " + paletteEntries.ToString());

            int dataSize = pixelCodec.CalculateTextureSize(Width, Height);

            if (pixelCodec.PaletteEntries != 0 && GvrDataFlags.HasFlag(GvrDataFlags.InternalPalette))
            {
                PixelCodec paletteCodec = PixelCodec.GetPixelCodec(GvrPaletteFormat, false);
                int paletteSize = paletteCodec.BytesPerPixel * pixelCodec.PaletteEntries;
                byte[] paletteData = new byte[paletteSize];
                Array.Copy(data, currentOffset, paletteData, 0, paletteSize);
                currentOffset += paletteSize;
                Palette = new TexturePalette(paletteData, paletteCodec, pixelCodec.PaletteEntries, bigEndian: true);
            }

            if (RequiresPaletteFile && extPalette != null)
                Palette = extPalette;

            if (HasMipmaps)
            {
                for (int size = Width >> 1; size > 0; size >>= 1)
                {
                    //Console.WriteLine("Mip size {0}", size);
                    dataSize += pixelCodec.CalculateTextureSize(size, size);
                }
            }

            HeaderlessData = new byte[dataSize];
            Array.Copy(data, currentOffset, HeaderlessData, 0, dataSize);

            //System.IO.File.WriteAllBytes("data.bin", HeaderlessData);
            int textureAddress = HeaderlessData.Length - pixelCodec.CalculateTextureSize(Width, Height);

            //Console.WriteLine("Offset: " + textureAddress.ToString("X"));
            ReadOnlySpan<byte> textureData = HeaderlessData[textureAddress..];
            byte[] result = pixelCodec.Decode(HeaderlessData, Width, Height);

            if (Indexed)
            {
                result = ApplyPalette(result, Width, Height).ToArray();
            }

            //System.IO.File.WriteAllBytes("test.bin", result);
            //System.IO.File.WriteAllBytes("pal.bin", palette.ToArray());
            TextureFunctions.RGBAtoBGRA(result);
            //System.IO.File.WriteAllBytes("test_reverse.bin", result);

            Image = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

            TextureFunctions.RawToBitmap(Image, result);
            Console.WriteLine(Image.GetPixel(0, 0).ToString());

            if (HasMipmaps)
            {
                int mipLevels = (int)Math.Floor(Math.Log2(Math.Max(Width, Height))) + 1;
                MipmapImages = new Bitmap[mipLevels];
                for (int mipmapIndex = 0; mipmapIndex < mipLevels; mipmapIndex++)
                {
                    int mipOffset = 0;
                    int size = Width;
                    for (int i = 0; i < mipmapIndex; i++, size >>= 1)
                    {
                        mipOffset += pixelCodec.CalculateTextureSize(size, size);
                    }
                    byte[] mipData = HeaderlessData[mipOffset..].ToArray();
                    byte[] mipRawData = pixelCodec.Decode(mipData, size, size);
                    if (Indexed)
                        mipRawData = ApplyPalette(mipRawData, size, size).ToArray();
                    TextureFunctions.RGBAtoBGRA(mipRawData);
                    Bitmap mipBitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb);
                    TextureFunctions.RawToBitmap(mipBitmap, mipRawData);
                    Console.WriteLine(mipBitmap.GetPixel(0, 0).ToString());
                    MipmapImages[mipmapIndex] = mipBitmap;
                    //mipBitmap.Save("mip" + mipmapIndex.ToString() + ".png");
                    //Console.WriteLine("Mip {0} saved", mipmapIndex);
                }
            }

            ByteConverter.RestoreBigEndian();
        }

        public GvrTexture(Bitmap texture, GvrDataFormat dataFormat, bool mipmaps, out TexturePalette? palette, uint gbix = 0, GvrPaletteFormat paletteFormat = GvrPaletteFormat.Rgb5A3orArgb4444, bool dither = false, bool paletteExternal = false, bool paletteSACompatible = true)
        {
            if (mipmaps && texture.Width != texture.Height)
            {
                Console.WriteLine("Mipmaps disabled because the texture is rectangular");
                mipmaps = false;
            }

            Gbix = gbix;
            GvrDataFormat = dataFormat;
            Width = texture.Width;
            Height = texture.Height;
            if (mipmaps)
            {
                HasMipmaps = true;
                GvrDataFlags |= GvrDataFlags.Mipmaps;
            }
            if (dataFormat == GvrDataFormat.Index8 || dataFormat == GvrDataFormat.Index4 || dataFormat == GvrDataFormat.Index14)
            {
                Indexed = true;
                GvrDataFlags |= paletteExternal ? GvrDataFlags.ExternalPalette : GvrDataFlags.InternalPalette;
                RequiresPaletteFile = paletteExternal;
            }
            GvrPaletteFormat = paletteFormat;

            GvrPixelCodec pixelCodec = GvrPixelCodec.GetPixelCodec(dataFormat);
            
            palette = null;
            MemoryStream stream = new();

            //Console.WriteLine("Encoding with: " + pixelCodec.ToString());
            // Deal with indexed
            if (pixelCodec.PaletteEntries > 0)
            {
                Bitmap indexedBitmap;

                bool index8 = dataFormat == GvrDataFormat.Index8;

                // If the texture doesn't match the target format, quantize it
                if (texture.PixelFormat != System.Drawing.Imaging.PixelFormat.Format4bppIndexed && texture.PixelFormat != System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                {
                    //Console.WriteLine("Quantize");
                    //GvrEncoding.CalculateLossyForPalette(texture, GvrPixelCodec.GetPixelCodecForPalette(paletteFormat)).Save("qq0.png");
                    indexedBitmap = TextureFunctions.QuantizeImage(GvrEncoding.CalculateLossyForPalette(texture, GvrPixelCodec.GetPixelCodecForPalette(paletteFormat)), index8, dither);
                    palette = TexturePalette.FromIndexedBitmap(indexedBitmap, index8);
                    palette.Encode(PixelCodec.GetPixelCodec(paletteFormat, paletteSACompatible), true);
                    //palette.SaveGVP("test.gvp");
                    //palette.SavePNG("palette.png");
                }
                else
                    indexedBitmap = texture;
                //indexedBitmap.Save("compressed.png");
                byte[] bitData = TextureFunctions.BitmapToRawIndexed(indexedBitmap);
                //File.WriteAllBytes("bitdata.bin", bitData);
                //Console.WriteLine(pixelCodec.ToString());
                if (!paletteExternal)
                {
                    //Console.WriteLine("Internal palette");
                    PixelCodec paletteCodec = PixelCodec.GetPixelCodec(GvrPaletteFormat, paletteSACompatible);
                    int paletteSize = paletteCodec.BytesPerPixel * pixelCodec.PaletteEntries;
                    byte[] paletteData = new byte[paletteSize];
                    Array.Copy(palette.RawData, 0, paletteData, 0, paletteSize);
                    stream.Write(paletteData);
                }
                stream.Write(pixelCodec.Encode(bitData, indexedBitmap.Width, indexedBitmap.Height));

                if (mipmaps)
                {
                    TexturePalette quantizerPalette = palette != null ? palette : TexturePalette.CreateDefaultPalette(index8);
                    PaletteQuantizer quantizer = TexturePalette.CreatePaletteQuantizer(quantizerPalette, palette.GetNumColors(), 0, dither);
                    byte[] raw = TextureFunctions.BitmapToRaw(indexedBitmap);
                    TextureFunctions.RGBAtoBGRA(raw);
                    GvrEncoding.EncodeMipMaps(SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(raw, texture.Width, texture.Height), quantizer.CreatePixelSpecificQuantizer<Rgba32>(SixLabors.ImageSharp.Configuration.Default), pixelCodec, stream);
                }
            }
            // Otherwise just encode
            else
            {
                byte[] encodedBytes = new byte[texture.Width * texture.Height * 4];
                TextureFunctions.BitmapToRaw(texture, encodedBytes);
                TextureFunctions.RGBAtoBGRA(encodedBytes);
                //File.WriteAllBytes("raw", test);
                //File.WriteAllBytes("test", pixelCodec.Encode(test, texture.Width, texture.Height));
                stream.Write(pixelCodec.Encode(encodedBytes, texture.Width, texture.Height));

                if (mipmaps)
                {
                    //Console.WriteLine("Encode mipmaps");
                    GvrEncoding.EncodeMipMaps(SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(TextureFunctions.BitmapToRaw(texture), texture.Width, texture.Height), null, pixelCodec, stream);
                }
            }
            RawData = stream.ToArray();
        }
    }
}