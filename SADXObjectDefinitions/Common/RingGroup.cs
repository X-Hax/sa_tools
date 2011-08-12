using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SADXLVL2;

namespace SADXObjectDefinitions.Common
{
    public class RingGroup : ObjectDefinition
    {
        private SonicRetro.SAModel.Object model;
        private Microsoft.DirectX.Direct3D.Mesh[] meshes;

        public override void Init(Dictionary<string, string> data, string name, Device dev)
        {
            model = ObjectHelper.LoadModel("Objects/Ring/Model.ini");
            meshes = ObjectHelper.GetMeshes(model, dev);
        }

        public override float CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
        {
            float mindist = -1;
            for (int i = 0; i < Math.Min(item.Scale.X + 1, 8); i++)
            {
                transform.Push();
                if (item.Scale.Z == 1) // circle
                {
                    transform.Push();
                    transform.TranslateLocal(item.Position.ToVector3());
                    transform.RotateXYZLocal(item.Rotation.X, item.Rotation.Y, item.Rotation.Z);
                    float cir = (float)(i * 360 / item.Scale.X * 65536.0 * 0.002777777777777778 / 63356.0);
                    Vector3 pos = Vector3.TransformCoordinate(new Vector3((float)(cir * item.Scale.Y), 0, (float)((1 - cir) * item.Scale.Y)), transform.Top);
                    transform.Pop();
                    transform.TranslateLocal(pos);
                    float dist = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
                    if (dist > 0 & dist < mindist)
                        mindist = dist;
                }
                else // line
                {
                    transform.Push();
                    transform.TranslateLocal(item.Position.ToVector3());
                    transform.RotateXYZLocal(item.Rotation.X, item.Rotation.Y, item.Rotation.Z);
                    double v5;
                    if (i % 2 == 1)
                        v5 = i * item.Scale.Y * -0.5;
                    else
                        v5 = Math.Ceiling(i * 0.5) * item.Scale.Y;
                    Vector3 pos = Vector3.TransformCoordinate(new Vector3(0, 0, (float)v5), transform.Top);
                    transform.Pop();
                    transform.TranslateLocal(pos);
                    float dist = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
                    if (dist > 0 & dist < mindist)
                        mindist = dist;
                }
                transform.Pop();
            }
            return mindist;
        }

        public override void Render(SETItem item, Device dev, MatrixStack transform, bool selected)
        {
            for (int i = 0; i < Math.Min(item.Scale.X + 1, 8); i++)
            {
                transform.Push();
                if (item.Scale.Z == 1) // circle
                {
                    transform.Push();
                    transform.TranslateLocal(item.Position.ToVector3());
                    transform.RotateXYZLocal(item.Rotation.X, item.Rotation.Y, item.Rotation.Z);
                    float dist = (float)(i * 360 / item.Scale.X * 65536.0 * 0.002777777777777778 / 63356.0);
                    Vector3 pos = Vector3.TransformCoordinate(new Vector3((float)(dist * item.Scale.Y), 0, (float)((1 - dist) * item.Scale.Y)), transform.Top);
                    transform.Pop();
                    transform.TranslateLocal(pos);
                    model.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_REGULAR"), meshes);
                    if (selected)
                        model.DrawModelTreeInvert(dev, transform, meshes);
                }
                else // line
                {
                    transform.Push();
                    transform.TranslateLocal(item.Position.ToVector3());
                    transform.RotateXYZLocal(item.Rotation.X, item.Rotation.Y, item.Rotation.Z);
                    double v5;
                    if (i % 2 == 1)
                        v5 = i * item.Scale.Y * -0.5;
                    else
                        v5 = Math.Ceiling(i * 0.5) * item.Scale.Y;
                    Vector3 pos = Vector3.TransformCoordinate(new Vector3(0, 0, (float)v5), transform.Top);
                    transform.Pop();
                    transform.TranslateLocal(pos);
                    model.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_REGULAR"), meshes);
                    if (selected)
                        model.DrawModelTreeInvert(dev, transform, meshes);
                }
                transform.Pop();
            }
        }

        public override string Name { get { return "Ring Group"; } }
    }
}