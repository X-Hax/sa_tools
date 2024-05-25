using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using nQuant;
using VrSharp.Pvr;

namespace VrSharp
{
    public abstract class VrTextureEncoder
    {
        #region Fields
        protected bool initalized = false; // Is the texture initalized?

        protected byte[] decodedData; // Decoded texture data (either 32-bit RGBA or 8-bit indexed)
        protected Bitmap decodedBitmap; // Decoded bitmap

        protected VrPixelCodec pixelCodec; // Pixel codec
        protected VrDataCodec dataCodec;   // Data codec
		protected VQCodeBook codeBook;
		protected QuantizedPalette vqPalette;

		protected byte[][] texturePalette; // The texture's palette
        #endregion

        #region Texture Properties
        /// <summary>
        /// Indicates whether or not this texture has a global index. If false, the texture will not include a GBIX header. The default value is true.
        /// </summary>
        public bool HasGlobalIndex
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return hasGlobalIndex;
            }
            set
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                hasGlobalIndex = value;
            }
        }
        protected bool hasGlobalIndex;

        /// <summary>
        /// Sets the texture's global index. This only matters if HasGlobalIndex is true. The default value is 0.
        /// </summary>
        public uint GlobalIndex
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return globalIndex;
            }
            set
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                globalIndex = value;
            }
        }
        protected uint globalIndex;

        /// <summary>
        /// Width of the texture (in pixels).
        /// </summary>
        public ushort TextureWidth
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return textureWidth;
            }
        }
        protected ushort textureWidth;

        /// <summary>
        /// Height of the texture (in pixels).
        /// </summary>
        public ushort TextureHeight
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return textureHeight;
            }
        }
        protected ushort textureHeight;
        #endregion

        #region Constructors & Initalizers
        public VrTextureEncoder(string file)
        {
            Initalize(new Bitmap(file));
        }

        public VrTextureEncoder(byte[] source) : this(source, 0, source.Length) { }

        public VrTextureEncoder(byte[] source, int offset, int length)
        {
            MemoryStream buffer = new MemoryStream();
            buffer.Write(source, offset, length);

            Initalize(new Bitmap(buffer));
        }

        public VrTextureEncoder(Stream source) : this(source, (int)(source.Length - source.Position)) { }

        public VrTextureEncoder(Stream source, int length)
        {
            MemoryStream buffer = new MemoryStream();
            PTStream.CopyPartTo(source, buffer, length);

            Initalize(new Bitmap(buffer));
        }

        public VrTextureEncoder(Bitmap source)
        {
            Initalize(source);
        }

        private void Initalize(Bitmap source)
        {
            // Make sure this bitmap's dimensions are valid
            if (!HasValidDimensions(source.Width, source.Height))
                return;

            try
            {
                decodedBitmap = source;

                textureWidth = (ushort)source.Width;
                textureHeight = (ushort)source.Height;
            }
            catch
            {
                decodedBitmap = null;

                textureWidth = 0;
                textureHeight = 0;
            }
        }

        /// <summary>
        /// Returns if the texture was loaded successfully.
        /// </summary>
        /// <returns></returns>
        public bool Initalized
        {
            get { return initalized; }
        }

        // Returns if the texture dimensuons are valid
        private bool HasValidDimensions(int width, int height)
        {
            if (width < 8 || height < 8 || width > 1024 || height > 1024)
                return false;

            if ((width & (width - 1)) != 0 || (height & (height - 1)) != 0)
                return false;

            return true;
        }
        #endregion

        #region Texture Retrieval
        /// <summary>
        /// Returns the encoded texture as a byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            if (!initalized)
            {
                throw new TextureNotInitalizedException("Cannot encode this texture as it is not initalized.");
            }

            return EncodeTexture().ToArray();
        }

        /// <summary>
        /// Returns the encoded texture as a stream.
        /// </summary>
        /// <returns></returns>
        public MemoryStream ToStream()
        {
            if (!initalized)
            {
                throw new TextureNotInitalizedException("Cannot encode this texture as it is not initalized.");
            }

            MemoryStream textureStream = EncodeTexture();
            textureStream.Position = 0;
            return textureStream;
        }

        /// <summary>
        /// Saves the encoded texture to the specified path.
        /// </summary>
        /// <param name="path">Name of the file to save the data to.</param>
        public void Save(string path)
        {
            if (!initalized)
            {
                throw new TextureNotInitalizedException("Cannot encode this texture as it is not initalized.");
            }

            using (FileStream destination = File.Create(path))
            {
                MemoryStream textureStream = EncodeTexture();
                textureStream.Position = 0;
                PTStream.CopyTo(textureStream, destination);
            }
        }

        /// <summary>
        /// Saves the encoded texture to the specified stream.
        /// </summary>
        /// <param name="destination">The stream to save the texture to.</param>
        public void Save(Stream destination)
        {
            if (!initalized)
            {
                throw new TextureNotInitalizedException("Cannot encode this texture as it is not initalized.");
            }

            MemoryStream textureStream = EncodeTexture();
            textureStream.Position = 0;
            PTStream.CopyTo(textureStream, destination);
        }

        // Encodes a texture
        protected abstract MemoryStream EncodeTexture();
        #endregion

        #region Mipmaps
        /// <summary>
        /// Returns if the texture has mipmaps.
        /// </summary>
        public virtual bool HasMipmaps
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return dataCodec.HasMipmaps;
            }
        }
        #endregion

        #region Palette
        /// <summary>
        /// Returns if the texture needs an external palette file.
        /// </summary>
        public virtual bool NeedsExternalPalette
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return dataCodec.NeedsExternalPalette;
            }
        }

        /// <summary>
        /// Returns the palette encoder if this texture uses an external palette file.
        /// </summary>
        public VpPaletteEncoder PaletteEncoder
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return paletteEncoder;
            }
        }
        protected VpPaletteEncoder paletteEncoder;
        #endregion

        #region Texture Conversion
        protected byte[] BitmapToRaw(Bitmap source)
        {
            Bitmap img = source;
            byte[] destination = new byte[img.Width * img.Height * 4];

            // If this is not a 32-bit ARGB bitmap, convert it to one
            if (img.PixelFormat != PixelFormat.Format32bppArgb)
            {
                Bitmap newImage = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(newImage))
                {
                    g.DrawImage(img, 0, 0, img.Width, img.Height);
                }
                img = newImage;
            }

            // Copy over the data to the destination. It's ok to do it without utilizing Stride
            // since each pixel takes up 4 bytes (aka Stride will always be equal to Width)
            BitmapData bitmapData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
            Marshal.Copy(bitmapData.Scan0, destination, 0, destination.Length);
            img.UnlockBits(bitmapData);

            return destination;
        }

        // Since this method is only used for mipmaps, and mipmaps are square, we can assume that width = height
        protected byte[] BitmapToRawResized(Bitmap source, int size, int minSize)
        {
            if (size > minSize)
                minSize = size;

            byte[] destination = new byte[minSize * minSize * 4];

            // Resize the image
            Bitmap img = new Bitmap(minSize, minSize, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(img))
            {
                using (ImageAttributes attr = new ImageAttributes())
                {
                    attr.SetWrapMode(WrapMode.TileFlipXY);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(source, new Rectangle(0, 0, size, size), 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attr);
                }
            }

            // Copy over the data to the destination. It's ok to do it without utilizing Stride
            // since each pixel takes up 4 bytes (aka Stride will always be equal to Width)
            BitmapData bitmapData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
            Marshal.Copy(bitmapData.Scan0, destination, 0, destination.Length);
            img.UnlockBits(bitmapData);

            return destination;
        }

		protected unsafe byte[] BitmapToRawIndexed(Bitmap source, int maxColors, out byte[][] palette)
		{
			Bitmap img = source;
			byte[] destination = new byte[img.Width * img.Height];

			// Quantize any non-palettized images, or any palettized-images with more palette entries than the encoded texture allows.
			if ((img.PixelFormat & PixelFormat.Indexed) == 0
				|| img.Palette.Entries.Length > maxColors)
			{
				// If this is not a 32-bit ARGB bitmap, convert it to one
				if (img.PixelFormat != PixelFormat.Format32bppArgb)
				{
					Bitmap newImage = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
					using (Graphics g = Graphics.FromImage(newImage))
					{
						g.DrawImage(img, 0, 0, img.Width, img.Height);
					}
					img = newImage;
				}

				// Quantize the image
				WuQuantizer quantizer = new WuQuantizer();
				img = (Bitmap)quantizer.QuantizeImage(img, maxColors);
			}

			int bitsPerPixel = Image.GetPixelFormatSize(img.PixelFormat);

			// Copy over the data to the destination. We need to use Stride in this case, as it may not
			// always be equal to Width.
			BitmapData bitmapData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);

			byte* pointer = (byte*)bitmapData.Scan0;

			if (bitsPerPixel == 1)
			{
				for (int y = 0; y < bitmapData.Height; y++)
				{
					for (int x = 0; x < bitmapData.Width; x++)
					{
						destination[(y * img.Width) + x] = (byte)((pointer[(y * bitmapData.Stride) + (x >> 3)] >> (7 - (x % 8))) & 0x1);
					}
				}
			}
			else if (bitsPerPixel == 4)
			{
				for (int y = 0; y < bitmapData.Height; y++)
				{
					for (int x = 0; x < bitmapData.Width; x++)
					{
						byte paletteIndex;
						if (x % 2 == 0)
						{
							paletteIndex = (byte)(pointer[(y * bitmapData.Stride) + (x >> 1)] >> 4);
						}
						else
						{
							paletteIndex = (byte)(pointer[(y * bitmapData.Stride) + (x >> 1)] & 0xF);
						}

						destination[(y * img.Width) + x] = paletteIndex;
					}
				}
			}
			else
			{
				for (int y = 0; y < bitmapData.Height; y++)
				{
					for (int x = 0; x < bitmapData.Width; x++)
					{
						destination[(y * img.Width) + x] = pointer[(y * bitmapData.Stride) + x];
					}
				}
			}

			img.UnlockBits(bitmapData);

			// Copy over the palette
			palette = new byte[maxColors][];
			for (int i = 0; i < maxColors && i < img.Palette.Entries.Length; i++)
			{
				palette[i] = new byte[4];

				palette[i][3] = img.Palette.Entries[i].A;
				palette[i][2] = img.Palette.Entries[i].R;
				palette[i][1] = img.Palette.Entries[i].G;
				palette[i][0] = img.Palette.Entries[i].B;
			}

			// If there are less palette entries in the source image than the destination, fill the remaining entries with opaque black.
			if (img.Palette.Entries.Length < maxColors)
			{
				for (int i = img.Palette.Entries.Length; i < maxColors; i++)
				{
					palette[i] = new byte[4];

					palette[i][3] = 0xFF;
					palette[i][2] = 0;
					palette[i][1] = 0;
					palette[i][0] = 0;
				}
			}

			return destination;
		}

		protected unsafe byte[] BitmapToRawIndexedResized(Bitmap source, int size, int minSize, QuantizedPalette palette)
		{
			if (size > minSize)
				minSize = size;

			byte[] destination = new byte[minSize * minSize * 4];

			// Resize the image
			Bitmap img = new Bitmap(minSize, minSize, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(img))
			{
				using (ImageAttributes attr = new ImageAttributes())
				{
					attr.SetWrapMode(WrapMode.TileFlipXY);
					g.InterpolationMode = InterpolationMode.HighQualityBicubic;
					g.DrawImage(source, new Rectangle(0, 0, size, size), 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attr);
				}
			}

			// Quantize the image
			WuQuantizer quantizer = new WuQuantizer();
			img = (Bitmap)quantizer.QuantizeImage(img, palette);

			// Copy over the data to the destination. We need to use Stride in this case, as it may not
			// always be equal to Width.
			BitmapData bitmapData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);

			byte* pointer = (byte*)bitmapData.Scan0;
			for (int y = 0; y < bitmapData.Height; y++)
			{
				for (int x = 0; x < bitmapData.Width; x++)
				{
					destination[(y * img.Width) + x] = pointer[(y * bitmapData.Stride) + x];
				}
			}

			img.UnlockBits(bitmapData);
			return destination;
		}

		#endregion

		#region VQ
		protected byte[] BitmapToRawVQ(Bitmap source, int codeBookSize, out byte[][] palette)
		{
			Bitmap img = source;
			byte[] destination = new byte[img.Width * img.Height];

			// If this is not a 32-bit ARGB bitmap, convert it to one
			if (img.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
			{
				Bitmap newImage = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newImage))
				{
					g.DrawImage(img, 0, 0, img.Width, img.Height);
				}
				img = newImage;
			}

			//Create code book
			codeBook = VectorQuantizer.CreateCodebook(img, codeBookSize / 4);

			//Write palette
			palette = new byte[codeBookSize][];
			for (int i = 0; i < codeBook.Entries.Length; i++)
			{
				byte[] pixels = codeBook.Entries[i].ToArray();
				for (int j = 0; j < 4; j++)
				{
					int paletteIndex = i * 4 + j;
					palette[paletteIndex] = new byte[4];
					palette[paletteIndex][0] = pixels[j * 4 + 3];
					palette[paletteIndex][1] = pixels[j * 4 + 2];
					palette[paletteIndex][2] = pixels[j * 4 + 1];
					palette[paletteIndex][3] = pixels[j * 4];
				}
			}

			//Quantize image
			return VectorQuantizer.QuantizeImage(img, codeBook);
		}

		protected byte[] BitmapToRawVQResized(Bitmap source, int size, int minSize, VQCodeBook codeBook)
		{
			if (size > minSize)
				minSize = size;

			// Resize the image
			Bitmap img = new Bitmap(minSize, minSize, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(img))
			{
				using (ImageAttributes attr = new ImageAttributes())
				{
					attr.SetWrapMode(WrapMode.TileFlipXY);
					g.InterpolationMode = InterpolationMode.HighQualityBicubic;
					g.DrawImage(source, new Rectangle(0, 0, size, size), 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attr);
				}
			}
			return VectorQuantizer.QuantizeImage(img, codeBook);
		}
		#endregion
	}
}