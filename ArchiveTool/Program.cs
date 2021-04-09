using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using PuyoTools.Modules.Archive;
using SonicRetro.SAModel;
using VrSharp;
using VrSharp.Gvr;
using VrSharp.Pvr;

namespace ArchiveTool
{
	static class Program
	{
		static void Main(string[] args)
		{
			//Usage
			if (args.Length == 0)
			{
				Console.WriteLine("ArchiveTool is a command line tool to extract and create PVM, GVM, PRS, DAT and PB archives.\nIt can also decompress SADX Gamecube 'SaCompGC' REL files.\n");
				Console.WriteLine("Usage:\n");
				Console.WriteLine("Extracting a PVM/GVM/PRS/PB/DAT/REL file:\nArchiveTool <archivefile>\nIf the archive is PRS compressed, it will be decompressed first.\nIf the archive contains textures/sounds, the program will extract them and create a list of files named 'index.txt'.\n");
				Console.WriteLine("Converting PVM/GVM to a folder texture pack: ArchiveTool -png <archivefile>\n");
                Console.WriteLine("Creating a PB archive from a folder with textures: ArchiveTool -pb <foldername>");
                Console.WriteLine("Creating a PVM/GVM/DAT from a folder with textures/sounds: ArchiveTool <foldername> [-prs]\nThe program will create an archive from files listed in 'index.txt' in the folder.\nThe -prs option will make the program output a PRS compressed archive.\n");
                Console.WriteLine("Creating a PVM from PNG textures: ArchiveTool -pvm <folder> [-prs]\nThe texture list 'index.txt' must contain global indices listed before each texture filename for this option to work.\n");
				Console.WriteLine("Converting GVM to PVM (lossy): ArchiveTool -gvm2pvm <file.gvm> [-prs]\n");
				Console.WriteLine("Creating a PRS compressed binary: ArchiveTool <file.bin>\nFile extension must be .BIN for this option to work.\n");
				Console.WriteLine("Press ENTER to exit.");
				Console.ReadLine();
				return;
			}
			//GVM2PVM mode
			else if (args[0] == "-gvm2pvm")
			{
				string filePath = args[1];
				bool isPRS = false;
				if (args.Length > 2 && args[2] == "-prs") isPRS = true;
				Console.WriteLine("Converting GVM to PVM: {0}", filePath);
				string directoryName = Path.GetDirectoryName(filePath);
				string extension = Path.GetExtension(filePath).ToLowerInvariant();
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
				string path = Path.Combine(directoryName, Path.GetFileNameWithoutExtension(filePath));
				Directory.CreateDirectory(path);
				byte[] filedata = File.ReadAllBytes(filePath);
				using (TextWriter texList = File.CreateText(Path.Combine(path, Path.GetFileName(path)+".txt")))
				{
					try
					{
						ArchiveBase gvmfile = null;
						byte[] gvmdata = File.ReadAllBytes(filePath);
						gvmfile = new GvmArchive();
						ArchiveReader gvmReader = gvmfile.Open(gvmdata);
						Stream pvmStream = File.Open(Path.ChangeExtension(filePath, ".pvm"), FileMode.Create);
						ArchiveBase pvmArchive = new PvmArchive();
						ArchiveWriter pvmWriter = pvmArchive.Create(pvmStream);
						foreach (ArchiveEntry file in gvmReader.Entries)
						{
							if (!File.Exists(Path.Combine(path, file.Name)))
								gvmReader.ExtractToFile(file, Path.Combine(path, file.Name));
							Stream data = File.Open(Path.Combine(path, file.Name), FileMode.Open);
							VrTexture vrfile = new GvrTexture(data);
							Bitmap tempTexture = vrfile.ToBitmap();
							System.Drawing.Imaging.BitmapData bmpd = tempTexture.LockBits(new Rectangle(Point.Empty, tempTexture.Size), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
							int stride = bmpd.Stride;
							byte[] bits = new byte[Math.Abs(stride) * bmpd.Height];
							System.Runtime.InteropServices.Marshal.Copy(bmpd.Scan0, bits, 0, bits.Length);
							tempTexture.UnlockBits(bmpd);
							int tlevels = 0;
							string archiveName = Path.GetFileNameWithoutExtension(filePath);
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
							string pvrPath = Path.ChangeExtension(Path.Combine(path, file.Name), ".pvr");
							if (!File.Exists(pvrPath)) 
								encoder.Save(pvrPath);
							data.Close();
							File.Delete(Path.Combine(path, file.Name));
							pvmWriter.CreateEntryFromFile(pvrPath);
							texList.WriteLine(Path.GetFileName(pvrPath));
							Console.WriteLine("Adding texture {0}", pvrPath);
						}
						pvmWriter.Flush();
						pvmStream.Flush();
						pvmStream.Close();
						if (isPRS)
						{
							Console.WriteLine("Compressing to PRS...");
							byte[] pvmdata = File.ReadAllBytes(Path.ChangeExtension(filePath, ".pvm"));
							pvmdata = FraGag.Compression.Prs.Compress(pvmdata);
							File.WriteAllBytes(Path.ChangeExtension(filePath, ".PVM.PRS"), pvmdata);
							File.Delete(Path.ChangeExtension(filePath, ".PVM"));
						}
						Console.WriteLine("Archive converted!");
					}
					catch (Exception ex)
					{
						Console.WriteLine("Exception thrown: {0}", ex.ToString());
						Console.WriteLine("Press ENTER to exit.");
						Console.ReadLine();
						return;
					}
				}
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
				if (Directory.Exists(filePath))
				{
					Console.WriteLine("Converting texture pack to PVM: {0}", filePath);
					StreamReader texlistStream = File.OpenText(Path.Combine(filePath, "index.txt"));
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
						string texturePath = Path.Combine(filePath, Path.ChangeExtension(texturename, ".png"));
						if (File.Exists(texturePath))
						{
							Console.WriteLine("Adding texture: " + (texturePath));
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
						Console.WriteLine("Compressing to PRS...");
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
            //Create PB mode
            else if (args[0] == "-pb")
            {
                string filePath = args[1];
                Console.WriteLine("Building PB from folder: {0}", filePath);
                if (Directory.Exists(filePath))
                {
                    string indexfilename = Path.Combine(filePath, "index.txt");
                    if (!File.Exists(indexfilename))
                    {
                        Console.WriteLine("Supplied path does not have an index file.");
                        Console.WriteLine("Press ENTER to exit.");
                        Console.ReadLine();
                        return;
                    }
                    List<string> filenames = new List<string>(File.ReadAllLines(indexfilename).Where(a => !string.IsNullOrEmpty(a)));
                    PBArchive pba = new PBArchive(filenames.Count);
                    int l = 0;
                    foreach (string tex in filenames)
                    {
                        byte[] texbytes = File.ReadAllBytes(Path.Combine(filePath, tex));
                        pba.AddPVR(texbytes, l);
                        l++;
                    }
                    string path = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filePath)), Path.GetFileNameWithoutExtension(filePath));
                    string filename_full = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filePath)), Path.GetFileName(filePath) + ".pb");
                    Console.WriteLine("Output file: {0}", filename_full);
                    File.WriteAllBytes(filename_full, pba.GetBytes());
                }
                else
                {
                    Console.WriteLine("Supplied path does not exist.");
                    Console.WriteLine("Press ENTER to exit.");
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
					Console.WriteLine("Converting file to texture pack: {0}", filename_full);
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
						Console.WriteLine("Conversion complete!");
					}
				}
			}
			//Other modes
			else
			{
				string filePath = args[0];
				bool IsPRS = false;
				bool IsGVM = false;
				bool IsBIN = false;
				bool isDAT = false;
				if (args.Length > 1 && args[1] == "-prs") IsPRS = true;
				string extension = Path.GetExtension(filePath).ToLowerInvariant();
				//Folder mode
				if (Directory.Exists(filePath))
				{
					string indexfilename = Path.Combine(filePath, "index.txt");
					List<string> filenames = new List<string>(File.ReadAllLines(indexfilename).Where(a => !string.IsNullOrEmpty(a)));
					ArchiveBase pvmArchive;
					string ext = Path.GetExtension(filenames[0]).ToLowerInvariant();
					if (filenames.Any(a => !Path.GetExtension(a).Equals(ext, StringComparison.OrdinalIgnoreCase)))
					{
						Console.WriteLine("Cannot create archive from mixed file types.");
						Console.WriteLine("Press ENTER to exit.");
						Console.ReadLine();
						return;
					}
					switch (ext)
					{
						case ".pvr":
							Console.WriteLine("Creating PVM archive from folder: {0}", filePath);
							pvmArchive = new PvmArchive();
							break;
						case ".gvr":
							Console.WriteLine("Creating GVM archive from folder: {0}", filePath);
							pvmArchive = new GvmArchive();
							IsGVM = true;
							break;
						case ".wav":
							Console.WriteLine("Creating DAT archive from folder: {0}", filePath);
							isDAT = true;
							pvmArchive = null;
							break;
						default:
							Console.WriteLine("Unknown file type \"{0}\".", ext);
							Console.WriteLine("Press ENTER to exit.");
							Console.ReadLine();
							return;
					}
					if (isDAT)
					{
						//Load
						List<FENTRY> files = new List<FENTRY>();
						TextReader tr = File.OpenText(Path.Combine(filePath, "index.txt"));
						string line = tr.ReadLine();
						while (line != null)
						{
							Console.WriteLine("Adding file {0}", Path.Combine(filePath, line));
							FENTRY newentry = new FENTRY();
							newentry.name = line;
							newentry.file = File.ReadAllBytes(Path.Combine(filePath, newentry.name));
							files.Add(newentry);
							line = tr.ReadLine();
						}
						tr.Close();
						//Save
						int fsize = 0x14;
						int hloc = fsize;
						fsize += files.Count * 0xC;
						int tloc = fsize;
						foreach (FENTRY item in files)
						{
							fsize += item.name.Length + 1;
						}
						int floc = fsize;
						foreach (FENTRY item in files)
						{
							fsize += item.file.Length;
						}
						byte[] file = new byte[fsize];
						System.Text.Encoding.ASCII.GetBytes("archive  V2.2").CopyTo(file, 0);
						BitConverter.GetBytes(files.Count).CopyTo(file, 0x10);
						foreach (FENTRY item in files)
						{
							BitConverter.GetBytes(tloc).CopyTo(file, hloc);
							hloc += 4;
							System.Text.Encoding.ASCII.GetBytes(item.name).CopyTo(file, tloc);
							tloc += item.name.Length + 1;
							BitConverter.GetBytes(floc).CopyTo(file, hloc);
							hloc += 4;
							item.file.CopyTo(file, floc);
							floc += item.file.Length;
							BitConverter.GetBytes(item.file.Length).CopyTo(file, hloc);
							hloc += 4;
						}
						File.WriteAllBytes(filePath + ".DAT", file);
						if (IsPRS)
						{
							Console.WriteLine("Compressing to PRS...");
							byte[] datdata = File.ReadAllBytes(filePath + ".DAT");
							datdata = FraGag.Compression.Prs.Compress(datdata);
							File.WriteAllBytes(filePath + ".PRS", datdata);
							File.Delete(filePath + ".DAT");
						}
						Console.WriteLine("Archive compiled successfully!");
						return;
					}
					else
					{
						if (!IsGVM) ext = ".pvm"; else ext = ".gvm";
						using (Stream pvmStream = File.Open(Path.ChangeExtension(filePath, ext), FileMode.Create))
						{
							ArchiveWriter pvmWriter = pvmArchive.Create(pvmStream);
							// Reading in textures
							foreach (string tex in filenames)
							{
								if (!IsGVM) pvmWriter.CreateEntryFromFile(Path.Combine(filePath, Path.ChangeExtension(tex, ".pvr")));
								else pvmWriter.CreateEntryFromFile(Path.Combine(filePath, Path.ChangeExtension(tex, ".gvr")));
								Console.WriteLine("Adding file: {0}", tex);
							}
							pvmWriter.Flush();
						}
					}
					if (IsPRS)
					{
						Console.WriteLine("Compressing to PRS...");
						byte[] pvmdata = File.ReadAllBytes(Path.ChangeExtension(filePath, ext));
						pvmdata = FraGag.Compression.Prs.Compress(pvmdata);
						File.WriteAllBytes(Path.ChangeExtension(filePath, ".prs"), pvmdata);
						File.Delete(Path.ChangeExtension(filePath, ext));
					}
					Console.WriteLine("Archive was compiled successfully!");
					return;
				}
				//Continue with file mode otherwise
				if (!File.Exists(filePath))
				{
					Console.WriteLine("Supplied archive/texture list does not exist!");
					Console.WriteLine("Press ENTER to exit.");
					Console.ReadLine();
					return;
				}
				switch (extension)
				{
					case ".rel":
						Console.WriteLine("Decompressing REL file: {0}", filePath);
						byte[] input = File.ReadAllBytes(args[0]);
						byte[] output = SA_Tools.HelperFunctions.DecompressREL(input);
						File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + "_dec.rel"), output);
						return;
					case ".dat":
						Console.WriteLine("Extracting DAT file: {0}", filePath);
						bool sndbank_steam = false;
						List<FENTRY> files = new List<FENTRY>();
						byte[] file = File.ReadAllBytes(filePath);
						switch (System.Text.Encoding.ASCII.GetString(file, 0, 0x10))
						{
							case "archive  V2.2\0\0\0":
								sndbank_steam = false;
								break;
							case "archive  V2.DMZ\0":
								sndbank_steam = true;
								break;
							default:
								Console.WriteLine("Error: Unknown archive version/type");
								return;
						}
						int count = BitConverter.ToInt32(file, 0x10);
						files = new List<FENTRY>(count);
						for (int i = 0; i < count; i++)
						{
							files.Add(new FENTRY(file, 0x14 + (i * 0xC)));
						}
						string dir = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));
						if (Directory.Exists(dir)) Directory.Delete(dir, true);
						Directory.CreateDirectory(dir);
						using (StreamWriter sw = File.CreateText(Path.Combine(dir, "index.txt")))
						{
							List<FENTRY> list = new List<FENTRY>(files);
							list.Sort((f1, f2) => StringComparer.OrdinalIgnoreCase.Compare(f1.name, f2.name));
							foreach (FENTRY item in list)
							{
								sw.WriteLine(item.name);
								string fname = item.name;
								if (sndbank_steam)
								{
									fname = Path.GetFileNameWithoutExtension(fname) + ".adx";
								}
								Console.WriteLine("Extracting file: {0}", fname);
								File.WriteAllBytes(Path.Combine(dir, fname), Compress.ProcessBuffer(item.file));
							}
							sw.Flush();
							sw.Close();
						}
						Console.WriteLine("Archive extracted!");
						break;
					case ".pb":
						Console.WriteLine("Extracting PB file: {0}", filePath);
						byte[] pbdata = File.ReadAllBytes(Path.ChangeExtension(filePath, ".pb"));
						dir = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));
						Directory.CreateDirectory(dir);
                        PBArchive pba = new PBArchive(pbdata);
                        using (TextWriter texList = File.CreateText(Path.Combine(dir, "index.txt")))
                        {
                            for (int u = 0; u < pba.Headers.Count; u++)
                            {
                                byte[] pvrt = pba.GetPVR(u);
                                string outpath = Path.Combine(dir, u.ToString("D3") + ".pvr");
                                File.WriteAllBytes(outpath, pvrt);
                                texList.WriteLine(u.ToString("D3") + ".pvr");
                            }
                            texList.Flush();
                            texList.Close();
                        }
						Console.WriteLine("Archive extracted!");
						break;
					case ".bin":
						Console.WriteLine("Compressing BIN file: {0}", filePath);
						byte[] bindata = File.ReadAllBytes(Path.ChangeExtension(filePath, ".bin"));
						bindata = FraGag.Compression.Prs.Compress(bindata);
						File.WriteAllBytes(Path.ChangeExtension(filePath, ".prs"), bindata);
						Console.WriteLine("PRS archive was compiled successfully!");
						return;
					case ".prs":
					case ".pvm":
					case ".gvm":
						Console.WriteLine("Extracting archive: {0}", filePath);
						string path = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));
						Directory.CreateDirectory(path);
						byte[] filedata = File.ReadAllBytes(filePath);
						using (TextWriter texList = File.CreateText(Path.Combine(path, "index.txt")))
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
									foreach (ArchiveEntry pvmentry in pvmReader.Entries)
									{
										Console.WriteLine("Extracting file: {0}", pvmentry.Name);
										texList.WriteLine(pvmentry.Name);
										pvmReader.ExtractToFile(pvmentry, Path.Combine(path, pvmentry.Name));
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
			Console.WriteLine("Adding texture: {0}", filename);
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

		internal class FENTRY
		{
			public string name;
			public byte[] file;

			public FENTRY()
			{
				name = string.Empty;
			}

			public FENTRY(string fileName)
			{
				name = Path.GetFileName(fileName);
				file = File.ReadAllBytes(fileName);
			}

			public FENTRY(byte[] file, int address)
			{
				name = GetCString(file, BitConverter.ToInt32(file, address));
				this.file = new byte[BitConverter.ToInt32(file, address + 8)];
				Array.Copy(file, BitConverter.ToInt32(file, address + 4), this.file, 0, this.file.Length);
			}

			private string GetCString(byte[] file, int address)
			{
				int textsize = 0;
				while (file[address + textsize] > 0)
					textsize += 1;
				return System.Text.Encoding.ASCII.GetString(file, address, textsize);
			}
		}

		internal static class Compress
		{
			const uint SLIDING_LEN = 0x1000;
			const uint SLIDING_MASK = 0xFFF;

			const byte NIBBLE_HIGH = 0xF0;
			const byte NIBBLE_LOW = 0x0F;

			//TODO: Documentation
			struct OffsetLengthPair
			{
				public byte highByte, lowByte;

				//TODO: Set
				public int Offset
				{
					get
					{
						return ((lowByte & NIBBLE_HIGH) << 4) | highByte;
					}
				}

				//TODO: Set
				public int Length
				{
					get
					{
						return (lowByte & NIBBLE_LOW) + 3;
					}
				}
			}

			//TODO: Documentation
			struct ChunkHeader
			{
				private byte flags;
				private byte mask;

				// TODO: Documentation
				public bool ReadFlag(out bool flag)
				{
					bool endOfHeader = mask != 0x00;

					flag = (flags & mask) != 0;

					mask <<= 1;
					return endOfHeader;
				}

				public ChunkHeader(byte flags)
				{
					this.flags = flags;
					this.mask = 0x01;
				}
			}

			//TODO:
			private static void CompressBuffer(byte[] compBuf, byte[] decompBuf /*Starting at + 20*/)
			{

			}

			// Decompresses a Lempel-Ziv buffer.
			// TODO: Add documentation
			private static void DecompressBuffer(byte[] decompBuf, byte[] compBuf /*Starting at + 20*/)
			{
				OffsetLengthPair olPair = new OffsetLengthPair();

				int compBufPtr = 0;
				int decompBufPtr = 0;

				//Create sliding dictionary buffer and clear first 4078 bytes of dictionary buffer to 0
				byte[] slidingDict = new byte[SLIDING_LEN];

				//Set an offset to the dictionary insertion point
				uint dictInsertionOffset = SLIDING_LEN - 18;

				// Current chunk header
				ChunkHeader chunkHeader = new ChunkHeader();

				while (decompBufPtr < decompBuf.Length)
				{
					// At the start of each chunk...
					if (!chunkHeader.ReadFlag(out bool flag))
					{
						// Load the chunk header
						chunkHeader = new ChunkHeader(compBuf[compBufPtr++]);
						chunkHeader.ReadFlag(out flag);
					}

					// Each chunk header is a byte and is a collection of 8 flags

					// If the flag is set, load a character
					if (flag)
					{
						// Copy the character
						byte rawByte = compBuf[compBufPtr++];
						decompBuf[decompBufPtr++] = rawByte;

						// Add the character to the dictionary, and slide the dictionary
						slidingDict[dictInsertionOffset++] = rawByte;
						dictInsertionOffset &= SLIDING_MASK;

					}
					// If the flag is clear, load an offset/length pair
					else
					{
						// Load the offset/length pair
						olPair.highByte = compBuf[compBufPtr++];
						olPair.lowByte = compBuf[compBufPtr++];

						// Get the offset from the offset/length pair
						int offset = olPair.Offset;

						// Get the length from the offset/length pair
						int length = olPair.Length;

						for (int i = 0; i < length; i++)
						{
							byte rawByte = slidingDict[(offset + i) & SLIDING_MASK];
							decompBuf[decompBufPtr++] = rawByte;

							if (decompBufPtr >= decompBuf.Length) return;

							// Add the character to the dictionary, and slide the dictionary
							slidingDict[dictInsertionOffset++] = rawByte;
							dictInsertionOffset &= SLIDING_MASK;
						}
					}
				}
			}

			public static bool isFileCompressed(byte[] CompressedBuffer)
			{
				return System.Text.Encoding.ASCII.GetString(CompressedBuffer, 0, 13) == "compress v1.0";
			}

			public static byte[] ProcessBuffer(byte[] CompressedBuffer)
			{
				if (isFileCompressed(CompressedBuffer))
				{
					uint DecompressedSize = BitConverter.ToUInt32(CompressedBuffer, 16);
					byte[] DecompressedBuffer = new byte[DecompressedSize];
					//Xor Decrypt the whole buffer
					byte XorEncryptionValue = CompressedBuffer[15];

					byte[] CompBuf = new byte[CompressedBuffer.Length - 20];
					for (int i = 20; i < CompressedBuffer.Length; i++)
					{
						CompBuf[i - 20] = (byte)(CompressedBuffer[i] ^ XorEncryptionValue);
					}

					//Decompress the whole buffer
					DecompressBuffer(DecompressedBuffer, CompBuf);

					//Switch the buffers around so the decompressed one gets saved instead
					return DecompressedBuffer;
				}
				else
				{
					return CompressedBuffer;
				}
			}
		}

        internal class PBTextureHeader
        {
            public int Offset { get; set; }
            public PvrPixelFormat PixelFormat { get; set; }
            public PvrDataFormat DataFormat { get; set; }
            public uint GBIX { get; set; }
            public ushort Width { get; set; }
            public ushort Height { get; set; }

            public byte[] GetBytes()
            {
                List<byte> result = new List<byte>();
                result.AddRange(ByteConverter.GetBytes(Offset));
                result.Add((byte)PixelFormat);
                result.Add((byte)DataFormat);
                result.Add(0);
                result.Add(0);
                result.AddRange(ByteConverter.GetBytes(GBIX));
                result.AddRange(ByteConverter.GetBytes(Width));
                result.AddRange(ByteConverter.GetBytes(Height));
                return result.ToArray();
            }

            public PBTextureHeader(byte[] pbdata, int tempaddr)
            {
                Offset = ByteConverter.ToInt32(pbdata, tempaddr);
                PixelFormat = (PvrPixelFormat)pbdata[tempaddr + 4];
                DataFormat = (PvrDataFormat)pbdata[tempaddr + 5];
                GBIX = ByteConverter.ToUInt32(pbdata, tempaddr + 8);
                Width = ByteConverter.ToUInt16(pbdata, tempaddr + 12);
                Height = ByteConverter.ToUInt16(pbdata, tempaddr + 12);
            }

            public PBTextureHeader(int offset, PvrPixelFormat pxformat, PvrDataFormat dataformat, uint gbix, ushort width, ushort height)
            {
                Offset = offset;
                PixelFormat = pxformat;
                DataFormat = dataformat;
                GBIX = gbix;
                Width = width;
                Height = height;
            }
        }

        public class PBArchive
        {
            public List<PBTextureHeader> Headers;
            public List<byte[]> Data;

            public PBArchive(byte[] pbdata)
            {
                Headers = new List<PBTextureHeader>();
                Data = new List<byte[]>();
                int numtextures = pbdata[4];
                for (int u = 0; u < numtextures; u++)
                {
                    PBTextureHeader hdr = new PBTextureHeader(pbdata, 8 + 16 * u);
                    Headers.Add(hdr);
                    //Console.WriteLine("Added header {0}: offset {1}, pixel format {2}, data format {3}, GBIX {4}, width {5}, height {6}", u, hdr.Offset, hdr.PixelFormat, hdr.DataFormat, hdr.GBIX, hdr.Width, hdr.Height);
                }
                PBTextureHeader[] headers = Headers.ToArray();
                for (int u = 0; u < numtextures; u++)
                {
                    int chunksize;
                    if (u == numtextures - 1) chunksize = pbdata.Length - headers[u].Offset;
                    else chunksize = headers[u + 1].Offset - headers[u].Offset;
                    byte[] pbchunk = new byte[chunksize];
                    Array.Copy(pbdata, headers[u].Offset, pbchunk, 0, chunksize);
                    Data.Add(pbchunk);
                    //Console.WriteLine("Added data: offset {0}, length {1}", headers[u].Offset, pbchunk.Length);
                }
            }

            public PBArchive(int count)
            {
                Headers = new List<PBTextureHeader>(count);
                Data = new List<byte[]>(count);
            }

            private int GetCurrentOffset(int index)
            {
                int offset_base = 8 + 16 * Headers.Capacity;
                if (index == 0)
                    return offset_base;
                for (int u = 0; u < index; u++)
                {
                    offset_base += Data[u].Length;
                }
                return offset_base;
            }

            public void AddPVR(byte[] pvrdata, int index)
            {
                int length = ByteConverter.ToInt32(pvrdata, 20) - 8;
                int offset = GetCurrentOffset(index);
                PvrTexture pvr = new PvrTexture(pvrdata);
                Headers.Add(new PBTextureHeader(offset, pvr.PixelFormat, pvr.DataFormat, pvr.GlobalIndex, pvr.TextureWidth, pvr.TextureHeight));
                byte[] pvrdata_nohdr = new byte[length];
                Array.Copy(pvrdata, 32, pvrdata_nohdr, 0, length);
                //Console.WriteLine("Adding texture {0} at offset {1}, length {2} (original PVR {3})", index, offset, length, pvrdata.Length);
                Data.Add(pvrdata_nohdr);
            }

            public byte[] GetPVR(int index)
            {
                List<byte> result = new List<byte>();
                int chunksize_file = Data[index].Length;
                // Make chunk size divisible by 16 because it crashes otherwise
                if (chunksize_file % 16 != 0)
                {
                    do
                    {
                        chunksize_file++;
                    }
                    while (chunksize_file % 16 != 0);
                }
                byte[] gbixheader = { 0x47, 0x42, 0x49, 0x58 };
                byte[] pvrtheader = { 0x50, 0x56, 0x52, 0x54 };
                byte[] padding = { 0x20, 0x20, 0x20, 0x20 };
                result.AddRange(gbixheader);
                result.AddRange(ByteConverter.GetBytes(8));
                result.AddRange(ByteConverter.GetBytes(Headers[index].GBIX));
                result.AddRange(padding);
                result.AddRange(pvrtheader);
                result.AddRange(ByteConverter.GetBytes(chunksize_file + 8));
                result.Add((byte)Headers[index].PixelFormat);
                result.Add((byte)Headers[index].DataFormat);
                result.Add(0);
                result.Add(0);
                result.AddRange(ByteConverter.GetBytes(Headers[index].Width));
                result.AddRange(ByteConverter.GetBytes(Headers[index].Height));
                result.AddRange(Data[index]);
                int pd = 0;
                // Make file size divisible by 16 because it crashes otherwise
                if (result.Count % 16 != 0)
                {
                    do
                    {
                        result.Add(0);
                        pd++;
                    }
                    while (result.Count % 16 != 0);
                }
                return result.ToArray();
            }

            public byte[] GetBytes()
            {
                List<byte> result = new List<byte>();
                result.Add(0x50); // P
                result.Add(0x56); // B
                result.Add(0x42); // V
                result.Add(0x02); // Version ID
                result.AddRange(ByteConverter.GetBytes((uint)Headers.Count));
                for (int u = 0; u < Headers.Count; u++)
                {
                    result.AddRange(Headers[u].GetBytes());
                }
                for (int u = 0; u < Data.Count; u++)
                {
                    result.AddRange(Data[u]);
                }
                return result.ToArray();
            }
        }
    }
}
