using SAModel;
using System;
using System.Collections.Generic;
using System.Drawing;

// Archives created by the njUtil tool from KATANA SDK.
namespace ArchiveLib
{
    public class NjArchive : GenericArchive
    {
        public override void CreateIndexFile(string path)
        {
            throw new NotImplementedException();
        }

        public class NjArchiveEntry : GenericArchiveEntry
        {

            public override Bitmap GetBitmap()
            {
                throw new NotImplementedException();
            }

            public NjArchiveEntry(byte[] data)
            {
                Data = data;
            }
        }

        public NjArchive(byte[] file)
        {
            bool bigendbk = ByteConverter.BigEndian;
            if (file[0] == 0)
                ByteConverter.BigEndian = true;
            Entries = new List<GenericArchiveEntry>();
            int count = ByteConverter.ToInt32(file, 0) - 1;
            List<int> sizehdrs = new List<int>();
            for (int i = 0; i < count; i++)
            {
                int sizeaddr = 4 + i * 4;
                int size = ByteConverter.ToInt32(file, sizeaddr);
                //Console.WriteLine("Entry size data {0} at offset {1}: size {2}", i, sizeaddr, size);
                sizehdrs.Add(size);
            }
            int[] sizes = sizehdrs.ToArray();
            int offset = 0x20;
            for (int i = 0; i < sizes.Length; i++)
            {
                if (i != 0)
                    offset += sizes[i - 1];
                byte[] data = new byte[sizes[i]];
                Array.Copy(file, offset, data, 0, sizes[i]);
                Entries.Add(new NjArchiveEntry(data));
            }
            ByteConverter.BigEndian = bigendbk;
        }

        public override byte[] GetBytes()
        {
            throw new NotImplementedException();
        }
    }
}