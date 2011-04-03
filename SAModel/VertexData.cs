using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SonicRetro.SAModel
{
    public struct VertexData
    {
        public Vertex Position;
        public Vertex Normal;
        public Color Color;
        public UV UV;

        public VertexData(Vertex position, Vertex normal, Color color, UV uv)
        {
            Position = position;
            Normal = normal;
            Color = color;
            UV = uv;
        }
    }
}
