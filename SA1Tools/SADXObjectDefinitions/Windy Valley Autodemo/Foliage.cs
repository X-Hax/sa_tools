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
	public abstract class Foliage : ObjectDefinition
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
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_WINDY"), meshes));
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

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation);

			return matrix;
		}
	}

	public class Grass1 : Foliage
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_grass1.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Short Blowing Grass"; } }
	}

	public class Grass2 : Foliage
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_grass2.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Long Blowing Grass"; } }
	}

	public class Grass3 : Foliage
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_grass3.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Large Hanging Leaves"; } }
	}

	public class Grass4 : Foliage
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_grass4.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Small Hanging Leaves"; } }
	}

	public class WKusa1 : Foliage
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_weed1.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Windy Weed"; } }
	}

	public class Flower0 : Foliage
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_flower0.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "White Flowers"; } }
	}

	public class Flower1 : Foliage
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_flower1.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Yellow Flowers"; } }
	}

	public class Tanpopo : Foliage
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_popo.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Dandelion"; } }
	}

	public class WKi1 : Foliage
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_tree1.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Blowing Tree (Sideways)"; } }
	}

	public class WKi2 : Foliage
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_tree2.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Blowing Tree (Standing)"; } }
	}

	public class Green0 : Foliage
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_green0.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Standing Grass"; } }
	}

	public class PinkF : Foliage
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_pinkflower.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Pink Flowers"; } }
	}
}