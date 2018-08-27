using SharpDX;
using SharpDX.Direct3D9;
using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using BoundingSphere = SonicRetro.SAModel.BoundingSphere;
using Mesh = SonicRetro.SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.EmeraldCoast
{
	public abstract class OArchRock1 : ObjectDefinition
	{
		protected NJS_OBJECT arch;
		protected Mesh[] archmsh;
		protected NJS_OBJECT side1;
		protected Mesh[] side1msh;
		protected NJS_OBJECT side2;
		protected Mesh[] side2msh;

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			transform.TranslateLocal(0, 110f, 0);
			transform.Push();
			result = HitResult.Min(result, arch.CheckHit(Near, Far, Viewport, Projection, View, transform, archmsh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			transform.TranslateLocal(0, 0, 73f);
			transform.Push();
			result = HitResult.Min(result, side1.CheckHit(Near, Far, Viewport, Projection, View, transform, side1msh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			transform.TranslateLocal(0, 0, -57f);
			transform.Push();
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
			transform.TranslateLocal(0, 110f, 0);
			result.AddRange(arch.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_BEACH"), archmsh));
			if (item.Selected)
				result.AddRange(arch.DrawModelTreeInvert(transform, archmsh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			transform.TranslateLocal(0, 0, 73f);
			result.AddRange(side1.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_BEACH"), side1msh));
			if (item.Selected)
				result.AddRange(side1.DrawModelTreeInvert(transform, side1msh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			transform.TranslateLocal(0, 0, -57f);
			result.AddRange(side2.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_BEACH"), side2msh));
			if (item.Selected)
				result.AddRange(side2.DrawModelTreeInvert(transform, side2msh));
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
	}

	public class ArchRock : OArchRock1
	{
		public override void Init(ObjectData data, string name, Device dev)
		{
			arch = ObjectHelper.LoadModel("Objects/Levels/Emerald Coast/O ARCHROCK.sa1mdl");
			archmsh = ObjectHelper.GetMeshes(arch, dev);
			side1 = ObjectHelper.LoadModel("Objects/Levels/Emerald Coast/O BIGROCK_A.sa1mdl");
			side1msh = ObjectHelper.GetMeshes(side1, dev);
			side2 = ObjectHelper.LoadModel("Objects/Levels/Emerald Coast/O BIGROCK_B.sa1mdl");
			side2msh = ObjectHelper.GetMeshes(side2, dev);
		}

		public override string Name { get { return "Arched Rock"; } }
	}
}