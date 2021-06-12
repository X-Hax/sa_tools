using System;
using System.Collections.Generic;
using System.ComponentModel;
using SharpDX;
using SharpDX.Direct3D9;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.UI;
using Mesh = SAModel.Direct3D.Mesh;

namespace SAModel.SAEditorCommon.DataTypes
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
					Meshes[i] = models[i].Attach.CreateD3DMesh();
			texture = textures;
			this.offset = offset;
			Position = position;
			YRotation = yrot;
		}

		[Browsable(false)]
		public int YRotation { get; set; }

		[Category("Data"), DisplayName("Y Rotation")]
		public float YRotDeg
		{
			get { return Rotation.BAMSToDeg(YRotation); }
			set { YRotation = Rotation.DegToBAMS(value); }
		}

		[Browsable(false)]
		public override Rotation Rotation { get { return new Rotation(0, YRotation, 0); } set { YRotation = value.Y; GetHandleMatrix(); } }

		public override bool CanCopy { get { return false; } }

		public override void Paste()
		{
			throw new NotImplementedException();
		}

		protected override void DeleteInternal(EditorItemSelection selectionManager)
		{
			
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
			transform.NJRotateY(-0x8000 - YRotation);
			if (LevelData.Textures != null && LevelData.Textures.Count > 0 && LevelData.Textures.ContainsKey(texture))
			{
				result.AddRange(Model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, LevelData.Textures[texture], Meshes));
			}
			else
			{
				result.AddRange(Model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, Meshes));
			}
			if (Selected)
				result.AddRange(Model.DrawModelTreeInvert(transform, Meshes));
			transform.Pop();
			return result;
		}
	}
}
