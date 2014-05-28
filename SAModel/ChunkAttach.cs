using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SonicRetro.SAModel
{
    [Serializable]
    public class ChunkAttach : Attach
    {
        public List<VertexChunk> Vertex { get; set; }
        public string VertexName { get; set; }
        public List<PolyChunk> Poly { get; set; }
        public string PolyName { get; set; }

        public ChunkAttach()
        {
            Name = "attach_" + Object.GenerateIdentifier();
            Bounds = new BoundingSphere();
        }

        public ChunkAttach(bool hasVertex, bool hasPoly)
            : this()
        {
            if (hasVertex)
            {
                Vertex = new List<VertexChunk>();
                VertexName = "vertex_" + Object.GenerateIdentifier();
            }
            if (hasPoly)
            {
                Poly = new List<PolyChunk>();
                PolyName = "poly_" + Object.GenerateIdentifier();
            }
        }

        public ChunkAttach(byte[] file, int address, uint imageBase)
            : this(file, address, imageBase, new Dictionary<int, string>()) { }

        public ChunkAttach(byte[] file, int address, uint imageBase, Dictionary<int, string> labels)
            : this()
        {
            if (labels.ContainsKey(address))
                Name = labels[address];
            else
                Name = "attach_" + address.ToString("X8");
            ChunkType ctype;
            int tmpaddr = ByteConverter.ToInt32(file, address);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                Vertex = new List<VertexChunk>();
                if (labels.ContainsKey(tmpaddr))
                    VertexName = labels[tmpaddr];
                else
                    VertexName = "vertex_" + tmpaddr.ToString("X8");
                ctype = (ChunkType)(ByteConverter.ToUInt32(file, tmpaddr) & 0xFF);
                while (ctype != ChunkType.End)
                {
                    VertexChunk chunk = new VertexChunk(file, tmpaddr);
                    Vertex.Add(chunk);
                    tmpaddr += (chunk.Size * 4) + 4;
                    ctype = (ChunkType)(ByteConverter.ToUInt32(file, tmpaddr) & 0xFF);
                }
            }
            tmpaddr = ByteConverter.ToInt32(file, address + 4);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                Poly = new List<PolyChunk>();
                if (labels.ContainsKey(tmpaddr))
                    PolyName = labels[tmpaddr];
                else
                    PolyName = "poly_" + tmpaddr.ToString("X8");
                PolyChunk chunk = PolyChunk.Load(file, tmpaddr);
                while (chunk.Type != ChunkType.End)
                {
                    if (chunk.Type != ChunkType.Null)
                        Poly.Add(chunk);
                    tmpaddr += chunk.ByteSize;
                    chunk = PolyChunk.Load(file, tmpaddr);
                }
            }
            Bounds = new BoundingSphere(file, address + 8);
        }

        public override byte[] GetBytes(uint imageBase, bool DX, Dictionary<string, uint> labels, out uint address)
        {
            List<byte> result = new List<byte>();
            uint vertexAddress = 0;
            if (Vertex != null && Vertex.Count > 0)
                if (labels.ContainsKey(VertexName))
                    vertexAddress = labels[VertexName];
                else
                {
                    vertexAddress = imageBase;
                    labels.Add(VertexName, vertexAddress);
                    foreach (VertexChunk item in Vertex)
                        result.AddRange(item.GetBytes());
                    result.AddRange(new VertexChunk(ChunkType.End).GetBytes());
                }
            result.Align(4);
            uint polyAddress = 0;
            if (Poly != null && Poly.Count > 0)
                if (labels.ContainsKey(PolyName))
                    polyAddress = labels[PolyName];
                else
                {
                    polyAddress = (uint)(imageBase + result.Count);
                    labels.Add(PolyName, polyAddress);
                    foreach (PolyChunk item in Poly)
                        result.AddRange(item.GetBytes());
                    result.AddRange(new PolyChunkEnd().GetBytes());
                }
            result.Align(4);
            address = (uint)result.Count;
            result.AddRange(ByteConverter.GetBytes(vertexAddress));
            result.AddRange(ByteConverter.GetBytes(polyAddress));
            result.AddRange(Bounds.GetBytes());
            labels.Add(Name, address + imageBase);
            return result.ToArray();
        }

        public override string ToStruct(bool DX)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder("{ ");
            result.Append(Vertex != null ? VertexName : "NULL");
            result.Append(", ");
            result.Append(Poly != null ? PolyName : "NULL");
            result.Append(", ");
            result.Append(Bounds.ToStruct());
            result.Append(" }");
            return result.ToString();
        }

        public override string ToStructVariables(bool DX, List<string> labels, string[] textures)
        {
            System.Text.StringBuilder result = new StringBuilder();
            if (Vertex != null && !labels.Contains(VertexName))
            {
                labels.Add(VertexName);
                result.Append("Sint32 ");
                result.Append(VertexName);
                result.Append("[] = { ");
                List<byte> chunks = new List<byte>();
                foreach (VertexChunk item in Vertex)
                    chunks.AddRange(item.GetBytes());
                chunks.AddRange(new VertexChunk(ChunkType.End).GetBytes());
                byte[] cb = chunks.ToArray();
                List<string> s = new List<string>(cb.Length / 4);
                for (int i = 0; i < cb.Length; i += 4)
				{
					int it = ByteConverter.ToInt32(cb, i);
					s.Add("0x" + it.ToString("X") + (it < 0 ? "u" : ""));
				}
				result.Append(string.Join(", ", s.ToArray()));
                result.AppendLine(" };");
                result.AppendLine();
            }
            if (Poly != null && !labels.Contains(PolyName))
            {
                labels.Add(PolyName);
                result.Append("Sint16 ");
                result.Append(PolyName);
                result.Append("[] = { ");
                List<byte> chunks = new List<byte>();
                foreach (PolyChunk item in Poly)
                    chunks.AddRange(item.GetBytes());
                chunks.AddRange(new PolyChunkEnd().GetBytes());
                byte[] cb = chunks.ToArray();
                List<string> s = new List<string>(cb.Length / 2);
				for (int i = 0; i < cb.Length; i += 2)
				{
					short sh = ByteConverter.ToInt16(cb, i);
					s.Add("0x" + sh.ToString("X") + (sh < 0 ? "u" : ""));
				}
                result.Append(string.Join(", ", s.ToArray()));
                result.AppendLine(" };");
                result.AppendLine();
            }
            result.Append("NJS_CNK_MODEL ");
            result.Append(Name);
            result.Append(" = ");
            result.Append(ToStruct(DX));
            result.AppendLine(";");
            return result.ToString();
        }

        static Material MaterialBuffer = new Material() { UseTexture = true };
        static Vertex[] VertexBuffer = new Vertex[0];
        static Vertex[] NormalBuffer = new Vertex[0];
        static readonly CachedPoly[] PolyCache = new CachedPoly[255];

        public override void ProcessVertexData()
        {
            if (Vertex != null)
                foreach (VertexChunk chunk in Vertex)
                {
                    if (VertexBuffer.Length < chunk.IndexOffset + chunk.VertexCount)
                        Array.Resize(ref VertexBuffer, chunk.IndexOffset + chunk.VertexCount);
                    if (NormalBuffer.Length < chunk.IndexOffset + chunk.VertexCount)
                        Array.Resize(ref NormalBuffer, chunk.IndexOffset + chunk.VertexCount);
                    Array.Copy(chunk.Vertices.ToArray(), 0, VertexBuffer, chunk.IndexOffset, chunk.Vertices.Count);
                    Array.Copy(chunk.Normals.ToArray(), 0, NormalBuffer, chunk.IndexOffset, chunk.Normals.Count);
                }
            List<MeshInfo> result = new List<MeshInfo>();
            if (Poly != null)
                result = ProcessPolyList(Poly, 0);
            MeshInfo = result.ToArray();
        }

        private List<MeshInfo> ProcessPolyList(List<PolyChunk> strips, int start)
        {
            List<MeshInfo> result = new List<MeshInfo>();
            for (int i = start; i < strips.Count; i++)
            {
                PolyChunk chunk = strips[i];
                switch (chunk.Type)
                {
                    case ChunkType.Bits_BlendAlpha:
                        {
                            PolyChunkBitsBlendAlpha c2 = (PolyChunkBitsBlendAlpha)chunk;
                            MaterialBuffer.SourceAlpha = c2.SourceAlpha;
                            MaterialBuffer.DestinationAlpha = c2.DestinationAlpha;
                        }
                        break;
                    case ChunkType.Bits_MipmapDAdjust:
                        break;
                    case ChunkType.Bits_SpecularExponent:
                        MaterialBuffer.Exponent = ((PolyChunkBitsSpecularExponent)chunk).SpecularExponent;
                        break;
                    case ChunkType.Bits_CachePolygonList:
                        PolyCache[((PolyChunkBitsCachePolygonList)chunk).List] = new CachedPoly(strips, i + 1);
                        return result;
                    case ChunkType.Bits_DrawPolygonList:
                        byte cachenum = ((PolyChunkBitsDrawPolygonList)chunk).List;
                        CachedPoly cached = PolyCache[cachenum];
                        PolyCache[cachenum] = null;
                        result.AddRange(ProcessPolyList(cached.Polys, cached.Index));
                        break;
                    case ChunkType.Tiny_TextureID:
                    case ChunkType.Tiny_TextureID2:
                        {
                            PolyChunkTinyTextureID c2 = (PolyChunkTinyTextureID)chunk;
                            MaterialBuffer.ClampU = c2.ClampU;
                            MaterialBuffer.ClampV = c2.ClampV;
                            MaterialBuffer.FilterMode = c2.FilterMode;
                            MaterialBuffer.FlipU = c2.FlipU;
                            MaterialBuffer.FlipV = c2.FlipV;
                            MaterialBuffer.SuperSample = c2.SuperSample;
                            MaterialBuffer.TextureID = c2.TextureID;
                        }
                        break;
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
                        {
                            PolyChunkMaterial c2 = (PolyChunkMaterial)chunk;
                            if (c2.Diffuse.HasValue)
                                MaterialBuffer.DiffuseColor = c2.Diffuse.Value;
                            if (c2.Specular.HasValue)
                            {
                                MaterialBuffer.SpecularColor = c2.Specular.Value;
                                MaterialBuffer.Exponent = c2.SpecularExponent;
                            }
                        }
                        break;
                    case ChunkType.Strip_Strip:
                    case ChunkType.Strip_StripUVN:
                    case ChunkType.Strip_StripUVH:
                    case ChunkType.Strip_StripNormal:
                    case ChunkType.Strip_StripUVNNormal:
                    case ChunkType.Strip_StripUVHNormal:
                    case ChunkType.Strip_StripColor:
                    case ChunkType.Strip_StripUVNColor:
                    case ChunkType.Strip_StripUVHColor:
                    case ChunkType.Strip_Strip2:
                    case ChunkType.Strip_StripUVN2:
                    case ChunkType.Strip_StripUVH2:
                        {
                            PolyChunkStrip c2 = (PolyChunkStrip)chunk;
                            MaterialBuffer.DoubleSided = c2.DoubleSide;
                            MaterialBuffer.EnvironmentMap = c2.EnvironmentMapping;
                            MaterialBuffer.FlatShading = c2.FlatShading;
                            MaterialBuffer.IgnoreLighting = c2.IgnoreLight;
                            MaterialBuffer.IgnoreSpecular = c2.IgnoreSpecular;
                            MaterialBuffer.UseAlpha = c2.UseAlpha;
                            bool hasVColor = false;
                            switch (chunk.Type)
                            {
                                case ChunkType.Strip_StripColor:
                                case ChunkType.Strip_StripUVNColor:
                                case ChunkType.Strip_StripUVHColor:
                                    hasVColor = true;
                                    break;
                            }
                            bool hasUV = false;
                            switch (chunk.Type)
                            {
                                case ChunkType.Strip_StripUVN:
                                case ChunkType.Strip_StripUVH:
                                case ChunkType.Strip_StripUVNColor:
                                case ChunkType.Strip_StripUVHColor:
                                case ChunkType.Strip_StripUVN2:
                                case ChunkType.Strip_StripUVH2:
                                    hasUV = true;
                                    break;
                            }
                            List<VertexData> verts = new List<VertexData>();
                            foreach (PolyChunkStrip.Strip strip in c2.Strips)
                            {
                                bool flip = !strip.Reversed;
                                for (int k = 0; k < strip.Indexes.Length - 2; k++)
                                {
                                    flip = !flip;
                                    if (!flip)
                                    {
                                        verts.Add(new VertexData(
                                            VertexBuffer[strip.Indexes[k]],
                                            NormalBuffer[strip.Indexes[k]],
                                            hasVColor ? strip.VColors[k] : Color.White,
                                            hasUV ? strip.UVs[k] : new UV()));
                                        verts.Add(new VertexData(
                                            VertexBuffer[strip.Indexes[k + 1]],
                                            NormalBuffer[strip.Indexes[k + 1]],
                                            hasVColor ? strip.VColors[k + 1] : Color.White,
                                            hasUV ? strip.UVs[k + 1] : new UV()));
                                        verts.Add(new VertexData(
                                            VertexBuffer[strip.Indexes[k + 2]],
                                            NormalBuffer[strip.Indexes[k + 2]],
                                            hasVColor ? strip.VColors[k + 2] : Color.White,
                                            hasUV ? strip.UVs[k + 2] : new UV()));
                                    }
                                    else
                                    {
                                        verts.Add(new VertexData(
                                            VertexBuffer[strip.Indexes[k + 1]],
                                            NormalBuffer[strip.Indexes[k + 1]],
                                            hasVColor ? strip.VColors[k + 1] : Color.White,
                                            hasUV ? strip.UVs[k + 1] : new UV()));
                                        verts.Add(new VertexData(
                                            VertexBuffer[strip.Indexes[k]],
                                            NormalBuffer[strip.Indexes[k]],
                                            hasVColor ? strip.VColors[k] : Color.White,
                                            hasUV ? strip.UVs[k] : new UV()));
                                        verts.Add(new VertexData(
                                            VertexBuffer[strip.Indexes[k + 2]],
                                            NormalBuffer[strip.Indexes[k + 2]],
                                            hasVColor ? strip.VColors[k + 2] : Color.White,
                                            hasUV ? strip.UVs[k + 2] : new UV()));
                                    }
                                }
                            }
                            result.Add(new MeshInfo(MaterialBuffer, verts.ToArray()));
                            MaterialBuffer = new Material(MaterialBuffer.GetBytes(), 0);
                        }
                        break;
                }
            }
            return result;
        }

        private class CachedPoly
        {
            public List<PolyChunk> Polys { get; private set; }
            public int Index { get; private set; }

            public CachedPoly(List<PolyChunk> polys, int index)
            {
                Polys = polys;
                Index = index;
            }
        }

        public override BasicAttach ToBasicModel()
        {
            throw new NotImplementedException();
        }

        public override ChunkAttach ToChunkModel()
        {
            return this;
        }
    }
}