using System;
using System.IO;

namespace SAModel.DataToolbox
{
	public static partial class ObjScan
	{
		static void CreateSplitIni(string filename)
		{
			if (addresslist.Count == 0)
				return;
			if (File.Exists(filename))
				filename = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + "_new.ini");
			Console.WriteLine("Creating split INI file: {0}", filename);
			StreamWriter sw = File.CreateText(filename);
			sw.WriteLine("key=" + ImageBase.ToString("X"));
			if (Path.GetExtension(filename).ToLowerInvariant() == ".prs")
				sw.WriteLine("compressed=true");
			if (BigEndian)
				sw.WriteLine("bigendian=true");
			if (ReverseColors)
				sw.WriteLine("reverse=true");
			if (DataOffset != 0)
				sw.WriteLine("offset=" + DataOffset.ToString("X8"));
			foreach (var entry in addresslist)
			{
				//Console.WriteLine("Adding object {0}", v.ObjectName);
				switch (entry.Value)
				{
					case "NJS_CNK_OBJECT":
					case "cnkobj":
						sw.WriteLine("[" + entry.Key.ToString("X8") + "]");
						sw.WriteLine("type=chunkmodel");
						sw.WriteLine("address=" + entry.Key.ToString("X8"));
						if (!SingleOutputFolder)
							sw.WriteLine("filename=chunkmodels/" + entry.Key.ToString("X8") + ".sa2mdl");
						else
							sw.WriteLine("filename=" + entry.Key.ToString("X8") + ".sa2mdl");
						sw.WriteLine();
						break;
					case "NJS_GC_OBJECT":
					case "gcobj":
						sw.WriteLine("[" + entry.Key.ToString("X8") + "]");
						sw.WriteLine("type=gcmodel");
						sw.WriteLine("address=" + entry.Key.ToString("X8"));
						if (!SingleOutputFolder)
							sw.WriteLine("filename=gcmodels/" + entry.Key.ToString("X8") + ".sa2bmdl");
						else
							sw.WriteLine("filename=" + entry.Key.ToString("X8") + ".sa2bmdl");
						sw.WriteLine();
						break;
					case "NJS_OBJECT":
					case "NJS_OBJECT_OLD":
					case "obj":
						sw.WriteLine("[" + entry.Key.ToString("X8") + "]");
						sw.WriteLine(entry.Value == "NJS_OBJECT_OLD" ? "type=basicmodel" : "type=basicdxmodel");
						sw.WriteLine("address=" + entry.Key.ToString("X8"));
						if (!SingleOutputFolder)
							sw.WriteLine("filename=basicmodels/" + entry.Key.ToString("X8") + ".sa1mdl");
						else
							sw.WriteLine("filename=" + entry.Key.ToString("X8") + ".sa1mdl");
						bool first = true;
						foreach (var item in actionlist)
						{
							if (item.Value[0] == entry.Key)
							{
								if (first)
								{
									sw.Write("animations=");
									first = false;
								}
								else sw.Write(",");
								if (!SingleOutputFolder)
									sw.Write("../actions/" + item.Key.ToString("X8") + ".saanim");
								else
									sw.Write(item.Key.ToString("X8") + ".saanim");
							}
						}
						if (!first) sw.WriteLine();
						sw.WriteLine();
						break;
					case "landtable_SADX":
					case "landtable_SA1":
					case "landtable_SA2":
					case "landtable_SA2B":
					case "LandTable":
					case "_OBJ_LANDTABLE":
						sw.WriteLine("[" + entry.Key.ToString("X8") + "]");
						sw.WriteLine("type=landtable");
						sw.WriteLine("address=" + entry.Key.ToString("X8"));
						if (!SingleOutputFolder)
							sw.WriteLine("filename=levels/" + entry.Key.ToString("X8") + ".sa1lvl");
						else
							sw.WriteLine("filename=" + entry.Key.ToString("X8") + ".sa1lvl");
						switch (entry.Value)
						{
							case "landtable_SA1":
								sw.WriteLine("format=SA1");
								break;
							case "landtable_SADX":
								sw.WriteLine("format=SADX");
								break;
							case "landtable_SA2":
								sw.WriteLine("format=SA2");
								break;
							case "landtable_SA2B":
								sw.WriteLine("format=SA2B");
								break;
							default:
								break;
						}
						sw.WriteLine();
						break;
					case "NJS_MOTION":
						sw.WriteLine("[" + entry.Key.ToString("X8") + "]");
						sw.WriteLine("type=animation");
						sw.WriteLine("address=" + entry.Key.ToString("X8"));
						sw.WriteLine("numparts=" + actionlist[entry.Key][1].ToString());
						if (!SingleOutputFolder)
							sw.WriteLine("filename=actions/" + entry.Key.ToString("X8") + ".saanim");
						else
							sw.WriteLine("filename=" + entry.Key.ToString("X8") + ".saanim");
						sw.WriteLine();
						break;
				}
			}
			sw.Flush();
			sw.Close();
		}
	}
}
