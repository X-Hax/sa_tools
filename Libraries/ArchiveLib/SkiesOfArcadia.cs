using SonicRetro.SAModel;
using System;
using System.Drawing;
using System.IO;
using System.Text;

// Skies of Arcadia (Dreamcast) MLD archives.
namespace ArchiveLib
{
    public class NMLDEntryInfo
    {
        public uint ObjectPointer;
        public uint MotionPointer;
        public uint TexlistPointer;
        public uint Unknown;
        byte[] Data;

        public NMLDEntryInfo(byte[] data, int start)
        {
            Data = data;
            ObjectPointer = ByteConverter.ToUInt32(Data, start) + (uint)start;
            MotionPointer = ByteConverter.ToUInt32(Data, start + 4) + (uint)start;
            TexlistPointer = ByteConverter.ToUInt32(Data, start + 8) + (uint)start;
            Unknown = ByteConverter.ToUInt32(Data, start + 12);
            Console.WriteLine("NMLD Entry Info at {0}: NJCM at {1}, NMDM at {2}, NJTL at {3}, Unknown: {4}", start.ToString("X"), ObjectPointer.ToString("X"), MotionPointer.ToString("X"), TexlistPointer.ToString("X"), Unknown.ToString("X"));
      
        }

        public byte[] GetModel()
        {
            byte[] njcm = new byte[MotionPointer - ObjectPointer];
            Array.Copy(Data, ObjectPointer, njcm, 0, njcm.Length);
            return njcm;
        }
    }

    public class NMLDEntry
    {
        public NMLDEntryInfo Info;

        public NMLDEntry(byte[] data, int start, int index)
        {
            int Count = ByteConverter.ToInt32(data, start);
            uint PointerInfo = ByteConverter.ToUInt32(data, start + 4);
            Console.WriteLine("NMLD Entry at {0}, Count {1}, Data at {2}", start.ToString("X"), Count, PointerInfo.ToString("X"));
            if (PointerInfo != 0)
            {
                Info = new NMLDEntryInfo(data, (int)PointerInfo);
            }
        }
    }

    public class NMLDObject
    {
        public NMLDEntry Entry;
        public int EntryID; // 0x00
        public int Rotation; // 0x04
        public uint Pointer1; // 0x08
        public uint Pointer2; // 0x0C
        public uint Pointer3; // 0x10
        public uint PointerEntry; // 0x14
        public uint PointerGRNDChunk; // 0x18
        public uint PointerMotion; // 0x1C
        public uint PointerTexlist; // 0x20
        public string Name; // 0x24 (32 bytes)
        public float Float1; // 0x44
        public float Float2; // 0x48
        public float Float3; // 0x4C
        public int Int1; // 0x50
        public int Int2; // 0x54
        public int Int3; // 0x58
        public float ScaleX; // 0x5C
        public float ScaleY; // 0x60
        public float ScaleZ; // 0x64

        public NMLDObject(byte[] data, int start)
        {
            EntryID = ByteConverter.ToInt32(data, start);
            Rotation = ByteConverter.ToInt32(data, start + 0x04);
            Pointer1 = ByteConverter.ToUInt32(data, start + 0x08);
            Pointer2 = ByteConverter.ToUInt32(data, start + 0x0C);
            Pointer3 = ByteConverter.ToUInt32(data, start + 0x10);
            PointerEntry = ByteConverter.ToUInt32(data, start + 0x14);
            PointerGRNDChunk = ByteConverter.ToUInt32(data, start + 0x18);
            PointerMotion = ByteConverter.ToUInt32(data, start + 0x1C);
            PointerTexlist = ByteConverter.ToUInt32(data, start + 0x20);
            int namesize = 0;
            for (int s = 0; s < 32; s++)
            {
                if (data[start + 0x24 + s] != 0)
                    namesize++;
                else
                    break;
            }
            byte[] namechunk = new byte[namesize];
            Array.Copy(data, start + 0x24, namechunk, 0, namesize);
            Name = System.Text.Encoding.ASCII.GetString(namechunk);
            Float1 = ByteConverter.ToSingle(data, start + 0x44);
            Float2 = ByteConverter.ToSingle(data, start + 0x48);
            Float3 = ByteConverter.ToSingle(data, start + 0x4C);
            Int1 = ByteConverter.ToInt32(data, start + 0x50);
            Int2 = ByteConverter.ToInt32(data, start + 0x54);
            Int3 = ByteConverter.ToInt32(data, start + 0x58);
            ScaleX = ByteConverter.ToSingle(data, start + 0x5C);
            ScaleY = ByteConverter.ToSingle(data, start + 0x60);
            ScaleZ = ByteConverter.ToSingle(data, start + 0x64);
            Console.WriteLine("NMLD at {0}, ID: {1}, Name: {2}, Entry at: {3}", start.ToString("X"),EntryID.ToString(), Name, PointerEntry.ToString("X"));
            Entry = new NMLDEntry(data, (int)PointerEntry, EntryID);
        }
    }

    public class MLDArchive : GenericArchive
    {
        public override void CreateIndexFile(string path)
        {
            return;
        }

        public class MLDArchiveEntry : GenericArchiveEntry
        {

            public override Bitmap GetBitmap()
            {
                throw new NotImplementedException();
            }

            public MLDArchiveEntry(byte[] data, string name)
            {
                Name = name;
                Data = data;
            }
        }

        public MLDArchive(byte[] file)
        {
            int count = BitConverter.ToInt32(file, 0);
            uint nmlddatapointer = BitConverter.ToUInt32(file, 0x04);
            uint flags = BitConverter.ToUInt32(file, 0x08);
            uint realdatapointer = BitConverter.ToUInt32(file, 0x0C);
            uint textablepointer = BitConverter.ToUInt32(file, 0x10);
            Console.WriteLine("Number of NMLD entries: {0}, NMLD data starts at {1}, real data starts at {2}", count, nmlddatapointer.ToString("X"), realdatapointer.ToString("X"));
            uint sizereal = textablepointer - realdatapointer;
            uint sizenmld = realdatapointer - nmlddatapointer;
            Console.WriteLine("First entry: {0} size {1}", realdatapointer.ToString("X"), sizereal);
            // Extract NMLD stuff
            for (int m = 0; m < count; m++)
            {
                NMLDObject nmld = new NMLDObject(file, (int)nmlddatapointer + 104 * m);
                if (nmld.Entry.Info != null)
                    Entries.Add(new MLDArchiveEntry(nmld.Entry.Info.GetModel(), Path.ChangeExtension(m.ToString("D3") + "_" + nmld.Name, ".nj")));
            }
            int numtex = BitConverter.ToInt32(file, (int)textablepointer);
            // Extract textures
            Console.WriteLine("Number of textures: {0}, pointer: {1}", numtex, textablepointer.ToString("X"));
            if (numtex > 0)
            {
                int texdataoffset = (int)textablepointer + 4 + numtex * 44;
                Console.WriteLine("Texture offset original: {0}", texdataoffset.ToString("X"));
                // Get through the padding
                if (file[texdataoffset] == 0)
                {
                    do
                    {
                        texdataoffset += 1;
                    }
                    while (file[texdataoffset] == 0);
                }
                Console.WriteLine("Textures from {0}", texdataoffset.ToString("X"));
                int currenttextureoffset = texdataoffset;
                for (int i = 0; i < numtex; i++)
                {
                    byte[] namestring = new byte[36];
                    Array.Copy(file, textablepointer + 4 + i * 44, namestring, 0, 36);
                    string entryfn = Encoding.ASCII.GetString(namestring).TrimEnd((char)0);
                    int size = BitConverter.ToInt32(file, (int)textablepointer + 4 + i * 44 + 40);
                    Console.WriteLine("Entry {0} name {1} size {2}", i, entryfn, size);
                    byte[] texture = new byte[size];
                    Array.Copy(file, currenttextureoffset, texture, 0, size);
                    Entries.Add(new MLDArchiveEntry(texture, entryfn + ".pvr"));
                    currenttextureoffset += size;
                }
            }
        }

        public override byte[] GetBytes()
        {
            throw new NotImplementedException();
        }
    }
}
