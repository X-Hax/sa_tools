using System;
using System.Collections.Generic;
using System.Drawing;
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
            foreach (MeshInfo mesh in attach.MeshInfo)
                foreach (VertexData vert in mesh.Vertices)
                    verts.Add(vert.Position.ToVector3());
            Vector3 center;
            attach.Bounds.Radius = Geometry.ComputeBoundingSphere(verts.ToArray(), VertexFormats.Position, out center);
            attach.Bounds.Center.X = center.X;
            attach.Bounds.Center.Y = center.Y;
            attach.Bounds.Center.Z = center.Z;
        }

        public static BoundingSphere CalculateBounds(this Attach attach, int mesh, Matrix transform)
        {
            List<Vector3> verts = new List<Vector3>();
            foreach (VertexData vert in attach.MeshInfo[mesh].Vertices)
                verts.Add(Vector3.TransformCoordinate(vert.Position.ToVector3(), transform));
            Vector3 center;
            float radius = Geometry.ComputeBoundingSphere(verts.ToArray(), VertexFormats.Position, out center);
            return new BoundingSphere(center.X, center.Y, center.Z, radius);
        }

        public static void CalculateBounds(this COL col)
        {
            Matrix matrix = Matrix.Identity;
            matrix *= Matrix.RotationYawPitchRoll(BAMSToRad(col.Model.Rotation.Y), BAMSToRad(col.Model.Rotation.X), BAMSToRad(col.Model.Rotation.Z));
            matrix *= Matrix.Translation(col.Model.Position.ToVector3());
            List<Vector3> verts = new List<Vector3>();
            foreach (MeshInfo mesh in col.Model.Attach.MeshInfo)
                foreach (VertexData vert in mesh.Vertices)
                    verts.Add(Vector3.TransformCoordinate(vert.Position.ToVector3(), matrix));
            Vector3 center = new Vector3();
            col.Bounds.Radius = Geometry.ComputeBoundingSphere(verts.ToArray(), VertexFormats.Position, out center);
            col.Bounds.Center.X = center.X;
            col.Bounds.Center.Y = center.Y;
            col.Bounds.Center.Z = center.Z;
        }

        public static Microsoft.DirectX.Direct3D.Mesh CreateD3DMesh(this Attach attach, Microsoft.DirectX.Direct3D.Device dev)
        {
            int numverts = 0;
            foreach (MeshInfo item in attach.MeshInfo)
                numverts += item.Vertices.Length;
            if (numverts == 0) return null;
            Microsoft.DirectX.Direct3D.Mesh functionReturnValue = new Microsoft.DirectX.Direct3D.Mesh(numverts / 3, numverts, MeshFlags.Managed, FVF_PositionNormalTexturedColored.Elements, dev);
            List<FVF_PositionNormalTexturedColored> vb = new List<FVF_PositionNormalTexturedColored>();
            List<short> ib = new List<short>();
            int[] at = functionReturnValue.LockAttributeBufferArray(LockFlags.None);
            int vind;
            for (int i = 0; i < attach.MeshInfo.Length; i++)
            {
                vind = vb.Count;
                for (int j = 0; j < attach.MeshInfo[i].Vertices.Length; j++)
                {
                    vb.Add(new FVF_PositionNormalTexturedColored(attach.MeshInfo[i].Vertices[j], attach.MeshInfo[i].Material));
                    ib.Add((short)(vind + j));
                }
                for (int j = 0; j < attach.MeshInfo[i].Vertices.Length / 3; j++)
                    at[(vind / 3) + j] = i;
            }
            functionReturnValue.SetVertexBufferData(vb.ToArray(), LockFlags.None);
            functionReturnValue.SetIndexBufferData(ib.ToArray(), LockFlags.None);
            functionReturnValue.UnlockAttributeBuffer(at);

            int[] adjacency = new int[functionReturnValue.NumberFaces * 3];
            functionReturnValue.GenerateAdjacency(0.0001f, adjacency);
            functionReturnValue.Optimize(MeshFlags.OptimizeCompact, adjacency);

            return functionReturnValue;
        }

        public static float BAMSToRad(int BAMS)
        {
            return (float)(BAMS / (65536 / (2 * Math.PI)));
        }

        public static RenderInfo[] DrawModel(this Object obj, Device device, MatrixStack transform, Texture[] textures, Microsoft.DirectX.Direct3D.Mesh mesh, bool useMat)
        {
            if (mesh == null) return new RenderInfo[0];
            List<RenderInfo> result = new List<RenderInfo>();
            transform.Push();
            transform.TranslateLocal(obj.Position.X, obj.Position.Y, obj.Position.Z);
            transform.RotateYawPitchRollLocal(BAMSToRad(obj.Rotation.Y), BAMSToRad(obj.Rotation.X), BAMSToRad(obj.Rotation.Z));
            transform.ScaleLocal(obj.Scale.X, obj.Scale.Y, obj.Scale.Z);
            if (obj.Attach != null)
                for (int j = 0; j < obj.Attach.MeshInfo.Length; j++)
                {
                    Material mat;
                    Texture texture = null;
                    if (useMat)
                    {
                        mat = obj.Attach.MeshInfo[j].Material;
                        if (textures != null && mat.TextureID < textures.Length)
                            texture = textures[mat.TextureID];
                    }
                    else
                    {
                        mat = new Material
                        {
                            DiffuseColor = Color.White,
                            IgnoreLighting = true,
                            UseAlpha = false
                        };
                    }
                    result.Add(new RenderInfo(mesh, j, transform.Top, mat, texture, device.RenderState.FillMode, obj.Attach.CalculateBounds(j, transform.Top)));
                }
            transform.Pop();
            return result.ToArray();
        }

        public static RenderInfo[] DrawModelInvert(this Object obj, Device device, MatrixStack transform, Microsoft.DirectX.Direct3D.Mesh mesh, bool useMat)
        {
            if (mesh == null) return new RenderInfo[0];
            List<RenderInfo> result = new List<RenderInfo>();
            transform.Push();
            transform.TranslateLocal(obj.Position.X, obj.Position.Y, obj.Position.Z);
            transform.RotateYawPitchRollLocal(BAMSToRad(obj.Rotation.Y), BAMSToRad(obj.Rotation.X), BAMSToRad(obj.Rotation.Z));
            transform.ScaleLocal(obj.Scale.X, obj.Scale.Y, obj.Scale.Z);
            if (obj.Attach != null)
                for (int j = 0; j < obj.Attach.MeshInfo.Length; j++)
                {
                    System.Drawing.Color col = Color.White;
                    if (useMat) col = obj.Attach.MeshInfo[j].Material.DiffuseColor;
                    col = System.Drawing.Color.FromArgb(255 - col.R, 255 - col.G, 255 - col.B);
                    Material mat = new Material
                    {
                        DiffuseColor = col,
                        IgnoreLighting = true,
                        UseAlpha = false
                    };
                    result.Add(new RenderInfo(mesh, j, transform.Top, mat, null, FillMode.WireFrame, obj.Attach.CalculateBounds(j, transform.Top)));
                }
            transform.Pop();
            return result.ToArray();
        }

        public static RenderInfo[] DrawModelTree(this Object obj, Device device, MatrixStack transform, Texture[] textures, Microsoft.DirectX.Direct3D.Mesh[] meshes)
        {
            int modelindex = -1;
            return obj.DrawModelTree(device, transform, textures, meshes, ref modelindex);
        }

        private static RenderInfo[] DrawModelTree(this Object obj, Device device, MatrixStack transform, Texture[] textures, Microsoft.DirectX.Direct3D.Mesh[] meshes, ref int modelindex)
        {
            List<RenderInfo> result = new List<RenderInfo>();
            transform.Push();
            modelindex++;
            transform.TranslateLocal(obj.Position.X, obj.Position.Y, obj.Position.Z);
            transform.RotateYawPitchRollLocal(BAMSToRad(obj.Rotation.Y), BAMSToRad(obj.Rotation.X), BAMSToRad(obj.Rotation.Z));
            transform.ScaleLocal(obj.Scale.X, obj.Scale.Y, obj.Scale.Z);
            if (obj.Attach != null & meshes[modelindex] != null)
                for (int j = 0; j < obj.Attach.MeshInfo.Length; j++)
                {
                    Material mat;
                    Texture texture = null;
                    if ((obj.Flags & ObjectFlags.NoDisplay) == 0)
                    {
                        mat = obj.Attach.MeshInfo[j].Material;
                        if (textures != null && mat.TextureID < textures.Length)
                            texture = textures[mat.TextureID];
                    }
                    else
                    {
                        mat = new Material
                        {
                            DiffuseColor = Color.White,
                            IgnoreLighting = true,
                            UseAlpha = false
                        };
                    }
                    result.Add(new RenderInfo(meshes[modelindex], j, transform.Top, mat, texture, device.RenderState.FillMode, obj.Attach.CalculateBounds(j, transform.Top)));
                }
            foreach (Object child in obj.Children)
                result.AddRange(DrawModelTree(child, device, transform, textures, meshes, ref modelindex));
            transform.Pop();
            return result.ToArray();
        }

        public static RenderInfo[] DrawModelTreeInvert(this Object obj, Device device, MatrixStack transform, Microsoft.DirectX.Direct3D.Mesh[] meshes)
        {
            int modelindex = -1;
            return obj.DrawModelTreeInvert(device, transform, meshes, ref modelindex);
        }

        private static RenderInfo[] DrawModelTreeInvert(this Object obj, Device device, MatrixStack transform, Microsoft.DirectX.Direct3D.Mesh[] meshes, ref int modelindex)
        {
            List<RenderInfo> result = new List<RenderInfo>();
            transform.Push();
            modelindex++;
            transform.TranslateLocal(obj.Position.X, obj.Position.Y, obj.Position.Z);
            transform.RotateYawPitchRollLocal(BAMSToRad(obj.Rotation.Y), BAMSToRad(obj.Rotation.X), BAMSToRad(obj.Rotation.Z));
            transform.ScaleLocal(obj.Scale.X, obj.Scale.Y, obj.Scale.Z);
            if (obj.Attach != null & meshes[modelindex] != null)
                for (int j = 0; j < obj.Attach.MeshInfo.Length; j++)
                {
                    System.Drawing.Color col = obj.Attach.MeshInfo[j].Material.DiffuseColor;
                    if ((obj.Flags & ObjectFlags.NoDisplay) == ObjectFlags.NoDisplay) col = Color.White;
                    col = System.Drawing.Color.FromArgb(255 - col.R, 255 - col.G, 255 - col.B);
                    Material mat = new Material
                    {
                        DiffuseColor = col,
                        IgnoreLighting = true,
                        UseAlpha = false
                    };
                    result.Add(new RenderInfo(meshes[modelindex], j, transform.Top, mat, null, FillMode.WireFrame, obj.Attach.CalculateBounds(j, transform.Top)));
                }
            foreach (Object child in obj.Children)
                result.AddRange(DrawModelTreeInvert(child, device, transform, meshes, ref modelindex));
            transform.Pop();
            return result.ToArray();
        }

        public static RenderInfo[] DrawModelTreeAnimated(this Object obj, Device device, MatrixStack transform, Texture[] textures, Microsoft.DirectX.Direct3D.Mesh[] meshes, Animation anim, int animframe)
        {
            int modelindex = -1;
            int animindex = -1;
            return obj.DrawModelTreeAnimated(device, transform, textures, meshes, anim, animframe, ref modelindex, ref animindex);
        }

        private static RenderInfo[] DrawModelTreeAnimated(this Object obj, Device device, MatrixStack transform, Texture[] textures, Microsoft.DirectX.Direct3D.Mesh[] meshes, Animation anim, int animframe, ref int modelindex, ref int animindex)
        {
            List<RenderInfo> result = new List<RenderInfo>();
            transform.Push();
            modelindex++;
            bool animate = ((obj.Flags & ObjectFlags.NoAnimate) == 0);
            if (animate) animindex++;
            if (!anim.Models.ContainsKey(animindex)) animate = false;
            if (animate)
            {
                if (anim.Models[animindex].Position.Count > 0)
                    transform.TranslateLocal(anim.Models[animindex].GetPosition(animframe).ToVector3());
                else
                    transform.TranslateLocal(obj.Position.ToVector3());
                Rotation rot;
                if (anim.Models[animindex].Rotation.Count > 0)
                    rot = anim.Models[animindex].GetRotation(animframe);
                else
                    rot = obj.Rotation;
                transform.RotateXYZLocal(rot.X, rot.Y, rot.Z);
                if (anim.Models[animindex].Scale.Count > 0)
                    transform.ScaleLocal(anim.Models[animindex].GetScale(animframe).ToVector3());
                else
                    transform.ScaleLocal(obj.Scale.ToVector3());
            }
            else
            {
                transform.TranslateLocal(obj.Position.X, obj.Position.Y, obj.Position.Z);
                transform.RotateYawPitchRollLocal(BAMSToRad(obj.Rotation.Y), BAMSToRad(obj.Rotation.X), BAMSToRad(obj.Rotation.Z));
                transform.ScaleLocal(obj.Scale.X, obj.Scale.Y, obj.Scale.Z);
            }
            if (obj.Attach != null & meshes[modelindex] != null)
                for (int j = 0; j < obj.Attach.MeshInfo.Length; j++)
                {
                    Material mat;
                    Texture texture = null;
                    if ((obj.Flags & ObjectFlags.NoDisplay) == 0)
                    {
                        mat = obj.Attach.MeshInfo[j].Material;
                        if (textures != null && mat.TextureID < textures.Length)
                            texture = textures[mat.TextureID];
                    }
                    else
                    {
                        mat = new Material
                        {
                            DiffuseColor = Color.White,
                            IgnoreLighting = true,
                            UseAlpha = false
                        };
                    }
                    result.Add(new RenderInfo(meshes[modelindex], j, transform.Top, mat, texture, device.RenderState.FillMode, obj.Attach.CalculateBounds(j, transform.Top)));
                }
            foreach (Object child in obj.Children)
                result.AddRange(DrawModelTreeAnimated(child, device, transform, textures, meshes, anim, animframe, ref modelindex, ref animindex));
            transform.Pop();
            return result.ToArray();
        }

        public static RenderInfo[] DrawModelTreeAnimatedInvert(this Object obj, Device device, MatrixStack transform, Microsoft.DirectX.Direct3D.Mesh[] meshes, Animation anim, int animframe)
        {
            int modelindex = -1;
            int animindex = -1;
            return obj.DrawModelTreeAnimatedInvert(device, transform, meshes, anim, animframe, ref modelindex, ref animindex);
        }

        private static RenderInfo[] DrawModelTreeAnimatedInvert(this Object obj, Device device, MatrixStack transform, Microsoft.DirectX.Direct3D.Mesh[] meshes, Animation anim, int animframe, ref int modelindex, ref int animindex)
        {
            List<RenderInfo> result = new List<RenderInfo>();
            transform.Push();
            modelindex++;
            bool animate = ((obj.Flags & ObjectFlags.NoAnimate) == 0);
            if (animate) animindex++;
            if (!anim.Models.ContainsKey(animindex)) animate = false;
            if (animate)
            {
                if (anim.Models[animindex].Position.Count > 0)
                    transform.TranslateLocal(anim.Models[animindex].GetPosition(animframe).ToVector3());
                else
                    transform.TranslateLocal(obj.Position.ToVector3());
                Rotation rot;
                if (anim.Models[animindex].Rotation.Count > 0)
                    rot = anim.Models[animindex].GetRotation(animframe);
                else
                    rot = obj.Rotation;
                transform.RotateXYZLocal(rot.X, rot.Y, rot.Z);
                if (anim.Models[animindex].Scale.Count > 0)
                    transform.ScaleLocal(anim.Models[animindex].GetScale(animframe).ToVector3());
                else
                    transform.ScaleLocal(obj.Scale.ToVector3());
            }
            else
            {
                transform.TranslateLocal(obj.Position.X, obj.Position.Y, obj.Position.Z);
                transform.RotateYawPitchRollLocal(BAMSToRad(obj.Rotation.Y), BAMSToRad(obj.Rotation.X), BAMSToRad(obj.Rotation.Z));
                transform.ScaleLocal(obj.Scale.X, obj.Scale.Y, obj.Scale.Z);
            }
            if (obj.Attach != null & meshes[modelindex] != null)
                for (int j = 0; j < obj.Attach.MeshInfo.Length; j++)
                {
                    System.Drawing.Color col = obj.Attach.MeshInfo[j].Material.DiffuseColor;
                    if ((obj.Flags & ObjectFlags.NoDisplay) == ObjectFlags.NoDisplay) col = Color.White;
                    col = System.Drawing.Color.FromArgb(255 - col.R, 255 - col.G, 255 - col.B);
                    Material mat = new Material
                    {
                        DiffuseColor = col,
                        IgnoreLighting = true,
                        UseAlpha = false
                    };
                    result.Add(new RenderInfo(meshes[modelindex], j, transform.Top, mat, null, FillMode.WireFrame, obj.Attach.CalculateBounds(j, transform.Top)));
                }
            foreach (Object child in obj.Children)
                result.AddRange(DrawModelTreeAnimatedInvert(child, device, transform, meshes, anim, animframe, ref modelindex, ref animindex));
            transform.Pop();
            return result.ToArray();
        }

        public static HitResult CheckHit(this Object obj, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, Microsoft.DirectX.Direct3D.Mesh mesh)
        {
            if (mesh == null) return HitResult.NoHit;
            MatrixStack transform = new MatrixStack();
            transform.Push();
            transform.TranslateLocal(obj.Position.ToVector3());
            transform.RotateYawPitchRollLocal(BAMSToRad(obj.Rotation.Y), BAMSToRad(obj.Rotation.X), BAMSToRad(obj.Rotation.Z));
            Vector3 pos = Vector3.Unproject(Near, Viewport, Projection, View, transform.Top);
            Vector3 dir = Vector3.Subtract(pos, Vector3.Unproject(Far, Viewport, Projection, View, transform.Top));
            IntersectInformation info;
            if (!mesh.Intersect(pos, dir, out info)) return HitResult.NoHit;
            return new HitResult(obj, info.Dist);
        }

        public static HitResult CheckHit(this Object obj, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform, Microsoft.DirectX.Direct3D.Mesh[] mesh)
        {
            int modelindex = -1;
            return CheckHit(obj, Near, Far, Viewport, Projection, View, transform, mesh, ref modelindex);
        }

        private static HitResult CheckHit(this Object obj, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform, Microsoft.DirectX.Direct3D.Mesh[] mesh, ref int modelindex)
        {
            transform.Push();
            modelindex++;
            transform.TranslateLocal(obj.Position.X, obj.Position.Y, obj.Position.Z);
            transform.RotateYawPitchRollLocal(BAMSToRad(obj.Rotation.Y), BAMSToRad(obj.Rotation.X), BAMSToRad(obj.Rotation.Z));
            transform.ScaleLocal(obj.Scale.X, obj.Scale.Y, obj.Scale.Z);
            HitResult result = HitResult.NoHit;
            if (obj.Attach != null & mesh[modelindex] != null)
            {
                Vector3 pos = Vector3.Unproject(Near, Viewport, Projection, View, transform.Top);
                Vector3 dir = Vector3.Subtract(pos, Vector3.Unproject(Far, Viewport, Projection, View, transform.Top));
                IntersectInformation info;
                if (mesh[modelindex].Intersect(pos, dir, out info))
                {
                    if (!result.IsHit)
                        result = new HitResult(obj, info.Dist);
                    else if (result.Distance > info.Dist)
                        result = new HitResult(obj, info.Dist);
                }
            }
            foreach (Object child in obj.Children)
                result = HitResult.Min(result, CheckHit(child, Near, Far, Viewport, Projection, View, transform, mesh, ref modelindex));
            transform.Pop();
            return result;
        }

        public static HitResult CheckHitAnimated(this Object obj, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform, Microsoft.DirectX.Direct3D.Mesh[] mesh, Animation anim, int animframe)
        {
            int modelindex = -1;
            int animindex = -1;
            return CheckHitAnimated(obj, Near, Far, Viewport, Projection, View, transform, mesh, anim, animframe, ref modelindex, ref animindex);
        }

        private static HitResult CheckHitAnimated(this Object obj, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform, Microsoft.DirectX.Direct3D.Mesh[] mesh, Animation anim, int animframe, ref int modelindex, ref int animindex)
        {
            transform.Push();
            modelindex++;
            bool animate = ((obj.Flags & ObjectFlags.NoAnimate) == 0);
            if (animate) animindex++;
            if (!anim.Models.ContainsKey(animindex)) animate = false;
            if (animate)
            {
                if (anim.Models[animindex].Position.Count > 0)
                    transform.TranslateLocal(anim.Models[animindex].GetPosition(animframe).ToVector3());
                else
                    transform.TranslateLocal(obj.Position.ToVector3());
                Rotation rot;
                if (anim.Models[animindex].Rotation.Count > 0)
                    rot = anim.Models[animindex].GetRotation(animframe);
                else
                    rot = obj.Rotation;
                transform.RotateXYZLocal(rot.X, rot.Y, rot.Z);
                if (anim.Models[animindex].Scale.Count > 0)
                    transform.ScaleLocal(anim.Models[animindex].GetScale(animframe).ToVector3());
                else
                    transform.ScaleLocal(obj.Scale.ToVector3());
            }
            else
            {
                transform.TranslateLocal(obj.Position.X, obj.Position.Y, obj.Position.Z);
                transform.RotateYawPitchRollLocal(BAMSToRad(obj.Rotation.Y), BAMSToRad(obj.Rotation.X), BAMSToRad(obj.Rotation.Z));
                transform.ScaleLocal(obj.Scale.X, obj.Scale.Y, obj.Scale.Z);
            }
            HitResult result = HitResult.NoHit;
            if (obj.Attach != null & mesh[modelindex] != null)
            {
                Vector3 pos = Vector3.Unproject(Near, Viewport, Projection, View, transform.Top);
                Vector3 dir = Vector3.Subtract(pos, Vector3.Unproject(Far, Viewport, Projection, View, transform.Top));
                IntersectInformation info;
                if (mesh[modelindex].Intersect(pos, dir, out info))
                {
                    if (!result.IsHit)
                        result = new HitResult(obj, info.Dist);
                    else if (result.Distance > info.Dist)
                        result = new HitResult(obj, info.Dist);
                }
            }
            foreach (Object child in obj.Children)
                result = HitResult.Min(result, CheckHitAnimated(child, Near, Far, Viewport, Projection, View, transform, mesh, anim, animframe, ref modelindex, ref animindex));
            transform.Pop();
            return result;
        }

        public static Attach obj2nj(string objfile)
        {
            string[] obj = System.IO.File.ReadAllLines(objfile);
            Attach model;
            List<UV> uvs = new List<UV>();
            List<Color> vcolors = new List<Color>();
            List<Vertex> verts = new List<Vertex>();
            List<Vertex> norms = new List<Vertex>();
            Dictionary<string, Material> mtls = new Dictionary<string, Material>();
            Material lastmtl = null;
            List<Vertex> model_Vertex = new List<Vertex>();
            List<Vertex> model_Normal = new List<Vertex>();
            List<Material> model_Material = new List<Material>();
            List<Mesh> model_Mesh = new List<Mesh>();
            List<ushort> model_Mesh_MaterialID = new List<ushort>();
            List<List<Poly>> model_Mesh_Poly = new List<List<Poly>>();
            List<List<UV>> model_Mesh_UV = new List<List<UV>>();
            List<List<Color>> model_Mesh_VColor = new List<List<Color>>();
            foreach (string ln in obj)
            {
                string[] lin = ln.Split('#')[0].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (lin.Length == 0)
                    continue;
                switch (lin[0].ToLowerInvariant())
                {
                    case "mtllib":
                        string[] mtlfile = System.IO.File.ReadAllLines(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(objfile), lin[1]));
                        foreach (string mln in mtlfile)
                        {
                            string[] mlin = mln.Split('#')[0].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            if (mlin.Length == 0)
                                continue;
                            #region Parsing Material Properties
                            switch (mlin[0].ToLowerInvariant())
                            {
                                case "newmtl":
                                    lastmtl = new Material();
                                    lastmtl.UseAlpha = false;
                                    lastmtl.UseTexture = false;
                                    mtls.Add(mlin[1], lastmtl);
                                    break;

                                case "kd":
                                    lastmtl.DiffuseColor = Color.FromArgb((int)Math.Round(float.Parse(mlin[1], System.Globalization.CultureInfo.InvariantCulture) * 255), (int)Math.Round(float.Parse(mlin[2], System.Globalization.CultureInfo.InvariantCulture) * 255), (int)Math.Round(float.Parse(mlin[3], System.Globalization.CultureInfo.InvariantCulture) * 255));
                                    break;

                                case "map_ka":
                                    lastmtl.UseAlpha = true;
                                    break;

                                case "map_kd":
                                    lastmtl.UseTexture = true;
                                    break;

                                case "ke":
                                    lastmtl.Exponent = float.Parse(mlin[1], System.Globalization.CultureInfo.InvariantCulture);
                                    break;

                                case "d":
                                case "tr":
                                    lastmtl.DiffuseColor = Color.FromArgb((int)Math.Round(float.Parse(mlin[1], System.Globalization.CultureInfo.InvariantCulture) * 255), lastmtl.DiffuseColor);
                                    break;

                                case "ks":
                                    lastmtl.SpecularColor = Color.FromArgb((int)Math.Round(float.Parse(mlin[1], System.Globalization.CultureInfo.InvariantCulture) * 255), (int)Math.Round(float.Parse(mlin[2], System.Globalization.CultureInfo.InvariantCulture) * 255), (int)Math.Round(float.Parse(mlin[3], System.Globalization.CultureInfo.InvariantCulture) * 255));
                                    break;

                                case "texid":
                                    lastmtl.TextureID = int.Parse(mlin[1], System.Globalization.CultureInfo.InvariantCulture);
                                    break;

                                case "-u_mirror":
                                    bool uMirror = false;

                                    if (bool.TryParse(mlin[1], out uMirror))
                                    {
                                        lastmtl.FlipU = uMirror;
                                    }
                                    break;

                                case "-v_mirror":
                                    bool vMirror = false;

                                    if (bool.TryParse(mlin[1], out vMirror))
                                    {
                                        lastmtl.FlipV = vMirror;
                                    }
                                    break;


                                case "-u_tile":
                                    bool uTile = true;

                                    if (bool.TryParse(mlin[1], out uTile))
                                    {
                                        lastmtl.ClampU = !uTile;
                                    }
                                    break;

                                case "-v_tile":
                                    bool vTile = true;

                                    if (bool.TryParse(mlin[1], out vTile))
                                    {
                                        lastmtl.ClampV = !vTile;
                                    }
                                    break;

                                case "-enviromap":
                                    lastmtl.EnvironmentMap = true;
                                    break;

                                case "-doublesided":
                                    lastmtl.DoubleSided = true;
                                    break;

                                case "-ignorelighting":
                                    lastmtl.IgnoreLighting = bool.Parse(mlin[1]);
                                    break;

                                case "-flatshaded":
                                    lastmtl.FlatShading = bool.Parse(mlin[1]);
                                    break;
                            }
                            #endregion
                        }

                        break;
                    case "v":
                        verts.Add(new Vertex(float.Parse(lin[1], System.Globalization.CultureInfo.InvariantCulture), float.Parse(lin[2], System.Globalization.CultureInfo.InvariantCulture), float.Parse(lin[3], System.Globalization.CultureInfo.InvariantCulture)));
                        break;
                    case "vn":
                        norms.Add(new Vertex(float.Parse(lin[1], System.Globalization.CultureInfo.InvariantCulture), float.Parse(lin[2], System.Globalization.CultureInfo.InvariantCulture), float.Parse(lin[3], System.Globalization.CultureInfo.InvariantCulture)));
                        break;
                    case "vt":
                        uvs.Add(new UV() { U = float.Parse(lin[1], System.Globalization.CultureInfo.InvariantCulture) * -1, V = float.Parse(lin[2], System.Globalization.CultureInfo.InvariantCulture) * -1 });
                        break;
                    case "vc":
                        vcolors.Add(Color.FromArgb((int)Math.Round(float.Parse(lin[1], System.Globalization.CultureInfo.InvariantCulture)), (int)Math.Round(float.Parse(lin[2], System.Globalization.CultureInfo.InvariantCulture)), (int)Math.Round(float.Parse(lin[3], System.Globalization.CultureInfo.InvariantCulture)), (int)Math.Round(float.Parse(lin[4], System.Globalization.CultureInfo.InvariantCulture))));
                        break;
                    case "usemtl":
                        model_Mesh_Poly.Add(new List<Poly>());
                        model_Mesh_UV.Add(new List<UV>());
                        model_Mesh_VColor.Add(new List<Color>());
                        if (mtls.ContainsKey(lin[1]))
                        {
                            Material mtl = mtls[lin[1]];
                            if (model_Material.Contains(mtl))
                                model_Mesh_MaterialID.Add((ushort)model_Material.IndexOf(mtl));
                            else
                            {
                                model_Mesh_MaterialID.Add((ushort)model_Material.Count);
                                model_Material.Add(mtl);
                            }
                        }
                        else
                            model_Mesh_MaterialID.Add(0);
                        break;
                    case "f":
                        if (model_Mesh_MaterialID.Count == 0)
                        {
                            model_Mesh_MaterialID.Add(0);
                            model_Mesh_Poly.Add(new List<Poly>());
                            model_Mesh_UV.Add(new List<UV>());
                            model_Mesh_VColor.Add(new List<Color>());
                        }
                        Vertex ver = default(Vertex);
                        Vertex nor = default(Vertex);
                        ushort[] pol = new ushort[3];
                        for (int i = 1; i <= 3; i++)
                        {
                            string[] lne = lin[i].Split('/');
                            ver = verts.GetItemNeg(int.Parse(lne[0]));
                            nor = norms.GetItemNeg(int.Parse(lne[2]));
                            if (uvs.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(lne[1]))
                                {
                                    model_Mesh_UV[model_Mesh_UV.Count - 1].Add(uvs.GetItemNeg(int.Parse(lne[1])));
                                }
                            }
                            if (vcolors.Count > 0)
                            {
                                if (lne.Length == 4)
                                {
                                    model_Mesh_VColor[model_Mesh_VColor.Count - 1].Add(vcolors.GetItemNeg(int.Parse(lne[3])));
                                }
                                else if (!string.IsNullOrEmpty(lne[1]))
                                {
                                    model_Mesh_VColor[model_Mesh_VColor.Count - 1].Add(vcolors.GetItemNeg(int.Parse(lne[1])));
                                }
                            }
                            int verind = model_Vertex.IndexOf(ver);
                            while (verind > -1)
                            {
                                if (model_Normal[verind] == nor)
                                    break; // TODO: might not be correct. Was : Exit While
                                verind = model_Vertex.IndexOf(ver, verind + 1);
                            }
                            if (verind > -1)
                            {
                                pol[i - 1] = (ushort)verind;
                            }
                            else
                            {
                                model_Vertex.Add(ver);
                                model_Normal.Add(nor);
                                pol[i - 1] = (ushort)(model_Vertex.Count - 1);
                            }
                        }
                        Poly tri = Poly.CreatePoly(Basic_PolyType.Triangles);
                        for (int i = 0; i < 3; i++)
                            tri.Indexes[i] = pol[i];
                        model_Mesh_Poly[model_Mesh_Poly.Count - 1].Add(tri);
                        break;
                    case "t":
                        if (model_Mesh_MaterialID.Count == 0)
                        {
                            model_Mesh_MaterialID.Add(0);
                            model_Mesh_Poly.Add(new List<Poly>());
                            model_Mesh_UV.Add(new List<UV>());
                            model_Mesh_VColor.Add(new List<Color>());
                        }
                        Vertex ver2 = default(Vertex);
                        Vertex nor2 = default(Vertex);
                        List<ushort> str = new List<ushort>();
                        for (int i = 1; i <= lin.Length - 1; i++)
                        {
                            ver2 = verts.GetItemNeg(int.Parse(lin[i]));
                            nor2 = norms.GetItemNeg(int.Parse(lin[i]));
                            if (uvs.Count > 0)
                                model_Mesh_UV[model_Mesh_UV.Count - 1].Add(uvs.GetItemNeg(int.Parse(lin[i])));
                            if (vcolors.Count > 0)
                                model_Mesh_VColor[model_Mesh_VColor.Count - 1].Add(vcolors.GetItemNeg(int.Parse(lin[i])));
                            int verind = model_Vertex.IndexOf(ver2);
                            while (verind > -1)
                            {
                                if (model_Normal[verind] == nor2)
                                    break; // TODO: might not be correct. Was : Exit While
                                verind = model_Vertex.IndexOf(ver2, verind + 1);
                            }
                            if (verind > -1)
                            {
                                str.Add((ushort)verind);
                            }
                            else
                            {
                                model_Vertex.Add(ver2);
                                model_Normal.Add(nor2);
                                str.Add((ushort)(model_Vertex.Count - 1));
                            }
                        }
                        model_Mesh_Poly[model_Mesh_Poly.Count - 1].Add(new Strip(str.ToArray(), false));
                        break;
                    case "q":
                        Vertex ver3 = default(Vertex);
                        Vertex nor3 = default(Vertex);
                        List<ushort> str2 = new List<ushort>(model_Mesh_Poly[model_Mesh_Poly.Count - 1][model_Mesh_Poly[model_Mesh_Poly.Count - 1].Count - 1].Indexes);
                        for (int i = 1; i <= lin.Length - 1; i++)
                        {
                            ver3 = verts.GetItemNeg(int.Parse(lin[i]));
                            nor3 = norms.GetItemNeg(int.Parse(lin[i]));
                            if (uvs.Count > 0)
                                model_Mesh_UV[model_Mesh_UV.Count - 1].Add(uvs.GetItemNeg(int.Parse(lin[i])));
                            if (vcolors.Count > 0)
                                model_Mesh_VColor[model_Mesh_VColor.Count - 1].Add(vcolors.GetItemNeg(int.Parse(lin[i])));
                            int verind = model_Vertex.IndexOf(ver3);
                            while (verind > -1)
                            {
                                if (model_Normal[verind] == nor3)
                                    break; // TODO: might not be correct. Was : Exit While
                                verind = model_Vertex.IndexOf(ver3, verind + 1);
                            }
                            if (verind > -1)
                            {
                                str2.Add((ushort)verind);
                            }
                            else
                            {
                                model_Vertex.Add(ver3);
                                model_Normal.Add(nor3);
                                str2.Add((ushort)(model_Vertex.Count - 1));
                            }
                        }
                        model_Mesh_Poly[model_Mesh_Poly.Count - 1][model_Mesh_Poly[model_Mesh_Poly.Count - 1].Count - 1] = new Strip(str2.ToArray(), false);
                        break;
                }
            }
            if (model_Material.Count == 0)
                model_Material.Add(new Material());
            for (int i = 0; i < model_Mesh_MaterialID.Count; i++)
            {
                model_Mesh.Add(new Mesh(model_Mesh_Poly[i].ToArray(), false, model_Mesh_UV[i].Count > 0, model_Mesh_VColor[i].Count > 0));
                model_Mesh[i].MaterialID = model_Mesh_MaterialID[i];
                if (model_Mesh[i].UV != null)
                {
                    for (int j = 0; j < model_Mesh[i].UV.Length; j++)
                        model_Mesh[i].UV[j] = model_Mesh_UV[i][j];
                }
                if (model_Mesh[i].VColor != null)
                {
                    for (int j = 0; j < model_Mesh[i].VColor.Length; j++)
                        model_Mesh[i].VColor[j] = model_Mesh_VColor[i][j];
                }
            }
            string sanitizedName = System.IO.Path.GetFileNameWithoutExtension(objfile);
            double throwAway = 0;
            if (double.TryParse(sanitizedName, out throwAway)) // checking to see if the name will cause compile-time issues.
            {
                sanitizedName = string.Concat("model_", sanitizedName);
            }
            model = new BasicAttach(model_Vertex.ToArray(), model_Normal.ToArray(), model_Mesh, model_Material) { Name = sanitizedName };
            model.ProcessVertexData();
            model.CalculateBounds();
            return model;
        }

        private static T GetItemNeg<T>(this List<T> list, int index)
        {
            if (index < 0)
                return list[list.Count + index];
            return list[index - 1];
        }

        private static readonly Vector3 XAxis = new Vector3(1, 0, 0);
        private static readonly Vector3 YAxis = new Vector3(0, 1, 0);
        private static readonly Vector3 ZAxis = new Vector3(0, 0, 1);

        public static void RotateXYZLocal(this MatrixStack transform, int x, int y, int z)
        {
            transform.RotateAxisLocal(ZAxis, BAMSToRad(z));
            transform.RotateAxisLocal(XAxis, BAMSToRad(x));
            transform.RotateAxisLocal(YAxis, BAMSToRad(y));
        }

        /// <summary>
        /// Writes an object model (basic format) to the specified stream, in Alias-Wavefront *.OBJ format.
        /// </summary>
        /// <param name="objstream">stream representing a wavefront obj file to export to</param>
        /// <param name="obj">Model to export.</param>
        /// <param name="transform">Used for calculating transforms.</param>
        /// <param name="totalVerts">This keeps track of how many verts have been exported to the current file. This is necessary because *.obj vertex indeces are file-level, not object-level.</param>
        /// <param name="totalNorms">This keeps track of how many vert normals have been exported to the current file. This is necessary because *.obj vertex normal indeces are file-level, not object-level.</param>
        /// <param name="totalUVs">This keeps track of how many texture verts have been exported to the current file. This is necessary because *.obj textue vert indeces are file-level, not object-level.</param>
        /// <param name="errorFlag">Set this to TRUE if you encounter an issue. The user will be alerted.</param>
        private static void WriteObjFromBasicAttach(System.IO.StreamWriter objstream, SAModel.Object obj, MatrixStack transform, ref int totalVerts, ref int totalNorms, ref int totalUVs, ref bool errorFlag)
        {
            transform.Push();
            transform.TranslateLocal(obj.Position.ToVector3());
            transform.RotateYawPitchRollLocal(SAModel.Direct3D.Extensions.BAMSToRad(obj.Rotation.Y), SAModel.Direct3D.Extensions.BAMSToRad(obj.Rotation.X), SAModel.Direct3D.Extensions.BAMSToRad(obj.Rotation.Z));
            transform.ScaleLocal(obj.Scale.ToVector3());

            if ((obj.Attach != null))
            {
                BasicAttach basicAttach = new BasicAttach();
                bool wroteNormals = false;

                if (obj.Attach is BasicAttach)
                {
                    basicAttach = (BasicAttach)obj.Attach;
                }
                if (obj.Attach is ChunkAttach)
                {
                    objstream.WriteLine("#Error - Chunk Model got sent to Basic writer.");
                    errorFlag = true;

                    goto skip_processing;
                }
                objstream.WriteLine("g " + obj.Name);

                #region Outputting Verts and Normals
                for (int vIndx = 0; vIndx < basicAttach.Vertex.Length; vIndx++)
                {
                    Vector3 inputVert = new Vector3(basicAttach.Vertex[vIndx].X, basicAttach.Vertex[vIndx].Y, basicAttach.Vertex[vIndx].Z);
                    Vector3 outputVert = Vector3.TransformCoordinate(inputVert, transform.Top);
                    objstream.WriteLine(String.Format("v {0} {1} {2}", outputVert.X, outputVert.Y, outputVert.Z));
                }

                if (basicAttach.Vertex.Length == basicAttach.Normal.Length)
                {
                    for (int vnIndx = 0; vnIndx < basicAttach.Normal.Length; vnIndx++)
                    {
                        objstream.WriteLine(String.Format("vn {0} {1} {2}", basicAttach.Normal[vnIndx].X, basicAttach.Normal[vnIndx].Y, basicAttach.Normal[vnIndx].Z));
                    }
                    wroteNormals = true;
                }
                #endregion

                #region Outputting Meshes
                for (int meshIndx = 0; meshIndx < basicAttach.Mesh.Count; meshIndx++)
                {
                    if (basicAttach.Material.Count > 0)
                    {
                        if (basicAttach.Material[basicAttach.Mesh[meshIndx].MaterialID].UseTexture)
                        {
                            objstream.WriteLine(String.Format("usemtl material_{0}", basicAttach.Material[basicAttach.Mesh[meshIndx].MaterialID].TextureID));
                        }
                    }

                    if (basicAttach.Mesh[meshIndx].UV != null)
                    {
                        for (int uvIndx = 0; uvIndx < basicAttach.Mesh[meshIndx].UV.Length; uvIndx++)
                        {
                            objstream.WriteLine(String.Format("vt {0} {1}", basicAttach.Mesh[meshIndx].UV[uvIndx].U, basicAttach.Mesh[meshIndx].UV[uvIndx].V * -1));
                        }
                    }

                    int processedUVStripCount = 0;
                    for (int polyIndx = 0; polyIndx < basicAttach.Mesh[meshIndx].Poly.Count; polyIndx++)
                    {
                        if (basicAttach.Mesh[meshIndx].Poly[polyIndx].PolyType == Basic_PolyType.Strips)
                        {
                            Strip polyStrip = (Strip)basicAttach.Mesh[meshIndx].Poly[polyIndx];
                            int expectedTrisCount = polyStrip.Indexes.Length - 2;
                            bool triangleWindReversed = polyStrip.Reversed;

                            for (int stripIndx = 0; stripIndx < polyStrip.Indexes.Length - 2; stripIndx++)
                            {
                                if (triangleWindReversed)
                                {
                                    Vector3 newFace = new Vector3((polyStrip.Indexes[stripIndx + 1] + 1), (polyStrip.Indexes[stripIndx] + 1), (polyStrip.Indexes[stripIndx + 2] + 1));

                                    if (basicAttach.Mesh[meshIndx].UV != null)
                                    {
                                        int uv1, uv2, uv3;

                                        uv1 = (stripIndx + 1) + processedUVStripCount + 1; // +1's are because obj indeces always start at 1, not 0
                                        uv2 = (stripIndx) + processedUVStripCount + 1;
                                        uv3 = (stripIndx + 2) + processedUVStripCount + 1;

                                        if (wroteNormals) objstream.WriteLine(String.Format("f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}", (int)newFace.X + totalVerts, uv1 + totalUVs, (int)newFace.X + totalNorms, (int)newFace.Y + totalVerts, uv2 + totalUVs, (int)newFace.Y + totalNorms, (int)newFace.Z + totalVerts, uv3 + totalUVs, (int)newFace.Z + totalNorms));
                                        else objstream.WriteLine(String.Format("f {0}/{1} {2}/{3} {4}/{5}", (int)newFace.X + totalVerts, uv1 + totalUVs, (int)newFace.Y + totalVerts, uv2 + totalUVs, (int)newFace.Z + totalVerts, uv3 + totalUVs));
                                    }
                                    else
                                    {
                                        if (wroteNormals) objstream.WriteLine(String.Format("f {0}//{1} {2}//{3} {4}//{5}", (int)newFace.X + totalVerts, (int)newFace.X + totalNorms, (int)newFace.Y + totalVerts, (int)newFace.Y + totalNorms, (int)newFace.Z + totalVerts, (int)newFace.Z + totalNorms));
                                        else objstream.WriteLine(String.Format("f {0} {1} {2}", (int)newFace.X + totalVerts, (int)newFace.Y + totalVerts, (int)newFace.Z + totalVerts));
                                    }
                                }
                                else
                                {
                                    Vector3 newFace = new Vector3((polyStrip.Indexes[stripIndx] + 1), (polyStrip.Indexes[stripIndx + 1] + 1), (polyStrip.Indexes[stripIndx + 2] + 1));

                                    if (basicAttach.Mesh[meshIndx].UV != null)
                                    {
                                        int uv1, uv2, uv3;

                                        uv1 = (stripIndx) + processedUVStripCount + 1; // +1's are because obj indeces always start at 1, not 0
                                        uv2 = stripIndx + 1 + processedUVStripCount + 1;
                                        uv3 = stripIndx + 2 + processedUVStripCount + 1;

                                        if (wroteNormals) objstream.WriteLine(String.Format("f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}", (int)newFace.X + totalVerts, uv1 + totalUVs, (int)newFace.X + totalNorms, (int)newFace.Y + totalVerts, uv2 + totalUVs, (int)newFace.Y + totalNorms, (int)newFace.Z + totalVerts, uv3 + totalUVs, (int)newFace.Z + totalNorms));
                                        else objstream.WriteLine(String.Format("f {0}/{1} {2}/{3} {4}/{5}", (int)newFace.X + totalVerts, uv1 + totalUVs, (int)newFace.Y + totalVerts, uv2 + totalUVs, (int)newFace.Z + totalVerts, uv3 + totalUVs));
                                    }
                                    else
                                    {
                                        if (wroteNormals) objstream.WriteLine(String.Format("f {0}//{1} {2}//{3} {4}//{5}", (int)newFace.X + totalVerts, (int)newFace.X + totalNorms, (int)newFace.Y + totalVerts, (int)newFace.Y + totalNorms, (int)newFace.Z + totalVerts, (int)newFace.Z + totalNorms));
                                        else objstream.WriteLine(String.Format("f {0} {1} {2}", (int)newFace.X + totalVerts, (int)newFace.Y + totalVerts, (int)newFace.Z + totalVerts));
                                    }
                                }

                                triangleWindReversed = !triangleWindReversed; // flip every other triangle or the output will be wrong
                            }

                            if (basicAttach.Mesh[meshIndx].UV != null)
                            {
                                processedUVStripCount += polyStrip.Indexes.Length;
                                objstream.WriteLine(String.Format("# processed UV strips this poly: {0}", processedUVStripCount));
                            }
                        }
                        else if (basicAttach.Mesh[meshIndx].Poly[polyIndx].PolyType == Basic_PolyType.Triangles)
                        {
                            for (int faceVIndx = 0; faceVIndx < basicAttach.Mesh[meshIndx].Poly[polyIndx].Indexes.Length - 3; faceVIndx++)
                            {
                                Vector3 newFace = new Vector3((basicAttach.Mesh[meshIndx].Poly[polyIndx].Indexes[faceVIndx] + 1), (basicAttach.Mesh[meshIndx].Poly[polyIndx].Indexes[faceVIndx + 1] + 1), (basicAttach.Mesh[meshIndx].Poly[polyIndx].Indexes[faceVIndx + 2] + 1));

                                if (basicAttach.Mesh[meshIndx].UV != null)
                                {
                                    int uv1, uv2, uv3;

                                    uv1 = (faceVIndx) + processedUVStripCount + 1; // +1's are because obj indeces always start at 1, not 0
                                    uv2 = faceVIndx + 1 + processedUVStripCount + 1;
                                    uv3 = faceVIndx + 2 + processedUVStripCount + 1;

                                    if (wroteNormals) objstream.WriteLine(String.Format("f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}", (int)newFace.X + totalVerts, uv1 + totalUVs, (int)newFace.X + totalNorms, (int)newFace.Y + totalVerts, uv2 + totalUVs, (int)newFace.Y + totalNorms, (int)newFace.Z + totalVerts, uv3 + totalUVs, (int)newFace.Z + totalNorms));
                                    else objstream.WriteLine(String.Format("f {0}/{1} {2}/{3} {4}/{5}", (int)newFace.X + totalVerts, uv1 + totalUVs, (int)newFace.Y + totalVerts, uv2 + totalUVs, (int)newFace.Z + totalVerts, uv3 + totalUVs));
                                }
                                else
                                {
                                    if (wroteNormals) objstream.WriteLine(String.Format("f {0}//{1} {2}//{3} {4}//{5}", (int)newFace.X + totalVerts, (int)newFace.X + totalNorms, (int)newFace.Y + totalVerts, (int)newFace.Y + totalNorms, (int)newFace.Z + totalVerts, (int)newFace.Z + totalNorms));
                                    else objstream.WriteLine(String.Format("f {0} {1} {2}", (int)newFace.X + totalVerts, (int)newFace.Y + totalVerts, (int)newFace.Z + totalVerts));
                                }

                                if (basicAttach.Mesh[meshIndx].UV != null)
                                {
                                    processedUVStripCount += 3;
                                    objstream.WriteLine(String.Format("# processed UV strips this poly: {0}", processedUVStripCount));
                                }
                            }
                        }
                        else if (basicAttach.Mesh[meshIndx].Poly[polyIndx].PolyType == Basic_PolyType.Quads)
                        {
                            for (int faceVIndx = 0; faceVIndx < basicAttach.Mesh[meshIndx].Poly[polyIndx].Indexes.Length - 4; faceVIndx++)
                            {
                                Vector4 newFace = new Vector4((basicAttach.Mesh[meshIndx].Poly[polyIndx].Indexes[faceVIndx] + 1), (basicAttach.Mesh[meshIndx].Poly[polyIndx].Indexes[faceVIndx + 1] + 1), (basicAttach.Mesh[meshIndx].Poly[polyIndx].Indexes[faceVIndx + 2] + 1), (basicAttach.Mesh[meshIndx].Poly[polyIndx].Indexes[faceVIndx + 3] + 1));

                                if (basicAttach.Mesh[meshIndx].UV != null)
                                {
                                    int uv1, uv2, uv3, uv4;

                                    uv1 = (faceVIndx) + processedUVStripCount + 1; // +1's are because obj indeces always start at 1, not 0
                                    uv2 = faceVIndx + 1 + processedUVStripCount + 1;
                                    uv3 = faceVIndx + 2 + processedUVStripCount + 1;
                                    uv4 = faceVIndx + 3 + processedUVStripCount + 1;

                                    if (wroteNormals) objstream.WriteLine(String.Format("f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8} {9}/{10}/{11}", (int)newFace.X + totalVerts, uv1 + totalUVs, (int)newFace.X + totalNorms, (int)newFace.Y + totalVerts, uv2 + totalUVs, (int)newFace.Y + totalNorms, (int)newFace.Z + totalVerts, uv3 + totalUVs, (int)newFace.Z + totalVerts), (int)newFace.W + totalVerts, uv4 + totalUVs, (int)newFace.W + totalNorms);
                                    else objstream.WriteLine(String.Format("f {0}/{1} {2}/{3} {4}/{5} {6}/{7}", (int)newFace.X + totalVerts, uv1 + totalUVs, (int)newFace.Y + totalVerts, uv2 + totalUVs, (int)newFace.Z + totalVerts, uv3 + totalUVs, (int)newFace.W + totalVerts, uv4 + totalUVs));
                                }
                                else
                                {
                                    if (wroteNormals) objstream.WriteLine(String.Format("f {0}//{1} {2}//{3} {4}//{5} {6}//{7}", (int)newFace.X + totalVerts, (int)newFace.X + totalNorms, (int)newFace.Y + totalVerts, (int)newFace.Y + totalNorms, (int)newFace.Z + totalVerts, (int)newFace.Z + totalNorms), (int)newFace.W + totalVerts, (int)newFace.W + totalNorms);
                                    else objstream.WriteLine(String.Format("f {0} {1} {2} {3}", (int)newFace.X + totalVerts, (int)newFace.Y + totalVerts, (int)newFace.Z + totalVerts), (int)newFace.W + totalVerts);
                                }

                                if (basicAttach.Mesh[meshIndx].UV != null)
                                {
                                    processedUVStripCount += 3;
                                    objstream.WriteLine(String.Format("# processed UV strips this poly: {0}", processedUVStripCount));
                                }
                            }
                        }
                        else if (basicAttach.Mesh[meshIndx].Poly[polyIndx].PolyType == Basic_PolyType.NPoly)
                        {
                            objstream.WriteLine("# Error in WriteObjFromBasicAttach() - NPoly not supported yet!");
                            continue;
                        }
                    }

                    if (basicAttach.Mesh[meshIndx].UV != null) totalUVs += basicAttach.Mesh[meshIndx].UV.Length;
                }
                #endregion

                objstream.WriteLine("");

                // add totals
                totalVerts += basicAttach.Vertex.Length;
                totalNorms += basicAttach.Normal.Length;
            }

            skip_processing:
            // handle child nodes should they exist.
            foreach (Object item in obj.Children)
                WriteModelAsObj(objstream, item, transform, ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);

            transform.Pop();
        }

        /// <summary>
        /// Writes an object model (chunk format) to the specified stream, in Alias-Wavefront *.OBJ format.
        /// </summary>
        /// <param name="objstream">stream representing a wavefront obj file to export to</param>
        /// <param name="obj">Model to export.</param>
        /// <param name="transform">Used for calculating transforms.</param>
        /// <param name="totalVerts">This keeps track of how many verts have been exported to the current file. This is necessary because *.obj vertex indeces are file-level, not object-level.</param>
        /// <param name="totalNorms">This keeps track of how many vert normals have been exported to the current file. This is necessary because *.obj vertex normal indeces are file-level, not object-level.</param>
        /// <param name="totalUVs">This keeps track of how many texture verts have been exported to the current file. This is necessary because *.obj textue vert indeces are file-level, not object-level.</param>
        /// <param name="errorFlag">Set this to TRUE if you encounter an issue. The user will be alerted.</param>
        private static void WriteObjFromChunkAttach(System.IO.StreamWriter objstream, SAModel.Object obj, MatrixStack transform, ref int totalVerts, ref int totalNorms, ref int totalUVs, ref bool errorFlag)
        {
            transform.Push();
            transform.TranslateLocal(obj.Position.ToVector3());
            transform.RotateYawPitchRollLocal(SAModel.Direct3D.Extensions.BAMSToRad(obj.Rotation.Y), SAModel.Direct3D.Extensions.BAMSToRad(obj.Rotation.X), SAModel.Direct3D.Extensions.BAMSToRad(obj.Rotation.Z));
            transform.ScaleLocal(obj.Scale.ToVector3());

            // add obj writing here
            if ((obj.Attach != null))
            {
                ChunkAttach chunkAttach = (ChunkAttach)obj.Attach;
                bool wroteNormals = false;
                int outputVertCount = 0;
                int outputNormalCount = 0;

                objstream.WriteLine("g " + obj.Name);

                #region Outputting Verts and Normals
                int vertexChunkCount = chunkAttach.Vertex.Count;
                int polyChunkCount = chunkAttach.Poly.Count;

                if (vertexChunkCount != 1)
                {
                    errorFlag = true;
                    objstream.WriteLine("#A chunk model with more than one vertex chunk was found. Output is probably corrupt.");
                }

                for (int vc = 0; vc < vertexChunkCount; vc++)
                {
                    for (int vIndx = 0; vIndx < chunkAttach.Vertex[vc].VertexCount; vIndx++)
                    {
                        if(chunkAttach.Vertex[vc].Flags == 0)
                        {
                            Vector3 inputVert = new Vector3(chunkAttach.Vertex[vc].Vertices[vIndx].X, chunkAttach.Vertex[vc].Vertices[vIndx].Y, chunkAttach.Vertex[vc].Vertices[vIndx].Z);
                            Vector3 outputVert = Vector3.TransformCoordinate(inputVert, transform.Top);
                            objstream.WriteLine(String.Format("v {0} {1} {2}", outputVert.X, outputVert.Y, outputVert.Z));

                            outputVertCount++;
                        }
                    }

                    if (chunkAttach.Vertex[vc].Normals.Count > 0)
                    {
                        if(chunkAttach.Vertex[vc].Flags == 0)
                        {
                            for (int vnIndx = 0; vnIndx < chunkAttach.Vertex[vc].Normals.Count; vnIndx++)
                            {
                                objstream.WriteLine(String.Format("vn {0} {1} {2}", chunkAttach.Vertex[vc].Normals[vnIndx].X, chunkAttach.Vertex[vc].Normals[vnIndx].Y, chunkAttach.Vertex[vc].Normals[vnIndx].Z));
                                outputNormalCount++;
                            }
                            wroteNormals = true;
                        }
                    }
                }
                #endregion

                #region Outputting Polys
                for (int pc = 0; pc < polyChunkCount; pc++)
                {
                    PolyChunk polyChunk = (PolyChunk)chunkAttach.Poly[pc];

                    if (polyChunk is PolyChunkStrip)
                    {
                        PolyChunkStrip chunkStrip = (PolyChunkStrip)polyChunk;

                        for (int stripNum = 0; stripNum < chunkStrip.StripCount; stripNum++)
                        {
                            // output texture verts before use, if necessary
                            bool uvsAreValid = false;
                            if (chunkStrip.Strips[stripNum].UVs != null)
                            {
                                if (chunkStrip.Strips[stripNum].UVs.Length > 0)
                                {
                                    uvsAreValid = true;
                                    for (int uvIndx = 0; uvIndx < chunkStrip.Strips[stripNum].UVs.Length; uvIndx++)
                                    {
                                        objstream.WriteLine(String.Format("vt {0} {1}", chunkStrip.Strips[stripNum].UVs[uvIndx].U, chunkStrip.Strips[stripNum].UVs[uvIndx].V));
                                    }
                                }
                            }

                            bool windingReversed = chunkStrip.Strips[stripNum].Reversed;
                            for (int currentStripIndx = 0; currentStripIndx < chunkStrip.Strips[stripNum].Indexes.Length - 2; currentStripIndx++)
                            {
                                if (windingReversed)
                                {
                                    if (uvsAreValid)
                                    {
                                        // note to self - uvs.length will equal strip indeces length! They are directly linked, just like you remembered.
                                        objstream.WriteLine(String.Format("f {0}/{1} {2}/{3} {4}/{5}", (chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 1] + totalVerts) + 1, (currentStripIndx + 1 + totalUVs) + 1, (chunkStrip.Strips[stripNum].Indexes[currentStripIndx] + totalVerts) + 1, (currentStripIndx + totalUVs) + 1, (chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 2] + totalVerts) + 1, (currentStripIndx + 2 + totalUVs) + 1));
                                    }
                                    else
                                    {
                                        objstream.WriteLine(String.Format("f {0} {1} {2}", (chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 1] + totalVerts) + 1, (chunkStrip.Strips[stripNum].Indexes[currentStripIndx] + totalVerts) + 1, (chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 2] + totalVerts) + 1));
                                    }
                                }
                                else
                                {
                                    if (uvsAreValid)
                                    {
                                        // note to self - uvs.length will equal strip indeces length! They are directly linked, just like you remembered.
                                        objstream.WriteLine(String.Format("f {0}/{1} {2}/{3} {4}/{5}", (chunkStrip.Strips[stripNum].Indexes[currentStripIndx] + totalVerts) + 1, currentStripIndx + totalUVs + 1, (chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 1] + totalVerts) + 1, currentStripIndx + 1 + totalUVs + 1, (chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 2] + totalVerts) + 1, currentStripIndx + 2 + totalUVs + 1));
                                    }
                                    else
                                    {
                                        objstream.WriteLine(String.Format("f {0} {1} {2}", (chunkStrip.Strips[stripNum].Indexes[currentStripIndx] + totalVerts) + 1, (chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 1] + totalVerts) + 1, (chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 2] + totalVerts) + 1));
                                    }
                                }

                                windingReversed = !windingReversed;
                            }

                            // increment output verts
                            if(uvsAreValid) totalUVs += chunkStrip.Strips[stripNum].UVs.Length;
                        }
                    }
                    else if (polyChunk is PolyChunkMaterial)
                    {
                        // no behavior defined yet.
                    }
                    else if (polyChunk is PolyChunkTinyTextureID)
                    {
                        PolyChunkTinyTextureID chunkTexID = (PolyChunkTinyTextureID)polyChunk;
                        objstream.WriteLine(String.Format("usemtl material_{0}", chunkTexID.TextureID));
                    }
                }
                #endregion

                totalVerts += outputVertCount;
                totalNorms += outputNormalCount;
            }

            // handle child nodes should they exist.
            foreach (Object item in obj.Children)
                WriteModelAsObj(objstream, item, transform, ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);

            transform.Pop();
        }

        /// <summary>
        /// Primary method for exporting models to Wavefront *.OBJ format. This will auto-detect the model type and send it to the proper export method.
        /// </summary>
        /// <param name="objstream">stream representing a wavefront obj file to export to</param>
        /// <param name="obj">Model to export.</param>
        /// <param name="transform">Used for calculating transforms.</param>
        /// <param name="totalVerts">This keeps track of how many verts have been exported to the current file. This is necessary because *.obj vertex indeces are file-level, not object-level.</param>
        /// <param name="totalNorms">This keeps track of how many vert normals have been exported to the current file. This is necessary because *.obj vertex normal indeces are file-level, not object-level.</param>
        /// <param name="totalUVs">This keeps track of how many texture verts have been exported to the current file. This is necessary because *.obj textue vert indeces are file-level, not object-level.</param>
        /// <param name="errorFlag">Set this to TRUE if you encounter an issue. The user will be alerted.</param>
        public static void WriteModelAsObj(System.IO.StreamWriter objstream, SAModel.Object obj, MatrixStack transform, ref int totalVerts, ref int totalNorms, ref int totalUVs, ref bool errorFlag)
        {
            if (obj.Attach is BasicAttach)
            {
                WriteObjFromBasicAttach(objstream, obj, transform, ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);
            }
            else if (obj.Attach is ChunkAttach)
            {
                WriteObjFromChunkAttach(objstream, obj, transform, ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);
            }
        }

        public static float Distance(this Vector3 vectorA, Vector3 vectorB)
        {
            return Vector3.Length(Vector3.Subtract(vectorA, vectorB));
        }
    }
}