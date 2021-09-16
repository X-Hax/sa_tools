using SAModel;
using SAModel.SAEditorCommon.ModelConversion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LevelConverter
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
			LandTable level = LandTable.LoadFromFile(filename);
			Dictionary<string, Attach> visitedAttaches = new Dictionary<string, Attach>();
			switch (level.Format)
			{
				case LandTableFormat.SA1:
					{
						List<COL> newcollist = new List<COL>();
						foreach (COL col in level.COL.Where((col) => col.Model != null && col.Model.Attach != null))
						{
							if ((col.SurfaceFlags & SA1SurfaceFlags.Visible) == SA1SurfaceFlags.Visible)
							{
								COL newcol = new COL() { Bounds = col.Bounds };
								newcol.SurfaceFlags = SA1SurfaceFlags.Visible;
								newcol.Model = new NJS_OBJECT() { Name = col.Model.Name + "_cnk" };
								newcol.Model.Position = col.Model.Position;
								newcol.Model.Rotation = col.Model.Rotation;
								newcol.Model.Scale = col.Model.Scale;
								BasicAttach basatt = (BasicAttach)col.Model.Attach;
								string newname = basatt.Name + "_cnk";
								if (visitedAttaches.ContainsKey(newname))
									newcol.Model.Attach = visitedAttaches[newname];
								else
								{
									ChunkAttach cnkatt = basatt.ToChunk();
									visitedAttaches[newname] = cnkatt;
									newcol.Model.Attach = cnkatt;
								}
								newcollist.Add(newcol);
							}
							if ((col.SurfaceFlags & ~SA1SurfaceFlags.Visible) != 0)
							{
								col.SurfaceFlags &= ~SA1SurfaceFlags.Visible;
								newcollist.Add(col);
							}
						}
						level.COL = newcollist;
					}
					level.Anim = new List<GeoAnimData>();
					level.SaveToFile(System.IO.Path.ChangeExtension(filename, "sa2lvl"), LandTableFormat.SA2);
					break;
				case LandTableFormat.SA2:
					foreach (COL col in level.COL.Where((col) => col.Model != null && col.Model.Attach is ChunkAttach))
						col.Model.Attach = col.Model.Attach.ToBasic();
					level.Anim = new List<GeoAnimData>();
					level.Attributes = SA1LandtableAttributes.LoadTextureFile; // set LandTable to use PVM/GVM
					level.SaveToFile(System.IO.Path.ChangeExtension(filename, "sa1lvl"), LandTableFormat.SA1);
					break;
			}
		}
	}
}