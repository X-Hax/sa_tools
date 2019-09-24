using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Linq;
namespace SonicRetro.SAModel
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

		public bool HasWeight { get { return Vertex != null && Vertex.Any(a => a.HasWeight); } }

		public ChunkAttach()
		{
			Name = "attach_" + Extensions.GenerateIdentifier();
			Bounds = new BoundingSphere();
		}
		public static ChunkAttach CreateFromAssimp(List<Assimp.Material> materials, List<Assimp.Mesh> meshes, string[] textures = null)
		{
			ChunkAttach attach = new ChunkAttach(true, true);
			NvTriStripDotNet.NvStripifier nvStripifier = new NvTriStripDotNet.NvStripifier() { StitchStrips = false, UseRestart = false };
			bool hasnormal = meshes.Any(a => a.HasNormals);
			bool hasvcolor = meshes.Any(a => a.HasVertexColors(0));
			List<CachedVertex> cache = new List<CachedVertex>();
			List<List<Strip>> strips = new List<List<Strip>>();
			List<List<List<UV>>> uvs = new List<List<List<UV>>>();
			VertexChunk vertexChunk;
			if (hasvcolor)
				vertexChunk = new VertexChunk(ChunkType.Vertex_VertexDiffuse8);
			else if (hasnormal)
				vertexChunk = new VertexChunk(ChunkType.Vertex_VertexNormal);
			else
				vertexChunk = new VertexChunk(ChunkType.Vertex_Vertex);
			foreach (Assimp.Mesh aiMesh in meshes)
			{			
				List<Strip> polys = new List<Strip>();
				List<List<UV>> us = null;
				bool hasUV = aiMesh.HasTextureCoords(0);
				bool hasVColor = aiMesh.HasVertexColors(0);
				int currentstriptotal = 0;

				List<ushort> tris = new List<ushort>();
				Dictionary<ushort, Assimp.Vector3D> uvmap = new Dictionary<ushort, Assimp.Vector3D>();
				foreach (Assimp.Face aiFace in aiMesh.Faces)
					for (int i = 0; i < 3; i++)
					{
						UV uv = null;
						if (hasUV)
						{
							uv = new UV()
							{
								U = aiMesh.TextureCoordinateChannels[0][currentstriptotal].X,
								V = 1.0f - aiMesh.TextureCoordinateChannels[0][currentstriptotal].Y
							};
						}

						Color vertexColor = Color.White;
						if (hasVColor)
						{
							Assimp.Color4D aiColor = aiMesh.VertexColorChannels[0][currentstriptotal];
							vertexColor = Color.FromArgb((int)(aiColor.A * 255.0f), (int)(aiColor.R * 255.0f), (int)(aiColor.G * 255.0f), (int)(aiColor.B * 255.0f));
						}

						ushort ind = (ushort)cache.AddUnique(new CachedVertex(
							new Vertex(aiMesh.Vertices[aiFace.Indices[i]].X, aiMesh.Vertices[aiFace.Indices[i]].Y,
							aiMesh.Vertices[aiFace.Indices[i]].Z),
							new Vertex(aiMesh.Normals[aiFace.Indices[i]].X, aiMesh.Normals[aiFace.Indices[i]].Y,
							aiMesh.Normals[aiFace.Indices[i]].Z),
							vertexColor,
							uv));
						if (hasUV)
							uvmap[ind] = aiMesh.TextureCoordinateChannels[0][currentstriptotal];
						++currentstriptotal;
						tris.Add(ind);
					}

				if (hasUV)
					us = new List<List<UV>>();

				nvStripifier.GenerateStrips(tris.ToArray(), out var primitiveGroups);
				foreach(NvTriStripDotNet.PrimitiveGroup grp in primitiveGroups)
				{
					var stripIndices = new ushort[grp.Indices.Length];
					List<UV> stripuv = new List<UV>();
					for (var j = 0; j < grp.Indices.Length; j++)
					{
						var vertexIndex = grp.Indices[j];
						stripIndices[j] = vertexIndex;
						if (hasUV)
							stripuv.Add(new UV() { U = uvmap[vertexIndex].X, V = 1.0f - uvmap[vertexIndex].Y });
					}

					polys.Add(new Strip(stripIndices, false));
					if (hasUV)
						us.Add(stripuv);
					//PolyChunkStrip.Strip strp = new PolyChunkStrip.Strip(false, grp.Indices, null, null);
					//strip.Strips.Add(strp);
				}
				strips.Add(polys);
				uvs.Add(us);
			}
			foreach (var item in cache)
			{
				vertexChunk.Vertices.Add(item.vertex);
				if (hasvcolor)
					vertexChunk.Diffuse.Add(item.color);
				else if (hasnormal)
					vertexChunk.Normals.Add(item.normal);
				
			}
			vertexChunk.VertexCount = (ushort)cache.Count;
			switch (vertexChunk.Type)
			{
				case ChunkType.Vertex_Vertex:
					vertexChunk.Size = (ushort)(vertexChunk.VertexCount * 3 + 1);
					break;
				case ChunkType.Vertex_VertexDiffuse8:
					vertexChunk.Size = (ushort)(vertexChunk.VertexCount * 4 + 1);
					break;
				case ChunkType.Vertex_VertexNormal:
					vertexChunk.Size = (ushort)(vertexChunk.VertexCount * 6 + 1);
					break;
				case ChunkType.Vertex_VertexNormalDiffuse8:
					vertexChunk.Size = (ushort)(vertexChunk.VertexCount * 7 + 1);
					break;
			}
			attach.Vertex.Add(vertexChunk);
			for (int i = 0; i < meshes.Count; i++)
			{
				Assimp.Mesh aiMesh = meshes[i];

				//material stuff
				Assimp.Material currentAiMat = materials[aiMesh.MaterialIndex];
				if (currentAiMat != null)
				{
					//output mat first then texID, thats how the official exporter worked
					PolyChunkMaterial material = new PolyChunkMaterial();
					attach.Poly.Add(material);
					if (currentAiMat.HasTextureDiffuse)
					{
						if (textures != null)
						{
							PolyChunkTinyTextureID tinyTexId = new PolyChunkTinyTextureID();
							int texId = 0;
							for (int j = 0; j < textures.Length; j++)
								if (textures[j] == Path.GetFileNameWithoutExtension(currentAiMat.TextureDiffuse.FilePath))
									texId = j;
							tinyTexId.TextureID = (ushort)texId;
							attach.Poly.Add(tinyTexId);
						}
					}
					else if (textures != null)
					{
						PolyChunkTinyTextureID tinyTexId = new PolyChunkTinyTextureID();
						int texId = 0;
						for (int j = 0; j < textures.Length; j++)
							if (textures[j].ToLower() == currentAiMat.Name.ToLower())
								texId = j;
						tinyTexId.TextureID = (ushort)texId;
						attach.Poly.Add(tinyTexId);
					}
				}

				PolyChunkStrip strip;
				if (aiMesh.HasTextureCoords(0))
					strip = new PolyChunkStrip(ChunkType.Strip_StripUVN);
				else
					strip = new PolyChunkStrip(ChunkType.Strip_Strip);

				for (int i1 = 0; i1 < strips[i].Count; i1++)
				{
					Strip item = strips[i][i1];
					UV[] uv2 = null;
					if (aiMesh.HasTextureCoords(0))
						uv2 = uvs[i][i1].ToArray();
					strip.Strips.Add(new PolyChunkStrip.Strip(item.Reversed, item.Indexes, uv2, null));
				}
				attach.Poly.Add(strip);
			}


			return attach;
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
					if (chunk.Type != ChunkType.Null)
						Poly.Add(chunk);
					tmpaddr += chunk.ByteSize;
					chunk = PolyChunk.Load(file, tmpaddr);
				}
			}
			Bounds = new BoundingSphere(file, address + 8);
		}

		public override byte[] GetBytes(uint imageBase, bool DX, Dictionary<string, uint> labels, out uint address)
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
			result.AddRange(ByteConverter.GetBytes(vertexAddress));
			result.AddRange(ByteConverter.GetBytes(polyAddress));
			result.AddRange(Bounds.GetBytes());
			labels.Add(Name, address + imageBase);
			return result.ToArray();
		}

		public override string ToStruct(bool DX)
		{
			StringBuilder result = new StringBuilder("{ ");
			result.Append(Vertex != null ? VertexName : "NULL");
			result.Append(", ");
			result.Append(Poly != null ? PolyName : "NULL");
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
				writer.Write(VertexName);
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
				writer.Write(PolyName);
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
			writer.Write(Name);
			writer.Write(" = ");
			writer.Write(ToStruct(DX));
			writer.WriteLine(";");
		}

		static NJS_MATERIAL MaterialBuffer = new NJS_MATERIAL { UseTexture = true };
		static VertexData[] VertexBuffer = new VertexData[4095];
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

		public override void ProcessShapeMotionVertexData(NJS_MOTION motion, int frame, int animindex)
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
				switch (chunk.Type)
				{
					case ChunkType.Bits_BlendAlpha:
						{
							PolyChunkBitsBlendAlpha c2 = (PolyChunkBitsBlendAlpha)chunk;
							MaterialBuffer.SourceAlpha = c2.SourceAlpha;
							MaterialBuffer.DestinationAlpha = c2.DestinationAlpha;
						}
						break;
					case ChunkType.Bits_MipmapDAdjust:
						break;
					case ChunkType.Bits_SpecularExponent:
						MaterialBuffer.Exponent = ((PolyChunkBitsSpecularExponent)chunk).SpecularExponent;
						break;
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
					case ChunkType.Tiny_TextureID:
					case ChunkType.Tiny_TextureID2:
						{
							PolyChunkTinyTextureID c2 = (PolyChunkTinyTextureID)chunk;
							MaterialBuffer.ClampU = c2.ClampU;
							MaterialBuffer.ClampV = c2.ClampV;
							MaterialBuffer.FilterMode = c2.FilterMode;
							MaterialBuffer.FlipU = c2.FlipU;
							MaterialBuffer.FlipV = c2.FlipV;
							MaterialBuffer.SuperSample = c2.SuperSample;
							MaterialBuffer.TextureID = c2.TextureID;
						}
						break;
					case ChunkType.Material_Diffuse:
					case ChunkType.Material_Ambient:
					case ChunkType.Material_DiffuseAmbient:
					case ChunkType.Material_Specular:
					case ChunkType.Material_DiffuseSpecular:
					case ChunkType.Material_AmbientSpecular:
					case ChunkType.Material_DiffuseAmbientSpecular:
					case ChunkType.Material_Diffuse2:
					case ChunkType.Material_Ambient2:
					case ChunkType.Material_DiffuseAmbient2:
					case ChunkType.Material_Specular2:
					case ChunkType.Material_DiffuseSpecular2:
					case ChunkType.Material_AmbientSpecular2:
					case ChunkType.Material_DiffuseAmbientSpecular2:
						{
							PolyChunkMaterial c2 = (PolyChunkMaterial)chunk;
							MaterialBuffer.SourceAlpha = c2.SourceAlpha;
							MaterialBuffer.DestinationAlpha = c2.DestinationAlpha;
							if (c2.Diffuse.HasValue)
								MaterialBuffer.DiffuseColor = c2.Diffuse.Value;
							if (c2.Specular.HasValue)
							{
								MaterialBuffer.SpecularColor = c2.Specular.Value;
								MaterialBuffer.Exponent = c2.SpecularExponent;
							}
						}
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
							MaterialBuffer.DoubleSided = c2.DoubleSide;
							MaterialBuffer.EnvironmentMap = c2.EnvironmentMapping;
							MaterialBuffer.FlatShading = c2.FlatShading;
							MaterialBuffer.IgnoreLighting = c2.IgnoreLight;
							MaterialBuffer.IgnoreSpecular = c2.IgnoreSpecular;
							MaterialBuffer.UseAlpha = c2.UseAlpha;
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

		public override BasicAttach ToBasicModel()
		{
			throw new NotImplementedException();
		}

		public override ChunkAttach ToChunkModel()
		{
			return this;
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