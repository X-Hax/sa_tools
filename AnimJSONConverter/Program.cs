using Newtonsoft.Json;
using SonicRetro.SAModel;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace AnimJSONConverter
{
	static class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Thread t = new Thread((ThreadStart)(() =>
				{
					using (OpenFileDialog ofd = new OpenFileDialog()
					{
						Filter = "All Animation Files|*.saanim;*.json|Javascript Object Notation|*.json|Sonic Adventure Animation|*.saanim|All Files|*.*",
						Multiselect = true
					})
					{
						if (ofd.ShowDialog() == DialogResult.OK)
						{
							Convert(ofd.FileNames);
						}
					}
				}));
				t.SetApartmentState(ApartmentState.STA);
				t.Start();
			}
			else
			{
				Convert(args);
			}
		}

		private static void Convert(string[] args)
		{
			foreach (string filename in args)
			{
				Console.WriteLine(filename);
				JsonSerializer js = new JsonSerializer() { Culture = System.Globalization.CultureInfo.InvariantCulture };
				switch (Path.GetExtension(filename).ToLowerInvariant())
				{
					case ".saanim":
						using (TextWriter tw = File.CreateText(Path.ChangeExtension(filename, ".json")))
						using (JsonTextWriter jtw = new JsonTextWriter(tw) { Formatting = Formatting.Indented })
							js.Serialize(jtw, NJS_MOTION.Load(filename));
						break;
					case ".json":
						using (TextReader tr = File.OpenText(filename))
						using (JsonTextReader jtr = new JsonTextReader(tr))
							js.Deserialize<NJS_MOTION>(jtr).Save(Path.ChangeExtension(filename, ".saanim"));
						break;
					default:
						Console.WriteLine("Unknown file type.");
						return;
				}
			}
		}
	}
}
