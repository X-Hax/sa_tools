using System;
using System.Collections.Generic;
using System.ComponentModel;
using SharpDX;
using SharpDX.Direct3D9;
using SAModel.Direct3D;
using SAModel.Direct3D.TextureSystem;
using SAModel.SAEditorCommon.UI;
using Mesh = SAModel.Direct3D.Mesh;
using System.IO;
using Newtonsoft.Json;

namespace SAModel.SAEditorCommon.DataTypes
{
	[Serializable]
	public class LevelAnim : Item
	{
		private BoundingSphere bounds;
		public override BoundingSphere Bounds { get { return bounds; } }
		private GeoAnimData GeoAnim { get; set; }
		[Browsable(false)]
		public GeoAnimData GeoAnimationData { get { return GeoAnim; } }
		private int index = 0;
		[NonSerialized]
		private Mesh[] meshes;
		[Browsable(false)]
		public Mesh[] Meshes { get { return meshes; } set { meshes = value; } }
		/// <summary>
		/// Creates a LevelAnim from an existing GeoAnim data.
		/// </summary>
		public LevelAnim(GeoAnimData geoanim, int index, EditorItemSelection selectionManager)
			: base(selectionManager)
		{
			GeoAnim = geoanim;
			this.index = index;
			GeoAnim.Model.ProcessVertexData();
			NJS_OBJECT[] models = GeoAnim.Model.GetObjects();
			Meshes = new Mesh[models.Length];
			for (int i = 0; i < models.Length; i++)
				if (models[i].Attach != null)
					try { Meshes[i] = models[i].Attach.CreateD3DMesh(); }
					catch { }
			CalculateBounds();
			GetHandleMatrix();
		}

		[ReadOnly(true)]
		[Category("Common")]
		public int Index
		{
			get
			{
				return index;
			}
		}

		[Category("Common")]
		public override Vertex Position { get { return GeoAnim.Model.Position; } set { GeoAnim.Model.Position = value; CalculateBounds(); GetHandleMatrix(); } }
		[Category("Common")]
		public override Rotation Rotation { get { return GeoAnim.Model.Rotation; } set { GeoAnim.Model.Rotation = value; GetHandleMatrix(); } }
		public override HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View)
		{
			if (GeoAnim.Animation != null)
				return GeoAnim.Model.CheckHitAnimated(Near, Far, Viewport, Projection, View, new MatrixStack(), Meshes, GeoAnim.Animation, GeoAnim.AnimationFrame);
			else
				return GeoAnim.Model.CheckHit(Near, Far, Viewport, Projection, View, new MatrixStack(), Meshes);
		}


		[Category("Animation"), ParenthesizePropertyName(true)]
		public float Frame
		{
			get
			{
				return GeoAnim.AnimationFrame;
			}
			set
			{
				GeoAnim.AnimationFrame = value;
			}
		}

		[Category("Animation")]
		public float Speed
		{
			get
			{
				return GeoAnim.AnimationSpeed;
			}
			set
			{
				GeoAnim.AnimationSpeed = value;
			}
		}

		[Category("Animation")]
		public float MaxFrame
		{
			get
			{
				return GeoAnim.MaxFrame;
			}
			set
			{
				GeoAnim.MaxFrame = value;
			}
		}

		[Category("Labels")]
		public string ActionName
		{
			get
			{
				return GeoAnim.ActionName;
			}
			set
			{
				GeoAnim.ActionName = value;
			}
		}

		[Category("Labels")]
		public string ObjectName
		{
			get
			{
				return GeoAnim.Model.Name;
			}
			set
			{
				GeoAnim.Model.Name = value;
			}
		}

		[Category("Labels")]
		public string MotionName
		{
			get
			{
				return GeoAnim.Animation.Name;
			}
			set
			{
				GeoAnim.Animation.Name = value;
			}
		}

		public override List<RenderInfo> Render(Device dev, EditorCamera camera, MatrixStack transform)
		{
			if (!camera.SphereInFrustum(bounds)) return EmptyRenderInfo;

			List<RenderInfo> result = new List<RenderInfo>();
			if (!string.IsNullOrEmpty(LevelData.leveltexs) && LevelData.Textures.Count > 0)
				result.AddRange(GeoAnim.Model.DrawModelTreeAnimated(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, LevelData.Textures[LevelData.leveltexs], Meshes, GeoAnim.Animation, GeoAnim.AnimationFrame, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			else
				result.AddRange(GeoAnim.Model.DrawModelTreeAnimated(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, Meshes, GeoAnim.Animation, GeoAnim.AnimationFrame, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (Selected)
				result.AddRange(GeoAnim.Model.DrawModelTreeAnimatedInvert(transform, Meshes, GeoAnim.Animation, GeoAnim.AnimationFrame));
			return result;
		}

		public override void Paste()
		{
			LevelData.geo.Anim.Add(GeoAnim);

			LevelData.AddLevelAnim(this);
		}

		protected override void DeleteInternal(EditorItemSelection selectionManager)
		{
			LevelData.geo.Anim.Remove(GeoAnim);

			LevelData.RemoveLevelAnim(this);
		}

		public void RegenerateMesh()
		{ }

		private void CalculateBounds()
		{
			MatrixStack transform = new MatrixStack();
			bounds = SETEditing.ObjectHelper.GetModelBounds(GeoAnim.Model, transform, 1.0f);
		}

		[Browsable(true)]
		[DisplayName("Export Model")]
		public void ExportModel()
		{
			string defaultex;

			switch (GeoAnim.Model.GetModelFormat())
			{
				case ModelFormat.Chunk:
					defaultex = ".sa2mdl";
					break;
				case ModelFormat.GC:
					defaultex = ".sa2bmdl";
					break;
				case ModelFormat.Basic:
				case ModelFormat.BasicDX:
				default:
					defaultex = ".sa1mdl";
					break;
			}

			using (System.Windows.Forms.SaveFileDialog a = new System.Windows.Forms.SaveFileDialog
			{
				FileName = ActionName + defaultex,
				Filter = "SAModel Files|*.sa?mdl|Collada|*.dae|Wavefront|*.obj"
			})
			{
				if (a.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					string ftype = "collada";
					switch (System.IO.Path.GetExtension(a.FileName).ToLowerInvariant())
					{
						case ".sa1mdl":
						case ".sa2mdl":
						case ".sa2bmdl":
							ModelFile.CreateFile(a.FileName, GeoAnim.Model, null, null, null, null, GeoAnim.Model.GetModelFormat());
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
					List<string> texturePaths = new List<string>();
					int numSteps = 0;
					if (LevelData.TextureBitmaps != null && LevelData.TextureBitmaps.Count > 0)
					{
						numSteps = LevelData.TextureBitmaps[LevelData.leveltexs].Length;
					}
					for (int i = 0; i < numSteps; i++)
					{
						BMPInfo bmp = LevelData.TextureBitmaps[LevelData.leveltexs][i];
						texturePaths.Add(System.IO.Path.Combine(rootPath, bmp.Name + ".png"));
						bmp.Image.Save(System.IO.Path.Combine(rootPath, bmp.Name + ".png"));
					}
					SAEditorCommon.Import.AssimpStuff.AssimpExport(GeoAnim.Model, scene, Matrix.Identity, texturePaths.Count > 0 ? texturePaths.ToArray() : null, scene.RootNode);
					context.ExportFile(scene, a.FileName, ftype, Assimp.PostProcessSteps.ValidateDataStructure | Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.FlipUVs);//
				}
			}
		}

		[Browsable(true)]
		[DisplayName("Export Animation")]
		public void ExportAnimation()
		{
			using (System.Windows.Forms.SaveFileDialog a = new System.Windows.Forms.SaveFileDialog
			{
				FileName = ActionName + ".saanim",
				Filter = "SAModel Animations|*.saanim|JSON|*.json|C structs|*.c"
			})
			{
				if (a.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					switch (System.IO.Path.GetExtension(a.FileName).ToLowerInvariant())
					{
						case ".c":
						case ".txt":
							using (System.IO.StreamWriter sw = System.IO.File.CreateText(a.FileName))
							{
								sw.WriteLine("/* NINJA Motion");
								sw.WriteLine(" * ");
								sw.WriteLine(" * Generated by DataToolbox");
								sw.WriteLine(" * ");
								sw.WriteLine(" */");
								sw.WriteLine();
								GeoAnim.Animation.ToStructVariables(sw);
								sw.Flush();
								sw.Close();
							}
							break;
						case ".json":
							Newtonsoft.Json.JsonSerializer js = new Newtonsoft.Json.JsonSerializer() { Culture = System.Globalization.CultureInfo.InvariantCulture };
							using (TextWriter tw = File.CreateText(a.FileName))
							using (JsonTextWriter jtw = new JsonTextWriter(tw) { Formatting = Formatting.Indented })
								js.Serialize(jtw, GeoAnim.Animation);
							break;
						case ".saanim":
						default:
							GeoAnim.Animation.Save(a.FileName);
							break;
					}
				}
			}
		}

		protected override void GetHandleMatrix()
		{
			position = Position;
			rotation = Rotation;
			base.GetHandleMatrix();
		}

		public void Save() { }

		// Form property update event method
		void pw_FormUpdated(object sender, EventArgs e)
		{
			LevelData.InvalidateRenderState();
		}
	}
}