using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;


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
            Vector3 center = new Vector3();
            attach.Radius = Geometry.ComputeBoundingSphere(verts.ToArray(), FVF_PositionNormalTexturedColored.Format, out center);
            attach.Center.X = center.X;
            attach.Center.Y = center.Y;
            attach.Center.Z = center.Z;
        }

        public static void CalculateBounds(this COL col)
        {
            Matrix matrix = Matrix.Identity;
            matrix *= Matrix.RotationYawPitchRoll(BAMSToRad(col.Object.Rotation.Y), BAMSToRad(col.Object.Rotation.X), BAMSToRad(col.Object.Rotation.Z));
            matrix *= Matrix.Translation(col.Object.Position.ToVector3());
            List<Vector3> verts = new List<Vector3>();
            foreach (SAModel.Mesh mesh in col.Object.Attach.Mesh)
                foreach (Poly poly in mesh.Poly)
                    foreach (ushort index in poly.Indexes)
                        verts.Add(Vector3.TransformCoordinate(col.Object.Attach.Vertex[index].ToVector3(), matrix));
            Vector3 center = new Vector3();
            col.Radius = Geometry.ComputeBoundingSphere(verts.ToArray(), FVF_PositionNormalTexturedColored.Format, out center);
            col.Center.X = center.X;
            col.Center.Y = center.Y;
            col.Center.Z = center.Z;
        }

        public static Microsoft.DirectX.Direct3D.Mesh CreateD3DMesh(this Attach attach, Microsoft.DirectX.Direct3D.Device dev)
        {
            int numverts = 0;
            VertexData[][] verts = attach.GetVertexData();
            foreach (VertexData[] item in verts)
                numverts += item.Length;
            Microsoft.DirectX.Direct3D.Mesh functionReturnValue = new Microsoft.DirectX.Direct3D.Mesh(numverts / 3, numverts, MeshFlags.Managed, FVF_PositionNormalTexturedColored.Elements, dev);
            List<FVF_PositionNormalTexturedColored> vb = new List<FVF_PositionNormalTexturedColored>();
            List<short> ib = new List<short>();
            int[] at = functionReturnValue.LockAttributeBufferArray(LockFlags.None);
            int vind;
            for (int i = 0; i < verts.Length; i++)
            {
                vind = vb.Count;
                for (int j = 0; j < verts[i].Length; j++)
                {
                    vb.Add(new FVF_PositionNormalTexturedColored(verts[i][j]));
                    ib.Add((short)(vind + j));
                }
                for (int j = 0; j < verts[i].Length / 3; j++)
                    at[(vind / 3) + j] = i;
            }
            functionReturnValue.SetVertexBufferData(vb.ToArray(), LockFlags.None);
            functionReturnValue.SetIndexBufferData(ib.ToArray(), LockFlags.None);
            functionReturnValue.UnlockAttributeBuffer(at);
            return functionReturnValue;
        }

        public static float BAMSToRad(int BAMS)
        {
            return (float)(BAMS / (65536 / (2 * Math.PI)));
        }

        public static void DrawModel(this Object obj, Device device, MatrixStack transform, Texture[] textures, Microsoft.DirectX.Direct3D.Mesh mesh)
        {
            transform.Push();
            transform.TranslateLocal(obj.Position.X, obj.Position.Y, obj.Position.Z);
            transform.RotateYawPitchRollLocal(BAMSToRad(obj.Rotation.Y), BAMSToRad(obj.Rotation.X), BAMSToRad(obj.Rotation.Z));
            transform.ScaleLocal(obj.Scale.X, obj.Scale.Y, obj.Scale.Z);
            if (obj.Attach != null & (obj.Flags & ObjectFlags.NoDisplay) == 0)
            {
                device.SetTransform(TransformType.World, transform.Top);
                for (int j = 0; j < obj.Attach.Mesh.Length; j++)
                {
                    Material mat = obj.Attach.Material[obj.Attach.Mesh[j].MaterialID];
                    device.Material = new Microsoft.DirectX.Direct3D.Material
                    {
                        Diffuse = mat.DiffuseColor,
                        Ambient = mat.DiffuseColor,
                        Specular = mat.SpecularColor,
                        SpecularSharpness = mat.Unknown1
                    };
                    if (textures != null && mat.TextureID < textures.Length)
                        device.SetTexture(0, textures[mat.TextureID]);
                    else
                        device.SetTexture(0, null);
                    device.SetRenderState(RenderStates.AlphaBlendEnable, (mat.Flags & 0x10) == 0x10);
                    if ((mat.Flags & 0x40) == 0x40)
                        device.SetTextureStageState(0, TextureStageStates.TextureCoordinateIndex, 0x40000);
                    else
                        device.SetTextureStageState(0, TextureStageStates.TextureCoordinateIndex, 0);
                    mesh.DrawSubset(j);
                }
            }
            transform.Pop();
        }

        public static void DrawModelTree(this Object obj, Device device, MatrixStack transform, Texture[] textures, Microsoft.DirectX.Direct3D.Mesh[] meshes, ref int modelindex)
        {
                transform.Push();
                modelindex++;
                transform.TranslateLocal(obj.Position.X, obj.Position.Y, obj.Position.Z);
                transform.RotateYawPitchRollLocal(BAMSToRad(obj.Rotation.Y), BAMSToRad(obj.Rotation.X), BAMSToRad(obj.Rotation.Z));
                transform.ScaleLocal(obj.Scale.X, obj.Scale.Y, obj.Scale.Z);
                if (obj.Attach != null & (obj.Flags & ObjectFlags.NoDisplay) == 0)
                {
                    device.SetTransform(TransformType.World, transform.Top);
                    for (int j = 0; j < obj.Attach.Mesh.Length; j++)
                    {
                        Material mat = obj.Attach.Material[obj.Attach.Mesh[j].MaterialID];
                        device.Material = new Microsoft.DirectX.Direct3D.Material
                        {
                            Diffuse = mat.DiffuseColor,
                            Ambient = mat.DiffuseColor,
                            Specular = mat.SpecularColor,
                            SpecularSharpness = mat.Unknown1
                        };
                        if (textures != null && mat.TextureID < textures.Length)
                            device.SetTexture(0, textures[mat.TextureID]);
                        else
                            device.SetTexture(0, null);
                        device.SetRenderState(RenderStates.AlphaBlendEnable, (mat.Flags & 0x10) == 0x10);
                        if ((mat.Flags & 0x40) == 0x40)
                            device.SetTextureStageState(0, TextureStageStates.TextureCoordinateIndex, 0x40000);
                        else
                            device.SetTextureStageState(0, TextureStageStates.TextureCoordinateIndex, 0);
                        meshes[modelindex].DrawSubset(j);
                    }
                }
                foreach (Object child in obj.Children)
                    DrawModelTree(child, device, transform, textures, meshes, ref modelindex);
                transform.Pop();
        }
    }
}