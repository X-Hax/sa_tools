using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Linq;
using System.Diagnostics;
using static SAModel.PolyChunkVolume;
using System.Security.Policy;
using static SAModel.PolyChunkStrip;

namespace SAModel
{
	class CachedVertex : IEquatable<CachedVertex>
	{
		public Vertex vertex;
		public Vertex normal;
		public Color color;
		public UV uv;

		public CachedVertex(Vertex v, Vertex n, Color c)
		{
			vertex = v;
			normal = n;
			color = c;
		}

		public CachedVertex(Vertex v, Vertex n, Color c, UV u)
		{
			vertex = v;
			normal = n;
			color = c;
			uv = u;
		}

		public bool Equals(CachedVertex other)
		{
			if (!vertex.Equals(other.vertex)) return false;
			if (!normal.Equals(other.normal)) return false;
			if (!color.Equals(other.color)) return false;
			if (uv == null && other.uv != null) return false;
			if (other.uv == null) return false;
			if (!uv.Equals(other.uv)) return false;
			return true;
		}
	}

	[Serializable]
	public class ChunkAttach : Attach
	{
		public List<VertexChunk> Vertex { get; set; }
		public string VertexName { get; set; }
		public List<PolyChunk> Poly { get; set; }
		public string PolyName { get; set; }

		public override bool HasWeight
		{
			get
			{
				if (base.HasWeight) return true;
				if (Poly == null || !Poly.Any(a => a is PolyChunkStrip))
					return Vertex != null && Vertex.Any(a => a.HasWeight);
				List<int> ids = new List<int>();
				if (Vertex != null)
					foreach (var vc in Vertex)
					{
						if (vc.HasWeight) return true;
						ids.AddRange(Enumerable.Range(vc.IndexOffset, vc.VertexCount));
					}
				return Poly.OfType<PolyChunkStrip>().SelectMany(a => a.Strips).SelectMany(a => a.Indexes).Any(a => !ids.Contains(a));
			}
		}

		public ChunkAttach()
		{
			Name = "attach_" + Extensions.GenerateIdentifier();
			Bounds = new BoundingSphere();
		}

		public ChunkAttach(bool hasVertex, bool hasPoly)
			: this()
		{
			if (hasVertex)
			{
				Vertex = new List<VertexChunk>();
				VertexName = "vertex_" + Extensions.GenerateIdentifier();
			}
			if (hasPoly)
			{
				Poly = new List<PolyChunk>();
				PolyName = "poly_" + Extensions.GenerateIdentifier();
			}
		}

		public ChunkAttach(byte[] file, int address, uint imageBase)
			: this(file, address, imageBase, new Dictionary<int, string>())
		{
		}

		public ChunkAttach(byte[] file, int address, uint imageBase, Dictionary<int, string> labels)
			: this()
		{
			if (labels.ContainsKey(address))
				Name = labels[address];
			else
				Name = "attach_" + address.ToString("X8");
			ChunkType ctype;
			int tmpaddr = ByteConverter.ToInt32(file, address);
			if (tmpaddr != 0)
			{
				tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
				Vertex = new List<VertexChunk>();
				if (labels.ContainsKey(tmpaddr))
					VertexName = labels[tmpaddr];
				else
					VertexName = "vertex_" + tmpaddr.ToString("X8");
				ctype = (ChunkType)(ByteConverter.ToUInt32(file, tmpaddr) & 0xFF);
				while (ctype != ChunkType.End)
				{
					VertexChunk chunk = new VertexChunk(file, tmpaddr);
					Vertex.Add(chunk);
					tmpaddr += (chunk.Size * 4) + 4;
					ctype = (ChunkType)(ByteConverter.ToUInt32(file, tmpaddr) & 0xFF);
				}
			}
			tmpaddr = ByteConverter.ToInt32(file, address + 4);
			if (tmpaddr != 0)
			{
				tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
				Poly = new List<PolyChunk>();
				if (labels.ContainsKey(tmpaddr))
					PolyName = labels[tmpaddr];
				else
					PolyName = "poly_" + tmpaddr.ToString("X8");
				PolyChunk chunk = PolyChunk.Load(file, tmpaddr);
				while (chunk.Type != ChunkType.End)
				{
					//if (chunk.Type != ChunkType.Null)
					Poly.Add(chunk);
					tmpaddr += chunk.ByteSize;
					chunk = PolyChunk.Load(file, tmpaddr);
				}
			}
			Bounds = new BoundingSphere(file, address + 8);
		}

		public override byte[] GetBytes(uint imageBase, bool DX, Dictionary<string, uint> labels, List<uint> njOffsets, out uint address)
		{
			List<byte> result = new List<byte>();
			uint vertexAddress = 0;
			if (Vertex != null && Vertex.Count > 0)
			{
				if (labels.ContainsKey(VertexName))
					vertexAddress = labels[VertexName];
				else
				{
					vertexAddress = imageBase;
					labels.Add(VertexName, vertexAddress);
					foreach (VertexChunk item in Vertex)
						result.AddRange(item.GetBytes());
					result.AddRange(new VertexChunk(ChunkType.End).GetBytes());
				}
			}
			result.Align(4);
			uint polyAddress = 0;
			if (Poly != null && Poly.Count > 0)
			{
				if (labels.ContainsKey(PolyName))
					polyAddress = labels[PolyName];
				else
				{
					polyAddress = (uint)(imageBase + result.Count);
					labels.Add(PolyName, polyAddress);
					foreach (PolyChunk item in Poly)
						result.AddRange(item.GetBytes());
					result.AddRange(new PolyChunkEnd().GetBytes());
				}
			}
			result.Align(4);
			address = (uint)result.Count;

			//POF0 offsets
			//if(vertexAddress != 0)
				njOffsets.Add((uint)(imageBase + result.Count));
			//if (polyAddress != 0)
				njOffsets.Add((uint)(imageBase + result.Count + 4));

			result.AddRange(ByteConverter.GetBytes(vertexAddress));
			result.AddRange(ByteConverter.GetBytes(polyAddress));
			result.AddRange(Bounds.GetBytes());
			labels.Add(Name, address + imageBase);
			return result.ToArray();
		}

		public override string ToStruct(bool DX)
		{
			StringBuilder result = new StringBuilder("{ ");
			result.Append(Vertex != null ? VertexName.MakeIdentifier() : "NULL");
			result.Append(", ");
			result.Append(Poly != null ? PolyName.MakeIdentifier() : "NULL");
			result.Append(", ");
			result.Append(Bounds.ToStruct());
			result.Append(" }");
			return result.ToString();
		}

		public override void ToStructVariables(TextWriter writer, bool DX, List<string> labels, string[] textures)
		{
			if (Vertex != null && !labels.Contains(VertexName))
			{
				labels.Add(VertexName);
				writer.Write("Sint32 ");
				writer.Write(VertexName.MakeIdentifier());
				writer.Write("[] = { ");
				List<byte> chunks = new List<byte>();
				foreach (VertexChunk item in Vertex)
					chunks.AddRange(item.GetBytes());
				chunks.AddRange(new VertexChunk(ChunkType.End).GetBytes());
				byte[] cb = chunks.ToArray();
				List<string> s = new List<string>(cb.Length / 4);
				for (int i = 0; i < cb.Length; i += 4)
				{
					int it = ByteConverter.ToInt32(cb, i);
					s.Add("0x" + it.ToString("X") + (it < 0 ? "u" : ""));
				}
				writer.Write(string.Join(", ", s.ToArray()));
				writer.WriteLine(" };");
				writer.WriteLine();
			}
			if (Poly != null && !labels.Contains(PolyName))
			{
				labels.Add(PolyName);
				writer.Write("Sint16 ");
				writer.Write(PolyName.MakeIdentifier());
				writer.Write("[] = { ");
				List<byte> chunks = new List<byte>();
				foreach (PolyChunk item in Poly)
					chunks.AddRange(item.GetBytes());
				chunks.AddRange(new PolyChunkEnd().GetBytes());
				byte[] cb = chunks.ToArray();
				List<string> s = new List<string>(cb.Length / 2);
				for (int i = 0; i < cb.Length; i += 2)
				{
					short sh = ByteConverter.ToInt16(cb, i);
					s.Add("0x" + sh.ToString("X") + (sh < 0 ? "u" : ""));
				}
				writer.Write(string.Join(", ", s.ToArray()));
				writer.WriteLine(" };");
				writer.WriteLine();
			}
			writer.Write("NJS_CNK_MODEL ");
			writer.Write(Name.MakeIdentifier());
			writer.Write(" = ");
			writer.Write(ToStruct(DX));
			writer.WriteLine(";");
		}

		public void ToNJA(TextWriter writer, List<string> labels, string[] textures)
		{
			if (Poly != null && !labels.Contains(PolyName))
			{
				writer.WriteLine("PLIST      " + PolyName.MakeIdentifier() + "[]");
				writer.WriteLine("START");

				foreach (PolyChunk item in Poly)
				{
					item.ToNJA(writer);
				}

				writer.WriteLine("\tCnkEnd()");
				writer.Write("END" + Environment.NewLine + Environment.NewLine);
			}

			if (Vertex != null && !labels.Contains(VertexName))
			{
				writer.WriteLine("VLIST      " + VertexName.MakeIdentifier() + "[]");
				writer.WriteLine("START");

				foreach (VertexChunk item in Vertex)
				{
					item.ToNJA(writer);
				}

				writer.WriteLine("\tCnkEnd()");
				writer.Write("END" + Environment.NewLine + Environment.NewLine);
			}

			writer.WriteLine("CNKMODEL   " + Name.MakeIdentifier() + "[]");
			writer.WriteLine("START");
			if (Vertex != null && !labels.Contains(VertexName))
				writer.WriteLine("VList      " + VertexName.MakeIdentifier() + ",");
			else
				writer.WriteLine("VList      NULL,");
			if (Poly != null && !labels.Contains(PolyName))
				writer.WriteLine("PList      " + PolyName.MakeIdentifier() + ",");
			else
				writer.WriteLine("PList      NULL,");
			writer.WriteLine("Center    " + Bounds.Center.X.ToNJA() + ", " + Bounds.Center.Y.ToNJA() + ", " + Bounds.Center.Z.ToNJA() + ",");
			writer.WriteLine("Radius    " + Bounds.Radius.ToNJA() + ",");
			writer.Write("END" + Environment.NewLine + Environment.NewLine);
		}

		static NJS_MATERIAL MaterialBuffer = new NJS_MATERIAL { UseTexture = true };
		static VertexData[] VertexBuffer = new VertexData[32768];
		static readonly CachedPoly[] PolyCache = new CachedPoly[255];

		public override void ProcessVertexData()
		{
#if modellog
			Extensions.Log("Processing Chunk Attach " + Name + Environment.NewLine);
#endif
			if (Vertex != null)
			{
				foreach (VertexChunk chunk in Vertex)
				{
#if modellog
					Extensions.Log("Vertex Declaration: " + chunk.IndexOffset + "-" + (chunk.IndexOffset + chunk.VertexCount - 1) + Environment.NewLine);
#endif
					if (VertexBuffer.Length < chunk.IndexOffset + chunk.VertexCount)
						Array.Resize(ref VertexBuffer, chunk.IndexOffset + chunk.VertexCount);
					for (int i = 0; i < chunk.VertexCount; i++)
					{
						VertexBuffer[i + chunk.IndexOffset] = new VertexData(chunk.Vertices[i]);
						if (chunk.Normals.Count > 0)
							VertexBuffer[i + chunk.IndexOffset].Normal = chunk.Normals[i];
						if (chunk.Diffuse.Count > 0)
							VertexBuffer[i + chunk.IndexOffset].Color = chunk.Diffuse[i];
					}
				}
			}
			List<MeshInfo> result = new List<MeshInfo>();
			if (Poly != null)
				result = ProcessPolyList(PolyName, Poly, 0);
			MeshInfo = result.ToArray();
		}

		public override void ProcessShapeMotionVertexData(NJS_MOTION motion, float frame, int animindex)
		{
			if (!motion.Models.ContainsKey(animindex))
			{
				ProcessVertexData();
				return;
			}
#if modellog
			Extensions.Log("Processing Chunk Attach " + Name + Environment.NewLine);
#endif
			if (Vertex != null)
			{
				foreach (VertexChunk chunk in Vertex)
				{
#if modellog
					Extensions.Log("Vertex Declaration: " + chunk.IndexOffset + "-" + (chunk.IndexOffset + chunk.VertexCount - 1) + Environment.NewLine);
#endif
					if (VertexBuffer.Length < chunk.IndexOffset + chunk.VertexCount)
						Array.Resize(ref VertexBuffer, chunk.IndexOffset + chunk.VertexCount);
					Vertex[] vertdata = chunk.Vertices.ToArray();
					Vertex[] normdata = chunk.Normals.ToArray();
					AnimModelData data = motion.Models[animindex];
					if (data.Vertex.Count > 0)
						vertdata = data.GetVertex(frame);
					if (data.Normal.Count > 0)
						normdata = data.GetNormal(frame);
					for (int i = 0; i < chunk.VertexCount; i++)
					{
						VertexBuffer[i + chunk.IndexOffset] = new VertexData(vertdata[i]);
						if (normdata.Length > 0)
							VertexBuffer[i + chunk.IndexOffset].Normal = normdata[i];
						if (chunk.Diffuse.Count > 0)
							VertexBuffer[i + chunk.IndexOffset].Color = chunk.Diffuse[i];
					}
				}
			}
			List<MeshInfo> result = new List<MeshInfo>();
			if (Poly != null)
				result = ProcessPolyList(PolyName, Poly, 0);
			MeshInfo = result.ToArray();
		}

		private List<MeshInfo> ProcessPolyList(string name, List<PolyChunk> strips, int start)
		{
			List<MeshInfo> result = new List<MeshInfo>();
			for (int i = start; i < strips.Count; i++)
			{
				PolyChunk chunk = strips[i];
				MaterialBuffer.UpdateFromPolyChunk(chunk);
				switch (chunk.Type)
				{
					case ChunkType.Bits_CachePolygonList:
						byte cachenum = ((PolyChunkBitsCachePolygonList)chunk).List;
#if modellog
						Extensions.Log("Caching Poly List " + name + "[" + (i + 1) + "] to cache #" + cachenum + Environment.NewLine);
#endif
						PolyCache[cachenum] = new CachedPoly(name, strips, i + 1);
						return result;
					case ChunkType.Bits_DrawPolygonList:
						cachenum = ((PolyChunkBitsDrawPolygonList)chunk).List;
						CachedPoly cached = PolyCache[cachenum];
#if modellog
						Extensions.Log("Drawing Poly List " + cached.Name + "[" + cached.Index + "] from cache #" + cachenum + Environment.NewLine);
#endif
						result.AddRange(ProcessPolyList(cached.Name, cached.Polys, cached.Index));
						break;
					case ChunkType.Strip_Strip:
					case ChunkType.Strip_StripUVN:
					case ChunkType.Strip_StripUVH:
					case ChunkType.Strip_StripNormal:
					case ChunkType.Strip_StripUVNNormal:
					case ChunkType.Strip_StripUVHNormal:
					case ChunkType.Strip_StripColor:
					case ChunkType.Strip_StripUVNColor:
					case ChunkType.Strip_StripUVHColor:
					case ChunkType.Strip_Strip2:
					case ChunkType.Strip_StripUVN2:
					case ChunkType.Strip_StripUVH2:
						{
							PolyChunkStrip c2 = (PolyChunkStrip)chunk;
#if modellog
							Extensions.Log("Strip " + c2.Type + Environment.NewLine);
#endif
							bool hasVColor = false;
							switch (chunk.Type)
							{
								case ChunkType.Strip_StripColor:
								case ChunkType.Strip_StripUVNColor:
								case ChunkType.Strip_StripUVHColor:
									hasVColor = true;
									break;
							}
							bool hasUV = false;
							switch (chunk.Type)
							{
								case ChunkType.Strip_StripUVN:
								case ChunkType.Strip_StripUVH:
								case ChunkType.Strip_StripUVNColor:
								case ChunkType.Strip_StripUVHColor:
								case ChunkType.Strip_StripUVN2:
								case ChunkType.Strip_StripUVH2:
									hasUV = true;
									break;
							}
							List<Poly> polys = new List<Poly>();
							List<VertexData> verts = new List<VertexData>();
#if modellog
							List<ushort> indexes = new List<ushort>();
#endif
							foreach (PolyChunkStrip.Strip strip in c2.Strips)
							{
#if modellog
								indexes.AddRange(strip.Indexes);
#endif
								Strip str = new Strip(strip.Indexes.Length, strip.Reversed);
								for (int k = 0; k < strip.Indexes.Length; k++)
								{
									str.Indexes[k] = (ushort)verts.Count;
									verts.Add(new VertexData(
										VertexBuffer[strip.Indexes[k]].Position,
										VertexBuffer[strip.Indexes[k]].Normal,
										hasVColor ? (Color?)strip.VColors[k] : VertexBuffer[strip.Indexes[k]].Color,
										hasUV ? strip.UVs[k] : null));
								}
								polys.Add(str);
							}
							if (!hasVColor)
								hasVColor = verts.Any(a => a.Color.HasValue && a.Color.Value != Color.White);
#if modellog
							indexes = new List<ushort>(System.Linq.Enumerable.Distinct(indexes));
							indexes.Sort();
							StringBuilder sb = new StringBuilder("Vertex Usage: ");
							ushort ist = indexes[0];
							for (int k = 0; k < indexes.Count - 1; k++)
							{
								if (indexes[k + 1] != indexes[k] + 1)
								{
									sb.Append(" " + ist);
									if (indexes[k] != ist)
										sb.Append("-" + indexes[k]);
									ist = indexes[++k];
								}
							}
							sb.Append(" " + ist);
							if (indexes[indexes.Count - 1] != ist)
								sb.Append("-" + indexes[indexes.Count - 1]);
							sb.Append(Environment.NewLine);
							Extensions.Log(sb.ToString());
#endif
							result.Add(new MeshInfo(MaterialBuffer, polys.ToArray(), verts.ToArray(), hasUV, hasVColor));
							MaterialBuffer = new NJS_MATERIAL(MaterialBuffer);
						}
						break;
					case ChunkType.Volume_Polygon3:
						PolyChunkVolume vol = (PolyChunkVolume)chunk;
						List<Poly> polytri = new List<Poly>();
						List<VertexData> verttri = new List<VertexData>();
						foreach (PolyChunkVolume.Triangle tri in vol.Polys)
						{
							Triangle three = new Triangle();
							for (int k = 0; k < tri.Indexes.Length; k++)
							{
								three.Indexes[k] = (ushort)verttri.Count;
								verttri.Add(new VertexData(
									VertexBuffer[tri.Indexes[k]].Position));
							}
							polytri.Add(three);
						}
						result.Add(new MeshInfo(MaterialBuffer, polytri.ToArray(), verttri.ToArray(), false, false, true));
						break;
				}
			}
			return result;
		}

		private class CachedPoly
		{
			public string Name { get; private set; }
			public List<PolyChunk> Polys { get; private set; }
			public int Index { get; private set; }

			public CachedPoly(string name, List<PolyChunk> polys, int index)
			{
				Name = name;
				Polys = polys;
				Index = index;
			}
		}

		public override Attach Clone()
		{
			ChunkAttach result = (ChunkAttach)MemberwiseClone();
			if (Vertex != null)
			{
				result.Vertex = new List<VertexChunk>(Vertex.Count);
				foreach (VertexChunk item in Vertex)
					result.Vertex.Add(item.Clone());
			}
			if (Poly != null)
			{
				result.Poly = new List<PolyChunk>(Poly.Count);
				foreach (PolyChunk item in Poly)
					result.Poly.Add(item.Clone());
			}
			result.Bounds = Bounds.Clone();
			return result;
		}
	}
}