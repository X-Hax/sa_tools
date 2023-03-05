﻿using SharpDX;
using SharpDX.Direct3D9;
using System.Runtime.InteropServices;
using Color = System.Drawing.Color;

namespace SAModel.Direct3D
{
	public interface IVertex
	{
		Vector3 GetPosition();
		void SetPosition(Vector3 v);
		VertexFormat GetFormat();
	}

	public interface IVertexNormal : IVertex
	{
		Vector3 GetNormal();
		void SetNormal(Vector3 v);
	}

	public interface IVertexColor
	{
		Color GetColor();
		void SetColor(Color c);
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct FVF_PositionNormal : IVertexNormal
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

		public const VertexFormat Format = VertexFormat.Position | VertexFormat.Normal;

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

		public Vector3 GetPosition() => Position;

		public void SetPosition(Vector3 v) => Position = v;

		public Vector3 GetNormal() => Normal;

		public void SetNormal(Vector3 v) => Normal = v;

		public VertexFormat GetFormat() => Format;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct FVF_PositionTextured : IVertex
	{
		[FieldOffset(0x00)]
		public Vector3 Position;
		[FieldOffset(0x0C)]
		public Vector2 UV;

		public static VertexElement[] Elements
		{
			get
			{
				return new VertexElement[] {
					new VertexElement(0, 0x00, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
					new VertexElement(0, 0x0C, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
					VertexElement.VertexDeclarationEnd
				};
			}
		}

		public const VertexFormat Format = VertexFormat.Position | VertexFormat.Normal | VertexFormat.Texture1;

		public FVF_PositionTextured(Vector3 Pos, Vector2 UV)
		{
			Position = Pos;
			this.UV = UV;
		}

		public FVF_PositionTextured(VertexData data)
		{
			Position = data.Position.ToVector3();
			if (data.UV != null)
				UV = new Vector2((float)data.UV.U, (float)data.UV.V);
			else
				UV = new Vector2();
		}

		public Vector3 GetPosition() => Position;

		public void SetPosition(Vector3 v) => Position = v;

		public VertexFormat GetFormat() => Format;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct FVF_PositionColored : IVertex, IVertexColor
	{
		[FieldOffset(0x00)]
		public Vector3 Position;
		[FieldOffset(0x0C)]
		public int Color;

		public static VertexElement[] Elements
		{
			get
			{
				return new VertexElement[] {
					new VertexElement(0, 0x00, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
					new VertexElement(0, 0x0C, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
					VertexElement.VertexDeclarationEnd
				};
			}
		}

		public const VertexFormat Format = VertexFormat.Position | VertexFormat.Diffuse;

		public FVF_PositionColored(Vector3 Pos, Color Col)
		{
			Position = Pos;
			Color = Col.ToArgb();
		}

		public FVF_PositionColored(VertexData data)
		{
			Position = data.Position.ToVector3();
			Color = (data.Color ?? System.Drawing.Color.White).ToArgb();
		}

		public Vector3 GetPosition() => Position;

		public void SetPosition(Vector3 v) => Position = v;

		public VertexFormat GetFormat() => Format;

		public Color GetColor() => System.Drawing.Color.FromArgb(Color);

		public void SetColor(Color c) => Color = c.ToArgb();
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct FVF_PositionNormalTextured : IVertexNormal
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

		public const VertexFormat Format = VertexFormat.Position | VertexFormat.Normal | VertexFormat.Texture1;

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
				UV = new Vector2((float)data.UV.U, (float)data.UV.V);
			else
				UV = new Vector2();
		}

		public Vector3 GetPosition() => Position;

		public void SetPosition(Vector3 v) => Position = v;

		public Vector3 GetNormal() => Normal;

		public void SetNormal(Vector3 v) => Normal = v;

		public VertexFormat GetFormat() => Format;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct FVF_PositionNormalColored : IVertexNormal, IVertexColor
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

		public const VertexFormat Format = VertexFormat.Position | VertexFormat.Normal | VertexFormat.Diffuse;

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

		public Vector3 GetPosition() => Position;

		public void SetPosition(Vector3 v) => Position = v;

		public Vector3 GetNormal() => Normal;

		public void SetNormal(Vector3 v) => Normal = v;

		public VertexFormat GetFormat() => Format;

		public Color GetColor() => System.Drawing.Color.FromArgb(Color);

		public void SetColor(Color c) => Color = c.ToArgb();
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct FVF_PositionNormalTexturedColored : IVertexNormal, IVertexColor
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

		public const VertexFormat Format = VertexFormat.Position | VertexFormat.Normal | VertexFormat.Diffuse | VertexFormat.Texture1;

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
				UV = new Vector2((float)data.UV.U, (float)data.UV.V);
			else
				UV = new Vector2();
		}

		public Vector3 GetPosition() => Position;

		public void SetPosition(Vector3 v) => Position = v;

		public Vector3 GetNormal() => Normal;

		public void SetNormal(Vector3 v) => Normal = v;

		public VertexFormat GetFormat() => Format;

		public Color GetColor() => System.Drawing.Color.FromArgb(Color);

		public void SetColor(Color c) => Color = c.ToArgb();
	}
}
