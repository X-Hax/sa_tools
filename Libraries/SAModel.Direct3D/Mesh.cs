using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAModel.Direct3D
{
	public abstract class Mesh
	{
		public abstract void DrawSubset(Device device, int subset);

		public abstract void DrawAll(Device device);

		public abstract HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, Matrix transform, NJS_OBJECT model = null);

		public HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform, NJS_OBJECT model = null) => CheckHit(Near, Far, Viewport, Projection, View, transform.Top, model);

		#region Box
		private static readonly FVF_PositionNormalTextured[] boxverts =
		{
			new FVF_PositionNormalTextured(new Vector3(-0.5f, -0.5f, 0.5f), Vector3.UnitZ, new Vector2(0, 1)),
			new FVF_PositionNormalTextured(new Vector3(0.5f, -0.5f, 0.5f), Vector3.UnitZ, new Vector2(1, 1)),
			new FVF_PositionNormalTextured(new Vector3(-0.5f, 0.5f, 0.5f), Vector3.UnitZ, new Vector2(0, 0)),
			new FVF_PositionNormalTextured(new Vector3(0.5f, 0.5f, 0.5f), Vector3.UnitZ, new Vector2(1, 0))
		};

		private static readonly short[] boxinds = { 0, 1, 2, 1, 3, 2 };

		public static Mesh Box(float width, float height, float depth)
		{
			List<FVF_PositionNormalTextured> verts = new List<FVF_PositionNormalTextured>(4 * 6);
			List<short> inds = new List<short>(6 * 6);
			short off = 0;
			MatrixStack transform = new MatrixStack();
			transform.NJScale(width, height, depth);
			for (int i = 0; i < 0x10000; i += 0x4000)
			{
				transform.Push();
				transform.NJRotateY(i);
				verts.AddRange(boxverts.Select(v => new FVF_PositionNormalTextured(Vector3.TransformCoordinate(v.Position, transform.Top), Vector3.TransformNormal(v.Normal, transform.Top), v.UV)));
				transform.Pop();
				inds.AddRange(boxinds.Select(a => (short)(a + off)));
				off += 4;
			}
			transform.Push();
			transform.NJRotateX(0x4000);
			verts.AddRange(boxverts.Select(v => new FVF_PositionNormalTextured(Vector3.TransformCoordinate(v.Position, transform.Top), Vector3.TransformNormal(v.Normal, transform.Top), v.UV)));
			transform.Pop();
			inds.AddRange(boxinds.Select(a => (short)(a + off)));
			off += 4;
			transform.Push();
			transform.NJRotateX(0xC000);
			verts.AddRange(boxverts.Select(v => new FVF_PositionNormalTextured(Vector3.TransformCoordinate(v.Position, transform.Top), Vector3.TransformNormal(v.Normal, transform.Top), v.UV)));
			transform.Pop();
			inds.AddRange(boxinds.Select(a => (short)(a + off)));
			return new Mesh<FVF_PositionNormalTextured>(verts.ToArray(), new short[][] { inds.ToArray() });
		}

		public static Mesh Box(float width, float height, float depth, params System.Drawing.Color[] colors)
		{
			System.Drawing.Color[] sidecolors = new System.Drawing.Color[6];
			if (colors.Length >= 6)
				Array.Copy(colors, sidecolors, 6);
			else if (colors.Length == 5)
			{
				Array.Copy(colors, sidecolors, 5);
				sidecolors[5] = colors[4];
			}
			else if (colors.Length >= 3)
			{
				sidecolors[0] = sidecolors[2] = colors[0];
				sidecolors[1] = sidecolors[3] = colors[1];
				sidecolors[4] = sidecolors[5] = colors[2];
			}
			else if (colors.Length == 2)
			{
				sidecolors[0] = sidecolors[1] = sidecolors[4] = colors[0];
				sidecolors[2] = sidecolors[3] = sidecolors[5] = colors[1];
			}
			else
				sidecolors[0] = sidecolors[1] = sidecolors[2] = sidecolors[3] = sidecolors[4] = sidecolors[5] = colors[0];
			List<FVF_PositionNormalTexturedColored> verts = new List<FVF_PositionNormalTexturedColored>(4 * 6);
			List<short> inds = new List<short>(6 * 6);
			short off = 0;
			Matrix matrix;
			for (int i = 0; i < 0x10000; i += 0x4000)
			{
				matrix = Matrix.Identity;
				MatrixFunctions.RotateY(ref matrix, i);
				MatrixFunctions.Scale(ref matrix, width, height, depth);
				verts.AddRange(boxverts.Select(v => new FVF_PositionNormalTexturedColored(Vector3.TransformCoordinate(v.Position, matrix), Vector3.TransformNormal(v.Normal, matrix), v.UV, sidecolors[i / 0x4000])));
				inds.AddRange(boxinds.Select(a => (short)(a + off)));
				off += 4;
			}
			matrix = Matrix.Identity;
			MatrixFunctions.RotateX(ref matrix, 0x4000);
			MatrixFunctions.Scale(ref matrix, width, height, depth);
			verts.AddRange(boxverts.Select(v => new FVF_PositionNormalTexturedColored(Vector3.TransformCoordinate(v.Position, matrix), Vector3.TransformNormal(v.Normal, matrix), v.UV, sidecolors[4])));
			inds.AddRange(boxinds.Select(a => (short)(a + off)));
			off += 4;
			matrix = Matrix.Identity;
			MatrixFunctions.RotateX(ref matrix, 0xC000);
			MatrixFunctions.Scale(ref matrix, width, height, depth);
			verts.AddRange(boxverts.Select(v => new FVF_PositionNormalTexturedColored(Vector3.TransformCoordinate(v.Position, matrix), Vector3.TransformNormal(v.Normal, matrix), v.UV, sidecolors[5])));
			inds.AddRange(boxinds.Select(a => (short)(a + off)));
			return new Mesh<FVF_PositionNormalTexturedColored>(verts.ToArray(), new short[][] { inds.ToArray() });
		}

		public static Mesh MultiTextureBox(float width, float height, float depth)
		{
			List<FVF_PositionNormalTextured> verts = new List<FVF_PositionNormalTextured>(4 * 6);
			List<short[]> inds = new List<short[]>(6);
			short off = 0;
			Matrix matrix;
			for (int i = 0; i < 0x10000; i += 0x4000)
			{
				matrix = Matrix.Identity;
				MatrixFunctions.RotateY(ref matrix, i);
				MatrixFunctions.Scale(ref matrix, width, height, depth);
				verts.AddRange(boxverts.Select(v => new FVF_PositionNormalTextured(Vector3.TransformCoordinate(v.Position, matrix), Vector3.TransformNormal(v.Normal, matrix), v.UV)));
				inds.Add(boxinds.Select(a => (short)(a + off)).ToArray());
				off += 4;
			}
			matrix = Matrix.Identity;
			MatrixFunctions.RotateX(ref matrix, 0x4000);
			MatrixFunctions.Scale(ref matrix, width, height, depth);
			verts.AddRange(boxverts.Select(v => new FVF_PositionNormalTextured(Vector3.TransformCoordinate(v.Position, matrix), Vector3.TransformNormal(v.Normal, matrix), v.UV)));
			inds.Add(boxinds.Select(a => (short)(a + off)).ToArray());
			off += 4;
			matrix = Matrix.Identity;
			MatrixFunctions.RotateX(ref matrix, 0xC000);
			MatrixFunctions.Scale(ref matrix, width, height, depth);
			verts.AddRange(boxverts.Select(v => new FVF_PositionNormalTextured(Vector3.TransformCoordinate(v.Position, matrix), Vector3.TransformNormal(v.Normal, matrix), v.UV)));
			inds.Add(boxinds.Select(a => (short)(a + off)).ToArray());
			return new Mesh<FVF_PositionNormalTextured>(verts.ToArray(), inds.ToArray());
		}

		public static Mesh MultiTextureBox(float width, float height, float depth, params System.Drawing.Color[] colors)
		{
			System.Drawing.Color[] sidecolors = new System.Drawing.Color[6];
			if (colors.Length >= 6)
				Array.Copy(colors, sidecolors, 6);
			else if (colors.Length == 5)
			{
				Array.Copy(colors, sidecolors, 5);
				sidecolors[5] = colors[4];
			}
			else if (colors.Length >= 3)
			{
				sidecolors[0] = sidecolors[2] = colors[0];
				sidecolors[1] = sidecolors[3] = colors[1];
				sidecolors[4] = sidecolors[5] = colors[2];
			}
			else if (colors.Length == 2)
			{
				sidecolors[0] = sidecolors[1] = sidecolors[4] = colors[0];
				sidecolors[2] = sidecolors[3] = sidecolors[5] = colors[1];
			}
			else
				sidecolors[0] = sidecolors[1] = sidecolors[2] = sidecolors[3] = sidecolors[4] = sidecolors[5] = colors[0];
			List<FVF_PositionNormalTexturedColored> verts = new List<FVF_PositionNormalTexturedColored>(4 * 6);
			List<short[]> inds = new List<short[]>(6);
			short off = 0;
			Matrix matrix;
			for (int i = 0; i < 0x10000; i += 0x4000)
			{
				matrix = Matrix.Identity;
				MatrixFunctions.RotateY(ref matrix, i);
				MatrixFunctions.Scale(ref matrix, width, height, depth);
				verts.AddRange(boxverts.Select(v => new FVF_PositionNormalTexturedColored(Vector3.TransformCoordinate(v.Position, matrix), Vector3.TransformNormal(v.Normal, matrix), v.UV, sidecolors[i / 0x4000])));
				inds.Add(boxinds.Select(a => (short)(a + off)).ToArray());
				off += 4;
			}
			matrix = Matrix.Identity;
			MatrixFunctions.RotateX(ref matrix, 0x4000);
			MatrixFunctions.Scale(ref matrix, width, height, depth);
			verts.AddRange(boxverts.Select(v => new FVF_PositionNormalTexturedColored(Vector3.TransformCoordinate(v.Position, matrix), Vector3.TransformNormal(v.Normal, matrix), v.UV, sidecolors[4])));
			inds.Add(boxinds.Select(a => (short)(a + off)).ToArray());
			off += 4;
			matrix = Matrix.Identity;
			MatrixFunctions.RotateX(ref matrix, 0xC000);
			MatrixFunctions.Scale(ref matrix, width, height, depth);
			verts.AddRange(boxverts.Select(v => new FVF_PositionNormalTexturedColored(Vector3.TransformCoordinate(v.Position, matrix), Vector3.TransformNormal(v.Normal, matrix), v.UV, sidecolors[5])));
			inds.Add(boxinds.Select(a => (short)(a + off)).ToArray());
			return new Mesh<FVF_PositionNormalTexturedColored>(verts.ToArray(), inds.ToArray());
		}
		#endregion

		#region Sphere
		// http://www.richardssoftware.net/2013/07/shapes-demo-with-direct3d11-and-slimdx.html
		public static Mesh Sphere(float radius, int slices, int stacks)
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
			return new Mesh<FVF_PositionNormalTextured>(verts.ToArray(), new short[][] { inds.ToArray() });
		}

		public static Mesh Sphere(float radius, int slices, int stacks, System.Drawing.Color color)
		{
			List<FVF_PositionNormalTexturedColored> verts = new List<FVF_PositionNormalTexturedColored>();
			verts.Add(new FVF_PositionNormalTexturedColored(new Vector3(0, radius, 0), new Vector3(0, 1, 0), new Vector2(0, 0), color));
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
					verts.Add(new FVF_PositionNormalTexturedColored(p, n, uv, color));
				}
			}
			verts.Add(new FVF_PositionNormalTexturedColored(new Vector3(0, -radius, 0), new Vector3(0, -1, 0), new Vector2(0, 1), color));

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
			return new Mesh<FVF_PositionNormalTexturedColored>(verts.ToArray(), new short[][] { inds.ToArray() });
		}
		#endregion
	}

	public class Mesh<T> : Mesh
		where T : struct, IVertex
	{
		static readonly System.Reflection.ConstructorInfo ctor;
		static Mesh() => ctor = typeof(T).GetConstructor(new[] { typeof(VertexData) });

		protected readonly T[] vertexBuffer;
		private readonly short[][] indexBuffer;

		protected Mesh() { }

		public Mesh(Attach attach)
		{
			indexBuffer = new short[attach.MeshInfo.Length][];
			List<T> vb = new List<T>();
			for (int i = 0; i < attach.MeshInfo.Length; i++)
			{
				int off = vb.Count;
				vb.AddRange(attach.MeshInfo[i].Vertices.Select(v => (T)ctor.Invoke(new object[] { v })));
				ushort[] tris = attach.MeshInfo[i].ToTriangles();
				indexBuffer[i] = tris.Select(t => (short)(t + off)).ToArray();
			}
			vertexBuffer = vb.ToArray();
		}

		public Mesh(T[] verts, short[][] inds)
		{
			vertexBuffer = verts;
			indexBuffer = inds;
		}

		public override void DrawSubset(Device device, int subset)
		{
			device.VertexFormat = vertexBuffer[0].GetFormat();
			device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, vertexBuffer.Length, indexBuffer[subset].Length / 3, indexBuffer[subset], Format.Index16, vertexBuffer);
		}

		public override void DrawAll(Device device)
		{
			for (int i = 0; i < indexBuffer.Length; i++)
				DrawSubset(device, i);
		}

		public override HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, Matrix transform, NJS_OBJECT model = null)
		{
			Vector3 pos = Viewport.Unproject(Near, Projection, View, transform);
			Vector3 dir = Vector3.Normalize(Vector3.Subtract(pos, Viewport.Unproject(Far, Projection, View, transform)));
			Ray ray = new Ray(pos, dir);
			foreach (short[] sub in indexBuffer)
				for (int i = 0; i < sub.Length; i += 3)
				{
					int v1Index = sub[i];
					int v2Index = sub[i + 1];
					int v3Index = sub[i + 2];
					if (v1Index < 0 || v2Index < 0 || v3Index < 0)
						throw new Exception("Vertex buffer overflow detected. Make sure the models use less than 32768 vertices per model.");
					Vector3 v1 = vertexBuffer[v1Index].GetPosition();
					Vector3 v2 = vertexBuffer[v2Index].GetPosition();
					Vector3 v3 = vertexBuffer[v3Index].GetPosition();
					if (Collision.RayIntersectsTriangle(ref ray, ref v1, ref v2, ref v3, out float distance))
					{
						Vector3 norm = Vector3.TransformNormal(Vector3.Normalize(Vector3.Cross(v2 - v1, v3 - v1)), transform);
						return new HitResult(model, distance, ray.Position + (ray.Direction * distance), norm);
					}
				}
			return HitResult.NoHit;
		}
	}

	public interface IWeightedMesh
	{
		void Update(IList<Matrix> matrices);

		void UpdateSelection(int selected);
	}

	public class WeightedMesh<T> : Mesh<T>, IWeightedMesh
		where T : struct, IVertexNormal, IVertexColor
	{
		private readonly List<List<WeightData>> weights;

		internal WeightedMesh(Attach attach, List<List<WeightData>> weights)
			: base(attach)
		{
			this.weights = weights;
		}

		public void Update(IList<Matrix> matrices)
		{
			for (int i = 0; i < weights.Count; i++)
				if (weights[i] != null)
				{
					Vector3 pos = Vector3.Zero;
					Vector3 nor = Vector3.Zero;
					foreach (var weight in weights[i])
					{
						pos += Vector3.TransformCoordinate(weight.Position, matrices[weight.Index]) * weight.Weight;
						nor += Vector3.TransformNormal(weight.Normal, matrices[weight.Index]) * weight.Weight;
					}
					vertexBuffer[i].SetPosition(pos);
					vertexBuffer[i].SetNormal(nor);
				}
		}

		private System.Drawing.Color GetColor(float weight)
		{
			byte[] color1 = new byte[] { 0, 0, 127 };
			byte[] color2 = new byte[] { 251, 64, 0 };
			float t = 0;
			if (weight < 0.25f)
			{
				color2 = new byte[] { 0, 153, 158 };
				t = weight;
			}
			else if (weight < 0.50f)
			{
				color1 = new byte[] { 0, 153, 158 };
				color2 = new byte[] { 0, 190, 20 };
				t = weight - 0.25f;
			}
			else if (weight < 0.75f)
			{
				color1 = new byte[] { 0, 190, 20 };
				color2 = new byte[] { 220, 213, 0 };
				t = weight - 0.5f;
			}
			else
			{
				color1 = new byte[] { 220, 213, 0 };
				t = weight - 0.75f;
			}

			t *= 4;
			for (int i = 0; i < 3; i++)
			{
				color1[i] = (byte)(Math.Round(color1[i] * (1.0f - t)) + Math.Round(color2[i] * t));
			}
			return System.Drawing.Color.FromArgb(color1[0], color1[1], color1[2]);
		}

		public void UpdateSelection(int selected)
		{
			if (selected != -1)
			{
				for (int i = 0; i < weights.Count; i++)
					if (weights[i] != null)
					{
						WeightData weight = weights[i].SingleOrDefault(a => a.Index == selected);
						vertexBuffer[i].SetColor(GetColor(weight?.Weight ?? 0));
					}
					else
						vertexBuffer[i].SetColor(System.Drawing.Color.White);
			}
			else
				for (int i = 0; i < weights.Count; i++)
					vertexBuffer[i].SetColor(System.Drawing.Color.White);
		}
	}
}
