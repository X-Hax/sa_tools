﻿using SAModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ByteConverter = SAModel.ByteConverter;

namespace SplitTools
{
	public class IniData
	{
		[IniName("datafile")]
		public string DataFilename { get; set; }
		[IniName("key")]
		[TypeConverter(typeof(UInt32HexConverter))]
		public uint? ImageBase { get; set; }
		[IniName("compressed")]
		public bool Compressed { get; set; }
		[IniName("compressedrel")]
		public bool CompressedREL { get; set; }
		[IniName("bigendian")]
		public bool BigEndian { get; set; }
		[IniName("reverse")]
		public bool Reverse { get; set; }
		[IniName("nometa")]
		public bool NoMeta { get; set; }
		[IniName("offset")]
		[TypeConverter(typeof(UInt32HexConverter))]
		public uint StartOffset { get; set; }
		[IniName("game")]
		[DefaultValue(Game.SADX)]
		public Game Game { get; set; }
		[IniCollection(IniCollectionMode.SingleLine, Format = ",")]
		public List<string> MD5 { get; set; }
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
		[IniName("length")]
		public int Length { get; set; }
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
			int numobjs = ByteConverter.ToInt16(file, address);
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
		public uint Code { get { if (uint.TryParse(CodeString, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out uint code)) return code; else return uint.MaxValue; } set { CodeString = value.ToString("X8"); } }
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
			List<byte> result = new List<byte>(Size)
			{
				Arg1,
				Arg2
			};
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
			List<byte> result = new List<byte>(Size)
			{
				Arg1,
				Arg2
			};
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
		public List<string> Names { get; set; }

		public MasterObjectListEntry() { }

		public MasterObjectListEntry(ObjectListEntry obj)
		{
			Arg1 = obj.Arg1;
			Arg2 = obj.Arg2;
			Flags = obj.Flags;
			Distance = obj.Distance;
			Name = obj.Name;
			Names = new List<string>() { obj.Name };
		}

		public void AddName(string name)
		{
			if (!Names.Contains(name))
				Names.Add(name);
		}
	}

	public static class SA1StartPosList
	{
		public static int Size { get { return SA1StartPosInfo.Size + 4; } }

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

	public static class SA2DCStartPosList
	{
		public static int Size { get { return SA2DCStartPosInfo.Size + 2; } }

		public static Dictionary<SA2DCLevelIDs, SA2DCStartPosInfo> Load(string filename)
		{
			return IniSerializer.Deserialize<Dictionary<SA2DCLevelIDs, SA2DCStartPosInfo>>(filename);
		}

		public static Dictionary<SA2DCLevelIDs, SA2DCStartPosInfo> Load(byte[] file, int address)
		{
			Dictionary<SA2DCLevelIDs, SA2DCStartPosInfo> result = new Dictionary<SA2DCLevelIDs, SA2DCStartPosInfo>();
			while (ByteConverter.ToUInt16(file, address) != (ushort)SA2DCLevelIDs.Invalid)
			{
				SA2DCStartPosInfo objgrp = new SA2DCStartPosInfo(file, address + 2);
				result.Add((SA2DCLevelIDs)ByteConverter.ToUInt16(file, address), objgrp);
				address += Size;
			}
			return result;
		}

		public static void Save(this Dictionary<SA2DCLevelIDs, SA2DCStartPosInfo> startpos, string filename)
		{
			IniSerializer.Serialize(startpos, filename);
		}

		public static byte[] GetBytes(this Dictionary<SA2DCLevelIDs, SA2DCStartPosInfo> startpos)
		{
			List<byte> result = new List<byte>(Size * (startpos.Count + 1));
			foreach (KeyValuePair<SA2DCLevelIDs, SA2DCStartPosInfo> item in startpos)
			{
				result.AddRange(ByteConverter.GetBytes((ushort)item.Key));
				result.AddRange(item.Value.GetBytes());
			}
			result.AddRange(ByteConverter.GetBytes((ushort)SA2DCLevelIDs.Invalid));
			result.AddRange(new byte[SA2DCStartPosInfo.Size]);
			return result.ToArray();
		}

		public static string ToStruct(this KeyValuePair<SA2DCLevelIDs, SA2DCStartPosInfo> startpos)
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
	public class SA2DCStartPosInfo
	{
		public SA2DCStartPosInfo()
		{
			Position = new Vertex();
			P1Position = new Vertex();
			P2Position = new Vertex();
		}

		public SA2DCStartPosInfo(byte[] file, int address)
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

	[Serializable]
	public class NJS_TEXLIST
	{
		public string Name { get; set; }
		public string TexnameArrayName { get; set; }
		public uint NumTextures { get; set; }
		public string[] TextureNames { get; set; }

		public NJS_TEXLIST()
		{
			TextureNames = new string[NumTextures];
		}

		public NJS_TEXLIST(byte[] file, int address, uint imageBase, Dictionary<int, string> labels = null, uint offset=0)
		{
			if (labels != null && labels.ContainsKey(address))
				Name = labels[address];
			else
				Name = "texlist_" + address.ToString("X8");
			uint TexnameArrayOffset = ByteConverter.ToUInt32(file, address);
			int TexnameArrayAddr = (int)(TexnameArrayOffset - imageBase);
			NumTextures = ByteConverter.ToUInt32(file, address + 4);
			if (labels != null && labels.ContainsKey(TexnameArrayAddr))
				TexnameArrayName = labels[TexnameArrayAddr];
			else
				TexnameArrayName = "textures_" + TexnameArrayAddr.ToString("X8");
			TextureNames = new string[NumTextures];
			if (TexnameArrayAddr == 0)
				return;
			if (NumTextures <= 300 && NumTextures > 0)
			{
				TextureNames = new string[NumTextures];
				for (int u = 0; u < NumTextures; u++)
				{
					// Hack for uninitialized data
					if (TexnameArrayAddr > file.Length - offset)
					{
						TextureNames[u] = null;
						continue;
					}
					uint TexnamePointer = ByteConverter.ToUInt32(file, (int)(TexnameArrayOffset + u * 12 - imageBase));
					if (TexnamePointer != 0)
					{
						int TexnameAddress = (int)(TexnamePointer - imageBase);
						TextureNames[u] = file.GetCString(TexnameAddress + (int)offset).TrimEnd();
					}
					else
						TextureNames[u] = null;
				}
			}
		}

		public NJS_TEXLIST(string[] textureNamelist)
		{
			TextureNames = textureNamelist;
			NumTextures = (uint)textureNamelist.Length;
		}

		public static NJS_TEXLIST Load(string textFile)
		{
			// Old SA2 projects fallback: Load the .tls file
			if (!File.Exists(textFile))
				return Load(Path.ChangeExtension(textFile, ".tls"));
			string extension = "pvr";
			string[] texnames_raw = File.ReadAllLines(textFile);
			// If this is an INI file, deserialize it
			if (texnames_raw.Length > 0 && texnames_raw[0].Contains("="))
				return IniSerializer.Deserialize<NJS_TEXLIST>(textFile);
			// Otherwise load it as a list of texture filenames
			else
			{
				if (texnames_raw.Length > 0)
				{
					switch (Path.GetExtension(texnames_raw[0]))
					{
						case ".gvr":
							extension = "gvr";
							break;
						case ".dds":
							extension = "dds";
							break;
						case ".pvr":
						default:
							break;
					}
				}
				string[] textureNames = new string[texnames_raw.Length];
				for (int i = 0; i < texnames_raw.Length; i++)
				{
					if (texnames_raw[i] == "")
						break;
					textureNames[i] = texnames_raw[i].Replace("." + extension, "");
				}
				return new NJS_TEXLIST { TextureNames = textureNames };
			}
		}

		public void SaveAsList(string fileOutputPath, string extension = "pvr")
		{
			StreamWriter sw = File.CreateText(fileOutputPath);
			for (int u = 0; u < TextureNames.Length; u++)
			{
				sw.WriteLine(TextureNames[u] != null ? TextureNames[u] : "empty" + "." + extension);
			}
			sw.Flush();
			sw.Close();
		}

		public void Save(string fileOutputPath)
		{
			IniSerializer.Serialize(this, fileOutputPath);
		}

		public void ToStruct(TextWriter tw, List<string> labels = null)
		{
			if (labels == null)
				labels = new List<string>();
			if (!labels.Contains(TexnameArrayName))
			{
				tw.WriteLine("NJS_TEXNAME {0}[] =", TexnameArrayName);
				tw.WriteLine("{");
				List<string> texs = new List<string>();
				for (int u = 0; u < NumTextures; u++)
				{
					string tname = (u < TextureNames.Length && TextureNames[u] != null) ? "\t{ \"" + TextureNames[u] + "\" }" : "\tNULL";
					texs.Add(tname);
				}
				tw.WriteLine(string.Join(",\n", texs));
				tw.WriteLine("};\n");
				labels.Add(TexnameArrayName);
			}
			if (!labels.Contains(Name))
			{
				tw.Write("NJS_TEXLIST {0}", Name);
				tw.Write("[] = { arrayptrandlength (");
				tw.Write(TexnameArrayName);
				tw.Write(") };");
				labels.Add(Name);
			}
		}

		public void ToNJA(TextWriter tw, List<string> labels = null)
		{
			if (labels == null)
				labels = new List<string>();
			if (!labels.Contains(Name))
			{
				tw.WriteLine("TEXTURE_START");
				tw.WriteLine();
				if (!labels.Contains(TexnameArrayName))
				{
					tw.WriteLine("TEXTURENAME {0}[]", TexnameArrayName);
					tw.WriteLine("START");
					for (int u = 0; u < NumTextures; u++)
					{
						string tname = (u < TextureNames.Length && TextureNames[u] != null) ? "\"" + TextureNames[u] + "\"" : "NULL";
						tw.WriteLine("\tTEXN ( {0} ),", tname);
					}
					tw.WriteLine("END");
					labels.Add(TexnameArrayName);
				}
				tw.WriteLine();
				tw.WriteLine("TEXTURELIST {0}", Name);
				tw.WriteLine("START");
				tw.WriteLine("TextureList {0},", TexnameArrayName);
				tw.WriteLine("TextureNum  {0},", NumTextures);
				tw.WriteLine("END");
				tw.WriteLine();
				tw.WriteLine("TEXTURE_END");
				labels.Add(Name);
			}
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
		}

		public string Name { get; set; }
		[TypeConverter(typeof(UInt32HexConverter))]
		public uint Textures { get; set; }

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
			List<byte> result = new List<byte>(Size)
			{
				(byte)Field,
				0
			};
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

	public static class SA2SoundList
	{
		public static SA2SoundListEntry[] Load(string filename)
		{
			return IniSerializer.Deserialize<SA2SoundListEntry[]>(filename);
		}

		public static SA2SoundListEntry[] Load(byte[] file, int address, uint imageBase)
		{
			byte numobjs = file[address];
			address = file.GetPointer(address + 4, imageBase);
			List<SA2SoundListEntry> objini = new List<SA2SoundListEntry>(numobjs);
			for (int i = 0; i < numobjs; i++)
			{
				objini.Add(new SA2SoundListEntry(file, address, imageBase));
				address += SA2SoundListEntry.Size;
			}
			return objini.ToArray();
		}

		public static void Save(this SA2SoundListEntry[] soundlist, string filename)
		{
			IniSerializer.Serialize(soundlist, filename);
		}
	}

	[Serializable]
	public class SA2SoundListEntry
	{
		[IniAlwaysInclude]
		public byte Bank { get; set; }
		[IniAlwaysInclude]
		public byte ID { get; set; }
		public byte SecondaryBank { get; set; }
		[IniAlwaysInclude]
		public byte DefaultFlags { get; set; }
		public uint Unknown { get; set; }
		public int DefaultDistance { get; set; }

		public static int Size { get { return 8; } }

		public SA2SoundListEntry() { }

		public SA2SoundListEntry(byte[] file, int address, uint imageBase)
		{
			Bank = file[address++];
			ID = file[address++];
			SecondaryBank = file[address++];
			DefaultFlags = file[address++];
			Unknown = ByteConverter.ToUInt16(file, address);
			address += sizeof(short);
			DefaultDistance = ByteConverter.ToInt16(file, address);
			address += sizeof(short);
		}

		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}

		public string ToStruct()
		{
			StringBuilder result = new StringBuilder("{ ");
			result.Append(Bank);
			result.Append(", ");
			result.Append(ID);
			result.Append(", ");
			result.Append(SecondaryBank);
			result.Append(", ");
			result.Append(DefaultFlags);
			result.Append(", ");
			result.Append(Unknown);
			result.Append(", ");
			result.Append(DefaultDistance);
			result.Append(" }");
			return result.ToString();
		}
	}

	public static class CharaSoundArray
	{
		public static CharaSoundArrayEntry[] Load(string filename)
		{
			return IniSerializer.Deserialize<CharaSoundArrayEntry[]>(filename);
		}

		public static CharaSoundArrayEntry[] Load(byte[] file, int address, uint imageBase, int count)
		{
			CharaSoundArrayEntry[] result = new CharaSoundArrayEntry[count];
			for (int i = 0; i < count; i++)
			{
				result[i] = new CharaSoundArrayEntry(file, address, imageBase);
				address += CharaSoundArrayEntry.Size;
			}
			return result;
		}

		public static void Save(this CharaSoundArrayEntry[] soundlist, string filename)
		{
			IniSerializer.Serialize(soundlist, filename);
		}
	}

	[Serializable]
	public class CharaSoundArrayEntry
	{
		[IniAlwaysInclude]
		public SA2Characters Character { get; set; }
		public string Filename { get; set; }
		public string SoundList { get; set; }

		public static int Size { get { return 0xC; } }

		public CharaSoundArrayEntry() { }

		public CharaSoundArrayEntry(byte[] file, int address, uint imageBase)
		{
			Character = (SA2Characters)file[address++];
			int ptr = ByteConverter.ToInt32(file, address + 3);
			if (ptr != 0)
				Filename = file.GetCString(file.GetPointer(address + 3, imageBase));
			address += sizeof(int);
			ptr = ByteConverter.ToInt32(file, address + 3);
			address += sizeof(int);
			if (ptr != 0)
				SoundList = ((uint)ptr - imageBase).ToString("X8");
		}

		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}

		public string ToStruct()
		{
			StringBuilder sb = new StringBuilder("{ ");
			sb.Append(Character);
			sb.Append(", ");
			if (!string.IsNullOrEmpty(Filename))
			{
				sb.Append(Filename ?? "nullptr");
			}
			else
				sb.Append("NULL");
			sb.Append(", ");
			if (!string.IsNullOrEmpty(SoundList))
			{
				sb.Append(SoundList ?? "nullptr");
			}
			else
				sb.Append("NULL");
			sb.Append(" }");
			return sb.ToString();
		}
	}

	public static class CharaVoiceArray
	{
		public static CharaVoiceArrayEntry[] Load(string filename)
		{
			return IniSerializer.Deserialize<CharaVoiceArrayEntry[]>(filename);
		}

		public static CharaVoiceArrayEntry[] Load(byte[] file, int address, uint imageBase, int count)
		{
			CharaVoiceArrayEntry[] result = new CharaVoiceArrayEntry[count];
			for (int i = 0; i < count; i++)
			{
				result[i] = new CharaVoiceArrayEntry(file, address, imageBase);
				address += CharaVoiceArrayEntry.Size;
			}
			return result;
		}

		public static void Save(this CharaVoiceArrayEntry[] soundlist, string filename)
		{
			IniSerializer.Serialize(soundlist, filename);
		}
	}

	[Serializable]
	public class CharaVoiceArrayEntry
	{
		[IniAlwaysInclude]
		public byte Mode { get; set; }
		[IniAlwaysInclude]
		public SA2Characters Character { get; set; }
		public string JPFilename { get; set; }
		public string ENFilename { get; set; }
		public string SoundList { get; set; }

		public static int Size { get { return 0x10; } }

		public CharaVoiceArrayEntry() { }

		public CharaVoiceArrayEntry(byte[] file, int address, uint imageBase)
		{
			Mode = file[address++];
			Character = (SA2Characters)file[address++];
			int ptr = ByteConverter.ToInt32(file, address + 2);
			if (ptr != 0)
				JPFilename = file.GetCString(file.GetPointer(address + 2, imageBase));
			address += sizeof(int);
			ptr = ByteConverter.ToInt32(file, address + 2);
			if (ptr != 0)
				ENFilename = file.GetCString(file.GetPointer(address + 2, imageBase));
			ptr = ByteConverter.ToInt32(file, address + 2);
			address += sizeof(int);
			if (ptr != 0)
				SoundList = ((uint)ptr - imageBase).ToString("X8");
		}

		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}

		public string ToStruct()
		{
			StringBuilder sb = new StringBuilder("{ ");
			sb.Append(Mode);
			sb.AppendFormat(", ");
			sb.Append(Character);
			sb.AppendFormat(", ");
			if (!string.IsNullOrEmpty(JPFilename))
			{
				sb.AppendFormat(JPFilename ?? "nullptr");
			}
			else
				sb.Append("NULL");
			sb.Append(", ");
			if (!string.IsNullOrEmpty(ENFilename))
			{
				sb.AppendFormat(ENFilename ?? "nullptr");
			}
			else
				sb.Append("NULL");
			sb.Append(", ");
			if (!string.IsNullOrEmpty(SoundList))
			{
				sb.AppendFormat(SoundList ?? "nullptr");
			}
			else
				sb.Append("NULL");
			sb.Append(" }");
			return sb.ToString();
		}
	}

	public static class MiniEventArray
	{
		public static MiniEventArrayEntry[] Load(string filename)
		{
			return IniSerializer.Deserialize<MiniEventArrayEntry[]>(filename);
		}

		public static MiniEventArrayEntry[] Load(byte[] file, int address, int count)
		{
			MiniEventArrayEntry[] result = new MiniEventArrayEntry[count];
			for (int i = 0; i < count; i++)
			{
				result[i] = new MiniEventArrayEntry(file, address);
				address += MiniEventArrayEntry.Size;
			}
			return result;
		}

		public static void Save(this MiniEventArrayEntry[] soundlist, string filename)
		{
			IniSerializer.Serialize(soundlist, filename);
		}
	}

	[Serializable]
	public class MiniEventArrayEntry
	{
		public SA2LevelIDs Level { get; set; }
		[IniAlwaysInclude]
		public SA2Characters Character { get; set; }
		public int CutsceneID { get; set; }

		public static int Size { get { return 0x6; } }

		public MiniEventArrayEntry() { }

		public MiniEventArrayEntry(byte[] file, int address)
		{
			Level = (SA2LevelIDs)ByteConverter.ToUInt16(file, address);
			address += sizeof(short);
			Character = (SA2Characters)ByteConverter.ToUInt16(file, address);
			address += sizeof(short);
			CutsceneID = ByteConverter.ToUInt16(file, address);
		}

		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}

		public string ToStruct()
		{
			StringBuilder sb = new StringBuilder("{ ");
			sb.Append(Level);
			sb.Append(", ");
			sb.Append(Character);
			sb.Append(", ");
			sb.Append(CutsceneID);
			sb.Append(" }");
			return sb.ToString();
		}
	}

	public static class DCMiniEventArray
	{
		public static DCMiniEventArrayEntry[] Load(string filename)
		{
			return IniSerializer.Deserialize<DCMiniEventArrayEntry[]>(filename);
		}

		public static DCMiniEventArrayEntry[] Load(byte[] file, int address, int count)
		{
			DCMiniEventArrayEntry[] result = new DCMiniEventArrayEntry[count];
			for (int i = 0; i < count; i++)
			{
				result[i] = new DCMiniEventArrayEntry(file, address);
				address += DCMiniEventArrayEntry.Size;
			}
			return result;
		}

		public static void Save(this DCMiniEventArrayEntry[] soundlist, string filename)
		{
			IniSerializer.Serialize(soundlist, filename);
		}
	}

	[Serializable]
	public class DCMiniEventArrayEntry
	{
		public SA2DCLevelIDs Level { get; set; }
		[IniAlwaysInclude]
		public SA2Characters Character { get; set; }
		public int CutsceneID { get; set; }

		public static int Size { get { return 0x6; } }

		public DCMiniEventArrayEntry() { }

		public DCMiniEventArrayEntry(byte[] file, int address)
		{
			Level = (SA2DCLevelIDs)ByteConverter.ToUInt16(file, address);
			address += sizeof(short);
			Character = (SA2Characters)ByteConverter.ToUInt16(file, address);
			address += sizeof(short);
			CutsceneID = ByteConverter.ToUInt16(file, address);
		}

		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}

		public string ToStruct()
		{
			StringBuilder sb = new StringBuilder("{ ");
			sb.Append(Level);
			sb.Append(", ");
			sb.Append(Character);
			sb.Append(", ");
			sb.Append(CutsceneID);
			sb.Append(" }");
			return sb.ToString();
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
			Save(directory, out string[] hashes);
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
			Save(list, directory, out string[][] hashes);
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
			Save(list, directory, out string[][] hashes);
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

		[IniCollection(IniCollectionMode.SingleLine, Format = ", ")]
		public List<ushort> EventFlags { get; set; }
		[IniCollection(IniCollectionMode.SingleLine, Format = ", ")]
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

		public DeathZoneFlags(byte[] file, int address, string filename)
		{
			Flags = (SA1CharacterFlags)ByteConverter.ToInt32(file, address);
			Filename = filename;
		}

		[IniAlwaysInclude]
		public SA1CharacterFlags Flags { get; set; }
		public string Filename { get; set; }
		public static int Size { get { return 4; } }

		public byte[] GetBytes()
		{
			return ByteConverter.GetBytes((int)Flags);
		}
	}

	public static class SA2BDeathZoneFlagsList
	{
		public static SA2BDeathZoneFlags[] Load(string filename)
		{
			return IniSerializer.Deserialize<SA2BDeathZoneFlags[]>(filename);
		}

		public static void Save(this SA2BDeathZoneFlags[] flags, string filename)
		{
			IniSerializer.Serialize(flags, filename);
		}
	}

	[Serializable]
	public class SA2BDeathZoneFlags
	{
		public SA2BDeathZoneFlags() { }

		public SA2BDeathZoneFlags(byte[] file, int address)
		{
			if (ByteConverter.BigEndian)
			{
				DeathFlag = file[address++];
				Constant2 = file[address++];
				Constant1 = file[address++];
				Flags = (SA2CharacterFlags)file[address++];
			}
			else
			{
				Flags = (SA2CharacterFlags)file[address++];
				Constant1 = file[address++];
				Constant2 = file[address++];
				DeathFlag = file[address++];
			}
		}

		public SA2BDeathZoneFlags(byte[] file, int address, string filename)
		{
			if (ByteConverter.BigEndian)
			{
				DeathFlag = file[address++];
				Constant2 = file[address++];
				Constant1 = file[address++];
				Flags = (SA2CharacterFlags)file[address++];
				Filename = filename;
			}
			else
			{
				Flags = (SA2CharacterFlags)file[address++];
				Constant1 = file[address++];
				Constant2 = file[address++];
				DeathFlag = file[address++];
				Filename = filename;
			}
		}

		[IniAlwaysInclude]
		public SA2CharacterFlags Flags { get; set; }
		public byte Constant1 { get; set; }
		public byte Constant2 { get; set; }
		[IniAlwaysInclude]
		public byte DeathFlag { get; set; }
		public string Filename { get; set; }

		public static int Size { get { return 4; } }

		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(Size)
		{
				(byte)Flags,
				Constant1,
				Constant2,
				DeathFlag,
		};
			return ByteConverter.GetBytes((int)Flags);
		}
	}

	public static class SA2DeathZoneFlagsList
	{
		public static SA2DeathZoneFlags[] Load(string filename)
		{
			return IniSerializer.Deserialize<SA2DeathZoneFlags[]>(filename);
		}

		public static void Save(this SA2DeathZoneFlags[] flags, string filename)
		{
			IniSerializer.Serialize(flags, filename);
		}
	}

	[Serializable]
	public class SA2DeathZoneFlags
	{
		public SA2DeathZoneFlags() { }

		public SA2DeathZoneFlags(byte[] file, int address)
		{
			Flags = (SA2CharacterFlags)file[address++];
			DeathFlag = file[address++];
		}

		public SA2DeathZoneFlags(byte[] file, int address, string filename)
		{
			Flags = (SA2CharacterFlags)file[address++];
			DeathFlag = file[address++];
			Filename = filename;
		}

		[IniAlwaysInclude]
		public SA2CharacterFlags Flags { get; set; }
		public byte DeathFlag { get; set; }
		public string Filename { get; set; }

		public static int Size { get { return 4; } }

		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(Size)
				{
					(byte)Flags,
					DeathFlag
				};
			return ByteConverter.GetBytes((int)Flags);
		}
	}

	public static class SkyboxScaleList
	{
		public static SkyboxScale[] Load(string filename)
		{
			if (File.Exists(filename))
				return IniSerializer.Deserialize<SkyboxScale[]>(filename);
			else
			{
				List<SkyboxScale> defaultScaleList = new List<SkyboxScale>();
				for (int act = 0; act < 6; act++)
				{
					defaultScaleList.Add(new SkyboxScale { Far = new Vertex(1.0f, 1.0f, 1.0f), Near = new Vertex(1.0f, 1.0f, 1.0f), Normal = new Vertex(1.0f, 1.0f, 1.0f) });
				}
				return defaultScaleList.ToArray();
			}	
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
			while (ByteConverter.ToUInt16(file, address) < (ushort)SA2LevelIDs.Invalid)
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

	public static class KartRankTimesList
	{
		public static KartRankTimes[] Load(string filename)
		{
			return IniSerializer.Deserialize<KartRankTimes[]>(filename);
		}

		public static KartRankTimes[] Load(byte[] file, int address, int count)
		{
			KartRankTimes[] result = new KartRankTimes[count];
			for (int i = 0; i < count; i++)
			{
				result[i] = new KartRankTimes(file, address);
				address += KartRankTimes.Size;
			}
			return result;
		}

		public static void Save(this KartRankTimes[] startpos, string filename)
		{
			IniSerializer.Serialize(startpos, filename);
		}
	}
	[Serializable]
	public class KartRankTimes
	{
		public SA2LevelIDs Level { get; set; }
		public MinSec DRank { get; set; }
		public MinSec CRank { get; set; }
		public MinSec BRank { get; set; }
		public MinSec ARank { get; set; }

		public static int Size
		{
			get { return 0x9; }
		}
		public KartRankTimes() { }

		public KartRankTimes(byte[] file, int address)
		{
			Level = (SA2LevelIDs)file[address++];
			DRank = new MinSec(file, address);
			address += MinSec.Size;
			CRank = new MinSec(file, address);
			address += MinSec.Size;
			BRank = new MinSec(file, address);
			address += MinSec.Size;
			ARank = new MinSec(file, address);
		}

		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}

		public string ToStruct()
		{
			StringBuilder sb = new StringBuilder("{ ");
			sb.Append(Level.ToC("LevelIDs"));
			sb.Append(", ");
			sb.Append(DRank.ToStruct());
			sb.Append(", ");
			sb.Append(CRank.ToStruct());
			sb.Append(", ");
			sb.Append(BRank.ToStruct());
			sb.Append(", ");
			sb.Append(ARank.ToStruct());
			sb.Append(" }");
			return sb.ToString();
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
		public ushort Model { get; set; }
		public ushort? Unknown1 { get { return null; } set { if (value.HasValue) Model = value.Value; } }
		[IniAlwaysInclude]
		public ushort Property { get; set; }
		public ushort? Unknown2 { get { return null; } set { if (value.HasValue) Property = value.Value; } }
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
			Model = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			Property = ByteConverter.ToUInt16(file, address);
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
			result.AddRange(ByteConverter.GetBytes(Model));
			result.AddRange(ByteConverter.GetBytes(Property));
			result.AddRange(ByteConverter.GetBytes(NextAnimation));
			result.AddRange(ByteConverter.GetBytes(TransitionSpeed));
			result.AddRange(ByteConverter.GetBytes(AnimationSpeed));
			return result.ToArray();
		}

		public string ToStruct()
		{
			return string.Format("{{ {0}, {1}, {2}, {3}, {4}, {5} }}", Animation, Model, Property, NextAnimation,
				TransitionSpeed.ToC(), AnimationSpeed.ToC());
		}
	}

	public static class SA1ActionInfoList
	{
		public static SA1ActionInfo[] Load(string filename)
		{
			return IniSerializer.Deserialize<SA1ActionInfo[]>(filename);
		}

		public static SA1ActionInfo[] Load(byte[] file, int address, uint imageBase, int count)
		{
			SA1ActionInfo[] result = new SA1ActionInfo[count];
			for (int i = 0; i < count; i++)
			{
				result[i] = new SA1ActionInfo(file, address, imageBase);
				address += SA1ActionInfo.Size;
			}
			return result;
		}

		public static void Save(this SA1ActionInfo[] levellist, string filename)
		{
			IniSerializer.Serialize(levellist, filename);
		}
	}
	[Serializable]
	public class SA1ActionInfo
	{
		[IniAlwaysInclude]
		public string Action { get; set; }
		[IniAlwaysInclude]
		public byte NodeCount { get; set; }
		[IniAlwaysInclude]
		public byte Property { get; set; }
		[IniAlwaysInclude]
		public ushort NextAnimation { get; set; }
		[IniAlwaysInclude]
		public float TransitionSpeed { get; set; }
		[IniAlwaysInclude]
		public float AnimationSpeed { get; set; }

		public static int Size { get { return 16; } }

		public SA1ActionInfo() { }

		public SA1ActionInfo(byte[] file, int address, uint imageBase)
		{
			int ptr = ByteConverter.ToInt32(file, address);
			address += sizeof(int);
			if (ptr != 0)
				if ((uint)ptr >= imageBase && ((uint)ptr < file.Length + imageBase))
					Action = "action_" + ((uint)ptr - imageBase).ToString("X8");
				else
					Action = ptr.ToCHex();
			NodeCount = file[address++];
			Property = file[address++];
			NextAnimation = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			TransitionSpeed = ByteConverter.ToSingle(file, address);
			address += sizeof(float);
			AnimationSpeed = ByteConverter.ToSingle(file, address);
			address += sizeof(float);
		}

		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}

		public string ToStruct()
		{
			StringBuilder sb = new StringBuilder("{ ");
			sb.Append(Action);
			sb.Append(", ");
			sb.Append(NodeCount);
			sb.Append(", ");
			sb.Append(Property);
			sb.Append(", ");
			sb.Append(NextAnimation);
			sb.Append(", ");
			sb.Append(TransitionSpeed.ToC());
			sb.Append(", ");
			sb.Append(AnimationSpeed.ToC());
			sb.Append(" }");
			return sb.ToString();
		}
	}

	public static class SA2EnemyAnimInfoList
	{
		public static SA2EnemyAnimInfo[] Load(string filename)
		{
			return IniSerializer.Deserialize<SA2EnemyAnimInfo[]>(filename);
		}

		public static SA2EnemyAnimInfo[] Load(byte[] file, int address, uint imageBase, int count)
		{
			SA2EnemyAnimInfo[] result = new SA2EnemyAnimInfo[count];
			for (int i = 0; i < count; i++)
			{
				result[i] = new SA2EnemyAnimInfo(file, address, imageBase);
				address += SA2EnemyAnimInfo.Size;
			}
			return result;
		}

		public static void Save(this SA2EnemyAnimInfo[] levellist, string filename)
		{
			IniSerializer.Serialize(levellist, filename);
		}
	}

	[Serializable]
	public class SA2EnemyAnimInfo
	{
		[IniAlwaysInclude]
		public string Animation { get; set; }
		[IniAlwaysInclude]
		public ushort Property { get; set; }
		[IniAlwaysInclude]
		public ushort NextAnimation { get; set; }
		[IniAlwaysInclude]
		public float TransitionSpeed { get; set; }
		[IniAlwaysInclude]
		public float AnimationSpeed { get; set; }

		public static int Size { get { return 16; } }

		public SA2EnemyAnimInfo() { }

		public SA2EnemyAnimInfo(byte[] file, int address, uint imageBase)
		{
			int ptr = ByteConverter.ToInt32(file, address);
			address += sizeof(int);
			if (ptr != 0)
				if ((uint)ptr >= imageBase && ((uint)ptr < file.Length + imageBase))
					Animation = "animation_" + ((uint)ptr - imageBase).ToString("X8");
				else
					Animation = ptr.ToCHex();
			Property = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			NextAnimation = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			TransitionSpeed = ByteConverter.ToSingle(file, address);
			address += sizeof(float);
			AnimationSpeed = ByteConverter.ToSingle(file, address);
			address += sizeof(float);
		}

		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}

		public string ToStruct()
		{
			StringBuilder sb = new StringBuilder("{ ");
			sb.Append(Animation);
			sb.Append(", ");
			sb.Append(Property);
			sb.Append(", ");
			sb.Append(NextAnimation);
			sb.Append(", ");
			sb.Append(TransitionSpeed.ToC());
			sb.Append(", ");
			sb.Append(AnimationSpeed.ToC());
			sb.Append(" }");
			return sb.ToString();
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
				result.AddRange(path.GetBytes(imageBase, out uint ptr));
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
		public ushort ZRotation { get; set; }
		[TypeConverter(typeof(UInt16HexConverter))]
		public ushort? YRotation { get { return null; } set { if (value.HasValue) ZRotation = value.Value; } }
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
			ZRotation = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			Distance = ByteConverter.ToSingle(file, address);
			address += sizeof(float);
			Position = new Vertex(file, address);
		}

		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>();
			result.AddRange(ByteConverter.GetBytes(XRotation));
			result.AddRange(ByteConverter.GetBytes(ZRotation));
			result.AddRange(ByteConverter.GetBytes(Distance));
			result.AddRange(Position.GetBytes());
			return result.ToArray();
		}

		public string ToStruct()
		{
			return string.Format("{{ {0}, {1}, {2}, {3} }}", XRotation.ToCHex(), ZRotation.ToCHex(), Distance.ToC(),
				Position.ToStruct());
		}
	}

	public static class SADXStageLightDataList
	{
		public static List<SADXStageLightData> Load(string filename)
		{
			return IniSerializer.Deserialize<List<SADXStageLightData>>(filename);
		}

		public static List<SADXStageLightData> Load(byte[] file, int address)
		{
			List<SADXStageLightData> result = new List<SADXStageLightData>();
			while (file[address] != 0xFF)
			{
				result.Add(new SADXStageLightData(file, address));
				address += SADXStageLightData.Size;
			}
			return result;
		}

		public static void Save(this List<SADXStageLightData> startpos, string filename)
		{
			IniSerializer.Serialize(startpos, filename);
		}

		public static byte[] GetBytes(this List<SADXStageLightData> startpos)
		{
			List<byte> result = new List<byte>(SADXStageLightData.Size * (startpos.Count + 1));
			foreach (SADXStageLightData item in startpos)
				result.AddRange(item.GetBytes());
			result.Add(0xFF);
			result.AddRange(new byte[SADXStageLightData.Size - 1]);
			return result.ToArray();
		}
	}

	[Serializable]
	public class SADXStageLightData
	{
		public SADXStageLightData() { Direction = new Vertex(); }

		public SADXStageLightData(byte[] file, int address)
		{
			Level = (SA1LevelIDs)file[address++];
			Act = file[address++];
			LightNum = file[address++];
			UseDirection = file[address++] != 0;
			Direction = new Vertex(file, address);
			address += Vertex.Size;
			Specular = ByteConverter.ToSingle(file, address);
			address += sizeof(float);
			Diffuse = ByteConverter.ToSingle(file, address);
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
		[IniName("Dif")]
		public float Specular { get; set; }
		[IniName("Multiplier")]
		public float Diffuse { get; set; }
		public Vertex RGB { get; set; }
		public Vertex AmbientRGB { get; set; }

		public static int Size { get { return 0x30; } }

		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(Size)
			{
				(byte)Level,
				Act,
				LightNum,
				(byte)(UseDirection ? 1 : 0)
			};
			result.AddRange(Direction.GetBytes());
			result.AddRange(ByteConverter.GetBytes(Specular));
			result.AddRange(ByteConverter.GetBytes(Diffuse));
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
			result.Append(Specular.ToC());
			result.Append(", ");
			result.Append(Diffuse.ToC());
			result.Append(", ");
			result.Append(RGB.ToStruct());
			result.Append(", ");
			result.Append(AmbientRGB.ToStruct());
			result.Append(" }");
			return result.ToString();
		}
	}

	public static class SA2DefaultObjectLights
	{
		public static List<SA2DefaultObjectLightData> Load(string filename)
		{
			return IniSerializer.Deserialize<List<SA2DefaultObjectLightData>>(filename);
		}

		public static SA2DefaultObjectLightData[] Load(byte[] file, int address, int count)
		{
			SA2DefaultObjectLightData[] result = new SA2DefaultObjectLightData[count];
			for (int i = 0; i < count; i++)
			{
				result[i] = new SA2DefaultObjectLightData(file, address);
				address += SA2DefaultObjectLightData.Size;
			}
			return result;
		}

		public static void Save(this SA2DefaultObjectLightData[] startpos, string filename)
		{
			IniSerializer.Serialize(startpos, filename);
		}
	}

	[Serializable]
	public class SA2DefaultObjectLightData
	{
		public SA2DefaultObjectLightData() { Direction = new Vertex(); }

		public SA2DefaultObjectLightData(byte[] file, int address)
		{
			Direction = new Vertex(file, address);
			address += Vertex.Size;
			Intensity = ByteConverter.ToSingle(file, address);
			address += sizeof(float);
			Ambient = ByteConverter.ToSingle(file, address);
			address += sizeof(float);
			RGB = new Vertex(file, address);
			address += Vertex.Size;
		}

		[IniAlwaysInclude]
		public Vertex Direction { get; set; }
		[IniAlwaysInclude]
		public float Intensity { get; set; }
		[IniAlwaysInclude]
		public float Ambient { get; set; }
		[IniAlwaysInclude]
		public Vertex RGB { get; set; }

		public static int Size { get { return 0x20; } }

		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(Size);
			result.AddRange(Direction.GetBytes());
			result.AddRange(ByteConverter.GetBytes(Intensity));
			result.AddRange(ByteConverter.GetBytes(Ambient));
			result.AddRange(RGB.GetBytes());
			return result.ToArray();
		}
		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}

		public string ToStruct()
		{
			StringBuilder result = new StringBuilder("{ ");
			result.Append(Direction.ToStruct());
			result.Append(", ");
			result.Append(Intensity.ToC());
			result.Append(", ");
			result.Append(Ambient.ToC());
			result.Append(", ");
			result.Append(RGB.ToStruct());
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
		[IniCollection(IniCollectionMode.SingleLine, Format = ", ")]
		public List<ushort> VertIndexes { get; set; }
		public string VertIndexName { get; set; }

		public static int Size { get { return 0x18; } }

		public WeldInfo() { }

		public WeldInfo(byte[] file, int address, uint imageBase)
		{
			int ptr = ByteConverter.ToInt32(file, address);
			address += sizeof(int);
			if (ptr != 0)
				if ((uint)ptr >= imageBase && ((uint)ptr < file.Length + imageBase))
					BaseModel = "object_" + ((uint)ptr - imageBase).ToString("X8");
				else
					BaseModel = ptr.ToCHex();
			ptr = ByteConverter.ToInt32(file, address);
			address += sizeof(int);
			if (ptr != 0)
				if ((uint)ptr >= imageBase && ((uint)ptr < file.Length + imageBase))
					ModelA = "object_" + ((uint)ptr - imageBase).ToString("X8");
				else
					ModelA = ptr.ToCHex();
			ptr = ByteConverter.ToInt32(file, address);
			address += sizeof(int);
			if (ptr != 0)
				if ((uint)ptr >= imageBase && ((uint)ptr < file.Length + imageBase))
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
				sb.AppendFormat("(uint8_t)(LengthOfArray({0}) / 2)", VertIndexName);
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

	public static class BlackMarketItemAttributesList
	{
		public static Dictionary<ChaoItemCategory, List<BlackMarketItemAttributes>> Load(string filename)
		{
			return IniSerializer.Deserialize<Dictionary<ChaoItemCategory, List<BlackMarketItemAttributes>>>(filename);
		}

		public static Dictionary<ChaoItemCategory, List<BlackMarketItemAttributes>> Load(byte[] file, int address, uint imageBase)
		{
			Dictionary<ChaoItemCategory, List<BlackMarketItemAttributes>> result = new Dictionary<ChaoItemCategory, List<BlackMarketItemAttributes>>();
			for (int i = 0; i < 11; i++)
			{
				int ptr = ByteConverter.ToInt32(file, address);
				address += sizeof(int);
				if (ptr != 0)
				{
					ptr = (int)(ptr - imageBase);
					int cnt = ByteConverter.ToInt32(file, address);
					List<BlackMarketItemAttributes> attrs = new List<BlackMarketItemAttributes>();
					for (int j = 0; j < cnt; j++)
					{
						attrs.Add(new BlackMarketItemAttributes(file, ptr));
						ptr += BlackMarketItemAttributes.Size;
					}
					result.Add((ChaoItemCategory)i, attrs);
				}
				address += sizeof(int);
			}
			return result;
		}

		public static void Save(this Dictionary<ChaoItemCategory, List<BlackMarketItemAttributes>> list, string filename)
		{
			IniSerializer.Serialize(list, filename);
		}
	}

	[Serializable]
	public class BlackMarketItemAttributes
	{
		public int PurchasePrice { get; set; }
		public int SalePrice { get; set; }
		[IniAlwaysInclude]
		public short RequiredEmblems { get; set; }
		public short NameID;
		public short DescriptionID;
		public short Unknown;

		public static int Size { get { return 0x10; } }

		public BlackMarketItemAttributes() { }

		public BlackMarketItemAttributes(byte[] file, int address)
		{
			PurchasePrice = ByteConverter.ToInt32(file, address);
			address += sizeof(int);
			SalePrice = ByteConverter.ToInt32(file, address);
			address += sizeof(int);
			RequiredEmblems = ByteConverter.ToInt16(file, address);
			address += sizeof(short);
			NameID = ByteConverter.ToInt16(file, address);
			address += sizeof(short);
			DescriptionID = ByteConverter.ToInt16(file, address);
			address += sizeof(short);
			Unknown = ByteConverter.ToInt16(file, address);
		}

		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}

		public string ToStruct()
		{
			return $"{{ {PurchasePrice}, {SalePrice}, {RequiredEmblems}, {NameID}, {DescriptionID}, {Unknown} }}";
		}
	}

	public static class CreditsTextList
	{
		public static CreditsTextListEntry[] Load(string filename)
		{
			return IniSerializer.Deserialize<CreditsTextListEntry[]>(filename);
		}

		public static CreditsTextListEntry[] Load(byte[] file, int address, uint imageBase)
		{
			int numobjs = ByteConverter.ToInt32(file, address + 4);
			address = file.GetPointer(address, imageBase);
			List<CreditsTextListEntry> objini = new List<CreditsTextListEntry>(numobjs);
			for (int i = 0; i < numobjs; i++)
			{
				objini.Add(new CreditsTextListEntry(file, address, imageBase));
				address += CreditsTextListEntry.Size;
			}
			return objini.ToArray();
		}

		public static void Save(this CreditsTextListEntry[] list, string filename)
		{
			IniSerializer.Serialize(list, filename);
		}
	}

	[Serializable]
	public class CreditsTextListEntry
	{
		public CreditsTextListEntry() { Text = string.Empty; }
		public CreditsTextListEntry(byte[] file, int address, uint imageBase)
		{
			Type = file[address++];
			TexID = (sbyte)file[address++];
			Unknown1 = file[address++];
			Unknown2 = file[address++];
			Text = file.GetCString(file.GetPointer(address, imageBase));
		}

		[IniAlwaysInclude]
		public byte Type { get; set; }
		[DefaultValue(-1)]
		public sbyte TexID { get; set; }
		public byte Unknown1 { get; set; }
		public byte Unknown2 { get; set; }
		public string Text { get; set; }

		public static int Size { get { return 8; } }

		public string ToStruct()
		{
			return $"{{ {Type}, {TexID}, {Unknown1}, {Unknown2}, {Text.ToC()} }}";
		}
	}

	public static class SA2CreditsTextList
	{
		public static SA2CreditsTextListEntry[] Load(string filename)
		{
			return IniSerializer.Deserialize<SA2CreditsTextListEntry[]>(filename);
		}

		public static SA2CreditsTextListEntry[] Load(byte[] file, int address, uint imageBase, int count)
		{
			SA2CreditsTextListEntry[] result = new SA2CreditsTextListEntry[count];
			for (int i = 0; i < count; i++)
			{
				result[i] = new SA2CreditsTextListEntry(file, address, imageBase);
				address += SA2CreditsTextListEntry.Size;
			}
			return result;
		}

		public static void Save(this SA2CreditsTextListEntry[] list, string filename)
		{
			IniSerializer.Serialize(list, filename);
		}
	}

	[Serializable]
	public class SA2CreditsTextListEntry
	{
		public SA2CreditsTextListEntry() { Text = string.Empty; }
		public SA2CreditsTextListEntry(byte[] file, int address, uint imageBase)
		{
			Type = ByteConverter.ToUInt32(file, address);
			address += sizeof(int);
			TextColorA = ByteConverter.ToSingle(file, address);
			address += sizeof(float);
			TextColorR = ByteConverter.ToSingle(file, address);
			address += sizeof(float);
			TextColorG = ByteConverter.ToSingle(file, address);
			address += sizeof(float);
			TextColorB = ByteConverter.ToSingle(file, address);
			address += sizeof(float);
			int ptr = ByteConverter.ToInt32(file, address);
			if (ptr != 0)
				Text = file.GetCString(file.GetPointer(address, imageBase));
		}

		[IniAlwaysInclude]
		public uint Type { get; set; }
		[IniAlwaysInclude]
		public float TextColorA { get; set; }
		[IniAlwaysInclude]
		public float TextColorR { get; set; }
		[IniAlwaysInclude]
		public float TextColorG { get; set; }
		[IniAlwaysInclude]
		public float TextColorB { get; set; }
		public string Text { get; set; }

		public static int Size { get { return 0x18; } }

		public string ToStruct()
		{
			StringBuilder sb = new StringBuilder("{ ");
			sb.Append(Type);
			sb.AppendFormat(", ");
			sb.Append(TextColorA);
			sb.AppendFormat(", ");
			sb.Append(TextColorR);
			sb.Append(", ");
			sb.Append(TextColorG);
			sb.AppendFormat(", ");
			sb.Append(TextColorB);
			sb.Append(", ");
			if (!string.IsNullOrEmpty(Text))
			{
				sb.AppendFormat(Text ?? "nullptr");
			}
			else
				sb.Append("NULL");
			sb.Append(" }");
			return sb.ToString();
		}
	}

	public static class SA2StoryList
	{
		public static List<SA2StoryEntry> Load(string filename)
		{
			return IniSerializer.Deserialize<List<SA2StoryEntry>>(filename);
		}

		public static List<SA2StoryEntry> Load(byte[] file, int address)
		{
			List<SA2StoryEntry> result = new List<SA2StoryEntry>();
			while (file[address] != 2)
			{
				result.Add(new SA2StoryEntry(file, address));
				address += SA2StoryEntry.Size;
			}
			return result;
		}

		public static void Save(this List<SA2StoryEntry> startpos, string filename)
		{
			IniSerializer.Serialize(startpos, filename);
		}

		public static byte[] GetBytes(this List<SA2StoryEntry> startpos)
		{
			List<byte> result = new List<byte>(SA2StoryEntry.Size * (startpos.Count + 1));
			foreach (SA2StoryEntry item in startpos)
				result.AddRange(item.GetBytes());
			result.Add(2);
			result.AddRange(new byte[SA2StoryEntry.Size - 1]);
			return result.ToArray();
		}
	}

	[Serializable]
	public class SA2StoryEntry
	{
		public SA2StoryEntry() { }

		public SA2StoryEntry(byte[] file, int address)
		{
			Type = (SA2StoryEntryType)file[address++];
			Character = (SA2Characters)file[address++];
			Level = (SA2LevelIDs)ByteConverter.ToInt16(file, address);
			address += sizeof(short);
			Events = new List<int>();
			for (int i = 0; i < 4; i++)
			{
				int tmp = ByteConverter.ToInt16(file, address);
				address += sizeof(short);
				if (tmp == -1)
					break;
				Events.Add(tmp);
			}
			if (Events.Count == 0) Events = null;
		}

		[IniAlwaysInclude]
		public SA2StoryEntryType Type { get; set; }
		[IniAlwaysInclude]
		public SA2Characters Character { get; set; }
		[IniAlwaysInclude]
		public SA2LevelIDs Level { get; set; }
		[IniCollection(IniCollectionMode.SingleLine, Format = ", ")]
		public List<int> Events { get; set; }

		public static int Size { get { return 0xC; } }

		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(Size)
			{
				(byte)Type,
				(byte)Character
			};
			result.AddRange(ByteConverter.GetBytes((short)Level));
			if (Events != null)
				for (int i = 0; i < 4; i++)
					result.AddRange(ByteConverter.GetBytes((short)(i < Events.Count ? Events[i] : -1)));
			else
				result.AddRange(System.Linq.Enumerable.Repeat((byte)0xFF, 8));
			return result.ToArray();
		}

		public string ToStruct()
		{
			StringBuilder result = new StringBuilder("{ ");
			result.Append(Type.ToC("StoryEntryType"));
			result.Append(", ");
			result.Append(Character.ToC("Characters"));
			result.Append(", ");
			result.Append(Level.ToC("LevelIDs"));
			if (Events != null)
				for (int i = 0; i < 4; i++)
				{
					result.Append(", ");
					result.Append(i < Events.Count ? Events[i] : -1);
				}
			else
				result.Append(", -1, -1, -1, -1");
			result.Append(" }");
			return result.ToString();
		}
	}

	public static class SA2DCStoryList
	{
		public static List<SA2DCStoryEntry> Load(string filename)
		{
			return IniSerializer.Deserialize<List<SA2DCStoryEntry>>(filename);
		}

		public static List<SA2DCStoryEntry> Load(byte[] file, int address)
		{
			List<SA2DCStoryEntry> result = new List<SA2DCStoryEntry>();
			while (file[address] != 2)
			{
				result.Add(new SA2DCStoryEntry(file, address));
				address += SA2DCStoryEntry.Size;
			}
			return result;
		}

		public static void Save(this List<SA2DCStoryEntry> startpos, string filename)
		{
			IniSerializer.Serialize(startpos, filename);
		}

		public static byte[] GetBytes(this List<SA2DCStoryEntry> startpos)
		{
			List<byte> result = new List<byte>(SA2DCStoryEntry.Size * (startpos.Count + 1));
			foreach (SA2DCStoryEntry item in startpos)
				result.AddRange(item.GetBytes());
			result.Add(2);
			result.AddRange(new byte[SA2DCStoryEntry.Size - 1]);
			return result.ToArray();
		}
	}

	[Serializable]
	public class SA2DCStoryEntry
	{
		public SA2DCStoryEntry() { }

		public SA2DCStoryEntry(byte[] file, int address)
		{
			Type = (SA2StoryEntryType)file[address++];
			Character = (SA2Characters)file[address++];
			Level = (SA2DCLevelIDs)ByteConverter.ToInt16(file, address);
			address += sizeof(short);
			Events = new List<int>();
			for (int i = 0; i < 4; i++)
			{
				int tmp = ByteConverter.ToInt16(file, address);
				address += sizeof(short);
				if (tmp == -1)
					break;
				Events.Add(tmp);
			}
			if (Events.Count == 0) Events = null;
		}

		[IniAlwaysInclude]
		public SA2StoryEntryType Type { get; set; }
		[IniAlwaysInclude]
		public SA2Characters Character { get; set; }
		[IniAlwaysInclude]
		public SA2DCLevelIDs Level { get; set; }
		[IniCollection(IniCollectionMode.SingleLine, Format = ", ")]
		public List<int> Events { get; set; }

		public static int Size { get { return 0xC; } }

		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(Size)
			{
				(byte)Type,
				(byte)Character
			};
			result.AddRange(ByteConverter.GetBytes((short)Level));
			if (Events != null)
				for (int i = 0; i < 4; i++)
					result.AddRange(ByteConverter.GetBytes((short)(i < Events.Count ? Events[i] : -1)));
			else
				result.AddRange(System.Linq.Enumerable.Repeat((byte)0xFF, 8));
			return result.ToArray();
		}

		public string ToStruct()
		{
			StringBuilder result = new StringBuilder("{ ");
			result.Append(Type.ToC("StoryEntryType"));
			result.Append(", ");
			result.Append(Character.ToC("Characters"));
			result.Append(", ");
			result.Append(Level.ToC("LevelIDs"));
			if (Events != null)
				for (int i = 0; i < 4; i++)
				{
					result.Append(", ");
					result.Append(i < Events.Count ? Events[i] : -1);
				}
			else
				result.Append(", -1, -1, -1, -1");
			result.Append(" }");
			return result.ToString();
		}
	}

	public enum SA2StoryEntryType
	{
		Event,
		Level,
		End,
		Credits
	}

	public static class SA2CutsceneVoices
	{
		public static List<SA2CutsceneVoiceInfo> Load(string filename)
		{
			return IniSerializer.Deserialize<List<SA2CutsceneVoiceInfo>>(filename);
		}

		public static List<SA2CutsceneVoiceInfo> Load(byte[] file, int address)
		{
			List<SA2CutsceneVoiceInfo> result = new List<SA2CutsceneVoiceInfo>();
			uint ptr = ByteConverter.ToUInt32(file, address);
			while (ptr != 0xFFFFFFFF)
			{
				result.Add(new SA2CutsceneVoiceInfo(file, address));
				address += SA2CutsceneVoiceInfo.Size;
				ptr = ByteConverter.ToUInt32(file, address);
			}
			return result;
		}

		public static void Save(this List<SA2CutsceneVoiceInfo> voice, string filename)
		{
			IniSerializer.Serialize(voice, filename);
		}
	}

	[Serializable]
	public class SA2CutsceneVoiceInfo
	{
		[IniAlwaysInclude]
		public string EventID { get; set; }
		[IniIgnore]
		public uint RealValue { get; set; }
		[IniIgnore]
		public int EventIDConstant { get; set; }
		[IniAlwaysInclude]
		public int EventVoiceID { get; set; }
		[IniAlwaysInclude]
		public int VoiceFileID { get; set; }
		public static int Size { get { return 0x8; } }

		public SA2CutsceneVoiceInfo() { }

		public SA2CutsceneVoiceInfo(byte[] file, int address)
		{
			RealValue = ByteConverter.ToUInt32(file, address);
			EventIDConstant = (int)Math.Floor(decimal.Divide(RealValue, 1000));
			switch (EventIDConstant)
			{
				case 1:
				case 7:
				case 8:
				case 10:
				case 12:
				case 13:
				case 18:
				case 23:
				case 104:
				case 108:
				case 110:
				case 114:
				case 115:
				case 117:
				case 121:
				case 125:
					EventID = "ME" + EventIDConstant.ToString("D4");
					break;
				default:
					EventID = "E" + EventIDConstant.ToString("D4");
					break;
			}
			EventVoiceID = (int)(RealValue % 1000);
			address += sizeof(int);
			VoiceFileID = ByteConverter.ToInt32(file, address);
		}

		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}

		public string ToStruct()
		{
			StringBuilder sb = new StringBuilder("{ ");
			sb.Append(RealValue);
			sb.Append(", ");
			sb.Append(VoiceFileID);
			sb.Append(" }");
			return sb.ToString();
		}
	}

	public static class SA2BetaCutsceneVoices
	{
		public static List<SA2BetaCutsceneVoiceInfo> Load(string filename)
		{
			return IniSerializer.Deserialize<List<SA2BetaCutsceneVoiceInfo>>(filename);
		}

		public static List<SA2BetaCutsceneVoiceInfo> Load(byte[] file, int address)
		{
			List<SA2BetaCutsceneVoiceInfo> result = new List<SA2BetaCutsceneVoiceInfo>();
			uint ptr = ByteConverter.ToUInt32(file, address);
			while (ptr != 0xFFFFFFFF)
			{
				result.Add(new SA2BetaCutsceneVoiceInfo(file, address));
				address += SA2BetaCutsceneVoiceInfo.Size;
				ptr = ByteConverter.ToUInt32(file, address);
			}
			return result;
		}

		public static void Save(this List<SA2BetaCutsceneVoiceInfo> voice, string filename)
		{
			IniSerializer.Serialize(voice, filename);
		}
	}

	[Serializable]
	public class SA2BetaCutsceneVoiceInfo
	{
		[IniAlwaysInclude]
		public string EventID { get; set; }
		[IniIgnore]
		public int EventIDConstant { get; set; }
		[IniIgnore]
		public uint RealValueJP { get; set; }
		[IniAlwaysInclude]
		public int VoiceFileJPID { get; set; }
		[IniAlwaysInclude]
		public int EventVoiceJPID { get; set; }
		[IniIgnore]
		public uint RealValueEN { get; set; }
		[IniAlwaysInclude]
		public int VoiceFileENID { get; set; }
		[IniAlwaysInclude]
		public int EventVoiceENID { get; set; }
		public static int Size { get { return 0x10; } }

		public SA2BetaCutsceneVoiceInfo() { }

		public SA2BetaCutsceneVoiceInfo(byte[] file, int address)
		{
			VoiceFileJPID = ByteConverter.ToInt32(file, address);
			address += sizeof(int);
			RealValueJP = ByteConverter.ToUInt32(file, address);
			EventIDConstant = (int)Math.Floor(decimal.Divide(RealValueJP, 1000));
			switch (EventIDConstant)
			{
				case 1:
				case 7:
				case 8:
				case 10:
				case 12:
				case 13:
				case 18:
				case 23:
				case 104:
				case 108:
				case 110:
				case 114:
				case 115:
				case 117:
				case 121:
				case 125:
					EventID = "ME" + EventIDConstant.ToString("D4");
					break;
				default:
					EventID = "E" + EventIDConstant.ToString("D4");
					break;
			}
			EventVoiceJPID = (int)(RealValueJP % 1000);
			address += sizeof(int);
			VoiceFileENID = ByteConverter.ToInt32(file, address);
			address += sizeof(int);
			RealValueEN = ByteConverter.ToUInt32(file, address);
			EventVoiceENID = (int)(RealValueEN % 1000);

		}

		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}

		public string ToStruct()
		{
			StringBuilder sb = new StringBuilder("{ ");
			sb.Append(VoiceFileJPID);
			sb.Append(", ");
			sb.Append(RealValueJP);
			sb.Append(", ");
			sb.Append(VoiceFileENID);
			sb.Append(", ");
			sb.Append(RealValueEN);
			sb.Append(" }");
			return sb.ToString();
		}
	}

	public static class ChaoItemStats
	{
		public static ChaoItemStatsEntry[] Load(string filename)
		{
			return IniSerializer.Deserialize<ChaoItemStatsEntry[]>(filename);
		}

		public static ChaoItemStatsEntry[] Load(byte[] file, int address, int count)
		{
			ChaoItemStatsEntry[] result = new ChaoItemStatsEntry[count];
			for (int i = 0; i < count; i++)
			{
				result[i] = new ChaoItemStatsEntry(file, address);
				address += ChaoItemStatsEntry.Size;
			}
			return result;
		}

		public static void Save(this ChaoItemStatsEntry[] list, string filename)
		{
			IniSerializer.Serialize(list, filename);
		}
	}

	[Serializable]
	public class ChaoItemStatsEntry
	{
		public ChaoItemStatsEntry() {}
		public ChaoItemStatsEntry(byte[] file, int address)
		{
			Mood = ByteConverter.ToInt16(file, address);
			address += sizeof(short);
			Belly = ByteConverter.ToInt16(file, address);
			address += sizeof(short);
			Swim = ByteConverter.ToInt16(file, address);
			address += sizeof(short);
			Fly = ByteConverter.ToInt16(file, address);
			address += sizeof(short);
			Run = ByteConverter.ToInt16(file, address);
			address += sizeof(short);
			Power = ByteConverter.ToInt16(file, address);
			address += sizeof(short);
			Stamina = ByteConverter.ToInt16(file, address);
			address += sizeof(short);
			Luck = ByteConverter.ToInt16(file, address);
			address += sizeof(short);
			Intelligence = ByteConverter.ToInt16(file, address);
			address += sizeof(short);
			Unknown = ByteConverter.ToInt16(file, address);
		}

		[IniAlwaysInclude]
		public short Mood { get; set; }
		[IniAlwaysInclude]
		public short Belly { get; set; }
		[IniAlwaysInclude]
		public short Swim { get; set; }
		[IniAlwaysInclude]
		public short Fly { get; set; }
		[IniAlwaysInclude]
		public short Run { get; set; }
		[IniAlwaysInclude]
		public short Power { get; set; }
		[IniAlwaysInclude]
		public short Stamina { get; set; }
		[IniAlwaysInclude]
		public short Luck { get; set; }
		[IniAlwaysInclude]
		public short Intelligence { get; set; }
		public short Unknown { get; set; }

		public static int Size { get { return 0x14; } }

		public string ToStruct()
		{
			StringBuilder sb = new StringBuilder("{ ");
			sb.Append(Mood);
			sb.AppendFormat(", ");
			sb.Append(Belly);
			sb.AppendFormat(", ");
			sb.Append(Swim);
			sb.AppendFormat(", ");
			sb.Append(Fly);
			sb.AppendFormat(", ");
			sb.Append(Run);
			sb.AppendFormat(", ");
			sb.Append(Power);
			sb.AppendFormat(", ");
			sb.Append(Stamina);
			sb.AppendFormat(", ");
			sb.Append(Luck);
			sb.AppendFormat(", ");
			sb.Append(Intelligence);
			sb.AppendFormat(", ");
			sb.Append(Unknown);
			sb.Append(" }");
			return sb.ToString();
		}
	}

	public static class DCChaoItemStats
	{
		public static DCChaoItemStatsEntry[] Load(string filename)
		{
			return IniSerializer.Deserialize<DCChaoItemStatsEntry[]>(filename);
		}

		public static DCChaoItemStatsEntry[] Load(byte[] file, int address, int count)
		{
			DCChaoItemStatsEntry[] result = new DCChaoItemStatsEntry[count];
			for (int i = 0; i < count; i++)
			{
				result[i] = new DCChaoItemStatsEntry(file, address);
				address += DCChaoItemStatsEntry.Size;
			}
			return result;
		}

		public static void Save(this DCChaoItemStatsEntry[] list, string filename)
		{
			IniSerializer.Serialize(list, filename);
		}
	}

	[Serializable]
	public class DCChaoItemStatsEntry
	{
		public DCChaoItemStatsEntry() { }
		public DCChaoItemStatsEntry(byte[] file, int address)
		{
			Swim = ByteConverter.ToInt16(file, address);
			address += sizeof(short);
			Fly = ByteConverter.ToInt16(file, address);
			address += sizeof(short);
			Run = ByteConverter.ToInt16(file, address);
			address += sizeof(short);
			Power = ByteConverter.ToInt16(file, address);
		}

		[IniAlwaysInclude]
		public short Swim { get; set; }
		[IniAlwaysInclude]
		public short Fly { get; set; }
		[IniAlwaysInclude]
		public short Run { get; set; }
		[IniAlwaysInclude]
		public short Power { get; set; }

		public static int Size { get { return 0x8; } }

		public string ToStruct()
		{
			StringBuilder sb = new StringBuilder("{ ");
			sb.Append(Swim);
			sb.AppendFormat(", ");
			sb.Append(Fly);
			sb.AppendFormat(", ");
			sb.Append(Run);
			sb.AppendFormat(", ");
			sb.Append(Power);
			sb.Append(" }");
			return sb.ToString();
		}
	}
	
	public class CharaObjectData
	{
		public string MainModel { get; set; }
		public string Animation1 { get; set; }
		public string Animation2 { get; set; }
		public string Animation3 { get; set; }
		public string AccessoryModel { get; set; }
		public string AccessoryAttachNode { get; set; }
		public string SuperModel { get; set; }
		public string SuperAnimation1 { get; set; }
		public string SuperAnimation2 { get; set; }
		public string SuperAnimation3 { get; set; }
		public int Unknown1 { get; set; }
		public int Rating { get; set; }
		public int DescriptionID { get; set; }
		public int TextBackTexture { get; set; }
		public float SelectionSize { get; set; }
		public float? Unknown5 { get { return null; } set { if (value.HasValue) SelectionSize = value.Value; } }


		public string ToStruct()
		{
			StringBuilder sb = new StringBuilder("{ ");
			sb.AppendFormat("{0}, ", MainModel);
			sb.AppendFormat("{0}, ", Animation1);
			sb.AppendFormat("{0}, ", Animation2);
			sb.AppendFormat("{0}, ", Animation3);
			if (!string.IsNullOrEmpty(AccessoryModel))
			{
				sb.AppendFormat("{0}, ", AccessoryModel);
				sb.AppendFormat("{0}, ", AccessoryAttachNode);
			}
			else
				sb.Append("NULL, NULL, ");
			if (!string.IsNullOrEmpty(SuperModel))
			{
				sb.AppendFormat("{0}, ", SuperModel);
				sb.AppendFormat("{0}, ", SuperAnimation1);
				sb.AppendFormat("{0}, ", SuperAnimation2);
				sb.AppendFormat("{0}, ", SuperAnimation3);
			}
			else
				sb.Append("NULL, NULL, NULL, NULL, ");
			sb.AppendFormat("{0}, ", Unknown1);
			sb.AppendFormat("{0}, ", Rating);
			sb.AppendFormat("{0}, ", DescriptionID);
			sb.AppendFormat("{0}, ", TextBackTexture);
			sb.Append(SelectionSize.ToC());
			sb.Append(" }");
			return sb.ToString();
		}
	}

	public class KartSpecialInfo
	{
		[IniAlwaysInclude]
		public SA2KartCharacters ID { get; set; }
		public string Model { get; set; }
		public string LowModel { get; set; }
		[TypeConverter(typeof(UInt32HexConverter))]
		public uint TexList { get; set; }
		public int Unknown1 { get; set; }
		public int Unknown2 { get; set; }
		public int Unknown3 { get; set; }

		public string ToStruct()
		{
			StringBuilder sb = new StringBuilder("{ ");
			sb.AppendFormat("{0}, ", ID);
			sb.AppendFormat("{0}, ", Model);
			if (!string.IsNullOrEmpty(LowModel))
			{
				sb.AppendFormat("{0}, ", LowModel);
			}
			else
				sb.Append("NULL, ");
			sb.AppendFormat("{0}, ", TexList.ToCHex());
			sb.AppendFormat("{0}, ", Unknown1);
			sb.AppendFormat("{0}, ", Unknown2);
			sb.AppendFormat("{0}", Unknown3);
			sb.Append(" }");
			return sb.ToString();
		}
	}

	public class DCKartSpecialInfo
	{
		[IniAlwaysInclude]
		public SA2KartCharacters ID { get; set; }
		public string Model { get; set; }
		public string LowModel { get; set; }
		[TypeConverter(typeof(UInt32HexConverter))]
		public uint TexList { get; set; }
		public int Unknown1 { get; set; }
		public int Unknown2 { get; set; }
		public int Unknown3 { get; set; }

		public string ToStruct()
		{
			StringBuilder sb = new StringBuilder("{ ");
			sb.AppendFormat("{0}, ", ID);
			sb.AppendFormat("{0}, ", Model);
			if (!string.IsNullOrEmpty(LowModel))
			{
				sb.AppendFormat("{0}, ", LowModel);
			}
			else
				sb.Append("NULL, ");
			sb.AppendFormat("{0}, ", TexList.ToCHex());
			sb.AppendFormat("{0}, ", Unknown1);
			sb.AppendFormat("{0}, ", Unknown2);
			sb.Append(" }");
			return sb.ToString();
		}
	}

	public class KartModelInfo
	{
		public string Model { get; set; }
		public string Collision { get; set; }
		public Vertex EndPoint { get; set; }
		[TypeConverter(typeof(UInt32HexConverter))]
		public uint YRotation { get; set; }
		public List<KartStreetLightPos> StreetLights { get; set; }

		public string ToStruct(string label)
		{
			StringBuilder sb = new StringBuilder("{ ");
			if (!string.IsNullOrEmpty(Model))
			{
				sb.AppendFormat("{0}, ", Model);
				sb.AppendFormat("{0}, ", Collision);
			}
			else
				sb.Append("NULL, NULL, ");
			sb.AppendFormat("{0}, ", EndPoint.ToStruct());
			sb.AppendFormat("{0}, ", YRotation.ToCHex());
			sb.Append("{ }, ");
			if (StreetLights?.Count > 0)
			{
				sb.AppendFormat("{0}, ", label);
				sb.AppendFormat("{0}", StreetLights.Count);
			}
			else
				sb.Append("NULL");
			sb.Append(" }");
			return sb.ToString();
		}
	}

	public class KartStreetLightPos
	{
		public Vertex Position { get; set; }
		[TypeConverter(typeof(UInt32HexConverter))]
		public uint YRotation { get; set; }

		public string ToStruct() => $"{{ {Position.ToStruct()}, {YRotation.ToCHex()} }}";
	}

	public static class KartCourse
	{
		public static List<byte> ReadBinary(byte[] file, int address, uint imageBase)
		{
			int ptr = (int)(ByteConverter.ToInt32(file, address) - imageBase);
			int cnt = ByteConverter.ToInt32(file, address + 4);
			var course = new List<byte>(cnt);
			for (; cnt > 0; cnt--)
				course.Add(file[ptr++]);
			return course;
		}

		public static List<byte> Load(string filename) => File.ReadAllLines(filename).Select(a => byte.Parse(a)).ToList();

		public static void Save(List<byte> course, string filename) => File.WriteAllLines(filename, course.Select(a => a.ToString()));
	}

	public class ModelIndex
	{
		public string Model { get; set; }
		[IniAlwaysInclude]
		public uint ID { get; set; }

		public string ToStruct()
		{
			StringBuilder sb = new StringBuilder("{ ");
			sb.AppendFormat("{0}, ", Model);
			sb.AppendFormat("{0}, ", ID.ToCHex());
			sb.Append(" }");
			return sb.ToString();
		}
	}

	public class KartObjectArray
	{
		public string Model { get; set; }
		public uint Property { get; set; }
		public string Unknown1 { get; set; }

		public string ToStruct()
		{
			StringBuilder sb = new StringBuilder("{ ");
			sb.AppendFormat("{0}, ", Model);
			sb.AppendFormat("{0}, ", Property.ToString());
			sb.AppendFormat("{0}, ", Unknown1);
			sb.Append(" }");
			return sb.ToString();
		}
	}

	public class KartMenuElements
	{
		[IniAlwaysInclude]
		public SA2Characters CharacterID { get; set; }
		public uint PortraitID { get; set; }
		public string KartModel { get; set; }
		public byte SPD { get; set; }
		public byte ACL { get; set; }
		public byte BRK { get; set; }
		public byte GRP { get; set; }

		public string ToStruct()
		{
			StringBuilder sb = new StringBuilder("{ ");
			sb.AppendFormat("{0}, ", CharacterID.ToC());
			sb.AppendFormat("{0}, ", PortraitID.ToCHex());
			sb.AppendFormat("{0}, ", KartModel);
			sb.AppendFormat("{0}, ", SPD.ToString());
			sb.AppendFormat("{0}, ", ACL.ToString());
			sb.AppendFormat("{0}, ", BRK.ToString());
			sb.AppendFormat("{0}, ", GRP.ToString());
			sb.Append(" }");
			return sb.ToString();
		}
	}

	public class KartSoundParameters
	{
		[IniAlwaysInclude]
		public uint EngineSFXID { get; set; }
		public uint BrakeSFXID { get; set; }
		public uint FinishVoice { get; set; }
		public uint FirstVoice { get; set; }
		public uint LastVoice { get; set; }
		public string ShadowModel { get; set; }

		public string ToStruct()
		{
			StringBuilder sb = new StringBuilder("{ ");
			sb.AppendFormat("{0}, ", EngineSFXID.ToCHex());
			sb.AppendFormat("{0}, ", BrakeSFXID.ToCHex());
			if (FinishVoice != 0xFFFFFFFF)
			{
				sb.AppendFormat("{0}, ", FinishVoice.ToString());
				sb.AppendFormat("{0}, ", FirstVoice.ToString());
				sb.AppendFormat("{0}, ", LastVoice.ToString());
			}
			else
				sb.Append("NULL, NULL, NULL");
			sb.AppendFormat("{0}, ", ShadowModel);
			sb.Append(" }");
			return sb.ToString();
		}
	}

	public class MotionTableEntry
	{
		public string Motion { get; set; }
		[TypeConverter(typeof(UInt32HexConverter))]
		public ushort LoopProperty { get; set; }
		public ushort? Flag1 { get { return null; } set { if (value.HasValue) LoopProperty = value.Value; } }
		public ushort Pose { get; set; }
		public int NextAnimation { get; set; }
		public int? TransitionID { get { return null; } set { if (value.HasValue) NextAnimation = value.Value; } }
		[TypeConverter(typeof(UInt32HexConverter))]
		public uint TransitionSpeed { get; set; }
		public uint? Flag2 { get { return null; } set { if (value.HasValue) TransitionSpeed = value.Value; } }
		public float StartFrame { get; set; }
		public float EndFrame { get; set; }
		public float PlaySpeed { get; set; }

		public string ToStruct()
		{
			StringBuilder sb = new StringBuilder("{ ");
			if (!string.IsNullOrEmpty(Motion))
			{
				sb.AppendFormat("{0}, ", Motion);
			}
			else
				sb.Append("NULL, ");
			sb.AppendFormat("{0}, ", LoopProperty.ToCHex());
			sb.AppendFormat("{0}, ", Pose.ToCHex());
			if (NextAnimation != -1)
			{
				sb.AppendFormat("{0}, ", NextAnimation);
			}
			else
				sb.Append("NULL, ");
			sb.AppendFormat("{0}, ", TransitionSpeed.ToCHex());
			sb.AppendFormat("{0}, ", StartFrame.ToC());
			sb.AppendFormat("{0}, ", EndFrame.ToC());
			sb.AppendFormat("{0}", PlaySpeed.ToC());
			sb.Append(" }");
			return sb.ToString();
		}
	}

	[Serializable]
	public class SingleString
	{
		[IniIgnore]
		private Languages Language;
		public string[][] Text { get; private set; }
		private SingleString() { Text = new string[1][]; }

		public SingleString(byte[] file, int address, uint imageBase, int count, Languages lang): this()
		{
			Text[0] = StringArray.Load(file, address, imageBase, count, lang);
			Language = lang;
		}

		public void Save(string directory)
		{
			Save(directory, out string[] hashes);
		}

		public void Save(string directory, out string[] hashes)
		{
			hashes = new string[1];
			Directory.CreateDirectory(directory);
			string textname = Path.Combine(directory, Language.ToString() + ".txt");
			Text[0].Save(textname);
			hashes[0] = HelperFunctions.FileHash(textname);
		}
	}

	[Serializable]
	public class MultilingualString
	{
		public string[][] Text { get; private set; }
		
		private MultilingualString() { Text = new string[5][]; }

		public MultilingualString(byte[] file, int address, uint imageBase, int count, bool doublePointer = false) : this()
		{
			for (int i = 0; i < 5; i++)
			{
				Text[i] = new string[count];
				for (int c = 0; c < count; c++)
				{
					if (doublePointer)
						Text[i][c] = StringArray.Load(file, file.GetPointer(address, imageBase) + 4*c, imageBase, 1, (Languages)i)[0];
					else
						Text[i][c] = StringArray.Load(file, address, imageBase, 1, (Languages)i)[0];
				}
				address += 4;
			}
		}

		public void Save(string directory)
		{
			Save(directory, out string[] hashes);
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

	public class NinjaCamera
	{
		public Vertex Position { get; set; } // Camera position
		public Vertex Vector { get; set; } // Camera vector in unit direction[Local Z axis]
		[TypeConverter(typeof(UInt32HexConverter))]
		public int Roll { get; set; } // Camera roll
		[TypeConverter(typeof(UInt32HexConverter))]
		public int Angle { get; set; } // Camera angle
		public float NearClip { get; set; } // Near clip 
		public float FarClip { get; set; } // Far clip
		public Vertex LocalX { get; set; } // Camera local X axis
		public Vertex LocalY { get; set; } //Camera local Y axis

		public NinjaCamera(byte[] file, int address)
		{
			Position = new Vertex(file, address);
			Vector = new Vertex(file, address + 12);
			Roll = ByteConverter.ToInt32(file, address + 24);
			Angle = ByteConverter.ToInt32(file, address + 28);
			NearClip = ByteConverter.ToSingle(file, address + 32);
			FarClip = ByteConverter.ToSingle(file, address + 36);
			LocalX = new Vertex(file, address + 40);
			LocalY = new Vertex(file, address + 52);
		}

		public void Save(string fileOutputPath)
		{
			IniSerializer.Serialize(this, fileOutputPath);
		}

		public static int Size { get { return 0x40; } }

		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(Size);
			result.AddRange(Position.GetBytes());
			result.AddRange(Vector.GetBytes());
			result.AddRange(ByteConverter.GetBytes(Roll));
			result.AddRange(ByteConverter.GetBytes(Angle));
			result.AddRange(ByteConverter.GetBytes(NearClip));
			result.AddRange(ByteConverter.GetBytes(FarClip));
			result.AddRange(LocalX.GetBytes());
			result.AddRange(LocalY.GetBytes());
			result.Align(0x40);
			return result.ToArray();
		}
	}

	public class FogData
	{
		public float FogStart { get; set; }
		public float FogEnd { get; set; }
		public byte A { get; set; }
		public byte R { get; set; }
		public byte G { get; set; }
		public byte B { get; set; }
		public int FogEnabled { get; set; }

		public FogData(byte[] file, int address)
		{
			FogStart = ByteConverter.ToSingle(file, address);
			FogEnd = ByteConverter.ToSingle(file, address + 4);
			if (ByteConverter.BigEndian)
			{
				A = file[address + 8];
				R = file[address + 9];
				G = file[address + 10];
				B = file[address + 11];
			}
			else
			{
				A = file[address + 11];
				R = file[address + 10];
				G = file[address + 9];
				B = file[address + 8];
			}
			FogEnabled = ByteConverter.ToInt32(file, address + 12);
		}

		public void Save(string fileOutputPath)
		{
			IniSerializer.Serialize(this, fileOutputPath);
		}

		public FogData() { }
	}

	public class FogDataArray
	{
		[IniIgnore]
		public FogData High;
		[IniIgnore]
		public FogData Medium;
		[IniIgnore]
		public FogData Low;
		// Fog Toggle
		public int FogEnabledHigh { get { return High.FogEnabled; } set { High.FogEnabled = value; } }
		public int FogEnabledMedium { get { return Medium.FogEnabled; } set { Medium.FogEnabled = value; } }
		public int FogEnabledLow { get { return Low.FogEnabled; } set { Low.FogEnabled = value; } }
		// Fog Start
		public float FogStartHigh { get { return High.FogStart; } set { High.FogStart = value; } }
		public float FogStartMedium { get { return Medium.FogStart; } set { Medium.FogStart = value; } }
		public float FogStartLow { get { return Low.FogStart; } set { Low.FogStart = value; } }
		// Fog End
		public float FogEndHigh { get { return High.FogEnd; } set { High.FogEnd = value; } }
		public float FogEndMedium { get { return Medium.FogEnd; } set { Medium.FogEnd = value; } }
		public float FogEndLow { get { return Low.FogEnd; } set { Low.FogEnd = value; } }
		// Fog Color A
		public byte ColorA_High { get { return High.A; } set { High.A = value; } }
		public byte ColorA_Medium { get { return Medium.A; } set { Medium.A = value; } }
		public byte ColorA_Low { get { return Low.A; } set { Low.A = value; } }
		// Fog Color R
		public byte ColorR_High { get { return High.R; } set { High.R = value; } }
		public byte ColorR_Medium { get { return Medium.R; } set { Medium.R = value; } }
		public byte ColorR_Low { get { return Low.R; } set { Low.R = value; } }
		// Fog Color G
		public byte ColorG_High { get { return High.G; } set { High.G = value; } }
		public byte ColorG_Medium { get { return Medium.G; } set { Medium.G = value; } }
		public byte ColorG_Low { get { return Low.G; } set { Low.G = value; } }
		// Fog Color B
		public byte ColorB_High { get { return High.B; } set { High.B = value; } }
		public byte ColorB_Medium { get { return Medium.B; } set { Medium.B = value; } }
		public byte ColorB_Low { get { return Low.B; } set { Low.B = value; } }

		public FogDataArray(byte[] datafile, int address)
		{
			High = new FogData(datafile, address);
			Medium = new FogData(datafile, address + 16);
			Low = new FogData(datafile, address + 32);
		}

		public void Save(string fileOutputPath)
		{
			IniSerializer.Serialize(this, fileOutputPath);
		}

		public FogDataArray() 
		{
			High = new FogData();
			Medium = new FogData();
			Low = new FogData();
		}
	}

	public class FogDataTable
	{
		[IniCollection(IniCollectionMode.IndexOnly)]
		public FogDataArray[] Act { get; set; }

		public FogDataTable(byte[] datafile, int address, uint imageBase, int count = 3)
		{
			List<FogDataArray> foglist = new List<FogDataArray>();
			for (int i = 0; i < count; i++)
			{
				uint ptr = BitConverter.ToUInt32(datafile, address + i * 4);
				if (ptr != 0)
				{
					FogDataArray arr = new FogDataArray(datafile, (int)(ptr - imageBase));
					foglist.Add(arr);
				}
			}
			Act = foglist.ToArray();
		}

		public void Save(string fileOutputPath)
		{
			IniSerializer.Serialize(this, fileOutputPath);
		}

		public FogDataTable() { }
	}

	public class LSPaletteData
	{
		[Flags]
		public enum LSPaletteFlags
		{
			Enabled = 0x1,
			UseLSLightDirection = 0x2, // Use the stage light direction if false
			IgnoreDirection = 0x4, // Set direction if false
			OverrideLastLight = 0x8, // Reverses light direction on the last light (ID 3)
			Unknown = 0x10
		}

		public enum LSPaletteTypes
		{
			Character = 0, // Characters, NPCs, Leon, some other enemies
			CharacterAlt = 6, // Gamma, Super Sonic, some Egg Carrier NPCs
			Boss = 8
		}

		[IniAlwaysInclude]
		public SA1LevelIDs Level { get; set; }
		[IniAlwaysInclude]
		public byte Act { get; set; }
		[IniAlwaysInclude]
		public LSPaletteTypes Type { get; set; }
		[IniAlwaysInclude]
		public LSPaletteFlags Flags { get; set; }
		[IniAlwaysInclude]
		public Vertex Direction { get; set; }
		[IniAlwaysInclude]
		public float Diffuse { get; set; }
		[IniAlwaysInclude]
		public Vertex Ambient { get; set; }
		[IniAlwaysInclude]
		public float Color1Power { get; set; }
		[IniAlwaysInclude]
		public Vertex Color1 { get; set; }
		[IniAlwaysInclude]
		public float Specular1Power { get; set; }
		[IniAlwaysInclude]
		public Vertex Specular1 { get; set; }
		[IniAlwaysInclude]
		public float Color2Power { get; set; }
		[IniAlwaysInclude]
		public Vertex Color2 { get; set; }
		[IniAlwaysInclude]
		public float Specular2Power { get; set; }
		[IniAlwaysInclude]
		public Vertex Specular2 { get; set; }

		public LSPaletteData(byte[] file, int address)
		{
			Level = (SA1LevelIDs)file[address];
			Act = file[address + 1];
			Type = (LSPaletteTypes)file[address + 2];
			Flags = (LSPaletteFlags)file[address + 3];
			Direction = new Vertex(file, address + 4);
			Diffuse = ByteConverter.ToSingle(file, address + 16);
			Ambient = new Vertex(file, address + 20);
			Color1Power = ByteConverter.ToSingle(file, address + 32);
			Color1 = new Vertex(file, address + 36);
			Specular1Power = ByteConverter.ToSingle(file, address + 48);
			Specular1 = new Vertex(file, address + 52);
			Color2Power = ByteConverter.ToSingle(file, address + 64);
			Color2 = new Vertex(file, address + 68);
			Specular2Power = ByteConverter.ToSingle(file, address + 80);
			Specular2 = new Vertex(file, address + 84);
		}

		public LSPaletteData()
		{ }
	}

	public class LSPaletteDataList
	{
		[IniCollection(IniCollectionMode.IndexOnly)]
		public LSPaletteData[] Lights { get; set; }

		public LSPaletteDataList(byte[] file, int address, int count)
		{
			List<LSPaletteData> lightlist = new List<LSPaletteData>();
			for (int i = 0; i < count; i++)
			{
				lightlist.Add(new LSPaletteData(file, address + i * 96));
			}
			Lights = lightlist.ToArray();
		}

		public void Save(string fileOutputPath)
		{
			IniSerializer.Serialize(this, fileOutputPath);
		}

		public static List<LSPaletteData> Load(string filename)
		{
			LSPaletteDataList origlist = IniSerializer.Deserialize<LSPaletteDataList>(filename);
			List<LSPaletteData> lights = new List<LSPaletteData>(origlist.Lights.Length);
			lights.AddRange(origlist.Lights);
			return lights;
		}

		public LSPaletteDataList()
		{ }
	}

	public class PlayerParameter
	{
		public int jump2_timer { get; set; }
		public float pos_error { get; set; }
		public float lim_h_spd { get; set; }
		public float lim_v_spd { get; set; }
		public float max_x_spd { get; set; }
		public float max_psh_spd { get; set; }
		public float jmp_y_spd { get; set; }
		public float nocon_speed { get; set; }
		public float slide_speed { get; set; }
		public float jog_speed { get; set; }
		public float run_speed { get; set; }
		public float rush_speed { get; set; }
		public float crash_speed { get; set; }
		public float dash_speed { get; set; }
		public float jmp_addit { get; set; }
		public float run_accel { get; set; }
		public float air_accel { get; set; }
		public float slow_down { get; set; }
		public float run_break { get; set; }
		public float air_break { get; set; }
		public float air_resist_air { get; set; }
		public float air_resist { get; set; }
		public float air_resist_y { get; set; }
		public float air_resist_z { get; set; }
		public float grd_frict { get; set; }
		public float grd_frict_z { get; set; }
		public float lim_frict { get; set; }
		public float rat_bound { get; set; }
		public float rad { get; set; }
		public float height { get; set; }
		public float weight { get; set; }
		public float eyes_height { get; set; }
		public float center_height { get; set; }

		public PlayerParameter(byte[] file, int address)
		{
			jump2_timer = ByteConverter.ToInt32(file, address);
			pos_error = ByteConverter.ToSingle(file, address + 4);
			lim_h_spd = ByteConverter.ToSingle(file, address + 8);
			lim_v_spd = ByteConverter.ToSingle(file, address + 12);
			max_x_spd = ByteConverter.ToSingle(file, address + 16);
			max_psh_spd = ByteConverter.ToSingle(file, address + 20);
			jmp_y_spd = ByteConverter.ToSingle(file, address + 24);
			nocon_speed = ByteConverter.ToSingle(file, address + 28);
			slide_speed = ByteConverter.ToSingle(file, address + 32);
			jog_speed = ByteConverter.ToSingle(file, address + 36);
			run_speed = ByteConverter.ToSingle(file, address + 40);
			rush_speed = ByteConverter.ToSingle(file, address + 44);
			crash_speed = ByteConverter.ToSingle(file, address + 48);
			dash_speed = ByteConverter.ToSingle(file, address + 52);
			jmp_addit = ByteConverter.ToSingle(file, address + 56);
			run_accel = ByteConverter.ToSingle(file, address + 60);
			air_accel = ByteConverter.ToSingle(file, address + 64);
			slow_down = ByteConverter.ToSingle(file, address + 68);
			run_break = ByteConverter.ToSingle(file, address + 72);
			air_break = ByteConverter.ToSingle(file, address + 76);
			air_resist_air = ByteConverter.ToSingle(file, address + 80);
			air_resist = ByteConverter.ToSingle(file, address + 84);
			air_resist_y = ByteConverter.ToSingle(file, address + 88);
			air_resist_z = ByteConverter.ToSingle(file, address + 92);
			grd_frict = ByteConverter.ToSingle(file, address + 96);
			grd_frict_z = ByteConverter.ToSingle(file, address + 100);
			lim_frict = ByteConverter.ToSingle(file, address + 104);
			rat_bound = ByteConverter.ToSingle(file, address + 108);
			rad = ByteConverter.ToSingle(file, address + 112);
			height = ByteConverter.ToSingle(file, address + 116);
			weight = ByteConverter.ToSingle(file, address + 120);
			eyes_height = ByteConverter.ToSingle(file, address + 124);
			center_height = ByteConverter.ToSingle(file, address + 128);
		}

		public static PlayerParameter Load(string filename) => IniSerializer.Deserialize<PlayerParameter>(filename);

		public void Save(string fileOutputPath) => IniSerializer.Serialize(this, fileOutputPath);

		public string ToStruct()
		{
			StringBuilder sb = new StringBuilder("{ ");
			sb.AppendFormat("{0}, ", jump2_timer);
			sb.AppendFormat("{0}, ", pos_error.ToC());
			sb.AppendFormat("{0}, ", lim_h_spd.ToC());
			sb.AppendFormat("{0}, ", lim_v_spd.ToC());
			sb.AppendFormat("{0}, ", max_x_spd.ToC());
			sb.AppendFormat("{0}, ", max_psh_spd.ToC());
			sb.AppendFormat("{0}, ", jmp_y_spd.ToC());
			sb.AppendFormat("{0}, ", nocon_speed.ToC());
			sb.AppendFormat("{0}, ", slide_speed.ToC());
			sb.AppendFormat("{0}, ", jog_speed.ToC());
			sb.AppendFormat("{0}, ", run_speed.ToC());
			sb.AppendFormat("{0}, ", rush_speed.ToC());
			sb.AppendFormat("{0}, ", crash_speed.ToC());
			sb.AppendFormat("{0}, ", dash_speed.ToC());
			sb.AppendFormat("{0}, ", jmp_addit.ToC());
			sb.AppendFormat("{0}, ", run_accel.ToC());
			sb.AppendFormat("{0}, ", air_accel.ToC());
			sb.AppendFormat("{0}, ", slow_down.ToC());
			sb.AppendFormat("{0}, ", run_break.ToC());
			sb.AppendFormat("{0}, ", air_break.ToC());
			sb.AppendFormat("{0}, ", air_resist_air.ToC());
			sb.AppendFormat("{0}, ", air_resist.ToC());
			sb.AppendFormat("{0}, ", air_resist_y.ToC());
			sb.AppendFormat("{0}, ", air_resist_z.ToC());
			sb.AppendFormat("{0}, ", grd_frict.ToC());
			sb.AppendFormat("{0}, ", grd_frict_z.ToC());
			sb.AppendFormat("{0}, ", lim_frict.ToC());
			sb.AppendFormat("{0}, ", rat_bound.ToC());
			sb.AppendFormat("{0}, ", rad.ToC());
			sb.AppendFormat("{0}, ", height.ToC());
			sb.AppendFormat("{0}, ", weight.ToC());
			sb.AppendFormat("{0}, ", eyes_height.ToC());
			sb.AppendFormat("{0}", center_height.ToC());
			sb.Append(" }");
			return sb.ToString();
		}

		public PlayerParameter()
		{ }
	}

	public class KartPhysics
	{
		public float Acceleration { get; set; }
		public float BrakePower { get; set; }
		public float Deceleration { get; set; }
		public float SpeedCap { get; set; }
		public float Weight { get; set; }
		public float Unk1 { get; set; }
		public float DriftHandling { get; set; }
		public float DriftSpeedThreshold { get; set; }
		public float Unk2 { get; set; }
		public float TopSpeed { get; set; }

		public KartPhysics(byte[] file, int address)
		{
			Acceleration = ByteConverter.ToSingle(file, address);
			BrakePower = ByteConverter.ToSingle(file, address + 4);
			Deceleration = ByteConverter.ToSingle(file, address + 8);
			SpeedCap = ByteConverter.ToSingle(file, address + 12);
			Weight = ByteConverter.ToSingle(file, address + 16);
			Unk1 = ByteConverter.ToSingle(file, address + 20);
			DriftHandling = ByteConverter.ToSingle(file, address + 24);
			DriftSpeedThreshold = ByteConverter.ToSingle(file, address + 28);
			Unk2 = ByteConverter.ToSingle(file, address + 32);
			TopSpeed = ByteConverter.ToSingle(file, address + 36);	
		}

		public static KartPhysics Load(string filename) => IniSerializer.Deserialize<KartPhysics>(filename);

		public void Save(string fileOutputPath) => IniSerializer.Serialize(this, fileOutputPath);

		public string ToStruct()
		{
			StringBuilder sb = new StringBuilder("{ ");
			sb.AppendFormat("{0}, ", Acceleration.ToC());
			sb.AppendFormat("{0}, ", BrakePower.ToC());
			sb.AppendFormat("{0}, ", Deceleration.ToC());
			sb.AppendFormat("{0}, ", SpeedCap.ToC());
			sb.AppendFormat("{0}, ", Weight.ToC());
			sb.AppendFormat("{0}, ", Unk1.ToC());
			sb.AppendFormat("{0}, ", DriftHandling.ToC());
			sb.AppendFormat("{0}, ", DriftSpeedThreshold.ToC());
			sb.AppendFormat("{0}, ", Unk2.ToC());
			sb.AppendFormat("{0}, ", TopSpeed.ToC());
			sb.Append(" }");
			return sb.ToString();
		}

		public KartPhysics()
		{ }
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
			if (value is string)
				return uint.TryParse((string)value, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out uint i);
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
			if (value is string)
				return int.TryParse((string)value, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out int i);
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
			if (value is string)
				return ushort.TryParse((string)value, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out ushort i);
			return base.IsValid(context, value);
		}

	}
}