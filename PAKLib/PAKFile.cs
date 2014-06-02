using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PAKLib
{
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
}