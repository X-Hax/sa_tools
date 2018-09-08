using System;
using System.Collections.Generic;
using System.ComponentModel;
using SharpDX;
using SharpDX.Direct3D9;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.PropertyGrid;
using SonicRetro.SAModel.SAEditorCommon.UI;

namespace SonicRetro.SAModel.SAEditorCommon.DataTypes
{
	[Serializable]
	public abstract class Item : IComponent
	{
		[ReadOnly(true)]
		[ParenthesizePropertyName(true)]
		public string Type { get { return GetType().Name; } }

		private bool selected;
		[Browsable(false)]
		public bool Selected { get { return selected; } }
		private BoundingSphere bounds = new BoundingSphere();
		[ParenthesizePropertyName(true)]
		public virtual BoundingSphere Bounds { get { return bounds; } }

		protected Matrix transformMatrix = Matrix.Identity;
		protected Vertex position = new Vertex();
		protected Rotation rotation = new Rotation();
		protected bool rotateZYX = false;

		public virtual Vertex Position { get { return position; } set { position = value; GetHandleMatrix(); } }
		public virtual Rotation Rotation { get { return rotation; } set { rotation = value; GetHandleMatrix(); } }
		public Matrix TransformMatrix { get { return transformMatrix; } }

		[Browsable(false)]
		public virtual bool CanCopy { get { return true; } }
		public abstract void Paste();
		public abstract void Delete();
		public abstract HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View);
		public abstract List<RenderInfo> Render(Device dev, EditorCamera camera, MatrixStack transform);

		public Item(EditorItemSelection selectionManager)
		{
			selectionManager.SelectionChanged += selectionManager_SelectionChanged;
		}

		void selectionManager_SelectionChanged(EditorItemSelection sender)
		{
			selected = (sender.GetSelection().Contains(this));
		}

		#region IComponent Members
		// IComponent required by PropertyGrid control to discover IMenuCommandService supporting DesignerVerbs

		public event EventHandler Disposed;

		// ** Item of interest ** Return the site object that supports DesignerVerbs
		[Browsable(false)]
		public ISite Site
		{
			// return our "site" which connects back to us to expose our tagged methods
			get { return new DesignerVerbSite(this); }
			set { throw new NotImplementedException(); }
		}

		#endregion

		#region IDisposable Members
		// IDisposable, part of IComponent support

		public void Dispose()
		{
			// never called in this specific context with the PropertyGrid
			// but just reference the required Disposed event to avoid warnings
			if (Disposed != null)
				Disposed(this, EventArgs.Empty);
		}
		#endregion

		protected static readonly List<RenderInfo> EmptyRenderInfo = new List<RenderInfo>();

		public static Vertex CenterFromSelection(List<Item> SelectedItems)
		{
			if (SelectedItems == null) return new Vertex();

			List<Vertex> vertList = new List<Vertex>();
			foreach (Item item in SelectedItems)
			{
				if (item is LevelItem)
				{
					LevelItem levelItem = (LevelItem)item;

					vertList.Add(levelItem.CollisionData.Bounds.Center);
				}
				else
				{
					vertList.Add(item.Position);
				}
			}

			return Vertex.CenterOfPoints(vertList);
		}

		protected virtual void GetHandleMatrix()
		{
			transformMatrix = Matrix.Identity;

			MatrixStack matrixStack = new MatrixStack();
			matrixStack.LoadMatrix(Matrix.Identity);
			matrixStack.NJTranslate(Position);
			if (!rotateZYX) matrixStack.NJRotateXYZ(Rotation);
			else matrixStack.NJRotateZYX(Rotation);

			transformMatrix = matrixStack.Top;
		}
	}
}
