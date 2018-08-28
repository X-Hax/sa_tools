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

		public abstract void DrawAll();

		public abstract HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform, NJS_OBJECT model = null);

		#region Box
		private static readonly FVF_PositionNormalTextured[] boxverts =
		{
			new FVF_PositionNormalTextured(new Vector3(-0.5f, -0.5f, 0.5f), Vector3.UnitZ, new Vector2(0, 0)),
			new FVF_PositionNormalTextured(new Vector3(0.5f, -0.5f, 0.5f), Vector3.UnitZ, new Vector2(1, 0)),
			new FVF_PositionNormalTextured(new Vector3(-0.5f, 0.5f, 0.5f), Vector3.UnitZ, new Vector2(0, 1)),
			new FVF_PositionNormalTextured(new Vector3(0.5f, 0.5f, 0.5f), Vector3.UnitZ, new Vector2(1, 1))
		};

		private static readonly short[] boxinds = { 0, 1, 2, 1, 3, 2 };

		public static Mesh Box(Device device, float width, float height, float depth)
		{
			List<FVF_PositionNormalTextured> verts = new List<FVF_PositionNormalTextured>(4 * 6);
			List<short> inds = new List<short>(6 * 6);
			short off = 0;
			Matrix matrix;
			for (int i = 0; i < 0x10000; i += 0x4000)
			{
				matrix = Matrix.Identity;
				MatrixFunctions.RotateY(ref matrix, i);
				MatrixFunctions.Scale(ref matrix, width, height, depth);
				verts.AddRange(boxverts.Select(v => new FVF_PositionNormalTextured(Vector3.TransformCoordinate(v.Position, matrix), Vector3.TransformNormal(v.Normal, matrix), v.UV)));
				inds.AddRange(boxinds.Select(a => (short)(a + off)));
				off += 4;
			}
			matrix = Matrix.Identity;
			MatrixFunctions.RotateX(ref matrix, 0x4000);
			MatrixFunctions.Scale(ref matrix, width, height, depth);
			verts.AddRange(boxverts.Select(v => new FVF_PositionNormalTextured(Vector3.TransformCoordinate(v.Position, matrix), Vector3.TransformNormal(v.Normal, matrix), v.UV)));
			inds.AddRange(boxinds.Select(a => (short)(a + off)));
			off += 4;
			matrix = Matrix.Identity;
			MatrixFunctions.RotateX(ref matrix, 0xC000);
			MatrixFunctions.Scale(ref matrix, width, height, depth);
			verts.AddRange(boxverts.Select(v => new FVF_PositionNormalTextured(Vector3.TransformCoordinate(v.Position, matrix), Vector3.TransformNormal(v.Normal, matrix), v.UV)));
			inds.AddRange(boxinds.Select(a => (short)(a + off)));
			return new Mesh<FVF_PositionNormalTextured>(device, verts.ToArray(), new short[][] { inds.ToArray() });
		}
		#endregion

		#region Sphere
		// http://www.richardssoftware.net/2013/07/shapes-demo-with-direct3d11-and-slimdx.html
		public static Mesh Sphere(Device device, float radius, int slices, int stacks)
		{
			List<FVF_PositionNormalTextured> verts = new List<FVF_PositionNormalTextured>();
			verts.Add(new FVF_PositionNormalTextured(new Vector3(0, radius, 0), new Vector3(0, 1, 0), new Vector2(0, 0)));
			var phiStep = Math.PI / stacks;
			var thetaStep = 2.0f * Math.PI / slices;

			for (int i = 1; i < stacks; i++)
			{
				var phi = i * phiStep;
				for (int j = 0; j <= slices; j++)
				{
					var theta = j * thetaStep;
					var p = new Vector3((float)(radius * Math.Sin(phi) * Math.Cos(theta)), (float)(radius * Math.Cos(phi)), (float)(radius * Math.Sin(phi) * Math.Sin(theta)));

					var n = p;
					n.Normalize();
					var uv = new Vector2((float)(theta / (Math.PI * 2)), (float)(phi / Math.PI));
					verts.Add(new FVF_PositionNormalTextured(p, n, uv));
				}
			}
			verts.Add(new FVF_PositionNormalTextured(new Vector3(0, -radius, 0), new Vector3(0, -1, 0), new Vector2(0, 1)));

			List<short> inds = new List<short>();
			for (short i = 1; i <= slices; i++)
			{
				inds.Add(0);
				inds.Add((short)(i + 1));
				inds.Add(i);
			}
			var baseIndex = 1;
			var ringVertexCount = slices + 1;
			for (int i = 0; i < stacks - 2; i++)
			{
				for (int j = 0; j < slices; j++)
				{
					inds.Add((short)(baseIndex + i * ringVertexCount + j));
					inds.Add((short)(baseIndex + i * ringVertexCount + j + 1));
					inds.Add((short)(baseIndex + (i + 1) * ringVertexCount + j));

					inds.Add((short)(baseIndex + (i + 1) * ringVertexCount + j));
					inds.Add((short)(baseIndex + i * ringVertexCount + j + 1));
					inds.Add((short)(baseIndex + (i + 1) * ringVertexCount + j + 1));
				}
			}
			short southPoleIndex = (short)(verts.Count - 1);
			baseIndex = southPoleIndex - ringVertexCount;
			for (int i = 0; i < slices; i++)
			{
				inds.Add(southPoleIndex);
				inds.Add((short)(baseIndex + i));
				inds.Add((short)(baseIndex + i + 1));
			}
			return new Mesh<FVF_PositionNormalTextured>(device, verts.ToArray(), new short[][] { inds.ToArray() });
		}
		#endregion
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
			device.VertexFormat = vertexBuffer[0].GetFormat();
			device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, vertexBuffer.Length, indexBuffer[subset].Length / 3, indexBuffer[subset], Format.Index16, vertexBuffer);
		}

		public override void DrawAll()
		{
			for (int i = 0; i < indexBuffer.Length; i++)
				DrawSubset(i);
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
