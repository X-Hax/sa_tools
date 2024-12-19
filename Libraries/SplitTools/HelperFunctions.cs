using SAModel;
using System;
using System.Buffers.Binary;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SplitTools
{
	public static class HelperFunctions
	{
		private static T ReadAs<T>(Span<byte> span, int byteIndex) where T : unmanaged
		{
			return MemoryMarshal.Read<T>(span[byteIndex..]);
		}
		
        private static uint SACompGC_GetData(uint numBits, ref SACompGCStatus data)
        {
	        uint retVal;

	        if (data.BitBufferRemaining < numBits)
	        {
		        var v6 = (int)(numBits - data.BitBufferRemaining);
		        retVal = data.BitBuffer << (int)(numBits - data.BitBufferRemaining);

		        if (data.ReadOffset != 0 || data.ChunkCount != 0)
		        {
			        var v9 = BinaryPrimitives.ReverseEndianness(ReadAs<uint>(data.InputBuffer, data.ReadHead + (int)data.ReadOffset));
			        data.ReadOffset += 4;

			        var clearBits = 32 - v6;
			        retVal |= v9 << clearBits >> clearBits;
			        data.BitBufferRemaining = (byte)clearBits;
			        data.BitBuffer = v9 >> v6;

			        if (data.ReadOffset == 0x8000)
			        {
				        data.ReadOffset = 0;
				        data.ReadHead += 0x8000;
				        if (data.ReadHead == data.EndOffset)
				        {
					        data.ReadHead = 0;
				        }
				        data.ChunkCount--;
			        }
		        }
	        }
	        else
	        {
		        var v4 = data.BitBuffer;
		        data.BitBufferRemaining -= (byte)numBits;
		        data.BitBuffer >>= (int)numBits;

		        var clearBits = (int)(32 - numBits);
		        retVal = v4 << clearBits >> clearBits;
	        }

	        return retVal;
        }
        
        private static void SACompGCStatus_Process(ref SACompGCStatus data)
        {
	        while (true)
	        {
		        if (SACompGC_GetData(1, ref data) != 0)
		        {
			        data.OutputBuffer[data.WriteHead] = (byte)SACompGC_GetData(8, ref data);
			        data.WriteHead++;
			        data.LengthLeft--;

			        if (data.LengthLeft == 0)
			        {
				        return;
			        }

			        continue;
		        }

		        // Perform RLE lookback
		        var copyIdx = (int)SACompGC_GetData(data.CopyOffsetBits, ref data) + 1;
		        var numBytes = SACompGC_GetData(data.CopySizeBits, ref data) + 2;
		        data.LengthLeft -= numBytes;

		        var lookbackHead = data.WriteHead - copyIdx;

		        if (!((int)data.LengthLeft < 0 || lookbackHead < 0))
		        {
			        for (int i = 0; i < numBytes; i++)
			        {
				        data.OutputBuffer[data.WriteHead++] = data.OutputBuffer[lookbackHead++];
			        }

			        if (data.LengthLeft == 0)
			        {
				        return;
			        }
		        }
		        else
		        {
			        // Invalid?
			        return;
		        }
	        }
        }
        
        private static uint SACompGC_GetDecompressedSize(byte[] inputBuffer, out byte[] dataStart)
        {
	        var saCompGcData = MemoryMarshal.Cast<byte, uint>(inputBuffer);

	        if (inputBuffer != null)
	        {
		        while (saCompGcData[0] != 0x6F436153)
		        {
			        saCompGcData = saCompGcData[1..];

			        // Do not read out of bounds
			        if (saCompGcData.Length == 0)
			        {
				        dataStart = null;
				        return 0;
			        }
		        }

		        dataStart = MemoryMarshal.Cast<uint, byte>(saCompGcData).ToArray();

		        return BinaryPrimitives.ReverseEndianness(saCompGcData[2]) & 0x0FFFFFFF;
	        }

	        dataStart = null;
	        return 0;
        }
        
        private static byte[] SACompGC_DecompressBuffer(byte[] input)
        {
	        if (input == null)
	        {
		        return [];
	        }

	        var size = SACompGC_GetDecompressedSize(input, out input);
	        var output = new byte[size];
		        
	        if (size == 0)
	        {
		        throw new ArgumentOutOfRangeException(nameof(input), "Empty file!");
	        }

	        SACompGCStatus data = new()
	        {
		        CopyOffsetBits = input[12],
		        CopySizeBits = input[13],
		        Field0x2D = (byte)(input[8] >> 6),
		        ReadOffset = 16,
		        LengthLeft = size,
		        Length = size,
		        ChunkCount = 0xFF, // -1 as byte
		        OutputBuffer = output,
		        WriteHead = 0,
		        InputBuffer = input,
		        ReadHead = 0,
		        EndOffset = (int)((size + 47) & 0xFFFFFFE0)
	        };

	        SACompGCStatus_Process(ref data);

	        return output.ToArray();
        }
        
        public static uint? SetupEXE(ref byte[] exefile)
		{
			if (ByteConverter.ToUInt16(exefile, 0) != 0x5A4D)
			{
				return null;
			}

			var ptr = ByteConverter.ToInt32(exefile, 0x3c);
			
			if (ByteConverter.ToInt32(exefile, ptr) != 0x4550) //PE\0\0
			{
				return null;
			}

			try
			{
				ptr += 4;
				var numSects = ByteConverter.ToUInt16(exefile, ptr + 2);
				ptr += 0x14;

				var imageBase = ByteConverter.ToUInt32(exefile, ptr + 28);
				
				if (imageBase != 0x82000000) // SADX X360 EXE doesn't like this
				{
					var result = new byte[ByteConverter.ToUInt32(exefile, ptr + 56)];
					Array.Copy(exefile, result, ByteConverter.ToUInt32(exefile, ptr + 60));
					ptr += 0xe0;
					
					for (var i = 0; i < numSects; i++)
					{
						Array.Copy(exefile, ByteConverter.ToInt32(exefile, ptr + (int)SectOffs.FAddr), result, ByteConverter.ToInt32(exefile, ptr + (int)SectOffs.VAddr), ByteConverter.ToInt32(exefile, ptr + (int)SectOffs.FSize));
						ptr += (int)SectOffs.Size;
					}
					
					exefile = result;
				}
				
				return imageBase;
			}
			catch 
			{
				return null;
			}
		}
        
		public static void FixRELPointers(byte[] file, uint imageBase = 0)
		{
			var header = new OsModuleHeader(file, 0);
			var sections = new OsSectionInfo[header.Info.NumSections];
			
			for (var i = 0; i < header.Info.NumSections; i++)
			{
				sections[i] = new OsSectionInfo(file, (int)header.Info.SectionInfoOffset + (i * 8));
			}

			var imports = new OsImportInfo[header.ImpSize / 8];
			
			for (var i = 0; i < imports.Length; i++)
			{
				imports[i] = new OsImportInfo(file, (int)header.ImpOffset + (i * 8));
			}

			var relAddr = (from import in imports where import.Id == header.Info.Id select (int)import.Offset).FirstOrDefault();

			var rel = new OsRel(file, relAddr);
			var dataAddr = 0;
				
			unchecked 
			{
				while (rel.Type != (byte)RelocTypes.R_DOLPHIN_END)
				{
					dataAddr += rel.Offset;
					var sectionBase = (uint)(sections[rel.Section].Offset & ~1);
					
					switch (rel.Type)
					{
						case 0x01: 
							ByteConverter.GetBytes(rel.Addend + sectionBase + imageBase).CopyTo(file, dataAddr);
							break;
						case 0x02:
							ByteConverter.GetBytes((ByteConverter.ToUInt32(file, dataAddr) & 0xFC000003) | ((rel.Addend + sectionBase) & 0x3FFFFFC) + imageBase).CopyTo(file, dataAddr);
							break;
						case 0x03:
						case 0x04:
							ByteConverter.GetBytes((ushort)(rel.Addend + sectionBase) + imageBase).CopyTo(file, dataAddr);
							break;
						case 0x05:
							ByteConverter.GetBytes((ushort)((rel.Addend + sectionBase) >> 16) + imageBase).CopyTo(file, dataAddr);
							break;
						case 0x06:
							ByteConverter.GetBytes((ushort)(((rel.Addend + sectionBase) >> 16) + (((rel.Addend + sectionBase) & 0x8000) == 0x8000 ? 1 : 0)) + imageBase).CopyTo(file, dataAddr);
							break;
						case 0x0A:
							ByteConverter.GetBytes((uint)((ByteConverter.ToUInt32(file, dataAddr) & 0xFC000003) | (((rel.Addend + sectionBase) - dataAddr) & 0x3FFFFFC)) + imageBase).CopyTo(file, dataAddr);
							break;
						case 0x00:
						case (byte)RelocTypes.R_DOLPHIN_NOP:
						case (byte)RelocTypes.R_DOLPHIN_END:
							break;
						case (byte)RelocTypes.R_DOLPHIN_SECTION:
							dataAddr = (int)sectionBase;
							break;
						default:
							throw new NotImplementedException();
					}
					relAddr += 8;
					rel = new OsRel(file, relAddr);
				}
			}
		}

		private static readonly System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

		public static string FileHash(string path, int rangeStart = 0, int rangeFinish = 0)
		{
			return FileHash(File.ReadAllBytes(path), rangeStart, rangeFinish);
		}

		public static string FileHash(byte[] file, int rangeStart = 0, int rangeFinish = 0)
		{
			if (rangeStart != 0 || rangeFinish != 0)
			{
				var newFile = new byte[rangeFinish - rangeStart];
				Array.Copy(file, rangeStart, newFile, 0, newFile.Length);
				file = newFile;
			}
			
			file = md5.ComputeHash(file);
			
			return file.Aggregate(string.Empty, (current, item) => current + item.ToString("x2"));
		}

		private static readonly Encoding jpenc = Encoding.GetEncoding(932);
		private static readonly Encoding krenc = Encoding.GetEncoding(949);
		private static readonly Encoding euenc = Encoding.GetEncoding(1252);

		public static bool KoreanMode = false;

		public static Encoding GetEncoding() { return KoreanMode? krenc : jpenc; }

		public static Encoding GetEncoding(Languages language) => GetEncoding(Game.SA1, language);

		public static Encoding GetEncoding(Game game, Languages language)
		{
			return language switch
			{
				Languages.Japanese => KoreanMode ? krenc : jpenc,
				Languages.English => game switch
				{
					Game.SA1 or Game.SADX => KoreanMode ? krenc : jpenc,
					Game.SA2 or Game.SA2B => euenc,
					_ => throw new ArgumentOutOfRangeException(nameof(game))
				},
				_ => euenc
			};
		}

		public static string GetCString(this byte[] file, int address)
		{
			return file.GetCString(address, jpenc);
		}

		public static int GetPointer(this byte[] file, int address, uint imageBase)
		{
			var tmp = ByteConverter.ToUInt32(file, address);
			
			if (tmp == 0)
			{
				return 0;
			}

			return (int)(tmp - imageBase);
		}

		public static string UnescapeNewlines(this string line)
		{
			var sb = new StringBuilder(line.Length);
			
			for (var c = 0; c < line.Length; c++)
			{
				switch (line[c])
				{
					case '\\': // Escape character
						if (c + 1 == line.Length)
						{
							goto default;
						}

						c++;
						switch (line[c])
						{
							case 'n': // Line feed
								sb.Append('\n');
								break;
							case 'r': // Carriage return
								sb.Append('\r');
								break;
							default: // Literal character
								sb.Append(line[c]);
								break;
						}
						break;
					default:
						sb.Append(line[c]);
						break;
				}
			}

			return sb.ToString();
		}

		public static string EscapeNewlines(this string line)
		{
			return line.Replace(@"\", @"\\").Replace("\n", @"\n").Replace("\r", @"\r");
		}

		public static string ToCHex(this ushort i)
		{
			if (i < 10)
			{
				return i.ToString(NumberFormatInfo.InvariantInfo);
			}

			return "0x" + i.ToString("X");
		}

		public static string ToC(this string str) => str.ToC(Languages.Japanese);

		public static string ToC(this string str, Languages language) => ToC(str, Game.SA1, language);

		public static string ToC(this string str, Game game, Languages language)
		{
			if (str == null)
			{
				return "NULL";
			}

			var enc = GetEncoding(game, language);
			var result = new StringBuilder("\"");
			
			foreach (var item in str)
			{
				switch (item)
				{
					case '\0':
						result.Append(@"\0");
						break;
					case '\a':
						result.Append(@"\a");
						break;
					case '\b':
						result.Append(@"\b");
						break;
					case '\f':
						result.Append(@"\f");
						break;
					case '\n':
						result.Append(@"\n");
						break;
					case '\r':
						result.Append(@"\r");
						break;
					case '\t':
						result.Append(@"\t");
						break;
					case '\v':
						result.Append(@"\v");
						break;
					case '"':
						result.Append(@"\""");
						break;
					case '\\':
						result.Append(@"\\");
						break;
					case < ' ':
						result.Append($@"\{Convert.ToString((short)item, 8).PadLeft(3, '0')}");
						break;
					case > '\x7F':
					{
						foreach (byte b in enc.GetBytes(item.ToString()))
							result.Append($@"\{Convert.ToString(b, 8).PadLeft(3, '0')}");
						break;
					}
					default:
						result.Append(item);
						break;
				}
			}
			
			result.Append('"');
			return result.ToString();
		}

		public static string ToComment(this string str)
		{
			return "/* " + str.ToCNoEncoding().Replace("*/", @"*\/") + " */";
		}

		private static string ToCNoEncoding(this string str)
		{
			if (str == null)
			{
				return "NULL";
			}

			var result = new StringBuilder("\"");
			
			foreach (var item in str)
			{
				switch (item)
				{
					case '\0':
						result.Append(@"\0");
						break;
					case '\a':
						result.Append(@"\a");
						break;
					case '\b':
						result.Append(@"\b");
						break;
					case '\f':
						result.Append(@"\f");
						break;
					case '\n':
						result.Append(@"\n");
						break;
					case '\r':
						result.Append(@"\r");
						break;
					case '\t':
						result.Append(@"\t");
						break;
					case '\v':
						result.Append(@"\v");
						break;
					case '"':
						result.Append(@"\""");
						break;
					case '\\':
						result.Append(@"\\");
						break;
					case < ' ':
						result.Append($@"\{Convert.ToString((short)item, 8).PadLeft(3, '0')}");
						break;
					default:
						result.Append(item);
						break;
				}
			}
			
			result.Append('"');
			return result.ToString();
		}

		public static string ToC<T>(this T item)
			where T : Enum
		{
			return item.ToC(typeof(T).Name);
		}

		public static string ToC<T>(this T item, string enumName)
			where T : Enum
		{
			var type = typeof(T);
			if (type.GetCustomAttributes(typeof(FlagsAttribute), false).Length == 0)
			{
				if (Enum.IsDefined(typeof(T), item))
				{
					return enumName + "_" + item;
				}

				return item.ToString();
			}

			var num = Convert.ToUInt64(item);
			var values = Array.ConvertAll((T[])Enum.GetValues(type), (a) => Convert.ToUInt64(a));
			var num2 = values.Length - 1;
			var stringBuilder = new StringBuilder();
			var flag = true;
			var num3 = num;
			
			while (num2 >= 0 && (num2 != 0 || values[num2] != 0uL))
			{
				if ((num & values[num2]) == values[num2])
				{
					num -= values[num2];
					if (!flag)
					{
						stringBuilder.Insert(0, " | ");
					}

					stringBuilder.Insert(0, enumName + "_" + Enum.GetName(type, values[num2]));
					flag = false;
				}
				num2--;
			}
			
			if (num != 0uL)
			{
				if (flag)
				{
					return item.ToString();
				}

				return stringBuilder + " | " + item;
			}
			
			if (num3 != 0uL)
			{
				return stringBuilder.ToString();
			}

			if (values.Length > 0 && values[0] == 0uL)
			{
				return enumName + "_" + Enum.GetName(type, 0);
			}

			return "0";
		}

		public static bool CheckBigEndianInt16(byte[] file, int address)
		{
			var bigEndState = ByteConverter.BigEndian;

			ByteConverter.BigEndian = true;
			var isBigEndian = BitConverter.ToUInt16(file, address) > ByteConverter.ToUInt16(file, address);

			ByteConverter.BigEndian = bigEndState;

			return isBigEndian;
		}
		public static bool CheckBigEndianInt32(byte[] file, int address)
		{
			var bigEndState = ByteConverter.BigEndian;

			ByteConverter.BigEndian = true;
			var isBigEndian = BitConverter.ToUInt32(file, address) > ByteConverter.ToUInt32(file, address);

			ByteConverter.BigEndian = bigEndState;

			return isBigEndian;
		}

		public static byte[] DecompressREL(byte[] file)
		{
			// Scan the array for the last instance of the "SaCompGC" string because there are some files with redundant headers
			var start = 0;
			var isCompressed = false;
			var bigEndian = ByteConverter.BigEndian;
			ByteConverter.BigEndian = true;
			
			for (var u = file.Length - 8; u >= 0; u--)
			{
				if (ByteConverter.ToUInt32(file, u) == 0x5361436F)// && BitConverter.ToUInt32(file, u + 4) == 0x4347706D)
				{
					start = u;
					isCompressed = true;
					break;
				}
			}
			
			if (!isCompressed)
			{
				return file;
			}

			var input = new byte[file.Length - start];
			Array.Copy(file, start, input, 0, input.Length);

			var decompBuf = SACompGC_DecompressBuffer(input);
			
			ByteConverter.BigEndian = bigEndian;
			return decompBuf;
		}
	}

	ref struct SACompGCStatus
	{
		public uint LengthLeft;
		public uint Length;
		public uint ReadOffset; // 0x8
		public uint BitBuffer; // 0xc
		public Span<byte> OutputBuffer;
		public int ReadHead;
		public Span<byte> InputBuffer;
		public int EndOffset; // 0x1c
		public int WriteHead; // byte offset instead of pointer

		public byte ChunkCount; // 0x28
		public byte BitBufferRemaining; // 0x29
		public byte CopyOffsetBits; //0x2a
		public byte CopySizeBits; //0x2b
		// special note: missing 0x2c for some reason. bad decomp?
		public byte Field0x2D;
	}
	
	enum SectOffs
	{
		VSize = 8,
		VAddr = 0xC,
		FSize = 0x10,
		FAddr = 0x14,
		Flags = 0x24,
		Size = 0x28
	}

	public enum SA1LevelIDs : byte
	{
		HedgehogHammer = 0,
		EmeraldCoast = 1,
		WindyValley = 2,
		TwinklePark = 3,
		SpeedHighway = 4,
		RedMountain = 5,
		SkyDeck = 6,
		LostWorld = 7,
		IceCap = 8,
		Casinopolis = 9,
		FinalEgg = 0xA,
		HotShelter = 0xC,
		Chaos0 = 0xF,
		Chaos2 = 0x10,
		Chaos4 = 0x11,
		Chaos6 = 0x12,
		PerfectChaos = 0x13,
		Chaos7 = 0x13,
		EggHornet = 0x14,
		EggWalker = 0x15,
		EggViper = 0x16,
		Zero = 0x17,
		E101 = 0x18,
		E101R = 0x19,
		StationSquare = 0x1A,
		EggCarrierOutside = 0x1D,
		EggCarrierInside = 0x20,
		MysticRuins = 0x21,
		Past = 0x22,
		TwinkleCircuit = 0x23,
		SkyChase1 = 0x24,
		SkyChase2 = 0x25,
		SandHill = 0x26,
		SSGarden = 0x27,
		ECGarden = 0x28,
		MRGarden = 0x29,
		ChaoRace = 0x2A,
		Invalid = 0x2B
	}

	public enum SA1Characters : byte
	{
		Sonic = 0,
		Eggman = 1,
		Tails = 2,
		Knuckles = 3,
		Tikal = 4,
		Amy = 5,
		Gamma = 6,
		Big = 7,
		MetalSonic = 8
	}

	[Flags]
	public enum SA1CharacterFlags
	{
		Sonic = 1 << SA1Characters.Sonic,
		Eggman = 1 << SA1Characters.Eggman,
		Tails = 1 << SA1Characters.Tails,
		Knuckles = 1 << SA1Characters.Knuckles,
		Tikal = 1 << SA1Characters.Tikal,
		Amy = 1 << SA1Characters.Amy,
		Gamma = 1 << SA1Characters.Gamma,
		Big = 1 << SA1Characters.Big
	}

	public enum SA2LevelIDs : byte
	{
		BasicTest = 0,
		KnucklesTest = 1,
		SonicTest = 2,
		GreenForest = 3,
		WhiteJungle = 4,
		PumpkinHill = 5,
		SkyRail = 6,
		AquaticMine = 7,
		SecurityHall = 8,
		PrisonLane = 9,
		MetalHarbor = 0xA,
		IronGate = 0xB,
		WeaponsBed = 0xC,
		CityEscape = 0xD,
		RadicalHighway = 0xE,
		WeaponsBed2P = 0xF,
		WildCanyon = 0x10,
		MissionStreet = 0x11,
		DryLagoon = 0x12,
		SonicVsShadow1 = 0x13,
		TailsVsEggman1 = 0x14,
		SandOcean = 0x15,
		CrazyGadget = 0x16,
		HiddenBase = 0x17,
		EternalEngine = 0x18,
		DeathChamber = 0x19,
		EggQuarters = 0x1A,
		LostColony = 0x1B,
		PyramidCave = 0x1C,
		TailsVsEggman2 = 0x1D,
		FinalRush = 0x1E,
		GreenHill = 0x1F,
		MeteorHerd = 0x20,
		KnucklesVsRouge = 0x21,
		CannonsCoreS = 0x22,
		CannonsCoreE = 0x23,
		CannonsCoreT = 0x24,
		CannonsCoreR = 0x25,
		CannonsCoreK = 0x26,
		MissionStreet2P = 0x27,
		FinalChase = 0x28,
		WildCanyon2P = 0x29,
		SonicVsShadow2 = 0x2A,
		CosmicWall = 0x2B,
		MadSpace = 0x2C,
		SandOcean2P = 0x2D,
		DryLagoon2P = 0x2E,
		PyramidRace = 0x2F,
		HiddenBase2P = 0x30,
		PoolQuest = 0x31,
		PlanetQuest = 0x32,
		DeckRace = 0x33,
		DowntownRace = 0x34,
		CosmicWall2P = 0x35,
		GrindRace = 0x36,
		LostColony2P = 0x37,
		EternalEngine2P = 0x38,
		MetalHarbor2P = 0x39,
		IronGate2P = 0x3A,
		DeathChamber2P = 0x3B,
		BigFoot = 0x3C,
		HotShot = 0x3D,
		FlyingDog = 0x3E,
		KingBoomBoo = 0x3F,
		EggGolemS = 0x40,
		Biolizard = 0x41,
		FinalHazard = 0x42,
		EggGolemE = 0x43,
		Route101280 = 70,
		KartRace = 71,
		ChaoWorld = 90,
		Invalid = 91
	}

	public enum SA2DCLevelIDs : byte
	{
		BasicTest = 0,
		KnucklesTest = 1,
		SonicTest = 2,
		GreenForest = 3,
		WhiteJungle = 4,
		PumpkinHill = 5,
		SkyRail = 6,
		AquaticMine = 7,
		SecurityHall = 8,
		PrisonLane = 9,
		MetalHarbor = 0xA,
		IronGate = 0xB,
		WeaponsBed = 0xC,
		CityEscape = 0xD,
		RadicalHighway = 0xE,
		WeaponsBed2P = 0xF,
		WildCanyon = 0x10,
		MissionStreet = 0x11,
		DryLagoon = 0x12,
		SonicVsShadow1 = 0x13,
		TailsVsEggman1 = 0x14,
		SandOcean = 0x15,
		CrazyGadget = 0x16,
		HiddenBase = 0x17,
		EternalEngine = 0x18,
		DeathChamber = 0x19,
		EggQuarters = 0x1A,
		LostColony = 0x1B,
		PyramidCave = 0x1C,
		TailsVsEggman2 = 0x1D,
		FinalRush = 0x1E,
		GreenHill = 0x1F,
		MeteorHerd = 0x20,
		KnucklesVsRouge = 0x21,
		CannonsCoreS = 0x22,
		CannonsCoreE = 0x23,
		CannonsCoreT = 0x24,
		CannonsCoreR = 0x25,
		CannonsCoreK = 0x26,
		MissionStreet2P = 0x27,
		FinalChase = 0x28,
		WildCanyon2P = 0x29,
		SonicVsShadow2 = 0x2A,
		CosmicWall = 0x2B,
		MadSpace = 0x2C,
		SandOcean2P = 0x2D,
		BigFoot = 0x32,
		HotShot = 0x33,
		FlyingDog = 0x34,
		KingBoomBoo = 0x35,
		EggGolemS = 0x36,
		Biolizard = 0x37,
		FinalHazard = 0x38,
		EggGolemE = 0x39,
		Route101280 = 70,
		KartRace = 71,
		ChaoWorld = 90,
		Invalid = 91
	}
	
	public enum SA2Characters
	{
		Sonic = 0,
		Shadow = 1,
		Tails = 2,
		Eggman = 3,
		Knuckles = 4,
		Rouge = 5,
		MechTails = 6,
		MechEggman = 7,
		SuperSonic = 9,
		SuperShadow = 0xA
	}

	public enum SA2KartCharacters
	{
		TailsStory = 0,
		RougeStory = 1,
		Sonic = 2,
		Knuckles = 3,
		Tails = 4,
		Eggman = 5,
		Shadow = 6,
		Rouge = 7
	}

	[Flags]
	public enum SA2CharacterFlags
	{
		Sonic = 1 << SA2Characters.Sonic,
		Shadow = 1 << SA2Characters.Shadow,
		Tails = 1 << SA2Characters.Tails,
		Eggman = 1 << SA2Characters.Eggman,
		Knuckles = 1 << SA2Characters.Knuckles,
		Rouge = 1 << SA2Characters.Rouge,
		MechTails = 1 << SA2Characters.MechTails,
		MechEggman = 1 << SA2Characters.MechEggman
	}

	public enum Languages
	{
		Japanese = 0,
		English = 1,
		French = 2,
		Spanish = 3,
		German = 4,
		Italian = 5
	}

	public enum ChaoItemCategory
	{
		Egg = 1,
		Fruit = 3,
		Seed = 7,
		Hat = 9,
		MenuTheme = 0x10
	}

	internal class OsModuleLink
	{
		public uint Next;
		public uint Prev;

		public OsModuleLink(byte[] file, int address)
		{
			Next = ByteConverter.ToUInt32(file, address);
			Prev = ByteConverter.ToUInt32(file, address + 4);
		}
	}

	internal class OsModuleInfo
	{
		public uint Id;				   // Unique identifier for the module
		public OsModuleLink Link;	   // Doubly linked list of modules
		public uint NumSections;	   // # of sections
		public uint SectionInfoOffset; // Offset to section info table
		public uint NameOffset;		   // Offset to module name
		public uint NameSize;		   // Size of module name
		public uint Version;		   // Version number

		public OsModuleInfo(byte[] file, int address)
		{
			Id = ByteConverter.ToUInt32(file, address);
			address += 4;
			Link = new OsModuleLink(file, address);
			address += 8;
			NumSections = ByteConverter.ToUInt32(file, address);
			address += 4;
			SectionInfoOffset = ByteConverter.ToUInt32(file, address);
			address += 4;
			NameOffset = ByteConverter.ToUInt32(file, address);
			address += 4;
			NameSize = ByteConverter.ToUInt32(file, address);
			address += 4;
			Version = ByteConverter.ToUInt32(file, address);
		}
	}

	internal class OsModuleHeader
	{
		// CAUTION: Info must be the 1st member
		public OsModuleInfo Info;

		// OS_MODULE_VERSION == 1
		public uint BssSize;		   // Total size of bss sections in bytes
		public uint RelOffset;
		public uint ImpOffset;
		public uint ImpSize;		   // Size in bytes
		public byte PrologSection;	   // Section # for prolog function
		public byte EpilogSection;	   // Section # for epilog function
		public byte UnresolvedSection; // Section # for unresolved function
		public byte Padding0;
		public uint Prolog;			   // Prolog function offset
		public uint Epilog;			   // Epilog function offset
		public uint Unresolved;		   // Unresolved function offset

		// OS_MODULE_VERSION == 2
		public uint Align;			   // Module alignment constraint
		public uint BssAlign;		   // Bss alignment constraint

		public OsModuleHeader(byte[] file, int address)
		{
			Info = new OsModuleInfo(file, address);
			address += 0x20;
			BssSize = ByteConverter.ToUInt32(file, address);
			address += 4;
			RelOffset = ByteConverter.ToUInt32(file, address);
			address += 4;
			ImpOffset = ByteConverter.ToUInt32(file, address);
			address += 4;
			ImpSize = ByteConverter.ToUInt32(file, address);
			address += 4;
			PrologSection = file[address++];
			EpilogSection = file[address++];
			UnresolvedSection = file[address++];
			Padding0 = file[address++];
			Prolog = ByteConverter.ToUInt32(file, address);
			address += 4;
			Epilog = ByteConverter.ToUInt32(file, address);
			address += 4;
			Unresolved = ByteConverter.ToUInt32(file, address);
			address += 4;
			Align = ByteConverter.ToUInt32(file, address);
			address += 4;
			BssAlign = ByteConverter.ToUInt32(file, address);
		}
	}

	class OsSectionInfo
	{
		public uint Offset;
		public uint Size;

		public OsSectionInfo(byte[] file, int address)
		{
			Offset = ByteConverter.ToUInt32(file, address);
			Size = ByteConverter.ToUInt32(file, address + 4);
		}
	}

	class OsImportInfo
	{
		public uint Id;		// External module id
		public uint Offset; // Offset to OSRel instructions

		public OsImportInfo(byte[] file, int address)
		{
			Id = ByteConverter.ToUInt32(file, address);
			Offset = ByteConverter.ToUInt32(file, address + 4);
		}
	}

	class OsRel
	{
		public ushort Offset; // Byte offset from the previous entry
		public byte Type;
		public byte Section;
		public uint Addend;

		public OsRel(byte[] file, int address)
		{
			Offset = ByteConverter.ToUInt16(file, address);
			Type = file[address + 2];
			Section = file[address + 3];
			Addend = ByteConverter.ToUInt32(file, address + 4);
		}
	}

	enum RelocTypes
	{
		R_DOLPHIN_NOP = 201,	 //  C9h current offset += OSRel.offset
		R_DOLPHIN_SECTION = 202, //  CAh current section = OSRel.section
		R_DOLPHIN_END = 203	     //  CBh
	}
}
