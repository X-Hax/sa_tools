using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon;
using SonicRetro.SAModel.SAEditorCommon.UI;

namespace SonicRetro.SAModel.SAEditorCommon.DataTypes
{
    public class DeathZoneItem : Item
    {
        [Browsable(false)]
        private SonicRetro.SAModel.Object Model { get; set; }
        [Browsable(false)]
        private Microsoft.DirectX.Direct3D.Mesh Mesh { get; set; }

        private Device dev;

        public DeathZoneItem(Device dev)
        {
            this.dev = dev;
            Model = new SonicRetro.SAModel.Object();
            ImportModel();
            Paste();
        }

        public DeathZoneItem(SonicRetro.SAModel.Object model, SADXPCTools.SA1CharacterFlags flags, Device dev)
        {
            Model = model;
            model.ProcessVertexData();
            Flags = flags;
            Mesh = Model.Attach.CreateD3DMesh(dev);
            this.dev = dev;
        }

        public override Vertex Position { get { return Model.Position; } set { Model.Position = value; } }

        public override Rotation Rotation { get { return Model.Rotation; } set { Model.Rotation = value; } }

        public override HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View)
        {
            return Model.CheckHit(Near, Far, Viewport, Projection, View, Mesh);
        }

        public override RenderInfo[] Render(Device dev, MatrixStack transform, bool selected)
        {
            List<RenderInfo> result = new List<RenderInfo>();
            result.AddRange(Model.DrawModel(dev, transform, LevelData.Textures[LevelData.leveltexs], Mesh, false));
            if (selected)
                result.AddRange(Model.DrawModelInvert(dev, transform, Mesh, false));
            return result.ToArray();
        }

        public override void Paste()
        {
            if (LevelData.DeathZones != null)
                LevelData.DeathZones.Add(this);
        }

        public override void Delete()
        {
            LevelData.DeathZones.Remove(this);
        }

        [Browsable(true)]
        [DisplayName("Import Model")]
        public void ImportModel()
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog() { DefaultExt = "obj", Filter = "OBJ Files|*.obj;*.objf", RestoreDirectory = true };
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Model.Attach = SonicRetro.SAModel.Direct3D.Extensions.obj2nj(dlg.FileName);
                Mesh = Model.Attach.CreateD3DMesh(dev);
            }
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
            if (Model.Attach is BasicAttach)
            {
                using (MaterialEditor pw = new MaterialEditor(((BasicAttach)Model.Attach).Material.ToArray(), LevelData.TextureBitmaps[LevelData.leveltexs]))
                {
                    pw.FormUpdated += new MaterialEditor.FormUpdatedHandler(pw_FormUpdated);
                    pw.ShowDialog();
                }
            }
        }

        public SADXPCTools.SA1CharacterFlags Flags { get; set; }

        [Browsable(false)]
        public bool Visible
        {
            get
            {
                switch (LevelData.Character)
                {
                    case 0:
                        return (Flags & SADXPCTools.SA1CharacterFlags.Sonic) == SADXPCTools.SA1CharacterFlags.Sonic;
                    case 1:
                        return (Flags & SADXPCTools.SA1CharacterFlags.Tails) == SADXPCTools.SA1CharacterFlags.Tails;
                    case 2:
                        return (Flags & SADXPCTools.SA1CharacterFlags.Knuckles) == SADXPCTools.SA1CharacterFlags.Knuckles;
                    case 3:
                        return (Flags & SADXPCTools.SA1CharacterFlags.Amy) == SADXPCTools.SA1CharacterFlags.Amy;
                    case 4:
                        return (Flags & SADXPCTools.SA1CharacterFlags.Gamma) == SADXPCTools.SA1CharacterFlags.Gamma;
                    case 5:
                        return (Flags & SADXPCTools.SA1CharacterFlags.Big) == SADXPCTools.SA1CharacterFlags.Big;
                }
                return false;
            }
        }

        public bool Sonic
        {
            get
            {
                return (Flags & SADXPCTools.SA1CharacterFlags.Sonic) == SADXPCTools.SA1CharacterFlags.Sonic;
            }
            set
            {
                Flags = (Flags & ~SADXPCTools.SA1CharacterFlags.Sonic) | (value ? SADXPCTools.SA1CharacterFlags.Sonic : 0);
            }
        }

        public bool Tails
        {
            get
            {
                return (Flags & SADXPCTools.SA1CharacterFlags.Tails) == SADXPCTools.SA1CharacterFlags.Tails;
            }
            set
            {
                Flags = (Flags & ~SADXPCTools.SA1CharacterFlags.Tails) | (value ? SADXPCTools.SA1CharacterFlags.Tails : 0);
            }
        }

        public bool Knuckles
        {
            get
            {
                return (Flags & SADXPCTools.SA1CharacterFlags.Knuckles) == SADXPCTools.SA1CharacterFlags.Knuckles;
            }
            set
            {
                Flags = (Flags & ~SADXPCTools.SA1CharacterFlags.Knuckles) | (value ? SADXPCTools.SA1CharacterFlags.Knuckles : 0);
            }
        }

        public bool Amy
        {
            get
            {
                return (Flags & SADXPCTools.SA1CharacterFlags.Amy) == SADXPCTools.SA1CharacterFlags.Amy;
            }
            set
            {
                Flags = (Flags & ~SADXPCTools.SA1CharacterFlags.Amy) | (value ? SADXPCTools.SA1CharacterFlags.Amy : 0);
            }
        }

        public bool Gamma
        {
            get
            {
                return (Flags & SADXPCTools.SA1CharacterFlags.Gamma) == SADXPCTools.SA1CharacterFlags.Gamma;
            }
            set
            {
                Flags = (Flags & ~SADXPCTools.SA1CharacterFlags.Gamma) | (value ? SADXPCTools.SA1CharacterFlags.Gamma : 0);
            }
        }

        public bool Big
        {
            get
            {
                return (Flags & SADXPCTools.SA1CharacterFlags.Big) == SADXPCTools.SA1CharacterFlags.Big;
            }
            set
            {
                Flags = (Flags & ~SADXPCTools.SA1CharacterFlags.Big) | (value ? SADXPCTools.SA1CharacterFlags.Big : 0);
            }
        }

        public SADXPCTools.DeathZoneFlags Save(string path, int i)
        {
            ModelFile.CreateFile(System.IO.Path.Combine(path, i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + ".sa1mdl"), Model, null, null, null, LevelData.LevelName + " Death Zone " + i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo), "SADXLVL2", null, ModelFormat.Basic);
            return new SADXPCTools.DeathZoneFlags() { Flags = Flags };
        }

        // Form property update event method
        void pw_FormUpdated(object sender, EventArgs e)
        {
            LevelData.InvalidateRenderState();
        }
    }
}
