using SAModel;
using SAModel.GC;
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
			LandTableFormat outfmt;
			if (args.Length > 1)
				outfmt = Enum.Parse<LandTableFormat>(args[1], true);
			else
			{
				Console.Write("Format: ");
				outfmt = Enum.Parse<LandTableFormat>(Console.ReadLine(), true);
			}
			LandTable level = LandTable.LoadFromFile(filename);
			if (level.Format == outfmt)
			{
				Console.WriteLine("Input and output formats identical, no conversion performed.");
				return;
			}
			Dictionary<string, Attach> visitedAttaches = new Dictionary<string, Attach>();
			switch (level.Format)
			{
				case LandTableFormat.SA1:
					{
						List<COL> newcollist = new List<COL>();
						foreach (COL col in level.COL.Where((col) => col.Model != null && col.Model.Attach != null))
						{
							//fix flags differences
							if ((col.SurfaceFlags & SA1SurfaceFlags.UseSkyDrawDistance) == SA1SurfaceFlags.UseSkyDrawDistance)
							{
								col.SurfaceFlags &= ~SA1SurfaceFlags.UseSkyDrawDistance;
							}

							if ((col.SurfaceFlags & SA1SurfaceFlags.Diggable) == SA1SurfaceFlags.Diggable)
							{
								col.SurfaceFlags &= ~SA1SurfaceFlags.Diggable;
								col.SurfaceFlags |= (SA1SurfaceFlags)SA2SurfaceFlags.Diggable;
							}

							if ((col.SurfaceFlags & SA1SurfaceFlags.Stairs) == SA1SurfaceFlags.Stairs)
							{
								col.SurfaceFlags &= ~SA1SurfaceFlags.Stairs;
								col.SurfaceFlags |= (SA1SurfaceFlags)SA2SurfaceFlags.Stairs;
							}

							if ((col.SurfaceFlags & SA1SurfaceFlags.Waterfall) == SA1SurfaceFlags.Waterfall)
							{
								col.SurfaceFlags &= ~SA1SurfaceFlags.Waterfall;
							}

							if ((col.SurfaceFlags & SA1SurfaceFlags.Hurt) == SA1SurfaceFlags.Hurt)
							{
								col.SurfaceFlags &= ~SA1SurfaceFlags.Hurt;
								col.SurfaceFlags |= (SA1SurfaceFlags)SA2SurfaceFlags.Hurt;
							}

							if ((col.SurfaceFlags & SA1SurfaceFlags.Unclimbable) == SA1SurfaceFlags.Unclimbable)
							{
								col.SurfaceFlags &= ~SA1SurfaceFlags.Unclimbable;
								col.SurfaceFlags |= (SA1SurfaceFlags)SA2SurfaceFlags.Unclimbable;
							}

							if ((col.SurfaceFlags & SA1SurfaceFlags.CannotLand) == SA1SurfaceFlags.CannotLand)
							{
								col.SurfaceFlags &= ~SA1SurfaceFlags.CannotLand;
								col.SurfaceFlags |= (SA1SurfaceFlags)SA2SurfaceFlags.CannotLand;
							}

							if ((col.SurfaceFlags & SA1SurfaceFlags.LowDepth) == SA1SurfaceFlags.LowDepth)
							{
								col.SurfaceFlags &= ~SA1SurfaceFlags.LowDepth;
							}

							if ((col.SurfaceFlags & SA1SurfaceFlags.Visible) == SA1SurfaceFlags.Visible)
							{
								COL newcol = new COL() { Bounds = col.Bounds };
								newcol.SurfaceFlags = SA1SurfaceFlags.Visible;
								newcol.Model = new NJS_OBJECT() { Name = col.Model.Name + (outfmt == LandTableFormat.SA2 ? "_cnk" : "_gc") };
								newcol.Model.Position = col.Model.Position;
								newcol.Model.Rotation = col.Model.Rotation;
								newcol.Model.Scale = col.Model.Scale;
								BasicAttach basatt = (BasicAttach)col.Model.Attach;
								string newname = basatt.Name + (outfmt == LandTableFormat.SA2 ? "_cnk" : "_gc");
								if (visitedAttaches.ContainsKey(newname))
									newcol.Model.Attach = visitedAttaches[newname];
								else
								{
									Attach newatt;
									if (outfmt == LandTableFormat.SA2)
										newatt = basatt.ToChunk();
									else
										throw new Exception("SA2B conversion not supported yet!");
									visitedAttaches[newname] = newatt;
									newcol.Model.Attach = newatt;
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
					level.SaveToFile(System.IO.Path.ChangeExtension(filename, outfmt == LandTableFormat.SA2 ? "sa2lvl" : "sa2blvl"), outfmt);
					break;
				case LandTableFormat.SA2:
				case LandTableFormat.SA2B:

					switch (outfmt)
					{
						case LandTableFormat.SA1:
						case LandTableFormat.SADX:
							foreach (COL col in level.COL.Where((col) => col.Model?.Attach != null))
							{
								if (visitedAttaches.ContainsKey(col.Model.Attach.Name))
									col.Model.Attach = visitedAttaches[col.Model.Attach.Name];
								else
								{
									col.Model.Attach = col.Model.Attach.ToBasic();
									visitedAttaches[col.Model.Attach.Name] = col.Model.Attach;
								}
							}

							foreach (COL col in level.COL.Where((col) => col.Model?.Attach != null))
							{
								//fix flags differences
								if ((col.SurfaceFlags & SA1SurfaceFlags.Diggable) == SA1SurfaceFlags.Diggable)
								{
									col.SurfaceFlags &= ~SA1SurfaceFlags.Diggable;
									col.SurfaceFlags |= SA1SurfaceFlags.Stairs;
								}

								if ((col.SurfaceFlags & SA1SurfaceFlags.UseSkyDrawDistance) == SA1SurfaceFlags.UseSkyDrawDistance)
								{
									col.SurfaceFlags &= ~SA1SurfaceFlags.UseSkyDrawDistance;
									col.SurfaceFlags |= SA1SurfaceFlags.Diggable;
								}

								if ((col.SurfaceFlags & SA1SurfaceFlags.Unclimbable) == SA1SurfaceFlags.Unclimbable)
								{
									col.SurfaceFlags &= ~SA1SurfaceFlags.Unclimbable;
									col.SurfaceFlags |= SA1SurfaceFlags.CannotLand;
								}

								if ((col.SurfaceFlags & SA1SurfaceFlags.IncreasedAcceleration) == SA1SurfaceFlags.IncreasedAcceleration)
								{
									col.SurfaceFlags &= ~SA1SurfaceFlags.IncreasedAcceleration;
									col.SurfaceFlags |= SA1SurfaceFlags.Unclimbable;
								}

								if ((col.SurfaceFlags & SA1SurfaceFlags.Waterfall) == SA1SurfaceFlags.Waterfall)
								{
									col.SurfaceFlags &= ~SA1SurfaceFlags.Waterfall;
									col.SurfaceFlags |= SA1SurfaceFlags.Hurt;
								}
							}

							level.Anim = new List<GeoAnimData>();
							level.Attributes = SA1LandtableAttributes.LoadTextureFile; // set LandTable to use PVM/GVM
							level.SaveToFile(System.IO.Path.ChangeExtension(filename, "sa1lvl"), LandTableFormat.SA1);
							break;
						case LandTableFormat.SA2:
							foreach (COL col in level.COL.Where((col) => col.Model?.Attach is GCAttach))
								col.Model.Attach = col.Model.Attach.ToChunk();
							level.SaveToFile(System.IO.Path.ChangeExtension(filename, "sa2lvl"), LandTableFormat.SA2);
							break;

						case LandTableFormat.SA2B:
							throw new Exception("SA2B conversion not supported yet!");
					}
					break;
			}
		}
	}
}