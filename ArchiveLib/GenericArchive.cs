using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VrSharp.Pvr;
using SonicRetro.SAModel;
using System.Drawing;
using System.Drawing.Imaging;
using static ArchiveLib.GenericArchive;
using PuyoTools.Modules.Archive;
using VrSharp;
using VrSharp.Gvr;
using SA_Tools;

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
                using (MemoryStream str = new MemoryStream(Data))
                {
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
                    byte[] tdata = Entries[u].Data;
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
            dictionary_field type;
            for (type = (dictionary_field)pvmxdata[off++]; type != dictionary_field.none; type = (dictionary_field)pvmxdata[off++])
            {
                string name = "";
                uint gbix = 0;
                int width = 0;
                int height = 0;
                while (type != dictionary_field.none)
                {
                    switch (type)
                    {
                        case dictionary_field.global_index:
                            gbix = BitConverter.ToUInt32(pvmxdata, off);
                            off += sizeof(uint);
                            break;

                        case dictionary_field.name:
                            int count = 0;
                            while (pvmxdata[off + count] != 0)
                                count++;
                            name = System.Text.Encoding.UTF8.GetString(pvmxdata, off, count);
                            off += count + 1;
                            break;

                        case dictionary_field.dimensions:
                            width = BitConverter.ToInt32(pvmxdata, off);
                            off += sizeof(int);
                            height = BitConverter.ToInt32(pvmxdata, off);
                            off += sizeof(int);
                            break;
                    }

                    type = (dictionary_field)pvmxdata[off++];

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
                bw.Write((byte)dictionary_field.global_index);
                bw.Write(tex.GBIX);
                bw.Write((byte)dictionary_field.name);
                bw.Write(tex.Name.ToCharArray());
                bw.Write((byte)0);
                if (tex.HasDimensions())
                {
                    bw.Write((byte)dictionary_field.dimensions);
                    bw.Write(tex.Width);
                    bw.Write(tex.Height);
                }
                bw.Write((byte)dictionary_field.none);
                long size;
                using (MemoryStream ms = new MemoryStream(tex.Data))
                {
                    texdata.Add(new OffData(str.Position, ms.ToArray()));
                    size = ms.Length;
                }
                bw.Write(0ul);
                bw.Write(size);
            }
            bw.Write((byte)dictionary_field.none);
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

        enum dictionary_field : byte
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
        const uint Magic_PVM = 0x484D5650; // PVMH
        const uint Magic_GVM = 0x484D5647; // GVMH

        public bool PaletteRequired;
        public PuyoArchiveType Type;

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

        public PuyoFile(byte[] pvmdata)
        {
            ArchiveBase puyobase;
            Entries = new List<GenericArchiveEntry>();

            Type = Identify(pvmdata);
            switch (Type)
            {
                case PuyoArchiveType.PVMFile:
                    puyobase = new PvmArchive();
                    break;
                case PuyoArchiveType.GVMFile:
                    puyobase = new GvmArchive();
                    break;
                default:
                    throw new Exception("Error: Unknown archive format");
            }

            ArchiveReader archiveReader = puyobase.Open(pvmdata);
            foreach (ArchiveEntry puyoentry in archiveReader.Entries)
            {
                MemoryStream vrstream = (MemoryStream)(puyoentry.Open());
                switch (Type)
                {
                    case PuyoArchiveType.PVMFile:
                        PvrTexture pvrt = new PvrTexture(vrstream);
                        if (pvrt.NeedsExternalPalette)
                            PaletteRequired = true;
                        Entries.Add(new PVMEntry(vrstream.ToArray(), Path.GetFileName(puyoentry.Name)));
                        break;
                    case PuyoArchiveType.GVMFile:
                        GvrTexture gvrt = new GvrTexture(vrstream);
                        if (gvrt.NeedsExternalPalette)
                            PaletteRequired = true;
                        Entries.Add(new GVMEntry(vrstream.ToArray(), Path.GetFileName(puyoentry.Name)));
                        break;
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
}