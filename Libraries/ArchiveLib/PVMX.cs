using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

// Custom texture pack format developed by SonicFreak94 for SADX Mod Loader.
namespace ArchiveLib
{
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

		public static bool Identify(byte[] data)
		{
			return data.Length > 4 && BitConverter.ToInt32(data, 0) == FourCC;
		}

		public PVMXFile(byte[] pvmxdata)
		{
			Entries = new List<GenericArchiveEntry>();
			if (!(pvmxdata.Length > 4 && BitConverter.ToInt32(pvmxdata, 0) == 0x584D5650))
				throw new FormatException("File is not a PVMX archive.");
			if (pvmxdata[4] != 1) throw new FormatException("Incorrect PVMX archive version.");
			int off = 5;
			PVMXDictionaryField type;
			for (type = (PVMXDictionaryField)pvmxdata[off++]; type != PVMXDictionaryField.none; type = (PVMXDictionaryField)pvmxdata[off++])
			{
				string name = "";
				uint gbix = 0;
				int width = 0;
				int height = 0;
				while (type != PVMXDictionaryField.none)
				{
					switch (type)
					{
						case PVMXDictionaryField.global_index:
							gbix = BitConverter.ToUInt32(pvmxdata, off);
							off += sizeof(uint);
							break;

						case PVMXDictionaryField.name:
							int count = 0;
							while (pvmxdata[off + count] != 0)
								count++;
							name = System.Text.Encoding.UTF8.GetString(pvmxdata, off, count);
							off += count + 1;
							break;

						case PVMXDictionaryField.dimensions:
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

		public PVMXFile()
		{
			Entries = new List<GenericArchiveEntry>();
		}

		public void AddFile(string name, uint gbix, byte[] data, int width = 0, int height = 0)
		{
			Entries.Add(new PVMXEntry(name, gbix, data, width, height));
		}

		public override byte[] GetBytes()
		{
			MemoryStream str = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(str);
			bw.Write(FourCC);
			bw.Write(Version);
			List<OffData> texdata = new List<OffData>();
			foreach (PVMXEntry tex in Entries)
			{
				bw.Write((byte)PVMXDictionaryField.global_index);
				bw.Write(tex.GBIX);
				bw.Write((byte)PVMXDictionaryField.name);
				bw.Write(tex.Name.ToCharArray());
				bw.Write((byte)0);
				if (tex.HasDimensions())
				{
					bw.Write((byte)PVMXDictionaryField.dimensions);
					bw.Write(tex.Width);
					bw.Write(tex.Height);
				}
				bw.Write((byte)PVMXDictionaryField.none);
				long size;
				using (MemoryStream ms = new MemoryStream(tex.Data))
				{
					texdata.Add(new OffData(str.Position, ms.ToArray()));
					size = ms.Length;
				}
				bw.Write(0ul);
				bw.Write(size);
			}
			bw.Write((byte)PVMXDictionaryField.none);
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

			public bool HasDimensions()
			{
				if (Width != 0 || Height != 0)
					return true;
				else
					return false;
			}

			public override Bitmap GetBitmap()
			{
				MemoryStream ms = new MemoryStream(Data);
				return new Bitmap(ms);
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

		enum PVMXDictionaryField : byte
		{
			none,
			/// <summary>
			/// 32-bit integer global index
			/// </summary>
			global_index,
			/// <summary>
			/// Null-terminated file name
			/// </summary>
			name,
			/// <summary>
			/// Two 32-bit integers defining width and height
			/// </summary>
			dimensions,
		}
	}
}