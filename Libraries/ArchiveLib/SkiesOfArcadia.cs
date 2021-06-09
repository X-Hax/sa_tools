using System;
using System.Drawing;
using System.Text;

// Various archive formats from Skies of Arcadia (Dreamcast).
namespace ArchiveLib
{
    #region Skies of Arcadia MLD
    public class MLDArchive : GenericArchive
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

        public class MLDArchiveEntry : GenericArchiveEntry
        {

            public override Bitmap GetBitmap()
            {
                throw new NotImplementedException();
            }

            public MLDArchiveEntry(byte[] data, string name)
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

        public enum MLDArchiveType
        {
            Manatee = 0,
            CRI = 1,
            CRIBigEndian = 2
        }

        public MLDArchiveType Identify(byte[] file)
        {
            if (file[0] == 0)
                return MLDArchiveType.CRIBigEndian;
            // Check if there is ADX
            for (int i = 0; i < file.Length; i += 8)
            {
                if (BitConverter.ToUInt64(file, i) == Magic_CRI)
                    return MLDArchiveType.CRI;
            }
            return MLDArchiveType.Manatee;
        }

        public MLDArchive(byte[] file)
        {
            int count = BitConverter.ToInt32(file, 0);
            int nmlddatapointer = BitConverter.ToInt32(file, 4);
            uint flags = BitConverter.ToUInt32(file, 8);
            int realdatapointer = BitConverter.ToInt32(file, 12);
            int textablepointer = BitConverter.ToInt32(file, 16);
            Console.WriteLine("Number of NMLD entries: {0}", count);
            int sizereal = textablepointer - realdatapointer;
            int sizenmld = realdatapointer - nmlddatapointer;
            Console.WriteLine("First entry: {0} size {1}", realdatapointer.ToString("X"), sizereal);
            byte[] nmld = new byte[sizenmld];
            Array.Copy(file, nmlddatapointer, nmld, 0, sizenmld);
            Entries.Add(new MLDArchiveEntry(nmld, "data.nmld"));
            //Entries = new List<GenericArchiveEntry>(count);
            //List<int> offsets = new List<int>(count);
            int numtex = BitConverter.ToInt32(file, textablepointer);
            Console.WriteLine("Number of textures: {0}, pointer: {1}", numtex, textablepointer.ToString("X"));
            if (numtex > 0)
            {
                int texdataoffset = textablepointer + 4 + numtex * 44;
                Console.WriteLine("Texture offset original: {0}", texdataoffset.ToString("X"));
                // Get through the padding
                if (file[texdataoffset] == 0)
                {
                    do
                    {
                        texdataoffset += 1;
                    }
                    while (file[texdataoffset] == 0);
                }
                Console.WriteLine("Textures from {0}", texdataoffset.ToString("X"));
                int currenttextureoffset = texdataoffset;
                for (int i = 0; i < numtex; i++)
                {
                    byte[] namestring = new byte[36];
                    Array.Copy(file, textablepointer + 4 + i * 44, namestring, 0, 36);
                    string entryfn = Encoding.ASCII.GetString(namestring).TrimEnd((char)0);
                    int size = BitConverter.ToInt32(file, textablepointer + 4 + i * 44 + 40);
                    Console.WriteLine("Entry {0} name {1} size {2}", i, entryfn, size);
                    byte[] texture = new byte[size];
                    Array.Copy(file, currenttextureoffset, texture, 0, size);
                    Entries.Add(new MLDArchiveEntry(texture, entryfn + ".pvr"));
                    currenttextureoffset += size;
                }
            }
        }

        public override byte[] GetBytes()
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
