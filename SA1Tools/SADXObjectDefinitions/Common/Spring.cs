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
	public abstract class SpringBase : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] meshes;

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_REGULAR"), meshes));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			return ObjectHelper.GetModelBounds(model, transform);
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}

		public override void PointTo(SETItem item, Vertex location)
		{
			SetOrientation(item, item.Position - location);
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Disable Timer", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Speed", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		private readonly VerbSpec[] customVerbs = new VerbSpec[] {
			new VerbSpec("Point To", o => LevelData.BeginPointOperation())
		};

		public override VerbSpec[] CustomVerbs { get { return customVerbs; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation);

			return matrix;
		}
	}

	public class Spring : SpringBase
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/bane_banebottom.nja.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}

		public override string Name { get { return "Ground Spring"; } }
	}

	public class SpringB : SpringBase
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/bane_type_b_type_b.nja.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}

		public override string Name { get { return "Air Spring"; } }
	}
}