using System;
using System.Drawing;
using System.IO;
using System.Linq;
using TextureLib;

namespace TexTool
{
	// This tool is used for prototype development and testing of the SA Tools' texture library (TextureLib) and is not meant for general use (yet).

	// TODO: Check handling of paths

	enum ProgramMode
	{
		Convert,
		Error
	}

	enum TextureFileFormat
	{
		Pvr,
		Gvr,
		Xvr,
		Dds,
		Png
	}

	class Program
	{
		// Input-output stuff
		static string inputFilename;
		static string outputFilenameNoExt;
		static string outputExtension;
		static string outputFullFilename;
		static string outputPaletteExtension;
		static string paletteFilename;
		static ProgramMode programMode;
		static TextureFileFormat sourceFileFormat;
		static TextureFileFormat targetFileFormat;
		static GenericTexture inputTexture;
		static TexturePalette inputPalette;

		// Shared data and flags
		static uint gbix;
		static bool useMipmaps;
		static bool useDitheringForIndexed;
		static bool encodeExternalPalette;

		// GVR stuff
		static GvrDataFormat targetGvrFormat;
		static GvrPaletteFormat targetGvrPaletteFormat;
		static bool saCompatibleGvrPalettes;
		static bool autoGvrDataFormat;
		static bool autoGvrPaletteFormat;

		// PVR stuff
		static PvrDataFormat targetPvrFormat;
		static PvrPixelFormat targetPvrPixelFormat;
		static bool autoPvrDataFormat;
		static bool autoPvrPixelFormat;

		// DDS stuff
		static DdsFormat targetDdsFormat;
		static bool autoDdsDataFormat;

		// XVR stuff
		static int targetXvrFormat;
		static bool autoXvrFormat;

		static void ParseCommandLine(string[] args)
		{
			if (args.Length == 0)
			{
				ShowUsage();
				programMode = ProgramMode.Error;
				return;
			}
			// Set input file
			inputFilename = args[0];
			if (!File.Exists(inputFilename))
			{
				Console.WriteLine("File not found: {0}", inputFilename);
				programMode = ProgramMode.Error;
				return;
			}
			// Check mipmaps flag
			useMipmaps = args.Contains("-m");
			// Determine the output file format
			if (args.Contains("-pvr"))
				targetFileFormat = TextureFileFormat.Pvr;
			else if (args.Contains("-gvr"))
				targetFileFormat = TextureFileFormat.Gvr;
			else if (args.Contains("-xvr"))
				targetFileFormat = TextureFileFormat.Xvr;
			else if (args.Contains("-dds"))
				targetFileFormat = TextureFileFormat.Dds;
			else
				targetFileFormat = TextureFileFormat.Png;
			// Determine the input file format
			switch (Path.GetExtension(inputFilename).ToLowerInvariant())
			{
				case ".pvr":
					sourceFileFormat = TextureFileFormat.Pvr;
					break;
				case ".gvr":
					sourceFileFormat = TextureFileFormat.Gvr;
					break;
				case ".xvr":
					sourceFileFormat = TextureFileFormat.Xvr;
					break;
				case ".dds":
					sourceFileFormat = TextureFileFormat.Dds;
					break;
				case ".png":
				case ".bmp":
				case ".jpg":
				case ".gif":
					sourceFileFormat = TextureFileFormat.Png;
					break;
				default:
					Console.WriteLine("Unsupported input format: {0}", inputFilename);
					programMode = ProgramMode.Error;
					break;
			}
			// Check arguments for the encoder
			if (targetFileFormat != TextureFileFormat.Png)
			{
				// Check gbix
				if (args.Contains("-gbix"))
					gbix = uint.Parse(args[FindArgument(args, "-gbix") + 1]);
				// Check dithering flag
				useDitheringForIndexed = args.Contains("-dither");
				// Check external palette flag
				encodeExternalPalette = args.Contains("-extpal");				
				// Get target texture pixel/data format from command line arguments
				switch (targetFileFormat)
				{
					// Data format
					case TextureFileFormat.Pvr:
						if (args.Contains("-tw") || args.Contains("-sq"))
							targetPvrFormat = useMipmaps ? PvrDataFormat.SquareTwiddledMipmaps : PvrDataFormat.SquareTwiddled;
						else if (args.Contains("-vq"))
							targetPvrFormat = useMipmaps ? PvrDataFormat.VqMipmaps : PvrDataFormat.Vq;
						else if (args.Contains("-i4"))
							targetPvrFormat = useMipmaps ? PvrDataFormat.Index4Mipmaps : PvrDataFormat.Index4;
						else if (args.Contains("-i8"))
							targetPvrFormat = useMipmaps ? PvrDataFormat.Index8Mipmaps : PvrDataFormat.Index8;
						else if (args.Contains("-rect"))
							targetPvrFormat = useMipmaps ? PvrDataFormat.RectangleMipmaps : PvrDataFormat.Rectangle; // No actual difference
						else if (args.Contains("-st"))
							targetPvrFormat = useMipmaps ? PvrDataFormat.RectangleStrideMipmaps : PvrDataFormat.RectangleStride; // No actual difference
						else if (args.Contains("-recttw"))
							targetPvrFormat = PvrDataFormat.RectangleTwiddled;
						else if (args.Contains("-bmp"))
							targetPvrFormat = useMipmaps ? PvrDataFormat.BitmapMipmaps : PvrDataFormat.Bitmap;
						else if (args.Contains("-svq"))
							targetPvrFormat = useMipmaps ? PvrDataFormat.SmallVqMipmaps : PvrDataFormat.SmallVq;
						else if (args.Contains("-dma"))
							targetPvrFormat = PvrDataFormat.SquareTwiddledMipmapsAlt;
						else
							autoPvrDataFormat = true;
						// Pixel format
						if (args.Contains("-1555"))
							targetPvrPixelFormat = PvrPixelFormat.Argb1555;
						else if (args.Contains("-565"))
							targetPvrPixelFormat = PvrPixelFormat.Rgb565;
						else if (args.Contains("-4444"))
							targetPvrPixelFormat = PvrPixelFormat.Argb4444;
						else if (args.Contains("-y422"))
							targetPvrPixelFormat = PvrPixelFormat.Yuv422;
						else if (args.Contains("-bump"))
							targetPvrPixelFormat = PvrPixelFormat.Bump88;
						else if (args.Contains("-555"))
							targetPvrPixelFormat = PvrPixelFormat.Rgb555;
						else if (args.Contains("-8888"))
							targetPvrPixelFormat = PvrPixelFormat.Argb8888;
						//else if (args.Contains("-y420"))
						//targetPvrPixelFormat = PvrPixelFormat.Argb8888orYUV420; // Not implemented and probably doesn't exist
						else
							autoPvrPixelFormat = true;
						break;
					case TextureFileFormat.Gvr:
						if (args.Contains("-int4"))
							targetGvrFormat = GvrDataFormat.Intensity4;
						else if (args.Contains("-inta4"))
							targetGvrFormat = GvrDataFormat.IntensityA44;
						else if (args.Contains("-int8"))
							targetGvrFormat = GvrDataFormat.Intensity8;
						else if (args.Contains("-inta8"))
							targetGvrFormat = GvrDataFormat.IntensityA88;
						else if (args.Contains("-565"))
							targetGvrFormat = GvrDataFormat.Rgb565;
						else if (args.Contains("-5a3"))
							targetGvrFormat = GvrDataFormat.Rgb5a3;
						else if (args.Contains("-8888"))
							targetGvrFormat = GvrDataFormat.Argb8888;
						else if (args.Contains("-i4"))
							targetGvrFormat = GvrDataFormat.Index4;
						else if (args.Contains("-i8"))
							targetGvrFormat = GvrDataFormat.Index8;
						else if (args.Contains("-i14"))
							targetGvrFormat = GvrDataFormat.Index14;
						else if (args.Contains("-dxt"))
							targetGvrFormat = GvrDataFormat.Dxt1;
						else
							autoGvrDataFormat = true;
						// Get target palette format if necessary
						if (targetGvrFormat == GvrDataFormat.Index4 || targetGvrFormat == GvrDataFormat.Index8 || targetGvrFormat == GvrDataFormat.Index14)
						{
							if (args.Contains("-p_565"))
								targetGvrPaletteFormat = GvrPaletteFormat.Rgb565;
							else if (args.Contains("-p_1555"))
							{
								targetGvrPaletteFormat = GvrPaletteFormat.IntensityA8orArgb1555;
								saCompatibleGvrPalettes = true;
							}
							else if (args.Contains("-p_4444"))
							{
								targetGvrPaletteFormat = GvrPaletteFormat.Rgb5A3orArgb4444;
								saCompatibleGvrPalettes = true;
							}
							else if (args.Contains("-p_5a3"))
							{
								targetGvrPaletteFormat = GvrPaletteFormat.Rgb5A3orArgb4444;
								saCompatibleGvrPalettes = false;
							}
							else if (args.Contains("-p_inta8"))
							{
								targetGvrPaletteFormat = GvrPaletteFormat.IntensityA8orArgb1555;
								saCompatibleGvrPalettes = false;
							}
							else if (args.Contains("-p_8888"))
							{
								targetGvrPaletteFormat = GvrPaletteFormat.Argb8888;
								saCompatibleGvrPalettes = false;
							}
							else
								autoGvrPaletteFormat = true;
						}
						break;
					case TextureFileFormat.Xvr:
						if (args.Contains("-f"))
							targetXvrFormat = int.Parse(args[FindArgument(args, "-f") + 1]);
						else
							autoXvrFormat = true;
						break;
					case TextureFileFormat.Dds:
						if (args.Contains("-888"))
							targetDdsFormat = DdsFormat.Rgb888;
						else if (args.Contains("-565"))
							targetDdsFormat = DdsFormat.Rgb565;
						else if (args.Contains("-1555"))
							targetDdsFormat = DdsFormat.Argb1555;
						else if (args.Contains("-4444"))
							targetDdsFormat = DdsFormat.Argb4444;
						else if (args.Contains("-8888"))
							targetDdsFormat = DdsFormat.Argb8888;
						else if (args.Contains("-dxt1"))
							targetDdsFormat = DdsFormat.Dxt1;
						else if (args.Contains("-dxt3"))
							targetDdsFormat = DdsFormat.Dxt3;
						else if (args.Contains("-dxt5"))
							targetDdsFormat = DdsFormat.Dxt5;
						else
							autoDdsDataFormat = true;
						break;
					default:
						break;
				}
			}
			// Check if palette filename is specified
			if (args.Contains("-p"))
			{
				paletteFilename = args[FindArgument(args, "-p") + 1];
				if (!File.Exists(paletteFilename))
				{
					Console.WriteLine("Palette file not found: {0}", paletteFilename);
					programMode = ProgramMode.Error;
					return;
				}
			}
			// Set output filename
			if (args.Contains("-o"))
			{
				outputFullFilename = args[FindArgument(args, "-o") + 1];
				outputFilenameNoExt = Path.GetFileNameWithoutExtension(outputFullFilename);
				outputExtension = Path.GetExtension(outputFullFilename);
			}
			else
			{
				outputFilenameNoExt = Path.GetFileNameWithoutExtension(inputFilename);
				outputExtension = "";
				switch (targetFileFormat)
				{
					case TextureFileFormat.Pvr:
						outputExtension = ".pvr";
						outputPaletteExtension = ".pvp";
						break;
					case TextureFileFormat.Gvr:
						outputExtension = ".gvr";
						outputPaletteExtension = ".gvp";
						break;
					case TextureFileFormat.Xvr:
						outputExtension = ".xvr";
						break;
					case TextureFileFormat.Dds:
						outputExtension = ".dds";
						break;
					case TextureFileFormat.Png:
						outputExtension = ".png";
						break;
					default:
						throw new Exception("Unknown target format: " + targetFileFormat.ToString());
				}
				outputFullFilename = outputFilenameNoExt + outputExtension;
			}
		}

		static void Main(string[] args)
		{
			ParseCommandLine(args);
			if (programMode == ProgramMode.Error)
				return;
			Console.WriteLine("Input file: {0}", inputFilename);
			Console.WriteLine("Output file: {0}", outputFullFilename);
			Console.WriteLine("Target format: {0}", targetFileFormat.ToString());
			Console.WriteLine("Mipmaps: {0}", useMipmaps.ToString());
			if (targetGvrFormat == GvrDataFormat.Index4 || targetGvrFormat == GvrDataFormat.Index8 || targetGvrFormat == GvrDataFormat.Index14 ||
				targetPvrFormat == PvrDataFormat.Index4 || targetPvrFormat == PvrDataFormat.Index8)
				Console.WriteLine("Dithering: {0}", useDitheringForIndexed.ToString());
			// Read palette file if specified
			inputPalette = string.IsNullOrEmpty(paletteFilename) ? null :
				new TexturePalette(File.ReadAllBytes(paletteFilename), false);
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
					inputTexture = new DdsTexture(inputFile);
					break;
				case TextureFileFormat.Xvr:
					inputTexture = new XvrTexture(inputFile);
					break;
				case TextureFileFormat.Png:
					inputTexture = new GdiTexture(inputFile);
					break;
				default:
					Console.WriteLine("{0}: input texture format not implemented", inputFile);
					return;
			}
			// Decode
			if (targetFileFormat == TextureFileFormat.Png)
			{
				// Save texture bitmap
				inputTexture.Image.Save(outputFullFilename);
				// Save mipmap bitmaps if specified
				if (useMipmaps && inputTexture.HasMipmaps && inputTexture.MipmapImages != null)
					for (int m = 0; m < inputTexture.MipmapImages.Length; m++)
						inputTexture.MipmapImages[m].Save(outputFilenameNoExt + "_mip" + m.ToString() + outputExtension);
			}
			// Encode
			else
			{
				// Read palette file if specified
				inputPalette = string.IsNullOrEmpty(paletteFilename) ? null :
					new TexturePalette(File.ReadAllBytes(paletteFilename), false);
				// Output encoder parameters
				Console.WriteLine("\nENCODER PARAMETERS");
				switch (targetFileFormat)
				{
					case TextureFileFormat.Dds:
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
				// Read the input bitmap
				TexturePalette outputTexturePalette = null;
				GenericTexture result = null;
				switch (targetFileFormat)
				{
					case TextureFileFormat.Dds:
						// Set formats for auto mode
						if (autoDdsDataFormat)
						{
							Console.WriteLine("Auto DDS formats not implemented yet");
							return;
						}
						// Encode texture
						result = new DdsTexture(inputTexture.Image, targetDdsFormat, useMipmaps);
						break;
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
					case TextureFileFormat.Pvr:
						// Convert from PVR
						if (inputTexture is PvrTexture pvr)
							result = new PvrTexture(pvr, targetPvrPixelFormat, targetPvrFormat);
						// Convert from GVR
						if (inputTexture is GvrTexture gvr)
							result = autoPvrDataFormat ? new PvrTexture(gvr) : new PvrTexture(gvr, targetPvrFormat);
						// Convert from DDS
						else if (inputTexture is DdsTexture dds)
							result = autoPvrDataFormat ? new PvrTexture(dds) : new PvrTexture(dds, targetPvrFormat);
						// Convert from XVR
						else if (inputTexture is XvrTexture xvr)
							result = autoPvrDataFormat ? new PvrTexture(xvr) : new PvrTexture(xvr, targetPvrFormat);
						// Convert from PNG
						else
						{
							// Set formats for auto mode
							if (autoPvrDataFormat)
							{
								Console.WriteLine("Auto PVR data formats not implemented yet");
								return;
							}
							if (autoPvrPixelFormat)
							{
								Console.WriteLine("Auto PVR pixel formats not implemented yet");
								return;
							}
							// Encode texture
							result = new PvrTexture(inputTexture.Image, targetPvrFormat, targetPvrPixelFormat, useMipmaps, inputPalette, gbix, null, useDitheringForIndexed, encodeExternalPalette);
						}
						break;
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
				Console.WriteLine("Finished!");
			}
		}

		static int FindArgument(string[] args, string arg)
		{
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i]==arg)
					return i;
			}
			return -1;
		}

		static void ShowUsage()
		{
			Console.WriteLine("No arguments");
		}
	}
}