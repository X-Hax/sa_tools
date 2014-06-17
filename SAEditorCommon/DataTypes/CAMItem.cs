using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.UI;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;

namespace SonicRetro.SAModel.SAEditorCommon.DataTypes
{
    [Serializable]
    public class CAMItem : Item
    {
        #region Camera Data Vars
        public byte CamType { get; set; }
        public byte Unknown { get; set; }
        public byte PanSpeed { get; set; }
        public byte Priority { get; set; }
        public short Unknown_2 { get; set; } // possibly a x or z rotation
        //public short YRotation { get; set; }
        public Vertex Scale { get; set; }
        public Int32 NotUsed { get; set; }
        public Vertex PointA { get; set; }
        public Vertex PointB { get; set; }
        public float Variable { get; set; }

        public override Vertex Position { get; set; }
        /*public override Rotation Rotation
        {
            get
            {
                return new Rotation(0, YRotation, 0);
            }
            set
            {
                YRotation = (short)value.Y;
            }
        }*/
        public override Rotation Rotation { get; set; }
        #endregion

        #region Render / Volume Vars
        Microsoft.DirectX.Direct3D.Mesh volumeMesh;
        SonicRetro.SAModel.Material material;
        #endregion

        #region Construction / Initialization
        /// <summary>
        ///  Create a new CAM Item from within the editor.
        /// </summary>
        /// <param name="dev">An active Direct3D device for meshing/material/rendering purposes.</param>
        public CAMItem(Device dev)
        {
            Position = new Vertex();
            Rotation = new Rotation();

            Init(dev);
        }

        /// <summary>
        /// Creates a new CAM Item from a byte array and offset.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="address"></param>
        public CAMItem(Device dev, byte[] file, int address)
        {
            CamType = file[address];
            Unknown = file[address + 1];
            PanSpeed = file[address + 2];
            Priority = file[address + 3];
            Unknown_2 = BitConverter.ToInt16(file, address + 4);
            Rotation = new Rotation(0, BitConverter.ToInt16(file, address + 6), 0);
            Position = new Vertex(file, address + 8);
            Scale = new Vertex(file, address + 20);
            NotUsed = BitConverter.ToInt32(file, address + 32);
            PointA = new Vertex(file, address + 36);
            PointB = new Vertex(file, address + 48);
            Variable = BitConverter.ToSingle(file, address + 60);

            Init(dev);
        }

        private void Init(Device dev)
        {
            volumeMesh = Microsoft.DirectX.Direct3D.Mesh.Box(dev, 1.5f, 1.5f, 1.5f);
            material = new Material();
            material.DiffuseColor = System.Drawing.Color.FromArgb(200, System.Drawing.Color.Purple);
            material.SpecularColor = System.Drawing.Color.Black;
            material.DoubleSided = false;
            material.Exponent = 10;
            material.IgnoreSpecular = false;
            material.UseTexture = false;
        }
        #endregion

        #region Add / Delete
        public override void Paste()
        {
            LevelData.CAMItems[LevelData.Character].Add(this);
        }

        public override void Delete()
        {
            LevelData.CAMItems[LevelData.Character].Remove(this);
        }
        #endregion

        #region Rendering / Picking
        public override RenderInfo[] Render(Device dev, EditorCamera camera, MatrixStack transform, bool selected)
        {
            float largestScale = this.Scale.X;
            if (this.Scale.Y > largestScale) largestScale = this.Scale.Y;
            if (this.Scale.Z > largestScale) largestScale = this.Scale.Z;

            SonicRetro.SAModel.BoundingSphere boxSphere = new SonicRetro.SAModel.BoundingSphere() { Center = new SonicRetro.SAModel.Vertex(this.Position.X, this.Position.Y, this.Position.Z), Radius = (1.5f * largestScale) };

            float dist = SonicRetro.SAModel.Direct3D.Extensions.Distance(camera.Position, boxSphere.Center.ToVector3());
            if (dist > camera.DrawDistance) return Item.EmptyRenderInfo;

            List<RenderInfo> result = new List<RenderInfo>();
            transform.Push();
            transform.NJTranslate(this.Position);
            transform.NJRotateY(this.Rotation.Y);
            transform.NJScale((this.Scale.X), (this.Scale.Y), (this.Scale.Z));

            RenderInfo outputInfo = new RenderInfo(volumeMesh, 0, transform.Top, material, null, FillMode.Solid, boxSphere);

            if (selected)
            {
                Material mat = new Material
                {
                    DiffuseColor = System.Drawing.Color.White,
                    IgnoreLighting = true,
                    UseAlpha = false
                };
                result.Add(new RenderInfo(volumeMesh, 0, transform.Top, mat, null, FillMode.WireFrame, boxSphere));
            }

            result.Add(outputInfo);

            transform.Pop();
            return result.ToArray();
        }

        public override HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View)
        {
            MatrixStack transform = new MatrixStack();

            transform.Push();
            transform.NJTranslate(this.Position);
            transform.NJRotateY(this.Rotation.Y);
            transform.NJScale((this.Scale.X), (this.Scale.Y), (this.Scale.Z));

            HitResult result = volumeMesh.CheckHit(Near, Far, Viewport, Projection, View, transform);

            transform.Pop();
            return result;
        }
        #endregion
    }
}
