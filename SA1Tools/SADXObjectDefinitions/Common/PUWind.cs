using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Color = System.Drawing.Color;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.Common
{
	class PUWind : ObjectDefinition
	{
		NJS_MATERIAL material;
		Mesh mesh;

		public override void Init(ObjectData data, string name)
		{
			mesh = Mesh.Box(1f, 1f, 1f);
			material = new NJS_MATERIAL
			{
				DiffuseColor = Color.FromArgb(180, 180, 180, 180),
				UseAlpha = true
			};
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y - 0x5772);
			transform.NJScale(item.Scale.X, item.Scale.Y, item.Scale.Z);
			HitResult result = mesh.CheckHit(Near, Far, Viewport, Projection, View, transform);

			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y - 0x5772);
			transform.NJScale(item.Scale.X, item.Scale.Y, item.Scale.Z);

			float largestScale = item.Scale.X;
			if (item.Scale.Y > largestScale) largestScale = item.Scale.Y;
			if (item.Scale.Z > largestScale) largestScale = item.Scale.Z;

			BoundingSphere boxSphere = new BoundingSphere() { Center = new Vertex(item.Position.X, item.Position.Y, item.Position.Z), Radius = (1.5f * largestScale) };

			RenderInfo outputInfo = new RenderInfo(mesh, 0, transform.Top, material, null, FillMode.Solid, boxSphere);
			result.Add(outputInfo);

			if (item.Selected)
			{
				RenderInfo highlightInfo = new RenderInfo(mesh, 0, transform.Top, material, null, FillMode.Wireframe, boxSphere);
				result.Add(highlightInfo);
			}

			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			return new List<ModelTransform>();
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y - 0x5772);

			return matrix;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			float largestScale = item.Scale.X;
			if (item.Scale.Y > largestScale) largestScale = item.Scale.Y;
			if (item.Scale.Z > largestScale) largestScale = item.Scale.Z;

			BoundingSphere boxSphere = new BoundingSphere() { Center = new Vertex(item.Position.X, item.Position.Y, item.Position.Z), Radius = (1.5f * largestScale) };

			return boxSphere;
		}

		public override string Name { get { return "Player-Up Wind"; } }
	}
}
