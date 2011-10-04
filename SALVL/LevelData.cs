using System.Collections.Generic;
using System.Drawing;
using System.IO;
using puyo_tools;
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
            PVM pvmfile = new PVM();
            Stream pvmdata = new MemoryStream(File.ReadAllBytes(filename));
            pvmdata = pvmfile.TranslateData(ref pvmdata);
            ArchiveFileList pvmentries = pvmfile.GetFileList(ref pvmdata);
            foreach (ArchiveFileList.Entry file in pvmentries.Entries)
            {
                byte[] data = new byte[file.Length];
                pvmdata.Seek(file.Offset, SeekOrigin.Begin);
                pvmdata.Read(data, 0, (int)file.Length);
                PvrTexture vrfile = new PvrTexture(data);
                functionReturnValue.Add(new BMPInfo(Path.GetFileNameWithoutExtension(file.Filename), vrfile.GetTextureAsBitmap()));
            }
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