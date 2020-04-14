using NvTriStripDotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SonicRetro.SAModel.SAEditorCommon.ModelConversion
{
	public static class ModelConversion
	{
		static readonly NvStripifier nvStripifier = new NvStripifier() { StitchStrips = false, UseRestart = false };

		public static ChunkAttach ToChunk(this Attach attach)
		{
			switch (attach)
			{
				case BasicAttach basatt:
					return basatt.ToChunk();
				case ChunkAttach cnkatt:
					return (ChunkAttach)cnkatt.Clone();
				case null:
					return null;
				default:
					throw new ArgumentException("Unknown attach type passed to ToChunk.", "attach");
			}
		}

		public static ChunkAttach ToChunk(this BasicAttach basatt)
		{
			ChunkAttach cnkatt = new ChunkAttach(true, true) { Name = basatt.Name + "_cnk", Bounds = basatt.Bounds };
			VertexChunk vcnk;
			bool hasnormal = basatt.Normal?.Length > 0;
			bool hasvcolor = basatt.Mesh.Any(a => a.VColor != null);
			if (hasvcolor)
			{
				vcnk = new VertexChunk(ChunkType.Vertex_VertexDiffuse8);
				hasnormal = false;
			}
			else if (hasnormal)
				vcnk = new VertexChunk(ChunkType.Vertex_VertexNormal);
			else
				vcnk = new VertexChunk(ChunkType.Vertex_Vertex);
			List<CachedVertex> cache = new List<CachedVertex>(basatt.Vertex.Length);
			List<List<Strip>> strips = new List<List<Strip>>();
			List<List<List<UV>>> uvs = new List<List<List<UV>>>();
			foreach (NJS_MESHSET mesh in basatt.Mesh)
			{
				List<Strip> polys = new List<Strip>();
				List<List<UV>> us = null;
				bool hasUV = mesh.UV != null;
				bool hasVColor = mesh.VColor != null;
				int currentstriptotal = 0;
				switch (mesh.PolyType)
				{
					case Basic_PolyType.Triangles:
						{
							List<ushort> tris = new List<ushort>();
							Dictionary<ushort, UV> uvmap = new Dictionary<ushort, UV>();
							foreach (Poly poly in mesh.Poly)
								for (int i = 0; i < 3; i++)
								{
									ushort ind = (ushort)cache.AddUnique(new CachedVertex(
										basatt.Vertex[poly.Indexes[i]],
										basatt.Normal[poly.Indexes[i]],
										hasVColor ? mesh.VColor[currentstriptotal] : Color.White,
										mesh.UV?[currentstriptotal]));
									if (hasUV)
										uvmap[ind] = mesh.UV[currentstriptotal];
									++currentstriptotal;
									tris.Add(ind);
								}

							if (hasUV)
								us = new List<List<UV>>();

							nvStripifier.GenerateStrips(tris.ToArray(), out var primitiveGroups);

							// Add strips
							for (var i = 0; i < primitiveGroups.Length; i++)
							{
								var primitiveGroup = primitiveGroups[i];
								System.Diagnostics.Debug.Assert(primitiveGroup.Type == PrimitiveType.TriangleStrip);

								var stripIndices = new ushort[primitiveGroup.Indices.Length];
								List<UV> stripuv = new List<UV>();
								for (var j = 0; j < primitiveGroup.Indices.Length; j++)
								{
									var vertexIndex = primitiveGroup.Indices[j];
									stripIndices[j] = vertexIndex;
									if (hasUV)
										stripuv.Add(uvmap[vertexIndex]);
								}

								polys.Add(new Strip(stripIndices, false));
								if (hasUV)
									us.Add(stripuv);
							}
						}
						break;
					case Basic_PolyType.Quads:
						{
							List<ushort> tris = new List<ushort>();
							Dictionary<ushort, UV> uvmap = new Dictionary<ushort, UV>();
							foreach (Poly poly in mesh.Poly)
							{
								ushort[] quad = new ushort[4];
								for (int i = 0; i < 4; i++)
								{
									ushort ind = (ushort)cache.AddUnique(new CachedVertex(
										basatt.Vertex[poly.Indexes[i]],
										basatt.Normal[poly.Indexes[i]],
										hasVColor ? mesh.VColor[currentstriptotal] : Color.White,
										mesh.UV?[currentstriptotal]));
									if (hasUV)
										uvmap[ind] = mesh.UV[currentstriptotal];
									++currentstriptotal;
									quad[i] = ind;
								}
								tris.Add(quad[0]);
								tris.Add(quad[1]);
								tris.Add(quad[2]);
								tris.Add(quad[2]);
								tris.Add(quad[1]);
								tris.Add(quad[3]);
							}

							if (hasUV)
								us = new List<List<UV>>();

							nvStripifier.GenerateStrips(tris.ToArray(), out var primitiveGroups);

							// Add strips
							for (var i = 0; i < primitiveGroups.Length; i++)
							{
								var primitiveGroup = primitiveGroups[i];
								System.Diagnostics.Debug.Assert(primitiveGroup.Type == PrimitiveType.TriangleStrip);

								var stripIndices = new ushort[primitiveGroup.Indices.Length];
								List<UV> stripuv = new List<UV>();
								for (var j = 0; j < primitiveGroup.Indices.Length; j++)
								{
									var vertexIndex = primitiveGroup.Indices[j];
									stripIndices[j] = vertexIndex;
									if (hasUV)
										stripuv.Add(uvmap[vertexIndex]);
								}

								polys.Add(new Strip(stripIndices, false));
								if (hasUV)
									us.Add(stripuv);
							}
						}
						break;
					case Basic_PolyType.NPoly:
					case Basic_PolyType.Strips:
						if (hasUV)
							us = new List<List<UV>>();
						foreach (Strip poly in mesh.Poly.Cast<Strip>())
						{
							List<UV> stripuv = new List<UV>();
							ushort[] inds = (ushort[])poly.Indexes.Clone();
							for (int i = 0; i < poly.Indexes.Length; i++)
							{
								inds[i] = (ushort)cache.AddUnique(new CachedVertex(
									basatt.Vertex[poly.Indexes[i]],
									basatt.Normal[poly.Indexes[i]],
									hasVColor ? mesh.VColor[currentstriptotal] : Color.White));
								if (hasUV)
									stripuv.Add(mesh.UV[currentstriptotal]);
								++currentstriptotal;
							}

							polys.Add(new Strip(inds, poly.Reversed));
							if (hasUV)
								us.Add(stripuv);
						}
						break;
				}
				strips.Add(polys);
				uvs.Add(us);
			}
			foreach (var item in cache)
			{
				vcnk.Vertices.Add(item.vertex);
				if (hasnormal)
					vcnk.Normals.Add(item.normal);
				if (hasvcolor)
					vcnk.Diffuse.Add(item.color);
			}
			cnkatt.Vertex.Add(vcnk);
			for (int i = 0; i < basatt.Mesh.Count; i++)
			{
				NJS_MESHSET mesh = basatt.Mesh[i];
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
				for (int i1 = 0; i1 < strips[i].Count; i1++)
				{
					Strip item = strips[i][i1];
					UV[] uv2 = null;
					if (mesh.UV != null)
						uv2 = uvs[i][i1].ToArray();
					strip.Strips.Add(new PolyChunkStrip.Strip(item.Reversed, item.Indexes, uv2, null));
				}
				cnkatt.Poly.Add(strip);
			}

			return cnkatt;
		}

		static Vertex[] VertexBuffer = new Vertex[0];
		static Vertex[] NormalBuffer = new Vertex[0];
		static Color?[] ColorBuffer = new Color?[0];
		public static void ResetVertexBuffer()
		{
			VertexBuffer = new Vertex[0];
			NormalBuffer = new Vertex[0];
			ColorBuffer = new Color?[0];
		}

		public static BasicAttach ToBasic(this Attach attach)
		{
			switch (attach)
			{
				case BasicAttach basatt:
					return (BasicAttach)basatt.Clone();
				case ChunkAttach cnkatt:
					return cnkatt.ToBasic();
				case null:
					return null;
				default:
					throw new ArgumentException("Unknown attach type passed to ToBasic.", "attach");
			}
		}

		public static BasicAttach ToBasic(this ChunkAttach cnkatt)
		{
			BasicAttach basatt = new BasicAttach() { Name = cnkatt.Name, Bounds = cnkatt.Bounds };
			if (cnkatt.Vertex != null)
				foreach (VertexChunk chunk in cnkatt.Vertex)
				{
					if (VertexBuffer.Length < chunk.IndexOffset + chunk.VertexCount)
					{
						Array.Resize(ref VertexBuffer, chunk.IndexOffset + chunk.VertexCount);
						Array.Resize(ref NormalBuffer, chunk.IndexOffset + chunk.VertexCount);
						Array.Resize(ref ColorBuffer, chunk.IndexOffset + chunk.VertexCount);
					}
					Array.Copy(chunk.Vertices.ToArray(), 0, VertexBuffer, chunk.IndexOffset, chunk.Vertices.Count);
					Array.Copy(chunk.Normals.ToArray(), 0, NormalBuffer, chunk.IndexOffset, chunk.Normals.Count);
					if (chunk.Diffuse.Count > 0)
						Array.Copy(chunk.Diffuse.Cast<Color?>().ToArray(), 0, ColorBuffer, chunk.IndexOffset, chunk.Diffuse.Count);
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
							bool hasStripVColor = false;
							switch (chunk.Type)
							{
								case ChunkType.Strip_StripColor:
								case ChunkType.Strip_StripUVNColor:
								case ChunkType.Strip_StripUVHColor:
									hasStripVColor = true;
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
							bool hasVertVColor = false;
							if (!hasStripVColor && c2.Strips.All(a => a.Indexes.All(b => ColorBuffer[b].HasValue)))
								hasVertVColor = true;
							bool hasVColor = hasStripVColor || hasVertVColor;
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
								if (hasStripVColor)
									vcolors.AddRange(strip.VColors);
								else if (hasVertVColor)
									foreach (short i in strip.Indexes)
										vcolors.Add(ColorBuffer[i].Value);
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
			return basatt;
		}
	}

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
}
