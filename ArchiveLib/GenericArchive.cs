using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VrSharp.Pvr;

// This library implements support for archive files that are used by tools other than (or in addition to) Texture Editor and SAMDL/SALVL.
// TODO: PVMX

namespace ArchiveLib
{
    #region PAK
    public class PAKFile
	{
		public class File
		{
			public string Name { get; set; }
			public string LongPath { get; set; }
			public byte[] Data { get; set; }

			public File()
			{
				Name = LongPath = string.Empty;
			}

			public File(string name, string longpath, byte[] data)
			{
				Name = name;
				LongPath = longpath;
				Data = data;
			}
		}

		public static uint Magic = 0x6B617001;

		public static bool Is(string filename)
		{
			using (FileStream fs = System.IO.File.OpenRead(filename))
			using (BinaryReader br = new BinaryReader(fs))
				return br.ReadUInt32() == Magic;
		}

		public List<File> Files { get; set; }

		public PAKFile()
		{
			Files = new List<File>();
		}

		public PAKFile(string filename)
			: this()
		{
			using (FileStream fs = System.IO.File.OpenRead(filename))
			using (BinaryReader br = new BinaryReader(fs, Encoding.ASCII))
			{
				if (br.ReadUInt32() != Magic)
					throw new Exception("Error: Unknown archive type");
				fs.Seek(0x39, SeekOrigin.Begin);
				int numfiles = br.ReadInt32();
				string[] longpaths = new string[numfiles];
				string[] names = new string[numfiles];
				int[] lens = new int[numfiles];
				for (int i = 0; i < numfiles; i++)
				{
					longpaths[i] = new string(br.ReadChars(br.ReadInt32()));
					names[i] = new string(br.ReadChars(br.ReadInt32()));
					lens[i] = br.ReadInt32();
					br.ReadInt32();
				}
				for (int i = 0; i < numfiles; i++)
					Files.Add(new File(names[i], longpaths[i], br.ReadBytes(lens[i])));
			}
		}

		public void Save(string filename)
		{
			using (FileStream fs = System.IO.File.Create(filename))
			using (BinaryWriter bw = new BinaryWriter(fs, Encoding.ASCII))
			{
				bw.Write(Magic);
				bw.Write(new byte[33]);
				bw.Write(Files.Count);
				byte[] totlen = BitConverter.GetBytes(Files.Sum((a) => a.Data.Length));
				bw.Write(totlen);
				bw.Write(totlen);
				bw.Write(new byte[8]);
				bw.Write(Files.Count);
				foreach (File item in Files)
				{
					bw.Write(item.LongPath.Length);
					bw.Write(item.LongPath.ToCharArray());
					bw.Write(item.Name.Length);
					bw.Write(item.Name.ToCharArray());
					bw.Write(item.Data.Length);
					bw.Write(item.Data.Length);
				}
				foreach (File item in Files)
					bw.Write(item.Data);
			}
		}
	}
    #endregion

    #region DAT
    public class DATFile
    {
        public List<FENTRY> Entries;
        public bool Steam;

        public int GetCount()
        {
            return Entries.Count;
        }

        public DATFile()
        {
            Entries = new List<FENTRY>();
        }

        public DATFile(byte[] file)
        {
            switch (System.Text.Encoding.ASCII.GetString(file, 0, 0x10))
            {
                case "archive  V2.2\0\0\0":
                    Steam = false;
                    break;
                case "archive  V2.DMZ\0":
                    Steam = true;
                    break;
                default:
                    throw new Exception("Error: Unknown archive type");
            }
            int count = BitConverter.ToInt32(file, 0x10);
            Entries = new List<FENTRY>(count);
            for (int i = 0; i < count; i++)
            {
                Entries.Add(new FENTRY(file, 0x14 + (i * 0xC)));
            }
        }

        public byte[] GetFile(int index)
        {
            return CompressDAT.ProcessBuffer(Entries[index].file);
        }

        public void AddFile(string filePath)
        {
            Entries.Add(new FENTRY(filePath));
        }

        public byte[] GetBytes()
        {
            int fsize = 0x14;
            int hloc = fsize;
            fsize += Entries.Count * 0xC;
            int tloc = fsize;
            foreach (FENTRY item in Entries)
            {
                fsize += item.name.Length + 1;
            }
            int floc = fsize;
            foreach (FENTRY item in Entries)
            {
                fsize += item.file.Length;
            }
            byte[] file = new byte[fsize];
            System.Text.Encoding.ASCII.GetBytes("archive  V2.2").CopyTo(file, 0); // 2004 format only because nobody mods the Steam version anyway
            BitConverter.GetBytes(Entries.Count).CopyTo(file, 0x10);
            foreach (FENTRY item in Entries)
            {
                BitConverter.GetBytes(tloc).CopyTo(file, hloc);
                hloc += 4;
                System.Text.Encoding.ASCII.GetBytes(item.name).CopyTo(file, tloc);
                tloc += item.name.Length + 1;
                BitConverter.GetBytes(floc).CopyTo(file, hloc);
                hloc += 4;
                item.file.CopyTo(file, floc);
                floc += item.file.Length;
                BitConverter.GetBytes(item.file.Length).CopyTo(file, hloc);
                hloc += 4;
            }
            return file;
        }
        public class FENTRY
        {
            public string name;
            public byte[] file;

            public FENTRY()
            {
                name = string.Empty;
            }

            public FENTRY(string fileName)
            {
                name = Path.GetFileName(fileName);
                file = File.ReadAllBytes(fileName);
            }

            public FENTRY(byte[] file, int address)
            {
                name = GetCString(file, BitConverter.ToInt32(file, address));
                this.file = new byte[BitConverter.ToInt32(file, address + 8)];
                Array.Copy(file, BitConverter.ToInt32(file, address + 4), this.file, 0, this.file.Length);
            }

            private string GetCString(byte[] file, int address)
            {
                int textsize = 0;
                while (file[address + textsize] > 0)
                    textsize += 1;
                return System.Text.Encoding.ASCII.GetString(file, address, textsize);
            }
        }

        public static class CompressDAT
        {
            const uint SLIDING_LEN = 0x1000;
            const uint SLIDING_MASK = 0xFFF;

            const byte NIBBLE_HIGH = 0xF0;
            const byte NIBBLE_LOW = 0x0F;

            //TODO: Documentation
            struct OffsetLengthPair
            {
                public byte highByte, lowByte;

                //TODO: Set
                public int Offset
                {
                    get
                    {
                        return ((lowByte & NIBBLE_HIGH) << 4) | highByte;
                    }
                }

                //TODO: Set
                public int Length
                {
                    get
                    {
                        return (lowByte & NIBBLE_LOW) + 3;
                    }
                }
            }

            //TODO: Documentation
            struct ChunkHeader
            {
                private byte flags;
                private byte mask;

                // TODO: Documentation
                public bool ReadFlag(out bool flag)
                {
                    bool endOfHeader = mask != 0x00;

                    flag = (flags & mask) != 0;

                    mask <<= 1;
                    return endOfHeader;
                }

                public ChunkHeader(byte flags)
                {
                    this.flags = flags;
                    this.mask = 0x01;
                }
            }

            //TODO:
            private static void CompressBuffer(byte[] compBuf, byte[] decompBuf /*Starting at + 20*/)
            {

            }

            // Decompresses a Lempel-Ziv buffer.
            // TODO: Add documentation
            private static void DecompressBuffer(byte[] decompBuf, byte[] compBuf /*Starting at + 20*/)
            {
                OffsetLengthPair olPair = new OffsetLengthPair();

                int compBufPtr = 0;
                int decompBufPtr = 0;

                //Create sliding dictionary buffer and clear first 4078 bytes of dictionary buffer to 0
                byte[] slidingDict = new byte[SLIDING_LEN];

                //Set an offset to the dictionary insertion point
                uint dictInsertionOffset = SLIDING_LEN - 18;

                // Current chunk header
                ChunkHeader chunkHeader = new ChunkHeader();

                while (decompBufPtr < decompBuf.Length)
                {
                    // At the start of each chunk...
                    if (!chunkHeader.ReadFlag(out bool flag))
                    {
                        // Load the chunk header
                        chunkHeader = new ChunkHeader(compBuf[compBufPtr++]);
                        chunkHeader.ReadFlag(out flag);
                    }

                    // Each chunk header is a byte and is a collection of 8 flags

                    // If the flag is set, load a character
                    if (flag)
                    {
                        // Copy the character
                        byte rawByte = compBuf[compBufPtr++];
                        decompBuf[decompBufPtr++] = rawByte;

                        // Add the character to the dictionary, and slide the dictionary
                        slidingDict[dictInsertionOffset++] = rawByte;
                        dictInsertionOffset &= SLIDING_MASK;

                    }
                    // If the flag is clear, load an offset/length pair
                    else
                    {
                        // Load the offset/length pair
                        olPair.highByte = compBuf[compBufPtr++];
                        olPair.lowByte = compBuf[compBufPtr++];

                        // Get the offset from the offset/length pair
                        int offset = olPair.Offset;

                        // Get the length from the offset/length pair
                        int length = olPair.Length;

                        for (int i = 0; i < length; i++)
                        {
                            byte rawByte = slidingDict[(offset + i) & SLIDING_MASK];
                            decompBuf[decompBufPtr++] = rawByte;

                            if (decompBufPtr >= decompBuf.Length) return;

                            // Add the character to the dictionary, and slide the dictionary
                            slidingDict[dictInsertionOffset++] = rawByte;
                            dictInsertionOffset &= SLIDING_MASK;
                        }
                    }
                }
            }

            public static bool isFileCompressed(byte[] CompressedBuffer)
            {
                return System.Text.Encoding.ASCII.GetString(CompressedBuffer, 0, 13) == "compress v1.0";
            }

            public static byte[] ProcessBuffer(byte[] CompressedBuffer)
            {
                if (isFileCompressed(CompressedBuffer))
                {
                    uint DecompressedSize = BitConverter.ToUInt32(CompressedBuffer, 16);
                    byte[] DecompressedBuffer = new byte[DecompressedSize];
                    //Xor Decrypt the whole buffer
                    byte XorEncryptionValue = CompressedBuffer[15];

                    byte[] CompBuf = new byte[CompressedBuffer.Length - 20];
                    for (int i = 20; i < CompressedBuffer.Length; i++)
                    {
                        CompBuf[i - 20] = (byte)(CompressedBuffer[i] ^ XorEncryptionValue);
                    }

                    //Decompress the whole buffer
                    DecompressBuffer(DecompressedBuffer, CompBuf);

                    //Switch the buffers around so the decompressed one gets saved instead
                    return DecompressedBuffer;
                }
                else
                {
                    return CompressedBuffer;
                }
            }
        }
    }
    #endregion

    #region PB
    public class PBFile
    {
        List<PBTextureHeader> Headers;
        List<byte[]> Data;

        public int GetCount()
        {
            return Headers.Count;
        }

        public PBFile(byte[] pbdata)
        {
            Headers = new List<PBTextureHeader>();
            Data = new List<byte[]>();
            int numtextures = pbdata[4];
            for (int u = 0; u < numtextures; u++)
            {
                PBTextureHeader hdr = new PBTextureHeader(pbdata, 8 + 16 * u);
                Headers.Add(hdr);
                //Console.WriteLine("Added header {0}: offset {1}, pixel format {2}, data format {3}, GBIX {4}, width {5}, height {6}", u, hdr.Offset, hdr.PixelFormat, hdr.DataFormat, hdr.GBIX, hdr.Width, hdr.Height);
            }
            PBTextureHeader[] headers = Headers.ToArray();
            for (int u = 0; u < numtextures; u++)
            {
                int chunksize;
                if (u == numtextures - 1) chunksize = pbdata.Length - headers[u].Offset;
                else chunksize = headers[u + 1].Offset - headers[u].Offset;
                byte[] pbchunk = new byte[chunksize];
                Array.Copy(pbdata, headers[u].Offset, pbchunk, 0, chunksize);
                Data.Add(pbchunk);
                //Console.WriteLine("Added data: offset {0}, length {1}", headers[u].Offset, pbchunk.Length);
            }
        }

        public PBFile(int count)
        {
            Headers = new List<PBTextureHeader>(count);
            Data = new List<byte[]>(count);
        }

        private int GetCurrentOffset(int index)
        {
            int offset_base = 8 + 16 * Headers.Capacity;
            if (index == 0)
                return offset_base;
            for (int u = 0; u < index; u++)
            {
                offset_base += Data[u].Length;
            }
            return offset_base;
        }

        public void AddPVR(byte[] pvrdata, int index)
        {
            int length = BitConverter.ToInt32(pvrdata, 20) - 8;
            int offset = GetCurrentOffset(index);
            PvrTexture pvr = new PvrTexture(pvrdata);
            Headers.Add(new PBTextureHeader(offset, pvr.PixelFormat, pvr.DataFormat, pvr.GlobalIndex, pvr.TextureWidth, pvr.TextureHeight));
            byte[] pvrdata_nohdr = new byte[length];
            Array.Copy(pvrdata, 32, pvrdata_nohdr, 0, length);
            //Console.WriteLine("Adding texture {0} at offset {1}, length {2} (original PVR {3})", index, offset, length, pvrdata.Length);
            Data.Add(pvrdata_nohdr);
        }

        public byte[] GetPVR(int index)
        {
            List<byte> result = new List<byte>();
            int chunksize_file = Data[index].Length;
            // Make chunk size divisible by 16 because it crashes otherwise
            if (chunksize_file % 16 != 0)
            {
                do
                {
                    chunksize_file++;
                }
                while (chunksize_file % 16 != 0);
            }
            byte[] gbixheader = { 0x47, 0x42, 0x49, 0x58 };
            byte[] pvrtheader = { 0x50, 0x56, 0x52, 0x54 };
            byte[] padding = { 0x20, 0x20, 0x20, 0x20 };
            result.AddRange(gbixheader);
            result.AddRange(BitConverter.GetBytes(8));
            result.AddRange(BitConverter.GetBytes(Headers[index].GBIX));
            result.AddRange(padding);
            result.AddRange(pvrtheader);
            result.AddRange(BitConverter.GetBytes(chunksize_file + 8));
            result.Add((byte)Headers[index].PixelFormat);
            result.Add((byte)Headers[index].DataFormat);
            result.Add(0);
            result.Add(0);
            result.AddRange(BitConverter.GetBytes(Headers[index].Width));
            result.AddRange(BitConverter.GetBytes(Headers[index].Height));
            result.AddRange(Data[index]);
            int pd = 0;
            // Make file size divisible by 16 because it crashes otherwise
            if (result.Count % 16 != 0)
            {
                do
                {
                    result.Add(0);
                    pd++;
                }
                while (result.Count % 16 != 0);
            }
            return result.ToArray();
        }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            result.Add(0x50); // P
            result.Add(0x56); // B
            result.Add(0x42); // V
            result.Add(0x02); // Version ID
            result.AddRange(BitConverter.GetBytes((uint)Headers.Count));
            for (int u = 0; u < Headers.Count; u++)
            {
                result.AddRange(Headers[u].GetBytes());
            }
            for (int u = 0; u < Data.Count; u++)
            {
                result.AddRange(Data[u]);
            }
            return result.ToArray();
        }
    }

    internal class PBTextureHeader
        {
            public int Offset { get; set; }
            public PvrPixelFormat PixelFormat { get; set; }
            public PvrDataFormat DataFormat { get; set; }
            public uint GBIX { get; set; }
            public ushort Width { get; set; }
            public ushort Height { get; set; }

            public byte[] GetBytes()
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

            public PBTextureHeader(byte[] pbdata, int tempaddr)
            {
                Offset = BitConverter.ToInt32(pbdata, tempaddr);
                PixelFormat = (PvrPixelFormat)pbdata[tempaddr + 4];
                DataFormat = (PvrDataFormat)pbdata[tempaddr + 5];
                GBIX = BitConverter.ToUInt32(pbdata, tempaddr + 8);
                Width = BitConverter.ToUInt16(pbdata, tempaddr + 12);
                Height = BitConverter.ToUInt16(pbdata, tempaddr + 12);
            }

            public PBTextureHeader(int offset, PvrPixelFormat pxformat, PvrDataFormat dataformat, uint gbix, ushort width, ushort height)
            {
                Offset = offset;
                PixelFormat = pxformat;
                DataFormat = dataformat;
                GBIX = gbix;
                Width = width;
                Height = height;
            }
        }

    #endregion
}