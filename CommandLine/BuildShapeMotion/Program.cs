using SA_Tools;
using SonicRetro.SAModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BuildShapeMotion
{
	class Program
	{
		static void Main(string[] args)
		{
			string path;
			if (args.Length > 0)
				path = args[0];
			else
			{
				Console.Write("Path: ");
				path = Console.ReadLine().Trim('"');
			}
			path = Path.GetFullPath(path);
			if (path.EndsWith(".ini"))
				path = Path.GetDirectoryName(path);
			string mtnfn = new DirectoryInfo(path).Name;
			MTNInfo info = IniSerializer.Deserialize<MTNInfo>(Path.Combine(path, mtnfn + ".ini"));
			string fext = null;
			switch (info.ModelFormat)
			{
				case ModelFormat.Basic:
				case ModelFormat.BasicDX:
					fext = "*.sa1mdl";
					break;
				case ModelFormat.Chunk:
					fext = "*.sa2mdl";
					break;
				case ModelFormat.GC:
					fext = "*.sa2bmdl";
					break;
			}
			SortedDictionary<int, NJS_OBJECT> mdls = new SortedDictionary<int, NJS_OBJECT>();
			foreach (string fn in Directory.EnumerateFiles(path, fext))
				if (int.TryParse(Path.GetFileNameWithoutExtension(fn), out int i) && i >= 0 && i < info.Frames)
					mdls[i] = new ModelFile(fn).Model;
			NJS_OBJECT first = mdls.First().Value;
			int nodecnt = first.CountMorph();
			int[] vcnt = new int[nodecnt];
			ushort[][] polys = new ushort[nodecnt][];
			NJS_OBJECT[] nodes = first.GetObjects().Where(a => a.Morph).ToArray();
			for (int i = 0; i < nodecnt; i++)
				switch (nodes[i].Attach)
				{
					case BasicAttach batt:
						vcnt[i] = batt.Vertex.Length;
						List<ushort> plist = new List<ushort>();
						foreach (NJS_MESHSET mesh in batt.Mesh)
							foreach (Poly p in mesh.Poly)
								plist.AddRange(p.Indexes);
						polys[i] = plist.ToArray();
						break;
					case ChunkAttach catt:
						if (catt.Vertex != null)
							vcnt[i] = catt.Vertex.Sum(a => a.VertexCount);
						plist = new List<ushort>();
						foreach (PolyChunkStrip pcs in catt.Poly.OfType<PolyChunkStrip>())
							foreach (PolyChunkStrip.Strip s in pcs.Strips)
								plist.AddRange(s.Indexes);
						polys[i] = plist.ToArray();
						break;
				}
			NJS_MOTION mtn = new NJS_MOTION() { Name = info.Name, Frames = info.Frames, InterpolationMode = info.InterpolationMode, ModelParts = nodecnt };
			foreach ((int frame, NJS_OBJECT mdl) in mdls)
			{
				NJS_OBJECT[] nodes2 = mdl.GetObjects().Where(a => a.Morph).ToArray();
				if (nodes2.Length != nodecnt)
				{
					Console.WriteLine("Warning: Model {0} skeleton does not match first file!", frame);
					if (nodes2.Length > mtn.ModelParts)
						mtn.ModelParts = nodes2.Length;
				}
				for (int i = 0; i < nodes2.Length; i++)
					switch (nodes2[i].Attach)
					{
						case BasicAttach batt:
							if (batt.Vertex.Length != vcnt[i])
								Console.WriteLine("Warning: Model {0} node {1} vertex count does not match first file!", frame, i);
							AnimModelData amd;
							if (!mtn.Models.TryGetValue(i, out amd))
							{
								amd = new AnimModelData() { VertexName = mtn.MdataName + "_vtx_" + i, NormalName = mtn.MdataName + "_nor_" + i };
								mtn.Models[i] = amd;
							}
							amd.Vertex[frame] = batt.Vertex;
							if (batt.Normal != null)
								amd.Normal[frame] = batt.Normal;
							List<ushort> plist = new List<ushort>();
							foreach (NJS_MESHSET mesh in batt.Mesh)
								foreach (Poly p in mesh.Poly)
									plist.AddRange(p.Indexes);
							if (!polys[i].SequenceEqual(plist))
								Console.WriteLine("Warning: Model {0} node {1} poly data does not match first file!", frame, i);
							break;
						case ChunkAttach catt:
							if (catt.Vertex != null)
							{
								if (catt.Vertex.Sum(a => a.VertexCount) != vcnt[i])
									Console.WriteLine("Warning: Model {0} node {1} vertex count does not match first file!", frame, i);
								if (!mtn.Models.TryGetValue(i, out amd))
								{
									amd = new AnimModelData() { VertexName = mtn.MdataName + "_vtx_" + i, NormalName = mtn.MdataName + "_nor_" + i };
									mtn.Models[i] = amd;
								}
								amd.Vertex[frame] = catt.Vertex.SelectMany(a => a.Vertices).ToArray();
								if (catt.Vertex.All(a => a.Normals != null))
									amd.Normal[frame] = catt.Vertex.SelectMany(a => a.Normals).ToArray();
							}
							plist = new List<ushort>();
							foreach (PolyChunkStrip pcs in catt.Poly.OfType<PolyChunkStrip>())
								foreach (PolyChunkStrip.Strip s in pcs.Strips)
									plist.AddRange(s.Indexes);
							polys[i] = plist.ToArray();
							break;
					}
			}
			foreach ((int _, AnimModelData amd) in mtn.Models)
			{
				amd.VertexItemName = Enumerable.Range(0, amd.Vertex.Count).Select(a => amd.VertexName + "_" + a).ToArray();
				if (amd.Normal.Count > 0)
					amd.NormalItemName = Enumerable.Range(0, amd.Normal.Count).Select(a => amd.NormalName + "_" + a).ToArray();
			}
			mtn.Save(Path.Combine(path, $"{mtnfn}.saanim"));
		}
	}

	class MTNInfo
	{
		public ModelFormat ModelFormat { get; set; }
		public string Name { get; set; }
		public int Frames { get; set; }
		public InterpolationMode InterpolationMode { get; set; }
	}
}
