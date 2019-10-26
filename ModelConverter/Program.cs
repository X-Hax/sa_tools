using SonicRetro.SAModel;
using SonicRetro.SAModel.SAEditorCommon.ModelConversion;
using System;
using System.Linq;

namespace ModelConverter
{
	static class Program
	{
		static void Main(string[] args)
		{
			string filename;
			if (args.Length > 0)
			{
				filename = args[0];
				Console.WriteLine("File: {0}", filename);
			}
			else
			{
				Console.Write("File: ");
				filename = Console.ReadLine();
			}
			ModelFile model = new ModelFile(filename);
			switch (model.Format)
			{
				case ModelFormat.Basic:
					foreach (NJS_OBJECT obj in model.Model.GetObjects().Where(obj => obj.Attach is BasicAttach))
						obj.Attach = obj.Attach.ToChunk();
					ModelFile.CreateFile(System.IO.Path.ChangeExtension(filename, "sa2mdl"), model.Model, null, null, null, null, ModelFormat.Chunk);
					break;
				case ModelFormat.Chunk:
					foreach (NJS_OBJECT obj in model.Model.GetObjects().Where(obj => obj.Attach is ChunkAttach))
						obj.Attach = obj.Attach.ToBasic();
					ModelFile.CreateFile(System.IO.Path.ChangeExtension(filename, "sa1mdl"), model.Model, null, null, null, null, ModelFormat.Basic);
					break;
			}
		}
	}
}