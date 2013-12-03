using System.Collections.Generic;
using System.Drawing;
using System.IO;
using PuyoTools;
using VrSharp.PvrTexture;
using Microsoft.DirectX.Direct3D;
using System;
using System.CodeDom.Compiler;

namespace SonicRetro.SAModel.SALVL
{
    internal static class LevelData
    {
        public static MainForm MainForm;
        public static LandTable geo;
        public static string leveltexs;
        public static Dictionary<string, BMPInfo[]> TextureBitmaps;
        public static Dictionary<string, Texture[]> Textures;
        public static List<LevelItem> LevelItems;

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

    internal class BMPInfo
    {
        public string Name { get; set; }
        public Bitmap Image { get; set; }

        public BMPInfo(string name, Bitmap image)
        {
            Name = name;
            Image = image;
        }
    }
}