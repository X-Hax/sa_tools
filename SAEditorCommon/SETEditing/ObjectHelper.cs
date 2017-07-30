using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;

namespace SonicRetro.SAModel.SAEditorCommon.SETEditing
{
	public static class ObjectHelper
	{
		internal static readonly CustomVertex.PositionTextured[] SquareVerts = {
		new CustomVertex.PositionTextured(-8, 8, 0, 1, 0),
		new CustomVertex.PositionTextured(-8, -8, 0, 1, 1),
		new CustomVertex.PositionTextured(8, 8, 0, 0, 0),
		new CustomVertex.PositionTextured(-8, -8, 0, 1, 1),
		new CustomVertex.PositionTextured(8, -8, 0, 0, 1),
		new CustomVertex.PositionTextured(8, 8, 0, 0, 0)};
		internal static Mesh SquareMesh;
		internal static BoundingSphere SquareBounds;

		public static void Init(Device device, Bitmap unknownBitmap)
		{
			SquareMesh = new Mesh(2, 6, MeshFlags.Managed, CustomVertex.PositionTextured.Format, device);
			List<short> ib = new List<short>();
			for (int i = 0; i < SquareVerts.Length; i++)
				ib.Add((short)(i));
			SquareMesh.SetVertexBufferData(SquareVerts, LockFlags.None);
			SquareMesh.SetIndexBufferData(ib.ToArray(), LockFlags.None);
			Vector3 center;
			float radius = Geometry.ComputeBoundingSphere(SquareVerts, CustomVertex.PositionTextured.Format, out center);
			SquareBounds = new BoundingSphere(center.ToVertex(), radius);

			QuestionMark = unknownBitmap != null ? new Texture(device, unknownBitmap, Usage.None, Pool.Managed) : new Texture(device, 16, 16, 0, Usage.None, Format.A16B16G16R16, Pool.Managed);
		}

		internal static Texture QuestionMark;

		public static NJS_OBJECT LoadModel(string file)
		{
			return new ModelFile(file).Model;
		}

		public static Mesh[] GetMeshes(NJS_OBJECT model, Device dev)
		{
			model.ProcessVertexData();
			NJS_OBJECT[] models = model.GetObjects();
			Mesh[] Meshes = new Mesh[models.Length];
			for (int i = 0; i < models.Length; i++)
				if (models[i].Attach != null)
					Meshes[i] = models[i].Attach.CreateD3DMesh(dev);
			return Meshes;
		}

		public static Texture[] GetTextures(string name)
		{
			if (LevelData.Textures.ContainsKey(name))
				return LevelData.Textures[name];
			return null;
		}

		public static HitResult CheckSpriteHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			return SquareMesh.CheckHit(Near, Far, Viewport, Projection, View, transform);
		}

		public static RenderInfo[] RenderSprite(Device dev, MatrixStack transform, Texture texture, Vector3 center, bool selected)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			NJS_MATERIAL mat = new NJS_MATERIAL
			{
				DiffuseColor = Color.White
			};
			if (texture == null)
				texture = QuestionMark;
			result.Add(new RenderInfo(SquareMesh, 0, transform.Top, mat, texture, dev.RenderState.FillMode, new BoundingSphere(center.X, center.Y, center.Z, 8)));
			if (selected)
			{
				mat = new NJS_MATERIAL
				{
					DiffuseColor = Color.Yellow,
					UseAlpha = false
				};
				result.Add(new RenderInfo(SquareMesh, 0, transform.Top, mat, null, FillMode.WireFrame, new BoundingSphere(center.X, center.Y, center.Z, 8)));
			}
			return result.ToArray();
		}

		public static BoundingSphere GetSpriteBounds(MatrixStack transform)
		{
			return GetSpriteBounds(transform, 1);
		}

		public static BoundingSphere GetSpriteBounds(MatrixStack transform, float scale)
		{
			return new BoundingSphere(Vector3.TransformCoordinate(SquareBounds.Center.ToVector3(), transform.Top).ToVertex(), SquareBounds.Radius * scale);
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

		public static float ConvertBAMS(int BAMS)
		{
			return Direct3D.Extensions.BAMSSin(BAMS);
		}

		public static float ConvertBAMSInv(int BAMS)
		{
			return Direct3D.Extensions.BAMSSinInv(BAMS);
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
