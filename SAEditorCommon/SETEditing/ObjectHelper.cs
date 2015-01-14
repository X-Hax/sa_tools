using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;

using SonicRetro.SAModel.SAEditorCommon.DataTypes;

namespace SonicRetro.SAModel.SAEditorCommon.SETEditing
{
    public static class ObjectHelper
    {
        internal static CustomVertex.PositionTextured[] SquareVerts = {
        new CustomVertex.PositionTextured(-8, 8, 0, 1, 0),
        new CustomVertex.PositionTextured(-8, -8, 0, 1, 1),
        new CustomVertex.PositionTextured(8, 8, 0, 0, 0),
        new CustomVertex.PositionTextured(-8, -8, 0, 1, 1),
        new CustomVertex.PositionTextured(8, -8, 0, 0, 1),
        new CustomVertex.PositionTextured(8, 8, 0, 0, 0)};
        internal static Microsoft.DirectX.Direct3D.Mesh SquareMesh;

        public static void Init(Device device, Bitmap unknownBitmap)
        {
            SquareMesh = new Microsoft.DirectX.Direct3D.Mesh(2, 6, MeshFlags.Managed, CustomVertex.PositionTextured.Format, device);
            List<short> ib = new List<short>();
            for (int i = 0; i < SquareVerts.Length; i++)
                ib.Add((short)(i));
            SquareMesh.SetVertexBufferData(SquareVerts, LockFlags.None);
            SquareMesh.SetIndexBufferData(ib.ToArray(), LockFlags.None);

            if (unknownBitmap != null)
            {
                QuestionMark = new Texture(device, unknownBitmap, Usage.None, Pool.Managed);
            }
            else
            {
				QuestionMark = new Texture(device, 16, 16, 0, Usage.None, Format.A16B16G16R16, Pool.Managed);
            }
        }

        internal static Texture QuestionMark;

        public static Object LoadModel(string file)
        {
            return new ModelFile(file).Model;
        }

        public static Microsoft.DirectX.Direct3D.Mesh[] GetMeshes(Object model, Device dev)
        {
            model.ProcessVertexData();
            Object[] models = model.GetObjects();
            Microsoft.DirectX.Direct3D.Mesh[] Meshes = new Microsoft.DirectX.Direct3D.Mesh[models.Length];
            for (int i = 0; i < models.Length; i++)
                if (models[i].Attach != null)
                    Meshes[i] = models[i].Attach.CreateD3DMesh(dev);
            return Meshes;
        }

        public static Texture[] GetTextures(string name)
        {
            if (LevelData.Textures.ContainsKey(name))
                return LevelData.Textures[name];
            return null;
        }

        public static HitResult CheckSpriteHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
        {
            Vector3 pos = Vector3.Unproject(Near, Viewport, Projection, View, transform.Top);
            Vector3 dir = Vector3.Subtract(pos, Vector3.Unproject(Far, Viewport, Projection, View, transform.Top));
            IntersectInformation info;
            if (!SquareMesh.Intersect(pos, dir, out info)) return HitResult.NoHit;
            return new HitResult(null, info.Dist);
        }

        public static RenderInfo[] RenderSprite(Device dev, MatrixStack transform, Texture texture, Vector3 center, bool selected)
        {
            List<RenderInfo> result = new List<RenderInfo>();
            Material mat = new Material
            {
                DiffuseColor = Color.White
            };
            if (texture == null)
                texture = QuestionMark;
            result.Add(new RenderInfo(SquareMesh, 0, transform.Top, mat, texture, dev.RenderState.FillMode, new BoundingSphere(center.X, center.Y, center.Z, 8)));
            if (selected)
            {
                mat = new Material
                {
                    DiffuseColor = Color.Yellow,
                    UseAlpha = false
                };
                result.Add(new RenderInfo(SquareMesh, 0, transform.Top, mat, null, FillMode.WireFrame, new BoundingSphere(center.X, center.Y, center.Z, 8)));
            }
            return result.ToArray();
        }

        public static float BAMSToRad(int BAMS)
        {
            return SonicRetro.SAModel.Direct3D.Extensions.BAMSToRad(BAMS);
        }

        public static int RadToBAMS(float rad)
        {
            return (int)(rad * (65536 / (2 * Math.PI)));
        }

        public static float BAMSToDeg(int BAMS)
        {
            return (float)(BAMS / (65536 / 360.0));
        }

        public static int DegToBAMS(float deg)
        {
            return (int)(deg * (65536 / 360.0));
        }

        public static float ConvertBAMS(int BAMS)
        {
            return SonicRetro.SAModel.Direct3D.Extensions.BAMSToFloat(BAMS);
        }

        public static float ConvertBAMSInv(int BAMS)
        {
			return SonicRetro.SAModel.Direct3D.Extensions.BAMSToFloatInv(BAMS);
        }
    }
}
