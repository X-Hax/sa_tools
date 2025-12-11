using CommunityToolkit.HighPerformance;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TextureLib
{
	public partial class GvrTexture
	{
		/// <summary>Dictionary of GVR and PVR formats that can be converted losslessly between each other. ARGB8888 and Indexed formats not included.</summary>
		private static Dictionary<GvrDataFormat, PvrPixelFormat> CompatibleFormatsGvrPvr = new Dictionary<GvrDataFormat, PvrPixelFormat>()
		{
			{ GvrDataFormat.Rgb565, PvrPixelFormat.Rgb565 }
			// IntensityA8 could be converted to Bump88 I guess?
		};

		/// <summary>Create a new GVR texture from a PVR texture, data format determined automatically.</summary>
		/// <param name="pvr">Source PVR texture.</param>
		/// <param name="forceMipmaps">Add mipmaps if the source texture doesn't have them.</param>		
		/// <param name="maxQuality">Use the ARGB8888 pixel format when a lossless conversion is impossible.</param>
		public GvrTexture(PvrTexture pvr, bool forceMipmaps = false, bool useDxt = true, bool maxQuality = false)
		{
			GvrDataFormat targetGvrDataFormat = AutoGvrDataFormatFromPvr(pvr.PvrPixelFormat, maxQuality);
			ConvertFromPvr(pvr, targetGvrDataFormat, forceMipmaps);
		}

		/// <summary>Create a new GVR texture from a PVR texture, data format specified manually.</summary>
		/// <param name="targetGvrDataFormat">Target GVR data format.</param>
		/// <param name="forceMipmaps">Add mipmaps if the source texture doesn't have them.</param>
		public GvrTexture(PvrTexture pvr, GvrDataFormat targetGvrDataFormat, bool forceMipmaps = false)
		{
			ConvertFromPvr(pvr, targetGvrDataFormat, forceMipmaps);
		}

		private void ConvertFromPvr(PvrTexture pvr, GvrDataFormat targetGvrDataFormat, bool forceMipmaps = false)
		{
			Console.WriteLine(targetGvrDataFormat.ToString());
			// Set common texture properties
			Image = pvr.Image;
			Gbix = pvr.Gbix;
			Name = pvr.Name;
			Width = pvr.Width;
			Height = pvr.Height;
			PaletteBank = pvr.PaletteBank;
			PaletteStartIndex = pvr.PaletteStartIndex;
			PakMetadata = pvr.PakMetadata;
			GvrDataFormat = targetGvrDataFormat;
			PvmxOriginalDimensions = pvr.PvmxOriginalDimensions;
			bool lossless = false;
			foreach (var item in CompatibleFormatsGvrPvr)
			{
				if (item.Key == GvrDataFormat && item.Value == pvr.PvrPixelFormat)
					lossless = true;
			}
			// Cannot convert losslessly from VQ and SmalLVQ
			switch (pvr.PvrDataFormat)
			{
				case PvrDataFormat.Vq:
				case PvrDataFormat.VqMipmaps:
				case PvrDataFormat.SmallVq:
				case PvrDataFormat.SmallVqMipmaps:
					lossless = false;
					break;
			}
			if (!lossless)
			{
				HasMipmaps = forceMipmaps ? true : pvr.HasMipmaps;
				Encode();
			}
			// Lossless conversion
			else
			{
#if DEBUG
				Console.WriteLine("Using lossless conversion");
#endif
				MemoryStream outputStream = new MemoryStream();
				PvrDataCodec inputCodec = PvrDataCodec.Create(pvr.PvrDataFormat, new Bypass16BitPixelCodec());
				GvrDataCodec outputCodec = new GvrBypass16BitDataCodec();
				// Original texture data
				int textureAddress = pvr.HeaderlessData.Length - inputCodec.CalculateTextureSize(Width, Height);
				ReadOnlySpan<byte> textureData = pvr.HeaderlessData[textureAddress..];
				byte[] mainTexRaw = inputCodec.Decode(textureData, Width, Height, null);
				// Write texture
				outputStream.Write(outputCodec.Encode(mainTexRaw, Width, Height));
				// If there are mipmaps, write them along with the main texture
				if (pvr.HasMipmaps)
				{
					// Calculate the number of mip levels
					int mipLevels = (int)Math.Floor(Math.Log2(Math.Max(Width, Height))) + 1;
					int[] mipmapOffsets = new int[mipLevels];
					List<byte[]> mips = new();
					// Start offset for the first mipmap
					int mipmapOffset = 0;
					for (int i = mipLevels - 1, sizex = 1; i >= 0; i--, sizex <<= 1)
					{
						mipmapOffsets[i] = sizex == 1 ? 2 : mipmapOffset;
						byte[] srcdata = pvr.HeaderlessData[mipmapOffsets[i]..];
						byte[] mipRawData = inputCodec.Decode(srcdata, sizex, sizex, null);
						if (sizex != Width)
							mips.Add(outputCodec.Encode(mipRawData, sizex, sizex));
						mipmapOffset += inputCodec.CalculateTextureSize(sizex, sizex);
					}
					for (int i = mips.Count - 1; i >= 0; i--)
					{
						outputStream.Write(mips[i]);
						Console.WriteLine(i.ToString());
					}
					HasMipmaps = true;
					GvrDataFlags |= GvrDataFlags.Mipmaps;
				}
				// Update raw data arrays
				HeaderlessData = outputStream.ToArray();
				RawData = GetBytes();
				// Add mipmaps if necessary
				if (!pvr.HasMipmaps && forceMipmaps)
				{
					HasMipmaps = false;
					AddMipmaps();
				}
			}
		}
	}
}