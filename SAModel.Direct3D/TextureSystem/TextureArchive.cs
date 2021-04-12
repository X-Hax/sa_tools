using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using PuyoTools.Modules.Archive;
using VrSharp;
using VrSharp.Gvr;
using VrSharp.Pvr;
using ArchiveLib;
using System.Linq;
using System.Drawing.Imaging;

namespace SonicRetro.SAModel.Direct3D.TextureSystem
{
    /// <summary>
    /// This TextureArchive class is the primary interface for retrieving a texture list/array from a container format, such as PVM/GVM, and eventually PAK.
    /// </summary>
    public static class TextureArchive
    {
        public static BMPInfo[] GetTextures(string filename)
        {
            if (!File.Exists(filename)) return null;
            string ext = Path.GetExtension(filename).ToLowerInvariant();
            switch (ext)
            {
                case ".pak":
                    List<BMPInfo> newtextures;
                    PAKFile pak = new PAKFile(filename);
                    string filenoext = Path.GetFileNameWithoutExtension(filename).ToLowerInvariant();
                    // Check if PAK INF exists
                    bool inf_exists = false;
                    foreach (PAKFile.File entry in pak.Files)
                        if (entry.Name.Equals(filenoext + '\\' + filenoext + ".inf", StringComparison.OrdinalIgnoreCase))
                                inf_exists = true;
                    // Get texture names from PAK INF, if it exists
                    if (inf_exists)
                    {
                        byte[] inf = pak.Files.Single((file) => file.Name.Equals(filenoext + '\\' + filenoext + ".inf", StringComparison.OrdinalIgnoreCase)).Data;
                        newtextures = new List<BMPInfo>(inf.Length / 0x3C);
                        for (int i = 0; i < inf.Length; i += 0x3C)
                        {
                            System.Text.StringBuilder sb = new System.Text.StringBuilder(0x1C);
                            for (int j = 0; j < 0x1C; j++)
                                if (inf[i + j] != 0)
                                    sb.Append((char)inf[i + j]);
                                else
                                    break;
                            newtextures.Add(new BMPInfo(sb.ToString(), pak.Files.First((file) => file.Name.Equals(filenoext + '\\' + sb.ToString() + ".dds", StringComparison.OrdinalIgnoreCase)).GetBitmap()));
                        }
                        return newtextures.ToArray();
                    }
                    // Otherwise just get the textures directly
                    {
                        newtextures = new List<BMPInfo>(pak.Files.Count);
                        foreach (PAKFile.File entry in pak.Files)
                        {
                            // Only add files that can be converted to Bitmap
                            string extension = Path.GetExtension(entry.Name).ToLowerInvariant();
                            switch (extension)
                            {
                                case ".dds":
                                case ".png":
                                case ".bmp":
                                case ".gif":
                                case ".jpg":
                                    newtextures.Add(new BMPInfo(Path.GetFileNameWithoutExtension(entry.Name), entry.GetBitmap()));
                                    break;
                                default:
                                    break;
                            }
                        }
                        return newtextures.ToArray();
                    }
                case ".pvmx":
                    PVMXFile pvmx = new PVMXFile(File.ReadAllBytes(filename));
                    List<BMPInfo> textures = new List<BMPInfo>();
                    for (int i = 0; i < pvmx.GetCount(); i++)
                    {
                        var bmp = new Bitmap(new MemoryStream(pvmx.GetFile(i)));
                        textures.Add(new BMPInfo(pvmx.GetNameWithoutExtension(i), bmp));
                    }
                    return textures.ToArray();
                case ".txt":
                    string[] files = File.ReadAllLines(filename);
                    List<BMPInfo> txts = new List<BMPInfo>();
                    for (int s = 0; s < files.Length; s++)
                    {
                        string[] entry = files[s].Split(',');
                        txts.Add(new BMPInfo(entry[1], new System.Drawing.Bitmap(Path.Combine(Path.GetDirectoryName(filename), entry[1]))));
                    }
                    return txts.ToArray();
                case ".pb":
                    PBFile pbdata = new PBFile(File.ReadAllBytes(filename));
                    List<BMPInfo> txtsp = new List<BMPInfo>();
                    for (int i = 0; i < pbdata.GetCount(); i++)
                    {
                        PvrTexture pvr = new PvrTexture(pbdata.GetPVR(i));
                        txtsp.Add(new BMPInfo(i.ToString("D3"), pvr.ToBitmap()));
                    }
                    return txtsp.ToArray();
                case ".pvm":
                case ".gvm":
                default:
                    List<BMPInfo> functionReturnValue = new List<BMPInfo>();
                    bool gvm = false;
                    ArchiveBase pvmfile = null;
                    byte[] pvmdata = File.ReadAllBytes(filename);
                    if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
                        pvmdata = FraGag.Compression.Prs.Decompress(pvmdata);
                    pvmfile = new PvmArchive();
                    MemoryStream stream = new MemoryStream(pvmdata);
                    if (!PvmArchive.Identify(stream))
                    {
                        pvmfile = new GvmArchive();
                        gvm = true;
                    }
                    VrSharp.VpPalette pvp = null;
                    ArchiveEntryCollection pvmentries = pvmfile.Open(pvmdata).Entries;
                    foreach (ArchiveEntry file in pvmentries)
                    {
                        VrTexture vrfile = gvm ? (VrTexture)new GvrTexture(file.Open()) : (VrTexture)new PvrTexture(file.Open());
                        if (vrfile.NeedsExternalPalette)
                        {
                            using (System.Windows.Forms.OpenFileDialog a = new System.Windows.Forms.OpenFileDialog
                            {
                                DefaultExt = gvm ? "gvp" : "pvp",
                                Filter = gvm ? "GVP Files|*.gvp" : "PVP Files|*.pvp",
                                InitialDirectory = System.IO.Path.GetDirectoryName(filename),
                                Title = "External palette file"
                            })
                            {
                                if (pvp == null)
                                    if (a.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                        pvp = gvm ? (VpPalette)new GvpPalette(a.FileName) : (VpPalette)new PvpPalette(a.FileName);
                                    else
                                        return new BMPInfo[0];
                            }
                            if (gvm)
                                ((GvrTexture)vrfile).SetPalette((GvpPalette)pvp);
                            else
                                ((PvrTexture)vrfile).SetPalette((PvpPalette)pvp);
                        }
                        try
                        {
                            functionReturnValue.Add(new BMPInfo(Path.GetFileNameWithoutExtension(file.Name), vrfile.ToBitmap()));
                        }
                        catch
                        {
                            functionReturnValue.Add(new BMPInfo(Path.GetFileNameWithoutExtension(file.Name), new Bitmap(1, 1)));
                        }
                    }
                    return functionReturnValue.ToArray();
            }
        }
        enum dictionary_field : byte
        {
            none,
            /// <summary>
            /// 32-bit integer global index
            /// </summary>
            global_index,
            /// <summary>
            /// Null-terminated file name
            /// </summary>
            name,
            /// <summary>
            /// Two 32-bit integers defining width and height
            /// </summary>
            dimensions,
        }
    }
}