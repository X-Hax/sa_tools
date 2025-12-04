using System;
using System.Drawing;

// Texture library for SA Tools' 3D editors and Texture Editor
// Mostly based on https://github.com/X-Hax/SA3D.Archival and https://github.com/nickworonekin/puyotools

// TODO: Smart GVR-PVR-XVR-DDS conversion
// TODO: Rework GVR codecs into data codec + standard pixel codec pairings?
// TODO: Class for textures that failed to initialize

namespace TextureLib
{
    public abstract class GenericTexture
    {
		// Common properties and data
		/// <summary>Texture name.</summary>
		public string Name;
		/// <summary>Texture width.</summary>
		public int Width;
		/// <summary>Texture height.</summary>
		public int Height;
		/// <summary>Texture Global Index.</summary>
		public uint Gbix;
		/// <summary>Texture bytes in the original format including the header.</summary>
		public byte[] RawData;

		// Flags
		/// <summary>Whether the texture has mipmaps or not.</summary>
		public bool HasMipmaps;
		/// <summary>Whether the texture is palettized or not.</summary>
		public bool Indexed;
		/// <summary>Whether the Indexed texture requires an external palette file or not.</summary>
		public bool RequiresPaletteFile;
		/// <summary>The texture's color palette (for Indexed textures).</summary>
		public TexturePalette Palette;
		/// <summary> Number of palette bank in the PVP file (for Indexed textures).</summary>
		public int PaletteBank;
		/// <summary>Starting color in the palette (for Indexed textures).</summary>
		public int PaletteStartIndex;
		/// <summary>Original texture dimensions for the PVMX archive.</summary>
		public Size PvmxOriginalDimensions;
		/// <summary>Additional fields for the PAK archive.</summary>
		public PakMetadata PakMetadata;
		/// <summary>Texture converted to Bitmap.</summary>
		public Bitmap Image;
		/// <summary>Texture mipmaps converted to Bitmaps. The first item (0) is the full size image.</summary>
		public Bitmap[] MipmapImages;

		/// <summary>
		/// This function does the basic initialization for all texture types.
		/// </summary>
		/// <param name="data">Byte array containing texture data, including the header.</param>
		/// <param name="offset">Offset where the texture's header starts.</param>
		/// <param name="name">Texture name (optional).</param>
		/// <returns>Returns true if texture data was read successfully. This only concerns texture raw data, not decoding or conversion.</returns>
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

		/// <summary>
		/// Retrieves the texture's byte array.
		/// </summary>
		/// <returns>Byte array containing texture header and data.</returns>
		public abstract byte[] GetBytes();

		/// <summary>
		/// Sets the selected palette and decodes the texture with it. This will update the texture's Image and Mipmaps.
		/// </summary>
		/// <param name="newPalette">Palette to use in decoding the texture.</param>
		public void SetPalette(TexturePalette newPalette)
		{
			Palette = newPalette;
			Decode();
		}

		/// <summary>
		/// Updates the texture's Image and Mipmaps by decoding its raw data.
		/// This method is called automatically when a texture is created from raw data. Normally there is no need to call it manually.
		/// </summary>
		public abstract void Decode();

		/// <summary>
		/// Updates the texture's raw data by encoding its Image and Mipmaps with the current texture properties.
		/// This method is called automatically when a texture is created from a Bitmap. Normally there is no need to call it manually.
		/// </summary>
		public abstract void Encode();

		/// <summary>
		/// Applies the currently selected palette to the Indexed texture's decoded data (raw Index8 bytes).
		/// This method is used internally at final steps of texture decoding. For a generic method, use SetPalette.
		/// </summary>
		/// <param name="src">Byte array of decoded texture data.</param>
		/// <param name="width">Texture width.</param>
		/// <param name="height">Texture height.</param>
		/// <returns>Byte array containing the palettized texture's raw decoded data (ARGB8888).</returns>
		internal Span<byte> ApplyPalette(byte[] src, int width, int height)
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
}