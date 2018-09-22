using System;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using Color = System.Drawing.Color;

namespace SonicRetro.SAModel.Direct3D
{
	[Flags]
	public enum VertexFormat
	{
		Position = 1 << 0,
		Normal   = 1 << 1,
		Diffuse  = 1 << 2,
		Specular = 1 << 3,
		Texture1 = 1 << 4,
	}

	public interface IVertex
	{
		Vector3      GetPosition();
		void         SetPosition(Vector3 v);
		VertexFormat GetFormat();
	}

	public interface IVertexNormal : IVertex
	{
		Vector3 GetNormal();
		void    SetNormal(Vector3 v);
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct FVF_PositionNormal : IVertexNormal
	{
		[FieldOffset(0x00)] public Vector3 Position;
		[FieldOffset(0x0C)] public Vector3 Normal;

		public static InputElement[] Elements => new InputElement[]
		{
			new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
			new InputElement("NORMAL",   0, SharpDX.DXGI.Format.R32G32B32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
		};

		public const VertexFormat Format = VertexFormat.Position | VertexFormat.Normal;

		public FVF_PositionNormal(Vector3 Pos, Vector3 Nor)
		{
			Position = Pos;
			Normal   = Nor;
		}

		public FVF_PositionNormal(VertexData data)
		{
			Position = data.Position.ToVector3();
			Normal   = data.Normal.ToVector3();
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
		[FieldOffset(0x00)] public Vector3 Position;
		[FieldOffset(0x0C)] public Vector2 UV;

		public static InputElement[] Elements => new InputElement[]
		{
			new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
			new InputElement("TEX",      0, SharpDX.DXGI.Format.R32G32_Float,    InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
		};

		public const VertexFormat Format = VertexFormat.Position | VertexFormat.Normal | VertexFormat.Texture1;

		public FVF_PositionTextured(Vector3 Pos, Vector2 UV)
		{
			Position = Pos;
			this.UV  = UV;
		}

		public FVF_PositionTextured(VertexData data)
		{
			Position = data.Position.ToVector3();
			UV       = data.UV != null ? new Vector2(data.UV.U, data.UV.V) : new Vector2();
		}

		public Vector3 GetPosition() => Position;

		public void SetPosition(Vector3 v) => Position = v;

		public VertexFormat GetFormat() => Format;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct FVF_PositionColored : IVertex
	{
		[FieldOffset(0x00)] public Vector3 Position;
		[FieldOffset(0x0C)] public int     Color;

		public static InputElement[] Elements => new InputElement[]
		{
			new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
			new InputElement("COLOR",    0, SharpDX.DXGI.Format.R8G8B8A8_UNorm,  InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
		};

		public const VertexFormat Format = VertexFormat.Position | VertexFormat.Diffuse;

		public FVF_PositionColored(Vector3 Pos, Color Col)
		{
			Position = Pos;
			Color    = Col.ToArgb();
		}

		public FVF_PositionColored(VertexData data)
		{
			Position = data.Position.ToVector3();
			Color    = (data.Color ?? System.Drawing.Color.White).ToArgb();
		}

		public Vector3 GetPosition() => Position;

		public void SetPosition(Vector3 v) => Position = v;

		public VertexFormat GetFormat() => Format;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct FVF_PositionNormalTextured : IVertexNormal
	{
		[FieldOffset(0x00)] public Vector3 Position;
		[FieldOffset(0x0C)] public Vector3 Normal;
		[FieldOffset(0x18)] public Vector2 UV;

		public static InputElement[] Elements => new InputElement[]
		{
			new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
			new InputElement("NORMAL",   0, SharpDX.DXGI.Format.R32G32B32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
			new InputElement("TEX",      0, SharpDX.DXGI.Format.R32G32_Float,    InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
		};

		public const VertexFormat Format = VertexFormat.Position | VertexFormat.Normal | VertexFormat.Texture1;

		public FVF_PositionNormalTextured(Vector3 Pos, Vector3 Nor, Vector2 UV)
		{
			Position = Pos;
			Normal   = Nor;
			this.UV  = UV;
		}

		public FVF_PositionNormalTextured(VertexData data)
		{
			Position = data.Position.ToVector3();
			Normal   = data.Normal.ToVector3();
			UV = data.UV != null ? new Vector2(data.UV.U, data.UV.V) : new Vector2();
		}

		public Vector3 GetPosition() => Position;

		public void SetPosition(Vector3 v) => Position = v;

		public Vector3 GetNormal() => Normal;

		public void SetNormal(Vector3 v) => Normal = v;

		public VertexFormat GetFormat() => Format;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct FVF_PositionNormalColored : IVertexNormal
	{
		[FieldOffset(0x00)] public Vector3 Position;
		[FieldOffset(0x0C)] public Vector3 Normal;
		[FieldOffset(0x18)] public int     Color;

		public static InputElement[] Elements => new InputElement[]
		{
			new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float,    InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
			new InputElement("NORMAL",   0, SharpDX.DXGI.Format.R32G32B32_Float,    InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
			new InputElement("COLOR",    0, SharpDX.DXGI.Format.R32G32B32A32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
		};

		public const VertexFormat Format = VertexFormat.Position | VertexFormat.Normal | VertexFormat.Diffuse;

		public FVF_PositionNormalColored(Vector3 Pos, Vector3 Nor, Color Col)
		{
			Position = Pos;
			Normal   = Nor;
			Color    = Col.ToArgb();
		}

		public FVF_PositionNormalColored(VertexData data)
		{
			Position = data.Position.ToVector3();
			Normal   = data.Normal.ToVector3();
			Color    = (data.Color ?? System.Drawing.Color.White).ToArgb();
		}

		public Vector3 GetPosition() => Position;

		public void SetPosition(Vector3 v) => Position = v;

		public Vector3 GetNormal() => Normal;

		public void SetNormal(Vector3 v) => Normal = v;

		public VertexFormat GetFormat() => Format;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct FVF_PositionNormalTexturedColored : IVertexNormal
	{
		[FieldOffset(0x00)] public Vector3 Position;
		[FieldOffset(0x0C)] public Vector3 Normal;
		[FieldOffset(0x18)] public int     Color;
		[FieldOffset(0x1C)] public Vector2 UV;

		public static InputElement[] Elements => new InputElement[]
		{
			new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
			new InputElement("NORMAL",   0, SharpDX.DXGI.Format.R32G32B32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
			new InputElement("COLOR",    0, SharpDX.DXGI.Format.R8G8B8A8_UNorm,  InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
			new InputElement("TEX",      0, SharpDX.DXGI.Format.R32G32_Float,    InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0)
		};

		public const VertexFormat Format = VertexFormat.Position | VertexFormat.Normal | VertexFormat.Diffuse | VertexFormat.Texture1;

		public FVF_PositionNormalTexturedColored(Vector3 Pos, Vector3 Nor, Vector2 UV, Color Col)
		{
			Position = Pos;
			Normal   = Nor;
			Color    = Col.ToArgb();
			this.UV  = UV;
		}

		public FVF_PositionNormalTexturedColored(VertexData data)
		{
			Position = data.Position.ToVector3();
			Normal   = data.Normal.ToVector3();
			Color    = (data.Color ?? System.Drawing.Color.White).ToArgb();
			UV = data.UV != null ? new Vector2(data.UV.U, data.UV.V) : new Vector2();
		}

		public Vector3 GetPosition() => Position;

		public void SetPosition(Vector3 v) => Position = v;

		public Vector3 GetNormal() => Normal;

		public void SetNormal(Vector3 v) => Normal = v;

		public VertexFormat GetFormat() => Format;
	}
}