using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PuyoTools.Modules.Archive;

namespace ArchiveTool
{
	static class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("ArchiveTool is a command line tool to manipulate PVM, GVM and PRS archives.\n");
				Console.WriteLine("Usage:\n");
				Console.WriteLine("Extracting a PVM/GVM/PRS archive:\nArchiveTool file.pvm\nIf the archive is PRS compressed, it will be decompressed first.\nIf the archive contains textures, the program will produce a folder with all PVR/GVR textures and a texture list.\nThe texture list is a text file with each line containing a texture file name.\n");
				Console.WriteLine("Creating a PVM/GVM:\nArchiveTool texturelist.txt\nThe program will produce a PVM or GVM archive from a texture list.\nThe textures must be in the same folder as the texture list.\n");
				Console.WriteLine("Creating a PRS compressed PVM/GVM:\nArchiveTool texturelist.txt -prs\nSame as the previous option but the PVM/GVM file will be PRS compressed.\n");
				Console.WriteLine("Creating a PRS compressed binary:\nArchiveTool file.bin\nA PRS archive will be created from the file.\nFile extension must be .BIN for this option to work.\n");
				Console.WriteLine("Press ENTER to exit.");
				Console.ReadLine();
				return;
			}
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
							if (!pvmfile.Is(pvmdata, filePath))
							{
								pvmfile = new GvmArchive();
								if (!pvmfile.Is(pvmdata, filePath))
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
}
