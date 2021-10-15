using SAModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

// Dreamcast Multi-Unit (MLT) container format used for soundbanks in SA1 and SA2.
// This class is for the base archive only, not the invidivual banks (MPB, MSB etc.) contained within.
namespace ArchiveLib
{
    #region MLT
    public class MLTFile : GenericArchive
    {
        public byte Version; // Set to 1 in SA1, 2 in SA2
        public byte Revision;  // Set to 1 in SA1, 0 in SA2

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
                Name = filename == "" ? "BANK" : filename + "_BANK" + BankID.ToString("D2") + GetMLTItemExtension(file, offset);
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

            public MLTEntry(string filename)
            { }

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

        public enum MLTEntryType : uint
        {
            MLT = 0x544C4D53, // Multi-Unit
            MSB = 0x42534D53, // MIDI Sequence Bank
            MPB = 0x42504D53, // MIDI Program Bank
            MDB = 0x42444D53, // MIDI Drum Bank
            OSB = 0x42534F53, // One-Shot Bank
            FPB = 0x42504653, // FX Program Bank
            FOB = 0x424F4653, // FX Output Bank
            FPW = 0x57504653, // FX Program Work
            PSR = 0x52535053  // PCM Stream Ring Buffer (WTF is that anyway?)
        }

        // Related methods
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
    }
    #endregion
}