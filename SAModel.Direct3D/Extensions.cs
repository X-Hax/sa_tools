using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;


namespace SonicRetro.SAModel.Direct3D
{
    public static class Extensions
    {
        public static Vector3 ToVector3(this Vertex vert)
        {
            return new Vector3(vert.X, vert.Y, vert.Z);
        }

        public static void CalculateBounds(this Attach attach)
        {
            List<Vector3> verts = new List<Vector3>();
            foreach (SAModel.Mesh mesh in attach.Mesh)
                foreach (Poly poly in mesh.Poly)
                    foreach (ushort index in poly.Indexes)
                        verts.Add(attach.Vertex[index].ToVector3());
            BoundingSphere bound = BoundingSphere.FromPoints(verts.ToArray());
            attach.Center.X = bound.Center.X;
            attach.Center.Y = bound.Center.Y;
            attach.Center.Z = bound.Center.Z;
            attach.Radius = bound.Radius;
        }

        public static SlimDX.Direct3D9.Mesh CreateD3DMesh(this Attach attach, SlimDX.Direct3D9.Device dev)
        {
            int numverts = 0;
            VertexData[][] verts = attach.GetVertexData();
            foreach (VertexData[] item in verts)
                numverts += item.Length;
            SlimDX.Direct3D9.Mesh functionReturnValue = new SlimDX.Direct3D9.Mesh(dev, numverts / 3, numverts, MeshFlags.Managed, FVF_PositionNormalTexturedColored.Format);
            DataStream vb = functionReturnValue.LockVertexBuffer(LockFlags.None);
            DataStream ib = functionReturnValue.LockIndexBuffer(LockFlags.None);
            DataStream at = functionReturnValue.LockAttributeBuffer(LockFlags.None);
            int vind = 0;
            for (int i = 0; i < verts.Length; i++)
            {
                for (int j = 0; j < verts[i].Length; j++)
                {
                    vb.Write(new FVF_PositionNormalTexturedColored(verts[i][j]));
                    ib.Write(vind + j);
                }
                for (int j = 0; j < verts[i].Length / 3; j++)
                {
                    at.Write(i);
                }
                vind += verts[i].Length;
            }
            functionReturnValue.UnlockVertexBuffer();
            functionReturnValue.UnlockIndexBuffer();
            functionReturnValue.UnlockAttributeBuffer();
            return functionReturnValue;
        }
    }
}
