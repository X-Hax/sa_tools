using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;

namespace SADXObjectDefinitions.EmeraldCoast
{
	class OBEWind : ObjectDefinition
	{
		private NJS_MATERIAL material;
		private Texture texture;
		private Mesh mesh;

		public override void Init(ObjectData data, string name, Device dev)
		{
			mesh = Mesh.Box(dev, 0.5f, 0.5f, 0.5f);
			material = new NJS_MATERIAL
			{
				DiffuseColor = Color.FromArgb(127, 178, 178, 178),
				UseAlpha = false
			};
			texture = new Texture(dev, new Bitmap(2, 2), 0, Pool.Managed);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			float scaleX = item.Scale.X;
			float scaleY = item.Scale.Y * 0.5f;
			float scaleZ = item.Scale.Z;
			if (scaleX >= 10.0f)
			{
				if (scaleX > 200.0f)
				{
					scaleX = 200f;
				}
			}
			else
			{
				scaleX = 10f;
			}
			if (scaleY >= 10.0f)
			{
				if (scaleY > 200.0f)
				{
					scaleY = 200f;
				}
			}
			else
			{
				scaleY = 10f;
			}
			if (scaleZ >= 10.0f)
			{
				if (scaleZ > 200.0f)
				{
					scaleZ = 200f;
				}
			}
			else
			{
				scaleZ = 10f;
			}
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(scaleX, scaleY, scaleZ);
			HitResult result = mesh.CheckHit(Near, Far, Viewport, Projection, View, transform);

			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			float scaleX = item.Scale.X;
			float scaleY = item.Scale.Y * 0.5f;
			float scaleZ = item.Scale.Z;
			if (scaleX >= 10.0f)
			{
				if (scaleX > 200.0f)
				{
					scaleX = 200f;
				}
			}
			else
			{
				scaleX = 10f;
			}
			if (scaleY >= 10.0f)
			{
				if (scaleY > 200.0f)
				{
					scaleY = 200f;
				}
			}
			else
			{
				scaleY = 10f;
			}
			if (scaleZ >= 10.0f)
			{
				if (scaleZ > 200.0f)
				{
					scaleZ = 200f;
				}
			}
			else
			{
				scaleZ = 10f;
			}
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(scaleX, scaleY, scaleZ);


			float largestScale = item.Scale.X;
			if (item.Scale.Z > largestScale) largestScale = item.Scale.Z;

			BoundingSphere boxSphere = new BoundingSphere() { Center = new Vertex(item.Position.X, item.Position.Y, item.Position.Z), Radius = largestScale};

			RenderInfo outputInfo = new RenderInfo(mesh, 0, transform.Top, material, texture, FillMode.WireFrame, boxSphere);
			result.Add(outputInfo);

			if (item.Selected)
			{
				RenderInfo highlightInfo = new RenderInfo(mesh, 0, transform.Top, material, texture, FillMode.WireFrame, boxSphere);
				result.Add(highlightInfo);
			}

			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			float scaleX = item.Scale.X;
			float scaleY = item.Scale.Y * 0.5f;
			float scaleZ = item.Scale.Z;
			if (scaleX >= 10.0f)
			{
				if (scaleX > 200.0f)
				{
					scaleX = 200f;
				}
			}
			else
			{
				scaleX = 10f;
			}
			if (scaleY >= 10.0f)
			{
				if (scaleY > 200.0f)
				{
					scaleY = 200f;
				}
			}
			else
			{
				scaleY = 10f;
			}
			if (scaleZ >= 10.0f)
			{
				if (scaleZ > 200.0f)
				{
					scaleZ = 200f;
				}
			}
			else
			{
				scaleZ = 10f;
			}
			float largestScale = scaleX;
			if (scaleY > largestScale) largestScale = scaleY;
			if (scaleZ > largestScale) largestScale = scaleZ;

			BoundingSphere boxSphere = new BoundingSphere() { Center = new Vertex(item.Position.X, item.Position.Y, item.Position.Z), Radius = largestScale };

			return boxSphere;
		}

		private PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Power", typeof(float), "Extended", null, null, (o) => o.Rotation.X, (o, v) => o.Rotation.X = (int)v)
		};

		public override string Name { get { return "Updraft"; } }
	}
}
