using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SonicRetro.SAModel;
using SA_Tools;
using System.Linq;

namespace ObjScan
{
	class Program
	{
		static public List<int> deleteditems;
		static public Dictionary<int, string> addresslist;
		static public Dictionary<int, int[]> actionlist;
		static void CreateSplitIni(string filename, Game game, uint imageBase, bool bigendian, bool reverse, uint startoffset)
		{
			if (addresslist.Count == 0) return;
			if (File.Exists(filename)) filename = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + "_new.ini");
			Console.WriteLine("Creating split INI file: {0}", filename);
			StreamWriter sw = File.CreateText(filename);
			sw.WriteLine("key=" + imageBase.ToString("X"));
			if (Path.GetExtension(filename).ToLowerInvariant() == ".prs") sw.WriteLine("compressed=true");
			sw.WriteLine("game=" + game.ToString());
			if (bigendian) sw.WriteLine("bigendian=true");
			if (reverse) sw.WriteLine("reverse=true");
			if (startoffset != 0) sw.WriteLine("offset=" + startoffset.ToString("X8"));
			foreach (var entry in addresslist)
			{
				if (deleteditems.Contains(entry.Key)) continue;
				//Console.WriteLine("Adding object {0}", v.ObjectName);
				switch (entry.Value)
				{
					case "NJS_CNK_OBJECT":
					case "cnkobj":
						sw.WriteLine("[" + entry.Key.ToString("X8") + "]");
						sw.WriteLine("type=chunkmodel");
						sw.WriteLine("address=" + entry.Key.ToString("X8"));
						sw.WriteLine("filename=chunkmodels/" + entry.Key.ToString("X8") + ".sa2mdl");
						sw.WriteLine();
						break;
					case "NJS_GC_OBJECT":
					case "gcobj":
						sw.WriteLine("[" + entry.Key.ToString("X8") + "]");
						sw.WriteLine("type=gcmodel");
						sw.WriteLine("address=" + entry.Key.ToString("X8"));
						sw.WriteLine("filename=gcmodels/" + entry.Key.ToString("X8") + ".sa2bmdl");
						sw.WriteLine();
						break;
					case "NJS_OBJECT":
					case "obj":
						sw.WriteLine("[" + entry.Key.ToString("X8") + "]");
						if (game == Game.SADX) sw.WriteLine("type=basicdxmodel");
						else sw.WriteLine("type=basicmodel");
						sw.WriteLine("address=" + entry.Key.ToString("X8"));
						sw.WriteLine("filename=basicmodels/" + entry.Key.ToString("X8") + ".sa1mdl");
						bool first = true;
						foreach (var item in actionlist)
						{
							if (item.Value[0] == entry.Key)
							{
								if (first)
								{
									sw.Write("animations=");
									first = false;
								}
								else sw.Write(",");
								sw.Write("../actions/" + item.Key.ToString("X8") + ".saanim");
							}
						}
						if (!first) sw.WriteLine();
						sw.WriteLine();
						break;
					case "landtable":
					case "LandTable":
					case "_OBJ_LANDTABLE":
						sw.WriteLine("[" + entry.Key.ToString("X8") + "]");
						sw.WriteLine("type=landtable");
						sw.WriteLine("address=" + entry.Key.ToString("X8"));
						sw.WriteLine("filename=levels/" + entry.Key.ToString("X8") + ".sa1lvl");
						sw.WriteLine();
						break;
					case "NJS_MOTION":
						sw.WriteLine("[" + entry.Key.ToString("X8") + "]");
						sw.WriteLine("type=animation");
						sw.WriteLine("address=" + entry.Key.ToString("X8"));
						sw.WriteLine("numparts=" + actionlist[entry.Key][1].ToString());
						sw.WriteLine("filename=actions/" + entry.Key.ToString("X8") + ".saanim");
						sw.WriteLine();
						break;
				}
			}
			sw.Flush();
			sw.Close();
		}
		static bool CompareModels(NJS_OBJECT model1, NJS_OBJECT model2)
		{
			if (model1.GetFlags() != model2.GetFlags()) return false;
			if (model1.Position.X != model2.Position.X) return false;
			if (model1.Position.Y != model2.Position.Y) return false;
			if (model1.Position.Z != model2.Position.Z) return false;
			if (model1.Rotation.X != model2.Rotation.X) return false;
			if (model1.Rotation.Y != model2.Rotation.Y) return false;
			if (model1.Rotation.Z != model2.Rotation.Z) return false;
			if (model1.Scale.X != model2.Scale.X) return false;
			if (model1.Scale.Y != model2.Scale.Y) return false;
			if (model1.Scale.Z != model2.Scale.Z) return false;
			if (model1.CountAnimated() != model2.CountAnimated()) return false;
			if (model1.Attach != null && model2.Attach != null)
			{
				BasicAttach attach1 = (BasicAttach)model1.Attach;
				BasicAttach attach2 = (BasicAttach)model2.Attach;
				if (attach1.Material.Count != attach2.Material.Count) return false;
				if (attach1.Vertex.Length != attach2.Vertex.Length) return false;
				if (attach1.Normal.Length != attach2.Normal.Length) return false;
				if (attach1.Mesh.Count != attach2.Mesh.Count) return false;
			}
			return true;
		}
		static uint FindModel(byte[] datafile, string dir, ModelFormat modelfmt, uint imageBase, string filename)
		{
			//Basic only for now
			uint result = 0;
			ModelFile modelFile = new ModelFile(filename);
			NJS_OBJECT originalmodel = modelFile.Model;
			string model_extension = ".sa1mdl";
			Directory.CreateDirectory(Path.Combine(dir, "models"));
			for (int u = 0; u < datafile.Length - 51; u += 4)
			{
				int address = u;
				string fileOutputPath = Path.Combine(dir, "models", address.ToString("X8"));
				try
				{
					if (!CheckModel(datafile, address, imageBase, false, modelfmt)) continue;
					NJS_OBJECT mdl = new NJS_OBJECT(datafile, address, imageBase, modelfmt, new Dictionary<int, Attach>());
					if (!CompareModels(originalmodel, mdl)) continue;
					NJS_OBJECT[] children1 = originalmodel.Children.ToArray();
					NJS_OBJECT[] children2 = mdl.Children.ToArray();
					if (children1.Length != children2.Length) continue;
					for (int k = 0; k < children1.Length; k++)
					{
						if (!CompareModels(children1[k], children2[k])) continue;
					}
					ModelFile.CreateFile(fileOutputPath + model_extension, mdl, null, null, null, null, modelfmt);
					Console.WriteLine("Model at {0} seems to match!", address.ToString("X"));
					addresslist.Add(address, "NJS_OBJECT");
				}
				catch (Exception)
				{
					continue;
				}
			}
			return result;
		}
		static bool CheckModel(byte[] datafile, int address, uint imageBase, bool recursive, ModelFormat modelfmt)
		{
			int flags = 0;
			uint vertlist = 0;
			uint polylist = 0;
			int radius = 0;
			uint attach = 0;
			uint child = 0;
			uint sibling = 0;
			uint vertices = 0;
			uint normals = 0;
			uint vert_count = 0;
			uint meshlists = 0;
			short mesh_count = 0;
			short mat_count = 0;
			uint opaquepoly = 0;
			uint alphapoly = 0;
			Vertex pos;
			Vertex scl;
			switch (modelfmt)
			{
				case ModelFormat.Basic:
				case ModelFormat.BasicDX:
					if (address <= 0 || address > datafile.Length - 20) return false;
					flags = ByteConverter.ToInt32(datafile, address);
					if (flags > 0x3FFF || flags < 0) return false;
					attach = ByteConverter.ToUInt32(datafile, address + 4);
					if (attach != 0)
					{
						if (attach < imageBase) return false;
						if (attach > (uint)datafile.Length + imageBase - 51) return false;
						vertices = ByteConverter.ToUInt32(datafile, (int)(attach - imageBase));
						if (vertices > (uint)datafile.Length + imageBase - 51) return false;
						if (vertices < imageBase) return false;
						normals = ByteConverter.ToUInt32(datafile, (int)(attach - imageBase) + 4);
						if (normals != 0 && normals < imageBase) return false;
						if (normals > (uint)datafile.Length + imageBase - 51) return false;
						vert_count = ByteConverter.ToUInt32(datafile, (int)(attach - imageBase) + 8);
						if (vert_count > 2048 || vert_count == 0) return false;
						meshlists = ByteConverter.ToUInt32(datafile, (int)(attach - imageBase) + 0xC);
						if (meshlists != 0 && meshlists < imageBase) return false;
						if (meshlists > (uint)datafile.Length + imageBase - 51) return false;
						mesh_count = ByteConverter.ToInt16(datafile, (int)(attach - imageBase) + 0x14);
						if (mesh_count > 2048 || mesh_count < 0) return false;
						mat_count = ByteConverter.ToInt16(datafile, (int)(attach - imageBase) + 0x16);
						if (mat_count > 2048 || mat_count < 0) return false;
					}
					pos = new Vertex(datafile, address + 8);
					if (pos.X < -100000 || pos.X > 100000) return false;
					if (pos.Y < -100000 || pos.Y > 100000) return false;
					if (pos.Z < -100000 || pos.Z > 100000) return false;
					scl = new Vertex(datafile, address + 0x20);
					if (scl.X <= 0 || scl.X > 10000) return false;
					if (scl.Y <= 0 || scl.Y > 10000) return false;
					if (scl.Z <= 0 || scl.Z > 10000) return false;
					child = ByteConverter.ToUInt32(datafile, address + 0x2C);
					sibling = ByteConverter.ToUInt32(datafile, address + 0x30);
					if (child > (uint)address + imageBase) return false;
					if (sibling > (uint)address + imageBase) return false;
					if (child > (uint)datafile.Length + imageBase - 52) return false;
					if (sibling > (uint)datafile.Length + imageBase - 52) return false;
					if (child != 0 && child < imageBase) return false;
					if (sibling != 0 && sibling < imageBase) return false;
					if (recursive && child != 0 && !CheckModel(datafile, (int)(child - imageBase), imageBase, false, modelfmt)) return false;
					if (recursive && sibling != 0 && !CheckModel(datafile, (int)(sibling - imageBase), imageBase, false, modelfmt)) return false;
					if (attach == 0 && flags == 0) return false;
					if (attach == 0 && child == 0 && sibling == 0) return false;
					break;
				case ModelFormat.Chunk:
					if (address <= 0 || address > datafile.Length - 20) return false;
					flags = ByteConverter.ToInt32(datafile, address);
					if (flags > 0x3FFF || flags < 0) return false;
					attach = ByteConverter.ToUInt32(datafile, address + 4);
					if (attach != 0)
					{
						if (attach < imageBase) return false;
						if (attach > (uint)datafile.Length + imageBase - 51) return false;
						vertlist = ByteConverter.ToUInt32(datafile, (int)(attach - imageBase));
						if (vertlist > (uint)datafile.Length + imageBase - 51) return false;
						if (vertlist != 0 && vertlist < imageBase) return false;
						polylist = ByteConverter.ToUInt32(datafile, (int)(attach - imageBase) + 4);
						if (polylist != 0 && polylist < imageBase) return false;
						if (polylist > (uint)datafile.Length + imageBase - 51) return false;
						radius = ByteConverter.ToInt32(datafile, (int)(attach - imageBase) + 0x14);
						if (radius < 0) return false;
					}
					pos = new Vertex(datafile, address + 8);
					if (pos.X < -100000 || pos.X > 100000) return false;
					if (pos.Y < -100000 || pos.Y > 100000) return false;
					if (pos.Z < -100000 || pos.Z > 100000) return false;
					scl = new Vertex(datafile, address + 0x20);
					if (scl.X <= 0 || scl.X > 10000) return false;
					if (scl.Y <= 0 || scl.Y > 10000) return false;
					if (scl.Z <= 0 || scl.Z > 10000) return false;
					child = ByteConverter.ToUInt32(datafile, address + 0x2C);
					sibling = ByteConverter.ToUInt32(datafile, address + 0x30);
					if (child > (uint)address + imageBase) return false;
					if (sibling > (uint)address + imageBase) return false;
					if (child > (uint)datafile.Length + imageBase - 52) return false;
					if (sibling > (uint)datafile.Length + imageBase - 52) return false;
					if (child != 0 && child < imageBase) return false;
					if (sibling != 0 && sibling < imageBase) return false;
					if (recursive && child != 0 && !CheckModel(datafile, (int)(child - imageBase), imageBase, false, modelfmt)) return false;
					if (recursive && sibling != 0 && !CheckModel(datafile, (int)(sibling - imageBase), imageBase, false, modelfmt)) return false;
					if (attach == 0 && flags == 0) return false;
					if (attach == 0 && child == 0 && sibling == 0) return false;
					break;
				case ModelFormat.GC:
					if (address <= 0 || address > datafile.Length - 20) return false;
					flags = ByteConverter.ToInt32(datafile, address);
					if (flags > 0x3FFF || flags < 0) return false;
					attach = ByteConverter.ToUInt32(datafile, address + 4);
					if (attach != 0)
					{
						if (attach < imageBase) return false;
						if (attach > (uint)datafile.Length + imageBase - 51) return false;
						vertlist = ByteConverter.ToUInt32(datafile, (int)(attach - imageBase));
						if (vertlist > (uint)datafile.Length + imageBase - 51) return false;
						if (vertlist < imageBase) return false;
						opaquepoly = ByteConverter.ToUInt32(datafile, (int)(attach - imageBase) + 8);
						if (opaquepoly != 0 && opaquepoly < imageBase) return false;
						if (opaquepoly > (uint)datafile.Length + imageBase - 51) return false;
						alphapoly = ByteConverter.ToUInt32(datafile, (int)(attach - imageBase) + 0xC);
						if (alphapoly != 0 && alphapoly < imageBase) return false;
						if (alphapoly > (uint)datafile.Length + imageBase - 51) return false;
					}
					pos = new Vertex(datafile, address + 8);
					if (pos.X < -100000 || pos.X > 100000) return false;
					if (pos.Y < -100000 || pos.Y > 100000) return false;
					if (pos.Z < -100000 || pos.Z > 100000) return false;
					scl = new Vertex(datafile, address + 0x20);
					if (scl.X <= 0 || scl.X > 10000) return false;
					if (scl.Y <= 0 || scl.Y > 10000) return false;
					if (scl.Z <= 0 || scl.Z > 10000) return false;
					child = ByteConverter.ToUInt32(datafile, address + 0x2C);
					sibling = ByteConverter.ToUInt32(datafile, address + 0x30);
					if (child > (uint)address + imageBase) return false;
					if (sibling > (uint)address + imageBase) return false;
					if (child > (uint)datafile.Length + imageBase - 52) return false;
					if (sibling > (uint)datafile.Length + imageBase - 52) return false;
					if (child != 0 && child < imageBase) return false;
					if (sibling != 0 && sibling < imageBase) return false;
					if (recursive && child != 0 && !CheckModel(datafile, (int)(child - imageBase), imageBase, false, modelfmt)) return false;
					if (recursive && sibling != 0 && !CheckModel(datafile, (int)(sibling - imageBase), imageBase, false, modelfmt)) return false;
					if (attach == 0 && flags == 0) return false;
					if (attach == 0 && child == 0 && sibling == 0) return false;
					break;
			}
			if (recursive) Console.WriteLine("{0} model at {1}", modelfmt.ToString(), address.ToString("X"));
			return true;
		}

		static bool CheckLandTable(byte[] datafile, int address, uint imageBase, LandTableFormat landfmt)
		{
			short COLCount;
			short AnimCount;
			short ChunkCount;
			ushort Unknown1;
			int COLAddress;
			int AnimPointer;
			int Texlist;
			uint Buffer;
			int ObjAddrPointer;
			int ObjAddr;
			ModelFormat modelfmt = ModelFormat.Basic;
			switch (landfmt)
			{
				case LandTableFormat.SA1:
					modelfmt = ModelFormat.Basic;
					break;
				case LandTableFormat.SADX:
					modelfmt = ModelFormat.BasicDX;
					break;
				case LandTableFormat.SA2:
					modelfmt = ModelFormat.Chunk;
					break;
				case LandTableFormat.SA2B:
					modelfmt = ModelFormat.GC;
					break;
			}
			switch (landfmt)
			{
				case LandTableFormat.SA1:
				case LandTableFormat.SADX:
					COLCount = ByteConverter.ToInt16(datafile, address);
					if (COLCount < 0) return false;
					AnimCount = ByteConverter.ToInt16(datafile, address + 2);
					if (AnimCount < 0) return false;
					COLAddress = ByteConverter.ToInt32(datafile, address + 0xC);
					if (COLAddress < imageBase || COLAddress == 0) return false;
					if (COLAddress - imageBase > datafile.Length - 32) return false;
					AnimPointer = ByteConverter.ToInt32(datafile, address + 0x10);
					if (AnimPointer != 0 && AnimPointer < imageBase) return false;
					if (AnimPointer - imageBase > datafile.Length - 32) return false;
					ObjAddrPointer = COLAddress - (int)imageBase + 0x18;
					ObjAddr = ByteConverter.ToInt32(datafile, ObjAddrPointer);
					if (ObjAddr < imageBase) return false;
					if (!CheckModel(datafile, (int)(ObjAddr - imageBase), imageBase, false, modelfmt)) return false;
					break;
				case LandTableFormat.SA2:
				case LandTableFormat.SA2B:
					COLCount = ByteConverter.ToInt16(datafile, address);
					if (COLCount < 0) return false;
					ChunkCount = ByteConverter.ToInt16(datafile, address + 2);
					if (ChunkCount < -1) return false;
					Unknown1 = ByteConverter.ToUInt16(datafile, address + 4);
					if (Unknown1 != 65535) return false;
					COLAddress = ByteConverter.ToInt32(datafile, address + 0x10);
					if (COLAddress < (int)imageBase) return false;
					if (COLAddress - (int)imageBase > datafile.Length - 32) return false;
					Buffer = ByteConverter.ToUInt32(datafile, address + 0x14);
					if (Buffer != 0) return false;
					AnimPointer = ByteConverter.ToInt32(datafile, address + 0x18);
					if (AnimPointer != 0 && AnimPointer < (int)imageBase) return false;
					if (AnimPointer != 0 && AnimPointer - (int)imageBase > datafile.Length - 32) return false;
					Texlist = ByteConverter.ToInt32(datafile, address + 0x1C);
					if ((int)(Texlist - imageBase) > datafile.Length - 32) return false;
					if (Texlist == 0 || Texlist < (int)imageBase) return false;
					break;
			}
			return true;
		}

		static void ScanLandtable(byte[] datafile, uint imageBase, string dir, LandTableFormat landfmt, List<int> landtablelist)
		{
			Console.WriteLine("Landtable scan: {0}", landfmt.ToString());
			string landtable_extension = ".sa1lvl";

			switch (landfmt)
			{
				case LandTableFormat.SA1:
				case LandTableFormat.SADX:
					landtable_extension = ".sa1lvl";
					break;
				case LandTableFormat.SA2:
					landtable_extension = ".sa2lvl";
					break;
				case LandTableFormat.SA2B:
					landtable_extension = ".sa2blvl";
					break;
			}
		
			Directory.CreateDirectory(Path.Combine(dir, "levels"));
			for (int u = 0; u < datafile.Length - 52; u += 4)
			{
				int address = u;
				string fileOutputPath = Path.Combine(dir, "levels", address.ToString("X8"));
				if (!CheckLandTable(datafile, address, imageBase, landfmt)) continue;
				try
				{
					LandTable land = new LandTable(datafile, address, imageBase, landfmt);
					if (land.COL.Count > 3)
					{
						land.SaveToFile(fileOutputPath + landtable_extension, landfmt);
						landtablelist.Add(address);
						Console.WriteLine("Landtable at {0}", address.ToString("X8"));
						addresslist.Add(address, "landtable_"+ landfmt.ToString());
					}
				}
				catch (Exception)
				{
					continue;
				}
			}
		}

		static void AddAction(int objectaddr, int motionaddr, string dir)
		{
			using (FileStream str = new FileStream(Path.Combine(dir, "basicmodels", objectaddr.ToString("X8") + ".action"), FileMode.Append, FileAccess.Write))
			using (StreamWriter tw = new StreamWriter(str))
			{
				tw.WriteLine("../actions/" + motionaddr.ToString("X8") + ".saanim");
				tw.Flush();
				tw.Close();
			}
		}
		static void ScanActions(byte[] datafile, uint imageBase, string dir, int addr, int nummdl, ModelFormat modelfmt, bool bigendian)
		{
			ByteConverter.BigEndian = SonicRetro.SAModel.ByteConverter.BigEndian = bigendian;
			if (nummdl == 0) return;
			for (int address = 0; address < datafile.Length - 8; address += 4)
			{
				if (ByteConverter.ToUInt32(datafile, address) != addr + imageBase) continue;
				uint motaddr = ByteConverter.ToUInt32(datafile, address + 4);
				if (motaddr < imageBase) continue;
				try
				{
					NJS_MOTION mot = new NJS_MOTION(datafile, (int)(motaddr -imageBase), imageBase, nummdl, null, false);
					if (mot.Models.Count == 0) continue;
					addresslist.Add((int)(motaddr - imageBase), "NJS_MOTION");
					Console.WriteLine("Motion found for model {0} at address {1}", addr.ToString("X8"), (motaddr-imageBase).ToString("X"));
					string fileOutputPath = Path.Combine(dir, "actions", (motaddr - imageBase).ToString("X8"));
					mot.Save(fileOutputPath + ".saanim");
					int[] arr = new int[2];
					arr[0] = addr;
					arr[1] = nummdl;
					actionlist.Add((int)(motaddr - imageBase), arr);
					AddAction(addr, (int)(motaddr - imageBase), dir); 
				}
				catch (Exception)
				{
					continue;
				}
			}
		}
		static void ScanAnimations(byte[] datafile, uint imageBase, string dir, ModelFormat modelfmt, bool bigendian)
		{
			ByteConverter.BigEndian = SonicRetro.SAModel.ByteConverter.BigEndian = bigendian;
			string modelstring = "basicmodels";
			string modelext = ".sa1mdl";
			List<int> modeladdr = new List<int>();
			if (addresslist.Count == 0) return;
			Directory.CreateDirectory(Path.Combine(dir, "actions"));
			Console.WriteLine("Scanning for actions...");
			switch (modelfmt)
			{
				case ModelFormat.Basic:
				case ModelFormat.BasicDX:
					modelstring = "basicmodels";
					modelext = ".sa1mdl";
					break;
				case ModelFormat.Chunk:
					modelstring = "chunkmodels";
					modelext = ".sa2mdl";
					break;
				case ModelFormat.GC:
					modelstring = "gcmodels";
					modelext = ".sa2bmdl";
					break;
			}
			foreach (var entry in addresslist)
			{
				if (deleteditems.Contains(entry.Key)) continue;
				if (entry.Value == "NJS_OBJECT" || entry.Value == "NJS_CNK_OBJECT" || entry.Value == "NJS_GC_OBJECT") modeladdr.Add(entry.Key);
			}
			foreach (int maddr in modeladdr)
			{
				if (File.Exists(Path.Combine(dir, modelstring, maddr.ToString("X8") + modelext)))
				{
					try
					{
						ModelFile mdlfile = new ModelFile(Path.Combine(dir, modelstring, maddr.ToString("X8") + modelext));
						ScanActions(datafile, imageBase, dir, maddr, mdlfile.Model.CountAnimated(), modelfmt, bigendian);
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error adding action for model at {0}: {1}", maddr.ToString("X"), ex.ToString());
					}
				}
			}
		}
		static void ScanModel(byte[] datafile, uint imageBase, string dir, ModelFormat modelfmt, bool bigendian)
		{
			ByteConverter.BigEndian = SonicRetro.SAModel.ByteConverter.BigEndian = bigendian;
			Console.WriteLine("Model scan: {0}", modelfmt);
			string model_extension = ".sa1mdl";
			string model_dir = "basicmodels";
			string model_type = "NJS_OBJECT";
			switch (modelfmt)
			{
				case ModelFormat.Basic:
				case ModelFormat.BasicDX:
					model_extension = ".sa1mdl";
					model_dir = "basicmodels";
					model_type = "NJS_OBJECT";
					break;
				case ModelFormat.Chunk:
					model_extension = ".sa2mdl";
					model_dir = "chunkmodels";
					model_type = "NJS_CNK_OBJECT";
					break;
				case ModelFormat.GC:
					model_extension = ".sa2bmdl";
					model_dir = "gcmodels";
					model_type = "NJS_GC_OBJECT";
					break;
			}
			Directory.CreateDirectory(Path.Combine(dir, model_dir));
			for (int u = 0; u < datafile.Length - 51; u += 4)
			{
				int address = u;
				string fileOutputPath = Path.Combine(dir, model_dir, address.ToString("X8"));
				try
				{
					if (!CheckModel(datafile, address, imageBase, true, modelfmt)) continue;
					NJS_OBJECT mdl = new NJS_OBJECT(datafile, address, imageBase, modelfmt, new Dictionary<int, Attach>());
					ModelFile.CreateFile(fileOutputPath + model_extension, mdl, null, null, null, null, modelfmt);

					addresslist.Add(address, model_type);
					if (mdl.Children.Count > 0)
					{
						foreach (NJS_OBJECT child in mdl.Children)
						{
							string cpath = Path.Combine(dir, model_dir, child.Name.Substring(7, child.Name.Length - 7) + model_extension);
							Console.WriteLine("Deleting child object {0}", cpath);
							File.Delete(cpath);
							deleteditems.Add(int.Parse(child.Name.Substring(7, child.Name.Length - 7), NumberStyles.AllowHexSpecifier));
						}
					}
					if (mdl.Sibling != null)
					{
						string spath = Path.Combine(dir,  model_dir, mdl.Sibling.Name.Substring(7, mdl.Sibling.Name.Length - 7) + model_extension);
						Console.WriteLine("Deleting sibling object {0}", spath);
						File.Delete(spath);
						deleteditems.Add(int.Parse(mdl.Sibling.Name.Substring(7, mdl.Sibling.Name.Length - 7), NumberStyles.AllowHexSpecifier));
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error adding model at {0}: {1}", address.ToString("X"), ex.Message.ToString());
					continue;
				}
			}
		}

		static void CleanUpLandtable(byte[] datafile, List<int> landtablelist, uint imageBase, string dir, bool bigendian)
		{
			bool delete_basic = false;
			bool delete_chunk = false;
			bool delete_gc = false;
			ByteConverter.BigEndian = SonicRetro.SAModel.ByteConverter.BigEndian = bigendian;
			string model_extension = ".sa1mdl";
			string model_dir = "basicmodels";
			LandTableFormat landfmt = LandTableFormat.SA1;
			foreach (int landaddr in landtablelist)
			{
				switch (addresslist[landaddr])
				{
					case "landtable_SA1":
						landfmt = LandTableFormat.SA1;
						delete_basic = true;
						break;
					case "landtable_SADX":
						landfmt = LandTableFormat.SADX;
						delete_basic = true;
						break;
					case "landtable_SA2":
						landfmt = LandTableFormat.SA2;
						delete_basic = true;
						delete_chunk = true;
						break;
					case "landtable_SA2B":
						landfmt = LandTableFormat.SA2B;
						delete_basic = true;
						delete_gc = true;
						delete_chunk = true;
						break;
				}
				//Console.WriteLine("Landtable {0}, {1}, {2}", landaddr.ToString("X"), imageBase.ToString("X"), landfmt.ToString());
				LandTable land = new LandTable(datafile, landaddr, imageBase, landfmt);
				if (land.COL.Count > 0)
				{
					foreach (COL col in land.COL)
					{
						for (int i = 0; i < 3; i++)
						{
							if (i == 0 && delete_basic)
							{
								model_dir = "basicmodels";
								model_extension = ".sa1mdl";
							}
							else if (i == 1 && delete_chunk)
							{
								model_dir = "chunkmodels";
								model_extension = ".sa2mdl";
							}
							else if (i == 2 && delete_gc)
							{
								model_dir = "gcmodels";
								model_extension = ".sa2bmdl";
							}
							string col_filename = Path.Combine(dir, model_dir, col.Model.Name.Substring(7, col.Model.Name.Length - 7) + model_extension);
							if (File.Exists(col_filename))
							{
								File.Delete(col_filename);
								Console.WriteLine("Deleting landtable object {0}", col_filename);
								deleteditems.Add(int.Parse(col.Model.Name.Substring(7, col.Model.Name.Length - 7), NumberStyles.AllowHexSpecifier));
							}
						}
					}
					foreach (GeoAnimData anim in land.Anim)
					{
						for (int i = 0; i < 3; i++)
						{
							if (i == 0 && delete_basic)
							{
								model_dir = "basicmodels";
								model_extension = ".sa1mdl";
							}
							else if (i == 1 && delete_chunk)
							{
								model_dir = "chunkmodels";
								model_extension = ".sa2mdl";
							}
							else if (i == 2 && delete_gc)
							{
								model_dir = "gcmodels";
								model_extension = ".sa2bmdl";
							}
							string anim_filename = Path.Combine(dir, model_dir, anim.Model.Name.Substring(7, anim.Model.Name.Length - 7) + model_extension);
							if (File.Exists(anim_filename))
							{
								File.Delete(anim_filename);
								Console.WriteLine("Deleting landtable GeoAnim object {0}", anim_filename);
								deleteditems.Add(int.Parse(anim.Model.Name.Substring(7, anim.Model.Name.Length - 7), NumberStyles.AllowHexSpecifier));
							}
						}
					}
				}
			}
		}

		static void Main(string[] args)
		{
			bool scan_sa1_land = false;
			bool scan_sadx_land = false;
			bool scan_sa2_land = false;
			bool scan_sa2b_land = false;
			bool scan_sa1_model = false;
			bool scan_sadx_model = false;
			bool scan_sa2_model = false;
			bool scan_sa2b_model = false;
			deleteditems = new List<int>();
			addresslist = new Dictionary<int, string>();
			List<int> landtablelist = new List<int>();
			actionlist = new Dictionary<int, int[]>();
			Game game;
			string filename;
			string dir;
			bool bigendian = false;
			bool reverse = false;
			string matchfile = "";
			bool skipactions = false;
			uint startoffset = 0;
			byte[] datafile;
			string type;
			uint imageBase;
			if (args.Length == 0)
			{
				Console.WriteLine("Object Scanner is a tool that scans a binary file or memory dump and extracts levels or models from it.");
				Console.WriteLine("Usage with split INI: objscan <FILENAME> <TYPE> [-noaction]");
				Console.WriteLine("Usage without split INI: objscan <GAME> <FILENAME> <KEY> <TYPE> [-offset offset] [-file modelfile] [-noaction]\n");
				Console.WriteLine("Argument description:");
				Console.WriteLine("<GAME>: SA1, SADX, SA2, SA2B. Add '_b' (e.g. SADX_b) to switch to Big Endian, use SADX_g to scan the Gamecube version of SADX.");
				Console.WriteLine("<FILENAME>: The name of the binary file, e.g. sonic.exe.");
				Console.WriteLine("<KEY>: Binary key, e.g. 400000 for sonic.exe or C900000 for SA1 STG file. Use C900000 for Gamecube REL files.");
				Console.WriteLine("<TYPE>: model, basicmodel, basicdxmodel, chunkmodel, gcmodel, landtable, all, match");
				Console.WriteLine("offset: Start offset (hexadecimal).");
				Console.WriteLine("modelfile: Path to .sa1mdl file to use in match mode.");
				Console.WriteLine("-noaction: Don't scan for actions.\n");
				Console.WriteLine("Cleaning up landtable objects:");
				Console.WriteLine("If a split INI file is used, the scanner will clean up landtable models for all levels defined in the INI file.\n");
				Console.WriteLine("Press ENTER to exit");
				Console.ReadLine();
				return;
			}
			if (args.Length == 2 || (args.Length == 3 && args[2] == "-noaction"))
			{
				filename = args[0];
				if ((Path.GetExtension(filename).ToLowerInvariant()) == ".ini")
				{
					Console.WriteLine("Error: input file must be binary, not INI.");
					Console.WriteLine("Press ENTER to exit");
					Console.ReadLine();
					return;
				}
				type = args[1];
				string inifilename = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + ".ini");
				if (File.Exists(inifilename))
				{
					IniData inifile = IniSerializer.Deserialize<IniData>(inifilename);
					game = inifile.Game;
					bigendian = inifile.BigEndian;
					reverse = inifile.Reverse;
					startoffset = inifile.StartOffset;
					if (inifile.ImageBase.HasValue) imageBase = inifile.ImageBase.Value;
					else imageBase = 0;
					foreach (KeyValuePair<string, SA_Tools.FileInfo> item in new List<KeyValuePair<string, SA_Tools.FileInfo>>(inifile.Files))
					{
						if (string.IsNullOrEmpty(item.Key)) continue;
						SA_Tools.FileInfo data = item.Value;
						string split_type = data.Type;
						int split_address = data.Address;
						if (split_type == "landtable")
						{
							landtablelist.Add(split_address);
							Console.WriteLine("Adding landtable at {0}", split_address.ToString("X8"));
						}
					}
				}
				else
				{
					Console.WriteLine("Could not find the split INI file. Use more arguments to scan without a split file.");
					Console.WriteLine("Press ENTER to exit");
					Console.ReadLine();
					return;
				}
			}
			else
			{
				switch (args[0].ToLowerInvariant())
				{
					case "sa1":
						game = Game.SA1;
						scan_sa1_land = true;
						scan_sa1_model = true;
						break;
					case "sa1_b":
						game = Game.SA1;
						bigendian = true;
						scan_sa1_land = true;
						scan_sa1_model = true;
						break;
					case "sadx":
						game = Game.SADX;
						scan_sadx_land = true;
						scan_sadx_model = true;
						scan_sa2_model = true;
						break;
					case "sadx_b":
						game = Game.SADX;
						bigendian = true;
						scan_sadx_land = true;
						scan_sadx_model = true;
						scan_sa2_model = true;
						break;
					case "sadx_g":
						game = Game.SA1;
						bigendian = true;
						reverse = true;
						scan_sa1_land = true;
						scan_sa1_model = true;
						scan_sa2_model = true;
						break;
					case "sa2":
						game = Game.SA2;
						scan_sa2_land = true;
						scan_sa1_model = true;
						scan_sa2_model = true;
						break;
					case "sa2_b":
						game = Game.SA2;
						bigendian = true;
						scan_sa2_land = true;
						scan_sa1_model = true;
						scan_sa2_model = true;
						break;
					case "sa2b":
						game = Game.SA2B;
						scan_sa2_land = true;
						scan_sa2b_land = true;
						scan_sa1_model = true;
						scan_sa2_model = true;
						scan_sa2b_model = true;
						break;
					case "sa2b_b":
						game = Game.SA2B;
						bigendian = true;
						scan_sa2_land = true;
						scan_sa2b_land = true;
						scan_sa1_model = true;
						scan_sa2_model = true;
						scan_sa2b_model = true;
						break;
					default:
						Console.WriteLine("Error parsing game type.\nCorrect game types are: SA1, SADX, SA1_b, SADX_b, SADX_x, SA2, SA2B, SA2_b, SA2B_b");
						Console.WriteLine("Press ENTER to exit.");
						Console.ReadLine();
						return;
				}
				filename = args[1];
				imageBase = uint.Parse(args[2], NumberStyles.AllowHexSpecifier);
				type = args[3];
			}
			for (int u = 2; u < args.Length; u++)
			{
				switch (args[u])
				{
					case "-offset":
						startoffset = uint.Parse(args[u + 1], NumberStyles.AllowHexSpecifier);
						break;
					case "-noaction":
						skipactions = true;
						break;
					case "-file":
						matchfile = args[u + 1];
						break;
				}
			}
			Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(filename));
			ByteConverter.BigEndian = SonicRetro.SAModel.ByteConverter.BigEndian = bigendian;
			ByteConverter.Reverse = SonicRetro.SAModel.ByteConverter.Reverse = reverse;
			bool SA2 = game == Game.SA2 | game == Game.SA2B;
			ModelFormat modelfmt = ModelFormat.BasicDX;
			LandTableFormat landfmt = LandTableFormat.SADX;
			switch (game)
			{
				case Game.SA1:
					modelfmt = ModelFormat.Basic;
					landfmt = LandTableFormat.SA1;
					break;
				case Game.SADX:
					modelfmt = ModelFormat.BasicDX;
					landfmt = LandTableFormat.SADX;
					break;
				case Game.SA2:
					modelfmt = ModelFormat.Chunk;
					landfmt = LandTableFormat.SA2;
					break;
				case Game.SA2B:
					modelfmt = ModelFormat.GC;
					landfmt = LandTableFormat.SA2B;
					break;
				
			}
			byte[] datafile_temp = File.ReadAllBytes(filename);
			if (Path.GetExtension(filename).ToLowerInvariant() == ".prs") datafile_temp = FraGag.Compression.Prs.Decompress(datafile_temp);
			if (Path.GetExtension(filename).ToLowerInvariant() == ".rel") HelperFunctions.FixRELPointers(datafile_temp, 0xC900000);
			if (startoffset != 0)
			{
				byte[] datafile_new = new byte[startoffset + datafile_temp.Length];
				datafile_temp.CopyTo(datafile_new, startoffset);
				datafile = datafile_new;
			}
			else datafile = datafile_temp;
			if (imageBase == 0) imageBase = HelperFunctions.SetupEXE(ref datafile) ?? 0;
			dir = Path.Combine(Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(filename));
			if (Directory.Exists(dir)) Directory.Delete(dir, true);
			Directory.CreateDirectory(dir);
			Console.Write("Game: {0}, file: {1}, key: 0x{2}, scanning for {3}", game.ToString(), filename, imageBase.ToString("X"), type);
			if (startoffset != 0)
				Console.Write(", Offset: {0}", startoffset);
			if (bigendian)
				Console.Write(", Big Endian");
			if (reverse)
				Console.Write(", Reversed");
			Console.Write(System.Environment.NewLine);
			switch (type.ToLowerInvariant())
			{
				case "match":
					FindModel(datafile, dir, modelfmt, imageBase, matchfile);
					skipactions = true;
					break;
				case "all":
					if (scan_sa1_land) ScanLandtable(datafile, imageBase, dir, LandTableFormat.SA1, landtablelist);
					if (scan_sadx_land) ScanLandtable(datafile, imageBase, dir, LandTableFormat.SADX, landtablelist);
					if (scan_sa2_land) ScanLandtable(datafile, imageBase, dir, LandTableFormat.SA2, landtablelist);
					if (scan_sa2b_land) ScanLandtable(datafile, imageBase, dir, LandTableFormat.SA2B, landtablelist);
					if (scan_sa1_model) ScanModel(datafile, imageBase, dir, ModelFormat.Basic, bigendian);
					if (scan_sadx_model) ScanModel(datafile, imageBase, dir, ModelFormat.BasicDX, bigendian);
					if (scan_sa2_model) ScanModel(datafile, imageBase, dir, ModelFormat.Chunk, bigendian);
					if (scan_sa2b_model) ScanModel(datafile, imageBase, dir, ModelFormat.GC, bigendian);
					break;
				case "landtable":
					if (scan_sa1_land) ScanLandtable(datafile, imageBase, dir, LandTableFormat.SA1, landtablelist);
					if (scan_sadx_land) ScanLandtable(datafile, imageBase, dir, LandTableFormat.SADX, landtablelist);
					if (scan_sa2_land) ScanLandtable(datafile, imageBase, dir, LandTableFormat.SA2, landtablelist);
					if (scan_sa2b_land) ScanLandtable(datafile, imageBase, dir, LandTableFormat.SA2B, landtablelist);
					skipactions = true;
					break;
				case "model":
					if (scan_sa1_model) ScanModel(datafile, imageBase, dir, ModelFormat.Basic, bigendian);
					if (scan_sadx_model) ScanModel(datafile, imageBase, dir, ModelFormat.BasicDX, bigendian);
					if (scan_sa2_model) ScanModel(datafile, imageBase, dir, ModelFormat.Chunk, bigendian);
					if (scan_sa2b_model) ScanModel(datafile, imageBase, dir, ModelFormat.GC, bigendian);
					break;
				case "basicmodel":
					ScanModel(datafile, imageBase, dir, ModelFormat.Basic, bigendian);
					break;
				case "basicdxmodel":
					ScanModel(datafile, imageBase, dir, ModelFormat.BasicDX, bigendian);
					break;
				case "chunkmodel":
					ScanModel(datafile, imageBase, dir, ModelFormat.Chunk, bigendian);
					break;
				case "gcmodel":
					ScanModel(datafile, imageBase, dir, ModelFormat.GC, bigendian);
					break;
			}
			CleanUpLandtable(datafile, landtablelist, imageBase, dir, bigendian);
			if (!skipactions) ScanAnimations(datafile, imageBase, dir, modelfmt, bigendian);
			CreateSplitIni(Path.Combine(Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(filename) + ".INI"), game, imageBase, bigendian, reverse, startoffset);
			//Clean up empty folders
			bool land = false;
			bool basicmodel = false;
			bool chunkmodel = false;
			bool gcmodel = false;
			bool motion = false;
			foreach (var item in addresslist)
			{
				if (item.Value == "landtable") land = true;
				if (item.Value == "NJS_OBJECT") basicmodel = true;
				if (item.Value == "NJS_CNK_OBJECT") chunkmodel = true;
				if (item.Value == "NJS_GC_OBJECT") gcmodel = true;
				if (item.Value == "NJS_MOTION") motion = true;
			}
			if (!motion && Directory.Exists(Path.Combine(dir, "actions"))) Directory.Delete(Path.Combine(dir, "actions"), true);
			if (!land && Directory.Exists(Path.Combine(dir, "levels"))) Directory.Delete(Path.Combine(dir, "levels"), true);
			if (!basicmodel && Directory.Exists(Path.Combine(dir, "basicmodels"))) Directory.Delete(Path.Combine(dir, "basicmodels"), true);
			if (!chunkmodel && Directory.Exists(Path.Combine(dir, "chunkmodels"))) Directory.Delete(Path.Combine(dir, "chunkmodels"), true);
			if (!gcmodel && Directory.Exists(Path.Combine(dir, "gcmodels"))) Directory.Delete(Path.Combine(dir, "gcmodels"), true);
			if (!land && !basicmodel && !motion && !basicmodel && !chunkmodel && !gcmodel && Directory.Exists(dir)) Directory.Delete(dir, true);
		}
	}
}