using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.EmeraldCoast
{
	public class ArchRock : ObjectDefinition
	{
		protected NJS_OBJECT arch;
		protected Mesh[] archmsh;
		protected NJS_OBJECT side1;
		protected Mesh[] side1msh;
		protected NJS_OBJECT side2;
		protected Mesh[] side2msh;

		public override void Init(ObjectData data, string name)
		{
			arch = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_kowareiwa_b.nja.sa1mdl");
			archmsh = ObjectHelper.GetMeshes(arch);
			side1 = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_kowareiwa_a.nja.sa1mdl");
			side1msh = ObjectHelper.GetMeshes(side1);
			side2 = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_kowareiwa_c.nja.sa1mdl");
			side2msh = ObjectHelper.GetMeshes(side2);
		}

		public override string Name { get { return "Arched Rock"; } }

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJTranslate(0, 110, 0);
			result = HitResult.Min(result, arch.CheckHit(Near, Far, Viewport, Projection, View, transform, archmsh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(ObjectHelper.NJSin(item.Rotation.Y) * 73 + item.Position.X, item.Position.Y, ObjectHelper.NJCos(item.Rotation.Y) * 73 + item.Position.Z);
			result = HitResult.Min(result, side1.CheckHit(Near, Far, Viewport, Projection, View, transform, side1msh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X - ObjectHelper.NJSin(item.Rotation.Y) * 57, item.Position.Y, item.Position.Z - ObjectHelper.NJCos(item.Rotation.Y) * 57);
			result = HitResult.Min(result, side2.CheckHit(Near, Far, Viewport, Projection, View, transform, side2msh));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJTranslate(0, 110, 0);
			result.AddRange(arch.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_BEACH"), archmsh));
			if (item.Selected)
				result.AddRange(arch.DrawModelTreeInvert(transform, archmsh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(ObjectHelper.NJSin(item.Rotation.Y) * 73 + item.Position.X, item.Position.Y, ObjectHelper.NJCos(item.Rotation.Y) * 73 + item.Position.Z);
			result.AddRange(side1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_BEACH"), side1msh));
			if (item.Selected)
				result.AddRange(side1.DrawModelTreeInvert(transform, side1msh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X - ObjectHelper.NJSin(item.Rotation.Y) * 57, item.Position.Y, item.Position.Z - ObjectHelper.NJCos(item.Rotation.Y) * 57);
			result.AddRange(side2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_BEACH"), side2msh));
			if (item.Selected)
				result.AddRange(side2.DrawModelTreeInvert(transform, side2msh));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJTranslate(0, 110, 0);
			result.Add(new ModelTransform(arch, transform.Top));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(ObjectHelper.NJSin(item.Rotation.Y) * 73 + item.Position.X, item.Position.Y, ObjectHelper.NJCos(item.Rotation.Y) * 73 + item.Position.Z);
			result.Add(new ModelTransform(side1, transform.Top));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X - ObjectHelper.NJSin(item.Rotation.Y) * 57, item.Position.Y, item.Position.Z - ObjectHelper.NJCos(item.Rotation.Y) * 57);
			result.Add(new ModelTransform(side2, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			BoundingSphere boxSphere = new BoundingSphere() { Center = new Vertex((item.Position.X + 0.75f), (item.Position.Y + 55f), item.Position.Z), Radius = 100f };

			return boxSphere;
		}

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);			

			return matrix;
		}
	}
}