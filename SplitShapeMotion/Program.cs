using SA_Tools;
using SonicRetro.SAModel;
using System;
using System.IO;
using System.Linq;

namespace SplitShapeMotion
{
	class Program
	{
		static void Main(string[] args)
		{
			string basemdlname;
			if (args.Length > 0)
				basemdlname = args[0];
			else
			{
				Console.Write("Base Model: ");
				basemdlname = Console.ReadLine().Trim('"');
			}
			string fext = Path.GetExtension(basemdlname);
			ModelFile modelFile = new ModelFile(basemdlname);
			ModelFormat fmt = modelFile.Format;
			NJS_OBJECT basemdl = modelFile.Model;
			string mtnname;
			if (args.Length > 1)
				mtnname = args[1];
			else
			{
				Console.Write("Motion: ");
				mtnname = Console.ReadLine().Trim('"');
			}
			NJS_MOTION mtn = NJS_MOTION.Load(mtnname, basemdl.CountMorph());
			string outdir;
			if (args.Length > 2)
				outdir = args[2];
			else
				outdir = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(mtnname)), Path.GetFileNameWithoutExtension(mtnname));
			Directory.CreateDirectory(outdir);
			MTNInfo inf = new MTNInfo() { ModelFormat = fmt, Name = mtn.Name, Frames = mtn.Frames, InterpolationMode = mtn.InterpolationMode };
			IniSerializer.Serialize(inf, Path.Combine(outdir, Path.ChangeExtension(Path.GetFileName(mtnname), ".ini")));
			NJS_OBJECT[] objs = basemdl.GetObjects().Where(a => a.Morph).ToArray();
			for (int frame = 0; frame < mtn.Frames; frame++)
			{
				if (!mtn.Models.Any(a => a.Value.Vertex.ContainsKey(frame))) continue;
				foreach ((int idx, AnimModelData amd) in mtn.Models)
					if (amd.Vertex.ContainsKey(frame))
					{
						Vertex[] verts = amd.Vertex[frame];
						amd.Normal.TryGetValue(frame, out Vertex[] norms);
						switch (objs[idx].Attach)
						{
							case BasicAttach batt:
								for (int i = 0; i < Math.Min(verts.Length, batt.Vertex.Length); i++)
								{
									batt.Vertex[i] = verts[i];
									if (norms != null && batt.Normal != null)
										batt.Normal[i] = norms[i];
								}
								break;
							case ChunkAttach catt:
								if (catt.Vertex == null) continue;
								int vcnt = catt.Vertex.Sum(a => a.VertexCount);
								int ci = 0;
								int vi = 0;
								for (int i = 0; i < Math.Min(verts.Length, vcnt); i++)
								{
									catt.Vertex[ci].Vertices[vi] = verts[i];
									if (norms != null && catt.Vertex[ci].Normals != null)
										catt.Vertex[ci].Normals[vi] = norms[i];
									if (++vi >= catt.Vertex[ci].VertexCount)
									{
										++ci;
										vi = 0;
									}
								}
								break;
						}
					}
				ModelFile.CreateFile(Path.Combine(outdir, $"{frame}{fext}"), basemdl, null, null, null, null, fmt);
			}
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
