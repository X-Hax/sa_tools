using System;
using System.Collections.Generic;
using System.Linq;
using SonicRetro.SAModel;
using System.Drawing;

namespace ModelConverter
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
				filename = Console.ReadLine();
			}
			ModelFile model = new ModelFile(filename);
			switch (model.Format)
			{
				case ModelFormat.Basic:
					foreach (NJS_OBJECT obj in model.Model.GetObjects().Where(obj => obj.Attach is BasicAttach))
					{
						BasicAttach basatt = (BasicAttach)obj.Attach;
						ChunkAttach cnkatt = new ChunkAttach(true, true) { Name = basatt.Name + "_cnk", Bounds = basatt.Bounds };
						obj.Attach = cnkatt;
						VertexChunk vcnk;
						bool hasnormal = basatt.Normal?.Length > 0;
						bool hasvcolor = basatt.Mesh.Any(a => a.VColor != null);
						if (hasvcolor)
							vcnk = new VertexChunk(ChunkType.Vertex_VertexDiffuse8);
						else if (hasnormal)
							vcnk = new VertexChunk(ChunkType.Vertex_VertexNormal);
						else
							vcnk = new VertexChunk(ChunkType.Vertex_Vertex);
						List<CachedVertex> cache = new List<CachedVertex>(basatt.Vertex.Length);
						List<List<ushort[]>> strips = new List<List<ushort[]>>();
						foreach (NJS_MESHSET mesh in basatt.Mesh)
						{
							List<ushort[]> polys = new List<ushort[]>();
							bool hasVColor = mesh.VColor != null;
							int currentstriptotal = 0;
							foreach (Poly poly in mesh.Poly)
							{
								ushort[] inds = (ushort[])poly.Indexes.Clone();
								for (int i = 0; i < poly.Indexes.Length; i++)
									inds[i] = (ushort)cache.AddUnique(new CachedVertex(
										basatt.Vertex[poly.Indexes[i]],
										basatt.Normal?[poly.Indexes[i]] ?? Vertex.UpNormal,
										hasVColor ? mesh.VColor[currentstriptotal++] : Color.White));
								polys.Add(inds);
							}
							strips.Add(polys);
						}
						foreach (var item in cache)
						{
							vcnk.Vertices.Add(item.vertex);
							if (hasnormal)
								vcnk.Normals.Add(item.normal);
							if (hasvcolor)
								vcnk.Diffuse.Add(item.color);
						}
						vcnk.VertexCount = (ushort)cache.Count;
						switch (vcnk.Type)
						{
							case ChunkType.Vertex_Vertex:
								vcnk.Size = (ushort)(vcnk.VertexCount * 3 + 1);
								break;
							case ChunkType.Vertex_VertexDiffuse8:
								vcnk.Size = (ushort)(vcnk.VertexCount * 4 + 1);
								break;
							case ChunkType.Vertex_VertexNormal:
								vcnk.Size = (ushort)(vcnk.VertexCount * 6 + 1);
								break;
							case ChunkType.Vertex_VertexNormalDiffuse8:
								vcnk.Size = (ushort)(vcnk.VertexCount * 7 + 1);
								break;
						}
						cnkatt.Vertex.Add(vcnk);
						for (int i = 0; i < basatt.Mesh.Count; i++)
						{
							NJS_MESHSET mesh = basatt.Mesh[i];
							if (mesh.PolyType != Basic_PolyType.Strips)
							{
								Console.WriteLine("Warning: Skipping non-strip mesh in {0} ({1}).", basatt.MeshName, mesh.PolyType);
								continue;
							}
							NJS_MATERIAL mat = null;
							if (basatt.Material != null && mesh.MaterialID < basatt.Material.Count)
							{
								mat = basatt.Material[mesh.MaterialID];
								cnkatt.Poly.Add(new PolyChunkTinyTextureID()
								{
									ClampU = mat.ClampU,
									ClampV = mat.ClampV,
									FilterMode = mat.FilterMode,
									FlipU = mat.FlipU,
									FlipV = mat.FlipV,
									SuperSample = mat.SuperSample,
									TextureID = (ushort)mat.TextureID
								});
								cnkatt.Poly.Add(new PolyChunkMaterial()
								{
									SourceAlpha = mat.SourceAlpha,
									DestinationAlpha = mat.DestinationAlpha,
									Diffuse = mat.DiffuseColor,
									Specular = mat.SpecularColor,
									SpecularExponent = (byte)mat.Exponent
								});
							}
							PolyChunkStrip strip;
							if (mesh.UV != null)
								strip = new PolyChunkStrip(ChunkType.Strip_StripUVN);
							else
								strip = new PolyChunkStrip(ChunkType.Strip_Strip);
							if (mat != null)
							{
								strip.IgnoreLight = mat.IgnoreLighting;
								strip.IgnoreSpecular = mat.IgnoreSpecular;
								strip.UseAlpha = mat.UseAlpha;
								strip.DoubleSide = mat.DoubleSided;
								strip.FlatShading = mat.FlatShading;
								strip.EnvironmentMapping = mat.EnvironmentMap;
							}
							int striptotal = 0;
							for (int i1 = 0; i1 < mesh.Poly.Count; i1++)
							{
								Strip item = (Strip)mesh.Poly[i1];
								UV[] uvs = null;
								if (mesh.UV != null)
								{
									uvs = new UV[item.Indexes.Length];
									Array.Copy(mesh.UV, striptotal, uvs, 0, item.Indexes.Length);
								}
								strip.Strips.Add(new PolyChunkStrip.Strip(item.Reversed, strips[i][i1], uvs, null));
								striptotal += item.Indexes.Length;
							}
							cnkatt.Poly.Add(strip);
						}
					}
					ModelFile.CreateFile(System.IO.Path.ChangeExtension(filename, "sa2mdl"), model.Model, null, null, null, null, null, ModelFormat.Chunk);
					break;
				case ModelFormat.Chunk:
					Vertex[] VertexBuffer = new Vertex[0];
					Vertex[] NormalBuffer = new Vertex[0];
					foreach (NJS_OBJECT obj in model.Model.GetObjects().Where(obj => obj.Attach is ChunkAttach))
					{
						ChunkAttach cnkatt = (ChunkAttach)obj.Attach;
						BasicAttach basatt = new BasicAttach() { Name = cnkatt.Name, Bounds = cnkatt.Bounds };
						obj.Attach = basatt;
						if (cnkatt.Vertex != null)
							foreach (VertexChunk chunk in cnkatt.Vertex)
							{
								if (VertexBuffer.Length < chunk.IndexOffset + chunk.VertexCount)
								{
									Array.Resize(ref VertexBuffer, chunk.IndexOffset + chunk.VertexCount);
									Array.Resize(ref NormalBuffer, chunk.IndexOffset + chunk.VertexCount);
								}
								Array.Copy(chunk.Vertices.ToArray(), 0, VertexBuffer, chunk.IndexOffset, chunk.Vertices.Count);
								Array.Copy(chunk.Normals.ToArray(), 0, NormalBuffer, chunk.IndexOffset, chunk.Normals.Count);
							}
						NJS_MATERIAL material = new NJS_MATERIAL() { UseTexture = true };
						int minVtx = int.MaxValue;
						int maxVtx = int.MinValue;
						foreach (PolyChunk chunk in cnkatt.Poly)
							switch (chunk.Type)
							{
								case ChunkType.Bits_BlendAlpha:
								{
									PolyChunkBitsBlendAlpha c2 = (PolyChunkBitsBlendAlpha)chunk;
									material.SourceAlpha = c2.SourceAlpha;
									material.DestinationAlpha = c2.DestinationAlpha;
								}
								break;
								case ChunkType.Bits_MipmapDAdjust:
									break;
								case ChunkType.Bits_SpecularExponent:
									material.Exponent = ((PolyChunkBitsSpecularExponent)chunk).SpecularExponent;
									break;
								case ChunkType.Tiny_TextureID:
								case ChunkType.Tiny_TextureID2:
								{
									PolyChunkTinyTextureID c2 = (PolyChunkTinyTextureID)chunk;
									material.ClampU = c2.ClampU;
									material.ClampV = c2.ClampV;
									material.FilterMode = c2.FilterMode;
									material.FlipU = c2.FlipU;
									material.FlipV = c2.FlipV;
									material.SuperSample = c2.SuperSample;
									material.TextureID = c2.TextureID;
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
									material.SourceAlpha = c2.SourceAlpha;
									material.DestinationAlpha = c2.DestinationAlpha;
									if (c2.Diffuse.HasValue)
										material.DiffuseColor = c2.Diffuse.Value;
									if (c2.Specular.HasValue)
									{
										material.SpecularColor = c2.Specular.Value;
										material.Exponent = c2.SpecularExponent;
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
									material.DoubleSided = c2.DoubleSide;
									material.EnvironmentMap = c2.EnvironmentMapping;
									material.FlatShading = c2.FlatShading;
									material.IgnoreLighting = c2.IgnoreLight;
									material.IgnoreSpecular = c2.IgnoreSpecular;
									material.UseAlpha = c2.UseAlpha;
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
									List<Strip> strips = new List<Strip>(c2.StripCount);
									List<UV> uvs = hasUV ? new List<UV>() : null;
									List<Color> vcolors = hasVColor ? new List<Color>() : null;
									foreach (PolyChunkStrip.Strip strip in c2.Strips)
									{
										minVtx = Math.Min(minVtx, strip.Indexes.Min());
										maxVtx = Math.Max(maxVtx, strip.Indexes.Max());
										strips.Add(new Strip((ushort[])strip.Indexes.Clone(), strip.Reversed));
										if (hasUV)
											uvs.AddRange(strip.UVs);
										if (hasVColor)
											vcolors.AddRange(strip.VColors);
									}
									NJS_MESHSET mesh = new NJS_MESHSET(strips.ToArray(), false, hasUV, hasVColor);
									if (hasUV)
										uvs.CopyTo(mesh.UV);
									if (hasVColor)
										vcolors.CopyTo(mesh.VColor);
									mesh.MaterialID = (ushort)basatt.Material.Count;
									basatt.Mesh.Add(mesh);
									basatt.Material.Add(material);
									material = new NJS_MATERIAL(material.GetBytes(), 0);
								}
								break;
							}
						int numVtx = maxVtx - minVtx + 1;
						basatt.ResizeVertexes(numVtx);
						Array.Copy(VertexBuffer, minVtx, basatt.Vertex, 0, numVtx);
						Array.Copy(NormalBuffer, minVtx, basatt.Normal, 0, numVtx);
						foreach (NJS_MESHSET mesh in basatt.Mesh)
							foreach (Poly poly in mesh.Poly)
								for (int i = 0; i < poly.Indexes.Length; i++)
									poly.Indexes[i] = (ushort)(poly.Indexes[i] - minVtx);
					}
					ModelFile.CreateFile(System.IO.Path.ChangeExtension(filename, "sa1mdl"), model.Model, null, null, null, null, null, ModelFormat.Basic);
					break;
			}
		}
	}

	class CachedVertex : IEquatable<CachedVertex>
	{
		public Vertex vertex;
		public Vertex normal;
		public Color color;

		public CachedVertex(Vertex v, Vertex n, Color c)
		{
			vertex = v;
			normal = n;
			color = c;
		}

		public bool Equals(CachedVertex other)
		{
			if (!vertex.Equals(other.vertex)) return false;
			if (!normal.Equals(other.normal)) return false;
			if (!color.Equals(other.color)) return false;
			return true;
		}
	}
}