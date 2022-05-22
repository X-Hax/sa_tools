using System.Collections.Generic;
using System.IO;
using System.Drawing;

namespace ArchiveLib
{
    public abstract class GenericArchive
    {
		public bool hasNameData = true;
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
}