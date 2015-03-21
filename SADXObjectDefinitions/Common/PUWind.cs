using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using Material = SonicRetro.SAModel.Material;
using Mesh = Microsoft.DirectX.Direct3D.Mesh;

namespace SADXObjectDefinitions.Common
{
	class PUWind : ObjectDefinition
	{
		Material material;
		Texture texture;
		Mesh mesh;

		public override void Init(ObjectData data, string name, Device dev)
		{
			mesh = Mesh.Box(dev, 1f, 1f, 1f);
			material = new Material();
			material.DiffuseColor = Color.FromArgb(180, 180, 180, 180);
			material.UseAlpha = true;
			texture = new Texture(dev, new Bitmap(2, 2), 0, Pool.Managed);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y - 0x5772);
			transform.NJScale((item.Scale.X), (item.Scale.Y), (item.Scale.Z));
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
			transform.NJScale((item.Scale.X), (item.Scale.Y), (item.Scale.Z));

			float largestScale = item.Scale.X;
			if (item.Scale.Y > largestScale) largestScale = item.Scale.Y;
			if (item.Scale.Z > largestScale) largestScale = item.Scale.Z;

			BoundingSphere boxSphere = new BoundingSphere() { Center = new Vertex(item.Position.X, item.Position.Y, item.Position.Z), Radius = (1.5f * largestScale) };

			RenderInfo outputInfo = new RenderInfo(mesh, 0, transform.Top, material, texture, FillMode.Solid, boxSphere);
			result.Add(outputInfo);

			if (item.Selected)
			{
				RenderInfo highlightInfo = new RenderInfo(mesh, 0, transform.Top, material, texture, FillMode.WireFrame, boxSphere);
				result.Add(highlightInfo);
			}

			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item, Object model=null)
		{
			float largestScale = (item.Scale.X + 10) / 5f;
			if (item.Scale.Y > largestScale) largestScale = (item.Scale.Y + 10) / 5f;
			if (item.Scale.Z > largestScale) largestScale = (item.Scale.Z + 10) / 5f;

			BoundingSphere boxSphere = new BoundingSphere() { Center = new Vertex(item.Position.X, item.Position.Y, item.Position.Z), Radius = largestScale };

			return boxSphere;
		}

		public override string Name { get { return "Player-Up Wind"; } }
	}
}
