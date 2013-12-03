using System;
using System.Collections.Generic;
using System.Drawing;

namespace SonicRetro.SAModel
{
    public class SA2BAttach : Attach
    {
        public Vertex[] Vertex { get; private set; }
        public Vertex[] Normal { get; private set; }
        public uint[] VColor { get; private set; }
        public UV[] UV { get; private set; }

        public SA2BAttach()
        {
            Name = "attach_" + Object.GenerateIdentifier();
            Vertex = new Vertex[0];
            Normal = new Vertex[0];
            VColor = new uint[0];
            UV = new UV[0];
            Bounds = new BoundingSphere();
        }

        public SA2BAttach(byte[] file, int address, uint imageBase)
            : this(file, address, imageBase, new Dictionary<int, string>()) { }

        public SA2BAttach(byte[] file, int address, uint imageBase, Dictionary<int, string> labels)
            : this()
        {
            if (labels.ContainsKey(address))
                Name = labels[address];
            else
                Name = "attach_" + address.ToString("X8");
            Vertex = new Vertex[ByteConverter.ToInt32(file, address + 8)];
            Normal = new Vertex[Vertex.Length];
            int tmpaddr = (int)(ByteConverter.ToUInt32(file, address) - imageBase);
            for (int i = 0; i < Vertex.Length; i++)
            {
                Vertex[i] = new Vertex(file, tmpaddr);
                tmpaddr += SAModel.Vertex.Size;
            }
            tmpaddr = ByteConverter.ToInt32(file, address + 4);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)((uint)tmpaddr - imageBase);
                for (int i = 0; i < Vertex.Length; i++)
                {
                    Normal[i] = new Vertex(file, tmpaddr);
                    tmpaddr += SAModel.Vertex.Size;
                }
            }
            else
                for (int i = 0; i < Vertex.Length; i++)
                    Normal[i] = new Vertex(0, 1, 0);
            int meshcnt = ByteConverter.ToInt16(file, address + 0x14);
            tmpaddr = ByteConverter.ToInt32(file, address + 0xC);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                for (int i = 0; i < meshcnt; i++)
                {
                }
            }
            int matcnt = ByteConverter.ToInt16(file, address + 0x16);
            tmpaddr = ByteConverter.ToInt32(file, address + 0x10);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                for (int i = 0; i < matcnt; i++)
                {
                    tmpaddr += SAModel.Material.Size;
                }
            }
            Bounds = new BoundingSphere(file, address + 0x18);
        }

        public SA2BAttach(Vertex[] vertex, Vertex[] normal, IEnumerable<Mesh> mesh, IEnumerable<Material> material)
            : this()
        {
            Vertex = vertex;
            Normal = normal;
        }

        public override byte[] GetBytes(uint imageBase, bool DX, Dictionary<string, uint> labels, out uint address)
        {
            List<byte> result = new List<byte>();
            address = (uint)result.Count;
            result.AddRange(Bounds.GetBytes());
            labels.Add(Name, address + imageBase);
            return result.ToArray();
        }

        public override string ToStruct(bool DX)
        {
            throw new NotImplementedException();
        }

        public override string ToStructVariables(bool DX, List<string> labels)
        {
            throw new NotImplementedException();
        }

        public override void ProcessVertexData()
        {
            List<MeshInfo> result = new List<MeshInfo>();
            MeshInfo = result.ToArray();
        }

        public override BasicAttach ToBasicModel()
        {
            throw new NotImplementedException(); // TODO
        }

        public override ChunkAttach ToChunkModel()
        {
            throw new NotImplementedException(); // TODO
        }
    }
}