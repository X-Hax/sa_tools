using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.Common
{
	public class DashHoop : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/dushring_cyl6_1.nja.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			int x = item.Rotation.X;
			int y = item.Rotation.Y;
			if (!item.Scale.IsEmpty)
				(item.Position - item.Scale).GetRotation(out x, out y);
			transform.NJRotateX(x);
			transform.NJRotateY(y);
			transform.NJRotateZ(item.Rotation.Z);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			int x = item.Rotation.X;
			int y = item.Rotation.Y;
			if (!item.Scale.IsEmpty)
				(item.Position - item.Scale).GetRotation(out x, out y);
			transform.NJRotateX(x);
			transform.NJRotateY(y);
			transform.NJRotateZ(item.Rotation.Z);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_REGULAR"), meshes));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			if (item.Selected)
			{
				FVF_PositionColored[] verts = new FVF_PositionColored[2];
				verts[0] = new FVF_PositionColored(item.Position.ToVector3(), System.Drawing.Color.Yellow);
				verts[1] = new FVF_PositionColored(item.Scale.ToVector3(), System.Drawing.Color.Yellow);
				dev.VertexFormat = FVF_PositionColored.Format;
				dev.DrawUserPrimitives(PrimitiveType.LineList, 1, verts);
			}
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			int x = item.Rotation.X;
			int y = item.Rotation.Y;
			if (!item.Scale.IsEmpty)
				(item.Position - item.Scale).GetRotation(out x, out y);
			transform.NJRotateX(x);
			transform.NJRotateY(y);
			transform.NJRotateZ(item.Rotation.Z);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			int x = item.Rotation.X;
			int y = item.Rotation.Y;
			if (!item.Scale.IsEmpty)
				(item.Position - item.Scale).GetRotation(out x, out y);
			transform.NJRotateX(x);
			transform.NJRotateY(y);
			transform.NJRotateZ(item.Rotation.Z);
			return ObjectHelper.GetModelBounds(model, transform);
		}

		public override void PointTo(SETItem item, Vertex location)
		{
			item.Scale = location;
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);

			int x = item.Rotation.X;
			int y = item.Rotation.Y;
			if (!item.Scale.IsEmpty)
				(item.Position - item.Scale).GetRotation(out x, out y);

			MatrixFunctions.RotateY(ref matrix, x);
			MatrixFunctions.RotateY(ref matrix, y);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Z);

			return matrix;
		}

		public override string Name { get { return "Dash Hoop"; } }

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Target", typeof(Vertex), "Extended", null, new Vertex(), (o) => o.Scale, (o, v) => o.Scale = (Vertex)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		private readonly VerbSpec[] customVerbs = new VerbSpec[] {
			new VerbSpec("Point To", o => LevelData.BeginPointOperation())
		};

		public override VerbSpec[] CustomVerbs { get { return customVerbs; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
}