using SharpDX;
using System.Collections.Generic;
using SharpDX.Direct3D11;

namespace SonicRetro.SAModel.Direct3D
{
	public class RenderInfo
	{
		public Mesh Mesh { get; private set; }
		public int Subset { get; private set; }
		public Matrix Transform { get; private set; }
		public NJS_MATERIAL Material { get; private set; }
		public SceneTexture Texture { get; private set; }
		public FillMode FillMode { get; private set; }
		public BoundingSphere Bounds { get; private set; }

		public RenderInfo(Mesh mesh, int subset, Matrix transform, NJS_MATERIAL material, SceneTexture texture, FillMode fillMode, BoundingSphere bounds)
		{
			Mesh = mesh;
			Subset = subset;
			Transform = transform;
			if (material != null)
				Material = new NJS_MATERIAL(material);
			Texture = texture;
			FillMode = fillMode;
			Bounds = bounds;
		}

		public void Draw(Renderer device)
		{
			FillMode lastFillMode = device.FillMode;
			device.FillMode = FillMode;

			device.SetTransform(TransformState.World, Transform);

			device.SetMaterial(Material);

			if (Material == null || !Material.UseTexture)
			{
				device.SetTexture(0, null);
			}
			else
			{
				device.SetTexture(0, Texture);
			}

			Mesh?.DrawSubset(device, Subset);
			device.FillMode = lastFillMode;
		}

		public static void Draw(IEnumerable<RenderInfo> items, Device device, EditorCamera camera)
		{
			List<KeyValuePair<float, RenderInfo>> drawList = new List<KeyValuePair<float, RenderInfo>>();
			foreach (RenderInfo item in items)
			{
				float dist = camera.Position.Distance(item.Bounds.Center.ToVector3()) + item.Bounds.Radius;
				if (dist > camera.DrawDistance) continue;

				if (item.Material != null && item.Material.UseAlpha)
				{
					bool ins = false;
					for (int i = 0; i < drawList.Count; i++)
					{
						if (drawList[i].Key < dist)
						{
							drawList.Insert(i, new KeyValuePair<float, RenderInfo>(dist, item));
							ins = true;
							break;
						}
					}
					if (!ins)
						drawList.Add(new KeyValuePair<float, RenderInfo>(dist, item));
				}
				else
					drawList.Insert(0, new KeyValuePair<float, RenderInfo>(float.MaxValue, item));
			}
			foreach (KeyValuePair<float, RenderInfo> item in drawList)
				item.Value.Draw(device);
		}
	}
}