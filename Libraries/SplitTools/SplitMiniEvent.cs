using FraGag.Compression;
using Newtonsoft.Json;
using SAModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SplitTools.SAArc
{
	public static class SA2MiniEvent
	{
		static List<string> nodenames = new List<string>();
		static Dictionary<string, MEModelInfo> modelfiles = new Dictionary<string, MEModelInfo>();
		static Dictionary<string, MEMotionInfo> motionfiles = new Dictionary<string, MEMotionInfo>();

		public static void Split(string filename, string outputPath)
		{
			nodenames.Clear();
			modelfiles.Clear();
			motionfiles.Clear();
			string dir = Environment.CurrentDirectory;
			try
			{
				if (outputPath[outputPath.Length - 1] != '/') outputPath = string.Concat(outputPath, "/");
				// get file name, read it from the console if nothing
				string evfilename = filename;
				evfilename = Path.GetFullPath(evfilename);

				Console.WriteLine("Splitting file {0}...", evfilename);
				byte[] fc;
				if (Path.GetExtension(evfilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
					fc = Prs.Decompress(evfilename);
				else
					fc = File.ReadAllBytes(evfilename);
				MiniEventIniData ini = new MiniEventIniData() { Name = Path.GetFileNameWithoutExtension(evfilename) };
				if (outputPath.Length != 0)
				{
					if (!Directory.Exists(outputPath))
						Directory.CreateDirectory(outputPath);
					Environment.CurrentDirectory = outputPath;
				}
				else
					Environment.CurrentDirectory = Path.GetDirectoryName(evfilename);
				Directory.CreateDirectory(Path.GetFileNameWithoutExtension(evfilename));
				uint key;
				List<NJS_MOTION> motions = null;
				if (fc[4] == 0x81)
				{
					Console.WriteLine("File is in GC/PC format.");
					ByteConverter.BigEndian = true;
					key = 0x816DFE60;
					ini.Game = Game.SA2B;
				}
				else
				{
					Console.WriteLine("File is in DC format.");
					ByteConverter.BigEndian = false;
					key = 0xCB00000;
					ini.Game = Game.SA2;
				}
				int address;
				for (int i = 0; i < 8; i++)
				{
					address = 8 + (4 * i);
					int ptr = fc.GetPointer(address, key);
					string chnm = null;
					switch (i)
					{
						case 0:
							chnm = "Sonic";
							break;
						case 1:
							chnm = "Shadow";
							break;
						case 2:
							chnm = "Tails";
							break;
						case 3:
							chnm = "Eggman";
							break;
						case 4:
							chnm = "Knuckles";
							break;
						case 5:
							chnm = "Rouge";
							break;
						case 6:
							chnm = "Mech Tails";
							break;
						case 7:
							chnm = "Mech Eggman";
							break;
					}
					if (ptr != 0)
					{
						Console.WriteLine($"{chnm} is in this Mini-Event");
						Directory.CreateDirectory(Path.Combine(Path.GetFileNameWithoutExtension(evfilename), $"{chnm}"));
						MiniEventMaster data = new MiniEventMaster();
						switch (i)
						{
							case 0:
							case 1:
							case 2:
							case 4:
							case 5:
								data.BodyAnims = GetMotion(fc, ptr, key, $"{chnm}\\Body.saanim", motions, 62);
								break;
							case 3:
								data.BodyAnims = GetMotion(fc, ptr, key, $"{chnm}\\Body.saanim", motions, 45);
								break;
							case 6:
							case 7:
								data.BodyAnims = GetMotion(fc, ptr, key, $"{chnm}\\Body.saanim", motions, 33);
								break;
						}
						int address2;
						for (int j = 0; j < 4; j++)
						{
							string prnm = null;
							switch (j)
							{
								case 0:
									prnm = "Head";
									break;
								case 1:
									prnm = "Mouth";
									break;
								case 2:
									prnm = "LeftHand";
									break;
								case 3:
									prnm = "RightHand";
									break;
							}
							address2 = ptr + 4 + (0xC * j);
							int ptr2 = fc.GetPointer(address2, key);
							if (ptr2 != 0)
							{
								MiniEventParts parts = new MiniEventParts();
								parts.Model = GetModel(fc, address2, key, $"{chnm}\\{prnm}.sa2mdl");
								if (parts.Model != null)
								{
									parts.Anims = GetMotion(fc, address2 + 4, key, $"{chnm}\\{prnm}.saanim", motions, modelfiles[parts.Model].Model.CountAnimated());
									if (parts.Anims != null)
										modelfiles[parts.Model].Motions.Add($"{prnm}.saanim");
									parts.ShapeMotions = GetMotion(fc, address2 + 8, key, $"{chnm}\\{prnm}Shape.saanim", motions, modelfiles[parts.Model].Model.CountMorph());
									if (parts.ShapeMotions != null)
										modelfiles[parts.Model].Motions.Add($"{prnm}Shape.saanim");
								}
								data.Parts.Add(parts);
							}
						}
						ini.MainData.Add(data);
					}
				}
				int cam = fc.GetPointer(4, key);
				if (cam != 0)
				{
					int ncam = fc.GetPointer(cam + 0xC, key);
					ini.Camera = GetMotion(fc, 4, key, $"Camera.saanim", motions, 1);
					NinjaCamera nincam = new NinjaCamera(fc, ncam);
					ini.NinjaCam = GetNinjaCam(fc, cam + 0xC, key, "CameraAttributes.ini");
					string fp = Path.Combine(Path.GetFileNameWithoutExtension(evfilename), "CameraAttributes.ini");
					nincam.Save(fp);
					ini.Files.Add("CameraAttributes.ini", HelperFunctions.FileHash(fp));
				}
				else
					Console.WriteLine("Mini-Event does not contain a camera.");
				ini.CharacterFlags = (SA2CharacterFlags)ByteConverter.ToInt32(fc, 0);
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
				JsonSerializer js = new JsonSerializer
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore
				};
				using (var tw = File.CreateText(Path.Combine(Path.GetFileNameWithoutExtension(evfilename), Path.ChangeExtension(Path.GetFileName(filename), ".json"))))
					js.Serialize(tw, ini);
			}
			finally
			{
				Environment.CurrentDirectory = dir;
			}
		}

			public static void Build(string filename)
		{
			nodenames.Clear();
			modelfiles.Clear();
			motionfiles.Clear();

			byte[] fc;
			if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				fc = Prs.Decompress(filename);
			else
				fc = File.ReadAllBytes(filename);
			string path = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filename)), Path.GetFileNameWithoutExtension(filename));
			JsonSerializer js = new JsonSerializer();
			MiniEventIniData ini;
			using (TextReader tr = File.OpenText(Path.Combine(path, Path.ChangeExtension(Path.GetFileName(filename), ".json"))))
			using (JsonTextReader jtr = new JsonTextReader(tr))
				ini = js.Deserialize<MiniEventIniData>(jtr);
			uint key;
			if (fc[4] == 0x81)
			{
				ByteConverter.BigEndian = true;
				key = 0x816DFE60;
			}
			else
			{
				ByteConverter.BigEndian = false;
				key = 0xCB00000;
			}
			List<byte> modelbytes = new List<byte>(fc);
			Dictionary<string, uint> labels = new Dictionary<string, uint>();
			foreach (string file in ini.Files.Where(a => a.Key.EndsWith(".sa2mdl", StringComparison.OrdinalIgnoreCase) && HelperFunctions.FileHash(Path.Combine(path, a.Key)) != a.Value).Select(a => a.Key))
				modelbytes.AddRange(new ModelFile(Path.Combine(path, file)).Model.GetBytes((uint)(key + modelbytes.Count), false, labels, new List<uint>(), out uint _));
			foreach (string file in ini.Files.Where(a => a.Key.EndsWith(".saanim", StringComparison.OrdinalIgnoreCase) && HelperFunctions.FileHash(Path.Combine(path, a.Key)) != a.Value).Select(a => a.Key))
				modelbytes.AddRange(NJS_MOTION.Load(Path.Combine(path, file)).GetBytes((uint)(key + modelbytes.Count), labels, out uint _));
			fc = modelbytes.ToArray();
			int address;
			for (int i = 0; i < 8; i++)
			{
				address = 8 + (4 * i);
				int ptr = fc.GetPointer(address, key);
				if (ptr != 0)
				{
					MiniEventMaster info = ini.MainData[i];
					if (info.BodyAnims != null)
					{
						if (labels.ContainsKeySafer(info.BodyAnims))
							ByteConverter.GetBytes(labels[info.BodyAnims]).CopyTo(fc, ptr);
						int address2;
						for (int j = 0; j < 4; j++)
						{
							address2 = ptr + 4 + (0xC * j);
							int ptr2 = fc.GetPointer(address2, key);
							if (ptr2 != 0)
							{
								MiniEventParts parts = ini.MainData[i].Parts[j];
								if (info.Parts != null)
								{
									if (labels.ContainsKeySafer(info.Parts[j].Model))
										ByteConverter.GetBytes(labels[info.Parts[j].Model]).CopyTo(fc, address2);
									if (labels.ContainsKeySafer(info.Parts[j].Anims))
										ByteConverter.GetBytes(labels[info.Parts[j].Anims]).CopyTo(fc, address2 + 4);
									if (labels.ContainsKeySafer(info.Parts[j].ShapeMotions))
										ByteConverter.GetBytes(labels[info.Parts[j].ShapeMotions]).CopyTo(fc, address2 + 8);
								}
							}
						}
					}
				}
			}
			int cam = fc.GetPointer(4, key);
			if (cam != 0 && labels.ContainsKeySafer(ini.Camera))
			{
				ByteConverter.GetBytes(labels[ini.Camera]).CopyTo(fc, 4);
				ByteConverter.GetBytes(labels[ini.Camera]).CopyTo(fc, cam + 0x10);
			}
			if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				Prs.Compress(fc, filename);
			else
				File.WriteAllBytes(filename, fc);
		}

		//Get Functions
		private static string GetModel(byte[] fc, int address, uint key, string fn)
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
					modelfiles.Add(obj.Name, new MEModelInfo(fn, obj, ModelFormat.Chunk));
				}
			}
			return name;
		}

		private static string GetMotion(byte[] fc, int address, uint key, string fn, List<NJS_MOTION> motions, int cnt)
		{
			NJS_MOTION mtn = null;
			if (motions != null)
				mtn = motions[ByteConverter.ToInt32(fc, address)];
			else
			{
				int ptr3 = fc.GetPointer(address, key);
				if (ptr3 != 0)
					mtn = new NJS_MOTION(fc, ptr3, key, cnt);
			}
			if (mtn == null) return null;
			if (!motionfiles.ContainsKey(mtn.Name) || motionfiles[mtn.Name].Filename == null)
				motionfiles[mtn.Name] = new MEMotionInfo(fn, mtn);
			return mtn.Name;
		}

		private static string GetNinjaCam(byte[] fc, int address, uint key, string fn)
		{
			string name = null;
			int ptr3 = fc.GetPointer(address, key);
			if (ptr3 != 0)
			{
				name = $"ninjacam_{ptr3:X8}";
			}
			return name;
		}

		public static bool ContainsKeySafer<TValue>(this IDictionary<string, TValue> dict, string key)
		{
			return key != null && dict.ContainsKey(key);
		}
	}

	public class MEModelInfo
	{
		public string Filename { get; set; }
		public NJS_OBJECT Model { get; set; }
		public ModelFormat Format { get; set; }
		public List<string> Motions { get; set; } = new List<string>();

		public MEModelInfo(string fn, NJS_OBJECT obj, ModelFormat format)
		{
			Filename = fn;
			Model = obj;
			Format = format;
		}
	}

	public class MEMotionInfo
	{
		public string Filename { get; set; }
		public NJS_MOTION Motion { get; set; }

		public MEMotionInfo(string fn, NJS_MOTION mtn)
		{
			Filename = fn;
			Motion = mtn;
		}
	}

	public class MiniEventIniData
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
		public Dictionary<string, string> Files { get; set; } = new Dictionary<string, string>();

		public SA2CharacterFlags CharacterFlags { get; set; }
		public string Camera { get; set; }
		public string NinjaCam { get; set; }
		public List<MiniEventMaster> MainData { get; set; } = new List<MiniEventMaster>();
		public List<string> Motions { get; set; }
	}

	public class MiniEventMaster
	{
		public string BodyAnims { get; set; }
		public List<MiniEventParts> Parts { get; set; } = new List<MiniEventParts>();
	}

	public class MiniEventParts
	{
		public string Model { get; set; }
		public string Anims { get; set; }
		public string ShapeMotions { get; set; }
	}
}