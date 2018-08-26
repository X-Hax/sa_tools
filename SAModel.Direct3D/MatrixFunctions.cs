using SharpDX;
using System.Collections.Generic;

namespace SonicRetro.SAModel.Direct3D
{
	public static class MatrixFunctions
	{
		public static void Translate(ref Matrix matrix, float x, float y, float z)
		{
			matrix.M41 = z * matrix.M31 + y * matrix.M21 + x * matrix.M11 + matrix.M41;
			matrix.M42 = z * matrix.M32 + y * matrix.M22 + x * matrix.M12 + matrix.M42;
			matrix.M43 = z * matrix.M33 + y * matrix.M23 + x * matrix.M13 + matrix.M43;
			matrix.M44 = z * matrix.M34 + y * matrix.M24 + x * matrix.M14 + matrix.M44;
		}

		public static void Translate(ref Matrix matrix, Vertex vertex)
		{
			Translate(ref matrix, vertex.X, vertex.Y, vertex.Z);
		}

		public static void Translate(ref Matrix matrix, Vector3 vector)
		{
			Translate(ref matrix, vector.X, vector.Y, vector.Z);
		}

		public static void RotateX(ref Matrix matrix, int x)
		{
			float v24 = Extensions.BAMSSin(x);
			float v17 = Extensions.BAMSSinInv(x);
			float v18 = matrix.M21;
			float v19 = matrix.M22;
			float v21 = matrix.M23;
			float v20 = matrix.M24;
			matrix.M21 = v24 * matrix.M31 + v18 * v17;
			matrix.M22 = v24 * matrix.M32 + v19 * v17;
			matrix.M23 = v21 * v17 + v24 * matrix.M33;
			matrix.M24 = v24 * matrix.M34 + v20 * v17;
			matrix.M31 = v17 * matrix.M31 - v18 * v24;
			matrix.M32 = v17 * matrix.M32 - v19 * v24;
			matrix.M33 = v17 * matrix.M33 - v21 * v24;
			matrix.M34 = v17 * matrix.M34 - v20 * v24;
		}

		public static void RotateY(ref Matrix matrix, int y)
		{
			float v22 = Extensions.BAMSSin(y);
			float v7 = Extensions.BAMSSinInv(y);
			float v8 = matrix.M11;
			float v9 = matrix.M12;
			float v10 = matrix.M13;
			float v11 = matrix.M14;
			matrix.M11 = matrix.M11 * v7 - v22 * matrix.M31;
			matrix.M12 = v9 * v7 - v22 * matrix.M32;
			matrix.M13 = v10 * v7 - v22 * matrix.M33;
			matrix.M14 = v11 * v7 - v22 * matrix.M34;
			matrix.M31 = v7 * matrix.M31 + v8 * v22;
			matrix.M32 = v9 * v22 + v7 * matrix.M32;
			matrix.M33 = v7 * matrix.M33 + v10 * v22;
			matrix.M34 = v11 * v22 + v7 * matrix.M34;
		}

		public static void RotateZ(ref Matrix matrix, int z)
		{
			float v22 = Extensions.BAMSSin(z);
			float v7 = Extensions.BAMSSinInv(z);
			float v8 = matrix.M11;
			float v9 = matrix.M12;
			float v10 = matrix.M13;
			float v11 = matrix.M14;
			matrix.M11 = v22 * matrix.M21 + v8 * v7;
			matrix.M12 = v22 * matrix.M22 + v9 * v7;
			matrix.M13 = v22 * matrix.M23 + v10 * v7;
			matrix.M14 = v22 * matrix.M24 + v11 * v7;
			matrix.M21 = v7 * matrix.M21 - v8 * v22;
			matrix.M22 = v7 * matrix.M22 - v9 * v22;
			matrix.M23 = v7 * matrix.M23 - v10 * v22;
			matrix.M24 = v7 * matrix.M24 - v11 * v22;
		}

		public static void RotateXYZ(ref Matrix matrix, int x, int y, int z)
		{
			if (z != 0)
				RotateZ(ref matrix, z);
			if (y != 0)
				RotateY(ref matrix, y);
			if (x != 0)
				RotateX(ref matrix, x);
		}

		public static void RotateXYZ(ref Matrix matrix, Rotation rotation)
		{
			RotateXYZ(ref matrix, rotation.X, rotation.Y, rotation.Z);
		}

		public static void RotateZYX(ref Matrix matrix, int x, int y, int z)
		{
			if (y != 0)
				RotateY(ref matrix, y);
			if (x != 0)
				RotateX(ref matrix, x);
			if (z != 0)
				RotateZ(ref matrix, z);
		}

		public static void RotateZYX(ref Matrix matrix, Rotation rotation)
		{
			RotateZYX(ref matrix, rotation.X, rotation.Y, rotation.Z);
		}

		public static void RotateObject(ref Matrix matrix, int x, int y, int z)
		{
			if (z != 0)
				RotateZ(ref matrix, z);
			if (x != 0)
				RotateX(ref matrix, x);
			if (y != 0)
				RotateY(ref matrix, y);
		}

		public static void RotateObject(ref Matrix matrix, Rotation rotation)
		{
			RotateObject(ref matrix, rotation.X, rotation.Y, rotation.Z);
		}

		public static void Scale(ref Matrix matrix, float x, float y, float z)
		{
			matrix.M11 *= x;
			matrix.M12 *= x;
			matrix.M13 *= x;
			matrix.M14 *= x;
			matrix.M21 *= y;
			matrix.M22 *= y;
			matrix.M23 *= y;
			matrix.M24 *= y;
			matrix.M31 *= z;
			matrix.M32 *= z;
			matrix.M33 *= z;
			matrix.M34 *= z;
		}

		public static void Scale(ref Matrix matrix, Vertex vertex)
		{
			Scale(ref matrix, vertex.X, vertex.Y, vertex.Z);
		}

		public static void Scale(ref Matrix matrix, Vector3 vector)
		{
			Scale(ref matrix, vector.X, vector.Y, vector.Z);
		}
	}

	public class MatrixStack
	{
		private Stack<Matrix> matrices;

		public MatrixStack()
		{
			matrices = new Stack<Matrix>();
			matrices.Push(Matrix.Identity);
		}

		public Matrix Top => matrices.Peek();

		public void Push() => matrices.Push(Top);

		public void Pop()
		{
			if (matrices.Count > 1)
				matrices.Pop();
		}

		public void LoadMatrix(Matrix m) { matrices.Pop(); matrices.Push(m); }

		public void TranslateLocal(Vector3 vector) => LoadMatrix(Top * Matrix.Translation(vector));

		public void TranslateLocal(float x, float y, float z) => LoadMatrix(Top * Matrix.Translation(x, y, z));

		public void ScaleLocal(Vector3 vector) => LoadMatrix(Top * Matrix.Scaling(vector));

		public void ScaleLocal(float x, float y, float z) => LoadMatrix(Top * Matrix.Scaling(x, y, z));

		public void NJTranslate(float x, float y, float z)
		{
			Matrix m = Top;
			MatrixFunctions.Translate(ref m, x, y, z);
			LoadMatrix(m);
		}

		public void NJTranslate(Vertex vertex)
		{
			NJTranslate(vertex.X, vertex.Y, vertex.Z);
		}

		public void NJTranslate(Vector3 vector)
		{
			NJTranslate(vector.X, vector.Y, vector.Z);
		}

		public void NJRotateX(int x)
		{
			Matrix m = Top;
			MatrixFunctions.RotateX(ref m, x);
			LoadMatrix(m);
		}

		public void NJRotateY(int y)
		{
			Matrix m = Top;
			MatrixFunctions.RotateY(ref m, y);
			LoadMatrix(m);
		}

		public void NJRotateZ(int z)
		{
			Matrix m = Top;
			MatrixFunctions.RotateZ(ref m, z);
			LoadMatrix(m);
		}

		public void NJRotateXYZ(int x, int y, int z)
		{
			Matrix m = Top;
			MatrixFunctions.RotateXYZ(ref m, x, y, z);
			LoadMatrix(m);
		}

		public void NJRotateXYZ(Rotation rotation)
		{
			NJRotateXYZ(rotation.X, rotation.Y, rotation.Z);
		}

		public void NJRotateZYX(int x, int y, int z)
		{
			Matrix m = Top;
			MatrixFunctions.RotateZYX(ref m, x, y, z);
			LoadMatrix(m);
		}

		public void NJRotateZYX(Rotation rotation)
		{
			NJRotateZYX(rotation.X, rotation.Y, rotation.Z);
		}

		public void NJRotateObject(int x, int y, int z)
		{
			Matrix m = Top;
			MatrixFunctions.RotateObject(ref m, x, y, z);
			LoadMatrix(m);
		}

		public void NJRotateObject(Rotation rotation) => NJRotateObject(rotation.X, rotation.Y, rotation.Z);

		public void NJScale(float x, float y, float z)
		{
			Matrix m = Top;
			MatrixFunctions.Scale(ref m, x, y, z);
			LoadMatrix(m);
		}

		public void NJScale(Vertex vertex) => NJScale(vertex.X, vertex.Y, vertex.Z);

		public void NJScale(Vector3 vector) => NJScale(vector.X, vector.Y, vector.Z);
	}
}