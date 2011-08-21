using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SADXLVL2;

namespace SADXObjectDefinitions.Common
{
    public class FallSpikeBall : ObjectDefinition
    {
        private SonicRetro.SAModel.Object ballmodel;
        private Microsoft.DirectX.Direct3D.Mesh[] ballmeshes;
        private SonicRetro.SAModel.Object cylindermodel;
        private Microsoft.DirectX.Direct3D.Mesh[] cylindermeshes;

        public override void Init(Dictionary<string, string> data, string name, Device dev)
        {
            ballmodel = ObjectHelper.LoadModel("Objects/FallBall/Model.ini");
            ballmeshes = ObjectHelper.GetMeshes(ballmodel, dev);
            cylindermodel = ObjectHelper.LoadModel("Objects/Collision/Cylinder Model.ini");
            cylindermeshes = ObjectHelper.GetMeshes(cylindermodel, dev);
        }

        public override float CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
        {
            float mindist = float.PositiveInfinity;
            transform.Push();
            transform.TranslateLocal(item.Position.ToVector3());
            transform.RotateXYZLocal(0, item.Rotation.Y, 0);
            float dist = ballmodel.CheckHit(Near, Far, Viewport, Projection, View, transform, ballmeshes);
            if (dist > 0 & dist < mindist)
                mindist = dist;
            transform.Pop();
            double v24 = item.Scale.X * 0.05000000074505806;
            transform.Push();
            double v22 = item.Scale.X * 0.5 + item.Position.Y;
            transform.TranslateLocal(item.Position.X, (float)v22, item.Position.Z);
            transform.ScaleLocal(1.0f, (float)v24, 1.0f);
            dist = cylindermodel.CheckHit(Near, Far, Viewport, Projection, View, transform, cylindermeshes);
            if (dist > 0 & dist < mindist)
                mindist = dist;
            transform.Pop();
            if (float.IsPositiveInfinity(mindist)) return -1;
            return mindist;
        }

        public override void Render(SETItem item, Device dev, MatrixStack transform, bool selected)
        {
            transform.Push();
            transform.TranslateLocal(item.Position.ToVector3());
            transform.RotateXYZLocal(0, item.Rotation.Y, 0);
            ballmodel.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_REGULAR"), ballmeshes);
            if (selected)
                ballmodel.DrawModelTreeInvert(dev, transform, ballmeshes);
            transform.Pop();
            double v24 = item.Scale.X * 0.05000000074505806;
            transform.Push();
            double v22 = item.Scale.X * 0.5 + item.Position.Y;
            transform.TranslateLocal(item.Position.X, (float)v22, item.Position.Z);
            transform.ScaleLocal(1.0f, (float)v24, 1.0f);
            cylindermodel.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_REGULAR"), cylindermeshes);
            if (selected)
                cylindermodel.DrawModelTreeInvert(dev, transform, cylindermeshes);
            transform.Pop();
        }

        public override string Name { get { return "Falling Spike Ball"; } }

        public override Type ObjectType
        {
            get
            {
                return typeof(FallSpikeBallSETItem);
            }
        }
    }

    public class FallSpikeBallSETItem : SETItem
    {
        public FallSpikeBallSETItem() : base() { }
        public FallSpikeBallSETItem(byte[] file, int address) : base(file, address) { }

        public float Distance { get { return Scale.X; } set { Scale.X = value; } }
        public float Speed { get { return Scale.Y; } set { Scale.Y = value; } }
    }
}