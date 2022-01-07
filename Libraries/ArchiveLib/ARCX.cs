using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

// ARCX is an archive format based on SonicFreak94's PVMX that is repurposed for storing models or other generic files. Supports folders.
namespace ArchiveLib
{
    public class ARCXFile : GenericArchive
    {
        const int FourCC = 0x58435241; // 'ARCX'
        const byte Version = 0;

        public override void CreateIndexFile(string path)
        {
			return;
        }

        public static bool Identify(byte[] data)
        {
            return data.Length > 4 && BitConverter.ToInt32(data, 0) == FourCC;
        }

        public ARCXFile(byte[] ARCXdata)
        {
            Entries = new List<GenericArchiveEntry>();
            if (!(ARCXdata.Length > 4 && BitConverter.ToInt32(ARCXdata, 0) == FourCC))
                throw new FormatException("File is not a ARCX archive.");
            if (ARCXdata[4] != Version) throw new FormatException("Incorrect ARCX archive version.");
            int off = 5;
            ARCXDictionaryField type;
            for (type = (ARCXDictionaryField)ARCXdata[off++]; type != ARCXDictionaryField.none; type = (ARCXDictionaryField)ARCXdata[off++])
            {
                string name = "";
				string folder = "";

                while (type != ARCXDictionaryField.none)
                {
                    switch (type)
                    {
                        case ARCXDictionaryField.name:
							int count = 0;
							while (ARCXdata[off + count] != 0)
								count++;
							name = System.Text.Encoding.UTF8.GetString(ARCXdata, off, count);
							off += count + 1;
							break;

                        case ARCXDictionaryField.folder:
                            int count2 = 0;
                            while (ARCXdata[off + count2] != 0)
                                count2++;
                            folder = System.Text.Encoding.UTF8.GetString(ARCXdata, off, count2);
                            off += count2 + 1;
                            break;
                    }

                    type = (ARCXDictionaryField)ARCXdata[off++];

                }
                ulong offset = BitConverter.ToUInt64(ARCXdata, off);
                off += sizeof(ulong);
                ulong length = BitConverter.ToUInt64(ARCXdata, off);
                off += sizeof(ulong);
                byte[] data = new byte[(int)length];
                Array.Copy(ARCXdata, (int)offset, data, 0, (int)length);
                Entries.Add(new ARCXEntry(name, folder, data));
            }
        }

        public ARCXFile()
        {
            Entries = new List<GenericArchiveEntry>();
        }

        public void AddFile(string name, string path, byte[] data)
        {
            Entries.Add(new ARCXEntry(name, path, data));
        }

        public override byte[] GetBytes()
        {
            MemoryStream str = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(str);
            bw.Write(FourCC);
            bw.Write(Version);
            List<OffData> texdata = new List<OffData>();
            foreach (ARCXEntry tex in Entries)
            {
                bw.Write((byte)ARCXDictionaryField.folder);
				bw.Write(tex.Folder.ToCharArray());
				bw.Write((byte)0);
				bw.Write((byte)ARCXDictionaryField.name);
                bw.Write(tex.Name.ToCharArray());
                bw.Write((byte)0);
                bw.Write((byte)ARCXDictionaryField.none);
                long size;
                using (MemoryStream ms = new MemoryStream(tex.Data))
                {
                    texdata.Add(new OffData(str.Position, ms.ToArray()));
                    size = ms.Length;
                }
                bw.Write(0ul);
                bw.Write(size);
            }
            bw.Write((byte)ARCXDictionaryField.none);
            foreach (OffData od in texdata)
            {
                long pos = str.Position;
                str.Position = od.off;
                bw.Write(pos);
                str.Position = pos;
                bw.Write(od.data);
            }
            return str.ToArray();
        }

        public class ARCXEntry : GenericArchiveEntry
        {
			public string Folder;

            public ARCXEntry(string name, string folder, byte[] data)
            {
                Name = name;
				Folder = folder;
				Data = data;
            }

            public override Bitmap GetBitmap()
            {
				throw new NotImplementedException();
			}
        }

        struct OffData
        {
            public long off;
            public byte[] data;

            public OffData(long o, byte[] d)
            {
                off = o;
                data = d;
            }
        }

        enum ARCXDictionaryField : byte
        {
            none,
            /// <summary>
            /// Relative path
            /// </summary>
            folder,
            /// <summary>
            /// Null-terminated file name
            /// </summary>
            name,
        }
    }
}