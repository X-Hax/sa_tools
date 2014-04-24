using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using PuyoTools;

using VrSharp.PvrTexture;

namespace SonicRetro.SAModel.Direct3D.TextureSystem
{
    /// <summary>
    /// This TextureArchive class is the primary interface for retrieving a texture list/array from a container format, such as PVM/GVM, and eventually PAK.
    /// </summary>
    public static class TextureArchive
    {
        public static BMPInfo[] GetTextures(string filename)
        {
            List<BMPInfo> functionReturnValue = new List<BMPInfo>();
            bool gvm = false;
            ArchiveModule pvmfile = null;
            Stream pvmdata = new MemoryStream(File.ReadAllBytes(filename));
            if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
            {
                Stream decomp = new MemoryStream();
                FraGag.Compression.Prs.Decompress(pvmdata, decomp);
                pvmdata.Dispose();
                pvmdata = decomp;
            }
            pvmfile = new PVM();
            if (!pvmfile.Check(pvmdata, filename))
            {
                pvmfile = new GVM();
                gvm = true;
            }
            VrSharp.VpClut pvp = null;
            pvmdata = pvmfile.TranslateData(pvmdata);
            ArchiveFileList pvmentries = pvmfile.GetFileList(pvmdata);
            foreach (ArchiveFileList.Entry file in pvmentries.Entries)
            {
                byte[] data = new byte[file.Length];
                pvmdata.Seek(file.Offset, SeekOrigin.Begin);
                pvmdata.Read(data, 0, (int)file.Length);
                VrSharp.VrTexture vrfile = gvm ? (VrSharp.VrTexture)new VrSharp.GvrTexture.GvrTexture(data) : (VrSharp.VrTexture)new VrSharp.PvrTexture.PvrTexture(data);
                if (vrfile.NeedsExternalClut())
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
                                pvp = gvm ? (VrSharp.VpClut)new VrSharp.GvrTexture.GvpClut(a.FileName) : (VrSharp.VpClut)new PvpClut(a.FileName);
                            else
                                return new BMPInfo[0];
                    }
                    vrfile.SetClut(pvp);
                }
                try
                {
                    functionReturnValue.Add(new BMPInfo(Path.GetFileNameWithoutExtension(file.Filename), vrfile.GetTextureAsBitmap()));
                }
                catch
                {
                    functionReturnValue.Add(new BMPInfo(Path.GetFileNameWithoutExtension(file.Filename), new Bitmap(1, 1)));
                }
            }
            pvmdata.Dispose();
            return functionReturnValue.ToArray();
        }
    }
}
