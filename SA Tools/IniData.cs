using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;

using SonicRetro.SAModel;

namespace SA_Tools
{
    public class IniData
    {
        [IniName("key")]
		[TypeConverter(typeof(UInt32HexConverter))]
        public uint? ImageBase { get; set; }
        [IniName("compressed")]
        public bool Compressed { get; set; }
        [IniName("bigendian")]
        public bool BigEndian { get; set; }
        [IniName("game")]
        [DefaultValue(Game.SADX)]
        public Game Game { get; set; }
		[IniName("masterobjlist")]
		public string MasterObjectList { get; set; }
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
        [IniCollection(IniCollectionMode.IndexOnly)]
        public Dictionary<string, FileInfo> Files { get; set; }
    }

    public enum Game
    {
        SA1,
        SADX,
        SA2,
        SA2B
    }

    public class FileInfo
    {
        [IniName("type")]
        public string Type { get; set; }
        [IniName("address")]
		[TypeConverter(typeof(Int32HexConverter))]
        public int Address { get; set; }
        [IniName("filename")]
        public string Filename { get; set; }
        [IniName("md5")]
        public string MD5Hash { get; set; }
        [IniName("pointer")]
		[IniCollection(IniCollectionMode.SingleLine, Format = ",", ValueConverter = typeof(Int32HexConverter))]
        public int[] PointerList { get; set; }
        [IniCollection(IniCollectionMode.IndexOnly)]
        public Dictionary<string, string> CustomProperties { get; set; }
    }

    [Serializable]
    [TypeConverter(typeof(StringConverter<SA1LevelAct>))]
    public struct SA1LevelAct : IComparable, IComparable<SA1LevelAct>, IComparable<ushort>, IEquatable<SA1LevelAct>
    {
        public SA1LevelAct(SA1LevelIDs level, int act) { Level = level; Act = (byte)act; }
        public SA1LevelAct(int level, int act) { Level = (SA1LevelIDs)level; Act = (byte)act; }
        public SA1LevelAct(ushort levelact) { Level = (SA1LevelIDs)(levelact >> 8); Act = (byte)(levelact & 0xFF); }
        public SA1LevelAct(string levelact)
        {
            if (levelact.Contains(" "))
            {
                Level = (SA1LevelIDs)Enum.Parse(typeof(SA1LevelIDs), levelact.Split(' ')[0]);
                Act = byte.Parse(levelact.Split(' ')[1], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
            }
            else
            {
                Level = (SA1LevelIDs)byte.Parse(levelact.Substring(0, 2), NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                Act = byte.Parse(levelact.Substring(2, 2), NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
            }
        }

        public SA1LevelIDs Level;
        public byte Act;
        public ushort LevelAndAct { get { return (ushort)((byte)Level << 8 | Act); } set { Level = (SA1LevelIDs)(value >> 8); Act = (byte)(value & 0xFF); } }

        public override string ToString()
        {
            return Level.ToString() + " " + Act.ToString(NumberFormatInfo.InvariantInfo);
        }

        public int CompareTo(object obj)
        {
            if (obj is SA1LevelAct)
                return CompareTo((SA1LevelAct)obj);
            else
                return LevelAndAct.CompareTo(obj);
        }

        public int CompareTo(SA1LevelAct other)
        {
            return LevelAndAct.CompareTo(other.LevelAndAct);
        }

        public int CompareTo(ushort other)
        {
            return LevelAndAct.CompareTo(other);
        }

        public override bool Equals(object obj)
        {
            if (obj is SA1LevelAct)
                return Equals((SA1LevelAct)obj);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return LevelAndAct.GetHashCode();
        }

        public bool Equals(SA1LevelAct other)
        {
            return LevelAndAct.Equals(other.LevelAndAct);
        }

        public static explicit operator ushort(SA1LevelAct obj)
        {
            return obj.LevelAndAct;
        }

        public static explicit operator SA1LevelAct(ushort obj)
        {
            return new SA1LevelAct(obj);
        }

        public static bool operator ==(SA1LevelAct a, SA1LevelAct b)
        {
            return a.LevelAndAct == b.LevelAndAct;
        }

        public static bool operator !=(SA1LevelAct a, SA1LevelAct b)
        {
            return a.LevelAndAct != b.LevelAndAct;
        }

        public static bool operator <(SA1LevelAct a, SA1LevelAct b)
        {
            return a.LevelAndAct < b.LevelAndAct;
        }

        public static bool operator >(SA1LevelAct a, SA1LevelAct b)
        {
            return a.LevelAndAct > b.LevelAndAct;
        }

        public static bool operator <=(SA1LevelAct a, SA1LevelAct b)
        {
            return a.LevelAndAct <= b.LevelAndAct;
        }

        public static bool operator >=(SA1LevelAct a, SA1LevelAct b)
        {
            return a.LevelAndAct >= b.LevelAndAct;
        }

        public string ToC()
        {
            return string.Format("levelact({0}, {1})", Level.ToC("LevelIDs"), Act);
        }
    }

    public static class ObjectList
    {
        public static ObjectListEntry[] Load(string filename, bool SA2)
        {
            if (SA2)
                return IniSerializer.Deserialize<SA2ObjectListEntry[]>(filename);
            else
                return IniSerializer.Deserialize<SA1ObjectListEntry[]>(filename);
        }

        public static ObjectListEntry[] Load(byte[] file, int address, uint imageBase, bool SA2)
        {
            int numobjs = ByteConverter.ToInt32(file, address);
            address = file.GetPointer(address + 4, imageBase);
            if (SA2)
            {
                List<SA2ObjectListEntry> objini = new List<SA2ObjectListEntry>(numobjs);
                for (int i = 0; i < numobjs; i++)
                {
                    objini.Add(new SA2ObjectListEntry(file, address, imageBase));
                    address += SA2ObjectListEntry.Size;
                }
                return objini.ToArray();
            }
            else
            {
                List<SA1ObjectListEntry> objini = new List<SA1ObjectListEntry>(numobjs);
                for (int i = 0; i < numobjs; i++)
                {
                    objini.Add(new SA1ObjectListEntry(file, address, imageBase));
                    address += SA1ObjectListEntry.Size;
                }
                return objini.ToArray();
            }
        }

        public static void Save(this ObjectListEntry[] objlist, string filename)
        {
            IniSerializer.Serialize(objlist, filename);
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
            datasection.AddRange(ByteConverter.GetBytes(objlist.Length));
            datasection.AddRange(ByteConverter.GetBytes(objentaddr));
            return datasection.ToArray();
        }
    }

    [Serializable]
    public abstract class ObjectListEntry
    {
        [IniAlwaysInclude]
        public byte Arg1 { get; set; }
        [IniAlwaysInclude]
        public byte Arg2 { get; set; }
        [IniAlwaysInclude]
        public ushort Flags { get; set; }
        public float Distance { get; set; }
        [IniIgnore]
        public uint Code { get { uint code; if (uint.TryParse(CodeString, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out code)) return code; else return uint.MaxValue; } set { CodeString = value.ToString("X8"); } }
        [IniName("Code")]
        public string CodeString { get; set; }
        public string Name { get; set; }

        public abstract byte[] GetBytes(Dictionary<string, uint> labels, uint nameAddress);

        public abstract string ToStruct();
    }

    [Serializable]
    public class SA1ObjectListEntry : ObjectListEntry
    {
        public SA1ObjectListEntry() { CodeString = string.Empty; Name = string.Empty; }

        public SA1ObjectListEntry(byte[] file, int address, uint imageBase)
        {
            Arg1 = file[address++];
            Arg2 = file[address++];
            Flags = ByteConverter.ToUInt16(file, address);
            address += 2;
            Distance = ByteConverter.ToSingle(file, address);
            address += 4;
            Unknown = ByteConverter.ToInt32(file, address);
            address += 4;
            Code = ByteConverter.ToUInt32(file, address);
            address += 4;
            Name = file.GetCString(file.GetPointer(address, imageBase));
        }

        public int Unknown { get; set; }

        public static int Size { get { return 0x14; } }

        public override byte[] GetBytes(Dictionary<string, uint> labels, uint nameAddress)
        {
            List<byte> result = new List<byte>(Size);
            result.Add(Arg1);
            result.Add(Arg2);
            result.AddRange(ByteConverter.GetBytes(Flags));
            result.AddRange(ByteConverter.GetBytes(Distance));
            result.AddRange(ByteConverter.GetBytes(Unknown));
            if (Code == uint.MaxValue)
                Code = labels[CodeString];
            result.AddRange(ByteConverter.GetBytes(Code));
            result.AddRange(ByteConverter.GetBytes(nameAddress));
            return result.ToArray();
        }

        public override string ToStruct()
        {
            StringBuilder result = new StringBuilder("{ ");
            result.Append(Arg1);
            result.Append(", ");
            result.Append(Arg2);
            result.Append(", ");
            result.Append(Flags.ToCHex());
            result.Append(", ");
            result.Append(Distance.ToC());
            result.Append(", ");
            result.Append(Unknown);
            result.Append(", (ObjectFuncPtr)");
            result.Append(Code.ToCHex());
            result.Append(", ");
            result.Append(Name.ToC());
            result.Append(" }");
            return result.ToString();
        }
    }

    [Serializable]
    public class SA2ObjectListEntry : ObjectListEntry
    {
        public SA2ObjectListEntry() { CodeString = string.Empty; Name = string.Empty; }

        public SA2ObjectListEntry(byte[] file, int address, uint imageBase)
        {
            Arg1 = file[address++];
            Arg2 = file[address++];
            Flags = ByteConverter.ToUInt16(file, address);
            address += 2;
            Distance = ByteConverter.ToSingle(file, address);
            address += 4;
            Code = ByteConverter.ToUInt32(file, address);
            address += 4;
            Name = file.GetCString(file.GetPointer(address, imageBase));
        }

        public static int Size { get { return 0x10; } }

        public override byte[] GetBytes(Dictionary<string, uint> labels, uint nameAddress)
        {
            List<byte> result = new List<byte>(Size);
            result.Add(Arg1);
            result.Add(Arg2);
            result.AddRange(ByteConverter.GetBytes(Flags));
            result.AddRange(ByteConverter.GetBytes(Distance));
            if (Code == uint.MaxValue)
                Code = labels[CodeString];
            result.AddRange(ByteConverter.GetBytes(Code));
            result.AddRange(ByteConverter.GetBytes(nameAddress));
            return result.ToArray();
        }

        public override string ToStruct()
        {
            StringBuilder result = new StringBuilder("{ ");
            result.Append(Arg1);
            result.Append(", ");
            result.Append(Arg2);
            result.Append(", ");
            result.Append(Flags.ToCHex());
            result.Append(", ");
            result.Append(Distance.ToC());
            result.Append(", (ObjectFuncPtr)");
            result.Append(Code.ToCHex());
            result.Append(", ");
            result.Append(Name.ToC());
            result.Append(" }");
            return result.ToString();
        }
    }

	public class MasterObjectListEntry
	{
		[IniAlwaysInclude]
		public byte Arg1 { get; set; }
		[IniAlwaysInclude]
		public byte Arg2 { get; set; }
		[IniAlwaysInclude]
		public ushort Flags { get; set; }
		public float Distance { get; set; }
		public string Name { get; set; }
		[IniCollection(IniCollectionMode.SingleLine, Format = ", ")]
		public string[] Names { get; set; }

		public MasterObjectListEntry() { }

		public MasterObjectListEntry(ObjectListEntry obj)
		{
			Arg1 = obj.Arg1;
			Arg2 = obj.Arg2;
			Flags = obj.Flags;
			Distance = obj.Distance;
			Name = obj.Name;
			Names = new[] { obj.Name };
		}
	}

    public static class SA1StartPosList
    {
        public static int Size { get { return SA1StartPosInfo.Size + 4;}}

        public static Dictionary<SA1LevelAct, SA1StartPosInfo> Load(string filename)
        {
            return IniSerializer.Deserialize<Dictionary<SA1LevelAct, SA1StartPosInfo>>(filename);
        }

        public static Dictionary<SA1LevelAct, SA1StartPosInfo> Load(byte[] file, int address)
        {
            Dictionary<SA1LevelAct, SA1StartPosInfo> result = new Dictionary<SA1LevelAct, SA1StartPosInfo>();
            while (ByteConverter.ToUInt16(file, address) != (ushort)SA1LevelIDs.Invalid)
            {
                SA1StartPosInfo objgrp = new SA1StartPosInfo(file, address + 4);
                result.Add(new SA1LevelAct(ByteConverter.ToUInt16(file, address), ByteConverter.ToUInt16(file, address + 2)), objgrp);
                address += Size;
            }
            return result;
        }

        public static void Save(this Dictionary<SA1LevelAct, SA1StartPosInfo> startpos, string filename)
        {
            IniSerializer.Serialize(startpos, filename);
        }

        public static byte[] GetBytes(this Dictionary<SA1LevelAct, SA1StartPosInfo> startpos)
        {
            List<byte> result = new List<byte>(Size * (startpos.Count + 1));
            foreach (KeyValuePair<SA1LevelAct, SA1StartPosInfo> item in startpos)
            {
                result.AddRange(ByteConverter.GetBytes((ushort)item.Key.Level));
                result.AddRange(ByteConverter.GetBytes((ushort)item.Key.Act));
                result.AddRange(item.Value.GetBytes());
            }
            result.AddRange(ByteConverter.GetBytes((ushort)SA1LevelIDs.Invalid));
            result.AddRange(new byte[SA1StartPosInfo.Size + 2]);
            return result.ToArray();
        }

        public static string ToStruct(this KeyValuePair<SA1LevelAct, SA1StartPosInfo> startpos)
        {
            StringBuilder result = new StringBuilder("{ ");
            result.Append(startpos.Key.Level.ToC("LevelIDs"));
            result.Append(", ");
            result.Append(startpos.Key.Act);
            result.Append(", ");
            result.Append(startpos.Value.Position.ToStruct());
            result.Append(", ");
            result.Append(startpos.Value.YRotation.ToCHex());
            result.Append(" }");
            return result.ToString();
        }
    }

    [Serializable]
    public class SA1StartPosInfo
    {
        public SA1StartPosInfo() { Position = new Vertex(); }

        public SA1StartPosInfo(byte[] file, int address)
        {
            Position = new Vertex(file, address);
            YRotation = ByteConverter.ToInt32(file, address + Vertex.Size);
        }

        public Vertex Position { get; set; }
		[TypeConverter(typeof(Int32HexConverter))]
        public int YRotation { get; set; }

        public static int Size { get { return Vertex.Size + 4; } }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>(Size);
            result.AddRange(Position.GetBytes());
            result.AddRange(ByteConverter.GetBytes(YRotation));
            return result.ToArray();
        }
    }

    public static class SA2StartPosList
    {
        public static int Size { get { return SA2StartPosInfo.Size + 2; } }

        public static Dictionary<SA2LevelIDs, SA2StartPosInfo> Load(string filename)
        {
            return IniSerializer.Deserialize<Dictionary<SA2LevelIDs, SA2StartPosInfo>>(filename);
        }

        public static Dictionary<SA2LevelIDs, SA2StartPosInfo> Load(byte[] file, int address)
        {
            Dictionary<SA2LevelIDs, SA2StartPosInfo> result = new Dictionary<SA2LevelIDs, SA2StartPosInfo>();
            while (ByteConverter.ToUInt16(file, address) != (ushort)SA2LevelIDs.Invalid)
            {
                SA2StartPosInfo objgrp = new SA2StartPosInfo(file, address + 2);
                result.Add((SA2LevelIDs)ByteConverter.ToUInt16(file, address), objgrp);
                address += Size;
            }
            return result;
        }

        public static void Save(this Dictionary<SA2LevelIDs, SA2StartPosInfo> startpos, string filename)
        {
            IniSerializer.Serialize(startpos, filename);
        }

        public static byte[] GetBytes(this Dictionary<SA2LevelIDs, SA2StartPosInfo> startpos)
        {
            List<byte> result = new List<byte>(Size * (startpos.Count + 1));
            foreach (KeyValuePair<SA2LevelIDs, SA2StartPosInfo> item in startpos)
            {
                result.AddRange(ByteConverter.GetBytes((ushort)item.Key));
                result.AddRange(item.Value.GetBytes());
            }
            result.AddRange(ByteConverter.GetBytes((ushort)SA2LevelIDs.Invalid));
            result.AddRange(new byte[SA2StartPosInfo.Size]);
            return result.ToArray();
        }

        public static string ToStruct(this KeyValuePair<SA2LevelIDs, SA2StartPosInfo> startpos)
        {
            StringBuilder result = new StringBuilder("{ ");
            result.Append(startpos.Key.ToC("LevelIDs"));
            result.Append(", ");
            result.Append(startpos.Value.YRotation.ToCHex());
            result.Append(", ");
            result.Append(startpos.Value.P1YRotation.ToCHex());
            result.Append(", ");
            result.Append(startpos.Value.P2YRotation.ToCHex());
            result.Append(", ");
            result.Append(startpos.Value.Position.ToStruct());
            result.Append(", ");
            result.Append(startpos.Value.P1Position.ToStruct());
            result.Append(", ");
            result.Append(startpos.Value.P2Position.ToStruct());
            result.Append(" }");
            return result.ToString();
        }
    }

    [Serializable]
    public class SA2StartPosInfo
    {
        public SA2StartPosInfo()
        {
            Position = new Vertex();
            P1Position = new Vertex();
            P2Position = new Vertex();
        }

        public SA2StartPosInfo(byte[] file, int address)
        {
            YRotation = ByteConverter.ToUInt16(file, address);
            address += sizeof(ushort);
            P1YRotation = ByteConverter.ToUInt16(file, address);
            address += sizeof(ushort);
            P2YRotation = ByteConverter.ToUInt16(file, address);
            address += sizeof(ushort);
            Position = new Vertex(file, address);
            address += Vertex.Size;
            P1Position = new Vertex(file, address);
            address += Vertex.Size;
            P2Position = new Vertex(file, address);
            address += Vertex.Size;
        }

        [TypeConverter(typeof(UInt16HexConverter))]
        public ushort YRotation { get; set; }
		[TypeConverter(typeof(UInt16HexConverter))]
		public ushort P1YRotation { get; set; }
		[TypeConverter(typeof(UInt16HexConverter))]
		public ushort P2YRotation { get; set; }
        public Vertex Position { get; set; }
        public Vertex P1Position { get; set; }
        public Vertex P2Position { get; set; }

        public static int Size { get { return (Vertex.Size + sizeof(ushort)) * 3; } }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>(Size);
            result.AddRange(ByteConverter.GetBytes(YRotation));
            result.AddRange(ByteConverter.GetBytes(P1YRotation));
            result.AddRange(ByteConverter.GetBytes(P2YRotation));
            result.AddRange(Position.GetBytes());
            result.AddRange(P1Position.GetBytes());
            result.AddRange(P2Position.GetBytes());
            return result.ToArray();
        }
    }

    public static class TextureList
    {
        public static TextureListEntry[] Load(string filename)
        {
            return IniSerializer.Deserialize<TextureListEntry[]>(filename);
        }

        public static TextureListEntry[] Load(byte[] file, int address, uint imageBase)
        {
            List<TextureListEntry> objini = new List<TextureListEntry>();
            while (ByteConverter.ToUInt64(file, address) != 0)
            {
                objini.Add(new TextureListEntry(file, address, imageBase));
                address += TextureListEntry.Size;
            }
            return objini.ToArray();
        }

        public static void Save(this TextureListEntry[] texlist, string filename)
        {
            IniSerializer.Serialize(texlist, filename);
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
            Level = new SA1LevelAct(ByteConverter.ToUInt16(file, address));
            ushort numobjs = ByteConverter.ToUInt16(file, address + 2);
            address = file.GetPointer(address + 4, imageBase);
            TextureList = new TextureListEntry[numobjs];
            for (int i = 0; i < numobjs; i++)
            {
                TextureList[i] = new TextureListEntry(file, address, imageBase);
                address += TextureListEntry.Size;
            }
        }

        public SA1LevelAct Level { get; set; }
        [IniCollection(IniCollectionMode.IndexOnly)]
        public TextureListEntry[] TextureList { get; set; }

        public static LevelTextureList Load(string filename)
        {
            return IniSerializer.Deserialize<LevelTextureList>(filename);
        }

        public void Save(string filename)
        {
            IniSerializer.Serialize(this, filename);
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
            datasection.AddRange(ByteConverter.GetBytes(Level.LevelAndAct));
            datasection.AddRange(ByteConverter.GetBytes((ushort)TextureList.Length));
            datasection.AddRange(ByteConverter.GetBytes(objentaddr));
            return datasection.ToArray();
        }
    }

    [Serializable]
    public class TextureListEntry
    {
        public TextureListEntry() { Name = string.Empty; }
        public TextureListEntry(byte[] file, int address, uint imageBase)
        {
            uint nameAddress = ByteConverter.ToUInt32(file, address);
            if (nameAddress == 0)
                Name = string.Empty;
            else
                Name = file.GetCString((int)(nameAddress - imageBase));
            Textures = ByteConverter.ToUInt32(file, address + 4);
            if (Textures != 0)
                Count = ByteConverter.ToInt32(file, (int)(Textures - imageBase) + 4);
        }

        public string Name { get; set; }
		[TypeConverter(typeof(UInt32HexConverter))]
		public uint Textures { get; set; }
        public int Count { get; set; }

        public static int Size { get { return 8; } }

        public byte[] GetBytes(uint nameAddress)
        {
            List<byte> result = new List<byte>(Size);
            result.AddRange(ByteConverter.GetBytes(nameAddress));
            result.AddRange(ByteConverter.GetBytes(Textures));
            return result.ToArray();
        }

        public string ToStruct()
        {
            StringBuilder result = new StringBuilder("{ ");
            if (!string.IsNullOrEmpty(Name))
                result.Append(Name.ToC());
            else
                result.Append("NULL");
            result.Append(", ");
            if (Textures != 0)
            {
                result.Append("(TexList *)");
                result.Append(Textures.ToCHex());
            }
            else
                result.Append("NULL");
            result.Append(" }");
            return result.ToString();
        }
    }

    public static class TrialLevelList
    {
        public static SA1LevelAct[] Load(string filename)
        {
            string[] data = File.ReadAllLines(filename);
            List<SA1LevelAct> result = new List<SA1LevelAct>(data.Length);
            foreach (string item in data)
                if (!string.IsNullOrEmpty(item))
                    result.Add(new SA1LevelAct(item));
            return result.ToArray();
        }

        public static SA1LevelAct[] Load(byte[] file, int address, uint imageBase)
        {
            uint lvlcnt = ByteConverter.ToUInt32(file, address + 4);
            address = file.GetPointer(address, imageBase);
            SA1LevelAct[] result = new SA1LevelAct[lvlcnt];
            for (int i = 0; i < lvlcnt; i++)
            {
                result[i] = new SA1LevelAct(file[address], file[address + 1]);
                address += 2;
            }
            return result;
        }

        public static void Save(SA1LevelAct[] levellist, string filename)
        {
            List<string> result = new List<string>(levellist.Length);
            foreach (SA1LevelAct item in levellist)
                result.Add(item.ToString());
            File.WriteAllLines(filename, result.ToArray());
        }
    }

    public static class BossLevelList
    {
        public static SA1LevelAct[] Load(string filename)
        {
            string[] data = File.ReadAllLines(filename);
            List<SA1LevelAct> result = new List<SA1LevelAct>(data.Length);
            foreach (string item in data)
                if (!string.IsNullOrEmpty(item))
                    result.Add(new SA1LevelAct(item));
            return result.ToArray();
        }

        public static SA1LevelAct[] Load(byte[] file, int address)
        {
            List<SA1LevelAct> result = new List<SA1LevelAct>();
            while (ByteConverter.ToUInt16(file, address) != (ushort)SA1LevelIDs.Invalid)
            {
                result.Add(new SA1LevelAct(ByteConverter.ToUInt16(file, address), ByteConverter.ToUInt16(file, address + 2)));
                address += 4;
            }
            return result.ToArray();
        }

        public static void Save(SA1LevelAct[] levellist, string filename)
        {
            List<string> result = new List<string>(levellist.Length);
            foreach (SA1LevelAct item in levellist)
                result.Add(item.ToString());
            File.WriteAllLines(filename, result.ToArray());
        }

        public static byte[] GetBytes(SA1LevelAct[] levellist)
        {
            List<byte> result = new List<byte>(4 * (levellist.Length + 1));
            foreach (SA1LevelAct item in levellist)
            {
                result.AddRange(ByteConverter.GetBytes((ushort)item.Level));
                result.AddRange(ByteConverter.GetBytes((ushort)item.Act));
            }
            result.AddRange(ByteConverter.GetBytes((ushort)SA1LevelIDs.Invalid));
            result.AddRange(new byte[2]);
            return result.ToArray();
        }
    }

    public static class FieldStartPosList
    {
        public static int Size { get { return FieldStartPosInfo.Size + 2; } }

        public static Dictionary<SA1LevelIDs, FieldStartPosInfo> Load(string filename)
        {
            return IniSerializer.Deserialize<Dictionary<SA1LevelIDs, FieldStartPosInfo>>(filename);
        }

        public static Dictionary<SA1LevelIDs, FieldStartPosInfo> Load(byte[] file, int address)
        {
            Dictionary<SA1LevelIDs, FieldStartPosInfo> result = new Dictionary<SA1LevelIDs, FieldStartPosInfo>();
            while ((SA1LevelIDs)file[address] != SA1LevelIDs.Invalid)
            {
                FieldStartPosInfo objgrp = new FieldStartPosInfo(file, address + 2);
                result.Add((SA1LevelIDs)file[address], objgrp);
                address += Size;
            }
            return result;
        }

        public static void Save(this Dictionary<SA1LevelIDs, FieldStartPosInfo> FieldStartPos, string filename)
        {
            IniSerializer.Serialize(FieldStartPos, filename);
        }

        public static byte[] GetBytes(this Dictionary<SA1LevelIDs, FieldStartPosInfo> FieldStartPos)
        {
            List<byte> result = new List<byte>(Size * (FieldStartPos.Count + 1));
            foreach (KeyValuePair<SA1LevelIDs, FieldStartPosInfo> item in FieldStartPos)
            {
                result.Add((byte)item.Key);
                result.Add(0);
                result.AddRange(item.Value.GetBytes());
            }
            result.AddRange(ByteConverter.GetBytes((ushort)SA1LevelIDs.Invalid));
            result.AddRange(new byte[FieldStartPosInfo.Size]);
            return result.ToArray();
        }

        public static string ToStruct(this KeyValuePair<SA1LevelIDs, FieldStartPosInfo> startpos)
        {
            StringBuilder result = new StringBuilder("{ ");
            result.Append(startpos.Key.ToC("LevelIDs"));
            result.Append(", ");
            result.Append(startpos.Value.Field.ToC("LevelIDs"));
            result.Append(", ");
            result.Append(startpos.Value.Position.ToStruct());
            result.Append(", ");
            result.Append(startpos.Value.YRotation.ToCHex());
            result.Append(" }");
            return result.ToString();
        }
    }

    [Serializable]
    public class FieldStartPosInfo
    {
        public FieldStartPosInfo() { Position = new Vertex(); }

        public FieldStartPosInfo(byte[] file, int address)
        {
            Field = (SA1LevelIDs)file[address];
            address += 2;
            Position = new Vertex(file, address);
            address += Vertex.Size;
            YRotation = ByteConverter.ToInt32(file, address);
        }

        [IniAlwaysInclude]
        public SA1LevelIDs Field { get; set; }
        public Vertex Position { get; set; }
		[TypeConverter(typeof(Int32HexConverter))]
		public int YRotation { get; set; }

        public static int Size { get { return 0x12; } }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>(Size);
            result.Add((byte)Field);
            result.Add(0);
            result.AddRange(Position.GetBytes());
            result.AddRange(ByteConverter.GetBytes(YRotation));
            return result.ToArray();
        }
    }

    public static class SoundTestList
    {
        public static SoundTestListEntry[] Load(string filename)
        {
            return IniSerializer.Deserialize<SoundTestListEntry[]>(filename);
        }

        public static SoundTestListEntry[] Load(byte[] file, int address, uint imageBase)
        {
            int numobjs = ByteConverter.ToInt32(file, address + 4);
            address = file.GetPointer(address, imageBase);
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
            IniSerializer.Serialize(soundlist, filename);
        }
    }

    [Serializable]
    public class SoundTestListEntry
    {
        public SoundTestListEntry() { Title = string.Empty; }
        public SoundTestListEntry(byte[] file, int address, uint imageBase)
        {
            Title = file.GetCString(file.GetPointer(address, imageBase));
            Track = ByteConverter.ToInt32(file, address + 4);
        }

        public string Title { get; set; }
        [IniAlwaysInclude]
        public int Track { get; set; }

        public static int Size { get { return 8; } }

        public byte[] GetBytes(uint titleAddress)
        {
            List<byte> result = new List<byte>(Size);
            result.AddRange(ByteConverter.GetBytes(titleAddress));
            result.AddRange(ByteConverter.GetBytes(Track));
            return result.ToArray();
        }

        public string ToStruct()
        {
            StringBuilder result = new StringBuilder("{ ");
            result.Append(Title.ToC());
            result.Append(", ");
            result.Append(Track);
            result.Append(" }");
            return result.ToString();
        }
    }

    public static class MusicList
    {
        public static MusicListEntry[] Load(string filename)
        {
            return IniSerializer.Deserialize<MusicListEntry[]>(filename);
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
            IniSerializer.Serialize(soundlist, filename);
        }
    }

    [Serializable]
    public class MusicListEntry
    {
        public MusicListEntry() { Filename = string.Empty; }
        public MusicListEntry(byte[] file, int address, uint imageBase)
        {
            Filename = file.GetCString(file.GetPointer(address, imageBase));
            Loop = Convert.ToBoolean(ByteConverter.ToInt32(file, address + 4));
        }

        public string Filename { get; set; }
        public bool Loop { get; set; }

        public static int Size { get { return 8; } }

        public byte[] GetBytes(uint titleAddress)
        {
            List<byte> result = new List<byte>(Size);
            result.AddRange(ByteConverter.GetBytes(titleAddress));
            result.AddRange(ByteConverter.GetBytes(Convert.ToInt32(Loop)));
            return result.ToArray();
        }

        public string ToStruct()
        {
            StringBuilder result = new StringBuilder("{ ");
            result.Append(Filename.ToC());
            result.Append(", ");
            result.Append(Loop ? 1 : 0);
            result.Append(" }");
            return result.ToString();
        }
    }

    public static class SoundList
    {
        public static SoundListEntry[] Load(string filename)
        {
            return IniSerializer.Deserialize<SoundListEntry[]>(filename);
        }

        public static SoundListEntry[] Load(byte[] file, int address, uint imageBase)
        {
            int numobjs = ByteConverter.ToInt32(file, address);
            address = file.GetPointer(address + 4, imageBase);
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
            IniSerializer.Serialize(soundlist, filename);
        }
    }

    [Serializable]
    public class SoundListEntry
    {
        public SoundListEntry() { Filename = string.Empty; }
        public SoundListEntry(byte[] file, int address, uint imageBase)
        {
            Bank = ByteConverter.ToInt32(file, address);
            Filename = file.GetCString(file.GetPointer(address + 4, imageBase));
        }

        [IniAlwaysInclude]
        public int Bank { get; set; }
        public string Filename { get; set; }

        public static int Size { get { return 8; } }

        public byte[] GetBytes(uint filenameAddress)
        {
            List<byte> result = new List<byte>(Size);
            result.AddRange(ByteConverter.GetBytes(Bank));
            result.AddRange(ByteConverter.GetBytes(filenameAddress));
            return result.ToArray();
        }

        public string ToStruct()
        {
            StringBuilder result = new StringBuilder("{ ");
            result.Append(Bank);
            result.Append(", ");
            result.Append(Filename.ToC());
            result.Append(" }");
            return result.ToString();
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

        public static string[] Load(byte[] file, int address, uint imageBase, int length, Languages language)
        {
            return Load(file, address, imageBase, length, HelperFunctions.GetEncoding(language));
        }

        public static string[] Load(byte[] file, int address, uint imageBase, int length, Encoding encoding)
        {
            string[] result = new string[length];
            for (int i = 0; i < length; i++)
            {
                uint straddr = ByteConverter.ToUInt32(file, address);
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
            return IniSerializer.Deserialize<NextLevelListEntry[]>(filename);
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
            IniSerializer.Serialize(levellist, filename);
        }

        public static byte[] GetBytes(this NextLevelListEntry[] levellist)
        {
            List<byte> result = new List<byte>();
            for (int i = 0; i < levellist.Length; i++)
                result.AddRange(levellist[i].GetBytes());
            result.AddRange(new NextLevelListEntry() { Level = (SA1LevelIDs)byte.MaxValue }.GetBytes());
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
            Level = (SA1LevelIDs)file[address + 1];
            NextLevel = ((SA1LevelIDs)file[address + 2]);
            NextAct = file[address + 3];
            StartPos = file[address + 4];
            AltNextLevel = (SA1LevelIDs)file[address + 5];
            AltNextAct = file[address + 6];
            AltStartPos = file[address + 7];
        }

        [IniAlwaysInclude]
        public byte CGMovie { get; set; }
        [IniAlwaysInclude]
        public SA1LevelIDs Level { get; set; }
        [IniAlwaysInclude]
        public SA1LevelIDs NextLevel { get; set; }
        [IniAlwaysInclude]
        public byte NextAct { get; set; }
        [IniAlwaysInclude]
        public byte StartPos { get; set; }
        [IniAlwaysInclude]
        public SA1LevelIDs AltNextLevel { get; set; }
        [IniAlwaysInclude]
        public byte AltNextAct { get; set; }
        [IniAlwaysInclude]
        public byte AltStartPos { get; set; }

        public static int Size { get { return 8; } }

        public byte[] GetBytes()
        {
            return new byte[] { CGMovie, (byte)Level, (byte)NextLevel, NextAct, StartPos, (byte)AltNextLevel, AltNextAct, AltStartPos };
        }

        public string ToStruct()
        {
            StringBuilder result = new StringBuilder("{ ");
            result.Append(CGMovie);
            result.Append(", ");
            result.Append(Level.ToC("LevelIDs"));
            result.Append(", ");
            result.Append(NextLevel.ToC("LevelIDs"));
            result.Append(", ");
            result.Append(NextAct);
            result.Append(", ");
            result.Append(StartPos);
            result.Append(", ");
            result.Append(AltNextLevel.ToC("LevelIDs"));
            result.Append(", ");
            result.Append(AltNextAct);
            result.Append(", ");
            result.Append(AltStartPos);
            result.Append(" }");
            return result.ToString();
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
                Text[i] = StringArray.Load(file, file.GetPointer(address, imageBase), imageBase, length,
                    (Languages)i);
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

    [Serializable]
    public static class RecapScreenList
    {
        public static RecapScreen[][] Load(string directory, int length)
        {
            RecapScreen[][] screens = new RecapScreen[length][];
            for (int i = 0; i < length; i++)
            {
                screens[i] = new RecapScreen[5];
                for (int l = 0; l < 5; l++)
                    screens[i][l] = IniSerializer.Deserialize<RecapScreen>(Path.Combine(Path.Combine(directory, (i + 1).ToString(NumberFormatInfo.InvariantInfo)), ((Languages)l).ToString() + ".ini"));
            }
            return screens;
        }

        public static RecapScreen[][] Load(byte[] file, int address, uint imageBase, int length)
        {
            RecapScreen[][] screens = new RecapScreen[length][];
            for (int i = 0; i < length; i++)
            {
                screens[i] = new RecapScreen[5];
                for (int l = 0; l < 5; l++)
                {
                    int tmpaddr = file.GetPointer(address + (l * 4), imageBase);
                    tmpaddr += i * 0xC;
                    screens[i][l] = new RecapScreen(file, tmpaddr, imageBase, (Languages)l);
                }
            }
            return screens;
        }

        public static void Save(this RecapScreen[][] list, string directory)
        {
            string[][] hashes;
            Save(list, directory, out hashes);
        }

        public static void Save(this RecapScreen[][] list, string directory, out string[][] hashes)
        {
            hashes = new string[list.Length][];
            Directory.CreateDirectory(directory);
            for (int i = 0; i < list.Length; i++)
            {
                string scrname = Path.Combine(directory, (i + 1).ToString(NumberFormatInfo.InvariantInfo));
                Directory.CreateDirectory(scrname);
                hashes[i] = new string[5];
                for (int l = 0; l < 5; l++)
                {
                    string textname = Path.Combine(scrname, ((Languages)l).ToString() + ".ini");
                    IniSerializer.Serialize(list[i][l], textname);
                    hashes[i][l] = HelperFunctions.FileHash(textname);
                }
            }
        }
    }

    public class RecapScreen
    {
        public RecapScreen()
		{
			Text = string.Empty;
		}

        public RecapScreen(byte[] file, int address, uint imageBase, Languages language)
        {
            Speed = ByteConverter.ToSingle(file, address);
            Text = string.Join("\n", StringArray.Load(file, (int)(ByteConverter.ToUInt32(file, address + 8) - imageBase), imageBase, ByteConverter.ToInt32(file, address + 4), language));
        }

        [IniAlwaysInclude]
        public float Speed { get; set; }
        [IniAlwaysInclude]
        public string Text { get; set; }
    }

    public static class NPCTextList
    {
        public static NPCText[][] Load(string directory, int length)
        {
            NPCText[][] screens = new NPCText[5][];
            for (int l = 0; l < 5; l++)
            {
                screens[l] = new NPCText[length];
                for (int i = 0; i < length; i++)
                    screens[l][i] = IniSerializer.Deserialize<NPCText>(Path.Combine(Path.Combine(directory, (i + 1).ToString(NumberFormatInfo.InvariantInfo)), ((Languages)l).ToString() + ".ini"));
            }
            return screens;
        }

        public static NPCText[][] Load(byte[] file, int address, uint imageBase, int length)
        {
            NPCText[][] screens = new NPCText[5][];
            for (int l = 0; l < 5; l++)
                screens[l] = Load(file, file.GetPointer(address + (l * 4), imageBase), imageBase, length, (Languages)l, true);
            return screens;
        }

        public static NPCText[] Load(byte[] file, int address, uint imageBase, int length, Languages language, bool includeTime)
        {
            NPCText[] screen = new NPCText[length];
            for (int i = 0; i < length; i++)
            {
                screen[i] = new NPCText(file, address, imageBase, language, includeTime);
                address += 8;
            }
            return screen;
        }

        public static void Save(this NPCText[][] list, string directory)
        {
            string[][] hashes;
            Save(list, directory, out hashes);
        }

        public static void Save(this NPCText[][] list, string directory, out string[][] hashes)
        {
            hashes = new string[5][];
            Directory.CreateDirectory(directory);
            for (int l = 0; l < 5; l++)
            {
                hashes[l] = new string[list[l].Length];
                for (int i = 0; i < list[l].Length; i++)
                {
                    string scrname = Path.Combine(directory, (i + 1).ToString(NumberFormatInfo.InvariantInfo));
                    Directory.CreateDirectory(scrname);
                    string textname = Path.Combine(scrname, ((Languages)l).ToString() + ".ini");
                    IniSerializer.Serialize(list[l][i], textname);
                    hashes[l][i] = HelperFunctions.FileHash(textname);
                }
            }
        }
    }

    public class NPCText
    {
        public NPCText()
        {
            Groups = new List<NPCTextGroup>();
        }

		public NPCText(byte[] file, int address, uint imageBase, Languages language, bool includeTime)
			: this()
		{
			NPCTextGroup group = new NPCTextGroup();
			int add = includeTime ? 8 : 4;
			bool hasText = ByteConverter.ToUInt32(file, address + 4) != 0;
			int textaddr = 0;
			if (hasText)
				textaddr = file.GetPointer(address + 4, imageBase);
			if (ByteConverter.ToUInt32(file, address) == 0)
			{
				if (!hasText)
					return;
				while (ByteConverter.ToInt32(file, textaddr) != 0)
				{
					group.Lines.Add(new NPCTextLine(file, textaddr, imageBase, language, includeTime));
					textaddr += add;
				}
				Groups.Add(group);
				return;
			}
			int controladdr = file.GetPointer(address, imageBase);
		newgroup:
			if (hasText)
			{
				while (ByteConverter.ToInt32(file, textaddr) != 0)
				{
					group.Lines.Add(new NPCTextLine(file, textaddr, imageBase, language, includeTime));
					textaddr += add;
				}
				textaddr += add;
			}
			while (true)
			{
				NPCTextControl code = (NPCTextControl)ByteConverter.ToInt16(file, controladdr);
				controladdr += sizeof(short);
				switch (code)
				{
					case NPCTextControl.EventFlag:
						group.EventFlags.Add(ByteConverter.ToUInt16(file, controladdr));
						controladdr += sizeof(short);
						break;
					case NPCTextControl.NPCFlag:
						group.NPCFlags.Add(ByteConverter.ToUInt16(file, controladdr));
						controladdr += sizeof(short);
						break;
					case NPCTextControl.Character:
						group.Character = (SA1CharacterFlags)ByteConverter.ToUInt16(file, controladdr);
						controladdr += sizeof(short);
						break;
					case NPCTextControl.Voice:
						group.Voice = ByteConverter.ToUInt16(file, controladdr);
						controladdr += sizeof(short);
						break;
					case NPCTextControl.SetEventFlag:
						group.SetEventFlag = ByteConverter.ToUInt16(file, controladdr);
						controladdr += sizeof(short);
						break;
					case NPCTextControl.NewGroup:
						Groups.Add(group);
						group = new NPCTextGroup();
						goto newgroup;
					case NPCTextControl.End:
						Groups.Add(group);
						return;
				}
			}
		}

        [IniCollection(IniCollectionMode.IndexOnly)]
        public List<NPCTextGroup> Groups { get; set; }

        [IniIgnore]
        public bool HasControl
        {
            get
            {
                if (Groups.Count > 1) return true;
                foreach (NPCTextGroup item in Groups)
                    if (item.HasControl) return true;
                return false;
            }
        }

        [IniIgnore]
        public bool HasText
        {
            get
            {
                foreach (NPCTextGroup item in Groups)
                    if (item.HasText)
                        return true;
                return false;
            }
        }
    }

    public enum NPCTextControl : short
    {
        EventFlag = -7,
        NPCFlag = -6,
        Character = -5,
        Voice = -4,
        SetEventFlag = -3,
        NewGroup = -2,
        End = -1
    }

    public class NPCTextGroup
	{
        public NPCTextGroup()
        {
            EventFlags = new List<ushort>();
            NPCFlags = new List<ushort>();
            Character = (SA1CharacterFlags)0xFF;
            Voice = null;
            SetEventFlag = null;
            Lines = new List<NPCTextLine>();
        }

		[IniCollection(IniCollectionMode.SingleLine, Format=", ")]
        public List<ushort> EventFlags { get; set; }
		[IniCollection(IniCollectionMode.SingleLine, Format=", ")]
        public List<ushort> NPCFlags { get; set; }
        [DefaultValue((SA1CharacterFlags)0xFF)]
        [IniAlwaysInclude]
        public SA1CharacterFlags Character { get; set; }
        public ushort? Voice { get; set; }
        public ushort? SetEventFlag { get; set; }
        public List<NPCTextLine> Lines { get; set; }

        [IniIgnore]
		public bool HasControl
		{
			get
			{
				return EventFlags.Count > 0 || NPCFlags.Count > 0
					|| Character != (SA1CharacterFlags)0xFF
					|| Voice.HasValue || SetEventFlag.HasValue;
			}
		}

		[IniIgnore]
        public bool HasText { get { return Lines.Count > 0; } }
	}

    public class NPCTextLine
	{
        public NPCTextLine()
		{
			Line = string.Empty;
		}

        public NPCTextLine(byte[] file, int address, uint imageBase, Languages language, bool includeTime)
        {
            Line = file.GetCString(file.GetPointer(address, imageBase), HelperFunctions.GetEncoding(language));
            if (includeTime)
                Time = ByteConverter.ToInt32(file, address + 4);
        }

        [IniAlwaysInclude]
        public string Line { get; set; }
        [IniAlwaysInclude]
        public int Time { get; set; }

        public string ToStruct(Languages lang)
        {
            return string.Format("{{ {0}, {1} }}", Line.ToC(lang), Time);
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
            while (ByteConverter.ToUInt16(file, address) != ushort.MaxValue)
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
            result.AddRange(ByteConverter.GetBytes(uint.MaxValue));
            return result.ToArray();
        }
    }

    public class LevelClearFlag
    {
        public LevelClearFlag() { }

        public LevelClearFlag(byte[] file, int address)
        {
            Level = (SA1LevelIDs)ByteConverter.ToUInt16(file, address);
            Flag = ByteConverter.ToUInt16(file, address + 2);
        }

        public LevelClearFlag(string data)
        {
            string[] splitline = data.Split(' ');
            Level = (SA1LevelIDs)Enum.Parse(typeof(SA1LevelIDs), splitline[0]);
            Flag = ushort.Parse(splitline[1], NumberStyles.HexNumber);
        }

        [IniAlwaysInclude]
        public SA1LevelIDs Level { get; set; }
        [IniAlwaysInclude]
        public ushort Flag { get; set; }

        public static int Size { get { return 4; } }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>(Size);
            result.AddRange(ByteConverter.GetBytes((ushort)Level));
            result.AddRange(ByteConverter.GetBytes(Flag));
            return result.ToArray();
        }

        public override string ToString()
        {
            return Level.ToString() + " " + Flag.ToString("X4");
        }

        public string ToStruct()
        {
            return string.Format("{{ {0}, {1} }}", Level.ToC("LevelIDs"), Flag.ToCHex());
        }
    }

    public static class DeathZoneFlagsList
    {
        public static DeathZoneFlags[] Load(string filename)
        {
            return IniSerializer.Deserialize<DeathZoneFlags[]>(filename);
        }

        public static void Save(this DeathZoneFlags[] flags, string filename)
        {
            IniSerializer.Serialize(flags, filename);
        }
    }

    [Serializable]
    public class DeathZoneFlags
    {
        public DeathZoneFlags() { }

        public DeathZoneFlags(byte[] file, int address)
        {
            Flags = (SA1CharacterFlags)ByteConverter.ToInt32(file, address);
        }

        [IniAlwaysInclude]
        public SA1CharacterFlags Flags { get; set; }

        public static int Size { get { return 4; } }

        public byte[] GetBytes()
        {
            return ByteConverter.GetBytes((int)Flags);
        }
    }

    public static class SkyboxScaleList
    {
        public static SkyboxScale[] Load(string filename)
        {
            return IniSerializer.Deserialize<SkyboxScale[]>(filename);
        }

        public static SkyboxScale[] Load(byte[] file, int address, uint imageBase, int count)
        {
            List<SkyboxScale> result = new List<SkyboxScale>(count);
            for (int i = 0; i < count; i++)
            {
                if (ByteConverter.ToUInt32(file, address) != 0)
                    result.Add(new SkyboxScale(file, file.GetPointer(address, imageBase)));
                else
                    result.Add(new SkyboxScale());
                address += 4;
            }
            return result.ToArray();
        }

        public static void Save(this SkyboxScale[] scales, string filename)
        {
            IniSerializer.Serialize(scales, filename);
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

        public string ToStruct()
        {
            return string.Format("{{ {0}, {1}, {2} }}", Far.ToStruct(), Normal.ToStruct(), Near.ToStruct());
        }
    }

    public static class StageSelectLevelList
    {
        public static StageSelectLevel[] Load(string filename)
        {
            return IniSerializer.Deserialize<StageSelectLevel[]>(filename);
        }

        public static StageSelectLevel[] Load(byte[] file, int address, int count)
        {
            StageSelectLevel[] result = new StageSelectLevel[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = new StageSelectLevel(file, address);
                address += StageSelectLevel.Size;
            }
            return result;
        }

        public static void Save(this StageSelectLevel[] levellist, string filename)
        {
            IniSerializer.Serialize(levellist, filename);
        }
    }

    [Serializable]
    public class StageSelectLevel
    {
        [IniAlwaysInclude]
        public SA2LevelIDs Level { get; set; }
        [IniAlwaysInclude]
        public SA2Characters Character { get; set; }
        [IniAlwaysInclude]
        public int Column { get; set; }
        [IniAlwaysInclude]
        public int Row { get; set; }

        public static int Size { get { return 16; } }

        public StageSelectLevel() { }

        public StageSelectLevel(byte[] file, int address)
        {
            Level = (SA2LevelIDs)ByteConverter.ToInt32(file, address);
            Character = (SA2Characters)ByteConverter.ToInt32(file, address + 4);
            Column = ByteConverter.ToInt32(file, address + 8);
            Row = ByteConverter.ToInt32(file, address + 12);
        }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>(Size);
            result.AddRange(ByteConverter.GetBytes((int)Level));
            result.AddRange(ByteConverter.GetBytes((int)Character));
            result.AddRange(ByteConverter.GetBytes(Column));
            result.AddRange(ByteConverter.GetBytes(Row));
            return result.ToArray();
        }

        public string ToStruct()
        {
            return string.Format("{{ {0}, {1}, {2}, {3} }}", Level.ToC("LevelIDs"), Character.ToC("Characters"), Column, Row);
        }
    }

    public static class LevelRankScoresList
    {
        public static int Size { get { return LevelRankScores.Size + 2; } }

        public static Dictionary<SA2LevelIDs, LevelRankScores> Load(string filename)
        {
            return IniSerializer.Deserialize<Dictionary<SA2LevelIDs, LevelRankScores>>(filename);
        }

        public static Dictionary<SA2LevelIDs, LevelRankScores> Load(byte[] file, int address)
        {
            Dictionary<SA2LevelIDs, LevelRankScores> result = new Dictionary<SA2LevelIDs, LevelRankScores>();
            while (ByteConverter.ToUInt16(file, address) != (ushort)SA2LevelIDs.Invalid)
            {
                LevelRankScores objgrp = new LevelRankScores(file, address + 2);
                result.Add((SA2LevelIDs)ByteConverter.ToUInt16(file, address), objgrp);
                address += Size;
            }
            return result;
        }

        public static void Save(this Dictionary<SA2LevelIDs, LevelRankScores> startpos, string filename)
        {
            IniSerializer.Serialize(startpos, filename);
        }

        public static byte[] GetBytes(this Dictionary<SA2LevelIDs, LevelRankScores> startpos)
        {
            List<byte> result = new List<byte>(Size * startpos.Count + 2);
            foreach (KeyValuePair<SA2LevelIDs, LevelRankScores> item in startpos)
            {
                result.AddRange(ByteConverter.GetBytes((ushort)item.Key));
                result.AddRange(item.Value.GetBytes());
            }
            result.AddRange(ByteConverter.GetBytes((ushort)SA2LevelIDs.Invalid));
            return result.ToArray();
        }

        public static string ToStruct(this KeyValuePair<SA2LevelIDs, LevelRankScores> item)
        {
            return string.Format("{{ {0}, {1}, {2}, {3}, {4} }}", item.Key.ToC("LevelIDs"),
                item.Value.DRank, item.Value.CRank, item.Value.BRank, item.Value.ARank);
        }
    }

    [Serializable]
    public class LevelRankScores
    {
        public LevelRankScores() { }

        public LevelRankScores(byte[] file, int address)
        {
            DRank = ByteConverter.ToUInt16(file, address);
            address += sizeof(ushort);
            CRank = ByteConverter.ToUInt16(file, address);
            address += sizeof(ushort);
            BRank = ByteConverter.ToUInt16(file, address);
            address += sizeof(ushort);
            ARank = ByteConverter.ToUInt16(file, address);
        }

        public ushort DRank { get; set; }
        public ushort CRank { get; set; }
        public ushort BRank { get; set; }
        public ushort ARank { get; set; }

        public static int Size { get { return sizeof(ushort) * 4; } }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>(Size);
            result.AddRange(ByteConverter.GetBytes(DRank));
            result.AddRange(ByteConverter.GetBytes(CRank));
            result.AddRange(ByteConverter.GetBytes(BRank));
            result.AddRange(ByteConverter.GetBytes(ARank));
            return result.ToArray();
        }
    }

    public static class LevelRankTimesList
    {
        public static int Size { get { return LevelRankTimes.Size + 1; } }

        public static Dictionary<SA2LevelIDs, LevelRankTimes> Load(string filename)
        {
            return IniSerializer.Deserialize<Dictionary<SA2LevelIDs, LevelRankTimes>>(filename);
        }

        public static Dictionary<SA2LevelIDs, LevelRankTimes> Load(byte[] file, int address)
        {
            Dictionary<SA2LevelIDs, LevelRankTimes> result = new Dictionary<SA2LevelIDs, LevelRankTimes>();
            while (file[address] != (byte)SA2LevelIDs.Invalid)
            {
                LevelRankTimes objgrp = new LevelRankTimes(file, address + 1);
                result.Add((SA2LevelIDs)file[address], objgrp);
                address += Size;
            }
            return result;
        }

        public static void Save(this Dictionary<SA2LevelIDs, LevelRankTimes> startpos, string filename)
        {
            IniSerializer.Serialize(startpos, filename);
        }

        public static byte[] GetBytes(this Dictionary<SA2LevelIDs, LevelRankTimes> startpos)
        {
            List<byte> result = new List<byte>(Size * startpos.Count + 1);
            foreach (KeyValuePair<SA2LevelIDs, LevelRankTimes> item in startpos)
            {
                result.Add((byte)item.Key);
                result.AddRange(item.Value.GetBytes());
            }
            result.Add((byte)SA2LevelIDs.Invalid);
            return result.ToArray();
        }

        public static string ToStruct(this KeyValuePair<SA2LevelIDs, LevelRankTimes> item)
        {
            return string.Format("{{ {0}, {1}, {2}, {3}, {4} }}", item.Key.ToC("LevelIDs"),
                item.Value.DRank.ToStruct(), item.Value.CRank.ToStruct(), item.Value.BRank.ToStruct(), item.Value.ARank.ToStruct());
        }
    }

    [Serializable]
    [TypeConverter(typeof(StringConverter<MinSec>))]
    public struct MinSec
    {
        public MinSec(byte minute, byte second)
            : this()
        {
            Minute = minute;
            Second = second;
        }

        public MinSec(byte[] file, int address)
            : this()
        {
            Minute = file[address++];
            Second = file[address];
        }

        public MinSec(string minsec)
            : this()
        {
            string[] split = minsec.Split(':');
            Minute = byte.Parse(split[0], NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
            Second = byte.Parse(split[1], NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
        }

        public byte Minute { get; set; }
        public byte Second { get; set; }

        public static int Size { get { return 2; } }

        public byte[] GetBytes() { return new byte[] { Minute, Second }; }

        public override string ToString()
        {
            return Minute.ToString(NumberFormatInfo.InvariantInfo) + ':' + Second.ToString("00", NumberFormatInfo.InvariantInfo);
        }

        public string ToStruct()
        {
            return string.Format("{{ {0}, {1} }}", Minute, Second);
        }
    }

    [Serializable]
    public class LevelRankTimes
    {
        public LevelRankTimes() { }

        public LevelRankTimes(byte[] file, int address)
        {
            DRank = new MinSec(file, address);
            address += MinSec.Size;
            CRank = new MinSec(file, address);
            address += MinSec.Size;
            BRank = new MinSec(file, address);
            address += MinSec.Size;
            ARank = new MinSec(file, address);
        }

        public MinSec DRank { get; set; }
        public MinSec CRank { get; set; }
        public MinSec BRank { get; set; }
        public MinSec ARank { get; set; }

        public static int Size { get { return MinSec.Size * 4; } }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>(Size);
            result.AddRange(DRank.GetBytes());
            result.AddRange(CRank.GetBytes());
            result.AddRange(BRank.GetBytes());
            result.AddRange(ARank.GetBytes());
            return result.ToArray();
        }
    }

    public static class SA2EndPosList
    {
        public static int Size { get { return SA2EndPosInfo.Size + 2; } }

        public static Dictionary<SA2LevelIDs, SA2EndPosInfo> Load(string filename)
        {
            return IniSerializer.Deserialize<Dictionary<SA2LevelIDs, SA2EndPosInfo>>(filename);
        }

        public static Dictionary<SA2LevelIDs, SA2EndPosInfo> Load(byte[] file, int address)
        {
            Dictionary<SA2LevelIDs, SA2EndPosInfo> result = new Dictionary<SA2LevelIDs, SA2EndPosInfo>();
            while (ByteConverter.ToUInt16(file, address) != (ushort)SA2LevelIDs.Invalid)
            {
                SA2EndPosInfo objgrp = new SA2EndPosInfo(file, address + 2);
                result.Add((SA2LevelIDs)ByteConverter.ToUInt16(file, address), objgrp);
                address += Size;
            }
            return result;
        }

        public static void Save(this Dictionary<SA2LevelIDs, SA2EndPosInfo> EndPos, string filename)
        {
            IniSerializer.Serialize(EndPos, filename);
        }

        public static byte[] GetBytes(this Dictionary<SA2LevelIDs, SA2EndPosInfo> EndPos)
        {
            List<byte> result = new List<byte>(Size * (EndPos.Count + 1));
            foreach (KeyValuePair<SA2LevelIDs, SA2EndPosInfo> item in EndPos)
            {
                result.AddRange(ByteConverter.GetBytes((ushort)item.Key));
                result.AddRange(item.Value.GetBytes());
            }
            result.AddRange(ByteConverter.GetBytes((ushort)SA2LevelIDs.Invalid));
            result.AddRange(new byte[SA2EndPosInfo.Size]);
            return result.ToArray();
        }

        public static string ToStruct(this KeyValuePair<SA2LevelIDs, SA2EndPosInfo> item)
        {
            return string.Format("{{ {0}, {1}, {2}, {3}, {4}, {5} }}", item.Key.ToC("LevelIDs"), item.Value.Mission2YRotation.ToCHex(),
                item.Value.Mission3YRotation.ToCHex(), item.Value.Unknown.ToCHex(), item.Value.Mission2Position.ToStruct(),
                item.Value.Mission3Position.ToStruct());
        }
    }

    [Serializable]
    public class SA2EndPosInfo
    {
        public SA2EndPosInfo()
        {
            Mission2Position = new Vertex();
            Mission3Position = new Vertex();
        }

        public SA2EndPosInfo(byte[] file, int address)
        {
            Mission2YRotation = ByteConverter.ToUInt16(file, address);
            address += sizeof(ushort);
            Mission3YRotation = ByteConverter.ToUInt16(file, address);
            address += sizeof(ushort);
            Unknown = ByteConverter.ToUInt16(file, address);
            address += sizeof(ushort);
            Mission2Position = new Vertex(file, address);
            address += Vertex.Size;
            Mission3Position = new Vertex(file, address);
            address += Vertex.Size;
        }

		[TypeConverter(typeof(UInt16HexConverter))]
		public ushort Mission2YRotation { get; set; }
		[TypeConverter(typeof(UInt16HexConverter))]
		public ushort Mission3YRotation { get; set; }
		[TypeConverter(typeof(UInt16HexConverter))]
		public ushort Unknown { get; set; }
        public Vertex Mission2Position { get; set; }
        public Vertex Mission3Position { get; set; }

        public static int Size { get { return (sizeof(ushort) * 3) + (Vertex.Size * 2); } }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>(Size);
            result.AddRange(ByteConverter.GetBytes(Mission2YRotation));
            result.AddRange(ByteConverter.GetBytes(Mission3YRotation));
            result.AddRange(ByteConverter.GetBytes(Unknown));
            result.AddRange(Mission2Position.GetBytes());
            result.AddRange(Mission3Position.GetBytes());
            return result.ToArray();
        }
    }

    public static class SA2AnimationInfoList
    {
        public static SA2AnimationInfo[] Load(string filename)
        {
            return IniSerializer.Deserialize<SA2AnimationInfo[]>(filename);
        }

        public static SA2AnimationInfo[] Load(byte[] file, int address, int count)
        {
            SA2AnimationInfo[] result = new SA2AnimationInfo[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = new SA2AnimationInfo(file, address);
                address += SA2AnimationInfo.Size;
            }
            return result;
        }

        public static void Save(this SA2AnimationInfo[] levellist, string filename)
        {
            IniSerializer.Serialize(levellist, filename);
        }

        public static byte[] GetBytes(this SA2AnimationInfo[] levellist)
        {
            List<byte> result = new List<byte>(SA2AnimationInfo.Size * levellist.Length);
            foreach (SA2AnimationInfo item in levellist)
                result.AddRange(item.GetBytes());
            return result.ToArray();
        }
    }

    [Serializable]
    public class SA2AnimationInfo
    {
        [IniAlwaysInclude]
        public ushort Animation { get; set; }
        [IniAlwaysInclude]
        public ushort Unknown1 { get; set; }
        [IniAlwaysInclude]
        public ushort Unknown2 { get; set; }
        [IniAlwaysInclude]
        public ushort NextAnimation { get; set; }
        [IniAlwaysInclude]
        public float TransitionSpeed { get; set; }
        [IniAlwaysInclude]
        public float AnimationSpeed { get; set; }

        public static int Size { get { return 16; } }

        public SA2AnimationInfo() { }

        public SA2AnimationInfo(byte[] file, int address)
        {
            Animation = ByteConverter.ToUInt16(file, address);
            address += sizeof(ushort);
            Unknown1 = ByteConverter.ToUInt16(file, address);
            address += sizeof(ushort);
            Unknown2 = ByteConverter.ToUInt16(file, address);
            address += sizeof(ushort);
            NextAnimation = ByteConverter.ToUInt16(file, address);
            address += sizeof(ushort);
            TransitionSpeed = ByteConverter.ToSingle(file, address);
            address += sizeof(float);
            AnimationSpeed = ByteConverter.ToSingle(file, address);
            address += sizeof(float);
        }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>(Size);
            result.AddRange(ByteConverter.GetBytes(Animation));
            result.AddRange(ByteConverter.GetBytes(Unknown1));
            result.AddRange(ByteConverter.GetBytes(Unknown2));
            result.AddRange(ByteConverter.GetBytes(NextAnimation));
            result.AddRange(ByteConverter.GetBytes(TransitionSpeed));
            result.AddRange(ByteConverter.GetBytes(AnimationSpeed));
            return result.ToArray();
        }

        public string ToStruct()
        {
            return string.Format("{{ {0}, {1}, {2}, {3}, {4}, {5} }}", Animation, Unknown1, Unknown2, NextAnimation,
                TransitionSpeed.ToC(), AnimationSpeed.ToC());
        }
    }

	public static class PathList
	{
		public static List<PathData> Load(string directory)
		{
			List<PathData> result = new List<PathData>();
			int i = 0;
			string filename = Path.Combine(directory, string.Format("{0}.ini", i++));
			while (File.Exists(filename))
			{
				result.Add(PathData.Load(filename));
				filename = Path.Combine(directory, string.Format("{0}.ini", i++));
			}
			return result;
		}

		public static List<PathData> Load(byte[] file, int address, uint imageBase)
		{
			List<PathData> result = new List<PathData>();
			int ptr = ByteConverter.ToInt32(file, address);
			address += 4;
			while (ptr != 0)
			{
				ptr = (int)((uint)ptr - imageBase);
				result.Add(new PathData(file, ptr, imageBase));
				ptr = ByteConverter.ToInt32(file, address);
				address += 4;
			}
			return result;
		}

		public static void Save(this List<PathData> paths, string directory, out string[] hashes)
		{
			Directory.CreateDirectory(directory);
			hashes = new string[paths.Count];
			for (int i = 0; i < paths.Count; i++)
			{
				string filename = Path.Combine(directory, string.Format("{0}.ini", i));
				IniSerializer.Serialize(paths[i], filename);
				hashes[i] = HelperFunctions.FileHash(filename);
			}
		}

		public static byte[] GetBytes(this List<PathData> paths, uint imageBase, out uint dataaddr)
		{
			List<byte> result = new List<byte>();
			List<uint> pointers = new List<uint>();
			foreach (PathData path in paths)
			{
				uint ptr;
				result.AddRange(path.GetBytes(imageBase, out ptr));
				pointers.Add(ptr);
			}
			dataaddr = imageBase + (uint)result.Count;
			foreach (uint item in pointers)
				result.AddRange(ByteConverter.GetBytes(item));
			result.AddRange(new byte[4]);
			return result.ToArray();
		}
	}

	[Serializable]
	public class PathData
	{
		public short Unknown { get; set; }
		public float TotalDistance { get; set; }
		[IniCollection(IniCollectionMode.IndexOnly)]
		public List<PathDataEntry> Path { get; set; }
		[TypeConverter(typeof(UInt32HexConverter))]
		public uint Code { get; set; }

		public PathData() { Path = new List<PathDataEntry>(); }

		public static PathData Load(string filename)
		{
			return IniSerializer.Deserialize<PathData>(filename);
		}

		public PathData(byte[] file, int address, uint imageBase)
		{
			Unknown = ByteConverter.ToInt16(file, address);
			address += sizeof(short);
			ushort count = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			TotalDistance = ByteConverter.ToSingle(file, address);
			address += sizeof(float);
			Path = new List<PathDataEntry>();
			int ptr = ByteConverter.ToInt32(file, address);
			address += sizeof(int);
			if (ptr != 0)
			{
				ptr = (int)((uint)ptr - imageBase);
				for (int i = 0; i < count; i++)
				{
					Path.Add(new PathDataEntry(file, ptr));
					ptr += PathDataEntry.Size;
				}
			}
			Code = ByteConverter.ToUInt32(file, address);
		}

		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}

		public byte[] GetBytes(uint imageBase, out uint dataaddr)
		{
			List<byte> result = new List<byte>(PathDataEntry.Size * Path.Count);
			foreach (PathDataEntry entry in Path)
				result.AddRange(entry.GetBytes());
			dataaddr = imageBase + (uint)result.Count;
			result.AddRange(ByteConverter.GetBytes(Unknown));
			result.AddRange(ByteConverter.GetBytes((ushort)result.Count));
			result.AddRange(ByteConverter.GetBytes(TotalDistance));
			result.AddRange(ByteConverter.GetBytes(imageBase));
			result.AddRange(ByteConverter.GetBytes(Code));
			return result.ToArray();
		}
	}

	[Serializable]
	public class PathDataEntry
	{
		[TypeConverter(typeof(UInt16HexConverter))]
		public ushort XRotation { get; set; }
		[TypeConverter(typeof(UInt16HexConverter))]
		public ushort YRotation { get; set; }
		public float Distance { get; set; }
		public Vertex Position { get; set; }

		public static int Size { get { return (sizeof(ushort) * 2) + sizeof(float) + Vertex.Size; } }

		public PathDataEntry() { Position = new Vertex(); }

		public PathDataEntry(float x, float y, float z)
		{
			Position = new Vertex(x, y, z);
		}

		public PathDataEntry(byte[] file, int address)
		{
			XRotation = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			YRotation = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			Distance = ByteConverter.ToSingle(file, address);
			address += sizeof(float);
			Position = new Vertex(file, address);
		}

		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>();
			result.AddRange(ByteConverter.GetBytes(XRotation));
			result.AddRange(ByteConverter.GetBytes(YRotation));
			result.AddRange(ByteConverter.GetBytes(Distance));
			result.AddRange(Position.GetBytes());
			return result.ToArray();
		}

		public string ToStruct()
		{
			return string.Format("{{ {0}, {1}, {2}, {3} }}", XRotation.ToCHex(), YRotation.ToCHex(), Distance.ToC(),
				Position.ToStruct());
		}
	}

	public static class SA1StageLightDataList
	{
		public static List<SA1StageLightData> Load(string filename)
		{
			return IniSerializer.Deserialize<List<SA1StageLightData>>(filename);
		}

		public static List<SA1StageLightData> Load(byte[] file, int address)
		{
			List<SA1StageLightData> result = new List<SA1StageLightData>();
			while (file[address] != 0xFF)
			{
				result.Add(new SA1StageLightData(file, address));
				address += SA1StageLightData.Size;
			}
			return result;
		}

		public static void Save(this List<SA1StageLightData> startpos, string filename)
		{
			IniSerializer.Serialize(startpos, filename);
		}

		public static byte[] GetBytes(this List<SA1StageLightData> startpos)
		{
			List<byte> result = new List<byte>(SA1StageLightData.Size * (startpos.Count + 1));
			foreach (SA1StageLightData item in startpos)
				result.AddRange(item.GetBytes());
			result.Add(0xFF);
			result.AddRange(new byte[SA1StageLightData.Size - 1]);
			return result.ToArray();
		}
	}

	[Serializable]
	public class SA1StageLightData
	{
		public SA1StageLightData() { Direction = new Vertex(); }

		public SA1StageLightData(byte[] file, int address)
		{
			Level = (SA1LevelIDs)file[address++];
			Act = file[address++];
			LightNum = file[address++];
			UseDirection = file[address++] != 0;
			Direction = new Vertex(file, address);
			address += Vertex.Size;
			Dif = ByteConverter.ToSingle(file, address);
			address += sizeof(float);
			Multiplier = ByteConverter.ToSingle(file, address);
			address += sizeof(float);
			RGB = new Vertex(file, address);
			address += Vertex.Size;
			AmbientRGB = new Vertex(file, address);
			address += Vertex.Size;
		}

		[IniAlwaysInclude]
		public SA1LevelIDs Level { get; set; }
		[IniAlwaysInclude]
		public byte Act { get; set; }
		[IniAlwaysInclude]
		public byte LightNum { get; set; }
		public bool UseDirection { get; set; }
		public Vertex Direction { get; set; }
		public float Dif { get; set; }
		public float Multiplier { get; set; }
		public Vertex RGB { get; set; }
		public Vertex AmbientRGB { get; set; }

		public static int Size { get { return 0x30; } }

		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(Size);
			result.Add((byte)Level);
			result.Add(Act);
			result.Add(LightNum);
			result.Add((byte)(UseDirection ? 1 : 0));
			result.AddRange(Direction.GetBytes());
			result.AddRange(ByteConverter.GetBytes(Dif));
			result.AddRange(ByteConverter.GetBytes(Multiplier));
			result.AddRange(RGB.GetBytes());
			result.AddRange(AmbientRGB.GetBytes());
			return result.ToArray();
		}

		public string ToStruct()
		{
			StringBuilder result = new StringBuilder("{ ");
			result.Append(Level.ToC("LevelIDs"));
			result.Append(", ");
			result.Append(Act);
			result.Append(", ");
			result.Append(LightNum);
			result.Append(", ");
			result.Append(UseDirection ? 1 : 0);
			result.Append(", ");
			result.Append(Direction.ToStruct());
			result.Append(", ");
			result.Append(Dif.ToC());
			result.Append(", ");
			result.Append(Multiplier.ToC());
			result.Append(", ");
			result.Append(RGB.ToStruct());
			result.Append(", ");
			result.Append(AmbientRGB.ToStruct());
			result.Append(" }");
			return result.ToString();
		}
	}

	public static class WeldList
	{
		public static List<WeldInfo> Load(string filename)
		{
			return IniSerializer.Deserialize<List<WeldInfo>>(filename);
		}

		public static List<WeldInfo> Load(byte[] file, int address, uint imageBase)
		{
			List<WeldInfo> result = new List<WeldInfo>();
			int ptr = ByteConverter.ToInt32(file, address);
			while (ptr != 0)
			{
				result.Add(new WeldInfo(file, address, imageBase));
				address += WeldInfo.Size;
				ptr = ByteConverter.ToInt32(file, address);
			}
			return result;
		}

		public static void Save(this List<WeldInfo> welds, string filename)
		{
			IniSerializer.Serialize(welds, filename);
		}
	}

	[Serializable]
	public class WeldInfo
	{
		public string BaseModel { get; set; }
		public string ModelA { get; set; }
		public string ModelB { get; set; }
		[IniAlwaysInclude]
		public byte WeldType { get; set; }
		public short Unknown { get; set; }
		[IniCollection(IniCollectionMode.SingleLine)]
		public List<ushort> VertIndexes { get; set; }
		public string VertIndexName { get; set; }

		public static int Size { get { return 0x18; } }

		public WeldInfo() { }

		public WeldInfo(byte[] file, int address, uint imageBase)
		{
			int ptr = ByteConverter.ToInt32(file, address);
			address += sizeof(int);
			if (ptr != 0)
				if (ptr >= imageBase && (ptr < file.Length + imageBase))
					BaseModel = "object_" + ((uint)ptr - imageBase).ToString("X8");
				else
					BaseModel = ptr.ToCHex();
			ptr = ByteConverter.ToInt32(file, address);
			address += sizeof(int);
			if (ptr != 0)
				if (ptr >= imageBase && (ptr < file.Length + imageBase))
					ModelA = "object_" + ((uint)ptr - imageBase).ToString("X8");
				else
					ModelA = ptr.ToCHex();
			ptr = ByteConverter.ToInt32(file, address);
			address += sizeof(int);
			if (ptr != 0)
				if (ptr >= imageBase && (ptr < file.Length + imageBase))
					ModelB = "object_" + ((uint)ptr - imageBase).ToString("X8");
				else
					ModelB = ptr.ToCHex();
			int cnt = file[address++] * 2;
			WeldType = file[address++];
			Unknown = ByteConverter.ToInt16(file, address);
			address += sizeof(short);
			address += sizeof(int);
			ptr = ByteConverter.ToInt32(file, address);
			if (ptr != 0)
			{
				ptr = (int)((uint)ptr - imageBase);
				VertIndexName = "vi_" + ptr.ToString("X8");
				VertIndexes = new List<ushort>(cnt);
				for (int i = 0; i < cnt; i++)
				{
					VertIndexes.Add(ByteConverter.ToUInt16(file, ptr));
					ptr += sizeof(ushort);
				}
			}
		}

		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}

		public string ToStruct()
		{
			StringBuilder sb = new StringBuilder("{ ");
			sb.Append(BaseModel ?? "nullptr");
			sb.Append(", ");
			sb.Append(ModelA ?? "nullptr");
			sb.Append(", ");
			sb.Append(ModelB ?? "nullptr");
			sb.Append(", ");
			if (VertIndexes != null)
				sb.AppendFormat("LengthOfArray({0})", VertIndexName);
			else
				sb.Append("0");
			sb.Append(", ");
			sb.Append(WeldType);
			sb.Append(", ");
			sb.Append(Unknown);
			sb.Append(", nullptr, ");
			sb.Append(VertIndexes != null ? VertIndexName : "nullptr");
			sb.Append(" }");
			return sb.ToString();
		}
	}

	/// <summary>
	/// Converts between <see cref="string"/> and <typeparamref name="T"/>
	/// </summary>
	public class StringConverter<T> : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
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

	public class UInt32HexConverter : TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string) && value is uint)
				return ((uint)value).ToString("X");
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
				return uint.Parse((string)value, NumberStyles.HexNumber);
			return base.ConvertFrom(context, culture, value);
		}

		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			if (value is uint)
				return true;
			uint i;
			if (value is string)
				return uint.TryParse((string)value, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out i);
			return base.IsValid(context, value);
		}
	}

	public class Int32HexConverter : TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string) && value is int)
				return ((int)value).ToString("X");
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
				return int.Parse((string)value, NumberStyles.HexNumber);
			return base.ConvertFrom(context, culture, value);
		}

		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			if (value is int)
				return true;
			int i;
			if (value is string)
				return int.TryParse((string)value, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out i);
			return base.IsValid(context, value);
		}
	}

	public class UInt16HexConverter : TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string) && value is ushort)
				return ((ushort)value).ToString("X");
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
				return ushort.Parse((string)value, NumberStyles.HexNumber);
			return base.ConvertFrom(context, culture, value);
		}

		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			if (value is ushort)
				return true;
			ushort i;
			if (value is string)
				return ushort.TryParse((string)value, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out i);
			return base.IsValid(context, value);
		}
	}
}