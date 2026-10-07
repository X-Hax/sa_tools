using System.Collections.Generic;
using System.IO;
using System.Linq;
using ArchiveLib;
using PSO.PRS;
using TextureLib;
using static ArchiveLib.GenericArchive;

namespace SAModel.Direct3D.TextureSystem
{
    /// <summary>
    /// This TextureArchive class is the primary interface for retrieving a texture list/array from a container format, such as PVM/GVM, and eventually PAK.
	/// TODO: Rework to properly use GenericTexture.
    /// </summary>
    public static class TextureArchive
    {
		public static GenericTexture[] GetTextures(string filename, out bool hasNames, string paletteFile = null)
        {
			hasNames = true;
            if (!File.Exists(filename))
                return null;
            GenericArchive arc;
            List<GenericTexture> textures = new List<GenericTexture>();
            byte[] file = File.ReadAllBytes(filename);
            string ext = Path.GetExtension(filename).ToLowerInvariant();
            switch (ext)
            {
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
				// Folder texture pack
				case ".txt":
					arc = new PVMXFile(filename);
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
					List<GenericTexture> arr = new List<GenericTexture>();
					arr.Add(new GdiTexture(new System.Drawing.Bitmap(filename), name: Path.GetFileNameWithoutExtension(filename)));
					return arr.ToArray();
				case ".prs":
                    file = PRS.Decompress(file);
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
                textures.Add(GenericTexture.LoadTexture(entry.Data, name: Path.GetFileNameWithoutExtension(entry.Name)));
			}
			hasNames = arc.HasNameData;

			return textures.ToArray();
        }
    }
}