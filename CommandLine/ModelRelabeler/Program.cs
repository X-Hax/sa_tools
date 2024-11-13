using System;
using System.Collections.Generic;
using SAModel;

namespace ModelRelabeler
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			Queue<string> fileQueue = new Queue<string>(args);
			string newModelFile;

			if (fileQueue.Count > 0)
			{
				newModelFile = fileQueue.Dequeue();
				Console.WriteLine("New Model File: {0}", newModelFile);
			}
			else
			{
				Console.Write("New Model File: ");
				newModelFile = Console.ReadLine().Trim('"');
			}

			string oldModelFile;
			if (fileQueue.Count > 0)
			{
				oldModelFile = fileQueue.Dequeue();
				Console.WriteLine("Old Model File: {0}", oldModelFile);
			}
			else
			{
				Console.Write("Old Model File: ");
				oldModelFile = Console.ReadLine().Trim('"');
			}

			var model = new ModelFile(newModelFile);
			var modelObjects = model.Model.GetObjects();

			var oldModel = new ModelFile(oldModelFile);
			var oldModelObjects = oldModel.Model.GetObjects();

			if (model.Format != oldModel.Format)
			{
				Console.WriteLine("Format mismatch between files! Most data will be unable to be relabeled.");
			}

			if (modelObjects.Length != oldModelObjects.Length)
			{
				Console.WriteLine("Models have different structures, the game may crash.");
			}

			for (var i = 0; i < Math.Min(modelObjects.Length, oldModelObjects.Length); i++)
			{
				modelObjects[i].Name = oldModelObjects[i].Name;

				if (modelObjects[i].Attach == null || oldModelObjects[i].Attach == null)
				{
					continue;
				}

				modelObjects[i].Attach.Name = oldModelObjects[i].Attach.Name;

				switch (modelObjects[i].Attach)
				{
					case BasicAttach when oldModelObjects[i].Attach is BasicAttach:
					{
						var attach = (BasicAttach)modelObjects[i].Attach;
						var oldAttach = (BasicAttach)oldModelObjects[i].Attach;

						attach.VertexName = oldAttach.VertexName;

						if (oldAttach.NormalName != null)
						{
							attach.NormalName = oldAttach.NormalName;
						}

						if (oldAttach.MaterialName != null)
						{
							attach.MaterialName = oldAttach.MaterialName;
						}

						attach.MeshName = oldAttach.MeshName;

						for (var j = 0; j < Math.Min(attach.Mesh.Count, oldAttach.Mesh.Count); j++)
						{
							attach.Mesh[j].PolyName = oldAttach.Mesh[j].PolyName;

							if (oldAttach.Mesh[j].PolyNormalName != null)
							{
								attach.Mesh[j].PolyNormalName = oldAttach.Mesh[j].PolyNormalName;
							}

							if (oldAttach.Mesh[j].UVName != null)
							{
								attach.Mesh[j].UVName = oldAttach.Mesh[j].UVName;
							}

							if (oldAttach.Mesh[j].VColorName != null)
							{
								attach.Mesh[j].VColorName = oldAttach.Mesh[j].VColorName;
							}
						}

						break;
					}
					case ChunkAttach when oldModelObjects[i].Attach is ChunkAttach:
					{
						var attach = (ChunkAttach)modelObjects[i].Attach;
						var oldAttach = (ChunkAttach)oldModelObjects[i].Attach;

						if (oldAttach.VertexName != null)
						{
							attach.VertexName = oldAttach.VertexName;
						}

						if (oldAttach.PolyName != null)
						{
							attach.PolyName = oldAttach.PolyName;
						}

						break;
					}
				}
			}

			model.SaveToFile(oldModelFile);
		}
	}
}
