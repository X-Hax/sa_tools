using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SADXLVL2;

namespace SADXObjectDefinitions.Common
{
    public class SwingSpikeBall : ObjectDefinition
    {
        private SonicRetro.SAModel.Object centermodel;
        private Microsoft.DirectX.Direct3D.Mesh[] centermeshes;
        private SonicRetro.SAModel.Object cylindermodel;
        private Microsoft.DirectX.Direct3D.Mesh[] cylindermeshes;
        private SonicRetro.SAModel.Object ballmodel;
        private Microsoft.DirectX.Direct3D.Mesh[] ballmeshes;

        public override void Init(ObjectData data, string name, Device dev)
        {
            centermodel = ObjectHelper.LoadModel("Objects/SwingBall/Center Model.sa1mdl");
            centermeshes = ObjectHelper.GetMeshes(centermodel, dev);
            cylindermodel = ObjectHelper.LoadModel("Objects/Collision/Cylinder Model.sa1mdl");
            cylindermeshes = ObjectHelper.GetMeshes(cylindermodel, dev);
            ballmodel = ObjectHelper.LoadModel("Objects/FallBall/Model.sa1mdl");
            ballmeshes = ObjectHelper.GetMeshes(ballmodel, dev);
        }

        public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
        {
            HitResult result = HitResult.NoHit;
            transform.Push();
            transform.TranslateLocal(item.Position.ToVector3());
            transform.RotateXYZLocal(item.Rotation.X, item.Rotation.Y, item.Rotation.Z);
            result = HitResult.Min(result, centermodel.CheckHit(Near, Far, Viewport, Projection, View, transform, centermeshes));
            transform.Pop();
            /*double v14 = (item.Scale.X + 6.0) * 0.4000000059604645 + 0.6000000238418579;
            transform.Push();
            double v8 = item.Scale.Y * 0.5;
            transform.TranslateLocal(item.Position.X, (float)v8, item.Position.Z);
            double v9 = item.Scale.Y * 0.05000000074505806;
            transform.ScaleLocal((float)v14, (float)v9, (float)v14);
            result = HitResult.Min(result, cylindermodel.CheckHit(Near, Far, Viewport, Projection, View, transform, cylindermeshes));
            transform.Pop();
            double v15 = (item.Scale.X + 6.0) * 0.4000000059604645 + 0.6000000238418579;
            transform.Push();
            double v13 = item.Scale.Y * 0.5;
            transform.TranslateLocal(item.Position.X, (float)v13, item.Position.Z);
            transform.ScaleLocal((float)v15, 0.1000000014901161f, (float)v15);
            result = HitResult.Min(result, cylindermodel.CheckHit(Near, Far, Viewport, Projection, View, transform, cylindermeshes));
            transform.Pop();*/
            return result;
        }

        public override RenderInfo[] Render(SETItem item, Device dev, MatrixStack transform, bool selected)
        {
            List<RenderInfo> result = new List<RenderInfo>();
            transform.Push();
            transform.TranslateLocal(item.Position.ToVector3());
            transform.RotateXYZLocal(item.Rotation.X, item.Rotation.Y, item.Rotation.Z);
            result.AddRange(centermodel.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_REGULAR"), centermeshes));
            if (selected)
                result.AddRange(centermodel.DrawModelTreeInvert(dev, transform, centermeshes));
            transform.Pop();
            /*double v14 = (item.Scale.X + 6.0) * 0.4000000059604645 + 0.6000000238418579;
            transform.Push();
            double v8 = item.Scale.Y * 0.5;
            transform.TranslateLocal(item.Position.X, (float)v8, item.Position.Z);
            double v9 = item.Scale.Y * 0.05000000074505806;
            transform.ScaleLocal((float)v14, (float)v9, (float)v14);
            result.AddRange(cylindermodel.DrawModelTree(dev, transform, null, cylindermeshes));
            if (selected)
                result.AddRange(cylindermodel.DrawModelTreeInvert(dev, transform, cylindermeshes));
            transform.Pop();
            double v15 = (item.Scale.X + 6.0) * 0.4000000059604645 + 0.6000000238418579;
            transform.Push();
            double v13 = item.Scale.Y * 0.5;
            transform.TranslateLocal(item.Position.X, (float)v13, item.Position.Z);
            transform.ScaleLocal((float)v15, 0.1000000014901161f, (float)v15);
            result.AddRange(cylindermodel.DrawModelTree(dev, transform, null, cylindermeshes));
            if (selected)
                result.AddRange(cylindermodel.DrawModelTreeInvert(dev, transform, cylindermeshes));
            transform.Pop();*/
            return result.ToArray();
        }

        public override string Name { get { return "Swinging Spike Ball"; } }

        public override Type ObjectType
        {
            get
            {
                return typeof(SwingSpikeBallSETItem);
            }
        }
    }

    public class SwingSpikeBallSETItem : SETItem
    {
        public SwingSpikeBallSETItem() : base() { }
        public SwingSpikeBallSETItem(byte[] file, int address) : base(file, address)
        {
            yspeed = (long)(Scale.Z / 1000);
             uint v4 = (uint)Math.Abs(Scale.Z % 1000);
             if (v4 >= 100)
                 Chain = false;
             uint v5 = v4 % 100;
             if (v5 >= 10)
             {
                 if (v5 < 90)
                     shadow = ShadowType.Off;
                 else
                     shadow = ShadowType.Heavy;
             }
             if (v5 % 10 >= 1)
                 oneball = true;
        }

        public float ChainLength { get { return Scale.X; } set { Scale.X = value; } }

        public float YDistance { get { return Scale.Y; } set { Scale.Y = value; } }

        private bool oneball = false;
        public bool OneBall { get { return oneball; } set { oneball = value; UpdateZScl(); } }

        private bool chain = true;
        public bool Chain { get { return chain; } set { chain = value; UpdateZScl(); } }

        private ShadowType shadow = ShadowType.Light;
        public ShadowType Shadow { get { return shadow; } set { shadow = value; UpdateZScl(); } }

        private long yspeed = 0;
        public long YSpeed { get { return yspeed; } set { yspeed = value; UpdateZScl(); } }

        private void UpdateZScl()
        {
            float value = (float)yspeed * 1000;
            if (!chain)
                value += 100;
            if (shadow == ShadowType.Off)
                value += 10;
            else if (shadow == ShadowType.Heavy)
                value += 90;
            if (oneball)
                value += 1;
            Scale.Z = value;
        }
    }

    public enum ShadowType
    {
        Off,
        Heavy,
        Light
    }
}