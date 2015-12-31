using PuyoTools.Modules.Archive;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using VrSharp;
using VrSharp.GvrTexture;
using VrSharp.PvrTexture;

namespace PVM2TexPack
{
	class Program
	{
		static void Main(string[] args)
		{
			Queue<string> files = new Queue<string>(args);
			if (files.Count == 0)
			{
				Console.Write("File: ");
				files.Enqueue(Console.ReadLine());
			}
			while (files.Count > 0)
			{
				string filename = files.Dequeue();
				string path = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filename)), Path.GetFileNameWithoutExtension(filename));
				Directory.CreateDirectory(path);
				byte[] filedata = File.ReadAllBytes(filename);
				using (TextWriter texList = File.CreateText(Path.Combine(path, "index.txt")))
				{
					try
					{
						if (PvrTexture.Is(filedata))
						{
							if (!AddTexture(false, path, Path.GetFileName(filename), new MemoryStream(filedata), texList))
							{
								texList.Close();
								Directory.Delete(path, true);
							}
							continue;
						}
						else if (GvrTexture.Is(filedata))
						{
							if (!AddTexture(true, path, Path.GetFileName(filename), new MemoryStream(filedata), texList))
							{
								texList.Close();
								Directory.Delete(path, true);
							}
							continue;
						}
						bool gvm = false;
						ArchiveBase pvmfile = null;
						byte[] pvmdata = File.ReadAllBytes(filename);
						if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
							pvmdata = FraGag.Compression.Prs.Decompress(pvmdata);
						pvmfile = new PvmArchive();
						if (!pvmfile.Is(pvmdata, filename))
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
					catch
					{
						Console.WriteLine("Exception thrown. Canceling conversion.");
						Directory.Delete(path, true);
						throw;
					}
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