using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.Security.Cryptography.Xml;
using Color = System.Drawing.Color;

namespace SAModel.Direct3D
{
	public static partial class Extensions
	{
		private const double halfpi = Math.PI / 2;

		static Extensions()
		{
			for (int i = 0; i < 1025; i++)
				sineTable[i] = (float)Math.Sin(halfpi * (i / 1024.0));
		}

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
			if (bitmap.PixelFormat != PixelFormat.Format32bppArgb) bitmap = bitmap.Clone(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format32bppArgb);
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
					Power = material.Exponent
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
			if (attach?.MeshInfo == null) return null;
			foreach (MeshInfo item in attach.MeshInfo)
			{
				numverts += item.Vertices.Length;
				if (item.HasUV)
					data |= 1;
				if (item.HasVC)
					data |= 2;
				if (item.IsMod)
					data |= 4;
			}
			if (numverts == 0) return null;
			switch (data)
			{
				case 4:
					return new Mesh<FVF_PositionColored>(attach);
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
			var node = obj.EnumerateObjects().FirstOrDefault(a => a.Attach != null);
			switch (node?.Attach)
			{
				case BasicAttach:
					return obj.ProcessWeightedBasicModel();
				case ChunkAttach:
					return obj.ProcessWeightedChunkModel();
				default:
					return Enumerable.Repeat<Mesh>(null, obj.EnumerateObjects().Count()).ToList();
			}
		}

		private static List<Mesh> ProcessWeightedBasicModel(this NJS_OBJECT obj)
		{
			NJS_OBJECT[] nodes = obj.GetObjects();
			List<Matrix> matrices = new List<Matrix>();
			obj.GetMatrices(new MatrixStack(), matrices);
			List<Mesh> meshes = new List<Mesh>();
			int mdlindex = -1;
			foreach (var o2 in obj.EnumerateObjects())
			{
				mdlindex++;
				if (o2.Attach is BasicAttach basatt)
					meshes.Add(ProcessWeightedBasicAttach(basatt, nodes, matrices, mdlindex));
				else
					meshes.Add(null);
			}
			return meshes;
		}

		private static Mesh ProcessWeightedBasicAttach(BasicAttach attach, NJS_OBJECT[] nodes, List<Matrix> matrices, int mdlindex)
		{
			Vertex[] newVerts = (Vertex[])attach.Vertex.Clone();
			Vertex[] newNorms = (Vertex[])attach.Normal.Clone();
			List<WeightData>[] weightBuf = new List<WeightData>[newVerts.Length];
			for (int i = 0; i < newVerts.Length; i++)
			{
				Vector3 newpos, newnor;
				if (attach.VertexWeights != null && attach.VertexWeights.TryGetValue(i, out var vw))
				{
					List<WeightData> wd = new List<WeightData>(vw.Count);
					newpos = new Vector3();
					newnor = new Vector3();
					foreach (var v in vw)
					{
						int ni = Array.IndexOf(nodes, v.Node);
						var va = (BasicAttach)v.Node.Attach;
						var origpos = va.Vertex[v.Vertex].ToVector3();
						newpos += Vector3.TransformCoordinate(origpos, matrices[ni]) * v.Weight;
						Vector3 orignor = va.Normal[v.Vertex].ToVector3();
						newnor += Vector3.TransformNormal(orignor, matrices[ni]) * v.Weight;
						wd.Add(new WeightData(ni, origpos, orignor, v.Weight));
					}
					newVerts[i] = newpos.ToVertex();
					newNorms[i] = Vector3.Normalize(newnor).ToVertex();
					weightBuf[i] = wd;
				}
				else
				{
					var origpos = newVerts[i].ToVector3();
					var orignor = newNorms[i].ToVector3();
					newVerts[i] = Vector3.TransformCoordinate(origpos, matrices[mdlindex]).ToVertex();
					newNorms[i] = Vector3.TransformNormal(orignor, matrices[mdlindex]).ToVertex();
					weightBuf[i] = new List<WeightData>() { new WeightData(mdlindex, origpos, orignor, 1) };
				}
			}

			List<MeshInfo> result = new List<MeshInfo>();
			List<List<WeightData>> weights = new List<List<WeightData>>();
			foreach (NJS_MESHSET mesh in attach.Mesh)
			{
				bool hasVColor = mesh.VColor != null;
				bool hasUV = mesh.UV != null;
				List<Poly> polys = new List<Poly>();
				List<VertexData> verts = new List<VertexData>();
				int currentstriptotal = 0;
				foreach (Poly poly in mesh.Poly)
				{
					Poly newpoly = null;
					switch (mesh.PolyType)
					{
						case Basic_PolyType.Triangles:
							newpoly = new Triangle();
							break;
						case Basic_PolyType.Quads:
							newpoly = new Quad();
							break;
						case Basic_PolyType.NPoly:
						case Basic_PolyType.Strips:
							newpoly = new Strip(poly.Indexes.Length, ((Strip)poly).Reversed);
							break;
					}
					for (int i = 0; i < poly.Indexes.Length; i++)
					{
						var v = new VertexData(
							newVerts[poly.Indexes[i]],
							newNorms[poly.Indexes[i]],
							hasVColor ? Color.FromArgb(mesh.VColor[currentstriptotal].R, mesh.VColor[currentstriptotal].G, mesh.VColor[currentstriptotal].B) : null,
							hasUV ? mesh.UV[currentstriptotal++] : null);
						if (verts.Contains(v))
							newpoly.Indexes[i] = (ushort)verts.IndexOf(v);
						else
						{
							weights.Add(weightBuf[poly.Indexes[i]]);
							newpoly.Indexes[i] = (ushort)verts.Count;
							verts.Add(v);
						}
					}
					polys.Add(newpoly);
				}
				NJS_MATERIAL mat = null;
				if (attach.Material != null && mesh.MaterialID < attach.Material.Count)
					mat = attach.Material[mesh.MaterialID];
				result.Add(new MeshInfo(mat, polys.ToArray(), verts.ToArray(), hasUV, hasVColor));
			}
			attach.MeshInfo = result.ToArray();
			if (attach.MeshInfo.All(a => a.Vertices.Length == 0))
				return null;
			if (attach.MeshInfo.Any(a => a.HasUV))
				return new WeightedMesh<FVF_PositionNormalTexturedColored>(attach, weights);
			else
				return new WeightedMesh<FVF_PositionNormalColored>(attach, weights);
		}

		public static List<Mesh> ProcessWeightedChunkModel(this NJS_OBJECT obj)
		{
			List<Mesh> meshes = new List<Mesh>();
			int mdlindex = -1;
			do
			{
				ProcessWeightedChunkModel(obj, new MatrixStack(), meshes, ref mdlindex);
				obj = obj.Sibling;
			} while (obj != null);
			return meshes;
		}

		private static void ProcessWeightedChunkModel(NJS_OBJECT obj, MatrixStack transform, List<Mesh> meshes, ref int mdlindex)
		{
			mdlindex++;
			transform.Push();
			obj.ProcessTransforms(transform);
			if (obj.Attach is ChunkAttach cnkatt)
				meshes.Add(ProcessWeightedChunkAttach(cnkatt, transform, mdlindex));
			else
				meshes.Add(null);
			foreach (NJS_OBJECT child in obj.Children)
				ProcessWeightedChunkModel(child, transform, meshes, ref mdlindex);
			transform.Pop();
		}


		private static Mesh ProcessWeightedChunkAttach(ChunkAttach attach, MatrixStack transform, int mdlindex)
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
							var weight = weightByte / (weightByte > 255 ? 65535f : 255f);
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
								if (chunk.WeightStatus == WeightStatus.End)
									cacheVertex.Normal = Vector3.Normalize(cacheVertex.Normal.ToVector3()).ToVertex();
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
			if (attach.MeshInfo.All(a => a.Vertices.Length == 0))
				return null;
			if (attach.MeshInfo.Any(a => a.HasUV))
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
										hasVColor ? strip.VColors[k] : VertexBuffer[strip.Indexes[k]].Color,
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

		public static void UpdateWeightedModelAnimated(this NJS_OBJECT obj, MatrixStack transform, NJS_MOTION anim, float animframe, Mesh[] meshes)
		{
			List<Matrix> matrices = new List<Matrix>();
			obj.GetMatricesAnimated(transform, anim, animframe, matrices);
			for (int i = 0; i < meshes.Length; i++)
				if (meshes[i] is IWeightedMesh mesh)
					mesh.Update(matrices);
		}

		public static void GetMatricesAnimated(this NJS_OBJECT obj, MatrixStack transform, NJS_MOTION anim, float animframe, List<Matrix> matrices)
		{
			int animindex = -1;
			do
			{
				obj.GetMatricesAnimated(transform, anim, animframe, ref animindex, matrices);
				obj = obj.Sibling;
			} while (obj != null);
		}

		private static void GetMatricesAnimated(this NJS_OBJECT obj, MatrixStack transform, NJS_MOTION anim, float animframe, ref int animindex, List<Matrix> matrices)
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
								DiffuseColor = Color.FromArgb(128, 128, 128, 255),
								IgnoreLighting = true,
								UseAlpha = true,
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
	
		private static List<RenderInfo> DrawModelTreeAnimated(this NJS_OBJECT obj, FillMode fillMode, MatrixStack transform, Texture[] textures, Mesh[] meshes, NJS_MOTION anim, float animframe, ref int modelindex, ref int animindex, bool ignorematcolors = false, bool ignorelight = false, bool invert = false, bool boundsByMesh = false)
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

		private static List<RenderInfo> DrawModelTreeAnimatedInvert(this NJS_OBJECT obj, MatrixStack transform, Mesh[] meshes, NJS_MOTION anim, float animframe, ref int modelindex, ref int animindex, bool ignorematcolor = false, bool ignorelight = false, bool invert = false, bool boundsByMesh = false)
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

		public static List<RenderInfo> DrawModelTreeAnimated(this NJS_OBJECT obj, FillMode fillMode, MatrixStack transform, Texture[] textures, Mesh[] meshes, NJS_MOTION anim, float animframe, bool ignorematcolor = false, bool ignorelight = false, bool invert = false, bool boundsByMesh = false)
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

		public static List<RenderInfo> DrawModelTreeAnimatedInvert(this NJS_OBJECT obj, MatrixStack transform, Mesh[] meshes, NJS_MOTION anim, float animframe)
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

		public static HitResult CheckHitAnimated(this NJS_OBJECT obj, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform, Mesh[] mesh, NJS_MOTION anim, float animframe)
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

		private static HitResult CheckHitAnimated(this NJS_OBJECT obj, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform, Mesh[] mesh, NJS_MOTION anim, float animframe, ref int modelindex, ref int animindex)
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

		public static Matrix ProcessTransforms(this NJS_OBJECT obj, AnimModelData anim, float animframe, Matrix matrix)
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
			else if (anim.Quaternion.Count > 0)
				rotation = anim.GetQuaternion(animframe);
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

		public static void ProcessTransforms(this NJS_OBJECT obj, AnimModelData anim, float animframe, MatrixStack transform)
		{
			transform.LoadMatrix(obj.ProcessTransforms(anim, animframe, transform.Top));
		}

		private static readonly float[] sineTable = new float[1025];

		/// <summary>
		/// Get the sine of an angle in BAMS.
		/// </summary>
		/// <param name="angle">The angle in BAMS.</param>
		/// <returns>The sine of the angle.</returns>
		public static float NJSin(int angle)
		{
			int v3 = (angle >> 4) & 0xFFF;
			int v2 = angle & 0xF;
			if (v2 != 0)
			{
				double v6;
				double v7;
				switch (v3 & 0xC00)
				{
					case 0:
						v7 = sineTable[v3];
						v6 = sineTable[1 + v3];
						return (float)(v7 + (v6 - v7) * v2 * 0.0625);
					case 0x400:
						v7 = sineTable[2048 - v3];
						v6 = sineTable[2048 + -v3 - 1];
						return (float)(v7 + (v6 - v7) * v2 * 0.0625);
					case 0x800:
						v7 = -sineTable[-2048 + v3];
						v6 = -sineTable[-2047 + v3];
						return (float)(v7 + (v6 - v7) * v2 * 0.0625);
					case 0xC00:
						v7 = -sineTable[4096 - v3];
						v6 = -sineTable[4096 + -v3 - 1];
						return (float)(v7 + (v6 - v7) * v2 * 0.0625);
					default:
						return angle;
				}
			}
			switch (v3 & 0xC00)
			{
				case 0:
					return sineTable[v3];
				case 0x400:
					return sineTable[2048 - v3];
				case 0x800:
					return -sineTable[-2048 + v3];
				case 0xC00:
					return -sineTable[4096 - v3];
				default:
					return angle;
			}
		}

		/// <summary>
		/// Get the cosine of an angle in BAMS.
		/// </summary>
		/// <param name="angle">The angle in BAMS.</param>
		/// <returns>The cosine of the angle.</returns>
		public static float NJCos(int angle)
		{
			int v3 = (angle >> 4) & 0xFFF;
			int v2 = angle & 0xF;
			if (v2 != 0)
			{
				double v6;
				double v7;
				switch (v3 & 0xC00)
				{
					case 0:
						v7 = sineTable[1024 + -v3];
						v6 = sineTable[1024 + -v3 - 1];
						return (float)(v7 + (v6 - v7) * v2 * 0.0625);
					case 0x400:
						v7 = -sineTable[-1024 + v3];
						v6 = -sineTable[-1023 + v3];
						return (float)(v7 + (v6 - v7) * v2 * 0.0625);
					case 0x800:
						v7 = -sineTable[3072 - v3];
						v6 = -sineTable[3072 + -v3 - 1];
						return (float)(v7 + (v6 - v7) * v2 * 0.0625);
					case 0xC00:
						v7 = sineTable[-3072 + v3];
						v6 = sineTable[-3071 + v3];
						return (float)(v7 + (v6 - v7) * v2 * 0.0625);
					default:
						return angle;
				}
			}
			switch (v3 & 0xC00)
			{
				case 0:
					return sineTable[1024 - v3];
				case 0x400:
					return -sineTable[-1024 + v3];
				case 0x800:
					return -sineTable[3072 - v3];
				case 0xC00:
					return sineTable[-3072 + v3];
				default:
					return angle;
			}
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