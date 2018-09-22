using System;
using System.Collections.Generic;
using SharpDX;

namespace SonicRetro.SAModel
{
	[Serializable]
	public abstract class PolyChunk : ICloneable
	{
		public ushort Header { get; set; }

		public ChunkType Type
		{
			get { return (ChunkType)(Header & 0xFF); }
			protected set { Header = (ushort)((Header & 0xFF00) | (byte)value); }
		}

		public byte Flags
		{
			get { return (byte)(Header >> 8); }
			set { Header = (ushort)((Header & 0xFF) | (ushort)(value << 8)); }
		}

		public abstract int ByteSize { get; }

		public static PolyChunk Load(byte[] file, int address)
		{
			ChunkType type = (ChunkType)(ByteConverter.ToUInt16(file, address) & 0xFF);
			switch (type)
			{
				case ChunkType.Null:
					return new PolyChunkNull(file, address);
				case ChunkType.Bits_BlendAlpha:
					return new PolyChunkBitsBlendAlpha(file, address);
				case ChunkType.Bits_MipmapDAdjust:
					return new PolyChunkBitsMipmapDAdjust(file, address);
				case ChunkType.Bits_SpecularExponent:
					return new PolyChunkBitsSpecularExponent(file, address);
				case ChunkType.Bits_CachePolygonList:
					return new PolyChunkBitsCachePolygonList(file, address);
				case ChunkType.Bits_DrawPolygonList:
					return new PolyChunkBitsDrawPolygonList(file, address);
				case ChunkType.Tiny_TextureID:
				case ChunkType.Tiny_TextureID2:
					return new PolyChunkTinyTextureID(file, address);
				case ChunkType.Material_Diffuse:
				case ChunkType.Material_Ambient:
				case ChunkType.Material_DiffuseAmbient:
				case ChunkType.Material_Specular:
				case ChunkType.Material_DiffuseSpecular:
				case ChunkType.Material_AmbientSpecular:
				case ChunkType.Material_DiffuseAmbientSpecular:
				case ChunkType.Material_Diffuse2:
				case ChunkType.Material_Ambient2:
				case ChunkType.Material_DiffuseAmbient2:
				case ChunkType.Material_Specular2:
				case ChunkType.Material_DiffuseSpecular2:
				case ChunkType.Material_AmbientSpecular2:
				case ChunkType.Material_DiffuseAmbientSpecular2:
					return new PolyChunkMaterial(file, address);
				case ChunkType.Material_Bump:
					return new PolyChunkMaterialBump(file, address);
				case ChunkType.Volume_Polygon3:
				case ChunkType.Volume_Polygon4:
				case ChunkType.Volume_Strip:
					return new PolyChunkVolume(file, address);
				case ChunkType.Strip_Strip:
				case ChunkType.Strip_StripUVN:
				case ChunkType.Strip_StripUVH:
				case ChunkType.Strip_StripColor:
				case ChunkType.Strip_StripUVNColor:
				case ChunkType.Strip_StripUVHColor:
				case ChunkType.Strip_Strip2:
				case ChunkType.Strip_StripUVN2:
				case ChunkType.Strip_StripUVH2:
					return new PolyChunkStrip(file, address);
				case ChunkType.End:
					return new PolyChunkEnd(file, address);
				default:
					throw new NotSupportedException("Unsupported chunk type " + type + " at " + address.ToString("X8") + ".");
			}
		}

		public abstract byte[] GetBytes();

		object ICloneable.Clone() => Clone();

		public virtual PolyChunk Clone() => (PolyChunk)MemberwiseClone();
	}

	[Serializable]
	public abstract class PolyChunkBits : PolyChunk
	{
		public override int ByteSize
		{
			get { return 2; }
		}

		public override byte[] GetBytes()
		{
			return ByteConverter.GetBytes(Header);
		}
	}

	[Serializable]
	public class PolyChunkNull : PolyChunkBits
	{
		public PolyChunkNull()
		{
			Type = ChunkType.Null;
		}

		public PolyChunkNull(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
		}
	}

	[Serializable]
	internal class PolyChunkEnd : PolyChunkBits
	{
		public PolyChunkEnd()
		{
			Type = ChunkType.End;
		}

		public PolyChunkEnd(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
		}
	}

	[Serializable]
	public class PolyChunkBitsBlendAlpha : PolyChunkBits
	{
		public AlphaInstruction SourceAlpha
		{
			get { return (AlphaInstruction)((Flags >> 3) & 7); }
			set { Flags = (byte)((Flags & ~0x38) | ((byte)value << 3)); }
		}

		public AlphaInstruction DestinationAlpha
		{
			get { return (AlphaInstruction)(Flags & 7); }
			set { Flags = (byte)((Flags & ~7) | (byte)value); }
		}

		public PolyChunkBitsBlendAlpha()
		{
			Type = ChunkType.Bits_BlendAlpha;
		}

		public PolyChunkBitsBlendAlpha(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
		}
	}

	[Serializable]
	public class PolyChunkBitsMipmapDAdjust : PolyChunkBits
	{
		public float MipmapDAdjust
		{
			get { return (Flags & 0xF) * 0.25f; }
			set
			{
				Flags = (byte)((Flags & 0xF0) | (byte)Math.Max(0, Math.Min(0xF, Math.Round(value / 0.25, MidpointRounding.AwayFromZero))));
			}
		}

		public PolyChunkBitsMipmapDAdjust()
		{
			Type = ChunkType.Bits_MipmapDAdjust;
		}

		public PolyChunkBitsMipmapDAdjust(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
		}
	}

	[Serializable]
	public class PolyChunkBitsSpecularExponent : PolyChunkBits
	{
		public byte SpecularExponent
		{
			get { return (byte)(Flags & 0x1F); }
			set { Flags = (byte)((Flags & ~0x1F) | Math.Min(value, (byte)16)); }
		}

		public PolyChunkBitsSpecularExponent()
		{
			Type = ChunkType.Bits_SpecularExponent;
		}

		public PolyChunkBitsSpecularExponent(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
		}
	}

	[Serializable]
	public class PolyChunkBitsCachePolygonList : PolyChunkBits
	{
		public byte List
		{
			get { return Flags; }
			set { Flags = value; }
		}

		public PolyChunkBitsCachePolygonList()
		{
			Type = ChunkType.Bits_CachePolygonList;
		}

		public PolyChunkBitsCachePolygonList(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
		}
	}

	[Serializable]
	public class PolyChunkBitsDrawPolygonList : PolyChunkBits
	{
		public byte List
		{
			get { return Flags; }
			set { Flags = value; }
		}

		public PolyChunkBitsDrawPolygonList()
		{
			Type = ChunkType.Bits_DrawPolygonList;
		}

		public PolyChunkBitsDrawPolygonList(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
		}
	}

	[Serializable]
	public class PolyChunkTinyTextureID : PolyChunk
	{
		public bool Second { get; set; }

		public float MipmapDAdjust
		{
			get { return (Flags & 0xF) * 0.25f; }
			set
			{
				Flags = (byte)((Flags & 0xF0) | (byte)Math.Max(0, Math.Min(0xF, Math.Round(value / 0.25, MidpointRounding.AwayFromZero))));
			}
		}

		public bool ClampV
		{
			get { return (Flags & 0x10) == 0x10; }
			set { Flags = (byte)((Flags & ~0x10) | (value ? 0x10 : 0)); }
		}

		public bool ClampU
		{
			get { return (Flags & 0x20) == 0x20; }
			set { Flags = (byte)((Flags & ~0x20) | (value ? 0x20 : 0)); }
		}

		public bool FlipV
		{
			get { return (Flags & 0x40) == 0x40; }
			set { Flags = (byte)((Flags & ~0x40) | (value ? 0x40 : 0)); }
		}

		public bool FlipU
		{
			get { return (Flags & 0x80) == 0x80; }
			set { Flags = (byte)((Flags & ~0x80) | (value ? 0x80 : 0)); }
		}

		public ushort Data { get; set; }

		public ushort TextureID
		{
			get { return (ushort)(Data & 0x1FFF); }
			set { Data = (ushort)((Data & ~0x1FFF) | Math.Min(value, (ushort)0x1FFF)); }
		}

		public bool SuperSample
		{
			get { return (Data & 0x2000) == 0x2000; }
			set { Data = (ushort)((Data & ~0x2000) | (value ? 0x2000 : 0)); }
		}

		public FilterMode FilterMode
		{
			get { return (FilterMode)(Data >> 14); }
			set { Data = (ushort)((Data & ~0xC000) | ((ushort)value << 14)); }
		}

		public override int ByteSize
		{
			get { return 4; }
		}

		public PolyChunkTinyTextureID()
		{
			Type = ChunkType.Tiny_TextureID;
		}

		public PolyChunkTinyTextureID(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
			Second = Type == ChunkType.Tiny_TextureID2;
			Data = ByteConverter.ToUInt16(file, address + 2);
		}

		public override byte[] GetBytes()
		{
			Type = Second ? ChunkType.Tiny_TextureID2 : ChunkType.Tiny_TextureID;
			List<byte> result = new List<byte>(4);
			result.AddRange(ByteConverter.GetBytes(Header));
			result.AddRange(ByteConverter.GetBytes(Data));
			return result.ToArray();
		}
	}

	[Serializable]
	public abstract class PolyChunkSize : PolyChunk
	{
		public ushort Size { get; protected set; }

		public override int ByteSize
		{
			get { return (Size * 2) + 4; }
		}

		public override byte[] GetBytes()
		{
			List<byte> result = new List<byte>(4);
			result.AddRange(ByteConverter.GetBytes(Header));
			result.AddRange(ByteConverter.GetBytes(Size));
			return result.ToArray();
		}
	}

	[Serializable]
	public class PolyChunkMaterial : PolyChunkSize
	{
		public AlphaInstruction SourceAlpha
		{
			get { return (AlphaInstruction)((Flags >> 3) & 7); }
			set { Flags = (byte)((Flags & ~0x38) | ((byte)value << 3)); }
		}

		public AlphaInstruction DestinationAlpha
		{
			get { return (AlphaInstruction)(Flags & 7); }
			set { Flags = (byte)((Flags & ~7) | (byte)value); }
		}

		public Color? Diffuse { get; set; }
		public Color? Ambient { get; set; }
		public Color? Specular { get; set; }
		public byte SpecularExponent { get; set; }
		public bool Second { get; set; }

		public PolyChunkMaterial()
		{
			Type = ChunkType.Material_Diffuse;
			Diffuse = new Color(0xB2, 0xB2, 0xB2, 0xFF);
		}

		public PolyChunkMaterial(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			Size = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			switch (Type)
			{
				case ChunkType.Material_Diffuse:
				case ChunkType.Material_DiffuseAmbient:
				case ChunkType.Material_DiffuseSpecular:
				case ChunkType.Material_DiffuseAmbientSpecular:
				case ChunkType.Material_Diffuse2:
				case ChunkType.Material_DiffuseAmbient2:
				case ChunkType.Material_DiffuseSpecular2:
				case ChunkType.Material_DiffuseAmbientSpecular2:
					Diffuse = VColor.FromBytes(file, address, ColorType.ARGB8888_16);
					address += VColor.Size(ColorType.ARGB8888_16);
					break;
			}
			switch (Type)
			{
				case ChunkType.Material_Ambient:
				case ChunkType.Material_DiffuseAmbient:
				case ChunkType.Material_AmbientSpecular:
				case ChunkType.Material_DiffuseAmbientSpecular:
				case ChunkType.Material_Ambient2:
				case ChunkType.Material_DiffuseAmbient2:
				case ChunkType.Material_AmbientSpecular2:
				case ChunkType.Material_DiffuseAmbientSpecular2:
					Ambient = VColor.FromBytes(file, address, ColorType.XRGB8888_16);
					address += VColor.Size(ColorType.XRGB8888_16);
					break;
			}
			switch (Type)
			{
				case ChunkType.Material_Specular:
				case ChunkType.Material_DiffuseSpecular:
				case ChunkType.Material_AmbientSpecular:
				case ChunkType.Material_DiffuseAmbientSpecular:
				case ChunkType.Material_Specular2:
				case ChunkType.Material_DiffuseSpecular2:
				case ChunkType.Material_AmbientSpecular2:
				case ChunkType.Material_DiffuseAmbientSpecular2:
					Specular = VColor.FromBytes(file, address, ColorType.XRGB8888_16);
					SpecularExponent = (byte)(ByteConverter.ToUInt16(file, address + 2) >> 8);
					address += VColor.Size(ColorType.XRGB8888_16);
					break;
			}
			switch (Type)
			{
				case ChunkType.Material_Diffuse2:
				case ChunkType.Material_Ambient2:
				case ChunkType.Material_DiffuseAmbient2:
				case ChunkType.Material_Specular2:
				case ChunkType.Material_DiffuseSpecular2:
				case ChunkType.Material_AmbientSpecular2:
				case ChunkType.Material_DiffuseAmbientSpecular2:
					Second = true;
					break;
			}
		}

		public override byte[] GetBytes()
		{
			byte t = (byte)ChunkType.Material;
			Size = 0;
			if (Diffuse.HasValue)
			{
				t |= 1;
				Size += 2;
			}
			if (Ambient.HasValue)
			{
				t |= 2;
				Size += 2;
			}
			if (Specular.HasValue)
			{
				t |= 4;
				Size += 2;
			}
			if (Second)
				t |= 8;
			Type = (ChunkType)t;
			List<byte> result = new List<byte>(base.GetBytes());
			if (Diffuse.HasValue)
				result.AddRange(VColor.GetBytes(Diffuse.Value, ColorType.ARGB8888_16));
			if (Ambient.HasValue)
				result.AddRange(VColor.GetBytes(Ambient.Value, ColorType.XRGB8888_16));
			if (Specular.HasValue)
			{
				int i = Specular.Value.ToRgba();
				result.AddRange(ByteConverter.GetBytes((ushort)(i & 0xFFFF)));
				result.AddRange(ByteConverter.GetBytes((ushort)(((i >> 16) & 0xFF) | SpecularExponent << 8)));
			}
			return result.ToArray();
		}
	}

	[Serializable]
	public class PolyChunkMaterialBump : PolyChunkSize
	{
		public ushort DX { get; set; }
		public ushort DY { get; set; }
		public ushort DZ { get; set; }
		public ushort UX { get; set; }
		public ushort UY { get; set; }
		public ushort UZ { get; set; }

		public PolyChunkMaterialBump()
		{
			Type = ChunkType.Material_Bump;
			Size = 12;
		}

		public PolyChunkMaterialBump(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			Size = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			DX = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			DY = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			DZ = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			UX = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			UY = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			UZ = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
		}

		public override byte[] GetBytes()
		{
			List<byte> result = new List<byte>(base.GetBytes());
			result.AddRange(ByteConverter.GetBytes(DX));
			result.AddRange(ByteConverter.GetBytes(DY));
			result.AddRange(ByteConverter.GetBytes(DZ));
			result.AddRange(ByteConverter.GetBytes(UX));
			result.AddRange(ByteConverter.GetBytes(UY));
			result.AddRange(ByteConverter.GetBytes(UZ));
			return result.ToArray();
		}
	}

	[Serializable]
	public class PolyChunkVolume : PolyChunkSize
	{
		[Serializable]
		public sealed class Triangle : Poly
		{
			public ushort? UserFlags1 { get; private set; }
			public ushort? UserFlags2 { get; private set; }
			public ushort? UserFlags3 { get; private set; }

			public Triangle()
			{
				Indexes = new ushort[3];
			}

			public Triangle(byte[] file, int address, byte userFlags)
				: this()
			{
				Indexes[0] = ByteConverter.ToUInt16(file, address);
				Indexes[1] = ByteConverter.ToUInt16(file, address + 2);
				Indexes[2] = ByteConverter.ToUInt16(file, address + 4);
				if (userFlags > 0)
				{
					UserFlags1 = ByteConverter.ToUInt16(file, address + 6);
					if (userFlags > 1)
					{
						UserFlags2 = ByteConverter.ToUInt16(file, address + 8);
						if (userFlags > 2)
							UserFlags3 = ByteConverter.ToUInt16(file, address + 10);
					}
				}
			}

			public override byte[] GetBytes()
			{
				List<byte> result = new List<byte>();
				foreach (ushort item in Indexes)
					result.AddRange(ByteConverter.GetBytes(item));
				if (UserFlags1.HasValue)
				{
					result.AddRange(ByteConverter.GetBytes(UserFlags1.Value));
					if (UserFlags2.HasValue)
					{
						result.AddRange(ByteConverter.GetBytes(UserFlags2.Value));
						if (UserFlags3.HasValue)
							result.AddRange(ByteConverter.GetBytes(UserFlags3.Value));
					}
				}
				return result.ToArray();
			}

			public override int Size
			{
				get
				{
					int size = 6;
					if (UserFlags1.HasValue)
					{
						size += 2;
						if (UserFlags2.HasValue)
						{
							size += 2;
							if (UserFlags3.HasValue)
								size += 2;
						}
					}
					return size;
				}
			}
		}

		[Serializable]
		public sealed class Quad : Poly
		{
			public ushort? UserFlags1 { get; private set; }
			public ushort? UserFlags2 { get; private set; }
			public ushort? UserFlags3 { get; private set; }

			public Quad()
			{
				Indexes = new ushort[4];
			}

			public Quad(byte[] file, int address, byte userFlags)
				: this()
			{
				Indexes[0] = ByteConverter.ToUInt16(file, address);
				Indexes[1] = ByteConverter.ToUInt16(file, address + 2);
				Indexes[2] = ByteConverter.ToUInt16(file, address + 4);
				Indexes[3] = ByteConverter.ToUInt16(file, address + 6);
				if (userFlags > 0)
				{
					UserFlags1 = ByteConverter.ToUInt16(file, address + 8);
					if (userFlags > 1)
					{
						UserFlags2 = ByteConverter.ToUInt16(file, address + 10);
						if (userFlags > 2)
							UserFlags3 = ByteConverter.ToUInt16(file, address + 12);
					}
				}
			}

			public override byte[] GetBytes()
			{
				List<byte> result = new List<byte>();
				foreach (ushort item in Indexes)
					result.AddRange(ByteConverter.GetBytes(item));
				if (UserFlags1.HasValue)
				{
					result.AddRange(ByteConverter.GetBytes(UserFlags1.Value));
					if (UserFlags2.HasValue)
					{
						result.AddRange(ByteConverter.GetBytes(UserFlags2.Value));
						if (UserFlags3.HasValue)
							result.AddRange(ByteConverter.GetBytes(UserFlags3.Value));
					}
				}
				return result.ToArray();
			}

			public override int Size
			{
				get
				{
					int size = 8;
					if (UserFlags1.HasValue)
					{
						size += 2;
						if (UserFlags2.HasValue)
						{
							size += 2;
							if (UserFlags3.HasValue)
								size += 2;
						}
					}
					return size;
				}
			}
		}

		[Serializable]
		public sealed class Strip : Poly
		{
			public bool Reversed { get; private set; }
			public ushort[] UserFlags1 { get; private set; }
			public ushort[] UserFlags2 { get; private set; }
			public ushort[] UserFlags3 { get; private set; }

			public Strip(int NumVerts, bool Reverse)
			{
				Indexes = new ushort[NumVerts];
				Reversed = Reverse;
			}

			public Strip(ushort[] Verts, bool Reverse)
			{
				Indexes = Verts;
				Reversed = Reverse;
			}

			public Strip(byte[] file, int address, byte userFlags)
			{
				Indexes = new ushort[ByteConverter.ToUInt16(file, address) & 0x7FFF];
				Reversed = (ByteConverter.ToUInt16(file, address) & 0x8000) == 0x8000;
				if (userFlags > 0)
					UserFlags1 = new ushort[Indexes.Length - 2];
				if (userFlags > 1)
					UserFlags2 = new ushort[Indexes.Length - 2];
				if (userFlags > 2)
					UserFlags3 = new ushort[Indexes.Length - 2];
				address += 2;
				for (int i = 0; i < Indexes.Length; i++)
				{
					Indexes[i] = ByteConverter.ToUInt16(file, address);
					address += 2;
					if (i > 1)
					{
						if (userFlags > 0)
						{
							UserFlags1[i - 2] = ByteConverter.ToUInt16(file, address);
							address += 2;
							if (userFlags > 1)
							{
								UserFlags2[i - 2] = ByteConverter.ToUInt16(file, address);
								address += 2;
								if (userFlags > 2)
								{
									UserFlags3[i - 2] = ByteConverter.ToUInt16(file, address);
									address += 2;
								}
							}
						}
					}
				}
			}

			public override int Size
			{
				get
				{
					int size = 2;
					size += Indexes.Length * 2;
					if (UserFlags1 != null)
					{
						size += UserFlags1.Length * 2;
						if (UserFlags2 != null)
						{
							size += UserFlags2.Length * 2;
							if (UserFlags3 != null)
								size += UserFlags3.Length * 2;
						}
					}
					return size;
				}
			}

			public override byte[] GetBytes()
			{
				List<byte> result = new List<byte>();
				int ind = Indexes.Length;
				if (Reversed)
					ind = -ind;
				result.AddRange(ByteConverter.GetBytes((short)(ind)));
				for (int i = 0; i < Indexes.Length; i++)
				{
					result.AddRange(ByteConverter.GetBytes(Indexes[i]));
					if (i > 1)
					{
						if (UserFlags1 != null)
						{
							result.AddRange(ByteConverter.GetBytes(UserFlags1[i - 2]));
							if (UserFlags2 != null)
							{
								result.AddRange(ByteConverter.GetBytes(UserFlags2[i - 2]));
								if (UserFlags3 != null)
									result.AddRange(ByteConverter.GetBytes(UserFlags3[i - 2]));
							}
						}
					}
				}
				return result.ToArray();
			}

			public override Poly Clone()
			{
				Strip result = (Strip)base.Clone();
				if (result.UserFlags1 != null)
					result.UserFlags1 = (ushort[])UserFlags1.Clone();
				if (result.UserFlags2 != null)
					result.UserFlags2 = (ushort[])UserFlags2.Clone();
				if (result.UserFlags3 != null)
					result.UserFlags3 = (ushort[])UserFlags3.Clone();
				return result;
			}
		}

		[Serializable]
		public abstract class Poly : ICloneable
		{
			public ushort[] Indexes { get; protected set; }

			internal Poly()
			{
			}

			public static Poly CreatePoly(ChunkType type)
			{
				switch (type)
				{
					case ChunkType.Volume_Polygon3:
						return new Triangle();
					case ChunkType.Volume_Polygon4:
						return new Quad();
					case ChunkType.Volume_Strip:
						throw new ArgumentException(
							"Cannot create strip-type poly without additional information.\nUse Strip.Strip(int NumVerts, bool Reverse) instead.",
							nameof(type));
				}
				throw new ArgumentException("Unknown poly type!", nameof(type));
			}

			public static Poly CreatePoly(ChunkType type, byte[] file, int address, byte userFlags)
			{
				switch (type)
				{
					case ChunkType.Volume_Polygon3:
						return new Triangle(file, address, userFlags);
					case ChunkType.Volume_Polygon4:
						return new Quad(file, address, userFlags);
					case ChunkType.Volume_Strip:
						return new Strip(file, address, userFlags);
				}
				throw new ArgumentException("Unknown poly type!", nameof(type));
			}

			public abstract int Size { get; }

			public abstract byte[] GetBytes();

			object ICloneable.Clone() => Clone();

			public virtual Poly Clone()
			{
				Poly result = (Poly)MemberwiseClone();
				Indexes = (ushort[])Indexes.Clone();
				return result;
			}
		}

		public ushort Header2 { get; private set; }

		public byte UserFlags
		{
			get { return (byte)(Header2 >> 14); }
			private set { Header2 = (ushort)((Header2 & 0x3FFF) | ((value & 3) << 14)); }
		}

		public ushort PolyCount
		{
			get { return (ushort)Polys.Count; }
			private set { Header2 = (ushort)((Header2 & 0xC000) | (value & 0x3FFF)); }
		}

		public List<Poly> Polys { get; private set; }

		public PolyChunkVolume(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			Size = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			Header2 = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			int polyCount = Header2 & 0x3FFF;
			Polys = new List<Poly>(polyCount);
			for (int i = 0; i < polyCount; i++)
			{
				Poly str = Poly.CreatePoly(Type, file, address, UserFlags);
				Polys.Add(str);
				address += str.Size;
			}
		}

		public override byte[] GetBytes()
		{
			PolyCount = (ushort)Polys.Count;
			Size = 1;
			foreach (Poly str in Polys)
				Size += (ushort)(str.Size / 2);
			List<byte> result = new List<byte>(base.GetBytes());
			result.AddRange(ByteConverter.GetBytes(Header2));
			foreach (Poly str in Polys)
				result.AddRange(str.GetBytes());
			return result.ToArray();
		}

		public override PolyChunk Clone()
		{
			PolyChunkVolume result = (PolyChunkVolume)base.Clone();
			result.Polys = new List<Poly>(Polys.Count);
			foreach (Poly item in Polys)
				result.Polys.Add(item.Clone());
			return result;
		}
	}

	[Serializable]
	public class PolyChunkStrip : PolyChunkSize
	{
		[Serializable]
		public class Strip : ICloneable
		{
			public bool Reversed { get; private set; }
			public ushort[] Indexes { get; private set; }
			public UV[] UVs { get; private set; }
			public Color[] VColors { get; private set; }
			public ushort[] UserFlags1 { get; private set; }
			public ushort[] UserFlags2 { get; private set; }
			public ushort[] UserFlags3 { get; private set; }

			public Strip(bool reversed, ushort[] indexes, UV[] uvs, Color[] vcolors)
			{
				Reversed = reversed;
				Indexes = indexes;
				UVs = uvs;
				VColors = vcolors;
			}

			public Strip(byte[] file, int address, ChunkType type, byte userFlags)
			{
				Indexes = new ushort[Math.Abs(ByteConverter.ToInt16(file, address))];
				Reversed = (ByteConverter.ToUInt16(file, address) & 0x8000) == 0x8000;
				address += 2;
				switch (type)
				{
					case ChunkType.Strip_StripUVN:
					case ChunkType.Strip_StripUVH:
					case ChunkType.Strip_StripUVN2:
					case ChunkType.Strip_StripUVH2:
						UVs = new UV[Indexes.Length];
						break;
					case ChunkType.Strip_StripColor:
						VColors = new Color[Indexes.Length];
						break;
					case ChunkType.Strip_StripUVNColor:
					case ChunkType.Strip_StripUVHColor:
						UVs = new UV[Indexes.Length];
						VColors = new Color[Indexes.Length];
						break;
				}
				if (userFlags > 0)
					UserFlags1 = new ushort[Indexes.Length - 2];
				if (userFlags > 1)
					UserFlags2 = new ushort[Indexes.Length - 2];
				if (userFlags > 2)
					UserFlags3 = new ushort[Indexes.Length - 2];
				for (int i = 0; i < Indexes.Length; i++)
				{
					Indexes[i] = ByteConverter.ToUInt16(file, address);
					address += 2;
					switch (type)
					{
						case ChunkType.Strip_StripUVN:
						case ChunkType.Strip_StripUVNColor:
						case ChunkType.Strip_StripUVN2:
							UVs[i] = new UV(file, address, false);
							address += UV.Size;
							break;
						case ChunkType.Strip_StripUVH:
						case ChunkType.Strip_StripUVHColor:
						case ChunkType.Strip_StripUVH2:
							UVs[i] = new UV(file, address, true);
							address += UV.Size;
							break;
					}
					switch (type)
					{
						case ChunkType.Strip_StripColor:
						case ChunkType.Strip_StripUVNColor:
						case ChunkType.Strip_StripUVHColor:
							VColors[i] = VColor.FromBytes(file, address, ColorType.ARGB8888_16);
							address += VColor.Size(ColorType.ARGB8888_16);
							break;
					}
					if (i > 1)
					{
						if (userFlags > 0)
						{
							UserFlags1[i - 2] = ByteConverter.ToUInt16(file, address);
							address += 2;
							if (userFlags > 1)
							{
								UserFlags2[i - 2] = ByteConverter.ToUInt16(file, address);
								address += 2;
								if (userFlags > 2)
								{
									UserFlags3[i - 2] = ByteConverter.ToUInt16(file, address);
									address += 2;
								}
							}
						}
					}
				}
			}

			public byte[] GetBytes(ChunkType type)
			{
				List<byte> result = new List<byte>();
				int ind = Indexes.Length;
				if (Reversed)
					ind = -ind;
				result.AddRange(ByteConverter.GetBytes((short)(ind)));
				for (int i = 0; i < Indexes.Length; i++)
				{
					result.AddRange(ByteConverter.GetBytes(Indexes[i]));
					switch (type)
					{
						case ChunkType.Strip_StripUVN:
						case ChunkType.Strip_StripUVNColor:
						case ChunkType.Strip_StripUVN2:
							result.AddRange(UVs[i].GetBytes(false));
							break;
						case ChunkType.Strip_StripUVH:
						case ChunkType.Strip_StripUVHColor:
						case ChunkType.Strip_StripUVH2:
							result.AddRange(UVs[i].GetBytes(true));
							break;
					}
					switch (type)
					{
						case ChunkType.Strip_StripColor:
						case ChunkType.Strip_StripUVNColor:
						case ChunkType.Strip_StripUVHColor:
							result.AddRange(VColor.GetBytes(VColors[i], ColorType.ARGB8888_16));
							break;
					}
					if (i > 1)
					{
						if (UserFlags1 != null)
						{
							result.AddRange(ByteConverter.GetBytes(UserFlags1[i - 2]));
							if (UserFlags2 != null)
							{
								result.AddRange(ByteConverter.GetBytes(UserFlags2[i - 2]));
								if (UserFlags3 != null)
									result.AddRange(ByteConverter.GetBytes(UserFlags3[i - 2]));
							}
						}
					}
				}
				return result.ToArray();
			}

			public int Size
			{
				get
				{
					int size = 2;
					size += Indexes.Length * 2;
					if (UVs != null)
						size += UVs.Length * UV.Size;
					if (VColors != null)
						size += VColors.Length * VColor.Size(ColorType.ARGB8888_16);
					if (UserFlags1 != null)
					{
						size += UserFlags1.Length * 2;
						if (UserFlags2 != null)
						{
							size += UserFlags2.Length * 2;
							if (UserFlags3 != null)
								size += UserFlags3.Length * 2;
						}
					}
					return size;
				}
			}

			object ICloneable.Clone() => Clone();

			public Strip Clone()
			{
				Strip result = (Strip)MemberwiseClone();
				result.Indexes = (ushort[])Indexes.Clone();
				if (UVs != null)
				{
					result.UVs = new UV[UVs.Length];
					for (int i = 0; i < UVs.Length; i++)
						result.UVs[i] = UVs[i].Clone();
				}
				if (VColors != null) result.VColors = (Color[])VColors.Clone();
				if (UserFlags1 != null) result.UserFlags1 = (ushort[])UserFlags1.Clone();
				if (UserFlags2 != null) result.UserFlags2 = (ushort[])UserFlags2.Clone();
				if (UserFlags3 != null) result.UserFlags3 = (ushort[])UserFlags3.Clone();
				return result;
			}
		}

		public bool IgnoreLight
		{
			get { return (Flags & 1) == 1; }
			set { Flags = (byte)((Flags & ~1) | (value ? 1 : 0)); }
		}

		public bool IgnoreSpecular
		{
			get { return (Flags & 2) == 2; }
			set { Flags = (byte)((Flags & ~2) | (value ? 2 : 0)); }
		}

		public bool IgnoreAmbient
		{
			get { return (Flags & 4) == 4; }
			set { Flags = (byte)((Flags & ~4) | (value ? 4 : 0)); }
		}

		public bool UseAlpha
		{
			get { return (Flags & 8) == 8; }
			set { Flags = (byte)((Flags & ~8) | (value ? 8 : 0)); }
		}

		public bool DoubleSide
		{
			get { return (Flags & 0x10) == 0x10; }
			set { Flags = (byte)((Flags & ~0x10) | (value ? 0x10 : 0)); }
		}

		public bool FlatShading
		{
			get { return (Flags & 0x20) == 0x20; }
			set { Flags = (byte)((Flags & ~0x20) | (value ? 0x20 : 0)); }
		}

		public bool EnvironmentMapping
		{
			get { return (Flags & 0x40) == 0x40; }
			set { Flags = (byte)((Flags & ~0x40) | (value ? 0x40 : 0)); }
		}

		public ushort Header2 { get; private set; }

		public byte UserFlags
		{
			get { return (byte)(Header2 >> 14); }
			private set { Header2 = (ushort)((Header2 & 0x3FFF) | ((value & 3) << 14)); }
		}

		public ushort StripCount
		{
			get { return (ushort)Strips.Count; }
			private set { Header2 = (ushort)((Header2 & 0xC000) | (value & 0x3FFF)); }
		}

		public List<Strip> Strips { get; private set; }

		public PolyChunkStrip(ChunkType type)
		{
			Type = type;
			Strips = new List<Strip>();
		}

		public PolyChunkStrip(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			Size = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			Header2 = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			int stripCount = Header2 & 0x3FFF;
			Strips = new List<Strip>(stripCount);
			for (int i = 0; i < stripCount; i++)
			{
				Strip str = new Strip(file, address, Type, UserFlags);
				Strips.Add(str);
				address += str.Size;
			}
		}

		public override byte[] GetBytes()
		{
			StripCount = (ushort)Strips.Count;
			Size = 1;
			foreach (Strip str in Strips)
				Size += (ushort)(str.Size / 2);
			List<byte> result = new List<byte>(base.GetBytes());
			result.AddRange(ByteConverter.GetBytes(Header2));
			foreach (Strip str in Strips)
				result.AddRange(str.GetBytes(Type));
			return result.ToArray();
		}

		public override PolyChunk Clone()
		{
			PolyChunkStrip result = (PolyChunkStrip)base.Clone();
			result.Strips = new List<Strip>(StripCount);
			foreach (Strip item in Strips)
				result.Strips.Add(item.Clone());
			return result;
		}
	}
}