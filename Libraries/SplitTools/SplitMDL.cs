using SAModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SplitTools.SAArc
{
	public static class SA2MDL
	{
		public static void Split(string filePath, string outputFolder, string[] animationPaths, string mdlLabelFile = null, string mtnLabelFile = null, string exMtnFile = null)
		{
			var dir = Environment.CurrentDirectory;
			
			try
			{
				if (outputFolder[^1] != '/')
				{
					outputFolder = string.Concat(outputFolder, "/");
				}

				// Get file name, read it from the console if nothing
				var mdlFilename = filePath;
				mdlFilename = Path.GetFullPath(mdlFilename);

				// Load model file
				byte[] mdlFile;
				
				if (Path.GetExtension(mdlFilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				{
					mdlFile = FraGag.Compression.Prs.Decompress(mdlFilename);
				}
				else
				{
					mdlFile = File.ReadAllBytes(mdlFilename);
				}

				ByteConverter.BigEndian = false;
				var addr = 0;
				var ile = ByteConverter.ToUInt16(mdlFile, 0);
				
				if (ile == 0)
				{
					ile = ByteConverter.ToUInt16(mdlFile, 8);
					addr = 8;
				}
				
				ByteConverter.BigEndian = true;
				
				if (ile < ByteConverter.ToUInt16(mdlFile, addr))
				{
					ByteConverter.BigEndian = false;
				}

				Environment.CurrentDirectory = Path.GetDirectoryName(mdlFilename);
				
				(string filename, byte[] data)[] animFiles = new (string, byte[])[animationPaths.Length];
				
				for (var j = 0; j < animationPaths.Length; j++)
				{
					var data = File.ReadAllBytes(animationPaths[j]);
					if (Path.GetExtension(animationPaths[j]).Equals(".prs", StringComparison.OrdinalIgnoreCase))
					{
						data = FraGag.Compression.Prs.Decompress(data);
					}

					animFiles[j] = (Path.GetFileNameWithoutExtension(animationPaths[j]), data);
				}
				
				if (outputFolder.Length != 0)
				{
					if (!Directory.Exists(outputFolder))
					{
						Directory.CreateDirectory(outputFolder);
					}

					Environment.CurrentDirectory = outputFolder;
				}
				else
				{
					Environment.CurrentDirectory = Path.GetDirectoryName(mdlFilename);
				}

				Directory.CreateDirectory(Path.GetFileNameWithoutExtension(mdlFilename));

				// mdl labels
				var mdlsectionlist = new Dictionary<int, string>();
				var mdlsplitfilenames = new Dictionary<int, string>();
				if (mdlLabelFile != null)
				{
					mdlLabelFile = Path.GetFullPath(mdlLabelFile);
				}

				if (File.Exists(mdlLabelFile))
				{
					mdlsplitfilenames = IniSerializer.Deserialize<Dictionary<int, string>>(mdlLabelFile);
				}
				string[] mdlmetadata = [];

				// mtn labels
				var mtnsectionlist = new Dictionary<int, string>();
				var mtnsplitfilenames = new Dictionary<int, string>();
				if (mtnLabelFile != null)
				{
					mtnLabelFile = Path.GetFullPath(mtnLabelFile);
				}

				if (File.Exists(mtnLabelFile))
				{
					mtnsplitfilenames = IniSerializer.Deserialize<Dictionary<int, string>>(mtnLabelFile);
				}
				var mtnmetadata = new string[0];

				// External motion paths
				var mtnpathlist = new Dictionary<int, string>();
				var mtnsplitpaths = new Dictionary<int, string>();
				if (exMtnFile != null)
				{
					exMtnFile = Path.GetFullPath(exMtnFile);
				}

				if (File.Exists(exMtnFile))
				{
					mtnsplitpaths = IniSerializer.Deserialize<Dictionary<int, string>>(exMtnFile);
				}
				string[] exmtndata = [];

				// Getting model pointers
				var address = 0;
				var i = ByteConverter.ToInt32(mdlFile, address);
				var modeladdrs = new SortedDictionary<int, int>();
				while (i != -1)
				{
					modeladdrs[i] = ByteConverter.ToInt32(mdlFile, address + 4);
					address += 8;
					i = ByteConverter.ToInt32(mdlFile, address);
				}

				// Load models from pointer list
				var models = new Dictionary<int, NJS_OBJECT>();
				var modelnames = new Dictionary<int, string>();
				var partnames = new List<string>();
				foreach (var item in modeladdrs)
				{
					var obj = new NJS_OBJECT(mdlFile, item.Value, 0, ModelFormat.Chunk, new Dictionary<int, Attach>());
					modelnames[item.Key] = obj.Name;
					
					if (!partnames.Contains(obj.Name))
					{
						var names = new List<string>(obj.GetObjects().Select((o) => o.Name));
						foreach (var idx in modelnames.Where(a => names.Contains(a.Value)).Select(a => a.Key))
						{
							models.Remove(idx);
						}

						models[item.Key] = obj;
						partnames.AddRange(names);
					}
				}

				// load animations
				var animfns = new Dictionary<int, string>();
				var anims = new Dictionary<int, NJS_MOTION>();
				string animmeta = null;
				foreach ((var anifilename, var anifile) in animFiles)
				{
					var processedanims = new Dictionary<int, int>();
					var ini = new MTNInfo { BigEndian = ByteConverter.BigEndian };
					
					Directory.CreateDirectory(anifilename);
					address = 0;
					i = ByteConverter.ToInt16(anifile, address);
					while (i != -1)
					{
						// This doesn't work as of November 2024. Reimplement when fixed.
						//if (mtnlabelfile != null)
						//{
						//	mtnmetadata = mtnsplitfilenames[i].Split('|'); // Filename|Description
						//	string outFilename = mtnmetadata[0];
						//	if (!mtnmetadata[0].StartsWith("NO FILE"))
						//	{
						//		animmeta = mtnmetadata[1];
						//		mtnsectionlist[i] = outFilename + "|" + animmeta;
						//	}
						//	else
						//		mtnsectionlist[i] = "NULL";
						//}
						var aniaddr = ByteConverter.ToInt32(anifile, address + 4);
						if (!processedanims.ContainsKey(aniaddr))
						{
							anims[i] = new NJS_MOTION(anifile, ByteConverter.ToInt32(anifile, address + 4), 0, ByteConverter.ToInt16(anifile, address + 2), shortRot: false, shortCheck: false);
							animfns[i] = Path.Combine(anifilename, i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim");
							anims[i].Description = animmeta;
							anims[i].ShortRot = false;
							anims[i].Save(animfns[i]);
							processedanims[aniaddr] = i;
						}
						ini.Indexes[(short)i] = "animation_" + aniaddr.ToString("X8");
						address += 8;
						i = ByteConverter.ToInt16(anifile, address);
					}
					IniSerializer.Serialize(ini, Path.Combine(anifilename, anifilename + ".ini"));
				}
				
				// Save output model files
				foreach (var model in models)
				{
					var animlist = new List<string>();
					
					foreach (var anim in anims)
					{
						if (model.Value.CountAnimated() == anim.Value.ModelParts)
						{
							var rel = animfns[anim.Key].Replace(outputFolder, string.Empty);
							if (rel.Length > 1 && rel[1] != ':')
							{
								rel = "../" + rel;
							}

							animlist.Add(rel);
						}
					}
					// This section does not work as of November 2024. Reimplement when fixed.
					// Model Labels
					//if (mdllabelfile != null)
					//{
					//	mdlmetadata = mdlsplitfilenames[model.Key].Split('|'); // Filename|Description|Texture file
					//	string outFilename = mdlmetadata[0];
					//	if (mdlsplitfilenames[model.Key] == "NULL")
					//		mdlsectionlist.Add(model.Key, "NULL");
					//	string outResult = outFilename;
					//	if (mdlmetadata.Length > 1)
					//		outResult += ("|" + mdlmetadata[1]);
					//	if (mdlmetadata.Length > 2)
					//		outResult += ("|" + mdlmetadata[2]);
					//	mdlsectionlist.Add(model.Key, outResult);
					//}
					// External motions for SAMDL Project Mode
					//if (exmtnfile != null && mtnsplitpaths.ContainsKey(model.Key))
					//{
					//	exmtndata = mtnsplitpaths[model.Key].Split(','); // Assigns external motions based on model ID
					//	animlist.AddRange(exmtndata);
					//}

					ModelFile.CreateFile(Path.Combine(Path.GetFileNameWithoutExtension(mdlFilename), model.Key.ToString(NumberFormatInfo.InvariantInfo) + ".sa2mdl"), 
						model.Value,
						animlist.ToArray(),
						null, null, null, ModelFormat.Chunk);
				}

				// labels for SAMDL Project Mode
				//if (mdllabelfile != null)
				//{
				//	string mdlsectionListFilename = Path.GetFileNameWithoutExtension(mdllabelfile) + "_data.ini";
				//	IniSerializer.Serialize(mdlsectionlist, Path.Combine(outputFolder, mdlsectionListFilename));
				//}

				//if (mtnlabelfile != null)
				//{
				//	string mtnsectionListFilename = Path.GetFileNameWithoutExtension(mtnlabelfile) + "_data.ini";
				//	IniSerializer.Serialize(mtnsectionlist, Path.Combine(outputFolder, mtnsectionListFilename));
				//}

				// Save ini file
				IniSerializer.Serialize(new MDLInfo { BigEndian = ByteConverter.BigEndian, Indexes = modelnames }, 
					Path.Combine(Path.GetFileNameWithoutExtension(mdlFilename), Path.GetFileNameWithoutExtension(mdlFilename) + ".ini"));
			}
			finally
			{
				Environment.CurrentDirectory = dir;
			}
		}

		public static void Build(bool? isBigEndian, string mdlfilename)
		{
			var dir = Environment.CurrentDirectory;
			try
			{
				mdlfilename = Path.GetFullPath(mdlfilename);
				if (Directory.Exists(mdlfilename))
				{
					mdlfilename += ".prs";
				}

				Environment.CurrentDirectory = Path.GetDirectoryName(mdlfilename);
				var models = new SortedDictionary<int, NJS_OBJECT>();
				
				foreach (var file in Directory.GetFiles(Path.GetFileNameWithoutExtension(mdlfilename), "*.sa2mdl"))
				{
					if (int.TryParse(Path.GetFileNameWithoutExtension(file), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var i))
					{
						models.Add(i, new ModelFile(file).Model);
					}
				}

				var mdlInfo = IniSerializer.Deserialize<MDLInfo>(Path.Combine(Path.GetFileNameWithoutExtension(mdlfilename), Path.GetFileNameWithoutExtension(mdlfilename) + ".ini"));
				
				ByteConverter.BigEndian = isBigEndian ?? mdlInfo.BigEndian;

				var mdlFile = new List<byte>();
				var modelBytes = new List<byte>();
				var labels = new Dictionary<string, uint>();
				var imageBase = (uint)(mdlInfo.Indexes.Count * 8) + 8;
				
				foreach (var item in models)
				{
					var tmp = item.Value.GetBytes(imageBase, false, labels, new List<uint>(), out var address);
					modelBytes.AddRange(tmp);
					imageBase += (uint)tmp.Length;
				}
				
				foreach (var item in mdlInfo.Indexes)
				{
					mdlFile.AddRange(ByteConverter.GetBytes(item.Key));
					mdlFile.AddRange(ByteConverter.GetBytes(labels[item.Value]));
				}
				
				mdlFile.AddRange([0xFF, 0xFF, 0xFF, 0xFF, 0, 0, 0, 0]);
				mdlFile.AddRange(modelBytes);
				
				if (Path.GetExtension(mdlfilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				{
					FraGag.Compression.Prs.Compress(mdlFile.ToArray(), mdlfilename);
				}
				else
				{
					File.WriteAllBytes(mdlfilename, mdlFile.ToArray());
				}
			}
			finally
			{
				Environment.CurrentDirectory = dir;
			}
		}
	}

	public class MDLInfo
	{
		public bool BigEndian { get; init; }
		[IniCollection(IniCollectionMode.IndexOnly)]
		public Dictionary<int, string> Indexes { get; init; } = new();
	}

	public class MTNInfo
	{
		public bool BigEndian { get; init; }
		[IniCollection(IniCollectionMode.IndexOnly)]
		public Dictionary<short, string> Indexes { get; init; } = new();
	}
}
