using SonicRetro.SAModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SA_Tools
{
	public static class HelperFunctions
	{
		[DllImport("SACompGC.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		private static extern uint GetDecompressedSize(IntPtr InputBuffer);
		[DllImport("SACompGC.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		private static extern void DecompressBuffer(IntPtr InputBuffer, IntPtr OutputBuffer);
		public static uint? SetupEXE(ref byte[] exefile)
		{
			if (ByteConverter.ToUInt16(exefile, 0) != 0x5A4D)
				return null;
			int ptr = ByteConverter.ToInt32(exefile, 0x3c);
			if (ByteConverter.ToInt32(exefile, (int)ptr) != 0x4550) //PE\0\0
				return null;
			ptr += 4;
			UInt16 numsects = ByteConverter.ToUInt16(exefile, (int)ptr + 2);
			ptr += 0x14;
			int PEHead = ptr;
			uint imageBase = ByteConverter.ToUInt32(exefile, ptr + 28);
			byte[] result = new byte[ByteConverter.ToUInt32(exefile, ptr + 56)];
			Array.Copy(exefile, result, ByteConverter.ToUInt32(exefile, ptr + 60));
			ptr += 0xe0;
			for (int i = 0; i < numsects; i++)
			{
				Array.Copy(exefile, ByteConverter.ToInt32(exefile, ptr + (int)SectOffs.FAddr), result, ByteConverter.ToInt32(exefile, ptr + (int)SectOffs.VAddr), ByteConverter.ToInt32(exefile, ptr + (int)SectOffs.FSize));
				ptr += (int)SectOffs.Size;
			}
			exefile = result;
			return imageBase;
		}

		public static uint GetNewSectionAddress(byte[] exefile)
		{
			int ptr = ByteConverter.ToInt32(exefile, 0x3c);
			ptr += 4;
			UInt16 numsects = ByteConverter.ToUInt16(exefile, (int)ptr + 2);
			ptr += 0x14;
			ptr += 0xe0;
			ptr += (int)SectOffs.Size * (numsects - 1);
			return HelperFunctions.Align(ByteConverter.ToUInt32(exefile, ptr + (int)SectOffs.VAddr) + ByteConverter.ToUInt32(exefile, ptr + (int)SectOffs.VSize));
		}

		public static void CreateNewSection(ref byte[] exefile, string name, byte[] data, bool isCode)
		{
			int ptr = ByteConverter.ToInt32(exefile, 0x3c);
			ptr += 4;
			UInt16 numsects = ByteConverter.ToUInt16(exefile, ptr + 2);
			int sectnumptr = ptr + 2;
			ptr += 0x14;
			int PEHead = ptr;
			ptr += 0xe0;
			int sectptr = ptr;
			ptr += (int)SectOffs.Size * numsects;
			ByteConverter.GetBytes((ushort)(numsects + 1)).CopyTo(exefile, sectnumptr);
			Array.Clear(exefile, ptr, 8);
			Encoding.ASCII.GetBytes(name).CopyTo(exefile, ptr);
			UInt32 vaddr = HelperFunctions.Align(ByteConverter.ToUInt32(exefile, ptr - (int)SectOffs.Size + (int)SectOffs.VAddr) + ByteConverter.ToUInt32(exefile, ptr - (int)SectOffs.Size + (int)SectOffs.VSize));
			ByteConverter.GetBytes(vaddr).CopyTo(exefile, ptr + (int)SectOffs.VAddr);
			UInt32 faddr = HelperFunctions.Align(ByteConverter.ToUInt32(exefile, ptr - (int)SectOffs.Size + (int)SectOffs.FAddr) + ByteConverter.ToUInt32(exefile, ptr - (int)SectOffs.Size + (int)SectOffs.FSize));
			ByteConverter.GetBytes(faddr).CopyTo(exefile, ptr + (int)SectOffs.FAddr);
			ByteConverter.GetBytes(isCode ? 0x60000020 : 0xC0000040).CopyTo(exefile, ptr + (int)SectOffs.Flags);
			int diff = (int)HelperFunctions.Align((uint)data.Length);
			ByteConverter.GetBytes(diff).CopyTo(exefile, ptr + (int)SectOffs.VSize);
			ByteConverter.GetBytes(diff).CopyTo(exefile, ptr + (int)SectOffs.FSize);
			if (isCode)
				ByteConverter.GetBytes(Convert.ToUInt32(ByteConverter.ToUInt32(exefile, PEHead + 4) + diff)).CopyTo(exefile, PEHead + 4);
			else
				ByteConverter.GetBytes(Convert.ToUInt32(ByteConverter.ToUInt32(exefile, PEHead + 8) + diff)).CopyTo(exefile, PEHead + 8);
			ByteConverter.GetBytes(Convert.ToUInt32(ByteConverter.ToUInt32(exefile, PEHead + 0x38) + diff)).CopyTo(exefile, PEHead + 0x38);
			Array.Resize(ref exefile, exefile.Length + diff);
			data.CopyTo(exefile, vaddr);
		}

		public static void CompactEXE(ref byte[] exefile)
		{
			if (ByteConverter.ToUInt16(exefile, 0) != 0x5A4D)
				return;
			int ptr = ByteConverter.ToInt32(exefile, 0x3c);
			if (ByteConverter.ToInt32(exefile, (int)ptr) != 0x4550) //PE\0\0
				return;
			ptr += 4;
			UInt16 numsects = ByteConverter.ToUInt16(exefile, (int)ptr + 2);
			ptr += 0x14;
			int PEHead = ptr;
			uint imageBase = ByteConverter.ToUInt32(exefile, ptr + 28);
			byte[] result = new byte[ByteConverter.ToInt32(exefile, ptr + 0xe0 + ((int)SectOffs.Size * (numsects - 1)) + (int)SectOffs.FAddr) + ByteConverter.ToInt32(exefile, ptr + 0xe0 + ((int)SectOffs.Size * (numsects - 1)) + (int)SectOffs.FSize)];
			Array.Copy(exefile, result, ByteConverter.ToUInt32(exefile, ptr + 60));
			ptr += 0xe0;
			for (int i = 0; i < numsects; i++)
			{
				Array.Copy(exefile, ByteConverter.ToInt32(exefile, ptr + (int)SectOffs.VAddr), result, ByteConverter.ToInt32(exefile, ptr + (int)SectOffs.FAddr), ByteConverter.ToInt32(exefile, ptr + (int)SectOffs.FSize));
				ptr += (int)SectOffs.Size;
			}
			exefile = result;
		}

		public static void FixRELPointers(byte[] file, uint imageBase = 0)
		{
			OSModuleHeader header = new OSModuleHeader(file, 0);
			OSSectionInfo[] sections = new OSSectionInfo[header.info.numSections];
			for (int i = 0; i < header.info.numSections; i++)
				sections[i] = new OSSectionInfo(file, (int)header.info.sectionInfoOffset + (i * 8));
			OSImportInfo[] imports = new OSImportInfo[header.impSize / 8];
			for (int i = 0; i < imports.Length; i++)
				imports[i] = new OSImportInfo(file, (int)header.impOffset + (i * 8));
			int reladdr = 0;
			for (int i = 0; i < imports.Length; i++)
				if (imports[i].id == header.info.id)
				{
					reladdr = (int)imports[i].offset;
					break;
				}
				OSRel rel = new OSRel(file, reladdr);
				int dataaddr = 0;
				unchecked
				{
					while (rel.type != (byte)RelocTypes.R_DOLPHIN_END)
					{
						dataaddr += rel.offset;
						uint sectionbase = (uint)(sections[rel.section].offset & ~1);
						switch (rel.type)
						{
							case 0x01:
								ByteConverter.GetBytes(rel.addend + sectionbase + imageBase).CopyTo(file, dataaddr);
								break;
							case 0x02:
								ByteConverter.GetBytes((ByteConverter.ToUInt32(file, dataaddr) & 0xFC000003) | ((rel.addend + sectionbase) & 0x3FFFFFC) + imageBase).CopyTo(file, dataaddr);
								break;
							case 0x03:
							case 0x04:
								ByteConverter.GetBytes((ushort)(rel.addend + sectionbase) + imageBase).CopyTo(file, dataaddr);
								break;
							case 0x05:
								ByteConverter.GetBytes((ushort)((rel.addend + sectionbase) >> 16) + imageBase).CopyTo(file, dataaddr);
								break;
							case 0x06:
								ByteConverter.GetBytes((ushort)(((rel.addend + sectionbase) >> 16) + (((rel.addend + sectionbase) & 0x8000) == 0x8000 ? 1 : 0)) + imageBase).CopyTo(file, dataaddr);
								break;
							case 0x0A:
								ByteConverter.GetBytes((uint)((ByteConverter.ToUInt32(file, dataaddr) & 0xFC000003) | (((rel.addend + sectionbase) - dataaddr) & 0x3FFFFFC)) + imageBase).CopyTo(file, dataaddr);
								break;
							case 0x00:
							case (byte)RelocTypes.R_DOLPHIN_NOP:
							case (byte)RelocTypes.R_DOLPHIN_END:
								break;
							case (byte)RelocTypes.R_DOLPHIN_SECTION:
								dataaddr = (int)sectionbase;
								break;
							default:
								throw new NotImplementedException();
						}
						reladdr += 8;
						rel = new OSRel(file, reladdr);
					}
				}
		}

		public static void AlignCode(this List<byte> me)
		{
			while (me.Count % 0x10 > 0)
				me.Add(0x90);
		}

		public static uint Align(uint address)
		{
			if (address % 0x1000 == 0) return address;
			return ((address / 0x1000) + 1) * 0x1000;
		}

		static readonly System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

		public static string FileHash(string path) { return FileHash(File.ReadAllBytes(path)); }

		public static string FileHash(byte[] file)
		{
			file = md5.ComputeHash(file);
			string result = string.Empty;
			foreach (byte item in file)
				result += item.ToString("x2");
			return result;
		}

		private static readonly Encoding jpenc = Encoding.GetEncoding(932);
		private static readonly Encoding euenc = Encoding.GetEncoding(1252);

		public static Encoding GetEncoding() { return jpenc; }

		public static Encoding GetEncoding(Languages language) => GetEncoding(Game.SA1, language);

		public static Encoding GetEncoding(Game game, Languages language)
		{
			switch (language)
			{
				case Languages.Japanese:
					return jpenc;
				case Languages.English:
					switch (game)
					{
						case Game.SA1:
						case Game.SADX:
							return jpenc;
						case Game.SA2:
						case Game.SA2B:
							return euenc;
					}
					throw new ArgumentOutOfRangeException("game");
				default:
					return euenc;
			}
		}

		public static string GetCString(this byte[] file, int address)
		{
			return file.GetCString(address, jpenc);
		}

		public static int GetPointer(this byte[] file, int address, uint imageBase)
		{
			uint tmp = ByteConverter.ToUInt32(file, address);
			if (tmp == 0) return 0;
			return (int)(tmp - imageBase);
		}

		public static string UnescapeNewlines(this string line)
		{
			StringBuilder sb = new StringBuilder(line.Length);
			for (int c = 0; c < line.Length; c++)
				switch (line[c])
				{
					case '\\': // escape character
						if (c + 1 == line.Length) goto default;
						c++;
						switch (line[c])
						{
							case 'n': // line feed
								sb.Append('\n');
								break;
							case 'r': // carriage return
								sb.Append('\r');
								break;
							default: // literal character
								sb.Append(line[c]);
								break;
						}
						break;
					default:
						sb.Append(line[c]);
						break;
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
				return i.ToString(NumberFormatInfo.InvariantInfo);
			else
				return "0x" + i.ToString("X");
		}

		public static string ToC(this string str) => str.ToC(Languages.Japanese);

		public static string ToC(this string str, Languages language) => ToC(str, Game.SA1, language);

		public static string ToC(this string str, Game game, Languages language)
		{
			if (str == null) return "NULL";
			Encoding enc = GetEncoding(game, language);
			StringBuilder result = new StringBuilder("\"");
			foreach (char item in str)
			{
				if (item == '\0')
					result.Append(@"\0");
				else if (item == '\a')
					result.Append(@"\a");
				else if (item == '\b')
					result.Append(@"\b");
				else if (item == '\f')
					result.Append(@"\f");
				else if (item == '\n')
					result.Append(@"\n");
				else if (item == '\r')
					result.Append(@"\r");
				else if (item == '\t')
					result.Append(@"\t");
				else if (item == '\v')
					result.Append(@"\v");
				else if (item == '"')
					result.Append(@"\""");
				else if (item == '\\')
					result.Append(@"\\");
				else if (item < ' ')
					result.AppendFormat(@"\{0}", Convert.ToString((short)item, 8).PadLeft(3, '0'));
				else if (item > '\x7F')
					foreach (byte b in enc.GetBytes(item.ToString()))
						result.AppendFormat(@"\{0}", Convert.ToString(b, 8).PadLeft(3, '0'));
				else
					result.Append(item);
			}
			result.Append("\"");
			return result.ToString();
		}

		public static string ToComment(this string str)
		{
			return "/* " + str.ToCNoEncoding().Replace("*/", @"*\/") + " */";
		}

		public static string ToCNoEncoding(this string str)
		{
			if (str == null) return "NULL";
			StringBuilder result = new StringBuilder("\"");
			foreach (char item in str)
			{
				if (item == '\0')
					result.Append(@"\0");
				else if (item == '\a')
					result.Append(@"\a");
				else if (item == '\b')
					result.Append(@"\b");
				else if (item == '\f')
					result.Append(@"\f");
				else if (item == '\n')
					result.Append(@"\n");
				else if (item == '\r')
					result.Append(@"\r");
				else if (item == '\t')
					result.Append(@"\t");
				else if (item == '\v')
					result.Append(@"\v");
				else if (item == '"')
					result.Append(@"\""");
				else if (item == '\\')
					result.Append(@"\\");
				else if (item < ' ')
					result.AppendFormat(@"\{0}", Convert.ToString((short)item, 8).PadLeft(3, '0'));
				else
					result.Append(item);
			}
			result.Append("\"");
			return result.ToString();
		}

		public static string ToC<T>(this T item)
			where T : Enum
		{
			return item.ToC(typeof(T).Name);
		}

		public static string ToC<T>(this T item, string enumname)
			where T : Enum
		{
			Type type = typeof(T);
			if (type.GetCustomAttributes(typeof(FlagsAttribute), false).Length == 0)
				if (Enum.IsDefined(typeof(T), item))
					return enumname + "_" + item.ToString();
				else
					return item.ToString();
			else
			{
				ulong num = Convert.ToUInt64(item);
				ulong[] values = Array.ConvertAll((T[])Enum.GetValues(type), (a) => Convert.ToUInt64(a));
				int num2 = values.Length - 1;
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = true;
				ulong num3 = num;
				while (num2 >= 0 && (num2 != 0 || values[num2] != 0uL))
				{
					if ((num & values[num2]) == values[num2])
					{
						num -= values[num2];
						if (!flag)
							stringBuilder.Insert(0, " | ");
						stringBuilder.Insert(0, enumname + "_" + Enum.GetName(type, values[num2]));
						flag = false;
					}
					num2--;
				}
				if (num != 0uL)
				{
					if (flag)
						return item.ToString();
					else
						return stringBuilder.ToString() + " | " + item.ToString();
				}
				if (num3 != 0uL)
					return stringBuilder.ToString();
				if (values.Length > 0 && values[0] == 0uL)
					return enumname + "_" + Enum.GetName(type, 0);
				return "0";
			}
		}

		public static bool CheckBigEndianInt16(byte[] file, int address)
		{
			bool bigEndState = ByteConverter.BigEndian;

			ByteConverter.BigEndian = true;
			bool isBigEndian = BitConverter.ToUInt16(file, address) > ByteConverter.ToUInt16(file, address);

			ByteConverter.BigEndian = bigEndState;

			return isBigEndian;
		}
		public static bool CheckBigEndianInt32(byte[] file, int address)
		{
			bool bigEndState = ByteConverter.BigEndian;

			ByteConverter.BigEndian = true;
			bool isBigEndian = BitConverter.ToUInt32(file, address) > ByteConverter.ToUInt32(file, address);

			ByteConverter.BigEndian = bigEndState;

			return isBigEndian;
		}

		public static byte[] DecompressREL(byte[] file)
		{
			// Scan the array for the last instance of the "SaCompGC" string because there are some files with redundant headers
			int start = 0;
			bool isCompressed = false;
			bool bigend = ByteConverter.BigEndian;
			ByteConverter.BigEndian = true;
			for (int u = file.Length - 8; u >= 0; u--)
			{
				if (ByteConverter.ToUInt32(file, u) == 0x5361436F)// && BitConverter.ToUInt32(file, u + 4) == 0x4347706D)
				{
					start = u;
					isCompressed = true;
					break;
				}
			}
			if (!isCompressed) return file;
			byte[] input = new byte[file.Length - start];
			Array.Copy(file, start, input, 0, input.Length);

			// Process the new array
			IntPtr pnt_input = Marshal.AllocHGlobal(input.Length);
			Marshal.Copy(input, 0, pnt_input, input.Length);
			int size_output = (int)GetDecompressedSize(pnt_input);
			IntPtr pnt_output = Marshal.AllocHGlobal(size_output);
			DecompressBuffer(pnt_input, pnt_output);
			byte[] decompbuf = new byte[size_output];
			Marshal.Copy(pnt_output, decompbuf, 0, size_output);
			Marshal.FreeHGlobal(pnt_output);
			Marshal.FreeHGlobal(pnt_input);
			ByteConverter.BigEndian = bigend;
			return decompbuf;
		}
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

	[Flags()]
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

	public enum SA2Characters
	{
		Sonic = 0,
		Shadow = 1,
		Tails = 2,
		Eggman = 3,
		Knuckles = 4,
		Rouge = 5,
		MechTails = 6,
		MechEggman = 7
	}

	public enum Languages
	{
		Japanese = 0,
		English = 1,
		French = 2,
		Spanish = 3,
		German = 4
	}

	public enum ChaoItemCategory
	{
		ChaoItemCategory_Egg = 1,
		ChaoItemCategory_Fruit = 3,
		ChaoItemCategory_Seed = 7,
		ChaoItemCategory_Hat = 9,
		ChaoItemCategory_MenuTheme = 0x10
	}

	class OSModuleLink
	{
		public uint next;
		public uint prev;

		public OSModuleLink(byte[] file, int address)
		{
			next = ByteConverter.ToUInt32(file, address);
			prev = ByteConverter.ToUInt32(file, address + 4);
		}
	}

	class OSModuleInfo
	{
		public uint id;				 // unique identifier for the module
		public OSModuleLink link;			   // doubly linked list of modules
		public uint numSections;		// # of sections
		public uint sectionInfoOffset;  // offset to section info table
		public uint nameOffset;		 // offset to module name
		public uint nameSize;		   // size of module name
		public uint version;			// version number

		public OSModuleInfo(byte[] file, int address)
		{
			id = ByteConverter.ToUInt32(file, address);
			address += 4;
			link = new OSModuleLink(file, address);
			address += 8;
			numSections = ByteConverter.ToUInt32(file, address);
			address += 4;
			sectionInfoOffset = ByteConverter.ToUInt32(file, address);
			address += 4;
			nameOffset = ByteConverter.ToUInt32(file, address);
			address += 4;
			nameSize = ByteConverter.ToUInt32(file, address);
			address += 4;
			version = ByteConverter.ToUInt32(file, address);
		}
	}

	class OSModuleHeader
	{
		// CAUTION: info must be the 1st member
		public OSModuleInfo info;

		// OS_MODULE_VERSION == 1
		public uint bssSize;			// total size of bss sections in bytes
		public uint relOffset;
		public uint impOffset;
		public uint impSize;			// size in bytes
		public byte prologSection;	  // section # for prolog function
		public byte epilogSection;	  // section # for epilog function
		public byte unresolvedSection;  // section # for unresolved function
		public byte padding0;
		public uint prolog;			 // prolog function offset
		public uint epilog;			 // epilog function offset
		public uint unresolved;		 // unresolved function offset

		// OS_MODULE_VERSION == 2
		public uint align;			  // module alignment constraint
		public uint bssAlign;		   // bss alignment constraint

		public OSModuleHeader(byte[] file, int address)
		{
			info = new OSModuleInfo(file, address);
			address += 0x20;
			bssSize = ByteConverter.ToUInt32(file, address);
			address += 4;
			relOffset = ByteConverter.ToUInt32(file, address);
			address += 4;
			impOffset = ByteConverter.ToUInt32(file, address);
			address += 4;
			impSize = ByteConverter.ToUInt32(file, address);
			address += 4;
			prologSection = file[address++];
			epilogSection = file[address++];
			unresolvedSection = file[address++];
			padding0 = file[address++];
			prolog = ByteConverter.ToUInt32(file, address);
			address += 4;
			epilog = ByteConverter.ToUInt32(file, address);
			address += 4;
			unresolved = ByteConverter.ToUInt32(file, address);
			address += 4;
			align = ByteConverter.ToUInt32(file, address);
			address += 4;
			bssAlign = ByteConverter.ToUInt32(file, address);
		}
	}

	class OSSectionInfo
	{
		public uint offset;
		public uint size;

		public OSSectionInfo(byte[] file, int address)
		{
			offset = ByteConverter.ToUInt32(file, address);
			size = ByteConverter.ToUInt32(file, address + 4);
		}
	}

	class OSImportInfo
	{
		public uint id;				 // external module id
		public uint offset;			 // offset to OSRel instructions

		public OSImportInfo(byte[] file, int address)
		{
			id = ByteConverter.ToUInt32(file, address);
			offset = ByteConverter.ToUInt32(file, address + 4);
		}
	}

	class OSRel
	{
		public ushort offset;			 // byte offset from the previous entry
		public byte type;
		public byte section;
		public uint addend;

		public OSRel(byte[] file, int address)
		{
			offset = ByteConverter.ToUInt16(file, address);
			type = file[address + 2];
			section = file[address + 3];
			addend = ByteConverter.ToUInt32(file, address + 4);
		}
	}

	enum RelocTypes
	{
		R_DOLPHIN_NOP = 201,	 //  C9h current offset += OSRel.offset
		R_DOLPHIN_SECTION = 202,	 //  CAh current section = OSRel.section
		R_DOLPHIN_END = 203	 //  CBh
	}


}