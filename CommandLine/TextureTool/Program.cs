using TextureLib;

namespace TextureTool
{
	// This tool is used for prototype development and testing of the SA Tools' texture library (TextureLib) and is not meant for general use (yet).

	// TODO: Check handling of paths

	partial class Program
	{
		// Input-output stuff
		static string inputFilename;
		static string outputFilenameNoExt;
		static string outputExtension;
		static string outputFullFilename;
		static string outputPaletteExtension;
		static string paletteFilename;
		static bool programError;
		static TextureFileFormat sourceFileFormat;
		static TextureFileFormat targetFileFormat;
		static GenericTexture inputTexture;
		static TexturePalette inputPalette;
		static bool autoOutputFilename = true;

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

		static void Main(string[] args)
		{
			ParseCommandLine(args);
			if (programError)
				return;
			Conversion();
		}
	}
}