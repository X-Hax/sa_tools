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
	public class RolTutaT : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] meshes;
		protected NJS_TEXLIST texarr;
		protected Texture[] texs;
		protected List<string> texpacks = [];

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg03_Jungle/models/VerticalVine.sa2mdl");
			meshes = model.ProcessWeightedModel().ToArray();
			texarr = NJS_TEXLIST.Load("stg03_Jungle/tls/ROLTUTAT.satex");
			texpacks.Add("objtex_stg03");
			texpacks.Add("landtx03");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 495F, item.Position.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJRotateZ(-16384);
			HitResult result = model.CheckHitWeighted(Near, Far, Viewport, Projection, View, transform.Top, meshes);
			transform.Pop();
			return result;
		}
		
		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texs == null)
				texs = ObjectHelper.GetTextures(texpacks, texarr, dev);
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 495F, item.Position.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJRotateZ(-16384);
			result.AddRange(model.DrawModelTreeWeighted(dev.GetRenderState<FillMode>(RenderState.FillMode), transform.Top, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeWeightedInvert(transform.Top, meshes));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 495F, item.Position.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJRotateZ(-16384);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}
		
		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.X, item.Position.Y + 495F, item.Position.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJRotateZ(-16384);
			return ObjectHelper.GetModelBounds(model, transform);
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
			new PropertySpec("Vine Initial Angle", typeof(int), "Extended", null, null, (o) => o.Rotation.X, (o, v) => o.Rotation.X = (int)v),
			new PropertySpec("Vine Ending Angle", typeof(int), "Extended", null, null, (o) => o.Rotation.Z, (o, v) => o.Rotation.Z = (int)v),
			new PropertySpec("Log Angle", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Vine Swing Length", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Launch Power", typeof(float), "Extended", "Negative values tend to be used here. Values closer to 0 dramatically increase launch power.", null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Vertical Swinging Vine"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
	public class RolTuta : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] meshes;
		protected NJS_TEXLIST texarr;
		protected Texture[] texs;
		protected List<string> texpacks = [];

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg03_Jungle/models/HorizontalVine.sa2mdl");
			meshes = model.ProcessWeightedModel().ToArray();
			texarr = NJS_TEXLIST.Load("stg03_Jungle/tls/ROLTUTA.satex");
			texpacks.Add("objtex_stg03");
			texpacks.Add("landtx03");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			float newX = item.Position.X - (ObjectHelper.NJSin(item.Rotation.X) * ((item.Scale.Y + 1F) * 100F) + item.Scale.X);
			float newY = (ObjectHelper.NJCos(item.Rotation.X) * ((item.Scale.Y + 1F) * 100F)) + item.Position.Y;
			transform.Push();
			transform.NJTranslate(newX, newY - 200F, item.Position.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJTranslate(item.Scale.X, 0, 0);
			transform.NJRotateZ(-16384);
			HitResult result = model.CheckHitWeighted(Near, Far, Viewport, Projection, View, transform.Top, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			float newX = item.Position.X - (ObjectHelper.NJSin(item.Rotation.X) * ((item.Scale.Y + 1F) * 100F) + item.Scale.X);
			float newY = (ObjectHelper.NJCos(item.Rotation.X) * ((item.Scale.Y + 1F) * 100F)) + item.Position.Y;
			List<RenderInfo> result = new List<RenderInfo>();
			if (texs == null)
				texs = ObjectHelper.GetTextures(texpacks, texarr, dev);
			transform.Push();
			transform.NJTranslate(newX, newY - 200F, item.Position.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJTranslate(item.Scale.X, 0, 0);
			transform.NJRotateZ(-16384);
			result.AddRange(model.DrawModelTreeWeighted(dev.GetRenderState<FillMode>(RenderState.FillMode), transform.Top, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeWeightedInvert(transform.Top, meshes));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			float newX = item.Position.X - (ObjectHelper.NJSin(item.Rotation.X) * ((item.Scale.Y + 1F) * 100F) + item.Scale.X);
			float newY = (ObjectHelper.NJCos(item.Rotation.X) * ((item.Scale.Y + 1F) * 100F)) + item.Position.Y;
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(newX, newY - 200F, item.Position.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJTranslate(item.Scale.X, 0, 0);
			transform.NJRotateZ(-16384);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			float newX = item.Position.X - (ObjectHelper.NJSin(item.Rotation.X) * ((item.Scale.Y + 1F) * 100F) + item.Scale.X);
			float newY = (ObjectHelper.NJCos(item.Rotation.X) * ((item.Scale.Y + 1F) * 100F)) + item.Position.Y;
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(newX, newY - 200F, item.Position.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJTranslate(item.Scale.X, 0, 0);
			transform.NJRotateZ(-16384);
			return ObjectHelper.GetModelBounds(model, transform);
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
			new PropertySpec("Vine X Rotation Increase", typeof(int), "Extended", null, null, (o) => o.Rotation.X, (o, v) => o.Rotation.X = (int)v),
			new PropertySpec("Revolutions", typeof(int), "Extended", null, null, (o) => o.Rotation.Z, (o, v) => o.Rotation.Z = (int)v),
			new PropertySpec("Vine Swing Radius", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Vine Length", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Launch Power", typeof(float), "Extended", "Negative values tend to be used here. Values closer to 0 dramatically increase launch power.", null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Horizontal Swinging Vine"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
	public class BangieTuta : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] meshes;
		protected NJS_TEXLIST texarr;
		protected Texture[] texs;
		protected List<string> texpacks = [];

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg03_Jungle/models/BungeeVine.sa2mdl");
			meshes = model.ProcessWeightedModel().ToArray();
			texarr = NJS_TEXLIST.Load("stg03_Jungle/tls/BungeeVine.satex");
			texpacks.Add("objtex_stg03");
			texpacks.Add("landtx03");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + item.Scale.X + 2600F, item.Position.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJRotateZ(-16384);
			HitResult result = model.CheckHitWeighted(Near, Far, Viewport, Projection, View, transform.Top, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texs == null)
				texs = ObjectHelper.GetTextures(texpacks, texarr, dev);
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + item.Scale.X + 2600F, item.Position.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJRotateZ(-16384);
			result.AddRange(model.DrawModelTreeWeighted(dev.GetRenderState<FillMode>(RenderState.FillMode), transform.Top, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeWeightedInvert(transform.Top, meshes));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + item.Scale.X + 2600F, item.Position.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJRotateZ(-16384);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.X, item.Position.Y + item.Scale.X + 2600F, item.Position.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJRotateZ(-16384);
			return ObjectHelper.GetModelBounds(model, transform);
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
			new PropertySpec("Launch Horizontal Speed", typeof(int), "Extended", null, null, (o) => o.Rotation.X, (o, v) => o.Rotation.X = (int)v),
			new PropertySpec("Maximum Drop Distance", typeof(float), "Extended", null, null, (o) => o.Position.Y, (o, v) => o.Position.Y = (float)v),
			new PropertySpec("Initial Height", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Launch Vertical Speed", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Jumpoff Point", typeof(float), "Extended", null, null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Bungee Vine"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
}