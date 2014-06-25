using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SonicRetro.SAModel
{
    /// <summary>
    /// This is a standard tri mesh representation of a BasicAttach or ChunkAttach.
    /// </summary>
    public class MeshInfo
    {
        public Material Material { get; private set; }
        public VertexData[] Vertices { get; private set; }
		public bool HasUV { get; private set; }
		public bool HasVC { get; private set; }

        public MeshInfo(Material material, VertexData[] vertices, bool hasUV, bool hasVC)
        {
            Material = material;
            Vertices = vertices;
			HasUV = hasUV;
			HasVC = hasVC;
        }
    }

    public struct VertexData
    {
        public Vertex Position;
        public Vertex Normal;
        public Color? Color;
        public UV UV;

        public VertexData(Vertex position)
            : this(position, null, null, null)
        { }

        public VertexData(Vertex position, Vertex normal)
            : this(position, normal, null, null)
        { }

        public VertexData(Vertex position, Vertex normal, Color? color, UV uv)
        {
            Position = position;
            Normal = normal ?? Vertex.UpNormal;
            Color = color;
            UV = uv;
        }
    }
}