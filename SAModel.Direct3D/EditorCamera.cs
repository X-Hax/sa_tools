using System;
using Microsoft.DirectX;

namespace SonicRetro.SAModel.Direct3D
{
    public class EditorCamera
    {
        public float Distance { get; set; }
        public Vector3 Position { get; set; }
        public ushort Yaw { get; set; }
        public ushort Pitch { get; set; }
        public Vector3 Up { get; private set; }
        public Vector3 Look { get; private set; }
        public Vector3 Right { get; private set; }
        public int mode { get; set; }
        public Vector3 FocalPoint { get; set; }
        public float DrawDistance { get; set; }
        public float MoveSpeed { get; set; }
        public float RotateSpeed { get; set; }

		public float FOV { get; set; }
		public float Aspect { get; set; }
		public float HNear { get; set; }
		public float WNear { get; set; }
		public float HFar { get; set; }
		public float WFar { get; set; }

		#region Frustum Vars
		private Vertex farTopLeft, farTopRight, farBottomLeft, farBottomRight;
		private Vertex nearTopLeft, nearTopRight, nearBottomLeft, nearBottomRight;

		Plane[] frustumPlanes = new Plane[6];
		#endregion

		public EditorCamera(float drawDistance) { RotateSpeed = 1.5f; MoveSpeed = 1.125f; Distance = 20; DrawDistance = drawDistance; }

        public Matrix ToMatrix()
        {
            if (mode == 0)
            {
                Up = new Vector3(0, 1, 0);
                Look = new Vector3(0, 0, 1);
                Right = new Vector3(1, 0, 0);
                Matrix yawm = Matrix.RotationAxis(Up, Extensions.BAMSToRad(Yaw));
                Look = Vector3.TransformCoordinate(Look, yawm);
                Right = Vector3.TransformCoordinate(Right, yawm);
                Matrix pitchm = Matrix.RotationAxis(Right, Extensions.BAMSToRad(Pitch));
                Look = Vector3.TransformCoordinate(Look, pitchm);
                Up = Vector3.TransformCoordinate(Up, pitchm);
                Matrix ViewMatrix = Matrix.Identity;
                ViewMatrix.M11 = Right.X;
                ViewMatrix.M12 = Up.X;
                ViewMatrix.M13 = Look.X;
                ViewMatrix.M21 = Right.Y;
                ViewMatrix.M22 = Up.Y;
                ViewMatrix.M23 = Look.Y;
                ViewMatrix.M31 = Right.Z;
                ViewMatrix.M32 = Up.Z;
                ViewMatrix.M33 = Look.Z;
                ViewMatrix.M41 = -Vector3.Dot(Position, Right);
                ViewMatrix.M42 = -Vector3.Dot(Position, Up);
				ViewMatrix.M43 = -Vector3.Dot(Position, Look);

                return ViewMatrix;
            }
            else
            {
                Vector3 distvec = Vector3.TransformCoordinate(new Vector3(0, 0, Distance), Matrix.RotationYawPitchRoll(Extensions.BAMSToRad(Yaw), Extensions.BAMSToRad(Pitch), 0));
                Position = distvec + FocalPoint;
                Matrix ViewMatrix = Matrix.LookAtRH(Position, FocalPoint, new Vector3(0, 1, 0));
                Up = new Vector3(ViewMatrix.M12, ViewMatrix.M22, ViewMatrix.M32);
                Look = new Vector3(ViewMatrix.M13, ViewMatrix.M23, ViewMatrix.M33);
                Right = new Vector3(ViewMatrix.M11, ViewMatrix.M21, ViewMatrix.M31);
                return ViewMatrix;
            }
        }

		/// <summary>
		/// Calculates the near and far plane dimensions, as well as the vertices for the entire frustum.
		/// </summary>
		/// <param name="viewMatrix"></param>
		public void BuildFrustum(Matrix view, Matrix projection)
		{
			Matrix viewMatrix = Matrix.Multiply(view, projection);

			#region Calculating vertex representation of Frustum
			// This might not even match the planes below, since these weren't used to calculate them. 
			// in fact, this code probably isn't even necessary. MainMemory if you see this, yes you can ask me to remove it.
			HNear = 2 * (float)Math.Tan(FOV / 2) * 1f;
			WNear = HNear * Aspect;

			HFar = 2 * (float)Math.Tan(FOV / 2) * DrawDistance;
			WFar = HFar * Aspect;

			Vector3 fc = (Position + Look) * DrawDistance;
			Vector3 ftl = fc + (Up * (HFar / 2)) - (Right * (WFar / 2));
			farTopLeft = ftl.ToVertex();

			Vector3 ftr = fc + (Up * (HFar/2)) + (Right * (WFar/2));
			farTopRight = ftr.ToVertex();
			Vector3 fbl = fc - (Up * (HFar/2)) - (Right * (WFar/2));
			farBottomLeft = fbl.ToVertex();
			Vector3 fbr = fc - (Up * (HFar/2)) + (Right * (WFar/2));
			farBottomRight = fbr.ToVertex();

			Vector3 nc = Position + Look * 1f;

			Vector3 ntl = nc + (Up * (HNear / 2)) - (Right * (WNear / 2));
			nearTopLeft = ntl.ToVertex();
			Vector3 ntr = nc + (Up * (HNear / 2)) + (Right * (WNear / 2));
			nearTopRight = ntr.ToVertex();
			Vector3 nbl = nc - (Up * (HNear / 2)) - (Right * (WNear / 2));
			nearBottomLeft = nbl.ToVertex();
			Vector3 nbr = nc - (Up * (HNear / 2)) + (Right * (WNear / 2));
			nearBottomRight = nbr.ToVertex();
			#endregion

			#region Building Frustum Planes
			// Left plane
			frustumPlanes[0].A = viewMatrix.M14 + viewMatrix.M11;
			frustumPlanes[0].B = viewMatrix.M24 + viewMatrix.M21;
			frustumPlanes[0].C = viewMatrix.M34 + viewMatrix.M31;
			frustumPlanes[0].D = viewMatrix.M44 + viewMatrix.M41;

			// Right plane
			frustumPlanes[1].A = viewMatrix.M14 - viewMatrix.M11;
			frustumPlanes[1].B = viewMatrix.M24 - viewMatrix.M21;
			frustumPlanes[1].C = viewMatrix.M34 - viewMatrix.M31;
			frustumPlanes[1].D = viewMatrix.M44 - viewMatrix.M41;

			// Top plane
			frustumPlanes[2].A = viewMatrix.M14 - viewMatrix.M12;
			frustumPlanes[2].B = viewMatrix.M24 - viewMatrix.M22;
			frustumPlanes[2].C = viewMatrix.M34 - viewMatrix.M32;
			frustumPlanes[2].D = viewMatrix.M44 - viewMatrix.M42;

			// Bottom plane
			frustumPlanes[3].A = viewMatrix.M14 + viewMatrix.M12;
			frustumPlanes[3].B = viewMatrix.M24 + viewMatrix.M22;
			frustumPlanes[3].C = viewMatrix.M34 + viewMatrix.M32;
			frustumPlanes[3].D = viewMatrix.M44 + viewMatrix.M42;

			// Near plane
			frustumPlanes[4].A = viewMatrix.M13;
			frustumPlanes[4].B = viewMatrix.M23;
			frustumPlanes[4].C = viewMatrix.M33;
			frustumPlanes[4].D = viewMatrix.M43;

			// Far plane
			frustumPlanes[5].A = viewMatrix.M14 - viewMatrix.M13;
			frustumPlanes[5].B = viewMatrix.M24 - viewMatrix.M23;
			frustumPlanes[5].C = viewMatrix.M34 - viewMatrix.M33;
			frustumPlanes[5].D = viewMatrix.M44 - viewMatrix.M43;
			#endregion

			// Normalize planes
			for (int i = 0; i < 6; i++)
			{
				frustumPlanes[i].Normalize();
			}
		}

		public bool SphereInFrustum(BoundingSphere sphere)
		{
			for (int i = 0; i < 6; i++)
			{
				if (frustumPlanes[i].Dot(sphere.Center.ToVector3()) + sphere.Radius < 0)
				{
					return false;
				}
			}

			return true;
		}
    }
}
