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
			new PropertySpec("Size", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
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
			for (int i = 0; i < Math.Min(item.Scale.Z + 0, 8); i++)
			{
				transform.Push();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
                double v5 = i * item.Scale.X * -1;
                Vector3 pos = Vector3.TransformCoordinate(new Vector3(0, 0, (float)v5), transform.Top);
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
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
                double v5 = i * item.Scale.X * -1;
                Vector3 pos = Vector3.TransformCoordinate(new Vector3(0, 0, (float)v5), transform.Top);
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
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
                double v5 = i * item.Scale.X * -1;
                Vector3 pos = Vector3.TransformCoordinate(new Vector3(0, 0, (float)v5), transform.Top);
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
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
				double v5 = i * item.Scale.X * -1;
				Vector3 pos = Vector3.TransformCoordinate(new Vector3(0, 0, (float)v5), transform.Top);
				transform.Pop();
				transform.NJTranslate(pos);
				result = SAModel.Direct3D.Extensions.Merge(result, ObjectHelper.GetModelBounds(model, transform));
				transform.Pop();
			}
			return result;
		}
		
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
					ObjectHelper.NJSin((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * item.Scale.X,
					ObjectHelper.NJCos((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * item.Scale.X);
                transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
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
					ObjectHelper.NJSin((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * item.Scale.X,
					ObjectHelper.NJCos((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * item.Scale.X);
                transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
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
					ObjectHelper.NJSin((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * item.Scale.X,
					ObjectHelper.NJCos((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * item.Scale.X);
                transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
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
					ObjectHelper.NJSin((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * item.Scale.X,
					ObjectHelper.NJCos((int)(v4 / item.Scale.Z * 65536.0 * 0.002777777777777778)) * item.Scale.X);
                transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
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
		public override string Name { get { return "Line of Rings (Mystic Melody)"; } }
	}
	
	public class MstRngC : RingCircle
	{
		public override string Name { get { return "Circle of Rings (Mystic Melody)"; } }
    }

    public class SwRngL : RingLine
    {
        public override string Name { get { return "Line of Rings (Switch)"; } }
    }

    public class SwRngC : RingCircle
    {
        public override string Name { get { return "Circle of Rings (Switch)"; } }
    }
}
