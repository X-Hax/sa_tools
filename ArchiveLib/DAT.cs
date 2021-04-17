using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

// Archives used for soundbanks in SADX PC2004/X360/PS3/Steam and SADX 2004 installer.
namespace ArchiveLib
{
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
}