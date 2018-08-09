using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SonicRetro.SAModel;
using System.Drawing;

namespace LevelConverter
{
	class Program
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
			LandTable level = LandTable.LoadFromFile(filename);
			switch (level.Format)
			{
				case LandTableFormat.SA1:
					{
						List<COL> newcollist = new List<COL>();
						foreach (COL col in level.COL.Where((col) => col.Model != null && col.Model.Attach != null))
						{
							if ((col.SurfaceFlags & SurfaceFlags.Visible) == SurfaceFlags.Visible)
							{
								COL newcol = new COL() { Bounds = col.Bounds };
								newcol.SurfaceFlags = SurfaceFlags.Visible;
								newcol.Model = new NJS_OBJECT() { Name = col.Model.Name + "_cnk" };
								newcol.Model.Position = col.Model.Position;
								newcol.Model.Rotation = col.Model.Rotation;
								newcol.Model.Scale = col.Model.Scale;
								BasicAttach basatt = (BasicAttach)col.Model.Attach;
								ChunkAttach cnkatt = new ChunkAttach(true, true) { Name = basatt.Name + "_cnk", Bounds = basatt.Bounds };
								newcol.Model.Attach = cnkatt;
								VertexChunk vcnk;
								if (basatt.Normal != null && basatt.Normal.Length > 0)
									vcnk = new VertexChunk(ChunkType.Vertex_VertexNormal);
								else
									vcnk = new VertexChunk(ChunkType.Vertex_Vertex);
								vcnk.Vertices = new List<Vertex>(basatt.Vertex);
								if (basatt.Normal != null)
									vcnk.Normals = new List<Vertex>(basatt.Normal);
								vcnk.VertexCount = (ushort)basatt.Vertex.Length;
								vcnk.Size = (ushort)((vcnk.Type == ChunkType.Vertex_VertexNormal ? vcnk.VertexCount * 6 : vcnk.VertexCount * 3) + 1);
								cnkatt.Vertex.Add(vcnk);
								foreach (NJS_MESHSET mesh in basatt.Mesh)
								{
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
									if (mesh.UV != null & mesh.VColor != null)
										strip = new PolyChunkStrip(ChunkType.Strip_StripUVNColor);
									else if (mesh.UV != null)
										strip = new PolyChunkStrip(ChunkType.Strip_StripUVN);
									else if (mesh.VColor != null)
										strip = new PolyChunkStrip(ChunkType.Strip_StripColor);
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
									foreach (Strip item in mesh.Poly.Cast<Strip>())
									{
										UV[] uvs = null;
										if (mesh.UV != null)
										{
											uvs = new UV[item.Indexes.Length];
											Array.Copy(mesh.UV, striptotal, uvs, 0, item.Indexes.Length);
										}
										Color[] vcolors = null;
										if (mesh.VColor != null)
										{
											vcolors = new Color[item.Indexes.Length];
											Array.Copy(mesh.VColor, striptotal, vcolors, 0, item.Indexes.Length);
										}
										strip.Strips.Add(new PolyChunkStrip.Strip(item.Reversed, item.Indexes, uvs, vcolors));
										striptotal += item.Indexes.Length;
									}
									cnkatt.Poly.Add(strip);
								}
								newcollist.Add(newcol);
							}
							if ((col.SurfaceFlags & ~SurfaceFlags.Visible) != 0)
							{
								col.SurfaceFlags &= ~SurfaceFlags.Visible;
								newcollist.Add(col);
							}
						}
						level.COL = newcollist;
					}
					level.Anim = new List<GeoAnimData>();
					level.Tool = "SA Tools Level Converter";
					level.SaveToFile(System.IO.Path.ChangeExtension(filename, "sa2lvl"), LandTableFormat.SA2);
					break;
				case LandTableFormat.SA2:
					Vertex[] VertexBuffer = new Vertex[0];
					Vertex[] NormalBuffer = new Vertex[0];
					foreach (COL col in level.COL.Where((col) => col.Model != null && col.Model.Attach is ChunkAttach))
					{
						ChunkAttach cnkatt = (ChunkAttach)col.Model.Attach;
						BasicAttach basatt = new BasicAttach() { Name = cnkatt.Name, Bounds = cnkatt.Bounds };
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
						col.Model.Attach = basatt;
					}
					level.Anim = new List<GeoAnimData>();
					level.Tool = "SA Tools Level Converter";
					level.SaveToFile(System.IO.Path.ChangeExtension(filename, "sa1lvl"), LandTableFormat.SA1);
					break;
			}
		}
	}
}