﻿using SAModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SplitTools.SAArc
{
	public static class sa2MDL
	{
		public static void Split(string filePath, string outputFolder, string[] animationPaths, string mdllabelfile = null, string mtnlabelfile = null)
		{
			string dir = Environment.CurrentDirectory;
			try
			{
				if (outputFolder[outputFolder.Length - 1] != '/') outputFolder = string.Concat(outputFolder, "/");

				// get file name, read it from the console if nothing
				string mdlfilename = filePath;

				mdlfilename = Path.GetFullPath(mdlfilename);

				// load model file
				byte[] mdlfile;
				if (Path.GetExtension(mdlfilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
					mdlfile = FraGag.Compression.Prs.Decompress(mdlfilename);
				else
					mdlfile = File.ReadAllBytes(mdlfilename);
				ByteConverter.BigEndian = false;
				int addr = 0;
				ushort ile = ByteConverter.ToUInt16(mdlfile, 0);
				if (ile == 0)
				{
					ile = ByteConverter.ToUInt16(mdlfile, 8);
					addr = 8;
				}
				ByteConverter.BigEndian = true;
				if (ile < ByteConverter.ToUInt16(mdlfile, addr))
					ByteConverter.BigEndian = false;
				Environment.CurrentDirectory = Path.GetDirectoryName(mdlfilename);
				(string filename, byte[] data)[] animfiles = new (string, byte[])[animationPaths.Length];
				for (int j = 0; j < animationPaths.Length; j++)
				{
					byte[] data = File.ReadAllBytes(animationPaths[j]);
					if (Path.GetExtension(animationPaths[j]).Equals(".prs", StringComparison.OrdinalIgnoreCase))
						data = FraGag.Compression.Prs.Decompress(data);
					animfiles[j] = (Path.GetFileNameWithoutExtension(animationPaths[j]), data);
				}
				if (outputFolder.Length != 0)
				{
					if (!Directory.Exists(outputFolder)) Directory.CreateDirectory(outputFolder);
					Environment.CurrentDirectory = outputFolder;
				}
				else
					Environment.CurrentDirectory = Path.GetDirectoryName(mdlfilename);
				Directory.CreateDirectory(Path.GetFileNameWithoutExtension(mdlfilename));

				// mdl labels
				Dictionary<int, string> mdlsectionlist = new Dictionary<int, string>();
				Dictionary<int, string> mdlsplitfilenames = new Dictionary<int, string>();
				if (mdllabelfile != null) mdllabelfile = Path.GetFullPath(mdllabelfile);
				if (File.Exists(mdllabelfile))
				{
					mdlsplitfilenames = IniSerializer.Deserialize<Dictionary<int, string>>(mdllabelfile);
				}
				string[] mdlmetadata = new string[0];

				// mtn labels
				Dictionary<int, string> mtnsectionlist = new Dictionary<int, string>();
				Dictionary<int, string> mtnsplitfilenames = new Dictionary<int, string>();
				if (mtnlabelfile != null) mtnlabelfile = Path.GetFullPath(mtnlabelfile);
				if (File.Exists(mtnlabelfile))
				{
					mtnsplitfilenames = IniSerializer.Deserialize<Dictionary<int, string>>(mtnlabelfile);
				}
				string[] mtnmetadata = new string[0];

				// getting model pointers
				int address = 0;
				int i = ByteConverter.ToInt32(mdlfile, address);
				SortedDictionary<int, int> modeladdrs = new SortedDictionary<int, int>();
				while (i != -1)
				{
					modeladdrs[i] = ByteConverter.ToInt32(mdlfile, address + 4);
					address += 8;
					i = ByteConverter.ToInt32(mdlfile, address);
				}

				// load models from pointer list
				Dictionary<int, NJS_OBJECT> models = new Dictionary<int, NJS_OBJECT>();
				Dictionary<int, string> modelnames = new Dictionary<int, string>();
				List<string> partnames = new List<string>();
				foreach (KeyValuePair<int, int> item in modeladdrs)
				{
					NJS_OBJECT obj = new NJS_OBJECT(mdlfile, item.Value, 0, ModelFormat.Chunk, new Dictionary<int, Attach>());
					modelnames[item.Key] = obj.Name;
					if (!partnames.Contains(obj.Name))
					{
						List<string> names = new List<string>(obj.GetObjects().Select((o) => o.Name));
						foreach (int idx in modelnames.Where(a => names.Contains(a.Value)).Select(a => a.Key))
							models.Remove(idx);
						models[item.Key] = obj;
						partnames.AddRange(names);
					}
				}

				// load animations
				Dictionary<int, string> animfns = new Dictionary<int, string>();
				Dictionary<int, NJS_MOTION> anims = new Dictionary<int, NJS_MOTION>();
				string animmeta = null;
				foreach ((string anifilename, byte[] anifile) in animfiles)
				{
					Dictionary<int, int> processedanims = new Dictionary<int, int>();
					MTNInfo ini = new MTNInfo() { BigEndian = ByteConverter.BigEndian };
					Directory.CreateDirectory(anifilename);
					address = 0;
					i = ByteConverter.ToInt16(anifile, address);
					while (i != -1)
					{
						if (mtnlabelfile != null)
						{
							mtnmetadata = mtnsplitfilenames[i].Split('|'); // Filename|Description
							string outFilename = mtnmetadata[0];
							if (!mtnmetadata[0].StartsWith("NO FILE"))
							{
								animmeta = mtnmetadata[1];
								mtnsectionlist[i] = outFilename + "|" + animmeta;
							}
							else
								mtnsectionlist[i] = "NULL";
						}
						int aniaddr = ByteConverter.ToInt32(anifile, address + 4);
						if (!processedanims.ContainsKey(aniaddr))
						{
							anims[i] = new NJS_MOTION(anifile, ByteConverter.ToInt32(anifile, address + 4), 0, ByteConverter.ToInt16(anifile, address + 2));
							animfns[i] = Path.Combine(anifilename, i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim");
							anims[i].Description = animmeta;
							anims[i].Save(animfns[i]);
							processedanims[aniaddr] = i;
						}
						ini.Indexes[(short)i] = "animation_" + aniaddr.ToString("X8");
						address += 8;
						i = ByteConverter.ToInt16(anifile, address);
					}
					IniSerializer.Serialize(ini, Path.Combine(anifilename, anifilename + ".ini"));
				}
				// save output model files
				foreach (KeyValuePair<int, NJS_OBJECT> model in models)
				{
					List<string> animlist = new List<string>();
					foreach (KeyValuePair<int, NJS_MOTION> anim in anims)
					{
						if (model.Value.CountAnimated() == anim.Value.ModelParts)
						{
							string rel = animfns[anim.Key].Replace(outputFolder, string.Empty);
							if (rel.Length > 1 && rel[1] != ':') rel = "../" + rel;
							animlist.Add(rel);
						}
					}
					// Model Labels
					if (mdllabelfile != null)
					{
						mdlmetadata = mdlsplitfilenames[model.Key].Split('|'); // Filename|Description|Texture file
						string outFilename = mdlmetadata[0];
						if (mdlsplitfilenames[model.Key] == "NULL")
							mdlsectionlist.Add(model.Key, "NULL");
						string outResult = outFilename;
						if (mdlmetadata.Length > 1)
							outResult += ("|" + mdlmetadata[1]);
						if (mdlmetadata.Length > 2)
							outResult += ("|" + mdlmetadata[2]);
						mdlsectionlist.Add(model.Key, outResult);
					}
					ModelFile.CreateFile(Path.Combine(Path.GetFileNameWithoutExtension(mdlfilename),
						model.Key.ToString(NumberFormatInfo.InvariantInfo) + ".sa2mdl"), model.Value, animlist.ToArray(),
						null, null, null, ModelFormat.Chunk);
				}

				// labels for SAMDL Project Mode
				if (mdllabelfile != null)
				{
					string mdlsectionListFilename = Path.GetFileNameWithoutExtension(mdllabelfile) + "_data.ini";
					IniSerializer.Serialize(mdlsectionlist, Path.Combine(outputFolder, mdlsectionListFilename));
				}

				if (mtnlabelfile != null)
				{
					string mtnsectionListFilename = Path.GetFileNameWithoutExtension(mtnlabelfile) + "_data.ini";
					IniSerializer.Serialize(mtnsectionlist, Path.Combine(outputFolder, mtnsectionListFilename));
				}

				// save ini file
				IniSerializer.Serialize(new MDLInfo() { BigEndian = ByteConverter.BigEndian, Indexes = modelnames },
					Path.Combine(Path.GetFileNameWithoutExtension(mdlfilename), Path.GetFileNameWithoutExtension(mdlfilename) + ".ini"));
			}
			finally
			{
				Environment.CurrentDirectory = dir;
			}
		}

		public static void Build(bool? isBigEndian, string mdlfilename)
		{
			string dir = Environment.CurrentDirectory;
			try
			{
				mdlfilename = Path.GetFullPath(mdlfilename);
				if (Directory.Exists(mdlfilename))
					mdlfilename += ".prs";
				Environment.CurrentDirectory = Path.GetDirectoryName(mdlfilename);
				SortedDictionary<int, NJS_OBJECT> models = new SortedDictionary<int, NJS_OBJECT>();
				foreach (string file in Directory.GetFiles(Path.GetFileNameWithoutExtension(mdlfilename), "*.sa2mdl"))
					if (int.TryParse(Path.GetFileNameWithoutExtension(file), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out int i))
						models.Add(i, new ModelFile(file).Model);
				MDLInfo mdlinfo = IniSerializer.Deserialize<MDLInfo>(
					Path.Combine(Path.GetFileNameWithoutExtension(mdlfilename), Path.GetFileNameWithoutExtension(mdlfilename) + ".ini"));
				if (!isBigEndian.HasValue)
					ByteConverter.BigEndian = mdlinfo.BigEndian;
				else
					ByteConverter.BigEndian = isBigEndian.Value;
				List<byte> mdlfile = new List<byte>();
				List<byte> modelbytes = new List<byte>();
				Dictionary<string, uint> labels = new Dictionary<string, uint>();
				uint imageBase = (uint)(mdlinfo.Indexes.Count * 8) + 8;
				foreach (KeyValuePair<int, NJS_OBJECT> item in models)
				{
					byte[] tmp = item.Value.GetBytes(imageBase, false, labels, new List<uint>(), out uint address);
					modelbytes.AddRange(tmp);
					imageBase += (uint)tmp.Length;
				}
				foreach (KeyValuePair<int, string> item in mdlinfo.Indexes)
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

	public class MDLInfo
	{
		public bool BigEndian { get; set; }
		[IniCollection(IniCollectionMode.IndexOnly)]
		public Dictionary<int, string> Indexes { get; set; } = new Dictionary<int, string>();
	}

	public class MTNInfo
	{
		public bool BigEndian { get; set; }
		[IniCollection(IniCollectionMode.IndexOnly)]
		public Dictionary<short, string> Indexes { get; set; } = new Dictionary<short, string>();
	}
}
