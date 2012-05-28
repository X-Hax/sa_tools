using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace SADXPCTools
{
    public class IniData
    {
        [IniIgnore]
        public uint ImageBase { get; set; }
        [IniName("key")]
        public string ImageBaseString { get { return ImageBase.ToString("X"); } set { ImageBase = uint.Parse(value, NumberStyles.HexNumber); } }
        [IniName("systemfolder")]
        [DefaultValue("system")]
        public string SystemFolder { get; set; }
        [IniName("musicfolder")]
        [DefaultValue("system/sounddata/bgm/wma")]
        public string MusicFolder { get; set; }
        [IniName("soundfolder")]
        [DefaultValue("system/sounddata/se")]
        public string SoundFolder { get; set; }
        [IniName("voicefolder")]
        [DefaultValue("system/sounddata/voice_us/wma")]
        public string VoiceFolder { get; set; }
        [IniCollection]
        public Dictionary<string, FileInfo> Files { get; set; }
    }

    public class FileInfo
    {
        [IniName("type")]
        public string Type { get; set; }
        [IniIgnore]
        public int Address { get; set; }
        [IniName("address")]
        public string AddressString { get { return Address.ToString("X"); } set { Address = int.Parse(value, NumberStyles.HexNumber); } }
        [IniName("filename")]
        public string Filename { get; set; }
        [IniName("nohash")]
        public bool NoHash { get; set; }
        [IniName("md5")]
        public string MD5Hash { get; set; }
        [IniIgnore]
        public int[] PointerList { get; set; }
        [IniName("pointer")]
        public string PointerListString
        {
            get
            {
                if (PointerList.Length == 0) return null;
                string[] result = new string[PointerList.Length];
                for (int i = 0; i < PointerList.Length; i++)
                    result[i] = PointerList[i].ToString("X");
                return string.Join(",", result);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    PointerList = new int[0];
                    return;
                }
                string[] data = value.Split(',');
                PointerList = new int[data.Length];
                for (int i = 0; i < data.Length; i++)
                    PointerList[i] = int.Parse(data[i], NumberStyles.HexNumber);
            }
        }
        [IniCollection]
        public Dictionary<string, string> CustomProperties { get; set; }
    }

    [Serializable]
    [TypeConverter(typeof(StringConverter<LevelAct>))]
    public struct LevelAct : IComparable, IComparable<LevelAct>, IComparable<ushort>, IEquatable<LevelAct>
    {
        public LevelAct(LevelIDs level, int act) { Level = level; Act = (byte)act; }
        public LevelAct(int level, int act) { Level = (LevelIDs)level; Act = (byte)act; }
        public LevelAct(ushort levelact) { Level = (LevelIDs)(levelact >> 8); Act = (byte)(levelact & 0xFF); }
        public LevelAct(string levelact)
        {
            if (levelact.Contains(" "))
            {
                Level = (LevelIDs)Enum.Parse(typeof(LevelIDs), levelact.Split(' ')[0]);
                Act = byte.Parse(levelact.Split(' ')[1], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
            }
            else
            {
                Level = (LevelIDs)byte.Parse(levelact.Substring(0, 2), NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                Act = byte.Parse(levelact.Substring(2, 2), NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
            }
        }

        public LevelIDs Level;
        public byte Act;
        public ushort LevelAndAct { get { return (ushort)((byte)Level << 8 | Act); } set { Level = (LevelIDs)(value >> 8); Act = (byte)(value & 0xFF); } }

        public override string ToString()
        {
            return Level.ToString() + " " + Act.ToString(NumberFormatInfo.InvariantInfo);
        }

        public int CompareTo(object obj)
        {
            if (obj is LevelAct)
                return CompareTo((LevelAct)obj);
            else
                return LevelAndAct.CompareTo(obj);
        }

        public int CompareTo(LevelAct other)
        {
            return LevelAndAct.CompareTo(other.LevelAndAct);
        }

        public int CompareTo(ushort other)
        {
            return LevelAndAct.CompareTo(other);
        }

        public override bool Equals(object obj)
        {
            if (obj is LevelAct)
                return Equals((LevelAct)obj);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return LevelAndAct.GetHashCode();
        }

        public bool Equals(LevelAct other)
        {
            return LevelAndAct.Equals(other.LevelAndAct);
        }

        public static explicit operator ushort(LevelAct obj)
        {
            return obj.LevelAndAct;
        }

        public static explicit operator LevelAct(ushort obj)
        {
            return new LevelAct(obj);
        }

        public static bool operator ==(LevelAct a, LevelAct b)
        {
            return a.LevelAndAct == b.LevelAndAct;
        }

        public static bool operator !=(LevelAct a, LevelAct b)
        {
            return a.LevelAndAct != b.LevelAndAct;
        }

        public static bool operator <(LevelAct a, LevelAct b)
        {
            return a.LevelAndAct < b.LevelAndAct;
        }

        public static bool operator >(LevelAct a, LevelAct b)
        {
            return a.LevelAndAct > b.LevelAndAct;
        }

        public static bool operator <=(LevelAct a, LevelAct b)
        {
            return a.LevelAndAct <= b.LevelAndAct;
        }

        public static bool operator >=(LevelAct a, LevelAct b)
        {
            return a.LevelAndAct >= b.LevelAndAct;
        }
    }

    public static class ObjectList
    {
        public static ObjectListEntry[] Load(string filename)
        {
            return IniFile.Deserialize<ObjectListEntry[]>(filename);
        }

        public static ObjectListEntry[] Load(byte[] file, int address, uint imageBase)
        {
            int numobjs = BitConverter.ToInt32(file, address);
            address = (int)(BitConverter.ToUInt32(file, address + 4) - imageBase);
            List<ObjectListEntry> objini = new List<ObjectListEntry>(numobjs);
            for (int i = 0; i < numobjs; i++)
            {
                objini.Add(new ObjectListEntry(file, address, imageBase));
                address += ObjectListEntry.Size;
            }
            return objini.ToArray();
        }

        public static void Save(this ObjectListEntry[] objlist, string filename)
        {
            IniFile.Serialize(objlist, filename);
        }

        public static byte[] GetBytes(this ObjectListEntry[] objlist, uint imageBase, Dictionary<string, uint> labels, out uint dataaddr)
        {
            List<byte> datasection = new List<byte>();
            List<byte> objents = new List<byte>();
            for (int i = 0; i < objlist.Length; i++)
            {
                objents.AddRange(objlist[i].GetBytes(labels, imageBase + (uint)datasection.Count));
                datasection.AddRange(HelperFunctions.GetEncoding().GetBytes(objlist[i].Name));
                datasection.Add(0);
                datasection.Align(4);
            }
            uint objentaddr = imageBase + (uint)datasection.Count;
            datasection.AddRange(objents.ToArray());
            datasection.Align(4);
            dataaddr = imageBase + (uint)datasection.Count;
            datasection.AddRange(BitConverter.GetBytes(objlist.Length));
            datasection.AddRange(BitConverter.GetBytes(objentaddr));
            return datasection.ToArray();
        }
    }

    [Serializable]
    public class ObjectListEntry
    {
        public ObjectListEntry() { CodeString = string.Empty; Name = string.Empty; }

        public ObjectListEntry(byte[] file, int address, uint imageBase)
        {
            Arg1 = file[address];
            Arg2 = file[address + 1];
            Flags = BitConverter.ToUInt16(file, address + 2);
            Distance = BitConverter.ToSingle(file, address + 4);
            Unknown = BitConverter.ToInt32(file, address + 8);
            Code = BitConverter.ToUInt32(file, address + 12);
            Name = file.GetCString((int)(BitConverter.ToUInt32(file, address + 16) - imageBase));
        }

        public byte Arg1 { get; set; }
        public byte Arg2 { get; set; }
        public ushort Flags { get; set; }
        public float Distance { get; set; }
        public int Unknown { get; set; }
        [IniIgnore]
        public uint Code { get { uint code; if (uint.TryParse(CodeString, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out code)) return code; else return uint.MaxValue; } set { CodeString = value.ToString("X8"); } }
        [IniName("Code")]
        public string CodeString { get; set; }
        public string Name { get; set; }

        public static int Size { get { return 0x14; } }

        public byte[] GetBytes(Dictionary<string, uint> labels, uint nameAddress)
        {
            List<byte> result = new List<byte>(Size);
            result.Add(Arg1);
            result.Add(Arg2);
            result.AddRange(BitConverter.GetBytes(Flags));
            result.AddRange(BitConverter.GetBytes(Distance));
            result.AddRange(BitConverter.GetBytes(Unknown));
            if (Code == uint.MaxValue)
                Code = labels[CodeString];
            result.AddRange(BitConverter.GetBytes(Code));
            result.AddRange(BitConverter.GetBytes(nameAddress));
            return result.ToArray();
        }
    }

    public static class StartPosList
    {
        public static int Size { get { return StartPosInfo.Size + 4;}}

        public static Dictionary<LevelAct, StartPosInfo> Load(string filename)
        {
            return IniFile.Deserialize<Dictionary<LevelAct, StartPosInfo>>(filename);
        }

        public static Dictionary<LevelAct, StartPosInfo> Load(byte[] file, int address)
        {
            Dictionary<LevelAct, StartPosInfo> result = new Dictionary<LevelAct, StartPosInfo>();
            while (BitConverter.ToUInt16(file, address) != (ushort)LevelIDs.Invalid)
            {
                StartPosInfo objgrp = new StartPosInfo(file, address + 4);
                result.Add(new LevelAct(BitConverter.ToUInt16(file, address), BitConverter.ToUInt16(file, address + 2)), objgrp);
                address += Size;
            }
            return result;
        }

        public static void Save(this Dictionary<LevelAct, StartPosInfo> startpos, string filename)
        {
            IniFile.Serialize(startpos, filename);
        }

        public static byte[] GetBytes(this Dictionary<LevelAct, StartPosInfo> startpos)
        {
            List<byte> result = new List<byte>(Size * (startpos.Count + 1));
            foreach (KeyValuePair<LevelAct,StartPosInfo> item in startpos)
            {
                result.AddRange(BitConverter.GetBytes((ushort)item.Key.Level));
                result.AddRange(BitConverter.GetBytes((ushort)item.Key.Act));
                result.AddRange(item.Value.GetBytes());
            }
            result.AddRange(BitConverter.GetBytes((ushort)LevelIDs.Invalid));
            result.AddRange(new byte[StartPosInfo.Size + 2]);
            return result.ToArray();
        }
    }

    [Serializable]
    public class StartPosInfo
    {
        public StartPosInfo() { Position = new Vertex(); }

        public StartPosInfo(byte[] file, int address)
        {
            Position = new Vertex(file, address);
            YRotation = BitConverter.ToInt32(file, address + Vertex.Size);
        }

        public Vertex Position { get; set; }
        [IniIgnore]
        public int YRotation { get; set; }
        [IniName("YRotation")]
        public string YRotationString { get { return YRotation.ToString("X8"); } set { YRotation = int.Parse(value, NumberStyles.HexNumber); } }

        public static int Size { get { return Vertex.Size + 4; } }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>(Size);
            result.AddRange(Position.GetBytes());
            result.AddRange(BitConverter.GetBytes(YRotation));
            return result.ToArray();
        }
    }

    public static class TextureList
    {
        public static TextureListEntry[] Load(string filename)
        {
            return IniFile.Deserialize<TextureListEntry[]>(filename);
        }

        public static TextureListEntry[] Load(byte[] file, int address, uint imageBase)
        {
            List<TextureListEntry> objini = new List<TextureListEntry>();
            while (BitConverter.ToUInt64(file, address) != 0)
            {
                objini.Add(new TextureListEntry(file, address, imageBase));
                address += TextureListEntry.Size;
            }
            return objini.ToArray();
        }

        public static void Save(this TextureListEntry[] texlist, string filename)
        {
            IniFile.Serialize(texlist, filename);
        }

        public static byte[] GetBytes(this TextureListEntry[] texlist, uint imageBase, Dictionary<string, uint> labels, out uint dataaddr)
        {
            List<byte> datasection = new List<byte>();
            List<byte> objents = new List<byte>();
            for (int i = 0; i < texlist.Length; i++)
            {
                if (string.IsNullOrEmpty(texlist[i].Name))
                    objents.AddRange(new byte[4]);
                else
                {
                    objents.AddRange(texlist[i].GetBytes(imageBase + (uint)datasection.Count));
                    datasection.AddRange(HelperFunctions.GetEncoding().GetBytes(texlist[i].Name));
                    datasection.Add(0);
                    datasection.Align(4);
                }
            }
            dataaddr = imageBase + (uint)datasection.Count;
            datasection.AddRange(objents.ToArray());
            return datasection.ToArray();
        }
    }

    [Serializable]
    public class LevelTextureList
    {
        public LevelTextureList() { }

        public LevelTextureList(byte[] file, int address, uint imageBase)
        {
            Level = new LevelAct(BitConverter.ToUInt16(file, address));
            ushort numobjs = BitConverter.ToUInt16(file, address + 2);
            address = (int)(BitConverter.ToUInt32(file, address + 4) - imageBase);
            TextureList = new TextureListEntry[numobjs];
            for (int i = 0; i < numobjs; i++)
            {
                TextureList[i] = new TextureListEntry(file, address, imageBase);
                address += TextureListEntry.Size;
            }
        }

        public LevelAct Level { get; set; }
        [IniCollection]
        public TextureListEntry[] TextureList { get; set; }

        public static LevelTextureList Load(string filename)
        {
            return IniFile.Deserialize<LevelTextureList>(filename);
        }

        public void Save(string filename)
        {
            IniFile.Serialize(this, filename);
        }

        public byte[] GetBytes(uint imageBase, out uint dataaddr)
        {
            List<byte> datasection = new List<byte>();
            List<byte> objents = new List<byte>();
            for (int i = 0; i < TextureList.Length; i++)
            {
                if (string.IsNullOrEmpty(TextureList[i].Name))
                    objents.AddRange(new byte[4]);
                else
                {
                    objents.AddRange(TextureList[i].GetBytes(imageBase + (uint)datasection.Count));
                    datasection.AddRange(HelperFunctions.GetEncoding().GetBytes(TextureList[i].Name));
                    datasection.Add(0);
                    datasection.Align(4);
                }
            }
            uint objentaddr = imageBase + (uint)datasection.Count;
            datasection.AddRange(objents.ToArray());
            datasection.Align(4);
            dataaddr = imageBase + (uint)datasection.Count;
            datasection.AddRange(BitConverter.GetBytes(Level.LevelAndAct));
            datasection.AddRange(BitConverter.GetBytes((ushort)TextureList.Length));
            datasection.AddRange(BitConverter.GetBytes(objentaddr));
            return datasection.ToArray();
        }
    }

    [Serializable]
    public class TextureListEntry
    {
        public TextureListEntry() { Name = string.Empty; }
        public TextureListEntry(byte[] file, int address, uint imageBase)
        {
            uint nameAddress = BitConverter.ToUInt32(file, address);
            if (nameAddress == 0)
                Name = string.Empty;
            else
                Name = file.GetCString((int)(nameAddress - imageBase));
            Textures = BitConverter.ToUInt32(file, address + 4);
        }

        public string Name { get; set; }
        [IniIgnore]
        public uint Textures { get; set; }
        [IniName("Textures")]
        public string TexturesString { get { return Textures.ToString("X8"); } set { Textures = uint.Parse(value, NumberStyles.HexNumber); } }

        public static int Size { get { return 8; } }

        public byte[] GetBytes(uint nameAddress)
        {
            List<byte> result = new List<byte>(Size);
            result.AddRange(BitConverter.GetBytes(nameAddress));
            result.AddRange(BitConverter.GetBytes(Textures));
            return result.ToArray();
        }
    }

    public static class TrialLevelList
    {
        public static LevelAct[] Load(string filename)
        {
            string[] data = File.ReadAllLines(filename);
            List<LevelAct> result = new List<LevelAct>(data.Length);
            foreach (string item in data)
                if (!string.IsNullOrEmpty(item))
                    result.Add(new LevelAct(item));
            return result.ToArray();
        }

        public static LevelAct[] Load(byte[] file, int address, uint imageBase)
        {
            uint lvlcnt = BitConverter.ToUInt32(file, address + 4);
            address = (int)(BitConverter.ToUInt32(file, address) - imageBase);
            LevelAct[] result = new LevelAct[lvlcnt];
            for (int i = 0; i < lvlcnt; i++)
            {
                result[i] = new LevelAct(file[address], file[address + 1]);
                address += 2;
            }
            return result;
        }

        public static void Save(LevelAct[] levellist, string filename)
        {
            List<string> result = new List<string>(levellist.Length);
            foreach (LevelAct item in levellist)
                result.Add(item.ToString());
            File.WriteAllLines(filename, result.ToArray());
        }
    }

    public static class BossLevelList
    {
        public static LevelAct[] Load(string filename)
        {
            string[] data = File.ReadAllLines(filename);
            List<LevelAct> result = new List<LevelAct>(data.Length);
            foreach (string item in data)
                if (!string.IsNullOrEmpty(item))
                    result.Add(new LevelAct(item));
            return result.ToArray();
        }

        public static LevelAct[] Load(byte[] file, int address)
        {
            List<LevelAct> result = new List<LevelAct>();
            while (BitConverter.ToUInt16(file, address) != (ushort)LevelIDs.Invalid)
            {
                result.Add(new LevelAct(BitConverter.ToUInt16(file, address), BitConverter.ToUInt16(file, address + 2)));
                address += 4;
            }
            return result.ToArray();
        }

        public static void Save(LevelAct[] levellist, string filename)
        {
            List<string> result = new List<string>(levellist.Length);
            foreach (LevelAct item in levellist)
                result.Add(item.ToString());
            File.WriteAllLines(filename, result.ToArray());
        }

        public static byte[] GetBytes(LevelAct[] levellist)
        {
            List<byte> result = new List<byte>(4 * (levellist.Length + 1));
            foreach (LevelAct item in levellist)
            {
                result.AddRange(BitConverter.GetBytes((ushort)item.Level));
                result.AddRange(BitConverter.GetBytes((ushort)item.Act));
            }
            result.AddRange(BitConverter.GetBytes((ushort)LevelIDs.Invalid));
            result.AddRange(new byte[2]);
            return result.ToArray();
        }
    }

    public static class FieldStartPosList
    {
        public static int Size { get { return FieldStartPosInfo.Size + 2; } }

        public static Dictionary<LevelIDs, FieldStartPosInfo> Load(string filename)
        {
            return IniFile.Deserialize<Dictionary<LevelIDs, FieldStartPosInfo>>(filename);
        }

        public static Dictionary<LevelIDs, FieldStartPosInfo> Load(byte[] file, int address)
        {
            Dictionary<LevelIDs, FieldStartPosInfo> result = new Dictionary<LevelIDs, FieldStartPosInfo>();
            while ((LevelIDs)file[address] != LevelIDs.Invalid)
            {
                FieldStartPosInfo objgrp = new FieldStartPosInfo(file, address + 2);
                result.Add((LevelIDs)file[address], objgrp);
                address += Size;
            }
            return result;
        }

        public static void Save(this Dictionary<LevelIDs, FieldStartPosInfo> FieldStartPos, string filename)
        {
            IniFile.Serialize(FieldStartPos, filename);
        }

        public static byte[] GetBytes(this Dictionary<LevelIDs, FieldStartPosInfo> FieldStartPos)
        {
            List<byte> result = new List<byte>(Size * (FieldStartPos.Count + 1));
            foreach (KeyValuePair<LevelIDs, FieldStartPosInfo> item in FieldStartPos)
            {
                result.Add((byte)item.Key);
                result.Add(0);
                result.AddRange(item.Value.GetBytes());
            }
            result.AddRange(BitConverter.GetBytes((byte)LevelIDs.Invalid));
            result.AddRange(new byte[FieldStartPosInfo.Size + 1]);
            return result.ToArray();
        }
    }

    [Serializable]
    public class FieldStartPosInfo
    {
        public FieldStartPosInfo() { Position = new Vertex(); }

        public FieldStartPosInfo(byte[] file, int address)
        {
            Field = (LevelIDs)file[address];
            address += 2;
            Position = new Vertex(file, address);
            address += Vertex.Size;
            YRotation = BitConverter.ToInt32(file, address);
        }

        public LevelIDs Field { get; set; }
        public Vertex Position { get; set; }
        [IniIgnore]
        public int YRotation { get; set; }
        [IniName("YRotation")]
        public string YRotationString { get { return YRotation.ToString("X8"); } set { YRotation = int.Parse(value, NumberStyles.HexNumber); } }

        public static int Size { get { return 0x12; } }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>(Size);
            result.Add((byte)Field);
            result.Add(0);
            result.AddRange(Position.GetBytes());
            result.AddRange(BitConverter.GetBytes(YRotation));
            return result.ToArray();
        }
    }

    public static class SoundTestList
    {
        public static SoundTestListEntry[] Load(string filename)
        {
            return IniFile.Deserialize<SoundTestListEntry[]>(filename);
        }

        public static SoundTestListEntry[] Load(byte[] file, int address, uint imageBase)
        {
            int numobjs = BitConverter.ToInt32(file, address + 4);
            address = (int)(BitConverter.ToUInt32(file, address) - imageBase);
            List<SoundTestListEntry> objini = new List<SoundTestListEntry>(numobjs);
            for (int i = 0; i < numobjs; i++)
            {
                objini.Add(new SoundTestListEntry(file, address, imageBase));
                address += SoundTestListEntry.Size;
            }
            return objini.ToArray();
        }

        public static void Save(this SoundTestListEntry[] soundlist, string filename)
        {
            IniFile.Serialize(soundlist, filename);
        }
    }

    [Serializable]
    public class SoundTestListEntry
    {
        public SoundTestListEntry() { Title = string.Empty; }
        public SoundTestListEntry(byte[] file, int address, uint imageBase)
        {
            Title = file.GetCString((int)(BitConverter.ToUInt32(file, address) - imageBase));
            Track = BitConverter.ToInt32(file, address + 4);
        }

        public string Title { get; set; }
        public int Track { get; set; }

        public static int Size { get { return 8; } }

        public byte[] GetBytes(uint titleAddress)
        {
            List<byte> result = new List<byte>(Size);
            result.AddRange(BitConverter.GetBytes(titleAddress));
            result.AddRange(BitConverter.GetBytes(Track));
            return result.ToArray();
        }
    }

    public static class MusicList
    {
        public static MusicListEntry[] Load(string filename)
        {
            return IniFile.Deserialize<MusicListEntry[]>(filename);
        }

        public static MusicListEntry[] Load(byte[] file, int address, uint imageBase, int numsongs)
        {
            List<MusicListEntry> objini = new List<MusicListEntry>(numsongs);
            for (int i = 0; i < numsongs; i++)
            {
                objini.Add(new MusicListEntry(file, address, imageBase));
                address += MusicListEntry.Size;
            }
            return objini.ToArray();
        }

        public static void Save(this MusicListEntry[] soundlist, string filename)
        {
            IniFile.Serialize(soundlist, filename);
        }
    }

    [Serializable]
    public class MusicListEntry
    {
        public MusicListEntry() { Filename = string.Empty; }
        public MusicListEntry(byte[] file, int address, uint imageBase)
        {
            Filename = file.GetCString((int)(BitConverter.ToUInt32(file, address) - imageBase));
            Loop = Convert.ToBoolean(BitConverter.ToInt32(file, address + 4));
        }

        public string Filename { get; set; }
        public bool Loop { get; set; }

        public static int Size { get { return 8; } }

        public byte[] GetBytes(uint titleAddress)
        {
            List<byte> result = new List<byte>(Size);
            result.AddRange(BitConverter.GetBytes(titleAddress));
            result.AddRange(BitConverter.GetBytes(Convert.ToInt32(Loop)));
            return result.ToArray();
        }
    }

    public static class SoundList
    {
        public static SoundListEntry[] Load(string filename)
        {
            return IniFile.Deserialize<SoundListEntry[]>(filename);
        }

        public static SoundListEntry[] Load(byte[] file, int address, uint imageBase)
        {
            int numobjs = BitConverter.ToInt32(file, address);
            address = (int)(BitConverter.ToUInt32(file, address + 4) - imageBase);
            List<SoundListEntry> objini = new List<SoundListEntry>(numobjs);
            for (int i = 0; i < numobjs; i++)
            {
                objini.Add(new SoundListEntry(file, address, imageBase));
                address += SoundListEntry.Size;
            }
            return objini.ToArray();
        }

        public static void Save(this SoundListEntry[] soundlist, string filename)
        {
            IniFile.Serialize(soundlist, filename);
        }
    }

    [Serializable]
    public class SoundListEntry
    {
        public SoundListEntry() { Filename = string.Empty; }
        public SoundListEntry(byte[] file, int address, uint imageBase)
        {
            Bank = BitConverter.ToInt32(file, address);
            Filename = file.GetCString((int)(BitConverter.ToUInt32(file, address + 4) - imageBase));
        }

        public int Bank { get; set; }
        public string Filename { get; set; }

        public static int Size { get { return 8; } }

        public byte[] GetBytes(uint filenameAddress)
        {
            List<byte> result = new List<byte>(Size);
            result.AddRange(BitConverter.GetBytes(Bank));
            result.AddRange(BitConverter.GetBytes(filenameAddress));
            return result.ToArray();
        }
    }

    public static class StringArray
    {
        public static string[] Load(string filename)
        {
            string[] result = File.ReadAllLines(filename);
            for (int i = 0; i < result.Length; i++)
                result[i] = result[i].UnescapeNewlines();
            return result;
        }

        public static string[] Load(byte[] file, int address, uint imageBase, int length)
        {
            return Load(file, address, imageBase, length, HelperFunctions.GetEncoding());
        }

        public static string[] Load(byte[] file, int address, uint imageBase, int length, Encoding encoding)
        {
            string[] result = new string[length];
            for (int i = 0; i < length; i++)
            {
                uint straddr = BitConverter.ToUInt32(file, address);
                if (straddr == 0)
                    result[i] = string.Empty;
                else
                    result[i] = file.GetCString((int)(straddr - imageBase), encoding);
                address += 4;
            }
            return result;
        }

        public static void Save(this string[] strings, string filename)
        {
            string[] result = (string[])strings.Clone();
            for (int i = 0; i < result.Length; i++)
                result[i] = result[i].EscapeNewlines();
            File.WriteAllLines(filename, result);
        }
    }

    public static class NextLevelList
    {
        public static NextLevelListEntry[] Load(string filename)
        {
            return IniFile.Deserialize<NextLevelListEntry[]>(filename);
        }

        public static NextLevelListEntry[] Load(byte[] file, int address)
        {
            List<NextLevelListEntry> result = new List<NextLevelListEntry>();
            while (file[address + 1] != byte.MaxValue)
            {
                result.Add(new NextLevelListEntry(file, address));
                address += NextLevelListEntry.Size;
            }
            return result.ToArray();
        }

        public static void Save(this NextLevelListEntry[] levellist, string filename)
        {
            IniFile.Serialize(levellist, filename);
        }

        public static byte[] GetBytes(this NextLevelListEntry[] levellist)
        {
            List<byte> result = new List<byte>();
            for (int i = 0; i < levellist.Length; i++)
                result.AddRange(levellist[i].GetBytes());
            result.AddRange(new NextLevelListEntry() { Level = (LevelIDs)byte.MaxValue }.GetBytes());
            return result.ToArray();
        }
    }

    [Serializable]
    public class NextLevelListEntry
    {
        public NextLevelListEntry() { }
        public NextLevelListEntry(byte[] file, int address)
        {
            CGMovie = file[address];
            Level = (LevelIDs)file[address + 1];
            NextLevel = ((LevelIDs)file[address + 2]);
            NextAct = file[address + 3];
            StartPos = file[address + 4];
            AltNextLevel = (LevelIDs)file[address + 5];
            AltNextAct = file[address + 6];
            AltStartPos = file[address + 7];
        }

        public byte CGMovie { get; set; }
        public LevelIDs Level { get; set; }
        public LevelIDs NextLevel { get; set; }
        public byte NextAct { get; set; }
        public byte StartPos { get; set; }
        public LevelIDs AltNextLevel { get; set; }
        public byte AltNextAct { get; set; }
        public byte AltStartPos { get; set; }

        public static int Size { get { return 8; } }

        public byte[] GetBytes()
        {
            return new byte[] { CGMovie, (byte)Level, (byte)NextLevel, NextAct, StartPos, (byte)AltNextLevel, AltNextAct, AltStartPos };
        }
    }

    [Serializable]
    public class CutsceneText
    {
        private CutsceneText() { Text = new string[5][]; }

        public CutsceneText(string directory)
            : this()
        {
            for (int i = 0; i < 5; i++)
                Text[i] = StringArray.Load(Path.Combine(directory, ((Languages)i).ToString() + ".txt"));
        }

        public CutsceneText(byte[] file, int address, uint imageBase, int length)
            : this()
        {
            for (int i = 0; i < 5; i++)
            {
                Text[i] = StringArray.Load(file, (int)(BitConverter.ToUInt32(file, address) - imageBase), imageBase, length,
                    HelperFunctions.GetEncoding((Languages)i));
                address += 4;
            }
        }

        public string[][] Text { get; private set; }

        public void Save(string directory)
        {
            string[] hashes;
            Save(directory, out hashes);
        }

        public void Save(string directory, out string[] hashes)
        {
            hashes = new string[5];
            Directory.CreateDirectory(directory);
            for (int i = 0; i < 5; i++)
            {
                string textname = Path.Combine(directory, ((Languages)i).ToString() + ".txt");
                Text[i].Save(textname);
                hashes[i] = HelperFunctions.FileHash(textname);
            }
        }
    }

    public static class LevelClearFlagList
    {
        public static LevelClearFlag[] Load(string filename)
        {
            string[] tmp = File.ReadAllLines(filename);
            List<LevelClearFlag> result = new List<LevelClearFlag>(tmp.Length);
            foreach (string line in tmp)
            {
                if (string.IsNullOrEmpty(line)) continue;
                result.Add(new LevelClearFlag(line));
            }
            return result.ToArray();
        }

        public static LevelClearFlag[] Load(byte[] file, int address)
        {
            List<LevelClearFlag> result = new List<LevelClearFlag>();
            while (BitConverter.ToUInt16(file, address) != ushort.MaxValue)
            {
                result.Add(new LevelClearFlag(file, address));
                address += 4;
            }
            return result.ToArray();
        }

        public static void Save(this LevelClearFlag[] levellist, string filename)
        {
            List<string> result = new List<string>(levellist.Length);
            foreach (LevelClearFlag item in levellist)
                result.Add(item.ToString());
            File.WriteAllLines(filename, result.ToArray());
        }

        public static byte[] GetBytes(this LevelClearFlag[] levellist)
        {
            List<byte> result = new List<byte>((levellist.Length + 1) * 4);
            foreach (LevelClearFlag item in levellist)
                result.AddRange(item.GetBytes());
            result.AddRange(BitConverter.GetBytes(uint.MaxValue));
            return result.ToArray();
        }
    }

    public class LevelClearFlag
    {
        public LevelClearFlag() { }

        public LevelClearFlag(byte[] file, int address)
        {
            Level = (LevelIDs)BitConverter.ToUInt16(file, address);
            Flag = BitConverter.ToUInt16(file, address + 2);
        }

        public LevelClearFlag(string data)
        {
            string[] splitline = data.Split(' ');
            Level = (LevelIDs)Enum.Parse(typeof(LevelIDs), splitline[0]);
            Flag = ushort.Parse(splitline[1], NumberStyles.HexNumber);
        }

        public LevelIDs Level { get; set; }
        public ushort Flag { get; set; }

        public static int Size { get { return 4; } }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>(Size);
            result.AddRange(BitConverter.GetBytes((ushort)Level));
            result.AddRange(BitConverter.GetBytes(Flag));
            return result.ToArray();
        }

        public override string ToString()
        {
            return Level.ToString() + " " + Flag.ToString("X4");
        }
    }

    public static class DeathZoneFlagsList
    {
        public static DeathZoneFlags[] Load(string filename)
        {
            return IniFile.Deserialize<DeathZoneFlags[]>(filename);
        }

        public static void Save(this DeathZoneFlags[] flags, string filename)
        {
            IniFile.Serialize(flags, filename);
        }
    }

    [Serializable]
    public class DeathZoneFlags
    {
        public DeathZoneFlags() { }

        public DeathZoneFlags(byte[] file, int address)
        {
            Flags = (CharacterFlags)BitConverter.ToInt32(file, address);
        }

        public CharacterFlags Flags { get; set; }

        public static int Size { get { return 4; } }

        public byte[] GetBytes()
        {
            return BitConverter.GetBytes((int)Flags);
        }
    }

    public static class SkyboxScaleList
    {
        public static SkyboxScale[] Load(string filename)
        {
            return IniFile.Deserialize<SkyboxScale[]>(filename);
        }

        public static SkyboxScale[] Load(byte[] file, int address, uint imageBase, int count)
        {
            List<SkyboxScale> result = new List<SkyboxScale>(count);
            for (int i = 0; i < count; i++)
            {
                if (BitConverter.ToUInt32(file, address) != 0)
                    result.Add(new SkyboxScale(file, (int)(BitConverter.ToUInt32(file, address) - imageBase)));
                else
                    result.Add(new SkyboxScale());
                address += 4;
            }
            return result.ToArray();
        }

        public static void Save(this SkyboxScale[] scales, string filename)
        {
            IniFile.Serialize(scales, filename);
        }
    }

    [Serializable]
    public class SkyboxScale
    {
        public SkyboxScale() { }

        public SkyboxScale(byte[] file, int address)
        {
            Far = new Vertex(file, address);
            address += Vertex.Size;
            Normal = new Vertex(file, address);
            address += Vertex.Size;
            Near = new Vertex(file, address);
        }

        public Vertex Far { get; set; }
        public Vertex Normal { get; set; }
        public Vertex Near { get; set; }

        public static int Size { get { return Vertex.Size * 3; } }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>(Size);
            result.AddRange(Far.GetBytes());
            result.AddRange(Normal.GetBytes());
            result.AddRange(Near.GetBytes());
            return result.ToArray();
        }
    }

    /// <summary>
    /// Converts between <see cref="System.String"/> and <typeparamref name="T"/>
    /// </summary>
    public class StringConverter<T> : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(T))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is T)
                return ((T)value).ToString();
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
                return Activator.CreateInstance(typeof(T), (string)value);
            return base.ConvertFrom(context, culture, value);
        }
    }
}