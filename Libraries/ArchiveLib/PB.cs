using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using TextureLib;
using static ArchiveLib.GenericArchive;

namespace ArchiveLib
{
	/// <summary>Headerless PVMs used in Sonic Adventure (Dreamcast) and Rez (Dreamcast).</summary>
	public class PBFile : GenericArchive
    {
		/// <summary>Checks whether the specified byte array is a PB archive.</summary>
		/// <param name="data">Byte array to analyze.</param>
		/// <returns>True if the byte array is a PB archive.</returns>
		public static bool Identify(byte[] data)
		{
			if (data == null || data.Length < 4)
				return false;
			return (BitConverter.ToUInt32(data, 0) == 0x02425650);
		}

		public override void CreateIndexFile(string path)
        {
            using (TextWriter texList = File.CreateText(Path.Combine(path, "index.txt")))
            {
                for (int u = 0; u < Entries.Count; u++)
                {
                    texList.WriteLine(u.ToString("D3") + ".pvr");
                }
                texList.Flush();
                texList.Close();
            }
        }

		public PBFile(byte[] pbdata, int offs = 0)
		{
			if (offs != 0)
			{
				byte[] data = new byte[pbdata.Length - offs];
				Array.Copy(pbdata, offs, data, 0, data.Length);
				pbdata = data;
			}
			Entries = new List<GenericArchiveEntry>();
			int numtextures = pbdata[4];
			for (int u = 0; u < numtextures; u++)
			{
				Entries.Add(new PBEntry(pbdata, 8 + 16 * u, u.ToString("D3") + ".pvr"));
				//Console.WriteLine("Added header {0}: offset {1}, pixel format {2}, data format {3}, GBIX {4}, width {5}, height {6}", u, hdr.Offset, hdr.PixelFormat, hdr.DataFormat, hdr.GBIX, hdr.Width, hdr.Height);
			}
			for (int u = 0; u < numtextures; u++)
			{
				PBEntry pbentry = (PBEntry)Entries[u];
				int chunksize;
				if (u == numtextures - 1) chunksize = pbdata.Length - pbentry.Offset;
				else
				{
					PBEntry pbentry_1 = (PBEntry)Entries[u + 1];
					chunksize = pbentry_1.Offset - pbentry.Offset;
				}
				byte[] headerless = new byte[chunksize];
				Array.Copy(pbdata, pbentry.Offset, headerless, 0, chunksize);
				pbentry.Data = pbentry.GetPVR(headerless);
				//Console.WriteLine("Added data: offset {0}, length {1}", headers[u].Offset, pbchunk.Length);
			}
		}

        public PBFile(int count)
        {
            Entries = new List<GenericArchiveEntry>(count);
        }

        public PBFile() { }

        public int GetCurrentOffset(int index, int total)
        {
            int offset_base = 8 + 16 * total;
            if (index == 0)
                return offset_base;
            for (int u = 0; u < index; u++)
            {
                PBEntry entry = (PBEntry)Entries[u];
                offset_base += entry.GetHeaderless().Length;
            }
            return offset_base;
        }

        public override byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            result.Add(0x50); // P
            result.Add(0x56); // B
            result.Add(0x42); // V
            result.Add(0x02); // Version ID
            result.AddRange(BitConverter.GetBytes((uint)Entries.Count));
            for (int u = 0; u < Entries.Count; u++)
            {
                PBEntry entry = (PBEntry)Entries[u];
                result.AddRange(entry.GetHeader());
            }
            for (int u = 0; u < Entries.Count; u++)
            {
                PBEntry entry = (PBEntry)Entries[u];
                result.AddRange(entry.GetHeaderless());
            }
            return result.ToArray();
        }

		/// <summary>Creates a PVM archive from the PB archive.</summary>
		/// <returns>PVM archive.</returns>
		public PuyoFile GetPVM()
		{
			PuyoFile pvm = new PuyoFile(PuyoArchiveType.PVMFile);
			pvm.HasNameData = false;
			foreach (PBEntry entry in Entries)
			{
				pvm.Entries.Add(new PVMEntry(entry.Data, entry.Name));
			}
			return pvm;
		}

		public override GenericArchiveEntry NewEntry()
		{
			return new PBEntry();
		}
	}

    public class PBEntry : GenericArchiveEntry
    {
		/// <summary>PVR texture's headerless data offset.</summary>
        public int Offset { get; set; }
		/// <summary>PVR texture's pixel format.</summary>
		public PvrPixelFormat PixelFormat { get; set; }
		/// <summary>PVR texture's data format.</summary>
		public PvrDataFormat DataFormat { get; set; }
		/// <summary>PVR texture's global index.</summary>
		public uint GBIX { get; set; }
		/// <summary>PVR texture's width.</summary>
		public ushort Width { get; set; }
		/// <summary>PVR texture's height.</summary>
		public ushort Height { get; set; }

		/// <summary>Gets the full PVRT header of the specified PB entry.</summary>
		/// <returns>PVRT header as a byte array.</returns>
        public byte[] GetHeader()
        {
            List<byte> result = new List<byte>();
            result.AddRange(BitConverter.GetBytes(Offset));
            result.Add((byte)PixelFormat);
            result.Add((byte)DataFormat);
            result.Add(0);
            result.Add(0);
            result.AddRange(BitConverter.GetBytes(GBIX));
            result.AddRange(BitConverter.GetBytes(Width));
            result.AddRange(BitConverter.GetBytes(Height));
            return result.ToArray();
        }

		/// <summary>Gets the data of the specified PB entry.</summary>
		/// <returns>Headerless texture as a byte array.</returns>
		public byte[] GetHeaderless()
        {
            int length = BitConverter.ToInt32(Data, 20) - 8;
            byte[] result = new byte[length];
            Array.Copy(Data, 32, result, 0, length);
            return result;
        }

        public PBEntry(byte[] pbdata, int tempaddr, string name)
        {
            Name = name;
            Offset = BitConverter.ToInt32(pbdata, tempaddr);
            PixelFormat = (PvrPixelFormat)pbdata[tempaddr + 4];
            DataFormat = (PvrDataFormat)pbdata[tempaddr + 5];
            GBIX = BitConverter.ToUInt32(pbdata, tempaddr + 8);
            Width = BitConverter.ToUInt16(pbdata, tempaddr + 12);
            Height = BitConverter.ToUInt16(pbdata, tempaddr + 14);
        }

		public PBEntry(byte[] pvrdata, int offset)
		{
			Data = pvrdata;
			PvrTexture pvrt = new PvrTexture(Data);
			Offset = offset;
			PixelFormat = pvrt.PvrPixelFormat;
			DataFormat = pvrt.PvrDataFormat;
			GBIX = pvrt.Gbix;
			Width = (ushort)pvrt.Width;
			Height = (ushort)pvrt.Height;
		}

		public PBEntry(string filename, int offset)
        {
            Data = File.ReadAllBytes(filename);
            PvrTexture pvrt = new PvrTexture(Data);
            Name = Path.GetFileNameWithoutExtension(filename);
            Offset = offset;
            PixelFormat = pvrt.PvrPixelFormat;
            DataFormat = pvrt.PvrDataFormat;
            GBIX = pvrt.Gbix;
            Width = (ushort)pvrt.Width;
            Height = (ushort)pvrt.Height;
        }

        public PBEntry(int offset, PvrPixelFormat pxformat, PvrDataFormat dataformat, uint gbix, ushort width, ushort height)
        {
            Offset = offset;
            PixelFormat = pxformat;
            DataFormat = dataformat;
            GBIX = gbix;
            Width = width;
            Height = height;
        }

		public PBEntry() { }

		public override Bitmap GetBitmap()
        {
            return new PvrTexture(Data).Image;
        }

		/// <summary>Converts a PB entry to a texture with a complete PVRT header.</summary>
		/// <param name="data">Byte array of the PB entry.</param>
		/// <returns>PVR texture as a byte array.</returns>
        public byte[] GetPVR(byte[] data)
        {
            List<byte> result = new List<byte>();
            int chunksize_file = data.Length;
			byte[] gbixheader = { 0x47, 0x42, 0x49, 0x58 };
            byte[] pvrtheader = { 0x50, 0x56, 0x52, 0x54 };
            byte[] padding = { 0x20, 0x20, 0x20, 0x20 };
            result.AddRange(gbixheader);
            result.AddRange(BitConverter.GetBytes(8));
            result.AddRange(BitConverter.GetBytes(GBIX));
            result.AddRange(padding);
            result.AddRange(pvrtheader);
            result.AddRange(BitConverter.GetBytes(chunksize_file + 8));
            result.Add((byte)PixelFormat);
            result.Add((byte)DataFormat);
            result.Add(0);
            result.Add(0);
            result.AddRange(BitConverter.GetBytes(Width));
            result.AddRange(BitConverter.GetBytes(Height));
            result.AddRange(data);
			return result.ToArray();
        }
    }
}