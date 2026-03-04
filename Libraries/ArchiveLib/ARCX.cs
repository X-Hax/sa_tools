using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace ArchiveLib
{
	/// <summary>
	/// ARCX is an archive format based on SonicFreak94's PVMX that is repurposed for storing models or other generic files. Supports folders.
	/// </summary>
	public class ARCXFile : GenericArchive
    {
        const int FourCC = 0x58435241; // 'ARCX'
        const byte Version = 0;

        public override void CreateIndexFile(string path)
        {
			return;
        }

		/// <summary>Checks whether the byte array is an ARCX file.</summary>
		/// <param name="data">Byte array to analyze.</param>
		/// <returns>True if the byte array is an ARCX file.</returns>
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
            for (type = (ARCXDictionaryField)ARCXdata[off++]; type != ARCXDictionaryField.None; type = (ARCXDictionaryField)ARCXdata[off++])
            {
                string name = "";
				string folder = "";

                while (type != ARCXDictionaryField.None)
                {
                    switch (type)
                    {
                        case ARCXDictionaryField.Name:
							int count = 0;
							while (ARCXdata[off + count] != 0)
								count++;
							name = System.Text.Encoding.UTF8.GetString(ARCXdata, off, count);
							off += count + 1;
							break;

                        case ARCXDictionaryField.Folder:
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

        public override byte[] GetBytes()
        {
            MemoryStream str = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(str);
            bw.Write(FourCC);
            bw.Write(Version);
            List<OffsetData> texdata = new List<OffsetData>();
            foreach (ARCXEntry tex in Entries)
            {
                bw.Write((byte)ARCXDictionaryField.Folder);
				bw.Write(tex.Folder.ToCharArray());
				bw.Write((byte)0);
				bw.Write((byte)ARCXDictionaryField.Name);
                bw.Write(tex.Name.ToCharArray());
                bw.Write((byte)0);
                bw.Write((byte)ARCXDictionaryField.None);
                long size;
                using (MemoryStream ms = new MemoryStream(tex.Data))
                {
                    texdata.Add(new OffsetData(str.Position, ms.ToArray()));
                    size = ms.Length;
                }
                bw.Write(0ul);
                bw.Write(size);
            }
            bw.Write((byte)ARCXDictionaryField.None);
            foreach (OffsetData od in texdata)
            {
                long pos = str.Position;
                str.Position = od.Offset;
                bw.Write(pos);
                str.Position = pos;
                bw.Write(od.Data);
            }
            return str.ToArray();
        }

		public override GenericArchiveEntry NewEntry()
		{
			return new ARCXEntry();
		}

		public class ARCXEntry : GenericArchiveEntry
        {
			public string Folder;

			public ARCXEntry() { }

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

		/// <summary>ARCX entry data.</summary>
		private class OffsetData
        {
			/// <summary>Entry data.</summary>
			public byte[] Data;
			/// <summary>Location of the data.</summary>
			public long Offset;
			
            public OffsetData(long o, byte[] d)
            {
                Offset = o;
                Data = d;
            }
        }

		/// <summary>Type of ARCX dictionary data in ARCX entry table.</summary>
		enum ARCXDictionaryField : byte
        {
			/// <summary>Entry has no metadata.</summary>
			None,
            /// <summary>Field is a relative path.</summary>
            Folder,
            /// <summary>Field is a null-terminated filename.</summary>
            Name,
        }
    }
}