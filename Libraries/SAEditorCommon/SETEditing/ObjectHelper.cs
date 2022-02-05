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

		public static Texture[] GetTextures(string name, SplitTools.TexnameArray texnames = null, Device dev = null)
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
	}
}