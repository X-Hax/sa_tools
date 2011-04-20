using Microsoft.DirectX;

namespace SonicRetro.SAModel.Direct3D
{
    public class Camera
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

        public Camera() { Distance = 20; }

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
    }
}
