using SAModel;
using System.Collections.Generic;
using System.Linq;

namespace SplitTools
{
	// NJS_MESHSET labels
	public class LabelMESHSET
	{
		[IniName("pl")]
		public string PolyName;
		[IniName("uv")]
		public string UVName;
		[IniName("pn")]
		public string PolyNormalName;
		[IniName("vc")]
		public string VColorName;

		public LabelMESHSET() { }

		public LabelMESHSET(string polyName, string uvName, string polyNormalName, string vColorName)
		{
			PolyName = polyName;
			UVName = uvName;
			PolyNormalName = polyNormalName;
			VColorName = vColorName;
		}

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
		[IniName("obj")]
		public string ObjectName;
		[IniName("att")]
		public string AttachName;
		[IniName("msh")]
		public string MeshsetOrPolyName; // Also polys for chunk
		[IniName("m")]
		[IniCollection(IniCollectionMode.NoSquareBrackets)]
		public List<LabelMESHSET> MeshsetItemNames;
		[IniName("vtx")]
		public string VertexName;
		[IniName("nml")]
		public string NormalName;
		[IniName("mat")]
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
						MeshsetOrPolyName = batt.MeshName;
						MeshsetItemNames = new List<LabelMESHSET>();
						foreach (NJS_MESHSET mesh in batt.Mesh)
							MeshsetItemNames.Add(new LabelMESHSET(mesh));
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
						batt.MeshName = MeshsetOrPolyName;
						int i = 0;
						foreach (NJS_MESHSET mesh in batt.Mesh)
						{
							MeshsetItemNames[i].Apply(mesh);
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
		[IniName("pos")]
		public string PositionName;
		[IniName("rot")]
		public string RotationName;
		[IniName("scl")]
		public string ScaleName;
		[IniName("vrt")]
		public string VertexName;
		[IniName("vct")]
		public string VectorName;
		[IniName("nrm")]
		public string NormalName;
		[IniName("tgt")]
		public string TargetName;
		[IniName("rll")]
		public string RollName;
		[IniName("ang")]
		public string AngleName;
		[IniName("col")]
		public string ColorName;
		[IniName("int")]
		public string IntensityName;
		[IniName("spt")]
		public string SpotName;
		[IniName("pnt")]
		public string PointName;
		[IniName("qut")]
		public string QuaternionName;
		[IniName("vtx")]
		public List<string> VertexItemNames;
		[IniName("nml")]
		public List<string> NormalItemNames;

		public LabelMKEY() { }

		public LabelMKEY(string positionName, string rotationName, string scaleName, string vertexName, string vectorName, string normalName, string targetName, string rollName, string angleName, string colorName, string intensityName, string spotName, string pointName, string quaternionName, List<string> vertexItemNames, List<string> normalItemNames)
		{
			PositionName = positionName;
			RotationName = rotationName;
			ScaleName = scaleName;
			VertexName = vertexName;
			VectorName = vectorName;
			NormalName = normalName;
			TargetName = targetName;
			RollName = rollName;
			AngleName = angleName;
			ColorName = colorName;
			IntensityName = intensityName;
			SpotName = spotName;
			PointName = pointName;
			QuaternionName = quaternionName;
			VertexItemNames = vertexItemNames;
			NormalItemNames = normalItemNames;
		}

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
				VertexItemNames = data.VertexItemName.ToList();
			if (data.NormalItemName != null)
				NormalItemNames = data.NormalItemName.ToList();
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
				data.VertexItemName = VertexItemNames.ToArray();
			if (data.NormalItemName != null)
				data.NormalItemName = NormalItemNames.ToArray();
		}
	}

	public class LabelMOTION
	{
		[IniName("mot")]
		public string MotionName;
		[IniName("mdt")]
		public string MdataName;
		[IniName("m")]
		[IniCollection(IniCollectionMode.NoSquareBrackets)]
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
		[IniName("act")]
		public string ActionName;
		[IniName("mtn")]
		public LabelMOTION MotionNames;
		[IniName("obj")]
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
		[IniName("lnd")]
		public string LandtableName;
		[IniName("col")]
		public string COLListName;
		[IniName("anm")]
		public string GeoAnimListName;
		[IniName("col")]
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
			foreach (GeoAnimData g in land.Anim)
				GeoAnimActionNames.Add(new LabelACTION(g));
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