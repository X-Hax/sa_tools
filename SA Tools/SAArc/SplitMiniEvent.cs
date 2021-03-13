using FraGag.Compression;
using Newtonsoft.Json;
using SonicRetro.SAModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SA_Tools.SAArc
{
	public static class SA2MiniEvent
	{
		static readonly string[] charbody = { "Head", "Mouth", "LHand", "RHand" };
		static List<string> nodenames = new List<string>();
		static Dictionary<string, MEModelInfo> modelfiles = new Dictionary<string, MEModelInfo>();
		static Dictionary<string, MEMotionInfo> motionfiles = new Dictionary<string, MEMotionInfo>();

		public static void Split(string filename)
		{
			nodenames.Clear();
			modelfiles.Clear();
			motionfiles.Clear();

			Console.WriteLine("Splitting file {0}...", filename);
			byte[] fc;
			if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				fc = Prs.Decompress(filename);
			else
				fc = File.ReadAllBytes(filename);
			MiniEventIniData ini = new MiniEventIniData() { Name = Path.GetFileNameWithoutExtension(filename) };
			string path = Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filename)), Path.GetFileNameWithoutExtension(filename))).FullName;
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
			int ptr = fc.GetPointer(8, key);
			if (ptr != 0)
			{
				Console.WriteLine("Sonic is in this Mini-Event");
				Directory.CreateDirectory(Path.Combine(path, "Sonic"));
				MiniEventSonic data = new MiniEventSonic();
				data.BodyAnims = GetMotion(fc, ptr, key, $"Sonic\\Body.saanim", motions, 62);
				int ptr2 = fc.GetPointer(ptr + 4, key);
					if (ptr2 != 0)
						data.HeadPart = GetModel(fc, ptr + 4, key, $"Sonic\\Head.sa2mdl");
					if (data.HeadPart != null)
					{
						data.HeadAnims = GetMotion(fc, ptr + 8, key, $"Sonic\\Head.saanim", motions, modelfiles[data.HeadPart].Model.CountAnimated());
						if (data.HeadAnims != null)
							modelfiles[data.HeadPart].Motions.Add(motionfiles[data.HeadAnims].Filename);
						data.HeadShapeMotions = GetMotion(fc, ptr + 0xC, key, $"Sonic\\HeadShape.saanim", motions, modelfiles[data.HeadPart].Model.CountMorph());
						if (data.HeadShapeMotions != null)
							modelfiles[data.HeadPart].Motions.Add(motionfiles[data.HeadShapeMotions].Filename);
					}
				ptr2 = fc.GetPointer(ptr + 0x10, key);
				if (ptr2 != 0)
					data.MouthPart = GetModel(fc, ptr + 0x10, key, $"Sonic\\Mouth.sa2mdl");
				if (data.MouthPart != null)
				{
					data.MouthAnims = GetMotion(fc, ptr + 0x14, key, $"Sonic\\Mouth.saanim", motions, modelfiles[data.MouthPart].Model.CountAnimated());
					if (data.MouthAnims != null)
						modelfiles[data.MouthPart].Motions.Add(motionfiles[data.MouthAnims].Filename);
					data.HeadShapeMotions = GetMotion(fc, ptr + 0x18, key, $"Sonic\\MouthShape.saanim", motions, modelfiles[data.MouthPart].Model.CountMorph());
					if (data.MouthShapeMotions != null)
						modelfiles[data.MouthPart].Motions.Add(motionfiles[data.MouthShapeMotions].Filename);
				}
				ptr2 = fc.GetPointer(ptr + 0x1C, key);
				if (ptr2 != 0)
					data.LHandPart = GetModel(fc, ptr + 0x1C, key, $"Sonic\\LeftHand.sa2mdl");
				if (data.LHandPart != null)
				{
					data.LHandAnims = GetMotion(fc, ptr + 0x20, key, $"Sonic\\LeftHand.saanim", motions, modelfiles[data.LHandPart].Model.CountAnimated());
					if (data.LHandAnims != null)
						modelfiles[data.LHandPart].Motions.Add(motionfiles[data.LHandAnims].Filename);
					data.HeadShapeMotions = GetMotion(fc, ptr + 0x24, key, $"Sonic\\LeftHandShape.saanim", motions, modelfiles[data.LHandPart].Model.CountMorph());
					if (data.LHandShapeMotions != null)
						modelfiles[data.LHandPart].Motions.Add(motionfiles[data.LHandShapeMotions].Filename);
				}
				ptr2 = fc.GetPointer(ptr + 0x28, key);
				if (ptr2 != 0)
					data.RHandPart = GetModel(fc, ptr + 0x28, key, $"Sonic\\RightHand.sa2mdl");
				if (data.RHandPart != null)
				{
					data.RHandAnims = GetMotion(fc, ptr + 0x2C, key, $"Sonic\\RightHand.saanim", motions, modelfiles[data.RHandPart].Model.CountAnimated());
					if (data.RHandAnims != null)
						modelfiles[data.RHandPart].Motions.Add(motionfiles[data.RHandAnims].Filename);
					data.HeadShapeMotions = GetMotion(fc, ptr + 0x30, key, $"Sonic\\RightHandShape.saanim", motions, modelfiles[data.RHandPart].Model.CountMorph());
					if (data.RHandShapeMotions != null)
						modelfiles[data.RHandPart].Motions.Add(motionfiles[data.RHandShapeMotions].Filename);
				}
				ini.Sonic.Add(data);
				}
			ptr = fc.GetPointer(0xC, key);
			if (ptr != 0)
			{
				Console.WriteLine("Shadow is in this Mini-Event");
				Directory.CreateDirectory(Path.Combine(path, "Shadow"));
				MiniEventShadow data = new MiniEventShadow();
				data.BodyAnims = GetMotion(fc, ptr, key, $"Shadow\\Body.saanim", motions, 62);
				int ptr2 = fc.GetPointer(ptr + 4, key);
				if (ptr2 != 0)
					data.HeadPart = GetModel(fc, ptr + 4, key, $"Shadow\\Head.sa2mdl");
				if (data.HeadPart != null)
				{
					data.HeadAnims = GetMotion(fc, ptr + 8, key, $"Shadow\\Head.saanim", motions, modelfiles[data.HeadPart].Model.CountAnimated());
					if (data.HeadAnims != null)
						modelfiles[data.HeadPart].Motions.Add(motionfiles[data.HeadAnims].Filename);
					data.HeadShapeMotions = GetMotion(fc, ptr + 0xC, key, $"Shadow\\HeadShape.saanim", motions, modelfiles[data.HeadPart].Model.CountMorph());
					if (data.HeadShapeMotions != null)
						modelfiles[data.HeadPart].Motions.Add(motionfiles[data.HeadShapeMotions].Filename);
				}
				ptr2 = fc.GetPointer(ptr + 0x10, key);
				if (ptr2 != 0)
					data.MouthPart = GetModel(fc, ptr + 0x10, key, $"Shadow\\Mouth.sa2mdl");
				if (data.MouthPart != null)
				{
					data.MouthAnims = GetMotion(fc, ptr + 0x14, key, $"Shadow\\Mouth.saanim", motions, modelfiles[data.MouthPart].Model.CountAnimated());
					if (data.MouthAnims != null)
						modelfiles[data.MouthPart].Motions.Add(motionfiles[data.MouthAnims].Filename);
					data.HeadShapeMotions = GetMotion(fc, ptr + 0x18, key, $"Shadow\\MouthShape.saanim", motions, modelfiles[data.MouthPart].Model.CountMorph());
					if (data.MouthShapeMotions != null)
						modelfiles[data.MouthPart].Motions.Add(motionfiles[data.MouthShapeMotions].Filename);
				}
				ptr2 = fc.GetPointer(ptr + 0x1C, key);
				if (ptr2 != 0)
					data.LHandPart = GetModel(fc, ptr + 0x1C, key, $"Shadow\\LeftHand.sa2mdl");
				if (data.LHandPart != null)
				{
					data.LHandAnims = GetMotion(fc, ptr + 0x20, key, $"Shadow\\LeftHand.saanim", motions, modelfiles[data.LHandPart].Model.CountAnimated());
					if (data.LHandAnims != null)
						modelfiles[data.LHandPart].Motions.Add(motionfiles[data.LHandAnims].Filename);
					data.HeadShapeMotions = GetMotion(fc, ptr + 0x24, key, $"Shadow\\LeftHandShape.saanim", motions, modelfiles[data.LHandPart].Model.CountMorph());
					if (data.LHandShapeMotions != null)
						modelfiles[data.LHandPart].Motions.Add(motionfiles[data.LHandShapeMotions].Filename);
				}
				ptr2 = fc.GetPointer(ptr + 0x28, key);
				if (ptr2 != 0)
					data.RHandPart = GetModel(fc, ptr + 0x28, key, $"Shadow\\RightHand.sa2mdl");
				if (data.RHandPart != null)
				{
					data.RHandAnims = GetMotion(fc, ptr + 0x2C, key, $"Shadow\\RightHand.saanim", motions, modelfiles[data.RHandPart].Model.CountAnimated());
					if (data.RHandAnims != null)
						modelfiles[data.RHandPart].Motions.Add(motionfiles[data.RHandAnims].Filename);
					data.HeadShapeMotions = GetMotion(fc, ptr + 0x30, key, $"Shadow\\RightHandShape.saanim", motions, modelfiles[data.RHandPart].Model.CountMorph());
					if (data.RHandShapeMotions != null)
						modelfiles[data.RHandPart].Motions.Add(motionfiles[data.RHandShapeMotions].Filename);
				}
				ini.Shadow.Add(data);
			}
			ptr = fc.GetPointer(0x18, key);
			if (ptr != 0)
			{
				Console.WriteLine("Knuckles is in this Mini-Event");
				Directory.CreateDirectory(Path.Combine(path, "Knuckles"));
				MiniEventKnux data = new MiniEventKnux();
				data.BodyAnims = GetMotion(fc, ptr, key, $"Knuckles\\Body.saanim", motions, 62);
				int ptr2 = fc.GetPointer(ptr + 4, key);
				if (ptr2 != 0)
					data.HeadPart = GetModel(fc, ptr + 4, key, $"Knuckles\\Head.sa2mdl");
				if (data.HeadPart != null)
				{
					data.HeadAnims = GetMotion(fc, ptr + 8, key, $"Knuckles\\Head.saanim", motions, modelfiles[data.HeadPart].Model.CountAnimated());
					if (data.HeadAnims != null)
						modelfiles[data.HeadPart].Motions.Add(motionfiles[data.HeadAnims].Filename);
					data.HeadShapeMotions = GetMotion(fc, ptr + 0xC, key, $"Knuckles\\HeadShape.saanim", motions, modelfiles[data.HeadPart].Model.CountMorph());
					if (data.HeadShapeMotions != null)
						modelfiles[data.HeadPart].Motions.Add(motionfiles[data.HeadShapeMotions].Filename);
				}
				ptr2 = fc.GetPointer(ptr + 0x10, key);
				if (ptr2 != 0)
					data.MouthPart = GetModel(fc, ptr + 0x10, key, $"Knuckles\\Mouth.sa2mdl");
				if (data.MouthPart != null)
				{
					data.MouthAnims = GetMotion(fc, ptr + 0x14, key, $"Knuckles\\Mouth.saanim", motions, modelfiles[data.MouthPart].Model.CountAnimated());
					if (data.MouthAnims != null)
						modelfiles[data.MouthPart].Motions.Add(motionfiles[data.MouthAnims].Filename);
					data.HeadShapeMotions = GetMotion(fc, ptr + 0x18, key, $"Knuckles\\MouthShape.saanim", motions, modelfiles[data.MouthPart].Model.CountMorph());
					if (data.MouthShapeMotions != null)
						modelfiles[data.MouthPart].Motions.Add(motionfiles[data.MouthShapeMotions].Filename);
				}
				ptr2 = fc.GetPointer(ptr + 0x1C, key);
				if (ptr2 != 0)
					data.LHandPart = GetModel(fc, ptr + 0x1C, key, $"Knuckles\\LeftHand.sa2mdl");
				if (data.LHandPart != null)
				{
					data.LHandAnims = GetMotion(fc, ptr + 0x20, key, $"Knuckles\\LeftHand.saanim", motions, modelfiles[data.LHandPart].Model.CountAnimated());
					if (data.LHandAnims != null)
						modelfiles[data.LHandPart].Motions.Add(motionfiles[data.LHandAnims].Filename);
					data.HeadShapeMotions = GetMotion(fc, ptr + 0x24, key, $"Knuckles\\LeftHandShape.saanim", motions, modelfiles[data.LHandPart].Model.CountMorph());
					if (data.LHandShapeMotions != null)
						modelfiles[data.LHandPart].Motions.Add(motionfiles[data.LHandShapeMotions].Filename);
				}
				ptr2 = fc.GetPointer(ptr + 0x28, key);
				if (ptr2 != 0)
					data.RHandPart = GetModel(fc, ptr + 0x28, key, $"Knuckles\\RightHand.sa2mdl");
				if (data.RHandPart != null)
				{
					data.RHandAnims = GetMotion(fc, ptr + 0x2C, key, $"Knuckles\\RightHand.saanim", motions, modelfiles[data.RHandPart].Model.CountAnimated());
					if (data.RHandAnims != null)
						modelfiles[data.RHandPart].Motions.Add(motionfiles[data.RHandAnims].Filename);
					data.HeadShapeMotions = GetMotion(fc, ptr + 0x30, key, $"Knuckles\\RightHandShape.saanim", motions, modelfiles[data.RHandPart].Model.CountMorph());
					if (data.RHandShapeMotions != null)
						modelfiles[data.RHandPart].Motions.Add(motionfiles[data.RHandShapeMotions].Filename);
				}
				ini.Knuckles.Add(data);
			}
			ptr = fc.GetPointer(0x1C, key);
			if (ptr != 0)
			{
				Console.WriteLine("Rouge is in this Mini-Event");
				Directory.CreateDirectory(Path.Combine(path, "Rouge"));
				MiniEventRouge data = new MiniEventRouge();
				data.BodyAnims = GetMotion(fc, ptr, key, $"Rouge\\Body.saanim", motions, 62);
				int ptr2 = fc.GetPointer(ptr + 4, key);
				if (ptr2 != 0)
					data.HeadPart = GetModel(fc, ptr + 4, key, $"Rouge\\Head.sa2mdl");
				if (data.HeadPart != null)
				{
					data.HeadAnims = GetMotion(fc, ptr + 8, key, $"Rouge\\Head.saanim", motions, modelfiles[data.HeadPart].Model.CountAnimated());
					if (data.HeadAnims != null)
						modelfiles[data.HeadPart].Motions.Add(motionfiles[data.HeadAnims].Filename);
					data.HeadShapeMotions = GetMotion(fc, ptr + 0xC, key, $"Rouge\\HeadShape.saanim", motions, modelfiles[data.HeadPart].Model.CountMorph());
					if (data.HeadShapeMotions != null)
						modelfiles[data.HeadPart].Motions.Add(motionfiles[data.HeadShapeMotions].Filename);
				}
				ptr2 = fc.GetPointer(ptr + 0x10, key);
				if (ptr2 != 0)
					data.MouthPart = GetModel(fc, ptr + 0x10, key, $"Rouge\\Mouth.sa2mdl");
				if (data.MouthPart != null)
				{
					data.MouthAnims = GetMotion(fc, ptr + 0x14, key, $"Rouge\\Mouth.saanim", motions, modelfiles[data.MouthPart].Model.CountAnimated());
					if (data.MouthAnims != null)
						modelfiles[data.MouthPart].Motions.Add(motionfiles[data.MouthAnims].Filename);
					data.HeadShapeMotions = GetMotion(fc, ptr + 0x18, key, $"Rouge\\MouthShape.saanim", motions, modelfiles[data.MouthPart].Model.CountMorph());
					if (data.MouthShapeMotions != null)
						modelfiles[data.MouthPart].Motions.Add(motionfiles[data.MouthShapeMotions].Filename);
				}
				ptr2 = fc.GetPointer(ptr + 0x1C, key);
				if (ptr2 != 0)
					data.LHandPart = GetModel(fc, ptr + 0x1C, key, $"Rouge\\LeftHand.sa2mdl");
				if (data.LHandPart != null)
				{
					data.LHandAnims = GetMotion(fc, ptr + 0x20, key, $"Rouge\\LeftHand.saanim", motions, modelfiles[data.LHandPart].Model.CountAnimated());
					if (data.LHandAnims != null)
						modelfiles[data.LHandPart].Motions.Add(motionfiles[data.LHandAnims].Filename);
					data.HeadShapeMotions = GetMotion(fc, ptr + 0x24, key, $"Rouge\\LeftHandShape.saanim", motions, modelfiles[data.LHandPart].Model.CountMorph());
					if (data.LHandShapeMotions != null)
						modelfiles[data.LHandPart].Motions.Add(motionfiles[data.LHandShapeMotions].Filename);
				}
				ptr2 = fc.GetPointer(ptr + 0x28, key);
				if (ptr2 != 0)
					data.RHandPart = GetModel(fc, ptr + 0x28, key, $"Rouge\\RightHand.sa2mdl");
				if (data.RHandPart != null)
				{
					data.RHandAnims = GetMotion(fc, ptr + 0x2C, key, $"Rouge\\RightHand.saanim", motions, modelfiles[data.RHandPart].Model.CountAnimated());
					if (data.RHandAnims != null)
						modelfiles[data.RHandPart].Motions.Add(motionfiles[data.RHandAnims].Filename);
					data.HeadShapeMotions = GetMotion(fc, ptr + 0x30, key, $"Rouge\\RightHandShape.saanim", motions, modelfiles[data.RHandPart].Model.CountMorph());
					if (data.RHandShapeMotions != null)
						modelfiles[data.RHandPart].Motions.Add(motionfiles[data.RHandShapeMotions].Filename);
				}
				ini.Rouge.Add(data);
			}
			ptr = fc.GetPointer(0x24, key);
			if (ptr != 0)
			{
				Console.WriteLine("Mech Eggman is in this Mini-Event");
				Directory.CreateDirectory(Path.Combine(path, "Mech Eggman"));
				ini.MechEggmanBodyAnims = GetMotion(fc, ptr, key, $"Mech Eggman\\Body.saanim", motions, 33);
			}
			ptr = fc.GetPointer(4, key);
			if (ptr != 0)
			{
				ini.Camera = GetMotion(fc, ptr + 0x10, key, $"Camera.saanim", motions, 1);
				//ini.CamFrames = ByteConverter.ToInt32(fc, ptr + 4);
			}
			else
				Console.WriteLine("Mini-Event does not contain a camera.");
			foreach (var item in motionfiles.Values)
			{
				string fn = item.Filename;
				string fp = Path.Combine(path, fn);
				item.Motion.Save(fp);
				ini.Files.Add(fn, HelperFunctions.FileHash(fp));
			}
			foreach (var item in modelfiles.Values)
			{
				string fp = Path.Combine(path, item.Filename);
				ModelFile.CreateFile(fp, item.Model, item.Motions.ToArray(), null, null, null, item.Format);
				ini.Files.Add(item.Filename, HelperFunctions.FileHash(fp));
			}
			JsonSerializer js = new JsonSerializer
			{
				Formatting = Formatting.Indented,
				NullValueHandling = NullValueHandling.Ignore
			};
			using (var tw = File.CreateText(Path.Combine(path, Path.ChangeExtension(Path.GetFileName(filename), ".json"))))
				js.Serialize(tw, ini);
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
				modelbytes.AddRange(new ModelFile(Path.Combine(path, file)).Model.GetBytes((uint)(key + modelbytes.Count), false, labels, out uint _));
			foreach (string file in ini.Files.Where(a => a.Key.EndsWith(".saanim", StringComparison.OrdinalIgnoreCase) && HelperFunctions.FileHash(Path.Combine(path, a.Key)) != a.Value).Select(a => a.Key))
				modelbytes.AddRange(NJS_MOTION.Load(Path.Combine(path, file)).GetBytes((uint)(key + modelbytes.Count), labels, out uint _));
			fc = modelbytes.ToArray();
			int ptr = fc.GetPointer(8, key);
			if (ptr != 0)
			{
				MiniEventSonic info = ini.Sonic[0];
				if (labels.ContainsKeySafer(info.BodyAnims))
				ByteConverter.GetBytes(labels[info.BodyAnims]).CopyTo(fc, ptr);
				if (info.HeadPart != null)
				{
					if (labels.ContainsKeySafer(info.HeadPart))
						ByteConverter.GetBytes(labels[info.HeadPart]).CopyTo(fc, ptr + 4);
					if (labels.ContainsKeySafer(info.HeadAnims))
						ByteConverter.GetBytes(labels[info.HeadAnims]).CopyTo(fc, ptr + 8);
					if (labels.ContainsKeySafer(info.HeadShapeMotions))
						ByteConverter.GetBytes(labels[info.HeadShapeMotions]).CopyTo(fc, ptr + 0xC);
					if (labels.ContainsKeySafer(info.MouthPart))
						ByteConverter.GetBytes(labels[info.MouthPart]).CopyTo(fc, ptr + 0x10);
					if (labels.ContainsKeySafer(info.MouthAnims))
						ByteConverter.GetBytes(labels[info.MouthAnims]).CopyTo(fc, ptr + 0x14);
					if (labels.ContainsKeySafer(info.MouthShapeMotions))
						ByteConverter.GetBytes(labels[info.MouthShapeMotions]).CopyTo(fc, ptr + 0x18);
					if (labels.ContainsKeySafer(info.LHandPart))
						ByteConverter.GetBytes(labels[info.LHandPart]).CopyTo(fc, ptr + 0x1C);
					if (labels.ContainsKeySafer(info.LHandAnims))
						ByteConverter.GetBytes(labels[info.LHandAnims]).CopyTo(fc, ptr + 0x20);
					if (labels.ContainsKeySafer(info.LHandShapeMotions))
						ByteConverter.GetBytes(labels[info.LHandShapeMotions]).CopyTo(fc, ptr + 0x24);
					if (labels.ContainsKeySafer(info.RHandPart))
						ByteConverter.GetBytes(labels[info.RHandPart]).CopyTo(fc, ptr + 0x28);
					if (labels.ContainsKeySafer(info.RHandAnims))
						ByteConverter.GetBytes(labels[info.RHandAnims]).CopyTo(fc, ptr + 0x2C);
					if (labels.ContainsKeySafer(info.RHandShapeMotions))
						ByteConverter.GetBytes(labels[info.RHandShapeMotions]).CopyTo(fc, ptr + 0x30);
				}
			}
			ptr = fc.GetPointer(0xC, key);
			if (ptr != 0)
			{
				MiniEventShadow info = ini.Shadow[0];
				if (labels.ContainsKeySafer(info.BodyAnims))
					ByteConverter.GetBytes(labels[info.BodyAnims]).CopyTo(fc, ptr);
				if (info.HeadPart != null)
				{
					if (labels.ContainsKeySafer(info.HeadPart))
						ByteConverter.GetBytes(labels[info.HeadPart]).CopyTo(fc, ptr + 4);
					if (labels.ContainsKeySafer(info.HeadAnims))
						ByteConverter.GetBytes(labels[info.HeadAnims]).CopyTo(fc, ptr + 8);
					if (labels.ContainsKeySafer(info.HeadShapeMotions))
						ByteConverter.GetBytes(labels[info.HeadShapeMotions]).CopyTo(fc, ptr + 0xC);
					if (labels.ContainsKeySafer(info.MouthPart))
						ByteConverter.GetBytes(labels[info.MouthPart]).CopyTo(fc, ptr + 0x10);
					if (labels.ContainsKeySafer(info.MouthAnims))
						ByteConverter.GetBytes(labels[info.MouthAnims]).CopyTo(fc, ptr + 0x14);
					if (labels.ContainsKeySafer(info.MouthShapeMotions))
						ByteConverter.GetBytes(labels[info.MouthShapeMotions]).CopyTo(fc, ptr + 0x18);
					if (labels.ContainsKeySafer(info.LHandPart))
						ByteConverter.GetBytes(labels[info.LHandPart]).CopyTo(fc, ptr + 0x1C);
					if (labels.ContainsKeySafer(info.LHandAnims))
						ByteConverter.GetBytes(labels[info.LHandAnims]).CopyTo(fc, ptr + 0x20);
					if (labels.ContainsKeySafer(info.LHandShapeMotions))
						ByteConverter.GetBytes(labels[info.LHandShapeMotions]).CopyTo(fc, ptr + 0x24);
					if (labels.ContainsKeySafer(info.RHandPart))
						ByteConverter.GetBytes(labels[info.RHandPart]).CopyTo(fc, ptr + 0x28);
					if (labels.ContainsKeySafer(info.RHandAnims))
						ByteConverter.GetBytes(labels[info.RHandAnims]).CopyTo(fc, ptr + 0x2C);
					if (labels.ContainsKeySafer(info.RHandShapeMotions))
						ByteConverter.GetBytes(labels[info.RHandShapeMotions]).CopyTo(fc, ptr + 0x30);
				}
			}
			ptr = fc.GetPointer(0x18, key);
			if (ptr != 0)
			{
				MiniEventKnux info = ini.Knuckles[0];
				if (labels.ContainsKeySafer(info.BodyAnims))
					ByteConverter.GetBytes(labels[info.BodyAnims]).CopyTo(fc, ptr);
				if (info.HeadPart != null)
				{
					if (labels.ContainsKeySafer(info.HeadPart))
						ByteConverter.GetBytes(labels[info.HeadPart]).CopyTo(fc, ptr + 4);
					if (labels.ContainsKeySafer(info.HeadAnims))
						ByteConverter.GetBytes(labels[info.HeadAnims]).CopyTo(fc, ptr + 8);
					if (labels.ContainsKeySafer(info.HeadShapeMotions))
						ByteConverter.GetBytes(labels[info.HeadShapeMotions]).CopyTo(fc, ptr + 0xC);
					if (labels.ContainsKeySafer(info.MouthPart))
						ByteConverter.GetBytes(labels[info.MouthPart]).CopyTo(fc, ptr + 0x10);
					if (labels.ContainsKeySafer(info.MouthAnims))
						ByteConverter.GetBytes(labels[info.MouthAnims]).CopyTo(fc, ptr + 0x14);
					if (labels.ContainsKeySafer(info.MouthShapeMotions))
						ByteConverter.GetBytes(labels[info.MouthShapeMotions]).CopyTo(fc, ptr + 0x18);
					if (labels.ContainsKeySafer(info.LHandPart))
						ByteConverter.GetBytes(labels[info.LHandPart]).CopyTo(fc, ptr + 0x1C);
					if (labels.ContainsKeySafer(info.LHandAnims))
						ByteConverter.GetBytes(labels[info.LHandAnims]).CopyTo(fc, ptr + 0x20);
					if (labels.ContainsKeySafer(info.LHandShapeMotions))
						ByteConverter.GetBytes(labels[info.LHandShapeMotions]).CopyTo(fc, ptr + 0x24);
					if (labels.ContainsKeySafer(info.RHandPart))
						ByteConverter.GetBytes(labels[info.RHandPart]).CopyTo(fc, ptr + 0x28);
					if (labels.ContainsKeySafer(info.RHandAnims))
						ByteConverter.GetBytes(labels[info.RHandAnims]).CopyTo(fc, ptr + 0x2C);
					if (labels.ContainsKeySafer(info.RHandShapeMotions))
						ByteConverter.GetBytes(labels[info.RHandShapeMotions]).CopyTo(fc, ptr + 0x30);
				}
			}
			ptr = fc.GetPointer(0x1C, key);
			if (ptr != 0)
			{
				MiniEventRouge info = ini.Rouge[0];
				if (labels.ContainsKeySafer(info.BodyAnims))
					ByteConverter.GetBytes(labels[info.BodyAnims]).CopyTo(fc, ptr);
				if (info.HeadPart != null)
				{
					if (labels.ContainsKeySafer(info.HeadPart))
						ByteConverter.GetBytes(labels[info.HeadPart]).CopyTo(fc, ptr + 4);
					if (labels.ContainsKeySafer(info.HeadAnims))
						ByteConverter.GetBytes(labels[info.HeadAnims]).CopyTo(fc, ptr + 8);
					if (labels.ContainsKeySafer(info.HeadShapeMotions))
						ByteConverter.GetBytes(labels[info.HeadShapeMotions]).CopyTo(fc, ptr + 0xC);
					if (labels.ContainsKeySafer(info.MouthPart))
						ByteConverter.GetBytes(labels[info.MouthPart]).CopyTo(fc, ptr + 0x10);
					if (labels.ContainsKeySafer(info.MouthAnims))
						ByteConverter.GetBytes(labels[info.MouthAnims]).CopyTo(fc, ptr + 0x14);
					if (labels.ContainsKeySafer(info.MouthShapeMotions))
						ByteConverter.GetBytes(labels[info.MouthShapeMotions]).CopyTo(fc, ptr + 0x18);
					if (labels.ContainsKeySafer(info.LHandPart))
						ByteConverter.GetBytes(labels[info.LHandPart]).CopyTo(fc, ptr + 0x1C);
					if (labels.ContainsKeySafer(info.LHandAnims))
						ByteConverter.GetBytes(labels[info.LHandAnims]).CopyTo(fc, ptr + 0x20);
					if (labels.ContainsKeySafer(info.LHandShapeMotions))
						ByteConverter.GetBytes(labels[info.LHandShapeMotions]).CopyTo(fc, ptr + 0x24);
					if (labels.ContainsKeySafer(info.RHandPart))
						ByteConverter.GetBytes(labels[info.RHandPart]).CopyTo(fc, ptr + 0x28);
					if (labels.ContainsKeySafer(info.RHandAnims))
						ByteConverter.GetBytes(labels[info.RHandAnims]).CopyTo(fc, ptr + 0x2C);
					if (labels.ContainsKeySafer(info.RHandShapeMotions))
						ByteConverter.GetBytes(labels[info.RHandShapeMotions]).CopyTo(fc, ptr + 0x30);
				}
			}
			ptr = fc.GetPointer(0x24, key);
			if (ptr != 0 && labels.ContainsKeySafer(ini.MechEggmanBodyAnims))
				ByteConverter.GetBytes(labels[ini.MechEggmanBodyAnims]).CopyTo(fc, ptr);
			ptr = fc.GetPointer(4, key);
			if (ptr != 0 && labels.ContainsKeySafer(ini.Camera))
			{
				ByteConverter.GetBytes(labels[ini.Camera]).CopyTo(fc, 4);
				//ByteConverter.GetBytes(labels[ini.Camera]).CopyTo(fc, ptr + 0x10);
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
		public string Camera { get; set; }
		//public int CamFrames { get; set; }
		public List<MiniEventSonic> Sonic { get; set; } = new List<MiniEventSonic>();
		public List<MiniEventShadow> Shadow { get; set; } = new List<MiniEventShadow>();
		public List<MiniEventKnux> Knuckles { get; set; } = new List<MiniEventKnux>();
		public List<MiniEventRouge> Rouge { get; set; } = new List<MiniEventRouge>();
		public string MechEggmanBodyAnims { get; set; }
		public List<string> Motions { get; set; }
	}

	public class MiniEventSonic
	{
		public string BodyAnims { get; set; }
		public string HeadPart { get; set; }
		public string HeadAnims { get; set; }
		public string HeadShapeMotions { get; set; }
		public string MouthPart { get; set; }
		public string MouthAnims { get; set; }
		public string MouthShapeMotions { get; set; }
		public string LHandPart { get; set; }
		public string LHandAnims { get; set; }
		public string LHandShapeMotions { get; set; }
		public string RHandPart { get; set; }
		public string RHandAnims { get; set; }
		public string RHandShapeMotions { get; set; }
	}

	public class MiniEventShadow
	{
		public string BodyAnims { get; set; }
		public string HeadPart { get; set; }
		public string HeadAnims { get; set; }
		public string HeadShapeMotions { get; set; }
		public string MouthPart { get; set; }
		public string MouthAnims { get; set; }
		public string MouthShapeMotions { get; set; }
		public string LHandPart { get; set; }
		public string LHandAnims { get; set; }
		public string LHandShapeMotions { get; set; }
		public string RHandPart { get; set; }
		public string RHandAnims { get; set; }
		public string RHandShapeMotions { get; set; }
	}

	public class MiniEventKnux
	{
		public string BodyAnims { get; set; }
		public string HeadPart { get; set; }
		public string HeadAnims { get; set; }
		public string HeadShapeMotions { get; set; }
		public string MouthPart { get; set; }
		public string MouthAnims { get; set; }
		public string MouthShapeMotions { get; set; }
		public string LHandPart { get; set; }
		public string LHandAnims { get; set; }
		public string LHandShapeMotions { get; set; }
		public string RHandPart { get; set; }
		public string RHandAnims { get; set; }
		public string RHandShapeMotions { get; set; }
	}

	public class MiniEventRouge
	{
		public string BodyAnims { get; set; }
		public string HeadPart { get; set; }
		public string HeadAnims { get; set; }
		public string HeadShapeMotions { get; set; }
		public string MouthPart { get; set; }
		public string MouthAnims { get; set; }
		public string MouthShapeMotions { get; set; }
		public string LHandPart { get; set; }
		public string LHandAnims { get; set; }
		public string LHandShapeMotions { get; set; }
		public string RHandPart { get; set; }
		public string RHandAnims { get; set; }
		public string RHandShapeMotions { get; set; }
	}
}

