using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using SA_Tools;
using VrSharp;
using VrSharp.Pvr;
using VrSharp.Gvr;
using PuyoTools.Modules.Archive;
using SonicRetro.SAModel;
using static ArchiveLib.GenericArchive;

namespace ArchiveLib
{
    #region Abstract class for all archives
    public abstract class GenericArchive
    {
        public List<GenericArchiveEntry> Entries { get; set; }

        public GenericArchive()
        {
            Entries = new List<GenericArchiveEntry>();
        }

        public void Save(string outputFile)
        {
            File.WriteAllBytes(outputFile, GetBytes());
        }

        public abstract byte[] GetBytes();

        public abstract void CreateIndexFile(string path);

        public abstract class GenericArchiveEntry
        {
            public string Name { get; set; }
            public byte[] Data { get; set; }

            public GenericArchiveEntry(string name, byte[] data)
            {
                Name = name;
                Data = data;
            }

            public GenericArchiveEntry()
            {
                Name = string.Empty;
            }

            public abstract Bitmap GetBitmap();

        }
    }
    #endregion

    #region PAK
    public class PAKFile : GenericArchive
    {
        const uint Magic = 0x6B617001;
        public string FolderName;

        public class PAKEntry : GenericArchiveEntry
        {
            public string LongPath { get; set; }

            public PAKEntry()
            {
                Name = LongPath = string.Empty;
            }

            public PAKEntry(string name, string longpath, byte[] data)
            {
                Name = name;
                LongPath = longpath;
                Data = data;
            }

            public override Bitmap GetBitmap()
            {
                MemoryStream str = new MemoryStream(Data);
                uint check = BitConverter.ToUInt32(Data, 0);
                if (check == 0x20534444) // DDS header
                {
                    PixelFormat pxformat;
                    var image = Pfim.Pfim.FromStream(str, new Pfim.PfimConfig());
                    switch (image.Format)
                    {
                        case Pfim.ImageFormat.Rgba32:
                            pxformat = PixelFormat.Format32bppArgb;
                            break;
                        default:
                            throw new Exception("Error: Unknown image format");
                    }
                    var bitmap = new Bitmap(image.Width, image.Height, pxformat);
                    BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, pxformat);
                    System.Runtime.InteropServices.Marshal.Copy(image.Data, 0, bmpData.Scan0, image.DataLen);
                    bitmap.UnlockBits(bmpData);
                    return bitmap;
                }
                else
                    return new Bitmap(str);
            }
        }

        public PAKFile() { }

        public class PAKIniItem
        {
            public string LongPath { get; set; }
            public PAKIniItem(string longPath)
            {
                LongPath = longPath;
            }
            public PAKIniItem() { }
        }

        public List<PAKEntry> GetSortedEntries(string filenoext)
        {
            bool inf_exists = false;
            foreach (PAKEntry entry in Entries)
                if (entry.Name.Equals(filenoext + '\\' + filenoext + ".inf", StringComparison.OrdinalIgnoreCase))
                    inf_exists = true;
            // Get texture names from PAK INF, if it exists
            if (inf_exists)
            {
                byte[] inf = Entries.Single((file) => file.Name.Equals(filenoext + '\\' + filenoext + ".inf", StringComparison.OrdinalIgnoreCase)).Data;
                List<PAKEntry> result = new List<PAKEntry>(inf.Length / 0x3C);
                for (int i = 0; i < inf.Length; i += 0x3C)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder(0x1C);
                    for (int j = 0; j < 0x1C; j++)
                        if (inf[i + j] != 0)
                            sb.Append((char)inf[i + j]);
                        else
                            break;
                    GenericArchiveEntry gen = Entries.First((file) => file.Name.Equals(filenoext + '\\' + sb.ToString() + ".dds", StringComparison.OrdinalIgnoreCase));
                    result.Add((PAKEntry)gen);
                }
                return result;
            }
            else
            {
                // Otherwise get the original list
                List<PAKEntry> result = new List<PAKEntry>();
                // But only add files that can be converted to Bitmap
                foreach (PAKEntry entry in Entries)
                {
                    string extension = Path.GetExtension(entry.Name).ToLowerInvariant();
                    switch (extension)
                    {
                        case ".dds":
                        case ".png":
                        case ".bmp":
                        case ".gif":
                        case ".jpg":
                            result.Add(entry);
                            break;
                        default:
                            break;
                    }
                }
                return result;
            }
        }

        public override void CreateIndexFile(string path)
		{
            Dictionary<string, PAKIniItem> list = new Dictionary<string, PAKIniItem>(Entries.Count);
            foreach (PAKEntry item in Entries)
            {
                list.Add(FolderName + "\\" + item.Name, new PAKIniItem(item.LongPath));
            }
            IniSerializer.Serialize(list, Path.Combine(Path.GetFileNameWithoutExtension(path), Path.GetFileNameWithoutExtension(path) + ".ini"));
        }

		public PAKFile(string filename)
        {
            FolderName = Path.GetFileNameWithoutExtension(filename).ToLowerInvariant();
            using (FileStream fs = File.OpenRead(filename))
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
                    Entries.Add(new PAKEntry(Path.GetFileName(names[i]), longpaths[i], br.ReadBytes(lens[i])));
            }
        }

        public static bool Identify(string filename)
        {
            using (FileStream fs = System.IO.File.OpenRead(filename))
            using (BinaryReader br = new BinaryReader(fs))
                return br.ReadUInt32() == Magic;
        }

        public override byte[] GetBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms, Encoding.ASCII))
            {
                bw.Write(Magic);
                bw.Write(new byte[33]);
                bw.Write(Entries.Count);
                byte[] totlen = BitConverter.GetBytes(Entries.Sum((a) => a.Data.Length));
                bw.Write(totlen);
                bw.Write(totlen);
                bw.Write(new byte[8]);
                bw.Write(Entries.Count);
                foreach (PAKEntry item in Entries)
                {
                    string fullname = FolderName + "\\" + item.Name;
                    bw.Write(item.LongPath.Length);
                    bw.Write(item.LongPath.ToCharArray());
                    bw.Write(fullname.Length);
                    bw.Write(fullname.ToCharArray());
                    bw.Write(item.Data.Length);
                    bw.Write(item.Data.Length);
                }
                foreach (PAKEntry item in Entries)
                    bw.Write(item.Data);
                return ms.ToArray();
            }
        }
    }
    #endregion

    #region DAT
    public class DATFile : GenericArchive
    {
        public bool Steam;

        public DATFile()
        {
            Entries = new List<GenericArchiveEntry>();
        }

		public override void CreateIndexFile(string path)
		{
            using (TextWriter tw = File.CreateText(Path.Combine(path, "index.txt")))
            {
                Entries.Sort((f1, f2) => StringComparer.OrdinalIgnoreCase.Compare(f1.Name, f2.Name));
                for (int i = 0; i < Entries.Count; i++)
                {
                    tw.WriteLine(Entries[i].Name);
                }
                tw.Flush();
                tw.Close();
            }
        }

        public enum DATArchiveType
        {
            SADX2004 = 0,
            SADX2010 = 1,
            Unknown = -1
        }

        public static DATArchiveType Identify(byte[] file)
        {
            switch (System.Text.Encoding.ASCII.GetString(file, 0, 0x10))
            {
                case "archive  V2.2\0\0\0":
                    return DATArchiveType.SADX2004;
                case "archive  V2.DMZ\0":
                    return DATArchiveType.SADX2010;
                default:
                    return DATArchiveType.Unknown;
            }
        }

        public DATFile(byte[] file)
        {
            switch (Identify(file))
            {
                case DATArchiveType.SADX2004:
                    Steam = false;
                    break;
                case DATArchiveType.SADX2010:
                    Steam = true;
                    break;
                default:
                    throw new Exception("Error: Unknown archive type");
            }
            int count = BitConverter.ToInt32(file, 0x10);
            Entries = new List<GenericArchiveEntry>(count);
            for (int i = 0; i < count; i++)
            {
                Entries.Add(new DATEntry(file, 0x14 + (i * 0xC)));
            }
        }

        public byte[] GetFile(int index)
        {
            return CompressDAT.ProcessBuffer(Entries[index].Data);
        }

        public bool IsFileCompressed(int index)
        {
            return CompressDAT.isFileCompressed(Entries[index].Data);
        }

        public void ReplaceFile(string path, int index)
        {
            Entries[index] = new DATEntry(path);
        }

        public override byte[] GetBytes()
        {
            int fsize = 0x14;
            int hloc = fsize;
            fsize += Entries.Count * 0xC;
            int tloc = fsize;
            foreach (DATEntry item in Entries)
            {
                fsize += item.Name.Length + 1;
            }
            int floc = fsize;
            foreach (DATEntry item in Entries)
            {
                fsize += item.Data.Length;
            }
            byte[] file = new byte[fsize];
            System.Text.Encoding.ASCII.GetBytes(Steam ? "archive  V2.DMZ" : "archive  V2.2").CopyTo(file, 0);
            BitConverter.GetBytes(Entries.Count).CopyTo(file, 0x10);
            foreach (DATEntry item in Entries)
            {
                BitConverter.GetBytes(tloc).CopyTo(file, hloc);
                hloc += 4;
                System.Text.Encoding.ASCII.GetBytes(item.Name).CopyTo(file, tloc);
                tloc += item.Name.Length + 1;
                BitConverter.GetBytes(floc).CopyTo(file, hloc);
                hloc += 4;
                item.Data.CopyTo(file, floc);
                floc += item.Data.Length;
                BitConverter.GetBytes(item.Data.Length).CopyTo(file, hloc);
                hloc += 4;
            }
            return file;
        }
        public class DATEntry : GenericArchiveEntry
        {

            public DATEntry()
            {
                Name = string.Empty;
            }

            public DATEntry(string fileName)
            {
                Name = Path.GetFileName(fileName);
                Data = File.ReadAllBytes(fileName);
            }

            public DATEntry(byte[] file, int address)
            {
                Name = GetCString(file, BitConverter.ToInt32(file, address));
                Data = new byte[BitConverter.ToInt32(file, address + 8)];
                Array.Copy(file, BitConverter.ToInt32(file, address + 4), Data, 0, Data.Length);
            }

            private string GetCString(byte[] file, int address)
            {
                int textsize = 0;
                while (file[address + textsize] > 0)
                    textsize += 1;
                return System.Text.Encoding.ASCII.GetString(file, address, textsize);
            }

            public override Bitmap GetBitmap()
            {
                MemoryStream str = new MemoryStream(Data);
                return new Bitmap(str);
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
    public class PBFile : GenericArchive
    {
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

        public PBFile(byte[] pbdata)
        {
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
    }

    public class PBEntry : GenericArchiveEntry
    {
        public int Offset { get; set; }
        public PvrPixelFormat PixelFormat { get; set; }
        public PvrDataFormat DataFormat { get; set; }
        public uint GBIX { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }

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

        public PBEntry(string filename, int offset)
        {
            Data = File.ReadAllBytes(filename);
            PvrTexture pvrt = new PvrTexture(Data);
            byte[] data = GetHeaderless();
            Name = Path.GetFileNameWithoutExtension(filename);
            Offset = offset;
            PixelFormat = pvrt.PixelFormat;
            DataFormat = pvrt.DataFormat;
            GBIX = pvrt.GlobalIndex;
            Width = pvrt.TextureWidth;
            Height = pvrt.TextureHeight;
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

        public override Bitmap GetBitmap()
        {
            return new PvrTexture(Data).ToBitmap();
        }

        public byte[] GetPVR(byte[] data)
        {
            List<byte> result = new List<byte>();
            int chunksize_file = data.Length;
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
    }

    #endregion

    #region NjArchive
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
    #endregion

    #region PVMX
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
    #endregion

    #region PVM/GVM
    public enum PuyoArchiveType
    {
        Unknown,
        PVMFile,
        GVMFile,
    }

    public class PuyoFile : GenericArchive
    {
        // Archive chunks
        const uint Magic_PVM = 0x484D5650; // PVMH archive
        const uint Magic_GVM = 0x484D5647; // GVMH archive

        // PVM metadata chunks
        const uint Magic_MDLN = 0x4E4C444D; // Model Name
        const uint Magic_COMM = 0x4D4D4F43; // Model Comment
        const uint Magic_CONV = 0x564E4F43; // PVM Converter
        const uint Magic_IMGC = 0x43474D49; // Image Container
        const uint Magic_PVMI = 0x494D5650; // PVM File Info

        // Texture chunks
        const uint Magic_GBIX = 0x58494247; // PVR texture header (GBIX)
        const uint Magic_PVRT = 0x54525650; // PVR texture header (texture data)
        const uint Magic_PVRI = 0x49525650; // PVR texture header (metadata)

        public bool PaletteRequired;
        public PuyoArchiveType Type;

        public enum PVMFlags : ushort
        {
            GlobalIndex = 0x1,
            TextureDimensions = 0x2,
            PixelDataFormat = 0x4,
            Filenames = 0x8,
            ModelName = 0x10,
            Unknown = 0x100, // "Generated by PVMConv" maybe?
        }

        public override void CreateIndexFile(string path)
        {
            using (TextWriter texList = File.CreateText(Path.Combine(path, "index.txt")))
            {
                foreach (GenericArchiveEntry pvmentry in Entries)
                {
                    texList.WriteLine(pvmentry.Name);
                }
                texList.Flush();
                texList.Close();
            }
        }

        public static PuyoArchiveType Identify(byte[] data)
        {
            uint magic = BitConverter.ToUInt32(data, 0);
            switch (magic)
            {
                case Magic_PVM:
                    return PuyoArchiveType.PVMFile;
                case Magic_GVM:
                    return PuyoArchiveType.GVMFile;
                default:
                    return PuyoArchiveType.Unknown;
            }
        }

        public void AddPalette(string startPath)
        {
            VpPalette Palette = null;
            bool gvm = Type == PuyoArchiveType.GVMFile;
            using (System.Windows.Forms.OpenFileDialog a = new System.Windows.Forms.OpenFileDialog
            {
                DefaultExt = gvm ? "gvp" : "pvp",
                Filter = gvm ? "GVP Files|*.gvp" : "PVP Files|*.pvp",
                InitialDirectory = startPath,
                Title = "External palette file"
            })
            {
                if (a.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    Palette = gvm ? (VpPalette)new GvpPalette(a.FileName) : (VpPalette)new PvpPalette(a.FileName);
            }
            foreach (GenericArchiveEntry entry in Entries)
            {
                if (entry is PVMEntry pvme)
                {
                    PvrTexture pvrt = new PvrTexture(pvme.Data);
                    if (pvrt.NeedsExternalPalette)
                        pvme.Palette = (PvpPalette)Palette;
                }
                else if (entry is GVMEntry gvme)
                {
                    GvrTexture gvrt = new GvrTexture(gvme.Data);
                    if (gvrt.NeedsExternalPalette)
                        gvme.Palette = (GvpPalette)Palette;
                }
            }
        }

        public PuyoFile() { }

        public int GetPVRTOffset(byte[] pvmdata, int offset)
        {
            uint header = BitConverter.ToUInt32(pvmdata, offset);
            int currentoffset = offset;
            int size = BitConverter.ToInt32(pvmdata, offset + 4);
            switch (header)
            {
                case Magic_MDLN:
                case Magic_CONV:
                case Magic_IMGC:
                case Magic_COMM:
                case Magic_PVMI:
                case Magic_PVRI: // This one probably shouldn't be here but there are no known examples yet of how this was used
                    goto default;
                case Magic_PVRT:
                    break;
                default:
                    byte[] metachunk = new byte[size];
                    Array.Copy(pvmdata, offset + 8, metachunk, 0, size);
                    currentoffset += size + 8;
                    // Go through metadata until it gets to a PVRT header
                    return GetPVRTOffset(pvmdata, currentoffset);
            }
            return currentoffset;
        }

        public PuyoFile(byte[] pvmdata)
        {
            Entries = new List<GenericArchiveEntry>();

            Type = Identify(pvmdata);
            switch (Type)
            {
                case PuyoArchiveType.PVMFile:
                case PuyoArchiveType.GVMFile:
                    break;
                default:
                    throw new Exception("Error: Unknown archive format");
            }

            if (Type == PuyoArchiveType.PVMFile)
            {
                // Get PVM flags and calculate item size in the entry table
                ushort numtextures = BitConverter.ToUInt16(pvmdata, 0x0A);
                int pvmentrysize = 2;
                int gbixoffset = 0;
                int nameoffset = 0;

                PVMFlags flags = (PVMFlags)BitConverter.ToUInt16(pvmdata, 0x08);
                {
                    if (flags.HasFlag(PVMFlags.Filenames))
                        nameoffset = pvmentrysize;
                    pvmentrysize += 28;
                    if (flags.HasFlag(PVMFlags.PixelDataFormat))
                        pvmentrysize += 2;
                    if (flags.HasFlag(PVMFlags.TextureDimensions))
                        pvmentrysize += 2;
                    if (flags.HasFlag(PVMFlags.GlobalIndex))
                        gbixoffset = pvmentrysize;
                    pvmentrysize += 4;
                }

                int offsetfirst = BitConverter.ToInt32(pvmdata, 0x4) + 8;
                int textureaddr = GetPVRTOffset(pvmdata, offsetfirst);

                for (int t = 0; t < numtextures; t++)
                {
                    int size_gbix = flags.HasFlag(PVMFlags.GlobalIndex) ? 16 : 0;
                    int size = BitConverter.ToInt32(pvmdata, textureaddr + 4);
                    byte[] pvrchunk = new byte[size + 8 + size_gbix];
                    Array.Copy(pvmdata, textureaddr, pvrchunk, 0 + size_gbix, size + 8);

                    // Add GBIX header if the PVM has GBIX enabled
                    if (flags.HasFlag(PVMFlags.GlobalIndex))
                    {
                        Array.Copy(BitConverter.GetBytes(Magic_GBIX), 0, pvrchunk, 0, 4);
                        pvrchunk[4] = 0x08; // Always 8 according to PuyoTools
                        byte[] gbix = BitConverter.GetBytes(BitConverter.ToUInt32(pvmdata, 0xC + pvmentrysize * t + gbixoffset));
                        Array.Copy(gbix, 0, pvrchunk, 9, 4);
                    }

                    // Set filename if the PVM has filenames
                    string entryfn = t.ToString("D3");
                    if (flags.HasFlag(PVMFlags.Filenames))
                    {
                        byte[] namestring = new byte[28];
                        Array.Copy(pvmdata, 0xC + pvmentrysize * t + nameoffset, namestring, 0, 28);
                        entryfn = Encoding.ASCII.GetString(namestring).TrimEnd((char)0);
                    }
                    textureaddr += size + 8;
                    PvrTexture pvrt = new PvrTexture(pvrchunk);
                    if (pvrt.NeedsExternalPalette)
                        PaletteRequired = true;
                    Entries.Add(new PVMEntry(pvrchunk, entryfn + ".pvr"));
                }
            }
            // If it's a GVM, just use Puyo Tools' reader
            else
            {
                ArchiveBase puyobase = new GvmArchive();
                ArchiveReader archiveReader = puyobase.Open(pvmdata);
                foreach (ArchiveEntry puyoentry in archiveReader.Entries)
                {
                    MemoryStream vrstream = (MemoryStream)puyoentry.Open();
                    GvrTexture gvrt = new GvrTexture(vrstream);
                    if (gvrt.NeedsExternalPalette)
                        PaletteRequired = true;
                    Entries.Add(new GVMEntry(vrstream.ToArray(), Path.GetFileName(puyoentry.Name)));
                }
            }
        }

        public override byte[] GetBytes()
        {
            MemoryStream pvmStream = new MemoryStream();
            ArchiveBase pvmbase = new PvmArchive();
            ArchiveWriter puyoArchiveWriter = pvmbase.Create(pvmStream);
            foreach (PVMEntry tex in Entries)
            {
                MemoryStream ms = new MemoryStream(tex.Data);
                puyoArchiveWriter.CreateEntry(ms, tex.Name);
            }
            puyoArchiveWriter.Flush();
            return pvmStream.ToArray();
        }
    }

    public class PVMEntry : GenericArchiveEntry
    {
        public uint GBIX;
        public PvpPalette Palette;

        public PVMEntry(byte[] pvrdata, string name)
        {
            Name = name;
            Data = pvrdata;
            PvrTexture pvrt = new PvrTexture(pvrdata);
            GBIX = pvrt.GlobalIndex;
        }

        public PVMEntry(string filename)
        {
            Name = Path.GetFileName(filename);
            Data = File.ReadAllBytes(filename);
            PvrTexture pvrt = new PvrTexture(Data);
            GBIX = pvrt.GlobalIndex;
        }

        public uint GetGBIX()
        {
            return GBIX;
        }

        public override Bitmap GetBitmap()
        {
            PvrTexture pvrt = new PvrTexture(Data);
            if (pvrt.NeedsExternalPalette)
                pvrt.SetPalette(Palette);
            return pvrt.ToBitmap();
        }
    }

    public class GVMEntry : GenericArchiveEntry
    {
        public uint GBIX;
        public GvpPalette Palette;

        public GVMEntry(byte[] gvrdata, string name)
        {
            Name = name;
            Data = gvrdata;
            GvrTexture gvrt = new GvrTexture(gvrdata);
            GBIX = gvrt.GlobalIndex;
        }

        public GVMEntry(string filename)
        {
            Name = Path.GetFileName(filename);
            Data = File.ReadAllBytes(filename);
            GvrTexture gvrt = new GvrTexture(Data);
            GBIX = gvrt.GlobalIndex;
        }

        public uint GetGBIX()
        {
            return GBIX;
        }

        public override Bitmap GetBitmap()
        {
            GvrTexture gvrt = new GvrTexture(Data);
            if (gvrt.NeedsExternalPalette)
                gvrt.SetPalette(Palette);
            return gvrt.ToBitmap();
        }
    }
    #endregion

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