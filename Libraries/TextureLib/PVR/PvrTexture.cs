using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace TextureLib
{
    // Class for Dreamcast PVR textures
    public class PvrTexture : GenericTexture
    {
        const uint Magic_GBIX = 0x58494247;
        const uint Magic_PVRT = 0x54525650;

        public PvrPixelFormat PvrPixelFormat;
        public PvrDataFormat PvrDataFormat;

        private byte[] HeaderlessData; // Raw data without the header
        public bool RLE; // Whether the texture is RLE compressed or not (not implemented)

        public PvrTexture(byte[] data, int offset = 0, string name = null, TexturePalette extPalette = null)
        {
            InitTexture(data, offset, name);
            Palette = extPalette;
            int currentOffset = offset;
            int gbixOffset = 0x00;
            int pvrtOffset = 0x00;
            if (BitConverter.ToUInt32(RawData, currentOffset) == Magic_GBIX)
            {
                gbixOffset = 0x00;
                pvrtOffset = 0x08 + BitConverter.ToInt32(RawData, gbixOffset + 4);
            }
            else if (BitConverter.ToUInt32(RawData, currentOffset + 0x04) == Magic_GBIX)
            {
                gbixOffset = 0x04;
                pvrtOffset = 0x0C + BitConverter.ToInt32(RawData, gbixOffset + 4);
            }
            else if (BitConverter.ToUInt32(RawData, currentOffset + 0x04) == Magic_PVRT)
            {
                gbixOffset = -1;
                pvrtOffset = 0x04;
            }
            else
            {
                gbixOffset = -1;
                pvrtOffset = 0x00;
            }
            // Get global index
            if (gbixOffset != -1)
                Gbix = BitConverter.ToUInt32(RawData, gbixOffset + 0x8);
            else
                Gbix = 0;
            Console.WriteLine("GBIXOffset: {0}", gbixOffset);
            currentOffset = pvrtOffset;
            // Parse header
            int chunksize = BitConverter.ToInt32(RawData, currentOffset + 0x4);
            PvrPixelFormat = (PvrPixelFormat)RawData[currentOffset + 0x8];
            PvrDataFormat = (PvrDataFormat)RawData[currentOffset + 0x9];
            PaletteBank = RawData[currentOffset + 0xA];
            PaletteStartIndex = RawData[currentOffset + 0xB];
            Width = ByteConverter.ToUInt16(RawData, currentOffset + 0xC);
            Height = ByteConverter.ToUInt16(RawData, currentOffset + 0xE);
            // Override pixel format for Bitmap
            if (PvrDataFormat is PvrDataFormat.Bitmap || PvrDataFormat is PvrDataFormat.BitmapMipmaps)
                PvrPixelFormat = PvrPixelFormat.Argb8888orYUV420;
            // Set texture properties
            switch (PvrDataFormat)
            {
                case PvrDataFormat.VqMipmaps:
                case PvrDataFormat.SmallVqMipmaps:
                case PvrDataFormat.SquareTwiddledMipmaps:
                case PvrDataFormat.RectangleMipmaps:
                case PvrDataFormat.RectangleStrideMipmaps:
                case PvrDataFormat.BitmapMipmaps:
                case PvrDataFormat.SquareTwiddledMipmapsAlt:
                    HasMipmaps = true;
                    break;
                case PvrDataFormat.Index4:
                case PvrDataFormat.Index8:
                    Indexed = true;
                    RequiresPaletteFile = true;
                    break;
                case PvrDataFormat.Index4Mipmaps:
                case PvrDataFormat.Index8Mipmaps:
                    Indexed = true;
                    HasMipmaps = true;
                    RequiresPaletteFile = true;
                    break;
            }

            currentOffset += 16;

            PixelCodec pixelCodec = PixelCodec.GetPixelCodec(PvrPixelFormat);
            PvrDataCodec dataCodec = PvrDataCodec.Create(PvrDataFormat, pixelCodec);
            int dataSize = dataCodec.CalculateTextureSize(Width, Height);
            int paletteEntries = dataCodec.GetPaletteEntries(Width);

            if (PvrPixelFormat is PvrPixelFormat.Argb8888orYUV420 && dataSize > RawData.Length)
                throw new NotImplementedException("YUV420 support is not implemented");

            Console.WriteLine("\nTEXTURE INFO");
            Console.WriteLine("Width: " + Width.ToString());
            Console.WriteLine("Height: " + Height.ToString());
            Console.WriteLine("Gbix: " + Gbix.ToString());
            Console.WriteLine("Pixel format: {0} ({1} bytes per {2} pixels)", PvrPixelFormat.ToString(), pixelCodec.BytesPerPixel.ToString(), pixelCodec.Pixels.ToString());
            Console.WriteLine("Data format: " + PvrDataFormat.ToString());
            Console.WriteLine("Mipmaps: " + HasMipmaps.ToString());
            Console.WriteLine("Indexed: " + Indexed.ToString());
            Console.WriteLine("Palette entries: " + paletteEntries.ToString());

            if (paletteEntries > 0 && !dataCodec.NeedsExternalPalette)
            {
                dataSize += paletteEntries / pixelCodec.Pixels * pixelCodec.BytesPerPixel;
            }
            if (dataCodec.HasMipmaps)
            {
                for (int size = 1; size < Width; size <<= 1)
                {
                    dataSize += dataCodec.CalculateTextureSize(size, size);
                }
            }
            HeaderlessData = new byte[dataSize];
            Array.Copy(data, currentOffset, HeaderlessData, 0, dataSize);

            ReadOnlySpan<byte> palette = DecodeInternalPalette(dataCodec, out int paletteSize);
            //System.IO.File.WriteAllBytes("data.bin", HeaderlessData);
            int textureAddress = HeaderlessData.Length - dataCodec.CalculateTextureSize(Width, Height);

            Console.WriteLine("Offset: " + textureAddress.ToString("X"));
            ReadOnlySpan<byte> textureData = HeaderlessData[textureAddress..];
            byte[] result = dataCodec.Decode(textureData, Width, Height, palette);

            if (Indexed)
            {
                result = ApplyPalette(result, Width, Height).ToArray();
            }

            //System.IO.File.WriteAllBytes("test.bin", result);
            //System.IO.File.WriteAllBytes("pal.bin", palette.ToArray());
            //TextureFunctions.RGBAtoBGRA(result);
            //System.IO.File.WriteAllBytes("test_reverse.bin", result);

            Image = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            TextureFunctions.RawToBitmap(Image, result);

            //if (PvrDataFormat is PvrDataFormat.Bitmap || PvrDataFormat is PvrDataFormat.BitmapMipmaps)
            //Image.RotateFlip(RotateFlipType.RotateNoneFlipY);

            if (HasMipmaps)
            {
                // Calculate the number of mip levels
                int mipLevels = (int)Math.Floor(Math.Log2(Math.Max(Width, Height))) + 1;
                MipmapImages = new Bitmap[mipLevels];
                int[] mipmapOffsets = new int[mipLevels];
                // Start offset for the first mipmap
                int mipmapOffset = paletteSize;
                for (int i = mipLevels - 1, sizex = 1; i >= 0; i--, sizex <<= 1)
                {
                    mipmapOffsets[i] = mipmapOffset;
                    byte[] src = HeaderlessData[mipmapOffsets[i]..];
                    int mipDataSize = Math.Max(1, HeaderlessData.Length - paletteSize - src.Length);
                    Console.WriteLine("Mipmap {0} ({1}x{1}) : {2} (size {3})", i, sizex, mipmapOffsets[i].ToString("X"), mipDataSize.ToString());
                    byte[] mipRawData = dataCodec.Decode(src, sizex, sizex, palette);
                    // Workarounds for 1x1 mipmaps
                    if (sizex == 1)
                    {
                        // In VQ Mipmap formats, the 1x1 mipmap is retrieved from the bottom right pixel of the 2x2 mipmap
                        if (PvrDataFormat == PvrDataFormat.VqMipmaps || PvrDataFormat == PvrDataFormat.SmallVqMipmaps)
                        {
                            src = HeaderlessData[(mipmapOffset + 1)..];
                            mipRawData = dataCodec.Decode(src, sizex * 2, sizex * 2, palette);
                            mipDataSize = HeaderlessData.Length - paletteSize - src.Length;
                            byte[] newmipdata = new byte[4];
                            Array.Copy(mipRawData, 12, newmipdata, 0, 4);
                            mipRawData = newmipdata;
                            Console.WriteLine("\tVQ Mipmap for 1x1 : {0} (size {1})", (mipmapOffset + 1).ToString("X"), mipDataSize.ToString());
                        }
                        // In YUV422 textures, the 1x1 mipmap is stored as RGB565
                        else if (PvrPixelFormat == PvrPixelFormat.Yuv422)
                        {
                            PvrDataCodec dataCodecTemp = PvrDataCodec.Create(PvrDataFormat, new RGB565PixelCodec());
                            mipRawData = dataCodecTemp.Decode(src, sizex, sizex, palette);
                            Console.WriteLine("\tYUV Mipmap for 1x1 in RGB565 format");
                        }
                    }
                    if (Indexed)
                        mipRawData = ApplyPalette(mipRawData, sizex, sizex).ToArray();
                    Console.WriteLine("MipRawData {0}", mipRawData.Length);
                    //TextureFunctions.RGBAtoBGRA(mipRawData);
                    Bitmap mipBitmap = new Bitmap(sizex, sizex, PixelFormat.Format32bppArgb);
                    TextureFunctions.RawToBitmap(mipBitmap, mipRawData);
                    Console.WriteLine(mipBitmap.GetPixel(0, 0).ToString());
                    MipmapImages[i] = mipBitmap;
                    mipmapOffset += dataCodec.CalculateTextureSize(sizex, sizex);
                }
            }
        }

     
        private ReadOnlySpan<byte> DecodeInternalPalette(PvrDataCodec dataCodec, out int bytesRead)
        {
            Span<byte> result = null;
            int paletteEntries = dataCodec.GetPaletteEntries(Width);
            bytesRead = 0;

            if (paletteEntries > 0 && !dataCodec.NeedsExternalPalette)
            {
                PixelCodec pixelCodec = dataCodec.PixelCodec;

                int srcAddress = 0;
                result = new byte[paletteEntries * 4];

                for (int i = 0; i < paletteEntries; i += pixelCodec.Pixels)
                {
                    pixelCodec.DecodePixel(HeaderlessData[srcAddress..], result[(i * 4)..], false);
                    srcAddress += pixelCodec.BytesPerPixel;
                }

                bytesRead = srcAddress;
            }

            return result;
        }
    }
}