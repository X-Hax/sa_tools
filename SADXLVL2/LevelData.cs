using System.Collections.Generic;
using System.Drawing;
using System.IO;
using PuyoTools;
using VrSharp.PvrTexture;
using Microsoft.DirectX.Direct3D;
using System;
using System.CodeDom.Compiler;

namespace SonicRetro.SAModel.SADXLVL2
{
    internal static class LevelData
    {
        public static MainForm MainForm;
        public static LandTable geo;
        public static string leveltexs;
        public static Dictionary<string, BMPInfo[]> TextureBitmaps;
        public static Dictionary<string, Texture[]> Textures;
        public static List<LevelItem> LevelItems;
        public static readonly string[] Characters = { "sonic", "tails", "knuckles", "amy", "gamma", "big" };
        public static readonly string[] SETChars = { "S", "M", "K", "A", "E", "B" };
        public static int Character;
        public static StartPosItem[] StartPositions;
        public static string SETName;
        public static List<ObjectDefinition> ObjDefs;
        public static List<SETItem>[] SETItems;
        public static List<DeathZoneItem> DeathZones;
        public static LevelDefinition leveleff;

        public static BMPInfo[] GetTextures(string filename)
        {
            List<BMPInfo> functionReturnValue = new List<BMPInfo>();
            PVM pvmfile = new PVM();
            Stream pvmdata = new MemoryStream(File.ReadAllBytes(filename));
            pvmdata = pvmfile.TranslateData(pvmdata);
            ArchiveFileList pvmentries = pvmfile.GetFileList(pvmdata);
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

        internal static void ChangeObjectType(SETItem entry)
        {
            Type t = ObjDefs[entry.ID].ObjectType;
            if (entry.GetType() == t) return;
            byte[] entb = entry.GetBytes();
            SETItem oe = (SETItem)Activator.CreateInstance(t, new object[] { entb, 0 });
            int i = SETItems[Character].IndexOf(entry);
            SETItems[Character][i] = oe;
            if (MainForm.SelectedItems != null)
            {
                i = MainForm.SelectedItems.IndexOf(entry);
                if (i > -1)
                {
                    MainForm.SelectedItems[i] = oe;
                    MainForm.SelectedItemChanged();
                }
            }
        }

        internal static SETItem CreateObject(ushort ID)
        {
            Type t = ObjDefs[ID].ObjectType;
            SETItem oe = (SETItem)Activator.CreateInstance(t, new object[] { });
            oe.ID = ID;
            return oe;
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