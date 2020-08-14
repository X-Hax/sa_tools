using SA_Tools;
using SA_Tools.SplitMDL;
using SonicRetro.SAModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

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
				bool? be = null;
				if (argq.Count > 0)
					if (argq.Peek().Equals("/be", StringComparison.OrdinalIgnoreCase))
					{
						be = true;
						argq.Dequeue();
					}
					else if (argq.Peek().Equals("/le", StringComparison.OrdinalIgnoreCase))
					{
						be = false;
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
				switch (be)
				{
					case true:
						ByteConverter.BigEndian = true;
						break;
					case null:
						int addr = 0;
						ushort ile = ByteConverter.ToUInt16(file, 0);
						if (ile == 0)
						{
							ile = ByteConverter.ToUInt16(file, 8);
							addr = 8;
						}
						ByteConverter.BigEndian = true;
						if (ile < ByteConverter.ToUInt16(file, addr))
							ByteConverter.BigEndian = false;
						break;
				}
				MTNInfo ini = new MTNInfo { BigEndian = ByteConverter.BigEndian };
				int address = 0;
				short i = ByteConverter.ToInt16(file, address);
				while (i != -1)
				{
					int aniaddr = ByteConverter.ToInt32(file, address + 4);
					if (!processedanims.ContainsKey(aniaddr))
					{
						new NJS_MOTION(file, aniaddr, 0, ByteConverter.ToInt16(file, address + 2))
						.Save(Path.GetFileNameWithoutExtension(filename) + "/" + i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim");
						processedanims[aniaddr] = i;
					}
					ini.Indexes[i] = "animation_" + aniaddr.ToString("X8");
					address += 8;
					i = ByteConverter.ToInt16(file, address);
				}
				IniSerializer.Serialize(ini, Path.Combine(Path.GetFileNameWithoutExtension(filename), Path.GetFileNameWithoutExtension(filename) + ".ini"));
			}
			finally
			{
				Environment.CurrentDirectory = dir;
			}
		}
	}
}