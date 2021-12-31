using System.Collections.Generic;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using System;
using SharpDX.Direct3D9;
using Mesh = SAModel.Direct3D.Mesh;
using SharpDX;

namespace SAModel.SAEditorCommon.SETEditing
{
	public class DefaultObjectDefinition : ObjectDefinition
	{
		public static ObjectDefinition DefaultInstance { get; private set; }

		static DefaultObjectDefinition()
		{
			DefaultInstance = new DefaultObjectDefinition();
		}

		private string name = "Unknown";
		private NJS_OBJECT model;
		private Mesh[] meshes;
		private string texture;
		private float? xpos, ypos, zpos, xscl, yscl, zscl, defxscl, defyscl, defzscl, gnddst;
		private int? xrot, yrot, zrot;
		private ushort? defxrot, defyrot, defzrot;

		public override void Init(ObjectData data, string name)
		{
			this.name = data.Name ?? name;
			if (!string.IsNullOrEmpty(data.Model))
			{
				model = ObjectHelper.LoadModel(data.Model);
				meshes = ObjectHelper.GetMeshes(model);
			}

			texture = data.Texture;
			xpos = data.XPos;
			ypos = data.YPos;
			zpos = data.ZPos;
			xrot = data.XRot;
			yrot = data.YRot;
			zrot = data.ZRot;
			xscl = data.XScl;
			yscl = data.YScl;
			zscl = data.ZScl;
			defxrot = data.DefXRot;
			defyrot = data.DefYRot;
			defzrot = data.DefZRot;
			defxscl = data.DefXScl;
			defyscl = data.DefYScl;
			defzscl = data.DefZScl;
			gnddst = data.GndDst;
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(xpos ?? item.Position.X, ypos ?? item.Position.Y, zpos ?? item.Position.Z);
			transform.NJRotateObject(xrot ?? item.Rotation.X, yrot ?? item.Rotation.Y, zrot ?? item.Rotation.Z);
			HitResult result;
			if (model == null)
				result = ObjectHelper.CheckSpriteHit(Near, Far, Viewport, Projection, View, transform);
			else
			{
				transform.NJScale(xscl ?? item.Scale.X, yscl ?? item.Scale.Y, zscl ?? item.Scale.Z);
				result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			}
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(xpos ?? item.Position.X, ypos ?? item.Position.Y, zpos ?? item.Position.Z);
			transform.NJRotateObject(xrot ?? item.Rotation.X, yrot ?? item.Rotation.Y, zrot ?? item.Rotation.Z);
			if (model == null)
				result.AddRange(ObjectHelper.RenderSprite(dev, transform, null, item.Position.ToVector3(), item.Selected));
			else
			{
				transform.NJScale(xscl ?? item.Scale.X, yscl ?? item.Scale.Y, zscl ?? item.Scale.Z);
				result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures(texture), meshes));
				if (item.Selected)
					result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			}
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			if (model == null) return new List<ModelTransform>();
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(xpos ?? item.Position.X, ypos ?? item.Position.Y, zpos ?? item.Position.Z);
			transform.NJRotateObject(xrot ?? item.Rotation.X, yrot ?? item.Rotation.Y, zrot ?? item.Rotation.Z);
			transform.NJScale(xscl ?? item.Scale.X, yscl ?? item.Scale.Y, zscl ?? item.Scale.Z);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(xpos ?? item.Position.X, ypos ?? item.Position.Y, zpos ?? item.Position.Z);
			transform.NJRotateObject(xrot ?? item.Rotation.X, yrot ?? item.Rotation.Y, zrot ?? item.Rotation.Z);
			if (model == null)
				return ObjectHelper.GetSpriteBounds(transform);
			else
			{
				transform.NJScale(xscl ?? item.Scale.X, yscl ?? item.Scale.Y, zscl ?? item.Scale.Z);
				return ObjectHelper.GetModelBounds(model, transform, Math.Max(Math.Max(xscl ?? item.Scale.X, yscl ?? item.Scale.Y), zscl ?? item.Scale.Z));
			}
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;
			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation);
			//MatrixFunctions.Scale(ref matrix, item.Scale);
			return matrix;
		}

		public override EditorRotationType GetRotationType(SETItem item)
		{
			return EditorRotationType.XYZ;
		}

		public override string Name { get { return name; } }

		public override ushort DefaultXRotation { get { return defxrot ?? base.DefaultXRotation; } }
		public override ushort DefaultYRotation { get { return defyrot ?? base.DefaultYRotation; } }
		public override ushort DefaultZRotation { get { return defzrot ?? base.DefaultZRotation; } }
		public override float DefaultXScale { get { return defxscl ?? base.DefaultXScale; } }
		public override float DefaultYScale { get { return defyscl ?? base.DefaultYScale; } }
		public override float DefaultZScale { get { return defzscl ?? base.DefaultZScale; } }
		public override float DistanceFromGround { get { return gnddst ?? base.DistanceFromGround; } }
	}
}