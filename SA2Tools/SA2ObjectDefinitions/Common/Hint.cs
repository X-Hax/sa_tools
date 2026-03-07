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
	public abstract class Hint : ObjectDefinition
	{
		protected NJS_OBJECT monitor;
		protected Mesh[] meshesMonitor;
		protected NJS_TEXLIST texarrMonitor;
		protected Texture[] texsMonitor;
		
		protected NJS_OBJECT screen;
		protected Mesh[] meshesScreen;
		protected NJS_TEXLIST texarrScreen;
		protected Texture[] texsScreen;

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y, item.Rotation.Z);
			HitResult result = monitor.CheckHit(Near, Far, Viewport, Projection, View, transform, meshesMonitor);
			transform.Pop();
			return result;
		}
		
		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texsMonitor == null)
				texsMonitor = ObjectHelper.GetTextures("objtex_common", texarrMonitor, dev);
			if (texsScreen == null)
				texsScreen = ObjectHelper.GetTextures("objtex_common", texarrScreen, dev);
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y, item.Rotation.Z);
			result.AddRange(monitor.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texsMonitor, meshesMonitor, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result.AddRange(screen.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texsScreen, meshesScreen, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(monitor.DrawModelTreeInvert(transform, meshesMonitor));
				result.AddRange(screen.DrawModelTreeInvert(transform, meshesScreen));
			}
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y, item.Rotation.Z);
			result.Add(new ModelTransform(monitor, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y, item.Rotation.Z);
			return ObjectHelper.GetModelBounds(monitor, transform);
		}
		
		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation.X, item.Rotation.Y, item.Rotation.Z);

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
	
	public class SchBox : Hint
	{
		public override void Init(ObjectData data, string name)
		{
			monitor = ObjectHelper.LoadModel("object/OBJECT_SCHBOX.sa2mdl");
			meshesMonitor = ObjectHelper.GetMeshes(monitor);
			texarrMonitor = NJS_TEXLIST.Load("object/tls/SCHBOX.satex");

			screen = ObjectHelper.LoadModel("object/OBJECT_SCHBOX_SCREEN.sa2mdl");
			meshesScreen = ObjectHelper.GetMeshes(screen);
			texarrScreen = NJS_TEXLIST.Load("object/tls/SCHBOX_SCREEN.satex");
		}
		
		public override string Name { get { return "Hint Monitor"; } }
	}
	
	public class HintBox : Hint
	{
		public override void Init(ObjectData data, string name)
		{
			monitor = ObjectHelper.LoadModel("object/OBJECT_HINTBOX.sa2mdl");
			meshesMonitor = ObjectHelper.GetMeshes(monitor);
			texarrMonitor = NJS_TEXLIST.Load("object/tls/HINTBOX.satex");

			screen = ObjectHelper.LoadModel("object/OBJECT_HINTBOX_SCREEN.sa2mdl");
			meshesScreen = ObjectHelper.GetMeshes(screen);
			texarrScreen = NJS_TEXLIST.Load("object/tls/HINTBOX_SCREEN.satex");
		}
		
		public override string Name { get { return "SA1 Hint Monitor"; } }
	}
	
	public class HidSchBox : Hint
	{
		public override void Init(ObjectData data, string name)
		{
			monitor = ObjectHelper.LoadModel("object/OBJECT_SCHBOX.sa2mdl");
			meshesMonitor = ObjectHelper.GetMeshes(monitor);
			texarrMonitor = NJS_TEXLIST.Load("object/tls/SCHBOX.satex");

			screen = ObjectHelper.LoadModel("object/OBJECT_SCHBOX_SCREEN.sa2mdl");
			meshesScreen = ObjectHelper.GetMeshes(screen);
			texarrScreen = NJS_TEXLIST.Load("object/tls/SCHBOX_SCREEN.satex");
		}
		
		public override string Name { get { return "Hint Monitor (Hidden)"; } }
	}
}