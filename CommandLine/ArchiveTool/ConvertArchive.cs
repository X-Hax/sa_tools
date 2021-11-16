using ArchiveLib;
using System;
using System.Drawing;
using System.IO;
using VrSharp;
using VrSharp.Gvr;
using VrSharp.Pvr;

namespace ArchiveTool
{
	static partial class Program
	{
		static string directoryName;
		static string fileNameFromFolder;
		/// <summary>
		/// Convert GVM to PVM.
		/// </summary>
		static void GVM2PVM(string[] args)
		{
			filePath = args[1];
			if (args.Length > 2 && args[2] == "-prs") compressPRS = true;
			Console.WriteLine("Converting GVM to PVM: {0}", filePath);
			directoryName = Path.GetDirectoryName(filePath);
			extension = Path.GetExtension(filePath).ToLowerInvariant();
			if (!File.Exists(filePath))
			{
				Console.WriteLine("Supplied GVM archive does not exist!");
				Console.WriteLine("Press ENTER to exit.");
				Console.ReadLine();
				return;
			}
			if (extension != ".gvm")
			{
				Console.WriteLine("GVM2PVM mode can only be used with GVM files.");
				Console.WriteLine("Press ENTER to exit.");
				Console.ReadLine();
				return;
			}
			fileNameFromFolder = Path.Combine(directoryName, Path.GetFileNameWithoutExtension(filePath));
			Directory.CreateDirectory(fileNameFromFolder);
			using (TextWriter texList = File.CreateText(Path.Combine(fileNameFromFolder, Path.GetFileName(fileNameFromFolder) + ".txt")))
			{
				try
				{
					PuyoFile gvmFile = new PuyoFile(File.ReadAllBytes(filePath));
					PuyoFile pvmFile = new PuyoFile();
					outputPath = Path.ChangeExtension(filePath, ".pvm");
					foreach (GVMEntry file in gvmFile.Entries)
					{
						if (!File.Exists(Path.Combine(fileNameFromFolder, file.Name)))
							File.WriteAllBytes(Path.Combine(fileNameFromFolder, file.Name), file.Data);
						Stream data = File.Open(Path.Combine(fileNameFromFolder, file.Name), FileMode.Open);
						VrTexture vrfile = new GvrTexture(data);
						Bitmap tempTexture = vrfile.ToBitmap();
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
						PvrDataFormat pdf;
						if (!vrfile.HasMipmaps)
						{
							if (tempTexture.Width == tempTexture.Height)
								pdf = PvrDataFormat.SquareTwiddled;
							else
								pdf = PvrDataFormat.Rectangle;
						}
						else
						{
							if (tempTexture.Width == tempTexture.Height)
								pdf = PvrDataFormat.SquareTwiddledMipmaps;
							else
								pdf = PvrDataFormat.RectangleTwiddled;
						}
						PvrTextureEncoder encoder = new PvrTextureEncoder(tempTexture, ppf, pdf);
						encoder.GlobalIndex = vrfile.GlobalIndex;
						string pvrPath = Path.ChangeExtension(Path.Combine(fileNameFromFolder, file.Name), ".pvr");
						if (!File.Exists(pvrPath))
							encoder.Save(pvrPath);
						data.Close();
						File.Delete(Path.Combine(fileNameFromFolder, file.Name));
						MemoryStream readfile = new MemoryStream(File.ReadAllBytes(pvrPath));
						pvmFile.Entries.Add(new PVMEntry(pvrPath));
						texList.WriteLine(Path.GetFileName(pvrPath));
						Console.WriteLine("Adding texture {0}", pvrPath);
					}
					File.WriteAllBytes(outputPath, pvmFile.GetBytes());
					texList.Flush();
					texList.Close();
				}
				catch (Exception ex)
				{
					Console.WriteLine("Exception thrown: {0}", ex.ToString());
					Console.WriteLine("Press ENTER to exit.");
					Console.ReadLine();
					return;
				}

			}
			if (compressPRS)
			{
				outputPath = Path.ChangeExtension(filePath, ".PVM.PRS");
				Console.WriteLine("Compressing to PRS...");
				byte[] pvmdata = File.ReadAllBytes(Path.ChangeExtension(filePath, ".pvm"));
				pvmdata = FraGag.Compression.Prs.Compress(pvmdata);
				File.WriteAllBytes(outputPath, pvmdata);
				File.Delete(Path.ChangeExtension(filePath, ".PVM"));
			}
			Console.WriteLine("Output file: {0}", Path.GetFullPath(outputPath));
			Console.WriteLine("Archive converted!");
			Directory.Delete(fileNameFromFolder, true);
		}
	}
}