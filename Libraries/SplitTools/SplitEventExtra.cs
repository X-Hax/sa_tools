using FraGag.Compression;
using Newtonsoft.Json;
using SAModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SplitTools.SAArc
{
	public static class sa2EventExtra
	{
		public static void Split(string filename, string outputPath)
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
				EventExtraIniData ini = new EventExtraIniData() { Name = Path.GetFileNameWithoutExtension(filename) };
				if (outputPath.Length != 0)
				{
					if (!Directory.Exists(outputPath))
						Directory.CreateDirectory(outputPath);
					Environment.CurrentDirectory = outputPath;
				}
				else
					Environment.CurrentDirectory = Path.GetDirectoryName(evfilename);
				Directory.CreateDirectory(Path.GetFileNameWithoutExtension(evfilename));
				if (fc[4] > 0 || fc[8] > 0 || fc[0x26800] > 0)
				{
					Console.WriteLine("File is in DC format.");
					ByteConverter.BigEndian = false;
					ini.Game = Game.SA2;
				}
				else
				{
					Console.WriteLine("File is in GC/PC format.");
					ByteConverter.BigEndian = true;
					ini.Game = Game.SA2B;
				}
				int address = 0;
				int subcount = 0;
				for (int i = 0; i < 100; i++)
				{
					address = 0x8 * i;
					SubtitleInfo subs = new SubtitleInfo();
					subs.FrameStart = ByteConverter.ToUInt32(fc, address);
					if (subs.FrameStart != 0)
						subcount++;
					subs.VisibleTime = ByteConverter.ToUInt32(fc, address + 4);
					ini.Subtitles.Add(subs);
				}
				if (subcount != 0)
					Console.WriteLine("Event contains {0} active subtitle entr{1}.", subcount, subcount == 1 ? "y" : "ies");
				else
					Console.WriteLine("Event does not use subtitles.");

				int audiocount = 0;
				for (int i = 0; i < 512; i++)
				{
					address = 0x800 + (0x48 * i);
					AudioInfo audio = new AudioInfo();
					audio.FrameStart = ByteConverter.ToUInt32(fc, address);
					if (audio.FrameStart != 0)
						audiocount++;
					audio.VoiceEntry = ByteConverter.ToUInt32(fc, address + 4).ToCHex();
					audio.MusicEntry = fc.GetCString(address + 8);
					ini.AudioInfo.Add(audio);
				}
				if (audiocount != 0)
					Console.WriteLine("Event contains {0} active audio entr{1}.", audiocount, audiocount == 1 ? "y" : "ies");
				else
					Console.WriteLine("Event does not contain active audio entries.");

				int screencount = 0;
				for (int i = 0; i < 64; i++)
				{
					address = 0x9800 + (0x40 * i);
					ScreenEffects screen = new ScreenEffects();
					screen.FrameStart = ByteConverter.ToUInt32(fc, address);
					if (screen.FrameStart != 0)
						screencount++;
					screen.Type = ByteConverter.ToInt32(fc, address + 4);
					screen.R = fc[address + 8];
					screen.G = fc[address + 9];
					screen.B = fc[address + 0xA];
					screen.A = fc[address + 0xB];
					ini.ScreenEffects.Add(screen);
				}
				if (screencount != 0)
					Console.WriteLine("Event contains {0} active screen effect entr{1}.", screencount, screencount == 1 ? "y" : "ies");
				else
					Console.WriteLine("Event does not use screen effects.");

				int misccount = 0;
				for (int i = 0; i < 2048; i++)
				{
					address = 0xA800 + (0x38 * i);
					MiscEffects1 misc1 = new MiscEffects1();
					misc1.FrameStart = ByteConverter.ToUInt32(fc, address);
					if (misc1.FrameStart != 0)
						misccount++;
					misc1.Type = ByteConverter.ToInt32(fc, address + 4);
					misc1.Color = new Vertex(fc, address + 8);
					misc1.Intensity = ByteConverter.ToSingle(fc, address + 0x14);
					ini.MiscEffects.Add(misc1);
				}
				if (misccount != 0)
					Console.WriteLine("Event contains {0} active unknown effect entr{1}.", misccount, misccount == 1 ? "y" : "ies");
				else
					Console.WriteLine("Event does not use unknown effects.");

				int lightcount = 0;
				for (int i = 0; i < 1144; i++)
				{
					address = 0x26800 + (0x44 * i);
					LightingInfo light = new LightingInfo();
					light.FrameStart = ByteConverter.ToUInt32(fc, address);
					if (light.FrameStart != 0)
						lightcount++;
					light.FadeType = ByteConverter.ToInt32(fc, address + 4);
					light.LightDirection = new Vertex(fc, address + 8);
					light.Color = new Vertex(fc, address + 0x14);
					light.Intensity = ByteConverter.ToSingle(fc, address + 0x20);
					light.AmbientColor = new Vertex(fc, address + 0x24);
					ini.Lighting.Add(light);
				}
				if (lightcount != 0)
					Console.WriteLine("Event contains {0} active lighting entr{1}.", lightcount, lightcount == 1 ? "y" : "ies");
				else
					Console.WriteLine("Event does not use lighting.");

				int videocount = 0;
				for (int i = 0; i < 40; i++)
				{
					address = 0x39800 + (0x40 * i);
					VideoInfo video = new VideoInfo();
					video.FrameStart = ByteConverter.ToUInt32(fc, address);
					if (video.FrameStart != 0)
						videocount++;
					video.A = fc[address + 4];
					video.R = fc[address + 5];
					video.G = fc[address + 6];
					video.B = fc[address + 7];
					video.Depth = ByteConverter.ToSingle(fc, address + 0x8);
					video.OverlayType = ByteConverter.ToUInt32(fc, address + 0xC).ToCHex();
					video.VideoName = fc.GetCString(address + 0x10);
					ini.VideoInfo.Add(video);
				}
				if (videocount != 0)
					Console.WriteLine("Event contains {0} active video entr{1}.", videocount, videocount == 1 ? "y" : "ies");
				else
					Console.WriteLine("Event does not use video effects.");

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

		public static void SplitLanguage(string filename, string outputPath)
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
				EventExtraIniData ini = new EventExtraIniData() { Name = Path.GetFileNameWithoutExtension(filename) };
				if (outputPath.Length != 0)
				{
					if (!Directory.Exists(outputPath))
						Directory.CreateDirectory(outputPath);
					Environment.CurrentDirectory = outputPath;
				}
				else
					Environment.CurrentDirectory = Path.GetDirectoryName(evfilename);
				Directory.CreateDirectory(Path.GetFileNameWithoutExtension(evfilename));
				if (fc[4] > 0 || fc[8] > 0 || fc[0x26800] > 0)
				{
					Console.WriteLine("File is in DC format.");
					ByteConverter.BigEndian = false;
					ini.Game = Game.SA2;
				}
				else
				{
					Console.WriteLine("File is in GC/PC format.");
					ByteConverter.BigEndian = true;
					ini.Game = Game.SA2B;
				}
				int address = 0;
				int subcount = 0;
				for (int i = 0; i < 100; i++)
				{
					address = 0x8 * i;
					SubtitleInfo subs = new SubtitleInfo();
					subs.FrameStart = ByteConverter.ToUInt32(fc, address);
					if (subs.FrameStart != 0)
						subcount++;
					subs.VisibleTime = ByteConverter.ToUInt32(fc, address + 4);
					ini.Subtitles.Add(subs);
				}
				if (subcount != 0)
					Console.WriteLine("Event contains {0} active subtitle entr{1}.", subcount, subcount == 1 ? "y" : "ies");
				else
					Console.WriteLine("Event does not use subtitles.");

				int audiocount = 0;
				for (int i = 0; i < 512; i++)
				{
					address = 0x800 + (0x48 * i);
					AudioInfo audio = new AudioInfo();
					audio.FrameStart = ByteConverter.ToUInt32(fc, address);
					if (audio.FrameStart != 0)
						audiocount++;
					audio.VoiceEntry = ByteConverter.ToUInt32(fc, address + 4).ToCHex();
					audio.MusicEntry = fc.GetCString(address + 8);
					ini.AudioInfo.Add(audio);
				}
				if (audiocount != 0)
					Console.WriteLine("Event contains {0} active audio entr{1}.", audiocount, audiocount == 1 ? "y" : "ies");
				else
					Console.WriteLine("Event does not contain active audio entries.");

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
			byte[] fc;
			if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				fc = Prs.Decompress(filename);
			else
				fc = File.ReadAllBytes(filename);
			string path = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filename)), Path.GetFileNameWithoutExtension(filename));
			JsonSerializer js = new JsonSerializer();
			EventExtraIniData ini;
			using (TextReader tr = File.OpenText(Path.Combine(path, Path.ChangeExtension(Path.GetFileName(filename), ".json"))))
			using (JsonTextReader jtr = new JsonTextReader(tr))
				ini = js.Deserialize<EventExtraIniData>(jtr);
			//		if (fc[0] == 0x81)
			//		{
			//			if (fc[0x2B] <= 1 && fc[0x2A] == 0)
			//			{
			//				ByteConverter.BigEndian = true;
			//				key = 0x8125FE60;
			//				battle = true;
			//			}
			//			else
			//			{
			//				ByteConverter.BigEndian = true;
			//				key = 0x812FFE60;
			//				battlebeta = true;
			//			}
			//		}
			//		else
			//		{
			//			if ((fc[37] == 0x25) || (fc[38] == 0x22) || ((fc[36] == 0) && ((fc[1] == 0xFE) || (fc[1] == 0xF2) || ((fc[1] == 0x27) && fc[2] == 0x9F))))
			//			{
			//				ByteConverter.BigEndian = false;
			//				key = 0xC600000;
			//				dcbeta = true;
			//			}
			//			else
			//			{
			//				ByteConverter.BigEndian = false;
			//				key = 0xC600000;
			//			}
			//		}
			//		List<byte> modelbytes = new List<byte>(fc);
			//		Dictionary<string, uint> labels = new Dictionary<string, uint>();
			//		foreach (string file in ini.Files.Where(a => a.Key.EndsWith(".sa2mdl", StringComparison.OrdinalIgnoreCase) && HelperFunctions.FileHash(Path.Combine(path, a.Key)) != a.Value).Select(a => a.Key))
			//			modelbytes.AddRange(new ModelFile(Path.Combine(path, file)).Model.GetBytes((uint)(key + modelbytes.Count), false, labels, new List<uint>(), out uint _));
			//		if (battle)
			//		{
			//			foreach (string file in ini.Files.Where(a => a.Key.EndsWith(".sa2bmdl", StringComparison.OrdinalIgnoreCase) && HelperFunctions.FileHash(Path.Combine(path, a.Key)) != a.Value).Select(a => a.Key))
			//				modelbytes.AddRange(new ModelFile(Path.Combine(path, file)).Model.GetBytes((uint)(key + modelbytes.Count), false, labels, new List<uint>(), out uint _));
			//			List<byte> motionbytes = new List<byte>(new byte[(ini.Motions.Count + 1) * 8]);
			//			Dictionary<string, int> partcounts = new Dictionary<string, int>(ini.Motions.Count);
			//			foreach (string file in ini.Files.Where(a => a.Key.EndsWith(".saanim", StringComparison.OrdinalIgnoreCase)).Select(a => a.Key))
			//			{
			//				NJS_MOTION motion = NJS_MOTION.Load(Path.Combine(path, file));
			//				motionbytes.AddRange(motion.GetBytes((uint)motionbytes.Count, labels, out uint _));
			//				partcounts.Add(motion.Name, motion.ModelParts);
			//			}
			//			byte[] mfc = motionbytes.ToArray();
			//			for (int i = 0; i < ini.Motions.Count; i++)
			//			{
			//				if (ini.Motions[i] == null)
			//					new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }.CopyTo(mfc, i * 8);
			//				else
			//				{
			//					ByteConverter.GetBytes(labels[ini.Motions[i]]).CopyTo(mfc, i * 8);
			//					ByteConverter.GetBytes(partcounts[ini.Motions[i]]).CopyTo(mfc, i * 8 + 4);
			//				}
			//			}
			//			File.WriteAllBytes(Path.ChangeExtension(Path.ChangeExtension(filename, null) + "motion", ".bin"), mfc);
			//		}
			//		else
			//			foreach (string file in ini.Files.Where(a => a.Key.EndsWith(".saanim", StringComparison.OrdinalIgnoreCase) && HelperFunctions.FileHash(Path.Combine(path, a.Key)) != a.Value).Select(a => a.Key))
			//				modelbytes.AddRange(NJS_MOTION.Load(Path.Combine(path, file)).GetBytes((uint)(key + modelbytes.Count), labels, out uint _));
			//		fc = modelbytes.ToArray();
			//		int ptr = fc.GetPointer(0x20, key);
			//		if (ptr != 0)
			//			if (!dcbeta)
			//			{
			//				for (int i = 0; i < (battle ? 18 : 16); i++)
			//				{
			//					UpgradeInfo info = ini.Upgrades[i];
			//					if (info.RootNode != null)
			//					{
			//						if (labels.ContainsKeySafe(info.RootNode))
			//							ByteConverter.GetBytes(labels[info.RootNode]).CopyTo(fc, ptr);
			//						if (labels.ContainsKeySafe(info.AttachNode1))
			//							ByteConverter.GetBytes(labels[info.AttachNode1]).CopyTo(fc, ptr + 4);
			//						if (labels.ContainsKeySafe(info.Model1))
			//							ByteConverter.GetBytes(labels[info.Model1]).CopyTo(fc, ptr + 8);
			//						if (labels.ContainsKeySafe(info.AttachNode2))
			//							ByteConverter.GetBytes(labels[info.AttachNode2]).CopyTo(fc, ptr + 12);
			//						if (labels.ContainsKeySafe(info.Model2))
			//							ByteConverter.GetBytes(labels[info.Model2]).CopyTo(fc, ptr + 16);
			//					}
			//					ptr += 0x14;
			//				}
			//			}
			//		else
			//		{
			//			for (int i = 0; i < 14; i++)
			//			{
			//				UpgradeInfo info = ini.Upgrades[i];
			//				if (info.RootNode != null)
			//				{
			//					if (labels.ContainsKeySafe(info.RootNode))
			//						ByteConverter.GetBytes(labels[info.RootNode]).CopyTo(fc, ptr);
			//					if (labels.ContainsKeySafe(info.AttachNode1))
			//						ByteConverter.GetBytes(labels[info.AttachNode1]).CopyTo(fc, ptr + 4);
			//					if (labels.ContainsKeySafe(info.Model1))
			//						ByteConverter.GetBytes(labels[info.Model1]).CopyTo(fc, ptr + 8);
			//					if (labels.ContainsKeySafe(info.AttachNode2))
			//						ByteConverter.GetBytes(labels[info.AttachNode2]).CopyTo(fc, ptr + 12);
			//					if (labels.ContainsKeySafe(info.Model2))
			//						ByteConverter.GetBytes(labels[info.Model2]).CopyTo(fc, ptr + 16);
			//				}
			//				ptr += 0x14;
			//			}
			//		}
			//		int gcnt = ByteConverter.ToInt32(fc, 8);
			//		ptr = fc.GetPointer(0, key);
			//		if (ptr != 0)
			//			for (int gn = 0; gn <= gcnt; gn++)
			//			{
			//				SceneInfo info = ini.Scenes[gn];
			//				int ptr2 = fc.GetPointer(ptr, key);
			//				int ecnt = Math.Min(ByteConverter.ToInt32(fc, ptr + 4), info.Entities?.Count ?? 0);
			//				if (ptr2 != 0)
			//					for (int en = 0; en < ecnt; en++)
			//					{
			//						if (labels.ContainsKeySafe(info.Entities[en].Model))
			//							ByteConverter.GetBytes(labels[info.Entities[en].Model]).CopyTo(fc, ptr2);
			//						if (!battle)
			//						{
			//							if (labels.ContainsKeySafe(info.Entities[en].Motion))
			//								ByteConverter.GetBytes(labels[info.Entities[en].Motion]).CopyTo(fc, ptr2 + 4);
			//							if (labels.ContainsKeySafe(info.Entities[en].ShapeMotion))
			//								ByteConverter.GetBytes(labels[info.Entities[en].ShapeMotion]).CopyTo(fc, ptr2 + 8);
			//						}
			//						else
			//						{
			//							if (labels.ContainsKeySafe(info.Entities[en].GCModel))
			//								ByteConverter.GetBytes(labels[info.Entities[en].GCModel]).CopyTo(fc, ptr2 + 12);
			//							if (labels.ContainsKeySafe(info.Entities[en].ShadowModel))
			//								ByteConverter.GetBytes(labels[info.Entities[en].ShadowModel]).CopyTo(fc, ptr2 + 16);
			//						}
			//						ptr2 += battle ? 0x2C : 0x20;
			//					}
			//				if (!battle)
			//				{
			//					ptr2 = fc.GetPointer(ptr + 8, key);
			//					if (ptr2 != 0)
			//					{
			//						int cnt = ByteConverter.ToInt32(fc, ptr + 12);
			//						for (int i = 0; i < cnt; i++)
			//						{
			//							if (labels.ContainsKeySafe(info.CameraMotions[i]))
			//								ByteConverter.GetBytes(labels[info.CameraMotions[i]]).CopyTo(fc, ptr2);
			//							ptr2 += sizeof(int);
			//						}
			//					}
			//				}
			//				ptr2 = fc.GetPointer(ptr + 0x18, key);
			//				if (ptr2 != 0 && info.Big != null)
			//				{
			//					if (labels.ContainsKeySafe(info.Big.Model))
			//						ByteConverter.GetBytes(labels[info.Big.Model]).CopyTo(fc, ptr2);
			//					if (!battle)
			//					{
			//						int ptr3 = fc.GetPointer(ptr2 + 4, key);
			//						if (ptr3 != 0)
			//						{
			//							int cnt = ByteConverter.ToInt32(fc, ptr2 + 8);
			//							for (int i = 0; i < cnt; i++)
			//							{
			//								if (labels.ContainsKeySafe(info.Big.Motions[i][0]))
			//									ByteConverter.GetBytes(labels[info.Big.Motions[i][0]]).CopyTo(fc, ptr3);
			//								if (labels.ContainsKeySafe(info.Big.Motions[i][1]))
			//									ByteConverter.GetBytes(labels[info.Big.Motions[i][1]]).CopyTo(fc, ptr3 + 4);
			//								ptr3 += 8;
			//							}
			//						}
			//					}
			//				}
			//				ptr += 0x20;
			//			}
			//		ptr = fc.GetPointer(0x18, key);
			//		if (ptr != 0)
			//			for (int i = 0; i < 93; i++)
			//			{
			//				if (ini.MechParts.ContainsKey(i) && labels.ContainsKeySafe(ini.MechParts[i]))
			//					ByteConverter.GetBytes(labels[ini.MechParts[i]]).CopyTo(fc, ptr);
			//				ptr += 4;
			//			}
			//		ptr = fc.GetPointer(0x1C, key);
			//		if (ptr != 0 && labels.ContainsKeySafe(ini.TailsTails))
			//			ByteConverter.GetBytes(labels[ini.TailsTails]).CopyTo(fc, ptr);
			//		if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
			//			Prs.Compress(fc, filename);
			//		else
			//			File.WriteAllBytes(filename, fc);
			//	}

			//	public static bool ContainsKeySafe<TValue>(this IDictionary<string, TValue> dict, string key)
			//	{
			//		return key != null && dict.ContainsKey(key);
			//	}
		}
	}

	public class EventExtraIniData
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
		public List<SubtitleInfo> Subtitles { get; set; } = new List<SubtitleInfo>();
		public List<AudioInfo> AudioInfo { get; set; } = new List<AudioInfo>();
		public List<ScreenEffects> ScreenEffects { get; set; } = new List<ScreenEffects>();
		public List<MiscEffects1> MiscEffects { get; set; } = new List<MiscEffects1>();
		public List<LightingInfo> Lighting { get; set; } = new List<LightingInfo>();
		public List<VideoInfo> VideoInfo { get; set; } = new List<VideoInfo>();
	}

	public class SubtitleInfo
	{
		public uint FrameStart { get; set; }
		public uint VisibleTime { get; set; }
	}

	public class AudioInfo
	{
		public uint FrameStart { get; set; }
		public string VoiceEntry { get; set; }
		public string MusicEntry { get; set; }
	}

	public class ScreenEffects
	{
		public uint FrameStart { get; set; }
		public int Type { get; set; }
		public byte R { get; set; }
		public byte G { get; set; }
		public byte B { get; set; }
		public byte A { get; set; }
	}

	public class MiscEffects1
	{
		public uint FrameStart { get; set; }
		public int Type { get; set; }
		public Vertex Color { get; set; }
		public float Intensity { get; set; }
	}

	public class LightingInfo
	{
		public uint FrameStart { get; set; }
		public int FadeType { get; set; }
		public Vertex LightDirection { get; set; }
		public Vertex Color { get; set; }
		public float Intensity { get; set; }
		public Vertex AmbientColor { get; set; }
	}

	public class VideoInfo
	{
		public uint FrameStart { get; set; }
		public byte A { get; set; }
		public byte R { get; set; }
		public byte G { get; set; }
		public byte B { get; set; }
		public float Depth { get; set; }
		public string OverlayType { get; set; }
		public string VideoName { get; set; }
	}
}
