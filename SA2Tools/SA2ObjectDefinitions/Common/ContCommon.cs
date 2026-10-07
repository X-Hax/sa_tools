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

namespace SA2ObjectDefinitions.Common
{
	public abstract class ContCommon : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] meshes;
		protected NJS_TEXLIST texarr;
		protected Texture[] texs;

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(0, item.Rotation.Y - 0x8000, 0);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texs == null)
				texs = ObjectHelper.GetTextures("objtex_common", texarr, dev);
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(0, item.Rotation.Y - 0x8000, 0);
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
			transform.NJRotateObject(0, item.Rotation.Y - 0x8000, 0);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateObject(0, item.Rotation.Y - 0x8000, 0);
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

	public class ContWood : ContCommon
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/OBJECT_CONTWOOD.sa2mdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("object/tls/CONTWOOD.satex");
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Wall Bump", typeof(bool), "Extended", "Determines if the box is treated as a wall that knocks the player back.", null, (o) => Convert.ToBoolean(o.Rotation.X % 2), (o, v) => o.Rotation.X = Convert.ToInt32((bool)v)),
			new PropertySpec("Spawn Animal", typeof(byte), "Extended", "A value of 10 will spawn an animal if the box is destroyed.", null, (o) => (o.Rotation.Z >> 4) & 0xF,
				(o, v) => { o.Rotation.Z &= 0xFF0F; o.Rotation.Z |= ((byte)v) << 4; }),
			new PropertySpec("Spawn Animal Type", typeof(byte), "Extended", "Values correspond to animal sets that are specific to each stage.", null, (o) => o.Rotation.Z & 0xF,
				(o, v) => { o.Rotation.Z &= 0xFFF0; o.Rotation.Z |= (byte)v; }),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Wooden Container"; } }
	}

	public class ContIron : ContCommon
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/OBJECT_CONTIRON.sa2mdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("object/tls/CONTIRON.satex");
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Wall Bump", typeof(bool), "Extended", "Determines if the box is treated as a wall that knocks the player back.", null, (o) => Convert.ToBoolean(o.Rotation.X % 2), (o, v) => o.Rotation.X = Convert.ToInt32((bool)v)),
			new PropertySpec("Spawn Animal", typeof(byte), "Extended", "A value of 10 will spawn an animal if the box is destroyed.", null, (o) => (o.Rotation.Z >> 4) & 0xF,
				(o, v) => { o.Rotation.Z &= 0xFF0F; o.Rotation.Z |= ((byte)v) << 4; }),
			new PropertySpec("Spawn Animal Type", typeof(byte), "Extended", "Values correspond to animal sets that are specific to each stage.", null, (o) => o.Rotation.Z & 0xF,
				(o, v) => { o.Rotation.Z &= 0xFFF0; o.Rotation.Z |= (byte)v; }),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Iron Container"; } }
	}

	public class ContChao : ContCommon
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/OBJECT_CONTCHAO.sa2mdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("object/tls/CONTCHAO.satex");
		}
		public override string Name { get { return "Chao Container"; } }
	}

	public class SolidBox : ContCommon
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/OBJECT_SOLIDBOX.sa2mdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("object/tls/SOLIDBOX.satex");
		}
		public override string Name { get { return "Unbreakable Container"; } }
	}
}