using SonicRetro.SAModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextureRemap
{
	static class Program
	{
		static void Main(string[] args)
		{
			string filename;
			if (args.Length > 0)
				Console.WriteLine("File: {0}", filename = args[0]);
			else
			{
				Console.Write("File: ");
				filename = Console.ReadLine();
			}
			ModelFile model = new ModelFile(filename);
			string[] mapstr;
			if (args.Length > 1)
				Console.WriteLine("Remaps: {0}", mapstr = args.Skip(1).ToArray());
			else
			{
				Console.WriteLine("Enter texture mappings (src,dst [src,dst ...]):");
				mapstr = Console.ReadLine().Split(' ');
			}
			Dictionary<ushort, ushort> maps = new Dictionary<ushort, ushort>();
			foreach (string str in mapstr)
			{
				string[] b = str.Split(',');
				maps.Add(ushort.Parse(b[0]), ushort.Parse(b[1]));
			}
			foreach (Attach att in model.Model.GetObjects().Where(a=>a.Attach!=null).Select(a=>a.Attach))
				switch (att)
				{
					case BasicAttach batt:
						if (batt.Material != null)
							foreach (NJS_MATERIAL mat in batt.Material)
								if (maps.ContainsKey((ushort)mat.TextureID))
									mat.TextureID = maps[(ushort)mat.TextureID];
						break;
					case ChunkAttach catt:
						if (catt.Poly != null)
							foreach (PolyChunkTinyTextureID tex in catt.Poly.OfType<PolyChunkTinyTextureID>())
								if (maps.ContainsKey(tex.TextureID))
									tex.TextureID = maps[tex.TextureID];
						break;
				}
			model.SaveToFile(filename);
		}
	}
}
