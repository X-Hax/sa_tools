using Assimp;
using SharpDX;
using SonicRetro.SAModel.Direct3D;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonicRetro.SAModel.SAEditorCommon.Import
{
	public static class AssimpStuff
	{
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
			public Vector3 Position { get; private set; }
			public Vector3 Normal { get; private set; }
			public float Weight { get; private set; }

			public WeightData(int index, Vector3 position, Vector3 normal, float weight)
			{
				Index = index;
				Position = position;
				Normal = normal;
				Weight = weight;
			}
		}

		static NJS_MATERIAL MaterialBuffer = new NJS_MATERIAL { UseTexture = true };
		static VertexData[] VertexBuffer = new VertexData[32768];
		static List<WeightData>[] WeightBuffer = new List<WeightData>[32768];
		static readonly CachedPoly[] PolyCache = new CachedPoly[255];
		static List<string> NodeNames;
		static List<Matrix> NodeTransforms;

		public static Node AssimpExportWeighted(this NJS_OBJECT obj, Scene scene, Matrix parentMatrix, string[] texInfo = null, Node parent = null)
		{
			NodeNames = new List<string>();
			NodeTransforms = new List<Matrix>();
			int mdlindex = -1;
			return AssimpExportWeighted(obj, scene, parentMatrix, texInfo, parent, ref mdlindex);
		}

		private static Node AssimpExportWeighted(this NJS_OBJECT obj, Scene scene, Matrix parentMatrix, string[] texInfo, Node parent, ref int mdlindex)
		{
			mdlindex++;
			Node node = null;
			if (parent == null)
				node = new Node(obj.Name);
			else
			{
				node = new Node(obj.Name, parent);
				parent.Children.Add(node);
			}
			NodeNames.Add(obj.Name);

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
			Matrix nodeWorldTransformInv = Matrix.Invert(nodeWorldTransform);
			node.Transform = nodeTransform.ToMatrix4X4();//nodeTransform;

			node.Name = obj.Name;
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
								var origpos = chunk.Vertices[i].ToVector3();
								var position = (Vector3.TransformCoordinate(origpos, nodeWorldTransform) * weight).ToVertex();
								var orignor = Vector3.Up;
								Vertex normal = null;
								if (chunk.Normals.Count > 0)
								{
									orignor = chunk.Normals[i].ToVector3();
									normal = (Vector3.TransformNormal(orignor, nodeWorldTransform) * weight).ToVertex();
								}

								// Store vertex in cache
								var vertexId = chunk.NinjaFlags[i] & 0x0000FFFF;
								var vertexCacheId = (int)(chunk.IndexOffset + vertexId);

								if (chunk.WeightStatus == WeightStatus.Start)
								{
									// Add new vertex to cache
									VertexBuffer[vertexCacheId] = new VertexData(position, normal);
									WeightBuffer[vertexCacheId] = new List<WeightData>
								{
									new WeightData(mdlindex, origpos, orignor, weight)
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
									WeightBuffer[vertexCacheId].Add(new WeightData(mdlindex, origpos, orignor, weight));
								}
							}
						}
						else
							for (int i = 0; i < chunk.VertexCount; i++)
							{
								var origpos = chunk.Vertices[i].ToVector3();
								var position = Vector3.TransformCoordinate(origpos, nodeWorldTransform).ToVertex();
								var orignor = Vector3.Up;
								Vertex normal = null;
								if (chunk.Normals.Count > 0)
								{
									orignor = chunk.Normals[i].ToVector3();
									normal = Vector3.TransformNormal(orignor, nodeWorldTransform).ToVertex();
								}
								VertexBuffer[i + chunk.IndexOffset] = new VertexData(position, normal);
								if (chunk.Diffuse.Count > 0)
									VertexBuffer[i + chunk.IndexOffset].Color = chunk.Diffuse[i];
								WeightBuffer[i + chunk.IndexOffset] = null;
								WeightBuffer[i + chunk.IndexOffset] = new List<WeightData>
							{
								new WeightData(mdlindex, origpos, orignor, 1)
							};
							}
					}
				}
				List<MeshInfo> result = new List<MeshInfo>();
				List<List<WeightData>> weights = new List<List<WeightData>>();
				if (attach.Poly != null)
					result = ProcessPolyList(attach.Poly, 0, weights);
				attach.MeshInfo = result.ToArray();
				int nameMeshIndex = 0;
				foreach (MeshInfo meshInfo in result)
				{
					Assimp.Mesh mesh = new Assimp.Mesh("mesh_" + nameMeshIndex);

					NJS_MATERIAL cur_mat = meshInfo.Material;
					Material materoial = new Material() { Name = "material_" + nameMeshIndex++ }; ;
					materoial.ColorDiffuse = new Color4D(cur_mat.DiffuseColor.R, cur_mat.DiffuseColor.G, cur_mat.DiffuseColor.B, cur_mat.DiffuseColor.A);
					if (cur_mat.UseTexture && texInfo != null)
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
						materoial.AddMaterialTexture(ref tex);
					}
					int matIndex = scene.MaterialCount;
					scene.Materials.Add(materoial);
					mesh.MaterialIndex = matIndex;

					mesh.PrimitiveType = PrimitiveType.Triangle;
					ushort[] tris = meshInfo.ToTriangles();
					List<List<WeightData>> vertexWeights = new List<List<WeightData>>(tris.Length);
					for (int i = 0; i < tris.Length; i += 3)
					{
						Face face = new Face();
						face.Indices.AddRange(new int[] { mesh.Vertices.Count + 2, mesh.Vertices.Count + 1, mesh.Vertices.Count });
						for (int j = 0; j < 3; j++)
						{
							mesh.Vertices.Add(Vector3.TransformCoordinate(meshInfo.Vertices[tris[i + j]].Position.ToVector3(), nodeWorldTransformInv).ToVector3D());
							mesh.Normals.Add(Vector3.TransformNormal(meshInfo.Vertices[tris[i + j]].Normal.ToVector3(), nodeWorldTransformInv).ToVector3D());
							if (meshInfo.Vertices[tris[i + j]].Color.HasValue)
								mesh.VertexColorChannels[0].Add(new Color4D(meshInfo.Vertices[tris[i + j]].Color.Value.R, meshInfo.Vertices[tris[i + j]].Color.Value.G, meshInfo.Vertices[tris[i + j]].Color.Value.B, meshInfo.Vertices[tris[i + j]].Color.Value.A));
							if (meshInfo.Vertices[tris[i + j]].UV != null)
								mesh.TextureCoordinateChannels[0].Add(new Vector3D(meshInfo.Vertices[tris[i + j]].UV.U, meshInfo.Vertices[tris[i + j]].UV.V, 1.0f));
							vertexWeights.Add(weights[tris[i + j]]);
						}
						mesh.Faces.Add(face);
					}

					// Convert vertex weights
					var aiBoneMap = new Dictionary<int, Bone>();
					for (int i = 0; i < vertexWeights.Count; i++)
					{
						for (int j = 0; j < vertexWeights[i].Count; j++)
						{
							var vertexWeight = vertexWeights[i][j];

							if (!aiBoneMap.TryGetValue(vertexWeight.Index, out var aiBone))
							{
								aiBone = aiBoneMap[vertexWeight.Index] = new Bone
								{
									Name = NodeNames[vertexWeight.Index]
								};

								// Offset matrix: difference between world transform of weighted bone node and the world transform of the mesh's parent node
								var offsetMatrix = Matrix.Invert(NodeTransforms[vertexWeight.Index] * nodeWorldTransformInv);
								aiBone.OffsetMatrix = offsetMatrix.ToMatrix4X4();
							}

							// Assimps way of storing weights is not very efficient
							aiBone.VertexWeights.Add(new VertexWeight(i, vertexWeight.Weight));
						}
					}

					mesh.Bones.AddRange(aiBoneMap.Values);
					scene.Meshes.Add(mesh);
				}
				int endMeshIndex = scene.MeshCount;
				for (int i = startMeshIndex; i < endMeshIndex; i++)
				{
					//node.MeshIndices.Add(i);
					Node meshChildNode = new Node("meshnode_" + i);
					meshChildNode.Transform = nodeWorldTransform.ToMatrix4X4();
					scene.RootNode.Children.Add(meshChildNode);
					meshChildNode.MeshIndices.Add(i);
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
						PolyCache[cachenum] = new CachedPoly(strips, i + 1);
						return result;
					case ChunkType.Bits_DrawPolygonList:
						cachenum = ((PolyChunkBitsDrawPolygonList)chunk).List;
						CachedPoly cached = PolyCache[cachenum];
						result.AddRange(ProcessPolyList(cached.Polys, cached.Index, weights));
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
							foreach (PolyChunkStrip.Strip strip in c2.Strips)
							{
								Strip str = new Strip(strip.Indexes.Length, strip.Reversed);
								for (int k = 0; k < strip.Indexes.Length; k++)
								{
									str.Indexes[k] = (ushort)verts.Count;
									verts.Add(new VertexData(
										VertexBuffer[strip.Indexes[k]].Position,
										VertexBuffer[strip.Indexes[k]].Normal,
										hasVColor ? (System.Drawing.Color?)strip.VColors[k] : VertexBuffer[strip.Indexes[k]].Color,
										hasUV ? strip.UVs[k] : null));
									weights.Add(WeightBuffer[strip.Indexes[k]]);
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

		public static Vector3D ToVector3D(this Vector3 v)
		{
			return new Vector3D(v.X, v.Y, v.Z);
		}

		public static Matrix4x4 ToMatrix4X4(this Matrix m)
		{
			return new Matrix4x4(m.M11, m.M21, m.M31, m.M41, m.M12, m.M22, m.M32, m.M42, m.M13, m.M23, m.M33, m.M43, m.M14, m.M24, m.M34, m.M44);
		}
	}
}
