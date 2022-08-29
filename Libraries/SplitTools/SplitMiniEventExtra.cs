using FraGag.Compression;
using Newtonsoft.Json;
using SAModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SplitTools.SAArc
{
	public static class sa2MiniEventExtra
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
				}
				else
				{
					Console.WriteLine("File is in GC/PC format.");
					ByteConverter.BigEndian = true;
					ini.Game = Game.SA2B;
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
					if (subs.FrameStart != 0 && subs.VisibleTime != 0)
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
					if (frame != 0)
					{
						fx.FrameStart = ByteConverter.ToUInt32(fc, addr);
						effectcount++;
					}
					fx.FadeType = fc[addr + 4];
					ushort sound = (ushort)fc.GetPointer(addr + 5, 0);
					if (sound != 0xFFFF)
					fx.SFXEntry = ByteConverter.ToUInt16(fc, addr + 5).ToCHex();
					ushort voice = (ushort)fc.GetPointer(addr + 8, 0);
					if (voice != 0xFFFF)
						fx.VoiceEntry = ByteConverter.ToUInt16(fc, addr + 8).ToCHex();
					fx.MusicControl = fc[addr + 0xA];
					if (fx.FrameStart != 0 && sound != 0xFFFF && voice != 0xFFFF && fx.FadeType != 0 && fx.MusicControl != 0)
					ini.Effects.Add(fx);
				}
				if (effectcount != 0)
					Console.WriteLine("Mini-Event contains {0} active effect entr{1}.", effectcount, effectcount == 1 ? "y" : "ies");
				else
					Console.WriteLine("Mini-Event does not use additional effects.");

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

	//	public static void Build(string filename)
	//	{
	//		byte[] fc;
	//		if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
	//			fc = Prs.Decompress(filename);
	//		else
	//			fc = File.ReadAllBytes(filename);
	//		string path = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filename)), Path.GetFileNameWithoutExtension(filename));
	//		JsonSerializer js = new JsonSerializer();
	//		EventIniData ini;
	//		using (TextReader tr = File.OpenText(Path.Combine(path, Path.ChangeExtension(Path.GetFileName(filename), ".json"))))
	//		using (JsonTextReader jtr = new JsonTextReader(tr))
	//			ini = js.Deserialize<EventIniData>(jtr);
	//		uint key;
	//		bool battle = ini.Game == Game.SA2B;
	//		bool battlebeta = ini.Game == Game.SA2B;
	//		bool dcbeta = ini.Game == Game.SA2;
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
		public List<SubtitleInfo> Subtitles { get; set; } = new List<SubtitleInfo>();
		public List<EffectInfo> Effects { get; set; } = new List<EffectInfo>();
	}

	public class EffectInfo
	{
		public uint FrameStart { get; set; }
		public byte FadeType { get; set; }
		public string SFXEntry { get; set; }
		public string VoiceEntry { get; set; }
		public byte MusicControl { get; set; }
	}
}
