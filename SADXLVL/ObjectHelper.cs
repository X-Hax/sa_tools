using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;

namespace SonicRetro.SAModel.SADXLVL2
{
    public static class ObjectHelper
    {
        internal static CustomVertex.PositionTextured[] Square = {
        new CustomVertex.PositionTextured(-8, 8, 0, 1, 0),
        new CustomVertex.PositionTextured(-8, -8, 0, 1, 1),
        new CustomVertex.PositionTextured(8, 8, 0, 0, 0),
        new CustomVertex.PositionTextured(-8, -8, 0, 1, 1),
        new CustomVertex.PositionTextured(8, -8, 0, 0, 1),
        new CustomVertex.PositionTextured(8, 8, 0, 0, 0)};

        internal static Texture QuestionMark;

        public static Object LoadModel(string file)
        {
            Dictionary<string, Dictionary<string, string>> mdlini = IniFile.Load(file);
            return new Object(mdlini, mdlini[string.Empty]["Root"]);
        }

        public static Microsoft.DirectX.Direct3D.Mesh[] GetMeshes(Object model, Device dev)
        {
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

        public static float CheckSpriteHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
        {
            Vector3 pos = Vector3.Unproject(Near, Viewport, Projection, View, transform.Top);
            Vector3 dir = Vector3.Subtract(pos, Vector3.Unproject(Far, Viewport, Projection, View, transform.Top));
            float dist = -1;
            for (int i = 0; i < 2; i++)
            {
                IntersectInformation info;
                if (Geometry.IntersectTri(Square[i * 3].Position, Square[i * 3 + 1].Position, Square[i * 3 + 2].Position, pos, dir, out info))
                {
                    if (dist == -1)
                        dist = info.Dist;
                    else if (dist > info.Dist)
                        dist = info.Dist;
                }
            }
            return dist;
        }

        public static void RenderSprite(Device dev, MatrixStack transform, Texture texture, bool selected)
        {
            VertexFormats fmt = dev.VertexFormat;
            dev.VertexFormat = CustomVertex.PositionTextured.Format;
            dev.Material = new Microsoft.DirectX.Direct3D.Material
            {
                Diffuse = Color.White,
                Ambient = Color.White
            };
            dev.TextureState[0].TextureCoordinateIndex = 0;
            TextureFilter magfilter = dev.SamplerState[0].MagFilter;
            TextureFilter minfilter = dev.SamplerState[0].MinFilter;
            TextureFilter mipfilter = dev.SamplerState[0].MipFilter;
            dev.SamplerState[0].MagFilter = TextureFilter.None;
            dev.SamplerState[0].MinFilter = TextureFilter.None;
            dev.SamplerState[0].MipFilter = TextureFilter.None;
            dev.Transform.World = transform.Top;
            if (texture == null)
                dev.SetTexture(0, QuestionMark);
            else
                dev.SetTexture(0, texture);
            dev.DrawUserPrimitives(PrimitiveType.TriangleList, 2, Square);
            if (selected)
            {
                dev.Material = new Microsoft.DirectX.Direct3D.Material
                {
                    Diffuse = Color.Yellow,
                    Ambient = Color.Yellow
                };
                FillMode mode = dev.RenderState.FillMode;
                dev.RenderState.FillMode = FillMode.WireFrame;
                dev.DrawUserPrimitives(PrimitiveType.TriangleList, 2, Square);
                dev.RenderState.FillMode = mode;
            }
            dev.SamplerState[0].MagFilter = magfilter;
            dev.SamplerState[0].MinFilter = minfilter;
            dev.SamplerState[0].MipFilter = mipfilter;
            dev.VertexFormat = fmt;
        }

        public static float BAMSToRad(int BAMS)
        {
            return (float)(BAMS / (65536 / (2 * Math.PI)));
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
    }
}