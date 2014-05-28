using System.ComponentModel;
using System;
using System.Collections.Generic;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.UI;

namespace SonicRetro.SAModel.SAEditorCommon.DataTypes
{
    [Serializable]
    public class LevelItem : Item
    {
        [Browsable(false)]
        private COL COL { get; set; }
        public COL CollisionData { get { return COL; } }
        [Browsable(false)]
        private Microsoft.DirectX.Direct3D.Mesh Mesh { get; set; }

        private Device dev;

        /// <summary>
        /// Creates a Levelitem from an external file.
        /// </summary>
        /// <param name="dev">Current Direct3D device.</param>
        /// <param name="filePath">location of the file to use.</param>
        /// <param name="position">Position to place the resulting model (worldspace).</param>
        /// <param name="rotation">Rotation to apply to the model.</param>
        public LevelItem(Device dev, string filePath, Vertex position, Rotation rotation)
        {
            this.dev = dev;
            COL = new COL();
            COL.Model = new SonicRetro.SAModel.Object();
            COL.Model.Position = position;
            COL.Model.Rotation = rotation;
            ImportModel(filePath);
            COL.CalculateBounds();
            Paste();
        }

        /// <summary>
        /// Creates a LevelItem from an existing COL data.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="dev">Current Direct3d Device.</param>
        public LevelItem(COL col, Device dev)
        {
            COL = col;
            col.Model.ProcessVertexData();
            Mesh = col.Model.Attach.CreateD3DMesh(dev);
            this.dev = dev;
        }

        /// <summary>
        /// Creates a new instance of an existing item with the specified position and rotation.
        /// </summary>
        /// <param name="attach">Attach to use for this levelItem</param>
        /// <param name="position">Position in worldspace to place this LevelItem.</param>
        /// <param name="rotation">Rotation.</param>
        public LevelItem(Device dev, Attach attach, Vertex position, Rotation rotation)
        {
            COL = new COL();
            COL.Model = new SonicRetro.SAModel.Object();
            COL.Model.Attach = attach;
            COL.Model.Position = position;
            COL.Model.Rotation = rotation;
            Visible = true;
            Solid = true;
            COL.CalculateBounds();
            Mesh = COL.Model.Attach.CreateD3DMesh(dev);
            Paste();
        }

        public override Vertex Position { get { return COL.Model.Position; } set { COL.Model.Position = value; } }

        public override Rotation Rotation { get { return COL.Model.Rotation; } set { COL.Model.Rotation = value; } }

        public override HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View)
        {
            return COL.Model.CheckHit(Near, Far, Viewport, Projection, View, Mesh);
        }

        public override RenderInfo[] Render(Device dev, EditorCamera camera, MatrixStack transform, bool selected)
        {
            float dist = SonicRetro.SAModel.Direct3D.Extensions.Distance(camera.Position, this.CollisionData.Bounds.Center.ToVector3());
            if (dist > camera.DrawDistance) return Item.EmptyRenderInfo;
            
            List<RenderInfo> result = new List<RenderInfo>();
            if (!string.IsNullOrEmpty(LevelData.leveltexs))
                result.AddRange(COL.Model.DrawModel(dev, transform, LevelData.Textures[LevelData.leveltexs], Mesh, Visible));
            else
                result.AddRange(COL.Model.DrawModel(dev, transform, null, Mesh, Visible));
            if (selected)
                result.AddRange(COL.Model.DrawModelInvert(dev, transform, Mesh, Visible));
            return result.ToArray();
        }

        public override void Paste()
        {
            LevelData.LevelItems.Add(this);
            LevelData.geo.COL.Add(COL);
        }

        public override void Delete()
        {
            LevelData.geo.COL.Remove(COL);
            LevelData.LevelItems.Remove(this);
        }

        [Browsable(true)]
        [DisplayName("Import Model")]
        public void ImportModel(string filePath)
        {
            COL.Model.Attach = SonicRetro.SAModel.Direct3D.Extensions.obj2nj(filePath);
            Visible = true;
            Solid = true;

            Mesh = COL.Model.Attach.CreateD3DMesh(dev);
        }

        //[Browsable(true)]
        [DisplayName("Export Model")]
        public void ExportModel()
        {

        }

        [Browsable(true)]
        [DisplayName("Edit Materials")]
        public void EditMaterials()
        {
            if (COL.Model.Attach is BasicAttach)
            {
                using (MaterialEditor pw = new MaterialEditor(((BasicAttach)COL.Model.Attach).Material.ToArray(), LevelData.TextureBitmaps[LevelData.leveltexs]))
                {
                    pw.FormUpdated += new MaterialEditor.FormUpdatedHandler(pw_FormUpdated);
                    pw.ShowDialog();
                }
            }
        }

        public string Flags
        {
            get
            {
                return COL.Flags.ToString("X8");
            }
            set
            {
                COL.Flags = int.Parse(value, System.Globalization.NumberStyles.HexNumber);
            }
        }

        public bool Visible
        {
            get
            {
                return (COL.SurfaceFlags & SurfaceFlags.Visible) == SurfaceFlags.Visible;
            }
            set
            {
                COL.SurfaceFlags = (COL.SurfaceFlags & ~SurfaceFlags.Visible) | (value ? SurfaceFlags.Visible : 0);
            }
        }

        public bool Solid
        {
            get
            {
                return (COL.SurfaceFlags & SurfaceFlags.Solid) == SurfaceFlags.Solid;
            }
            set
            {
                COL.SurfaceFlags = (COL.SurfaceFlags & ~SurfaceFlags.Solid) | (value ? SurfaceFlags.Solid : 0);
            }
        }

        public bool Water
        {
            get
            {
                return (COL.SurfaceFlags & SurfaceFlags.Water) == SurfaceFlags.Water;
            }
            set
            {
                COL.SurfaceFlags = (COL.SurfaceFlags & ~SurfaceFlags.Water) | (value ? SurfaceFlags.Water : 0);
            }
        }

        public void Save() { COL.CalculateBounds(); }

        // Form property update event method
        void pw_FormUpdated(object sender, EventArgs e)
        {
            LevelData.InvalidateRenderState();
        }
    }
}
