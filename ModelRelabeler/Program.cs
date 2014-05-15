using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SonicRetro.SAModel;
using ModelObject = SonicRetro.SAModel.Object;

namespace ModelRelabeler
{
	class Program
	{
		static void Main(string[] args)
		{
			Queue<string> argq = new Queue<string>(args);
			string mdlfilename;
			if (argq.Count > 0)
			{
				mdlfilename = argq.Dequeue();
				Console.WriteLine("Model File: {0}", mdlfilename);
			}
			else
			{
				Console.Write("Model File: ");
				mdlfilename = Console.ReadLine();
			}
			ModelFile model = new ModelFile(mdlfilename);
			ModelObject[] objects = model.Model.GetObjects();
			string repmdlfilename;
			if (argq.Count > 0)
			{
				repmdlfilename = argq.Dequeue();
				Console.WriteLine("Replacement Model File: {0}", repmdlfilename);
			}
			else
			{
				Console.Write("Replacement Model File: ");
				repmdlfilename = Console.ReadLine();
			}
			ModelFile repmodel = new ModelFile(repmdlfilename);
			ModelObject[] repobjects = repmodel.Model.GetObjects();
			if (model.Format != repmodel.Format)
				Console.WriteLine("Format mismatch between files! Most data will be unable to be relabeled.");
			if (objects.Length != repobjects.Length)
				Console.WriteLine("Models have different structures, the game may crash.");
			for (int i = 0; i < Math.Min(objects.Length, repobjects.Length); i++)
			{
				objects[i].Name = repobjects[i].Name;
				if (objects[i].Attach != null && repobjects[i].Attach != null)
				{
					objects[i].Attach.Name = repobjects[i].Attach.Name;
					if (objects[i].Attach is BasicAttach && repobjects[i].Attach is BasicAttach)
					{
						BasicAttach attach = (BasicAttach)objects[i].Attach;
						BasicAttach repattach = (BasicAttach)repobjects[i].Attach;
						attach.VertexName = repattach.VertexName;
						attach.NormalName = repattach.NormalName;
						attach.MaterialName = repattach.MaterialName;
						attach.MeshName = repattach.MeshName;
						for (int j = 0; j < Math.Min(attach.Mesh.Count, repattach.Mesh.Count); j++)
						{
							attach.Mesh[j].PolyName = repattach.Mesh[j].PolyName;
							attach.Mesh[j].PolyNormalName = repattach.Mesh[j].PolyNormalName;
							attach.Mesh[j].UVName = repattach.Mesh[j].UVName;
							attach.Mesh[j].VColorName = repattach.Mesh[j].VColorName;
						}
					}
				}
				else if (objects[i].Attach is ChunkAttach && repobjects[i].Attach is ChunkAttach)
				{
					ChunkAttach attach = (ChunkAttach)objects[i].Attach;
					ChunkAttach repattach = (ChunkAttach)objects[i].Attach;
					attach.VertexName = repattach.VertexName;
					attach.PolyName = repattach.PolyName;
				}
			}
			model.SaveToFile(mdlfilename);
		}
	}
}
