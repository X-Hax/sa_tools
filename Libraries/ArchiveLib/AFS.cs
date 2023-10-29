using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using VrSharp.Gvr;
using VrSharp.Pvr;
using VrSharp.Xvr;
using static ArchiveLib.GenericArchive;

// Archive format used in some Dreamcast, Gamecube and Xbox games.
namespace ArchiveLib
{
	public class AFSFile : GenericArchive
	{
		const uint Magic_AFS1 = 0x00534641;
		const uint Magic_AFS2 = 0x20534641; // No idea how it's different from AFS1

		AFSMetaMode MetaMode;

		public enum AFSType
		{
			AFS1,
			AFS2,
			Unknown
		}

		public enum AFSMetaMode
		{
			NoMeta,
			OffsetEndTable,
			OffsetBeforeFirstEntry,
		}

		public class AFSMetadata
		{
			public string Name;
			public DateTime Timestamp;
			public uint CustomData; // In old AFS files usually the same as data size, otherwise custom developer data

			public AFSMetadata(byte[] data, int offset)
			{
				Name = System.Text.Encoding.ASCII.GetString(data, offset, 32).TrimEnd((Char)0);
				Timestamp = new DateTime(BitConverter.ToUInt16(data, offset + 32), // Year
										 BitConverter.ToUInt16(data, offset + 34), // Month
										 BitConverter.ToUInt16(data, offset + 36), // Day
										 BitConverter.ToUInt16(data, offset + 38), // Hour
										 BitConverter.ToUInt16(data, offset + 40), // Minute
										 BitConverter.ToUInt16(data, offset + 42)); // Second
				CustomData = BitConverter.ToUInt32(data, offset + 44);
			}

			public AFSMetadata(int id, byte[] data)
			{
				Timestamp = DateTime.Now;
				Name += id.ToString("D4") + GetExtensionForData(data);
				CustomData = 0;
			}

			public AFSMetadata(string name, DateTime timestamp, uint custom)
			{
				Name = name;
				Timestamp = timestamp;
				CustomData = custom;
			}

			public byte[] GetBytes()
			{
				List<byte> result = new List<byte>();
				result.AddRange(new byte[32]);
				byte[] name = System.Text.Encoding.ASCII.GetBytes(Name);
				result.AddRange(BitConverter.GetBytes((ushort)Timestamp.Year));
				result.AddRange(BitConverter.GetBytes((ushort)Timestamp.Month));
				result.AddRange(BitConverter.GetBytes((ushort)Timestamp.Day));
				result.AddRange(BitConverter.GetBytes((ushort)Timestamp.Hour));
				result.AddRange(BitConverter.GetBytes((ushort)Timestamp.Minute));
				result.AddRange(BitConverter.GetBytes((ushort)Timestamp.Second));
				result.AddRange(BitConverter.GetBytes(CustomData));
				byte[] resultb = result.ToArray();
				Array.Copy(name, 0, resultb, 0, name.Length);
				return resultb;
			}

			public static string GetExtensionForData(byte[] Data)
			{
				// Texture archive
				switch (PuyoFile.Identify(Data))
				{
					case PuyoArchiveType.PVMFile:
						return ".pvm";
					case PuyoArchiveType.GVMFile:
						return ".gvm";
					case PuyoArchiveType.XVMFile:
						return ".xvm";
				}
				// Texture
				if (PvrTexture.Is(Data))
					return ".pvr";
				else if (GvrTexture.Is(Data))
					return ".gvr";
				else if (XvrTexture.Is(Data))
					return ".xvr";
				// Audio
				switch (BitConverter.ToUInt32(Data, 0))
				{
					case 0x544C4D53:
						return ".mlt";
					case 0x42504D53:
						return ".mpb";
					case 0x42534D53:
						return ".msb";
					case 0x42504653:
						return ".fpb";
					case 0x424F4653:
						return ".fob";
					case 0x56524453:
						return ".drv";
					case 0x42534F53:
						return ".osb";
					default:
						if (Data.Length < 20)
							return ".bin";
						if (BitConverter.ToUInt16(Data, 0) == 0x0080 && Data[4] == 0x03 && (Data[18] == 0x03 || Data[18] == 0x04))
							return ".adx";
						else
							return ".bin";
				}
			}

		}

		private static AFSType GetAFSType(byte[] data)
		{
			if (data == null || data.Length < 4)
				return AFSType.Unknown;
			switch (BitConverter.ToUInt32(data, 0))
			{
				case Magic_AFS1:
					return AFSType.AFS1;
				case Magic_AFS2:
					return AFSType.AFS2;
				default:
					return AFSType.Unknown;
			}
		}

		public override void CreateIndexFile(string path)
		{
			using (TextWriter texList = File.CreateText(Path.Combine(path, "index.txt")))
			{
				foreach (AFSEntry entry in Entries)
				{
					texList.WriteLine(entry.Name + ((entry.CustomData != 0 && entry.Data.Length != entry.CustomData) ? "," + entry.CustomData.ToString() : ""));
				}
				texList.Flush();
				texList.Close();
			}
		}

		public static bool Identify(byte[] data)
		{
			return GetAFSType(data) != AFSType.Unknown;
		}

		public AFSFile(byte[] afsdata)
		{
			Entries = new List<GenericArchiveEntry>();
			int numentries = BitConverter.ToInt32(afsdata, 4);
			AFSType afstype = GetAFSType(afsdata);
			if (afstype == AFSType.Unknown)
				return;
			int firstentryoffset = BitConverter.ToInt32(afsdata, 8);
			// Usually the offset for metadata is located after the entry table
			int metaoffset = BitConverter.ToInt32(afsdata, 8 + numentries * 8);
			MetaMode = AFSMetaMode.OffsetEndTable;
			// However, sometimes it is located 8 bytes before the first item instead
			if (metaoffset == 0)
			{
				metaoffset = BitConverter.ToInt32(afsdata, firstentryoffset - 8);
				MetaMode = AFSMetaMode.OffsetBeforeFirstEntry;
			}
			if (metaoffset == 0)
				MetaMode = AFSMetaMode.NoMeta;
			for (int u = 0; u < numentries; u++)
			{
				int entryoffset = BitConverter.ToInt32(afsdata, 8 + u * 8);
				int entrysize = BitConverter.ToInt32(afsdata, 8 + u * 8 + 4);
				int metaitemoffset = metaoffset != 0 ? metaoffset + 48 * u : 0;
				Entries.Add(new AFSEntry(u, afsdata, entryoffset, entrysize, metaitemoffset));
			}

		}

		public AFSFile(int count)
		{
			Entries = new List<GenericArchiveEntry>(count);
		}

		public AFSFile(AFSMetaMode mode) 
		{
			MetaMode = mode;
		}

		public byte[] GetBytes(AFSType type, AFSMetaMode metaMode)
		{
			List<byte> result = new List<byte>();
			result.AddRange(BitConverter.GetBytes(type == AFSType.AFS1 ? Magic_AFS1 : Magic_AFS2));
			result.AddRange(BitConverter.GetBytes(Entries.Count));
			// Entry table
			int entrytablesize = Math.Max(2040, Entries.Count * 8 + (0x8 + Entries.Count * 8) % 2048);
			byte[] entrytable = new byte[entrytablesize];
			result.AddRange(entrytable);
			// Align entries by 2048
			int firstentryaligned = entrytablesize + 8;
			int entrytableend = 8 + Entries.Count * 8;
			int currentoffset = firstentryaligned;
			// Add entries
			for (int index = 0; index < Entries.Count; index++)
			{
				// Align by 2048
				int sizealigned = Entries[index].Data.Length + (2048-Entries[index].Data.Length % 2048);
				byte[] entryaligned = new byte[sizealigned];
				Array.Copy(Entries[index].Data, entryaligned, Entries[index].Data.Length);
				// Save data
				result.AddRange(entryaligned);
				// Set start offset in the entry table
				Array.Copy(BitConverter.GetBytes((uint)currentoffset), 0, entrytable, index * 8, 4);
				// Set size in the entry table
				Array.Copy(BitConverter.GetBytes((uint)Entries[index].Data.Length), 0, entrytable, index * 8 + 4, 4);
				currentoffset += sizealigned;
			}
			// Set meta offset
			int metaoffset = result.Count;
			// Set metadata
			if (metaMode != AFSMetaMode.NoMeta)
			{
				// Add meta entries
				foreach (AFSEntry entry in Entries)
				{
					result.AddRange(new AFSMetadata(entry.Name, entry.Timestamp, entry.CustomData).GetBytes());
				}
				// Add bytes to align with 2048
				if (result.Count % 2048 != 0)
				{
					while (result.Count % 2048 > 0)
						result.Add(0);
				}
			}
			// Convert to bytes
			byte[] resultarr = result.ToArray();
			// Copy entry table
			Array.Copy(entrytable, 0, resultarr, 8, entrytable.Length);
			// Set meta offset
			Array.Copy(BitConverter.GetBytes(metaoffset), 0, resultarr, metaMode == AFSMetaMode.OffsetBeforeFirstEntry ? firstentryaligned - 0x8 : entrytableend, 4);
			// Set meta size
			Array.Copy(BitConverter.GetBytes(48 * Entries.Count), 0, resultarr, metaMode == AFSMetaMode.OffsetBeforeFirstEntry ? firstentryaligned - 0x4 : entrytableend + 4, 4);
			return resultarr;
		}

		public override byte[] GetBytes()
		{
			return GetBytes(AFSType.AFS1, MetaMode);
		}
	}

	public class AFSEntry : GenericArchiveEntry
	{
		public DateTime Timestamp;
		public uint CustomData; // In old AFS files usually the same as data size, otherwise custom developer data

		public AFSEntry(int id, byte[] data, int offset, int size, int metaoffset)
		{
			AFSFile.AFSMetadata meta = metaoffset != 0 ? new AFSFile.AFSMetadata(data, metaoffset) : new AFSFile.AFSMetadata(id, data);
			Name = meta.Name;
			Timestamp = meta.Timestamp;
			CustomData = meta.CustomData;
			Data = new byte[size];
			Array.Copy(data, offset, Data, 0, size);
		}

		public AFSEntry(string filename, DateTime timestamp, uint custom)
		{
			Data = File.ReadAllBytes(filename);
			Name = Path.GetFileName(filename);
			Timestamp = timestamp;
			CustomData = custom != 0 ? custom : (uint)Data.Length;
		}

		public override Bitmap GetBitmap()
		{
			throw new NotImplementedException();
		}
	}
}