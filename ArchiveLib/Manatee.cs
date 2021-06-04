using SonicRetro.SAModel;
using System;
using System.Collections.Generic;
using System.Drawing;

// Manatee-related formats (MLT, MPB etc. soundbanks used by some KATANA games)
// WIP

namespace ArchiveLib
{
    #region MLT
    public class MLTArchive : GenericArchive
    {
        public override void CreateIndexFile(string path)
        {
            return;
        }

        public class MLTArchiveEntry : GenericArchiveEntry
        {
            public MLTEntryType Type;
            public int BankID;
            public uint StartOffset;
            
            public override Bitmap GetBitmap()
            {
                throw new NotImplementedException();
            }

            public MLTArchiveEntry(byte[] data, string name)
            {
                Name = name;
                Data = data;
            }

            public MLTEntryType GetEntryType()
            {
                return (MLTEntryType)BitConverter.ToUInt32(Data, 0);
            }
        }

        public enum MLTEntryType: uint
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

        public string GetMLTItemExtension(byte[] file)
        {
            switch ((MLTEntryType)BitConverter.ToUInt32(file, 0))
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

        public MLTArchive(byte[] file)
        {
            List<string> bankfiles = new List<string>();
            int numfiles = BitConverter.ToInt32(file, 8) + 1;
            int mltsize = numfiles * 0x20;
            byte[] mlt = new byte[mltsize];
            Array.Copy(file, mlt, mltsize);
            Entries.Add(new MLTArchiveEntry(mlt, "filemap.mlt"));
            for (int u = 0; u < numfiles; u++)
            {
                int tempoff = 0x20 * u;
                byte bank = file[tempoff + 4];
                int pointer = BitConverter.ToInt32(file, tempoff + 0x10);
                if (pointer + 8 > file.Length)
                {
                    Console.WriteLine("Invalid pointer at {0} for header {1}", tempoff.ToString("X"), u);
                    continue;
                }
                int size = BitConverter.ToInt32(file, pointer + 8);
                //Console.WriteLine("Header {0} ({1}) at {2} (MLT {3}), size: {4}, bank {5}", hdr, u, pointer.ToString("X"), tempoff.ToString("X"), size, bank);
                if (size > 0 && pointer != -1)
                {
                    byte[] arr = new byte[size];
                    Array.Copy(file, pointer, arr, 0, size);
                    Entries.Add(new MLTArchiveEntry(arr, "BANK" + bank.ToString("D3") + GetMLTItemExtension(arr)));
                }
            }
        }

        public override byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            /*
            // Filemap header
            result.AddRange(BitConverter.GetBytes((uint)MLTEntryType.MLT));
            result.Add(1); // Version
            result.Add(1); // Whatever
            result.AddRange(BitConverter.GetBytes((ushort)0)); // Whatever
            result.AddRange(BitConverter.GetBytes(Entries.Count));
            result.AddRange(BitConverter.GetBytes(0xFFFFFFFF));
            result.AddRange(BitConverter.GetBytes(0xFFFFFFFF));
            result.AddRange(BitConverter.GetBytes(0xFFFFFFFF));
            result.AddRange(BitConverter.GetBytes(0xFFFFFFFF));
            result.AddRange(BitConverter.GetBytes(0xFFFFFFFF));
            // Headers
            int PreviousDataSize = 0;
            foreach (MLTArchiveEntry mltentry in Entries)
            {
                result.AddRange(BitConverter.GetBytes((uint)mltentry.GetEntryType()));
                result.AddRange(BitConverter.GetBytes(mltentry.BankID));
                result.AddRange(BitConverter.GetBytes(mltentry.StartOffset));
                result.AddRange(BitConverter.GetBytes(mltentry.Data.Length));
                int FileOffset = (Entries.Count + 1) * 32 + PreviousDataSize;
                PreviousDataSize += mltentry.Data.Length;
                result.AddRange(BitConverter.GetBytes(FileOffset));
            }
            */
            return result.ToArray();
        }
    }
    #endregion
}
