using System;
using System.Collections.Generic;
using System.IO;
using SonicRetro.SAModel;
using SA_Tools;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging.ColorReduction;
using System.Runtime.InteropServices;

namespace DLCTool
{
	class Program
	{
		const int BIT_0 = (1 << 0);
		const int BIT_1 = (1 << 1);
		const int BIT_2 = (1 << 2);
		const int BIT_3 = (1 << 3);
		const int BIT_4 = (1 << 4);
		const int BIT_5 = (1 << 5);
		const int BIT_6 = (1 << 6);
		const int BIT_7 = (1 << 7);
		const int BIT_8 = (1 << 8);
		const int BIT_9 = (1 << 9);
		const int BIT_10 = (1 << 10);
		const int BIT_11 = (1 << 11);
		const int BIT_12 = (1 << 12);
		const int BIT_13 = (1 << 13);
		const int BIT_14 = (1 << 14);
		const int BIT_15 = (1 << 15);

		public enum DLCObjectTypes
		{
			TYPE_MODEL = 0,
			TYPE_SPRITE = 0x80,
			TYPE_INVISIBLE = 0xFF,
		}

		public enum DLCRegionLocks
		{
			Disabled = -1,
			Japan = 1,
			US = 3,
			Europe = 4,
			All = 7,
		}

		[Flags]
		public enum DLCObjectFlags
		{
			FLAG_SOLID = BIT_8,
			FLAG_SOUND = BIT_9,
			FLAG_MESSAGE = BIT_10,
			FLAG_COLLISION_ONLY = BIT_11,
			FLAG_WARP = BIT_12,
			FLAG_COLLECTIBLE = BIT_13,
			FLAG_TIMER = BIT_14,
			FLAG_CHALLENGE = BIT_15,
			UNKNOWN_0 = BIT_0, //??
			UNKNOWN_4 = BIT_4, //??
			UNUSED_1 = BIT_1,
			UNUSED_2 = BIT_2,
			UNUSED_3 = BIT_3,
			UNUSED_5 = BIT_5,
			UNUSED_6 = BIT_6,
			UNUSED_7 = BIT_7,
		}

		internal struct DLCMetadata
		{
			[IniName("Title")]
			public string title;
			[IniName("Description")]
			public string description;
			[IniName("Author")]
			public string appname;
			[IniName("Identifier")]
			public uint dlc_id;
			[IniAlwaysInclude]
			[IniName("ContainsSound")]
			public bool has_mlt;
			[IniAlwaysInclude]
			[IniName("EnableSonic")]
			public bool chars_sonic;
			[IniAlwaysInclude]
			[IniName("EnableTails")]
			public bool chars_tails;
			[IniAlwaysInclude]
			[IniName("EnableKnuckles")]
			public bool chars_knuckles;
			[IniAlwaysInclude]
			[IniName("EnableGamma")]
			public bool chars_e102;
			[IniAlwaysInclude]
			[IniName("EnableAmy")]
			public bool chars_amy;
			[IniAlwaysInclude]
			[IniName("EnableBig")]
			public bool chars_big;
			[IniName("Whatever")]
			public byte whatever;
			[IniAlwaysInclude]
			[IniName("Region")]
			public DLCRegionLocks region;
			[IniCollection(IniCollectionMode.NoSquareBrackets)]
			[IniName("Item ")]
			public List<DLCObjectData> items;
			[IniCollection(IniCollectionMode.NoSquareBrackets)]
			[IniName("JapaneseMessage")]
			public string[] JapaneseStrings;
			[IniCollection(IniCollectionMode.NoSquareBrackets)]
			[IniName("EnglishMessage")]
			public string[] EnglishStrings;
			[IniCollection(IniCollectionMode.NoSquareBrackets)]
			[IniName("FrenchMessage")]
			public string[] FrenchStrings;
			[IniCollection(IniCollectionMode.NoSquareBrackets)]
			[IniName("SpanishMessage")]
			public string[] SpanishStrings;
			[IniCollection(IniCollectionMode.NoSquareBrackets)]
			[IniName("GermanMessage")]
			public string[] GermanStrings;
		}

		internal struct DLCObjectData
		{
			[IniAlwaysInclude]
			[IniName("Level")]
			public byte level;
			[IniAlwaysInclude]
			[IniName("Act")]
			public byte act;
			[IniAlwaysInclude]
			[IniName("ScaleX")]
			public byte scale_x;
			[IniAlwaysInclude]
			[IniName("ScaleY")]
			public byte scale_y;
			[IniAlwaysInclude]
			[IniName("ScaleZ")]
			public byte scale_z;
			[IniName("RotSpeedX")]
			public byte rotspeed_x;
			[IniName("RotSpeedY")]
			public byte rotspeed_y;
			[IniName("RotSpeedZ")]
			public byte rotspeed_z;
			[IniAlwaysInclude]
			[IniName("ObjectType")]
			public DLCObjectTypes objecttype;
			[IniName("TextureID")]
			public byte textureid;
			[IniAlwaysInclude]
			[IniName("Flags")]
			public DLCObjectFlags flags;
			[IniName("CollectibleID")] //TIMER: Number of objects to collect, CHALLENGE: Number of seconds / 10
			public byte objectid;
			[IniName("Unknown3")]
			public byte unknown3;
			[IniName("MessageID")]
			public byte message;
			[IniName("TriggerRadius")]
			public byte radius;
			[IniName("WarpLevelOrSoundbank")]
			public byte warplevel;
			[IniName("WarpActOrSoundID")]
			public byte sound;
			[IniName("RotationX")]
			public ushort rot_x;
			[IniName("RotationY")]
			public ushort rot_y;
			[IniName("RotationZ")]
			public ushort rot_z;
			[IniName("X")]
			public short x;
			[IniName("Y")]
			public short y;
			[IniName("Z")]
			public short z;
		}

		static void ProcessVMS(ref byte[] input)
		{
			int aniFrames = input[0x40] + (input[0x41] << 8);
			int dataStart = 0x80 + (aniFrames * 0x200);
			byte[] encrypted = new byte[input.Length - dataStart];
			Array.Copy(input, dataStart, encrypted, 0, encrypted.Length);
			DecryptData(ref encrypted);
			Array.Copy(encrypted, 0, input, dataStart, encrypted.Length);
		}

		static void DecryptData(ref byte[] input)
		{
			//C# code by Sappharad
			//Original Perl code by Darksecond
			//Original post here: http://assemblergames.com/l/threads/decoding-decrypting-sonic-adventure-ranking-data.60036/#post-863289
			byte[] xor_code = new byte[] { 0x41, 0x54, 0x45, 0x5A };
			byte[] plus_val = new byte[] { 0x41, 0x4E, 0x41, 0x4E };
			byte[] tmp_val = new byte[] { 0, 0, 0, 0 };

			for (int i = 0; i < input.Length / 4; i++)
			{
				int tmp_val2 = 0;
				for (int j = 0; j < 4; j++)
				{
					input[i * 4 + j] = (byte)(input[i * 4 + j] ^ tmp_val[j] ^ xor_code[j]);
					int next = (tmp_val[j] + plus_val[j] + tmp_val2);
					tmp_val2 = (next >> 8);
					tmp_val[j] = (byte)(next & 0xFF);
				}
			}
		}

		static byte[] ConvertIcon(string filename)
		{
			List<byte> result = new List<byte>();
			//Get palette
			ushort[] colors_short = new ushort[16];
			Color[] colors = new Color[16];
			Bitmap bitmap = new Bitmap(filename);
			if (bitmap.PixelFormat != PixelFormat.Format4bppIndexed)
			{
				Console.WriteLine("Icon is {0}, converting to {1} (loss of quality possible)", bitmap.PixelFormat.ToString(), PixelFormat.Format4bppIndexed.ToString());
				ColorImageQuantizer ciq = new ColorImageQuantizer(new MedianCutQuantizer());
				bitmap = ciq.ReduceColors(bitmap, 16);
			}
			for (int u = 0; u < 16; u++)
			{
				colors[u] = bitmap.Palette.Entries[u];
				int a = colors[u].A / 16;
				int r = colors[u].R / 16;
				int g = colors[u].G / 16;
				int b = colors[u].B / 16;
				colors_short[u] = (ushort)(a << 12 | r << 8 | g << 4 | b);
				//Console.WriteLine("Color to binary {0} ({1}) : {2} comp A:{3} R:{4} G:{5} B:{6}", u, colors[u].ToString(), colors_short[u].ToString("X"), a.ToString("X"),r.ToString("X"), g.ToString("X"), b.ToString("X"));
				result.AddRange(BitConverter.GetBytes(colors_short[u]));
				//Console.WriteLine("Color {0}:{1}", u, colors[u].ToString());
			}
			//Get colors
			byte[] image = new byte[1024];
			for (int y = 0; y < 32; y++)
			{
				for (int x = 0; x < 32; x++)
				{
					for (byte c = 0; c < 16; c++)
					{
						if (bitmap.GetPixel(x, y) == colors[c])
						{
							image[y * 32 + x] = c;
							break;
						}
					}
				}
			}
			byte[] squeeze = new byte[512];
			for (int u = 0; u < 1024; u++)
			{
				if (u % 2 == 0)
				{
					squeeze[u / 2] = (byte)(image[u] << 4 | image[u + 1] & 0xF);
					//Console.WriteLine("Byte {0}: {1} / {2}", image[u], image[u] << 4, image[u + 1] & 0xF);
				}
			}
			result.AddRange(squeeze);
			return result.ToArray();
		}

		static byte[] ConvertModel(string filename)
		{
			List<byte> result = new List<byte>();
			ModelFile mfile = new ModelFile(filename);
			byte[] mdlarr = mfile.Model.GetBytes(0xCCA4000 + 4, false, out uint addr);
			result.AddRange(BitConverter.GetBytes(addr + 4 + 0xCCA4000));
			result.AddRange(mdlarr);
			byte[] res_arr = result.ToArray();
			result = FraGag.Compression.Prs.Compress(res_arr).ToList();
			do
			{
				result.Add(0);
			}
			while (result.Count % 16 != 0);
			return result.ToArray();
		}

		static byte[] ProcessEuropeanStrings(string[] list)
		{
			List<byte> result = new List<byte>();
			foreach (string str in list)
			{
				for (int s = 0; s < str.Length; s++)
				{
					if (str[s] == '\t')
					{
						result.AddRange(System.Text.Encoding.GetEncoding(1252).GetBytes("~"));
					}
					else if (str[s] == '\n')
					{
						result.AddRange(System.Text.Encoding.GetEncoding(1252).GetBytes("@"));
					}
					else
					{
						result.AddRange(System.Text.Encoding.GetEncoding(1252).GetBytes(str[s].ToString()));
					}
				}
				do
				{
					result.Add(0);
				}
				while (result.Count % 64 != 0);
			}
			return result.ToArray();
		}

		static byte[] ConvertMetadata(string filename)
		{
			Console.WriteLine("Converting DLC metadata: {0}", filename);
			DLCMetadata header = IniSerializer.Deserialize<DLCMetadata>(filename);
			byte[] mlt = new byte[0];
			byte[] prs;
			byte[] pvm = File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(filename), Path.GetFileNameWithoutExtension(filename) + ".pvm"));
			if (File.Exists(Path.Combine(Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(filename), Path.GetFileNameWithoutExtension(filename) + ".bin")))
			{
				prs = FraGag.Compression.Prs.Compress(File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(filename), Path.GetFileNameWithoutExtension(filename) + ".bin")));
				Console.WriteLine("Using binary data instead of SA1MDL file");
			}
			else prs = ConvertModel(Path.Combine(Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(filename), Path.GetFileNameWithoutExtension(filename) + ".sa1mdl"));
			if (header.has_mlt) mlt = File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(filename), Path.GetFileNameWithoutExtension(filename) + ".mlt"));
			List<byte> result = new List<byte>();
			//Convert item table header
			List<byte> itemtable = new List<byte>();
			itemtable.AddRange(BitConverter.GetBytes(header.dlc_id));
			Console.WriteLine("Identifier: {0}", header.dlc_id);
			byte chars_sonictails = 0;
			byte chars_knuxe102 = 0;
			byte chars_amybig = 0;
			if (header.chars_sonic) chars_sonictails |= 0x1;
			if (header.chars_tails) chars_sonictails |= 0x10;
			itemtable.Add(chars_sonictails);
			if (header.chars_knuckles) chars_knuxe102 |= 0x1;
			if (header.chars_e102) chars_knuxe102 |= 0x10;
			itemtable.Add(chars_knuxe102);
			if (header.chars_amy) chars_amybig |= 0x1;
			if (header.chars_big) chars_amybig |= 0x10;
			itemtable.Add(chars_amybig);
			itemtable.Add(header.whatever);
			itemtable.AddRange(BitConverter.GetBytes((int)(header.region)));
			Console.Write("Characters: ");
			if (header.chars_sonic) Console.Write("Sonic ");
			if (header.chars_tails) Console.Write("Tails ");
			if (header.chars_knuckles) Console.Write("Knuckles ");
			if (header.chars_amy) Console.Write("Amy ");
			if (header.chars_big) Console.Write("Big ");
			if (header.chars_e102) Console.Write("Gamma");
			Console.Write(System.Environment.NewLine);
			//Convert item table
			foreach (DLCObjectData item in header.items)
			{
				itemtable.Add(item.level);
				itemtable.Add(item.act);
				itemtable.Add(item.scale_x);
				itemtable.Add(item.scale_y);
				itemtable.Add(item.scale_z);
				itemtable.Add(item.rotspeed_x);
				itemtable.Add(item.rotspeed_y);
				itemtable.Add(item.rotspeed_z);
				itemtable.Add((byte)item.objecttype);
				itemtable.Add(item.textureid);
				itemtable.AddRange((BitConverter.GetBytes((ushort)item.flags)));
				itemtable.Add(item.objectid);
				itemtable.Add(item.unknown3);
				itemtable.Add(item.message);
				itemtable.Add(item.radius);
				itemtable.Add(item.warplevel);
				itemtable.Add(item.sound);
				itemtable.AddRange(BitConverter.GetBytes(item.rot_x));
				itemtable.AddRange(BitConverter.GetBytes(item.rot_y));
				itemtable.AddRange(BitConverter.GetBytes(item.rot_z));
				itemtable.AddRange(BitConverter.GetBytes(item.x));
				itemtable.AddRange(BitConverter.GetBytes(item.y));
				itemtable.AddRange(BitConverter.GetBytes(item.z));
			}
			do
				itemtable.Add(0);
			while (itemtable.Count % 512 != 0);
			//Convert Japanese strings
			List<byte> stringtable = new List<byte>();
			foreach (string str in header.JapaneseStrings)
			{
				for (int s = 0; s < str.Length; s++)
				{
					if (str[s] == '\t')
					{
						stringtable.AddRange(System.Text.Encoding.GetEncoding(932).GetBytes("t"));
					}
					else if (str[s] == '\n')
					{
						stringtable.AddRange(System.Text.Encoding.GetEncoding(932).GetBytes("n"));
					}
					else
					{
						stringtable.AddRange(System.Text.Encoding.GetEncoding(932).GetBytes(str[s].ToString()));
					}
				}
				do 
				{
					stringtable.Add(0);
				}
				while (stringtable.Count % 64 != 0);
			}
			//Convert European strings
			stringtable.AddRange(ProcessEuropeanStrings(header.EnglishStrings));
			stringtable.AddRange(ProcessEuropeanStrings(header.FrenchStrings));
			stringtable.AddRange(ProcessEuropeanStrings(header.SpanishStrings));
			stringtable.AddRange(ProcessEuropeanStrings(header.GermanStrings));
			//Set size
			int fullsize = mlt.Length + prs.Length + pvm.Length + itemtable.Count + stringtable.Count + 64; //64 is sections table at 0x280 w/checksum + 16 bytes of padding
			if ((fullsize + 640) % 512 != 0)
			{
				do
				{
					fullsize++;
				}
				while ((fullsize + 640) % 512 != 0);
			}
			//Convert title
			byte[] title_b = new byte[16];
			byte[] title = System.Text.Encoding.GetEncoding(932).GetBytes(header.title);
			Array.Copy(title, 0, title_b, 0, title.Length);
			result.AddRange(title_b);
			byte[] desc_b = new byte[32];
			byte[] desc = System.Text.Encoding.GetEncoding(932).GetBytes(header.description);
			Array.Copy(desc, 0, desc_b, 0, desc.Length);
			result.AddRange(desc_b);
			byte[] app_b = new byte[16];
			byte[] app = System.Text.Encoding.GetEncoding(932).GetBytes(header.appname);
			Array.Copy(app, 0, app_b, 0, app.Length);
			result.AddRange(app_b);
			result.AddRange(BitConverter.GetBytes((ushort)1)); //Number of icons
			result.AddRange(BitConverter.GetBytes((ushort)1)); //Animation speed
			result.AddRange(BitConverter.GetBytes((ushort)0)); //Eyecatch type
			result.AddRange(BitConverter.GetBytes((ushort)0)); //CRC (unused)
			result.AddRange(BitConverter.GetBytes((uint)fullsize)); //Size of the entire thing without VMS header
			for (int u = 0; u < 20; u++)
				result.Add(0);
			result.AddRange(ConvertIcon(Path.Combine(Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(filename), Path.GetFileNameWithoutExtension(filename) + ".bmp")));
			result.AddRange(BitConverter.GetBytes((uint)0x2C0)); //Item layout table pointer
			Console.WriteLine("Item table at 2C0, count {0} (size {1} + 12)", header.items.Count, header.items.Count * 30);
			result.AddRange(BitConverter.GetBytes((uint)header.items.Count));
			int textpointer = 704 + itemtable.Count;
			result.AddRange(BitConverter.GetBytes((uint)textpointer)); //Text table pointer
			int textcount = header.JapaneseStrings.Length + header.EnglishStrings.Length + header.FrenchStrings.Length + header.GermanStrings.Length + header.SpanishStrings.Length;
			result.AddRange(BitConverter.GetBytes((uint)textcount));
			Console.WriteLine("String table at {0}, count {1} (size {2})", textpointer.ToString("X"), textcount, textcount * 64);
			int pvmpointer = textpointer + 64 * textcount;
			result.AddRange(BitConverter.GetBytes((uint)pvmpointer));
			result.AddRange(BitConverter.GetBytes((uint)1)); //PVM count
			ushort numtextures = BitConverter.ToUInt16(pvm, 0xA);
			result.AddRange(BitConverter.GetBytes((uint)numtextures));
			Console.WriteLine("PVM at {0}, number of textures {1} (size {2})", pvmpointer.ToString("X"), numtextures, pvm.Length);
			int mltpointer = pvmpointer + pvm.Length;
			result.AddRange(BitConverter.GetBytes((uint)mltpointer));
			if (header.has_mlt)
			{
				result.AddRange(BitConverter.GetBytes((uint)1));
				Console.WriteLine("MLT at {0} (size {1})", mltpointer.ToString("X"), mlt.Length);
			}
			else result.AddRange(BitConverter.GetBytes((uint)0));
			int prspointer = mltpointer + mlt.Length;
			result.AddRange(BitConverter.GetBytes((uint)prspointer));
			result.AddRange(BitConverter.GetBytes((uint)1)); //PRS count
			byte[] final = new byte[fullsize - 64];
			Console.WriteLine("PRS at {0} (size {1})", prspointer.ToString("X"), prs.Length);
			Array.Copy(itemtable.ToArray(), 0, final, 0, itemtable.Count);
			Array.Copy(stringtable.ToArray(), 0, final, textpointer - 704, stringtable.Count);
			Array.Copy(pvm, 0, final, pvmpointer - 704, pvm.Length);
			if (header.has_mlt) Array.Copy(mlt, 0, final, mltpointer - 704, mlt.Length);
			Array.Copy(prs, 0, final, prspointer - 704, prs.Length);
			uint checksum = CalculateChecksum(ref final, 0, final.Length);
			result.AddRange(BitConverter.GetBytes(checksum));
			Console.WriteLine("Checksum: {0} ({1})", checksum.ToString("X8"), (int)checksum);
			for (int u = 0; u < 16; u++)
				result.Add(0);
			result.AddRange(final);
			if (result.Count % 512 != 0)
			{
				do
				{
					result.Add(0);
				}
				while (result.Count % 512 != 0);
			}
			return result.ToArray();
		}

		static uint CalculateChecksum(ref byte[] buf, int start, int end)
		{
			//Code by Sappharad
			uint result = 0;
			for (int i = start; i < end; i++)
			{
				int notByte = buf[i];
				if (notByte >= 128)
				{
					notByte -= 256;
				}
				result = (uint)(result + notByte);
			}
			return ~result;
		}

		static byte[] GetTextItem(ref byte[] text, int start, int maxlength)
		{
			List<byte> result = new List<byte>();
			for (int a = start; a < start + maxlength; a++)
			{
				if (text[a] == 0) break;
				result.Add(text[a]);
			}
			return result.ToArray();
		}

		static void MetatadaToINI(byte[] file, string filename)
		{
			DLCMetadata header = new DLCMetadata();
			//Get title
			byte[] title_b = GetTextItem(ref file, 0, 16);
			byte[] description_b = GetTextItem(ref file, 0x10, 32);
			byte[] appname_b = GetTextItem(ref file, 0x30, 16);
			header.description = System.Text.Encoding.GetEncoding(932).GetString(description_b);
			header.title = System.Text.Encoding.GetEncoding(932).GetString(title_b);
			header.appname = System.Text.Encoding.GetEncoding(932).GetString(appname_b);
			//Get items
			int item_pointer = BitConverter.ToInt32(file, 0x280);
			header.dlc_id = BitConverter.ToUInt32(file, item_pointer);
			Console.WriteLine("Identifier: {0}", header.dlc_id);
			if ((file[item_pointer + 4] & 0xF) > 0) header.chars_sonic = true;
			if ((file[item_pointer + 4] >> 4) > 0) header.chars_tails = true;
			if ((file[item_pointer + 5] & 0xF) > 0) header.chars_knuckles = true;
			if ((file[item_pointer + 5] >> 4) > 0) header.chars_e102 = true;
			if ((file[item_pointer + 6] & 0xF) > 0) header.chars_amy = true;
			if ((file[item_pointer + 6] >> 4) > 0) header.chars_big = true;
			Console.Write("Characters: ");
			if (header.chars_sonic) Console.Write("Sonic ");
			if (header.chars_tails) Console.Write("Tails ");
			if (header.chars_knuckles) Console.Write("Knuckles ");
			if (header.chars_amy) Console.Write("Amy ");
			if (header.chars_big) Console.Write("Big ");
			if (header.chars_e102) Console.Write("Gamma");
			Console.Write(System.Environment.NewLine);
			header.whatever = file[item_pointer + 7];
			header.region = (DLCRegionLocks)BitConverter.ToInt32(file, item_pointer + 8);
			int item_count = BitConverter.ToInt32(file, 0x284);
			Console.WriteLine("Item table at {0}, count {1} (size {2} + 12)", item_pointer.ToString("X"), item_count, item_count * 30);
			header.items = new List<DLCObjectData>(item_count);
			item_pointer += 12; //Skip 12-byte item section header
			for (int u = 0; u < item_count; u++)
			{
				DLCObjectData dlcitem = new DLCObjectData();
				dlcitem.level = file[item_pointer + u * 30];
				dlcitem.act = file[item_pointer + u * 30 + 1];
				dlcitem.scale_x = file[item_pointer + u * 30 + 2];
				dlcitem.scale_y = file[item_pointer + u * 30 + 3];
				dlcitem.scale_z = file[item_pointer + u * 30 + 4];
				dlcitem.rotspeed_x = file[item_pointer + u * 30 + 5];
				dlcitem.rotspeed_y = file[item_pointer + u * 30 + 6];
				dlcitem.rotspeed_z = file[item_pointer + u * 30 + 7];
				dlcitem.objecttype = (DLCObjectTypes)file[item_pointer + u * 30 + 8];
				dlcitem.textureid = file[item_pointer + u * 30 + 9];
				dlcitem.flags = (DLCObjectFlags)(BitConverter.ToUInt16(file, item_pointer + u * 30 + 10));
				dlcitem.objectid = file[item_pointer + u * 30 + 12];
				dlcitem.unknown3 = file[item_pointer + u * 30 + 13];
				dlcitem.message = file[item_pointer + u * 30 + 14];
				dlcitem.radius = file[item_pointer + u * 30 + 15];
				dlcitem.warplevel = file[item_pointer + u * 30 + 16];
				dlcitem.sound = file[item_pointer + u * 30 + 17];
				dlcitem.rot_x = BitConverter.ToUInt16(file, item_pointer + u * 30 + 18);
				dlcitem.rot_y = BitConverter.ToUInt16(file, item_pointer + u * 30 + 20);
				dlcitem.rot_z = BitConverter.ToUInt16(file, item_pointer + u * 30 + 22);
				dlcitem.x = BitConverter.ToInt16(file, item_pointer + u * 30 + 24);
				dlcitem.y = BitConverter.ToInt16(file, item_pointer + u * 30 + 26);
				dlcitem.z = BitConverter.ToInt16(file, item_pointer + u * 30 + 28);
				header.items.Add(dlcitem);
			}
			//Get strings
			header.JapaneseStrings = new string[16];
			header.EnglishStrings = new string[16];
			header.FrenchStrings = new string[16];
			header.SpanishStrings = new string[16];
			header.GermanStrings = new string[16];
			int text_pointer = BitConverter.ToInt32(file, 0x288);
			int text_count = BitConverter.ToInt32(file, 0x28C);
			Console.WriteLine("String table at {0}, count {1} (size {2})", text_pointer.ToString("X"), text_count, text_count * 64);
			List<string> strings = new List<string>();
			for (int u = 0; u < text_count; u++)
			{
				byte[] arr = new byte[64];
				Array.Copy(file, text_pointer + 64 * u, arr, 0, 64);
				int charcount = 0;
				for (int a = 0; a < arr.Length; a++)
				{
					if (arr[a] == 0) break;
					charcount++;
				}
				System.Text.Encoding enc = System.Text.Encoding.GetEncoding(932);
				if (arr[0] == 126) enc = System.Text.Encoding.GetEncoding(1252);
				string str = enc.GetString(arr, 0, charcount);
				strings.Add(str);
			}
			//Process special characters
			List<string> strings_new = new List<string>();
			foreach (string str in strings)
			{
				string newstr = System.String.Empty;
				for (int s = 0; s < str.Length; s++)
				{
					if (s == 0 && (str[s] == '~' || str[s] == 't'))
					{
						newstr += ("\t");
					}
					else if (str[s] == 't' && str[0] == 't') newstr += ("\t");
					else if (str[s] == '~' && str[0] == '~') newstr += ("\t");
					else if ((str[s] == 'n' && str[0] == 't') || (str[s] == '@' && str[0] == '~'))
						newstr += ("\n");
					else newstr += str[s];
				}
				strings_new.Add(newstr);
			}
			string[] stringarr = strings_new.ToArray();
			for (int u = 0; u < 16; u++)
			{
				if (u >= text_count) break;
				header.JapaneseStrings[u] = stringarr[u];
				if (text_count <= 16) continue;
				header.EnglishStrings[u] = stringarr[u + 16];
				if (text_count <= 32) continue;
				header.FrenchStrings[u] = stringarr[u + 32];
				if (text_count <= 48) continue;
				header.SpanishStrings[u] = stringarr[u + 48];
				if (text_count <= 64) continue;
				header.GermanStrings[u] = stringarr[u + 64];
			}
			//Check if an MLT is present
			int mlt_value = BitConverter.ToInt32(file, 0x2A0);
			if (mlt_value != 0) header.has_mlt = true;
			IniSerializer.Serialize(header, filename);
		}

		static void SaveIcon(byte[] file, string filename)
		{
			Bitmap bmp = new Bitmap(32, 32, PixelFormat.Format4bppIndexed);
			var newpalette = bmp.Palette;
			//Get palette
			ushort[] colors_short = new ushort[16];
			Color[] colors = new Color[16];
			for (int u = 0; u < 16; u++)
			{
				colors_short[u] = BitConverter.ToUInt16(file, 0x60 + u * 2);
				//Console.WriteLine("Source color {0}: {1}", u, colors_short[u].ToString("X4"));
				int a = colors_short[u] >> 12;
				int r = (colors_short[u] & 0xF00) >> 8;
				int g = (colors_short[u] & 0xF0) >> 4;
				int b = (colors_short[u] & 0xF);
				//Console.WriteLine("{0} {1} {2} {3}", a.ToString("X"), r.ToString("X"), g.ToString("X"), b.ToString("X"));
				colors[u] = Color.FromArgb(a * 16, r * 16, g * 16, b * 16);
				newpalette.Entries[u] = colors[u];
				//Console.WriteLine("Color from binary {0}: {1}", colors_short[u].ToString("X"), colors[u].ToString());
			}
			//Save icon
			bmp.Palette = newpalette;
			int pixel = 0;
			for (int y = 0; y < 32; y++)
			{
				for (int x = 0; x < 32; x += 2)
				{
					BitmapData bmpData = bmp.LockBits(new Rectangle(x, y, 2, 1), ImageLockMode.ReadOnly, PixelFormat.Format4bppIndexed);
					Marshal.WriteByte(bmpData.Scan0, file[0x80 + pixel]);
					bmp.UnlockBits(bmpData);
					pixel++;
				}
			}
			bmp.Save(filename, ImageFormat.Bmp);
		}

		static void Main(string[] args)
		{
			bool writeall = false;
			bool noencrypt = false;
			if (args.Length == 0)
			{
				Console.WriteLine("No file/folder specified.");
				Console.WriteLine("Press ENTER to exit");
				Console.ReadLine();
				return;
			}
			string filename = args[0];
			string fname = Path.GetFileNameWithoutExtension(filename);
			string dir = Path.Combine(Environment.CurrentDirectory, fname);
			for (int a = 0; a < args.Length; a++)
			{
				if (args[a] == "-w") writeall = true;
				if (args[a] == "-d") noencrypt = true;
			}
			if (Directory.Exists(filename))
			{
				byte[] vms = ConvertMetadata(Path.Combine(dir, fname + ".ini"));
				if (!noencrypt) ProcessVMS(ref vms);
				File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, fname  + "_new.vms"), vms);
				Console.Write("Output file: {0}", Path.Combine(Environment.CurrentDirectory, fname + "_new.vms"));
				if (noencrypt) Console.Write(" (no encryption)");
				Console.Write(System.Environment.NewLine);
				return;
			}
			byte[] file = File.ReadAllBytes(filename);
			Console.Write("Extracting DLC file: {0}", filename);
			if (BitConverter.ToUInt32(file, 0x280) != 0x2C0)
			{
				Console.Write(" (encrypted)");
				ProcessVMS(ref file);
			}
			if (writeall) Console.Write(" with binary data");
			Console.Write(System.Environment.NewLine);
			Directory.CreateDirectory(dir);
			MetatadaToINI(file, Path.Combine(dir, fname + ".ini"));
			SaveIcon(file, Path.Combine(dir, fname + ".bmp"));
			uint pvm_pointer = BitConverter.ToUInt32(file, 0x290);
			int pvm_value = BitConverter.ToInt32(file, 0x294);
			int pvm_count = BitConverter.ToInt32(file, 0x298);
			if (pvm_value != 0)
				Console.WriteLine("PVM at {0}, number of textures {1}", pvm_pointer.ToString("X"), pvm_count);
			uint mlt_pointer = BitConverter.ToUInt32(file, 0x29C);
			int mlt_value = BitConverter.ToInt32(file, 0x2A0);
			if (mlt_value != 0)
				Console.WriteLine("MLT at {0}", mlt_pointer.ToString("X"));
			uint prs_pointer = BitConverter.ToUInt32(file, 0x2A4);
			int prs_value = BitConverter.ToInt32(file, 0x2A8);
			if (prs_value != 0) 
				Console.WriteLine("PRS at {0}", prs_pointer.ToString("X"));
			//Checksum
			uint crc = CalculateChecksum(ref file, 0x2C0, file.Length);
			Console.WriteLine("Checksum file / calculated: {0} ({1}) / {2} ({3})", BitConverter.ToInt32(file, 0x2AC).ToString("X"), BitConverter.ToInt32(file, 0x2AC), crc.ToString("X"), (int)crc);
			//Save sections
			int pvm_size = (int)mlt_pointer - (int)pvm_pointer;
			if (pvm_size > 0)
			{
				byte[] pvmdata = new byte[pvm_size];
				Array.Copy(file, pvm_pointer, pvmdata, 0, pvm_size);
				File.WriteAllBytes(Path.Combine(dir, fname + ".pvm"), pvmdata);
			}
			int mlt_size = (int)prs_pointer - (int)mlt_pointer;
			if (mlt_size > 0)
			{
				byte[] mltdata = new byte[mlt_size];
				Array.Copy(file, mlt_pointer, mltdata, 0, mlt_size);
				File.WriteAllBytes(Path.Combine(dir, fname + ".mlt"), mltdata);
			}
			uint sectionsize = BitConverter.ToUInt32(file, 0x48);
			int text_count = BitConverter.ToInt32(file, 0x28C);
			int item_count = BitConverter.ToInt32(file, 0x284);
			int item_size = (item_count * 30 + 12); //12-byte header
			do
			{
				item_size++;
			}
			while (item_size % 16 != 0);
			int prs_size = file.Length - (int)prs_pointer;
			Console.WriteLine("Headerless size {0}, item size {1}, text size {2}, PVM size {3}, MLT size {4}, PRS size {5}", sectionsize, item_size, text_count * 64, pvm_size, mlt_size, prs_size);
			if (prs_size > 0)
			{
				byte[] prsdata = new byte[prs_size];
				//Console.WriteLine("Copy from array of size {0} from {1} to array size {2}", file.Length, prs_pointer, prsdata.Length);
				Array.Copy(file, prs_pointer, prsdata, 0, prs_size);
				if (writeall) File.WriteAllBytes(Path.Combine(dir, fname + ".prs"), prsdata);
				prsdata = FraGag.Compression.Prs.Decompress(prsdata);
				if (writeall) File.WriteAllBytes(Path.Combine(dir, fname + ".bin"), prsdata);
				//Model pointer
				uint modelpointer = BitConverter.ToUInt32(prsdata, 0) - 0xCCA4000;
				Console.WriteLine("Model pointer: {0}", modelpointer.ToString("X"));
				NJS_OBJECT mdl = new NJS_OBJECT(prsdata, (int)modelpointer, 0xCCA4000, ModelFormat.Basic, null);
				ModelFile.CreateFile((Path.Combine(dir, fname + ".sa1mdl")), mdl, null, null, null, null, ModelFormat.Basic); 
			}
			Console.WriteLine("Output folder: {0}", dir);
		}
	}
}