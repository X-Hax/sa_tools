using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using SonicRetro.SAModel;

namespace SA_Tools.Split
{
	public static class SplitNB
	{

		[DllImport("shlwapi.dll", SetLastError = true)]
		private static extern bool PathRelativePathTo(System.Text.StringBuilder pszPath,
string pszFrom, int dwAttrFrom, string pszTo, int dwAttrTo);

		private enum AnimSections
		{
			MKEY_F = 0,
			MKEY_A = 1,
			UNKNOWN = 2,
			MDATA2_HEADER = 3,
			MOTION = 4,
		}
		private enum ModelSections
		{
			POLY = 0,
			VERTEX = 1,
			NORMAL = 2,
			UV = 3,
			MATERIAL = 4,
			MESHSET = 5,
			MODEL = 6,
			OBJECT = 7,
		}
		public static void BuildNBFile(string filename, string dest, int verbose = 0)
		{
			//Needs update to use filenames from an INI file
			Dictionary<int, string> sectionlist = IniSerializer.Deserialize<Dictionary<int, string>>(filename);
			List<byte> result = new List<byte>();
			result.AddRange(BitConverter.GetBytes(0x04424A4E));
			int numfiles = sectionlist.Count;
			result.AddRange(BitConverter.GetBytes(numfiles));
			foreach (var item in sectionlist)
			{
				string insidepath = Path.Combine(Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(filename), item.Key.ToString("D2"));
				byte[] writeout;
				//Convert models and motions to BIN
				if (item.Value != "NULL")
				{
					switch (Path.GetExtension(item.Value).ToLowerInvariant())
					{
						case ".sa1mdl":
							ModelFile mdlfile = new ModelFile(insidepath + ".sa1mdl");
							writeout = GetSections(mdlfile.Model);
							result.AddRange(BitConverter.GetBytes((ushort)1));
							result.AddRange(BitConverter.GetBytes((ushort)0xCDCD));
							result.AddRange(BitConverter.GetBytes((uint)writeout.Length));
							result.AddRange(writeout);
							File.WriteAllBytes(insidepath + ".bin", writeout);
							break;
						case ".saanim":
							NJS_MOTION mot = NJS_MOTION.Load(insidepath + ".saanim");
							if (verbose > 1) Console.WriteLine("Section {0}", item.Key);
							writeout = GetSections(mot, verbose);
							result.AddRange(BitConverter.GetBytes((ushort)3));
							result.AddRange(BitConverter.GetBytes((ushort)0xCDCD));
							result.AddRange(BitConverter.GetBytes((uint)writeout.Length));
							result.AddRange(writeout);
							File.WriteAllBytes(insidepath + ".bin", writeout);
							break;
					}
				}
				else
				{
					result.AddRange(BitConverter.GetBytes((ushort)0));
					result.AddRange(BitConverter.GetBytes((ushort)0xCDCD));
					result.AddRange(BitConverter.GetBytes((uint)0));
				}
				//Build the NB file
				File.WriteAllBytes(dest, result.ToArray());
			}
		}
		public static void SplitNBFile(string filename, bool extractchunks, string outdir, int verbose = 0, string inifilename = null)
		{
			Dictionary<int, string> sectionlist = new Dictionary<int, string>();
			Dictionary<int, NJS_OBJECT> modellist = new Dictionary<int, NJS_OBJECT>();
			Dictionary<int, NJS_MOTION> animlist = new Dictionary<int, NJS_MOTION>();
			byte[] file = File.ReadAllBytes(filename);
			if (file.Length == 180920) //Patch in unused rotation for E101R, required for rebuilding with correct pointers
			{
				file[129896] = 0xE0;
				file[129897] = 0x0A;
				file[129904] = 0x02;
			}
			if (BitConverter.ToInt32(file, 0) != 0x04424A4E)
			{
				Console.WriteLine("Invalid NB file.");
				return;
			}
			if (!Directory.Exists(outdir))
				Directory.CreateDirectory(outdir);
			Environment.CurrentDirectory = outdir;
			int numfiles = BitConverter.ToInt16(file, 4);
			Dictionary<int, string> splitfilenames = new Dictionary<int, string>();
			inifilename = Path.GetFullPath(inifilename);
			if (File.Exists(inifilename))
			{
				splitfilenames = IniSerializer.Deserialize<Dictionary<int, string>>(inifilename);
				if (verbose > 0)
					Console.WriteLine("Split INI: {0}", inifilename);
			}
			else
			{
				if (verbose > 0)
					Console.WriteLine("Split INI {0} not found!", inifilename);
				for (int i = 0; i < numfiles; i++)
				{
					splitfilenames[i] = i.ToString("D2");
				}
			}
			int curaddr = 8;
			for (int i = 0; i < numfiles; i++)
			{
				ushort type = BitConverter.ToUInt16(file, curaddr);
				byte[] chunk = new byte[BitConverter.ToInt32(file, curaddr + 4)];
				Array.Copy(file, curaddr + 8, chunk, 0, chunk.Length);
				switch (type)
				{
					case 0:
						if (verbose > 0) Console.Write("\nSection {0} at {1} is empty\n", i.ToString("D2", NumberFormatInfo.InvariantInfo), curaddr.ToString("X"));
						sectionlist.Add(i, "NULL");
						break;
					case 1:
						if (verbose > 0) Console.WriteLine("\nSection {0} at {1} is a model", i.ToString("D2", NumberFormatInfo.InvariantInfo), curaddr.ToString("X"));
						if (extractchunks) File.WriteAllBytes(i.ToString("D2", NumberFormatInfo.InvariantInfo) + ".bin", chunk);
						NJS_OBJECT mdl = ProcessModel(chunk, verbose, curaddr + 8);
						//if (extractchunks) File.WriteAllBytes(i.ToString("D2", NumberFormatInfo.InvariantInfo) + "_p.bin", GetSections(mdl));
						modellist.Add(i, mdl);
						sectionlist.Add(i, splitfilenames[i] + ".sa1mdl");
						break;
					case 3:
						if (verbose > 0) Console.WriteLine("\nSection {0} at {1} is a motion", i.ToString("D2", NumberFormatInfo.InvariantInfo), curaddr.ToString("X"));
						if (extractchunks) File.WriteAllBytes(i.ToString("D2", NumberFormatInfo.InvariantInfo) + ".bin", chunk);
						NJS_MOTION mot = ProcessMotion(chunk, verbose, curaddr + 8);
						//if (extractchunks) File.WriteAllBytes(i.ToString("D2", NumberFormatInfo.InvariantInfo) + "_p.bin", GetSections(mot));
						animlist.Add(i, mot);
						sectionlist.Add(i, splitfilenames[i] + ".saanim");
						break;
					default:
						if (verbose > 0) Console.WriteLine("\nSection {0} at {1} is an unknown type", i.ToString("D2", NumberFormatInfo.InvariantInfo), curaddr.ToString("X"));
						if (extractchunks) File.WriteAllBytes(i.ToString("D2", NumberFormatInfo.InvariantInfo) + ".bin", chunk);
						sectionlist.Add(i, splitfilenames[i] + ".wtf");
						break;
				}
				curaddr += chunk.Length + 8;
			}
			//Save models and animations
			foreach (var modelitem in modellist)
			{
				List<string> anims = new List<string>();
				foreach (var animitem in animlist)
				{
					if (modelitem.Value.CountAnimated() == animitem.Value.ModelParts)
					{
						if (!Directory.Exists(Path.GetDirectoryName(splitfilenames[animitem.Key] + ".saanim")) && Path.GetDirectoryName(splitfilenames[animitem.Key] + ".saanim") != "")
							Directory.CreateDirectory(Path.GetDirectoryName(splitfilenames[animitem.Key] + ".saanim"));
						animitem.Value.Save(splitfilenames[animitem.Key] + ".saanim");
						System.Text.StringBuilder sb = new System.Text.StringBuilder(1024);
						PathRelativePathTo(sb, Path.GetFullPath(Path.Combine(outdir, splitfilenames[animitem.Key] + ".saanim")), 0, Path.GetFullPath(splitfilenames[animitem.Key] + ".saanim"), 0);
						anims.Add(sb.ToString());
					}
				}
				if (!Directory.Exists(Path.GetDirectoryName(splitfilenames[modelitem.Key] + ".sa1mdl")) && Path.GetDirectoryName(splitfilenames[modelitem.Key] + ".sa1mdl") != "")
					Directory.CreateDirectory(Path.GetDirectoryName(splitfilenames[modelitem.Key] + ".sa1mdl"));
				ModelFile.CreateFile(splitfilenames[modelitem.Key] + ".sa1mdl", modelitem.Value, anims.ToArray(), null, null, null, ModelFormat.Basic);
			}
			IniSerializer.Serialize(sectionlist, Path.Combine(outdir, Path.GetFileNameWithoutExtension(filename) + ".ini"));
		}
		static byte[] GetSections(NJS_OBJECT mdl)
		{
			List<byte> result = new List<byte>();
			NJS_OBJECT[] objs = mdl.GetObjects();
			int[] modeladdr = new int[objs.Length];
			int[] objaddr = new int[objs.Length];
			int[] mataddr = new int[objs.Length];
			int[] meshaddr = new int[objs.Length];
			List<int>[] polyaddr = new List<int>[objs.Length];
			List<int>[] uvaddr = new List<int>[objs.Length];
			int[] vertaddr = new int[objs.Length];
			int[] normaddr = new int[objs.Length];
			int currentaddr = 0;
			//Write all polys
			int polysection = currentaddr;
			currentaddr += 8;
			short polycount = 0;
			for (int m = objs.Length - 1; m > -1; m--)
			{
				polyaddr[m] = new List<int>();
				if (objs[m].Attach != null)
				{
					BasicAttach att = (BasicAttach)objs[m].Attach;
					if (att.Mesh != null)
					{
						foreach (NJS_MESHSET mesh in att.Mesh)
						{
							polyaddr[m].Add(result.Count);
							//Console.WriteLine("Polyaddr in model {0} : {1}", m, currentaddr.ToString("X"));
							foreach (Strip poly in mesh.Poly)
							{
								if (poly.Reversed)
									result.AddRange(BitConverter.GetBytes((short)(0x8000 | (poly.Indexes.Length & 0x7FFF))));
								else
									result.AddRange(BitConverter.GetBytes((short)(poly.Indexes.Length & 0x7FFF)));
								polycount++;
								for (int ind = 0; ind < poly.Indexes.Length; ind++)
								{
									result.AddRange(BitConverter.GetBytes(poly.Indexes[ind]));
									polycount++;
								}
							}
							currentaddr = result.Count;
						}
					}
				}
			}
			if ((polycount * 2) % 4 != 0)
			{
				int res = polycount * 2;
				do
				{
					result.Add(0xCD);
					res++;
				}
				while (res % 4 != 0);
			}
			currentaddr = result.Count;
			result.InsertRange(polysection, BitConverter.GetBytes((uint)0));
			result.InsertRange(polysection + 4, BitConverter.GetBytes((uint)polycount * 2));
			//Write all vertices
			int vertsection = currentaddr;
			currentaddr += 8;
			short vertexcount = 0;
			for (int m = objs.Length - 1; m > -1; m--)
			{
				if (objs[m].Attach != null)
				{
					BasicAttach att = (BasicAttach)objs[m].Attach;
					vertaddr[m] = currentaddr;
					if (att.Vertex != null)
					{
						for (int v = 0; v < att.Vertex.Length; v++)
						{
							result.AddRange(BitConverter.GetBytes(att.Vertex[v].X));
							result.AddRange(BitConverter.GetBytes(att.Vertex[v].Y));
							result.AddRange(BitConverter.GetBytes(att.Vertex[v].Z));
							vertexcount++;
						}
					}
					currentaddr = result.Count;
				}
			}
			result.InsertRange(vertsection + 8, BitConverter.GetBytes((uint)1));
			result.InsertRange(vertsection + 4 + 8, BitConverter.GetBytes((uint)vertexcount * 12));
			//Write all normals
			int normsection = currentaddr;
			currentaddr += 8;
			short normalcount = 0;
			for (int m = objs.Length - 1; m > -1; m--)
			{
				if (objs[m].Attach != null)
				{
					BasicAttach att = (BasicAttach)objs[m].Attach;
					normaddr[m] = currentaddr;
					if (att.Vertex != null)
					{
						for (int v = 0; v < att.Vertex.Length; v++)
						{
							result.AddRange(BitConverter.GetBytes(att.Normal[v].X));
							result.AddRange(BitConverter.GetBytes(att.Normal[v].Y));
							result.AddRange(BitConverter.GetBytes(att.Normal[v].Z));
							normalcount++;
						}
					}
					currentaddr = result.Count;
				}
			}
			result.InsertRange(normsection + 8, BitConverter.GetBytes((uint)2));
			result.InsertRange(normsection + 4 + 8, BitConverter.GetBytes((uint)normalcount * 12));
			//Write all UVs
			int uvsection = currentaddr;
			currentaddr += 8;
			short uvcount = 0;
			for (int m = objs.Length - 1; m > -1; m--)
			{
				uvaddr[m] = new List<int>();
				if (objs[m].Attach != null)
				{
					BasicAttach att = (BasicAttach)objs[m].Attach;
					foreach (NJS_MESHSET mesh in att.Mesh)
					{
						uvaddr[m].Add(currentaddr);
						if (mesh.UV != null)
						{
							for (int uv = 0; uv < mesh.UV.Length; uv++)
							{
								result.AddRange(BitConverter.GetBytes((short)(mesh.UV[uv].U * 255f)));
								result.AddRange(BitConverter.GetBytes((short)(mesh.UV[uv].V * 255f)));
								uvcount++;
							}
						}
						currentaddr = result.Count;
					}
				}
			}
			result.InsertRange(uvsection + 8, BitConverter.GetBytes((uint)3));
			result.InsertRange(uvsection + 4 + 8, BitConverter.GetBytes((uint)uvcount * 4));
			//Write materials
			int matsection = currentaddr;
			currentaddr += 8;
			short matcount = 0;
			for (int m = objs.Length - 1; m > -1; m--)
			{
				if (objs[m].Attach != null)
				{
					BasicAttach att = (BasicAttach)objs[m].Attach;
					if (att.Material != null && att.Material.Count > 0)
					{
						mataddr[m] = result.Count;
						foreach (NJS_MATERIAL mat in att.Material)
						{
							result.AddRange(BitConverter.GetBytes(mat.DiffuseColor.ToArgb()));
							result.AddRange(BitConverter.GetBytes(mat.SpecularColor.ToArgb()));
							result.AddRange(BitConverter.GetBytes(mat.Exponent));
							result.AddRange(BitConverter.GetBytes(mat.TextureID));
							result.AddRange(BitConverter.GetBytes(mat.Flags));
							matcount++;
							currentaddr = result.Count;
						}
					}
				}
			}
			result.InsertRange(matsection + 8, BitConverter.GetBytes((short)4));
			result.InsertRange(matsection + 2 + 8, BitConverter.GetBytes(matcount));
			result.InsertRange(matsection + 4 + 8, BitConverter.GetBytes((uint)matcount * 20));
			//Write meshsets
			short meshcount = 0;
			int meshsection = currentaddr;
			currentaddr += 8;
			for (int m = objs.Length - 1; m > -1; m--)
			{
				if (objs[m].Attach != null)
				{
					BasicAttach att = (BasicAttach)objs[m].Attach;
					if (att.Material != null && att.Material.Count > 0)
					{
						int[] polys = polyaddr[m].ToArray();
						int[] uvs = uvaddr[m].ToArray();
						meshaddr[m] = result.Count;
						NJS_MESHSET[] meshes = att.Mesh.ToArray();
						for (int mesh = 0; mesh < meshes.Length; mesh++)
						{
							result.AddRange(BitConverter.GetBytes((ushort)((meshes[mesh].MaterialID & 0x3FFF) | ((int)meshes[mesh].PolyType << 0xE))));
							result.AddRange(BitConverter.GetBytes((ushort)meshes[mesh].Poly.Count));
							result.AddRange(BitConverter.GetBytes((uint)polys[mesh] + 8));
							result.AddRange(BitConverter.GetBytes((uint)0)); //polyattr
							result.AddRange(BitConverter.GetBytes((uint)0)); //polynormal pointer
							result.AddRange(BitConverter.GetBytes((uint)0)); //vcolor pointer
							if (meshes[mesh].UV != null && meshes[mesh].UV.Length > 0) result.AddRange(BitConverter.GetBytes((uint)uvs[mesh] + 8));
							else result.AddRange(BitConverter.GetBytes((uint)0));
							meshcount++;
						}
					}
					currentaddr = result.Count;
				}
			}
			result.InsertRange(meshsection + 8, BitConverter.GetBytes((ushort)5));
			result.InsertRange(meshsection + 2 + 8, BitConverter.GetBytes(meshcount));
			result.InsertRange(meshsection + 4 + 8, BitConverter.GetBytes((uint)meshcount * 24));
			//Write models
			short modelcount = 0;
			int modelsection = currentaddr;
			currentaddr += 8;
			for (int m = objs.Length - 1; m > -1; m--)
			{
				if (objs[m].Attach != null)
				{
					modeladdr[m] = currentaddr;
					BasicAttach att = (BasicAttach)objs[m].Attach;
					result.AddRange(BitConverter.GetBytes((uint)vertaddr[m] + 8));
					result.AddRange(BitConverter.GetBytes((uint)normaddr[m] + 8));
					result.AddRange(BitConverter.GetBytes((uint)att.Vertex.Length));
					result.AddRange(BitConverter.GetBytes((uint)meshaddr[m] + 8));
					result.AddRange(BitConverter.GetBytes((uint)mataddr[m] + 8));
					result.AddRange(BitConverter.GetBytes((ushort)att.Mesh.Count));
					result.AddRange(BitConverter.GetBytes((ushort)att.Material.Count));
					result.AddRange(BitConverter.GetBytes(att.Bounds.Center.X));
					result.AddRange(BitConverter.GetBytes(att.Bounds.Center.Y));
					result.AddRange(BitConverter.GetBytes(att.Bounds.Center.Z));
					result.AddRange(BitConverter.GetBytes(att.Bounds.Radius));
					modelcount++;
					currentaddr = result.Count;
				}
			}
			result.InsertRange(modelsection + 8, BitConverter.GetBytes((ushort)6));
			result.InsertRange(modelsection + 2 + 8, BitConverter.GetBytes((ushort)modelcount));
			result.InsertRange(modelsection + 4 + 8, BitConverter.GetBytes((uint)(40 * modelcount)));
			//Write objects
			int objectsection = currentaddr;
			currentaddr += 8;
			for (int m = objs.Length - 1; m > -1; m--)
			{
				objaddr[m] = currentaddr;
				result.AddRange(BitConverter.GetBytes((int)objs[m].GetFlags()));
				if (objs[m].Attach != null)
					result.AddRange(BitConverter.GetBytes((uint)modeladdr[m] + 8));
				else
					result.AddRange(BitConverter.GetBytes((uint)0));
				result.AddRange(BitConverter.GetBytes(objs[m].Position.X));
				result.AddRange(BitConverter.GetBytes(objs[m].Position.Y));
				result.AddRange(BitConverter.GetBytes(objs[m].Position.Z));
				result.AddRange(BitConverter.GetBytes(objs[m].Rotation.X));
				result.AddRange(BitConverter.GetBytes(objs[m].Rotation.Y));
				result.AddRange(BitConverter.GetBytes(objs[m].Rotation.Z));
				result.AddRange(BitConverter.GetBytes(objs[m].Scale.X));
				result.AddRange(BitConverter.GetBytes(objs[m].Scale.Y));
				result.AddRange(BitConverter.GetBytes(objs[m].Scale.Z));
				if (objs[m].Children != null && objs[m].Children.Count > 0)
					result.AddRange(BitConverter.GetBytes((uint)(objaddr[FindModelIndex(objs[m].Children[0], objs)]) + 8)); //child
				else
					result.AddRange(BitConverter.GetBytes((uint)0));
				if (objs[m].Sibling != null)
					result.AddRange(BitConverter.GetBytes((uint)(objaddr[FindModelIndex(objs[m].Sibling, objs)]) + 8)); //sibling
				else
					result.AddRange(BitConverter.GetBytes((uint)0));
				currentaddr = result.Count;
			}
			result.InsertRange(objectsection + 8, BitConverter.GetBytes((ushort)7));
			result.InsertRange(objectsection + 2 + 8, BitConverter.GetBytes((ushort)objs.Length));
			result.InsertRange(objectsection + 4 + 8, BitConverter.GetBytes((uint)objs.Length * 52));
			return result.ToArray();
		}
		static byte[] GetSections(NJS_MOTION motion, int verbose = 0)
		{
			List<byte> result = new List<byte>();
			int[] mkey_f_addr = new int[motion.ModelParts];
			int[] mkey_a_addr = new int[motion.ModelParts];
			int mkey_f_section = 0;
			int mkey_f_count = 0;
			//Get MKEY_F
			foreach (var anim in motion.Models)
			{
				AnimModelData model = anim.Value;
				if (model.Position != null && model.Position.Count > 0)
				{
					mkey_f_addr[anim.Key] = result.Count;
					if (verbose > 1) Console.WriteLine("MKEY_F: {0} at {1}", anim.Key, result.Count.ToString("X"));
					foreach (var item in model.Position)
					{
						result.AddRange(BitConverter.GetBytes(item.Key));
						result.AddRange(BitConverter.GetBytes(item.Value.X));
						result.AddRange(BitConverter.GetBytes(item.Value.Y));
						result.AddRange(BitConverter.GetBytes(item.Value.Z));
						mkey_f_count++;
					}
				}
			}
			if (mkey_f_count > 0)
			{
				result.InsertRange(mkey_f_section, BitConverter.GetBytes((uint)0));
				result.InsertRange(mkey_f_section + 4, BitConverter.GetBytes((uint)mkey_f_count * 16));
			}
			//Get MKEY_A
			int mkey_a_section = result.Count;
			int mkey_a_count = 0;
			foreach (var anim in motion.Models)
			{
				AnimModelData model = anim.Value;
				if (model.Rotation != null && model.Rotation.Count > 0)
				{
					mkey_a_addr[anim.Key] = result.Count;
					if (verbose > 1) Console.WriteLine("MKEY_A: {0} at {1}", anim.Key, result.Count.ToString("X"));
					foreach (var item in model.Rotation)
					{
						result.AddRange(BitConverter.GetBytes(item.Key));
						result.AddRange(BitConverter.GetBytes(item.Value.X));
						result.AddRange(BitConverter.GetBytes(item.Value.Y));
						result.AddRange(BitConverter.GetBytes(item.Value.Z));
						mkey_a_count++;
					}
				}
			}
			if (mkey_a_count > 0)
			{
				result.InsertRange(mkey_a_section, BitConverter.GetBytes((uint)1));
				result.InsertRange(mkey_a_section + 4, BitConverter.GetBytes((uint)mkey_a_count * 16));
			}
			//Get MDATA2 header
			int mdata_section = result.Count;
			for (int m = 0; m < motion.ModelParts; m++)
			{
				if (motion.Models.ContainsKey(m))
				{
					AnimModelData model = motion.Models[m];
					if (model.Position != null && model.Position.Count > 0)
						result.AddRange(BitConverter.GetBytes((uint)(mkey_f_addr[m] + 8)));
					else
						result.AddRange(BitConverter.GetBytes((uint)0));
					if (model.Rotation != null && model.Rotation.Count > 0)
						result.AddRange(BitConverter.GetBytes((uint)(mkey_a_addr[m] + 8)));
					else
						result.AddRange(BitConverter.GetBytes((uint)0));
					if (model.Position != null)
						result.AddRange(BitConverter.GetBytes((uint)(model.Position.Count)));
					else
						result.AddRange(BitConverter.GetBytes((uint)0));
					if (model.Rotation != null)
						result.AddRange(BitConverter.GetBytes((uint)(model.Rotation.Count)));
					else
						result.AddRange(BitConverter.GetBytes((uint)0));
				}
				else
				{
					result.AddRange(BitConverter.GetBytes((uint)0));
					result.AddRange(BitConverter.GetBytes((uint)0));
					result.AddRange(BitConverter.GetBytes((uint)0));
					result.AddRange(BitConverter.GetBytes((uint)0));
				}
			}
			result.InsertRange(mdata_section, BitConverter.GetBytes((uint)3));
			result.InsertRange(mdata_section + 4, BitConverter.GetBytes((uint)(16 * motion.ModelParts + 8)));
			result.InsertRange(mdata_section + 8, BitConverter.GetBytes((uint)motion.ModelParts << 2));
			result.InsertRange(mdata_section + 12, BitConverter.GetBytes((uint)2));
			//Write motion header
			result.AddRange(BitConverter.GetBytes((uint)4));
			result.AddRange(BitConverter.GetBytes((uint)12));
			result.AddRange(BitConverter.GetBytes(mdata_section + 8));
			result.AddRange(BitConverter.GetBytes(motion.Frames));
			//It's MDATA2 so flags are hardcoded
			AnimFlags flags = 0;
			flags |= AnimFlags.Position;
			flags |= AnimFlags.Rotation;
			result.AddRange(BitConverter.GetBytes((short)flags));
			result.AddRange(BitConverter.GetBytes((short)motion.InterpolationMode));
			return result.ToArray();
		}
		static NJS_MOTION ProcessMotion(byte[] file, int verbose = 0, int startaddress = 0)
		{
			int frames;
			InterpolationMode intmode;
			AnimFlags animtype;
			int curaddr = 0;
			do
			{
				AnimSections section_type = (AnimSections)BitConverter.ToInt16(file, curaddr);
				int section_size = BitConverter.ToInt32(file, curaddr + 4);
				int section_addr = curaddr + 8;
				if (verbose > 1) Console.Write("Subsection type {0}, size {1}, data begins at {2}\n", section_type.ToString(), section_size, section_addr.ToString("X"));
				switch (section_type)
				{
					case AnimSections.MKEY_F:
						if (verbose > 1) Console.Write(", calculated item count: {0}\n", (float)section_size / 16.0f);
						break;
					case AnimSections.MKEY_A:
						if (verbose > 1) Console.Write(", calculated item count: {0}\n", (float)section_size / 16.0f);
						break;
					case AnimSections.UNKNOWN:
						if (verbose > 1) Console.Write(", calculated item count: {0}\n", (float)section_size / 16.0f);
						break;
					case AnimSections.MDATA2_HEADER:
						if (verbose > 1) Console.Write(", number of MDATA entries: {0}\n", BitConverter.ToInt32(file, section_addr) >> BitConverter.ToInt32(file, section_addr + 4));
						break;
					case AnimSections.MOTION:
						if (verbose > 0)
							Console.WriteLine("Motion at {0}", (startaddress + section_addr).ToString("X"));
						frames = BitConverter.ToInt32(file, section_addr + 4);
						intmode = (InterpolationMode)BitConverter.ToInt16(file, section_addr + 10);
						animtype = (AnimFlags)BitConverter.ToInt16(file, section_addr + 8);
						int mdataaddr = BitConverter.ToInt32(file, section_addr);
						if (verbose > 1) Console.Write("\nMDATA header at: {0}, frames: {1}, flags: {2}, interpolation: {3}", mdataaddr.ToString("X"), frames, animtype.ToString(), intmode.ToString());
						//Create motion stub
						NJS_MOTION mot = new NJS_MOTION();
						mot.Name = "animation_" + section_addr.ToString("X8");
						mot.MdataName = mot.Name + "_mdat";
						mot.Frames = frames;
						mot.InterpolationMode = intmode;
						//Read the MDATA header and get the number of MDATA entries
						mot.ModelParts = BitConverter.ToInt32(file, mdataaddr) >> BitConverter.ToInt32(file, mdataaddr + 4);
						if (verbose > 1) Console.WriteLine(", model parts: {0}", mot.ModelParts);
						int tmpaddr = mdataaddr + 8; //Start of actual MDATA array
						for (int u = 0; u < mot.ModelParts; u++)
						{
							//Console.Write("\nMotion data {0}", u.ToString());
							AnimModelData data = new AnimModelData();
							if (animtype.HasFlag(AnimFlags.Position))
							{
								uint posoff = ByteConverter.ToUInt32(file, tmpaddr + u * 16);
								data.PositionName = mot.Name + "_mkey_" + u.ToString() + "_pos_" + posoff.ToString("X8");
								int pos_count = ByteConverter.ToInt32(file, tmpaddr + u * 16 + 8);
								//Console.Write(", position at: {0} ({1} entries)", posoff.ToString("X"), pos_count);
								for (int p = 0; p < pos_count; p++)
								{
									int index = ByteConverter.ToInt32(file, (int)posoff + p * 16);
									float pos_x = ByteConverter.ToSingle(file, (int)posoff + p * 16 + 4);
									float pos_y = ByteConverter.ToSingle(file, (int)posoff + p * 16 + 8);
									float pos_z = ByteConverter.ToSingle(file, (int)posoff + p * 16 + 12);
									//Console.WriteLine("\nAdded position index {3}: X: {0} Y: {1} Z: {2}", pos_x, pos_y, pos_z, index);
									data.Position.Add(index, new Vertex(pos_x, pos_y, pos_z));
								}
							}
							if (animtype.HasFlag(AnimFlags.Rotation))
							{
								uint rotoff = ByteConverter.ToUInt32(file, tmpaddr + u * 16 + 4);
								data.RotationName = mot.Name + "_mkey_" + u.ToString() + "_rot_" + rotoff.ToString("X8");
								int rot_count = ByteConverter.ToInt32(file, tmpaddr + u * 16 + 12);
								//Console.Write(", rotation at: {0} ({1} entries)", rotoff.ToString("X"), rot_count);
								for (int p = 0; p < rot_count; p++)
								{
									int index = ByteConverter.ToInt32(file, (int)rotoff + p * 16);
									int rot_x = ByteConverter.ToInt32(file, (int)rotoff + p * 16 + 4);
									int rot_y = ByteConverter.ToInt32(file, (int)rotoff + p * 16 + 8);
									int rot_z = ByteConverter.ToInt32(file, (int)rotoff + p * 16 + 12);
									//Console.WriteLine("\nAdded rotation index {3}: X: {0} Y: {1} Z: {2}", rot_x, rot_y, rot_z, index);
									data.Rotation.Add(index, new Rotation(rot_x, rot_y, rot_z));
								}
							}
							mot.Models.Add(u, data);
						}
						return mot;
				}
				curaddr += 8 + section_size;
			}
			while (curaddr < file.Length);
			return null;
		}
		static NJS_OBJECT ProcessModel(byte[] file, int verbose = 0, int startaddress = 0)
		{
			int curaddr = 0;
			do
			{
				int temp = BitConverter.ToInt16(file, curaddr);
				if (temp < 0 || temp > 7)
				{
					if (verbose > 1) Console.WriteLine("Skipping two padding bytes at {0}", curaddr.ToString("X"));
					curaddr += 2;
				}
				ModelSections section_type = (ModelSections)BitConverter.ToInt16(file, curaddr);
				int itemnum = BitConverter.ToInt16(file, curaddr + 2);
				int section_size = BitConverter.ToInt32(file, curaddr + 4);
				int section_addr = curaddr + 8;
				if (verbose > 1) Console.Write("Subsection type {0}, size {1}, data begins at {2}", section_type.ToString(), section_size, section_addr.ToString("X"));
				switch (section_type)
				{
					case ModelSections.POLY:
						if (verbose > 1) Console.Write(", calculated item count: {0}\n", (float)section_size / 2.0f);
						break;
					case ModelSections.VERTEX:
						if (verbose > 1) Console.Write(", calculated item count: {0}\n", (float)section_size / 12.0f);
						break;
					case ModelSections.NORMAL:
						if (verbose > 1) Console.Write(", calculated item count: {0}\n", (float)section_size / 12.0f);
						break;
					case ModelSections.UV:
						if (verbose > 1) Console.Write(", calculated item count: {0}\n", (float)section_size / 4.0f);
						break;
					case ModelSections.MATERIAL:
						if (verbose > 1) Console.Write(", item count: {0} (calculated {1})\n", itemnum, (float)section_size / 20.0f);
						break;
					case ModelSections.MESHSET:
						if (verbose > 1) Console.Write(", item count: {0} (calculated {1})\n", itemnum, (float)section_size / 24.0f);
						break;
					case ModelSections.MODEL:
						if (verbose > 1) Console.Write(", item count: {0} (calculated {1})\n", itemnum, (float)section_size / 40.0f);
						break;
					case ModelSections.OBJECT:
						if (verbose > 1) Console.Write(", item count: {0} (calculated {1})\n", itemnum, (float)section_size / 52.0f);
						break;
				}
				curaddr += 8 + section_size;
			}
			while (curaddr < file.Length);
			if (verbose > 0)
				Console.WriteLine("Model at {0}", (file.Length - 52 + startaddress).ToString("X"));
			return new NJS_OBJECT(file, file.Length - 52, 0, ModelFormat.Basic, null);
		}
		static int FindModelIndex(NJS_OBJECT mdl, NJS_OBJECT[] array)
		{
			for (int u = 0; u < array.Length; u++)
			{
				if (mdl == array[u]) return u;
			}
			return 0;
		}
	}

}
