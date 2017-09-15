using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;

namespace SADXObjectDefinitions.EmeraldCoast
{
	public abstract class O_TAKI : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] mesh;

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJScale((item.Scale.X + 1f), (item.Scale.Y + 1f), (item.Scale.Z + 1f));
			transform.Push();
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJScale((item.Scale.X + 1f), (item.Scale.Y + 1f), (item.Scale.Z + 1f));
			result.AddRange(model.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_BEACH"), mesh));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, mesh));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJScale((item.Scale.X + 1f), (item.Scale.Y + 1f), (item.Scale.Z + 1f));
			return ObjectHelper.GetModelBounds(model, transform);
		}
	}

	public class Waterfall : O_TAKI
	{
		public override void Init(ObjectData data, string name, Device dev)
		{
			model = ObjectHelper.LoadModel("Objects/Levels/Emerald Coast/O TAKI.sa1mdl");
			mesh = ObjectHelper.GetMeshes(model, dev);
		}

		public override string Name { get { return "Waterfall"; } }
	}
}