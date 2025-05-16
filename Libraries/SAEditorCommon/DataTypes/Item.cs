﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using SharpDX;
using SharpDX.Direct3D9;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.PropertyGrid;
using SAModel.SAEditorCommon.UI;

namespace SAModel.SAEditorCommon.DataTypes
{
	[Serializable]
	public abstract class Item : IComponent
	{
		[ReadOnly(true)]
		[Category("Common"), ParenthesizePropertyName(true)]
		public string Type { get { return GetType().Name; } }

		[Browsable(false)]
		public bool Selected { get; private set; }
		private BoundingSphere bounds = new BoundingSphere();
		[Category("Common"), ParenthesizePropertyName(true)]
		public virtual BoundingSphere Bounds { get { return bounds; } }
		protected Matrix transformMatrix = Matrix.Identity;
		protected Vertex position = new Vertex();
		protected Rotation rotation = new Rotation();
		protected Vertex scale = new Vertex(1, 1, 1);
		protected bool rotateZYX = false;

		[Category("Common")]
		public virtual Vertex Position { get { return position; } set { position = value; GetHandleMatrix(); } }
		[Category("Common")]
		public virtual Rotation Rotation { get { return rotation; } set { rotation = value; GetHandleMatrix(); } }
		[Browsable(false)]
		public Matrix TransformMatrix { get { return transformMatrix; } }

		[Browsable(false)]
		public virtual bool CanCopy { get { return true; } }
		public abstract void Paste();
		protected abstract void DeleteInternal(EditorItemSelection selectionManager);

		public void Delete(EditorItemSelection selectionManager)
		{
			selectionManager.SelectionChanged -= selectionManager_SelectionChanged;
			DeleteInternal(selectionManager);
		}

		public abstract HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View);
		public abstract List<RenderInfo> Render(Device dev, EditorCamera camera, MatrixStack transform);

		public Item(EditorItemSelection selectionManager)
		{
			selectionManager.SelectionChanged += selectionManager_SelectionChanged;
		}

		void selectionManager_SelectionChanged(EditorItemSelection sender)
		{
			Selected = sender.Contains(this);
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
