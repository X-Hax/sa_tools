using FraGag.Compression;
using Newtonsoft.Json;
using SAModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Web.Services.Description;
using static System.Windows.Forms.Design.AxImporter;

namespace SplitTools.SAArc
{
	public static class sa2EventTailsPlane
	{
		static List<string> nodenames = new List<string>();
		static Dictionary<string, ModelInfo> modelfiles = new Dictionary<string, ModelInfo>();
		static Dictionary<string, MotionInfo> motionfiles = new Dictionary<string, MotionInfo>();
		static Dictionary<string, CameraInfo> camarrayfiles = new Dictionary<string, CameraInfo>();
		static Dictionary<string, TexAnimFileInfo> texanimfiles = new Dictionary<string, TexAnimFileInfo>();

		public static void Split(string filename, string outputPath, string labelFile = null)
		{
			nodenames.Clear();
			modelfiles.Clear();
			camarrayfiles.Clear();
			motionfiles.Clear();
			texanimfiles.Clear();
			string dir = Environment.CurrentDirectory;
			try
			{
				if (outputPath[outputPath.Length - 1] != '/') outputPath = string.Concat(outputPath, "/");
				// get file name, read it from the console if nothing
				string evfilename = filename;

				evfilename = Path.GetFullPath(evfilename);
				string EventFileName = Path.GetFileNameWithoutExtension(evfilename);
				if (Path.GetExtension(evfilename).Equals(".bin", StringComparison.OrdinalIgnoreCase))
					EventFileName += "_bin";

				Console.WriteLine("Splitting file {0}...", evfilename);
				byte[] fc;
				if (Path.GetExtension(evfilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
					fc = Prs.Decompress(evfilename);
				else
					fc = File.ReadAllBytes(evfilename);
				if (Path.GetExtension(evfilename).Equals(".bin", StringComparison.OrdinalIgnoreCase) && fc[0] == 0x0F && fc[1] == 0x81)
					fc = Prs.Decompress(evfilename);
				EventCycloneIniData ini = new EventCycloneIniData() { Name = Path.GetFileNameWithoutExtension(evfilename) };
				if (outputPath.Length != 0)
				{
					if (!Directory.Exists(outputPath))
						Directory.CreateDirectory(outputPath);
					Environment.CurrentDirectory = outputPath;
				}
				else
					Environment.CurrentDirectory = Path.GetDirectoryName(evfilename);
					Directory.CreateDirectory(EventFileName);

				// Metadata for SAMDL Project Mode
				byte[] mlength = null;
				Dictionary<string, string> evsectionlist = new Dictionary<string, string>();
				Dictionary<string, string> evsplitfilenames = new Dictionary<string, string>();
				if (labelFile != null) labelFile = Path.GetFullPath(labelFile);
				if (File.Exists(labelFile))
				{
					evsplitfilenames = IniSerializer.Deserialize<Dictionary<string, string>>(labelFile);
					mlength = File.ReadAllBytes(labelFile);
				}
				string evname = Path.GetFileNameWithoutExtension(evfilename);
				string[] evmetadata = new string[0];

				string evtexname = Path.Combine("EVENT", evname);

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
				int ptr = fc.GetPointer(0, key);
				if (ptr != 0)
				{
					NJS_TEXLIST tls = new NJS_TEXLIST(fc, ptr, key);
					ini.Texlist = GetTexlist(fc, 0, key, "tailsPlain.satex");
					string fp = Path.Combine(Path.GetFileNameWithoutExtension(evfilename), "tailsPlain.satex");
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
							outResult += modelfiles[ini.Model].MetaName + "|" + $"{evtexname}TEX";
						
						else
							outResult += modelfiles[ini.Model].MetaName + "|" + $"{evtexname}";
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
					string fn = item.Filename;
					string fp = Path.Combine(Path.GetFileNameWithoutExtension(evfilename), fn);
					item.Motion.Save(fp);
					ini.Files.Add(fn, HelperFunctions.FileHash(fp));
				}
				foreach (var item in modelfiles.Values)
				{
					string fp = Path.Combine(Path.GetFileNameWithoutExtension(evfilename), item.Filename);
					ModelFile.CreateFile(fp, item.Model, item.Motions.ToArray(), null, null, null, item.Format);
					ini.Files.Add(item.Filename, HelperFunctions.FileHash(fp));
				}
				foreach (var item in camarrayfiles.Values)
				{
					string fn = item.Filename;
					string fp = Path.Combine(Path.GetFileNameWithoutExtension(evfilename), fn);
					item.CamData.Save(fp);
					ini.Files.Add(fn, HelperFunctions.FileHash(fp));
				}
				// Creates metadata ini file for SAMDL Project Mode
				if (labelFile != null)
				{
					string evsectionListFilename = Path.GetFileNameWithoutExtension(labelFile) + "_data.ini";
					IniSerializer.Serialize(evsectionlist, Path.Combine(outputPath, evsectionListFilename));
				}
				JsonSerializer js = new JsonSerializer
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

			string dir = Environment.CurrentDirectory;
			try
			{
				if (fileOutputPath[fileOutputPath.Length - 1] != '/') fileOutputPath = string.Concat(fileOutputPath, "/");
				filename = Path.GetFullPath(filename);
				if (Directory.Exists(filename))
					filename += ".prs";
				Environment.CurrentDirectory = Path.GetDirectoryName(filename);
				string path = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filename)), Path.GetFileNameWithoutExtension(filename));
				JsonSerializer js = new JsonSerializer();
				EventCycloneIniData evinfo;
				using (TextReader tr = File.OpenText(Path.Combine(path, Path.ChangeExtension(Path.GetFileName(filename), ".json"))))
				using (JsonTextReader jtr = new JsonTextReader(tr))
					evinfo = js.Deserialize<EventCycloneIniData>(jtr);
				uint gamekey;
				if (!isBigEndian.HasValue)
					ByteConverter.BigEndian = evinfo.BigEndian;
				else
					ByteConverter.BigEndian = isBigEndian.Value;
				List<byte> evfile = new List<byte>();
				List<byte> databytes = new List<byte>();
				Dictionary<string, int> animaddrs = new Dictionary<string, int>();
				Dictionary<int, uint> mdladdrs = new Dictionary<int, uint>();
				Dictionary<int, int> panimaddrs = new Dictionary<int, int>();
				Dictionary<string, uint> labels = new Dictionary<string, uint>();
				if (evinfo.BigEndian == true)
					gamekey = 0x8162FE60;
				else
					gamekey = 0xCB00000;
				uint imageBase = gamekey + 0x10;
				uint tlsstart = gamekey + 0x10;
				NJS_TEXLIST tex = NJS_TEXLIST.Load(Path.Combine(path, "tailsPlain.satex"));
				List<byte> texbytes = new List<byte>();
				List<byte> namebytes = new List<byte>();
				Dictionary<int, int> texaddrs = new Dictionary<int, int>();
				for (int t = 0; t < tex.NumTextures; t++)
				{
					string names = tex.TextureNames[t];
					string texname = names.PadRight(28);
					namebytes.AddRange(Encoding.ASCII.GetBytes(texname));
					namebytes.AddRange(new byte[4]);
					int texaddr = 0x20 * t;
					texaddrs[t] = texaddr;
				}
				int texlistaddr = (int)(imageBase + (tex.NumTextures * 0xC));
				imageBase += (uint)((tex.NumTextures * 0xC) + 8);

				NJS_OBJECT partmdldata = new ModelFile(Path.Combine(path, "tailsPlain.sa2mdl")).Model;
				byte[] tmpmdl = partmdldata.GetBytes(imageBase, false, labels, new List<uint>(), out uint addrmdl);
				databytes.AddRange(tmpmdl);
				mdladdrs[0] = labels[partmdldata.Name];
				imageBase += (uint)tmpmdl.Length;

				List<byte> animbytes = new List<byte>();
				NJS_MOTION anim = NJS_MOTION.Load(Path.Combine(path, "tailsPlain.saanim"));
				animbytes.AddRange(anim.GetBytes(imageBase, out uint addranim));
				panimaddrs[0] = (int)(addranim + imageBase);
				databytes.AddRange(animbytes);
				imageBase += (uint)animbytes.Count;

				NJS_CAMERA camfile = NJS_CAMERA.Load(Path.Combine(path, "CameraAttributes.ini"));
				List<byte> ncambytes = new List<byte>();
				NinjaCamera ndata = camfile.NinjaCameraData;
				int ncamaddr = (int)imageBase;
				ncambytes.AddRange(ndata.GetBytes());
				databytes.AddRange(ncambytes);
				imageBase += (uint)ncambytes.Count;

				List<byte> canimbytes = new List<byte>();
				NJS_MOTION camanim = NJS_MOTION.Load(Path.Combine(path, "Camera.saanim"));
				canimbytes.AddRange(camanim.GetBytes(imageBase, out uint addrcam));
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
				for (int n = 0; n < tex.NumTextures; n++)
				{
					int tlsaddrs = (int)imageBase + texaddrs[n];
					evfile.AddRange(ByteConverter.GetBytes(tlsaddrs));
					evfile.AddRange(new byte[8]);
				}
				evfile.AddRange(ByteConverter.GetBytes(tlsstart));
				evfile.AddRange(ByteConverter.GetBytes(tex.NumTextures));
				evfile.AddRange(databytes);

				if (fileOutputPath.Length != 0)
				{
					if (!Directory.Exists(fileOutputPath))
						Directory.CreateDirectory(fileOutputPath);
					filename = Path.Combine(fileOutputPath, Path.GetFileName(filename));
				}

				if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				{ 
					FraGag.Compression.Prs.Compress(evfile.ToArray(), filename);
					if (!File.Exists(filename))
						File.Create(filename);
				}
				else
					File.WriteAllBytes(filename, evfile.ToArray());
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
				int ptr3 = fc.GetPointer(address, key);
				if (ptr3 != 0)
				{
					name = $"object_{ptr3:X8}";
					if (!nodenames.Contains(name))
					{
						NJS_OBJECT obj = new NJS_OBJECT(fc, ptr3, key, ModelFormat.Chunk, null);
						name = obj.Name;
						List<string> names = new List<string>(obj.GetObjects().Select((o) => o.Name));
						foreach (string s in names)
							if (modelfiles.ContainsKey(s))
								modelfiles.Remove(s);
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
				mtn = motions[ByteConverter.ToInt32(fc, address)];
			else
			{
				int ptr3 = fc.GetPointer(address, key);
				if (ptr3 != 0)
				{
					mtn = new NJS_MOTION(fc, ptr3, key, cnt);
					mtn.Description = meta;
					mtn.OptimizeShape();
				}
			}
			if (mtn == null) return null;
			if (!motionfiles.ContainsKey(mtn.Name) || motionfiles[mtn.Name].Filename == null)
				motionfiles[mtn.Name] = new MotionInfo(fn, mtn);
			return mtn.Name;
		}

		private static string GetTexlist(byte[] fc, int address, uint key, string fn)
		{
			string name = null;
			int ptr3 = fc.GetPointer(address, key);
			if (ptr3 != 0)
			{
				name = $"texlist_{ptr3:X8}";
			}
			return name;
		}

		private static string GetTexSizes(byte[] fc, int address, uint key, string fn)
		{
			string name = null;
			int ptr3 = fc.GetPointer(address, key);
			if (ptr3 != 0)
			{
				name = $"texsizes_{ptr3:X8}";
			}
			return name;
		}

		private static string GetReflectData(byte[] fc, int address, uint key, string fn)
		{
			string name = null;
			int ptr3 = fc.GetPointer(address, key);
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
				ncam = ncams[ByteConverter.ToInt32(fc, address + 0xC)];
			else
			{
				int ptr3 = fc.GetPointer(address, key);
				if (ptr3 != 0)
				{
					ncam = new NJS_CAMERA(fc, ptr3 + 0xC, key);
				}
			}
			if (ncam == null) return null;
			if (!camarrayfiles.ContainsKey(ncam.Name) || camarrayfiles[ncam.Name].Filename == null)
				camarrayfiles[ncam.Name] = new CameraInfo(fn, ncam);
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
			get { return Game.ToString(); }
			set { Game = (Game)Enum.Parse(typeof(Game), value); }
		}
		public bool BigEndian { get; set; }
		public Dictionary<string, string> Files { get; set; } = new Dictionary<string, string>();
		public string Texlist { get; set; }
		public string Model { get; set; }
		public string Animation { get; set; }
		public string Camera { get; set; }
		public string NinjaCamera { get; set; }
	}
}
