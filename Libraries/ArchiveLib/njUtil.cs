using SAModel;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ArchiveLib
{
	/// <summary>Archives created by the njUtil tool from KATANA SDK (incomplete implementation).</summary>
	public class NjArchive : GenericArchive
    {
        public class NjArchiveEntry : GenericArchiveEntry
        {
            public NjArchiveEntry(int index, byte[] data)
            {
                Data = data;
				string ext = ".bin";
				if (data.Length >= 4)
				{
					string magic = System.Text.Encoding.ASCII.GetString(data, 0, 4);
					switch (magic)
					{
						case "NJIN":
							ext = ".nji";
							break;
						case "NJLI":
							ext = ".njl";
							break;
						case "NLIM":
							ext = ".njlm";
							break;
						case "NSSM":
							ext = ".njsm";
							break;
						case "NCAM":
							ext = ".ncm";
							break;
						case "NJBM":
						case "NJCM":
						case "NJTL":
							ext = ".nj";
							break;
						case "GJBM":
						case "GJCM":
						case "GJTL":
							ext = ".gj";
							break;
						case "NMDM":
							ext = ".njm";
							break;
						case "PVMH":
							ext = ".pvm";
							break;
						case "GVMH":
							ext = ".gvm";
							break;
						case "XVMH":
							ext = ".xvm";
							break;
						case "PVRT":
							ext = ".pvr";
							break;
						case "GVRT":
							ext = ".gvr";
							break;
						case "XVRT":
							ext = ".xvr";
							break;
					}
				}
				Name = index.ToString("D3") + ext;
			}

			public NjArchiveEntry() { }
		}

        public NjArchive(byte[] file, int off = 0)
        {
			if (off != 0)
			{
				byte[] data = new byte[file.Length - off];
				Array.Copy(file, off, data, 0, data.Length);
				file = data;
			}
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
                Entries.Add(new NjArchiveEntry(i, data));
            }
            ByteConverter.BigEndian = bigendbk;
        }

        public override byte[] GetBytes()
        {
            throw new NotImplementedException();
        }

		public override GenericArchiveEntry NewEntry()
		{
			return new NjArchiveEntry();
		}
	}
}