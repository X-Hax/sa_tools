using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SonicRetro.SAModel;

namespace buildMTN
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir = Environment.CurrentDirectory;
            try
            {
                Queue<string> argq = new Queue<string>(args);
                if (argq.Count > 0 && argq.Peek().Equals("/be", StringComparison.OrdinalIgnoreCase))
                {
                    ByteConverter.BigEndian = true;
                    argq.Dequeue();
                }
                string mtnfilename;
                if (argq.Count > 0)
                {
                    mtnfilename = argq.Dequeue();
                    Console.WriteLine("File: {0}", mtnfilename);
                }
                else
                {
                    Console.Write("File: ");
                    mtnfilename = Console.ReadLine();
                }
                mtnfilename = Path.Combine(Environment.CurrentDirectory, mtnfilename);
                Environment.CurrentDirectory = Path.GetDirectoryName(mtnfilename);
                SortedDictionary<short, Animation> anims = new SortedDictionary<short, Animation>();
				foreach (string file in Directory.GetFiles(Path.GetFileNameWithoutExtension(mtnfilename), "*.saanim"))
					if (short.TryParse(Path.GetFileNameWithoutExtension(file), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out short i))
						anims.Add(i, Animation.Load(file));
				List<byte> mtnfile = new List<byte>();
                List<byte> animbytes = new List<byte>();
                uint imageBase = (uint)(anims.Count * 8) + 8;
                foreach (KeyValuePair<short, Animation> item in anims)
                {
                    mtnfile.AddRange(ByteConverter.GetBytes(item.Key));
                    mtnfile.AddRange(ByteConverter.GetBytes((short)item.Value.ModelParts));
					animbytes.AddRange(item.Value.GetBytes((uint)(imageBase), out uint address));
					mtnfile.AddRange(ByteConverter.GetBytes(imageBase + address));
                    imageBase = (uint)(anims.Count * 8) + 8 + (uint)animbytes.Count;
                }
                mtnfile.AddRange(new byte[] { 0xFF, 0xFF, 0, 0, 0, 0, 0, 0 });
                mtnfile.AddRange(animbytes);
                if (Path.GetExtension(mtnfilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
                    FraGag.Compression.Prs.Compress(mtnfile.ToArray(), mtnfilename);
                else
                    File.WriteAllBytes(mtnfilename, mtnfile.ToArray());
            }
            finally
            {
                Environment.CurrentDirectory = dir;
            }
        }
    }
}