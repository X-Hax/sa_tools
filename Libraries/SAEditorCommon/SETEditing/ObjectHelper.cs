using SharpDX;
using SharpDX.Direct3D9;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using Color = System.Drawing.Color;
using Mesh = SAModel.Direct3D.Mesh;
using SAModel.Direct3D.TextureSystem;

namespace SAModel.SAEditorCommon.SETEditing
{
	public static class ObjectHelper
	{
		private static NJS_OBJECT QuestionBoxModel;
		private static Mesh QuestionBoxMesh;

		public static void Init(Device device)
		{
			QuestionBoxModel = new ModelFile(Resources.questionmark).Model;
			QuestionBoxMesh = GetMeshes(QuestionBoxModel).First();
			QuestionMark = Resources.questionmark_t.ToTexture(device);
		}

		internal static Texture QuestionMark;

		public static NJS_OBJECT LoadModel(string file)
		{
			if (!System.IO.File.Exists(file))
				return QuestionBoxModel;
			return new ModelFile(file).Model;
		}

		public static Mesh[] GetMeshes(NJS_OBJECT model)
		{
			model.ProcessVertexData();
			NJS_OBJECT[] models = model.GetObjects();
			Mesh[] Meshes = new Mesh[models.Length];
			for (int i = 0; i < models.Length; i++)
				if (models[i].Attach != null)
					Meshes[i] = models[i].Attach.CreateD3DMesh();
			return Meshes;
		}

		public static Texture[] GetTextures(string name, SplitTools.NJS_TEXLIST texnames = null, Device dev = null)
		{
			Texture[] result = null;
			if (LevelData.Textures == null || EditorOptions.DisableTextures)
				return result;
			if (LevelData.Textures.ContainsKey(name))
				result = LevelData.Textures[name];
			else if (LevelData.Textures.ContainsKey(name.ToUpperInvariant()))
				result = LevelData.Textures[name.ToUpperInvariant()];
			else if (LevelData.Textures.ContainsKey(name.ToLowerInvariant()))
				result = LevelData.Textures[name.ToLowerInvariant()];
			if (texnames == null)
				return result;
			// Partial texlist
			else
			{
				if (LevelData.TextureBitmaps == null || dev == null)
					return result;
				Direct3D.TextureSystem.BMPInfo[] texturebmps = null;
				if (LevelData.TextureBitmaps.ContainsKey(name))
					texturebmps = LevelData.TextureBitmaps[name];
				else if (LevelData.TextureBitmaps.ContainsKey(name.ToUpperInvariant()))
					texturebmps = LevelData.TextureBitmaps[name.ToUpperInvariant()];
				else if (LevelData.TextureBitmaps.ContainsKey(name.ToLowerInvariant()))
					texturebmps = LevelData.TextureBitmaps[name.ToLowerInvariant()];
				List<Texture> texlist = new List<Texture>();
				if (texturebmps == null)
					return result;
				for (int i = 0; i < texnames.TextureNames.Length; i++)
				{
					for (int b = 0; b < texturebmps.Length; b++)
					{
						if (texturebmps[b].Name.ToLowerInvariant() == texnames.TextureNames[i].ToLowerInvariant())
						{
							texlist.Add(texturebmps[b].Image.ToTexture(dev));
							break;
						}
					}
				}
				return texlist.ToArray();
			}
		}
		// This is primarily used for SA2 level objects that pull from multiple texture sources.
		// Copied from pieces of SAMDL's code.
		public static Texture[] GetTexturesMultiSource(List<string> name, SplitTools.NJS_TEXLIST texnames = null, Device dev = null)
		{
			Texture[] result = null;
			if (texnames == null)
				return result;
			else
			{
				List<BMPInfo> texturedata = new List<BMPInfo>();
				Direct3D.TextureSystem.BMPInfo[] texturebmps = null;
				if (texturebmps != null && texturebmps.Length > 0)
					texturedata.AddRange(texturebmps);
				for (int i = 0; i < name.Count; i++)
				{
					if (LevelData.TextureBitmaps.ContainsKey(name[i]))
						texturedata.AddRange(LevelData.TextureBitmaps[name[i]]);
					else if (LevelData.TextureBitmaps.ContainsKey(name[i].ToUpperInvariant()))
						texturedata.AddRange(LevelData.TextureBitmaps[name[i].ToUpperInvariant()]);
					else if (LevelData.TextureBitmaps.ContainsKey(name[i].ToLowerInvariant()))
						texturedata.AddRange(LevelData.TextureBitmaps[name[i].ToLowerInvariant()]);
				}
				texturebmps = texturedata.ToArray();
				if (LevelData.TextureBitmaps == null || dev == null)
					return result;
				List<Texture> texlist = new List<Texture>();
				List<BMPInfo> texinfo = new List<BMPInfo>();
				List<string> dupnames = new List<string>();
				for (int i = 0; i < texnames.TextureNames.Length; i++)
				{
					for (int b = 0; b < texturebmps.Length; b++)
					{
						if (string.IsNullOrEmpty(texnames.TextureNames[i]) || (texnames.TextureNames[i].ToLowerInvariant() == texturebmps[b].Name.ToLowerInvariant() && !dupnames.Contains(texnames.TextureNames[i].ToLowerInvariant())))
						{
							texinfo.Add(texturebmps[b]);
							texlist.Add(texturebmps[b].Image.ToTexture(dev));
							dupnames.Add(texturebmps[b].Name.ToLowerInvariant());
							continue;
						}
					}
				}
				return texlist.ToArray();
			}
		}

		public static HitResult CheckQuestionBoxHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			return QuestionBoxMesh.CheckHit(Near, Far, Viewport, Projection, View, transform);
		}

		public static RenderInfo[] RenderQuestionBox(Device dev, MatrixStack transform, Texture texture, Vector3 center, bool selected)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			NJS_MATERIAL mat = new NJS_MATERIAL
			{
				DiffuseColor = Color.White
			};
			if (texture == null && !EditorOptions.DisableTextures)
				texture = QuestionMark;
			result.Add(new RenderInfo(QuestionBoxMesh, 0, transform.Top, mat, texture, dev.GetRenderState<FillMode>(RenderState.FillMode), new BoundingSphere(center.X, center.Y, center.Z, 8)));
			if (selected)
			{
				mat = new NJS_MATERIAL
				{
					DiffuseColor = Color.Yellow,
					UseAlpha = false
				};
				result.Add(new RenderInfo(QuestionBoxMesh, 0, transform.Top, mat, null, FillMode.Wireframe, new BoundingSphere(center.X, center.Y, center.Z, 8)));
			}
			return result.ToArray();
		}

		public static BoundingSphere GetQuestionBoxBounds(MatrixStack transform)
		{
			return GetQuestionBoxBounds(transform, 1);
		}

		public static BoundingSphere GetQuestionBoxBounds(MatrixStack transform, float scale)
		{
			return GetModelBounds(QuestionBoxModel, transform, scale, new BoundingSphere());
		}

		public static float BAMSToRad(int BAMS)
		{
			return Direct3D.Extensions.BAMSToRad(BAMS);
		}

		public static int RadToBAMS(float rad)
		{
			return (int)(rad * (65536 / (2 * Math.PI)));
		}

		public static float BAMSToDeg(int BAMS)
		{
			return (float)(BAMS / (65536 / 360.0));
		}

		public static int DegToBAMS(float deg)
		{
			return (int)(deg * (65536 / 360.0));
		}

		public static float NJSin(int BAMS)
		{
			return Direct3D.Extensions.NJSin(BAMS);
		}

		public static float NJCos(int BAMS)
		{
			return Direct3D.Extensions.NJCos(BAMS);
		}

		public static BoundingSphere GetModelBounds(NJS_OBJECT model, MatrixStack transform)
		{
			return GetModelBounds(model, transform, 1);
		}

		public static BoundingSphere GetModelBounds(NJS_OBJECT model, MatrixStack transform, float scale)
		{
			return GetModelBounds(model, transform, scale, new BoundingSphere());
		}

		public static BoundingSphere GetModelBounds(NJS_OBJECT model, MatrixStack transform, float scale, BoundingSphere bounds)
		{
			transform.Push();
			model.ProcessTransforms(transform);
			scale *= Math.Max(Math.Max(model.Scale.X, model.Scale.Y), model.Scale.Z);
			if (model.Attach != null)
				bounds = Direct3D.Extensions.Merge(bounds, new BoundingSphere(Vector3.TransformCoordinate(model.Attach.Bounds.Center.ToVector3(), transform.Top).ToVertex(), model.Attach.Bounds.Radius * scale));
			foreach (NJS_OBJECT child in model.Children)
				bounds = GetModelBounds(child, transform, scale, bounds);
			transform.Pop();
			return bounds;
		}

		public static void RotateObject(MatrixStack transform, SETItem item, Rotation addrot, string type = "XYZ")
		{
			if (addrot.X != 0 && addrot.Y != 0 && addrot.Z != 0)
				transform.NJRotateObject(addrot);

			switch (type)
			{
				case "X":
					transform.NJRotateX(item.Rotation.X);
					break;
				case "Y":
					transform.NJRotateY(item.Rotation.Y);
					break;
				case "Z":
					transform.NJRotateZ(item.Rotation.Z);
					break;
				case "XY":
					transform.NJRotateX(item.Rotation.X);
					transform.NJRotateY(item.Rotation.Y);
					break;
				case "XZ":
					transform.NJRotateX(item.Rotation.X);
					transform.NJRotateZ(item.Rotation.Z);
					break;
				case "YX":
					transform.NJRotateY(item.Rotation.Y);
					transform.NJRotateX(item.Rotation.X);
					break;
				case "YZ":
					transform.NJRotateY(item.Rotation.Y);
					transform.NJRotateZ(item.Rotation.Z);
					break;
				case "ZX":
					transform.NJRotateZ(item.Rotation.Z);
					transform.NJRotateX(item.Rotation.X);
					break;
				case "ZY":
					transform.NJRotateZ(item.Rotation.Z);
					transform.NJRotateY(item.Rotation.Y);
					break;
				case "XZY":
					transform.NJRotateX(item.Rotation.X);
					transform.NJRotateZ(item.Rotation.Z);
					transform.NJRotateY(item.Rotation.Y);
					break;
				case "YXZ":
					transform.NJRotateY(item.Rotation.Y);
					transform.NJRotateX(item.Rotation.X);
					transform.NJRotateZ(item.Rotation.Z);
					break;
				case "YZX":
					transform.NJRotateY(item.Rotation.Y);
					transform.NJRotateZ(item.Rotation.Z);
					transform.NJRotateX(item.Rotation.X);
					break;
				case "ZXY":
					transform.NJRotateZ(item.Rotation.Z);
					transform.NJRotateX(item.Rotation.X);
					transform.NJRotateY(item.Rotation.Y);
					break;
				case "ZYX":
					transform.NJRotateZYX(item.Rotation.X, item.Rotation.Y, item.Rotation.Z);
					break;
				case "None":
					break;
				case "XYZ":
				default:
					transform.NJRotateXYZ(item.Rotation.X, item.Rotation.Y, item.Rotation.Z);
					break;
			}
		}

		public static Vector3 GetScale(SETItem item, Vector3 addscl, string type = "None")
		{
			float x = 1;
			float y = 1;
			float z = 1;

			switch (type)
			{
				case "X":
					x = item.Scale.X;
					break;
				case "Y":
					y = item.Scale.Y;
					break;
				case "Z":
					z = item.Scale.Z;
					break;
				case "XY":
					x = item.Scale.X;
					y = item.Scale.Y;
					break;
				case "XZ":
					x = item.Scale.X;
					z = item.Scale.Z;
					break;
				case "YZ":
					y = item.Scale.Y;
					z = item.Scale.Z;
					break;
				case "XYZ":
					x = item.Scale.X;
					y = item.Scale.Y;
					z = item.Scale.Z;
					break;
				case "AllX":
					x = item.Scale.X;
					y = item.Scale.X;
					z = item.Scale.X;
					break;
				case "AllY":
					x = item.Scale.Y;
					y = item.Scale.Y;
					z = item.Scale.Y;
					break;
				case "AllZ":
					x = item.Scale.Z;
					y = item.Scale.Z;
					z = item.Scale.Z;
					break;
				case "None":
				default:
					break;
			}

			if (addscl.X != 0)
				x += addscl.X;
			if (addscl.Y != 0)
				y += addscl.Y;
			if (addscl.Z != 0)
				z += addscl.Z;

			return new Vector3(x, y, z);
		}
	}
}