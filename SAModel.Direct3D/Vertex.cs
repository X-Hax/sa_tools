using System;
using System.Runtime.InteropServices;
using SharpDX;

namespace SonicRetro.SAModel.Direct3D
{
	[Flags]
	public enum VertexFormat
	{
		Position = 1 << 0,
		Normal   = 1 << 1,
		Diffuse  = 1 << 2,
		Texture1 = 1 << 4,
	}

	public interface IVertex
	{
		Vector3      GetPosition();
		void         SetPosition(Vector3 v);
		VertexFormat GetFormat();
		Vector3      GetNormal();
		void         SetNormal(Vector3 v);
		Color        GetColor();
		void         SetColor(Color c);
		Vector2      GetUV();
		void         SetUV(Vector2 v);
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct FVF_PositionNormal : IVertex
	{
		[FieldOffset(0x00)] public Vector3 Position;
		[FieldOffset(0x0C)] public Vector3 Normal;

		public const VertexFormat Format = VertexFormat.Position | VertexFormat.Normal;

		public FVF_PositionNormal(Vector3 position, Vector3 normal)
		{
			Position = position;
			Normal   = normal;
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

		public Color GetColor() => Color.Zero;

		public void SetColor(Color c)
		{
		}

		public Vector2 GetUV() => Vector2.Zero;

		public void SetUV(Vector2 v)
		{
		}

		public VertexFormat GetFormat() => Format;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct FVF_PositionTextured : IVertex
	{
		[FieldOffset(0x00)] public Vector3 Position;
		[FieldOffset(0x0C)] public Vector2 UV;

		public const VertexFormat Format = VertexFormat.Position | VertexFormat.Normal | VertexFormat.Texture1;

		public FVF_PositionTextured(Vector3 position, Vector2 uv)
		{
			Position = position;
			UV       = uv;
		}

		public FVF_PositionTextured(VertexData data)
		{
			Position = data.Position.ToVector3();
			UV       = data.UV != null ? new Vector2(data.UV.U, data.UV.V) : new Vector2();
		}

		public Vector3 GetPosition() => Position;

		public void SetPosition(Vector3 v) => Position = v;

		public VertexFormat GetFormat() => Format;

		public Vector3 GetNormal() => Vector3.Zero;

		public void SetNormal(Vector3 v)
		{
		}

		public Color GetColor() => Color.Zero;

		public void SetColor(Color c)
		{
		}

		public Vector2 GetUV()          => UV;
		public void    SetUV(Vector2 v) => UV = v;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct FVF_PositionColored : IVertex
	{
		[FieldOffset(0x00)] public Vector3 Position;
		[FieldOffset(0x0C)] public Color   Color;

		public const VertexFormat Format = VertexFormat.Position | VertexFormat.Diffuse;

		public FVF_PositionColored(Vector3 position, Color color)
		{
			Position = position;
			Color    = color;
		}

		public FVF_PositionColored(VertexData data)
		{
			Position = data.Position.ToVector3();
			Color    = data.Color ?? Color.White;
		}

		public Vector3 GetPosition() => Position;

		public void SetPosition(Vector3 v) => Position = v;

		public VertexFormat GetFormat() => Format;

		public Vector3 GetNormal()
		{
			return Vector3.Zero;
		}

		public void SetNormal(Vector3 v)
		{
		}

		public Color GetColor() => Color;

		public void SetColor(Color c) => Color = c;

		public Vector2 GetUV() => Vector2.Zero;

		public void SetUV(Vector2 v)
		{
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct FVF_PositionNormalTextured : IVertex
	{
		[FieldOffset(0x00)] public Vector3 Position;
		[FieldOffset(0x0C)] public Vector3 Normal;
		[FieldOffset(0x18)] public Vector2 UV;

		public const VertexFormat Format = VertexFormat.Position | VertexFormat.Normal | VertexFormat.Texture1;

		public FVF_PositionNormalTextured(Vector3 position, Vector3 normal, Vector2 uv)
		{
			Position = position;
			Normal   = normal;
			UV       = uv;
		}

		public FVF_PositionNormalTextured(VertexData data)
		{
			Position = data.Position.ToVector3();
			Normal   = data.Normal.ToVector3();
			UV       = data.UV != null ? new Vector2(data.UV.U, data.UV.V) : new Vector2();
		}

		public Vector3 GetPosition() => Position;

		public void SetPosition(Vector3 v) => Position = v;

		public Vector3 GetNormal() => Normal;

		public void SetNormal(Vector3 v) => Normal = v;

		public Color GetColor() => Color.Zero;

		public void SetColor(Color c)
		{
		}

		public Vector2 GetUV() => UV;

		public void SetUV(Vector2 v) => UV = v;

		public VertexFormat GetFormat() => Format;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct FVF_PositionNormalColored : IVertex
	{
		[FieldOffset(0x00)] public Vector3 Position;
		[FieldOffset(0x0C)] public Vector3 Normal;
		[FieldOffset(0x18)] public Color   Color;

		public const VertexFormat Format = VertexFormat.Position | VertexFormat.Normal | VertexFormat.Diffuse;

		public FVF_PositionNormalColored(Vector3 position, Vector3 normal, Color color)
		{
			Position = position;
			Normal   = normal;
			Color    = color;
		}

		public FVF_PositionNormalColored(VertexData data)
		{
			Position = data.Position.ToVector3();
			Normal   = data.Normal.ToVector3();
			Color    = (data.Color ?? Color.White);
		}

		public Vector3 GetPosition() => Position;

		public void SetPosition(Vector3 v) => Position = v;

		public Vector3 GetNormal() => Normal;

		public void SetNormal(Vector3 v) => Normal = v;

		public Color GetColor() => Color;

		public void SetColor(Color c) => Color = c;

		public Vector2 GetUV() => Vector2.Zero;

		public void SetUV(Vector2 v)
		{
		}

		public VertexFormat GetFormat() => Format;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct FVF_PositionNormalTexturedColored : IVertex
	{
		[FieldOffset(0x00)] public Vector3 Position;
		[FieldOffset(0x0C)] public Vector3 Normal;
		[FieldOffset(0x18)] public Color   Color;
		[FieldOffset(0x1C)] public Vector2 UV;

		public const VertexFormat Format = VertexFormat.Position | VertexFormat.Normal | VertexFormat.Diffuse | VertexFormat.Texture1;

		public FVF_PositionNormalTexturedColored(Vector3 position, Vector3 normal, Vector2 uv, Color color)
		{
			Position = position;
			Normal   = normal;
			Color    = color;
			UV       = uv;
		}

		public FVF_PositionNormalTexturedColored(VertexData data)
		{
			Position = data.Position.ToVector3();
			Normal   = data.Normal.ToVector3();
			Color    = (data.Color ?? Color.White);
			UV       = data.UV != null ? new Vector2(data.UV.U, data.UV.V) : new Vector2();
		}

		public Vector3 GetPosition() => Position;

		public void SetPosition(Vector3 v) => Position = v;

		public Vector3 GetNormal() => Normal;

		public void SetNormal(Vector3 v) => Normal = v;

		public Color GetColor() => Color;

		public void SetColor(Color c) => Color = c;

		public Vector2 GetUV() => UV;

		public void SetUV(Vector2 v) => UV = v;

		public VertexFormat GetFormat() => Format;
	}
}