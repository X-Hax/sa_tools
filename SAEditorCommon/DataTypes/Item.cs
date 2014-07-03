using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;

using SonicRetro.SAModel.SAEditorCommon.PropertyGrid;

namespace SonicRetro.SAModel.SAEditorCommon.DataTypes
{
    [Serializable]
    public abstract class Item : IComponent
    {
        [ReadOnly(true)]
        [ParenthesizePropertyName(true)]
        public string Type { get { return GetType().Name; } }

        public abstract Vertex Position { get; set; }
        public abstract Rotation Rotation { get; set; }

        [Browsable(false)]
        public virtual bool CanCopy { get { return true; } }
        public abstract void Paste();
        public abstract void Delete();
        public abstract HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View);
        public abstract RenderInfo[] Render(Device dev, EditorCamera camera, MatrixStack transform, bool selected);

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

        protected static readonly RenderInfo[] EmptyRenderInfo = new RenderInfo[0];

        public static Vertex CenterFromSelection(List<Item> SelectedItems)
        {
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
			SAModel.Direct3D.MatrixFunctions.RotateXYZ(ref transform, Rotation.X, Rotation.Y, Rotation.Z);

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
