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

namespace SA2ObjectDefinitions.Common
{
	public abstract class Boost : ObjectDefinition
	{
		protected NJS_OBJECT conveyer;
		protected Mesh[] meshesConv;
		protected NJS_TEXLIST texarrConv;
		protected Texture[] texsConv;
		
		protected NJS_OBJECT panelbase;
		protected Mesh[] meshesBase;
		protected NJS_TEXLIST texarrBase;
		protected Texture[] texsBase;

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Speed", typeof(float), "Extended", null, 14.0f, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v > 0 ? (float)v : 14.0f),
			new PropertySpec("Disable Timer", typeof(float), "Extended", null, 60.0f, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v > 0 ? (float)v : 60.0f)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
	
	public class DashPanel : Boost
	{
		public override void Init(ObjectData data, string name)
		{
			conveyer = ObjectHelper.LoadModel("object/OBJECT_KASOKU_PANEL.sa2mdl");
			meshesConv = ObjectHelper.GetMeshes(conveyer);
			texarrConv = NJS_TEXLIST.Load("object/tls/KASOKU_PANEL.satex");

			panelbase = ObjectHelper.LoadModel("object/OBJECT_KASOKU.sa2mdl");
			meshesBase = ObjectHelper.GetMeshes(panelbase);
			texarrBase = NJS_TEXLIST.Load("object/tls/KASOKU.satex");
		}
		
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);
			HitResult result = conveyer.CheckHit(Near, Far, Viewport, Projection, View, transform, meshesConv);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texsConv == null)
				texsConv = ObjectHelper.GetTextures("objtex_common", texarrConv, dev);
			if (texsBase == null)
				texsBase = ObjectHelper.GetTextures("objtex_common", texarrBase, dev);
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);
			result.AddRange(conveyer.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texsConv, meshesConv, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result.AddRange(panelbase.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texsBase, meshesBase, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(conveyer.DrawModelTreeInvert(transform, meshesConv));
				result.AddRange(panelbase.DrawModelTreeInvert(transform, meshesBase));
			}
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);
			result.Add(new ModelTransform(conveyer, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);
			return ObjectHelper.GetModelBounds(conveyer, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);

			return matrix;
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}
		
		public override string Name { get { return "Dash Panel"; } }
	}
	
	public class LaunchPanel : Boost
	{
		public override void Init(ObjectData data, string name)
		{
			conveyer = ObjectHelper.LoadModel("object/OBJECT_BIGJUMP_PANEL.sa2mdl");
			meshesConv = ObjectHelper.GetMeshes(conveyer);
			texarrConv = NJS_TEXLIST.Load("object/tls/BIGJUMP_PANEL.satex");

			panelbase = ObjectHelper.LoadModel("object/OBJECT_BIGJUMP.sa2mdl");
			meshesBase = ObjectHelper.GetMeshes(panelbase);
			texarrBase = NJS_TEXLIST.Load("object/tls/BIGJUMP.satex");
		}
		
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateZYX(item.Rotation);
			HitResult result = conveyer.CheckHit(Near, Far, Viewport, Projection, View, transform, meshesConv);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texsConv == null)
				texsConv = ObjectHelper.GetTextures("objtex_common", texarrConv, dev);
			if (texsBase == null)
				texsBase = ObjectHelper.GetTextures("objtex_common", texarrBase, dev);
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateZYX(item.Rotation);
			result.AddRange(conveyer.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texsConv, meshesConv, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result.AddRange(panelbase.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texsBase, meshesBase, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(conveyer.DrawModelTreeInvert(transform, meshesConv));
				result.AddRange(panelbase.DrawModelTreeInvert(transform, meshesBase));
			}
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateZYX(item.Rotation);
			result.Add(new ModelTransform(conveyer, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateZYX(item.Rotation);
			return ObjectHelper.GetModelBounds(conveyer, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation);

			return matrix;
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Horizontal Speed", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Disable Timer (Unused)", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Added Vertical Speed", typeof(float), "Extended", null, 3.2f, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Launch Panel"; } }
	}
}