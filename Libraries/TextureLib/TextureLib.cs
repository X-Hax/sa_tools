using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

// Early WIP texture library for SA Tools' 3D editors and Texture Editor
// Mostly based on https://github.com/X-Hax/SA3D.Archival and https://github.com/nickworonekin/puyotools

// TODO: Rework GVR codecs into data codec + standard pixel codec pairings
// TODO: Class for Xbox XVR textures
// TODO: Class for DDS textures
// TODO: Smart GVR-PVR-XVR-DDS conversion

namespace TextureLib
{
    public abstract class GenericTexture
    {
        public string Name; // Texture name or file name without file extension
        public int Width; // Texture width
        public int Height; // Texture height
        public uint Gbix; // Texture Global Index
        public byte[] RawData; // Texture bytes in the original format including header

        public bool HasMipmaps; // Whether the texture has mipmaps or not
        public bool Indexed; // Whether the texture is palettized or not
        public bool RequiresPaletteFile; // Whether the texture requires an external palette file or not

        public TexturePalette Palette; // The texture's color palette (for Indexed)
        public int PaletteBank; // Number of palette bank in the PVP file (for Indexed)
        public int PaletteStartIndex; // Starting color in the palette (for Indexed)

        public Size PvmxOriginalDimensions; // Original texture dimensions for the PVMX archive
        public PakMetadata PakMetadata; // Additional fields for the PAK archive

        public Bitmap Image; // Texture converted to Bitmap
        public Bitmap[] MipmapImages; // Mipmaps converted to Bitmaps and mipmap dimensions

        // This function does the basic initialization for all texture types
        public bool InitTexture(byte[] data, int offset = 0, string name = null)
        {
            // Set texture name
            if (!string.IsNullOrEmpty(name))
                Name = name;
            // Copy raw data
            if (data != null)
            {
                RawData = new byte[data.Length - offset];
                Array.Copy(data, offset, RawData, 0, data.Length - offset);
                return true;
            }
            return false;
        }

        // This function creates mipmaps from the original image
        public void GenerateMipmaps()
        {
            // Calculate mipmap levels based on texture dimensions
            int levels = (int)Math.Floor(Math.Log2(Math.Max(Width, Height))) + 1;
            MipmapImages = new Bitmap[levels];
            // Set initial width before division
            int mipWidth = Width;
            int mipHeight = Height;
            // Generate individual mipmaps
            for (int m = 0; m < levels; m++)
            {
                // Divide original or previous dimensions by two for each mipmap
                mipWidth = mipWidth / 2;
                mipHeight = mipHeight / 2;
                Bitmap mip = new Bitmap(mipWidth, mipHeight);
                // Write mipmap image onto the bitmap from largest to smallest
                using (Graphics gfx = Graphics.FromImage(mip))
                {
                    gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    gfx.DrawImage(Image, 0, 0, mipWidth, mipHeight);
                }
                // Save bitmap to the dictionary
                MipmapImages[m] = mip;
            }
        }

        public Span<byte> ApplyPalette(byte[] src, int width, int height)
        {
            bool index8 = false;
            if (this is PvrTexture pvr)
                index8 = (pvr.PvrDataFormat == PvrDataFormat.Index8 || pvr.PvrDataFormat == PvrDataFormat.Index8Mipmaps);
            else if (this is GvrTexture gvr)
                index8 = (gvr.GvrDataFormat == GvrDataFormat.Index8);
            byte[] result = new byte[width * height * 4];
            if (Palette == null)
                Palette = TexturePalette.CreateDefaultPalette(index8);
            for (int colorID = 0; colorID < src.Length; colorID++)
            {
                byte decodedID = src[colorID];
                if (!index8)
                {
                    decodedID = (byte)(decodedID >> 4); // This is because the Index4 codec expects 8 bit
                }
                result[colorID * 4 + 0] = Palette.DecodedData[decodedID * 4 + 0];
                result[colorID * 4 + 1] = Palette.DecodedData[decodedID * 4 + 1];
                result[colorID * 4 + 2] = Palette.DecodedData[decodedID * 4 + 2];
                result[colorID * 4 + 3] = Palette.DecodedData[decodedID * 4 + 3];
            }
            return result;
        }

    }

    // Class for textures that failed to initialize
    //public class InvalidTexture : GenericTexture { }

}