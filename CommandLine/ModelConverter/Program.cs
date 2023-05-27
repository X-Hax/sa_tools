using SAModel;
using SAModel.SAEditorCommon.ModelConversion;
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
				filename = Console.ReadLine().Trim('"');
			}
			ModelFormat outfmt;
			if (args.Length > 1)
				outfmt = Enum.Parse<ModelFormat>(args[1], true);
			else
			{
				Console.Write("Format: ");
				outfmt = Enum.Parse<ModelFormat>(Console.ReadLine(), true);
			}
			ModelFile model = new ModelFile(filename);
			foreach (NJS_OBJECT obj in model.Model.GetObjects().Where(obj => obj.Attach != null))
				switch (outfmt)
				{
					case ModelFormat.Basic:
					case ModelFormat.BasicDX:
						obj.Attach = obj.Attach.ToBasic();
						break;
					case ModelFormat.Chunk:
						obj.Attach = obj.Attach.ToChunk();
						break;
					default:
						throw new Exception($"Output format {outfmt} not supported!");
				}
			switch (outfmt)
			{
				case ModelFormat.Basic:
				case ModelFormat.BasicDX:
					filename = System.IO.Path.ChangeExtension(filename, "sa1mdl");
					break;
				case ModelFormat.Chunk:
					filename = System.IO.Path.ChangeExtension(filename, "sa2mdl");
					break;
				case ModelFormat.GC:
					filename = System.IO.Path.ChangeExtension(filename, "sa2bmdl");
					break;
				case ModelFormat.XJ:
					filename = System.IO.Path.ChangeExtension(filename, "xjmdl");
					break;
			}
			ModelFile.CreateFile(filename, model.Model, null, null, null, null, outfmt);
		}
	}
}