using System.Collections.Generic;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using System;
using SharpDX.Direct3D9;
using Mesh = SAModel.Direct3D.Mesh;
using SharpDX;
using SplitTools;

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
		private float? xpos, ypos, zpos, xscl, yscl, zscl, defxscl, defyscl, defzscl, gnddst, addxscl, addyscl, addzscl;
		private int? xrot, yrot, zrot;
		private ushort? defxrot, defyrot, defzrot, addxrot, addyrot, addzrot;
		private string rottype;
		private string scltype;
		private Texture[] texs;
		private TexnameArray texnames;

		public override void Init(ObjectData data, string name)
		{
			this.name = data.Name ?? name;
			if (!string.IsNullOrEmpty(data.Model))
			{
				model = ObjectHelper.LoadModel(data.Model);
				meshes = ObjectHelper.GetMeshes(model);
				if (!string.IsNullOrEmpty(data.Texlist))
					texnames = new TexnameArray(data.Texlist);
			}

			texture = data.Texture;
			xpos = data.XPos;
			ypos = data.YPos;
			zpos = data.ZPos;
			//xrot = data.XRot;
			//yrot = data.YRot;
			//zrot = data.ZRot;
			//xscl = data.XScl;
			//yscl = data.YScl;
			//zscl = data.ZScl;
			defxrot = data.DefXRot;
			defyrot = data.DefYRot;
			defzrot = data.DefZRot;
			defxscl = data.DefXScl;
			defyscl = data.DefYScl;
			defzscl = data.DefZScl;
			gnddst = data.GndDst;
			rottype = data.RotType;
			scltype = data.SclType;
			addxrot = data.AddXRot;
			addyrot = data.AddYRot;
			addzrot = data.AddZRot;
			addxscl = data.AddXScl;
			addyscl = data.AddYScl;
			addzscl = data.AddZScl;
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(xpos ?? item.Position.X, ypos ?? item.Position.Y, zpos ?? item.Position.Z);
			//transform.NJRotateObject(xrot ?? item.Rotation.X, yrot ?? item.Rotation.Y, zrot ?? item.Rotation.Z);
			Rotation addrot = new Rotation(addxrot ?? 0, addyrot ?? 0, addzrot ?? 0);
			ObjectHelper.RotateObject(transform, item, addrot, rottype);
			HitResult result;
			if (model == null)
				result = ObjectHelper.CheckQuestionBoxHit(Near, Far, Viewport, Projection, View, transform);
			else
			{
				//transform.NJScale(xscl ?? item.Scale.X, yscl ?? item.Scale.Y, zscl ?? item.Scale.Z);
				Vector3 addscl = new Vector3();
				Vector3 scl = ObjectHelper.GetScale(item, addscl, scltype);
				transform.NJScale(scl);
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
			//transform.NJRotateObject(xrot ?? item.Rotation.X, yrot ?? item.Rotation.Y, zrot ?? item.Rotation.Z);
			Rotation addrot = new Rotation(addxrot ?? 0, addyrot ?? 0, addzrot ?? 0);
			ObjectHelper.RotateObject(transform, item, addrot, rottype);
			if (model == null)
				result.AddRange(ObjectHelper.RenderQuestionBox(dev, transform, null, item.Position.ToVector3(), item.Selected));
			else
			{
				if (texs == null)
					texs = ObjectHelper.GetTextures(texture, texnames, dev);
				//transform.NJScale(xscl ?? item.Scale.X, yscl ?? item.Scale.Y, zscl ?? item.Scale.Z);
				Vector3 addscl = new Vector3();
				Vector3 scl = ObjectHelper.GetScale(item, addscl, scltype);
				transform.NJScale(scl);
				result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
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
			//transform.NJRotateObject(xrot ?? item.Rotation.X, yrot ?? item.Rotation.Y, zrot ?? item.Rotation.Z);
			//transform.NJScale(xscl ?? item.Scale.X, yscl ?? item.Scale.Y, zscl ?? item.Scale.Z);
			Rotation addrot = new Rotation(addxrot ?? 0, addyrot ?? 0, addzrot ?? 0);
			ObjectHelper.RotateObject(transform, item, addrot, rottype);
			Vector3 addscl = new Vector3();
			Vector3 scl = ObjectHelper.GetScale(item, addscl, scltype);
			transform.NJScale(scl);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(xpos ?? item.Position.X, ypos ?? item.Position.Y, zpos ?? item.Position.Z);
			//transform.NJRotateObject(xrot ?? item.Rotation.X, yrot ?? item.Rotation.Y, zrot ?? item.Rotation.Z);
			Rotation addrot = new Rotation(addxrot ?? 0, addyrot ?? 0, addzrot ?? 0);
			ObjectHelper.RotateObject(transform, item, addrot, rottype);
			if (model == null)
				return ObjectHelper.GetQuestionBoxBounds(transform);
			else
			{
				//transform.NJScale(xscl ?? item.Scale.X, yscl ?? item.Scale.Y, zscl ?? item.Scale.Z);
				Vector3 addscl = new Vector3();
				Vector3 scl = ObjectHelper.GetScale(item, addscl, scltype);
				transform.NJScale(scl);
				return ObjectHelper.GetModelBounds(model, transform, Math.Max(Math.Max(scl.X, scl.Y), scl.Z));
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

		public override string Name { get { return name; } }

		public override ushort DefaultXRotation { get { return defxrot ?? base.DefaultXRotation; } }
		public override ushort DefaultYRotation { get { return defyrot ?? base.DefaultYRotation; } }
		public override ushort DefaultZRotation { get { return defzrot ?? base.DefaultZRotation; } }
		public override float DefaultXScale { get { return defxscl ?? base.DefaultXScale; } }
		public override float DefaultYScale { get { return defyscl ?? base.DefaultYScale; } }
		public override float DefaultZScale { get { return defzscl ?? base.DefaultZScale; } }
		public override float DistanceFromGround { get { return gnddst ?? base.DistanceFromGround; } }
		public override ushort AddXRotation { get { return addxrot ?? base.AddXRotation; } }
		public override ushort AddYRotation { get { return addyrot ?? base.AddYRotation; } }
		public override ushort AddZRotation { get { return addzrot ?? base.AddZRotation; } }
		public override float AddXScale { get { return addxscl ?? base.AddXScale; } }
		public override float AddYScale { get { return addyscl ?? base.AddYScale; } }
		public override float AddZScale { get { return addzscl ?? base.AddZScale; } }
	}
}