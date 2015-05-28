using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.UI;

namespace SonicRetro.SAModel.SAEditorCommon.DataTypes
{
	public class StartPosItem : Item
	{
		private NJS_OBJECT Model;
		private Mesh[] Meshes;
		private string texture;
		private float offset;

		public StartPosItem(NJS_OBJECT model, string textures, float offset, Vertex position, int yrot, Device dev, EditorItemSelection selectionManager)
			: base(selectionManager)
		{
			Model = model;
			model.ProcessVertexData();
			NJS_OBJECT[] models = model.GetObjects();
			Meshes = new Mesh[models.Length];
			for (int i = 0; i < models.Length; i++)
				if (models[i].Attach != null)
					Meshes[i] = models[i].Attach.CreateD3DMesh(dev);
			texture = textures;
			this.offset = offset;
			Position = position;
			YRotation = yrot;
		}

		public override Vertex Position { get; set; }

		[Browsable(false)]
		public int YRotation { get; set; }

		[DisplayName("Y Rotation")]
		public float YRotDeg
		{
			get { return Rotation.BAMSToDeg(YRotation); }
			set { YRotation = Rotation.DegToBAMS(value); }
		}

		[Browsable(false)]
		public override Rotation Rotation { get { return new Rotation(0, YRotation, 0); } set { YRotation = value.Y; } }

		public override bool CanCopy { get { return false; } }

		public override void Paste()
		{
			throw new NotImplementedException();
		}

		public override void Delete()
		{
			throw new NotImplementedException();
		}

		public override HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View)
		{
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(0, offset, 0);
			transform.NJTranslate(Position);
			transform.NJRotateY(YRotation);
			return Model.CheckHit(Near, Far, Viewport, Projection, View, transform, Meshes);
		}

		public override List<RenderInfo> Render(Device dev, EditorCamera camera, MatrixStack transform)
		{
			float dist = Direct3D.Extensions.Distance(camera.Position, Position.ToVector3());
			if (dist > camera.DrawDistance) return EmptyRenderInfo;

			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(0, offset, 0);
			transform.NJTranslate(Position);
			transform.NJRotateY(YRotation);
			result.AddRange(Model.DrawModelTree(dev, transform, LevelData.Textures[texture], Meshes));
			if (Selected)
				result.AddRange(Model.DrawModelTreeInvert(dev, transform, Meshes));
			transform.Pop();
			return result;
		}
	}
}
