using System.Collections.Generic;
using System.IO;
using System.Linq;
using ArchiveLib;
using static ArchiveLib.GenericArchive;

namespace SAModel.Direct3D.TextureSystem
{
    /// <summary>
    /// This TextureArchive class is the primary interface for retrieving a texture list/array from a container format, such as PVM/GVM, and eventually PAK.
    /// </summary>
    public static class TextureArchive
    {
		public static BMPInfo[] GetTextures(string filename, out bool hasNames, string paletteFile = null)
        {
			hasNames = true;
            if (!File.Exists(filename))
                return null;
            GenericArchive arc;
            List<BMPInfo> textures = new List<BMPInfo>();
            byte[] file = File.ReadAllBytes(filename);
            string ext = Path.GetExtension(filename).ToLowerInvariant();
            switch (ext)
            {
                // Folder texture pack
                case ".txt":
                    string[] files = File.ReadAllLines(filename);
                    List<BMPInfo> txts = new List<BMPInfo>();
                    for (int s = 0; s < files.Length; s++)
                    {
                        string[] entry = files[s].Split(',');
                        txts.Add(new BMPInfo(Path.GetFileNameWithoutExtension(entry[1]), new System.Drawing.Bitmap(Path.Combine(Path.GetDirectoryName(filename), entry[1]))));
                    }
                    return txts.ToArray();
                case ".pak":
                    arc = new PAKFile(filename);
                    string filenoext = Path.GetFileNameWithoutExtension(filename).ToLowerInvariant();
                    PAKFile pak = (PAKFile)arc;
                    // Get sorted entires from the INF file if it exists
                    List<PAKFile.PAKEntry> sorted = pak.GetSortedEntries(filenoext);
                    arc.Entries = new List<GenericArchiveEntry>(sorted.Cast<GenericArchiveEntry>());
                    break;
                case ".pvmx":
                    arc = new PVMXFile(file);
                    break;
                case ".pb":
                    arc = new PBFile(file);
                    break;
                case ".pvr":
					arc = new PuyoFile(PuyoArchiveType.PVMFile);
					PuyoFile parcx = (PuyoFile)arc;
					arc.Entries.Add(new PVMEntry(filename));
					if (paletteFile != null)
						parcx.SetPalette(Path.Combine(Path.GetDirectoryName(filename), paletteFile));
					if (parcx.PaletteRequired)
						parcx.AddPaletteFromDialog(Path.GetDirectoryName(filename));
					break;
				case ".gvr":
					arc = new PuyoFile(PuyoArchiveType.GVMFile);
					PuyoFile garcx = (PuyoFile)arc;
					arc.Entries.Add(new GVMEntry(filename));
					if (paletteFile != null)
						garcx.SetPalette(Path.Combine(Path.GetDirectoryName(filename), paletteFile));
					if (garcx.PaletteRequired)
						garcx.AddPaletteFromDialog(Path.GetDirectoryName(filename));
					break;
				case ".xvr":
					arc = new PuyoFile(PuyoArchiveType.XVMFile);
					arc.Entries.Add(new XVMEntry(filename));
					break;
				case ".png":
				case ".jpg":
				case ".gif":
				case ".bmp":
					List<BMPInfo> arr = new List<BMPInfo>();
					arr.Add(new BMPInfo(Path.GetFileNameWithoutExtension(filename), new System.Drawing.Bitmap(filename)));
					return arr.ToArray();
				case ".prs":
                    file = FraGag.Compression.Prs.Decompress(file);
                    goto default;
                case ".pvm":
                case ".gvm":
				case ".xvm":
				default:
                    arc = new PuyoFile(file);
                    PuyoFile parc = (PuyoFile)arc;
					if (paletteFile != null)
						parc.SetPalette(Path.Combine(Path.GetDirectoryName(filename), paletteFile));
					if (parc.PaletteRequired)
                        parc.AddPaletteFromDialog(Path.GetDirectoryName(filename));
                    break;
            }
            foreach (GenericArchiveEntry entry in arc.Entries)
            {
                textures.Add(new BMPInfo(Path.GetFileNameWithoutExtension(entry.Name), entry.GetBitmap()));
			}
			hasNames = arc.hasNameData;

			return textures.ToArray();
        }
    }
}