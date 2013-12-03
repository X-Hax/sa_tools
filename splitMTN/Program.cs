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
                int address = 0;
                int i = ByteConverter.ToInt16(file, address);
                while (i != -1)
                {
                    new Animation(file, ByteConverter.ToInt32(file, address + 4), 0, ByteConverter.ToInt16(file, address + 2))
                        .Save(Path.GetFileNameWithoutExtension(filename) + "/" + i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim");
                    address += 8;
                    i = ByteConverter.ToInt16(file, address);
                }
            }
            finally
            {
                Environment.CurrentDirectory = dir;
            }
        }
    }
}