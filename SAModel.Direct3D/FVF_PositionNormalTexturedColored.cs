using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace SonicRetro.SAModel.Direct3D
{
    [StructLayout(LayoutKind.Explicit)]
    public struct FVF_PositionNormal
    {
        [FieldOffset(0x00)]
        public Vector3 Position;
        [FieldOffset(0x0C)]
        public Vector3 Normal;

        public static VertexElement[] Elements
        {
            get
            {
                return new VertexElement[] {
                    new VertexElement(0, 0x00, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                    new VertexElement(0, 0x0C, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
                    VertexElement.VertexDeclarationEnd
                };
            }
        }

        public const VertexFormats Format = VertexFormats.PositionNormal;

        public FVF_PositionNormal(Vector3 Pos, Vector3 Nor)
        {
            Position = Pos;
            Normal = Nor;
        }

        public FVF_PositionNormal(VertexData data)
        {
            Position = data.Position.ToVector3();
            Normal = data.Normal.ToVector3();
        }
    }

	[StructLayout(LayoutKind.Explicit)]
	public struct FVF_PositionNormalTextured
	{
		[FieldOffset(0x00)]
		public Vector3 Position;
		[FieldOffset(0x0C)]
		public Vector3 Normal;
		[FieldOffset(0x18)]
		public Vector2 UV;

		public static VertexElement[] Elements
		{
			get
			{
				return new VertexElement[] {
                    new VertexElement(0, 0x00, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                    new VertexElement(0, 0x0C, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
                    new VertexElement(0, 0x18, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
                    VertexElement.VertexDeclarationEnd
                };
			}
		}

		public const VertexFormats Format = VertexFormats.PositionNormal | VertexFormats.Texture1;

		public FVF_PositionNormalTextured(Vector3 Pos, Vector3 Nor, Vector2 UV)
		{
			Position = Pos;
			Normal = Nor;
			this.UV = UV;
		}

		public FVF_PositionNormalTextured(VertexData data)
		{
			Position = data.Position.ToVector3();
			Normal = data.Normal.ToVector3();
			if (data.UV != null)
				UV = new Vector2(data.UV.U, data.UV.V);
			else
				UV = new Vector2();
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct FVF_PositionNormalColored
	{
		[FieldOffset(0x00)]
		public Vector3 Position;
		[FieldOffset(0x0C)]
		public Vector3 Normal;
		[FieldOffset(0x18)]
		public int Color;

		public static VertexElement[] Elements
		{
			get
			{
				return new VertexElement[] {
                    new VertexElement(0, 0x00, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                    new VertexElement(0, 0x0C, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
                    new VertexElement(0, 0x18, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                    VertexElement.VertexDeclarationEnd
                };
			}
		}

		public const VertexFormats Format = VertexFormats.PositionNormal | VertexFormats.Diffuse;

		public FVF_PositionNormalColored(Vector3 Pos, Vector3 Nor, Color Col)
		{
			Position = Pos;
			Normal = Nor;
			Color = Col.ToArgb();
		}

		public FVF_PositionNormalColored(VertexData data)
		{
			Position = data.Position.ToVector3();
			Normal = data.Normal.ToVector3();
			Color = (data.Color ?? System.Drawing.Color.White).ToArgb();
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct FVF_PositionNormalTexturedColored
	{
		[FieldOffset(0x00)]
		public Vector3 Position;
		[FieldOffset(0x0C)]
		public Vector3 Normal;
		[FieldOffset(0x18)]
		public int Color;
		[FieldOffset(0x1C)]
		public Vector2 UV;

		public static VertexElement[] Elements
		{
			get
			{
				return new VertexElement[] {
                    new VertexElement(0, 0x00, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                    new VertexElement(0, 0x0C, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
                    new VertexElement(0, 0x18, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                    new VertexElement(0, 0x1C, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
                    VertexElement.VertexDeclarationEnd
                };
			}
		}

		public const VertexFormats Format = VertexFormats.PositionNormal | VertexFormats.Diffuse | VertexFormats.Texture1;

		public FVF_PositionNormalTexturedColored(Vector3 Pos, Vector3 Nor, Vector2 UV, Color Col)
		{
			Position = Pos;
			Normal = Nor;
			Color = Col.ToArgb();
			this.UV = UV;
		}

		public FVF_PositionNormalTexturedColored(VertexData data)
		{
			Position = data.Position.ToVector3();
			Normal = data.Normal.ToVector3();
			Color = (data.Color ?? System.Drawing.Color.White).ToArgb();
			if (data.UV != null)
				UV = new Vector2(data.UV.U, data.UV.V);
			else
				UV = new Vector2();
		}
	}
}
