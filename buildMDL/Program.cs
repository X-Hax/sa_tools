using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using IniFile;
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
                SortedDictionary<int, SonicRetro.SAModel.NJS_OBJECT> models = new SortedDictionary<int, SonicRetro.SAModel.NJS_OBJECT>();
				foreach (string file in Directory.GetFiles(Path.GetFileNameWithoutExtension(mdlfilename), "*.sa2mdl"))
					if (int.TryParse(Path.GetFileNameWithoutExtension(file), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out int i))
						models.Add(i, new ModelFile(file).Model);
				Dictionary<int, string> modelnames = IniSerializer.Deserialize<Dictionary<int, string>>(
					Path.Combine(Path.GetFileNameWithoutExtension(mdlfilename), Path.GetFileNameWithoutExtension(mdlfilename) + ".ini"),
					new IniCollectionSettings(IniCollectionMode.IndexOnly));
				List<byte> mdlfile = new List<byte>();
                List<byte> modelbytes = new List<byte>();
				Dictionary<string, uint> labels = new Dictionary<string, uint>();
                uint imageBase = (uint)(modelnames.Count * 8) + 8;
				foreach (KeyValuePair<int, SonicRetro.SAModel.NJS_OBJECT> item in models)
				{
					byte[] tmp = item.Value.GetBytes(imageBase, false, labels, out uint address);
					modelbytes.AddRange(tmp);
					imageBase += (uint)tmp.Length;
				}
				foreach (KeyValuePair<int, string> item in modelnames)
                {
                    mdlfile.AddRange(ByteConverter.GetBytes(item.Key));
                    mdlfile.AddRange(ByteConverter.GetBytes(labels[item.Value]));
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