using System;
using System.Drawing;
using System.IO;
using System.Linq;
using TextureLib;

namespace TexTool
{
	// This tool is used for prototype development and testing of the SA Tools' texture library (TextureLib) and is not meant for general use (yet).

	class Program
	{
		static void Main(string[] args)
		{
			int mode = args.Contains("-d") ? 1 : 0;
			switch (mode)
			{
				case 0:
					Bitmap test = new Bitmap(args[0]);
					GvrDataFormat targetFormat = GvrDataFormat.Index8;
					TexturePalette texpal;
					GvrTexture result = new GvrTexture(test, targetFormat, true, out texpal, 1111, GvrPaletteFormat.Rgb5A3orArgb4444, false, false, false);
					File.WriteAllBytes("test.gvr", result.GetBytes());
					if (texpal != null)
					{
						texpal.SaveGVP("test.gvp");
					}
					return;
				case 1:
					GenericTexture tex = null;
					TexturePalette pale = null;
					byte[] inputFile = File.ReadAllBytes(args[0]);
					if (args.Length > 2)
					{
						byte[] inputPalette = File.ReadAllBytes(args[2]);
						pale = new TexturePalette(inputPalette, false);
						pale.SavePNG("pale.png");
					}
					switch (Path.GetExtension(args[0]).ToLowerInvariant())
					{
						case ".pvr":
							tex = new PvrTexture(inputFile, extPalette: pale);
							break;
						case ".gvr":
							tex = new GvrTexture(inputFile, extPalette: pale);
							break;
					}

					tex.Image.Save("test.png");
					if (tex.HasMipmaps && tex.MipmapImages != null)
						for (int m = 0; m < tex.MipmapImages.Length; m++)
							tex.MipmapImages[m].Save("test_" + m.ToString() + ".png");
					return;
			}

			//GvrTexture ass2 = new GvrTexture(File.ReadAllBytes("test.gvr"));
			//ass2.Image.Save("ass2.png");
			return;
			//TexturePalette testP = new TexturePalette(File.ReadAllBytes(args[0]));
			//testP.SaveGVP("test.gvp");
			//testP.SavePVP("test.pvp");
			//testP.SavePNG("test.png");
			//return;
			/*

            */

		}
	}
}