using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using TextureLib;

namespace ArchiveLib
{
	/// <summary>Custom texture pack format developed by SonicFreak94 for SADX Mod Loader.</summary>
	public class PVMXFile : GenericArchive
    {
        const int FourCC = 0x584D5650; // 'PVMX'
        const byte Version = 1;

        public override void CreateIndexFile(string path)
        {
            using (TextWriter texList = File.CreateText(Path.Combine(path, "index.txt")))
            {
                for (int u = 0; u < Entries.Count; u++)
                {
                    string entry;
                    PVMXEntry pvmxentry = (PVMXEntry)Entries[u];
                    string dimensions = string.Join("x", pvmxentry.Width.ToString(), pvmxentry.Height.ToString());
                    if (pvmxentry.HasDimensions())
                        entry = string.Join(",", pvmxentry.GBIX.ToString(), pvmxentry.Name, dimensions);
                    else
                        entry = string.Join(",", pvmxentry.GBIX.ToString(), pvmxentry.Name);
                    texList.WriteLine(entry);
                }
                texList.Flush();
                texList.Close();
            }
        }

		/// <summary>Returns true if the specified byte array is a PVMX archive.</summary>
		/// <param name="data">Byte array to analyze.</param>
		/// <returns>True if the data is a PVMX archive.</returns>
        public static bool Identify(byte[] data)
        {
            return data.Length > 4 && BitConverter.ToInt32(data, 0) == FourCC;
        }

        public PVMXFile(byte[] pvmxdata, int offs = 0)
        {
			if (offs != 0)
			{
				byte[] data = new byte[pvmxdata.Length - offs];
				Array.Copy(pvmxdata, offs, data, 0, data.Length);
				pvmxdata = data;
			}
			Entries = new List<GenericArchiveEntry>();
            if (!(pvmxdata.Length > 4 && BitConverter.ToInt32(pvmxdata, 0) == 0x584D5650))
                throw new FormatException("File is not a PVMX archive.");
            if (pvmxdata[4] != 1) throw new FormatException("Incorrect PVMX archive version.");
            int off = 5;
            PVMXDictionaryField type;
            for (type = (PVMXDictionaryField)pvmxdata[off++]; type != PVMXDictionaryField.None; type = (PVMXDictionaryField)pvmxdata[off++])
            {
                string name = "";
                uint gbix = 0;
                int width = 0;
                int height = 0;
                while (type != PVMXDictionaryField.None)
                {
                    switch (type)
                    {
                        case PVMXDictionaryField.GlobalIndex:
                            gbix = BitConverter.ToUInt32(pvmxdata, off);
                            off += sizeof(uint);
                            break;

                        case PVMXDictionaryField.Name:
                            int count = 0;
                            while (pvmxdata[off + count] != 0)
                                count++;
                            name = System.Text.Encoding.UTF8.GetString(pvmxdata, off, count);
                            off += count + 1;
                            break;

                        case PVMXDictionaryField.Dimensions:
                            width = BitConverter.ToInt32(pvmxdata, off);
                            off += sizeof(int);
                            height = BitConverter.ToInt32(pvmxdata, off);
                            off += sizeof(int);
                            break;
                    }

                    type = (PVMXDictionaryField)pvmxdata[off++];

                }
                ulong offset = BitConverter.ToUInt64(pvmxdata, off);
                off += sizeof(ulong);
                ulong length = BitConverter.ToUInt64(pvmxdata, off);
                off += sizeof(ulong);
                byte[] texdata = new byte[(int)length];
                Array.Copy(pvmxdata, (int)offset, texdata, 0, (int)length);
                //Console.WriteLine("Added entry {0} at {1} GBIX {2} width {3} height {4}", name, off, gbix, width, height);
                Entries.Add(new PVMXEntry(name, gbix, texdata, width, height));
            }
        }

		public PVMXFile(string indexFileName)
		{
			Entries = new List<GenericArchiveEntry>();
			if (!File.Exists(indexFileName))
				return;
			string folderPath = Path.GetDirectoryName(indexFileName);
			List<string> filenames = new List<string>(File.ReadAllLines(indexFileName).Where(a => !string.IsNullOrEmpty(a)));
			foreach (string line in filenames)
			{
				string[] split = line.Split(',');
				string filename = split[0];
				int width = 0;
				int height = 0;
				uint gbix = uint.Parse(split[0]);
				if (split.Length > 2)
				{
					width = int.Parse(split[2].Split('x')[0]);
					height = int.Parse(split[2].Split('x')[1]);
				}
				Entries.Add(new PVMXEntry(Path.GetFileName(filename), gbix, File.ReadAllBytes(Path.Combine(folderPath, filename)), width, height));
			}
		}

        public PVMXFile()
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
            foreach (PVMXEntry tex in Entries)
            {
                bw.Write((byte)PVMXDictionaryField.GlobalIndex);
                bw.Write(tex.GBIX);
                bw.Write((byte)PVMXDictionaryField.Name);
                bw.Write(tex.Name.ToCharArray());
                bw.Write((byte)0);
                if (tex.HasDimensions())
                {
                    bw.Write((byte)PVMXDictionaryField.Dimensions);
                    bw.Write(tex.Width);
                    bw.Write(tex.Height);
                }
                bw.Write((byte)PVMXDictionaryField.None);
                long size;
                using (MemoryStream ms = new MemoryStream(tex.Data))
                {
                    texdata.Add(new OffsetData(str.Position, ms.ToArray()));
                    size = ms.Length;
                }
                bw.Write(0ul);
                bw.Write(size);
            }
            bw.Write((byte)PVMXDictionaryField.None);
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
			return new PVMXEntry();
		}

		public class PVMXEntry : GenericArchiveEntry
		{
			public int Width { get; set; }

			public int Height { get; set; }

			public uint GBIX { get; set; }

			public PVMXEntry(string name, uint gbix, byte[] data, int width, int height)
			{
				Name = name;
				Width = width;
				Height = height;
				Data = data;
				GBIX = gbix;
			}

			public PVMXEntry() { }

			/// <summary>Checks whether the PVMX entry has texture dimension overrides.</summary>
			/// <returns>True if dimensions are present.</returns>
			public bool HasDimensions()
			{
				if (Width != 0 || Height != 0)
					return true;
				else
					return false;
			}

			public override Bitmap GetBitmap()
			{
				Bitmap bitmap;
				bitmap = new Bitmap(GenericTexture.LoadTexture(Data).Image);
				return bitmap;
			}
		}

		/// <summary>PVMX entry data.</summary>
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

		/// <summary>Type of PVMX dictionary data in PVMX entry table.</summary>
		enum PVMXDictionaryField : byte
        {
			/// <summary>Entry has no metadata.</summary>
			None,
            /// <summary>Field is a Global Index.</summary>
            GlobalIndex,
			/// <summary>Field is a null-terminated filename.</summary>
			Name,
			/// <summary>Field is two 32-bit unsigned integers defining texture dimensions.</summary>
			Dimensions,
        }
    }
}