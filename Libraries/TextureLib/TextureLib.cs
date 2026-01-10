using System;
using System.Drawing;

// Texture library for SA Tools' 3D editors and Texture Editor
// Mostly based on https://github.com/X-Hax/SA3D.Archival and https://github.com/nickworonekin/puyotools

// TODO: Rework GVR codecs into data codec + standard pixel codec pairings?

namespace TextureLib
{
	/// <summary>Texture file types used in texture identification.</summary>
	public enum TextureFileFormat {	Pvr, Gvr, Dds, Xvr, Png, Jpg, Gif, Bmp, Unknown, Invalid }

	/// <summary>Indexed texture types used in texture identification.</summary>
	public enum IndexedTextureFormat { Index4, Index8, Index14, NotIndexed }

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
			// Init metadata
			PakMetadata = new PakMetadata();
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
		/// <param name="bankID">Palette bank ID for multi-bank palettes (optional).</param>
		/// <param name="startColor">Adjust the bank's start color index (optional).</param>
		public void SetPalette(TexturePalette newPalette, int bankID = 0, int startColor = 0)
		{
			// Assign the new palette.
			Palette = newPalette;
			// Decode() overwrites the texture's StartBank and StartColor, so the ones in the palette should be used.
			Palette.StartBank = bankID;
			Palette.StartColor = startColor;
			// Decode the texture with the new palette.
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
		/// Imports a new Bitmap and encodes the texture.
		/// </summary>
		/// <param name="bmp">Image to use.</param>
		public void ImportBitmap(Bitmap bmp)
		{
			Image = new Bitmap(bmp);
			Width = Image.Width;
			Height = Image.Height;
			Encode();
		}

		/// <summary>
		/// Applies the currently selected palette (or a default palette) to the Indexed texture's decoded data (raw Index8 bytes).
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
                int decodedID = src[colorID];
                if (!index8)
                {
                    decodedID = decodedID >> 4; // This is because the Index4 codec expects 8 bit
                }
				// Add bank and color offset if specified
				decodedID += Palette.StartBank * (index8 ? 256 : 16) + Palette.StartColor;
				// Get color
                result[colorID * 4 + 0] = Palette.DecodedData[decodedID * 4 + 0];
                result[colorID * 4 + 1] = Palette.DecodedData[decodedID * 4 + 1];
                result[colorID * 4 + 2] = Palette.DecodedData[decodedID * 4 + 2];
                result[colorID * 4 + 3] = Palette.DecodedData[decodedID * 4 + 3];
            }
            return result;
        }

		/// <summary>
		/// Adds mipmaps to texture data, without reencoding the original texture whenever possible. 
		/// Also changes the PVR format to the mipmapped one if necessary.
		/// </summary>
		public abstract void AddMipmaps();

		/// <summary>
		/// Removes mipmaps from texture data, without reencoding the original texture whenever possible. 
		/// Also changes the PVR format to the non-mipmapped one if necessary.
		/// </summary>
		public abstract void RemoveMipmaps();

		/// <summary>
		/// Checks whether the texture's current format allows mipmaps. 
		/// The actual texture may or may not have them. 
		/// To check whether mipmaps are present or not, use the HasMipmaps property.
		/// </summary>
		/// <returns>True if the texture format supports mipmaps.</returns>
		public abstract bool CanHaveMipmaps();

		/// <summary>
		/// Prints out the texture's information, such as flags, dimensions, pixel and data formats etc.
		/// </summary>
		/// <returns>String with various information.</returns>
		public abstract string Info();

		/// <summary>
		/// Checks data at the specified offset and returns the texture file format.
		/// </summary>
		/// <param name="data">Byte array to check.</param>
		/// <param name="offset">Offset where texture header starts.</param>
		/// <returns>TextureFileFormat value, such as Pvr, Gvr etc.</returns>
		public static TextureFileFormat GetTextureFileType(byte[] data, int offset = 0)
		{
			const ushort MagicBMP = 0x4D42;
			const uint MagicJPG = 0xE0FFD8FF;
			const uint MagicGIF = 0x38464947;
			const uint MagicPNG = 0x474E5089;

			// Invalid
			if (data == null || data.Length < 4)
				return TextureFileFormat.Invalid;

			// Textures with GBIX
			if (PvrTexture.Identify(data, offset))
				return TextureFileFormat.Pvr;
			if (GvrTexture.Identify(data, offset))
				return TextureFileFormat.Gvr;
			if (XvrTexture.Identify(data, offset))
				return TextureFileFormat.Xvr;

			// DDS
			if (DdsTexture.Identify(data, offset))
				return TextureFileFormat.Dds;

			// BMP
			if (BitConverter.ToUInt16(data, offset) == MagicBMP)
				return TextureFileFormat.Bmp;

			// GIF, JPG, PNG or unsupported
			return BitConverter.ToUInt32(data, offset) switch
			{
				MagicJPG => TextureFileFormat.Jpg,
				MagicGIF => TextureFileFormat.Gif,
				MagicPNG => TextureFileFormat.Png,
				_ => TextureFileFormat.Unknown,
			};
		}

		/// <summary>
		/// Returns the extension string by identifying texture file format in a byte array.
		/// </summary>
		public static string IdentifyTextureFileExtension(byte[] file)
		{
			return GetTextureFileType(file) switch
			{
				TextureFileFormat.Bmp => ".bmp",
				TextureFileFormat.Gif => ".gif",
				TextureFileFormat.Jpg => ".jpg",
				TextureFileFormat.Png => ".png",
				TextureFileFormat.Dds => ".dds",
				TextureFileFormat.Pvr => ".pvr",
				TextureFileFormat.Gvr => ".gvr",
				TextureFileFormat.Xvr => ".xvr",
				TextureFileFormat.Invalid => throw new Exception("Invalid texture data"),
				_ => throw new Exception("Unknown texture file format"),
			};
		}

		/// <summary>
		/// Returns the texture's indexed data format.
		/// </summary>
		/// <returns>IndexedTextureFormat value (Index4, Index8 or Non-Indexed).</returns>
		public IndexedTextureFormat GetIndexedFormat()
		{
			return this switch
			{
				PvrTexture pvr => pvr.PvrDataFormat switch
				{
					PvrDataFormat.Index4 or PvrDataFormat.Index4Mipmaps => IndexedTextureFormat.Index4,
					PvrDataFormat.Index8 or PvrDataFormat.Index8Mipmaps => IndexedTextureFormat.Index8,
					_ => IndexedTextureFormat.NotIndexed,
				},
				GvrTexture gvr => gvr.GvrDataFormat switch
				{
					GvrDataFormat.Index4 => IndexedTextureFormat.Index4,
					GvrDataFormat.Index8 => IndexedTextureFormat.Index8,
					GvrDataFormat.Index14 => IndexedTextureFormat.Index14,
					_ => IndexedTextureFormat.NotIndexed,
				},
				GdiTexture gdi => gdi.GdiPixelFormat switch
				{
					System.Drawing.Imaging.PixelFormat.Format4bppIndexed => IndexedTextureFormat.Index4,
					System.Drawing.Imaging.PixelFormat.Format8bppIndexed => IndexedTextureFormat.Index8,
					_ => IndexedTextureFormat.NotIndexed,
				},
				_ => IndexedTextureFormat.NotIndexed,
			};
		}

		/// <summary>
		/// Loads texture data at the specified offset and returns a PVR, GVR, XVR, DDS, GDI or Invalid texture.
		/// </summary>
		/// <param name="data">Byte array to load.</param>
		/// <param name="offset">Offset to load.</param>
		/// <returns></returns>
		public static GenericTexture LoadTexture(byte[] data, int offset = 0)
		{
			if (PvrTexture.Identify(data, offset))
				return new PvrTexture(data, offset);
			else if (GvrTexture.Identify(data, offset))
				return new GvrTexture(data, offset);
			else if (XvrTexture.Identify(data, offset))
				return new XvrTexture(data, offset);
			else if (DdsTexture.Identify(data, offset))
				return new DdsTexture(data, offset);
			else if (GdiTexture.Identify(data, offset))
				return new GdiTexture(data, offset);
			return new InvalidTexture(data, offset);
		}

		/// <summary>
		/// Loads texture data from the specified file and returns a PVR, GVR, XVR, DDS or Invalid texture.
		/// </summary>
		/// <param name="file">Path to the input file.</param>
		/// <returns></returns>
		public static GenericTexture LoadTexture(string file)
		{
			return LoadTexture(System.IO.File.ReadAllBytes(file));
		}

		/// <summary>
		/// Converts any texture to a PVR texture.
		/// </summary>
		/// <param name="maxQuality">Prefer higher quality pixel formats to avoid data loss.</param>
		/// <param name="forceMipmaps">Force add mipmaps if they are not in the original texture.</param>
		/// <param name="useCompressed">Use VQ compression formats.</param>
		/// <returns>A PVR texture.</returns>
		/// <exception cref="Exception"></exception>
		public PvrTexture ToPvr(bool maxQuality = false, bool useCompressed = false, bool forceMipmaps = false)
		{
			return this switch
			{
				PvrTexture => (PvrTexture)this,
				GvrTexture gvr => new PvrTexture(gvr, forceMipmaps, true, maxQuality),
				DdsTexture dds => new PvrTexture(dds, forceMipmaps),
				GdiTexture gdi => new PvrTexture(gdi, forceMipmaps),
				_ => throw new Exception("Cannot convert texture to PVR"),
			};
		}

		/// <summary>
		/// Converts any texture to a GDI (PNG) texture.
		/// </summary>
		/// <returns>A GDI Texture.</returns>
		/// <exception cref="Exception"></exception>
		public GdiTexture ToGdi()
		{
			return this switch
			{
				GdiTexture gdi => (GdiTexture)this,
				PvrTexture pvr => new GdiTexture(pvr.Image, pvr.HasMipmaps, pvr.Gbix, pvr.Name),
				GvrTexture gvr => new GdiTexture(gvr.Image, gvr.HasMipmaps, gvr.Gbix, gvr.Name),
				XvrTexture xvr => new GdiTexture(xvr.Image, xvr.HasMipmaps, xvr.Gbix, xvr.Name),
				DdsTexture dds => new GdiTexture(dds.Image, dds.HasMipmaps, dds.Gbix, dds.Name),
				_ => throw new Exception("Cannot convert texture to GDI"),
			};
		}

		/// <summary>
		/// Converts any texture to a GVR texture.
		/// </summary>
		/// <param name="forceMipmaps">Force add mipmaps if the input texture doesn't have them.</param>
		/// <param name="useCompressed">Allow usage of the DXT1 compression format.</param>
		/// <param name="maxQuality">Use higher quality formats to avoid data loss.</param>
		/// <returns>A GVR texture.</returns>
		/// <exception cref="Exception"></exception>
		public GvrTexture ToGvr(bool maxQuality = false, bool useCompressed = false, bool forceMipmaps = false)
		{
			return this switch
			{
				GvrTexture => (GvrTexture)this,
				PvrTexture pvr => new GvrTexture(pvr, forceMipmaps, useCompressed, maxQuality),
				DdsTexture gvr => new GvrTexture(gvr, forceMipmaps, maxQuality),
				GdiTexture gdi => new GvrTexture(gdi),
				_ => throw new Exception("Cannot convert texture to GVR"),
			};
		}

		/// <summary>
		/// Converts any texture to a DDS texture.
		/// </summary>
		/// <param name="forceMipmaps">Force add mipmaps if the original texture doesn't have them.</param>
		/// <param name="useCompressed">Allow use of DXT formats.</param>
		/// <param name="maxQuality">Prefer maximum quality.</param>
		/// <returns>A DDS texture.</returns>
		/// <exception cref="Exception"></exception>
		public DdsTexture ToDds(bool maxQuality = false, bool useCompressed = false, bool forceMipmaps = false)
		{
			return this switch
			{
				XvrTexture or DdsTexture => (DdsTexture)this,
				PvrTexture pvr => new DdsTexture(pvr, forceMipmaps, maxQuality),
				GvrTexture gvr => new DdsTexture(gvr, forceMipmaps, maxQuality),
				GdiTexture gdi => new DdsTexture(gdi, forceMipmaps, maxQuality, useCompressed),
				_ => throw new Exception("Cannot convert texture to DDS"),
			};
		}

		/// <summary>
		/// Converts any texture to a XVR texture.
		/// </summary>
		/// <param name="forceMipmaps">Force add mipmaps if the original texture doesn't have them.</param>
		/// <param name="useCompressed">Allow use of DXT formats.</param>
		/// <param name="maxQuality">Prefer maximum quality.</param>
		/// <returns>An XVR texture.</returns>
		/// <exception cref="Exception"></exception>
		public XvrTexture ToXvr(bool maxQuality = false, bool useCompressed = false, bool forceMipmaps = false)
		{
			return this switch
			{
				XvrTexture or DdsTexture => (XvrTexture)this,
				PvrTexture pvr => (XvrTexture)(new DdsTexture(pvr, forceMipmaps, maxQuality)),
				GvrTexture gvr => (XvrTexture)(new DdsTexture(gvr, forceMipmaps, maxQuality)),
				GdiTexture gdi => (XvrTexture)(new DdsTexture(gdi, forceMipmaps, maxQuality, useCompressed)),
				_ => throw new Exception("Cannot convert texture to XVR"),
			};
		}
	}
}