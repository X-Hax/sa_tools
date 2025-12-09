using System;
using System.IO;
using TextureLib;

namespace TextureTool
{
	partial class Program
	{
		static void Conversion()
		{
			Console.WriteLine("GENERAL INFO");
			Console.WriteLine("Input file: {0}", inputFilename);
			Console.WriteLine("Output file: {0}", outputFullFilename);
			Console.WriteLine("Source format: {0}", sourceFileFormat.ToString().ToUpperInvariant());
			Console.WriteLine("Target format: {0}", targetFileFormat.ToString().ToUpperInvariant());
			Console.WriteLine("Force mipmaps: {0}", useMipmaps.ToString());
			// Show dithering flag if applicable
			if (targetGvrFormat == GvrDataFormat.Index4 || targetGvrFormat == GvrDataFormat.Index8 || targetGvrFormat == GvrDataFormat.Index14 ||
				targetPvrFormat == PvrDataFormat.Index4 || targetPvrFormat == PvrDataFormat.Index8 ||
				targetPvrFormat == PvrDataFormat.Index4Mipmaps || targetPvrFormat == PvrDataFormat.Index8Mipmaps)
				Console.WriteLine("Use dithering: {0}", useDitheringForIndexed.ToString());
			// Read palette file if specified
			inputPalette = string.IsNullOrEmpty(paletteFilename) ? null :
				new TexturePalette(File.ReadAllBytes(paletteFilename), false);
			if (inputPalette != null)
				Console.WriteLine("Using palette file: {0}", paletteFilename);
			// Read input texture data
			byte[] inputFile = File.ReadAllBytes(inputFilename);
			// Get input texture format from file extension. This is temporary, should have methods to auto-detect input texture format
			switch (sourceFileFormat)
			{
				case TextureFileFormat.Pvr:
					inputTexture = new PvrTexture(inputFile, extPalette: inputPalette);
					break;
				case TextureFileFormat.Gvr:
					inputTexture = new GvrTexture(inputFile, extPalette: inputPalette);
					break;
				case TextureFileFormat.Dds:
					inputTexture = new DdsTexture(inputFile, 0, gbix);
					break;
				case TextureFileFormat.Xvr:
					inputTexture = new XvrTexture(inputFile);
					break;
				case TextureFileFormat.Png:
					inputTexture = new GdiTexture(inputFile, 0, useMipmaps, gbix);
					break;
				default:
					Console.WriteLine("{0}: input texture format not implemented", inputFile);
					return;
			}
			// Encode
			// Output encoder parameters
			Console.WriteLine("\nENCODER PARAMETERS");
			switch (targetFileFormat)
			{
				case TextureFileFormat.Png:
					break;
				case TextureFileFormat.Dds:
					Console.WriteLine("GBIX: {0}", gbix == 0 ? "Auto" : gbix.ToString());
					Console.WriteLine("DDS format: {0}", autoDdsDataFormat ? "Auto" : targetDdsFormat.ToString());
					break;
				case TextureFileFormat.Xvr:
					Console.WriteLine("GBIX: {0}", gbix == 0 ? "Auto" : gbix.ToString());
					Console.WriteLine("XVR format: {0} ({1})", autoXvrFormat ? "Auto" : targetXvrFormat.ToString(), XvrFormats.GetDxgiFormatFromXvrPixelFormat((XvrFormat)targetXvrFormat).ToString());
					break;
				case TextureFileFormat.Pvr:
					Console.WriteLine("GBIX: {0}", gbix == 0 ? "Auto" : gbix.ToString());
					Console.WriteLine("PVR data format: {0}", autoPvrDataFormat ? "Auto" : targetPvrFormat.ToString());
					Console.WriteLine("PVR pixel format: {0}", autoPvrPixelFormat ? "Auto" : targetPvrPixelFormat.ToString());
					break;
				case TextureFileFormat.Gvr:
					Console.WriteLine("GBIX: {0}", gbix == 0 ? "Auto" : gbix.ToString());
					Console.WriteLine("GVR format: {0}", autoGvrDataFormat ? "Auto" : targetGvrFormat.ToString());
					switch (targetGvrFormat)
					{
						case GvrDataFormat.Index4:
						case GvrDataFormat.Index8:
						case GvrDataFormat.Index14:
							Console.WriteLine("GVR palette mode: {0}", encodeExternalPalette ? "External" : "Internal");
							string gvrpalfmt = targetGvrPaletteFormat.ToString();
							switch (targetGvrPaletteFormat)
							{
								case GvrPaletteFormat.IntensityA8orArgb1555:
									gvrpalfmt = saCompatibleGvrPalettes ? "ARGB1555 (SA)" : "IntensityA8 (Regular)";
									break;
								case GvrPaletteFormat.Rgb5A3orArgb4444:
									gvrpalfmt = saCompatibleGvrPalettes ? "ARGB4444 (SA)" : "RGB5A3 (Regular)";
									break;
								case GvrPaletteFormat.Rgb565:
									gvrpalfmt = "RGB565";
									break;
								case GvrPaletteFormat.Argb8888:
									gvrpalfmt = "ARGB8888";
									break;
							}
							Console.WriteLine("GVR palette format: {0}", gvrpalfmt);
							break;
					}
					break;
			}
			// Read the input texture
			TexturePalette outputTexturePalette = null;
			GenericTexture result = null;
			switch (targetFileFormat)
			{
				// Converting to PNG
				case TextureFileFormat.Png:
					result = new GdiTexture(inputTexture.Image, inputTexture.HasMipmaps, gbix);
					break;
				// Converting to DDS
				case TextureFileFormat.Dds:
					// Convert from GVR
					if (inputTexture is GvrTexture gvrr)
						result = new DdsTexture(gvrr);
					else
					{
						// Set formats for auto mode
						if (autoDdsDataFormat)
						{
							Console.WriteLine("Auto DDS formats not implemented yet");
							return;
						}
						// Encode texture
						result = new DdsTexture(inputTexture.Image, targetDdsFormat, useMipmaps);
					}
					break;
				// Converting to XVR
				case TextureFileFormat.Xvr:
					// Set formats for auto mode
					if (autoXvrFormat)
					{
						Console.WriteLine("Auto XVR formats not implemented yet");
						return;
					}
					// Encode texture
					result = new XvrTexture(inputTexture.Image, (XvrFormat)targetXvrFormat, useMipmaps, gbix);
					break;
				// Converting to PVR
				case TextureFileFormat.Pvr:
					switch (inputTexture)
					{
						// Convert from PVR
						case PvrTexture pvrt:
							// Auto pixel & data
							if (autoPvrDataFormat && autoPvrPixelFormat)
								result = new PvrTexture(pvrt, useMipmaps);
							// Auto data
							else if (autoPvrDataFormat)
								result = new PvrTexture(pvrt, targetPvrPixelFormat, useMipmaps);
							// Auto pixel
							else if (autoPvrPixelFormat)
								result = new PvrTexture(pvrt, targetPvrFormat);
							break;
						// Convert from GVR
						case GvrTexture gvrt:
							result = autoPvrDataFormat ? new PvrTexture(gvrt, useMipmaps) : new PvrTexture(gvrt, targetPvrFormat);
							break;
						// Convert from DDS or XVR
						case DdsTexture ddst:
							result = autoPvrDataFormat ? new PvrTexture(ddst, useMipmaps) : new PvrTexture(ddst, targetPvrFormat);
							break;
						// Convert from PNG
						case GdiTexture gdix:
							// Auto pixel & data
							if (autoPvrDataFormat && autoPvrPixelFormat)
								result = new PvrTexture(gdix, useMipmaps);
							// Auto data
							else if (autoPvrDataFormat)
								result = new PvrTexture(gdix, targetPvrPixelFormat, useMipmaps);
							// Auto pixel
							else if (autoPvrPixelFormat)
								result = new PvrTexture(gdix, targetPvrFormat);
							break;
					}
					break;
				// Converting to GVR
				case TextureFileFormat.Gvr:
					// Set formats for auto mode
					if (autoGvrDataFormat)
					{
						Console.WriteLine("Auto GVR formats not implemented yet");
						return;
					}
					if (autoGvrPaletteFormat)
					{
						Console.WriteLine("Auto GVR palette formats not implemented yet");
						return;
					}
					// Encode texture
					result = new GvrTexture(inputTexture.Image, targetGvrFormat, useMipmaps, inputPalette, gbix, null, targetGvrPaletteFormat, useDitheringForIndexed, encodeExternalPalette, saCompatibleGvrPalettes);
					break;
			}
			// Save the encoded texture
			File.WriteAllBytes(outputFilenameNoExt + outputExtension, result.GetBytes());
			// Save the encoded palette if available
			if (encodeExternalPalette && outputTexturePalette != null)
				outputTexturePalette.Save(outputFilenameNoExt + outputPaletteExtension, targetFileFormat == TextureFileFormat.Gvr ? true : false);
			Console.WriteLine("\nFinished!");
		}
	}
}