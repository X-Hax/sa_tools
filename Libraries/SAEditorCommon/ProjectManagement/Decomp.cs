using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;

namespace SAModel.SAEditorCommon.ProjectManagement
{
	public static class NJAExporter
	{
		public static void GenerateDup(string filename, string duppath)
		{
			string outpath = Path.Combine(Path.GetDirectoryName(filename), duppath);
			LandTable land = LandTable.LoadFromFile(filename);
			List<Attach> atts = new List<Attach>();
			List<NJS_MOTION> mots = new List<NJS_MOTION>();
			List<NJS_OBJECT> dupmodels = new List<NJS_OBJECT>();
			List<NJS_ACTION> dupactions = new List<NJS_ACTION>();
			foreach (COL col in land.COL)
			{
				if (CheckDuplicateObject(col.Model) && !dupmodels.Contains(col.Model))
				{
					dupmodels.Add(col.Model);
				}
			}
			// Ice Cap Act 4 reuses Act 2 models but has some additional dup models, so the counting has to be done for both
			if (Path.GetFileName(filename).ToLowerInvariant() == "landtable0801.c.sa1lvl")
			{
				LandTable act4 = LandTable.LoadFromFile(Path.Combine(Path.GetDirectoryName(filename), "landtable0803.c.sa1lvl"));
				foreach (COL col in act4.COL)
				{
					if (CheckDuplicateObject(col.Model) && !dupmodels.Contains(col.Model))
					{
						dupmodels.Add(col.Model);
					}
				}
			}
			// Make a list of duplicate models
			if (dupmodels.Count > 0)
			{
				TextWriter tw_nja = File.CreateText(Path.Combine(outpath, "dupmodel.dup"));
				foreach (NJS_OBJECT obj in dupmodels)
				{
					ObjectToNJA(obj, tw_nja);
				}
				tw_nja.Flush();
				tw_nja.Close();
			}
			if (land.Anim != null)
			{
				foreach (GeoAnimData geo in land.Anim)
				{
					NJS_ACTION act = new NJS_ACTION(geo.Model, geo.Animation);
					act.Name = geo.Animation.ActionName;
					if (CheckDuplicateAction(act) && !dupactions.Contains(act))
					{
						dupactions.Add(act);
					}
				}
				// Make a list of duplicate actions
				if (dupactions.Count > 0)
				{
					TextWriter twa_nja = File.CreateText(Path.Combine(outpath, "dupmotion.dup"));
					foreach (NJS_ACTION act in dupactions)
					{
						ActionToNJA(act, twa_nja);
					}
					twa_nja.Flush();
					twa_nja.Close();
				}
			}
		}

		static bool CheckDuplicateObject(NJS_OBJECT obj)
		{
			// This relies on the label of the OBJECT being different from that of the MODEL.
			// If the MODEL is being reused, usually the OBJECT label has a number at the end.
			// If the OBJECT has the number and the MODEL doesn't, it's most likely a duplicate.
			return obj.Name.Substring(obj.Name.Length - 3, 3) != obj.Attach.Name.Substring(obj.Attach.Name.Length - 3, 3);
		}

		static bool CheckDuplicateAction(NJS_ACTION act)
		{
			// Same as above but for ACTIONs using the same MOTION
			return act.Name.Substring(act.Name.Length - 3, 3) != act.Animation.Name.Substring(act.Animation.Name.Length - 3, 3);
		}

		static void ActionToNJA(NJS_ACTION act, TextWriter writer)
		{
			writer.WriteLine("ACTION {0}[]", act.Name);
			writer.WriteLine("START");
			writer.WriteLine("ObjectHead      {0},", act.Model.Name);
			writer.WriteLine("Motion          " + act.Animation.Name);
			writer.WriteLine("END" + Environment.NewLine);
		}

		static void ObjectToNJA(NJS_OBJECT obj, TextWriter writer)
		{
			writer.Write("OBJECT      ");
			writer.Write(obj.Name);
			writer.WriteLine("[]");
			writer.WriteLine("START");
			writer.WriteLine("EvalFlags ( 0x" + ((int)obj.GetFlags()).ToString("x8") + " ),");
			writer.WriteLine("Model       " + (obj.Attach != null ? obj.Attach.Name : "NULL") + ",");
			writer.WriteLine("OPosition  {0},", obj.Position.ToNJA());
			writer.WriteLine("OAngle     ( " + ((float)obj.Rotation.X / 182.044f).ToNJA() + ", " + ((float)obj.Rotation.Y / 182.044f).ToNJA() + ", " + ((float)obj.Rotation.Z / 182.044f).ToNJA() + " ),");
			writer.WriteLine("OScale     {0},", obj.Scale.ToNJA());
			writer.WriteLine("Child       " + (obj.Children.Count > 0 ? obj.Children[0].Name : "NULL") + ",");
			writer.WriteLine("Sibling     " + (obj.Sibling != null ? obj.Sibling.Name : "NULL") + ",");
			writer.WriteLine("END" + Environment.NewLine);
		}
	}
}