using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;
using SplitTools;
using System;

namespace SA2ObjectDefinitions.Common
{
	public abstract class PicBase : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] meshes;
		protected NJS_TEXLIST texarr;
		protected Texture[] texs;
		protected List<string> texpacks = [];

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
			transform.NJRotateY(item.Rotation.Y);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}
		
		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texs == null)
				texs = ObjectHelper.GetTextures(texpacks, texarr, dev);
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
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
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}
		
		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
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

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
	public class MetalHarborPic : PicBase
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg10_metalharbor/models/GC/Helmet.sa2bmdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("stg10_metalharbor/tls/Helmet.satex");
			texpacks.Add("objtex_stg10");
			texpacks.Add("landtx10");
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Animal Group", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Hard Hat"; } }
	}
	public class CityEscapePic : PicBase
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg13_CityEscape/models/GC/PIC.sa2bmdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("stg13_CityEscape/tls/PIC.satex");
			texpacks.Add("objtex_stg13");
			texpacks.Add("landtx13");
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Animal Group", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Small Tire"; } }
	}
	public class GreenForestPic : PicBase
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg03_Jungle/models/GC/Mushroom.sa2bmdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("stg03_Jungle/tls/Mushroom.satex");
			texpacks.Add("objtex_stg03");
			texpacks.Add("landtx03");
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Animal Group", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Mushroom"; } }
	}
}