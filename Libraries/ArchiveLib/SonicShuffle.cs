using SAModel;
using System;
using System.Collections.Generic;
using System.Drawing;

// Various archive formats from Sonic Shuffle.
namespace ArchiveLib
{
    #region Sonic Shuffle MDL
    public class MDLArchive : GenericArchive
    {
        public override void CreateIndexFile(string path)
        {
            return;
        }

        public class MDLArchiveEntry : GenericArchiveEntry
        {
            public MDLEntryType Type;

            public override Bitmap GetBitmap()
            {
                throw new NotImplementedException();
            }

            public MDLArchiveEntry(byte[] data, string name)
            {
                Name = name;
                Data = data;
            }
        }

        public enum MDLEntryType : uint
        {
            PVM = 1,
            ChunkModel = 2,
            Motion = 4,
            ShapeMotion = 8,
            SomeWeirdShit = 10,
        }

        public MDLArchive(byte[] file)
        {
            bool bigendbk = ByteConverter.BigEndian;
            if (file[0] == 0)
                ByteConverter.BigEndian = true;
            int count = ByteConverter.ToUInt16(file, 2);
            Entries = new List<GenericArchiveEntry>(count);
            for (int i = 0; i < count; i++)
            {
                MDLEntryType type = (MDLEntryType)BitConverter.ToUInt32(file, 8 + i * 12);
                int size = BitConverter.ToInt32(file, 12 + i * 12);
                int offset = BitConverter.ToInt32(file, 16 + i * 12);
                Console.WriteLine("Entry {0} type {1} at offset {2}: size {3}", i, type.ToString(), offset, size);
                byte[] entrydata = new byte[size];
                Array.Copy(file, offset, entrydata, 0, size);
                string extension;
                switch (type)
                {
                    case MDLEntryType.PVM:
                        extension = ".pvm";
                        break;
                    case MDLEntryType.ShapeMotion:
                    case MDLEntryType.Motion:
                        extension = ".njm";
                        break;
                    case MDLEntryType.ChunkModel:
                        extension = ".nj";
                        break;
                    case MDLEntryType.SomeWeirdShit:
                    default:
                        extension = ".bin";
                        break;
                }
                Entries.Add(new MDLArchiveEntry(entrydata, i.ToString("D3") + extension));
            }
            ByteConverter.BigEndian = bigendbk;
        }

        public override byte[] GetBytes()
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region Sonic Shuffle MDT
    public class MDTArchive : GenericArchive
    {
        const uint Magic_SMPB = 0x42504D53; // SMPB
        const uint Magic_SMSB = 0x42534D53; // SMPB
        const uint Magic_SFPB = 0x42504653; // SFPB
        const uint Magic_SDRV = 0x56524453; // SDRV
        const uint Magic_SFOB = 0x424F4653; // SFOB
        const uint Magic_SMLT = 0x544C4D53; // SMLT
        const uint Magic_SOSB = 0x42534F53; // SOSB
        const ulong Magic_CRI = 0x4952432963280000; // 0000(c)CRI at 0x20

        public override void CreateIndexFile(string path)
        {
            return;
        }

        public class MDTArchiveEntry : GenericArchiveEntry
        {

            public override Bitmap GetBitmap()
            {
                throw new NotImplementedException();
            }

            public MDTArchiveEntry(byte[] data, string name)
            {
                Name = name;
                Data = data;
            }
        }

        public string GetEntryExtension(byte[] data)
        {
            if (data.Length < 4)
                return ".bin";
            switch (BitConverter.ToUInt32(data, 0))
            {
                case Magic_SMLT:
                    return ".mlt";
                case Magic_SMPB:
                    return ".mpb";
                case Magic_SMSB:
                    return ".msb";
                case Magic_SFPB:
                    return ".fpb";
                case Magic_SFOB:
                    return ".fob";
                case Magic_SDRV:
                    return ".drv";
                case Magic_SOSB:
                    return ".osb";
                default:
                    if (data.Length < 40)
                        return ".bin";
                    if (BitConverter.ToUInt64(data, 0x20) == Magic_CRI)
                        return ".adx";
                    else return ".bin";
            }
        }

        public enum MDTArchiveType
        {
            // There are 3 types of MDT files:
            // Type 0 stores chunk lengths 4 bytes before the data begins.
            // Type 1 doesn't store chunk lengths.
            // Type 2 is like Type 2 but the offsets are Big Endian.
            Manatee = 0,
            CRI = 1,
            CRIBigEndian = 2
        }

        public MDTArchiveType Identify(byte[] file)
        {
            if (file[0] == 0)
                return MDTArchiveType.CRIBigEndian;
            // Check if there is ADX
            for (int i = 0; i < file.Length; i += 8)
            {
                if (BitConverter.ToUInt64(file, i) == Magic_CRI)
                    return MDTArchiveType.CRI;
            }
            return MDTArchiveType.Manatee;
        }

        public MDTArchive(byte[] file)
        {
            bool bigendbk = ByteConverter.BigEndian;
            MDTArchiveType type = Identify(file);
            if (type == MDTArchiveType.CRIBigEndian)
                ByteConverter.BigEndian = true;
            int firstoffset = ByteConverter.ToInt32(file, 0);
            int count = firstoffset / 4;
            Console.WriteLine("Number of entries: {0}", count);
            Entries = new List<GenericArchiveEntry>(count);
            List<int> offsets = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                int offset = ByteConverter.ToInt32(file, i * 4);
                Console.WriteLine("Entry {0} at header offset {1}", i, offset.ToString("X"));
                offsets.Add(offset);
            }
            for (int u = 0; u < count; u++)
            {
                //Type 0 archives store chunk length in the 4 bytes before the data begins
                int type1_offset = type == MDTArchiveType.Manatee ? 4 : 0;
                int end = file.Length - 1;
                if (u < count - 1)
                    end = offsets[u + 1];
                int size = end - offsets[u];
                // Get size for Type 0 archives
                if (type == MDTArchiveType.Manatee)
                    size = BitConverter.ToInt32(file, offsets[u]);
                byte[] entrydata = new byte[size];
                Array.Copy(file, offsets[u] + type1_offset, entrydata, 0, size);
                string extension = GetEntryExtension(entrydata);
                Entries.Add(new MDTArchiveEntry(entrydata, u.ToString("D3") + extension));
            }
            ByteConverter.BigEndian = bigendbk;
        }

        public override byte[] GetBytes()
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
