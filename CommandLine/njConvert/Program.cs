using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SAModel;
using SplitTools.Split;
using Newtonsoft.Json;

namespace njConvert
{
	class Program
	{
		static int nodeCount = 0;
		static bool cleanup = false;
		static bool jsConv = false;

		static void ProcessTexList(List<string> names, string path)
		{
			Console.WriteLine("Saving texture list");
			string tlsName = Path.GetFileNameWithoutExtension(path) + ".tls";
			StreamWriter file = File.CreateText(Path.Combine(Path.GetDirectoryName(path), tlsName));

			foreach (string name in names)
				file.WriteLine(name + ".pvr");
			file.Close();
		}

		static void ProcessNinjaMotionFile(byte[] file, int addr, string path)
		{
			bool lost = false;
			int njSize = ByteConverter.ToInt32(file, 4);
			byte[] njFile = new byte[njSize];
			Array.Copy(file, addr, njFile, 0, njFile.Length);

			if (nodeCount == 0)
			{
				int mdataptr = ByteConverter.ToInt32(njFile, 0);
				AnimFlags animtype = (AnimFlags)ByteConverter.ToUInt16(njFile, 8);

				int mdataSize = njFile.Length - mdataptr;

				int mdata = 0;
				int mdSize = 0;
				if (animtype.HasFlag(AnimFlags.Position)) mdata++;
				if (animtype.HasFlag(AnimFlags.Rotation)) mdata++;
				if (animtype.HasFlag(AnimFlags.Scale)) mdata++;
				if (animtype.HasFlag(AnimFlags.Vector)) mdata++;
				if (animtype.HasFlag(AnimFlags.Vertex)) mdata++;
				if (animtype.HasFlag(AnimFlags.Normal)) mdata++;
				if (animtype.HasFlag(AnimFlags.Color)) mdata++;
				if (animtype.HasFlag(AnimFlags.Intensity)) mdata++;
				if (animtype.HasFlag(AnimFlags.Target)) mdata++;
				if (animtype.HasFlag(AnimFlags.Spot)) mdata++;
				if (animtype.HasFlag(AnimFlags.Point)) mdata++;
				if (animtype.HasFlag(AnimFlags.Roll)) mdata++;

				switch (mdata)
				{
					case 1:
					case 2:
						mdSize = 16;
						break;
					case 3:
						mdSize = 24;
						break;
					case 4:
						mdSize = 32;
						break;
					case 5:
						mdSize = 40;
						break;
					default:
						lost = true;
						break;
				}

				if (mdSize > 0)
					nodeCount = mdataSize / mdSize;
			}

			if (!lost)
			{
				NJS_MOTION mot = new NJS_MOTION(njFile, 0, 0, nodeCount);
				string filename = path + ".saanim";
				mot.Save(filename, true);

				if (cleanup)
					File.Delete(path);
			}
			else
				Console.WriteLine("Cannot determine node count for motion file {0}", Path.GetFileName(path));
		}

		static void ProcessNinjaFile(byte[] file, int addr, bool basic, string path)
		{
			int njSize = ByteConverter.ToInt32(file, (addr-4));
			byte[] njFile = new byte[njSize];
			Array.Copy(file, addr, njFile, 0, njFile.Length);
			int format = (int)ModelFormat.Chunk;
			
			string ext = "";

			if (basic)
			{
				format = (int)ModelFormat.Basic;
				ext = ".sa1mdl";
			}
			else
				ext = ".sa2mdl";

			NJS_OBJECT obj = new NJS_OBJECT(njFile, 0, 0, (ModelFormat)format, new Dictionary<int, Attach>());
			nodeCount = obj.CountAnimated();
			string filename = path + ext;
			ModelFile.CreateFile(filename, obj, null, null, null, null, (ModelFormat)format);

			LoadActionFile(path);
			if (cleanup)
				File.Delete(path);
		}

		static void LoadActionFile(string path)
		{
			string rootPath = Path.GetDirectoryName(path);
			string actPath = Path.Combine(rootPath, (Path.GetFileNameWithoutExtension(path) + ".action"));
			List<string> newNames = new List<string>();
			List<string> saanimFiles = new List<string>();

			if (File.Exists(actPath))
			{
				Console.WriteLine("Related .action file found, processing motions...");
				string[] files = File.ReadAllLines(actPath);

				if (files.Length > 0)
				{
					for (int l = 0; l < files.Length; l++)
					{
						string filename = Path.Combine(rootPath, files[l]);
						byte[] njmFile = File.ReadAllBytes(filename);
						newNames.Add(files[l] + ".saanim");
						saanimFiles.Add(filename + ".saanim");
						ProcessFile(njmFile, filename);
					}
				}

				TextWriter newActionFile = File.CreateText(path + ".action");
				foreach (string animName in newNames)
					newActionFile.WriteLine(animName);
				newActionFile.Close();

				if (jsConv && saanimFiles.Count != 0)
				{
					JsonSerializer js = new JsonSerializer() { Culture = System.Globalization.CultureInfo.InvariantCulture };
					foreach (string saanim in saanimFiles)
					{
						if (File.Exists(saanim))
						{
							using (TextWriter tw = File.CreateText(Path.ChangeExtension(saanim, ".json")))
							using (JsonTextWriter jtw = new JsonTextWriter(tw) { Formatting = Formatting.Indented })
								js.Serialize(jtw, NJS_MOTION.Load(saanim));
						}
					}
				}
			}
			else
				Console.WriteLine("No action file exists.");
		}

		static void ProcessFile(byte[] file, string path)
		{
			List<string> texNames = new List<string>();
			ByteConverter.BigEndian = SplitTools.HelperFunctions.CheckBigEndianInt32(file, 0x8);

			int ninjaDataOffset = 0;
			bool basicModel = false;
			bool motFile = false;

			string magic = System.Text.Encoding.UTF8.GetString(BitConverter.GetBytes(BitConverter.ToInt32(file, 0)));

			switch (magic)
			{
				case "GJTL":
				case "NJTL":
					int POF0Offset = ByteConverter.ToInt32(file, 0x4) + 0x8;
					int POF0Size = ByteConverter.ToInt32(file, POF0Offset + 0x4);
					int texListOffset = POF0Offset + POF0Size + 0x8;
					ninjaDataOffset = texListOffset + 0x8;
					//Get Texture Listings for if SA Tools gets that implemented
					int texCount = ByteConverter.ToInt32(file, 0xC);
					int texOffset = 0;

					for (int i = 0; i < texCount; i++)
					{
						int textAddress = ByteConverter.ToInt32(file, texOffset + 0x10) + 0x8;

						// Read null terminated string
						List<byte> namestring = new List<byte>();
						byte namechar = (file[textAddress]);
						int j = 0;
						while (namechar != 0)
						{
							namestring.Add(namechar);
							j++;
							namechar = (file[textAddress + j]);
						}
						texNames.Add(System.Text.Encoding.ASCII.GetString(namestring.ToArray()));

						texOffset += 0xC;
					}
					break;

				case "GJCM":
				case "NJCM":
					ninjaDataOffset = 0x8;
					break;

				case "NJBM":
					ninjaDataOffset = 0x8;
					basicModel = true;
					break;

				case "NMDM":
					ninjaDataOffset = 0x8;
					motFile = true;
					break;

				default:
					Console.WriteLine("Incorrect format!");
					return;
			}

			if (!motFile)
			{
				Console.WriteLine("Ninja Model found, processing {0}", Path.GetFileName(path));
				if (texNames.Count > 0)
					ProcessTexList(texNames, path);
				ProcessNinjaFile(file, ninjaDataOffset, basicModel, path);
			}
			else
			{
				Console.WriteLine("Ninja Motion found, processing {0}", Path.GetFileName(path));
				ProcessNinjaMotionFile(file, ninjaDataOffset, path);
			}
		}

		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("Ninja Converter");
				Console.WriteLine("This tool was designed to convert Ninja or Ninja Motion files to sa2mdl and saanim files respectively.\n");
				Console.WriteLine("Usage:");
				Console.WriteLine("njConvert <filename> [optional arguments]");
				Console.WriteLine("Supported files:\n(*.nj) - Ninja File\n(*.njm) - Ninja Motion File");
				Console.WriteLine("Optional Arguments:\n-cleanup || -c - Deletes source files.\n-json || -js - Converts produced saanim files to .json.\n");
				Console.WriteLine("If a supplied Ninja File has a .action file associated with it, the .njm files will be converted at the same time.");
			}
			else
			{
				if (File.Exists(args[0]))
				{
					Console.WriteLine(args.Length.ToString());
					byte[] file = File.ReadAllBytes(args[0]);
					string path = Path.GetFullPath(args[0]);

					for (int u = 1; u < args.Length; u++)
					{
						switch (args[u])
						{
							case "-cleanup":
							case "-c":
								cleanup = true;
								break;
							case "-json":
							case "-js":
								jsConv = true;
								break;
						}
					}
					Console.WriteLine(Path.GetFileName(path));
					ProcessFile(file, path);
				}
			}
		}
	}
}
