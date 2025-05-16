using SplitTools;
using SAModel;
using System;
using System.IO;
using System.Linq;

namespace SplitShapeMotion
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			string baseModelName;

			if (args.Length > 0)
			{
				baseModelName = args[0];
			}
			else
			{
				Console.Write("Base Model: ");
				baseModelName = Console.ReadLine().Trim('"');
			}

			var fileExtension = Path.GetExtension(baseModelName);

			string motionPath;
			if (args.Length > 1)
			{
				motionPath = args[1];
			}
			else
			{
				Console.Write("Motion: ");
				motionPath = Console.ReadLine().Trim('"');
			}

			string outputDirectory;
			if (args.Length > 2)
			{
				outputDirectory = args[2];
			}
			else
			{
				outputDirectory = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(motionPath)), Path.GetFileNameWithoutExtension(motionPath));
			}

			Directory.CreateDirectory(outputDirectory);

			var modelFile = new ModelFile(baseModelName);
			var baseModel = modelFile.Model;

			var motion = NJS_MOTION.Load(motionPath, baseModel.CountMorph());

			var inf = new MTNInfo
			{
				ModelFormat = modelFile.Format,
				Name = motion.Name,
				Frames = motion.Frames,
				InterpolationMode = motion.InterpolationMode
			};

			IniSerializer.Serialize(inf, Path.Combine(outputDirectory, Path.ChangeExtension(motionPath, ".ini")));

			var objs = baseModel.GetObjects().Where(a => a.Morph).ToArray();
			for (var frame = 0; frame < motion.Frames; frame++)
			{
				if (!motion.Models.Any(a => a.Value.Vertex.ContainsKey(frame)))
				{
					continue;
				}

				foreach ((int idx, AnimModelData amd) in motion.Models)
				{
					if (amd.Vertex.ContainsKey(frame))
					{
						Vertex[] verts = amd.Vertex[frame];
						amd.Normal.TryGetValue(frame, out Vertex[] norms);
						switch (objs[idx].Attach)
						{
							case BasicAttach basicAttach:
							{
								for (var i = 0; i < Math.Min(verts.Length, basicAttach.Vertex.Length); i++)
								{
									basicAttach.Vertex[i] = verts[i];

									if (norms != null && basicAttach.Normal != null)
									{
										basicAttach.Normal[i] = norms[i];
									}
								}

								break;
							}
							case ChunkAttach catt:
							{
								if (catt.Vertex == null)
								{
									continue;
								}

								int vertexCount = catt.Vertex.Sum(a => a.VertexCount);

								var ci = 0;
								var vi = 0;

								for (var i = 0; i < Math.Min(verts.Length, vertexCount); i++)
								{
									catt.Vertex[ci].Vertices[vi] = verts[i];

									if (norms != null && catt.Vertex[ci].Normals != null)
									{
										catt.Vertex[ci].Normals[vi] = norms[i];
									}

									if (++vi >= catt.Vertex[ci].VertexCount)
									{
										++ci;
										vi = 0;
									}
								}

								break;
							}
						}
					}
				}

				ModelFile.CreateFile(Path.Combine(outputDirectory, $"{frame}{fileExtension}"),
					baseModel,
					null,
					null,
					null,
					null,
					modelFile.Format);
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
