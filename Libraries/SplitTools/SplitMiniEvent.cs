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
		static Dictionary<string, ModelInfo> modelfiles = new Dictionary<string, ModelInfo>();
		static Dictionary<string, MotionInfo> motionfiles = new Dictionary<string, MotionInfo>();
		static Dictionary<string, CameraInfo> camarrayfiles = new Dictionary<string, CameraInfo>();

		public static void Split(string filename, string outputPath)
		{
			nodenames.Clear();
			modelfiles.Clear();
			motionfiles.Clear();
			camarrayfiles.Clear();
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
				List<NJS_CAMERA> ncams = null;
				if (fc[4] == 0x81)
				{
					Console.WriteLine("File is in GC/PC format.");
					ByteConverter.BigEndian = true;
					key = 0x816DFE60;
					ini.Game = Game.SA2B;
					ini.BigEndian = true;
				}
				else
				{
					Console.WriteLine("File is in DC format.");
					ByteConverter.BigEndian = false;
					key = 0xCB00000;
					ini.Game = Game.SA2;
					ini.BigEndian = false;
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
				int camaddr = ByteConverter.ToInt32(fc, 4);
				if (cam != 0)
				{
					ini.Camera = GetMotion(fc, 4, key, $"Camera.saanim", motions, 1);
					ini.NinjaCamera = GetCamData(fc, 4, key, "CameraAttributes.ini", ncams);
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
				foreach (var item in camarrayfiles.Values)
				{
					string fn = item.Filename;
					string fp = Path.Combine(Path.GetFileNameWithoutExtension(evfilename), fn);
					item.CamData.Save(fp);
					ini.Files.Add(fn, HelperFunctions.FileHash(fp));
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

		public static void SplitExtra(string filename, string outputPath)
		{
			string dir = Environment.CurrentDirectory;
			try
			{
				if (outputPath[outputPath.Length - 1] != '/') outputPath = string.Concat(outputPath, "/");
				// get file name, read it from the console if nothing
				string evfilename = filename;

				evfilename = Path.GetFullPath(evfilename);
				Console.WriteLine("Splitting file {0}...", filename);
				byte[] fc;
				if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
					fc = Prs.Decompress(filename);
				else
					fc = File.ReadAllBytes(filename);
				MiniEventExtraIniData ini = new MiniEventExtraIniData() { Name = Path.GetFileNameWithoutExtension(filename) };
				if (outputPath.Length != 0)
				{
					if (!Directory.Exists(outputPath))
						Directory.CreateDirectory(outputPath);
					Environment.CurrentDirectory = outputPath;
				}
				else
					Environment.CurrentDirectory = Path.GetDirectoryName(evfilename);
				Directory.CreateDirectory(Path.GetFileNameWithoutExtension(evfilename));
				if (fc[4] > 0 || fc[8] > 0 || fc[0x100] > 0)
				{
					Console.WriteLine("File is in DC format.");
					ByteConverter.BigEndian = false;
					ini.Game = Game.SA2;
					ini.BigEndian = false;
				}
				else
				{
					Console.WriteLine("File is in GC/PC format.");
					ByteConverter.BigEndian = true;
					ini.Game = Game.SA2B;
					ini.BigEndian = true;
				}
				int addr = 0;
				int subcount = 0;
				for (int i = 0; i < 32; i++)
				{
					addr = 0x8 * i;
					SubtitleInfo subs = new SubtitleInfo();
					subs.FrameStart = ByteConverter.ToUInt32(fc, addr);
					if (subs.FrameStart != 0)
						subcount++;
					subs.VisibleTime = ByteConverter.ToUInt32(fc, addr + 4);
					ini.Subtitles.Add(subs);
				}
				if (subcount != 0)
					Console.WriteLine("Mini-Event contains {0} active subtitle entr{1}.", subcount, subcount == 1 ? "y" : "ies");
				else
					Console.WriteLine("Mini-Event does not use subtitles.");

				int effectcount = 0;
				for (int i = 0; i < 64; i++)
				{
					addr = 0x100 + (0x4C * i);
					EffectInfo fx = new EffectInfo();
					int frame = fc.GetPointer(addr, 0);
					fx.FrameStart = ByteConverter.ToUInt32(fc, addr);
					if (frame != 0)
						effectcount++;
					fx.FadeType = fc[addr + 4];
					fx.SFXEntry1 = fc[addr + 5];
					fx.SFXEntry2 = fc[addr + 6];
					fx.VoiceEntry = ByteConverter.ToUInt16(fc, addr + 8).ToCHex();
					fx.MusicControl = fc[addr + 0xA];
					ini.Effects.Add(fx);
				}
				if (effectcount != 0)
					Console.WriteLine("Mini-Event contains {0} active effect entr{1}.", effectcount, effectcount == 1 ? "y" : "ies");
				else
					Console.WriteLine("Mini-Event does not use additional effects.");
				int misccount = 0;
				for (int i = 0; i < 1; i++)
				{
					addr = 0x1400;
					MiscMiniEffect misc = new MiscMiniEffect();
					int unkdata1 = fc.GetPointer(addr, 0);
					misc.Unk1 = new Vertex(fc, addr);
					misc.Unk2 = ByteConverter.ToSingle(fc, addr + 0xC);
					int unkdata2 = fc.GetPointer(addr + 0x10, 0);
					misc.Unk3 = new Vertex(fc, addr + 0x10);
					if (unkdata1 != 0 || unkdata2 != 0)
						misccount++;
					ini.Unknown.Add(misc);
				}
				if (misccount != 0)
					Console.WriteLine("Mini-Event contains an unknown effect entry.");
				else
					Console.WriteLine("Mini-Event does not use unknown effects.");

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
			camarrayfiles.Clear();

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
						if (labels.ContainsKeySafe(info.BodyAnims))
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
									if (labels.ContainsKeySafe(info.Parts[j].Model))
										ByteConverter.GetBytes(labels[info.Parts[j].Model]).CopyTo(fc, address2);
									if (labels.ContainsKeySafe(info.Parts[j].Anims))
										ByteConverter.GetBytes(labels[info.Parts[j].Anims]).CopyTo(fc, address2 + 4);
									if (labels.ContainsKeySafe(info.Parts[j].ShapeMotions))
										ByteConverter.GetBytes(labels[info.Parts[j].ShapeMotions]).CopyTo(fc, address2 + 8);
								}
							}
						}
					}
				}
			}
			int cam = fc.GetPointer(4, key);
			if (cam != 0 && labels.ContainsKeySafe(ini.Camera))
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
					modelfiles.Add(obj.Name, new ModelInfo(fn, obj, ModelFormat.Chunk));
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
				motionfiles[mtn.Name] = new MotionInfo(fn, mtn);
			return mtn.Name;
		}

		private static string GetCamData(byte[] fc, int address, uint key, string fn, List<NJS_CAMERA> ncams)
		{
			NJS_CAMERA ncam = null;
			if (ncams != null)
				ncam = ncams[ByteConverter.ToInt32(fc, address)];
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
		public bool BigEndian { get; set; }
		public Dictionary<string, string> Files { get; set; } = new Dictionary<string, string>();

		public SA2CharacterFlags CharacterFlags { get; set; }
		public string Camera { get; set; }
		public string NinjaCamera { get; set; }
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

	public class MiniEventExtraIniData
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
		public List<SubtitleInfo> Subtitles { get; set; } = new List<SubtitleInfo>();
		public List<EffectInfo> Effects { get; set; } = new List<EffectInfo>();
		public List<MiscMiniEffect> Unknown { get; set; } = new List<MiscMiniEffect>();
	}

	public class EffectInfo
	{
		public uint FrameStart { get; set; }
		public byte FadeType { get; set; }
		public byte SFXEntry1 { get; set; }
		public byte SFXEntry2 { get; set; }
		public string VoiceEntry { get; set; }
		public byte MusicControl { get; set; }
	}
	public class MiscMiniEffect
	{
		public Vertex Unk1 { get; set; }
		public float Unk2 { get; set; }
		public Vertex Unk3 { get; set; }
	}
}