using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.UI;

namespace SonicRetro.SAModel.SAEditorCommon.DataTypes
{
	public class DeathZoneItem : Item
	{
		[Browsable(false)]
		private Object Model { get; set; }
		[NonSerialized]
		private Microsoft.DirectX.Direct3D.Mesh mesh;
		[Browsable(false)]
		private Microsoft.DirectX.Direct3D.Mesh Mesh { get { return mesh; } set { mesh = value; } }

		[Browsable(false)]
		public override BoundingSphere Bounds
		{
			get
			{
				return Model.Attach.Bounds;
			}
		}

		[NonSerialized]
		private Device dev;

		public DeathZoneItem(Device dev, EditorItemSelection selectionManager)
			: base (selectionManager)
		{
			this.dev = dev;
			Model = new Object();
			ImportModel();
			Paste();
		}

		public DeathZoneItem(Object model, SA_Tools.SA1CharacterFlags flags, Device dev, EditorItemSelection selectionManager)
			: base(selectionManager)
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

		public override List<RenderInfo> Render(Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			result.AddRange(Model.DrawModel(dev, transform, LevelData.Textures[LevelData.leveltexs], Mesh, false));
			if (Selected)
				result.AddRange(Model.DrawModelInvert(dev, transform, Mesh, false));
			return result;
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
				Model.Attach = Direct3D.Extensions.obj2nj(dlg.FileName, LevelData.TextureBitmaps[LevelData.leveltexs].Select(a => a.Name).ToArray());
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
				using (MaterialEditor pw = new MaterialEditor(((BasicAttach)Model.Attach).Material, LevelData.TextureBitmaps[LevelData.leveltexs]))
				{
					pw.FormUpdated += new MaterialEditor.FormUpdatedHandler(pw_FormUpdated);
					pw.ShowDialog();
				}
			}
		}

		public SA_Tools.SA1CharacterFlags Flags { get; set; }

		[Browsable(false)]
		public bool Visible
		{
			get
			{
				switch (LevelData.Character)
				{
					case 0:
						return (Flags & SA_Tools.SA1CharacterFlags.Sonic) == SA_Tools.SA1CharacterFlags.Sonic;
					case 1:
						return (Flags & SA_Tools.SA1CharacterFlags.Tails) == SA_Tools.SA1CharacterFlags.Tails;
					case 2:
						return (Flags & SA_Tools.SA1CharacterFlags.Knuckles) == SA_Tools.SA1CharacterFlags.Knuckles;
					case 3:
						return (Flags & SA_Tools.SA1CharacterFlags.Amy) == SA_Tools.SA1CharacterFlags.Amy;
					case 4:
						return (Flags & SA_Tools.SA1CharacterFlags.Gamma) == SA_Tools.SA1CharacterFlags.Gamma;
					case 5:
						return (Flags & SA_Tools.SA1CharacterFlags.Big) == SA_Tools.SA1CharacterFlags.Big;
				}
				return false;
			}
		}

		public bool Sonic
		{
			get
			{
				return (Flags & SA_Tools.SA1CharacterFlags.Sonic) == SA_Tools.SA1CharacterFlags.Sonic;
			}
			set
			{
				Flags = (Flags & ~SA_Tools.SA1CharacterFlags.Sonic) | (value ? SA_Tools.SA1CharacterFlags.Sonic : 0);
			}
		}

		public bool Tails
		{
			get
			{
				return (Flags & SA_Tools.SA1CharacterFlags.Tails) == SA_Tools.SA1CharacterFlags.Tails;
			}
			set
			{
				Flags = (Flags & ~SA_Tools.SA1CharacterFlags.Tails) | (value ? SA_Tools.SA1CharacterFlags.Tails : 0);
			}
		}

		public bool Knuckles
		{
			get
			{
				return (Flags & SA_Tools.SA1CharacterFlags.Knuckles) == SA_Tools.SA1CharacterFlags.Knuckles;
			}
			set
			{
				Flags = (Flags & ~SA_Tools.SA1CharacterFlags.Knuckles) | (value ? SA_Tools.SA1CharacterFlags.Knuckles : 0);
			}
		}

		public bool Amy
		{
			get
			{
				return (Flags & SA_Tools.SA1CharacterFlags.Amy) == SA_Tools.SA1CharacterFlags.Amy;
			}
			set
			{
				Flags = (Flags & ~SA_Tools.SA1CharacterFlags.Amy) | (value ? SA_Tools.SA1CharacterFlags.Amy : 0);
			}
		}

		public bool Gamma
		{
			get
			{
				return (Flags & SA_Tools.SA1CharacterFlags.Gamma) == SA_Tools.SA1CharacterFlags.Gamma;
			}
			set
			{
				Flags = (Flags & ~SA_Tools.SA1CharacterFlags.Gamma) | (value ? SA_Tools.SA1CharacterFlags.Gamma : 0);
			}
		}

		public bool Big
		{
			get
			{
				return (Flags & SA_Tools.SA1CharacterFlags.Big) == SA_Tools.SA1CharacterFlags.Big;
			}
			set
			{
				Flags = (Flags & ~SA_Tools.SA1CharacterFlags.Big) | (value ? SA_Tools.SA1CharacterFlags.Big : 0);
			}
		}

		public SA_Tools.DeathZoneFlags Save(string path, int i)
		{
			ModelFile.CreateFile(System.IO.Path.Combine(path, i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + ".sa1mdl"), Model, null, null, null, LevelData.LevelName + " Death Zone " + i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo), "SADXLVL2", null, ModelFormat.Basic);
			return new SA_Tools.DeathZoneFlags() { Flags = Flags };
		}

		// Form property update event method
		void pw_FormUpdated(object sender, EventArgs e)
		{
			LevelData.InvalidateRenderState();
		}
	}
}
