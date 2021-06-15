using SplitTools;
using SharpDX;
using SharpDX.Direct3D9;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Mesh = SAModel.Direct3D.Mesh;

namespace SAModel.SAEditorCommon.DataTypes
{
	public class DeathZoneItem : Item, IScaleable
	{
		[Browsable(false)]
		private NJS_OBJECT Model { get; set; }
		[NonSerialized]
		private Mesh mesh;
		[Browsable(false)]
		private Mesh Mesh { get { return mesh; } set { mesh = value; } }
		[Category("Common")]
		public string Name { get { return Model.Name; } }

		[Browsable(false)]
		public override BoundingSphere Bounds
		{
			get
			{
				Matrix m = Matrix.Identity;
				Model.ProcessTransforms(m);
				float scale = Math.Max(Math.Max(Model.Scale.X, Model.Scale.Y), Model.Scale.Z);
				return new BoundingSphere(Vector3.TransformCoordinate(Model.Attach.Bounds.Center.ToVector3(), m).ToVertex(), Model.Attach.Bounds.Radius * scale);
			}
		}

		public DeathZoneItem(EditorItemSelection selectionManager)
			: base(selectionManager)
		{
			Model = new NJS_OBJECT();
			ImportModel();

			rotateZYX = Model.RotateZYX;
			GetHandleMatrix();
		}

		public DeathZoneItem(NJS_OBJECT model, SA1CharacterFlags flags, EditorItemSelection selectionManager)
			: base(selectionManager)
		{
			Model = model;
			if (model.Attach is BasicAttach)
			{
				BasicAttach attach = (BasicAttach)model.Attach;
				if (attach.Material.Count == 0) attach.Material.Add(new NJS_MATERIAL());
				attach.Material[0].DiffuseColor = System.Drawing.Color.FromArgb(96, 255, 0, 0);
				attach.Material[0].Flags = 0x96102400;
			}
			model.ProcessVertexData();
			Flags = flags;
			
			Mesh = Model.Attach.CreateD3DMesh();

			rotateZYX = Model.RotateZYX;
			GetHandleMatrix();
		}

		public override Vertex Position { get { return Model.Position; } set { Model.Position = value; } }

		public override Rotation Rotation { get { return Model.Rotation; } set { Model.Rotation = value; } }

		protected override void GetHandleMatrix()
		{
			position = Model.Position;
			rotation = Model.Rotation;
			base.GetHandleMatrix();
		}

		public override HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View)
		{
			return Model.CheckHit(Near, Far, Viewport, Projection, View, Mesh);
		}

		public override List<RenderInfo> Render(Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (LevelData.Textures != null && LevelData.leveltexs != null && LevelData.Textures.Count > 0)
			{
				result.AddRange(Model.DrawModel(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, LevelData.Textures[LevelData.leveltexs], Mesh, true));
			}
			else
			{
				result.AddRange(Model.DrawModel(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, Mesh, true));
			}
			if (Selected)
				result.AddRange(Model.DrawModelInvert(transform, Mesh, true));
			return result;
		}

		public override void Paste()
		{
			if (LevelData.DeathZones != null)
				LevelData.DeathZones.Add(this);
		}

		protected override void DeleteInternal(EditorItemSelection selectionManager)
		{
			LevelData.DeathZones.Remove(this);
		}

		[Browsable(true)]
		[DisplayName("Import Model")]
		public void ImportModel()
		{
			OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "sa1mdl", Filter = "Model Files|*.sa1mdl;*.obj;*.objf", RestoreDirectory = true };
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				switch (Path.GetExtension(dlg.FileName).ToLowerInvariant())
				{
					case ".obj":
					case ".fbx":
					case ".dae":
					case ".objf":
						Assimp.AssimpContext context = new Assimp.AssimpContext();
						context.SetConfig(new Assimp.Configs.FBXPreservePivotsConfig(false));
						Assimp.Scene scene = context.ImportFile(dlg.FileName, Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.JoinIdenticalVertices | Assimp.PostProcessSteps.FlipUVs);
						NJS_OBJECT newmodel = SAEditorCommon.Import.AssimpStuff.AssimpImport(scene, scene.RootNode, ModelFormat.BasicDX, LevelData.TextureBitmaps[LevelData.leveltexs].Select(a => a.Name).ToArray(), true);
						Model.Attach = newmodel.Attach;
						Model.ProcessVertexData();
						Mesh = Model.Attach.CreateD3DMesh();
						break;
					case ".sa1mdl":
						ModelFile mf = new ModelFile(dlg.FileName);
						Model.Attach = mf.Model.Attach;
						Model.ProcessVertexData();
						Mesh = Model.Attach.CreateD3DMesh();
						break;
				}
			}
		}

		[Browsable(true)]
		[DisplayName("Export Model")]
		public void ExportModel()
		{
			using (System.Windows.Forms.SaveFileDialog a = new System.Windows.Forms.SaveFileDialog
			{
				DefaultExt = "dae",
				Filter = "SAModel Files|*.sa1mdl|Collada|*.dae|Wavefront|*.obj"
			})
			{
				if (a.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					string ftype = "collada";
					switch (System.IO.Path.GetExtension(a.FileName).ToLowerInvariant())
					{
						case ".sa1mdl":
							ModelFile.CreateFile(a.FileName, Model, null, null, null, null, Model.GetModelFormat());
							return;
						case ".fbx":
							ftype = "fbx";
							break;
						case ".obj":
							ftype = "obj";
							break;
					}
					Assimp.AssimpContext context = new Assimp.AssimpContext();
					Assimp.Scene scene = new Assimp.Scene();
					scene.Materials.Add(new Assimp.Material());
					Assimp.Node n = new Assimp.Node();
					n.Name = "RootNode";
					scene.RootNode = n;
					string rootPath = System.IO.Path.GetDirectoryName(a.FileName);
					SAEditorCommon.Import.AssimpStuff.AssimpExport(Model, scene, Matrix.Identity, null, scene.RootNode);
					context.ExportFile(scene, a.FileName, ftype, Assimp.PostProcessSteps.ValidateDataStructure | Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.FlipUVs);//
				}
			}
		}
		[Category("Common"), Description("All characters flags in one field.")]
		public SA1CharacterFlags Flags { get; set; }

		[Browsable(false)]
		public bool Visible
		{
			get
			{
				switch (LevelData.Character)
				{
					case 0:
						return (Flags & SA1CharacterFlags.Sonic) == SA1CharacterFlags.Sonic;
					case 1:
						return (Flags & SA1CharacterFlags.Tails) == SA1CharacterFlags.Tails;
					case 2:
						return (Flags & SA1CharacterFlags.Knuckles) == SA1CharacterFlags.Knuckles;
					case 3:
						return (Flags & SA1CharacterFlags.Amy) == SA1CharacterFlags.Amy;
					case 4:
						return (Flags & SA1CharacterFlags.Gamma) == SA1CharacterFlags.Gamma;
					case 5:
						return (Flags & SA1CharacterFlags.Big) == SA1CharacterFlags.Big;
				}
				return false;
			}
		}
		[Category("Flags")]
		public bool Sonic
		{
			get
			{
				return (Flags & SA1CharacterFlags.Sonic) == SA1CharacterFlags.Sonic;
			}
			set
			{
				Flags = (Flags & ~SA1CharacterFlags.Sonic) | (value ? SA1CharacterFlags.Sonic : 0);
			}
		}
		[Category("Flags")]
		public bool Tails
		{
			get
			{
				return (Flags & SA1CharacterFlags.Tails) == SA1CharacterFlags.Tails;
			}
			set
			{
				Flags = (Flags & ~SA1CharacterFlags.Tails) | (value ? SA1CharacterFlags.Tails : 0);
			}
		}
		[Category("Flags")]
		public bool Knuckles
		{
			get
			{
				return (Flags & SA1CharacterFlags.Knuckles) == SA1CharacterFlags.Knuckles;
			}
			set
			{
				Flags = (Flags & ~SA1CharacterFlags.Knuckles) | (value ? SA1CharacterFlags.Knuckles : 0);
			}
		}
		[Category("Flags")]
		public bool Amy
		{
			get
			{
				return (Flags & SA1CharacterFlags.Amy) == SA1CharacterFlags.Amy;
			}
			set
			{
				Flags = (Flags & ~SA1CharacterFlags.Amy) | (value ? SA1CharacterFlags.Amy : 0);
			}
		}
		[Category("Flags")]
		public bool Gamma
		{
			get
			{
				return (Flags & SA1CharacterFlags.Gamma) == SA1CharacterFlags.Gamma;
			}
			set
			{
				Flags = (Flags & ~SA1CharacterFlags.Gamma) | (value ? SA1CharacterFlags.Gamma : 0);
			}
		}
		[Category("Flags")]
		public bool Big
		{
			get
			{
				return (Flags & SA1CharacterFlags.Big) == SA1CharacterFlags.Big;
			}
			set
			{
				Flags = (Flags & ~SA1CharacterFlags.Big) | (value ? SA1CharacterFlags.Big : 0);
			}
		}

		public DeathZoneFlags Save(string path, int i, string filename)
		{
			ModelFile.CreateFile(Path.Combine(path, filename), Model, null, null, LevelData.LevelName + " Death Zone " + i.ToString(NumberFormatInfo.InvariantInfo), null, ModelFormat.Basic);
			return new DeathZoneFlags() { Flags = Flags, Filename = filename };
		}

		public Vertex GetScale()
		{
			return Model.Scale;
		}

		public void SetScale(Vertex scale)
		{
			Model.Scale = scale;
		}
	}
}
