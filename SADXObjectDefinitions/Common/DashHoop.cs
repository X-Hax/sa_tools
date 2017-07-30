using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;

namespace SADXObjectDefinitions.Common
{
	public class DashHoop : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;

		public override void Init(ObjectData data, string name, Device dev)
		{
			model = ObjectHelper.LoadModel("Objects/Common/Dash Hoop.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model, dev);
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
			result.AddRange(model.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_REGULAR"), meshes));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			if (item.Selected)
			{
				CustomVertex.PositionColored[] verts = new CustomVertex.PositionColored[2];
				verts[0] = new CustomVertex.PositionColored(item.Position.ToVector3(), System.Drawing.Color.Yellow.ToArgb());
				verts[1] = new CustomVertex.PositionColored(item.Scale.ToVector3(), System.Drawing.Color.Yellow.ToArgb());
				dev.VertexFormat = VertexFormats.Position | VertexFormats.Diffuse;
				dev.DrawUserPrimitives(PrimitiveType.LineList, 1, verts);
			}
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

		public override string Name { get { return "Dash Hoop"; } }

		private PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Target", typeof(Vertex), "Extended", null, new Vertex(), (o) => o.Scale, (o, v) => o.Scale = (Vertex)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		private VerbSpec[] customVerbs = new VerbSpec[] {
			new VerbSpec("Point To", o => LevelData.BeginPointOperation())
		};

		public override VerbSpec[] CustomVerbs { get { return customVerbs; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
}