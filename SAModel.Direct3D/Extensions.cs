using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Color = System.Drawing.Color;

namespace SonicRetro.SAModel.Direct3D
{
	public static partial class Extensions
	{
		public static Vector3 ToVector3(this Vertex vert) => new Vector3(vert.X, vert.Y, vert.Z);

		public static Vector3 ToVector3(this Rotation rotation) => new Vector3(rotation.XDeg, rotation.YDeg, rotation.ZDeg);

		#region Project Point On Plane
		// acquired from here: https://stackoverflow.com/questions/28653628/getting-closest-point-on-a-plane
		public static Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
		{
			float distance;
			Vector3 translationVector;

			//First calculate the distance from the point to the plane:
			distance = SignedDistancePlanePoint(planeNormal, planePoint, point);

			//Reverse the sign of the distance
			distance *= -1;

			//Get a translation vector
			translationVector = SetVectorLength(planeNormal, distance);

			//Translate the point to form a projection
			return point + translationVector;
		}

		//Get the shortest distance between a point and a plane. The output is signed so it holds information
		//as to which side of the plane normal the point is.
		public static float SignedDistancePlanePoint(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
		{
			return Vector3.Dot(planeNormal, (point - planePoint));
		}

		//create a vector of direction "vector" with length "size"
		public static Vector3 SetVectorLength(Vector3 vector, float size)
		{
			//normalize the vector
			Vector3 vectorNormalized = Vector3.Normalize(vector);

			//scale the vector
			return vectorNormalized *= size;
		}
		#endregion

		public static Vertex ToVertex(this Vector3 input) => new Vertex(input.X, input.Y, input.Z);

		public static BoundingSphere ToSAModel(this SharpDX.BoundingSphere sphere) => new BoundingSphere(sphere.Center.ToVertex(), sphere.Radius);

		public static SharpDX.BoundingSphere ToSharpDX(this BoundingSphere sphere) => new SharpDX.BoundingSphere(sphere.Center.ToVector3(), sphere.Radius);

		public static SharpDX.Mathematics.Interop.RawColor4 ToRawColor4(this Color color) => new SharpDX.Mathematics.Interop.RawColor4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);

		public static SharpDX.Mathematics.Interop.RawColorBGRA ToRawColorBGRA(this Color color) => new SharpDX.Mathematics.Interop.RawColorBGRA(color.B, color.G, color.R, color.A);

		public static SharpDX.Mathematics.Interop.RawColor4 ToRawColor4(this Vertex vertex) => new SharpDX.Mathematics.Interop.RawColor4(vertex.X, vertex.Y, vertex.Z, 1);

		public static Texture ToTexture(this Bitmap bitmap, Device device)
		{
			//if (bitmap.PixelFormat != PixelFormat.Format32bppArgb) bitmap = bitmap.Clone(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format32bppArgb); // May no longer be necessary
			BitmapData bmpData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
			int depth = Image.GetPixelFormatSize(bmpData.PixelFormat);
			int pixelCount = bmpData.Width * bmpData.Height;
			int bytesPerPixel = depth / 8;
			var rgb = new byte[pixelCount * bytesPerPixel];
			int lineBytes = bmpData.Width * bytesPerPixel;
			IntPtr ptr = bmpData.Scan0;
			for (int y = 0; y < bmpData.Height; y++)
			{
				int ptrOffset = y * bmpData.Stride;
				int bufferOffset = y * lineBytes;
				Marshal.Copy(ptr + ptrOffset, rgb, bufferOffset, lineBytes);
			}
			bitmap.UnlockBits(bmpData);
			Texture texture = new Texture(device, bitmap.Width, bitmap.Height, 0, Usage.Dynamic | Usage.AutoGenerateMipMap, Format.A8R8G8B8, Pool.Default);
			DataRectangle dataRectangle = texture.LockRectangle(0, LockFlags.None);
			for (int y = 0; y < bmpData.Height; y++)
				Marshal.Copy(rgb, y * bmpData.Width * bytesPerPixel, dataRectangle.DataPointer + y * dataRectangle.Pitch, bmpData.Width * bytesPerPixel);
			texture.UnlockRectangle(0);
			return texture;
		}

		public static BoundingSphere TransformBounds(this Attach attach, Matrix transform)
		{
			if (attach != null)
				return new BoundingSphere(Vector3.TransformCoordinate(attach.Bounds.Center.ToVector3(), transform).ToVertex(), attach.Bounds.Radius);
			else return new BoundingSphere();
		}

		public static void CalculateBounds(this Attach attach)
		{
			List<Vector3> verts = new List<Vector3>();
			foreach (MeshInfo mesh in attach.MeshInfo)
				foreach (VertexData vert in mesh.Vertices)
					verts.Add(vert.Position.ToVector3());
			attach.Bounds = SharpDX.BoundingSphere.FromPoints(verts.ToArray()).ToSAModel();
		}

		static readonly Matrix EnvironmentMapMatrix = new Matrix(-0.5f, 0, 0, 0, 0, 0.5f, 0, 0, 0, 0, 1, 0, 0.5f, 0.5f, 0, 1);
		public static void SetDeviceStates(this NJS_MATERIAL material, Device device, Texture texture, Matrix transform, FillMode fillMode)
		{
			device.SetRenderState(RenderState.FillMode, fillMode);
			device.SetTransform(TransformState.World, transform);
			if (material != null)
			{
				device.Material = new Material
				{
					Diffuse = material.DiffuseColor.ToRawColor4(),
					Ambient = material.DiffuseColor.ToRawColor4(),
					Specular = (material.IgnoreSpecular ? Color.Transparent : material.SpecularColor).ToRawColor4(),
					Power = material.Exponent * material.Exponent
				};
				/*if (!material.SuperSample)
				{
					device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.None);
					device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.None);
					device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.None);
				}*/
				device.SetTexture(0, material.UseTexture ? texture : null);
				device.SetRenderState(RenderState.Ambient, Color.Black.ToArgb());
				device.SetRenderState(RenderState.AlphaBlendEnable, material.UseAlpha);
				if (material.UseAlpha)
				{
					device.SetRenderState(RenderState.Lighting, true);
					device.SetRenderState(RenderState.BlendFactor, material.DiffuseColor.ToArgb());
					if (material.IgnoreLighting) device.SetRenderState(RenderState.Ambient, Color.White.ToArgb());
				}
				// When a material ignores lighting, SADXPC converts material color to vertex color if it isn't FFFFFFFF or FFB2B2B2
				else if (material.IgnoreLighting && material.DiffuseColor != Color.White && material.DiffuseColor != Color.FromArgb(255, 178, 178, 178))
				{
					device.SetRenderState(RenderState.Lighting, true);
					if (material.IgnoreLighting) device.SetRenderState(RenderState.Ambient, Color.White.ToArgb());
				}
				else device.SetRenderState(RenderState.Lighting, !material.IgnoreLighting);
				switch (material.DestinationAlpha)
				{
					case AlphaInstruction.Zero:
						device.SetRenderState(RenderState.DestinationBlend, Blend.Zero);
						break;

					case AlphaInstruction.One:
						device.SetRenderState(RenderState.DestinationBlend, Blend.One);
						break;

					case AlphaInstruction.OtherColor:
						break;

					case AlphaInstruction.InverseOtherColor:
						break;

					case AlphaInstruction.SourceAlpha:
						device.SetRenderState(RenderState.DestinationBlend, Blend.SourceAlpha);
						break;

					case AlphaInstruction.InverseSourceAlpha:
						device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
						break;

					case AlphaInstruction.DestinationAlpha:
						device.SetRenderState(RenderState.DestinationBlend, Blend.DestinationAlpha);
						break;

					case AlphaInstruction.InverseDestinationAlpha:
						device.SetRenderState(RenderState.DestinationBlend, Blend.InverseDestinationAlpha);
						break;
				}
				switch (material.SourceAlpha)
				{
					case AlphaInstruction.Zero:
						device.SetRenderState(RenderState.SourceBlend, Blend.Zero);
						break;

					case AlphaInstruction.One:
						device.SetRenderState(RenderState.SourceBlend, Blend.One);
						break;

					case AlphaInstruction.OtherColor:
						break;

					case AlphaInstruction.InverseOtherColor:
						break;

					case AlphaInstruction.SourceAlpha:
						device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
						break;

					case AlphaInstruction.InverseSourceAlpha:
						device.SetRenderState(RenderState.SourceBlend, Blend.InverseSourceAlpha);
						break;

					case AlphaInstruction.DestinationAlpha:
						device.SetRenderState(RenderState.SourceBlend, Blend.DestinationAlpha);
						break;

					case AlphaInstruction.InverseDestinationAlpha:
						device.SetRenderState(RenderState.SourceBlend, Blend.InverseDestinationAlpha);
						break;
				}
				if (material.EnvironmentMap)
				{
					device.SetTextureStageState(0, TextureStage.TextureTransformFlags, TextureTransform.Count2);
					device.SetTransform(TransformState.Texture0, EnvironmentMapMatrix);
					device.SetTextureStageState(0, TextureStage.TexCoordIndex, (int)TextureCoordIndex.CameraSpaceNormal);
				}
				else
				{
					device.SetTextureStageState(0, TextureStage.TextureTransformFlags, TextureTransform.Disable);
					device.SetTransform(TransformState.Texture0, Matrix.Identity);
					device.SetTextureStageState(0, TextureStage.TexCoordIndex, (int)TextureCoordIndex.PassThru);
				}
				if (material.ClampU)
					device.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Clamp);
				else if (material.FlipU)
					device.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Mirror);
				else
					device.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Wrap);
				if (material.ClampV)
					device.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Clamp);
				else if (material.FlipV)
					device.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Mirror);
				else
					device.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Wrap);
			}
			else
			{
				device.Material = new Material
				{
					Diffuse = Color.White.ToRawColor4(),
					Ambient = Color.White.ToRawColor4(),
					Specular = Color.Transparent.ToRawColor4()
				};
				device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.None);
				device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.None);
				device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.None);
				device.SetTexture(0, null);
				device.SetRenderState(RenderState.Ambient, Color.White.ToArgb());
				device.SetRenderState(RenderState.AlphaBlendEnable, false);
				device.SetTextureStageState(0, TextureStage.TexCoordIndex, 0);
				device.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Wrap);
				device.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Wrap);
			}
		}

		public static BoundingSphere CalculateBounds(this Attach attach, int mesh, Matrix transform)
		{
			if (attach == null) return new BoundingSphere();
			List<Vector3> verts = new List<Vector3>();
			foreach (VertexData vert in attach.MeshInfo[mesh].Vertices)
				verts.Add(Vector3.TransformCoordinate(vert.Position.ToVector3(), transform));
			return Calculate(verts);
		}

		public static void CalculateBounds(this COL col)
		{
			Matrix matrix = col.Model.ProcessTransforms(Matrix.Identity);
			List<Vector3> verts = new List<Vector3>();
			foreach (MeshInfo mesh in col.Model.Attach.MeshInfo)
				foreach (VertexData vert in mesh.Vertices)
					verts.Add(Vector3.TransformCoordinate(vert.Position.ToVector3(), matrix));
			col.Bounds = Calculate(verts);
		}

		/// <summary>
		/// We are using this implementation of Ritter's Bounding Sphere,
		/// because whatever was going on with SharpDX's BoundingSphere class was catastrophically under-shooting
		/// the bounds.
		/// </summary>
		/// <param name="aPoints"></param>
		/// <returns></returns>
		private static BoundingSphere Calculate(IEnumerable<Vector3> aPoints)
		{
			Vector3 one = new Vector3(1, 1, 1);

			Vector3 xmin, xmax, ymin, ymax, zmin, zmax;
			xmin = ymin = zmin = one * float.PositiveInfinity;
			xmax = ymax = zmax = one * float.NegativeInfinity;
			foreach (Vector3 p in aPoints)
			{
				if (p.X < xmin.X) xmin = p;
				if (p.X > xmax.X) xmax = p;
				if (p.Y < ymin.Y) ymin = p;
				if (p.Y > ymax.Y) ymax = p;
				if (p.Z < zmin.Z) zmin = p;
				if (p.Z > zmax.Z) zmax = p;
			}
			float xSpan = (xmax - xmin).LengthSquared();
			float ySpan = (ymax - ymin).LengthSquared();
			float zSpan = (zmax - zmin).LengthSquared();
			Vector3 dia1 = xmin;
			Vector3 dia2 = xmax;
			var maxSpan = xSpan;
			if (ySpan > maxSpan)
			{
				maxSpan = ySpan;
				dia1 = ymin; dia2 = ymax;
			}
			if (zSpan > maxSpan)
			{
				dia1 = zmin; dia2 = zmax;
			}
			Vector3 center = (dia1 + dia2) * 0.5f;
			float sqRad = (dia2 - center).LengthSquared();
			float radius = (float)Math.Sqrt(sqRad);

			foreach (var p in aPoints)
			{
				float d = (p - center).LengthSquared();
				if (d > sqRad)
				{
					var r = (float)Math.Sqrt(d);
					radius = r;
					sqRad = radius * radius;
					var offset = r - radius;
					center = (radius * center + offset * p) / r;
				}
			}
			return new BoundingSphere(center.ToVertex(), radius * 1.125f); // fixed multiplier hack because while ours 
				// undershoots less than SharpDX's, there still is some and this should handle it in most cases.
		}

		public static BoundingSphere Merge(BoundingSphere sphereA, BoundingSphere sphereB)
		{
			// if one sphere is empty, return the other one
			if (sphereA.Center.IsEmpty && sphereA.Radius == 0)
				return sphereB;
			if (sphereB.Center.IsEmpty && sphereB.Radius == 0)
				return sphereA;

			return SharpDX.BoundingSphere.Merge(sphereA.ToSharpDX(), sphereB.ToSharpDX()).ToSAModel();
		}

		public static Mesh CreateD3DMesh(this Attach attach)
		{
			int numverts = 0;
			byte data = 0;
			if (attach == null || attach.MeshInfo == null) return null;
			foreach (MeshInfo item in attach.MeshInfo)
			{
				numverts += item.Vertices.Length;
				if (item.HasUV)
					data |= 1;
				if (item.HasVC)
					data |= 2;
			}
			if (numverts == 0) return null;
			switch (data)
			{
				case 3:
					return new Mesh<FVF_PositionNormalTexturedColored>(attach);
				case 2:
					return new Mesh<FVF_PositionNormalColored>(attach);
				case 1:
					return new Mesh<FVF_PositionNormalTextured>(attach);
				default:
					return new Mesh<FVF_PositionNormal>(attach);
			}
		}

		public static float BAMSToRad(int BAMS)
		{
			return (float)(BAMS / (65536 / (2 * Math.PI)));
		}

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

		static NJS_MATERIAL MaterialBuffer = new NJS_MATERIAL { UseTexture = true };
		static VertexData[] VertexBuffer = new VertexData[32768];
		static List<WeightData>[] WeightBuffer = new List<WeightData>[32768];
		static readonly CachedPoly[] PolyCache = new CachedPoly[255];

		public static List<Mesh> ProcessWeightedModel(this NJS_OBJECT obj)
		{
			List<Mesh> meshes = new List<Mesh>();
			int mdlindex = -1;
			do
			{
				ProcessWeightedModel(obj, new MatrixStack(), meshes, ref mdlindex);
				obj = obj.Sibling;
			} while (obj != null);
			return meshes;
		}

		private static void ProcessWeightedModel(NJS_OBJECT obj, MatrixStack transform, List<Mesh> meshes, ref int mdlindex)
		{
			mdlindex++;
			transform.Push();
			obj.ProcessTransforms(transform);
			if (obj.Attach is ChunkAttach attach)
				meshes.Add(ProcessWeightedAttach(attach, transform, mdlindex));
			else
				meshes.Add(null);
			foreach (NJS_OBJECT child in obj.Children)
				ProcessWeightedModel(child, transform, meshes, ref mdlindex);
			transform.Pop();
		}

		private static Mesh ProcessWeightedAttach(ChunkAttach attach, MatrixStack transform, int mdlindex)
		{
			if (attach.Vertex != null)
			{
				foreach (VertexChunk chunk in attach.Vertex)
				{
					if (VertexBuffer.Length < chunk.IndexOffset + chunk.VertexCount)
					{
						Array.Resize(ref VertexBuffer, chunk.IndexOffset + chunk.VertexCount);
						Array.Resize(ref WeightBuffer, chunk.IndexOffset + chunk.VertexCount);
					}
					if (chunk.HasWeight)
					{
						for (int i = 0; i < chunk.VertexCount; i++)
						{
							var weightByte = chunk.NinjaFlags[i] >> 16;
							var weight = weightByte / 255f;
							var origpos = chunk.Vertices[i].ToVector3();
							var position = (Vector3.TransformCoordinate(origpos, transform.Top) * weight).ToVertex();
							var orignor = Vector3.Up;
							Vertex normal = null;
							if (chunk.Normals.Count > 0)
							{
								orignor = chunk.Normals[i].ToVector3();
								normal = (Vector3.TransformNormal(orignor, transform.Top) * weight).ToVertex();
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
							var position = Vector3.TransformCoordinate(origpos, transform.Top).ToVertex();
							var orignor = Vector3.Up;
							Vertex normal = null;
							if (chunk.Normals.Count > 0)
							{
								orignor = chunk.Normals[i].ToVector3();
								normal = Vector3.TransformNormal(orignor, transform.Top).ToVertex();
							}
							VertexBuffer[i + chunk.IndexOffset] = new VertexData(position, normal);
							if (chunk.Diffuse.Count > 0)
								VertexBuffer[i + chunk.IndexOffset].Color = chunk.Diffuse[i];
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
			int numverts = 0;
			bool uv = false;
			foreach (MeshInfo item in attach.MeshInfo)
			{
				numverts += item.Vertices.Length;
				if (item.HasUV)
					uv = true;
			}
			if (numverts == 0) return null;
			if (uv)
				return new WeightedMesh<FVF_PositionNormalTexturedColored>(attach, weights);
			else
				return new WeightedMesh<FVF_PositionNormalColored>(attach, weights);
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

		public static void UpdateWeightedModel(this NJS_OBJECT obj, MatrixStack transform, Mesh[] meshes)
		{
			List<Matrix> matrices = new List<Matrix>();
			obj.GetMatrices(transform, matrices);
			for (int i = 0; i < meshes.Length; i++)
				if (meshes[i] is IWeightedMesh mesh)
					mesh.Update(matrices);
		}

		public static void GetMatrices(this NJS_OBJECT obj, MatrixStack transform, List<Matrix> matrices)
		{
			transform.Push();
			obj.ProcessTransforms(transform);
			matrices.Add(transform.Top);
			foreach (NJS_OBJECT child in obj.Children)
				child.GetMatrices(transform, matrices);
			transform.Pop();
			if (obj.Parent == null && obj.Sibling != null)
				obj.Sibling.GetMatrices(transform, matrices);
		}

		public static void UpdateWeightedModelAnimated(this NJS_OBJECT obj, MatrixStack transform, NJS_MOTION anim, int animframe, Mesh[] meshes)
		{
			List<Matrix> matrices = new List<Matrix>();
			obj.GetMatricesAnimated(transform, anim, animframe, matrices);
			for (int i = 0; i < meshes.Length; i++)
				if (meshes[i] is IWeightedMesh mesh)
					mesh.Update(matrices);
		}

		public static void GetMatricesAnimated(this NJS_OBJECT obj, MatrixStack transform, NJS_MOTION anim, int animframe, List<Matrix> matrices)
		{
			int animindex = -1;
			do
			{
				obj.GetMatricesAnimated(transform, anim, animframe, ref animindex, matrices);
				obj = obj.Sibling;
			} while (obj != null);
		}

		private static void GetMatricesAnimated(this NJS_OBJECT obj, MatrixStack transform, NJS_MOTION anim, int animframe, ref int animindex, List<Matrix> matrices)
		{
			transform.Push();
			bool animate = obj.Animate;
			if (animate) animindex++;
			if (!anim.Models.ContainsKey(animindex)) animate = false;
			if (animate)
				obj.ProcessTransforms(anim.Models[animindex], animframe, transform);
			else
				obj.ProcessTransforms(transform);
			matrices.Add(transform.Top);
			foreach (NJS_OBJECT child in obj.Children)
				child.GetMatricesAnimated(transform, anim, animframe, ref animindex, matrices);
			transform.Pop();
		}

		public static void UpdateWeightedModelSelection(this NJS_OBJECT obj, NJS_OBJECT selected, Mesh[] meshes)
		{
			NJS_OBJECT[] objs = obj.GetObjects();
			int selind = Array.IndexOf(objs, selected);
			for (int i = 0; i < objs.Length; i++)
				if (meshes[i] is IWeightedMesh mesh)
					mesh.UpdateSelection(selind);
		}

		#region Model drawing functions
		public static List<RenderInfo> DrawModel(this NJS_OBJECT obj, FillMode fillMode, MatrixStack transform, Texture[] textures, Mesh mesh, bool useMat, bool ignorematcolors = false, bool ignorelight = false, bool invert = false, bool boundsByMesh = false)
		{
			List<RenderInfo> result = new List<RenderInfo>();

			if (mesh == null)
				return result;

			transform.Push();
			obj.ProcessTransforms(transform);
		
			if (obj.Attach != null)
			{
				BoundingSphere attachBounds = TransformBounds(obj.Attach, transform.Top);
				for (int j = 0; j < obj.Attach.MeshInfo.Length; j++)
				{
					NJS_MATERIAL mat;
					Texture texture = null;

					// Inverted drawing for editor selection
					if (invert)
					{
						Color col = Color.White;
						if (useMat) col = obj.Attach.MeshInfo[j].Material.DiffuseColor;
						col = Color.FromArgb(255 - col.R, 255 - col.G, 255 - col.B);
						mat = new NJS_MATERIAL
						{
							DiffuseColor = col,
							IgnoreLighting = true,
							UseAlpha = false
						};
					}

					// Regular drawing
					else
					{
						// HACK: When useMat is true, mat shouldn't be null. However, checking it anyway ensures Sky Deck 3 loads.
						// If it is in fact null, it applies a placeholder so that the editor doesn't crash.
						if (useMat && obj.Attach.MeshInfo[j].Material != null)
						{
							mat = new NJS_MATERIAL(obj.Attach.MeshInfo[j].Material);
							if (textures != null && mat != null && mat.TextureID < textures.Length)
								texture = textures[mat.TextureID];
						}
						else
						{
							mat = new NJS_MATERIAL
							{
								DiffuseColor = Color.White,
								IgnoreLighting = true,
								UseAlpha = false
							};

							if (obj.Attach.MeshInfo[j].Material == null)
							{
								MeshInfo old = obj.Attach.MeshInfo[j];
								obj.Attach.MeshInfo[j] = new MeshInfo(mat, old.Polys, old.Vertices, old.HasUV, old.HasVC);
							}
						}
					}

					// Special flags
					if (ignorematcolors)
					{
						mat.DiffuseColor = Color.FromArgb(mat.DiffuseColor.A, Color.White);
					}
					if (ignorelight)
					{
						mat.IgnoreLighting = true;
					}
					if (boundsByMesh)
					{
						attachBounds = obj.Attach.CalculateBounds(j, transform.Top);
					}

					result.Add(new RenderInfo(mesh, j, transform.Top, mat, texture, fillMode, attachBounds));
				}
			}

			transform.Pop();
			return result;
		}

		private static List<RenderInfo> DrawModelTree(this NJS_OBJECT obj, FillMode fillMode, MatrixStack transform, Texture[] textures, Mesh[] meshes, ref int modelindex, bool ignorematcolors = false, bool ignorelight = false, bool invert = false, bool boundsByMesh = false)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			modelindex++;
			obj.ProcessTransforms(transform);

			bool attachValid = obj.Attach != null;
			bool meshValid = modelindex >= 0 && modelindex < meshes.Length && meshes[modelindex] != null;

			if (attachValid & meshValid)
			{
				BoundingSphere attachBounds = TransformBounds(obj.Attach, transform.Top);
				for (int j = 0; j < obj.Attach.MeshInfo.Length; j++)
				{
					Texture texture = null;
					NJS_MATERIAL mat;

					// Inverted drawing for editor selection
					if (invert)
					{
						Color color = Color.Black;

						// HACK: Null material hack 3: Fixes selecting objects in SADXLVL2, Twinkle Park 1.
						if (obj.Attach.MeshInfo[j].Material != null)
							color = obj.Attach.MeshInfo[j].Material.DiffuseColor;

						color = Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
						mat = new NJS_MATERIAL
						{
							DiffuseColor = color,
							IgnoreLighting = true,
							UseAlpha = false
						};
					}

					// Regular drawing
					else
					{
						if (obj.Attach.MeshInfo[j].Material != null)
							mat = new NJS_MATERIAL(obj.Attach.MeshInfo[j].Material);
						else
						{
							mat = new NJS_MATERIAL
							{
								DiffuseColor = Color.White,
								IgnoreLighting = true,
								UseAlpha = false
							};
						}
						// HACK: Null material hack 2: Fixes display of objects in SADXLVL2, Twinkle Park 1
						if (textures != null && mat != null && mat.TextureID < textures.Length)
							texture = textures[mat.TextureID];
					}

					// Special flags
					if (ignorematcolors)
					{
						mat.DiffuseColor = Color.FromArgb(mat.DiffuseColor.A, Color.White);
					}
					if (ignorelight)
					{
						mat.IgnoreLighting = true;
					}
					if (boundsByMesh)
					{
						attachBounds = obj.Attach.CalculateBounds(j, transform.Top);
					}

					result.Add(new RenderInfo(meshes[modelindex], j, transform.Top, mat, texture, fillMode, attachBounds));
				}
			}
			
			foreach (NJS_OBJECT child in obj.Children)
				result.AddRange(DrawModelTree(child, fillMode, transform, textures, meshes, ref modelindex, ignorematcolors, ignorelight, invert, boundsByMesh));
			transform.Pop();
			return result;
		}
	
		private static List<RenderInfo> DrawModelTreeAnimated(this NJS_OBJECT obj, FillMode fillMode, MatrixStack transform, Texture[] textures, Mesh[] meshes, NJS_MOTION anim, int animframe, ref int modelindex, ref int animindex, bool ignorematcolors = false, bool ignorelight = false, bool invert = false, bool boundsByMesh = false)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			modelindex++;
			bool animate = obj.Animate;
			if (animate) animindex++;
			if (!anim.Models.ContainsKey(animindex)) animate = false;
			if (animate)
				obj.ProcessTransforms(anim.Models[animindex], animframe, transform);
			else
				obj.ProcessTransforms(transform);
			if (obj.Attach != null & meshes[modelindex] != null)
			{
				BoundingSphere attachBounds = TransformBounds(obj.Attach, transform.Top);
				for (int j = 0; j < obj.Attach.MeshInfo.Length; j++)
				{
					Texture texture = null;
					NJS_MATERIAL mat;
					// Inverted drawing for editor selection
					if (invert)
					{
						Color col = obj.Attach.MeshInfo[j].Material.DiffuseColor;
						col = Color.FromArgb(255 - col.R, 255 - col.G, 255 - col.B);
						mat = new NJS_MATERIAL
						{
							DiffuseColor = col,
							IgnoreLighting = true,
							UseAlpha = false
						};
					}

					// Regular drawing
					else
					{
						if (obj.Attach.MeshInfo[j].Material != null)
							mat = new NJS_MATERIAL(obj.Attach.MeshInfo[j].Material);
						else
						{
							mat = new NJS_MATERIAL
							{
								DiffuseColor = Color.White,
								IgnoreLighting = true,
								UseAlpha = false
							};
						}
						if (textures != null && mat.TextureID < textures.Length)
							texture = textures[mat.TextureID];
					}

					// Special flags
					if (ignorematcolors)
					{
						mat.DiffuseColor = Color.FromArgb(mat.DiffuseColor.A, Color.White);
					}
					if (ignorelight)
					{
						mat.IgnoreLighting = true;
					}
					if (boundsByMesh)
					{
						attachBounds = obj.Attach.CalculateBounds(j, transform.Top);
					}
					result.Add(new RenderInfo(meshes[modelindex], j, transform.Top, mat, texture, fillMode, attachBounds));
				}
			}
			foreach (NJS_OBJECT child in obj.Children)
				result.AddRange(DrawModelTreeAnimated(child, fillMode, transform, textures, meshes, anim, animframe, ref modelindex, ref animindex, ignorematcolors, ignorelight));
			transform.Pop();
			return result;
		}

		public static List<RenderInfo> DrawModelTreeWeighted(this NJS_OBJECT obj, FillMode fillMode, Matrix transform, Texture[] textures, Mesh[] meshes, bool ignorematcolor = false, bool ignorelight = false, bool invert = false, bool boundsByMesh = false)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			NJS_OBJECT[] objs = obj.GetObjects();
			for (int i = 0; i < objs.Length; i++)
				if (objs[i].Attach != null & meshes[i] != null)
				{
					BoundingSphere attachBounds = TransformBounds(obj.Attach, transform);
					for (int j = 0; j < objs[i].Attach.MeshInfo.Length; j++)
					{
						Texture texture = null;
						NJS_MATERIAL mat;
						// Inverted drawing for editor selection
						if (invert)
						{
							Color color = Color.Black;

							// HACK: Null material hack 3: Fixes selecting objects in SADXLVL2, Twinkle Park 1.
							if (objs[i].Attach.MeshInfo[j].Material != null)
								color = objs[i].Attach.MeshInfo[j].Material.DiffuseColor;

							color = Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
							mat = new NJS_MATERIAL
							{
								DiffuseColor = color,
								IgnoreLighting = true,
								UseAlpha = false
							};
						}
						// Regular drawing
						else
						{
							if (objs[i].Attach.MeshInfo[j].Material != null)
								mat = new NJS_MATERIAL(objs[i].Attach.MeshInfo[j].Material);
							else
							{
								mat = new NJS_MATERIAL
								{
									DiffuseColor = Color.White,
									IgnoreLighting = true,
									UseAlpha = false
								};
							}
							if (textures != null && mat != null && mat.TextureID < textures.Length)
								texture = textures[mat.TextureID];
						}

						// Special flags
						if (ignorematcolor)
						{
							mat.DiffuseColor = Color.FromArgb(mat.DiffuseColor.A, Color.White);
						}
						if (ignorelight)
						{
							mat.IgnoreLighting = true;
						}
						if (boundsByMesh)
						{
							attachBounds = obj.Attach.CalculateBounds(j, transform);
						}

						result.Add(new RenderInfo(meshes[i], j, transform, mat, texture, fillMode, attachBounds));
					}
				}
			return result;
		}

		#endregion

		#region Inverted model drawing functions
		public static List<RenderInfo> DrawModelInvert(this NJS_OBJECT obj, MatrixStack transform, Mesh mesh, bool useMat, bool boundsByMesh = false)
		{
			return DrawModel(obj, FillMode.Wireframe, transform, null, mesh, useMat, invert: true);
		}

		private static List<RenderInfo> DrawModelTreeInvert(this NJS_OBJECT obj, MatrixStack transform, Mesh[] meshes, ref int modelindex, bool ignorematcolor = false, bool ignorelight = false, bool boundsByMesh = false)
		{
			return DrawModelTree(obj, FillMode.Wireframe, transform, null, meshes, ignorematcolor, ignorelight, true, boundsByMesh);
		}

		private static List<RenderInfo> DrawModelTreeAnimatedInvert(this NJS_OBJECT obj, MatrixStack transform, Mesh[] meshes, NJS_MOTION anim, int animframe, ref int modelindex, ref int animindex, bool ignorematcolor = false, bool ignorelight = false, bool invert = false, bool boundsByMesh = false)
		{
			return DrawModelTreeAnimated(obj, FillMode.Wireframe, transform, null, meshes, anim, animframe, ignorematcolor, ignorelight, true, boundsByMesh);
		}

		public static List<RenderInfo> DrawModelTreeWeightedInvert(this NJS_OBJECT obj, Matrix transform, Mesh[] meshes, bool ignorematcolor = false, bool ignorelight = false, bool invert = false, bool boundsByMesh = false)
		{
			return DrawModelTreeWeighted(obj, FillMode.Wireframe, transform, null, meshes, ignorematcolor, ignorelight, invert, boundsByMesh);
		}
		#endregion

		#region Indexed model drawing functions
		public static List<RenderInfo> DrawModelTreeInvert(this NJS_OBJECT obj, MatrixStack transform, Mesh[] meshes, bool ignorematcolor = false, bool ignorelight = false, bool invert = false, bool boundsByMesh = false)
		{
			int modelindex = -1;
			List<RenderInfo> result = new List<RenderInfo>();
			do
			{
				result.AddRange(obj.DrawModelTreeInvert(transform, meshes, ref modelindex, ignorelight, invert, boundsByMesh));
				obj = obj.Sibling;
			} while (obj != null);
			return result;
		}

		public static List<RenderInfo> DrawModelTree(this NJS_OBJECT obj, FillMode fillMode, MatrixStack transform, Texture[] textures, Mesh[] meshes, bool ignorematcolor = false, bool ignorelight = false, bool invert = false, bool boundsByMesh = false)
		{
			int modelindex = -1;
			List<RenderInfo> result = new List<RenderInfo>();
			do
			{
				result.AddRange(obj.DrawModelTree(fillMode, transform, textures, meshes, ref modelindex, ignorematcolor, ignorelight, invert, boundsByMesh));
				obj = obj.Sibling;
			} while (obj != null);
			return result;
		}

		public static List<RenderInfo> DrawModelTreeAnimated(this NJS_OBJECT obj, FillMode fillMode, MatrixStack transform, Texture[] textures, Mesh[] meshes, NJS_MOTION anim, int animframe, bool ignorematcolor = false, bool ignorelight = false, bool invert = false, bool boundsByMesh = false)
		{
			int modelindex = -1;
			int animindex = -1;
			List<RenderInfo> result = new List<RenderInfo>();
			do
			{
				result.AddRange(obj.DrawModelTreeAnimated(fillMode, transform, textures, meshes, anim, animframe, ref modelindex, ref animindex, ignorematcolor, ignorelight, invert, boundsByMesh));
				obj = obj.Sibling;
			} while (obj != null);
			return result;
		}

		public static List<RenderInfo> DrawModelTreeAnimatedInvert(this NJS_OBJECT obj, MatrixStack transform, Mesh[] meshes, NJS_MOTION anim, int animframe)
		{
			int modelindex = -1;
			int animindex = -1;
			List<RenderInfo> result = new List<RenderInfo>();
			do
			{
				result.AddRange(obj.DrawModelTreeAnimatedInvert(transform, meshes, anim, animframe, ref modelindex, ref animindex));
				obj = obj.Sibling;
			} while (obj != null);
			return result;
		}
		#endregion

		public static HitResult CheckHit(this NJS_OBJECT obj, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, Mesh mesh)
		{
			if (mesh == null) return HitResult.NoHit;
			MatrixStack transform = new MatrixStack();
			obj.ProcessTransforms(transform);
			return mesh.CheckHit(Near, Far, Viewport, Projection, View, transform, obj);
		}

		public static HitResult CheckHit(this NJS_OBJECT obj, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform, Mesh[] mesh)
		{
			int modelindex = -1;
			HitResult result = HitResult.NoHit;
			do
			{
				result = HitResult.Min(result, CheckHit(obj, Near, Far, Viewport, Projection, View, transform, mesh, ref modelindex));
				obj = obj.Sibling;
			} while (obj != null);
			return result;
		}

		private static HitResult CheckHit(this NJS_OBJECT obj, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform, Mesh[] mesh, ref int modelindex)
		{
			transform.Push();
			modelindex++;
			obj.ProcessTransforms(transform);
			HitResult result = HitResult.NoHit;
			if (obj.Attach != null && mesh[modelindex] != null && !obj.Scale.IsEmpty)
				result = mesh[modelindex].CheckHit(Near, Far, Viewport, Projection, View, transform, obj);
			foreach (NJS_OBJECT child in obj.Children)
				result = HitResult.Min(result, CheckHit(child, Near, Far, Viewport, Projection, View, transform, mesh, ref modelindex));
			transform.Pop();
			return result;
		}

		public static HitResult CheckHitAnimated(this NJS_OBJECT obj, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform, Mesh[] mesh, NJS_MOTION anim, int animframe)
		{
			int modelindex = -1;
			int animindex = -1;
			HitResult result = HitResult.NoHit;
			do
			{
				result = HitResult.Min(result, CheckHitAnimated(obj, Near, Far, Viewport, Projection, View, transform, mesh, anim, animframe, ref modelindex, ref animindex));
				obj = obj.Sibling;
			} while (obj != null);
			return result;
		}

		private static HitResult CheckHitAnimated(this NJS_OBJECT obj, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform, Mesh[] mesh, NJS_MOTION anim, int animframe, ref int modelindex, ref int animindex)
		{
			transform.Push();
			modelindex++;
			bool animate = obj.Animate;
			if (animate) animindex++;
			if (!anim.Models.ContainsKey(animindex)) animate = false;
			if (animate)
				obj.ProcessTransforms(anim.Models[animindex], animframe, transform);
			else
				obj.ProcessTransforms(transform);
			HitResult result = HitResult.NoHit;
			if (obj.Attach != null && mesh[modelindex] != null && !obj.Scale.IsEmpty)
				result = HitResult.Min(result, mesh[modelindex].CheckHit(Near, Far, Viewport, Projection, View, transform, obj));
			foreach (NJS_OBJECT child in obj.Children)
				result = HitResult.Min(result, CheckHitAnimated(child, Near, Far, Viewport, Projection, View, transform, mesh, anim, animframe, ref modelindex, ref animindex));
			transform.Pop();
			return result;
		}

		public static HitResult CheckHitWeighted(this NJS_OBJECT obj, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, Matrix transform, Mesh[] mesh)
		{
			HitResult result = HitResult.NoHit;
			NJS_OBJECT[] objs = obj.GetObjects();
			for (int i = 0; i < objs.Length; i++)
				if (objs[i].Attach != null && mesh[i] != null)
					result = HitResult.Min(result, mesh[i].CheckHit(Near, Far, Viewport, Projection, View, transform, objs[i]));
			return result;
		}

		private static T GetItemNeg<T>(this List<T> list, int index)
		{
			if (index < 0)
				return list[list.Count + index];
			return list[index - 1];
		}

		public static int RadToBAMS(float rad)
		{
			return (int)(rad * 65536.0 / (2 * Math.PI));
		}

		/// <summary>
		/// This is supposed to convert a matrix back into rotation values. But it doesn't.
		/// </summary>
		/// <param name="matrix"></param>
		/// <returns></returns>
		public static Rotation FromMatrix(this Matrix matrix)
		{
			Rotation eulerRotationZXY = new Rotation();

			// code below is adopted from Ogre Engine source
			float rfYAngle, rfPAngle, rfRAngle; // rfY angle = z, rfP angle = y, rfRAngle = x

			// rot = cy*cz-sx*sy*sz -cx*sz cz*sy+cy*sx*sz
			// cz*sx*sy+cy*sz cx*cz -cy*cz*sx+sy*sz
			// -cx*sy sx cx*cy

			//rfPAngle = Math::ASin(m[2][1]);
			rfPAngle = (float)Math.Asin(matrix.M32);
			if (rfPAngle < (Math.PI / 2))
			{
				if (rfPAngle > (-Math.PI / 2))
				{
					//rfYAngle = Math::ATan2(-m[0][1],m[1][1]);
					//rfRAngle = Math::ATan2(-m[2][0],m[2][2]);
					rfYAngle = (float)Math.Atan2(-matrix.M12, matrix.M22);
					rfRAngle = (float)Math.Atan2(-matrix.M31, matrix.M33);
				}
				else
				{
					// WARNING. Not a unique solution.
					//Radian fRmY = Math::ATan2(m[0][2],m[0][0]);
					float fRmY = (float)Math.Atan2(matrix.M13, matrix.M11);
					rfRAngle = 0f; // any angle works
					rfYAngle = rfRAngle - fRmY;
				}
			}
			else
			{
				// WARNING. Not a unique solution.
				// Radian fRpY = Math::ATan2(m[0][2],m[0][0]);
				float fRpY = (float)Math.Atan2(matrix.M13, matrix.M11);
				rfRAngle = 0f; // any angle works
				rfYAngle = fRpY - rfRAngle;
			}

			eulerRotationZXY.X = RadToBAMS(rfRAngle);
			eulerRotationZXY.Y = RadToBAMS(rfPAngle);
			eulerRotationZXY.Z = RadToBAMS(rfYAngle);

			return eulerRotationZXY;
		}


		public static float Distance(this Vector3 vectorA, Vector3 vectorB)
		{
			return Vector3.Subtract(vectorA, vectorB).Length();
		}

		public static Matrix ProcessTransforms(this NJS_OBJECT obj, Matrix matrix)
		{
			if (!obj.Position.IsEmpty)
				MatrixFunctions.Translate(ref matrix, obj.Position);
			if (obj.RotateZYX)
				MatrixFunctions.RotateZYX(ref matrix, obj.Rotation);
			else
				MatrixFunctions.RotateXYZ(ref matrix, obj.Rotation);
			if (obj.Scale.X != 1 || obj.Scale.Y != 1 || obj.Scale.Z != 1)
				MatrixFunctions.Scale(ref matrix, obj.Scale);
			return matrix;
		}

		public static void ProcessTransforms(this NJS_OBJECT obj, MatrixStack transform)
		{
			transform.LoadMatrix(obj.ProcessTransforms(transform.Top));
		}

		public static Matrix ProcessTransforms(this NJS_OBJECT obj, AnimModelData anim, int animframe, Matrix matrix)
		{
			if (anim == null)
				return obj.ProcessTransforms(matrix);
			Vertex position = obj.Position;
			if (anim.Position.Count > 0)
				position = anim.GetPosition(animframe);
			if (!position.IsEmpty)
				MatrixFunctions.Translate(ref matrix, position);
			Rotation rotation = obj.Rotation;
			if (anim.Rotation.Count > 0)
				rotation = anim.GetRotation(animframe);
			if (obj.RotateZYX)
				MatrixFunctions.RotateZYX(ref matrix, rotation);
			else
				MatrixFunctions.RotateXYZ(ref matrix, rotation);
			Vertex scale = obj.Scale;
			if (anim.Scale.Count > 0)
				scale = anim.GetScale(animframe);
			if (scale.X != 1 || scale.Y != 1 || scale.Z != 1)
				MatrixFunctions.Scale(ref matrix, scale);
			return matrix;
		}

		public static void ProcessTransforms(this NJS_OBJECT obj, AnimModelData anim, int animframe, MatrixStack transform)
		{
			transform.LoadMatrix(obj.ProcessTransforms(anim, animframe, transform.Top));
		}

		private static readonly float[] sineTable = {
			0.0F,
			0.001534F,
			0.0030680001F,
			0.0046020001F,
			0.0061360002F,
			0.0076700002F,
			0.0092040002F,
			0.010738F,
			0.012272F,
			0.013805F,
			0.015339F,
			0.016873F,
			0.018407F,
			0.01994F,
			0.021474F,
			0.023008F,
			0.024541F,
			0.026075F,
			0.027608F,
			0.029142F,
			0.030675F,
			0.032207999F,
			0.033741001F,
			0.035273999F,
			0.036807001F,
			0.038339999F,
			0.039873F,
			0.041405998F,
			0.042938001F,
			0.044470999F,
			0.046002999F,
			0.047534999F,
			0.049068F,
			0.0506F,
			0.052131999F,
			0.053663999F,
			0.055195F,
			0.056727F,
			0.058258001F,
			0.05979F,
			0.061321001F,
			0.062852003F,
			0.064383F,
			0.065912999F,
			0.067443997F,
			0.068974003F,
			0.070505001F,
			0.072035F,
			0.073564999F,
			0.075094F,
			0.076623999F,
			0.078152999F,
			0.079682F,
			0.081211001F,
			0.082740001F,
			0.084269002F,
			0.085796997F,
			0.087325998F,
			0.088854F,
			0.090380996F,
			0.091908999F,
			0.093436003F,
			0.094962999F,
			0.096490003F,
			0.098017F,
			0.099544004F,
			0.10107F,
			0.102596F,
			0.104122F,
			0.105647F,
			0.107172F,
			0.108697F,
			0.110222F,
			0.111747F,
			0.113271F,
			0.114795F,
			0.116319F,
			0.117842F,
			0.119365F,
			0.120888F,
			0.122411F,
			0.123933F,
			0.12545501F,
			0.126977F,
			0.128498F,
			0.13001899F,
			0.13154F,
			0.13306101F,
			0.134581F,
			0.13610099F,
			0.13762F,
			0.139139F,
			0.14065801F,
			0.142177F,
			0.143695F,
			0.14521299F,
			0.14673001F,
			0.148248F,
			0.149765F,
			0.151281F,
			0.152797F,
			0.154313F,
			0.155828F,
			0.157343F,
			0.158858F,
			0.160372F,
			0.16188601F,
			0.16339999F,
			0.164913F,
			0.166426F,
			0.16793799F,
			0.16945F,
			0.17096201F,
			0.172473F,
			0.17398401F,
			0.175494F,
			0.17700399F,
			0.178514F,
			0.180023F,
			0.181532F,
			0.18303999F,
			0.18454801F,
			0.186055F,
			0.187562F,
			0.189069F,
			0.190575F,
			0.19208001F,
			0.19358601F,
			0.19509F,
			0.196595F,
			0.198098F,
			0.19960199F,
			0.201105F,
			0.20260701F,
			0.204109F,
			0.20561001F,
			0.207111F,
			0.20861199F,
			0.21011201F,
			0.211611F,
			0.21311F,
			0.214609F,
			0.216107F,
			0.217604F,
			0.219101F,
			0.220598F,
			0.222094F,
			0.223589F,
			0.22508401F,
			0.226578F,
			0.228072F,
			0.22956499F,
			0.231058F,
			0.23255F,
			0.234042F,
			0.235533F,
			0.23702399F,
			0.23851401F,
			0.240003F,
			0.241492F,
			0.24298F,
			0.244468F,
			0.24595501F,
			0.24744201F,
			0.248928F,
			0.250413F,
			0.25189799F,
			0.253382F,
			0.254866F,
			0.256349F,
			0.25783101F,
			0.25931299F,
			0.26079401F,
			0.26227501F,
			0.26375499F,
			0.26523399F,
			0.26671299F,
			0.26819101F,
			0.26966801F,
			0.27114499F,
			0.27262101F,
			0.274097F,
			0.275572F,
			0.27704599F,
			0.27851999F,
			0.279993F,
			0.28146499F,
			0.28293699F,
			0.284408F,
			0.285878F,
			0.28734699F,
			0.288816F,
			0.29028499F,
			0.29175201F,
			0.293219F,
			0.29468501F,
			0.29615101F,
			0.297616F,
			0.29908001F,
			0.30054301F,
			0.30200601F,
			0.30346799F,
			0.30492899F,
			0.30638999F,
			0.30785F,
			0.30930901F,
			0.31076699F,
			0.31222501F,
			0.31368199F,
			0.31513801F,
			0.31659299F,
			0.318048F,
			0.319502F,
			0.32095501F,
			0.32240799F,
			0.32385901F,
			0.32530999F,
			0.32675999F,
			0.32821F,
			0.329658F,
			0.33110601F,
			0.332553F,
			0.33399999F,
			0.33544499F,
			0.33689001F,
			0.33833399F,
			0.33977699F,
			0.34121901F,
			0.34266099F,
			0.34410101F,
			0.345541F,
			0.34698001F,
			0.34841901F,
			0.34985599F,
			0.351293F,
			0.35272899F,
			0.354164F,
			0.355598F,
			0.35703099F,
			0.35846299F,
			0.35989499F,
			0.36132601F,
			0.36275601F,
			0.36418501F,
			0.36561301F,
			0.36704001F,
			0.368467F,
			0.369892F,
			0.371317F,
			0.37274101F,
			0.37416399F,
			0.375586F,
			0.37700701F,
			0.37842801F,
			0.37984699F,
			0.381266F,
			0.38268301F,
			0.38409999F,
			0.38551599F,
			0.386931F,
			0.388345F,
			0.38975799F,
			0.39117F,
			0.392582F,
			0.39399201F,
			0.395401F,
			0.39681F,
			0.39821801F,
			0.39962399F,
			0.40103F,
			0.402435F,
			0.40383801F,
			0.40524101F,
			0.406643F,
			0.40804401F,
			0.409444F,
			0.41084301F,
			0.41224101F,
			0.413638F,
			0.415034F,
			0.41643F,
			0.417824F,
			0.41921699F,
			0.420609F,
			0.42199999F,
			0.42339F,
			0.42478001F,
			0.42616799F,
			0.42755499F,
			0.42894101F,
			0.43032601F,
			0.43171099F,
			0.43309399F,
			0.43447599F,
			0.435857F,
			0.43723699F,
			0.43861601F,
			0.43999401F,
			0.44137099F,
			0.442747F,
			0.44412199F,
			0.44549599F,
			0.44686899F,
			0.448241F,
			0.44961101F,
			0.45098099F,
			0.45234999F,
			0.45371699F,
			0.455084F,
			0.456449F,
			0.45781299F,
			0.45917699F,
			0.46053901F,
			0.4619F,
			0.46325999F,
			0.46461901F,
			0.46597701F,
			0.46733299F,
			0.46868899F,
			0.470043F,
			0.47139701F,
			0.47274899F,
			0.47409999F,
			0.47545001F,
			0.47679901F,
			0.478147F,
			0.47949401F,
			0.48083901F,
			0.48218399F,
			0.483527F,
			0.484869F,
			0.48620999F,
			0.48754999F,
			0.48888901F,
			0.49022701F,
			0.49156299F,
			0.49289799F,
			0.494232F,
			0.495565F,
			0.49689701F,
			0.49822801F,
			0.49955699F,
			0.50088501F,
			0.50221199F,
			0.50353801F,
			0.50486302F,
			0.50618702F,
			0.50750899F,
			0.50883001F,
			0.51015002F,
			0.51146901F,
			0.51278597F,
			0.514103F,
			0.51541799F,
			0.51673198F,
			0.51804399F,
			0.51935601F,
			0.520666F,
			0.52197498F,
			0.523283F,
			0.52459002F,
			0.525895F,
			0.52719897F,
			0.52850199F,
			0.52980399F,
			0.53110403F,
			0.53240299F,
			0.533701F,
			0.534998F,
			0.53629303F,
			0.53758699F,
			0.53887999F,
			0.54017198F,
			0.541462F,
			0.54275101F,
			0.54403901F,
			0.54532498F,
			0.54661F,
			0.547894F,
			0.54917699F,
			0.55045801F,
			0.55173802F,
			0.55301702F,
			0.55429399F,
			0.55557001F,
			0.55684501F,
			0.558119F,
			0.55939102F,
			0.56066197F,
			0.56193101F,
			0.56319898F,
			0.564466F,
			0.565732F,
			0.56699598F,
			0.568259F,
			0.56952101F,
			0.57078099F,
			0.57204002F,
			0.57329702F,
			0.57455301F,
			0.57580799F,
			0.57706201F,
			0.57831401F,
			0.57956499F,
			0.580814F,
			0.58206201F,
			0.58330899F,
			0.58455402F,
			0.58579803F,
			0.58704001F,
			0.58828199F,
			0.58952099F,
			0.59075999F,
			0.59199703F,
			0.59323198F,
			0.59446698F,
			0.59569901F,
			0.59693098F,
			0.59816098F,
			0.59938902F,
			0.60061598F,
			0.60184199F,
			0.60306698F,
			0.60429001F,
			0.60551101F,
			0.606731F,
			0.60794997F,
			0.60916698F,
			0.61038297F,
			0.611597F,
			0.61281002F,
			0.61402202F,
			0.61523199F,
			0.61644F,
			0.61764699F,
			0.61885297F,
			0.62005699F,
			0.62125999F,
			0.62246102F,
			0.62366098F,
			0.62485999F,
			0.62605602F,
			0.62725198F,
			0.62844598F,
			0.62963802F,
			0.63082898F,
			0.63201898F,
			0.63320702F,
			0.63439298F,
			0.63557798F,
			0.63676202F,
			0.63794398F,
			0.63912398F,
			0.64030302F,
			0.64148098F,
			0.64265698F,
			0.64383203F,
			0.64500499F,
			0.64617598F,
			0.64734602F,
			0.64851397F,
			0.64968097F,
			0.65084702F,
			0.65201098F,
			0.65317303F,
			0.65433401F,
			0.65549302F,
			0.65665102F,
			0.65780699F,
			0.658961F,
			0.66011399F,
			0.66126603F,
			0.66241598F,
			0.66356403F,
			0.664711F,
			0.665856F,
			0.667F,
			0.66814202F,
			0.66928297F,
			0.67042202F,
			0.67155898F,
			0.67269498F,
			0.67382902F,
			0.67496198F,
			0.67609298F,
			0.67722201F,
			0.67834997F,
			0.67947602F,
			0.680601F,
			0.68172401F,
			0.68284601F,
			0.68396503F,
			0.68508399F,
			0.68620002F,
			0.68731499F,
			0.688429F,
			0.68954098F,
			0.690651F,
			0.69175899F,
			0.69286603F,
			0.69397098F,
			0.69507498F,
			0.69617701F,
			0.69727701F,
			0.698376F,
			0.69947302F,
			0.70056897F,
			0.70166302F,
			0.70275497F,
			0.70384502F,
			0.704934F,
			0.70602101F,
			0.70710701F,
			0.70819098F,
			0.70927298F,
			0.71035302F,
			0.71143198F,
			0.71250898F,
			0.71358502F,
			0.71465898F,
			0.71573102F,
			0.71680099F,
			0.71787F,
			0.71893698F,
			0.72000301F,
			0.721066F,
			0.72212797F,
			0.723189F,
			0.72424698F,
			0.72530401F,
			0.72635901F,
			0.727413F,
			0.72846401F,
			0.729514F,
			0.73056298F,
			0.73160899F,
			0.73265398F,
			0.733697F,
			0.73473901F,
			0.73577899F,
			0.736817F,
			0.73785299F,
			0.73888701F,
			0.73992002F,
			0.740951F,
			0.74198002F,
			0.74300802F,
			0.74403399F,
			0.745058F,
			0.74607998F,
			0.74710101F,
			0.748119F,
			0.74913597F,
			0.75015199F,
			0.75116497F,
			0.752177F,
			0.753187F,
			0.75419497F,
			0.75520098F,
			0.75620598F,
			0.757209F,
			0.75821F,
			0.75920898F,
			0.760207F,
			0.76120198F,
			0.762196F,
			0.763188F,
			0.76417899F,
			0.765167F,
			0.76615399F,
			0.76713902F,
			0.76812202F,
			0.76910299F,
			0.77008301F,
			0.771061F,
			0.77203602F,
			0.77301002F,
			0.773983F,
			0.77495301F,
			0.775922F,
			0.77688801F,
			0.77785301F,
			0.778817F,
			0.779778F,
			0.78073698F,
			0.78169501F,
			0.78265101F,
			0.78360498F,
			0.78455698F,
			0.78550702F,
			0.78645498F,
			0.78740197F,
			0.78834599F,
			0.789289F,
			0.79022998F,
			0.79116899F,
			0.79210699F,
			0.793042F,
			0.793975F,
			0.79490697F,
			0.79583699F,
			0.79676503F,
			0.79769099F,
			0.79861498F,
			0.799537F,
			0.80045801F,
			0.80137599F,
			0.802293F,
			0.80320799F,
			0.80412F,
			0.805031F,
			0.80593997F,
			0.80684799F,
			0.80775303F,
			0.80865598F,
			0.80955797F,
			0.81045699F,
			0.81135499F,
			0.81225097F,
			0.81314403F,
			0.81403601F,
			0.81492603F,
			0.81581402F,
			0.81670099F,
			0.81758499F,
			0.81846702F,
			0.81934798F,
			0.82022601F,
			0.82110202F,
			0.82197702F,
			0.82284999F,
			0.82372099F,
			0.82458901F,
			0.82545602F,
			0.82632101F,
			0.82718402F,
			0.82804501F,
			0.82890397F,
			0.82976103F,
			0.830616F,
			0.83147001F,
			0.83232099F,
			0.83317F,
			0.83401799F,
			0.83486301F,
			0.835706F,
			0.83654797F,
			0.83738703F,
			0.83822501F,
			0.83906001F,
			0.839894F,
			0.840725F,
			0.841555F,
			0.84238303F,
			0.84320801F,
			0.84403199F,
			0.844854F,
			0.84567302F,
			0.84649098F,
			0.84730703F,
			0.84811997F,
			0.84893203F,
			0.849742F,
			0.85055F,
			0.85135502F,
			0.85215902F,
			0.852961F,
			0.85376F,
			0.85455799F,
			0.85535401F,
			0.85614699F,
			0.85693902F,
			0.85772902F,
			0.85851598F,
			0.85930198F,
			0.86008501F,
			0.86086702F,
			0.861646F,
			0.86242402F,
			0.863199F,
			0.86397302F,
			0.86474401F,
			0.86551398F,
			0.86628097F,
			0.867046F,
			0.867809F,
			0.86857098F,
			0.86932999F,
			0.87008703F,
			0.87084198F,
			0.87159503F,
			0.87234598F,
			0.87309498F,
			0.873842F,
			0.874587F,
			0.87532902F,
			0.87607002F,
			0.876809F,
			0.877545F,
			0.87827998F,
			0.87901199F,
			0.87974298F,
			0.88047099F,
			0.88119698F,
			0.88192099F,
			0.88264298F,
			0.88336301F,
			0.88408101F,
			0.88479698F,
			0.88551098F,
			0.88622302F,
			0.88693202F,
			0.88764F,
			0.888345F,
			0.88904798F,
			0.88975F,
			0.89044899F,
			0.891146F,
			0.89184099F,
			0.89253402F,
			0.893224F,
			0.89391297F,
			0.89459902F,
			0.895284F,
			0.89596599F,
			0.89664602F,
			0.89732498F,
			0.89800102F,
			0.89867401F,
			0.89934599F,
			0.90001601F,
			0.90068299F,
			0.90134901F,
			0.90201199F,
			0.90267301F,
			0.903332F,
			0.90398902F,
			0.90464401F,
			0.90529698F,
			0.90594703F,
			0.906596F,
			0.907242F,
			0.90788603F,
			0.90852797F,
			0.909168F,
			0.90980601F,
			0.91044098F,
			0.911075F,
			0.91170597F,
			0.91233498F,
			0.91296202F,
			0.91358697F,
			0.91421002F,
			0.91483003F,
			0.91544902F,
			0.91606498F,
			0.91667902F,
			0.91729099F,
			0.91790098F,
			0.91850799F,
			0.91911399F,
			0.91971701F,
			0.92031801F,
			0.92091697F,
			0.92151397F,
			0.92210901F,
			0.922701F,
			0.92329103F,
			0.92387998F,
			0.924465F,
			0.92504901F,
			0.92563099F,
			0.92620999F,
			0.92678702F,
			0.92736298F,
			0.927935F,
			0.92850602F,
			0.929075F,
			0.92964101F,
			0.93020499F,
			0.930767F,
			0.93132699F,
			0.93188399F,
			0.93243998F,
			0.93299299F,
			0.93354398F,
			0.934093F,
			0.93463898F,
			0.935184F,
			0.93572599F,
			0.93626601F,
			0.93680298F,
			0.93733901F,
			0.93787199F,
			0.93840402F,
			0.938932F,
			0.93945903F,
			0.93998402F,
			0.94050598F,
			0.94102597F,
			0.941544F,
			0.94205999F,
			0.94257301F,
			0.943084F,
			0.94359303F,
			0.94410002F,
			0.94460499F,
			0.94510698F,
			0.94560701F,
			0.946105F,
			0.94660097F,
			0.94709402F,
			0.947586F,
			0.948075F,
			0.94856101F,
			0.94904602F,
			0.94952798F,
			0.95000798F,
			0.950486F,
			0.95096201F,
			0.95143503F,
			0.95190603F,
			0.95237499F,
			0.952842F,
			0.95330602F,
			0.95376801F,
			0.95422798F,
			0.95468599F,
			0.95514101F,
			0.955594F,
			0.95604497F,
			0.95649397F,
			0.95694F,
			0.957385F,
			0.95782602F,
			0.95826602F,
			0.95870298F,
			0.95913899F,
			0.959571F,
			0.96000201F,
			0.96043098F,
			0.96085697F,
			0.96127999F,
			0.96170199F,
			0.96212101F,
			0.962538F,
			0.96295297F,
			0.96336597F,
			0.96377599F,
			0.96418399F,
			0.96459001F,
			0.964993F,
			0.96539402F,
			0.96579301F,
			0.96618998F,
			0.96658403F,
			0.96697599F,
			0.96736598F,
			0.96775401F,
			0.96813899F,
			0.96852201F,
			0.96890301F,
			0.96928102F,
			0.969657F,
			0.97003102F,
			0.97040302F,
			0.97077203F,
			0.97113901F,
			0.97150397F,
			0.97186601F,
			0.97222698F,
			0.97258401F,
			0.97294003F,
			0.97329301F,
			0.97364402F,
			0.973993F,
			0.97433901F,
			0.974684F,
			0.975025F,
			0.97536498F,
			0.97570199F,
			0.97603703F,
			0.97636998F,
			0.97670001F,
			0.97702801F,
			0.97735399F,
			0.97767699F,
			0.97799802F,
			0.97831702F,
			0.978634F,
			0.978948F,
			0.97926003F,
			0.97956997F,
			0.979877F,
			0.98018199F,
			0.98048502F,
			0.98078501F,
			0.98108298F,
			0.98137897F,
			0.981673F,
			0.98196399F,
			0.98225302F,
			0.982539F,
			0.98282403F,
			0.983105F,
			0.98338503F,
			0.98366201F,
			0.98393703F,
			0.98421001F,
			0.98448002F,
			0.98474801F,
			0.98501402F,
			0.98527801F,
			0.98553902F,
			0.985798F,
			0.986054F,
			0.98630798F,
			0.98655999F,
			0.98680902F,
			0.98705697F,
			0.98730099F,
			0.987544F,
			0.98778403F,
			0.98802203F,
			0.988258F,
			0.988491F,
			0.98872203F,
			0.98895001F,
			0.98917699F,
			0.98940003F,
			0.989622F,
			0.98984098F,
			0.990058F,
			0.990273F,
			0.99048501F,
			0.990695F,
			0.99090302F,
			0.991108F,
			0.99131101F,
			0.99151099F,
			0.99171001F,
			0.99190599F,
			0.99209899F,
			0.99229097F,
			0.99247998F,
			0.99266601F,
			0.99285001F,
			0.99303198F,
			0.99321198F,
			0.99338901F,
			0.99356401F,
			0.99373698F,
			0.99390697F,
			0.994075F,
			0.99423999F,
			0.99440402F,
			0.99456501F,
			0.99472302F,
			0.99487901F,
			0.99503303F,
			0.99518502F,
			0.99533403F,
			0.99548101F,
			0.99562502F,
			0.995767F,
			0.99590701F,
			0.99604499F,
			0.99618F,
			0.99631298F,
			0.99644297F,
			0.996571F,
			0.99669701F,
			0.99681997F,
			0.99694097F,
			0.99706F,
			0.99717599F,
			0.99729002F,
			0.99740201F,
			0.99751103F,
			0.99761802F,
			0.99772298F,
			0.99782503F,
			0.99792498F,
			0.99802297F,
			0.99811798F,
			0.99821103F,
			0.99830198F,
			0.99839002F,
			0.99847603F,
			0.998559F,
			0.99864F,
			0.99871898F,
			0.99879497F,
			0.99887002F,
			0.998941F,
			0.99901098F,
			0.99907798F,
			0.99914199F,
			0.99920499F,
			0.99926502F,
			0.999322F,
			0.99937803F,
			0.99943101F,
			0.99948102F,
			0.999529F,
			0.99957502F,
			0.99961901F,
			0.99966002F,
			0.999699F,
			0.999735F,
			0.99976897F,
			0.99980098F,
			0.99983102F,
			0.99985802F,
			0.99988198F,
			0.99990499F,
			0.99992502F,
			0.999942F,
			0.99995798F,
			0.99997097F,
			0.99998099F,
			0.99998897F,
			0.99999499F,
			0.99999899F,
			1.0F
		};

		/// <summary>
		/// Get the sine of an angle in BAMS.
		/// </summary>
		/// <param name="angle">The angle in BAMS.</param>
		/// <returns>The sine of the angle.</returns>
		public static float NJSin(int angle)
		{
			int a1 = angle;
			float v8 = a1;
			var v4 = (byte)a1;
			int v3 = (a1 >> 4) & 0xFFF;
			int v2 = v4 & 0xF;
			int v1 = v3 & 0xC00;
			if (v2 != 0)
			{
				double v6;
				double v7;
				if (v1 > 0x800)
				{
					if (v1 == 0xC00)
					{
						v7 = -sineTable[4096 - v3];
						v6 = -sineTable[4096 + -v3 - 1];
						return (float)(v7 + (v6 - v7) * v2 * 0.0625);
					}
				}
				else
				{
					if (v1 == 0x800)
					{
						v7 = -sineTable[-2048 + v3];
						v6 = -sineTable[-2047 + v3];
						return (float)(v7 + (v6 - v7) * v2 * 0.0625);
					}
					if ((v3 & 0xC00) == 0)
					{
						v7 = sineTable[v3];
						v6 = sineTable[1 + v3];
						return (float)(v7 + (v6 - v7) * v2 * 0.0625);
					}
					if (v1 == 0x400)
					{
						v7 = sineTable[2048 - v3];
						v6 = sineTable[2048 + -v3 - 1];
						return (float)(v7 + (v6 - v7) * v2 * 0.0625);
					}
				}
				v7 = v8;
				v6 = v8;
				return (float)(v7 + (v6 - v7) * v2 * 0.0625);
			}
			if (v1 > 0x800)
			{
				if (v1 == 0xC00)
					return -sineTable[4096 - v3];
			}
			else
			{
				if (v1 == 0x800)
					return -sineTable[-2048 + v3];
				if ((v3 & 0xC00) == 0)
					return sineTable[v3];
				if (v1 == 0x400)
					return sineTable[2048 - v3];
			}
			return v8;
		}

		/// <summary>
		/// Get the inverse sine of an angle in BAMS.
		/// </summary>
		/// <param name="angle">The angle in BAMS.</param>
		/// <returns>The inverse sine of the angle.</returns>
		public static float NJCos(int angle)
		{
			int a1 = angle;
			float v8 = a1;
			var v4 = (byte)a1;
			int v3 = (a1 >> 4) & 0xFFF;
			int v2 = v4 & 0xF;
			int v1 = v3 & 0xC00;
			if (v2 != 0)
			{
				double v6;
				double v7;
				if (v1 > 0x800)
				{
					if (v1 == 0xC00)
					{
						v7 = sineTable[-3072 + v3];
						v6 = sineTable[-3071 + v3];
						return (float)(v7 + (v6 - v7) * v2 * 0.0625);
					}
				}
				else
				{
					if (v1 == 0x800)
					{
						v7 = -sineTable[3072 - v3];
						v6 = -sineTable[3072 + -v3 - 1];
						return (float)(v7 + (v6 - v7) * v2 * 0.0625);
					}
					if ((v3 & 0xC00) == 0)
					{
						v7 = sineTable[(sineTable.Length - 1) + -v3];
						v6 = sineTable[(sineTable.Length - 1) + -v3 - 1];
						return (float)(v7 + (v6 - v7) * v2 * 0.0625);
					}
					if (v1 == 0x400)
					{
						v7 = -sineTable[-1024 + v3];
						v6 = -sineTable[-1023 + v3];
						return (float)(v7 + (v6 - v7) * v2 * 0.0625);
					}
				}
				v7 = v8;
				v6 = v8;
				return (float)(v7 + (v6 - v7) * v2 * 0.0625);
			}
			if (v1 > 0x800)
			{
				if (v1 == 0xC00)
					return sineTable[-3072 + v3];
			}
			else
			{
				if (v1 == 0x800)
					return -sineTable[3072 - v3];
				if ((v3 & 0xC00) == 0)
					return sineTable[(sineTable.Length - 1) - v3];
				if (v1 == 0x400)
					return -sineTable[-1024 + v3];
			}
			return v8;
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
}