using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;
using SplitTools;
using SAModel.SAEditorCommon;

namespace SA2ObjectDefinitions.Common
{
	public abstract class RingGroup : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] meshes;
		protected NJS_TEXLIST texarr;
		protected Texture[] texs;
		
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/OBJECT_RING.sa2mdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("object/tls/RING.satex");
		}
		
		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}
		
		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;
			
			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation);

			return matrix;
		}
		
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Number of Rings", typeof(byte), "Extended", null, 1, (o) => (byte)Math.Min(o.Scale.Z + 0, 8), (o, v) => o.Scale.X = Math.Max(Math.Min((byte)v - 0, 8), 0)),
			new PropertySpec("Size", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DistanceFromGround
		{
			get
			{
				return 7;
			}
		}
	}
	
	// TODO: Fix interaction between X-rotation and Y-rotation.
	//  This problem isn't specific to RingLine, but it's much more noticeable with RingLine.
	public class RingLine : RingGroup
	{
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			int sine = 0;
			for (int i = 0; i < Math.Min(item.Scale.Z + 0, 8); i++)
			{
				transform.Push();
				transform.Push();
				transform.NJTranslate(item.Position);
				if (item.Rotation.Z != 0)
					transform.NJRotateZ(item.Rotation.Z);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				if (item.Rotation.X != 0)
					transform.NJRotateX(item.Rotation.X);
				double v5 = i * ((item.Scale.X + 10f) * -1);
				double v6 = ObjectHelper.NJSin(sine) * item.Scale.Y;
				sine += 0x4000 / (int)item.Scale.Z;
				Vector3 pos = Vector3.TransformCoordinate(new Vector3(0, (float)v6, (float)v5), transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
				transform.Pop();
			}
			return result;
		}
		
		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			if (texs == null)
				texs = ObjectHelper.GetTextures("objtex_common", texarr, dev);
			List<RenderInfo> result = new List<RenderInfo>();
			int sine = 0;
			for (int i = 0; i < Math.Min(item.Scale.Z + 0, 8); i++)
			{
				transform.Push();
				transform.Push();
				transform.NJTranslate(item.Position);
				if (item.Rotation.Z != 0)
					transform.NJRotateZ(item.Rotation.Z);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				if (item.Rotation.X != 0)
					transform.NJRotateX(item.Rotation.X);
				double v5 = i * ((item.Scale.X + 10f) * -1);
				double v6 = ObjectHelper.NJSin(sine) * item.Scale.Y;
				sine += 0x4000 / (int)item.Scale.Z;
				Vector3 pos = Vector3.TransformCoordinate(new Vector3(0, (float)v6, (float)v5), transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model.DrawModelTreeInvert(transform, meshes));
				transform.Pop();
			}
			return result;
		}
		
		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			int sine = 0;
			for (int i = 0; i < Math.Min(item.Scale.Z + 0, 8); i++)
			{
				transform.Push();
				transform.Push();
				transform.NJTranslate(item.Position);
				if (item.Rotation.Z != 0)
					transform.NJRotateZ(item.Rotation.Z);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				if (item.Rotation.X != 0)
					transform.NJRotateX(item.Rotation.X);
				double v5 = i * ((item.Scale.X + 10f) * -1);
				double v6 = ObjectHelper.NJSin(sine) * item.Scale.Y;
				sine += 0x4000 / (int)item.Scale.Z;
				Vector3 pos = Vector3.TransformCoordinate(new Vector3(0, (float)v6, (float)v5), transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result.Add(new ModelTransform(model, transform.Top));
				transform.Pop();
			}
			return result;
		}
		
		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			BoundingSphere result = new BoundingSphere();
			int sine = 0;
			for (int i = 0; i < Math.Min(item.Scale.Z + 0, 8); i++)
			{
				transform.Push();
				transform.Push();
				transform.NJTranslate(item.Position);
				if (item.Rotation.Z != 0)
					transform.NJRotateZ(item.Rotation.Z);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				if (item.Rotation.X != 0)
					transform.NJRotateX(item.Rotation.X);
				double v5 = i * ((item.Scale.X + 10f) * -1);
				double v6 = ObjectHelper.NJSin(sine) * item.Scale.Y;
				sine += 0x4000 / (int)item.Scale.Z;
				Vector3 pos = Vector3.TransformCoordinate(new Vector3(0, (float)v6, (float)v5), transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result = SAModel.Direct3D.Extensions.Merge(result, ObjectHelper.GetModelBounds(model, transform));
				transform.Pop();
			}
			return result;
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Number of Rings", typeof(byte), "Extended", null, 1, (o) => (byte)Math.Min(o.Scale.Z + 0, 8), (o, v) => o.Scale.Z = Math.Max(Math.Min((byte)v - 0, 8), 0)),
			new PropertySpec("Size", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Sine Wave Strength", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override string Name { get { return "Line of Rings"; } }
	}
	
	public class RingCircle : RingGroup
	{
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			for (int i = 0; i < Math.Min(item.Scale.Z + 0, 8); i++)
			{
				transform.Push();
				double v4 = i * 360.0;
				Vector3 v7 = new Vector3(
					0,
					ObjectHelper.NJSin((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f),
					ObjectHelper.NJCos((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f));
                transform.Push();
				transform.NJTranslate(item.Position);
				if (item.Rotation.Z != 0)
					transform.NJRotateZ(item.Rotation.Z);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				if (item.Rotation.X != 0)
					transform.NJRotateX(item.Rotation.X);
				Vector3 pos = Vector3.TransformCoordinate(v7, transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
				transform.Pop();
			}
			return result;
		}
		
		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			if (texs == null)
				texs = ObjectHelper.GetTextures("objtex_common", texarr, dev);
			List<RenderInfo> result = new List<RenderInfo>();
			for (int i = 0; i < Math.Min(item.Scale.Z + 0, 8); i++)
			{
				transform.Push();
				double v4 = i * 360.0;
				Vector3 v7 = new Vector3(
					0,
					ObjectHelper.NJSin((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f),
					ObjectHelper.NJCos((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f));
                transform.Push();
				transform.NJTranslate(item.Position);
				if (item.Rotation.Z != 0)
					transform.NJRotateZ(item.Rotation.Z);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				if (item.Rotation.X != 0)
					transform.NJRotateX(item.Rotation.X);
				Vector3 pos = Vector3.TransformCoordinate(v7, transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model.DrawModelTreeInvert(transform, meshes));
				transform.Pop();
			}
			return result;
		}
		
		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			for (int i = 0; i < Math.Min(item.Scale.Z + 0, 8); i++)
			{
				transform.Push();
				double v4 = i * 360.0;
				Vector3 v7 = new Vector3(
					0,
					ObjectHelper.NJSin((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f),
					ObjectHelper.NJCos((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f));
                transform.Push();
				transform.NJTranslate(item.Position);
				if (item.Rotation.Z != 0)
					transform.NJRotateZ(item.Rotation.Z);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				if (item.Rotation.X != 0)
					transform.NJRotateX(item.Rotation.X);
				Vector3 pos = Vector3.TransformCoordinate(v7, transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result.Add(new ModelTransform(model, transform.Top));
				transform.Pop();
			}
			return result;
		}
		
		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			BoundingSphere result = new BoundingSphere();
			for (int i = 0; i < Math.Min(item.Scale.Z + 0, 8); i++)
			{
				transform.Push();
				double v4 = i * 360.0;
				Vector3 v7 = new Vector3(
					0,
					ObjectHelper.NJSin((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f),
					ObjectHelper.NJCos((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f));
                transform.Push();
				transform.NJTranslate(item.Position);
				if (item.Rotation.Z != 0)
					transform.NJRotateZ(item.Rotation.Z);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				if (item.Rotation.X != 0)
					transform.NJRotateX(item.Rotation.X);
				Vector3 pos = Vector3.TransformCoordinate(v7, transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result = SAModel.Direct3D.Extensions.Merge(result, ObjectHelper.GetModelBounds(model, transform));
				transform.Pop();
			}
			return result;
		}
		
		public override string Name { get { return "Circle of Rings"; } }
	}
		
	public class MstRngL : RingLine
	{
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			float absolutemin;
			int sine = 0;
			if (item.Scale.Z > 0)
				absolutemin = Math.Min(item.Scale.Z + 0, 8);
			else
				absolutemin = item.Scale.Z + 1f;
			for (int i = 0; i < absolutemin; i++)
			{
				transform.Push();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation.X, item.Rotation.Y, 0);
				double v5 = i * ((item.Scale.X + 10f) * -1);
				double v6 = ObjectHelper.NJSin(sine) * item.Scale.Y;
				sine += 0x4000 / (int)item.Scale.Z;
				Vector3 pos = Vector3.TransformCoordinate(new Vector3(0, (float)v6, (float)v5), transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
				transform.Pop();
			}
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texs == null)
				texs = ObjectHelper.GetTextures("objtex_common", texarr, dev);
			float absolutemin;
			int sine = 0;
			if (item.Scale.Z > 0)
				absolutemin = Math.Min(item.Scale.Z + 0, 8);
			else
				absolutemin = item.Scale.Z + 1f;
			for (int i = 0; i < absolutemin; i++)
			{
				transform.Push();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation.X, item.Rotation.Y, 0);
				double v5 = i * ((item.Scale.X + 10f) * -1);
				double v6 = ObjectHelper.NJSin(sine) * item.Scale.Y;
				sine += 0x4000 / (int)item.Scale.Z;
				Vector3 pos = Vector3.TransformCoordinate(new Vector3(0, (float)v6, (float)v5), transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model.DrawModelTreeInvert(transform, meshes));
				transform.Pop();
			}
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			float absolutemin;
			int sine = 0;
			if (item.Scale.Z > 0)
				absolutemin = Math.Min(item.Scale.Z + 0, 8);
			else
				absolutemin = item.Scale.Z + 1f;
			for (int i = 0; i < absolutemin; i++)
			{
				transform.Push();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation.X, item.Rotation.Y, 0);
				double v5 = i * ((item.Scale.X + 10f) * -1);
				double v6 = ObjectHelper.NJSin(sine) * item.Scale.Y;
				sine += 0x4000 / (int)item.Scale.Z;
				Vector3 pos = Vector3.TransformCoordinate(new Vector3(0, (float)v6, (float)v5), transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result.Add(new ModelTransform(model, transform.Top));
				transform.Pop();
			}
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			BoundingSphere result = new BoundingSphere();
			float absolutemin;
			int sine = 0;
			if (item.Scale.Z > 0)
				absolutemin = Math.Min(item.Scale.Z + 0, 8);
			else
				absolutemin = item.Scale.Z + 1f;
			for (int i = 0; i < absolutemin; i++)
			{
				transform.Push();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation.X, item.Rotation.Y, 0);
				double v5 = i * ((item.Scale.X + 10f) * -1);
				double v6 = ObjectHelper.NJSin(sine) * item.Scale.Y;
				sine += 0x4000 / (int)item.Scale.Z;
				Vector3 pos = Vector3.TransformCoordinate(new Vector3(0, (float)v6, (float)v5), transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result = SAModel.Direct3D.Extensions.Merge(result, ObjectHelper.GetModelBounds(model, transform));
				transform.Pop();
			}
			return result;
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Number of Rings", typeof(byte), "Extended", null, 1, (o) => (byte)Math.Min(o.Scale.Z + 0, 8), (o, v) => o.Scale.Z = Math.Max(Math.Min((byte)v - 0, 8), 0)),
			new PropertySpec("Size", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Shrine ID", typeof(int), "Extended", null, null, (o) => o.Rotation.Z, (o, v) => o.Rotation.Z = (int)v),
			new PropertySpec("Sine Wave Strength", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Line of Rings (Mystic Melody)"; } }
	}
	
	public class MstRngC : RingCircle
	{
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			for (int i = 0; i < Math.Min(item.Scale.Z + 0, 8); i++)
			{
				transform.Push();
				double v4 = i * 360.0;
				Vector3 v7 = new Vector3(
					0,
					ObjectHelper.NJSin((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f),
					ObjectHelper.NJCos((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f));
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation.X, item.Rotation.Y, 0);
				Vector3 pos = Vector3.TransformCoordinate(v7, transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
				transform.Pop();
			}
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texs == null)
				texs = ObjectHelper.GetTextures("objtex_common", texarr, dev);
			for (int i = 0; i < Math.Min(item.Scale.Z + 0, 8); i++)
			{
				transform.Push();
				double v4 = i * 360.0;
				Vector3 v7 = new Vector3(
					0,
					ObjectHelper.NJSin((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f),
					ObjectHelper.NJCos((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f));
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation.X, item.Rotation.Y, 0);
				Vector3 pos = Vector3.TransformCoordinate(v7, transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model.DrawModelTreeInvert(transform, meshes));
				transform.Pop();
			}
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			for (int i = 0; i < Math.Min(item.Scale.Z + 0, 8); i++)
			{
				transform.Push();
				double v4 = i * 360.0;
				Vector3 v7 = new Vector3(
					0,
					ObjectHelper.NJSin((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f),
					ObjectHelper.NJCos((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f));
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation.X, item.Rotation.Y, 0);
				Vector3 pos = Vector3.TransformCoordinate(v7, transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result.Add(new ModelTransform(model, transform.Top));
				transform.Pop();
			}
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			BoundingSphere result = new BoundingSphere();
			for (int i = 0; i < Math.Min(item.Scale.Z + 0, 8); i++)
			{
				transform.Push();
				double v4 = i * 360.0;
				Vector3 v7 = new Vector3(
					0,
					ObjectHelper.NJSin((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f),
					ObjectHelper.NJCos((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f));
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation.X, item.Rotation.Y, 0);
				Vector3 pos = Vector3.TransformCoordinate(v7, transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result = SAModel.Direct3D.Extensions.Merge(result, ObjectHelper.GetModelBounds(model, transform));
				transform.Pop();
			}
			return result;
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Number of Rings", typeof(byte), "Extended", null, 1, (o) => (byte)Math.Min(o.Scale.Z + 0, 8), (o, v) => o.Scale.Z = Math.Max(Math.Min((byte)v - 0, 8), 0)),
			new PropertySpec("Size", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Shrine ID", typeof(int), "Extended", null, null, (o) => o.Rotation.Z, (o, v) => o.Rotation.Z = (int)v)
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Circle of Rings (Mystic Melody)"; } }
    }

    public class SwRngL : RingLine
    {
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			int sine = 0;
			for (int i = 0; i < Math.Min(item.Scale.Z + 0, 8); i++)
			{
				transform.Push();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation.X, item.Rotation.Y, 0);
				double v5 = i * ((item.Scale.X + 10f) * -1);
				double v6 = ObjectHelper.NJSin(sine) * item.Scale.Y;
				sine += 0x4000 / (int)item.Scale.Z;
				Vector3 pos = Vector3.TransformCoordinate(new Vector3(0, (float)v6, (float)v5), transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
				transform.Pop();
			}
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texs == null)
				texs = ObjectHelper.GetTextures("objtex_common", texarr, dev);
			int sine = 0;
			for (int i = 0; i < Math.Min(item.Scale.Z + 0, 8); i++)
			{
				transform.Push();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation.X, item.Rotation.Y, 0);
				double v5 = i * ((item.Scale.X + 10f) * -1);
				double v6 = ObjectHelper.NJSin(sine) * item.Scale.Y;
				sine += 0x4000 / (int)item.Scale.Z;
				Vector3 pos = Vector3.TransformCoordinate(new Vector3(0, (float)v6, (float)v5), transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model.DrawModelTreeInvert(transform, meshes));
				transform.Pop();
			}
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			int sine = 0;
			for (int i = 0; i < Math.Min(item.Scale.Z + 0, 8); i++)
			{
				transform.Push();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation.X, item.Rotation.Y, 0);
				double v5 = i * ((item.Scale.X + 10f) * -1);
				double v6 = ObjectHelper.NJSin(sine) * item.Scale.Y;
				sine += 0x4000 / (int)item.Scale.Z;
				Vector3 pos = Vector3.TransformCoordinate(new Vector3(0, (float)v6, (float)v5), transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result.Add(new ModelTransform(model, transform.Top));
				transform.Pop();
			}
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			BoundingSphere result = new BoundingSphere();
			int sine = 0;
			for (int i = 0; i < Math.Min(item.Scale.Z + 0, 8); i++)
			{
				transform.Push();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation.X, item.Rotation.Y, 0);
				double v5 = i * ((item.Scale.X + 10f) * -1);
				double v6 = ObjectHelper.NJSin(sine) * item.Scale.Y;
				sine += 0x4000 / (int)item.Scale.Z;
				Vector3 pos = Vector3.TransformCoordinate(new Vector3(0, (float)v6, (float)v5), transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result = SAModel.Direct3D.Extensions.Merge(result, ObjectHelper.GetModelBounds(model, transform));
				transform.Pop();
			}
			return result;
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Number of Rings", typeof(byte), "Extended", null, 1, (o) => (byte)Math.Min(o.Scale.Z + 0, 8), (o, v) => o.Scale.Z = Math.Max(Math.Min((byte)v - 0, 8), 0)),
			new PropertySpec("Size", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Switch ID", typeof(int), "Extended", null, null, (o) => o.Rotation.Z, (o, v) => o.Rotation.Z = (int)v),
			new PropertySpec("Sine Wave Strength", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v)
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Line of Rings (Switch)"; } }
    }

    public class SwRngC : RingCircle
    {
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			for (int i = 0; i < Math.Min(item.Scale.Z + 0, 8); i++)
			{
				transform.Push();
				double v4 = i * 360.0;
				Vector3 v7 = new Vector3(
					0,
					ObjectHelper.NJSin((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f),
					ObjectHelper.NJCos((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f));
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation.X, item.Rotation.Y, 0);
				Vector3 pos = Vector3.TransformCoordinate(v7, transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
				transform.Pop();
			}
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texs == null)
				texs = ObjectHelper.GetTextures("objtex_common", texarr, dev);
			for (int i = 0; i < Math.Min(item.Scale.Z + 0, 8); i++)
			{
				transform.Push();
				double v4 = i * 360.0;
				Vector3 v7 = new Vector3(
					0,
					ObjectHelper.NJSin((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f),
					ObjectHelper.NJCos((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f));
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation.X, item.Rotation.Y, 0);
				Vector3 pos = Vector3.TransformCoordinate(v7, transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model.DrawModelTreeInvert(transform, meshes));
				transform.Pop();
			}
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			for (int i = 0; i < Math.Min(item.Scale.Z + 0, 8); i++)
			{
				transform.Push();
				double v4 = i * 360.0;
				Vector3 v7 = new Vector3(
					0,
					ObjectHelper.NJSin((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f),
					ObjectHelper.NJCos((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f));
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation.X, item.Rotation.Y, 0);
				Vector3 pos = Vector3.TransformCoordinate(v7, transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result.Add(new ModelTransform(model, transform.Top));
				transform.Pop();
			}
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			BoundingSphere result = new BoundingSphere();
			for (int i = 0; i < Math.Min(item.Scale.Z + 0, 8); i++)
			{
				transform.Push();
				double v4 = i * 360.0;
				Vector3 v7 = new Vector3(
					0,
					ObjectHelper.NJSin((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f),
					ObjectHelper.NJCos((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * (item.Scale.X + 10f));
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation.X, item.Rotation.Y, 0);
				Vector3 pos = Vector3.TransformCoordinate(v7, transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result = SAModel.Direct3D.Extensions.Merge(result, ObjectHelper.GetModelBounds(model, transform));
				transform.Pop();
			}
			return result;
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Number of Rings", typeof(byte), "Extended", null, 1, (o) => (byte)Math.Min(o.Scale.Z + 0, 8), (o, v) => o.Scale.X = Math.Max(Math.Min((byte)v - 0, 8), 0)),
			new PropertySpec("Size", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Switch ID", typeof(int), "Extended", null, null, (o) => o.Rotation.Z, (o, v) => o.Rotation.Z = (int)v)
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Circle of Rings (Switch)"; } }
    }
}
