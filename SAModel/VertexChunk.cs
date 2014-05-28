using System;
using System.Collections.Generic;
using System.Drawing;

namespace SonicRetro.SAModel
{
    public class VertexChunk
    {
        public uint Header1 { get; set; }

        public ChunkType Type
        {
            get { return (ChunkType)(Header1 & 0xFF); }
            private set { Header1 = (uint)((Header1 & 0xFFFFFF00u) | (byte)value); }
        }
        
        public byte Flags
        {
            get { return (byte)((Header1 >> 8) & 0xFF); }
            set { Header1 = (uint)((Header1 & 0xFFFF00FFu) | (uint)(value << 8)); }
        }

        public ushort Size
        {
            get { return (ushort)(Header1 >> 16); }
            set { Header1 = (uint)((Header1 & 0xFFFFu) | (uint)(value << 16)); }
        }

        public uint Header2 { get; set; }

        public ushort IndexOffset
        {
            get { return (ushort)(Header2 & 0xFFFF); }
            set { Header2 = (ushort)((Header2 & 0xFFFF0000u) | value); }
        }

        public ushort VertexCount
        {
            get { return (ushort)(Header2 >> 16); }
            set { Header2 = (uint)((Header2 & 0xFFFFu) | (uint)(value << 16)); }
        }

        public List<Vertex> Vertices { get; set; }
        public List<Vertex> Normals { get; set; }
        public List<Color> Diffuse { get; set; }
        public List<Color> Specular { get; set; }
        public List<uint> VertFlags { get; set; }

        public VertexChunk()
        {
            Type = ChunkType.Vertex_Vertex;
            Vertices = new List<Vertex>();
            Normals = new List<Vertex>();
            Diffuse = new List<Color>();
            Specular = new List<Color>();
            VertFlags = new List<uint>();
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
            address = address + 8;
            for (int i = 0; i < VertexCount; i++)
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
                    case ChunkType.Vertex_VertexNinjaFlags:
                        Vertices.Add(new Vertex(file, address));
                        address += Vertex.Size;
                        VertFlags.Add(ByteConverter.ToUInt32(file, address));
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
                    case ChunkType.Vertex_VertexNormalNinjaFlags:
                        Vertices.Add(new Vertex(file, address));
                        address += Vertex.Size;
                        Normals.Add(new Vertex(file, address));
                        address += Vertex.Size;
                        VertFlags.Add(ByteConverter.ToUInt32(file, address));
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

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>((Size * 4) + 4);
            result.AddRange(ByteConverter.GetBytes(Header1));
            result.AddRange(ByteConverter.GetBytes(Header2));
            for (int i = 0; i < VertexCount; i++)
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
                    case ChunkType.Vertex_VertexNinjaFlags:
                        result.AddRange(Vertices[i].GetBytes());
                        result.AddRange(ByteConverter.GetBytes(VertFlags[i]));
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
                    case ChunkType.Vertex_VertexNormalNinjaFlags:
                        result.AddRange(Vertices[i].GetBytes());
                        result.AddRange(Normals[i].GetBytes());
                        result.AddRange(ByteConverter.GetBytes(VertFlags[i]));
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
                    default:
                        throw new NotSupportedException("Unsupported chunk type " + Type + ".");
                }
            return result.ToArray();
        }
    }
}