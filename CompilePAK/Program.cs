using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using PAKLib;
using Microsoft.DirectX.Direct3D;
using System.Text;

namespace CompilePVM
{
	class Program
	{
		static void Main(string[] args)
		{
			string directoryName;
			UInt32 startingGBIX = 0;
			List<String> textureNames = new List<String>();

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

				string filenoext = Path.GetFileNameWithoutExtension(filePath).ToLowerInvariant();
				string longdir = "..\\..\\..\\sonic2\\resource\\gd_pc\\prs\\" + filenoext;
				PAKFile pak = new PAKFile();
				List<byte> inf = new List<byte>();
				using (System.Windows.Forms.Panel panel1 = new System.Windows.Forms.Panel())
				using (Device d3ddevice = new Device(0, DeviceType.Hardware, panel1, CreateFlags.SoftwareVertexProcessing, new PresentParameters[] { new PresentParameters() { Windowed = true, SwapEffect = SwapEffect.Discard, EnableAutoDepthStencil = true, AutoDepthStencilFormat = DepthFormat.D24X8 } }))
				{
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

						Stream tex = TextureLoader.SaveToStream(ImageFileFormat.Dds, Texture.FromBitmap(d3ddevice, tempTexture, Usage.SoftwareProcessing, Pool.Managed));
						byte[] tb = new byte[tex.Length];
						tex.Read(tb, 0, tb.Length);
						pak.Files.Add(new PAKFile.File(filenoext + '\\' + Path.ChangeExtension(textureNames[(int)imgIndx], ".dds"), longdir + '\\' + Path.ChangeExtension(textureNames[(int)imgIndx], ".dds"), tb));
						int i = inf.Count;
						inf.AddRange(Encoding.ASCII.GetBytes(Path.ChangeExtension(textureNames[(int)imgIndx], null)));
						inf.AddRange(new byte[0x1C - (inf.Count - i)]);
						inf.AddRange(BitConverter.GetBytes(startingGBIX + imgIndx));
						inf.AddRange(BitConverter.GetBytes(0));
						inf.AddRange(BitConverter.GetBytes(0));
						inf.AddRange(BitConverter.GetBytes(0));
						inf.AddRange(BitConverter.GetBytes(tempTexture.Width));
						inf.AddRange(BitConverter.GetBytes(tempTexture.Height));
						inf.AddRange(BitConverter.GetBytes(0));
						inf.AddRange(BitConverter.GetBytes(0x80000000));
					}
				}
				pak.Files.Insert(0, new PAKFile.File(filenoext + '\\' + filenoext + ".inf", longdir + '\\' + filenoext + ".inf", inf.ToArray()));
				pak.Save(Path.ChangeExtension(filePath, "pak"));

				Console.WriteLine("PAK was compiled successfully!");
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