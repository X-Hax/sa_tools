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
			List<LabelOBJECT> result = js.Deserialize<List<LabelOBJECT>>(jtr);
			jtr.Close();
			return result;
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

		public static void OutputLabelList(NJS_OBJECT root, List<LabelOBJECT> LabelObjectList, Dictionary<int, string> labelList)
		{
			int o = 0;
			foreach (NJS_OBJECT obj in root.GetObjects())
			{
				LabelOBJECT label = LabelObjectList[o];
				int objAddr = 0;
				int.TryParse(obj.Name.Substring(obj.Name.Length - 8, 8), System.Globalization.NumberStyles.HexNumber, null, out objAddr);
				if (objAddr != 0 && !labelList.ContainsKey(objAddr))
				{
					labelList.Add(objAddr, label.ObjectName);
				}
				if (obj.Attach != null)
				{
					int attAddr = 0;
					int.TryParse(obj.Attach.Name.Substring(obj.Attach.Name.Length - 8, 8), System.Globalization.NumberStyles.HexNumber, null, out attAddr);
					if (attAddr != 0 && !labelList.ContainsKey(attAddr))
					{
						labelList.Add(attAddr, label.AttachName);
					}
					if (obj.Attach is BasicAttach batt)
					{
						if (batt.Vertex != null)
						{
							int vrtAddr = 0;
							int.TryParse(batt.VertexName.Substring(batt.VertexName.Length - 8, 8), System.Globalization.NumberStyles.HexNumber, null, out vrtAddr);
							if (vrtAddr != 0 && !labelList.ContainsKey(vrtAddr))
							{
								labelList.Add(vrtAddr, label.VertexName);
							}
						}
						if (batt.Normal != null)
						{
							int nrmAddr = 0;
							int.TryParse(batt.NormalName.Substring(batt.NormalName.Length - 8, 8), System.Globalization.NumberStyles.HexNumber, null, out nrmAddr);
							if (nrmAddr != 0 && !labelList.ContainsKey(nrmAddr))
							{
								labelList.Add(nrmAddr, label.NormalName);
							}
						}
						if (batt.Material != null)
						{
							int matAddr = 0;
							int.TryParse(batt.MaterialName.Substring(batt.NormalName.Length - 8, 8), System.Globalization.NumberStyles.HexNumber, null, out matAddr);
							if (matAddr != 0 && !labelList.ContainsKey(matAddr))
							{
								labelList.Add(matAddr, label.MaterialName);
							}
						}
						if (batt.Mesh != null)
						{
							int meshsetAddr = 0;
							int.TryParse(batt.MeshName.Substring(batt.MeshName.Length - 8, 8), System.Globalization.NumberStyles.HexNumber, null, out meshsetAddr);
							if (meshsetAddr != 0 && !labelList.ContainsKey(meshsetAddr))
							{
								labelList.Add(meshsetAddr, label.MeshsetName);
							}
							int i = 0;
							foreach (NJS_MESHSET mesh in batt.Mesh)
							{
								int polyAddr = 0;
								int.TryParse(mesh.PolyName.Substring(mesh.PolyName.Length - 8, 8), System.Globalization.NumberStyles.HexNumber, null, out polyAddr);
								if (polyAddr != 0 && !labelList.ContainsKey(polyAddr))
								{
									labelList.Add(polyAddr, label.MeshsetItems[i].PolyName);
								}
								int uvAddr = 0;
								int.TryParse(mesh.UVName.Substring(mesh.UVName.Length - 8, 8), System.Globalization.NumberStyles.HexNumber, null, out uvAddr);
								if (uvAddr != 0 && !labelList.ContainsKey(uvAddr))
								{
									labelList.Add(uvAddr, label.MeshsetItems[i].UVName);
								}
								int pnAddr = 0;
								int.TryParse(mesh.PolyNormalName.Substring(mesh.PolyNormalName.Length - 8, 8), System.Globalization.NumberStyles.HexNumber, null, out pnAddr);
								if (pnAddr != 0 && !labelList.ContainsKey(pnAddr))
								{
									labelList.Add(pnAddr, label.MeshsetItems[i].PolyNormalName);
								}
								int vcAddr = 0;
								int.TryParse(mesh.VColorName.Substring(mesh.VColorName.Length - 8, 8), System.Globalization.NumberStyles.HexNumber, null, out vcAddr);
								if (vcAddr != 0 && !labelList.ContainsKey(vcAddr))
								{
									labelList.Add(vcAddr, label.MeshsetItems[i].VColorName);
								}
								i++;
							}
						}
					}
					else if (obj.Attach is ChunkAttach catt)
					{
						if (catt.Vertex != null)
						{
							int vrtAddr = 0;
							int.TryParse(catt.VertexName.Substring(catt.VertexName.Length - 8, 8), System.Globalization.NumberStyles.HexNumber, null, out vrtAddr);
							if (vrtAddr != 0 && !labelList.ContainsKey(vrtAddr))
							{
								labelList.Add(vrtAddr, label.VertexName);
							}
						}
						if (catt.Poly != null)
						{
							int plyAddr = 0;
							int.TryParse(catt.PolyName.Substring(catt.PolyName.Length - 8, 8), System.Globalization.NumberStyles.HexNumber, null, out plyAddr);
							if (plyAddr != 0 && !labelList.ContainsKey(plyAddr))
							{
								labelList.Add(plyAddr, label.MeshsetName);
							}
						}
					}
				}
				o++;
			}
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
			LabelMOTION result = js.Deserialize<LabelMOTION>(jtr);
			jtr.Close();
			return result;
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

		public static void OutputLabelList(NJS_MOTION mot, LabelMOTION label, Dictionary<int, string> labelList)
		{
			// I got tired, maybe later
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
			if (anim.Animation != null)
			MotionNames = new LabelMOTION(anim.Animation);
			if (anim.Model != null)
				ObjectNames = new LabelOBJECT(anim.Model);
		}

		public LabelACTION(NJS_ACTION act)
		{
			ActionName = act.Name;
			if (act.Animation != null)
				MotionNames = new LabelMOTION(act.Animation);
			if (act.Model != null)
				ObjectNames = new LabelOBJECT(act.Model);
		}

		public static LabelACTION Load(string filename)
		{
			JsonSerializer js = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore, Culture = System.Globalization.CultureInfo.InvariantCulture };
			using TextReader tr = File.OpenText(filename);
			using JsonTextReader jtr = new JsonTextReader(tr);
			LabelACTION result = js.Deserialize<LabelACTION>(jtr);
			jtr.Close();
			return result;
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

		public Dictionary<int, string> OutputLabelList(NJS_ACTION act, LabelACTION label)
		{
			Dictionary<int, string> labelList = new();
			// I got tired, maybe later
			return labelList;
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
			LabelLANDTABLE result = js.Deserialize<LabelLANDTABLE>(jtr);
			jtr.Close();
			return result;
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

		public static void OutputLabelList(LandTable act, LabelLANDTABLE label, Dictionary<int, string> labelList)
		{
			// I got tired, maybe later
		}
	}

	// Texlist labels
	public class LabelTEXLIST
	{
		public string TexlistName;
		public string TexnamesName;

		public LabelTEXLIST() { }

		public LabelTEXLIST(NJS_TEXLIST texlist)
		{
			TexlistName = texlist.Name;
			TexnamesName = texlist.TexnameArrayName;
		}

		public static LabelTEXLIST Load(string filename)
		{
			JsonSerializer js = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore, Culture = System.Globalization.CultureInfo.InvariantCulture };
			using TextReader tr = File.OpenText(filename);
			using JsonTextReader jtr = new JsonTextReader(tr);
			LabelTEXLIST result = js.Deserialize<LabelTEXLIST>(jtr);
			jtr.Close();
			return result;
		}

		public void Save(string filename)
		{
			JsonSerializer js = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore, Culture = System.Globalization.CultureInfo.InvariantCulture };
			using (TextWriter tw = File.CreateText(filename))
			using (JsonTextWriter jtw = new JsonTextWriter(tw) { Formatting = Formatting.Indented })
				js.Serialize(jtw, this);
		}

		public void Apply(NJS_TEXLIST texlist)
		{
			texlist.Name = TexlistName;
			texlist.TexnameArrayName = TexnamesName;
		}

		public static void OutputLabelList(NJS_TEXLIST texlist, LabelTEXLIST label, Dictionary<int, string> labelList)
		{
			int texlistAddr = 0;
			int.TryParse(texlist.Name.Substring(texlist.Name.Length - 8, 8), System.Globalization.NumberStyles.HexNumber, null, out texlistAddr);
			if (texlistAddr != 0 && !labelList.ContainsKey(texlistAddr))
			{
				labelList.Add(texlistAddr, label.TexlistName);
			}
			int texnameAddr = 0;
			int.TryParse(texlist.TexnameArrayName.Substring(texlist.TexnameArrayName.Length - 8, 8), System.Globalization.NumberStyles.HexNumber, null, out texnameAddr);
			if (texnameAddr != 0 && !labelList.ContainsKey(texnameAddr))
			{
				labelList.Add(texnameAddr, label.TexnamesName);
			}
		}
	}
}