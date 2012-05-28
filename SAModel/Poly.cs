using System;
using System.Collections.Generic;

namespace SonicRetro.SAModel
{
    public sealed class Triangle : Poly
    {
        public Triangle()
        {
            Indexes = new ushort[3];
        }

        public Triangle(byte[] file, int address)
            : this()
        {
            Indexes[0] = ByteConverter.ToUInt16(file, address);
            Indexes[1] = ByteConverter.ToUInt16(file, address + 2);
            Indexes[2] = ByteConverter.ToUInt16(file, address + 4);
        }

        public override PolyType PolyType { get { return SAModel.PolyType.Triangles; } }
    }

    public sealed class Quad : Poly
    {
        public Quad()
        {
            Indexes = new ushort[4];
        }

        public Quad(byte[] file, int address)
            : this()
        {
            Indexes[0] = ByteConverter.ToUInt16(file, address);
            Indexes[1] = ByteConverter.ToUInt16(file, address + 2);
            Indexes[2] = ByteConverter.ToUInt16(file, address + 4);
            Indexes[3] = ByteConverter.ToUInt16(file, address + 6);
        }

        public override PolyType PolyType { get { return SAModel.PolyType.Quads; } }
    }

    public sealed class Strip : Poly
    {
        public bool Reversed { get; private set; }

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

        public Strip(byte[] file, int address)
        {
            Indexes = new ushort[ByteConverter.ToUInt16(file, address) & 0x7FFF];
            Reversed = (ByteConverter.ToUInt16(file, address) & 0x8000) == 0x8000;
            address += 2;
            for (int i = 0; i < Indexes.Length; i++)
            {
                Indexes[i] = ByteConverter.ToUInt16(file, address);
                address += 2;
            }
        }

        public override int Size { get { return base.Size + 2; } }

        public override PolyType PolyType { get { return SAModel.PolyType.Strips; } }

        public override byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            result.AddRange(ByteConverter.GetBytes((ushort)(Indexes.Length | (Reversed ? 0x8000 : 0))));
            result.AddRange(base.GetBytes());
            return result.ToArray();
        }
    }

    public abstract class Poly
    {
        public ushort[] Indexes { get; protected set; }

        internal Poly() { }

        public virtual int Size { get { return Indexes.Length * 2; } }

        public abstract PolyType PolyType { get; }

        public virtual byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            foreach (ushort item in Indexes)
                result.AddRange(ByteConverter.GetBytes(item));
            return result.ToArray();
        }

        public static Poly CreatePoly(PolyType type)
        {
            switch (type)
            {
                case PolyType.Triangles:
                    return new Triangle();
                case PolyType.Quads:
                    return new Quad();
                case PolyType.NPoly:
                case PolyType.Strips:
                    throw new ArgumentException("Cannot create strip-type poly without additional information.\nUse Strip.Strip(int NumVerts, bool Reverse) instead.", "type");
            }
            throw new ArgumentException("Unknown poly type!", "type");
        }

        public static Poly CreatePoly(PolyType type, byte[] file, int address)
        {
            switch (type)
            {
                case PolyType.Triangles:
                    return new Triangle(file, address);
                case PolyType.Quads:
                    return new Quad(file, address);
                case PolyType.NPoly:
                case PolyType.Strips:
                    return new Strip(file, address);
            }
            throw new ArgumentException("Unknown poly type!", "type");
        }
    }
}