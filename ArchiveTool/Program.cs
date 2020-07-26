using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using PuyoTools.Modules.Archive;
using VrSharp;
using VrSharp.Gvr;
using VrSharp.Pvr;

namespace ArchiveTool
{
	static class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("ArchiveTool is a command line tool to extract and create PVM, GVM and PRS archives.\n");
				Console.WriteLine("Usage:\n");
				Console.WriteLine("Extracting a PVM/GVM/PRS archive:\nArchiveTool file.pvm\nIf the archive is PRS compressed, it will be decompressed first.\nIf the archive contains textures, the program will produce a folder with all PVR/GVR textures and a texture list.\n");
				Console.WriteLine("Converting PVM/GVM to a folder texture pack: ArchiveTool -png file.pvm\n");
				Console.WriteLine("Creating a PVM/GVM from a list of PVR/GVR textures:\nArchiveTool texturelist.txt\nThe program will create a PVM/GVM archive from a texture list. Textures must be in the same folder as the texture list.\n");
				Console.WriteLine("Creating a PVM from PNG textures: ArchiveTool -pvm texturelist.txt\nThe texture list must contain global indices listed before each texture filename for this option to work.\n");
				Console.WriteLine("Creating a PRS compressed PVM/GVM:\nArchiveTool texturelist.txt -prs\nSame as the previous two options but the PVM/GVM file will be PRS compressed.\n");
				Console.WriteLine("Creating a PRS compressed binary:\nArchiveTool file.bin\nA PRS archive will be created from the file.\nFile extension must be .BIN for this option to work.\n");
				Console.WriteLine("Press ENTER to exit.");
				Console.ReadLine();
				return;
			}
			//CompilePVM mode
			else if (args[0] == "-pvm")
			{
				bool IsPRS = false;
				if (args[args.Length - 1] == "-prs") IsPRS = true;
				String filePath = args[1];
				string directoryName;
				string FullLine;
				string texturename;
				UInt32 GBIX = 0;
				List<String> textureNames = new List<String>();
				List<PvrTexture> finalTextureList = new List<PvrTexture>();
				directoryName = Path.GetDirectoryName(filePath);
				string archiveName = Path.GetFileNameWithoutExtension(filePath);
				ArchiveBase pvmArchive;
				ArchiveWriter pvmWriter;
				if (File.Exists(filePath))
				{
					StreamReader texlistStream = File.OpenText(filePath);
					while (!texlistStream.EndOfStream) textureNames.Add(texlistStream.ReadLine());
					pvmArchive = new PvmArchive();
					Stream pvmStream = File.Open(Path.ChangeExtension(filePath, ".pvm"), FileMode.Create);
					pvmWriter = (PvmArchiveWriter)pvmArchive.Create(pvmStream);
					// Reading in textures
					for (uint imgIndx = 0; imgIndx < textureNames.Count; imgIndx++)
					{
						FullLine = textureNames[(int)imgIndx];
						if (string.IsNullOrEmpty(FullLine)) continue;
						String[] substrings = FullLine.Split(',');
						GBIX = UInt32.Parse(substrings[0]);
						texturename = substrings[1];
						Bitmap tempTexture = new Bitmap(8, 8);
						string texturePath = Path.Combine(directoryName, Path.ChangeExtension(texturename, ".png"));
						if (File.Exists(texturePath))
						{
							Console.WriteLine("Adding texture:" + (texturePath));
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
						encoder.GlobalIndex = GBIX;
						string pvrPath = Path.ChangeExtension(texturePath, ".pvr");
						encoder.Save(pvrPath);
						pvmWriter.CreateEntryFromFile(pvrPath);
					}
					pvmWriter.Flush();
					pvmStream.Close();
					if (IsPRS)
					{
						byte[] pvmdata = File.ReadAllBytes(Path.ChangeExtension(filePath, ".pvm"));
						pvmdata = FraGag.Compression.Prs.Compress(pvmdata);
						File.WriteAllBytes(Path.ChangeExtension(filePath, ".prs"), pvmdata);
						File.Delete(Path.ChangeExtension(filePath, ".pvm"));
					}
					Console.WriteLine("Archive was compiled successfully!");
					return;
				}
				else
				{
					Console.WriteLine("Supplied texture list does not exist!");
					Console.WriteLine("Press ENTER to continue...");
					Console.ReadLine();
					return;
				}
			}
			//PVM2TexPack mode
			else if (args[0] == "-png")
			{
				Queue<string> files = new Queue<string>();
				for (int u = 1; u < args.Length; u++)
				{
					files.Enqueue(args[u]);
				}
				if (files.Count == 0)
				{
					Console.Write("File: ");
					files.Enqueue(Console.ReadLine());
				}
				while (files.Count > 0)
				{
					string filename = files.Dequeue();
					string path = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filename)), Path.GetFileNameWithoutExtension(filename));
					string filename_full = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filename)), Path.GetFileName(filename));
					Console.WriteLine("File: " + filename_full);
					Directory.CreateDirectory(path);
					byte[] filedata = File.ReadAllBytes(filename_full);
					using (TextWriter texList = File.CreateText(Path.Combine(path, "index.txt")))
					{
						try
						{
							if (PvrTexture.Is(filedata))
							{
								if (!AddTexture(false, path, Path.GetFileName(filename_full), new MemoryStream(filedata), texList))
								{
									texList.Close();
									Directory.Delete(path, true);
								}
								continue;
							}
							else if (GvrTexture.Is(filedata))
							{
								if (!AddTexture(true, path, Path.GetFileName(filename_full), new MemoryStream(filedata), texList))
								{
									texList.Close();
									Directory.Delete(path, true);
								}
								continue;
							}
							bool gvm = false;
							ArchiveBase pvmfile = null;
							byte[] pvmdata = File.ReadAllBytes(filename_full);
							if (Path.GetExtension(filename_full).Equals(".prs", StringComparison.OrdinalIgnoreCase))
								pvmdata = FraGag.Compression.Prs.Decompress(pvmdata);
							pvmfile = new PvmArchive();
							MemoryStream stream = new MemoryStream(pvmdata);
							if (!PvmArchive.Identify(stream))
							{
								pvmfile = new GvmArchive();
								gvm = true;
							}
							ArchiveEntryCollection pvmentries = pvmfile.Open(pvmdata).Entries;
							bool fail = false;
							foreach (ArchiveEntry file in pvmentries)
								if (!AddTexture(gvm, path, file.Name, file.Open(), texList))
								{
									texList.Close();
									Directory.Delete(path, true);
									fail = true;
									break;
								}
							if (fail)
								continue;
						}
						catch (Exception ex)
						{
							Console.WriteLine("Exception thrown: " + ex.ToString() + "\nCanceling conversion.");
							return;
						}
					}
				}
			}
			else
			{
				string filePath = args[0];
				bool IsPRS = false;
				bool IsGVM = false;
				bool IsBIN = false;
				if (args.Length > 1 && args[1] == "-prs") IsPRS = true;
				string directoryName = Path.GetDirectoryName(filePath);
				string extension = Path.GetExtension(filePath).ToLowerInvariant();
				if (!File.Exists(filePath))
				{
					Console.WriteLine("Supplied archive/texture list does not exist!");
					Console.WriteLine("Press ENTER to exit.");
					Console.ReadLine();
					return;
				}
				switch (extension)
				{
					case ".bin":
						byte[] bindata = File.ReadAllBytes(Path.ChangeExtension(filePath, ".bin"));
						bindata = FraGag.Compression.Prs.Compress(bindata);
						File.WriteAllBytes(Path.ChangeExtension(filePath, ".prs"), bindata);
						Console.WriteLine("PRS archive was compiled successfully!");
						break;
					case ".txt":
						string archiveName = Path.GetFileNameWithoutExtension(filePath);
						List<string> textureNames = new List<string>(File.ReadAllLines(filePath).Where(a => !string.IsNullOrEmpty(a)));
						ArchiveBase pvmArchive;
						string ext = Path.GetExtension(textureNames[0]).ToLowerInvariant();
						if (textureNames.Any(a => !Path.GetExtension(a).Equals(ext, StringComparison.OrdinalIgnoreCase)))
						{
							Console.WriteLine("Cannot create archive from mixed file types.");
							Console.WriteLine("Press ENTER to exit.");
							Console.ReadLine();
							return;
						}
						switch (ext)
						{
							case ".pvr":
								pvmArchive = new PvmArchive();
								break;
							case ".gvr":
								pvmArchive = new GvmArchive();
								IsGVM = true;
								break;
							default:
								Console.WriteLine("Unknown file type \"{0}\".", ext);
								Console.WriteLine("Press ENTER to exit.");
								Console.ReadLine();
								return;
						}
						if (!IsGVM) ext = ".pvm"; else ext = ".gvm";
						using (Stream pvmStream = File.Open(Path.ChangeExtension(filePath, ext), FileMode.Create))
						{
							ArchiveWriter pvmWriter = pvmArchive.Create(pvmStream);
							// Reading in textures
							foreach (string tex in textureNames)
								if (!IsGVM) pvmWriter.CreateEntryFromFile(Path.Combine(directoryName, Path.ChangeExtension(tex, ".pvr")));
								else pvmWriter.CreateEntryFromFile(Path.Combine(directoryName, Path.ChangeExtension(tex, ".gvr")));
							pvmWriter.Flush();
						}
						if (IsPRS)
						{
							byte[] pvmdata = File.ReadAllBytes(Path.ChangeExtension(filePath, ext));
							pvmdata = FraGag.Compression.Prs.Compress(pvmdata);
							File.WriteAllBytes(Path.ChangeExtension(filePath, ".prs"), pvmdata);
							File.Delete(Path.ChangeExtension(filePath, ext));
						}
						Console.WriteLine("Archive was compiled successfully!");
						break;
					case ".prs":
					case ".pvm":
					case ".gvm":
						string path = Path.Combine(directoryName, Path.GetFileNameWithoutExtension(filePath));
						Directory.CreateDirectory(path);
						byte[] filedata = File.ReadAllBytes(filePath);
						using (TextWriter texList = File.CreateText(Path.Combine(path, Path.ChangeExtension(filePath, ".txt"))))
						{
							try
							{
								ArchiveBase pvmfile = null;
								byte[] pvmdata = File.ReadAllBytes(filePath);
								if (extension == ".prs") pvmdata = FraGag.Compression.Prs.Decompress(pvmdata);
								pvmfile = new PvmArchive();
								MemoryStream stream = new MemoryStream(pvmdata);
								if (!PvmArchive.Identify(stream))
								{
									pvmfile = new GvmArchive();
									if (!GvmArchive.Identify(stream))
									{
										File.WriteAllBytes(Path.ChangeExtension(filePath, ".bin"), pvmdata);
										IsBIN = true;
										Console.WriteLine("PRS archive extracted!");
									}
								}
								if (!IsBIN)
								{
									ArchiveReader pvmReader = pvmfile.Open(pvmdata);
									foreach (ArchiveEntry file in pvmReader.Entries)
									{
										texList.WriteLine(file.Name);
										pvmReader.ExtractToFile(file, Path.Combine(path, file.Name));
									}
									Console.WriteLine("Archive extracted!");
								}
							}
							catch
							{
								Console.WriteLine("Exception thrown. Canceling conversion.");
								Console.WriteLine("Press ENTER to exit.");
								Console.ReadLine();
								Directory.Delete(path, true);
								throw;
							}
						}
						if (IsBIN)
						{
							File.Delete(Path.Combine(path, Path.ChangeExtension(filePath, ".txt")));
							Directory.Delete(path, true);
						}
						break;
					default:
						Console.WriteLine("Unknown extension \"{0}\".", extension);
						Console.WriteLine("Press ENTER to exit.");
						Console.ReadLine();
						break;
				}
			}
		}
		static bool AddTexture(bool gvm, string path, string filename, Stream data, TextWriter index)
		{
			VrTexture vrfile = gvm ? (VrTexture)new GvrTexture(data) : (VrTexture)new PvrTexture(data);
			if (vrfile.NeedsExternalPalette)
			{
				Console.WriteLine("Cannot convert texture files which require external palettes!");
				return false;
			}
			Bitmap bmp;
			try { bmp = vrfile.ToBitmap(); }
			catch { bmp = new Bitmap(1, 1); }
			bmp.Save(Path.Combine(path, Path.ChangeExtension(filename, "png")));
			bmp.Dispose();
			index.WriteLine("{0},{1}", vrfile.HasGlobalIndex ? vrfile.GlobalIndex : uint.MaxValue, Path.ChangeExtension(filename, "png"));
			return true;
		}
	}
}
