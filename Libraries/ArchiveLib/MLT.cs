using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace ArchiveLib
{
	/// <summary>
	/// Dreamcast Multi-Unit (MLT) container format used for soundbanks in SA1 and SA2.
	/// This class is for the base archive only, not the invidivual banks (MPB, MSB etc.) contained within.
	/// </summary>
	public class MLTFile : GenericArchive
    {
		/// <summary>MLT format version. Set to 1 in SA1 and 2 in SA2.
		public byte Version;
		/// <summary>MLT minor format version. Set to 1 in SA1 and 0 in SA2.
		public byte Revision;

        public MLTFile(byte[] file, string filename = "")
        {
            Version = file[4];
            Revision = file[5];
            int numfiles = BitConverter.ToInt32(file, 8);
            int fileoffset = 0x20;
            for (int u = 0; u < numfiles; u++)
            {
                Entries.Add(new MLTEntry(file, fileoffset + u * 32, filename));
            }
        }

        public override byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            // SMLT header
            result.AddRange(System.Text.Encoding.ASCII.GetBytes("SMLT"));
            result.Add(Version);
            result.Add(Revision);
            result.AddRange(BitConverter.GetBytes((ushort)0));
            result.AddRange(BitConverter.GetBytes(Entries.Count));
            for (int k = 0; k < 20; k++)
                result.Add(255);
            // Create array for all data items before adding pointers to individual headers
            Dictionary<MLTEntry, int> itemArray = new Dictionary<MLTEntry, int>();
            int pointer = 32 + Entries.Count * 32;
            foreach (MLTEntry entry in Entries)
            {
                itemArray.Add(entry, entry.Data == null ? -1 : pointer);
                if (entry.Data != null)
                    pointer += entry.Data.Length;
            }
            foreach (var item in itemArray)
            {
                result.Add((byte)('S'));
                result.AddRange(System.Text.Encoding.ASCII.GetBytes(item.Key.Type.ToString()));
                result.AddRange(BitConverter.GetBytes(item.Key.BankID));
                result.AddRange(BitConverter.GetBytes(item.Key.LoadAddress));
                result.AddRange(BitConverter.GetBytes(item.Key.AllocatedMemory));
                result.AddRange(BitConverter.GetBytes(item.Value));
                result.AddRange(BitConverter.GetBytes(item.Key.Data == null ? -1 : item.Key.Data.Length));
                result.AddRange(BitConverter.GetBytes((int)0));
                result.AddRange(BitConverter.GetBytes((int)0));
            }
            foreach (var item in itemArray)
            {
                if (item.Key.Data != null)
                    result.AddRange(item.Key.Data);
            }
            return result.ToArray();
        }

        public override void CreateIndexFile(string path)
        {
            using (System.IO.TextWriter texList = File.CreateText(Path.Combine(path, "index.txt")))
            {
                foreach (MLTEntry entry in Entries)
                    texList.WriteLine("{0},{1},{2},{3}", entry.Name, entry.BankID.ToString("D2"), entry.LoadAddress.ToString("X"), entry.AllocatedMemory.ToString());
                texList.Flush();
                texList.Close();
            }
            using (System.IO.TextWriter verList = File.CreateText(Path.Combine(path, "version.txt")))
            {
                verList.WriteLine(Version);
                verList.WriteLine(Revision);
                verList.Flush();
                verList.Close();
            }
        }

        public class MLTEntry : GenericArchiveEntry
        {
            public MLTEntryType Type;
            public int BankID;
            public int LoadAddress;
            public int AllocatedMemory;

            public MLTEntry(byte[] file, int offset, string filename = "")
            {
                Type = ((MLTEntryType)BitConverter.ToUInt32(file, offset));
                BankID = file[offset + 4];
                LoadAddress = BitConverter.ToInt32(file, offset + 0x08);
                AllocatedMemory = BitConverter.ToInt32(file, offset + 0x0C);
                Name = (filename == "" ? "BANK" : filename + "_BANK") + BankID.ToString("D2") + GetMLTItemExtension(file, offset);
                int pointer = BitConverter.ToInt32(file, offset + 0x10);
                int size = BitConverter.ToInt32(file, offset + 0x14);
                if (pointer != -1)
                {
                    Data = new byte[size];
                    Array.Copy(file, pointer, Data, 0, size);
                }
                /*
                Console.Write("Entry {0} Bank {1}, Address {2}, Allocated Memory {3}", Type.ToString(), BankID.ToString(), LoadAddress.ToString("X"), AllocatedMemory.ToString(""));
                if (Data != null)
                    Console.Write(", Bank Data Size {0}", Data.Length.ToString());
                else
                    Console.Write(", No Bank Data");
                Console.Write(System.Environment.NewLine);
                */
            }

            public MLTEntry(string filename, int bankID, int loadAddr, int allocMem)
            {
                Type = GetMLTEntryTypeFromFilename(filename);
                BankID = bankID;
                LoadAddress = loadAddr;
                AllocatedMemory = allocMem;
                if (File.Exists(filename))
                    Data = File.ReadAllBytes(filename);
            }

			public MLTEntry() { }

			public byte[] GetBytes()
            {
                List<byte> result = new List<byte>();
                if (Data != null)
                    result.AddRange(Data);
                return result.ToArray();
            }

            public override Bitmap GetBitmap()
            {
                throw new NotImplementedException();
            }
        }

        public MLTFile(int version = 1, int revision = 1)
        {
            Version = (byte)version;
            Revision = (byte)revision;
        }

		/// <summary>Type of MLT entry: MPB, MSB etc.</summary>
        public enum MLTEntryType : uint
        {
			/// <summary>Multi-Unit. Contains memory layout of the AICA for the entries.</summary>
            MLT = 0x544C4D53,
			/// <summary>MIDI Sequence Bank. Contains MIDI-like sequences.</summary>
			MSB = 0x42534D53,
			/// <summary>MIDI Program Bank. Contains MIDI-like instrument data.</summary>
			MPB = 0x42504D53, 
			/// <summary>MIDI Drum Bank. Similar to MPB except for drums.</summary>
			MDB = 0x42444D53,
			/// <summary>One-Shot Bank. Similar to MPB except with less MIDI stuff (no base note etc.).</summary>
			OSB = 0x42534F53, 
			/// <summary>FX Program Bank. Contains DSP programs for the AICA to create various effects.</summary>
			FPB = 0x42504653, 
			/// <summary>FX Output Bank. Required for sound effects using DSP.</summary>
			FOB = 0x424F4653,
			/// <summary>FX Program Work. Reserved DSP work area.</summary>
			FPW = 0x57504653,
			/// <summary>PCM Stream Ring Buffer (WTF is that anyway?).</summary>
			PSR = 0x52535053
        }

		/// <summary>Retrieves the file extension of an MLT entry.</summary>
		/// <param name="file">Byte array to analyze.</param>
		/// <param name="offset">Offset to analyze.</param>
		/// <returns>File extension with the leading period, such as ".mpb".</returns>
		public static string GetMLTItemExtension(byte[] file, int offset = 0)
        {
            switch ((MLTEntryType)BitConverter.ToUInt32(file, offset))
            {
                case MLTEntryType.MSB:
                    return ".msb";
                case MLTEntryType.MPB:
                    return ".mpb";
                case MLTEntryType.MDB:
                    return ".mdb";
                case MLTEntryType.OSB:
                    return ".osb";
                case MLTEntryType.FPB:
                    return ".fpb";
                case MLTEntryType.FOB:
                    return ".fob";
                case MLTEntryType.FPW:
                    return ".fpw";
                case MLTEntryType.PSR:
                    return ".psr";
                case MLTEntryType.MLT:
                default:
                    return ".mlt";
            }
        }

		/// <summary>Retrieves the type of an MLT entry from a file.</summary>
		/// <param name="filename">Path to the file to analyze.</param>
		/// <returns>MLT entry type.</returns>
		public static MLTEntryType GetMLTEntryTypeFromFilename(string filename)
        {
            switch (Path.GetExtension(filename).ToLowerInvariant())
            {
                case ".mpb":
                    return MLTEntryType.MPB;
                case ".mdb":
                    return MLTEntryType.MDB;
                case ".msb":
                    return MLTEntryType.MSB;
                case ".osb":
                    return MLTEntryType.OSB;
                case ".fpb":
                    return MLTEntryType.FPB;
                case ".fob":
                    return MLTEntryType.FOB;
                case ".fpw":
                    return MLTEntryType.FPW;
                case ".psr":
                    return MLTEntryType.PSR;
                case ".mlt":
                default:
                    return MLTEntryType.MLT;
            }
        }

		public override GenericArchiveEntry NewEntry()
		{
			return new MLTEntry();
		}
	}
}