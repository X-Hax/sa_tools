using SAModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SAModel.DataToolbox
{
	public static partial class ObjScan
	{
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

		static uint FindModel(string filename)
		{
			CurrentStep++;
			CurrentScanData = "models similar to '" + Path.GetFileName(filename) + "'";
			ByteConverter.BigEndian = BigEndian;
			// Basic only for now
			uint result = 0;
			ModelFile modelFile = new ModelFile(filename);
			ModelFormat modelfmt = modelFile.Format;
			if (modelfmt == ModelFormat.Basic && BasicModelsAreDX)
				modelfmt = ModelFormat.BasicDX;
			Console.WriteLine("Model format: {0}", modelfmt);
			NJS_OBJECT originalmodel = modelFile.Model;
			string model_extension = ".sa1mdl";
			if (!SingleOutputFolder)
				Directory.CreateDirectory(Path.Combine(OutputFolder, "models"));
			for (uint address = StartAddress; address < EndAddress; address += 1)
			{
				if (CancelScan)
				{
					break;
				}
				CurrentAddress = address;
				string fileOutputPath = Path.Combine(OutputFolder, "models", address.ToString("X8"));
				if (SingleOutputFolder)
					fileOutputPath = Path.Combine(OutputFolder, address.ToString("X8"));
				try
				{
					if (!CheckModel(address, 0, modelfmt))
					{
						//Console.WriteLine("Not found: {0}", address.ToString("X"));
						continue;
					}
					NJS_OBJECT mdl = new NJS_OBJECT(datafile, (int)address, ImageBase, modelfmt, new Dictionary<int, Attach>());
					if (!CompareModels(originalmodel, mdl)) 
						continue;
					NJS_OBJECT[] children1 = originalmodel.Children.ToArray();
					NJS_OBJECT[] children2 = mdl.Children.ToArray();
					if (children1.Length != children2.Length) 
						continue;
					for (int k = 0; k < children1.Length; k++)
					{
						if (!CompareModels(children1[k], children2[k])) 
							continue;
					}
					ModelFile.CreateFile(fileOutputPath + model_extension, mdl, null, null, null, null, modelfmt, NoMeta);
					Console.WriteLine("Model at {0} seems to match!", address.ToString("X"));
					addresslist.Add(address, "NJS_OBJECT");
					FoundBasicModels++;
				}
				catch (Exception)
				{
					continue;
				}
			}
			return result;
		}
	}
}
