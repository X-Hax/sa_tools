using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SonicRetro.SAModel
{
	public class LandTable
	{
		public const ulong SA1LVL = 0x4C564C314153u;
		public const ulong SA2LVL = 0x4C564C324153u;
		public const ulong SA2BLVL = 0x4C564C42324153u;
		public const ulong FormatMask = 0xFFFFFFFFFFFFFFu;
		public const ulong CurrentVersion = 3;
		public const ulong SA1LVLVer = SA1LVL | (CurrentVersion << 56);
		public const ulong SA2LVLVer = SA2LVL | (CurrentVersion << 56);
		public const ulong SA2BLVLVer = SA2BLVL | (CurrentVersion << 56);

		public List<COL> COL { get; set; }
		public string COLName { get; set; }
		public List<GeoAnimData> Anim { get; set; }
		public string AnimName { get; set; }
		public int Flags { get; set; }
		public float Unknown1 { get; set; }
		public string TextureFileName { get; set; }
		public uint TextureList { get; set; }
		public int Unknown2 { get; set; }
		public int Unknown3 { get; set; }
		public string Name { get; set; }
		public LandTableFormat Format { get; private set; }
		public string Author { get; set; }
		public string Description { get; set; }
		public Dictionary<uint, byte[]> Metadata { get; set; }

		public static int Size(LandTableFormat format)
		{
			switch (format)
			{
				case LandTableFormat.SA1:
				case LandTableFormat.SADX:
					return 0x24;
				case LandTableFormat.SA2:
					return 0x20;
				default:
					throw new ArgumentOutOfRangeException("format");
			}
		}

		public LandTable()
		{
			Name = "landtable_" + Extensions.GenerateIdentifier();
			COL = new List<COL>();
			COLName = "collist_" + Extensions.GenerateIdentifier();
			Anim = new List<GeoAnimData>();
			AnimName = "animlist_" + Extensions.GenerateIdentifier();
			Metadata = new Dictionary<uint, byte[]>();
		}

		public LandTable(byte[] file, int address, uint imageBase, LandTableFormat format)
			: this(file, address, imageBase, format, new Dictionary<int, string>())
		{
		}

		public LandTable(byte[] file, int address, uint imageBase, LandTableFormat format, Dictionary<int, string> labels)
		{
			Format = format;
			if (labels.ContainsKey(address))
				Name = labels[address];
			else
				Name = "landtable_" + address.ToString("X8");
			short colcnt = ByteConverter.ToInt16(file, address);
			Dictionary<int, Attach> attaches = new Dictionary<int, Attach>();
			switch (format)
			{
				case LandTableFormat.SA1:
				case LandTableFormat.SADX:
					short anicnt = ByteConverter.ToInt16(file, address + 2);
					Flags = ByteConverter.ToInt32(file, address + 4);
					Unknown1 = ByteConverter.ToSingle(file, address + 8);
					COL = new List<COL>();
					int tmpaddr = ByteConverter.ToInt32(file, address + 0xC);
					if (tmpaddr != 0)
					{
						tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
						if (labels.ContainsKey(tmpaddr))
							COLName = labels[tmpaddr];
						else
							COLName = "collist_" + tmpaddr.ToString("X8");
						for (int i = 0; i < colcnt; i++)
						{
							COL.Add(new COL(file, tmpaddr, imageBase, format, labels, attaches));
							tmpaddr += SAModel.COL.Size(format);
						}
					}
					else
						COLName = "collist_" + Extensions.GenerateIdentifier();
					Anim = new List<GeoAnimData>();
					tmpaddr = ByteConverter.ToInt32(file, address + 0x10);
					if (tmpaddr != 0)
					{
						tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
						if (labels.ContainsKey(tmpaddr))
							AnimName = labels[tmpaddr];
						else
							AnimName = "animlist_" + tmpaddr.ToString("X8");
						for (int i = 0; i < anicnt; i++)
						{
							Anim.Add(new GeoAnimData(file, tmpaddr, imageBase, format, labels, attaches));
							tmpaddr += GeoAnimData.Size;
						}
					}
					else
						AnimName = "animlist_" + Extensions.GenerateIdentifier();
					tmpaddr = ByteConverter.ToInt32(file, address + 0x14);
					if (tmpaddr != 0)
					{
						tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
						TextureFileName = file.GetCString(tmpaddr, Encoding.ASCII);
					}
					TextureList = ByteConverter.ToUInt32(file, address + 0x18);
					Unknown2 = ByteConverter.ToInt32(file, address + 0x1C);
					Unknown3 = ByteConverter.ToInt32(file, address + 0x20);
					break;
				case LandTableFormat.SA2:
				case LandTableFormat.SA2B:
					short cnkcnt = ByteConverter.ToInt16(file, address + 2);
					Unknown1 = ByteConverter.ToSingle(file, address + 0xC);
					COL = new List<COL>();
					tmpaddr = ByteConverter.ToInt32(file, address + 0x10);
					if (tmpaddr != 0)
					{
						tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
						if (labels.ContainsKey(tmpaddr))
							COLName = labels[tmpaddr];
						else
							COLName = "collist_" + tmpaddr.ToString("X8");
						for (int i = 0; i < colcnt; i++)
						{
							COL.Add(new COL(file, tmpaddr, imageBase, format, labels, cnkcnt < 0 ? null : (bool?)(i >= cnkcnt), attaches));
							tmpaddr += SAModel.COL.Size(format);
						}
					}
					else
						COLName = "collist_" + Extensions.GenerateIdentifier();
					Anim = new List<GeoAnimData>();
					AnimName = "animlist_" + Extensions.GenerateIdentifier();
					tmpaddr = ByteConverter.ToInt32(file, address + 0x18);
					if (tmpaddr != 0)
					{
						tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
						TextureFileName = file.GetCString(tmpaddr, Encoding.ASCII);
					}
					TextureList = ByteConverter.ToUInt32(file, address + 0x1C);
					break;
			}
			Metadata = new Dictionary<uint, byte[]>();
		}

		public static LandTable LoadFromFile(string filename)
		{
			bool be = ByteConverter.BigEndian;
			ByteConverter.BigEndian = false;
			byte[] file = File.ReadAllBytes(filename);
			ulong magic = ByteConverter.ToUInt64(file, 0) & FormatMask;
			byte version = file[7];
			if (version > CurrentVersion)
				throw new FormatException("Not a valid SA1LVL/SA2LVL file.");
			Dictionary<int, string> labels = new Dictionary<int, string>();
			string author = null, description = null;
			Dictionary<uint, byte[]> meta = new Dictionary<uint, byte[]>();
			if (version < 2)
			{
				if (version == 1)
				{
					int tmpaddr = ByteConverter.ToInt32(file, 0xC);
					if (tmpaddr != 0)
					{
						int addr = ByteConverter.ToInt32(file, tmpaddr);
						while (addr != -1)
						{
							labels.Add(addr, file.GetCString(ByteConverter.ToInt32(file, tmpaddr + 4)));
							tmpaddr += 8;
							addr = ByteConverter.ToInt32(file, tmpaddr);
						}
					}
				}
			}
			else
			{
				int tmpaddr = ByteConverter.ToInt32(file, 0xC);
				if (tmpaddr != 0)
				{
					bool finished = false;
					while (!finished)
					{
						ChunkTypes type = (ChunkTypes)ByteConverter.ToUInt32(file, tmpaddr);
						int chunksz = ByteConverter.ToInt32(file, tmpaddr + 4);
						int nextchunk = tmpaddr + 8 + chunksz;
						tmpaddr += 8;
						if (version == 2)
						{
							switch (type)
							{
								case ChunkTypes.Label:
									while (ByteConverter.ToInt64(file, tmpaddr) != -1)
									{
										labels.Add(ByteConverter.ToInt32(file, tmpaddr), file.GetCString(ByteConverter.ToInt32(file, tmpaddr + 4)));
										tmpaddr += 8;
									}
									break;
								case ChunkTypes.Author:
									author = file.GetCString(tmpaddr);
									break;
								case ChunkTypes.Tool:
									break;
								case ChunkTypes.Description:
									description = file.GetCString(tmpaddr);
									break;
								case ChunkTypes.End:
									finished = true;
									break;
							}
						}
						else
						{
							byte[] chunk = new byte[chunksz];
							Array.Copy(file, tmpaddr, chunk, 0, chunksz);
							int chunkaddr = 0;
							switch (type)
							{
								case ChunkTypes.Label:
									while (ByteConverter.ToInt64(chunk, chunkaddr) != -1)
									{
										labels.Add(ByteConverter.ToInt32(chunk, chunkaddr),
											chunk.GetCString(ByteConverter.ToInt32(chunk, chunkaddr + 4)));
										chunkaddr += 8;
									}
									break;
								case ChunkTypes.Author:
									author = chunk.GetCString(0);
									break;
								case ChunkTypes.Tool:
									break;
								case ChunkTypes.Description:
									description = chunk.GetCString(0);
									break;
								case ChunkTypes.End:
									finished = true;
									break;
								default:
									meta.Add((uint)type, chunk);
									break;
							}
						}
						tmpaddr = nextchunk;
					}
				}
			}
			if (magic == SA1LVL)
			{
				LandTable table = new LandTable(file, ByteConverter.ToInt32(file, 8), 0, LandTableFormat.SA1, labels)
				{
					Author = author,
					Description = description,
					Metadata = meta
				};
				ByteConverter.BigEndian = be;
				return table;
			}
			if (magic == SA2LVL)
			{
				LandTable table = new LandTable(file, ByteConverter.ToInt32(file, 8), 0, LandTableFormat.SA2, labels)
				{
					Author = author,
					Description = description,
					Metadata = meta
				};
				ByteConverter.BigEndian = be;
				return table;
			}
			if (magic == SA2BLVL)
			{
				LandTable table = new LandTable(file, ByteConverter.ToInt32(file, 8), 0, LandTableFormat.SA2B, labels)
				{
					Author = author,
					Description = description,
					Metadata = meta
				};
				ByteConverter.BigEndian = be;
				return table;
			}
			ByteConverter.BigEndian = be;
			throw new FormatException("Not a valid SA1LVL/SA2LVL file.");
		}

		public static bool CheckLevelFile(string filename)
		{
			bool be = ByteConverter.BigEndian;
			ByteConverter.BigEndian = false;
			byte[] file = File.ReadAllBytes(filename);
			ulong format = ByteConverter.ToUInt64(file, 0) & FormatMask;
			ByteConverter.BigEndian = be;
			switch (format)
			{
				case SA1LVL:
				case SA2LVL:
				case SA2BLVL:
					return file[7] <= CurrentVersion;
				default:
					return false;
			}
		}

		public byte[] GetBytes(uint imageBase, LandTableFormat format, Dictionary<string, uint> labels, out uint address)
		{
			List<byte> result = new List<byte>();
			byte[] tmpbyte;
			uint[] colmdladdrs = new uint[COL.Count];
			uint tmpaddr;
			List<COL> cnk = new List<COL>();
			List<COL> bas = new List<COL>();
			foreach (COL item in COL)
			{
				if (item.Model.Attach is BasicAttach)
					bas.Add(item);
				else
					cnk.Add(item);
			}
			COL.Clear();
			COL.AddRange(cnk);
			COL.AddRange(bas);
			for (int i = 0; i < COL.Count; i++)
			{
				if (labels.ContainsKey(COL[i].Model.Name))
					colmdladdrs[i] = labels[COL[i].Model.Name];
				else
				{
					result.Align(4);
					tmpbyte = COL[i].Model.GetBytes(imageBase + (uint)result.Count, format == LandTableFormat.SADX, labels, out tmpaddr);
					colmdladdrs[i] = tmpaddr + (uint)result.Count + imageBase;
					result.AddRange(tmpbyte);
				}
			}
			uint[] animmdladdrs = new uint[Anim.Count];
			uint[] animaniaddrs = new uint[Anim.Count];
			for (int i = 0; i < Anim.Count; i++)
			{
				if (labels.ContainsKey(Anim[i].Model.Name))
					animmdladdrs[i] = labels[Anim[i].Model.Name];
				else
				{
					result.Align(4);
					tmpbyte = Anim[i].Model.GetBytes(imageBase + (uint)result.Count, format == LandTableFormat.SADX, labels,
						out tmpaddr);
					animmdladdrs[i] = tmpaddr + (uint)result.Count + imageBase;
					result.AddRange(tmpbyte);
				}
				uint mtnaddr;
				if (labels.ContainsKey(Anim[i].Animation.Name))
					mtnaddr = labels[Anim[i].Animation.Name];
				else
				{
					result.Align(4);
					tmpbyte = Anim[i].Animation.GetBytes(imageBase + (uint)result.Count, labels, out tmpaddr);
					mtnaddr = tmpaddr + (uint)result.Count + imageBase;
					result.AddRange(tmpbyte);
				}
				result.Align(4);
				animaniaddrs[i] = (uint)result.Count + imageBase;
				result.AddRange(ByteConverter.GetBytes(animmdladdrs[i]));
				result.AddRange(ByteConverter.GetBytes(mtnaddr));
			}
			uint coladdr;
			if (COL.Count > 0)
			{
				if (labels.ContainsKey(COLName))
					coladdr = labels[COLName];
				else
				{
					coladdr = imageBase + (uint)result.Count;
					labels.Add(COLName, coladdr);
					for (int i = 0; i < COL.Count; i++)
					{
						result.Align(4);
						result.AddRange(COL[i].GetBytes(imageBase + (uint)result.Count, colmdladdrs[i], format));
					}
				}
			}
			else
				coladdr = 0;
			uint animaddr;
			if (Anim.Count > 0)
			{
				if (labels.ContainsKey(AnimName))
					animaddr = labels[AnimName];
				else
				{
					animaddr = imageBase + (uint)result.Count;
					if (!labels.ContainsKey(AnimName)) labels.Add(AnimName, animaddr);
					for (int i = 0; i < Anim.Count; i++)
					{
						if (!labels.ContainsValue(animaniaddrs[i])) labels.Add(Anim[i].ActionName, animaniaddrs[i]);
						result.Align(4);
						result.AddRange(Anim[i].GetBytes(imageBase + (uint)result.Count, animmdladdrs[i], animaniaddrs[i]));
					}
				}
			}
			else
				animaddr = 0;
			result.Align(4);
			uint texnameaddr = 0;
			if (TextureFileName != null)
			{
				texnameaddr = imageBase + (uint)result.Count;
				result.AddRange(Encoding.ASCII.GetBytes(TextureFileName));
				result.Add(0);
			}
			result.Align(4);
			address = (uint)result.Count;
			result.AddRange(ByteConverter.GetBytes((ushort)COL.Count));
			switch (format)
			{
				case LandTableFormat.SA1:
				case LandTableFormat.SADX:
					result.AddRange(ByteConverter.GetBytes((ushort)Anim.Count));
					result.AddRange(ByteConverter.GetBytes(Flags));
					result.AddRange(ByteConverter.GetBytes(Unknown1));
					result.AddRange(ByteConverter.GetBytes(coladdr));
					result.AddRange(ByteConverter.GetBytes(animaddr));
					result.AddRange(ByteConverter.GetBytes(texnameaddr));
					result.AddRange(ByteConverter.GetBytes(TextureList));
					result.AddRange(ByteConverter.GetBytes(Unknown2));
					result.AddRange(ByteConverter.GetBytes(Unknown3));
					break;
				case LandTableFormat.SA2:
				case LandTableFormat.SA2B:
					result.AddRange(ByteConverter.GetBytes((ushort)cnk.Count));
					result.AddRange(new byte[8]); // TODO: figure out what these do
					result.AddRange(ByteConverter.GetBytes(Unknown1));
					result.AddRange(ByteConverter.GetBytes(coladdr));
					result.AddRange(ByteConverter.GetBytes(animaddr));
					result.AddRange(ByteConverter.GetBytes(texnameaddr));
					result.AddRange(ByteConverter.GetBytes(TextureList));
					break;
			}
			labels.Add(Name, address + imageBase);
			return result.ToArray();
		}

		public byte[] GetBytes(uint imageBase, LandTableFormat format, out uint address)
		{
			return GetBytes(imageBase, format, new Dictionary<string, uint>(), out address);
		}

		public byte[] GetBytes(uint imageBase, LandTableFormat format)
		{
			return GetBytes(imageBase, format, out uint address);
		}

		public void ToStructVariables(TextWriter writer, LandTableFormat format, List<string> labels, string[] textures = null)
		{
			List<COL> cnk = new List<COL>();
			List<COL> bas = new List<COL>();
			foreach (COL item in COL)
			{
				if (item.Model.Attach is BasicAttach)
					bas.Add(item);
				else
					cnk.Add(item);
			}
			COL.Clear();
			COL.AddRange(cnk);
			COL.AddRange(bas);
			for (int i = 0; i < COL.Count; i++)
			{
				if (!labels.Contains(COL[i].Model.Name))
				{
					labels.Add(COL[i].Model.Name);
					COL[i].Model.ToStructVariables(writer, format == LandTableFormat.SADX, labels, textures);
				}
			}
			for (int i = 0; i < Anim.Count; i++)
			{
				if (!labels.Contains(Anim[i].Model.Name))
				{
					labels.Add(Anim[i].Model.Name);
					Anim[i].Model.ToStructVariables(writer, format == LandTableFormat.SADX, labels, textures);
				}
				if (!labels.Contains(Anim[i].ActionName))
				{
					labels.Add(Anim[i].ActionName);
					if (!labels.Contains(Anim[i].Animation.Name)) Anim[i].Animation.ToStructVariables(writer, labels);
					writer.Write("NJS_ACTION ");
					writer.Write(Anim[i].ActionName);
					writer.Write(" = { &");
					writer.Write(Anim[i].Model.Name);
					writer.Write(", &");
					writer.Write(Anim[i].Animation.Name);
					writer.WriteLine(" };");
					writer.WriteLine();
				}
			}
			if (!labels.Contains(COLName))
			{
				labels.Add(COLName);
				writer.Write("COL ");
				writer.Write(COLName);
				writer.WriteLine("[] = {");
				List<string> lines = new List<string>(COL.Count);
				foreach (COL item in COL)
					lines.Add(item.ToStruct(format));
				writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", lines.ToArray()));
				writer.WriteLine("};");
				writer.WriteLine();
			}
			if (Anim.Count > 0 && !labels.Contains(AnimName))
			{
				labels.Add(AnimName);
				writer.Write("GeoAnimData ");
				writer.Write(AnimName);
				writer.WriteLine("[] = {");
				List<string> lines = new List<string>(Anim.Count);
				foreach (GeoAnimData item in Anim)
					lines.Add(item.ToStruct());
				writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", lines.ToArray()));
				writer.WriteLine("};");
				writer.WriteLine();
			}
			writer.Write("LandTable ");
			writer.Write(Name);
			writer.Write(" = { LengthOfArray<int16_t>(");
			writer.Write(COLName);
			writer.Write("), ");
			switch (format)
			{
				case LandTableFormat.SA1:
				case LandTableFormat.SADX:
					writer.Write(Anim.Count > 0 ? "LengthOfArray<int16_t>(" + AnimName + ")" : "0");
					writer.Write(", ");
					writer.Write(Flags.ToCHex());
					writer.Write(", ");
					writer.Write(Unknown1.ToC());
					writer.Write(", ");
					writer.Write(COLName);
					writer.Write(", ");
					writer.Write(Anim.Count > 0 ? AnimName : "NULL");
					writer.Write(", ");
					writer.Write(TextureFileName.ToC());
					writer.Write(", (NJS_TEXLIST *)");
					writer.Write(TextureList.ToCHex());
					writer.Write(", ");
					writer.Write(Unknown2.ToCHex());
					writer.Write(", ");
					writer.Write(Unknown3.ToCHex());
					break;
				case LandTableFormat.SA2:
					writer.Write(cnk.Count);
					writer.Write(", 0, 0, 0, 0, ");
					writer.Write(Unknown1.ToC());
					writer.Write(", ");
					writer.Write(COLName);
					writer.Write(", ");
					writer.Write(Anim.Count > 0 ? AnimName : "NULL");
					writer.Write(", ");
					writer.Write(TextureFileName.ToC());
					writer.Write(", (NJS_TEXLIST *)");
					writer.Write(TextureList.ToCHex());
					break;
			}
			writer.WriteLine(" };");
		}

		public string ToStructVariables(LandTableFormat format, List<string> labels, string[] textures = null)
		{
			using (StringWriter sw = new StringWriter())
			{
				ToStructVariables(sw, format, labels, textures);
				return sw.ToString();
			}
		}

		public void SaveToFile(string filename, LandTableFormat format)
		{
			bool be = ByteConverter.BigEndian;
			ByteConverter.BigEndian = false;
			if (format == LandTableFormat.SADX)
				format = LandTableFormat.SA1;
			List<byte> file = new List<byte>();
			ulong magic;
			switch (format)
			{
				case LandTableFormat.SA1:
					magic = SA1LVLVer;
					break;
				case LandTableFormat.SA2:
					magic = SA2LVLVer;
					break;
				case LandTableFormat.SA2B:
					magic = SA2BLVLVer;
					break;
				default:
					throw new ArgumentException("Cannot save " + format + " format levels to file!", "format");
			}
			file.AddRange(ByteConverter.GetBytes(magic));
			Dictionary<string, uint> labels = new Dictionary<string, uint>();
			byte[] lvl = GetBytes(0x10, format, labels, out uint addr);
			file.AddRange(ByteConverter.GetBytes(addr + 0x10));
			file.AddRange(ByteConverter.GetBytes(lvl.Length + 0x10));
			file.AddRange(lvl);
			if (labels.Count > 0)
			{
				List<byte> chunk = new List<byte>(labels.Count * 8);
				int straddr = (labels.Count * 8) + 8;
				List<byte> strbytes = new List<byte>();
				foreach (KeyValuePair<string, uint> label in labels)
				{
					chunk.AddRange(ByteConverter.GetBytes(label.Value));
					chunk.AddRange(ByteConverter.GetBytes(straddr + strbytes.Count));
					strbytes.AddRange(Encoding.UTF8.GetBytes(label.Key));
					strbytes.Add(0);
					strbytes.Align(4);
				}
				chunk.AddRange(ByteConverter.GetBytes(-1L));
				chunk.AddRange(strbytes);
				file.AddRange(ByteConverter.GetBytes((uint)ChunkTypes.Label));
				file.AddRange(ByteConverter.GetBytes(chunk.Count));
				file.AddRange(chunk);
			}
			if (!string.IsNullOrEmpty(Author))
			{
				List<byte> chunk = new List<byte>(Author.Length + 1);
				chunk.AddRange(Encoding.UTF8.GetBytes(Author));
				chunk.Add(0);
				chunk.Align(4);
				file.AddRange(ByteConverter.GetBytes((uint)ChunkTypes.Author));
				file.AddRange(ByteConverter.GetBytes(chunk.Count));
				file.AddRange(chunk);
			}
			if (!string.IsNullOrEmpty(Description))
			{
				List<byte> chunk = new List<byte>(Description.Length + 1);
				chunk.AddRange(Encoding.UTF8.GetBytes(Description));
				chunk.Add(0);
				chunk.Align(4);
				file.AddRange(ByteConverter.GetBytes((uint)ChunkTypes.Description));
				file.AddRange(ByteConverter.GetBytes(chunk.Count));
				file.AddRange(chunk);
			}
			foreach (KeyValuePair<uint, byte[]> item in Metadata)
			{
				file.AddRange(ByteConverter.GetBytes(item.Key));
				file.AddRange(ByteConverter.GetBytes(item.Value.Length));
				file.AddRange(item.Value);
			}
			file.AddRange(ByteConverter.GetBytes((uint)ChunkTypes.End));
			file.AddRange(new byte[4]);
			File.WriteAllBytes(filename, file.ToArray());
			ByteConverter.BigEndian = be;
		}

		public enum ChunkTypes : uint
		{
			Label = 0x4C42414C,
			Author = 0x48545541,
			Tool = 0x4C4F4F54,
			Description = 0x43534544,
			End = 0x444E45
		}
	}
}
