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
		public const ulong XJMDL = 0x4C444D4A58u;
		public const ulong FormatMask = 0xFFFFFFFFFFFFFFu;
		public const ulong CurrentVersion = 3;
		public const ulong SA1MDLVer = SA1MDL | (CurrentVersion << 56);
		public const ulong SA2MDLVer = SA2MDL | (CurrentVersion << 56);
		public const ulong SA2BMDLVer = SA2BMDL | (CurrentVersion << 56);
		public const ulong XJMDLVer = XJMDL | (CurrentVersion << 56);

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
			byte[] wght = null;
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
								case ChunkTypes.Weights:
									wght = chunk;
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
					case XJMDL:
						Format = ModelFormat.XJ;
						break;
					default:
						throw new FormatException("Not a valid SA1MDL/SA2MDL file.");
				}
				Model = new NJS_OBJECT(file, ByteConverter.ToInt32(file, 8), 0, Format, labels, attaches);
				var nodedict = Model.EnumerateObjects().ToDictionary(a => a.Name);
				if (wght != null)
				{
					int addr = ByteConverter.ToInt32(wght, 0);
					int off = 4;
					while (addr != -1)
					{
						var mdl = nodedict[labels[addr]].Attach;
						int vcnt = ByteConverter.ToInt32(wght, off);
						off += 4;
						mdl.VertexWeights = new Dictionary<int, List<VertexWeight>>(vcnt);
						for (int vi = 0; vi < vcnt; vi++)
						{
							int ind = ByteConverter.ToInt32(wght, off);
							off += 4;
							int wcnt = ByteConverter.ToInt32(wght, off);
							off += 4;
							var weights = new List<VertexWeight>(wcnt);
							for (int wi = 0; wi < wcnt; wi++)
							{
								weights.Add(new VertexWeight(
									nodedict[labels[ByteConverter.ToInt32(wght, off)]],
									ByteConverter.ToInt32(wght, off + 4),
									ByteConverter.ToSingle(wght, off + 8)));
								off += 12;
							}
							mdl.VertexWeights.Add(ind, weights);
						}
						addr = ByteConverter.ToInt32(wght, off);
						off += 4;
					}
				}
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

		public ModelFile(ModelFormat format, NJS_OBJECT model, string basePath, params string[] animationFiles)
		{
			switch (format)
			{
				case ModelFormat.BasicDX:
					format = ModelFormat.Basic;
					break;
				case ModelFormat.Basic:
				case ModelFormat.Chunk:
				case ModelFormat.GC:
				case ModelFormat.XJ:
					break;
				default:
					throw new ArgumentException($"Cannot save {format} format models to file!", "format");
			}
			Format = format;
			Model = model;
			this.animationFiles = animationFiles;
			List<NJS_MOTION> anims = new List<NJS_MOTION>();
			if (animationFiles != null)
				try
				{
					foreach (string item in animationFiles)
					{
						if (Path.GetExtension(item).ToLowerInvariant() == ".json")
						{
							JsonSerializer js = new JsonSerializer() { Culture = System.Globalization.CultureInfo.InvariantCulture };
							using (TextReader tr = File.OpenText(Path.Combine(basePath, item)))
							{
								using (JsonTextReader jtr = new JsonTextReader(tr))
									anims.Add(js.Deserialize<NJS_MOTION>(jtr));
							}
						}
						else
							anims.Add(NJS_MOTION.Load(Path.Combine(basePath, item), Model.CountAnimated()));
					}
				}
				catch
				{
					anims.Clear();
				}
			Animations = anims.AsReadOnly();
			Metadata = new Dictionary<uint, byte[]>();
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
				case XJMDL:
					return file[7] <= CurrentVersion;
				default:
					return false;
			}
		}

		public void SaveToFile(string filename, bool nometa = false, bool useNinjaMetaData = false, bool njbLittleEndian = false)
		{
			uint ninjaMagic;
			uint imageBase = (uint)(useNinjaMetaData ? 0 : 0x10);
			bool be = ByteConverter.BigEndian;
			if (!useNinjaMetaData)
				ByteConverter.BigEndian = false;
			List<byte> file = new List<byte>();
			ulong magic;
			switch (Format)
			{
				case ModelFormat.Basic:
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
				case ModelFormat.XJ:
					magic = XJMDLVer;
					ninjaMagic = NJCMMagic; //XJ uses Chunk's magic
					break;
				default:
					throw new ArgumentException("Cannot save " + Format.ToString() + " format models to file!", "Format");
			}
			Dictionary<string, uint> labels = new Dictionary<string, uint>();
			List<uint> njOffsets = new List<uint>();
			byte[] mdl;
			uint addr;

			if (useNinjaMetaData)
			{
				mdl = Model.NJGetBytes(imageBase, false, labels, njOffsets, out addr);
				file.AddRange(BitConverter.GetBytes(ninjaMagic));
				file.AddRange(njbLittleEndian ? BitConverter.GetBytes(mdl.Length) : ByteConverter.GetBytes(mdl.Length));
			}
			else
			{
				mdl = Model.GetBytes(imageBase, false, labels, njOffsets, out addr);
				file.AddRange(ByteConverter.GetBytes(magic));
				file.AddRange(ByteConverter.GetBytes(addr + 0x10));
				file.AddRange(ByteConverter.GetBytes(mdl.Length + 0x10));
			}
			file.AddRange(mdl);
			if (!nometa)
			{
				string path = Path.GetDirectoryName(Path.GetFullPath(filename));
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
				if (Model.EnumerateObjects().Any(a => a.Attach?.VertexWeights != null))
				{
					List<byte> chunk = new List<byte>();
					foreach (var node in Model.EnumerateObjects().Where(a => a.Attach?.VertexWeights != null))
					{
						chunk.AddRange(ByteConverter.GetBytes(labels[node.Name]));
						chunk.AddRange(ByteConverter.GetBytes(node.Attach.VertexWeights.Count));
						foreach (var vert in node.Attach.VertexWeights)
						{
							chunk.AddRange(ByteConverter.GetBytes(vert.Key));
							chunk.AddRange(ByteConverter.GetBytes(vert.Value.Count));
							foreach (var weight in vert.Value)
							{
								chunk.AddRange(ByteConverter.GetBytes(labels[weight.Node.Name]));
								chunk.AddRange(ByteConverter.GetBytes(weight.Vertex));
								chunk.AddRange(ByteConverter.GetBytes(weight.Weight));
							}
						}
					}
					chunk.AddRange(ByteConverter.GetBytes(-1));
					file.AddRange(ByteConverter.GetBytes((uint)ChunkTypes.Weights));
					file.AddRange(ByteConverter.GetBytes(chunk.Count));
					file.AddRange(chunk);
				}
				foreach (KeyValuePair<uint, byte[]> item in Metadata)
				{
					file.AddRange(ByteConverter.GetBytes(item.Key));
					file.AddRange(ByteConverter.GetBytes(item.Value.Length));
					file.AddRange(item.Value);
				}
			}
			if (useNinjaMetaData)
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
				StringBuilder sb = new StringBuilder();
				foreach (uint off in njOffsets)
					sb.AppendLine(off.ToString("X"));
				File.WriteAllText("C:\\Users\\PkR\\Desktop\\offsets.txt", sb.ToString());
				file.AddRange(POF0Helper.GetPOFData(njOffsets));
				// Write out the NJTL chunk if it exists
				if (Metadata.Count != 0 && Metadata.ContainsKey(uint.MaxValue))
				{
					file.InsertRange(0, Metadata[uint.MaxValue]);
				}
			}
			else
			{
				file.AddRange(ByteConverter.GetBytes((uint)ChunkTypes.End));
				file.AddRange(new byte[4]);
			}
			File.WriteAllBytes(filename, file.ToArray());
			ByteConverter.BigEndian = be;
		}

		public static void CreateFile(string filename, NJS_OBJECT model, string[] animationFiles, string author,
			string description, Dictionary<uint, byte[]> metadata, ModelFormat format, bool nometa = false, bool useNinjaMetaData = false, bool njbLittleEndian = false)
		{
			new ModelFile(format, model, Path.GetDirectoryName(Path.GetFullPath(filename)), animationFiles)
			{
				Author = author,
				Description = description,
				Metadata = metadata ?? new Dictionary<uint, byte[]>()
			}.SaveToFile(filename, nometa, useNinjaMetaData, njbLittleEndian);
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
			RightHandNode = 0x444E4852,
			LeftHandNode = 0x444E484C,
			RightFootNode = 0x444E4652,
			LeftFootNode = 0x444E464C,
			User0Node = 0x444E3055,
			User1Node = 0x444E3155,
			Weights = 0x54484757,
			End = 0x444E45
		}

		public enum NodeDirection
		{
			X,
			Y,
			Z
		}
	}
}