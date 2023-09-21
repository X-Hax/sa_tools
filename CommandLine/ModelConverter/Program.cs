using SAModel;
using SAModel.SAEditorCommon.ModelConversion;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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
				filename = Console.ReadLine().Trim('"');
			}
			ModelFormat outfmt;
			if (args.Length > 1)
				outfmt = Enum.Parse<ModelFormat>(args[1], true);
			else
			{
				Console.Write("Format: ");
				outfmt = Enum.Parse<ModelFormat>(Console.ReadLine(), true);
			}
			ModelFile model = new ModelFile(filename);
			if (model.Format == ModelFormat.Chunk && outfmt == ModelFormat.Basic && model.Model.HasWeight)
			{
				WeightedChunkToBasic(model.Model);
			}
			else
			{
				foreach (NJS_OBJECT obj in model.Model.GetObjects().Where(obj => obj.Attach != null))
					switch (outfmt)
					{
						case ModelFormat.Basic:
						case ModelFormat.BasicDX:
							obj.Attach = obj.Attach.ToBasic();
							break;
						case ModelFormat.Chunk:
							obj.Attach = obj.Attach.ToChunk();
							break;
						default:
							throw new Exception($"Output format {outfmt} not supported!");
					}
			}
			switch (outfmt)
			{
				case ModelFormat.Basic:
				case ModelFormat.BasicDX:
					filename = System.IO.Path.ChangeExtension(filename, "sa1mdl");
					break;
				case ModelFormat.Chunk:
					filename = System.IO.Path.ChangeExtension(filename, "sa2mdl");
					break;
				case ModelFormat.GC:
					filename = System.IO.Path.ChangeExtension(filename, "sa2bmdl");
					break;
				case ModelFormat.XJ:
					filename = System.IO.Path.ChangeExtension(filename, "xjmdl");
					break;
			}
			ModelFile.CreateFile(filename, model.Model, null, null, null, null, outfmt);
		}

		static readonly List<PolyChunk>[] PolyCache = new List<PolyChunk>[255];
		static void WeightedChunkToBasic(NJS_OBJECT obj)
		{
			if (obj.Attach is ChunkAttach attach)
				if (attach.Poly != null)
					for (int i = 0; i < attach.Poly.Count; i++)
					{
						switch (attach.Poly[i].Type)
						{
							case ChunkType.Bits_CachePolygonList:
								PolyCache[((PolyChunkBitsCachePolygonList)attach.Poly[i]).List] = attach.Poly.Skip(i + 1).ToList();
								attach.Poly = attach.Poly.Take(i).ToList();
								break;
							case ChunkType.Bits_DrawPolygonList:
								int list = ((PolyChunkBitsDrawPolygonList)attach.Poly[i]).List;
								attach.Poly.RemoveAt(i);
								attach.Poly.InsertRange(i--, PolyCache[list]);
								break;
						}
					}
			WeightedChunkToBasic(obj, new Dictionary<int, List<VertexWeight>>());
		}

		static void WeightedChunkToBasic(NJS_OBJECT obj, Dictionary<int, List<VertexWeight>> weightDict)
		{
			if (obj.Attach is ChunkAttach cnkatt)
			{
				BasicAttach basatt = new BasicAttach() { Name = cnkatt.Name, Bounds = cnkatt.Bounds };
				Vertex[] VertexBuffer = new Vertex[short.MaxValue + 1];
				Array.Fill(VertexBuffer, new Vertex());
				Vertex[] NormalBuffer = new Vertex[short.MaxValue + 1];
				Array.Fill(NormalBuffer, new Vertex());
				Color?[] ColorBuffer = new Color?[short.MaxValue + 1];
				float?[] WeightBuffer = new float?[short.MaxValue + 1];
				int minVtx = int.MaxValue;
				int maxVtx = int.MinValue;
				if (cnkatt.Vertex != null)
					foreach (VertexChunk chunk in cnkatt.Vertex)
					{
						if (chunk.HasWeight)
						{
							for (int i = 0; i < chunk.VertexCount; i++)
							{
								// Store vertex in cache
								var vertexCacheId = (int)(chunk.IndexOffset + (chunk.NinjaFlags[i] & 0xFFFF));

								VertexBuffer[vertexCacheId] = chunk.Vertices[i];
								NormalBuffer[vertexCacheId] = chunk.Normals[i];
								if (chunk.Diffuse.Count > 0)
									ColorBuffer[vertexCacheId] = chunk.Diffuse[i];
								WeightBuffer[vertexCacheId] = (chunk.NinjaFlags[i] >> 16) / 255f;
								if (chunk.WeightStatus == WeightStatus.Start)
									weightDict[vertexCacheId] = new List<VertexWeight>();
								minVtx = Math.Min(minVtx, vertexCacheId);
								maxVtx = Math.Max(maxVtx, vertexCacheId);
							}
						}
						else
						{
							chunk.Vertices.CopyTo(VertexBuffer, chunk.IndexOffset);
							chunk.Normals.CopyTo(NormalBuffer, chunk.IndexOffset);
							if (chunk.Diffuse.Count > 0)
								chunk.Diffuse.Cast<Color?>().ToArray().CopyTo(ColorBuffer, chunk.IndexOffset);
							Array.Fill(WeightBuffer, 1, chunk.IndexOffset, chunk.VertexCount);
							for (int i = 0; i < chunk.VertexCount; i++)
								weightDict[chunk.IndexOffset + i] = new List<VertexWeight>();
							minVtx = Math.Min(minVtx, chunk.IndexOffset);
							maxVtx = Math.Max(maxVtx, chunk.IndexOffset + chunk.VertexCount - 1);
						}
					}
				NJS_MATERIAL material = new NJS_MATERIAL() { UseTexture = true };
				SortedSet<int> usedVerts = new SortedSet<int>();
				if (cnkatt.Poly != null)
					for (int pi = 0; pi < cnkatt.Poly.Count; pi++)
					{
						PolyChunk chunk = cnkatt.Poly[pi];
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
										foreach (ushort ind in strip.Indexes)
											usedVerts.Add(ind);
										strips.Add(new Strip((ushort[])strip.Indexes.Clone(), strip.Reversed));
										if (hasUV)
											uvs.AddRange(strip.UVs);
										if (hasStripVColor)
											vcolors.AddRange(strip.VColors);
										else if (hasVertVColor)
											foreach (ushort i in strip.Indexes)
												vcolors.Add(ColorBuffer[i] ?? Color.White);
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
					}

				int numVtx = maxVtx - minVtx + 1;
				basatt.ResizeVertexes(numVtx);
				Array.Copy(VertexBuffer, minVtx, basatt.Vertex, 0, numVtx);
				Array.Copy(NormalBuffer, minVtx, basatt.Normal, 0, numVtx);
				foreach (NJS_MESHSET mesh in basatt.Mesh)
					foreach (Poly poly in mesh.Poly)
						for (int i = 0; i < poly.Indexes.Length; i++)
							poly.Indexes[i] = (ushort)(poly.Indexes[i] - minVtx);
				for (int i = minVtx; i <= maxVtx; i++)
					if (WeightBuffer[i].HasValue)
						weightDict[i].Add(new VertexWeight(obj, i - minVtx, WeightBuffer[i].Value));
				Dictionary<int, List<VertexWeight>> weights = new Dictionary<int, List<VertexWeight>>();
				foreach (int ind in usedVerts)
					if (weightDict.TryGetValue(ind, out var weight) && (weight.Count > 1 || weight[0].Node != obj))
						weights.Add(ind - minVtx, weight);
				if (weights.Count > 0)
					basatt.VertexWeights = weights;
				obj.Attach = basatt;
				if (basatt.Mesh.Count == 0)
					obj.SkipDraw = true;
			}
			foreach (NJS_OBJECT child in obj.Children)
				WeightedChunkToBasic(child, weightDict);
		}
	}
}