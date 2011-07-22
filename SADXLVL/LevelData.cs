using System.Collections.Generic;
using System.Drawing;
using System.IO;
using puyo_tools;
using VrSharp.PvrTexture;
using Microsoft.DirectX.Direct3D;
using System;

namespace SonicRetro.SAModel.SADXLVL2
{
    internal static class LevelData
    {
        public static MainForm MainForm;
        public static LandTable geo;
        public static string leveltexs;
        public static Dictionary<string, Bitmap[]> TextureBitmaps;
        public static Dictionary<string, Texture[]> Textures;
        public static List<LevelItem> LevelItems;

        public static Bitmap[] GetTextures(string filename)
        {
            List<Bitmap> functionReturnValue = new List<Bitmap>();
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
                functionReturnValue.Add(vrfile.GetTextureAsBitmap());
            }
            return functionReturnValue.ToArray();
        }

        public static float BAMSToRad(int BAMS)
        {
            return (float)(BAMS / (65536 / (2 * Math.PI)));
        }

        public static int RadToBAMS(float rad)
        {
            return (int)(rad * (65536 / (2 * Math.PI)));
        }

        public static float BAMSToDeg(int BAMS)
        {
            return (float)(BAMS / (65536 / 360.0));
        }

        public static int DegToBAMS(float deg)
        {
            return (int)(deg * (65536 / 360.0));
        }
    }
}