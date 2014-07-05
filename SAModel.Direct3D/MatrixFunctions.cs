using Microsoft.DirectX;

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

		public static void NJTranslate(this MatrixStack transform, float x, float y, float z)
		{
			Matrix m = transform.Top;
			Translate(ref m, x, y, z);
			transform.LoadMatrix(m);
		}

		public static void NJTranslate(this MatrixStack transform, Vertex vertex)
		{
			transform.NJTranslate(vertex.X, vertex.Y, vertex.Z);
		}

		public static void NJTranslate(this MatrixStack transform, Vector3 vector)
		{
			transform.NJTranslate(vector.X, vector.Y, vector.Z);
		}

		public static void RotateX(ref Matrix matrix, int x)
		{
			float v24 = Extensions.BAMSToFloat(x);
			float v17 = Extensions.BAMSToFloatInv(x);
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

		public static void NJRotateX(this MatrixStack transform, int x)
		{
			Matrix m = transform.Top;
			RotateX(ref m, x);
			transform.LoadMatrix(m);
		}

		public static void RotateY(ref Matrix matrix, int y)
		{
			float v22 = Extensions.BAMSToFloat(y);
			float v7 = Extensions.BAMSToFloatInv(y);
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

		public static void NJRotateY(this MatrixStack transform, int y)
		{
			Matrix m = transform.Top;
			RotateY(ref m, y);
			transform.LoadMatrix(m);
		}

		public static void RotateZ(ref Matrix matrix, int z)
		{
			float v22 = Extensions.BAMSToFloat(z);
			float v7 = Extensions.BAMSToFloatInv(z);
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

		public static void NJRotateZ(this MatrixStack transform, int z)
		{
			Matrix m = transform.Top;
			RotateZ(ref m, z);
			transform.LoadMatrix(m);
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

		public static void NJRotateXYZ(this MatrixStack transform, int x, int y, int z)
		{
			Matrix m = transform.Top;
			RotateXYZ(ref m, x, y, z);
			transform.LoadMatrix(m);
		}

		public static void NJRotateXYZ(this MatrixStack transform, Rotation rotation)
		{
			transform.NJRotateXYZ(rotation.X, rotation.Y, rotation.Z);
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

		public static void NJRotateZYX(this MatrixStack transform, int x, int y, int z)
		{
			Matrix m = transform.Top;
			RotateZYX(ref m, x, y, z);
			transform.LoadMatrix(m);
		}

		public static void NJRotateZYX(this MatrixStack transform, Rotation rotation)
		{
			transform.NJRotateZYX(rotation.X, rotation.Y, rotation.Z);
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

		public static void NJRotateObject(this MatrixStack transform, int x, int y, int z)
		{
			Matrix m = transform.Top;
			RotateObject(ref m, x, y, z);
			transform.LoadMatrix(m);
		}

		public static void NJRotateObject(this MatrixStack transform, Rotation rotation)
		{
			transform.NJRotateObject(rotation.X, rotation.Y, rotation.Z);
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

		public static void NJScale(this MatrixStack transform, float x, float y, float z)
		{
			Matrix m = transform.Top;
			Scale(ref m, x, y, z);
			transform.LoadMatrix(m);
		}

		public static void NJScale(this MatrixStack transform, Vertex vertex)
		{
			transform.NJScale(vertex.X, vertex.Y, vertex.Z);
		}

		public static void NJScale(this MatrixStack transform, Vector3 vector)
		{
			transform.NJScale(vector.X, vector.Y, vector.Z);
		}
	}
}