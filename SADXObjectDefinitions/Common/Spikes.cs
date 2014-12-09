using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SADXLVL2;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;

namespace SADXObjectDefinitions.Common
{
    public class Spikes : ObjectDefinition
    {
        private SonicRetro.SAModel.Object model;
        private Microsoft.DirectX.Direct3D.Mesh[] meshes;

        public override void Init(ObjectData data, string name, Device dev)
        {
            model = ObjectHelper.LoadModel("Objects/Spikes.sa1mdl");
            meshes = ObjectHelper.GetMeshes(model, dev);
        }

        public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
        {
            HitResult result = HitResult.NoHit;
            int rows = (int)Math.Max(item.Scale.X, 1);
            int cols = (int)Math.Max(item.Scale.Z, 1);
            transform.Push();
            transform.NJTranslate(item.Position);
            transform.NJRotateObject(item.Rotation.X & 0xC000, item.Rotation.Y, 0);
            double v4 = (1 - cols) * 7.5;
            double v5 = (1 - rows) * 7.5;
            transform.NJTranslate((1 - rows) * 7.5f, 0, (1 - cols) * 7.5f);
            for (int i = 0; i < rows; ++i)
            {
                transform.Push();
                for (int j = 0; j < cols; ++j)
                {
                    result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
                    transform.NJTranslate(0, 0, 15);
                }
                transform.Pop();
                transform.NJTranslate(15, 0, 0);
            }
            transform.Pop();
            return result;
        }

		public override RenderInfo[] Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform, bool selected)
        {
            List<RenderInfo> result = new List<RenderInfo>();
            int rows = (int)Math.Max(item.Scale.X, 1);
            int cols = (int)Math.Max(item.Scale.Z, 1);
            transform.Push();
            transform.NJTranslate(item.Position);
            transform.NJRotateObject(item.Rotation.X & 0xC000, item.Rotation.Y, 0);
            double v4 = (1 - cols) * 7.5;
            double v5 = (1 - rows) * 7.5;
            transform.NJTranslate((1 - rows) * 7.5f, 0, (1 - cols) * 7.5f);
            for (int i = 0; i < rows; ++i)
            {
                transform.Push();
                for (int j = 0; j < cols; ++j)
                {
                    result.AddRange(model.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_REGULAR"), meshes));
                    if (selected)
                        result.AddRange(model.DrawModelTreeInvert(dev, transform, meshes));
                    transform.NJTranslate(0, 0, 15);
                }
                transform.Pop();
                transform.NJTranslate(15, 0, 0);
            }
            transform.Pop();
            return result.ToArray();
        }

		public override SonicRetro.SAModel.BoundingSphere GetBounds(SETItem item)
		{
			SonicRetro.SAModel.BoundingSphere bounds = new SonicRetro.SAModel.BoundingSphere(item.Position, SonicRetro.SAModel.Direct3D.Extensions.GetLargestRadius(meshes));

			return bounds;
		}

        public override string Name { get { return "Spikes"; } }

		private PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Rows", typeof(int), "Extended", null, 1, (o) => Math.Max((int)o.Scale.X, 1), (o, v) => o.Scale.X = Math.Max((int)v, 1)),
			new PropertySpec("Columns", typeof(int), "Extended", null, 1, (o) => Math.Max((int)o.Scale.Z, 1), (o, v) => o.Scale.Z = Math.Max((int)v, 1))
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
	}
}