using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;

using SonicRetro.SAModel.SAEditorCommon.DataTypes;

namespace SonicRetro.SAModel.SAEditorCommon.SETEditing
{
    public abstract class ObjectDefinition
    {
        public abstract void Init(ObjectData data, string name, Device dev);
        public abstract HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform);
        public abstract RenderInfo[] Render(SETItem item, Device dev, MatrixStack transform, bool selected);
        public abstract string Name { get; }

        public virtual Type ObjectType { get { return typeof(SETItem); } }
    }
}
