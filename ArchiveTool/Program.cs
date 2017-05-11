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
				Console.WriteLine("Error - please specify a texture list (.txt), a PVM archive (.pvm), or a GVM archive (.gvm).");
				Console.WriteLine("Press ENTER to continue...");
				Console.ReadLine();
				return;
			}
			string filePath = args[0];
			string directoryName = Path.GetDirectoryName(filePath);
			string extension = Path.GetExtension(filePath).ToLowerInvariant();
			switch (extension)
			{
				case ".txt":
					string archiveName = Path.GetFileNameWithoutExtension(filePath);
					if (File.Exists(filePath))
					{
						List<string> textureNames = new List<string>(File.ReadAllLines(filePath).Where(a => !string.IsNullOrEmpty(a)));
						ArchiveBase pvmArchive;
						string ext = Path.GetExtension(textureNames[0]).ToLowerInvariant();
						if (textureNames.Any(a => !Path.GetExtension(a).Equals(ext, StringComparison.OrdinalIgnoreCase)))
						{
							Console.WriteLine("Cannot create archive from mixed file types.");
							return;
						}
						switch (ext)
						{
							case ".pvr":
								pvmArchive = new PvmArchive();
								break;
							case ".gvr":
								pvmArchive = new GvmArchive();
								break;
							default:
								Console.WriteLine("Unknown file type \"{0}\".", ext);
								return;
						}
						using (Stream pvmStream = File.Open(Path.ChangeExtension(filePath, ".pvm"), FileMode.Create))
						{
							ArchiveWriter pvmWriter = pvmArchive.Create(pvmStream);
							// Reading in textures
							foreach (string tex in textureNames)
								pvmWriter.CreateEntryFromFile(Path.Combine(directoryName, Path.ChangeExtension(tex, ".pvr")));
							pvmWriter.Flush();
						}
						Console.WriteLine("PVM was compiled successfully!");
					}
					else // error, supplied path is invalid
					{
						Console.WriteLine("Supplied texture list/PVM does not exist!");
						Console.WriteLine("Press ENTER to continue...");
						Console.ReadLine();
					}
					break;
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
							pvmfile = new PvmArchive();
							if (!pvmfile.Is(pvmdata, filePath))
								pvmfile = new GvmArchive();
							ArchiveReader pvmReader = pvmfile.Open(pvmdata);
							foreach (ArchiveEntry file in pvmReader.Entries)
							{
								texList.WriteLine(file.Name);
								pvmReader.ExtractToFile(file, Path.Combine(path, file.Name));
							}
							Console.WriteLine("Archive extracted!");
						}
						catch
						{
							Console.WriteLine("Exception thrown. Canceling conversion.");
							Directory.Delete(path, true);
							throw;
						}
					}
					break;
				default:
					Console.WriteLine("Unknown extension \"{0}\".", extension);
					break;
			}
		}
	}
}