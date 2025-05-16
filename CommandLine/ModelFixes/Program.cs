using System;
using System.Collections.Generic;
using System.IO;
using SAModel;

// This program fixes the wrong flags, rotation and scaling values from a new model based on an existing one (usually legacy).

namespace ModelFixes
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			Queue<string> fileQueue = new(args);
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

			ModelFile newModel = new(newModelFile);

			var newModelObjects = newModel.Model.GetObjects();
			string legacyModelFile;
			if (fileQueue.Count > 0)
			{
				legacyModelFile = fileQueue.Dequeue();
				Console.WriteLine("Legacy Model File: {0}", legacyModelFile);
			}
			else
			{
				Console.Write("Legacy Model File: ");
				legacyModelFile = Console.ReadLine().Trim('"');
			}

			ModelFile legacyModel = new(legacyModelFile);
			var legacyModelObjects = legacyModel.Model.GetObjects();

			if (newModel.Format != legacyModel.Format)
			{
				Console.WriteLine("Format mismatch between files! Most data won't be fixed.");
			}

			if (newModelObjects.Length != legacyModelObjects.Length)
			{
				Console.WriteLine("Models have different structures, the game may crash.");
			}

			for (var i = 0; i < Math.Min(newModelObjects.Length, legacyModelObjects.Length); i++)
			{
				newModelObjects[i].Rotation = legacyModelObjects[i].Rotation;
				newModelObjects[i].Scale = legacyModelObjects[i].Scale;
				newModelObjects[i].SkipChildren = legacyModelObjects[i].SkipChildren;
				newModelObjects[i].SkipDraw = legacyModelObjects[i].SkipDraw;
				newModelObjects[i].IgnorePosition = legacyModelObjects[i].IgnorePosition;
				newModelObjects[i].IgnoreRotation = legacyModelObjects[i].IgnoreRotation;
				newModelObjects[i].IgnoreScale = legacyModelObjects[i].IgnoreScale;
			}

			var newModelFileWithoutExtension = Path.GetFileNameWithoutExtension(newModelFile);
			var extension = Path.GetExtension(newModelFile);

			newModel.SaveToFile(newModelFileWithoutExtension + "_fixed" + extension);
		}
	}
}
