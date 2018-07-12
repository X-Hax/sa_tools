using Newtonsoft.Json;
using SonicRetro.SAModel;
using System;
using System.IO;

namespace AnimJSONConverter
{
	static class Program
	{
		static void Main(string[] args)
		{
			string filename;
			Console.Write("File: ");
			if (args.Length > 0)
				Console.WriteLine(filename = args[0]);
			else
				filename = Console.ReadLine().Trim('"');
			JsonSerializer js = new JsonSerializer();
			switch (Path.GetExtension(filename).ToLowerInvariant())
			{
				case ".saanim":
					using (TextWriter tw = File.CreateText(Path.ChangeExtension(filename, ".json")))
					using (JsonTextWriter jtw = new JsonTextWriter(tw) { Formatting = Formatting.Indented })
						js.Serialize(jtw, Animation.Load(filename));
					break;
				case ".json":
					using (TextReader tr = File.OpenText(filename))
					using (JsonTextReader jtr = new JsonTextReader(tr))
						js.Deserialize<Animation>(jtr).Save(Path.ChangeExtension(filename, ".saanim"));
						break;
				default:
					Console.WriteLine("Unknown file type.");
					return;
			}
		}
	}
}
