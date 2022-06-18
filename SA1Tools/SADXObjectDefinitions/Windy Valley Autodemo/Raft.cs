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

namespace SADXObjectDefinitions.WindyValleyAutodemo
{
	public abstract class RaftBase : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] meshes;
		protected NJS_OBJECT destModel;
		protected Mesh[] destMesh;

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			float Scale = item.Scale.Z + 1.0f;

			if (item.Scale.Y != 0 && item.Selected)
			{
				transform.Push();
				transform.NJTranslate(item.Position.X, (item.Position.Y + item.Scale.Y), item.Position.Z);
				transform.NJRotateY(item.Rotation.Y);
				transform.NJScale(Scale, Scale, Scale);
				result = destModel.CheckHit(Near, Far, Viewport, Projection, View, transform, destMesh);
				transform.Pop();
			}
			
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(Scale, Scale, Scale);
			result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			float Scale = item.Scale.Z + 1.0f;

			if (item.Scale.Y != 0 && item.Selected)
			{

				transform.Push();
				transform.NJTranslate(item.Position.X, (item.Position.Y + item.Scale.Y), item.Position.Z);
				transform.NJRotateY(item.Rotation.Y);
				transform.NJScale(Scale, Scale, Scale);
				result.AddRange(destModel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, destMesh));
				if (item.Selected)
					result.AddRange(destModel.DrawModelTreeInvert(transform, destMesh));
				transform.Pop();
			}

			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(Scale, Scale, Scale);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_WINDY"), meshes));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			return result;
		}
		
		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();

			float Scale = item.Scale.Z + 1.0f;

			if (item.Scale.Y != 0 && item.Selected)
			{
				transform.Push();
				transform.NJTranslate(item.Position.X, (item.Position.Y + item.Scale.Y), item.Position.Z);
				transform.NJRotateY(item.Rotation.Y);
				transform.NJScale(Scale, Scale, Scale);
				result.Add(new ModelTransform(destModel, transform.Top));
				transform.Pop();
			}

			
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(Scale, Scale, Scale);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();

			float middle = (item.Position.Y + (item.Scale.Y / 2));
			float radius = (((Math.Abs(item.Scale.Y)) / 2) + 50.0f);
			BoundingSphere boxSphere = new BoundingSphere() { Center = new Vertex((item.Position.X), (middle), item.Position.Z), Radius = radius };
			return boxSphere;
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;
			float Scale = item.Scale.Z + 1.0f;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);
			MatrixFunctions.Scale(ref matrix, Scale, Scale, Scale);

			return matrix;
		}
	}

	public class Raft2 : RaftBase
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_raft1.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
			destModel = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_raft1.sa1mdl");
			destMesh = ObjectHelper.GetMeshes(destModel);
		}
		public override string Name { get { return "Smaller Floating Platform"; } }

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Y Destination (Local)", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Platform Scale", typeof(float), "Extended", null, null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}

	public class Raft3 : RaftBase
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_raft2.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
			destModel = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_raft2.sa1mdl");
			destMesh = ObjectHelper.GetMeshes(destModel);
		}
		public override string Name { get { return "Larger Floating Platform"; } }

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Y Destination (Local)", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Platform Scale", typeof(float), "Extended", null, null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}

	public class TRaft1 : RaftBase
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_transportraft1.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
			destModel = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_transportraft1.sa1mdl");
			destMesh = ObjectHelper.GetMeshes(destModel);
		}
		public override string Name { get { return "Tiny Floating Platform (Tornado)"; } }

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Y Destination (Local)", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Platform Scale", typeof(float), "Extended", null, null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}

	public class TRaft2 : RaftBase
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_transportraft2.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
			destModel = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_transportraft2.sa1mdl");
			destMesh = ObjectHelper.GetMeshes(destModel);
		}
		public override string Name { get { return "Large U Shaped Floating Platform (Tornado)"; } }

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Y Destination (Local)", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Platform Scale", typeof(float), "Extended", null, null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
}