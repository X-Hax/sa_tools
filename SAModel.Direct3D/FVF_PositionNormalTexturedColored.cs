using System.Drawing;
using System.Runtime.InteropServices;
using SlimDX;
using SlimDX.Direct3D9;

namespace SonicRetro.SAModel.Direct3D
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FVF_PositionNormalTexturedColored
    {
        public Vector3 Position;
        public Vector3 Normal;
        public int Diffuse;
        public float tu;
        public float tv;

        public const VertexFormat Format = VertexFormat.PositionNormal | VertexFormat.Diffuse | VertexFormat.Texture1;

        public FVF_PositionNormalTexturedColored(Vector3 Pos, Vector3 Nor, Color Col, float U, float V)
        {
            Position = Pos;
            Normal = Nor;
            Diffuse = Col.ToArgb();
            tu = U;
            tv = V;
        }

        public FVF_PositionNormalTexturedColored(VertexData data)
        {
            Position = data.Position.ToVector3();
            Normal = data.Normal.ToVector3();
            Diffuse = data.Color.ToArgb();
            tu = data.UV.U / 255f;
            tv = data.UV.V / 255f;
        }
    }
}
