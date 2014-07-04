using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SADXLVL2;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using System.Drawing;

namespace SADXObjectDefinitions.Common
{
    class PUWind : ObjectDefinition
    {
        SonicRetro.SAModel.Material material;
        Texture texture;
        Mesh mesh;

        public override void Init(ObjectData data, string name, Device dev)
        {
            mesh = Mesh.Box(dev, 1f, 1f, 1f);
            material = new SonicRetro.SAModel.Material();
            material.DiffuseColor = Color.FromArgb(180,180,180,180);
            material.UseAlpha = true;
            texture = new Texture(dev, new System.Drawing.Bitmap(2, 2), 0, Pool.Managed);
        }

        public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
        {
            transform.Push();
            transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y - 0x5772);
            transform.NJScale((item.Scale.X), (item.Scale.Y), (item.Scale.Z));
            HitResult result = mesh.CheckHit(Near, Far, Viewport, Projection, View, transform);
            
            transform.Pop(); 
            return result;
        }

		public override RenderInfo[] Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform, bool selected)
        {
            List<RenderInfo> result = new List<RenderInfo>();
            transform.Push();
            transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y - 0x5772);
            transform.NJScale((item.Scale.X), (item.Scale.Y), (item.Scale.Z));

            float largestScale = item.Scale.X;
            if (item.Scale.Y > largestScale) largestScale = item.Scale.Y;
            if(item.Scale.Z > largestScale) largestScale = item.Scale.Z;

            SonicRetro.SAModel.BoundingSphere boxSphere = new SonicRetro.SAModel.BoundingSphere() { Center = new SonicRetro.SAModel.Vertex(item.Position.X,item.Position.Y, item.Position.Z), Radius = (1.5f * largestScale) };

            RenderInfo outputInfo = new RenderInfo(mesh, 0, transform.Top, material, texture, FillMode.Solid, boxSphere);
            result.Add(outputInfo);

            transform.Pop();
            return result.ToArray();
        }

		public override SonicRetro.SAModel.BoundingSphere GetBounds(SETItem item)
		{
			float largestScale = (item.Scale.X + 10) / 5f;
			if (item.Scale.Y > largestScale) largestScale = (item.Scale.Y + 10) / 5f;
			if (item.Scale.Z > largestScale) largestScale = (item.Scale.Z + 10) / 5f;

			SonicRetro.SAModel.BoundingSphere boxSphere = new SonicRetro.SAModel.BoundingSphere() { Center = new SonicRetro.SAModel.Vertex(item.Position.X, item.Position.Y, item.Position.Z), Radius = (largestScale / 2) };

			return boxSphere;
		}

        public override string Name { get { return "Player-Up Wind"; } }
    }
}
