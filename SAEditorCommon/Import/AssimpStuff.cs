using Assimp;
using SharpDX;
using SonicRetro.SAModel.Direct3D;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Color = System.Drawing.Color;

namespace SonicRetro.SAModel.SAEditorCommon.Import
{
	public static class AssimpStuff
	{
		#region Export
		private class CachedPoly
		{
			public List<PolyChunk> Polys { get; private set; }
			public int Index { get; private set; }

			public CachedPoly(List<PolyChunk> polys, int index)
			{
				Polys = polys;
				Index = index;
			}
		}

		internal class WeightData
		{
			public int Index { get; private set; }
			public float Weight { get; private set; }

			public WeightData(int index, float weight)
			{
				Index = index;
				Weight = weight;
			}
		}

		static NJS_MATERIAL MaterialBuffer = new NJS_MATERIAL { UseTexture = true };
		static readonly VertexData[] VertexBuffer = new VertexData[32768];
		static readonly List<WeightData>[] WeightBuffer = new List<WeightData>[32768];
		static readonly CachedPoly[] PolyCache = new CachedPoly[255];
		static List<string> NodeNames;
		static List<Matrix> NodeTransforms;

		public static Node AssimpExport(this NJS_OBJECT obj, Scene scene, Matrix parentMatrix, string[] texInfo = null, Node parent = null)
		{
			if (obj.GetObjects().Any(a => a.Attach is ChunkAttach))
				return AssimpExportWeighted(obj, scene, parentMatrix, texInfo, parent);
			return obj.AssimpExport(scene, texInfo, parent);
		}

		private static Node AssimpExportWeighted(this NJS_OBJECT obj, Scene scene, Matrix parentMatrix, string[] texInfo, Node parent)
		{
			NodeNames = new List<string>();
			NodeTransforms = new List<Matrix>();
			int mdlindex = -1;
			ProcessNodes(obj, parentMatrix, ref mdlindex);
			mdlindex = -1;
			return AssimpExportWeighted(obj, scene, parentMatrix, texInfo, parent, ref mdlindex);
		}

		private static void ProcessNodes(this NJS_OBJECT obj, Matrix parentMatrix, ref int mdlindex)
		{
			mdlindex++;
			string nodename = $"n{mdlindex:000}_{obj.Name}";
			NodeNames.Add(nodename);

			Matrix nodeTransform = Matrix.Identity;

			nodeTransform *= Matrix.Scaling(obj.Scale.X, obj.Scale.Y, obj.Scale.Z);

			float rotX = ((obj.Rotation.X) * (2 * (float)Math.PI) / 65536.0f);
			float rotY = ((obj.Rotation.Y) * (2 * (float)Math.PI) / 65536.0f);
			float rotZ = ((obj.Rotation.Z) * (2 * (float)Math.PI) / 65536.0f);
			Matrix rotation = Matrix.RotationX(rotX) *
					   Matrix.RotationY(rotY) *
					   Matrix.RotationZ(rotZ);
			nodeTransform *= rotation;

			nodeTransform *= Matrix.Translation(obj.Position.X, obj.Position.Y, obj.Position.Z);

			Matrix nodeWorldTransform = nodeTransform * parentMatrix;
			NodeTransforms.Add(nodeWorldTransform);
			if (obj.Children != null)
				foreach (NJS_OBJECT child in obj.Children)
					child.ProcessNodes(nodeWorldTransform, ref mdlindex);
		}

		private static Node AssimpExportWeighted(this NJS_OBJECT obj, Scene scene, Matrix parentMatrix, string[] texInfo, Node parent, ref int mdlindex)
		{
			mdlindex++;
			string nodename = NodeNames[mdlindex];
			Node node;
			if (parent == null)
				node = new Node(nodename);
			else
			{
				node = new Node(nodename, parent);
				parent.Children.Add(node);
			}

			Matrix nodeTransform = Matrix.Identity;

			nodeTransform *= Matrix.Scaling(obj.Scale.X, obj.Scale.Y, obj.Scale.Z);

			float rotX = ((obj.Rotation.X) * (2 * (float)Math.PI) / 65536.0f);
			float rotY = ((obj.Rotation.Y) * (2 * (float)Math.PI) / 65536.0f);
			float rotZ = ((obj.Rotation.Z) * (2 * (float)Math.PI) / 65536.0f);
			Matrix rotation = Matrix.RotationX(rotX) *
					   Matrix.RotationY(rotY) *
					   Matrix.RotationZ(rotZ);
			nodeTransform *= rotation;

			nodeTransform *= Matrix.Translation(obj.Position.X, obj.Position.Y, obj.Position.Z);

			Matrix nodeWorldTransform = NodeTransforms[mdlindex];
			Matrix nodeWorldTransformInv = Matrix.Invert(parentMatrix);
			node.Transform = nodeTransform.ToAssimp();//nodeTransform;

			node.Name = nodename;
			int startMeshIndex = scene.MeshCount;
			if (obj.Attach != null)
			{
				ChunkAttach attach = (ChunkAttach)obj.Attach;
				if (attach.Vertex != null)
				{
					foreach (VertexChunk chunk in attach.Vertex)
					{
						if (chunk.HasWeight)
						{
							for (int i = 0; i < chunk.VertexCount; i++)
							{
								var weightByte = chunk.NinjaFlags[i] >> 16;
								var weight = weightByte / 255f;
								var position = (Vector3.TransformCoordinate(chunk.Vertices[i].ToVector3(), nodeWorldTransform) * weight).ToVertex();
								Vertex normal = null;
								if (chunk.Normals.Count > 0)
									normal = (Vector3.TransformNormal(chunk.Normals[i].ToVector3(), nodeWorldTransform) * weight).ToVertex();

								// Store vertex in cache
								var vertexId = chunk.NinjaFlags[i] & 0x0000FFFF;
								var vertexCacheId = (int)(chunk.IndexOffset + vertexId);

								if (chunk.WeightStatus == WeightStatus.Start)
								{
									// Add new vertex to cache
									VertexBuffer[vertexCacheId] = new VertexData(position, normal);
									WeightBuffer[vertexCacheId] = new List<WeightData>
								{
									new WeightData(mdlindex, weight)
								};
									if (chunk.Diffuse.Count > 0)
										VertexBuffer[vertexCacheId].Color = chunk.Diffuse[i];
								}
								else
								{
									// Update cached vertex
									var cacheVertex = VertexBuffer[vertexCacheId];
									cacheVertex.Position += position;
									cacheVertex.Normal += normal;
									if (chunk.Diffuse.Count > 0)
										cacheVertex.Color = chunk.Diffuse[i];
									VertexBuffer[vertexCacheId] = cacheVertex;
									WeightBuffer[vertexCacheId].Add(new WeightData(mdlindex, weight));
								}
							}
						}
						else
							for (int i = 0; i < chunk.VertexCount; i++)
							{
								var position = Vector3.TransformCoordinate(chunk.Vertices[i].ToVector3(), nodeWorldTransform).ToVertex();
								Vertex normal = null;
								if (chunk.Normals.Count > 0)
									normal = Vector3.TransformNormal(chunk.Normals[i].ToVector3(), nodeWorldTransform).ToVertex();
								VertexBuffer[i + chunk.IndexOffset] = new VertexData(position, normal);
								if (chunk.Diffuse.Count > 0)
									VertexBuffer[i + chunk.IndexOffset].Color = chunk.Diffuse[i];
								WeightBuffer[i + chunk.IndexOffset] = new List<WeightData> { new WeightData(mdlindex, 1) };
							}
					}
				}
				List<MeshInfo> result = new List<MeshInfo>();
				List<List<WeightData>> weights = new List<List<WeightData>>();
				if (attach.Poly != null)
					result = ProcessPolyList(attach.Poly, 0, weights);
				if (result.Count > 0)
				{
					int nameMeshIndex = 0;
					int vertind = 0;
					foreach (MeshInfo meshInfo in result)
					{
						Assimp.Mesh mesh = new Assimp.Mesh($"{attach.Name}_mesh_{nameMeshIndex}");

						NJS_MATERIAL cur_mat = meshInfo.Material;
						Material material = new Material() { Name = $"{attach.Name}_material_{nameMeshIndex++}" }; ;
						material.ColorDiffuse = cur_mat.DiffuseColor.ToAssimp();
						if (cur_mat.UseTexture && texInfo != null && cur_mat.TextureID < texInfo.Length)
						{
							string texPath = Path.GetFileName(texInfo[cur_mat.TextureID]);
							TextureWrapMode wrapU = TextureWrapMode.Wrap;
							TextureWrapMode wrapV = TextureWrapMode.Wrap;
							if (cur_mat.ClampU)
								wrapU = TextureWrapMode.Clamp;
							else if (cur_mat.FlipU)
								wrapU = TextureWrapMode.Mirror;

							if (cur_mat.ClampV)
								wrapV = TextureWrapMode.Clamp;
							else if (cur_mat.FlipV)
								wrapV = TextureWrapMode.Mirror;

							TextureSlot tex = new TextureSlot(texPath, TextureType.Diffuse, 0,
								TextureMapping.FromUV, 0, 1.0f, TextureOperation.Add,
								wrapU, wrapV, 0); //wrapmode and shit add here
							material.AddMaterialTexture(ref tex);
						}
						int matIndex = scene.MaterialCount;
						scene.Materials.Add(material);
						mesh.MaterialIndex = matIndex;

						List<List<WeightData>> vertexWeights = new List<List<WeightData>>(meshInfo.Vertices.Length);
						for (int i = 0; i < meshInfo.Vertices.Length; i++)
						{
							mesh.Vertices.Add(meshInfo.Vertices[i].Position.ToAssimp());
							mesh.Normals.Add(meshInfo.Vertices[i].Normal.ToAssimp());
							if (meshInfo.Vertices[i].Color.HasValue)
								mesh.VertexColorChannels[0].Add(meshInfo.Vertices[i].Color.Value.ToAssimp());
							if (meshInfo.Vertices[i].UV != null)
								mesh.TextureCoordinateChannels[0].Add(new Vector3D(meshInfo.Vertices[i].UV.U, meshInfo.Vertices[i].UV.V, 1.0f));
							vertexWeights.Add(weights[vertind + i]);
						}

						mesh.PrimitiveType = PrimitiveType.Triangle;
						ushort[] tris = meshInfo.ToTriangles();
						for (int i = 0; i < tris.Length; i += 3)
						{
							Face face = new Face();
							face.Indices.AddRange(new int[] { tris[i], tris[i + 1], tris[i + 2] });
							mesh.Faces.Add(face);
						}

						// Convert vertex weights
						var aiBoneMap = new Dictionary<int, Bone>();
						for (int i = 0; i < NodeNames.Count; i++)
							aiBoneMap.Add(i, new Bone() { Name = NodeNames[i], OffsetMatrix = Matrix.Invert(NodeTransforms[i] * Matrix.Translation(0.0001f, 0, 0)).ToAssimp() });
						for (int i = 0; i < vertexWeights.Count; i++)
						{
							for (int j = 0; j < vertexWeights[i].Count; j++)
							{
								var vertexWeight = vertexWeights[i][j];

								var aiBone = aiBoneMap[vertexWeight.Index];

								// Assimps way of storing weights is not very efficient
								aiBone.VertexWeights.Add(new VertexWeight(i, vertexWeight.Weight));
							}
						}

						mesh.Bones.AddRange(aiBoneMap.Values);
						scene.Meshes.Add(mesh);
						vertind += meshInfo.Vertices.Length;
					}
					int endMeshIndex = scene.MeshCount;
					for (int i = startMeshIndex; i < endMeshIndex; i++)
					{
						//node.MeshIndices.Add(i);
						Node meshChildNode = new Node($"meshnode_{i}");
						//meshChildNode.Transform = nodeWorldTransform.ToAssimp();
						scene.RootNode.Children.Add(meshChildNode);
						meshChildNode.MeshIndices.Add(i);
					}
				}
			}
			if (obj.Children != null)
			{
				foreach (NJS_OBJECT child in obj.Children)
					child.AssimpExportWeighted(scene, nodeWorldTransform, texInfo, node, ref mdlindex);
			}
			return node;
		}

		private static List<MeshInfo> ProcessPolyList(List<PolyChunk> strips, int start, List<List<WeightData>> weights)
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
						PolyCache[cachenum] = new CachedPoly(strips, i + 1);
						return result;
					case ChunkType.Bits_DrawPolygonList:
						cachenum = ((PolyChunkBitsDrawPolygonList)chunk).List;
						CachedPoly cached = PolyCache[cachenum];
						result.AddRange(ProcessPolyList(cached.Polys, cached.Index, weights));
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
							foreach (PolyChunkStrip.Strip strip in c2.Strips)
							{
								Strip str = new Strip(strip.Indexes.Length, strip.Reversed);
								for (int k = 0; k < strip.Indexes.Length; k++)
								{
									var v = new VertexData(
										VertexBuffer[strip.Indexes[k]].Position,
										VertexBuffer[strip.Indexes[k]].Normal,
										hasVColor ? (Color?)strip.VColors[k] : VertexBuffer[strip.Indexes[k]].Color,
										hasUV ? strip.UVs[k] : null);
									if (verts.Contains(v))
										str.Indexes[k] = (ushort)verts.IndexOf(v);
									else
									{
										weights.Add(WeightBuffer[strip.Indexes[k]]);
										str.Indexes[k] = (ushort)verts.Count;
										verts.Add(v);
									}
								}
								polys.Add(str);
							}
							result.Add(new MeshInfo(MaterialBuffer, polys.ToArray(), verts.ToArray(), hasUV, hasVColor));
							MaterialBuffer = new NJS_MATERIAL(MaterialBuffer);
						}
						break;
				}
			}
			return result;
		}

		private static Node AssimpExport(this NJS_OBJECT obj, Scene scene, string[] texInfo, Node parent)
		{
			int mdlindex = -1;
			return AssimpExport(obj, scene, texInfo, parent, ref mdlindex);
		}

		private static Node AssimpExport(NJS_OBJECT obj, Scene scene, string[] texInfo, Node parent, ref int mdlindex)
		{
			mdlindex++;
			string nodename = $"n{mdlindex:000}_{obj.Name}";
			Node node;
			if (parent == null)
				node = new Node(nodename);
			else
			{
				node = new Node(nodename, parent);
				parent.Children.Add(node);
			}

			Matrix4x4 nodeTransform = Matrix4x4.Identity;

			nodeTransform *= Matrix4x4.FromScaling(new Vector3D(obj.Scale.X, obj.Scale.Y, obj.Scale.Z));

			float rotX = ((obj.Rotation.X) * (2 * (float)Math.PI) / 65536.0f);
			float rotY = ((obj.Rotation.Y) * (2 * (float)Math.PI) / 65536.0f);
			float rotZ = ((obj.Rotation.Z) * (2 * (float)Math.PI) / 65536.0f);
			Matrix4x4 rotation = Matrix4x4.FromRotationX(rotX) *
					   Matrix4x4.FromRotationY(rotY) *
					   Matrix4x4.FromRotationZ(rotZ);
			nodeTransform *= rotation;

			nodeTransform *= Matrix4x4.FromTranslation(new Vector3D(obj.Position.X, obj.Position.Y, obj.Position.Z));

			node.Transform = nodeTransform;//nodeTransform;

			node.Name = nodename;
			if (obj.Attach != null)
			{
				foreach (int i in ExportAttachMeshes(obj.Attach, scene, texInfo))
				{
					node.MeshIndices.Add(i);
					//Node meshChildNode = new Node("meshnode_" + i);
					//meshChildNode.Transform = thing;
					//scene.RootNode.Children.Add(meshChildNode);
					//meshChildNode.MeshIndices.Add(i);
				}
			}
			if (obj.Children != null)
			{
				foreach (NJS_OBJECT child in obj.Children)
					AssimpExport(child, scene, texInfo, node, ref mdlindex);
			}
			return node;
		}

		public static List<int> ExportAttachMeshes(this Attach attach, Scene scene, string[] texInfo = null)
		{
			List<int> result = new List<int>();
			if (attach is GC.GCAttach gcAttach)
			{
				int nameMeshIndex = 0;
				foreach (GC.GCMesh m in gcAttach.opaqueMeshes)
				{
					result.Add(scene.Meshes.Count);
					scene.Meshes.Add(ExportGCMesh(gcAttach, m, scene, texInfo, ref nameMeshIndex));
				}
				foreach (GC.GCMesh m in gcAttach.translucentMeshes)
				{
					result.Add(scene.Meshes.Count);
					scene.Meshes.Add(ExportGCMesh(gcAttach, m, scene, texInfo, ref nameMeshIndex));
				}
				MaterialBuffer = new NJS_MATERIAL();
			}
			else
			{
				int nameMeshIndex = 0;
				foreach (MeshInfo meshInfo in attach.MeshInfo)
				{
					Assimp.Mesh mesh = new Assimp.Mesh($"{attach.Name}_mesh_{nameMeshIndex}");

					NJS_MATERIAL cur_mat = meshInfo.Material;
					Material material = new Material() { Name = $"{attach.Name}_material_{nameMeshIndex++}" };
					material.ColorDiffuse = cur_mat.DiffuseColor.ToAssimp();
					material.ColorAmbient = cur_mat.DiffuseColor.ToAssimp();
					material.ColorSpecular = cur_mat.SpecularColor.ToAssimp();
					material.Shininess = cur_mat.Exponent;
					if (cur_mat.UseTexture && texInfo != null && cur_mat.TextureID < texInfo.Length)
					{
						string texPath = Path.GetFileName(texInfo[cur_mat.TextureID]);
						TextureWrapMode wrapU = TextureWrapMode.Wrap;
						TextureWrapMode wrapV = TextureWrapMode.Wrap;
						if (cur_mat.ClampU)
							wrapU = TextureWrapMode.Clamp;
						else if (cur_mat.FlipU)
							wrapU = TextureWrapMode.Mirror;

						if (cur_mat.ClampV)
							wrapV = TextureWrapMode.Clamp;
						else if (cur_mat.FlipV)
							wrapV = TextureWrapMode.Mirror;

						TextureSlot tex = new TextureSlot(texPath, TextureType.Diffuse, 0,
							TextureMapping.FromUV, 0, 1.0f, TextureOperation.Add,
							wrapU, wrapV, 0); // Wrap mode doesn't export to Collada
						material.AddMaterialTexture(ref tex);
					}
					int matIndex = scene.MaterialCount;
					scene.Materials.Add(material);
					mesh.MaterialIndex = matIndex;

					for (int i = 0; i < meshInfo.Vertices.Length; i++)
					{
						mesh.Vertices.Add(meshInfo.Vertices[i].Position.ToAssimp());
						mesh.Normals.Add(meshInfo.Vertices[i].Normal.ToAssimp());
						if (meshInfo.Vertices[i].Color.HasValue)
							mesh.VertexColorChannels[0].Add(meshInfo.Vertices[i].Color.Value.ToAssimp());
						if (meshInfo.Vertices[i].UV != null)
							mesh.TextureCoordinateChannels[0].Add(new Vector3D(meshInfo.Vertices[i].UV.U, meshInfo.Vertices[i].UV.V, 1.0f));
					}

					mesh.PrimitiveType = PrimitiveType.Triangle;
					ushort[] tris = meshInfo.ToTriangles();
					for (int i = 0; i < tris.Length; i += 3)
					{
						Face face = new Face();
						face.Indices.AddRange(new int[] { tris[i], tris[i + 1], tris[i + 2] });
						mesh.Faces.Add(face);
					}
					result.Add(scene.Meshes.Count);
					scene.Meshes.Add(mesh);
				}
			}
			return result;
		}

		private static Assimp.Mesh ExportGCMesh(GC.GCAttach gcAttach, GC.GCMesh m, Scene scene, string[] texInfo, ref int nameMeshIndex)
		{
			Assimp.Mesh mesh = new Assimp.Mesh(gcAttach.Name + "_mesh_" + nameMeshIndex);
			nameMeshIndex++;
			mesh.PrimitiveType = PrimitiveType.Triangle;
			List<Vector3D> positions = new List<Vector3D>();
			List<Vector3D> normals = new List<Vector3D>();
			List<Vector3D> texcoords = new List<Vector3D>();
			List<Color4D> colors = new List<Color4D>();
			foreach (GC.GCParameter param in m.parameters)
			{
				if (param.type == GC.ParameterType.Texture)
				{
					GC.TextureParameter tex = param as GC.TextureParameter;
					MaterialBuffer.UseTexture = true;
					MaterialBuffer.TextureID = tex.TextureID;
					if (tex.Tile.HasFlag(GC.GCTileMode.MirrorU))
						MaterialBuffer.FlipU = true;
					if (tex.Tile.HasFlag(GC.GCTileMode.MirrorV))
						MaterialBuffer.FlipV = true;
					if (!tex.Tile.HasFlag(GC.GCTileMode.WrapU))
						MaterialBuffer.ClampU = true;
					if (!tex.Tile.HasFlag(GC.GCTileMode.WrapV))
						MaterialBuffer.ClampV = true;

					MaterialBuffer.ClampU &= tex.Tile.HasFlag(GC.GCTileMode.Unk_1);
					MaterialBuffer.ClampV &= tex.Tile.HasFlag(GC.GCTileMode.Unk_1);
				}
				else if (param.type == GC.ParameterType.TexCoordGen)
				{
					GC.TexCoordGenParameter gen = param as GC.TexCoordGenParameter;
					if (gen.TexGenSrc == GC.GCTexGenSrc.Normal)
						MaterialBuffer.EnvironmentMap = true;
					else MaterialBuffer.EnvironmentMap = false;
				}
				else if (param.type == GC.ParameterType.BlendAlpha)
				{
					GC.BlendAlphaParameter blend = param as GC.BlendAlphaParameter;
					MaterialBuffer.SourceAlpha = blend.NJSourceAlpha;
					MaterialBuffer.DestinationAlpha = blend.NJDestAlpha;
				}
			}

			List<GC.IOVtx> gcPositions = gcAttach.vertexData.Find(x => x.attribute == GC.GCVertexAttribute.Position)?.data;
			List<GC.IOVtx> gcNormals = gcAttach.vertexData.Find(x => x.attribute == GC.GCVertexAttribute.Normal)?.data;
			List<GC.IOVtx> gcColors = gcAttach.vertexData.Find(x => x.attribute == GC.GCVertexAttribute.Color0)?.data;
			List<GC.IOVtx> gcUVs = gcAttach.vertexData.Find(x => x.attribute == GC.GCVertexAttribute.Tex0)?.data;

			foreach (GC.GCPrimitive prim in m.primitives)
			{
				for (int i = 0; i < prim.ToTriangles().Count; i += 3)
				{
					Face newPoly = new Face();
					newPoly.Indices.AddRange(new int[] { positions.Count + 2, positions.Count + 1, positions.Count });
					for (int j = 0; j < 3; j++)
					{
						GC.Vector3 vertex = (GC.Vector3)gcPositions[prim.ToTriangles()[i + j].PositionIndex];
						positions.Add(new Vector3D(vertex.x, vertex.y, vertex.z));
						if (gcNormals != null)
						{
							GC.Vector3 normal = (GC.Vector3)gcNormals[prim.ToTriangles()[i + j].NormalIndex];
							normals.Add(new Vector3D(normal.x, normal.y, normal.z));
						}
						if (gcUVs != null)
						{
							GC.UV uv = (GC.UV)gcUVs[prim.ToTriangles()[i + j].UV0Index];
							texcoords.Add(new Vector3D(uv.XF, uv.YF, 1.0f));
						}
						if (gcUVs != null)
						{
							GC.Color c = (GC.Color)gcColors[(int)prim.ToTriangles()[i + j].Color0Index];
							colors.Add(new Color4D(c.AlphaF, c.BlueF, c.GreenF, c.RedF));//colors.Add( new Color4D(c.A,c.B,c.G,c.R));
						}
					}
					//vertData.Add(new SAModel.VertexData(

					//VertexData.Positions[(int)prim.Vertices[i].PositionIndex],
					//VertexData.Normals.Count > 0 ? VertexData.Normals[(int)prim.Vertices[i].NormalIndex] : new Vector3(0, 1, 0),
					//hasVColor ? VertexData.Color_0[(int)prim.Vertices[i].Color0Index] : new GC.Color { R = 1, G = 1, B = 1, A = 1 },
					//hasUV ? VertexData.TexCoord_0[(int)prim.Vertices[i].UVIndex] : new Vector2() { X = 0, Y = 0 }));
					mesh.Faces.Add(newPoly);
				}
			}

			mesh.Vertices.AddRange(positions);
			if (normals.Count > 0)
				mesh.Normals.AddRange(normals);
			if (texcoords.Count > 0)
				mesh.TextureCoordinateChannels[0].AddRange(texcoords);
			if (colors.Count > 0)
				mesh.VertexColorChannels[0].AddRange(colors);
			Material material = new Material() { Name = "material_" + nameMeshIndex + "_" + MaterialBuffer.TextureID };
			material.ColorDiffuse = MaterialBuffer.DiffuseColor.ToAssimp();
			if (MaterialBuffer.UseTexture && texInfo != null)
			{
				string texPath = Path.GetFileName(texInfo[MaterialBuffer.TextureID]);
				TextureWrapMode wrapU = TextureWrapMode.Wrap;
				TextureWrapMode wrapV = TextureWrapMode.Wrap;
				if (MaterialBuffer.ClampU)
					wrapU = TextureWrapMode.Clamp;
				else if (MaterialBuffer.FlipU)
					wrapU = TextureWrapMode.Mirror;

				if (MaterialBuffer.ClampV)
					wrapV = TextureWrapMode.Clamp;
				else if (MaterialBuffer.FlipV)
					wrapV = TextureWrapMode.Mirror;

				TextureSlot tex = new TextureSlot(texPath, TextureType.Diffuse, 0,
					TextureMapping.FromUV, 0, 1.0f, TextureOperation.Add,
					wrapU, wrapV, 0); //wrapmode and shit add here
				material.AddMaterialTexture(ref tex);

			}
			int matIndex = scene.MaterialCount;
			scene.Materials.Add(material);
			mesh.MaterialIndex = matIndex;
			return mesh;
		}
		#endregion

		#region Import
		static class VertexCacheManager
		{
			struct CacheEntry : IComparable<CacheEntry>
			{
				public int start;
				public int length;
				public int handle;

				public int end => start + length;

				int IComparable<CacheEntry>.CompareTo(CacheEntry other)
				{
					return start.CompareTo(other.start);
				}
			}

			static readonly SortedSet<CacheEntry> entries = new SortedSet<CacheEntry>();
			static int handle = 0;

			public static void Clear()
			{
				entries.Clear();
				handle = 0;
			}

			public static (int start, int handle) Reserve(int length)
			{
				int st = 0;
				foreach (var en in entries)
					if (en.start < st + length)
						st = en.end;
					else
						break;
				if (st + length > short.MaxValue)
					throw new Exception("No space in cache to reserve that many vertices!");
				CacheEntry entry = new CacheEntry
				{
					start = st,
					length = length,
					handle = handle++
				};
				entries.Add(entry);
				return (st, entry.handle);
			}

			public static void Release(int handle)
			{
				foreach (var en in entries)
					if (en.handle == handle)
					{
						entries.Remove(en);
						return;
					}
				throw new ArgumentException($"No cache entry with handle '{handle}' was found.", "handle");
			}
		}

		static Dictionary<string, int> nodeIndexForSort = new Dictionary<string, int>();
		static Dictionary<string, Node> nodemap = new Dictionary<string, Node>();

		private static void FillNodeIndexForSort(Scene scene, Node aiNode, ref int mdlindex)
		{
			mdlindex++;
			nodeIndexForSort.Add(aiNode.Name, mdlindex);
			nodemap.Add(aiNode.Name, aiNode);
			foreach (Node child in aiNode.Children)
				FillNodeIndexForSort(scene, child, ref mdlindex);
		}

		class ChunkMeshData
		{
			public string FirstNode { get; set; }
			public Dictionary<string, List<VertexChunk>> Vertex { get; set; }
			public string LastNode { get; set; }
			public List<PolyChunk> Poly { get; set; }
			public int VertexCount { get; set; }
			public SharpDX.BoundingSphere Bounds { get; set; }
			public int CacheHandle { get; set; }

			public ChunkMeshData(int vcount)
			{
				VertexCount = vcount;
				Vertex = new Dictionary<string, List<VertexChunk>>();
				Poly = new List<PolyChunk>();
			}
		}

		class VertInfo
		{
			public readonly int index;
			public readonly List<VertWeight> weights;

			public VertInfo(int ind)
			{
				index = ind;
				weights = new List<VertWeight>();
			}
		}

		class VertWeight
		{
			public readonly string name;
			public readonly float weight;

			public VertWeight(string name, float weight)
			{
				this.name = name;
				this.weight = weight;
			}
		}

		private static WeightStatus GetWeightStatus(string name, List<VertWeight> weights)
		{
			int i = weights.Select((w, ind) => (ind, w)).First(a => a.w.name == name).ind;
			if (i == 0)
				return WeightStatus.Start;
			else if (i == weights.Count - 1)
				return WeightStatus.End;
			else
				return WeightStatus.Middle;
		}

		static readonly NvTriStripDotNet.NvStripifier nvStripifier = new NvTriStripDotNet.NvStripifier() { StitchStrips = false, UseRestart = false };
		private static ChunkMeshData ProcessMeshChunk(Assimp.Mesh aiMesh, Scene scene, string[] texInfo, out string[] bones)
		{
			ChunkMeshData result = new ChunkMeshData(aiMesh.VertexCount);
			result.Bounds = SharpDX.BoundingSphere.FromPoints(aiMesh.Vertices.Select(a => a.ToSharpDX()).ToArray());
			var verts = new List<VertInfo>(aiMesh.VertexCount);
			for (int i = 0; i < aiMesh.VertexCount; i++)
				verts.Add(new VertInfo(i));
			List<string> sortedbones = new List<string>();
			var matrices = new Dictionary<string, Matrix>();
			if (aiMesh.HasBones)
			{
				bones = aiMesh.Bones.Select(a => a.Name).ToArray();
				foreach (var bone in aiMesh.Bones.Where(a => a.HasVertexWeights).OrderBy(a => nodeIndexForSort[a.Name]))
				{
					sortedbones.Add(bone.Name);
					foreach (var weight in bone.VertexWeights)
						verts[weight.VertexID].weights.Add(new VertWeight(bone.Name, weight.Weight));
					matrices[bone.Name] = bone.OffsetMatrix.ToSharpDX();
				}
			}
			else
				bones = null;
			if (sortedbones.Count > 1)
			{
				result.FirstNode = sortedbones.First();
				string lastbone = sortedbones.Last();
				result.LastNode = lastbone;
				result.Bounds = new SharpDX.BoundingSphere(Vector3.TransformCoordinate(result.Bounds.Center, matrices[lastbone]), result.Bounds.Radius);
				for (int i = 0; i < aiMesh.VertexCount; i++)
					if (verts[i].weights.Count == 0)
						verts[i].weights.Add(new VertWeight(lastbone, 1));
				foreach (var bonename in sortedbones)
				{
					List<VertexChunk> chunks = new List<VertexChunk>();
					Dictionary<WeightStatus, List<VertInfo>> vertsbyweight = new Dictionary<WeightStatus, List<VertInfo>>()
					{
						{ WeightStatus.Start, new List<VertInfo>() },
						{ WeightStatus.Middle, new List<VertInfo>() },
						{ WeightStatus.End, new List<VertInfo>() }
					};
					foreach (var v in verts.Where(a => a.weights.Any(b => b.name == bonename)))
						vertsbyweight[GetWeightStatus(bonename, v.weights)].Add(v);
					foreach (var (weight, vertinds) in vertsbyweight.Where(a => a.Value.Count > 0))
					{
						VertexChunk vc = new VertexChunk(ChunkType.Vertex_VertexNormalNinjaFlags) { IndexOffset = (ushort)vertinds.Min(a => a.index), WeightStatus = weight };
						foreach (var vert in vertinds)
						{
							vc.Vertices.Add(Vector3.TransformCoordinate(aiMesh.Vertices[vert.index].ToSharpDX(), matrices[bonename]).ToVertex());
							vc.Normals.Add(Vector3.TransformNormal(aiMesh.HasNormals ? aiMesh.Normals[vert.index].ToSharpDX() : Vector3.Up, matrices[bonename]).ToVertex());
							vc.NinjaFlags.Add((uint)(((byte)(vert.weights.First(a => a.name == bonename).weight * 255.0f) << 16) | (vert.index - vc.IndexOffset)));
						}
						chunks.Add(vc);
					}

					result.Vertex.Add(bonename, chunks);
				}
			}
			else
			{
				Matrix transform = Matrix.Identity;
				if (sortedbones.Count == 1)
				{
					result.LastNode = result.FirstNode = sortedbones[0];
					transform = matrices[sortedbones[0]];
				}
				result.Bounds = new SharpDX.BoundingSphere(Vector3.TransformCoordinate(result.Bounds.Center, transform), result.Bounds.Radius);
				ChunkType type = ChunkType.Vertex_Vertex;
				bool hasnormal = false;
				bool hasvcolor = false;
				if (aiMesh.HasVertexColors(0))
				{
					hasvcolor = true;
					type = ChunkType.Vertex_VertexDiffuse8;
				}
				else if (aiMesh.HasNormals)
				{
					hasnormal = true;
					type = ChunkType.Vertex_VertexNormal;
				}
				VertexChunk vc = new VertexChunk(type);
				for (int i = 0; i < aiMesh.VertexCount; i++)
				{
					vc.Vertices.Add(Vector3.TransformCoordinate(aiMesh.Vertices[i].ToSharpDX(), transform).ToVertex());
					if (hasnormal)
						vc.Normals.Add(Vector3.TransformNormal(aiMesh.Normals[i].ToSharpDX(), transform).ToVertex());
					if (hasvcolor)
						vc.Diffuse.Add(aiMesh.VertexColorChannels[0][i].ToColor());
				}
				result.Vertex[result.FirstNode ?? string.Empty] = new List<VertexChunk>() { vc };
			}
			bool hasUV = aiMesh.HasTextureCoords(0);
			List<PolyChunkStrip.Strip> polys = new List<PolyChunkStrip.Strip>();
			List<ushort> tris = new List<ushort>();
			foreach (Face aiFace in aiMesh.Faces)
				for (int i = 0; i < 3; i++)
					tris.Add((ushort)aiFace.Indices[i]);

			nvStripifier.GenerateStrips(tris.ToArray(), out var primitiveGroups);
			foreach (NvTriStripDotNet.PrimitiveGroup grp in primitiveGroups)
			{
				bool rev = grp.Indices[1] == grp.Indices[0];
				var stripIndices = new List<ushort>(grp.Indices.Length);
				List<UV> stripuv = new List<UV>();
				for (var j = rev ? 1 : 0; j < grp.Indices.Length; j++)
				{
					var vertexIndex = grp.Indices[j];
					stripIndices.Add(vertexIndex);
					if (hasUV)
						stripuv.Add(new UV() { U = aiMesh.TextureCoordinateChannels[0][vertexIndex].X, V = aiMesh.TextureCoordinateChannels[0][vertexIndex].Y });
				}

				polys.Add(new PolyChunkStrip.Strip(rev, stripIndices.ToArray(), hasUV ? stripuv.ToArray() : null, null));
			}

			//material stuff
			Material currentAiMat = scene.Materials[aiMesh.MaterialIndex];
			if (currentAiMat != null)
			{
				//output mat first then texID, thats how the official exporter worked
				result.Poly.Add(new PolyChunkMaterial() { SourceAlpha = AlphaInstruction.SourceAlpha, DestinationAlpha = AlphaInstruction.InverseSourceAlpha });
				if (currentAiMat.HasTextureDiffuse)
				{
					if (texInfo != null)
					{
						PolyChunkTinyTextureID tinyTexId = new PolyChunkTinyTextureID();
						int texId = 0;
						for (int j = 0; j < texInfo.Length; j++)
							if (texInfo[j] == Path.GetFileNameWithoutExtension(currentAiMat.TextureDiffuse.FilePath))
								texId = j;
						tinyTexId.TextureID = (ushort)texId;
						result.Poly.Add(tinyTexId);
					}
				}
				else if (texInfo != null)
				{
					PolyChunkTinyTextureID tinyTexId = new PolyChunkTinyTextureID();
					int texId = 0;
					for (int j = 0; j < texInfo.Length; j++)
						if (texInfo[j].ToLower() == currentAiMat.Name.ToLower())
							texId = j;
					tinyTexId.TextureID = (ushort)texId;
					result.Poly.Add(tinyTexId);
				}
			}

			PolyChunkStrip strip;
			if (hasUV)
				strip = new PolyChunkStrip(ChunkType.Strip_StripUVN);
			else
				strip = new PolyChunkStrip(ChunkType.Strip_Strip);

			strip.Strips.AddRange(polys);
			result.Poly.Add(strip);
			return result;
		}

		public static NJS_OBJECT AssimpImport(Scene scene, Node node, ModelFormat modelFormat, string[] texInfo = null, bool asSingle = false)
		{
			if (modelFormat == ModelFormat.Chunk)
				foreach (var mesh in scene.Meshes)
					if (mesh.BoneCount > 0)
						return AssimpImportWeighted(scene, texInfo);
			if (node == null || node == scene.RootNode)
			{
				NJS_OBJECT result = AssimpImportNonWeighted(scene, scene.RootNode, modelFormat, texInfo, asSingle);
				return result;
			}
			return AssimpImportNonWeighted(scene, node, modelFormat, texInfo, asSingle);
		}

		private static NJS_OBJECT AssimpImportWeighted(Scene scene, string[] texInfo = null)
		{
			VertexCacheManager.Clear();
			nodeIndexForSort.Clear();
			nodemap.Clear();

			//get node indices for sorting
			int mdlindex = -1;
			FillNodeIndexForSort(scene, scene.RootNode, ref mdlindex);
			List<ChunkMeshData> meshdata = new List<ChunkMeshData>();
			Dictionary<string, int> bonelist = new Dictionary<string, int>();
			foreach (var mesh in scene.Meshes)
			{
				meshdata.Add(ProcessMeshChunk(mesh, scene, texInfo, out var bones));
				if (bones != null)
					foreach (string b in bones)
						if (bonelist.ContainsKey(b))
							bonelist[b]++;
						else
							bonelist[b] = 1;
			}
			Dictionary<Node, int> roots = new Dictionary<Node, int>();
			foreach (var (name, count) in bonelist)
			{
				Node n = nodemap[name];
				while (n.Parent != scene.RootNode)
				{
					n = n.Parent;
				}
				if (roots.ContainsKey(n))
					roots[n] += count;
				else
					roots[n] = count;
			}
			mdlindex = -1;
			return AssimpImportWeighted(roots.OrderByDescending(a => a.Value).First().Key, meshdata, ref mdlindex);
		}

		static readonly System.Text.RegularExpressions.Regex nodenameregex = new System.Text.RegularExpressions.Regex("^n[0-9]{3}_", System.Text.RegularExpressions.RegexOptions.CultureInvariant);
		private static NJS_OBJECT AssimpImportWeighted(Node aiNode, List<ChunkMeshData> meshdata, ref int mdlindex)
		{
			NJS_OBJECT obj = new NJS_OBJECT { Animate = true, Morph = true };
			if (nodenameregex.IsMatch(aiNode.Name))
				obj.Name = aiNode.Name.Substring(5);
			else
				obj.Name = aiNode.Name;
			aiNode.Transform.Decompose(out Vector3D scaling, out Assimp.Quaternion rotation, out Vector3D translation);
			Vector3D rotationConverted = rotation.ToEulerAngles();
			obj.Position = new Vertex(translation.X, translation.Y, translation.Z);
			//Rotation = new Rotation(0, 0, 0);
			obj.Rotation = new Rotation(Rotation.DegToBAMS(rotationConverted.X), Rotation.DegToBAMS(rotationConverted.Y), Rotation.DegToBAMS(rotationConverted.Z));
			obj.Scale = new Vertex(scaling.X, scaling.Y, scaling.Z);

			ChunkAttach attach = null;
			SharpDX.BoundingSphere bounds = new SharpDX.BoundingSphere();
			List<int> releasehandles = new List<int>();
			if (meshdata.Any(a => a.Vertex.ContainsKey(aiNode.Name)))
			{
				attach = new ChunkAttach(true, meshdata.Any(a => a.LastNode == aiNode.Name) || aiNode.HasMeshes);
				obj.Attach = attach;
				foreach (var mesh in meshdata.Where(a => a.Vertex.ContainsKey(aiNode.Name)))
				{
					if (mesh.FirstNode == aiNode.Name)
					{
						(int vstart, int handle) = VertexCacheManager.Reserve(mesh.VertexCount);
						mesh.CacheHandle = handle;
						foreach (var vc in mesh.Vertex.Values.SelectMany(a => a))
							vc.IndexOffset += (ushort)vstart;
						foreach (var str in mesh.Poly.OfType<PolyChunkStrip>().SelectMany(a => a.Strips))
							for (int i = 0; i < str.Indexes.Length; i++)
								str.Indexes[i] += (ushort)vstart;
					}
					attach.Vertex.AddRange(mesh.Vertex[aiNode.Name]);
					if (mesh.LastNode == aiNode.Name)
					{
						attach.Poly.AddRange(mesh.Poly);
						if (bounds.Radius == 0)
							bounds = mesh.Bounds;
						else
							bounds = SharpDX.BoundingSphere.Merge(bounds, mesh.Bounds);
						releasehandles.Add(mesh.CacheHandle);
					}
				}
			}
			if (aiNode.HasMeshes)
			{
				if (attach == null)
				{
					attach = new ChunkAttach(true, true);
					obj.Attach = attach;
				}
				foreach (var mesh in aiNode.MeshIndices.Select(i => meshdata[i]))
				{
					(int vstart, int handle) = VertexCacheManager.Reserve(mesh.VertexCount);
					foreach (var vc in mesh.Vertex[string.Empty])
						vc.IndexOffset += (ushort)vstart;
					foreach (var str in mesh.Poly.OfType<PolyChunkStrip>().SelectMany(a => a.Strips))
						for (int i = 0; i < str.Indexes.Length; i++)
							str.Indexes[i] += (ushort)vstart;
					attach.Vertex.AddRange(mesh.Vertex[string.Empty]);
					attach.Poly.AddRange(mesh.Poly);
					if (bounds.Radius == 0)
						bounds = mesh.Bounds;
					else
						bounds = SharpDX.BoundingSphere.Merge(bounds, mesh.Bounds);
					releasehandles.Add(handle);
				}
			}
			if (attach != null)
			{
				attach.Vertex = VertexChunk.Merge(attach.Vertex);
				attach.Bounds = bounds.ToSAModel();
			}
			foreach (int handle in releasehandles)
				VertexCacheManager.Release(handle);
			foreach (Node child in aiNode.Children)
			{
				NJS_OBJECT c = AssimpImportWeighted(child, meshdata, ref mdlindex);
				//HACK: workaround for those weird empty nodes created by most 3d editors
				if (child.Name == "")
				{
					c.Children[0].Position = c.Position;
					c.Children[0].Rotation = c.Rotation;
					c.Children[0].Scale = c.Scale;
					c = c.Children[0];
				}
				obj.AddChild(c);
			}
			return obj;
		}

		private static NJS_OBJECT AssimpImportNonWeighted(Scene scene, Node node, ModelFormat format, string[] textures = null, bool asSingle = false)
		{
			NJS_OBJECT obj = new NJS_OBJECT() { Animate = true, Morph = true };
			if (nodenameregex.IsMatch(node.Name))
				obj.Name = node.Name.Substring(5);
			else
				obj.Name = node.Name;
			node.Transform.Decompose(out Vector3D scaling, out Assimp.Quaternion rotation, out Vector3D translation);
			Vector3D rotationConverted = rotation.ToEulerAngles();
			obj.Position = new Vertex(translation.X, translation.Y, translation.Z);
			obj.Rotation = new Rotation(Rotation.DegToBAMS(rotationConverted.X), Rotation.DegToBAMS(rotationConverted.Y), Rotation.DegToBAMS(rotationConverted.Z));
			obj.Scale = new Vertex(scaling.X, scaling.Y, scaling.Z);
			List<Assimp.Mesh> meshes = new List<Assimp.Mesh>();

			// Import all meshes into a single NJS_OBJECT regardless of nodes 
			if (asSingle)
				meshes = new List<Assimp.Mesh>(scene.Meshes);

			// Import nodes separately
			else if (!asSingle && node.HasMeshes)
				meshes = new List<Assimp.Mesh>(node.MeshIndices.Select(a => scene.Meshes[a]));

			// Proceed with import
			if (asSingle || node.HasMeshes)
			{
				switch (format)
				{
					case ModelFormat.Basic:
					case ModelFormat.BasicDX:
						obj.Attach = AssimpImportBasic(scene.Materials, meshes, textures);
						break;
					case ModelFormat.Chunk:
						obj.Attach = AssimpImportChunk(scene.Materials, meshes, textures);
						break;
					case ModelFormat.GC:
						obj.Attach = AssimpImportGC(scene.Materials, meshes, textures);
						break;
				}
			}

			// Proceed child nodes if nodes are imported separately
			if (!asSingle && node.HasChildren)
			{
				foreach (Node n in node.Children)
				{
					NJS_OBJECT t = AssimpImportNonWeighted(scene, n, format, textures, false);
					//HACK: workaround for those weird empty nodes created by most 3d editors
					if (n.Name == "")
					{
						t.Children[0].Position = t.Position;
						t.Children[0].Rotation = t.Rotation;
						t.Children[0].Scale = t.Scale;
						obj.AddChild(t.Children[0]);
					}
					else
						obj.AddChild(t);
				}
			}
			return obj;
		}

		private static BasicAttach AssimpImportBasic(List<Material> materials, List<Assimp.Mesh> meshes, string[] textures = null)
		{
			BasicAttach attach = new BasicAttach();
			attach.Name = "attach_" + Extensions.GenerateIdentifier();
			attach.Bounds = new BoundingSphere();
			attach.Material = new List<NJS_MATERIAL>();
			attach.MaterialName = "matlist_" + Extensions.GenerateIdentifier();
			attach.Mesh = new List<NJS_MESHSET>();
			attach.MeshName = "meshlist_" + Extensions.GenerateIdentifier();
			attach.VertexName = "vertex_" + Extensions.GenerateIdentifier();
			attach.NormalName = "normal_" + Extensions.GenerateIdentifier();

			List<Vertex> vertices = new List<Vertex>();
			List<Vertex> normals = new List<Vertex>();
			Dictionary<int, int> lookupMaterial = new Dictionary<int, int>();
			foreach (Assimp.Mesh m in meshes)
			{
				foreach (Vector3D ve in m.Vertices)
				{
					vertices.Add(new Vertex(ve.X, ve.Y, ve.Z));
				}
				foreach (Vector3D ve in m.Normals)
				{
					normals.Add(new Vertex(ve.X, ve.Y, ve.Z));
				}
				if (!lookupMaterial.ContainsKey(m.MaterialIndex)) 
					lookupMaterial.Add(m.MaterialIndex, attach.Material.Count);
				attach.Material.Add(materials[m.MaterialIndex].ToSAModel());

				if (materials[m.MaterialIndex].HasTextureDiffuse)
				{
					if (textures != null)
					{
						attach.Material[attach.Material.Count - 1].UseTexture = true;
						for (int i = 0; i < textures.Length; i++)
							if (textures[i] == Path.GetFileNameWithoutExtension(materials[m.MaterialIndex].TextureDiffuse.FilePath))
								attach.Material[attach.Material.Count - 1].TextureID = i;
					}
				}
				else if (textures != null)
				{
					for (int i = 0; i < textures.Length; i++)
						if (textures[i].ToLower() == materials[m.MaterialIndex].Name.ToLower())
						{
							attach.Material[attach.Material.Count - 1].TextureID = i;
							attach.Material[attach.Material.Count - 1].UseTexture = true;
						}
				}
			}
			attach.ResizeVertexes(vertices.Count);
			vertices.CopyTo(attach.Vertex);
			normals.CopyTo(attach.Normal);

			int polyIndex = 0;
			List<NJS_MESHSET> meshsets = new List<NJS_MESHSET>();
			foreach (Assimp.Mesh aiMesh in meshes)
			{
				bool hasUV = aiMesh.HasTextureCoords(0);
				bool hasVColor = aiMesh.HasVertexColors(0);

				List<ushort> tris = new List<ushort>();
				foreach (Face aiFace in aiMesh.Faces)
					for (int i = 0; i < 3; i++)
						tris.Add((ushort)aiFace.Indices[i]);

				List<Poly> polys = new List<Poly>();
				List<UV> stripuv = new List<UV>();
				List<Color> stripvc = new List<Color>();
				nvStripifier.GenerateStrips(tris.ToArray(), out var primitiveGroups);
				foreach (NvTriStripDotNet.PrimitiveGroup grp in primitiveGroups)
				{
					bool rev = grp.Indices[1] == grp.Indices[0];
					var stripIndices = new List<ushort>(grp.Indices.Length);
					for (var j = rev ? 1 : 0; j < grp.Indices.Length; j++)
					{
						var vertexIndex = grp.Indices[j];
						stripIndices.Add((ushort)(vertexIndex + polyIndex));
						if (hasUV)
							stripuv.Add(new UV() { U = aiMesh.TextureCoordinateChannels[0][vertexIndex].X, V = aiMesh.TextureCoordinateChannels[0][vertexIndex].Y });
						if (hasVColor)
							stripvc.Add(aiMesh.VertexColorChannels[0][vertexIndex].ToColor());
					}

					polys.Add(new Strip(stripIndices.ToArray(), rev));
				}

				NJS_MESHSET meshset = new NJS_MESHSET(polys.ToArray(), false, hasUV, hasVColor)
				{
					PolyName = "poly_" + Extensions.GenerateIdentifier(),
					MaterialID = (ushort)lookupMaterial[aiMesh.MaterialIndex]
				};

				if (hasUV)
				{
					meshset.UVName = "uv_" + Extensions.GenerateIdentifier();
					stripuv.CopyTo(meshset.UV);
				}

				if (hasVColor)
				{
					meshset.VColorName = "vcolor_" + Extensions.GenerateIdentifier();
					stripvc.CopyTo(meshset.VColor);
				}
				polyIndex += aiMesh.VertexCount;
				meshsets.Add(meshset);//4B4834
			}
			attach.Mesh = meshsets;
			attach.Bounds = new BoundingSphere() { Radius = 1.0f };
			return attach;
		}

		private static ChunkAttach AssimpImportChunk(List<Material> materials, List<Assimp.Mesh> meshes, string[] textures = null)
		{
			ChunkAttach attach = new ChunkAttach(true, true);
			List<List<Strip>> strips = new List<List<Strip>>();
			List<List<List<UV>>> uvs = new List<List<List<UV>>>();
			VertexChunk vertexChunk;
			ChunkType type = ChunkType.Vertex_Vertex;
			bool hasnormal = false;
			bool hasvcolor = false;
			if (meshes.Any(a => a.HasVertexColors(0)))
			{
				hasvcolor = true;
				type = ChunkType.Vertex_VertexDiffuse8;
			}
			else if (meshes.Any(a => a.HasNormals))
			{
				hasnormal = true;
				type = ChunkType.Vertex_VertexNormal;
			}
			vertexChunk = new VertexChunk(type);
			foreach (Assimp.Mesh aiMesh in meshes)
			{
				int vertoff = vertexChunk.Vertices.Count;
				for (int i = 0; i < aiMesh.VertexCount; i++)
				{
					vertexChunk.Vertices.Add(aiMesh.Vertices[i].ToSAModel());
					if (hasnormal)
						vertexChunk.Normals.Add(aiMesh.Normals[i].ToSAModel());
					if (hasvcolor)
						vertexChunk.Diffuse.Add(aiMesh.VertexColorChannels[0][i].ToColor());
				}
				List<Strip> polys = new List<Strip>();
				List<List<UV>> us = null;
				bool hasUV = aiMesh.HasTextureCoords(0);

				List<ushort> tris = new List<ushort>();
				foreach (Face aiFace in aiMesh.Faces)
					for (int i = 0; i < 3; i++)
						tris.Add((ushort)aiFace.Indices[i]);

				if (hasUV)
					us = new List<List<UV>>();

				nvStripifier.GenerateStrips(tris.ToArray(), out var primitiveGroups);
				foreach (NvTriStripDotNet.PrimitiveGroup grp in primitiveGroups)
				{
					bool rev = grp.Indices[1] == grp.Indices[0];
					var stripIndices = new List<ushort>(grp.Indices.Length);
					List<UV> stripuv = new List<UV>();
					for (var j = rev ? 1 : 0; j < grp.Indices.Length; j++)
					{
						var vertexIndex = grp.Indices[j];
						stripIndices.Add((ushort)(vertexIndex + vertoff));
						if (hasUV)
							stripuv.Add(new UV() { U = aiMesh.TextureCoordinateChannels[0][vertexIndex].X, V = aiMesh.TextureCoordinateChannels[0][vertexIndex].Y });
					}

					polys.Add(new Strip(stripIndices.ToArray(), rev));
					if (hasUV)
						us.Add(stripuv);
					//PolyChunkStrip.Strip strp = new PolyChunkStrip.Strip(false, grp.Indices, null, null);
					//strip.Strips.Add(strp);
				}
				strips.Add(polys);
				uvs.Add(us);
			}
			attach.Vertex.Add(vertexChunk);
			for (int i = 0; i < meshes.Count; i++)
			{
				Assimp.Mesh aiMesh = meshes[i];

				//material stuff
				Material currentAiMat = materials[aiMesh.MaterialIndex];
				if (currentAiMat != null)
				{
					//output mat first then texID, thats how the official exporter worked
					attach.Poly.Add(new PolyChunkMaterial() { SourceAlpha = AlphaInstruction.SourceAlpha, DestinationAlpha = AlphaInstruction.InverseSourceAlpha });
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

		private static GC.GCAttach AssimpImportGC(List<Material> materials, List<Assimp.Mesh> meshes, string[] textures = null)
		{
			GC.GCAttach attach = new GC.GCAttach
			{
				Name = "attach_" + Extensions.GenerateIdentifier()
			};
			//setup names!!!
			List<GC.GCMesh> gcmeshes = new List<GC.GCMesh>();
			List<Vector3D> vertices = new List<Vector3D>();
			List<Vector3D> normals = new List<Vector3D>();
			List<GC.UV> texcoords = new List<GC.UV>();
			List<GC.Color> colors = new List<GC.Color>();
			List<GC.GCVertexSet> vertexAttribs = new List<GC.GCVertexSet>();
			foreach (Assimp.Mesh m in meshes)
			{
				Material currentAiMat = null;
				if (m.MaterialIndex >= 0)
				{
					currentAiMat = materials[m.MaterialIndex];
				}

				List<GC.GCPrimitive> primitives = new List<GC.GCPrimitive>();
				List<GC.GCParameter> parameters = new List<GC.GCParameter>();

				ushort vertStartIndex = (ushort)vertices.Count;
				ushort normStartIndex = (ushort)normals.Count;
				ushort uvStartIndex = (ushort)texcoords.Count;
				ushort colorStartIndex = (ushort)colors.Count;
				vertices.AddRange(m.Vertices);
				if (m.HasNormals)
				{
					normals.AddRange(m.Normals);
				}
				if (m.HasTextureCoords(0))
				{
					foreach (Vector3D texcoord in m.TextureCoordinateChannels[0])
						texcoords.Add(new GC.UV(texcoord.X, texcoord.Y));
				}
				if (m.HasVertexColors(0))
				{
					foreach (Color4D texcoord in m.VertexColorChannels[0])
						colors.Add(new GC.Color(texcoord.A, texcoord.B, texcoord.G, texcoord.R));//colors.Add(new Color(texcoord.A * 255.0f, texcoord.B * 255.0f, texcoord.G * 255.0f, texcoord.R * 255.0f));
																																  //colors.Add(new Color4D(c.A / 255.0f, c.B / 255.0f, c.G / 255.0f, c.R / 255.0f)); rgba
				}

				//Parameter stuff
				GC.GCIndexAttributeFlags indexattr = GC.GCIndexAttributeFlags.HasPosition | GC.GCIndexAttributeFlags.Position16BitIndex;
				if (m.HasVertexColors(0))
					indexattr |= GC.GCIndexAttributeFlags.HasColor | GC.GCIndexAttributeFlags.Color16BitIndex;
				else
					indexattr |= GC.GCIndexAttributeFlags.HasNormal | GC.GCIndexAttributeFlags.Normal16BitIndex;

				if (m.HasTextureCoords(0))
					indexattr |= GC.GCIndexAttributeFlags.HasUV | GC.GCIndexAttributeFlags.UV16BitIndex;

				parameters.Add(new GC.IndexAttributeParameter(indexattr));

				parameters.Add(new GC.VtxAttrFmtParameter(GC.GCVertexAttribute.Position));

				if (m.HasVertexColors(0))
				{
					parameters.Add(new GC.VtxAttrFmtParameter(GC.GCVertexAttribute.Color0));
				}
				else
				{
					parameters.Add(new GC.VtxAttrFmtParameter(GC.GCVertexAttribute.Normal));
				}
				if (m.HasTextureCoords(0))
				{
					parameters.Add(new GC.VtxAttrFmtParameter(GC.GCVertexAttribute.Tex0));
				}

				if (m.HasVertexColors(0))
				{
					parameters.Add(new GC.LightingParameter(0xB11, 1));
				}
				else
				{
					parameters.Add(new GC.LightingParameter(0x211, 1));
				}

				if (currentAiMat != null)
				{
					if (currentAiMat.HasTextureDiffuse)
					{
						if (textures != null)
						{
							int texId = 0;
							GC.GCTileMode tileMode = GC.GCTileMode.Mask;
							switch (currentAiMat.TextureDiffuse.WrapModeU)
							{
								case TextureWrapMode.Clamp:
									tileMode &= ~GC.GCTileMode.WrapU;
									break;
								case TextureWrapMode.Mirror:
									tileMode &= ~GC.GCTileMode.MirrorU;
									break;
							}
							switch (currentAiMat.TextureDiffuse.WrapModeV)
							{
								case TextureWrapMode.Clamp:
									tileMode &= ~GC.GCTileMode.WrapV;
									break;
								case TextureWrapMode.Mirror:
									tileMode &= ~GC.GCTileMode.MirrorV;
									break;
							}
							for (int i = 0; i < textures.Length; i++)
								if (textures[i] == Path.GetFileNameWithoutExtension(currentAiMat.TextureDiffuse.FilePath))
									texId = i;
							GC.TextureParameter texParam = new GC.TextureParameter((ushort)texId, tileMode);
							parameters.Add(texParam);

						}
					}
					else if (textures != null)
					{
						int texId = 0;
						for (int i = 0; i < textures.Length; i++)
							if (textures[i].ToLower() == currentAiMat.Name.ToLower())
								texId = i;
						GC.TextureParameter texParam = new GC.TextureParameter((ushort)texId, GC.GCTileMode.WrapU | GC.GCTileMode.WrapV);
						parameters.Add(texParam);
					}
				}
				List<ushort> tris = new List<ushort>();

				foreach (Face f in m.Faces)
				{
					foreach (int index in f.Indices)
					{
						tris.Add((ushort)index);
					}
				}
				nvStripifier.GenerateStrips(tris.ToArray(), out var primitiveGroups);
				
				foreach (NvTriStripDotNet.PrimitiveGroup grp in primitiveGroups)
				{
					GC.GCPrimitive prim = new GC.GCPrimitive(GC.GCPrimitiveType.TriangleStrip);
					for (var j = 0; j < grp.Indices.Length; j++)
					{
						GC.Loop vert = new GC.Loop();
						vert.PositionIndex = (ushort)(vertStartIndex + grp.Indices[j]);
						if (m.HasTextureCoords(0))
							vert.UV0Index = (ushort)(uvStartIndex + grp.Indices[j]);
						if (m.HasVertexColors(0))
							vert.Color0Index = (ushort)(colorStartIndex + grp.Indices[j]);
						else if (m.HasNormals)
							vert.NormalIndex = (ushort)(normStartIndex + grp.Indices[j]);
						prim.loops.Add(vert);
					}
					primitives.Add(prim);
				}
				gcmeshes.Add(new GC.GCMesh(parameters, primitives));
			}

			List<GC.Vector3> gcvertices = new List<GC.Vector3>();
			foreach (Vector3D aivert in vertices)
				gcvertices.Add(new GC.Vector3(aivert.X, aivert.Y, aivert.Z));

			List<GC.Vector3> gcnormals = new List<GC.Vector3>();
			foreach (Vector3D aivert in normals)
				gcnormals.Add(new GC.Vector3(aivert.X, aivert.Y, aivert.Z));

			//VertexAttribute stuff
			GC.GCVertexSet vtxPositions = new GC.GCVertexSet(GC.GCVertexAttribute.Position);
			vtxPositions.data.AddRange(gcvertices);
			attach.vertexData.Add(vtxPositions);

			if (texcoords.Count > 0)
			{
				GC.GCVertexSet vtxUV = new GC.GCVertexSet(GC.GCVertexAttribute.Tex0);
				vtxUV.data.AddRange(texcoords);
				attach.vertexData.Add(vtxUV);
			}

			if (colors.Count > 0)
			{
				GC.GCVertexSet vtxColors = new GC.GCVertexSet(GC.GCVertexAttribute.Color0);
				vtxColors.data.AddRange(colors);
				attach.vertexData.Add(vtxColors);
			}
			else
			{
				GC.GCVertexSet vtxNormals = new GC.GCVertexSet(GC.GCVertexAttribute.Normal);
				vtxNormals.data.AddRange(gcnormals);
				attach.vertexData.Add(vtxNormals);
			}

			attach.opaqueMeshes.AddRange(gcmeshes);
			return attach;
		}
		#endregion

		//https://stackoverflow.com/questions/12088610/conversion-between-euler-quaternion-like-in-unity3d-engine
		static Vector3D NormalizeAngles(Vector3D angles)
		{
			angles.X = NormalizeAngle(angles.X);
			angles.Y = NormalizeAngle(angles.Y);
			angles.Z = NormalizeAngle(angles.Z);
			return angles;
		}

		static float NormalizeAngle(float angle)
		{
			while (angle > 360)
				angle -= 360;
			while (angle < 0)
				angle += 360;
			return angle;
		}

		public static Vector3D ToEulerAngles(this Assimp.Quaternion q)
		{
			// https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
			// roll (x-axis rotation)
			var sinr = +2.0 * (q.W * q.X + q.Y * q.Z);
			var cosr = +1.0 - 2.0 * (q.X * q.X + q.Y * q.Y);
			var roll = Math.Atan2(sinr, cosr);

			// pitch (y-axis rotation)
			var sinp = +2.0 * (q.W * q.Y - q.Z * q.X);
			double pitch;
			if (Math.Abs(sinp) >= 1)
			{
				var sign = sinp < 0 ? -1f : 1f;
				pitch = (Math.PI / 2) * sign; // use 90 degrees if out of range
			}
			else
			{
				pitch = Math.Asin(sinp);
			}

			// yaw (z-axis rotation)
			var siny = +2.0 * (q.W * q.Z + q.X * q.Y);
			var cosy = +1.0 - 2.0 * (q.Y * q.Y + q.Z * q.Z);
			var yaw = Math.Atan2(siny, cosy);

			return new Vector3D((float)(roll * 57.2958d), (float)(pitch * 57.2958d), (float)(yaw * 57.2958d));
		}

		public static Vector3D ToAssimp(this Vector3 v) => new Vector3D(v.X, v.Y, v.Z);

		public static Vector3 ToSharpDX(this Vector3D v) => new Vector3(v.X, v.Y, v.Z);

		public static Vector3D ToAssimp(this Vertex v) => new Vector3D(v.X, v.Y, v.Z);

		public static Vertex ToSAModel(this Vector3D v) => new Vertex(v.X, v.Y, v.Z);

		public static Color4D ToAssimp(this Color c) => new Color4D(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);

		public static Color ToColor(this Color4D c) => Color.FromArgb((int)(c.A * 255), (int)(c.R * 255), (int)(c.G * 255), (int)(c.B * 255));

		public static NJS_MATERIAL ToSAModel(this Material mat)
		{
			NJS_MATERIAL nj = new NJS_MATERIAL();
			if (mat.HasColorAmbient)
				nj.DiffuseColor = mat.ColorAmbient.ToColor();
			else if (mat.HasColorDiffuse)
				nj.DiffuseColor = mat.ColorDiffuse.ToColor();
			if (nj.DiffuseColor.A == 255) nj.UseAlpha = false;
			if (mat.HasColorSpecular)
				nj.SpecularColor = mat.ColorSpecular.ToColor();
			if (mat.HasTextureDiffuse)
			{
				if (mat.TextureDiffuse.WrapModeU == TextureWrapMode.Clamp)
					nj.ClampU = true;
				if (mat.TextureDiffuse.WrapModeV == TextureWrapMode.Clamp)
					nj.ClampV = true;
				if (mat.TextureDiffuse.WrapModeU == TextureWrapMode.Mirror)
					nj.FlipU = true;
				if (mat.TextureDiffuse.WrapModeV == TextureWrapMode.Mirror)
					nj.FlipV = true;
				nj.UseTexture = true;
			}
			nj.Exponent = mat.Shininess;
			return nj;
		}

		public static Matrix4x4 ToAssimp(this Matrix m) => new Matrix4x4(m.M11, m.M21, m.M31, m.M41, m.M12, m.M22, m.M32, m.M42, m.M13, m.M23, m.M33, m.M43, m.M14, m.M24, m.M34, m.M44);

		public static Matrix ToSharpDX(this Matrix4x4 m) => new Matrix(m.A1, m.B1, m.C1, m.D1, m.A2, m.B2, m.C2, m.D2, m.A3, m.B3, m.C3, m.D3, m.A4, m.B4, m.C4, m.D4);
	}
}
