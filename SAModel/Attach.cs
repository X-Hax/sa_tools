using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonicRetro.SAModel
{
    public abstract class Attach
    {
        public string Name { get; set; }
        public BoundingSphere Bounds { get; set; }
        public MeshInfo[] MeshInfo { get; protected set; }

        public static int Size(ModelFormat format)
        {
            switch (format)
            {
                case ModelFormat.Basic:
                    return 0x28;
                case ModelFormat.BasicDX:
                    return 0x2C;
                case ModelFormat.Chunk:
                    return 0x18;
            }
            return -1;
        }

        public static Attach Load(ModelFormat format)
        {
            switch (format)
            {
                case ModelFormat.Basic:
                case ModelFormat.BasicDX:
                    return new BasicAttach();
                case ModelFormat.Chunk:
                    return new ChunkAttach();
            }
            throw new ArgumentOutOfRangeException("format");
        }

        public static Attach Load(byte[] file, int address, uint imageBase, ModelFormat format) { return Load(file, address, imageBase, format, new Dictionary<int, string>()); }

        public static Attach Load(byte[] file, int address, uint imageBase, ModelFormat format, Dictionary<int, string> labels)
        {
            switch (format)
            {
                case ModelFormat.Basic:
                case ModelFormat.BasicDX:
                    return new BasicAttach(file, address, imageBase, format == ModelFormat.BasicDX, labels);
                case ModelFormat.Chunk:
                    return new ChunkAttach(file, address, imageBase, labels);
            }
            throw new ArgumentOutOfRangeException("format");
        }

        public abstract byte[] GetBytes(uint imageBase, bool DX, Dictionary<string, uint> labels, out uint address);

        public byte[] GetBytes(uint imageBase, bool DX, out uint address)
        {
            return GetBytes(imageBase, DX, new Dictionary<string, uint>(), out address);
        }

        public byte[] GetBytes(uint imageBase, bool DX)
        {
            uint address;
            return GetBytes(imageBase, DX, out address);
        }

        public abstract string ToStruct(bool DX);

        public abstract string ToStructVariables(bool DX, List<string> labels);

        public abstract void ProcessVertexData();

        public abstract BasicAttach ToBasicModel();

        public abstract ChunkAttach ToChunkModel();
    }
}