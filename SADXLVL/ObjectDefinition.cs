using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;

namespace SonicRetro.SAModel.SADXLVL2
{
    public abstract class ObjectDefinition
    {
        public abstract void Init(Dictionary<string, string> data, string name, Device dev);
        public abstract float CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform);
        public abstract void Render(SETItem item, Device dev, MatrixStack transform, bool selected);
        public abstract string Name { get; }

        public virtual Type ObjectType { get { return typeof(SETItem); } }
    }

    internal class DefaultObjectDefinition : ObjectDefinition
    {
        private string name = "Unknown";
        private Object model;
        private Microsoft.DirectX.Direct3D.Mesh[] meshes;
        private string texture;

        public override void Init(Dictionary<string, string> data, string name, Device dev)
        {
            this.name = name;
            if (data.ContainsKey("Name"))
                this.name = data["Name"];
            if (data.ContainsKey("Model"))
            {
                model = ObjectHelper.LoadModel(data["Model"]);
                meshes = ObjectHelper.GetMeshes(model, dev);
            }
            if (data.ContainsKey("Texture"))
                texture = data["Texture"];
        }

        public override float CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
        {
            transform.Push();
            transform.TranslateLocal(item.Position.ToVector3());
            transform.RotateXYZLocal(item.Rotation.X, item.Rotation.Y, item.Rotation.Z);
            float dist = -1;
            if (model == null)
                dist = ObjectHelper.CheckSpriteHit(Near, Far, Viewport, Projection, View, transform);
            else
                dist = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
            transform.Pop();
            return dist;
        }

        public override void Render(SETItem item, Device dev, MatrixStack transform, bool selected)
        {
            transform.Push();
            transform.TranslateLocal(item.Position.ToVector3());
            transform.RotateXYZLocal(item.Rotation.X, item.Rotation.Y, item.Rotation.Z);
            if (model == null)
                ObjectHelper.RenderSprite(dev, transform, null, selected);
            else
            {
                model.DrawModelTree(dev, transform, ObjectHelper.GetTextures(texture), meshes);
                if (selected)
                    model.DrawModelTreeInvert(dev, transform, meshes);
            }
            transform.Pop();
        }

        public override string Name { get { return name; } }
    }
}
