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
        public Vector2 UV;
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

        public FVF_PositionNormalTexturedColored(Vector3 Pos, Vector3 Nor, Vector2 UV, Color Col)
        {
            Position = Pos;
            Normal = Nor;
            this.UV = UV;
            Color = Col.ToArgb();
        }

        public FVF_PositionNormalTexturedColored(VertexData data)
        {
            Position = data.Position.ToVector3();
            Normal = data.Normal.ToVector3();
            UV = new Vector2(data.UV.U / 255f, data.UV.V / 255f);
            Color = data.Color.ToArgb();
        }
    }
}
