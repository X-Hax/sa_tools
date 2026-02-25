using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using SplitTools;
using TextureLib;

namespace ArchiveLib
{
	/// <summary>Generic archive format used in Sonic Adventure 2 PC.</summary>	
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
				Bitmap bitmap_temp;
				Bitmap textureimg;
				GenericTexture texture = GenericTexture.LoadTexture(Data);
				bitmap_temp = texture.Image;
				// The bitmap is cloned to avoid access error on the original file
				textureimg = (Bitmap)bitmap_temp.Clone();
				bitmap_temp.Dispose();
				return textureimg;
			}

		}

        public PAKFile() { }

		/// <summary>Metadata class used to build PAK archives.</summary>
		public class PAKIniData
		{
			/// <summary>Folder name to be used in texture replacement. Should match the PAK filename.</summary>
			public string FolderName { get; set; }

			/// <summary>Dictionary of PAK INI entries.</summary>
			[IniCollection(IniCollectionMode.IndexOnly)]
			public Dictionary<string, PAKIniItem> Items { get; set; }
		}

		/// <summary>PAK item used for building PAK files in SA Tools.</summary>
		public class PAKIniItem
        {
			/// <summary>
			/// Relative path used in texture replacement. For GVM-like PAKs, it's '..\\..\\..\\sonic2\\resource\\gd_pc\\prs\\PAK filename w/o extension'.
			/// For SOC PAKs, it's '..\\..\\..\\sonic2\\resource\\gd_pc\\soc\\PAK filename w/out extension'.
			/// </summary>
			public string LongPath { get; set; }

            public PAKIniItem(string longPath)
            {
                LongPath = longPath;
            }

            public PAKIniItem() { }
        }

		/// <summary>
		/// Retrieves a list of PAK archive entries in the order specified in the PAK archive's INF file.
		/// This method should be used to retrieve the correct texture order for GVM-like PAKs.
		/// </summary>
		/// <param name="filenoext">INF filename (should match the PAK filename) without extension.</param>
		/// <returns>List of PAK entries.</returns>
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
				// There are cases when a texture is present in the PAK but not listed in the INF file. These need to be handled separately.
				foreach (PAKEntry entry in Entries)
				{
					if (!result.Contains(entry))
						result.Add(entry);
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
            IniSerializer.Serialize(new PAKIniData() { FolderName = FolderName, Items = list }, Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path), Path.GetFileNameWithoutExtension(path) + ".ini"));
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

		/// <summary>Checks whether the file is a PAK archive.</summary>
		/// <param name="filename">Path to the file.</param>
		/// <returns>True if the file is a PAK archive.</returns>
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