using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Drawing;


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
            matrix *= Matrix.RotationYawPitchRoll(BAMSToRad(col.Model.Rotation.Y), BAMSToRad(col.Model.Rotation.X), BAMSToRad(col.Model.Rotation.Z));
            matrix *= Matrix.Translation(col.Model.Position.ToVector3());
            List<Vector3> verts = new List<Vector3>();
            foreach (SAModel.Mesh mesh in col.Model.Attach.Mesh)
                foreach (Poly poly in mesh.Poly)
                    foreach (ushort index in poly.Indexes)
                        verts.Add(Vector3.TransformCoordinate(col.Model.Attach.Vertex[index].ToVector3(), matrix));
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
            if (numverts == 0) return null;
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
            if (mesh == null) return;
            transform.Push();
            transform.TranslateLocal(obj.Position.X, obj.Position.Y, obj.Position.Z);
            transform.RotateYawPitchRollLocal(BAMSToRad(obj.Rotation.Y), BAMSToRad(obj.Rotation.X), BAMSToRad(obj.Rotation.Z));
            transform.ScaleLocal(obj.Scale.X, obj.Scale.Y, obj.Scale.Z);
            if (obj.Attach != null)
            {
                device.SetTransform(TransformType.World, transform.Top);
                for (int j = 0; j < obj.Attach.Mesh.Count; j++)
                {
                    if ((obj.Flags & ObjectFlags.NoDisplay) == 0)
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
                    }
                    else
                    {
                        device.Material = new Microsoft.DirectX.Direct3D.Material
                        {
                            Diffuse = Color.White,
                            Ambient = Color.White,
                        };
                        device.SetTexture(0, null);
                        device.SetRenderState(RenderStates.AlphaBlendEnable, false);
                        device.SetTextureStageState(0, TextureStageStates.TextureCoordinateIndex, 0);
                    }
                    mesh.DrawSubset(j);
                }
            }
            transform.Pop();
        }

        public static void DrawModelInvert(this Object obj, Device device, MatrixStack transform, Microsoft.DirectX.Direct3D.Mesh mesh)
        {
            if (mesh == null) return;
            FillMode mode = device.RenderState.FillMode;
            device.RenderState.FillMode = FillMode.WireFrame;
            transform.Push();
            transform.TranslateLocal(obj.Position.X, obj.Position.Y, obj.Position.Z);
            transform.RotateYawPitchRollLocal(BAMSToRad(obj.Rotation.Y), BAMSToRad(obj.Rotation.X), BAMSToRad(obj.Rotation.Z));
            transform.ScaleLocal(obj.Scale.X, obj.Scale.Y, obj.Scale.Z);
            if (obj.Attach != null)
            {
                device.SetTransform(TransformType.World, transform.Top);
                for (int j = 0; j < obj.Attach.Mesh.Count; j++)
                {
                    System.Drawing.Color col = obj.Attach.Material[obj.Attach.Mesh[j].MaterialID].DiffuseColor;
                    if ((obj.Flags & ObjectFlags.NoDisplay) == ObjectFlags.NoDisplay) col = Color.White;
                    col = System.Drawing.Color.FromArgb(255 - col.R, 255 - col.G, 255 - col.B);
                    device.Material = new Microsoft.DirectX.Direct3D.Material
                    {
                        Diffuse = col,
                        Ambient = col
                    };
                    device.SetTexture(0, null);
                    device.SetRenderState(RenderStates.AlphaBlendEnable, false);
                    mesh.DrawSubset(j);
                }
            }
            transform.Pop();
            device.RenderState.FillMode = mode;
        }

        public static void DrawModelTree(this Object obj, Device device, MatrixStack transform, Texture[] textures, Microsoft.DirectX.Direct3D.Mesh[] meshes)
        {
            int modelindex = -1;
            obj.DrawModelTree(device, transform, textures, meshes, ref modelindex);
        }

        public static void DrawModelTree(this Object obj, Device device, MatrixStack transform, Texture[] textures, Microsoft.DirectX.Direct3D.Mesh[] meshes, ref int modelindex)
        {
                transform.Push();
                modelindex++;
                transform.TranslateLocal(obj.Position.X, obj.Position.Y, obj.Position.Z);
                transform.RotateYawPitchRollLocal(BAMSToRad(obj.Rotation.Y), BAMSToRad(obj.Rotation.X), BAMSToRad(obj.Rotation.Z));
                transform.ScaleLocal(obj.Scale.X, obj.Scale.Y, obj.Scale.Z);
                if (obj.Attach != null & meshes[modelindex] != null)
                {
                    device.SetTransform(TransformType.World, transform.Top);
                    for (int j = 0; j < obj.Attach.Mesh.Count; j++)
                    {
                        if ((obj.Flags & ObjectFlags.NoDisplay) == 0)
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
                        }
                        else
                        {
                            device.Material = new Microsoft.DirectX.Direct3D.Material
                            {
                                Diffuse = Color.White,
                                Ambient = Color.White,
                            };
                            device.SetTexture(0, null);
                            device.SetRenderState(RenderStates.AlphaBlendEnable, false);
                            device.SetTextureStageState(0, TextureStageStates.TextureCoordinateIndex, 0);
                        }
                        meshes[modelindex].DrawSubset(j);
                    }
                }
                foreach (Object child in obj.Children)
                    DrawModelTree(child, device, transform, textures, meshes, ref modelindex);
                transform.Pop();
        }

        public static void DrawModelTreeInvert(this Object obj, Device device, MatrixStack transform, Microsoft.DirectX.Direct3D.Mesh[] meshes)
        {
            int modelindex = -1;
            obj.DrawModelTreeInvert(device, transform, meshes, ref modelindex);
        }

        public static void DrawModelTreeInvert(this Object obj, Device device, MatrixStack transform, Microsoft.DirectX.Direct3D.Mesh[] meshes, ref int modelindex)
        {
            FillMode mode = device.RenderState.FillMode;
            device.RenderState.FillMode = FillMode.WireFrame;
            transform.Push();
            modelindex++;
            transform.TranslateLocal(obj.Position.X, obj.Position.Y, obj.Position.Z);
            transform.RotateYawPitchRollLocal(BAMSToRad(obj.Rotation.Y), BAMSToRad(obj.Rotation.X), BAMSToRad(obj.Rotation.Z));
            transform.ScaleLocal(obj.Scale.X, obj.Scale.Y, obj.Scale.Z);
            if (obj.Attach != null & meshes[modelindex] != null)
            {
                device.SetTransform(TransformType.World, transform.Top);
                for (int j = 0; j < obj.Attach.Mesh.Count; j++)
                {
                    System.Drawing.Color col = obj.Attach.Material[obj.Attach.Mesh[j].MaterialID].DiffuseColor;
                    if ((obj.Flags & ObjectFlags.NoDisplay) == ObjectFlags.NoDisplay) col = Color.White;
                    col = System.Drawing.Color.FromArgb(255 - col.R, 255 - col.G, 255 - col.B);
                    device.Material = new Microsoft.DirectX.Direct3D.Material
                    {
                        Diffuse = col,
                        Ambient = col
                    };
                    device.SetTexture(0, null);
                    device.SetRenderState(RenderStates.AlphaBlendEnable, false);
                    meshes[modelindex].DrawSubset(j);
                }
            }
            foreach (Object child in obj.Children)
                DrawModelTreeInvert(child, device, transform, meshes, ref modelindex);
            transform.Pop();
            device.RenderState.FillMode = mode;
        }

        public static float CheckHit(this Object obj, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, Microsoft.DirectX.Direct3D.Mesh mesh)
        {
            if (mesh == null) return -1;
            Matrix transform = Matrix.Identity;
            transform.Multiply(Matrix.RotationYawPitchRoll(BAMSToRad(obj.Rotation.Y), BAMSToRad(obj.Rotation.X), BAMSToRad(obj.Rotation.Z)));
            transform.Multiply(Matrix.Translation(obj.Position.ToVector3()));
            Vector3 pos = Vector3.Unproject(Near, Viewport, Projection, View, transform);
            Vector3 dir = Vector3.Subtract(pos, Vector3.Unproject(Far, Viewport, Projection, View, transform));
            IntersectInformation info;
            if (!mesh.Intersect(pos, dir, out info)) return -1;
            return info.Dist;
        }

        public static float CheckHit(this Object obj, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform, Microsoft.DirectX.Direct3D.Mesh[] mesh)
        {
            int modelindex = -1;
            return CheckHit(obj, Near, Far, Viewport, Projection, View, transform, mesh, ref modelindex);
        }

        public static float CheckHit(this Object obj, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform, Microsoft.DirectX.Direct3D.Mesh[] mesh, ref int modelindex)
        {
            transform.Push();
            modelindex++;
            transform.TranslateLocal(obj.Position.X, obj.Position.Y, obj.Position.Z);
            transform.RotateYawPitchRollLocal(BAMSToRad(obj.Rotation.Y), BAMSToRad(obj.Rotation.X), BAMSToRad(obj.Rotation.Z));
            transform.ScaleLocal(obj.Scale.X, obj.Scale.Y, obj.Scale.Z);
            float dist = -1;
            if (obj.Attach != null & mesh[modelindex] != null)
            {
                Vector3 pos = Vector3.Unproject(Near, Viewport, Projection, View, transform.Top);
                Vector3 dir = Vector3.Subtract(pos, Vector3.Unproject(Far, Viewport, Projection, View, transform.Top));
                IntersectInformation info;
                if (mesh[modelindex].Intersect(pos, dir, out info))
                {
                    if (dist == -1)
                        dist = info.Dist;
                    else if (dist > info.Dist)
                        dist = info.Dist;
                }
            }
            foreach (Object child in obj.Children)
            {
                float r = CheckHit(child, Near, Far, Viewport, Projection, View, transform, mesh, ref modelindex);
                if (dist == -1)
                    dist = r;
                else if (dist > r & r != -1)
                    dist = r;
            }
            return dist;
        }

        public static Attach obj2nj(string objfile)
        {
            string[] obj = System.IO.File.ReadAllLines(objfile);
            Attach model;
            List<UV> uvs = new List<UV>();
            List<Color> vcolors = new List<Color>();
            List<Vertex> verts = new List<Vertex>();
            List<Vertex> norms = new List<Vertex>();
            List<Material> mtls = new List<Material>();
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
                            switch (mlin[0].ToLowerInvariant())
                            {
                                case "newmtl":
                                    mtls.Add(new Material { Name = mlin[1] });
                                    break;
                                case "kd":
                                    mtls[mtls.Count - 1].DiffuseColor = Color.FromArgb((int)Math.Round(float.Parse(mlin[1], System.Globalization.CultureInfo.InvariantCulture) * 255), (int)Math.Round(float.Parse(mlin[2], System.Globalization.CultureInfo.InvariantCulture) * 255), (int)Math.Round(float.Parse(mlin[3], System.Globalization.CultureInfo.InvariantCulture) * 255));
                                    break;
                                case "d":
                                case "tr":
                                    mtls[mtls.Count - 1].DiffuseColor = Color.FromArgb((int)Math.Round(float.Parse(mlin[1], System.Globalization.CultureInfo.InvariantCulture) * 255), mtls[mtls.Count - 1].DiffuseColor);
                                    break;
                                case "ks":
                                    mtls[mtls.Count - 1].SpecularColor = Color.FromArgb((int)Math.Round(float.Parse(mlin[1], System.Globalization.CultureInfo.InvariantCulture) * 255), (int)Math.Round(float.Parse(mlin[2], System.Globalization.CultureInfo.InvariantCulture) * 255), (int)Math.Round(float.Parse(mlin[3], System.Globalization.CultureInfo.InvariantCulture) * 255));
                                    break;
                                case "texid":
                                    mtls[mtls.Count - 1].TextureID = int.Parse(mlin[1], System.Globalization.CultureInfo.InvariantCulture);
                                    break;
                            }
                        }

                        break;
                    case "v":
                        verts.Add(new Vertex(float.Parse(lin[1], System.Globalization.CultureInfo.InvariantCulture), float.Parse(lin[2], System.Globalization.CultureInfo.InvariantCulture), float.Parse(lin[3], System.Globalization.CultureInfo.InvariantCulture)));
                        break;
                    case "vn":
                        norms.Add(new Vertex(float.Parse(lin[1], System.Globalization.CultureInfo.InvariantCulture), float.Parse(lin[2], System.Globalization.CultureInfo.InvariantCulture), float.Parse(lin[3], System.Globalization.CultureInfo.InvariantCulture)));
                        break;
                    case "vt":
                        uvs.Add(new UV() { U = unchecked((short)(float.Parse(lin[1], System.Globalization.CultureInfo.InvariantCulture) * 255)), V = unchecked((short)(float.Parse(lin[2], System.Globalization.CultureInfo.InvariantCulture) * 255)) });
                        break;
                    case "vc":
                        vcolors.Add(Color.FromArgb((int)Math.Round(float.Parse(lin[1], System.Globalization.CultureInfo.InvariantCulture)), (int)Math.Round(float.Parse(lin[2], System.Globalization.CultureInfo.InvariantCulture)), (int)Math.Round(float.Parse(lin[3], System.Globalization.CultureInfo.InvariantCulture)), (int)Math.Round(float.Parse(lin[4], System.Globalization.CultureInfo.InvariantCulture))));
                        break;
                    case "usemtl":
                        model_Mesh_Poly.Add(new List<Poly>());
                        model_Mesh_UV.Add(new List<UV>());
                        model_Mesh_VColor.Add(new List<Color>());
                        bool found = false;
                        for (int i = 0; i <= model_Material.Count - 1; i++)
                        {
                            if (model_Material[i].Name == lin[1])
                            {
                                model_Mesh_MaterialID.Add((ushort)i);
                                found = true;
                                break;
                            }
                        }
                        if (found) break;
                        for (int i = 0; i <= mtls.Count - 1; i++)
                        {
                            if (mtls[i].Name == lin[1])
                            {
                                model_Mesh_MaterialID.Add((ushort)model_Material.Count);
                                model_Material.Add(mtls[i]);
                                found = true;
                                break;
                            }
                        }
                        if (found) break;
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
                        Poly tri = Poly.CreatePoly(PolyType.Triangles);
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
            model = new Attach(model_Vertex.ToArray(), model_Normal.ToArray(), model_Mesh, model_Material) { Name = System.IO.Path.GetFileNameWithoutExtension(objfile) };
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
    }
}