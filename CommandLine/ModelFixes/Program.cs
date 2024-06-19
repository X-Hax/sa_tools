using System;
using System.Collections.Generic;
using System.IO;
using SAModel;

//this program fixes the wrong flags, rotation and scaling values from a new model based on an existing one (usually legacy).

namespace ModelFixes
{
	static class Program
	{
		static void Main(string[] args)
		{
			Queue<string> argq = new(args);
			string newmdlFileName;

			if (argq.Count > 0)
			{
				newmdlFileName = argq.Dequeue();
				Console.WriteLine("New Model File: {0}", newmdlFileName);
			}
			else
			{
				Console.Write("New Model File: ");
				newmdlFileName = Console.ReadLine().Trim('"');
			}

			ModelFile newModel = new(newmdlFileName);
			string newMdlNameNoExt = Path.GetFileNameWithoutExtension(newmdlFileName);
			string ext = Path.GetExtension(newmdlFileName);
			NJS_OBJECT[] newmdlObjects = newModel.Model.GetObjects();
			string legacymdlfilename;
			if (argq.Count > 0)
			{
				legacymdlfilename = argq.Dequeue();
				Console.WriteLine("Legacy Model File: {0}", legacymdlfilename);
			}
			else
			{
				Console.Write("Legacy Model File: ");
				legacymdlfilename = Console.ReadLine().Trim('"');
			}
			ModelFile legacyModel = new(legacymdlfilename);
			NJS_OBJECT[] legacymdlObjects = legacyModel.Model.GetObjects();

			if (newModel.Format != legacyModel.Format)
				Console.WriteLine("Format mismatch between files! Most data won't be fixed.");
			if (newmdlObjects.Length != legacymdlObjects.Length)
				Console.WriteLine("Models have different structures, the game may crash.");

			for (int i = 0; i < Math.Min(newmdlObjects.Length, legacymdlObjects.Length); i++)
			{
				newmdlObjects[i].Rotation = legacymdlObjects[i].Rotation;
				newmdlObjects[i].Scale = legacymdlObjects[i].Scale;
				newmdlObjects[i].SkipChildren = legacymdlObjects[i].SkipChildren;
				newmdlObjects[i].SkipDraw = legacymdlObjects[i].SkipDraw;
				newmdlObjects[i].IgnorePosition = legacymdlObjects[i].IgnorePosition;
				newmdlObjects[i].IgnoreRotation = legacymdlObjects[i].IgnoreRotation;
				newmdlObjects[i].IgnoreScale = legacymdlObjects[i].IgnoreScale;
			}


			newModel.SaveToFile(newMdlNameNoExt + "_fixed" + ext);

		}
	}
}
