using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SonicRetro.SAModel;

namespace splitMTN
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
                string filename;
                if (argq.Count > 0)
                {
                    filename = argq.Dequeue();
                    Console.WriteLine("File: {0}", filename);
                }
                else
                {
                    Console.Write("File: ");
                    filename = Console.ReadLine();
                }
                filename = Path.Combine(Environment.CurrentDirectory, filename);
                Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(filename));
                byte[] file = File.ReadAllBytes(filename);
                if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
                    file = FraGag.Compression.Prs.Decompress(file);
                Directory.CreateDirectory(Path.GetFileNameWithoutExtension(filename));
				Dictionary<int, int> processedanims = new Dictionary<int, int>();
				Dictionary<int, string> ini = new Dictionary<int, string>();
                int address = 0;
                int i = ByteConverter.ToInt16(file, address);
                while (i != -1)
                {
					int aniaddr = ByteConverter.ToInt32(file, address + 4);
					if (!processedanims.ContainsKey(aniaddr))
					{
						new Animation(file, aniaddr, 0, ByteConverter.ToInt16(file, address + 2))
                        .Save(Path.GetFileNameWithoutExtension(filename) + "/" + i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim");
						processedanims[aniaddr] = i;
					}
					ini[i]= "animation_" + aniaddr.ToString("X8");
					address += 8;
                    i = ByteConverter.ToInt16(file, address);
                }
				IniSerializer.Serialize(ini, new IniCollectionSettings(IniCollectionMode.IndexOnly), Path.Combine(Path.GetFileNameWithoutExtension(filename), Path.GetFileNameWithoutExtension(filename) + ".ini"));
			}
			finally
            {
                Environment.CurrentDirectory = dir;
            }
        }
    }
}