using SAModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace SplitTools
{
	// NJS_MESHSET labels
	public class LabelMESHSET
	{
		public string PolyName;
		public string UVName;
		public string PolyNormalName;
		public string VColorName;

		public LabelMESHSET() { }

		public LabelMESHSET(NJS_MESHSET mesh)
		{
			if (mesh.Poly != null)
				PolyName = mesh.PolyName;
			if (mesh.UV != null)
				UVName = mesh.UVName;
			if (mesh.PolyNormal != null)
				PolyNormalName = mesh.PolyNormalName;
			if (mesh.VColor != null)
				VColorName = mesh.VColorName;
		}

		public void Apply(NJS_MESHSET mesh)
		{
			if (mesh.Poly != null)
				mesh.PolyName = PolyName;
			if (mesh.UV != null)
				mesh.UVName = UVName;
			if (mesh.Poly != null)
				mesh.PolyNormalName = PolyNormalName;
			if (mesh.VColor != null)
				mesh.VColorName = VColorName;
		}
	}

	// NJS_OBJECT labels
	public class LabelOBJECT
	{
		public string ObjectName;
		public string AttachName;
		public string MeshsetName; // Also polys for chunk
		public List<LabelMESHSET> MeshsetItems;
		public string VertexName;
		public string NormalName;
		public string MaterialName;

		public LabelOBJECT() { }

		public LabelOBJECT(NJS_OBJECT obj)
		{
			ObjectName = obj.Name;
			if (obj.Attach != null)
			{
				AttachName = obj.Attach.Name;
				if (obj.Attach is BasicAttach batt)
				{
					if (batt.Vertex != null)
						VertexName = batt.VertexName;
					if (batt.Normal != null)
						NormalName = batt.NormalName;
					if (batt.Material != null)
						MaterialName = batt.MaterialName;
					if (batt.Mesh != null)
					{
						MeshsetName = batt.MeshName;
						MeshsetItems = new List<LabelMESHSET>();
						foreach (NJS_MESHSET mesh in batt.Mesh)
							MeshsetItems.Add(new LabelMESHSET(mesh));
					}
				}
				else if (obj.Attach is ChunkAttach catt)
				{
					if (catt.Vertex != null)
						VertexName = catt.VertexName;
					if (catt.Poly != null)
						NormalName = catt.PolyName;
				}
			}
		}

		public static List<LabelOBJECT> Load(string filename)
		{
			JsonSerializer js = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore, Culture = System.Globalization.CultureInfo.InvariantCulture };
			using TextReader tr = File.OpenText(filename);
			using JsonTextReader jtr = new JsonTextReader(tr);
			return js.Deserialize<List<LabelOBJECT>>(jtr);
		}

		public static void ImportLabels(NJS_OBJECT obj, List<LabelOBJECT> labels)
		{
			NJS_OBJECT[] objs = obj.GetObjects();
			for (int i = 0; i < objs.Length; i++)
				labels[i].Apply(objs[i]);
		}

		public static List<LabelOBJECT> ExportLabels(NJS_OBJECT obj)
		{
			List<LabelOBJECT> result = new List<LabelOBJECT>();
			NJS_OBJECT[] objs = obj.GetObjects();
			for (int i = 0; i < objs.Length; i++)
				result.Add(new LabelOBJECT(objs[i]));
			return result;
		}

		public static void Save(List<LabelOBJECT> list, string filename)
		{
			JsonSerializer js = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore, Culture = System.Globalization.CultureInfo.InvariantCulture };
			using (TextWriter tw = File.CreateText(filename))
			using (JsonTextWriter jtw = new JsonTextWriter(tw) { Formatting = Formatting.Indented })
				js.Serialize(jtw, list);
		}

		public void Apply(NJS_OBJECT obj)
		{
			obj.Name = ObjectName;
			if (obj.Attach != null)
			{
				obj.Attach.Name = AttachName;
				if (obj.Attach is BasicAttach batt)
				{
					if (batt.Vertex != null)
						batt.VertexName = VertexName;
					if (batt.Normal != null)
						batt.NormalName = NormalName;
					if (batt.Material != null)
						batt.MaterialName = MaterialName;
					if (batt.Mesh != null)
					{
						batt.MeshName = MeshsetName;
						int i = 0;
						foreach (NJS_MESHSET mesh in batt.Mesh)
						{
							MeshsetItems[i].Apply(mesh);
							i++;
						}

					}
				}
				else if (obj.Attach is ChunkAttach catt)
				{
					if (catt.Vertex != null)
						catt.VertexName = VertexName;
					if (catt.Poly != null)
						catt.PolyName = NormalName;
				}
			}
		}
	}

	// NJS_MOTION labels
	public class LabelMKEY
	{
		public string PositionName;
		public string RotationName;
		public string ScaleName;
		public string VertexName;
		public string VectorName;
		public string NormalName;
		public string TargetName;
		public string RollName;
		public string AngleName;
		public string ColorName;
		public string IntensityName;
		public string SpotName;
		public string PointName;
		public string QuaternionName;
		public List<string> VertexItems;
		public List<string> NormalItems;

		public LabelMKEY() { }

		public LabelMKEY(AnimModelData data)
		{
			if (!string.IsNullOrEmpty(data.PositionName))
				PositionName = data.PositionName;
			if (!string.IsNullOrEmpty(data.RotationName))
				RotationName = data.RotationName;
			if (!string.IsNullOrEmpty(data.ScaleName))
				ScaleName = data.ScaleName;
			if (!string.IsNullOrEmpty(data.VertexName))
				VertexName = data.VertexName;
			if (!string.IsNullOrEmpty(data.VectorName))
				VectorName = data.VectorName;
			if (!string.IsNullOrEmpty(data.NormalName))
				NormalName = data.NormalName;
			if (!string.IsNullOrEmpty(data.TargetName))
				TargetName = data.TargetName;
			if (!string.IsNullOrEmpty(data.RollName))
				RollName = data.RollName;
			if (!string.IsNullOrEmpty(data.AngleName))
				AngleName = data.AngleName;
			if (!string.IsNullOrEmpty(data.ColorName))
				ColorName = data.ColorName;
			if (!string.IsNullOrEmpty(data.IntensityName))
				IntensityName = data.IntensityName;
			if (!string.IsNullOrEmpty(data.SpotName))
				SpotName = data.SpotName;
			if (!string.IsNullOrEmpty(data.PointName))
				PointName = data.PointName;
			if (!string.IsNullOrEmpty(data.QuaternionName))
				QuaternionName = data.QuaternionName;
			if (data.VertexItemName != null)
				VertexItems = data.VertexItemName.ToList();
			if (data.NormalItemName != null)
				NormalItems = data.NormalItemName.ToList();
		}

		public void Apply(AnimModelData data)
		{
			if (!string.IsNullOrEmpty(data.PositionName))
				data.PositionName = PositionName;
			if (!string.IsNullOrEmpty(data.RotationName))
				data.RotationName = RotationName;
			if (!string.IsNullOrEmpty(data.ScaleName))
				data.ScaleName = ScaleName;
			if (!string.IsNullOrEmpty(data.VertexName))
				data.VertexName = VertexName;
			if (!string.IsNullOrEmpty(data.VectorName))
				data.VectorName = VectorName;
			if (!string.IsNullOrEmpty(data.NormalName))
				data.NormalName = NormalName;
			if (!string.IsNullOrEmpty(data.TargetName))
				data.TargetName = TargetName;
			if (!string.IsNullOrEmpty(data.RollName))
				data.RollName = RollName;
			if (!string.IsNullOrEmpty(data.AngleName))
				data.AngleName = AngleName;
			if (!string.IsNullOrEmpty(data.ColorName))
				data.ColorName = ColorName;
			if (!string.IsNullOrEmpty(data.IntensityName))
				data.IntensityName = IntensityName;
			if (!string.IsNullOrEmpty(data.SpotName))
				data.SpotName = SpotName;
			if (!string.IsNullOrEmpty(data.PointName))
				data.PointName = PointName;
			if (!string.IsNullOrEmpty(data.QuaternionName))
				data.QuaternionName = QuaternionName;
			if (data.VertexItemName != null)
				data.VertexItemName = VertexItems.ToArray();
			if (data.NormalItemName != null)
				data.NormalItemName = NormalItems.ToArray();
		}
	}

	// NJS_MOTION labels
	public class LabelMOTION
	{
		public string MotionName;
		public string MdataName;
		public Dictionary<int, LabelMKEY> MkeyNames;

		public LabelMOTION() { }

		public LabelMOTION(NJS_MOTION mot)
		{
			MotionName = mot.Name;
			MdataName = mot.MdataName;
			MkeyNames = new Dictionary<int, LabelMKEY>();
			foreach (KeyValuePair<int, AnimModelData> anm in mot.Models)
				MkeyNames.Add(anm.Key, new LabelMKEY(anm.Value));
		}

		public static LabelMOTION Load(string filename)
		{
			JsonSerializer js = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore, Culture = System.Globalization.CultureInfo.InvariantCulture };
			using TextReader tr = File.OpenText(filename);
			using JsonTextReader jtr = new JsonTextReader(tr);
			return js.Deserialize<LabelMOTION>(jtr);
		}

		public void Save(string filename)
		{
			JsonSerializer js = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore, Culture = System.Globalization.CultureInfo.InvariantCulture };
			using (TextWriter tw = File.CreateText(filename))
			using (JsonTextWriter jtw = new JsonTextWriter(tw) { Formatting = Formatting.Indented })
				js.Serialize(jtw, this);
		}

		public void Apply(NJS_MOTION mot)
		{
			mot.Name = MotionName;
			mot.MdataName = MdataName;
			foreach (KeyValuePair<int, AnimModelData> anm in mot.Models)
				MkeyNames[anm.Key].Apply(anm.Value);
		}
	}

	public class LabelACTION
	{
		public string ActionName;
		public LabelMOTION MotionNames;
		public LabelOBJECT ObjectNames;

		public LabelACTION() { }

		public LabelACTION(GeoAnimData anim)
		{
			ActionName = anim.Animation.ActionName;
			MotionNames = new LabelMOTION(anim.Animation);
			ObjectNames = new LabelOBJECT(anim.Model);
		}

		public LabelACTION(NJS_ACTION act)
		{
			ActionName = act.Name;
			MotionNames = new LabelMOTION(act.Animation);
			ObjectNames = new LabelOBJECT(act.Model);
		}

		public static LabelACTION Load(string filename)
		{
			JsonSerializer js = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore, Culture = System.Globalization.CultureInfo.InvariantCulture };
			using TextReader tr = File.OpenText(filename);
			using JsonTextReader jtr = new JsonTextReader(tr);
			return js.Deserialize<LabelACTION>(jtr);
		}

		public void Save(string filename)
		{
			JsonSerializer js = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore, Culture = System.Globalization.CultureInfo.InvariantCulture };
			using (TextWriter tw = File.CreateText(filename))
			using (JsonTextWriter jtw = new JsonTextWriter(tw) { Formatting = Formatting.Indented })
				js.Serialize(jtw, this);
		}

		public void Apply(NJS_ACTION act)
		{
			act.Name = ActionName;
			MotionNames.Apply(act.Animation);
			act.Animation.ActionName = act.Name;
			ObjectNames.Apply(act.Model);
		}
	}

	// Landtable labels
	public class LabelLANDTABLE
	{
		public string LandtableName;
		public string COLListName;
		public string GeoAnimListName;
		public List<LabelOBJECT> ColObjectNames;
		public List<LabelACTION> GeoAnimActionNames;

		public LabelLANDTABLE() { }

		public LabelLANDTABLE(LandTable land)
		{
			LandtableName = land.Name;
			COLListName = land.COLName;
			GeoAnimListName = land.AnimName;
			ColObjectNames = new List<LabelOBJECT>();
			foreach (COL o in land.COL)
				ColObjectNames.Add(new LabelOBJECT(o.Model));
			if (land.Anim != null)
			{
				GeoAnimActionNames = new List<LabelACTION>();
				foreach (GeoAnimData g in land.Anim)
					GeoAnimActionNames.Add(new LabelACTION(g));
			}
		}

		public static LabelLANDTABLE Load(string filename)
		{
			JsonSerializer js = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore, Culture = System.Globalization.CultureInfo.InvariantCulture };
			using TextReader tr = File.OpenText(filename);
			using JsonTextReader jtr = new JsonTextReader(tr);
			return js.Deserialize<LabelLANDTABLE>(jtr);
		}

		public void Save(string filename)
		{
			JsonSerializer js = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore, Culture = System.Globalization.CultureInfo.InvariantCulture };
			using (TextWriter tw = File.CreateText(filename))
			using (JsonTextWriter jtw = new JsonTextWriter(tw) { Formatting = Formatting.Indented })
				js.Serialize(jtw, this);
		}

		public void Apply(LandTable land)
		{
			land.Name = LandtableName;
			land.COLName = COLListName;
			land.AnimName = GeoAnimListName;
			foreach (COL o in land.COL)
				ColObjectNames[land.COL.IndexOf(o)].Apply(o.Model);
			if (land.Anim != null)
				foreach (GeoAnimData g in land.Anim)
				{
					g.Animation.ActionName = GeoAnimActionNames[land.Anim.IndexOf(g)].ActionName;
					GeoAnimActionNames[land.Anim.IndexOf(g)].MotionNames.Apply(g.Animation);
					GeoAnimActionNames[land.Anim.IndexOf(g)].ObjectNames.Apply(g.Model);
				}
		}
	}
}