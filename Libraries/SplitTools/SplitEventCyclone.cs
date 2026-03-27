using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using PSO.PRS;
using SAModel;

namespace SplitTools.SAArc
{
	public static class sa2EventTailsPlane
	{
		static List<string> nodenames = new();
		static Dictionary<string, ModelInfo> modelfiles = new();
		static Dictionary<string, MotionInfo> motionfiles = new();
		static Dictionary<string, CameraInfo> camarrayfiles = new();
		static Dictionary<string, TexAnimFileInfo> texanimfiles = new();

		public static void Split(string filename, string outputPath, string labelFile = null)
		{
			nodenames.Clear();
			modelfiles.Clear();
			camarrayfiles.Clear();
			motionfiles.Clear();
			texanimfiles.Clear();
			
			var dir = Environment.CurrentDirectory;
			
			try
			{
				if (outputPath[outputPath.Length - 1] != '/')
				{
					outputPath = string.Concat(outputPath, "/");
				}

				// get file name, read it from the console if nothing
				var evfilename = filename;

				evfilename = Path.GetFullPath(evfilename);
				var EventFileName = Path.GetFileNameWithoutExtension(evfilename);
				if (Path.GetExtension(evfilename).Equals(".bin", StringComparison.OrdinalIgnoreCase))
				{
					EventFileName += "_bin";
				}

				Console.WriteLine("Splitting file {0}...", evfilename);
				byte[] fc;
				if (Path.GetExtension(evfilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				{
					fc = PRS.Decompress(File.ReadAllBytes(evfilename));
				}
				else
				{
					fc = File.ReadAllBytes(evfilename);
				}

				if (Path.GetExtension(evfilename).Equals(".bin", StringComparison.OrdinalIgnoreCase) && fc[0] == 0x0F && fc[1] == 0x81)
				{
					fc = PRS.Decompress(File.ReadAllBytes(evfilename));
				}

				var ini = new EventCycloneIniData { Name = Path.GetFileNameWithoutExtension(evfilename) };
				if (outputPath.Length != 0)
				{
					if (!Directory.Exists(outputPath))
					{
						Directory.CreateDirectory(outputPath);
					}

					Environment.CurrentDirectory = outputPath;
				}
				else
				{
					Environment.CurrentDirectory = Path.GetDirectoryName(evfilename);
				}

				Directory.CreateDirectory(EventFileName);

				// Metadata for SAMDL Project Mode
				byte[] mlength = null;
				var evsectionlist = new Dictionary<string, string>();
				var evsplitfilenames = new Dictionary<string, string>();
				if (labelFile != null)
				{
					labelFile = Path.GetFullPath(labelFile);
				}

				if (File.Exists(labelFile))
				{
					evsplitfilenames = IniSerializer.Deserialize<Dictionary<string, string>>(labelFile);
					mlength = File.ReadAllBytes(labelFile);
				}
				var evname = Path.GetFileNameWithoutExtension(evfilename);
				string[] evmetadata = [];

				var evtexname = Path.Combine("EVENT", evname);

				List<NJS_MOTION> motions = null;
				List<NJS_CAMERA> ncams = null;
				bool battle;
				uint key;
				if (fc[0] == 0x81)
				{
					ByteConverter.BigEndian = true;
					ini.Game = Game.SA2B;
					battle = true;
					Console.WriteLine("File is in GC/PC format.");
					key = 0x8162FE60;
				}
				else
				{
					ByteConverter.BigEndian = false;
					key = 0xCB00000;
					ini.Game = Game.SA2;
					battle = false;
					Console.WriteLine("File is in DC format.");
				}
				var ptr = fc.GetPointer(0, key);
				if (ptr != 0)
				{
					var tls = new NJS_TEXLIST(fc, ptr, key);
					ini.Texlist = GetTexlist(fc, 0, key, "tailsPlain.satex");
					var fp = Path.Combine(Path.GetFileNameWithoutExtension(evfilename), "tailsPlain.satex");
					tls.Save(fp);
					ini.Files.Add("tailsPlain.satex", HelperFunctions.FileHash(fp));
				}
				ptr = fc.GetPointer(4, key);
				string EntityName = null;
				if (ptr != 0)
				{
					ini.Model = GetModel(fc, 4, key, "tailsPlain.sa2mdl", "Tails' Cyclone (Event)");
					// populating metadata file
					string outResult = null;
					// checks if the source ini is a placeholder
					if (labelFile != null && mlength.Length != 0)
					{
						evmetadata = evsplitfilenames[modelfiles[ini.Model].Filename].Split('|'); // Description|Texture file
						outResult += evmetadata[0] + "|" + evmetadata[1];
						evsectionlist[modelfiles[ini.Model].Filename] = outResult;
						EntityName = evmetadata[0];
					}
					else
					{
						EntityName = modelfiles[ini.Model].MetaName;
						if (battle)
						{
							outResult += modelfiles[ini.Model].MetaName + "|" + $"{evtexname}TEX";
						}

						else
						{
							outResult += modelfiles[ini.Model].MetaName + "|" + $"{evtexname}";
						}

						evsectionlist[modelfiles[ini.Model].Filename] = outResult;
					}
				}
				ptr = fc.GetPointer(8, key);
				if (ptr != 0)
				{
					ini.Animation = GetMotion(fc, 8, key, "tailsPlain.saanim", motions, modelfiles[ini.Model].Model.CountAnimated(), $"{EntityName} Flying Away (Event)");
					modelfiles[ini.Model].Motions.Add(motionfiles[ini.Animation].Filename);
				}
				ptr = fc.GetPointer(0xC, key);
				if (ptr != 0)
				{
					ini.Camera = GetMotion(fc, 0xC, key, "Camera.saanim", motions, 1, $"{EntityName} Flying Away Camera Animation");
					ini.NinjaCamera = GetCamData(fc, 0xC, key, "CameraAttributes.ini", ncams);
				}
				foreach (var item in motionfiles.Values)
				{
					var fn = item.Filename;
					var fp = Path.Combine(Path.GetFileNameWithoutExtension(evfilename), fn);
					item.Motion.Save(fp);
					ini.Files.Add(fn, HelperFunctions.FileHash(fp));
				}
				foreach (var item in modelfiles.Values)
				{
					var fp = Path.Combine(Path.GetFileNameWithoutExtension(evfilename), item.Filename);
					ModelFile.CreateFile(fp, item.Model, item.Motions.ToArray(), null, null, null, item.Format);
					ini.Files.Add(item.Filename, HelperFunctions.FileHash(fp));
				}
				foreach (var item in camarrayfiles.Values)
				{
					var fn = item.Filename;
					var fp = Path.Combine(Path.GetFileNameWithoutExtension(evfilename), fn);
					item.CamData.Save(fp);
					ini.Files.Add(fn, HelperFunctions.FileHash(fp));
				}
				// Creates metadata ini file for SAMDL Project Mode
				if (labelFile != null)
				{
					var evsectionListFilename = Path.GetFileNameWithoutExtension(labelFile) + "_data.ini";
					IniSerializer.Serialize(evsectionlist, Path.Combine(outputPath, evsectionListFilename));
				}
				var js = new JsonSerializer
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore
				};
					using (var tw = File.CreateText(Path.Combine(EventFileName, Path.ChangeExtension(Path.GetFileName(evfilename), ".json"))))
						js.Serialize(tw, ini);
			}
			finally
			{
				Environment.CurrentDirectory = dir;
			}
		}

		public static void Build(bool? isBigEndian, string filename, string fileOutputPath)
		{
			nodenames.Clear();
			modelfiles.Clear();
			motionfiles.Clear();
			camarrayfiles.Clear();

			var dir = Environment.CurrentDirectory;
			try
			{
				if (fileOutputPath[fileOutputPath.Length - 1] != '/')
				{
					fileOutputPath = string.Concat(fileOutputPath, "/");
				}

				filename = Path.GetFullPath(filename);
				if (Directory.Exists(filename))
				{
					filename += ".prs";
				}

				Environment.CurrentDirectory = Path.GetDirectoryName(filename);
				var path = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filename)), Path.GetFileNameWithoutExtension(filename));
				var js = new JsonSerializer();
				EventCycloneIniData evinfo;
				using (TextReader tr = File.OpenText(Path.Combine(path, Path.ChangeExtension(Path.GetFileName(filename), ".json"))))
				using (var jtr = new JsonTextReader(tr))
					evinfo = js.Deserialize<EventCycloneIniData>(jtr);
				uint gamekey;
				if (!isBigEndian.HasValue)
				{
					ByteConverter.BigEndian = evinfo.BigEndian;
				}
				else
				{
					ByteConverter.BigEndian = isBigEndian.Value;
				}

				var evfile = new List<byte>();
				var databytes = new List<byte>();
				var animaddrs = new Dictionary<string, int>();
				var mdladdrs = new Dictionary<int, uint>();
				var panimaddrs = new Dictionary<int, int>();
				var labels = new Dictionary<string, uint>();
				if (evinfo.BigEndian)
				{
					gamekey = 0x8162FE60;
				}
				else
				{
					gamekey = 0xCB00000;
				}

				var imageBase = gamekey + 0x10;
				var tlsstart = gamekey + 0x10;
				var tex = NJS_TEXLIST.Load(Path.Combine(path, "tailsPlain.satex"));
				var texbytes = new List<byte>();
				var namebytes = new List<byte>();
				var texaddrs = new Dictionary<int, int>();
				for (var t = 0; t < tex.NumTextures; t++)
				{
					var names = tex.TextureNames[t];
					var texname = names.PadRight(28);
					namebytes.AddRange(Encoding.ASCII.GetBytes(texname));
					namebytes.AddRange(new byte[4]);
					var texaddr = 0x20 * t;
					texaddrs[t] = texaddr;
				}
				var texlistaddr = (int)(imageBase + (tex.NumTextures * 0xC));
				imageBase += (tex.NumTextures * 0xC) + 8;

				var partmdldata = new ModelFile(Path.Combine(path, "tailsPlain.sa2mdl")).Model;
				var tmpmdl = partmdldata.GetBytes(imageBase, false, labels, new List<uint>(), out var addrmdl);
				databytes.AddRange(tmpmdl);
				mdladdrs[0] = labels[partmdldata.Name];
				imageBase += (uint)tmpmdl.Length;

				var animbytes = new List<byte>();
				var anim = NJS_MOTION.Load(Path.Combine(path, "tailsPlain.saanim"));
				animbytes.AddRange(anim.GetBytes(imageBase, out var addranim));
				panimaddrs[0] = (int)(addranim + imageBase);
				databytes.AddRange(animbytes);
				imageBase += (uint)animbytes.Count;

				var camfile = NJS_CAMERA.Load(Path.Combine(path, "CameraAttributes.ini"));
				var ncambytes = new List<byte>();
				var ndata = camfile.NinjaCameraData;
				var ncamaddr = (int)imageBase;
				ncambytes.AddRange(ndata.GetBytes());
				databytes.AddRange(ncambytes);
				imageBase += (uint)ncambytes.Count;

				var canimbytes = new List<byte>();
				var camanim = NJS_MOTION.Load(Path.Combine(path, "Camera.saanim"));
				canimbytes.AddRange(camanim.GetBytes(imageBase, out var addrcam));
				animaddrs[camanim.Name] = (int)(addrcam + imageBase);
				databytes.AddRange(canimbytes);
				imageBase += (uint)canimbytes.Count;

				// Sets up NJS_CAMERA pointers
				databytes.AddRange(ByteConverter.GetBytes(ncamaddr));
				databytes.AddRange(ByteConverter.GetBytes(animaddrs[camanim.Name]));
				databytes.AddRange(namebytes);
				imageBase += 8;

				evfile.AddRange(ByteConverter.GetBytes(texlistaddr));
				evfile.AddRange(ByteConverter.GetBytes(mdladdrs[0]));
				evfile.AddRange(ByteConverter.GetBytes(panimaddrs[0]));
				evfile.AddRange(ByteConverter.GetBytes(animaddrs[camanim.Name]));
				for (var n = 0; n < tex.NumTextures; n++)
				{
					var tlsaddrs = (int)imageBase + texaddrs[n];
					evfile.AddRange(ByteConverter.GetBytes(tlsaddrs));
					evfile.AddRange(new byte[8]);
				}
				evfile.AddRange(ByteConverter.GetBytes(tlsstart));
				evfile.AddRange(ByteConverter.GetBytes(tex.NumTextures));
				evfile.AddRange(databytes);

				if (fileOutputPath.Length != 0)
				{
					if (!Directory.Exists(fileOutputPath))
					{
						Directory.CreateDirectory(fileOutputPath);
					}

					filename = Path.Combine(fileOutputPath, Path.GetFileName(filename));
				}

				if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				{ 
					File.WriteAllBytes(filename, PRS.Compress(evfile.ToArray(), 255));
					if (!File.Exists(filename))
					{
						File.Create(filename);
					}
				}
				else
				{
					File.WriteAllBytes(filename, evfile.ToArray());
				}
			}
			finally
			{
				Environment.CurrentDirectory = dir;
			}
		}

		//Get Functions
		private static string GetModel(byte[] fc, int address, uint key, string fn, string meta = null)
			{
				string name = null;
				var ptr3 = fc.GetPointer(address, key);
				if (ptr3 != 0)
				{
					name = $"object_{ptr3:X8}";
					if (!nodenames.Contains(name))
					{
						var obj = new NJS_OBJECT(fc, ptr3, key, ModelFormat.Chunk, null);
						name = obj.Name;
						var names = new List<string>(obj.GetObjects().Select(o => o.Name));
						foreach (var s in names)
							if (modelfiles.ContainsKey(s))
							{
								modelfiles.Remove(s);
							}

						nodenames.AddRange(names);
						modelfiles.Add(obj.Name, new ModelInfo(fn, obj, ModelFormat.Chunk, meta));
					}
				}
				return name;
			}

		private static string GetMotion(byte[] fc, int address, uint key, string fn, List<NJS_MOTION> motions, int cnt, string meta = null)
		{
			NJS_MOTION mtn = null;
			if (motions != null)
			{
				mtn = motions[ByteConverter.ToInt32(fc, address)];
			}
			else
			{
				var ptr3 = fc.GetPointer(address, key);
				if (ptr3 != 0)
				{
					mtn = new NJS_MOTION(fc, ptr3, key, cnt)
					{
						Description = meta
					};
					mtn.OptimizeShape();
				}
			}
			if (mtn == null)
			{
				return null;
			}

			if (!motionfiles.ContainsKey(mtn.Name) || motionfiles[mtn.Name].Filename == null)
			{
				motionfiles[mtn.Name] = new MotionInfo(fn, mtn);
			}

			return mtn.Name;
		}

		private static string GetTexlist(byte[] fc, int address, uint key, string fn)
		{
			string name = null;
			var ptr3 = fc.GetPointer(address, key);
			if (ptr3 != 0)
			{
				name = $"texlist_{ptr3:X8}";
			}
			return name;
		}

		private static string GetTexSizes(byte[] fc, int address, uint key, string fn)
		{
			string name = null;
			var ptr3 = fc.GetPointer(address, key);
			if (ptr3 != 0)
			{
				name = $"texsizes_{ptr3:X8}";
			}
			return name;
		}

		private static string GetReflectData(byte[] fc, int address, uint key, string fn)
		{
			string name = null;
			var ptr3 = fc.GetPointer(address, key);
			if (ptr3 != 0)
			{
				name = $"matrix_{ptr3:X8}";
			}
			return name;
		}

		private static string GetCamData(byte[] fc, int address, uint key, string fn, List<NJS_CAMERA> ncams)
		{
			NJS_CAMERA ncam = null;
			if (ncams != null)
			{
				ncam = ncams[ByteConverter.ToInt32(fc, address + 0xC)];
			}
			else
			{
				var ptr3 = fc.GetPointer(address, key);
				if (ptr3 != 0)
				{
					ncam = new NJS_CAMERA(fc, ptr3 + 0xC, key);
				}
			}
			if (ncam == null)
			{
				return null;
			}

			if (!camarrayfiles.ContainsKey(ncam.Name) || camarrayfiles[ncam.Name].Filename == null)
			{
				camarrayfiles[ncam.Name] = new CameraInfo(fn, ncam);
			}

			return ncam.Name;
		}
	}
	public class EventCycloneIniData
	{
		public string Name { get; set; }
		[JsonIgnore]
		public Game Game { get; set; }
		[JsonProperty(PropertyName = "Game")]
		public string GameString
		{
			get => Game.ToString();
			set => Game = Enum.Parse<Game>(value);
		}
		public bool BigEndian { get; set; }
		public Dictionary<string, string> Files { get; set; } = new();
		public string Texlist { get; set; }
		public string Model { get; set; }
		public string Animation { get; set; }
		public string Camera { get; set; }
		public string NinjaCamera { get; set; }
	}
}
