using System.Collections.Generic;
using System.Drawing;
using System.IO;
using puyo_tools;
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

        internal static string ParseLevelAct(string levelact)
        {
            if (levelact.Contains(" "))
                return ((byte)Enum.Parse(typeof(LevelIDs), levelact.Split(' ')[0])).ToString(System.Globalization.NumberFormatInfo.InvariantInfo).PadLeft(2, '0') + levelact.Split(' ')[1].PadLeft(2, '0');
            else
                return levelact;
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

    enum LevelIDs : byte
    {
        HedgehogHammer = 0,
        EmeraldCoast = 1,
        WindyValley = 2,
        TwinklePark = 3,
        SpeedHighway = 4,
        RedMountain = 5,
        SkyDeck = 6,
        LostWorld = 7,
        IceCap = 8,
        Casinopolis = 9,
        FinalEgg = 0xA,
        HotShelter = 0xC,
        Chaos0 = 0xF,
        Chaos2 = 0x10,
        Chaos4 = 0x11,
        Chaos6 = 0x12,
        Chaos7 = 0x13,
        EggHornet = 0x14,
        EggWalker = 0x15,
        EggViper = 0x16,
        Zero = 0x17,
        E101 = 0x18,
        E101R = 0x19,
        StationSquare = 0x1A,
        EggCarrierOutside = 0x1D,
        EggCarrierInside = 0x20,
        MysticRuins = 0x21,
        Past = 0x22,
        TwinkleCircuit = 0x23,
        SkyChase1 = 0x24,
        SkyChase2 = 0x25,
        SandHill = 0x26,
        SSGarden = 0x27,
        ECGarden = 0x28,
        MRGarden = 0x29,
        ChaoRace = 0x2A,
        Invalid = 0x2B,
        Maximum = 0xFF
    }

    [Flags()]
    public enum CharacterFlags
    {
        Sonic = 1,
        Eggman = 2,
        Tails = 4,
        Knuckles = 8,
        Tikal = 0x10,
        Amy = 0x20,
        Gamma = 0x40,
        Big = 0x80
    }
}