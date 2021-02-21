using SharpDX;
using SharpDX.Direct3D9;
using System.Collections.Generic;

namespace SonicRetro.SAModel.Direct3D
{
	public class RenderInfo
	{
		public Mesh Mesh { get; private set; }
		public int Subset { get; private set; }
		public Matrix Transform { get; private set; }
		public NJS_MATERIAL Material { get; private set; }
		public Texture Texture { get; private set; }
		public FillMode FillMode { get; private set; }
		public BoundingSphere Bounds { get; private set; }

		public RenderInfo(Mesh mesh, int subset, Matrix transform, NJS_MATERIAL material, Texture texture, FillMode fillMode, BoundingSphere bounds)
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

		public void Draw(Device device)
		{
			FillMode mode = device.GetRenderState<FillMode>(RenderState.FillMode);
			TextureFilter magfilter = device.GetSamplerState<TextureFilter>(0, SamplerState.MagFilter);
			TextureFilter minfilter = device.GetSamplerState<TextureFilter>(0, SamplerState.MinFilter);
			TextureFilter mipfilter = device.GetSamplerState<TextureFilter>(0, SamplerState.MipFilter);

			Material.SetDeviceStates(device, Texture, Transform, FillMode);

			if (Mesh != null)
				Mesh.DrawSubset(device, Subset);
			device.SetRenderState(RenderState.Ambient, System.Drawing.Color.Black.ToArgb());
			device.SetRenderState(RenderState.FillMode, mode);
			device.SetSamplerState(0, SamplerState.MagFilter, magfilter);
			device.SetSamplerState(0, SamplerState.MinFilter, minfilter);
			device.SetSamplerState(0, SamplerState.MipFilter, mipfilter);
		}

		public static List<RenderInfo> Queue(IEnumerable<RenderInfo> items, EditorCamera camera)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			foreach (RenderInfo item in items)
			{
				float dist = Extensions.Distance(camera.Position, item.Bounds.Center.ToVector3()) + item.Bounds.Radius;
				if (dist > camera.DrawDistance) continue;
				result.Add(item);
			}
			return result;
		}

		public static void Draw(IEnumerable<RenderInfo> items, Device device, EditorCamera camera, bool useOldSorting = false)
		{
			if (useOldSorting)
			{
				List<KeyValuePair<float, RenderInfo>> drawList = new List<KeyValuePair<float, RenderInfo>>();
				foreach (RenderInfo item in items)
				{
					float dist = Extensions.Distance(camera.Position, item.Bounds.Center.ToVector3()) + item.Bounds.Radius;
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
			else
			{
				List<KeyValuePair<float, RenderInfo>> drawListTransparent = new List<KeyValuePair<float, RenderInfo>>();
				List<RenderInfo> drawListOpaque = new List<RenderInfo>();
				foreach (RenderInfo item in items)
				{
					// Don't draw if too far
					float dist = Extensions.Distance(camera.Position, item.Bounds.Center.ToVector3()) - item.Bounds.Radius;
					if (dist > camera.DrawDistance) continue;

					// Split into transparent and opaque lists
					if (item.Material != null && item.Material.UseAlpha)
					{
						drawListTransparent.Add(new KeyValuePair<float, RenderInfo>(dist, item));
					}
					else
						drawListOpaque.Add(item);
				}

				// Sort transparent meshes
				drawListTransparent.Sort((y, x) => x.Key.CompareTo(y.Key));

				// Draw opaque meshes
				for (int i = drawListOpaque.Count - 1; i >= 0; i--)
				{
					drawListOpaque[i].Draw(device);
				}

				// Draw transparent meshes			
				foreach (KeyValuePair<float, RenderInfo> item in drawListTransparent)
					item.Value.Draw(device);
			}
		}
	}
}