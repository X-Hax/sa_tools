using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using SharpDX;
using SharpDX.Direct3D9;
using SplitTools;
using System;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SA2ObjectDefinitions.GreenForest
{
	public class Gake : ObjectDefinition
	{
		protected NJS_OBJECT[] models;
		protected Mesh[][] meshes;
		protected NJS_OBJECT point;
		protected Mesh[] pointmesh;
		protected NJS_TEXLIST[] texarrs;
		protected Texture[][] texs;
		protected List<string> texpacks = [];

		public override void Init(ObjectData data, string name)
		{
			models = new NJS_OBJECT[6];
			meshes = new Mesh[6][];
			texarrs = new NJS_TEXLIST[6];
			texs = new Texture[6][];
			
			models[0] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/GAKE_A.sa2bmdl");
			models[1] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/GAKE_B.sa2bmdl");
			models[2] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/GAKE_C.sa2bmdl");
			models[3] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/GAKE_D.sa2bmdl");
			models[4] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/GAKE_E.sa2bmdl");
			models[5] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/GAKE_F.sa2bmdl");

			texarrs[0] = NJS_TEXLIST.Load("stg03_Jungle/tls/GAKE_A.satex");
			texarrs[1] = NJS_TEXLIST.Load("stg03_Jungle/tls/GAKE_B.satex");
			texarrs[2] = NJS_TEXLIST.Load("stg03_Jungle/tls/GAKE_C.satex");
			texarrs[3] = NJS_TEXLIST.Load("stg03_Jungle/tls/GAKE_D.satex");
			texarrs[4] = NJS_TEXLIST.Load("stg03_Jungle/tls/GAKE_E.satex");
			texarrs[5] = NJS_TEXLIST.Load("stg03_Jungle/tls/GAKE_F.satex");

			for (int i = 0; i < 6; i++)
				meshes[i] = ObjectHelper.GetMeshes(models[i]);

			point = ObjectHelper.LoadModel("OBJECT/SmallAnimalBubbleCollision.sa2mdl");
			pointmesh = ObjectHelper.GetMeshes(point);

			texpacks.Add("objtex_stg03");
			texpacks.Add("landtx03");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			float radius = Math.Abs(item.Scale.X / 10F);
			for (int i = 0; i < 6; i++)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				result = HitResult.Min(result, models[i].CheckHit(Near, Far, Viewport, Projection, View, transform, meshes[i]));
				transform.Pop();
			}
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 520.0F, item.Position.Z);
			transform.NJScale(radius, radius, radius);
			result = HitResult.Min(result, point.CheckHit(Near, Far, Viewport, Projection, View, transform, pointmesh));
			transform.Pop();
			return result;
		}
		
		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			float radius = Math.Abs(item.Scale.X / 10F);
			for (int i = 0; i < 6; i++)
			{
				if (texs[i] == null)
					texs[i] = ObjectHelper.GetTextures(texpacks, texarrs[i], dev);
			}
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			for (int i = 0; i < 6; i++)
			{
				result.AddRange(models[i].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs[i], meshes[i], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(models[i].DrawModelTreeInvert(transform, meshes[i]));
			}
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 520.0F, item.Position.Z);
			transform.NJScale(radius, radius, radius);
			result.AddRange(point.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, pointmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(point.DrawModelTreeInvert(transform, pointmesh));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			float radius = Math.Abs(item.Scale.X / 10F);
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			for (int i = 0; i < 6; i++)
			{
				result.Add(new ModelTransform(models[i], transform.Top));
			}
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 520.0F, item.Position.Z);
			transform.NJScale(radius, radius, radius);
			result.Add(new ModelTransform(point, transform.Top));
			transform.Pop();
			return result;
		}
		
		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			float radius = Math.Abs(item.Scale.X / 10F);
			transform.Push();
			transform.NJTranslate(item.Position.ToVector3());
			BoundingSphere platformbounds = ObjectHelper.GetModelBounds(point, transform);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			for (int i = 0; i < 6; i++)
				platformbounds = SAModel.Direct3D.Extensions.Merge(platformbounds, ObjectHelper.GetModelBounds(models[i], transform));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 520.0F, item.Position.Z);
			transform.NJScale(radius, radius, radius);
			platformbounds = SAModel.Direct3D.Extensions.Merge(platformbounds, ObjectHelper.GetModelBounds(point, transform));
			transform.Pop();
			return platformbounds;
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, 0, item.Rotation.Y - 0x8000, 0);

			return matrix;
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Trigger Radius", typeof(float), "Extended", "The trigger's position will always be 520 units above the set position, and the radius is always treated as positive.", null, (o) => Math.Abs(o.Scale.X), (o, v) => o.Scale.X = Math.Abs((float)v)),
			new PropertySpec("Use Normal Parameters", typeof(bool), "Extended", "This determines the falling behavior of the platforms. Setting this to False will cause the platforms to fall slower and stop above water, which also overrides Hard Mode.", null, (o) => Convert.ToBoolean((o.Rotation.X & 0xF) % 2 == 0),
				(o, v) => { o.Rotation.X &= 0xFFF0; o.Rotation.X |= Convert.ToByte(!(bool)v); }),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Falling Platforms"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
}