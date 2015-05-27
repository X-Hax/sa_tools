using System;
using System.Globalization;
using System.IO;
using SonicRetro.SAModel;

namespace SplitNB
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir = Environment.CurrentDirectory;
            try
            {
                string filename;
                if (args.Length > 0)
                {
                    filename = args[0];
                    Console.WriteLine("File: {0}", filename);
                }
                else
                {
                    Console.Write("File: ");
                    filename = Console.ReadLine();
                }
                Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(filename));
                Environment.CurrentDirectory = Directory.CreateDirectory(Path.GetFileNameWithoutExtension(filename)).FullName;
                byte[] file = File.ReadAllBytes(filename);
                if (BitConverter.ToInt32(file, 0) != 0x04424A4E)
                {
                    Console.WriteLine("Invalid NB file.");
                    Environment.CurrentDirectory = dir;
                    return;
                }
                int numfiles = BitConverter.ToInt16(file, 4);
                int curaddr = 8;
                for (int i = 0; i < numfiles; i++)
                {
                    ushort type = BitConverter.ToUInt16(file, curaddr);
                    byte[] chunk = new byte[BitConverter.ToInt32(file, curaddr + 4)];
                    Array.Copy(file, curaddr + 8, chunk, 0, chunk.Length);
                    switch (type)
                    {
                        case 1:
                            ProcessModel(chunk);
                            break;
                        default:
                            File.WriteAllBytes(i.ToString(NumberFormatInfo.InvariantInfo) + ".bin", chunk);
                            break;
                    }
                    curaddr += chunk.Length + 8;
                }
            }
            finally
            {
                Environment.CurrentDirectory = dir;
            }
        }

        static NJS_OBJECT ProcessModel(byte[] file)
        {
            return null;
        }
    }
}