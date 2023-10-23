using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace SAModel
{
	[Serializable]
	public class BasicAttach : Attach
	{
		public Vertex[] Vertex { get; private set; }
		public string VertexName { get; set; }
		public Vertex[] Normal { get; private set; }
		public string NormalName { get; set; }
		public List<NJS_MESHSET> Mesh { get; set; }
		public string MeshName { get; set; }
		public List<NJS_MATERIAL> Material { get; set; }
		public string MaterialName { get; set; }

		public BasicAttach()
		{
			Name = "attach_" + Extensions.GenerateIdentifier();
			Bounds = new BoundingSphere();
			Material = new List<NJS_MATERIAL>();
			MaterialName = "matlist_" + Extensions.GenerateIdentifier();
			Mesh = new List<NJS_MESHSET>();
			MeshName = "meshlist_" + Extensions.GenerateIdentifier();
			Vertex = new Vertex[0];
			VertexName = "vertex_" + Extensions.GenerateIdentifier();
			Normal = new Vertex[0];
			NormalName = "normal_" + Extensions.GenerateIdentifier();
		}

		public BasicAttach(byte[] file, int address, uint imageBase, bool DX)
			: this(file, address, imageBase, DX, new Dictionary<int, string>())
		{
		}

		public BasicAttach(byte[] file, int address, uint imageBase, bool DX, Dictionary<int, string> labels)
			: this()
		{
			if (labels.ContainsKey(address))
				Name = labels[address];
			else
				Name = "attach_" + address.ToString("X8");
			Vertex = new Vertex[ByteConverter.ToInt32(file, address + 8)];
			Normal = new Vertex[Vertex.Length];
			int tmpaddr = (int)(ByteConverter.ToUInt32(file, address) - imageBase);
			if (labels.ContainsKey(tmpaddr))
				VertexName = labels[tmpaddr];
			else
				VertexName = "vertex_" + tmpaddr.ToString("X8");
			for (int i = 0; i < Vertex.Length; i++)
			{
				Vertex[i] = new Vertex(file, tmpaddr);
				tmpaddr += SAModel.Vertex.Size;
			}
			tmpaddr = ByteConverter.ToInt32(file, address + 4);
			if (tmpaddr != 0)
			{
				tmpaddr = (int)((uint)tmpaddr - imageBase);
				if (labels.ContainsKey(tmpaddr))
					NormalName = labels[tmpaddr];
				else
					NormalName = "normal_" + tmpaddr.ToString("X8");
				for (int i = 0; i < Vertex.Length; i++)
				{
					Normal[i] = new Vertex(file, tmpaddr);
					tmpaddr += SAModel.Vertex.Size;
				}
			}
			else
			{
				for (int i = 0; i < Vertex.Length; i++)
					Normal[i] = new Vertex(0, 1, 0);
			}
			int maxmat = -1;
			int meshcnt = ByteConverter.ToInt16(file, address + 0x14);
			tmpaddr = ByteConverter.ToInt32(file, address + 0xC);
			if (tmpaddr != 0)
			{
				tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
				if (labels.ContainsKey(tmpaddr))
					MeshName = labels[tmpaddr];
				else
					MeshName = "meshlist_" + tmpaddr.ToString("X8");
				for (int i = 0; i < meshcnt; i++)
				{
					Mesh.Add(new NJS_MESHSET(file, tmpaddr, imageBase, labels));
					maxmat = Math.Max(maxmat, Mesh[i].MaterialID);
					tmpaddr += NJS_MESHSET.Size(DX);
				}
			}
			// fixes case where model declares material array as shorter than it really is
			int matcnt = Math.Max(ByteConverter.ToInt16(file, address + 0x16), maxmat + 1);
			tmpaddr = ByteConverter.ToInt32(file, address + 0x10);
			if (tmpaddr != 0)
			{
				tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
				if (labels.ContainsKey(tmpaddr))
					MaterialName = labels[tmpaddr];
				else
					MaterialName = "matlist_" + tmpaddr.ToString("X8");
				for (int i = 0; i < matcnt; i++)
				{
					Material.Add(new NJS_MATERIAL(file, tmpaddr, labels));
					tmpaddr += NJS_MATERIAL.Size;
				}
			}
			Bounds = new BoundingSphere(file, address + 0x18);

		}

		public BasicAttach(Vertex[] vertex, Vertex[] normal, IEnumerable<NJS_MESHSET> mesh, IEnumerable<NJS_MATERIAL> material)
			: this()
		{
			Vertex = vertex;
			Normal = normal;
			Mesh = new List<NJS_MESHSET>(mesh);
			Material = new List<NJS_MATERIAL>(material);

			Name = "attach_" + Extensions.GenerateIdentifier();
		}

		public override byte[] GetBytes(uint imageBase, bool DX, Dictionary<string, uint> labels, List<uint> njOffsets, out uint address)
		{
			List<byte> result = new List<byte>();
			uint materialAddress = 0;
			if (Material != null && Material.Count > 0)
			{
				if (labels.ContainsKey(MaterialName))
					materialAddress = labels[MaterialName];
				else
				{
					materialAddress = imageBase;
					labels.Add(MaterialName, materialAddress);
					foreach (NJS_MATERIAL item in Material)
						result.AddRange(item.GetBytes());
				}
			}
			uint meshAddress = 0;
			if (Mesh != null && Mesh.Count > 0)
			{
				if (labels.ContainsKey(MeshName))
					meshAddress = labels[MeshName];
				else
				{
					uint[] polyAddrs = new uint[Mesh.Count];
					uint[] polyNormalAddrs = new uint[Mesh.Count];
					uint[] vColorAddrs = new uint[Mesh.Count];
					uint[] uVAddrs = new uint[Mesh.Count];
					for (int i = 0; i < Mesh.Count; i++)
					{
						if (labels.ContainsKey(Mesh[i].PolyName))
							polyAddrs[i] = labels[Mesh[i].PolyName];
						else
						{
							result.Align(4);
							polyAddrs[i] = (uint)result.Count + imageBase;
							labels.Add(Mesh[i].PolyName, polyAddrs[i]);
							for (int j = 0; j < Mesh[i].Poly.Count; j++)
								result.AddRange(Mesh[i].Poly[j].GetBytes());
						}
					}
					for (int i = 0; i < Mesh.Count; i++)
					{
						if (Mesh[i].PolyNormal != null && Mesh[i].PolyNormal.Length > 0)
						{
							if (labels.ContainsKey(Mesh[i].PolyNormalName))
								polyNormalAddrs[i] = labels[Mesh[i].PolyNormalName];
							else
							{
								result.Align(4);
								polyNormalAddrs[i] = (uint)result.Count + imageBase;
								labels.Add(Mesh[i].PolyNormalName, polyNormalAddrs[i]);
								for (int j = 0; j < Mesh[i].PolyNormal.Length; j++)
									result.AddRange(Mesh[i].PolyNormal[j].GetBytes());
							}
						}
					}
					for (int i = 0; i < Mesh.Count; i++)
					{
						if (Mesh[i].VColor != null && Mesh[i].VColor.Length > 0)
						{
							if (labels.ContainsKey(Mesh[i].VColorName))
								vColorAddrs[i] = labels[Mesh[i].VColorName];
							else
							{
								result.Align(4);
								vColorAddrs[i] = (uint)result.Count + imageBase;
								labels.Add(Mesh[i].VColorName, vColorAddrs[i]);
								for (int j = 0; j < Mesh[i].VColor.Length; j++)
									result.AddRange(VColor.GetBytes(Mesh[i].VColor[j]));
							}
						}
					}
					for (int i = 0; i < Mesh.Count; i++)
					{
						if (Mesh[i].UV != null && Mesh[i].UV.Length > 0)
						{
							if (labels.ContainsKey(Mesh[i].UVName))
								uVAddrs[i] = labels[Mesh[i].UVName];
							else
							{
								result.Align(4);
								uVAddrs[i] = (uint)result.Count + imageBase;
								labels.Add(Mesh[i].UVName, uVAddrs[i]);
								for (int j = 0; j < Mesh[i].UV.Length; j++)
									result.AddRange(Mesh[i].UV[j].GetBytes());
							}
						}
					}
					result.Align(4);
					meshAddress = (uint)result.Count + imageBase;
					labels.Add(MeshName, meshAddress);
					for (int i = 0; i < Mesh.Count; i++)
					{
						//POF0
						//if (polyAddrs[i] != 0)
							njOffsets.Add((uint)(result.Count + imageBase + 0x4));
						//if (polyNormalAddrs[i] != 0)
							njOffsets.Add((uint)(result.Count + imageBase + 0xC));
						//if (vColorAddrs[i] != 0)
							njOffsets.Add((uint)(result.Count + imageBase + 0x10));
						//if (uVAddrs[i] != 0)
							njOffsets.Add((uint)(result.Count + imageBase + 0x14));

						result.AddRange(Mesh[i].GetBytes(polyAddrs[i], polyNormalAddrs[i], vColorAddrs[i], uVAddrs[i], DX));
					}
				}
				result.Align(4);
			}
			uint vertexAddress = 0;
			if (Vertex != null && Vertex.Length > 0)
			{
				if (labels.ContainsKey(VertexName))
					vertexAddress = labels[VertexName];
				else
				{
					vertexAddress = (uint)result.Count + imageBase;
					labels.Add(VertexName, vertexAddress);
					foreach (Vertex item in Vertex)
					{
						if (item == null)
							result.AddRange(new byte[SAModel.Vertex.Size]);
						else
							result.AddRange(item.GetBytes());
					}
				}
				result.Align(4);
			}
			uint normalAddress = 0;
			if (Normal != null && Normal.Length > 0)
			{
				if (labels.ContainsKey(NormalName))
					normalAddress = labels[NormalName];
				else
				{
					normalAddress = (uint)result.Count + imageBase;
					labels.Add(NormalName, normalAddress);
					foreach (Vertex item in Normal)
					{
						if (item == null)
							result.AddRange(new byte[SAModel.Vertex.Size]);
						else
							result.AddRange(item.GetBytes());
					}
				}
				result.Align(4);
			}
			address = (uint)result.Count;

			//POF0
			//if (vertexAddress != 0)
				njOffsets.Add((uint)(result.Count + imageBase));
			//if (normalAddress != 0)
				njOffsets.Add((uint)(result.Count + imageBase + 0x4));
			//if (meshAddress != 0)
				njOffsets.Add((uint)(result.Count + imageBase + 0xC));
			//if (materialAddress != 0)
				njOffsets.Add((uint)(result.Count + imageBase + 0x10));

			result.AddRange(ByteConverter.GetBytes(vertexAddress));
			result.AddRange(ByteConverter.GetBytes(normalAddress));
			result.AddRange(ByteConverter.GetBytes(Vertex.Length));
			result.AddRange(ByteConverter.GetBytes(meshAddress));
			result.AddRange(ByteConverter.GetBytes(materialAddress));
			result.AddRange(ByteConverter.GetBytes((short)Mesh.Count));
			result.AddRange(ByteConverter.GetBytes((short)Material.Count));
			result.AddRange(Bounds.GetBytes());
			if (DX)
				result.AddRange(new byte[4]);
			labels.Add(Name, address + imageBase);
			return result.ToArray();
		}

		public override string ToStruct(bool DX)
		{
			StringBuilder result = new StringBuilder("{ ");
			result.Append(Vertex != null ? VertexName : "NULL");
			result.Append(", ");
			result.Append(Normal != null ? NormalName : "NULL");
			result.Append(", ");
			result.Append(Vertex != null ? "LengthOfArray<Sint32>(" + VertexName + ")" : "0");
			result.Append(", ");
			result.Append(Mesh != null ? MeshName : "NULL");
			result.Append(", ");
			result.Append(Material != null && Material.Count > 0 ? MaterialName : "NULL");
			result.Append(", ");
			result.Append(Mesh != null ? "LengthOfArray<Uint16>(" + MeshName + ")" : "0");
			result.Append(", ");
			result.Append(Material != null && Material.Count > 0 ? "LengthOfArray<Uint16>(" + MaterialName + ")" : "0");
			result.Append(", ");
			result.Append(Bounds.ToStruct());
			if (DX)
				result.Append(", NULL");
			result.Append(" }");
			return result.ToString();
		}

		public override void ToStructVariables(TextWriter writer, bool DX, List<string> labels, string[] textures)
		{
			if (Material != null && Material.Count > 0 && !labels.Contains(MaterialName))
			{
				labels.Add(MaterialName);
				writer.Write("NJS_MATERIAL ");
				writer.Write(MaterialName);
				writer.WriteLine("[] = {");
				List<string> mtls = new List<string>(Material.Count);
				foreach (NJS_MATERIAL item in Material)
					mtls.Add(item.ToStruct(textures));
				writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", mtls.ToArray()));
				writer.WriteLine("};");
				writer.WriteLine();
			}
			if (!labels.Contains(MeshName))
			{
				for (int i = 0; i < Mesh.Count; i++)
				{
					if (!labels.Contains(Mesh[i].PolyName))
					{
						labels.Add(Mesh[i].PolyName);
						writer.Write("Sint16 ");
						writer.Write(Mesh[i].PolyName);
						writer.WriteLine("[] = {");
						List<string> plys = new List<string>(Mesh[i].Poly.Count);
						foreach (Poly item in Mesh[i].Poly)
							plys.Add(item.ToStruct());
						writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", plys.ToArray()));
						writer.WriteLine("};");
						writer.WriteLine();
					}
				}
				for (int i = 0; i < Mesh.Count; i++)
				{
					if (Mesh[i].PolyNormal != null && !labels.Contains(Mesh[i].PolyNormalName))
					{
						labels.Add(Mesh[i].PolyNormalName);
						writer.Write("NJS_VECTOR ");
						writer.Write(Mesh[i].PolyNormalName);
						writer.WriteLine("[] = {");
						List<string> plys = new List<string>(Mesh[i].PolyNormal.Length);
						foreach (Vertex item in Mesh[i].PolyNormal)
							plys.Add(item.ToStruct());
						writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", plys.ToArray()));
						writer.WriteLine("};");
						writer.WriteLine();
					}
				}
				for (int i = 0; i < Mesh.Count; i++)
				{
					if (Mesh[i].VColor != null && !labels.Contains(Mesh[i].VColorName))
					{
						labels.Add(Mesh[i].VColorName);
						writer.Write("NJS_COLOR ");
						writer.Write(Mesh[i].VColorName);
						writer.WriteLine("[] = {");
						List<string> vcs = new List<string>(Mesh[i].VColor.Length);
						foreach (Color item in Mesh[i].VColor)
							vcs.Add(item.ToStruct());
						writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", vcs.ToArray()));
						writer.WriteLine("};");
						writer.WriteLine();
					}
				}
				for (int i = 0; i < Mesh.Count; i++)
				{
					if (Mesh[i].UV != null && !labels.Contains(Mesh[i].UVName))
					{
						labels.Add(Mesh[i].UVName);
						writer.Write("NJS_TEX ");
						writer.Write(Mesh[i].UVName);
						writer.WriteLine("[] = {");
						List<string> uvs = new List<string>(Mesh[i].UV.Length);
						foreach (UV item in Mesh[i].UV)
							uvs.Add(item.ToStruct());
						writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", uvs.ToArray()));
						writer.WriteLine("};");
						writer.WriteLine();
					}
				}
				labels.Add(MeshName);
				writer.Write("NJS_MESHSET");
				if (DX)
					writer.Write("_SADX");
				writer.Write(" ");
				writer.Write(MeshName);
				writer.WriteLine("[] = {");
				List<string> mshs = new List<string>(Mesh.Count);
				foreach (NJS_MESHSET item in Mesh)
					mshs.Add(item.ToStruct(DX));
				writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", mshs.ToArray()));
				writer.WriteLine("};");
				writer.WriteLine();
			}
			if (!labels.Contains(VertexName))
			{
				labels.Add(VertexName);
				writer.Write("NJS_VECTOR ");
				writer.Write(VertexName);
				writer.WriteLine("[] = {");
				List<string> vtxs = new List<string>(Vertex.Length);
				foreach (Vertex item in Vertex)
					vtxs.Add(item != null ? item.ToStruct() : "{ 0 }");
				writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", vtxs.ToArray()));
				writer.WriteLine("};");
				writer.WriteLine();
			}
			if (Normal != null && !labels.Contains(NormalName))
			{
				labels.Add(VertexName);
				writer.Write("NJS_VECTOR ");
				writer.Write(NormalName);
				writer.WriteLine("[] = {");
				List<string> vtxs = new List<string>(Normal.Length);
				foreach (Vertex item in Normal)
					vtxs.Add(item != null ? item.ToStruct() : "{ 0 }");
				writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", vtxs.ToArray()));
				writer.WriteLine("};");
				writer.WriteLine();
			}
			writer.Write("NJS_MODEL");
			if (DX)
				writer.Write("_SADX");
			writer.Write(" ");
			writer.Write(Name);
			writer.Write(" = ");
			writer.Write(ToStruct(DX));
			writer.WriteLine(";");
		}

		public void ToNJA(TextWriter writer, List<string> labels, string[] textures)
		{
			if (Material != null && Material.Count > 0 && !labels.Contains(MaterialName))
			{
				labels.Add(MaterialName);
				writer.Write("MATERIAL    ");
				writer.Write(MaterialName + "[]" + Environment.NewLine);
				writer.WriteLine("START" + Environment.NewLine);
				List<string> mtls = new List<string>(Material.Count);
				foreach (NJS_MATERIAL item in Material)
					mtls.Add(item.ToNJA(textures));
				writer.WriteLine(string.Join(Environment.NewLine, mtls.ToArray()));
				writer.WriteLine("END");
				writer.WriteLine();
			}
			if (!labels.Contains(MeshName))
			{
				for (int i = 0; i < Mesh.Count; i++)
				{
					if (!labels.Contains(Mesh[i].PolyName))
					{
						labels.Add(Mesh[i].PolyName);
						writer.Write("POLYGON     ");
						writer.Write(Mesh[i].PolyName);
						writer.WriteLine("[]");
						writer.WriteLine("START");

						List<string> plys = new List<string>(Mesh[i].Poly.Count);
						for (int p = 0; p < Mesh[i].Poly.Count; p++)
						{
							Poly item = Mesh[i].Poly[p];
							if (item.PolyType == Basic_PolyType.Strips)
							{
								Strip strip = item as Strip;
								plys.Add("Strip(" + (strip.Reversed ? "0x8000, " : "0x0, ") + strip.Indexes.Length.ToString() + ")");
								plys.Add(item.ToNJA());
							}
							else
							{
								plys.Add(item.ToNJA());
							}
						}
						writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", plys.ToArray()) + ",");
						writer.WriteLine("END");
						writer.WriteLine();
					}
				}
				for (int i = 0; i < Mesh.Count; i++)
				{
					if (Mesh[i].PolyNormal != null && !labels.Contains(Mesh[i].PolyNormalName))
					{
						labels.Add(Mesh[i].PolyNormalName);
						writer.Write("POLYNORMAL  ");
						writer.Write(Mesh[i].PolyNormalName);
						writer.WriteLine("[]");
						writer.WriteLine("START");
						List<string> plys = new List<string>(Mesh[i].PolyNormal.Length);
						foreach (Vertex item in Mesh[i].PolyNormal)
							plys.Add("PNORM " + item.ToNJA());
						writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", plys.ToArray()) + ",");
						writer.WriteLine("END");
						writer.WriteLine();
					}
				}
				for (int i = 0; i < Mesh.Count; i++)
				{
					if (Mesh[i].VColor != null && !labels.Contains(Mesh[i].VColorName))
					{
						labels.Add(Mesh[i].VColorName);
						writer.Write("VERTCOLOR   ");
						writer.Write(Mesh[i].VColorName);
						writer.WriteLine("[]");
						writer.WriteLine("START");
						List<string> vcs = new List<string>(Mesh[i].VColor.Length);
						foreach (Color item in Mesh[i].VColor)
							vcs.Add(item.ToNJA());
						writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", vcs.ToArray()) + ",");
						writer.WriteLine("END");
						writer.WriteLine();
					}
				}
				for (int i = 0; i < Mesh.Count; i++)
				{
					if (Mesh[i].UV != null && !labels.Contains(Mesh[i].UVName))
					{
						labels.Add(Mesh[i].UVName);
						writer.Write("VERTUV      ");
						writer.Write(Mesh[i].UVName);
						writer.WriteLine("[]");
						writer.WriteLine("START");
						List<string> uvs = new List<string>(Mesh[i].UV.Length);
						foreach (UV item in Mesh[i].UV)
							uvs.Add(item.ToNJA());
						writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", uvs.ToArray()) + ",");
						writer.WriteLine("END");
						writer.WriteLine();
					}
				}
				labels.Add(MeshName);
				writer.Write("MESHSET");
				writer.Write("     ");
				writer.Write(MeshName);
				writer.WriteLine("[]");
				writer.WriteLine("START");
				writer.WriteLine();
				List<string> mshs = new List<string>(Mesh.Count);
				foreach (NJS_MESHSET item in Mesh)
					mshs.Add(item.ToNJA());
				writer.WriteLine(string.Join(Environment.NewLine, mshs.ToArray()));
				writer.WriteLine("END");
				writer.WriteLine();
			}
			if (!labels.Contains(VertexName))
			{
				labels.Add(VertexName);
				writer.Write("POINT      ");
				writer.Write(VertexName);
				writer.WriteLine("[]");
				writer.WriteLine("START");
				List<string> vtxs = new List<string>(Vertex.Length);
				foreach (Vertex item in Vertex)
					vtxs.Add("VERT " + item.ToNJA());
				writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", vtxs.ToArray()) + ",");
				writer.WriteLine("END");
				writer.WriteLine();
			}
			if (Normal != null && !labels.Contains(NormalName))
			{
				labels.Add(VertexName);
				writer.Write("NORMAL      ");
				writer.Write(NormalName);
				writer.WriteLine("[]");
				writer.WriteLine("START");
				List<string> vtxs = new List<string>(Normal.Length);
				foreach (Vertex item in Normal)
					vtxs.Add("NORM " + item.ToNJA());
				writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", vtxs.ToArray()) + ",");
				writer.WriteLine("END");
				writer.WriteLine();
			}
			writer.Write("MODEL");
			writer.Write("       ");
			writer.Write(Name);
			writer.WriteLine("[]");
			writer.WriteLine("START");
			writer.WriteLine("Points      " + VertexName + ",");
			writer.WriteLine("Normal      " + NormalName + ",");
			writer.WriteLine("PointNum    " + Vertex.Length + ",");
			writer.WriteLine("Meshset     " + MeshName + ",");
			writer.WriteLine("Materials   " + MaterialName + ",");
			writer.WriteLine("MeshsetNum  " + Mesh.Count + ",");
			writer.WriteLine("MatNum      " + Material.Count + ",");
			writer.WriteLine("Center      " + Bounds.Center.X.ToNJA() + ", " + Bounds.Center.Y.ToNJA() + ", " + Bounds.Center.Z.ToNJA() + ",");
			writer.WriteLine("Radius      " + Bounds.Radius.ToNJA() + ",");
			writer.WriteLine("END" + Environment.NewLine);
		}

		public override void ProcessVertexData()
		{
			List<MeshInfo> result = new List<MeshInfo>();
			foreach (NJS_MESHSET mesh in Mesh)
			{
				bool hasVColor = mesh.VColor != null;
				bool hasUV = mesh.UV != null;
				List<Poly> polys = new List<Poly>();
				List<VertexData> verts = new List<VertexData>();
				int currentstriptotal = 0;
				foreach (Poly poly in mesh.Poly)
				{
					Poly newpoly = null;
					switch (mesh.PolyType)
					{
						case Basic_PolyType.Triangles:
							newpoly = new Triangle();
							break;
						case Basic_PolyType.Quads:
							newpoly = new Quad();
							break;
						case Basic_PolyType.NPoly:
						case Basic_PolyType.Strips:
							newpoly = new Strip(poly.Indexes.Length, ((Strip)poly).Reversed);
							break;
					}
					for (int i = 0; i < poly.Indexes.Length; i++)
					{
						newpoly.Indexes[i] = (ushort)verts.Count;
						verts.Add(new VertexData(
							Vertex[poly.Indexes[i]],
							Normal[poly.Indexes[i]],
							hasVColor ? Color.FromArgb(mesh.VColor[currentstriptotal].R, mesh.VColor[currentstriptotal].G, mesh.VColor[currentstriptotal].B)  : null,
							hasUV ? mesh.UV[currentstriptotal++] : null));
					}
					polys.Add(newpoly);
				}
				NJS_MATERIAL mat = null;
				if (Material != null && mesh.MaterialID < Material.Count)
					mat = Material[mesh.MaterialID];
				result.Add(new MeshInfo(mat, polys.ToArray(), verts.ToArray(), hasUV, hasVColor));
			}
			MeshInfo = result.ToArray();
		}

		public override void ProcessShapeMotionVertexData(NJS_MOTION motion, float frame, int animindex)
		{
			if (!motion.Models.ContainsKey(animindex))
			{
				ProcessVertexData();
				return;
			}
			Vertex[] vertdata = Vertex;
			Vertex[] normdata = Normal;
			AnimModelData data = motion.Models[animindex];
			if (data.Vertex.Count > 0)
				vertdata = data.GetVertex(frame);
			if (data.Normal.Count > 0)
				normdata = data.GetNormal(frame);
			List<MeshInfo> result = new List<MeshInfo>();
			foreach (NJS_MESHSET mesh in Mesh)
			{
				bool hasVColor = mesh.VColor != null;
				bool hasUV = mesh.UV != null;
				List<Poly> polys = new List<Poly>();
				List<VertexData> verts = new List<VertexData>();
				int currentstriptotal = 0;
				foreach (Poly poly in mesh.Poly)
				{
					Poly newpoly = null;
					switch (mesh.PolyType)
					{
						case Basic_PolyType.Triangles:
							newpoly = new Triangle();
							break;
						case Basic_PolyType.Quads:
							newpoly = new Quad();
							break;
						case Basic_PolyType.NPoly:
						case Basic_PolyType.Strips:
							newpoly = new Strip(poly.Indexes.Length, ((Strip)poly).Reversed);
							break;
					}
					for (int i = 0; i < poly.Indexes.Length; i++)
					{
						newpoly.Indexes[i] = (ushort)verts.Count;
						verts.Add(new VertexData(
							vertdata[poly.Indexes[i]],
							normdata[poly.Indexes[i]],
							hasVColor ? (Color?)mesh.VColor[currentstriptotal] : null,
							hasUV ? mesh.UV[currentstriptotal++] : null));
					}
					polys.Add(newpoly);
				}
				NJS_MATERIAL mat = null;
				if (Material != null && mesh.MaterialID < Material.Count)
					mat = Material[mesh.MaterialID];
				result.Add(new MeshInfo(mat, polys.ToArray(), verts.ToArray(), hasUV, hasVColor));
			}
			MeshInfo = result.ToArray();
		}

		public void ResizeVertexes(int newSize)
		{
			Vertex[] vert = Vertex;
			Array.Resize(ref vert, newSize);
			Vertex = vert;
			vert = Normal;
			Array.Resize(ref vert, newSize);
			Normal = vert;
		}

		public override Attach Clone()
		{
			BasicAttach result = (BasicAttach)MemberwiseClone();
			result.Vertex = new Vertex[Vertex.Length];
			result.Normal = new Vertex[Normal.Length];
			for (int i = 0; i < Vertex.Length; i++)
			{
				result.Vertex[i] = Vertex[i].Clone();
				result.Normal[i] = Normal[i].Clone();
			}
			result.Material = new List<NJS_MATERIAL>(Material.Count);
			foreach (NJS_MATERIAL item in Material)
				result.Material.Add(item.Clone());
			result.Mesh = new List<NJS_MESHSET>(Mesh.Count);
			foreach (NJS_MESHSET item in Mesh)
				result.Mesh.Add(item.Clone());
			result.Bounds = Bounds.Clone();
			return result;
		}
	}
}