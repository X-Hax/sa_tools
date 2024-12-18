using FraGag.Compression;
using Newtonsoft.Json;
using SAModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SplitTools.SAArc
{
	public static class SA2MiniEvent
	{
		static List<string> nodenames = new();
		static Dictionary<string, ModelInfo> modelfiles = new();
		static Dictionary<string, MotionInfo> motionfiles = new();
		static Dictionary<string, CameraInfo> camarrayfiles = new();

		public static void Split(string filename, string outputPath, string labelFile = null)
		{
			nodenames.Clear();
			modelfiles.Clear();
			motionfiles.Clear();
			camarrayfiles.Clear();
			string dir = Environment.CurrentDirectory;
			try
			{
				if (outputPath[outputPath.Length - 1] != '/')
				{
					outputPath = string.Concat(outputPath, "/");
				}

				// get file name, read it from the console if nothing
				string evfilename = filename;
				evfilename = Path.GetFullPath(evfilename);

				Console.WriteLine("Splitting file {0}...", evfilename);
				byte[] fc;
				if (Path.GetExtension(evfilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				{
					fc = Prs.Decompress(evfilename);
				}
				else
				{
					fc = File.ReadAllBytes(evfilename);
				}

				MiniEventIniData ini = new MiniEventIniData() { Name = Path.GetFileNameWithoutExtension(evfilename) };
				if (outputPath.Length != 0)
				{
					if (!Directory.Exists(outputPath))
					{
						Directory.CreateDirectory(outputPath);
					}

					Environment.CurrentDirectory = outputPath;
				}
				else
				{
					Environment.CurrentDirectory = Path.GetDirectoryName(evfilename);
				}

				Directory.CreateDirectory(Path.GetFileNameWithoutExtension(evfilename));
				// Metadata for SAMDL Project Mode
				byte[] mlength = null;
				Dictionary<string, string> evsectionlist = new Dictionary<string, string>();
				Dictionary<string, string> evsplitfilenames = new Dictionary<string, string>();
				if (labelFile != null)
				{
					labelFile = Path.GetFullPath(labelFile);
				}

				if (File.Exists(labelFile))
				{
					evsplitfilenames = IniSerializer.Deserialize<Dictionary<string, string>>(labelFile);
					mlength = File.ReadAllBytes(labelFile);
				}
				string evname = Path.GetFileNameWithoutExtension(evfilename);
				string[] evmetadata = new string[0];

				uint key;
				List<NJS_MOTION> motions = null;
				List<NJS_CAMERA> ncams = null;
				if (fc[4] == 0x81)
				{
					Console.WriteLine("File is in GC/PC format.");
					ByteConverter.BigEndian = true;
					key = 0x816DFE60;
					ini.Game = Game.SA2B;
					ini.BigEndian = true;
				}
				else
				{
					Console.WriteLine("File is in DC format.");
					ByteConverter.BigEndian = false;
					key = 0xCB00000;
					ini.Game = Game.SA2;
					ini.BigEndian = false;
				}
				int address;
				for (int i = 0; i < 8; i++)
				{
					address = 8 + (4 * i);
					int ptr = fc.GetPointer(address, key);
					MiniEventMaster data = new MiniEventMaster();
					string chnm = null;
					string texname = null;
					switch (i)
					{
						case 0:
							chnm = "Sonic";
							texname = "SONICTEX";
							break;
						case 1:
							chnm = "Shadow";
							texname = "TERIOSTEX";
							break;
						case 2:
							chnm = "Tails";
							texname = "MILESTEX";
							break;
						case 3:
							chnm = "Eggman";
							texname = "EGGTEX";
							break;
						case 4:
							chnm = "Knuckles";
							texname = "KNUCKTEX";
							break;
						case 5:
							chnm = "Rouge";
							texname = "ROUGETEX";
							break;
						case 6:
							chnm = "Mech Tails";
							texname = "TWALKTEX";
							break;
						case 7:
							chnm = "Mech Eggman";
							texname = "EWALKTEX";
							break;
					}
					if (ptr != 0)
					{
						ini.MainDataAddrs.Add(i, $"evassets_{ptr:X8}");
						Console.WriteLine($"{chnm} is in this Mini-Event");
						Directory.CreateDirectory(Path.Combine(Path.GetFileNameWithoutExtension(evfilename), $"{chnm}"));
						switch (i)
						{
							case 0:
							case 1:
							case 2:
							case 4:
							case 5:
								data.Name = $"evassets_{ptr:X8}";
								data.Character = chnm;
								data.BodyAnims = GetMotion(fc, ptr, key, $"{chnm}\\Body.saanim", motions, 62, $"{evname} {chnm} Body Motions");
								break;
							case 3:
								data.Name = $"evassets_{ptr:X8}";
								data.Character = chnm;
								data.BodyAnims = GetMotion(fc, ptr, key, $"{chnm}\\Body.saanim", motions, 45, $"{evname} {chnm} Body Motions");
								break;
							case 6:
							case 7:
								data.Name = $"evassets_{ptr:X8}";
								data.Character = chnm;
								data.BodyAnims = GetMotion(fc, ptr, key, $"{chnm}\\Body.saanim", motions, 33, $"{evname} {chnm} Body Motions");
								break;
						}
						int address2;
						for (int j = 0; j < 4; j++)
						{
							string prnm = null;
							string partmetaname = null;
							switch (j)
							{
								case 0:
									prnm = "Head";
									partmetaname = prnm;
									break;
								case 1:
									prnm = "Mouth";
									partmetaname = prnm;
									break;
								case 2:
									prnm = "LeftHand";
									partmetaname = "Left Hand";
									break;
								case 3:
									prnm = "RightHand";
									partmetaname = "Right Hand";
									break;
							}
							address2 = ptr + 4 + (0xC * j);
							int ptr2 = fc.GetPointer(address2, key);
							if (ptr2 != 0)
							{
								MiniEventParts parts = new MiniEventParts();
								parts.Part = prnm;
								parts.Model = GetModel(fc, address2, key, $"{chnm}\\{prnm}.sa2mdl", $"{evname} {chnm} {partmetaname}");
								if (parts.Model != null)
								{
									parts.Anims = GetMotion(fc, address2 + 4, key, $"{chnm}\\{prnm}.saanim", motions, modelfiles[parts.Model].Model.CountAnimated(), $"{evname} {chnm} EV {partmetaname} Animation");
									if (parts.Anims != null)
									{
										modelfiles[parts.Model].Motions.Add($"{prnm}.saanim");
									}

									parts.ShapeMotions = GetMotion(fc, address2 + 8, key, $"{chnm}\\{prnm}Shape.saanim", motions, modelfiles[parts.Model].Model.CountMorph(), $"{evname} {chnm} EV {partmetaname} Shape Motion");
									if (parts.ShapeMotions != null)
									{
										modelfiles[parts.Model].Motions.Add($"{prnm}Shape.saanim");
									}

									// populating metadata file
									string outResult = null;
									// checks if the source ini is a placeholder
									if (labelFile != null && mlength.Length != 0)
									{
										evmetadata = evsplitfilenames[modelfiles[parts.Model].Filename].Split('|'); // Description|Texture file
										outResult += evmetadata[0] + "|" + evmetadata[1];
										evsectionlist[modelfiles[parts.Model].Filename] = outResult;
									}
									else
									{
										outResult += modelfiles[parts.Model].MetaName + "|" + $"{texname}";
										evsectionlist.Add(modelfiles[parts.Model].Filename, outResult);
									}
								}
								data.Parts.Add(parts);
							}
							else
							{
								data.Parts.Add(null);
							}
						}
						ini.MainData.Add(data);
					}
					else
					{
						ini.MainData.Add(null);
						ini.MainDataAddrs.Add(i, null);
					}
				}
				int cam = fc.GetPointer(4, key);
				int camaddr = ByteConverter.ToInt32(fc, 4);
				if (cam != 0)
				{
					ini.Camera = GetMotion(fc, 4, key, $"Camera.saanim", motions, 1, $"{evname} Camera Animation");
					ini.NinjaCamera = GetCamData(fc, 4, key, "CameraAttributes.ini", ncams);
				}
				else
				{
					Console.WriteLine("Mini-Event does not contain a camera.");
				}

				ini.CharacterFlags = (SA2CharacterFlags)ByteConverter.ToInt32(fc, 0);
				foreach (var item in motionfiles.Values)
				{
					string fn = item.Filename;
					string fp = Path.Combine(Path.GetFileNameWithoutExtension(evfilename), fn);
					item.Motion.Save(fp);
					ini.Files.Add(fn, HelperFunctions.FileHash(fp));
				}
				foreach (var item in modelfiles.Values)
				{
					string fp = Path.Combine(Path.GetFileNameWithoutExtension(evfilename), item.Filename);
					ModelFile.CreateFile(fp, item.Model, item.Motions.ToArray(), null, null, null, item.Format);
					ini.Files.Add(item.Filename, HelperFunctions.FileHash(fp));
				}
				foreach (var item in camarrayfiles.Values)
				{
					string fn = item.Filename;
					string fp = Path.Combine(Path.GetFileNameWithoutExtension(evfilename), fn);
					item.CamData.Save(fp);
					ini.Files.Add(fn, HelperFunctions.FileHash(fp));
				}
				JsonSerializer js = new JsonSerializer
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore
				};

				// Creates metadata ini file for SAMDL Project Mode
				if (labelFile != null)
				{
					string evsectionListFilename = Path.GetFileNameWithoutExtension(labelFile) + "_data.ini";
					IniSerializer.Serialize(evsectionlist, Path.Combine(outputPath, evsectionListFilename));
				}
				using (var tw = File.CreateText(Path.Combine(Path.GetFileNameWithoutExtension(evfilename), Path.ChangeExtension(Path.GetFileName(filename), ".json"))))
					js.Serialize(tw, ini);
			}
			finally
			{
				Environment.CurrentDirectory = dir;
			}
		}

		public static void Build(bool? isBigEndian, string filename, string fileOutputPath)
		{
			nodenames.Clear();
			modelfiles.Clear();
			motionfiles.Clear();
			camarrayfiles.Clear();

			string dir = Environment.CurrentDirectory;
			try
			{
				if (fileOutputPath[fileOutputPath.Length - 1] != '/')
				{
					fileOutputPath = string.Concat(fileOutputPath, "/");
				}

				filename = Path.GetFullPath(filename);
				if (Directory.Exists(filename))
				{
					filename += ".prs";
				}

				Environment.CurrentDirectory = Path.GetDirectoryName(filename);
				string path = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filename)), Path.GetFileNameWithoutExtension(filename));
				JsonSerializer js = new JsonSerializer();
				MiniEventIniData evinfo;
				using (TextReader tr = File.OpenText(Path.Combine(path, Path.ChangeExtension(Path.GetFileName(filename), ".json"))))
				using (JsonTextReader jtr = new JsonTextReader(tr))
				evinfo = js.Deserialize<MiniEventIniData>(jtr);
				uint gamekey;
				if (!isBigEndian.HasValue)
				{
					ByteConverter.BigEndian = evinfo.BigEndian;
				}
				else
				{
					ByteConverter.BigEndian = isBigEndian.Value;
				}

				List<byte> evfile = new List<byte>();
				List<byte> databytes = new List<byte>();
				Dictionary<string, int> animaddrs = new Dictionary<string, int>();
				Dictionary<int, int> banimaddrs = new Dictionary<int, int>();
				Dictionary<int, uint> mdladdrs = new Dictionary<int, uint>();
				Dictionary<int, int> panimaddrs = new Dictionary<int, int>();
				Dictionary<int, int> pshapeaddrs = new Dictionary<int, int>();
				Dictionary<string, uint> labels = new Dictionary<string, uint>();
				if (ByteConverter.BigEndian)
				{
					gamekey = 0x816DFE60;
				}
				else
				{
					gamekey = 0xCB00000;
				}

				uint imageBase = gamekey + 0x2C;
				// Character flags
				evfile.AddRange(ByteConverter.GetBytes(Convert.ToUInt32(evinfo.CharacterFlags)));
				for (int i = 0; i < 8; i++)
				{
					if (evinfo.MainDataAddrs[i] != null)
					{
						MiniEventMaster master = evinfo.MainData[i];
						if (master.BodyAnims != null)
						{
							for (int p = 0; p < 4; p++)
							{
								MiniEventParts part = master.Parts[p];
								// There has to be a better way to do this
								int constant = i * 10;
								if (part != null)
								{
									if (part.Anims != null)
									{
										List<byte> animbytes = new List<byte>();
										NJS_MOTION anim = NJS_MOTION.Load(Path.Combine(Path.Combine(path, $"{master.Character}"), part.Part + ".saanim"));
										animbytes.AddRange(anim.GetBytes(imageBase, out uint addranim));
										panimaddrs[constant + p] = (int)(addranim + imageBase);
										databytes.AddRange(animbytes);
										imageBase += (uint)animbytes.Count;
									}
									else
									{
										panimaddrs[constant + p] = 0;
									}
									if (part.ShapeMotions != null)
									{
										List<byte> shapebytes = new List<byte>();
										NJS_MOTION shape = NJS_MOTION.Load(Path.Combine(Path.Combine(path, $"{master.Character}"), part.Part + "Shape.saanim"));
										shape.OptimizeShape();
										shapebytes.AddRange(shape.GetBytes(imageBase, out uint addrshape));
										pshapeaddrs[constant + p] = (int)(addrshape + imageBase);
										databytes.AddRange(shapebytes);
										imageBase += (uint)shapebytes.Count;
									}
									else
									{
										pshapeaddrs[constant + p] = 0;
									}
									NJS_OBJECT partmdldata = new ModelFile(Path.Combine(Path.Combine(path, $"{master.Character}"), part.Part + ".sa2mdl")).Model;
									byte[] tmpmdl = partmdldata.GetBytes(imageBase, false, labels, new List<uint>(), out uint addrmdl);
									databytes.AddRange(tmpmdl);
									mdladdrs[constant + p] = labels[partmdldata.Name];
									imageBase += (uint)tmpmdl.Length;
								}
								else
								{
									mdladdrs[constant + p] = 0;
									panimaddrs[constant + p] = 0;
									pshapeaddrs[constant + p] = 0;
								}
							}
							List<byte> banimbytes = new List<byte>();
							NJS_MOTION banim = NJS_MOTION.Load(Path.Combine(Path.Combine(path, $"{master.Character}"), "Body.saanim"));
							banimbytes.AddRange(banim.GetBytes(imageBase, out uint addrbody));
							banimaddrs[i] = (int)(addrbody + imageBase);
							databytes.AddRange(banimbytes);
							imageBase += (uint)banimbytes.Count;
						}
					}
				}
				// Camera data always exists
				NJS_CAMERA camfile = NJS_CAMERA.Load(Path.Combine(path, "CameraAttributes.ini"));
				List<byte> ncambytes = new List<byte>();
				NinjaCamera ndata = camfile.NinjaCameraData;
				int ncamaddr = (int)imageBase;
				ncambytes.AddRange(ndata.GetBytes());
				databytes.AddRange(ncambytes);
				imageBase += (uint)ncambytes.Count;

				List<byte> canimbytes = new List<byte>();
				NJS_MOTION camanim = NJS_MOTION.Load(Path.Combine(path, "Camera.saanim"));
				canimbytes.AddRange(camanim.GetBytes(imageBase, out uint addrcam));
				animaddrs[camanim.Name] = (int)(addrcam + imageBase);
				databytes.AddRange(canimbytes);
				imageBase += (uint)canimbytes.Count;
				evfile.AddRange(ByteConverter.GetBytes(animaddrs[camanim.Name]));

				// Sets up NJS_CAMERA pointers
				databytes.AddRange(ByteConverter.GetBytes(ncamaddr));
				databytes.AddRange(ByteConverter.GetBytes(animaddrs[camanim.Name]));

				// Calculates pointers for main arrays
				imageBase += 8;
				for (int a = 0; a < 8; a++)
				{
					if (evinfo.MainDataAddrs[a] != null)
					{
						int masterptr = (int)imageBase;
						evfile.AddRange(ByteConverter.GetBytes(imageBase));
						imageBase += 0x34;
					}
					else
					{
						evfile.AddRange(new byte[4]);
					}
				}
				evfile.AddRange(new byte[4]);
				// Raw data goes here
				evfile.AddRange(databytes);
				// Calculations for master pointer array
				for (int d = 0; d < 8; d++)
				{
					MiniEventMaster data = evinfo.MainData[d];
					if (data != null)
					{
						if (data.BodyAnims != null)
						{
							evfile.AddRange(ByteConverter.GetBytes(banimaddrs[d]));
							for (int p = 0; p < 4; p++)
							{
								int constant = d * 10;
								MiniEventParts part = data.Parts[p];
								if (part != null)
								{
									evfile.AddRange(ByteConverter.GetBytes(mdladdrs[constant + p]));
									if (part.Anims != null)
									{
										evfile.AddRange(ByteConverter.GetBytes(panimaddrs[constant + p]));
									}
									else
									{
										evfile.AddRange(new byte[4]);
									}

									if (part.ShapeMotions != null)
									{
										evfile.AddRange(ByteConverter.GetBytes(pshapeaddrs[constant + p]));
									}
									else
									{
										evfile.AddRange(new byte[4]);
									}
								}
								else
								{
									evfile.AddRange(new byte[0xC]);
								}
							}
						}
					}
				}

				if (fileOutputPath.Length != 0)
				{
					if (!Directory.Exists(fileOutputPath))
					{
						Directory.CreateDirectory(fileOutputPath);
					}

					filename = Path.Combine(fileOutputPath, Path.GetFileName(filename));
				}

				if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				{
					FraGag.Compression.Prs.Compress(evfile.ToArray(), filename);
					if (!File.Exists(filename))
					{
						File.Create(filename);
					}
				}
				else
				{
					File.WriteAllBytes(filename, evfile.ToArray());
				}
			}
			finally
			{
				Environment.CurrentDirectory = dir;
			}
		}

		//Get Functions
		private static string GetModel(byte[] fc, int address, uint key, string fn, string meta = null)
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
						{
							modelfiles.Remove(s);
						}

					nodenames.AddRange(names);
					modelfiles.Add(obj.Name, new ModelInfo(fn, obj, ModelFormat.Chunk, meta));
				}
			}
			return name;
		}

		private static string GetMotion(byte[] fc, int address, uint key, string fn, List<NJS_MOTION> motions, int cnt, string meta = null)
		{
			NJS_MOTION mtn = null;
			if (motions != null)
			{
				mtn = motions[ByteConverter.ToInt32(fc, address)];
			}
			else
			{
				int ptr3 = fc.GetPointer(address, key);
				if (ptr3 != 0)
				{
					mtn = new NJS_MOTION(fc, ptr3, key, cnt);
					mtn.Description = meta;
					mtn.OptimizeShape();
				}
			}
			if (mtn == null)
			{
				return null;
			}

			if (!motionfiles.ContainsKey(mtn.Name) || motionfiles[mtn.Name].Filename == null)
			{
				motionfiles[mtn.Name] = new MotionInfo(fn, mtn, meta);
			}

			return mtn.Name;
		}

		private static string GetCamData(byte[] fc, int address, uint key, string fn, List<NJS_CAMERA> ncams)
		{
			NJS_CAMERA ncam = null;
			if (ncams != null)
			{
				ncam = ncams[ByteConverter.ToInt32(fc, address)];
			}
			else
			{
				int ptr3 = fc.GetPointer(address, key);
				if (ptr3 != 0)
				{
					ncam = new NJS_CAMERA(fc, ptr3 + 0xC, key);
				}
			}
			if (ncam == null)
			{
				return null;
			}

			if (!camarrayfiles.ContainsKey(ncam.Name) || camarrayfiles[ncam.Name].Filename == null)
			{
				camarrayfiles[ncam.Name] = new CameraInfo(fn, ncam);
			}

			return ncam.Name;
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
		public bool BigEndian { get; set; }
		public Dictionary<string, string> Files { get; set; } = new();
		[JsonIgnore]
		public SA2CharacterFlags CharacterFlags { get; set; }
		[JsonProperty(PropertyName = "CharacterFlags")]
		public string CharFlagString
		{
			get { return CharacterFlags.ToString(); }
			set { CharacterFlags = (SA2CharacterFlags)Enum.Parse(typeof(SA2CharacterFlags), value); }
		}
		public string Camera { get; set; }
		public string NinjaCamera { get; set; }
		public Dictionary<int, string> MainDataAddrs { get; set; } = new();
		public List<MiniEventMaster> MainData { get; set; } = new();
	}

	public class MiniEventMaster
	{
		public string Name { get; set; }
		public string Character { get; set; }
		public string BodyAnims { get; set; }
		public List<MiniEventParts> Parts { get; set; } = new();
	}

	public class MiniEventParts
	{
		public string Part { get; set; }
		public string Model { get; set; }
		public string Anims { get; set; }
		public string ShapeMotions { get; set; }
	}
}