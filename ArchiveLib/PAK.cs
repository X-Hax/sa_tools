using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using SA_Tools;

// Generic archive format used in Sonic Adventure 2 PC.
namespace ArchiveLib
{
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

		public class PAKIniData
		{
			public string FolderName { get; set; }
			[IniCollection(IniCollectionMode.IndexOnly)]
			public Dictionary<string, PAKIniItem> Items { get; set; }
		}

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
                list.Add(item.Name, new PAKIniItem(item.LongPath));
            }
            IniSerializer.Serialize(new PAKIniData() { FolderName = FolderName, Items = list }, Path.Combine(Path.GetFileNameWithoutExtension(path), Path.GetFileNameWithoutExtension(path) + ".ini"));
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
            {
                if (fs.Length < 4)
                    return false;
                using (BinaryReader br = new BinaryReader(fs))
                    return br.ReadUInt32() == Magic;
            }
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
}