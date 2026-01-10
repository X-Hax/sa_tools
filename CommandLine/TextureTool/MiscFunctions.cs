using System;

namespace TextureTool
{
	internal class MiscFunctions
	{
		public static int FindArgument(string[] args, string arg)
		{
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == arg)
					return i;
			}
			return -1;
		}

		public static void ShowUsage()
		{
			Console.WriteLine("TextureTool is a program that converts between Dreamcast PVR, Gamecube GVR, Xbox/PC XVR, DDS and PNG formats.");
			Console.WriteLine("\nUsage:");
			Console.WriteLine("TextureTool <input file> [-o <output file>] [options]");
			Console.WriteLine("\nMAIN OPTIONS:");
			Console.WriteLine("-pvr : Convert the input texture to PVR.");
			Console.WriteLine("-gvr : Convert the input texture to GVR.");
			Console.WriteLine("-xvr : Convert the input texture to XVR.");
			Console.WriteLine("-dds : Convert the input texture to DDS.");
			Console.WriteLine("-o <output file> : Specify output filename and extension. Output format will be overridden based on the extension.");
			Console.WriteLine("-gbix <number> : Set Global Index to the specified decimal value.");
			Console.WriteLine("-dither : Enable dithering when encoding to Indexed texture formats.");
			Console.WriteLine("-extpal : Save the palette file; Prefer external palettes to internal CLUT in GVR textures.");
			Console.WriteLine("-m : Force add mipmaps to PVR/GVR/XVR/DDS; Save individual mipmaps to PNGs when decoding.");
			Console.WriteLine("-p <palette file> : Use the specified PVP/GVP palette file to encode or decode an Indexed texture.");
			Console.WriteLine("\nPVR PIXEL CODEC/PALETTE CODEC OPTIONS:");
			Console.WriteLine("-565 : Encode to RGB565 pixel format.");
			Console.WriteLine("-1555 : Encode to ARGB1555 pixel format.");
			Console.WriteLine("-4444 : Encode to ARGB4444 pixel format.");
			Console.WriteLine("-y422 : Encode to YUV422 pixel format.");
			Console.WriteLine("-bump : Encode to BUMP88 pixel format.");
			Console.WriteLine("-8888 : Encode to ARGB8888 pixel format (format ID 6).");
			Console.WriteLine("-8888alt : Encode to ARGB8888 pixel format (format ID 7).");
			Console.WriteLine("\nPVR DATA CODEC OPTIONS:");
			Console.WriteLine("-tw or -sq : Encode to Square Twiddled data format.");
			Console.WriteLine("-rect : Encode to Rectangular data format.");
			Console.WriteLine("-recttw : Encode to Rectangular Twiddled data format.");
			Console.WriteLine("-vq : Encode to VQ data format.");
			Console.WriteLine("-svq : Encode to Small VQ data format.");
			Console.WriteLine("-st : Encode to Rectangle Stride data format.");
			Console.WriteLine("-i4 : Encode to Indexed 4-bit data format.");
			Console.WriteLine("-i8 : Encode to Indexed 8-bit data format.");
			Console.WriteLine("-bmp : Encode to Bitmap data format. Forces pixel format to ARGB8888.");
			Console.WriteLine("-dma : Encode to Square Twiddled Mipmaps DMA data format.");
			Console.WriteLine("\nGVR CODEC OPTIONS:");
			Console.WriteLine("-int4 : Encode to Intensity4 format.");
			Console.WriteLine("-int4a : Encode to Intensity4A4 format.");
			Console.WriteLine("-int8 : Encode to Intensity8 format.");
			Console.WriteLine("-int8a : Encode to Intensity8A8 format.");
			Console.WriteLine("-565 : Encode to RGB565 format.");
			Console.WriteLine("-5a3 : Encode to RGB5A3 format.");
			Console.WriteLine("-8888 : Encode to ARGB8888 format.");
			Console.WriteLine("-i4 : Encode to Indexed 4-bit format.");
			Console.WriteLine("-i8 : Encode to Indexed 8-bit format.");
			Console.WriteLine("-dxt or -dxt1 : Encode to DXT1 format.");
			Console.WriteLine("\nGVR PALETTE FORMAT OPTIONS:");
			Console.WriteLine("-p_sa : Treat IntensityA8 and RGB5A3 as ARGB1555 and ARGB4444 (SADX GC/SA2B GC).");
			Console.WriteLine("-p_565 : Palette is in RGB565 format.");
			Console.WriteLine("-p_1555 : Palette is in ARGB1555 format.");
			Console.WriteLine("-p_4444 : Palette is in ARGB4444 format.");
			Console.WriteLine("-p_5a3 : Palette is in RGB5A3 format.");
			Console.WriteLine("-p_inta8 : Palette is in Intensity8A8 format.");
			Console.WriteLine("-p_8888 : Palette is in ARGB8888 format.");
			Console.WriteLine("\nXVR CODEC OPTIONS:");
			Console.WriteLine("-f <id>: Specify XVR format ID:");
			Console.WriteLine("1 or 11: ARGB8888");
			Console.WriteLine("2 or 12: RGB565");
			Console.WriteLine("3 or 13: ARGB1555");
			Console.WriteLine("4 or 14: ARGB4444");
			Console.WriteLine("6: DXT1");
			Console.WriteLine("7 or 8: DXT3");
			Console.WriteLine("9 or 10: DXT5");
			Console.WriteLine("\nDDS CODEC OPTIONS:");
			Console.WriteLine("-888 : Encode to RGB888 format.");
			Console.WriteLine("-565 : Encode to RGB565 format.");
			Console.WriteLine("-1555 : Encode to ARGB1555 format.");
			Console.WriteLine("-4444 : Encode to ARGB4444 format.");
			Console.WriteLine("-8888 : Encode to ARGB8888 format.");
			Console.WriteLine("-dxt1 : Encode to DXT1 format.");
			Console.WriteLine("-dxt3 : Encode to DXT3 format.");
			Console.WriteLine("-dxt5 : Encode to DXT5 format.");
			Console.WriteLine("\nPress ENTER to exit.");
			Console.ReadLine();
			return;
		}
	}
}