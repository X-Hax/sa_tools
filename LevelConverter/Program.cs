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
					Console.WriteLine("SA1->SA2 conversion is not yet supported.");
					break;
				case LandTableFormat.SA2:
					Vertex[] VertexBuffer = new Vertex[0];
					Vertex[] NormalBuffer = new Vertex[0];
					foreach (COL col in level.COL)
						if (col.Model != null && col.Model.Attach != null && col.Model.Attach is ChunkAttach)
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
							Material material = new Material() { UseTexture = true };
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
												strips.Add(new Strip(strip.Indexes, strip.Reversed));
												if (hasUV)
													uvs.AddRange(strip.UVs);
												if (hasVColor)
													vcolors.AddRange(strip.VColors);
											}
											Mesh mesh = new Mesh(strips.ToArray(), false, hasUV, hasVColor);
											if (hasUV)
												uvs.CopyTo(mesh.UV);
											if (hasVColor)
												vcolors.CopyTo(mesh.VColor);
											mesh.MaterialID = (ushort)basatt.Material.Count;
											basatt.Mesh.Add(mesh);
											basatt.Material.Add(material);
											material = new Material(material.GetBytes(), 0);
										}
										break;
								}
							int numVtx = maxVtx - minVtx + 1;
							basatt.ResizeVertexes(numVtx);
							Array.Copy(VertexBuffer, minVtx, basatt.Vertex, 0, numVtx);
							Array.Copy(NormalBuffer, minVtx, basatt.Normal, 0, numVtx);
							if (basatt.Name == "attach_0007C460")
								System.Diagnostics.Debugger.Break();
							foreach (Mesh mesh in basatt.Mesh)
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