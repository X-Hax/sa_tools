using System;
using System.Collections.Generic;
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
            BoundingSphere bound = BoundingSphere.FromPoints(verts.ToArray());
            col.Center.X = bound.Center.X;
            col.Center.Y = bound.Center.Y;
            col.Center.Z = bound.Center.Z;
            col.Radius = bound.Radius;
        }

        public static SlimDX.Direct3D9.Mesh CreateD3DMesh(this Attach attach, SlimDX.Direct3D9.Device dev)
        {
            int numverts = 0;
            VertexData[][] verts = attach.GetVertexData();
            foreach (VertexData[] item in verts)
                numverts += item.Length;
            SlimDX.Direct3D9.Mesh functionReturnValue = new SlimDX.Direct3D9.Mesh(dev, numverts / 3, numverts, MeshFlags.Managed, FVF_PositionNormalTexturedColored.Elements);
            DataStream vb = functionReturnValue.LockVertexBuffer(LockFlags.None);
            DataStream ib = functionReturnValue.LockIndexBuffer(LockFlags.None);
            DataStream at = functionReturnValue.LockAttributeBuffer(LockFlags.None);
            ushort vind = 0;
            for (int i = 0; i < verts.Length; i++)
            {
                for (int j = 0; j < verts[i].Length; j++)
                {
                    vb.Write(new FVF_PositionNormalTexturedColored(verts[i][j]));
                    ib.Write(vind);
                    vind++;
                }
                for (int j = 0; j < verts[i].Length / 3; j++)
                {
                    at.Write(i);
                }
            }
            functionReturnValue.UnlockVertexBuffer();
            functionReturnValue.UnlockIndexBuffer();
            functionReturnValue.UnlockAttributeBuffer();
            return functionReturnValue;
        }

        public static float BAMSToRad(int BAMS)
        {
            return (float)(BAMS / (65536 / (2 * Math.PI)));
        }

        public static void DrawModel(this Object obj, Device device, MatrixStack transform, Texture[] textures, SlimDX.Direct3D9.Mesh mesh)
        {
            if (obj != null)
            {
                transform.Push();
                transform.TranslateLocal(obj.Position.X, obj.Position.Y, obj.Position.Z);
                transform.RotateYawPitchRollLocal(BAMSToRad(obj.Rotation.Y), BAMSToRad(obj.Rotation.X), BAMSToRad(obj.Rotation.Z));
                transform.ScaleLocal(obj.Scale.X, obj.Scale.Y, obj.Scale.Z);
                if (obj.Attach != null & (obj.Flags & ObjectFlags.NoDisplay) == 0)
                {
                    device.SetTransform(TransformState.World, transform.Top);
                    for (int j = 0; j < obj.Attach.Mesh.Length; j++)
                    {
                        Material mat = obj.Attach.Material[obj.Attach.Mesh[j].MaterialID];
                        device.Material = new SlimDX.Direct3D9.Material
                        {
                            Diffuse = mat.DiffuseColor,
                            Ambient = mat.DiffuseColor,
                            Specular = mat.SpecularColor,
                            Power = mat.Unknown1
                        };
                        if (textures != null && mat.TextureID < textures.Length)
                            device.SetTexture(0, textures[mat.TextureID]);
                        else
                            device.SetTexture(0, null);
                        device.SetRenderState(RenderState.AlphaBlendEnable, (mat.Flags & 0x10) == 0x10);
                        if ((mat.Flags & 0x40) == 0x40)
                            device.SetTextureStageState(0, TextureStage.TexCoordIndex, 0x40000);
                        else
                            device.SetTextureStageState(0, TextureStage.TexCoordIndex, 0);
                        mesh.DrawSubset(j);
                    }
                }
                transform.Pop();
            }
        }

        public static void DrawModelTree(this Object obj, Device device, MatrixStack transform, Texture[] textures, SlimDX.Direct3D9.Mesh[] meshes, ref int modelindex)
        {
            while (obj != null)
            {
                transform.Push();
                modelindex++;
                transform.TranslateLocal(obj.Position.X, obj.Position.Y, obj.Position.Z);
                transform.RotateYawPitchRollLocal(BAMSToRad(obj.Rotation.Y), BAMSToRad(obj.Rotation.X), BAMSToRad(obj.Rotation.Z));
                transform.ScaleLocal(obj.Scale.X, obj.Scale.Y, obj.Scale.Z);
                if (obj.Attach != null & (obj.Flags & ObjectFlags.NoDisplay) == 0)
                {
                    device.SetTransform(TransformState.World, transform.Top);
                    for (int j = 0; j < obj.Attach.Mesh.Length; j++)
                    {
                        Material mat = obj.Attach.Material[obj.Attach.Mesh[j].MaterialID];
                        device.Material = new SlimDX.Direct3D9.Material
                        {
                            Diffuse = mat.DiffuseColor,
                            Ambient = mat.DiffuseColor,
                            Specular = mat.SpecularColor,
                            Power = mat.Unknown1
                        };
                        if (textures != null && mat.TextureID < textures.Length)
                            device.SetTexture(0, textures[mat.TextureID]);
                        else
                            device.SetTexture(0, null);
                        device.SetRenderState(RenderState.AlphaBlendEnable, (mat.Flags & 0x10) == 0x10);
                        if ((mat.Flags & 0x40) == 0x40)
                            device.SetTextureStageState(0, TextureStage.TexCoordIndex, 0x40000);
                        else
                            device.SetTextureStageState(0, TextureStage.TexCoordIndex, 0);
                        meshes[modelindex].DrawSubset(j);
                    }
                }
                if (obj.Child != null)
                    DrawModelTree(obj.Child, device, transform, textures, meshes, ref modelindex);
                transform.Pop();
                obj = obj.Sibling;
            }
        }
    }
}