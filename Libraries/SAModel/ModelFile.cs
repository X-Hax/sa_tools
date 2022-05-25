using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SAModel
{
	public class ModelFile
	{
		public const uint NJCMMagic = 0x4D434A4E;
		public const uint NJBMMagic = 0x4D424A4E;
		public const uint GJCMMagic = 0x4D434A47;
		public const ulong SA1MDL = 0x4C444D314153u;
		public const ulong SA2MDL = 0x4C444D324153u;
		public const ulong SA2BMDL = 0x4C444D42324153u;
		public const ulong FormatMask = 0xFFFFFFFFFFFFFFu;
		public const ulong CurrentVersion = 3;
		public const ulong SA1MDLVer = SA1MDL | (CurrentVersion << 56);
		public const ulong SA2MDLVer = SA2MDL | (CurrentVersion << 56);
		public const ulong SA2BMDLVer = SA2BMDL | (CurrentVersion << 56);

		public ModelFormat Format { get; private set; }
		public NJS_OBJECT Model { get; private set; }
		public ReadOnlyCollection<NJS_MOTION> Animations { get; private set; }
		public string Author { get; set; }
		public string Description { get; set; }
		public Dictionary<uint, byte[]> Metadata { get; set; }
		private string[] animationFiles;

		public ModelFile(string filename)
			: this(File.ReadAllBytes(filename), filename)
		{
		}

		public ModelFile(byte[] file, string filename = null)
		{
			int tmpaddr;
			bool be = ByteConverter.BigEndian;
			ByteConverter.BigEndian = false;
			ulong magic = ByteConverter.ToUInt64(file, 0) & FormatMask;
			byte version = file[7];
			if (version > CurrentVersion)
				throw new FormatException("Not a valid SA1MDL/SA2MDL file.");
			Metadata = new Dictionary<uint, byte[]>();
			Dictionary<int, string> labels = new Dictionary<int, string>();
			Dictionary<int, Attach> attaches = new Dictionary<int, Attach>();
			if (version < 2)
			{
				if (version == 1)
				{
					tmpaddr = ByteConverter.ToInt32(file, 0x14);
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
				switch (magic)
				{
					case SA1MDL:
						Format = ModelFormat.Basic;
						break;
					case SA2MDL:
						Format = ModelFormat.Chunk;
						break;
					default:
						throw new FormatException("Not a valid SA1MDL/SA2MDL file.");
				}
				Model = new NJS_OBJECT(file, ByteConverter.ToInt32(file, 8), 0, Format, labels, attaches);
				if (filename != null)
				{
					tmpaddr = ByteConverter.ToInt32(file, 0xC);
					if (tmpaddr != 0)
					{
						List<string> animfiles = new List<string>();
						int addr = ByteConverter.ToInt32(file, tmpaddr);
						while (addr != -1)
						{
							animfiles.Add(file.GetCString(addr));
							tmpaddr += 4;
							addr = ByteConverter.ToInt32(file, tmpaddr);
						}
						animationFiles = animfiles.ToArray();
					}
					else
						animationFiles = new string[0];
					string path = Path.GetDirectoryName(filename);
					List<NJS_MOTION> anims = new List<NJS_MOTION>();
					try
					{
						foreach (string item in animationFiles)
							anims.Add(NJS_MOTION.Load(Path.Combine(path, item), Model.CountAnimated()));
					}
					catch
					{
						anims.Clear();
					}
					Animations = anims.AsReadOnly();
				}
			}
			else
			{
				animationFiles = new string[0];
				tmpaddr = ByteConverter.ToInt32(file, 0xC);
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
								case ChunkTypes.Animation:
									List<string> animfiles = new List<string>();
									while (ByteConverter.ToInt32(file, tmpaddr) != -1)
									{
										animfiles.Add(file.GetCString(ByteConverter.ToInt32(file, tmpaddr)));
										tmpaddr += 4;
									}
									animationFiles = animfiles.ToArray();
									break;
								case ChunkTypes.Morph:
									break;
								case ChunkTypes.Author:
									Author = file.GetCString(tmpaddr);
									break;
								case ChunkTypes.Tool:
									break;
								case ChunkTypes.Description:
									Description = file.GetCString(tmpaddr);
									break;
								case ChunkTypes.Texture:
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
								case ChunkTypes.Animation:
									List<string> animchunks = new List<string>();
									while (ByteConverter.ToInt32(chunk, chunkaddr) != -1)
									{
										animchunks.Add(chunk.GetCString(ByteConverter.ToInt32(chunk, chunkaddr)));
										chunkaddr += 4;
									}
									animationFiles = animchunks.ToArray();
									break;
								case ChunkTypes.Morph:
									break;
								case ChunkTypes.Author:
									Author = chunk.GetCString(chunkaddr);
									break;
								case ChunkTypes.Tool:
									break;
								case ChunkTypes.Description:
									Description = chunk.GetCString(chunkaddr);
									break;
								case ChunkTypes.End:
									finished = true;
									break;
								default:
									Metadata.Add((uint)type, chunk);
									break;
							}
						}
						tmpaddr = nextchunk;
					}
				}
				switch (magic)
				{
					case SA1MDL:
						Format = ModelFormat.Basic;
						break;
					case SA2MDL:
						Format = ModelFormat.Chunk;
						break;
					case SA2BMDL:
						Format = ModelFormat.GC;
						break;
					default:
						throw new FormatException("Not a valid SA1MDL/SA2MDL file.");
				}
				Model = new NJS_OBJECT(file, ByteConverter.ToInt32(file, 8), 0, Format, labels, attaches);
				if (filename != null)
				{
					string path = Path.GetDirectoryName(filename);
					if (File.Exists(Path.GetFileNameWithoutExtension(filename) + ".action"))
					{
						using (TextReader tr = File.OpenText(Path.GetFileNameWithoutExtension(filename) + ".action"))
						{
							List<string> animlist = new List<string>();
							int count = File.ReadLines(Path.GetFileNameWithoutExtension(filename) + ".action").Count();
							for (int i = 0; i < count; i++)
							{
								string line = tr.ReadLine();
								if (File.Exists(Path.Combine(path, line))) animlist.Add(line);
							}
							animationFiles = animlist.ToArray();
						}
					}
					List<NJS_MOTION> anims = new List<NJS_MOTION>();
					try
					{
						foreach (string item in animationFiles)
						{
							if (Path.GetExtension(item).ToLowerInvariant() == ".json")
							{
								JsonSerializer js = new JsonSerializer() { Culture = System.Globalization.CultureInfo.InvariantCulture };
								using (TextReader tr = File.OpenText(Path.Combine(path, item)))
								{
									using (JsonTextReader jtr = new JsonTextReader(tr))
										anims.Add(js.Deserialize<NJS_MOTION>(jtr));
								}
							}
							else
								anims.Add(NJS_MOTION.Load(Path.Combine(path, item), Model.CountAnimated()));
						}
					}
					catch
					{
						anims.Clear();
					}
					Animations = anims.AsReadOnly();
				}
			}
			ByteConverter.BigEndian = be;
		}

		public static bool CheckModelFile(string filename)
		{
			bool be = ByteConverter.BigEndian;
			ByteConverter.BigEndian = false;
			byte[] file = File.ReadAllBytes(filename);
			ulong format = ByteConverter.ToUInt64(file, 0) & FormatMask;
			ByteConverter.BigEndian = be;
			switch (format)
			{
				case SA1MDL:
				case SA2MDL:
				case SA2BMDL:
					return file[7] <= CurrentVersion;
				default:
					return false;
			}
		}

		public void SaveToFile(string filename)
		{
			bool be = ByteConverter.BigEndian;
			ByteConverter.BigEndian = false;
			List<byte> file = new List<byte>();
			ulong magic;
			switch (Format)
			{
				case ModelFormat.Basic:
					magic = SA1MDLVer;
					break;
				case ModelFormat.Chunk:
					magic = SA2MDLVer;
					break;
				case ModelFormat.GC:
					magic = SA2BMDLVer;
					break;
				default:
					throw new ArgumentException("Cannot save " + Format.ToString() + " format models to file!", "Format");
			}
			file.AddRange(ByteConverter.GetBytes(magic));
			Dictionary<string, uint> labels = new Dictionary<string, uint>();
			byte[] mdl = Model.GetBytes(0x10, false, labels, new List<uint>(), out uint addr);
			file.AddRange(ByteConverter.GetBytes(addr + 0x10));
			file.AddRange(ByteConverter.GetBytes(mdl.Length + 0x10));
			file.AddRange(mdl);
			string path = Path.GetDirectoryName(filename);
			if (labels.Count > 0)
			{
				List<byte> chunk = new List<byte>((labels.Count * 8) + 8);
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
			if (Animations.Count > 0)
			{
				using (TextWriter tw = File.CreateText(Path.ChangeExtension(filename, ".action")))
				{
					for (int a = 0; a < animationFiles.Count(); a++)
					{
						tw.WriteLine(animationFiles[a]);
					}
					tw.Flush();
					tw.Close();
				}
				/*
				//Old animation code
				List<byte> chunk = new List<byte>((Animations.Count + 1) * 4);
				int straddr = (Animations.Count + 1) * 4;
				List<byte> strbytes = new List<byte>();
				for (int i = 0; i < Animations.Count; i++)
				{
					//Animations[i].Save(Path.Combine(path, animationFiles[i]));
					chunk.AddRange(ByteConverter.GetBytes(straddr + strbytes.Count));
					strbytes.AddRange(Encoding.UTF8.GetBytes(animationFiles[i]));
					strbytes.Add(0);
					strbytes.Align(4);
				}
				chunk.AddRange(ByteConverter.GetBytes(-1));
				chunk.AddRange(strbytes);
				file.AddRange(ByteConverter.GetBytes((uint)ChunkTypes.Animation));
				file.AddRange(ByteConverter.GetBytes(chunk.Count));
				file.AddRange(chunk);
				*/
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

		public static void CreateFile(string filename, NJS_OBJECT model, string[] animationFiles, string author,
			string description, Dictionary<uint, byte[]> metadata, ModelFormat format, bool nometa = false, bool useNinjaMetaData = false)
		{
			uint ninjaMagic;
			uint imageBase = (uint)(useNinjaMetaData ? 0 : 0x10);
			bool be = ByteConverter.BigEndian;
			if(useNinjaMetaData == false)
			{
				ByteConverter.BigEndian = false;
			}
			if (format == ModelFormat.BasicDX)
				format = ModelFormat.Basic;
			List<byte> file = new List<byte>();
			ulong magic;
			switch (format)
			{
				case ModelFormat.Basic:
				case ModelFormat.BasicDX:
					magic = SA1MDLVer;
					ninjaMagic = NJBMMagic;
					break;
				case ModelFormat.Chunk:
					magic = SA2MDLVer;
					ninjaMagic = NJCMMagic;
					break;
				case ModelFormat.GC:
					magic = SA2BMDLVer;
					ninjaMagic = GJCMMagic;
					break;
				default:
					throw new ArgumentException("Cannot save " + format.ToString() + " format models to file!", "format");
			}
			Dictionary<string, uint> labels = new Dictionary<string, uint>();
			List<uint> njOffsets = new List<uint>();
			byte[] mdl;
			uint addr;

			if(useNinjaMetaData == true)
			{
				mdl = model.NJGetBytes(imageBase, false, labels, njOffsets, out addr);
				//***Ninja metadata should always be little endian!***
				file.AddRange(BitConverter.GetBytes(ninjaMagic));
				file.AddRange(BitConverter.GetBytes(mdl.Length));
				//***Ninja metadata should always be little endian!***
			}
			else
			{
				mdl = model.GetBytes(imageBase, false, labels, njOffsets, out addr);
				file.AddRange(ByteConverter.GetBytes(magic));
				file.AddRange(ByteConverter.GetBytes(addr + 0x10));
				file.AddRange(ByteConverter.GetBytes(mdl.Length + 0x10));
			}
			file.AddRange(mdl);
			if (!nometa)
			{
				if (labels.Count > 0)
				{
					List<byte> chunk = new List<byte>((labels.Count * 8) + 8);
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
				if (animationFiles != null && animationFiles.Length > 0)
				{
					using (TextWriter tw = File.CreateText(Path.ChangeExtension(filename, ".action")))
					{
						for (int a = 0; a < animationFiles.Count(); a++)
						{
							tw.WriteLine(animationFiles[a]);
						}
						tw.Flush();
						tw.Close();
					}
					/*
					//Old animation code
					List<byte> chunk = new List<byte>((animationFiles.Length + 1) * 4);
					int straddr = (animationFiles.Length + 1) * 4;
					List<byte> strbytes = new List<byte>();
					for (int i = 0; i < animationFiles.Length; i++)
					{
						chunk.AddRange(ByteConverter.GetBytes(straddr + strbytes.Count));
						strbytes.AddRange(Encoding.UTF8.GetBytes(animationFiles[i]));
						strbytes.Add(0);
						strbytes.Align(4);
					}
					chunk.AddRange(ByteConverter.GetBytes(-1));
					chunk.AddRange(strbytes);
					file.AddRange(ByteConverter.GetBytes((uint)ChunkTypes.Animation));
					file.AddRange(ByteConverter.GetBytes(chunk.Count));
					file.AddRange(chunk);
					*/
				}
				if (!string.IsNullOrEmpty(author))
				{
					List<byte> chunk = new List<byte>(author.Length + 1);
					chunk.AddRange(Encoding.UTF8.GetBytes(author));
					chunk.Add(0);
					chunk.Align(4);
					file.AddRange(ByteConverter.GetBytes((uint)ChunkTypes.Author));
					file.AddRange(ByteConverter.GetBytes(chunk.Count));
					file.AddRange(chunk);
				}
				if (!string.IsNullOrEmpty(description))
				{
					List<byte> chunk = new List<byte>(description.Length + 1);
					chunk.AddRange(Encoding.UTF8.GetBytes(description));
					chunk.Add(0);
					chunk.Align(4);
					file.AddRange(ByteConverter.GetBytes((uint)ChunkTypes.Description));
					file.AddRange(ByteConverter.GetBytes(chunk.Count));
					file.AddRange(chunk);
				}
				if (metadata != null)
				{
					foreach (KeyValuePair<uint, byte[]> item in metadata)
					{
						file.AddRange(ByteConverter.GetBytes(item.Key));
						file.AddRange(ByteConverter.GetBytes(item.Value.Length));
						file.AddRange(item.Value);
					}
				}
			}
			if(useNinjaMetaData == true)
			{
				/*
				List<uint> addresses = new List<uint>();
				foreach(var pair in labels)
				{
					if(pair.Value != 0)
					{
						addresses.Add(pair.Value);
					}
				}
				addresses.Insert(0, 0x4);
				addresses.Sort();*/
				njOffsets = njOffsets.Distinct().ToList();
				njOffsets.Sort();
				List<byte> pof0 = new List<byte>();
				pof0.Add(0x41);
				for(int i = 1; i < njOffsets.Count; i++)
				{
					pof0.AddRange(POF0Helper.calcPOF0Pointer(njOffsets[i - 1], njOffsets[i]));
				}
				POF0Helper.finalizePOF0(pof0);
				file.AddRange(pof0);
				
				if(metadata.Count != 0 && metadata.ContainsKey(uint.MaxValue))
				{
					file.InsertRange(0, metadata[uint.MaxValue]);
				}
			} else
			{
				file.AddRange(ByteConverter.GetBytes((uint)ChunkTypes.End));
				file.AddRange(new byte[4]);
			}
			File.WriteAllBytes(filename, file.ToArray());
			ByteConverter.BigEndian = be;
		}

		public enum ChunkTypes : uint
		{
			Label = 0x4C42414C,
			Animation = 0x4D494E41,
			Morph = 0x46524F4D,
			Author = 0x48545541,
			Tool = 0x4C4F4F54,
			Description = 0x43534544,
			Texture = 0x584554,
			End = 0x444E45
		}
	}
}