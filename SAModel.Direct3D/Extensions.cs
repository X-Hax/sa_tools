using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using SharpDX;
using Color = System.Drawing.Color;
using SharpDX.Direct3D9;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SonicRetro.SAModel.Direct3D
{
	public static class Extensions
	{
		public static Vector3 ToVector3(this Vertex vert) => new Vector3(vert.X, vert.Y, vert.Z);

		public static Vector3 ToVector3(this Rotation rotation) => new Vector3(rotation.XDeg, rotation.YDeg, rotation.ZDeg);

		public static Color ToColor(this Vertex vert) => Color.FromArgb(255, (int)(vert.X * 255), (int)(vert.Y * 255), (int)(vert.Z * 255));

		public static Vertex ToVertex(this Vector3 input) => new Vertex(input.X, input.Y, input.Z);

		public static BoundingSphere ToSAModel(this SharpDX.BoundingSphere sphere) => new BoundingSphere(sphere.Center.ToVertex(), sphere.Radius);

		public static SharpDX.BoundingSphere ToSharpDX(this BoundingSphere sphere) => new SharpDX.BoundingSphere(sphere.Center.ToVector3(), sphere.Radius);

		public static SharpDX.Mathematics.Interop.RawColor4 ToRawColor4(this Color color) => new SharpDX.Mathematics.Interop.RawColor4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);

		public static SharpDX.Mathematics.Interop.RawColorBGRA ToRawColorBGRA(this Color color) => new SharpDX.Mathematics.Interop.RawColorBGRA(color.B, color.G, color.R, color.A);

		public static SharpDX.Mathematics.Interop.RawColor4 ToRawColor4(this Vertex vertex) => new SharpDX.Mathematics.Interop.RawColor4(vertex.X, vertex.Y, vertex.Z, 1);

		public static Texture ToTexture(this Bitmap bitmap, Device device)
		{
			if (bitmap.PixelFormat != PixelFormat.Format32bppArgb) bitmap = bitmap.Clone(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format32bppArgb);
			BitmapData bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			byte[] tmp = new byte[Math.Abs(bitmapData.Stride) * bitmapData.Height];
			Marshal.Copy(bitmapData.Scan0, tmp, 0, tmp.Length);
			bitmap.UnlockBits(bitmapData);
			Texture texture = new Texture(device, bitmap.Width, bitmap.Height, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
			DataRectangle dataRectangle = texture.LockRectangle(0, LockFlags.None);
			Marshal.Copy(tmp, 0, dataRectangle.DataPointer, tmp.Length);
			texture.UnlockRectangle(0);
			return texture;
		}

		public static void CalculateBounds(this Attach attach)
		{
			List<Vector3> verts = new List<Vector3>();
			foreach (MeshInfo mesh in attach.MeshInfo)
				foreach (VertexData vert in mesh.Vertices)
					verts.Add(vert.Position.ToVector3());
			attach.Bounds = SharpDX.BoundingSphere.FromPoints(verts.ToArray()).ToSAModel();
		}

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
				if (!material.SuperSample)
				{
					device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.None);
					device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.None);
					device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.None);
				}
				device.SetTexture(0, material.UseTexture ? texture : null);
				device.SetRenderState(RenderState.Ambient, (material.IgnoreLighting ? Color.White : Color.Black).ToArgb());
				device.SetRenderState(RenderState.AlphaBlendEnable, material.UseAlpha);

				if (material.UseAlpha)
					device.SetRenderState(RenderState.Ambient, material.DiffuseColor.ToArgb());

				switch (material.DestinationAlpha)
				{
					case AlphaInstruction.Zero:
						device.SetRenderState(RenderState.DestinationBlendAlpha, Blend.Zero);
						break;

					case AlphaInstruction.One:
						device.SetRenderState(RenderState.DestinationBlendAlpha, Blend.One);
						break;

					case AlphaInstruction.OtherColor:
						break;

					case AlphaInstruction.InverseOtherColor:
						break;

					case AlphaInstruction.SourceAlpha:
						device.SetRenderState(RenderState.DestinationBlendAlpha, Blend.SourceAlpha);
						break;

					case AlphaInstruction.InverseSourceAlpha:
						device.SetRenderState(RenderState.DestinationBlendAlpha, Blend.InverseSourceAlpha);
						break;

					case AlphaInstruction.DestinationAlpha:
						device.SetRenderState(RenderState.DestinationBlendAlpha, Blend.DestinationAlpha);
						break;

					case AlphaInstruction.InverseDestinationAlpha:
						device.SetRenderState(RenderState.DestinationBlendAlpha, Blend.InverseDestinationAlpha);
						break;
				}
				switch (material.SourceAlpha)
				{
					case AlphaInstruction.Zero:
						device.SetRenderState(RenderState.SourceBlendAlpha, Blend.Zero);
						break;

					case AlphaInstruction.One:
						device.SetRenderState(RenderState.SourceBlendAlpha, Blend.One);
						break;

					case AlphaInstruction.OtherColor:
						break;

					case AlphaInstruction.InverseOtherColor:
						break;

					case AlphaInstruction.SourceAlpha:
						device.SetRenderState(RenderState.SourceBlendAlpha, Blend.SourceAlpha);
						break;

					case AlphaInstruction.InverseSourceAlpha:
						device.SetRenderState(RenderState.SourceBlendAlpha, Blend.InverseSourceAlpha);
						break;

					case AlphaInstruction.DestinationAlpha:
						device.SetRenderState(RenderState.SourceBlendAlpha, Blend.DestinationAlpha);
						break;

					case AlphaInstruction.InverseDestinationAlpha:
						device.SetRenderState(RenderState.SourceBlendAlpha, Blend.InverseDestinationAlpha);
						break;
				}
				device.SetTextureStageState(0, TextureStage.TexCoordIndex, material.EnvironmentMap ? (int)TextureCoordIndex.SphereMap : 0);
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
			List<Vector3> verts = new List<Vector3>();
			foreach (VertexData vert in attach.MeshInfo[mesh].Vertices)
				verts.Add(Vector3.TransformCoordinate(vert.Position.ToVector3(), transform));
			return SharpDX.BoundingSphere.FromPoints(verts.ToArray()).ToSAModel();
		}

		public static void CalculateBounds(this COL col)
		{
			Matrix matrix = col.Model.ProcessTransforms(Matrix.Identity);
			List<Vector3> verts = new List<Vector3>();
			foreach (MeshInfo mesh in col.Model.Attach.MeshInfo)
				foreach (VertexData vert in mesh.Vertices)
					verts.Add(Vector3.TransformCoordinate(vert.Position.ToVector3(), matrix));
			col.Bounds = SharpDX.BoundingSphere.FromPoints(verts.ToArray()).ToSAModel();
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

		public static Mesh CreateD3DMesh(this Attach attach, Device dev)
		{
			int numverts = 0;
			byte data = 0;
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
					return new Mesh<FVF_PositionNormalTexturedColored>(attach, dev);
				case 2:
					return new Mesh<FVF_PositionNormalColored>(attach, dev);
				case 1:
					return new Mesh<FVF_PositionNormalTextured>(attach, dev);
				default:
					return new Mesh<FVF_PositionNormal>(attach, dev);
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
		static VertexData[] VertexBuffer = new VertexData[4095];
		static readonly CachedPoly[] PolyCache = new CachedPoly[255];

		public static List<Mesh> ProcessWeightedModel(this NJS_OBJECT obj, Device device)
		{
			List<Mesh> meshes = new List<Mesh>();
			ProcessWeightedModel(obj, device, new MatrixStack(), meshes);
			return meshes;
		}

		private static void ProcessWeightedModel(NJS_OBJECT obj, Device device, MatrixStack transform, List<Mesh> meshes)
		{
			transform.Push();
			obj.ProcessTransforms(transform);
			if (obj.Attach is ChunkAttach attach)
				meshes.Add(ProcessWeightedAttach(device, transform, attach));
			else
				meshes.Add(null);
			foreach (NJS_OBJECT child in obj.Children)
				ProcessWeightedModel(child, device, transform, meshes);
			transform.Pop();
		}

		public static List<Mesh> ProcessWeightedModelAnimated(this NJS_OBJECT obj, Device device, Animation anim, int animframe)
		{
			List<Mesh> meshes = new List<Mesh>();
			int animindex = -1;
			ProcessWeightedModelAnimated(obj, device, new MatrixStack(), meshes, anim, animframe, ref animindex);
			return meshes;
		}

		private static void ProcessWeightedModelAnimated(NJS_OBJECT obj, Device device, MatrixStack transform, List<Mesh> meshes, Animation anim, int animframe, ref int animindex)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			bool animate = obj.Animate;
			if (animate) animindex++;
			if (!anim.Models.ContainsKey(animindex)) animate = false;
			if (animate)
				obj.ProcessTransforms(anim.Models[animindex], animframe, transform);
			else
				obj.ProcessTransforms(transform);
			if (obj.Attach is ChunkAttach attach)
				meshes.Add(ProcessWeightedAttach(device, transform, attach));
			else
				meshes.Add(null);
			foreach (NJS_OBJECT child in obj.Children)
				ProcessWeightedModelAnimated(child, device, transform, meshes, anim, animframe, ref animindex);
			transform.Pop();
		}

		private static Mesh ProcessWeightedAttach(Device device, MatrixStack transform, ChunkAttach attach)
		{
			if (attach.Vertex != null)
			{
				foreach (VertexChunk chunk in attach.Vertex)
				{
					if (VertexBuffer.Length < chunk.IndexOffset + chunk.VertexCount)
						Array.Resize(ref VertexBuffer, chunk.IndexOffset + chunk.VertexCount);
					if (chunk.HasWeight)
					{
						for (int i = 0; i < chunk.VertexCount; i++)
						{
							var weightByte = chunk.NinjaFlags[i] >> 16;
							var weight = weightByte * (1f / 255f);
							var position = (Vector3.TransformCoordinate(chunk.Vertices[i].ToVector3(), transform.Top) * weight).ToVertex();
							Vertex normal = null;
							if (chunk.Normals.Count > 0)
								normal = (Vector3.TransformNormal(chunk.Normals[i].ToVector3(), transform.Top) * weight).ToVertex();

							// Store vertex in cache
							var vertexId = chunk.NinjaFlags[i] & 0x0000FFFF;
							var vertexCacheId = (int)(chunk.IndexOffset + vertexId);

							if (chunk.WeightStatus == WeightStatus.Start)
							{
								// Add new vertex to cache
								VertexBuffer[vertexCacheId] = new VertexData(position, normal);
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
							}
						}
					}
					else
						for (int i = 0; i < chunk.VertexCount; i++)
						{
							var position = Vector3.TransformCoordinate(chunk.Vertices[i].ToVector3(), transform.Top).ToVertex();
							Vertex normal = null;
							if (chunk.Normals.Count > 0)
								normal = Vector3.TransformNormal(chunk.Normals[i].ToVector3(), transform.Top).ToVertex();
							VertexBuffer[i + chunk.IndexOffset] = new VertexData(position, normal);
							if (chunk.Diffuse.Count > 0)
								VertexBuffer[i + chunk.IndexOffset].Color = chunk.Diffuse[i];
						}
				}
			}
			List<MeshInfo> result = new List<MeshInfo>();
			if (attach.Poly != null)
				result = ProcessPolyList(attach.Poly, 0);
			attach.MeshInfo = result.ToArray();
			return attach.CreateD3DMesh(device);
		}

		private static List<MeshInfo> ProcessPolyList(List<PolyChunk> strips, int start)
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
						result.AddRange(ProcessPolyList(cached.Polys, cached.Index));
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
									str.Indexes[k] = (ushort)verts.AddUnique(new VertexData(
										VertexBuffer[strip.Indexes[k]].Position,
										VertexBuffer[strip.Indexes[k]].Normal,
										hasVColor ? (Color?)strip.VColors[k] : VertexBuffer[strip.Indexes[k]].Color,
										hasUV ? strip.UVs[k] : null));
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

		public static List<RenderInfo> DrawModel(this NJS_OBJECT obj, Device device, MatrixStack transform, Texture[] textures, Mesh mesh, bool useMat)
		{
			List<RenderInfo> result = new List<RenderInfo>();

			if (mesh == null)
				return result;

			transform.Push();
			obj.ProcessTransforms(transform);

			if (obj.Attach != null)
			{
				for (int j = 0; j < obj.Attach.MeshInfo.Length; j++)
				{
					NJS_MATERIAL mat;
					Texture texture = null;
					// HACK: When useMat is true, mat shouldn't be null. However, checking it anyway ensures Sky Deck 3 loads.
					// If it is in fact null, it applies a placeholder so that the editor doesn't crash.
					if (useMat && obj.Attach.MeshInfo[j].Material != null)
					{
						mat = obj.Attach.MeshInfo[j].Material;

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
					result.Add(new RenderInfo(mesh, j, transform.Top, mat, texture, device.GetRenderState<FillMode>(RenderState.FillMode), obj.Attach.CalculateBounds(j, transform.Top)));
				}
			}

			transform.Pop();
			return result;
		}

		public static List<RenderInfo> DrawModelInvert(this NJS_OBJECT obj, MatrixStack transform, Mesh mesh, bool useMat)
		{
			List<RenderInfo> result = new List<RenderInfo>();

			if (mesh == null)
				return result;

			transform.Push();
			obj.ProcessTransforms(transform);
			if (obj.Attach != null)
				for (int j = 0; j < obj.Attach.MeshInfo.Length; j++)
				{
					Color col = Color.White;
					if (useMat) col = obj.Attach.MeshInfo[j].Material.DiffuseColor;
					col = Color.FromArgb(255 - col.R, 255 - col.G, 255 - col.B);
					NJS_MATERIAL mat = new NJS_MATERIAL
					{
						DiffuseColor = col,
						IgnoreLighting = true,
						UseAlpha = false
					};
					result.Add(new RenderInfo(mesh, j, transform.Top, mat, null, FillMode.Wireframe, obj.Attach.CalculateBounds(j, transform.Top)));
				}
			transform.Pop();
			return result;
		}

		public static List<RenderInfo> DrawModelTree(this NJS_OBJECT obj, Device device, MatrixStack transform, Texture[] textures, Mesh[] meshes)
		{
			int modelindex = -1;
			return obj.DrawModelTree(device, transform, textures, meshes, ref modelindex);
		}

		private static List<RenderInfo> DrawModelTree(this NJS_OBJECT obj, Device device, MatrixStack transform, Texture[] textures, Mesh[] meshes, ref int modelindex)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			modelindex++;
			obj.ProcessTransforms(transform);

			if (obj.Attach != null & meshes[modelindex] != null)
			{
				for (int j = 0; j < obj.Attach.MeshInfo.Length; j++)
				{
					Texture texture = null;
					NJS_MATERIAL mat = obj.Attach.MeshInfo[j].Material;
					// HACK: Null material hack 2: Fixes display of objects in SADXLVL2, Twinkle Park 1
					if (textures != null && mat != null && mat.TextureID < textures.Length)
						texture = textures[mat.TextureID];
					result.Add(new RenderInfo(meshes[modelindex], j, transform.Top, mat, texture, device.GetRenderState<FillMode>(RenderState.FillMode), obj.Attach.CalculateBounds(j, transform.Top)));
				}
			}

			foreach (NJS_OBJECT child in obj.Children)
				result.AddRange(DrawModelTree(child, device, transform, textures, meshes, ref modelindex));
			transform.Pop();
			return result;
		}

		public static List<RenderInfo> DrawModelTreeInvert(this NJS_OBJECT obj, MatrixStack transform, Mesh[] meshes)
		{
			int modelindex = -1;
			return obj.DrawModelTreeInvert(transform, meshes, ref modelindex);
		}

		private static List<RenderInfo> DrawModelTreeInvert(this NJS_OBJECT obj, MatrixStack transform, Mesh[] meshes, ref int modelindex)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			modelindex++;
			obj.ProcessTransforms(transform);

			if (obj.Attach != null & meshes[modelindex] != null)
			{
				for (int j = 0; j < obj.Attach.MeshInfo.Length; j++)
				{
					Color color = Color.Black;

					// HACK: Null material hack 3: Fixes selecting objects in SADXLVL2, Twinkle Park 1.
					if (obj.Attach.MeshInfo[j].Material != null)
						color = obj.Attach.MeshInfo[j].Material.DiffuseColor;

					color = Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
					NJS_MATERIAL mat = new NJS_MATERIAL
					{
						DiffuseColor = color,
						IgnoreLighting = true,
						UseAlpha = false
					};
					result.Add(new RenderInfo(meshes[modelindex], j, transform.Top, mat, null, FillMode.Wireframe, obj.Attach.CalculateBounds(j, transform.Top)));
				}
			}

			foreach (NJS_OBJECT child in obj.Children)
				result.AddRange(DrawModelTreeInvert(child, transform, meshes, ref modelindex));
			transform.Pop();
			return result;
		}

		public static List<RenderInfo> DrawModelTreeAnimated(this NJS_OBJECT obj, Device device, MatrixStack transform, Texture[] textures, Mesh[] meshes, Animation anim, int animframe)
		{
			int modelindex = -1;
			int animindex = -1;
			return obj.DrawModelTreeAnimated(device, transform, textures, meshes, anim, animframe, ref modelindex, ref animindex);
		}

		private static List<RenderInfo> DrawModelTreeAnimated(this NJS_OBJECT obj, Device device, MatrixStack transform, Texture[] textures, Mesh[] meshes, Animation anim, int animframe, ref int modelindex, ref int animindex)
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
				for (int j = 0; j < obj.Attach.MeshInfo.Length; j++)
				{
					Texture texture = null;
					NJS_MATERIAL mat = obj.Attach.MeshInfo[j].Material;
					if (textures != null && mat.TextureID < textures.Length)
						texture = textures[mat.TextureID];
					result.Add(new RenderInfo(meshes[modelindex], j, transform.Top, mat, texture, device.GetRenderState<FillMode>(RenderState.FillMode), obj.Attach.CalculateBounds(j, transform.Top)));
				}
			foreach (NJS_OBJECT child in obj.Children)
				result.AddRange(DrawModelTreeAnimated(child, device, transform, textures, meshes, anim, animframe, ref modelindex, ref animindex));
			transform.Pop();
			return result;
		}

		public static List<RenderInfo> DrawModelTreeAnimatedInvert(this NJS_OBJECT obj, MatrixStack transform, Mesh[] meshes, Animation anim, int animframe)
		{
			int modelindex = -1;
			int animindex = -1;
			return obj.DrawModelTreeAnimatedInvert(transform, meshes, anim, animframe, ref modelindex, ref animindex);
		}

		private static List<RenderInfo> DrawModelTreeAnimatedInvert(this NJS_OBJECT obj, MatrixStack transform, Mesh[] meshes, Animation anim, int animframe, ref int modelindex, ref int animindex)
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
				for (int j = 0; j < obj.Attach.MeshInfo.Length; j++)
				{
					Color col = obj.Attach.MeshInfo[j].Material.DiffuseColor;
					col = Color.FromArgb(255 - col.R, 255 - col.G, 255 - col.B);
					NJS_MATERIAL mat = new NJS_MATERIAL
					{
						DiffuseColor = col,
						IgnoreLighting = true,
						UseAlpha = false
					};
					result.Add(new RenderInfo(meshes[modelindex], j, transform.Top, mat, null, FillMode.Wireframe, obj.Attach.CalculateBounds(j, transform.Top)));
				}
			foreach (NJS_OBJECT child in obj.Children)
				result.AddRange(DrawModelTreeAnimatedInvert(child, transform, meshes, anim, animframe, ref modelindex, ref animindex));
			transform.Pop();
			return result;
		}

		public static List<RenderInfo> DrawModelTreeWeighted(this NJS_OBJECT obj, Device device, Matrix transform, Texture[] textures, Mesh[] meshes)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			NJS_OBJECT[] objs = obj.GetObjects();
			for (int i = 0; i < objs.Length; i++)
				if (objs[i].Attach != null & meshes[i] != null)
				{
					for (int j = 0; j < objs[i].Attach.MeshInfo.Length; j++)
					{
						Texture texture = null;
						NJS_MATERIAL mat = objs[i].Attach.MeshInfo[j].Material;
						if (textures != null && mat != null && mat.TextureID < textures.Length)
							texture = textures[mat.TextureID];
						result.Add(new RenderInfo(meshes[i], j, transform, mat, texture, device.GetRenderState<FillMode>(RenderState.FillMode), objs[i].Attach.CalculateBounds(j, transform)));
					}
				}
			return result;
		}

		public static List<RenderInfo> DrawModelTreeWeightedInvert(this NJS_OBJECT obj, Matrix transform, Mesh[] meshes)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			NJS_OBJECT[] objs = obj.GetObjects();
			for (int i = 0; i < objs.Length; i++)
				if (objs[i].Attach != null & meshes[i] != null)
				{
					for (int j = 0; j < objs[i].Attach.MeshInfo.Length; j++)
					{
						Color color = Color.Black;

						// HACK: Null material hack 3: Fixes selecting objects in SADXLVL2, Twinkle Park 1.
						if (objs[i].Attach.MeshInfo[j].Material != null)
							color = objs[i].Attach.MeshInfo[j].Material.DiffuseColor;

						color = Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
						NJS_MATERIAL mat = new NJS_MATERIAL
						{
							DiffuseColor = color,
							IgnoreLighting = true,
							UseAlpha = false
						};
						result.Add(new RenderInfo(meshes[i], j, transform, mat, null, FillMode.Wireframe, objs[i].Attach.CalculateBounds(j, transform)));
					}
				}
			return result;
		}

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
			return CheckHit(obj, Near, Far, Viewport, Projection, View, transform, mesh, ref modelindex);
		}

		private static HitResult CheckHit(this NJS_OBJECT obj, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform, Mesh[] mesh, ref int modelindex)
		{
			transform.Push();
			modelindex++;
			obj.ProcessTransforms(transform);
			HitResult result = HitResult.NoHit;
			if (obj.Attach != null && mesh[modelindex] != null)
				result = HitResult.Min(result, mesh[modelindex].CheckHit(Near, Far, Viewport, Projection, View, transform, obj));
			foreach (NJS_OBJECT child in obj.Children)
				result = HitResult.Min(result, CheckHit(child, Near, Far, Viewport, Projection, View, transform, mesh, ref modelindex));
			transform.Pop();
			return result;
		}

		public static HitResult CheckHitAnimated(this NJS_OBJECT obj, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform, Mesh[] mesh, Animation anim, int animframe)
		{
			int modelindex = -1;
			int animindex = -1;
			return CheckHitAnimated(obj, Near, Far, Viewport, Projection, View, transform, mesh, anim, animframe, ref modelindex, ref animindex);
		}

		private static HitResult CheckHitAnimated(this NJS_OBJECT obj, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform, Mesh[] mesh, Animation anim, int animframe, ref int modelindex, ref int animindex)
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
			if (obj.Attach != null && mesh[modelindex] != null)
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

		public static Attach obj2nj(string objfile, string[] textures = null)
		{
			string[] objFile = File.ReadAllLines(objfile);
			List<UV> uvs = new List<UV>();
			List<Color> vcolors = new List<Color>();
			List<Vertex> verts = new List<Vertex>();
			List<Vertex> norms = new List<Vertex>();
			Dictionary<string, NJS_MATERIAL> materials = new Dictionary<string, NJS_MATERIAL>();

			NJS_MATERIAL lastMaterial = null;
			// Determines whether or not the Texture ID for lastMaterial has been set.
			bool textureIdAssigned = false;
			// Used for materials that don't have a texid field.
			int lastTextureId = 0;

			List<Vertex> model_Vertex = new List<Vertex>();
			List<Vertex> model_Normal = new List<Vertex>();
			List<NJS_MATERIAL> model_Material = new List<NJS_MATERIAL>();
			List<NJS_MESHSET> model_Mesh = new List<NJS_MESHSET>();
			List<ushort> model_Mesh_MaterialID = new List<ushort>();
			List<List<Poly>> model_Mesh_Poly = new List<List<Poly>>();
			List<List<UV>> model_Mesh_UV = new List<List<UV>>();
			List<List<Color>> model_Mesh_VColor = new List<List<Color>>();

			foreach (string objLine in objFile)
			{
				string[] lin = objLine.Split('#')[0].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

				if (lin.Length == 0)
					continue;

				switch (lin[0].ToLowerInvariant())
				{
					case "mtllib":
						string[] mtlFile = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(objfile), lin[1]));
						foreach (string mtlLine in mtlFile)
						{
							string[] mlin = mtlLine.Split('#')[0].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

							if (mlin.Length == 0)
								continue;

							#region Parsing Material Properties
							// Calling trim on this to be compatible with 3ds Max mtl files.
							// It likes to indent them with tabs (\t)
							switch (mlin[0].ToLowerInvariant().Trim())
							{
								case "newmtl":
									// Texture ID failsafe
									if (!textureIdAssigned && lastMaterial != null)
										lastMaterial.TextureID = ++lastTextureId;

									textureIdAssigned = false;
									lastMaterial = new NJS_MATERIAL { UseAlpha = false, UseTexture = false };
									materials.Add(mlin[1], lastMaterial);
									break;

								case "kd":
									lastMaterial.DiffuseColor = Color.FromArgb(
										(int)Math.Round(float.Parse(mlin[1], CultureInfo.InvariantCulture) * 255),
										(int)Math.Round(float.Parse(mlin[2], CultureInfo.InvariantCulture) * 255),
										(int)Math.Round(float.Parse(mlin[3], CultureInfo.InvariantCulture) * 255));
									break;

								case "map_ka":
									lastMaterial.UseAlpha = true;
									if (textures != null && mlin.Length > 1)
									{
										string baseName = Path.GetFileNameWithoutExtension(mlin[1]);
										for (int tid = 0; tid < textures.Length; tid++)
										{
											if (textures[tid] == baseName)
											{
												lastMaterial.TextureID = tid;
												lastTextureId = tid;
												textureIdAssigned = true;
												break;
											}
										}
									}
									break;

								case "map_kd":
									lastMaterial.UseTexture = true;
									if (textures != null && mlin.Length > 1)
									{
										string baseName = Path.GetFileNameWithoutExtension(mlin[1]);
										for (int tid = 0; tid < textures.Length; tid++)
										{
											if (textures[tid] == baseName)
											{
												lastMaterial.TextureID = tid;
												lastTextureId = tid;
												textureIdAssigned = true;
												break;
											}
										}
									}
									break;

								case "ke":
									lastMaterial.Exponent = float.Parse(mlin[1], CultureInfo.InvariantCulture);
									break;

								case "d":
								case "tr":
									lastMaterial.DiffuseColor = Color.FromArgb(
										(int)Math.Round(float.Parse(mlin[1], CultureInfo.InvariantCulture) * 255),
										lastMaterial.DiffuseColor);
									break;

								case "ks":
									lastMaterial.SpecularColor = Color.FromArgb(
										(int)Math.Round(float.Parse(mlin[1], CultureInfo.InvariantCulture) * 255),
										(int)Math.Round(float.Parse(mlin[2], CultureInfo.InvariantCulture) * 255),
										(int)Math.Round(float.Parse(mlin[3], CultureInfo.InvariantCulture) * 255));
									break;

								case "texid":
									if (!textureIdAssigned)
									{
										int textureID = int.Parse(mlin[1], CultureInfo.InvariantCulture);
										lastMaterial.TextureID = textureID;
										lastTextureId = textureID;
										textureIdAssigned = true;
									}
									break;

								case "-u_mirror":
									if (bool.TryParse(mlin[1], out bool uMirror))
										lastMaterial.FlipU = uMirror;

									break;

								case "-v_mirror":
									if (bool.TryParse(mlin[1], out bool vMirror))
										lastMaterial.FlipV = vMirror;

									break;

								case "-u_tile":
									if (bool.TryParse(mlin[1], out bool uTile))
										lastMaterial.ClampU = !uTile;

									break;

								case "-v_tile":
									if (bool.TryParse(mlin[1], out bool vTile))
										lastMaterial.ClampV = !vTile;

									break;

								case "-enviromap":
									lastMaterial.EnvironmentMap = true;
									break;

								case "-doublesided":
									lastMaterial.DoubleSided = true;
									break;

								case "-ignorelighting":
									lastMaterial.IgnoreLighting = bool.Parse(mlin[1]);
									break;

								case "-flatshaded":
									lastMaterial.FlatShading = bool.Parse(mlin[1]);
									break;
							}
							#endregion
						}

						break;

					case "v":
						verts.Add(new Vertex(float.Parse(lin[1], CultureInfo.InvariantCulture),
							float.Parse(lin[2], CultureInfo.InvariantCulture),
							float.Parse(lin[3], CultureInfo.InvariantCulture)));
						break;

					case "vn":
						norms.Add(new Vertex(float.Parse(lin[1], CultureInfo.InvariantCulture),
							float.Parse(lin[2], CultureInfo.InvariantCulture),
							float.Parse(lin[3], CultureInfo.InvariantCulture)));
						break;

					case "vt":
						uvs.Add(new UV
						{
							U = float.Parse(lin[1], CultureInfo.InvariantCulture) * -1,
							V = float.Parse(lin[2], CultureInfo.InvariantCulture) * -1
						});
						break;

					case "vc":
						vcolors.Add(Color.FromArgb((int)Math.Round(float.Parse(lin[1], CultureInfo.InvariantCulture)),
							(int)Math.Round(float.Parse(lin[2], CultureInfo.InvariantCulture)),
							(int)Math.Round(float.Parse(lin[3], CultureInfo.InvariantCulture)),
							(int)Math.Round(float.Parse(lin[4], CultureInfo.InvariantCulture))));
						break;

					case "usemtl":
						model_Mesh_Poly.Add(new List<Poly>());
						model_Mesh_UV.Add(new List<UV>());
						model_Mesh_VColor.Add(new List<Color>());
						if (materials.ContainsKey(lin[1]))
						{
							NJS_MATERIAL mtl = materials[lin[1]];
							if (model_Material.Contains(mtl))
								model_Mesh_MaterialID.Add((ushort)model_Material.IndexOf(mtl));
							else
							{
								model_Mesh_MaterialID.Add((ushort)model_Material.Count);
								model_Material.Add(mtl);
							}
						}
						else
							model_Mesh_MaterialID.Add(0);
						break;

					case "f":
						if (model_Mesh_MaterialID.Count == 0)
						{
							model_Mesh_MaterialID.Add(0);
							model_Mesh_Poly.Add(new List<Poly>());
							model_Mesh_UV.Add(new List<UV>());
							model_Mesh_VColor.Add(new List<Color>());
						}
						ushort[] pol = new ushort[3];
						for (int i = 1; i <= 3; i++)
						{
							string[] lne = lin[i].Split('/');
							Vertex ver = verts.GetItemNeg(int.Parse(lne[0]));
							Vertex nor = norms.GetItemNeg(int.Parse(lne[2]));
							if (uvs.Count > 0)
							{
								if (!string.IsNullOrEmpty(lne[1]))
								{
									model_Mesh_UV[model_Mesh_UV.Count - 1].Add(uvs.GetItemNeg(int.Parse(lne[1])));
								}
							}
							if (vcolors.Count > 0)
							{
								if (lne.Length == 4)
								{
									model_Mesh_VColor[model_Mesh_VColor.Count - 1].Add(vcolors.GetItemNeg(int.Parse(lne[3])));
								}
								else if (!string.IsNullOrEmpty(lne[1]))
								{
									model_Mesh_VColor[model_Mesh_VColor.Count - 1].Add(vcolors.GetItemNeg(int.Parse(lne[1])));
								}
							}
							int verind = model_Vertex.IndexOf(ver);
							while (verind > -1)
							{
								if (Equals(model_Normal[verind], nor))
									break;
								verind = model_Vertex.IndexOf(ver, verind + 1);
							}
							if (verind > -1)
							{
								pol[i - 1] = (ushort)verind;
							}
							else
							{
								model_Vertex.Add(ver);
								model_Normal.Add(nor);
								pol[i - 1] = (ushort)(model_Vertex.Count - 1);
							}
						}
						Poly tri = Poly.CreatePoly(Basic_PolyType.Triangles);
						for (int i = 0; i < 3; i++)
							tri.Indexes[i] = pol[i];
						model_Mesh_Poly[model_Mesh_Poly.Count - 1].Add(tri);
						break;

					case "t":
						if (model_Mesh_MaterialID.Count == 0)
						{
							model_Mesh_MaterialID.Add(0);
							model_Mesh_Poly.Add(new List<Poly>());
							model_Mesh_UV.Add(new List<UV>());
							model_Mesh_VColor.Add(new List<Color>());
						}
						List<ushort> str = new List<ushort>();
						for (int i = 1; i <= lin.Length - 1; i++)
						{
							Vertex ver2 = verts.GetItemNeg(int.Parse(lin[i]));
							Vertex nor2 = norms.GetItemNeg(int.Parse(lin[i]));
							if (uvs.Count > 0)
								model_Mesh_UV[model_Mesh_UV.Count - 1].Add(uvs.GetItemNeg(int.Parse(lin[i])));
							if (vcolors.Count > 0)
								model_Mesh_VColor[model_Mesh_VColor.Count - 1].Add(vcolors.GetItemNeg(int.Parse(lin[i])));
							int verind = model_Vertex.IndexOf(ver2);
							while (verind > -1)
							{
								if (Equals(model_Normal[verind], nor2))
									break;
								verind = model_Vertex.IndexOf(ver2, verind + 1);
							}
							if (verind > -1)
							{
								str.Add((ushort)verind);
							}
							else
							{
								model_Vertex.Add(ver2);
								model_Normal.Add(nor2);
								str.Add((ushort)(model_Vertex.Count - 1));
							}
						}
						model_Mesh_Poly[model_Mesh_Poly.Count - 1].Add(new Strip(str.ToArray(), false));
						break;

					case "q":
						List<ushort> str2 = new List<ushort>(model_Mesh_Poly[model_Mesh_Poly.Count - 1][model_Mesh_Poly[model_Mesh_Poly.Count - 1].Count - 1].Indexes);
						for (int i = 1; i <= lin.Length - 1; i++)
						{
							Vertex ver3 = verts.GetItemNeg(int.Parse(lin[i]));
							Vertex nor3 = norms.GetItemNeg(int.Parse(lin[i]));
							if (uvs.Count > 0)
								model_Mesh_UV[model_Mesh_UV.Count - 1].Add(uvs.GetItemNeg(int.Parse(lin[i])));
							if (vcolors.Count > 0)
								model_Mesh_VColor[model_Mesh_VColor.Count - 1].Add(vcolors.GetItemNeg(int.Parse(lin[i])));
							int verind = model_Vertex.IndexOf(ver3);
							while (verind > -1)
							{
								if (Equals(model_Normal[verind], nor3))
									break;
								verind = model_Vertex.IndexOf(ver3, verind + 1);
							}
							if (verind > -1)
							{
								str2.Add((ushort)verind);
							}
							else
							{
								model_Vertex.Add(ver3);
								model_Normal.Add(nor3);
								str2.Add((ushort)(model_Vertex.Count - 1));
							}
						}
						model_Mesh_Poly[model_Mesh_Poly.Count - 1][model_Mesh_Poly[model_Mesh_Poly.Count - 1].Count - 1] = new Strip(str2.ToArray(), false);
						break;
				}
			}

			// Material failsafe
			if (model_Material.Count == 0)
				model_Material.Add(new NJS_MATERIAL());

			for (int i = 0; i < model_Mesh_MaterialID.Count; i++)
			{
				model_Mesh.Add(new NJS_MESHSET(model_Mesh_Poly[i].ToArray(), false, model_Mesh_UV[i].Count > 0, model_Mesh_VColor[i].Count > 0));
				model_Mesh[i].MaterialID = model_Mesh_MaterialID[i];
				if (model_Mesh[i].UV != null)
				{
					// HACK: Checking if j < model_Mesh_UV[i].Count prevents an out-of-range exception with SADXLVL2 when importing a whole stage export of Emerald Coast 1.
					for (int j = 0; j < model_Mesh[i].UV.Length && j < model_Mesh_UV[i].Count; j++)
						model_Mesh[i].UV[j] = model_Mesh_UV[i][j];
				}
				if (model_Mesh[i].VColor != null)
				{
					for (int j = 0; j < model_Mesh[i].VColor.Length; j++)
						model_Mesh[i].VColor[j] = model_Mesh_VColor[i][j];
				}
			}

			Attach model = new BasicAttach(model_Vertex.ToArray(), model_Normal.ToArray(), model_Mesh, model_Material);
			model.ProcessVertexData();
			model.CalculateBounds();
			return model;
		}

		private static T GetItemNeg<T>(this List<T> list, int index)
		{
			if (index < 0)
				return list[list.Count + index];
			return list[index - 1];
		}

		public static int RadToBAMS(float rad)
		{
			return (int)(rad * (65536 / (2 * Math.PI)));
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

		/// <summary>
		/// Writes an object model (basic format) to the specified stream, in Alias-Wavefront *.OBJ format.
		/// </summary>
		/// <param name="objstream">stream representing a wavefront obj file to export to</param>
		/// <param name="obj">Model to export.</param>
		/// <param name="materialPrefix">idk</param>
		/// <param name="transform">Used for calculating transforms.</param>
		/// <param name="totalVerts">This keeps track of how many verts have been exported to the current file. This is necessary because *.obj vertex indeces are file-level, not object-level.</param>
		/// <param name="totalNorms">This keeps track of how many vert normals have been exported to the current file. This is necessary because *.obj vertex normal indeces are file-level, not object-level.</param>
		/// <param name="totalUVs">This keeps track of how many texture verts have been exported to the current file. This is necessary because *.obj textue vert indeces are file-level, not object-level.</param>
		private static void WriteObjFromBasicAttach(StreamWriter objstream, NJS_OBJECT obj, string materialPrefix, Matrix transform, ref int totalVerts, ref int totalNorms, ref int totalUVs)
		{
			if (obj.Attach != null)
			{
				var basicAttach = (BasicAttach)obj.Attach;
				bool wroteNormals = false;
				objstream.WriteLine("g " + obj.Name);

				#region Outputting Verts and Normals
				foreach (Vertex v in basicAttach.Vertex)
				{
					Vector3 inputVert = new Vector3(v.X, v.Y, v.Z);
					Vector3 outputVert = Vector3.TransformCoordinate(inputVert, transform);
					objstream.WriteLine("v {0} {1} {2}", outputVert.X.ToString(NumberFormatInfo.InvariantInfo), outputVert.Y.ToString(NumberFormatInfo.InvariantInfo), outputVert.Z.ToString(NumberFormatInfo.InvariantInfo));
				}

				if (basicAttach.Vertex.Length == basicAttach.Normal.Length)
				{
					foreach (Vertex v in basicAttach.Normal)
					{
						objstream.WriteLine("vn {0} {1} {2}", v.X.ToString(NumberFormatInfo.InvariantInfo), v.Y.ToString(NumberFormatInfo.InvariantInfo), v.Z.ToString(NumberFormatInfo.InvariantInfo));
					}
					wroteNormals = true;
				}
				#endregion

				#region Outputting Meshes
				foreach (NJS_MESHSET set in basicAttach.Mesh)
				{
					if (basicAttach.Material.Count > 0)
					{
						if (basicAttach.Material[set.MaterialID].UseTexture)
						{
							objstream.WriteLine("usemtl {0}_material_{1}", materialPrefix, basicAttach.Material[set.MaterialID].TextureID);
						}
					}

					if (set.UV != null)
					{
						foreach (UV uv in set.UV)
						{
							objstream.WriteLine("vt {0} {1}", uv.U.ToString(NumberFormatInfo.InvariantInfo), (-uv.V).ToString(NumberFormatInfo.InvariantInfo));
						}
					}

					int processedUVStripCount = 0;
					foreach (Poly poly in set.Poly)
					{
						switch (poly.PolyType)
						{
							case Basic_PolyType.Strips:
								var polyStrip = (Strip)poly;
								int expectedTrisCount = polyStrip.Indexes.Length - 2;
								bool triangleWindReversed = polyStrip.Reversed;

								for (int stripIndx = 0; stripIndx < expectedTrisCount; stripIndx++)
								{
									if (triangleWindReversed)
									{
										Vector3 newFace = new Vector3((polyStrip.Indexes[stripIndx + 1] + 1),
											(polyStrip.Indexes[stripIndx] + 1),
											(polyStrip.Indexes[stripIndx + 2] + 1));

										if (set.UV != null)
										{
											int uv1 = (stripIndx + 1) + processedUVStripCount + 1;
											int uv2 = (stripIndx) + processedUVStripCount + 1;
											int uv3 = (stripIndx + 2) + processedUVStripCount + 1;

											if (wroteNormals)
											{
												objstream.WriteLine("f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}",
													(int)newFace.X + totalVerts, uv1 + totalUVs, (int)newFace.X + totalNorms,
													(int)newFace.Y + totalVerts, uv2 + totalUVs, (int)newFace.Y + totalNorms,
													(int)newFace.Z + totalVerts, uv3 + totalUVs, (int)newFace.Z + totalNorms);
											}
											else
											{
												objstream.WriteLine("f {0}/{1} {2}/{3} {4}/{5}",
													(int)newFace.X + totalVerts, uv1 + totalUVs,
													(int)newFace.Y + totalVerts, uv2 + totalUVs,
													(int)newFace.Z + totalVerts, uv3 + totalUVs);
											}
										}
										else
										{
											if (wroteNormals)
											{
												objstream.WriteLine("f {0}//{1} {2}//{3} {4}//{5}",
													(int)newFace.X + totalVerts, (int)newFace.X + totalNorms,
													(int)newFace.Y + totalVerts, (int)newFace.Y + totalNorms,
													(int)newFace.Z + totalVerts, (int)newFace.Z + totalNorms);
											}
											else
											{
												objstream.WriteLine("f {0} {1} {2}", (int)newFace.X + totalVerts, (int)newFace.Y + totalVerts, (int)newFace.Z + totalVerts);
											}
										}
									}
									else
									{
										Vector3 newFace = new Vector3((polyStrip.Indexes[stripIndx] + 1), (polyStrip.Indexes[stripIndx + 1] + 1), (polyStrip.Indexes[stripIndx + 2] + 1));

										if (set.UV != null)
										{
											int uv1 = (stripIndx) + processedUVStripCount + 1;
											int uv2 = stripIndx + 1 + processedUVStripCount + 1;
											int uv3 = stripIndx + 2 + processedUVStripCount + 1;

											if (wroteNormals)
											{
												objstream.WriteLine("f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}",
													(int)newFace.X + totalVerts, uv1 + totalUVs, (int)newFace.X + totalNorms,
													(int)newFace.Y + totalVerts, uv2 + totalUVs, (int)newFace.Y + totalNorms,
													(int)newFace.Z + totalVerts, uv3 + totalUVs, (int)newFace.Z + totalNorms);
											}
											else
											{
												objstream.WriteLine("f {0}/{1} {2}/{3} {4}/{5}",
													(int)newFace.X + totalVerts, uv1 + totalUVs,
													(int)newFace.Y + totalVerts, uv2 + totalUVs,
													(int)newFace.Z + totalVerts, uv3 + totalUVs);
											}
										}
										else
										{
											if (wroteNormals)
											{
												objstream.WriteLine("f {0}//{1} {2}//{3} {4}//{5}",
													(int)newFace.X + totalVerts, (int)newFace.X + totalNorms,
													(int)newFace.Y + totalVerts, (int)newFace.Y + totalNorms,
													(int)newFace.Z + totalVerts, (int)newFace.Z + totalNorms);
											}
											else
											{
												objstream.WriteLine("f {0} {1} {2}", (int)newFace.X + totalVerts, (int)newFace.Y + totalVerts, (int)newFace.Z + totalVerts);
											}
										}
									}

									triangleWindReversed = !triangleWindReversed; // flip every other triangle or the output will be wrong
								}

								if (set.UV != null)
								{
									processedUVStripCount += polyStrip.Indexes.Length;
									objstream.WriteLine("# processed UV strips this poly: {0}", processedUVStripCount);
								}
								break;

							case Basic_PolyType.Triangles:
								for (int faceVIndx = 0; faceVIndx < poly.Indexes.Length / 3; faceVIndx++)
								{
									Vector3 newFace = new Vector3((poly.Indexes[faceVIndx] + 1),
										(poly.Indexes[faceVIndx + 1] + 1), (poly.Indexes[faceVIndx + 2] + 1));

									if (set.UV != null)
									{
										int uv1 = (faceVIndx) + processedUVStripCount + 1;
										int uv2 = faceVIndx + 1 + processedUVStripCount + 1;
										int uv3 = faceVIndx + 2 + processedUVStripCount + 1;

										if (wroteNormals)
										{
											objstream.WriteLine("f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}",
												(int)newFace.X + totalVerts, uv1 + totalUVs, (int)newFace.X + totalNorms,
												(int)newFace.Y + totalVerts, uv2 + totalUVs, (int)newFace.Y + totalNorms,
												(int)newFace.Z + totalVerts, uv3 + totalUVs, (int)newFace.Z + totalNorms);
										}
										else
										{
											objstream.WriteLine("f {0}/{1} {2}/{3} {4}/{5}",
												(int)newFace.X + totalVerts, uv1 + totalUVs,
												(int)newFace.Y + totalVerts, uv2 + totalUVs,
												(int)newFace.Z + totalVerts, uv3 + totalUVs);
										}
									}
									else
									{
										if (wroteNormals)
										{
											objstream.WriteLine("f {0}//{1} {2}//{3} {4}//{5}",
												(int)newFace.X + totalVerts, (int)newFace.X + totalNorms,
												(int)newFace.Y + totalVerts, (int)newFace.Y + totalNorms,
												(int)newFace.Z + totalVerts, (int)newFace.Z + totalNorms);
										}
										else
										{
											objstream.WriteLine("f {0} {1} {2}", (int)newFace.X + totalVerts, (int)newFace.Y + totalVerts, (int)newFace.Z + totalVerts);
										}
									}

									if (set.UV != null)
									{
										processedUVStripCount += 3;
										objstream.WriteLine("# processed UV strips this poly: {0}", processedUVStripCount);
									}
								}
								break;

							case Basic_PolyType.Quads:
								for (int faceVIndx = 0; faceVIndx < poly.Indexes.Length / 4; faceVIndx++)
								{
									Vector4 newFace = new Vector4((poly.Indexes[faceVIndx + 0] + 1),
										(poly.Indexes[faceVIndx + 1] + 1),
										(poly.Indexes[faceVIndx + 2] + 1),
										(poly.Indexes[faceVIndx + 3] + 1));

									if (set.UV != null)
									{
										int uv1 = faceVIndx + 0 + processedUVStripCount + 1; // +1's are because obj indeces always start at 1, not 0
										int uv2 = faceVIndx + 1 + processedUVStripCount + 1;
										int uv3 = faceVIndx + 2 + processedUVStripCount + 1;
										int uv4 = faceVIndx + 3 + processedUVStripCount + 1;

										if (wroteNormals)
										{
											objstream.WriteLine("f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}",
												(int)newFace.X + totalVerts, uv1 + totalUVs, (int)newFace.X + totalNorms,
												(int)newFace.Y + totalVerts, uv2 + totalUVs, (int)newFace.Y + totalNorms,
												(int)newFace.Z + totalVerts, uv3 + totalUVs, (int)newFace.Z + totalNorms);
											objstream.WriteLine("f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}",
												(int)newFace.Z + totalVerts, uv3 + totalUVs, (int)newFace.Z + totalNorms,
												(int)newFace.Y + totalVerts, uv2 + totalUVs, (int)newFace.Y + totalNorms,
												(int)newFace.W + totalVerts, uv4 + totalUVs, (int)newFace.W + totalNorms);
										}
										else
										{
											objstream.WriteLine("f {0}/{1} {2}/{3} {4}/{5}",
												(int)newFace.X + totalVerts, uv1 + totalUVs,
												(int)newFace.Y + totalVerts, uv2 + totalUVs,
												(int)newFace.Z + totalVerts, uv3 + totalUVs);
											objstream.WriteLine("f {0}/{1} {2}/{3} {4}/{5}",
												(int)newFace.Z + totalVerts, uv3 + totalUVs,
												(int)newFace.Y + totalVerts, uv2 + totalUVs,
												(int)newFace.W + totalVerts, uv4 + totalUVs);
										}
									}
									else
									{
										if (wroteNormals)
										{
											objstream.WriteLine("f {0}//{1} {2}//{3} {4}//{5}",
												(int)newFace.X + totalVerts, (int)newFace.X + totalNorms,
												(int)newFace.Y + totalVerts, (int)newFace.Y + totalNorms,
												(int)newFace.Z + totalVerts, (int)newFace.Z + totalNorms);
											objstream.WriteLine("f {0}//{1} {2}//{3} {4}//{5}",
												(int)newFace.Z + totalVerts, (int)newFace.Z + totalNorms,
												(int)newFace.Y + totalVerts, (int)newFace.Y + totalNorms,
												(int)newFace.W + totalVerts, (int)newFace.W + totalNorms);
										}
										else
										{
											objstream.WriteLine("f {0} {1} {2}", (int)newFace.X + totalVerts, (int)newFace.Y + totalVerts, (int)newFace.Z + totalVerts);
											objstream.WriteLine("f {0} {1} {2}", (int)newFace.Z + totalVerts, (int)newFace.Y + totalVerts, (int)newFace.W + totalVerts);
										}
									}

									if (set.UV != null)
									{
										processedUVStripCount += 4;
										objstream.WriteLine("# processed UV strips this poly: {0}", processedUVStripCount);
									}
								}
								break;

							case Basic_PolyType.NPoly:
								objstream.WriteLine("# Error in WriteObjFromBasicAttach() - NPoly not supported yet!");
								continue;
						}
					}

					if (set.UV != null)
					{
						totalUVs += set.UV.Length;
					}
				}
				#endregion

				objstream.WriteLine("");

				// add totals
				totalVerts += basicAttach.Vertex.Length;
				totalNorms += basicAttach.Normal.Length;
			}
		}

		/// <summary>
		/// Writes an object model (chunk format) to the specified stream, in Alias-Wavefront *.OBJ format.
		/// </summary>
		/// <param name="objstream">stream representing a wavefront obj file to export to</param>
		/// <param name="obj">Model to export.</param>
		/// <param name="materialPrefix">idk</param>
		/// <param name="transform">Used for calculating transforms.</param>
		/// <param name="totalVerts">This keeps track of how many verts have been exported to the current file. This is necessary because *.obj vertex indeces are file-level, not object-level.</param>
		/// <param name="totalNorms">This keeps track of how many vert normals have been exported to the current file. This is necessary because *.obj vertex normal indeces are file-level, not object-level.</param>
		/// <param name="totalUVs">This keeps track of how many texture verts have been exported to the current file. This is necessary because *.obj textue vert indeces are file-level, not object-level.</param>
		/// <param name="errorFlag">Set this to TRUE if you encounter an issue. The user will be alerted.</param>
		private static void WriteObjFromChunkAttach(StreamWriter objstream, NJS_OBJECT obj, string materialPrefix, Matrix transform, ref int totalVerts, ref int totalNorms, ref int totalUVs, ref bool errorFlag)
		{
			// add obj writing here
			if (obj.Attach != null)
			{
				ChunkAttach chunkAttach = (ChunkAttach)obj.Attach;

				if ((chunkAttach.Vertex != null) && (chunkAttach.Poly != null))
				{
					int outputVertCount = 0;
					int outputNormalCount = 0;

					objstream.WriteLine("g " + obj.Name);

					#region Outputting Verts and Normals
					int vertexChunkCount = chunkAttach.Vertex.Count;
					int polyChunkCount = chunkAttach.Poly.Count;

					if (vertexChunkCount != 1)
					{
						errorFlag = true;
						objstream.WriteLine("#A chunk model with more than one vertex chunk was found. Output is probably corrupt.");
					}

					for (int vc = 0; vc < vertexChunkCount; vc++)
					{
						for (int vIndx = 0; vIndx < chunkAttach.Vertex[vc].VertexCount; vIndx++)
						{
							if (chunkAttach.Vertex[vc].Flags == 0)
							{
								Vector3 inputVert = new Vector3(chunkAttach.Vertex[vc].Vertices[vIndx].X, chunkAttach.Vertex[vc].Vertices[vIndx].Y, chunkAttach.Vertex[vc].Vertices[vIndx].Z);
								Vector3 outputVert = Vector3.TransformCoordinate(inputVert, transform);
								objstream.WriteLine("v {0} {1} {2}", outputVert.X.ToString(NumberFormatInfo.InvariantInfo), outputVert.Y.ToString(NumberFormatInfo.InvariantInfo), outputVert.Z.ToString(NumberFormatInfo.InvariantInfo));

								outputVertCount++;
							}
						}

						if (chunkAttach.Vertex[vc].Normals.Count <= 0)
						{
							continue;
						}

						if (chunkAttach.Vertex[vc].Flags != 0)
						{
							continue;
						}

						foreach (Vertex v in chunkAttach.Vertex[vc].Normals)
						{
							objstream.WriteLine("vn {0} {1} {2}",
								v.X.ToString(NumberFormatInfo.InvariantInfo), v.Y.ToString(NumberFormatInfo.InvariantInfo), v.Z.ToString(NumberFormatInfo.InvariantInfo));
							outputNormalCount++;
						}
					}
					#endregion

					#region Outputting Polys
					for (int pc = 0; pc < polyChunkCount; pc++)
					{
						PolyChunk polyChunk = chunkAttach.Poly[pc];

						if (polyChunk is PolyChunkStrip chunkStrip)
						{
							for (int stripNum = 0; stripNum < chunkStrip.StripCount; stripNum++)
							{
								// output texture verts before use, if necessary
								bool uvsAreValid = false;
								if (chunkStrip.Strips[stripNum].UVs != null)
								{
									if (chunkStrip.Strips[stripNum].UVs.Length > 0)
									{
										uvsAreValid = true;
										foreach (UV uv in chunkStrip.Strips[stripNum].UVs)
										{
											objstream.WriteLine("vt {0} {1}", uv.U.ToString(NumberFormatInfo.InvariantInfo), (-uv.V).ToString(NumberFormatInfo.InvariantInfo));
										}
									}
								}

								bool windingReversed = chunkStrip.Strips[stripNum].Reversed;
								for (int currentStripIndx = 0; currentStripIndx < chunkStrip.Strips[stripNum].Indexes.Length - 2; currentStripIndx++)
								{
									if (windingReversed)
									{
										if (uvsAreValid)
										{
											// note to self - uvs.length will equal strip indeces length! They are directly linked, just like you remembered.
											objstream.WriteLine("f {0}/{1} {2}/{3} {4}/{5}",
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 1] + totalVerts) + 1, (currentStripIndx + 1 + totalUVs) + 1,
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx] + totalVerts) + 1, (currentStripIndx + totalUVs) + 1,
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 2] + totalVerts) + 1, (currentStripIndx + 2 + totalUVs) + 1);
										}
										else
										{
											objstream.WriteLine("f {0} {1} {2}",
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 1] + totalVerts) + 1,
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx] + totalVerts) + 1,
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 2] + totalVerts) + 1);
										}
									}
									else
									{
										if (uvsAreValid)
										{
											// note to self - uvs.length will equal strip indeces length! They are directly linked, just like you remembered.
											objstream.WriteLine("f {0}/{1} {2}/{3} {4}/{5}",
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx] + totalVerts) + 1, currentStripIndx + totalUVs + 1,
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 1] + totalVerts) + 1, currentStripIndx + 1 + totalUVs + 1,
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 2] + totalVerts) + 1, currentStripIndx + 2 + totalUVs + 1);
										}
										else
										{
											objstream.WriteLine("f {0} {1} {2}",
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx] + totalVerts) + 1,
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 1] + totalVerts) + 1,
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 2] + totalVerts) + 1);
										}
									}

									windingReversed = !windingReversed;
								}

								// increment output verts
								if (uvsAreValid) totalUVs += chunkStrip.Strips[stripNum].UVs.Length;
							}
						}
						else if (polyChunk is PolyChunkMaterial)
						{
							// no behavior defined yet.
						}
						else if (polyChunk is PolyChunkTinyTextureID chunkTexID)
						{
							objstream.WriteLine("usemtl {0}_material_{1}", materialPrefix, chunkTexID.TextureID);
						}
					}
					#endregion

					totalVerts += outputVertCount;
					totalNorms += outputNormalCount;
				}
				else
				{
					errorFlag = true;
					objstream.WriteLine("#A chunk model with no vertex or no poly was found. Output is definitely corrupt.");
				}
			}
		}

		/// <summary>
		/// Primary method for exporting models to Wavefront *.OBJ format. This will auto-detect the model type and send it to the proper export method.
		/// </summary>
		/// <param name="objstream">stream representing a wavefront obj file to export to</param>
		/// <param name="obj">Model to export.</param>
		/// <param name="materialPrefix">used to prevent name collisions if mixing/matching outputs.</param>
		/// <param name="transform">Used for calculating transforms.</param>
		/// <param name="totalVerts">This keeps track of how many verts have been exported to the current file. This is necessary because *.obj vertex indeces are file-level, not object-level.</param>
		/// <param name="totalNorms">This keeps track of how many vert normals have been exported to the current file. This is necessary because *.obj vertex normal indeces are file-level, not object-level.</param>
		/// <param name="totalUVs">This keeps track of how many texture verts have been exported to the current file. This is necessary because *.obj textue vert indeces are file-level, not object-level.</param>
		/// <param name="errorFlag">Set this to TRUE if you encounter an issue. The user will be alerted.</param>
		public static void WriteModelAsObj(StreamWriter objstream, NJS_OBJECT obj, string materialPrefix, MatrixStack transform, ref int totalVerts, ref int totalNorms, ref int totalUVs, ref bool errorFlag)
		{
			transform.Push();
			obj.ProcessTransforms(transform);
			if (obj.Attach is BasicAttach)
				WriteObjFromBasicAttach(objstream, obj, materialPrefix, transform.Top, ref totalVerts, ref totalNorms, ref totalUVs);
			else if (obj.Attach is ChunkAttach)
				WriteObjFromChunkAttach(objstream, obj, materialPrefix, transform.Top, ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);
			foreach (NJS_OBJECT child in obj.Children)
				WriteModelAsObj(objstream, child, materialPrefix, transform, ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);
			transform.Pop();
		}

		/// <summary>
		/// Primary method for exporting models to Wavefront *.OBJ format. This will auto-detect the model type and send it to the proper export method.
		/// </summary>
		/// <param name="objstream">stream representing a wavefront obj file to export to</param>
		/// <param name="obj">Model to export.</param>
		/// <param name="materialPrefix">used to prevent name collisions if mixing/matching outputs.</param>
		/// <param name="errorFlag">Set this to TRUE if you encounter an issue. The user will be alerted.</param>
		public static void WriteSingleModelAsObj(StreamWriter objstream, NJS_OBJECT obj, string materialPrefix, ref bool errorFlag)
		{
			int v = 0, n = 0, u = 0;
			if (obj.Attach is BasicAttach)
				WriteObjFromBasicAttach(objstream, obj, materialPrefix, Matrix.Identity, ref v, ref n, ref u);
			else if (obj.Attach is ChunkAttach)
				WriteObjFromChunkAttach(objstream, obj, materialPrefix, Matrix.Identity, ref v, ref n, ref u, ref errorFlag);
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
		public static float BAMSSin(int angle)
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
		public static float BAMSSinInv(int angle)
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
}