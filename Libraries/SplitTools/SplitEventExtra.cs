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
				bool battle;
				if (fc[4] > 0 || fc[8] > 0 || fc[0x26800] > 0)
				{
					Console.WriteLine("File is in DC format.");
					ByteConverter.BigEndian = false;
					ini.Game = Game.SA2;
					battle = false;
					ini.BigEndian = false;
				}
				else
				{
					Console.WriteLine("File is in GC/PC format.");
					ByteConverter.BigEndian = true;
					ini.Game = Game.SA2B;
					battle = true;
					ini.BigEndian = true;
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
					audio.VoiceEntry1 = ByteConverter.ToInt16(fc, address + 4).ToCHex();
					audio.VoiceEntry2 = ByteConverter.ToInt16(fc, address + 6).ToCHex();
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
					screen.Type = fc[address + 4];
					if (battle)
					{
						screen.A = fc[address + 8];
						screen.R = fc[address + 9];
						screen.G = fc[address + 0xA];
						screen.B = fc[address + 0xB];
					}
					else
					{
						screen.B = fc[address + 8];
						screen.G = fc[address + 9];
						screen.R = fc[address + 0xA];
						screen.A = fc[address + 0xB];
					}
					ini.ScreenEffects.Add(screen);
				}
				if (screencount != 0)
					Console.WriteLine("Event contains {0} active screen effect entr{1}.", screencount, screencount == 1 ? "y" : "ies");
				else
					Console.WriteLine("Event does not use screen effects.");

				int misc1count = 0;
				for (int i = 0; i < 2048; i++)
				{
					address = 0xA800 + (0x38 * i);
					MiscEffects1 misc1 = new MiscEffects1();
					misc1.FrameStart = ByteConverter.ToUInt32(fc, address);
					if (misc1.FrameStart != 0)
						misc1count++;
					misc1.Type1 = fc[address + 4];
					misc1.Type2 = fc[address + 5];
					misc1.Color = new Vertex(fc, address + 8);
					misc1.Intensity = ByteConverter.ToSingle(fc, address + 0x14);
					ini.MiscEffects1.Add(misc1);
				}
				if (misc1count != 0)
					Console.WriteLine("Event contains {0} active unknown effect entr{1}.", misc1count, misc1count == 1 ? "y" : "ies");
				else
					Console.WriteLine("Event does not use unknown effects.");

				int lightcount = 0;
				for (int i = 0; i < 1084; i++)
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

				//add a buffer of 0x10 between these two chunks for build operations

				int misc2count = 0;
				for (int i = 0; i < 40; i++)
				{
					address = 0x38800 + (0x40 * i);
					MiscEffects2 misc2 = new MiscEffects2();
					misc2.Unk1 = new Vertex(fc, address);
					misc2.Unk2 = new Vertex(fc, address + 0xC);
					misc2.Unk3 = new Vertex(fc, address + 0x18);
					misc2.Unk4 = new Vertex(fc, address + 0x24);
					misc2.Unk5 = ByteConverter.ToInt32(fc, address + 0x30);
					misc2.Unk6 = ByteConverter.ToInt32(fc, address + 0x34);
					misc2.Unk7 = ByteConverter.ToInt32(fc, address + 0x38);
					misc2.Unk8 = ByteConverter.ToInt32(fc, address + 0x3C);
					if (misc2.Unk5 > 0 || misc2.Unk6 > 0 || misc2.Unk7 > 0 || misc2.Unk8 > 0)
						misc2count++;
					ini.MiscEffects2.Add(misc2);
				}
				if (misc2count != 0)
					Console.WriteLine("Event contains {0} active unknown float-based effect entr{1}.", misc2count, misc2count == 1 ? "y" : "ies");
				else
					Console.WriteLine("Event does not use unknown float-based effects.");

				int videocount = 0;
				for (int i = 0; i < 40; i++)
				{
					address = 0x39800 + (0x40 * i);
					VideoInfo video = new VideoInfo();
					video.FrameStart = ByteConverter.ToUInt32(fc, address);
					if (video.FrameStart != 0)
						videocount++;
					video.VideoType = ByteConverter.ToUInt16(fc, address + 0x4);
					video.Unknown = ByteConverter.ToUInt16(fc, address + 0x6);
					video.Depth = ByteConverter.ToSingle(fc, address + 0x8);
					video.OverlayType = fc[address + 0xC];
					video.OverlayType2 = fc[address + 0xD];
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
					audio.VoiceEntry1 = ByteConverter.ToInt16(fc, address + 4).ToCHex();
					audio.VoiceEntry2 = ByteConverter.ToInt16(fc, address + 6).ToCHex();
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

		public static void Build(bool? isBigEndian, string evfilename)
		{
			byte[] fc;
			if (Path.GetExtension(evfilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				fc = Prs.Decompress(evfilename);
			else
				fc = File.ReadAllBytes(evfilename);
			string path = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(evfilename)), Path.GetFileNameWithoutExtension(evfilename));
			JsonSerializer js = new JsonSerializer();
			EventExtraIniData ini;
			using (TextReader tr = File.OpenText(Path.Combine(path, Path.ChangeExtension(Path.GetFileName(evfilename), ".json"))))
			using (JsonTextReader jtr = new JsonTextReader(tr))
				ini = js.Deserialize<EventExtraIniData>(jtr);
			//if (!isBigEndian.HasValue)
			//	ByteConverter.BigEndian = EventExtraIniData.BigEndian;
			//else
			//	ByteConverter.BigEndian = isBigEndian.Value;
			List<byte> evextra = new List<byte>();
			//evextra.AddRange(ByteConverter.GetBytes(SubtitleInfo));
			//evextra.AddRange(ByteConverter.GetBytes(AudioInfo));
			if (Path.GetExtension(evfilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				FraGag.Compression.Prs.Compress(evextra.ToArray(), evfilename);
			else
				File.WriteAllBytes(evfilename, evextra.ToArray());
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
		public bool BigEndian { get; set; }
		public List<SubtitleInfo> Subtitles { get; set; } = new List<SubtitleInfo>();
		public List<AudioInfo> AudioInfo { get; set; } = new List<AudioInfo>();
		public List<ScreenEffects> ScreenEffects { get; set; } = new List<ScreenEffects>();
		public List<MiscEffects1> MiscEffects1 { get; set; } = new List<MiscEffects1>();
		public List<LightingInfo> Lighting { get; set; } = new List<LightingInfo>();
		public List<MiscEffects2> MiscEffects2 { get; set; } = new List<MiscEffects2>();
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
		public string VoiceEntry1 { get; set; }
		public string VoiceEntry2 { get; set; }
		public string MusicEntry { get; set; }
	}

	public class ScreenEffects
	{
		public uint FrameStart { get; set; }
		public int Type { get; set; }
		public byte A { get; set; }
		public byte R { get; set; }
		public byte G { get; set; }
		public byte B { get; set; }
	}

	public class MiscEffects1
	{
		public uint FrameStart { get; set; }
		public byte Type1 { get; set; }
		public byte Type2 { get; set; }
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

	public class MiscEffects2
	{
		public Vertex Unk1 { get; set; }
		public Vertex Unk2 { get; set; }
		public Vertex Unk3 { get; set; }
		public Vertex Unk4 { get; set; }
		public int Unk5 { get; set; }
		public int Unk6 { get; set; }
		public int Unk7 { get; set; }
		public int Unk8 { get; set; }
	}

	public class VideoInfo
	{
		public uint FrameStart { get; set; }
		public ushort VideoType { get; set; }
		public ushort Unknown { get; set; }
		public float Depth { get; set; }
		public byte OverlayType { get; set; }
		public byte OverlayType2 { get; set; }
		public string VideoName { get; set; }
	}
}
