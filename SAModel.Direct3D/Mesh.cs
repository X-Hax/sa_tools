using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicRetro.SAModel.Direct3D
{
	public abstract class Mesh
	{
		public abstract void DrawSubset(int subset);

		public abstract HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform, NJS_OBJECT model = null);

		/*public static Mesh Box(Device device, float xscl, float yscl, float zscl)
		{
			throw new NotImplementedException();
		}*/

		/*public static Mesh Sphere(Device device)
		{
			throw new NotImplementedException();
		}*/
	}

	public class Mesh<T> : Mesh
		where T : struct, IVertex
	{
		private readonly Device device;
		private readonly T[] vertexBuffer;
		private readonly short[][] indexBuffer;

		public Mesh(Attach attach, Device device)
		{
			this.device = device;
			indexBuffer = new short[attach.MeshInfo.Length][];
			List<T> vb = new List<T>();
			for (int i = 0; i < attach.MeshInfo.Length; i++)
			{
				int off = vb.Count;
				vb.AddRange(attach.MeshInfo[i].Vertices.Select(v => (T)Activator.CreateInstance(typeof(T), v)));
				ushort[] tris = attach.MeshInfo[i].ToTriangles();
				indexBuffer[i] = tris.Select(t => (short)(t + off)).ToArray();
			}
			vertexBuffer = vb.ToArray();
		}

		public Mesh(Device device, T[] verts, short[][] inds)
		{
			this.device = device;
			vertexBuffer = verts;
			indexBuffer = inds;
		}

		public override void DrawSubset(int subset)
		{
			device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, vertexBuffer.Length, indexBuffer[subset].Length / 3, indexBuffer[subset], Format.Index16, vertexBuffer);
		}

		public override HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform, NJS_OBJECT model = null)
		{
			Vector3 pos = Viewport.Unproject(Near, Projection, View, transform.Top);
			Vector3 dir = Vector3.Subtract(pos, Viewport.Unproject(Far, Projection, View, transform.Top));
			Ray ray = new Ray(pos, dir);
			foreach (short[] sub in indexBuffer)
				for (int i = 0; i < sub.Length; i += 3)
				{
					Vector3 v1 = vertexBuffer[sub[i]].GetPosition();
					Vector3 v2 = vertexBuffer[sub[i + 1]].GetPosition();
					Vector3 v3 = vertexBuffer[sub[i + 2]].GetPosition();
					if (Collision.RayIntersectsTriangle(ref ray, ref v1, ref v2, ref v3, out Vector3 point))
					{
						Vector3 norm = Vector3.TransformNormal(Vector3.Normalize(Vector3.Cross(v2 - v1, v3 - v1)), transform.Top);
						return new HitResult(model, pos.Distance(point), point, norm);
					}
				}
			return HitResult.NoHit;
		}
	}
}
