using System.Collections;
using System.IO;

namespace SASave
{
	class SaveData
	{
		public uint PlayTime { get; set; }
		public int[] LevelScores { get; private set; }
		public LevelTime[] LevelTimes { get; private set; }
		public ushort[][] LevelWeights { get; private set; }
		public short[] LevelRings { get; private set; }
		public int[][] SkyChase1Scores { get; private set; }
		public int[][] SkyChase2Scores { get; private set; }
		public int[][] IceCapScores { get; private set; }
		public int[][] SandHillScores { get; private set; }
		public int[] HedgehogHammerScores { get; private set; }
		public CircuitData[] TwinkleCircuitTimes { get; private set; }
		public LevelTime[][] BossTimes { get; private set; }
		public BitArray Emblems { get; private set; }
		public MessageOptions MessageOption { get; set; }
		public VoiceLanguages VoiceLanguage { get; set; }
		public TextLanguages TextLanguage { get; set; }
		public sbyte[] Lives { get; private set; }
		public byte LastCharacter { get; set; }
		public bool Rumble { get; set; }
		public short LastLevel { get; set; }
		public BitArray EventFlags { get; private set; }
		public BitArray NPCFlags { get; private set; }
		public AdventureData[] AdventureModeData { get; private set; }
		public byte[][] LevelClearCounts { get; private set; }
		public MissionStatus[] Missions { get; private set; }
		public int BlackMarketRings { get; set; }
		public int[] MetalLevelScores { get; private set; }
		public LevelTime[] MetalLevelTimes { get; private set; }
		public short[] MetalLevelRings { get; private set; }
		public int[] MetalIceCapScores { get; private set; }
		public int[] MetalSandHillScores { get; private set; }
		public CircuitData MetalTwinkleCircuitTimes { get; private set; }
		public LevelTime[] MetalBossTimes { get; private set; }
		public BitArray MetalEmblems { get; private set; }

		public SaveData()
		{
			LevelScores = new int[32];
			LevelTimes = new LevelTime[28];
			for (int i = 0; i < 28; i++)
				LevelTimes[i] = new LevelTime();
			LevelWeights = new ushort[4][];
			for (int i = 0; i < 4; i++)
				LevelWeights[i] = new ushort[3];
			LevelRings = new short[32];
			SkyChase1Scores = new int[2][];
			for (int i = 0; i < 2; i++)
				SkyChase1Scores[i] = new int[3];
			SkyChase2Scores = new int[2][];
			for (int i = 0; i < 2; i++)
				SkyChase2Scores[i] = new int[3];
			IceCapScores = new int[2][];
			for (int i = 0; i < 2; i++)
				IceCapScores[i] = new int[3];
			SandHillScores = new int[2][];
			for (int i = 0; i < 2; i++)
				SandHillScores[i] = new int[3];
			HedgehogHammerScores = new int[3];
			TwinkleCircuitTimes = new CircuitData[6];
			for (int i = 0; i < 6; i++)
				TwinkleCircuitTimes[i] = new CircuitData();
			BossTimes = new LevelTime[6][];
			for (int i = 0; i < 6; i++)
				BossTimes[i] = new LevelTime[3];
			Emblems = new BitArray(136);
			Lives = new sbyte[7];
			EventFlags = new BitArray(512);
			NPCFlags = new BitArray(512);
			AdventureModeData = new AdventureData[8];
			for (int i = 0; i < 8; i++)
				AdventureModeData[i] = new AdventureData();
			LevelClearCounts = new byte[8][];
			for (int i = 0; i < 8; i++)
				LevelClearCounts[i] = new byte[43];
			Missions = new MissionStatus[60];
			for (int i = 0; i < 60; i++)
				Missions[i] = new MissionStatus();
			MetalLevelScores = new int[10];
			MetalLevelTimes = new LevelTime[10];
			for (int i = 0; i < 10; i++)
				MetalLevelTimes[i] = new LevelTime();
			MetalLevelRings = new short[10];
			MetalIceCapScores = new int[3];
			MetalSandHillScores = new int[3];
			MetalTwinkleCircuitTimes = new CircuitData();
			MetalBossTimes = new LevelTime[3];
			MetalEmblems = new BitArray(32);
		}

		public SaveData(byte[] data)
			: this()
		{
			BinaryReader file = new BinaryReader(new MemoryStream(data));
			file.ReadInt32();
			PlayTime = file.ReadUInt32();
			for (int i = 0; i < 32; i++)
				LevelScores[i] = file.ReadInt32();
			for (int i = 0; i < 28; i++)
				LevelTimes[i] = new LevelTime(file.ReadBytes(LevelTime.Size));
			for (int i = 0; i < 4; i++)
				for (int j = 0; j < 3; j++)
					LevelWeights[i][j] = file.ReadUInt16();
			file.BaseStream.Seek(0x104, SeekOrigin.Begin);
			for (int i = 0; i < 32; i++)
				LevelRings[i] = file.ReadInt16();
			for (int i = 0; i < 2; i++)
				for (int j = 0; j < 3; j++)
					SkyChase1Scores[i][j] = file.ReadInt32();
			for (int i = 0; i < 2; i++)
				for (int j = 0; j < 3; j++)
					SkyChase2Scores[i][j] = file.ReadInt32();
			for (int i = 0; i < 2; i++)
				for (int j = 0; j < 3; j++)
					IceCapScores[i][j] = file.ReadInt32();
			for (int i = 0; i < 2; i++)
				for (int j = 0; j < 3; j++)
					SandHillScores[i][j] = file.ReadInt32();
			for (int i = 0; i < 3; i++)
				HedgehogHammerScores[i] = file.ReadInt32();
			for (int i = 0; i < 6; i++)
				TwinkleCircuitTimes[i] = new CircuitData(file.ReadBytes(CircuitData.Size));
			for (int i = 0; i < 6; i++)
				for (int j = 0; j < 3; j++)
					BossTimes[i][j] = new LevelTime(file.ReadBytes(LevelTime.Size));
			Emblems = new BitArray(file.ReadBytes(17));
			byte options = file.ReadByte();
			MessageOption = (MessageOptions)((options >> 1) & 1);
			VoiceLanguage = (VoiceLanguages)((options >> 2) & 3);
			TextLanguage = (TextLanguages)((options >> 4) & 7);
			for (int i = 0; i < 7; i++)
				Lives[i] = file.ReadSByte();
			LastCharacter = file.ReadByte();
			Rumble = file.ReadBoolean();
			file.ReadByte();
			LastLevel = file.ReadInt16();
			file.ReadInt16();
			EventFlags = new BitArray(file.ReadBytes(64));
			NPCFlags = new BitArray(file.ReadBytes(64));
			file.BaseStream.Seek(0x2E8, SeekOrigin.Begin);
			for (int i = 0; i < 8; i++)
				AdventureModeData[i] = new AdventureData(file.ReadBytes(AdventureData.Size));
			for (int i = 0; i < 8; i++)
				LevelClearCounts[i] = file.ReadBytes(43);
			for (int i = 0; i < 60; i++)
				Missions[i] = new MissionStatus(file.ReadByte());
			BlackMarketRings = file.ReadInt32();
			for (int i = 0; i < 10; i++)
				MetalLevelScores[i] = file.ReadInt32();
			for (int i = 0; i < 10; i++)
				MetalLevelTimes[i] = new LevelTime(file.ReadBytes(LevelTime.Size));
			for (int i = 0; i < 10; i++)
				MetalLevelRings[i] = file.ReadInt16();
			file.ReadInt16();
			for (int i = 0; i < 3; i++)
				MetalIceCapScores[i] = file.ReadInt32();
			for (int i = 0; i < 3; i++)
				MetalSandHillScores[i] = file.ReadInt32();
			MetalTwinkleCircuitTimes = new CircuitData(file.ReadBytes(CircuitData.Size));
			for (int i = 0; i < 3; i++)
				MetalBossTimes[i] = new LevelTime(file.ReadBytes(LevelTime.Size));
			MetalEmblems = new BitArray(file.ReadBytes(4));
			file.Close();
		}

		public byte[] GetBytes()
		{
			MemoryStream buffer = new MemoryStream(0x570);
			BinaryWriter writer = new BinaryWriter(buffer);
			writer.Write(0);
			writer.Write(PlayTime);
			for (int i = 0; i < 32; i++)
				writer.Write(LevelScores[i]);
			for (int i = 0; i < 28; i++)
				writer.Write(LevelTimes[i]);
			for (int i = 0; i < 4; i++)
				for (int j = 0; j < 3; j++)
					writer.Write(LevelWeights[i][j]);
			writer.Write(new byte[16]);
			for (int i = 0; i < 32; i++)
				writer.Write(LevelRings[i]);
			for (int i = 0; i < 2; i++)
				for (int j = 0; j < 3; j++)
					writer.Write(SkyChase1Scores[i][j]);
			for (int i = 0; i < 2; i++)
				for (int j = 0; j < 3; j++)
					writer.Write(SkyChase2Scores[i][j]);
			for (int i = 0; i < 2; i++)
				for (int j = 0; j < 3; j++)
					writer.Write(IceCapScores[i][j]);
			for (int i = 0; i < 2; i++)
				for (int j = 0; j < 3; j++)
					writer.Write(SandHillScores[i][j]);
			for (int i = 0; i < 3; i++)
				writer.Write(HedgehogHammerScores[i]);
			for (int i = 0; i < 6; i++)
				writer.Write(TwinkleCircuitTimes[i]);
			for (int i = 0; i < 6; i++)
				for (int j = 0; j < 3; j++)
					writer.Write(BossTimes[i][j]);
			byte[] emblems = new byte[17];
			for (int i = 0; i < 130; i++)
				emblems[i / 8] |= (byte)(Emblems[i] ? 1 << (i % 8) : 0);
			writer.Write(emblems);
			byte options = 0;
			options |= (byte)((int)MessageOption << 1);
			options |= (byte)((int)VoiceLanguage << 2);
			options |= (byte)((int)TextLanguage << 4);
			writer.Write(options);
			for (int i = 0; i < 7; i++)
				writer.Write(Lives[i]);
			writer.Write(LastCharacter);
			writer.Write(Rumble);
			writer.Write((byte)0);
			writer.Write(LastLevel);
			writer.Write((short)0);
			byte[] events = new byte[64];
			for (int i = 0; i < 512; i++)
				events[i / 8] |= (byte)(EventFlags[i] ? 1 << (i % 8) : 0);
			writer.Write(events);
			byte[] npcs = new byte[64];
			for (int i = 0; i < 512; i++)
				npcs[i / 8] |= (byte)(NPCFlags[i] ? 1 << (i % 8) : 0);
			writer.Write(npcs);
			writer.Write(0L);
			for (int i = 0; i < 8; i++)
				writer.Write(AdventureModeData[i]);
			for (int i = 0; i < 8; i++)
				writer.Write(LevelClearCounts[i]);
			for (int i = 0; i < 60; i++)
				writer.Write(Missions[i]);
			writer.Write(BlackMarketRings);
			for (int i = 0; i < 10; i++)
				writer.Write(MetalLevelScores[i]);
			for (int i = 0; i < 10; i++)
				writer.Write(MetalLevelTimes[i]);
			for (int i = 0; i < 10; i++)
				writer.Write(MetalLevelRings[i]);
			writer.Write((short)0);
			for (int i = 0; i < 3; i++)
				writer.Write(MetalIceCapScores[i]);
			for (int i = 0; i < 3; i++)
				writer.Write(MetalSandHillScores[i]);
			writer.Write(MetalTwinkleCircuitTimes);
			for (int i = 0; i < 3; i++)
				writer.Write(MetalBossTimes[i]);
			byte[] metalemblems = new byte[4];
			for (int i = 0; i < 30; i++)
				metalemblems[i / 8] |= (byte)(MetalEmblems[i] ? 1 << (i % 8) : 0);
			writer.Write(metalemblems);
			byte[] result = buffer.ToArray();
			writer.Close();
			return result;
		}
	}

	class LevelTime
	{
		public byte Minutes { get; set; }
		public byte Seconds { get; set; }
		public byte Frames { get; set; }

		public const int Size = 3;

		public LevelTime() { }

		public LevelTime(byte minutes, byte seconds, byte frames)
		{
			Minutes = minutes;
			Seconds = seconds;
			Frames = frames;
		}

		public LevelTime(byte[] data)
		{
			Minutes = data[0];
			Seconds = data[1];
			Frames = data[2];
		}

		public byte[] GetBytes()
		{
			return new byte[] { Minutes, Seconds, Frames };
		}

		public static implicit operator byte[] (LevelTime data)
		{
			return data.GetBytes();
		}
	}

	class CircuitTime
	{
		public byte Minutes { get; set; }
		public byte Seconds { get; set; }
		public byte Centiseconds { get; set; }

		public const int Size = 3;

		public CircuitTime() { }

		public CircuitTime(byte minutes, byte seconds, byte centiseconds)
		{
			Minutes = minutes;
			Seconds = seconds;
			Centiseconds = centiseconds;
		}

		public CircuitTime(byte[] data)
		{
			Minutes = data[0];
			Seconds = data[1];
			Centiseconds = data[2];
		}

		public byte[] GetBytes()
		{
			return new byte[] { Minutes, Seconds, Centiseconds };
		}

		public static implicit operator byte[] (CircuitTime data)
		{
			return data.GetBytes();
		}
	}

	class CircuitData
	{
		public CircuitTime[] BestTimes { get; private set; }
		public CircuitTime Lap1Time { get; set; }
		public CircuitTime Lap2Time { get; set; }

		public const int Size = CircuitTime.Size * 5;

		public CircuitData()
		{
			BestTimes = new CircuitTime[3];
		}

		public CircuitData(byte[] data)
			: this()
		{
			BinaryReader file = new BinaryReader(new MemoryStream(data));
			for (int i = 0; i < 3; i++)
				BestTimes[i] = new CircuitTime(file.ReadBytes(CircuitTime.Size));
			Lap1Time = new CircuitTime(file.ReadBytes(CircuitTime.Size));
			Lap2Time = new CircuitTime(file.ReadBytes(CircuitTime.Size));
			file.Close();
		}

		public byte[] GetBytes()
		{
			MemoryStream buffer = new MemoryStream(Size);
			BinaryWriter writer = new BinaryWriter(buffer);
			for (int i = 0; i < 3; i++)
				writer.Write(BestTimes[i].GetBytes());
			writer.Write(Lap1Time.GetBytes());
			writer.Write(Lap2Time.GetBytes());
			byte[] result = buffer.ToArray();
			writer.Close();
			return result;
		}

		public static implicit operator byte[] (CircuitData data)
		{
			return data.GetBytes();
		}
	}

	enum MessageOptions
	{
		VoiceAndText,
		VoiceOnly
	}

	enum VoiceLanguages
	{
		Default,
		Japanese,
		English
	}

	enum TextLanguages
	{
		Default,
		Japanese,
		English,
		French,
		Spanish,
		German
	}

	class AdventureData
	{
		public TimesOfDay TimeOfDay { get; set; }
		public short CurrentSequence { get; set; }
		public short NextSequence { get; set; }
		public short Entrance { get; set; }
		public ushort Level { get; set; }
		public short Destination { get; set; }

		public const int Size = 12;

		public AdventureData() { }

		public AdventureData(byte[] data)
		{
			BinaryReader file = new BinaryReader(new MemoryStream(data));
			TimeOfDay = (TimesOfDay)file.ReadByte();
			file.ReadByte();
            CurrentSequence = file.ReadInt16();
            NextSequence = file.ReadInt16();
			Entrance = file.ReadInt16();
			Level = file.ReadUInt16();
            Destination = file.ReadInt16();
			file.Close();
		}

		public byte[] GetBytes()
		{
			MemoryStream buffer = new MemoryStream(Size);
			BinaryWriter writer = new BinaryWriter(buffer);
			writer.Write((byte)TimeOfDay);
			writer.Write((byte)0);
			writer.Write(CurrentSequence);
			writer.Write(NextSequence);
			writer.Write(Entrance);
			writer.Write(Level);
			writer.Write(Destination);
			byte[] result = buffer.ToArray();
			writer.Close();
			return result;
		}

		public static implicit operator byte[] (AdventureData data)
		{
			return data.GetBytes();
		}
	}

	enum TimesOfDay
	{
		Day,
		Evening,
		Night
	}

	class MissionStatus
	{
		public bool Unlocked { get; set; }
		public bool Active { get; set; }
		public bool Complete { get; set; }

		public MissionStatus() { }

		public MissionStatus(byte data)
		{
			Unlocked = (data & 0x40) == 0x40;
			Active = (data & 1) == 1;
			Complete = (data & 0x80) == 0x80;
		}

		public byte ToByte()
		{
			byte result = 0;
			if (Active) result |= 1;
			if (Unlocked) result |= 0x40;
			if (Complete) result |= 0x80;
			return result;
		}

		public static implicit operator byte(MissionStatus data)
		{
			return data.ToByte();
		}
	}
}