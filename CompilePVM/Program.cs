using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using PuyoTools.Modules.Archive;
using VrSharp.PvrTexture;

namespace CompilePVM
{
	class Program
	{
		static void Main(string[] args)
		{
			string directoryName;
			UInt32 startingGBIX = 0;
			List<String> textureNames = new List<String>();
			List<PvrTexture> finalTextureList = new List<PvrTexture>();

			if (args.Length == 0)
			{
				Console.WriteLine("Error - no texturelist provided. Provide a path to a texturelist as your first command line argument");
				Console.WriteLine("Press ENTER to continue...");
				Console.ReadLine();
				return;
			}

			String filePath = args[0];
			directoryName = Path.GetDirectoryName(filePath);
			string archiveName = Path.GetFileNameWithoutExtension(filePath);

			if (File.Exists(filePath))
			{
				StreamReader texlistStream = File.OpenText(filePath);

				startingGBIX = UInt32.Parse(texlistStream.ReadLine());

				while (!texlistStream.EndOfStream)
					textureNames.Add(texlistStream.ReadLine());

				PvmArchive pvmArchive = new PvmArchive();
				Stream pvmStream = File.Open(Path.ChangeExtension(filePath, ".pvm"), FileMode.Create);
				PvmArchiveWriter pvmWriter = (PvmArchiveWriter)pvmArchive.Create(pvmStream);

				// Reading in textures
				for (uint imgIndx = 0; imgIndx < textureNames.Count; imgIndx++)
				{
					Bitmap tempTexture = new Bitmap(8, 8);
					string texturePath = Path.Combine(directoryName, Path.ChangeExtension(textureNames[(int)imgIndx], ".png"));
					if (File.Exists(texturePath))
					{
						tempTexture = (Bitmap)Bitmap.FromFile(texturePath);
						tempTexture = tempTexture.Clone(new Rectangle(Point.Empty, tempTexture.Size), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
					}
					else
					{
						Console.WriteLine(String.Concat("Texture ", textureNames[(int)imgIndx], " not found. Generating a placeholder. Check your files."));
					}

					System.Drawing.Imaging.BitmapData bmpd = tempTexture.LockBits(new Rectangle(Point.Empty, tempTexture.Size), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
					int stride = bmpd.Stride;
					byte[] bits = new byte[Math.Abs(stride) * bmpd.Height];
					System.Runtime.InteropServices.Marshal.Copy(bmpd.Scan0, bits, 0, bits.Length);
					tempTexture.UnlockBits(bmpd);
					int tlevels = 0;
					for (int y = 0; y < tempTexture.Height; y++)
					{
						int srcaddr = y * Math.Abs(stride);
						for (int x = 0; x < tempTexture.Width; x++)
						{
							Color c = Color.FromArgb(BitConverter.ToInt32(bits, srcaddr + (x * 4)));
							if (c.A == 0)
								tlevels = 1;
							else if (c.A < 255)
							{
								tlevels = 2;
								break;
							}
						}
						if (tlevels == 2)
							break;
					}
					PvrPixelFormat ppf = PvrPixelFormat.Rgb565;
					if (tlevels == 1)
						ppf = PvrPixelFormat.Argb1555;
					else if (tlevels == 2)
						ppf = PvrPixelFormat.Argb4444;
					PvrDataFormat pdf = PvrDataFormat.Rectangle;
					if (tempTexture.Width == tempTexture.Height)
						pdf = PvrDataFormat.SquareTwiddled;
					PvrTextureEncoder encoder = new PvrTextureEncoder(tempTexture, ppf, pdf);
					encoder.GlobalIndex = startingGBIX + imgIndx;
					string pvrPath = Path.ChangeExtension(texturePath, ".pvr");
					encoder.Save(pvrPath);

					pvmWriter.CreateEntryFromFile(pvrPath);
				}

				pvmWriter.Flush();
				pvmStream.Close();

				Console.WriteLine("PVM was compiled successfully!");
				Console.WriteLine("Press ENTER to continue...");
				Console.ReadLine();
				return;
			}
			else // error, supplied path is invalid
			{
				Console.WriteLine("Supplied texturelist does not exist!");
				Console.WriteLine("Press ENTER to continue...");
				Console.ReadLine();
				return;
			}
		}
	}
}