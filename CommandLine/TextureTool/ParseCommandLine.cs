using System;
using System.IO;
using System.Linq;
using TextureLib;

namespace TextureTool
{
	static partial class Program
	{
		static void ParseCommandLine(string[] args)
		{
			if (args.Length == 0)
			{
				MiscFunctions.ShowUsage();
				programError = true;
				return;
			}
			// Set input file
			inputFilename = args[0];
			if (!File.Exists(inputFilename))
			{
				Console.WriteLine("File not found: {0}", inputFilename);
				programError = true;
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
			sourceFileFormat = GenericTexture.GetTextureFileType(File.ReadAllBytes(inputFilename));
			if (sourceFileFormat == TextureFileFormat.Unknown)
			{
				Console.WriteLine("Unsupported input format: {0}", inputFilename);
				programError = true;
				return;
			}
			// Set output filename
			if (args.Contains("-o"))
			{
				autoOutputFilename = false;
				outputFullFilename = args[MiscFunctions.FindArgument(args, "-o") + 1];
				outputFilenameNoExt = Path.GetFileNameWithoutExtension(outputFullFilename);
				outputExtension = Path.GetExtension(outputFullFilename);
				// Set output file format if the filename is specified
				switch (outputExtension.ToLowerInvariant())
				{
					case ".pvr":
						targetFileFormat = TextureFileFormat.Pvr;
						break;
					case ".gvr":
						targetFileFormat = TextureFileFormat.Gvr;
						break;
					case ".dds":
						targetFileFormat = TextureFileFormat.Dds;
						break;
					case ".xvr":
						targetFileFormat = TextureFileFormat.Xvr;
						break;
					case ".png":
						targetFileFormat = TextureFileFormat.Png;
						break;
					default:
						break;
				}
			}
			// Check arguments for the encoder
			if (targetFileFormat != TextureFileFormat.Png)
			{
				// Check gbix
				if (args.Contains("-gbix"))
					gbix = uint.Parse(args[MiscFunctions.FindArgument(args, "-gbix") + 1]);
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
							targetPvrFormat = PvrDataFormat.SquareTwiddledMipmapsDma;
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
						//else if (args.Contains("-i14"))
							//targetGvrFormat = GvrDataFormat.Index14;
						else if (args.Contains("-dxt") || args.Contains("-dxt1"))
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
							targetXvrFormat = int.Parse(args[MiscFunctions.FindArgument(args, "-f") + 1]);
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
				paletteFilename = args[MiscFunctions.FindArgument(args, "-p") + 1];
				if (!File.Exists(paletteFilename))
				{
					Console.WriteLine("Palette file not found: {0}", paletteFilename);
					programError = true;
					return;
				}
			}
			// Set output filename
			if (autoOutputFilename)
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
						Console.WriteLine("Unknown target format: " + targetFileFormat.ToString());
						programError = true;
						return;
				}
				outputFullFilename = outputFilenameNoExt + outputExtension;
			}
		}
	}
}