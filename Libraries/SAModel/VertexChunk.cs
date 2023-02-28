using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace SAModel
{
	[Serializable]
	public class VertexChunk : ICloneable
	{
		public uint Header1 { get; set; }

		public ChunkType Type
		{
			get { return (ChunkType)(Header1 & 0xFF); }
			private set { Header1 = (Header1 & 0xFFFFFF00u) | (byte)value; }
		}

		public byte Flags
		{
			get { return (byte)((Header1 >> 8) & 0xFF); }
			set { Header1 = (Header1 & 0xFFFF00FFu) | (uint)(value << 8); }
		}

		public WeightStatus WeightStatus
		{
			get { return (WeightStatus)(Flags & 3); }
			set { Flags = (byte)((Flags & ~3) | (int)value); }
		}

		public ushort Size
		{
			get { return (ushort)(Header1 >> 16); }
			set { Header1 = (Header1 & 0xFFFFu) | (uint)(value << 16); }
		}

		public uint Header2 { get; set; }

		public ushort IndexOffset
		{
			get { return (ushort)(Header2 & 0xFFFF); }
			set { Header2 = (Header2 & 0xFFFF0000u) | value; }
		}

		private uint GetVertCount() => Header2 >> 16;

		private void SetVertCount(int count) => Header2 = (Header2 & 0xFFFFu) | (uint)(count << 16);

		public int VertexCount => Vertices.Count;

		public bool HasWeight { get { return Type == ChunkType.Vertex_VertexNinjaFlags | Type == ChunkType.Vertex_VertexNormalNinjaFlags; } }

		public List<Vertex> Vertices { get; set; }
		public List<Vertex> Normals { get; set; }
		public List<Color> Diffuse { get; set; }
		public List<Color> Specular { get; set; }
		public List<uint> UserFlags { get; set; }
		public List<uint> NinjaFlags { get; set; }

		public VertexChunk()
		{
			Type = ChunkType.Vertex_Vertex;
			Vertices = new List<Vertex>();
			Normals = new List<Vertex>();
			Diffuse = new List<Color>();
			Specular = new List<Color>();
			UserFlags = new List<uint>();
			NinjaFlags = new List<uint>();
		}

		public VertexChunk(ChunkType type)
			: this()
		{
			Type = type;
			switch (type)
			{
				case ChunkType.Vertex_VertexSH:
				case ChunkType.Vertex_Vertex:
				case ChunkType.Vertex_VertexDiffuse8:
				case ChunkType.Vertex_VertexUserFlags:
				case ChunkType.Vertex_VertexNinjaFlags:
				case ChunkType.Vertex_VertexDiffuseSpecular5:
				case ChunkType.Vertex_VertexDiffuseSpecular4:
				case ChunkType.Vertex_VertexNormalSH:
				case ChunkType.Vertex_VertexNormal:
				case ChunkType.Vertex_VertexNormalDiffuse8:
				case ChunkType.Vertex_VertexNormalUserFlags:
				case ChunkType.Vertex_VertexNormalNinjaFlags:
				case ChunkType.Vertex_VertexNormalDiffuseSpecular5:
				case ChunkType.Vertex_VertexNormalDiffuseSpecular4:
				case ChunkType.End:
					break;
				default:
					throw new NotSupportedException("Unsupported chunk type " + type + ".");
			}
		}

		public VertexChunk(byte[] file, int address)
			: this()
		{
			Header1 = ByteConverter.ToUInt32(file, address);
			Header2 = ByteConverter.ToUInt32(file, address + 4);
			address += 8;
			for (int i = 0; i < GetVertCount(); i++)
			{
				switch (Type)
				{
					case ChunkType.Vertex_VertexSH:
						Vertices.Add(new Vertex(file, address));
						address += Vertex.Size + sizeof(float);
						break;
					case ChunkType.Vertex_VertexNormalSH:
						Vertices.Add(new Vertex(file, address));
						address += Vertex.Size + sizeof(float);
						Normals.Add(new Vertex(file, address));
						address += Vertex.Size + sizeof(float);
						break;
					case ChunkType.Vertex_Vertex:
						Vertices.Add(new Vertex(file, address));
						address += Vertex.Size;
						break;
					case ChunkType.Vertex_VertexDiffuse8:
						Vertices.Add(new Vertex(file, address));
						address += Vertex.Size;
						Diffuse.Add(VColor.FromBytes(file, address, ColorType.ARGB8888_32));
						address += VColor.Size(ColorType.ARGB8888_32);
						break;
					case ChunkType.Vertex_VertexUserFlags:
						Vertices.Add(new Vertex(file, address));
						address += Vertex.Size;
						UserFlags.Add(ByteConverter.ToUInt32(file, address));
						address += sizeof(uint);
						break;
					case ChunkType.Vertex_VertexNinjaFlags:
						Vertices.Add(new Vertex(file, address));
						address += Vertex.Size;
						NinjaFlags.Add(ByteConverter.ToUInt32(file, address));
						address += sizeof(uint);
						break;
					case ChunkType.Vertex_VertexDiffuseSpecular5:
						Vertices.Add(new Vertex(file, address));
						address += Vertex.Size;
						uint tmpcolor = ByteConverter.ToUInt32(file, address);
						address += sizeof(uint);
						Diffuse.Add(VColor.FromBytes(ByteConverter.GetBytes((ushort)(tmpcolor & 0xFFFF)), 0, ColorType.RGB565));
						Specular.Add(VColor.FromBytes(ByteConverter.GetBytes((ushort)(tmpcolor >> 16)), 0, ColorType.RGB565));
						break;
					case ChunkType.Vertex_VertexDiffuseSpecular4:
						Vertices.Add(new Vertex(file, address));
						address += Vertex.Size;
						tmpcolor = ByteConverter.ToUInt32(file, address);
						address += sizeof(uint);
						Diffuse.Add(VColor.FromBytes(ByteConverter.GetBytes((ushort)(tmpcolor & 0xFFFF)), 0, ColorType.ARGB4444));
						Specular.Add(VColor.FromBytes(ByteConverter.GetBytes((ushort)(tmpcolor >> 16)), 0, ColorType.RGB565));
						break;
					case ChunkType.Vertex_VertexNormal:
						Vertices.Add(new Vertex(file, address));
						address += Vertex.Size;
						Normals.Add(new Vertex(file, address));
						address += Vertex.Size;
						break;
					case ChunkType.Vertex_VertexNormalDiffuse8:
						Vertices.Add(new Vertex(file, address));
						address += Vertex.Size;
						Normals.Add(new Vertex(file, address));
						address += Vertex.Size;
						Diffuse.Add(VColor.FromBytes(file, address, ColorType.ARGB8888_32));
						address += VColor.Size(ColorType.ARGB8888_32);
						break;
					case ChunkType.Vertex_VertexNormalUserFlags:
						Vertices.Add(new Vertex(file, address));
						address += Vertex.Size;
						Normals.Add(new Vertex(file, address));
						address += Vertex.Size;
						UserFlags.Add(ByteConverter.ToUInt32(file, address));
						address += sizeof(uint);
						break;
					case ChunkType.Vertex_VertexNormalNinjaFlags:
						Vertices.Add(new Vertex(file, address));
						address += Vertex.Size;
						Normals.Add(new Vertex(file, address));
						address += Vertex.Size;
						NinjaFlags.Add(ByteConverter.ToUInt32(file, address));
						address += sizeof(uint);
						break;
					case ChunkType.Vertex_VertexNormalDiffuseSpecular5:
						Vertices.Add(new Vertex(file, address));
						address += Vertex.Size;
						Normals.Add(new Vertex(file, address));
						address += Vertex.Size;
						tmpcolor = ByteConverter.ToUInt32(file, address);
						address += sizeof(uint);
						Diffuse.Add(VColor.FromBytes(ByteConverter.GetBytes((ushort)(tmpcolor & 0xFFFF)), 0, ColorType.RGB565));
						Specular.Add(VColor.FromBytes(ByteConverter.GetBytes((ushort)(tmpcolor >> 16)), 0, ColorType.RGB565));
						break;
					case ChunkType.Vertex_VertexNormalDiffuseSpecular4:
						Vertices.Add(new Vertex(file, address));
						address += Vertex.Size;
						Normals.Add(new Vertex(file, address));
						address += Vertex.Size;
						tmpcolor = ByteConverter.ToUInt32(file, address);
						address += sizeof(uint);
						Diffuse.Add(VColor.FromBytes(ByteConverter.GetBytes((ushort)(tmpcolor & 0xFFFF)), 0, ColorType.ARGB4444));
						Specular.Add(VColor.FromBytes(ByteConverter.GetBytes((ushort)(tmpcolor >> 16)), 0, ColorType.RGB565));
						break;
					default:
						throw new NotSupportedException("Unsupported chunk type " + Type + " at " + address.ToString("X8") + ".");
				}
			}
		}

		public byte[] GetBytes()
		{
			VertexChunk next = null;
			int vertlimit;
			int vertcount = Vertices.Count;
			switch (Type)
			{
				case ChunkType.Vertex_VertexSH:
					vertlimit = 65535 / 4;
					if (Vertices.Count > vertlimit)
					{
						next = new VertexChunk(Type) { Vertices = Vertices.Skip(vertlimit).ToList() };
						vertcount = vertlimit;
					}
					break;
				case ChunkType.Vertex_VertexNormalSH:
					vertlimit = 65535 / 8;
					if (Vertices.Count > vertlimit)
					{
						next = new VertexChunk(Type) { Vertices = Vertices.Skip(vertlimit).ToList(), Normals = Normals.Skip(vertlimit).ToList() };
						vertcount = vertlimit;
					}
					break;
				case ChunkType.Vertex_Vertex:
					vertlimit = 65535 / 3;
					if (Vertices.Count > vertlimit)
					{
						next = new VertexChunk(Type) { Vertices = Vertices.Skip(vertlimit).ToList() };
						vertcount = vertlimit;
					}
					break;
				case ChunkType.Vertex_VertexDiffuse8:
					vertlimit = 65535 / 4;
					if (Vertices.Count > vertlimit)
					{
						next = new VertexChunk(Type) { Vertices = Vertices.Skip(vertlimit).ToList(), Diffuse = Diffuse.Skip(vertlimit).ToList() };
						vertcount = vertlimit;
					}
					break;
				case ChunkType.Vertex_VertexUserFlags:
					vertlimit = 65535 / 4;
					if (Vertices.Count > vertlimit)
					{
						next = new VertexChunk(Type) { Vertices = Vertices.Skip(vertlimit).ToList(), UserFlags = UserFlags.Skip(vertlimit).ToList() };
						vertcount = vertlimit;
					}
					break;
				case ChunkType.Vertex_VertexNinjaFlags:
					vertlimit = 65535 / 4;
					if (Vertices.Count > vertlimit)
					{
						next = new VertexChunk(Type) { Vertices = Vertices.Skip(vertlimit).ToList(), NinjaFlags = NinjaFlags.Skip(vertlimit).ToList() };
						vertcount = vertlimit;
					}
					break;
				case ChunkType.Vertex_VertexDiffuseSpecular5:
				case ChunkType.Vertex_VertexDiffuseSpecular4:
					vertlimit = 65535 / 4;
					if (Vertices.Count > vertlimit)
					{
						next = new VertexChunk(Type)
						{
							Vertices = Vertices.Skip(vertlimit).ToList(),
							Diffuse = Diffuse.Skip(vertlimit).ToList(),
							Specular = Specular.Skip(vertlimit).ToList()
						};
						vertcount = vertlimit;
					}
					break;
				case ChunkType.Vertex_VertexNormal:
					vertlimit = 65535 / 6;
					if (Vertices.Count > vertlimit)
					{
						next = new VertexChunk(Type) { Vertices = Vertices.Skip(vertlimit).ToList(), Normals = Normals.Skip(vertlimit).ToList() };
						vertcount = vertlimit;
					}
					break;
				case ChunkType.Vertex_VertexNormalDiffuse8:
					vertlimit = 65535 / 7;
					if (Vertices.Count > vertlimit)
					{
						next = new VertexChunk(Type)
						{
							Vertices = Vertices.Skip(vertlimit).ToList(),
							Normals = Normals.Skip(vertlimit).ToList(),
							Diffuse = Diffuse.Skip(vertlimit).ToList()
						};
						vertcount = vertlimit;
					}
					break;
				case ChunkType.Vertex_VertexNormalUserFlags:
					vertlimit = 65535 / 7;
					if (Vertices.Count > vertlimit)
					{
						next = new VertexChunk(Type)
						{
							Vertices = Vertices.Skip(vertlimit).ToList(),
							Normals = Normals.Skip(vertlimit).ToList(),
							UserFlags = UserFlags.Skip(vertlimit).ToList()
						};
						vertcount = vertlimit;
					}
					break;
				case ChunkType.Vertex_VertexNormalNinjaFlags:
					vertlimit = 65535 / 7;
					if (Vertices.Count > vertlimit)
					{
						next = new VertexChunk(Type)
						{
							Vertices = Vertices.Skip(vertlimit).ToList(),
							Normals = Normals.Skip(vertlimit).ToList(),
							NinjaFlags = NinjaFlags.Skip(vertlimit).ToList()
						};
						vertcount = vertlimit;
					}
					break;
				case ChunkType.Vertex_VertexNormalDiffuseSpecular5:
				case ChunkType.Vertex_VertexNormalDiffuseSpecular4:
					vertlimit = 65535 / 7;
					if (Vertices.Count > vertlimit)
					{
						next = new VertexChunk(Type)
						{
							Vertices = Vertices.Skip(vertlimit).ToList(),
							Normals = Normals.Skip(vertlimit).ToList(),
							Diffuse = Diffuse.Skip(vertlimit).ToList(),
							Specular = Specular.Skip(vertlimit).ToList()
						};
						vertcount = vertlimit;
					}
					break;
				case ChunkType.End:
					break;
				default:
					throw new NotSupportedException("Unsupported chunk type " + Type + ".");
			}
			SetVertCount(vertcount);
			switch (Type)
			{
				case ChunkType.Vertex_Vertex:
					Size = (ushort)(vertcount * 3 + 1);
					break;
				case ChunkType.Vertex_VertexSH:
				case ChunkType.Vertex_VertexDiffuse8:
				case ChunkType.Vertex_VertexUserFlags:
				case ChunkType.Vertex_VertexNinjaFlags:
				case ChunkType.Vertex_VertexDiffuseSpecular5:
				case ChunkType.Vertex_VertexDiffuseSpecular4:
					Size = (ushort)(vertcount * 4 + 1);
					break;
				case ChunkType.Vertex_VertexNormal:
					Size = (ushort)(vertcount * 6 + 1);
					break;
				case ChunkType.Vertex_VertexNormalDiffuse8:
				case ChunkType.Vertex_VertexNormalUserFlags:
				case ChunkType.Vertex_VertexNormalNinjaFlags:
				case ChunkType.Vertex_VertexNormalDiffuseSpecular5:
				case ChunkType.Vertex_VertexNormalDiffuseSpecular4:
					Size = (ushort)(vertcount * 7 + 1);
					break;
				case ChunkType.Vertex_VertexNormalSH:
					Size = (ushort)(vertcount * 8 + 1);
					break;
			}
			List<byte> result = new List<byte>((Size * 4) + 4);
			result.AddRange(ByteConverter.GetBytes(Header1));
			result.AddRange(ByteConverter.GetBytes(Header2));
			for (int i = 0; i < vertcount; i++)
			{
				switch (Type)
				{
					case ChunkType.Vertex_VertexSH:
						result.AddRange(Vertices[i].GetBytes());
						result.AddRange(ByteConverter.GetBytes(1.0f));
						break;
					case ChunkType.Vertex_VertexNormalSH:
						result.AddRange(Vertices[i].GetBytes());
						result.AddRange(ByteConverter.GetBytes(1.0f));
						result.AddRange(Normals[i].GetBytes());
						result.AddRange(ByteConverter.GetBytes(1.0f));
						break;
					case ChunkType.Vertex_Vertex:
						result.AddRange(Vertices[i].GetBytes());
						break;
					case ChunkType.Vertex_VertexDiffuse8:
						result.AddRange(Vertices[i].GetBytes());
						result.AddRange(VColor.GetBytes(Diffuse[i], ColorType.ARGB8888_32));
						break;
					case ChunkType.Vertex_VertexUserFlags:
						result.AddRange(Vertices[i].GetBytes());
						result.AddRange(ByteConverter.GetBytes(UserFlags[i]));
						break;
					case ChunkType.Vertex_VertexNinjaFlags:
						result.AddRange(Vertices[i].GetBytes());
						result.AddRange(ByteConverter.GetBytes(NinjaFlags[i]));
						break;
					case ChunkType.Vertex_VertexDiffuseSpecular5:
						result.AddRange(Vertices[i].GetBytes());
						result.AddRange(ByteConverter.GetBytes(
							ByteConverter.ToUInt16(VColor.GetBytes(Diffuse[i], ColorType.RGB565), 0)
							| (ByteConverter.ToUInt16(VColor.GetBytes(Specular[i], ColorType.RGB565), 0) << 16)));
						break;
					case ChunkType.Vertex_VertexDiffuseSpecular4:
						result.AddRange(Vertices[i].GetBytes());
						result.AddRange(ByteConverter.GetBytes(
							ByteConverter.ToUInt16(VColor.GetBytes(Diffuse[i], ColorType.ARGB4444), 0)
							| (ByteConverter.ToUInt16(VColor.GetBytes(Specular[i], ColorType.RGB565), 0) << 16)));
						break;
					case ChunkType.Vertex_VertexNormal:
						result.AddRange(Vertices[i].GetBytes());
						result.AddRange(Normals[i].GetBytes());
						break;
					case ChunkType.Vertex_VertexNormalDiffuse8:
						result.AddRange(Vertices[i].GetBytes());
						result.AddRange(Normals[i].GetBytes());
						result.AddRange(VColor.GetBytes(Diffuse[i], ColorType.ARGB8888_32));
						break;
					case ChunkType.Vertex_VertexNormalUserFlags:
						result.AddRange(Vertices[i].GetBytes());
						result.AddRange(Normals[i].GetBytes());
						result.AddRange(ByteConverter.GetBytes(UserFlags[i]));
						break;
					case ChunkType.Vertex_VertexNormalNinjaFlags:
						result.AddRange(Vertices[i].GetBytes());
						result.AddRange(Normals[i].GetBytes());
						result.AddRange(ByteConverter.GetBytes(NinjaFlags[i]));
						break;
					case ChunkType.Vertex_VertexNormalDiffuseSpecular5:
						result.AddRange(Vertices[i].GetBytes());
						result.AddRange(Normals[i].GetBytes());
						result.AddRange(ByteConverter.GetBytes(
							ByteConverter.ToUInt16(VColor.GetBytes(Diffuse[i], ColorType.RGB565), 0)
							| (ByteConverter.ToUInt16(VColor.GetBytes(Specular[i], ColorType.RGB565), 0) << 16)));
						break;
					case ChunkType.Vertex_VertexNormalDiffuseSpecular4:
						result.AddRange(Vertices[i].GetBytes());
						result.AddRange(Normals[i].GetBytes());
						result.AddRange(ByteConverter.GetBytes(
							ByteConverter.ToUInt16(VColor.GetBytes(Diffuse[i], ColorType.ARGB4444), 0)
							| (ByteConverter.ToUInt16(VColor.GetBytes(Specular[i], ColorType.RGB565), 0) << 16)));
						break;
				}
			}
			if (next != null)
				result.AddRange(next.GetBytes());
			return result.ToArray();
		}

		public void ToNJA(TextWriter writer)
		{
			switch(Type)
			{
				case ChunkType.Vertex_VertexSH:
					writer.WriteLine("\tCnkV_SH(0, " + (VertexCount * 4 + 1).ToString() + "),");
					writer.WriteLine("\tOffnbIdx(" + IndexOffset.ToString() + ", " + VertexCount.ToString() + "),");
					for (int i = 0; i < VertexCount; ++i)
					{
						writer.WriteLine("\tVERT_SH(" + Vertices[i].X.ToCHex().ToString() + ", " + Vertices[i].Y.ToCHex().ToString() + ", " + Vertices[i].Z.ToCHex().ToString() + "),");
					}
					break;
				case ChunkType.Vertex_VertexNormalSH:
					writer.WriteLine("\tCnkV_VN_SH(0, " + (VertexCount * 8 + 1).ToString() + "),");
					writer.WriteLine("\tOffnbIdx(" + IndexOffset.ToString() + ", " + VertexCount.ToString() + "),");
					for (int i = 0; i < VertexCount; ++i)
					{
						writer.WriteLine("\tVERT_SH(" + Vertices[i].X.ToCHex().ToString() + ", " + Vertices[i].Y.ToCHex().ToString() + ", " + Vertices[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tNORM_SH(" + Normals[i].X.ToCHex().ToString() + ", " + Normals[i].Y.ToCHex().ToString() + ", " + Normals[i].Z.ToCHex().ToString() + "),");
					}
					break;
				case ChunkType.Vertex_Vertex:
					writer.WriteLine("\tCnkV(0, " + (VertexCount * 3 + 1).ToString() + "),");
					writer.WriteLine("\tOffnbIdx(" + IndexOffset.ToString() + ", " + VertexCount.ToString() + "),");
					for (int i = 0; i < VertexCount; ++i)
					{
						writer.WriteLine("\tVERT(" + Vertices[i].X.ToCHex().ToString() + ", " + Vertices[i].Y.ToCHex().ToString() + ", " + Vertices[i].Z.ToCHex().ToString() + "),");
					}
					break;
				case ChunkType.Vertex_VertexDiffuse8:
					writer.WriteLine("\tCnkV_D8(0, " + (VertexCount * 4 + 1).ToString() + "),");
					writer.WriteLine("\tOffnbIdx(" + IndexOffset.ToString() + ", " + VertexCount.ToString() + "),");
					for (int i = 0; i < VertexCount; ++i)
					{
						writer.WriteLine("\tVERT(" + Vertices[i].X.ToCHex().ToString() + ", " + Vertices[i].Y.ToCHex().ToString() + ", " + Vertices[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tD8888(" + Diffuse[i].A.ToString() + ", " + Diffuse[i].R.ToString() + ", " + Diffuse[i].G.ToString() + ", " + Diffuse[i].B.ToString() + "),");
					}
					break;
				case ChunkType.Vertex_VertexUserFlags:
					writer.WriteLine("\tCnkV_UF(0, " + (VertexCount * 4 + 1).ToString() + "),");
					writer.WriteLine("\tOffnbIdx(" + IndexOffset.ToString() + ", " + VertexCount.ToString() + "),");
					for (int i = 0; i < VertexCount; ++i)
					{
						writer.WriteLine("\tVERT(" + Vertices[i].X.ToCHex().ToString() + ", " + Vertices[i].Y.ToCHex().ToString() + ", " + Vertices[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tUFlags(" + UserFlags.ToString() + "),");
					}
					break;
				case ChunkType.Vertex_VertexNinjaFlags:
					writer.WriteLine("\tCnkV_NF(0, " + (VertexCount * 4 + 1).ToString() + "),");
					writer.WriteLine("\tOffnbIdx(" + IndexOffset.ToString() + ", " + VertexCount.ToString() + "),");
					for (int i = 0; i < VertexCount; ++i)
					{
						writer.WriteLine("\tVERT(" + Vertices[i].X.ToCHex().ToString() + ", " + Vertices[i].Y.ToCHex().ToString() + ", " + Vertices[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tNFlags(" + UserFlags.ToString() + "),");
					}
					break;
				case ChunkType.Vertex_VertexDiffuseSpecular5:
					writer.WriteLine("\tCnkV_S5(0, " + (VertexCount * 4 + 1).ToString() + "),");
					writer.WriteLine("\tOffnbIdx(" + IndexOffset.ToString() + ", " + VertexCount.ToString() + "),");
					for (int i = 0; i < VertexCount; ++i)
					{
						writer.WriteLine("\tVERT(" + Vertices[i].X.ToCHex().ToString() + ", " + Vertices[i].Y.ToCHex().ToString() + ", " + Vertices[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tD565S565(" + Diffuse[i].R.ToString() + ", " + Diffuse[i].G.ToString() + ", " + Diffuse[i].B.ToString()
							+ ", " + Specular[i].R.ToString() + ", " + Specular[i].G.ToString() + ", " + Specular[i].B.ToString() + "),");
					}
					break;
				case ChunkType.Vertex_VertexDiffuseSpecular4:
					writer.WriteLine("\tCnkV_S4(0, " + (VertexCount * 4 + 1).ToString() + "),");
					writer.WriteLine("\tOffnbIdx(" + IndexOffset.ToString() + ", " + VertexCount.ToString() + "),");
					for (int i = 0; i < VertexCount; ++i)
					{
						writer.WriteLine("\tVERT(" + Vertices[i].X.ToCHex().ToString() + ", " + Vertices[i].Y.ToCHex().ToString() + ", " + Vertices[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tD4444S565(" + Diffuse[i].A.ToString() + ", " + Diffuse[i].R.ToString() + ", " + Diffuse[i].G.ToString() + ", " + Diffuse[i].B.ToString()
							+ ", " + Specular[i].R.ToString() + ", " + Specular[i].G.ToString() + ", " + Specular[i].B.ToString() + "),");
					}
					break;
				case ChunkType.Vertex_VertexDiffuseSpecular16:
					writer.WriteLine("\tCnkV_IN(0, " + (VertexCount * 4 + 1).ToString() + "),");
					writer.WriteLine("\tOffnbIdx(" + IndexOffset.ToString() + ", " + VertexCount.ToString() + "),");
					for (int i = 0; i < VertexCount; ++i)
					{
						writer.WriteLine("\tVERT(" + Vertices[i].X.ToCHex().ToString() + ", " + Vertices[i].Y.ToCHex().ToString() + ", " + Vertices[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tD16S16(" + Diffuse[i].ToString() + ", " + Specular[i].ToString() + "),");
					}
					break;
				case ChunkType.Vertex_VertexNormal:
					writer.WriteLine("\tCnkV_VN(0, " + (VertexCount * 6 + 1).ToString() + "),");
					writer.WriteLine("\tOffnbIdx(" + IndexOffset.ToString() + ", " + VertexCount.ToString() + "),");
					for (int i = 0; i < VertexCount; ++i)
					{
						writer.WriteLine("\tVERT(" + Vertices[i].X.ToCHex().ToString() + ", " + Vertices[i].Y.ToCHex().ToString() + ", " + Vertices[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tNORM(" + Normals[i].X.ToCHex().ToString() + ", " + Normals[i].Y.ToCHex().ToString() + ", " + Normals[i].Z.ToCHex().ToString() + "),");
					}
					break;
				case ChunkType.Vertex_VertexNormalDiffuse8:
					writer.WriteLine("\tCnkV_VN_D8(0, " + (VertexCount * 7 + 1).ToString() + "),");
					writer.WriteLine("\tOffnbIdx(" + IndexOffset.ToString() + ", " + VertexCount.ToString() + "),");
					for (int i = 0; i < VertexCount; ++i)
					{
						writer.WriteLine("\tVERT(" + Vertices[i].X.ToCHex().ToString() + ", " + Vertices[i].Y.ToCHex().ToString() + ", " + Vertices[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tNORM(" + Normals[i].X.ToCHex().ToString() + ", " + Normals[i].Y.ToCHex().ToString() + ", " + Normals[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tD8888(" + Diffuse[i].A.ToString() + ", " + Diffuse[i].R.ToString() + ", " + Diffuse[i].G.ToString() + ", " + Diffuse[i].B.ToString() + "),");
					}
					break;
				case ChunkType.Vertex_VertexNormalUserFlags:
					writer.WriteLine("\tCnkV_VN_UF(0, " + (VertexCount * 7 + 1).ToString() + "),");
					writer.WriteLine("\tOffnbIdx(" + IndexOffset.ToString() + ", " + VertexCount.ToString() + "),");
					for (int i = 0; i < VertexCount; ++i)
					{
						writer.WriteLine("\tVERT(" + Vertices[i].X.ToCHex().ToString() + ", " + Vertices[i].Y.ToCHex().ToString() + ", " + Vertices[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tNORM(" + Normals[i].X.ToCHex().ToString() + ", " + Normals[i].Y.ToCHex().ToString() + ", " + Normals[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tUFlags(" + UserFlags.ToString() + "),");
					}
					break;
				case ChunkType.Vertex_VertexNormalNinjaFlags:
					writer.WriteLine("\tCnkV_VN_NF(0, " + (VertexCount * 7 + 1).ToString() + "),");
					writer.WriteLine("\tOffnbIdx(" + IndexOffset.ToString() + ", " + VertexCount.ToString() + "),");
					for (int i = 0; i < VertexCount; ++i)
					{
						writer.WriteLine("\tVERT(" + Vertices[i].X.ToCHex().ToString() + ", " + Vertices[i].Y.ToCHex().ToString() + ", " + Vertices[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tNORM(" + Normals[i].X.ToCHex().ToString() + ", " + Normals[i].Y.ToCHex().ToString() + ", " + Normals[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tNFlags(" + UserFlags.ToString() + "),");
					}
					break;
				case ChunkType.Vertex_VertexNormalDiffuseSpecular5:
					writer.WriteLine("\tCnkV_VN_S5(0, " + (VertexCount * 7 + 1).ToString() + "),");
					writer.WriteLine("\tOffnbIdx(" + IndexOffset.ToString() + ", " + VertexCount.ToString() + "),");
					for (int i = 0; i < VertexCount; ++i)
					{
						writer.WriteLine("\tVERT(" + Vertices[i].X.ToCHex().ToString() + ", " + Vertices[i].Y.ToCHex().ToString() + ", " + Vertices[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tNORM(" + Normals[i].X.ToCHex().ToString() + ", " + Normals[i].Y.ToCHex().ToString() + ", " + Normals[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tD565S565(" + Diffuse[i].R.ToString() + ", " + Diffuse[i].G.ToString() + ", " + Diffuse[i].B.ToString()
							+ ", " + Specular[i].R.ToString() + ", " + Specular[i].G.ToString() + ", " + Specular[i].B.ToString() + "),");
					}
					break;
				case ChunkType.Vertex_VertexNormalDiffuseSpecular4:
					writer.WriteLine("\tCnkV_VN_S4(0, " + (VertexCount * 7 + 1).ToString() + "),");
					writer.WriteLine("\tOffnbIdx(" + IndexOffset.ToString() + ", " + VertexCount.ToString() + "),");
					for (int i = 0; i < VertexCount; ++i)
					{
						writer.WriteLine("\tVERT(" + Vertices[i].X.ToCHex().ToString() + ", " + Vertices[i].Y.ToCHex().ToString() + ", " + Vertices[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tNORM(" + Normals[i].X.ToCHex().ToString() + ", " + Normals[i].Y.ToCHex().ToString() + ", " + Normals[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tD4444S565(" + Diffuse[i].A.ToString() + ", " + Diffuse[i].R.ToString() + ", " + Diffuse[i].G.ToString() + ", " + Diffuse[i].B.ToString()
							+ ", " + Specular[i].R.ToString() + ", " + Specular[i].G.ToString() + ", " + Specular[i].B.ToString() + "),");
					}
					break;
				case ChunkType.Vertex_VertexNormalDiffuseSpecular16:
					writer.WriteLine("\tCnkV_VN_IN(0, " + (VertexCount * 7 + 1).ToString() + "),");
					writer.WriteLine("\tOffnbIdx(" + IndexOffset.ToString() + ", " + VertexCount.ToString() + "),");
					for (int i = 0; i < VertexCount; ++i)
					{
						writer.WriteLine("\tVERT(" + Vertices[i].X.ToCHex().ToString() + ", " + Vertices[i].Y.ToCHex().ToString() + ", " + Vertices[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tNORM(" + Normals[i].X.ToCHex().ToString() + ", " + Normals[i].Y.ToCHex().ToString() + ", " + Normals[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tD16S16(" + Diffuse[i].ToString() + ", " + Specular[i].ToString() + "),");
					}
					break;
				case ChunkType.Vertex_VertexNormalX:
					writer.WriteLine("\tCnkV_VNX(0, " + (VertexCount * 4 + 1).ToString() + "),");
					writer.WriteLine("\tOffnbIdx(" + IndexOffset.ToString() + ", " + VertexCount.ToString() + "),");
					for (int i = 0; i < VertexCount; ++i)
					{
						writer.WriteLine("\tVERT(" + Vertices[i].X.ToCHex().ToString() + ", " + Vertices[i].Y.ToCHex().ToString() + ", " + Vertices[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tNORM32(" + Normals[i].X.ToCHex().ToString() + ", " + Normals[i].Y.ToCHex().ToString() + ", " + Normals[i].Z.ToCHex().ToString() + "),");
					}
					break;
				case ChunkType.Vertex_VertexNormalXDiffuse8:
					writer.WriteLine("\tCnkV_VNX_D8(0, " + (VertexCount * 5 + 1).ToString() + "),");
					writer.WriteLine("\tOffnbIdx(" + IndexOffset.ToString() + ", " + VertexCount.ToString() + "),");
					for (int i = 0; i < VertexCount; ++i)
					{
						writer.WriteLine("\tVERT(" + Vertices[i].X.ToCHex().ToString() + ", " + Vertices[i].Y.ToCHex().ToString() + ", " + Vertices[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tNORM32(" + Normals[i].X.ToCHex().ToString() + ", " + Normals[i].Y.ToCHex().ToString() + ", " + Normals[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tD8888(" + Diffuse[i].A.ToString() + ", " + Diffuse[i].R.ToString() + ", " + Diffuse[i].G.ToString() + ", " + Diffuse[i].B.ToString() + "),");
					}
					break;
				case ChunkType.Vertex_VertexNormalXUserFlags:
					writer.WriteLine("\tCnkV_VNX_UF(0, " + (VertexCount * 5 + 1).ToString() + "),");
					writer.WriteLine("\tOffnbIdx(" + IndexOffset.ToString() + ", " + VertexCount.ToString() + "),");
					for (int i = 0; i < VertexCount; ++i)
					{
						writer.WriteLine("\tVERT(" + Vertices[i].X.ToCHex().ToString() + ", " + Vertices[i].Y.ToCHex().ToString() + ", " + Vertices[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tNORM32(" + Normals[i].X.ToCHex().ToString() + ", " + Normals[i].Y.ToCHex().ToString() + ", " + Normals[i].Z.ToCHex().ToString() + "),");
						writer.WriteLine("\tUFlags(" + UserFlags.ToString() + "),");
					}
					break;
			}
		}

		object ICloneable.Clone() => Clone();

		public VertexChunk Clone()
		{
			VertexChunk result = (VertexChunk)MemberwiseClone();
			result.Vertices = new List<Vertex>(Vertices.Count);
			foreach (Vertex item in Vertices)
				result.Vertices.Add(item.Clone());
			result.Normals = new List<Vertex>(Normals.Count);
			foreach (Vertex item in Normals)
				result.Normals.Add(item.Clone());
			result.Diffuse = new List<Color>(Diffuse);
			result.Specular = new List<Color>(Specular);
			result.UserFlags = new List<uint>(UserFlags);
			result.NinjaFlags = new List<uint>(NinjaFlags);
			return result;
		}

		public static List<VertexChunk> Merge(List<VertexChunk> source)
		{
			if (source == null) return null;
			if (source.Count < 2) return source;
			var chunks = new Dictionary<ChunkType, List<VertexChunk>>();
			foreach (var c in source)
			{
				if (!chunks.ContainsKey(c.Type))
					chunks[c.Type] = new List<VertexChunk>();
				chunks[c.Type].Add(c);
			}
			var result = new List<VertexChunk>();
			foreach (var list in chunks.Values)
			{
				var t = list[0].Type;
				switch (t)
				{
					case ChunkType.Vertex_VertexNinjaFlags:
					case ChunkType.Vertex_VertexNormalNinjaFlags:
						var weights = new Dictionary<WeightStatus, List<VertexChunk>>();
						foreach (var c in list)
						{
							if (!weights.ContainsKey(c.WeightStatus))
								weights[c.WeightStatus] = new List<VertexChunk>();
							weights[c.WeightStatus].Add(c);
						}
						foreach (var (s, l2) in weights.OrderBy(a => a.Key))
						{
							var r = new VertexChunk(t) { WeightStatus = s, IndexOffset = l2.Min(a => a.IndexOffset) };
							foreach (var c in l2)
							{
								r.Vertices.AddRange(c.Vertices);
								if (c.Normals?.Count > 0)
									r.Normals.AddRange(c.Normals);
								if (c.IndexOffset == r.IndexOffset)
									r.NinjaFlags.AddRange(c.NinjaFlags);
								else
									for (int i = 0; i < c.Vertices.Count; i++)
									{
										int ind = (int)c.NinjaFlags[i] & 0xFFFF;
										ind += c.IndexOffset - r.IndexOffset;
										r.NinjaFlags.Add((c.NinjaFlags[i] & 0xFFFF0000) | (uint)ind);
									}
							}
							result.Add(r);
						}
						break;
					default:
						list.Sort((a, b) => a.IndexOffset.CompareTo(b.IndexOffset));
						var r2 = new VertexChunk(t) { IndexOffset = list[0].IndexOffset };
						foreach (var c in list)
						{
							if (c.IndexOffset != r2.IndexOffset + r2.VertexCount)
							{
								result.Add(r2);
								r2 = c;
							}
							else
							{
								r2.Vertices.AddRange(c.Vertices);
								if (c.Normals?.Count > 0)
									r2.Normals.AddRange(c.Normals);
								if (c.Diffuse?.Count > 0)
									r2.Diffuse.AddRange(c.Diffuse);
								if (c.Specular?.Count > 0)
									r2.Specular.AddRange(c.Specular);
								if (c.UserFlags?.Count > 0)
									r2.UserFlags.AddRange(c.UserFlags);
							}
						}
						result.Add(r2);
						break;
				}
			}
			return result;
		}
	}
}