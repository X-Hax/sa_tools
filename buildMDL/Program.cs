using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SonicRetro.SAModel;

namespace buildMDL
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
                string mdlfilename;
                if (argq.Count > 0)
                {
                    mdlfilename = argq.Dequeue();
                    Console.WriteLine("File: {0}", mdlfilename);
                }
                else
                {
                    Console.Write("File: ");
                    mdlfilename = Console.ReadLine();
                }
                mdlfilename = Path.GetFullPath(mdlfilename);
                Environment.CurrentDirectory = Path.GetDirectoryName(mdlfilename);
                SortedDictionary<int, SonicRetro.SAModel.Object> models = new SortedDictionary<int, SonicRetro.SAModel.Object>();
                int i;
                foreach (string file in Directory.GetFiles(Path.GetFileNameWithoutExtension(mdlfilename), "*.sa2mdl"))
                    if (int.TryParse(Path.GetFileNameWithoutExtension(file), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out i))
                        models.Add(i, new ModelFile(file).Model);
                List<byte> mdlfile = new List<byte>();
                List<byte> modelbytes = new List<byte>();
                uint imageBase = (uint)(models.Count * 8) + 8;
                foreach (KeyValuePair<int, SonicRetro.SAModel.Object> item in models)
                {
                    mdlfile.AddRange(ByteConverter.GetBytes(item.Key));
                    uint address;
                    modelbytes.AddRange(item.Value.GetBytes((uint)(imageBase), false, out address));
                    mdlfile.AddRange(ByteConverter.GetBytes(imageBase + address));
                    imageBase = (uint)(models.Count * 8) + 8 + (uint)modelbytes.Count;
                }
                mdlfile.AddRange(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0, 0, 0, 0 });
                mdlfile.AddRange(modelbytes);
                if (Path.GetExtension(mdlfilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
                    FraGag.Compression.Prs.Compress(mdlfile.ToArray(), mdlfilename);
                else
                    File.WriteAllBytes(mdlfilename, mdlfile.ToArray());
            }
            finally
            {
                Environment.CurrentDirectory = dir;
            }
        }
    }
}