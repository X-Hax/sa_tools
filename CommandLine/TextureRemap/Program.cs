using SAModel;
using System;
using System.Linq;

namespace TextureRemap
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			string filename;

			if (args.Length > 0)
			{
				Console.WriteLine("File: {0}", filename = args[0]);
			}
			else
			{
				Console.Write("File: ");
				filename = Console.ReadLine();
			}

			string[] mappings;

			if (args.Length > 1)
			{
				mappings = args.Skip(1).ToArray();
				Console.WriteLine("Remaps: {0}", string.Join(" ", mappings));
			}
			else
			{
				Console.WriteLine("Enter texture mappings (src,dst [src,dst ...]):");
				mappings = Console.ReadLine().Split(' ');
			}

			var maps = mappings
				.Select(str => str.Split(','))
				.ToDictionary(b => ushort.Parse(b[0]), b => ushort.Parse(b[1]));

			var model = new ModelFile(filename);

			var attaches = model.Model.GetObjects()
				.Where(a => a.Attach != null)
				.Select(a => a.Attach);

			foreach (var attach in attaches)
			{
				switch (attach)
				{
					case BasicAttach basicAttach:
					{
						if (basicAttach.Material == null) { continue; }

						foreach (NJS_MATERIAL mat in basicAttach.Material)
						{
							if (maps.TryGetValue((ushort)mat.TextureID, out var map))
							{
								mat.TextureID = map;
							}
						}

						break;
					}
					case ChunkAttach chunkAttach:
					{
						if (chunkAttach.Poly == null) { continue; }

						foreach (PolyChunkTinyTextureID tex in chunkAttach.Poly.OfType<PolyChunkTinyTextureID>())
						{
							if (maps.TryGetValue(tex.TextureID, out var map))
							{
								tex.TextureID = map;
							}
						}

						break;
					}
				}
			}

			model.SaveToFile(filename);
		}
	}
}
