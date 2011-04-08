using System.Drawing;
using System.Runtime.InteropServices;
using SlimDX;
using SlimDX.Direct3D9;

namespace SonicRetro.SAModel.Direct3D
{
    [StructLayout(LayoutKind.Explicit)]
    public struct FVF_PositionNormalTexturedColored
    {
        [FieldOffset(0x00)]
        public Vector3 Position;
        [FieldOffset(0x0C)]
        public Vector3 Normal;
        [FieldOffset(0x18)]
        public float tu;
        [FieldOffset(0x1C)]
        public float tv;
        [FieldOffset(0x20)]
        public int Color;

        public static VertexElement[] Elements
        {
            get
            {
                return new VertexElement[] {
                    new VertexElement(0, 0x00, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                    new VertexElement(0, 0x0C, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
                    new VertexElement(0, 0x18, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
                    new VertexElement(0, 0x20, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                    VertexElement.VertexDeclarationEnd
                };
            }
        }

        public FVF_PositionNormalTexturedColored(Vector3 Pos, Vector3 Nor, Color Col, float U, float V)
        {
            Position = Pos;
            Normal = Nor;
            Color = Col.ToArgb();
            tu = U;
            tv = V;
        }

        public FVF_PositionNormalTexturedColored(VertexData data)
        {
            Position = data.Position.ToVector3();
            Normal = data.Normal.ToVector3();
            Color = data.Color.ToArgb();
            tu = data.UV.U / 255f;
            tv = data.UV.V / 255f;
        }
    }
}
