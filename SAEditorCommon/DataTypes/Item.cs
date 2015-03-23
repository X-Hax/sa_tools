using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

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

		public virtual Vertex Position { get; set; }

		private bool selected;
		public bool Selected { get { return selected; } }
		private BoundingSphere bounds = new BoundingSphere();
		public virtual BoundingSphere Bounds { get { return bounds; } }
		public abstract Rotation Rotation { get; set; }

		[Browsable(false)]
		public virtual bool CanCopy { get { return true; } }
		public abstract void Paste();
		public abstract void Delete();
		public abstract HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View);
		public abstract List<RenderInfo> Render(Device dev, EditorCamera camera, MatrixStack transform);

		public Item(EditorItemSelection selectionManager)
		{
			selectionManager.SelectionChanged += new EditorItemSelection.SelectionChangeHandler(selectionManager_SelectionChanged);
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

			Vertex center = new Vertex();

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

			center = Vertex.CenterOfPoints(vertList);

			return center;
		}

		public Matrix GetLocalAxes(out Vector3 Up, out Vector3 Right, out Vector3 Look)
		{
			Matrix transform = Matrix.Identity;

			try
			{
				SAModel.Direct3D.MatrixFunctions.RotateXYZ(ref transform, Rotation.X, Rotation.Y, Rotation.Z);
			}
			catch (NotSupportedException)
			{
				Console.WriteLine("Certain Item types don't support rotations. This can be ignored.");
			}

			Up = new Vector3(0, 1, 0);
			Look = new Vector3(0, 0, 1);
			Right = new Vector3(1, 0, 0);

			Up = Vector3.TransformCoordinate(Up, transform);
			Look = Vector3.TransformCoordinate(Look, transform);
			Right = Vector3.TransformCoordinate(Right, transform);

			return transform;
		}
	}
}
