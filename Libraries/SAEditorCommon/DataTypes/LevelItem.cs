﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D9;
using SAModel.Direct3D;
using SAModel.Direct3D.TextureSystem;
using SAModel.SAEditorCommon.UI;
using Mesh = SAModel.Direct3D.Mesh;
using System.IO;
using SplitTools;
using SAModel.SAEditorCommon.SETEditing;

namespace SAModel.SAEditorCommon.DataTypes
{
	[Serializable]
	public class LevelItem : Item, IScaleable
	{
		private COL COL { get; set; }
		[Browsable(false)]
		public COL CollisionData { get { return COL; } }
		[NonSerialized]
		private Mesh mesh;
		[Browsable(false)]
		public Mesh Mesh { get { return mesh; } set { mesh = value; } }
		[Browsable(false)]
		private NJS_TEXLIST LevelTexlist;
		private Texture[] LevelTextures;
		private Texture[] SecondaryLevelTextures;
		[Browsable(false)]
		private bool IsSecondary;
		[Browsable(false)]
		private List<string> SecondaryTexPacks = [];

		private int index = 0;

		/// <summary>
		/// Creates a Levelitem from an external file.
		/// </summary>
		/// <param name="dev">Current Direct3D device.</param>
		/// <param name="filePath">location of the file to use.</param>
		/// <param name="position">Position to place the resulting model (worldspace).</param>
		/// <param name="rotation">Rotation to apply to the model.</param>
		public LevelItem(string filePath, Vertex position, Rotation rotation, int index, EditorItemSelection selectionManager, bool legacyImport = false, string leveltexlist = null)
			: base(selectionManager)
		{
			string nopathfname = Path.GetFileName(filePath);
			this.index = index;
			COL = new COL
			{
				Model = new NJS_OBJECT
				{
					Position = position,
					Rotation = rotation
				}
			};
			ImportModel(filePath, legacyImport);
			COL.CalculateBounds();
			LevelFileName = nopathfname;
			if (leveltexlist != null)
				LevelTexlist = NJS_TEXLIST.Load(leveltexlist);
			else
				LevelTexlist = null;
			Paste();

			GetHandleMatrix();
		}

		/// <summary>
		/// Creates a LevelItem from an existing COL data.
		/// </summary>
		/// <param name="col"></param>
		/// <param name="dev">Current Direct3d Device.</param>
		public LevelItem(COL col, int index, EditorItemSelection selectionManager, string filePath = null, string leveltexlist = null, List<string> secondtexs = null, bool secondary = false)
			: base(selectionManager)
		{
			this.index = index;
			COL = col;
			col.Model.ProcessVertexData();
			Mesh = col.Model.Attach.CreateD3DMesh();
			LevelFileName = Path.GetFileName(filePath);
			if (leveltexlist != null)
				LevelTexlist = NJS_TEXLIST.Load(leveltexlist);
			else
				LevelTexlist = null;
			IsSecondary = secondary;
			SecondaryTexPacks = secondtexs;

			GetHandleMatrix();
		}

		/// <summary>
		/// Creates a new instance of an existing item with the specified position and rotation.
		/// </summary>
		/// <param name="attach">Attach to use for this levelItem</param>
		/// <param name="position">Position in worldspace to place this LevelItem.</param>
		/// <param name="rotation">Rotation.</param>
		public LevelItem(Attach attach, Vertex position, Rotation rotation, int index, EditorItemSelection selectionManager, string filename = null, bool secondary = false)
			: base(selectionManager)
		{
			this.index = index;
			COL = new COL
			{
				Model = new NJS_OBJECT
				{
					Attach = attach,
					Position = position,
					Rotation = rotation
				}
			};
			Visible = true;
			Solid = true;
			COL.CalculateBounds();
			Mesh = COL.Model.Attach.CreateD3DMesh();
			Paste();
		}

		[Category("Common"), DisplayName("File Source"), ParenthesizePropertyName(true)]
		public string LevelFileName { get; set; }

		[Category("Common")]
		[Description("The label of the NJS_OBJECT of the current item.")]
		public string Name
		{
			get
			{
				return COL.Model.Name;
			}
			set
			{
				COL.Model.Name = value;
			}
		}

		[ReadOnly(true)]
		[Category("Common"), ParenthesizePropertyName(true)]
		[Description("This item's index in the landtable's model (COL) list.")]

		public int Index
		{
			get
			{
				return index;
			}
		}

		[Category("Common")]
		public override Vertex Position { get { return COL.Model.Position; } set { COL.Model.Position = value; GetHandleMatrix(); } }
		[Category("Common")]
		public override Rotation Rotation { get { return COL.Model.Rotation; } set { COL.Model.Rotation = value; GetHandleMatrix(); } }
		[Category("Common")]
		[DisplayName("Scale (Unused)")]
		[Description("This field is unused. SALVL will render the model with scaling, but the game will still render it as if the scale was 1,1,1. This field is sometimes present in SA1 Autodemo levels.")]
		public Vertex Scale { get { return COL.Model.Scale; } set { COL.Model.Scale = value; GetHandleMatrix(); } }
		[Category("Common")]
		[DisplayName("Bounds")]
		[Description("Stores the coordinates and radius of a sphere used to determine whether the item should be rendered. This field needs to be recalculated when the model is edited.")]
		public override BoundingSphere Bounds { get { return COL.Bounds; } }

		public override HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View)
		{
			return COL.Model.CheckHit(Near, Far, Viewport, Projection, View, Mesh);
		}

		public override List<RenderInfo> Render(Device dev, EditorCamera camera, MatrixStack transform)
		{
			if (!camera.SphereInFrustum(COL.Bounds)) return EmptyRenderInfo;
			if (LevelTextures == null && !string.IsNullOrEmpty(LevelData.leveltexs))
				LevelTextures = ObjectHelper.GetTextures(LevelData.leveltexs, LevelTexlist, dev);
			if (IsSecondary && SecondaryLevelTextures == null && SecondaryTexPacks != null)
			{ 
				SecondaryLevelTextures = ObjectHelper.GetTexturesMultiSource(SecondaryTexPacks, LevelTexlist, dev);
			}
				List<RenderInfo> result = new List<RenderInfo>();
			if (IsSecondary && SecondaryTexPacks != null)
			{
				if (SecondaryLevelTextures != null)
				{
					result.AddRange(COL.Model.DrawModel(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, SecondaryLevelTextures, Mesh, Visible, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				}
				else
				{
					result.AddRange(COL.Model.DrawModel(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, LevelData.Textures[LevelData.leveltexs], Mesh, Visible, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				}
			}
			else if (!string.IsNullOrEmpty(LevelData.leveltexs) && LevelData.Textures.Count > 0 && LevelData.Textures.ContainsKey(LevelData.leveltexs) && !EditorOptions.DisableTextures)
			{
				if (LevelTexlist != null)
				{
					result.AddRange(COL.Model.DrawModel(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, LevelTextures, Mesh, Visible, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				}
				else
				{
					result.AddRange(COL.Model.DrawModel(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, LevelData.Textures[LevelData.leveltexs], Mesh, Visible, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				}
			}
			else
				result.AddRange(COL.Model.DrawModel(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, Mesh, Visible, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (Selected)
				result.AddRange(COL.Model.DrawModelInvert(transform, Mesh, Visible));
			return result;
		}

		public override void Paste()
		{
			LevelData.geo.COL.Add(COL);

			LevelData.AddLevelItem(this);
		}

		protected override void DeleteInternal(EditorItemSelection selectionManager)
		{
			LevelData.geo.COL.Remove(COL);

			LevelData.RemoveLevelItem(this);
		}

		public void RegenerateMesh() => mesh = COL.Model.Attach.CreateD3DMesh();

        public void ImportModel(string filePath, bool legacyImport = false)
		{
            NJS_OBJECT newmodel;
            // Old OBJ import (with vcolor face) for NodeTable and legacy import
            if (legacyImport)
            {
                newmodel = new NJS_OBJECT
                {
                    Attach = SAModel.Direct3D.Extensions.obj2nj(filePath, LevelData.TextureBitmaps != null ? LevelData.TextureBitmaps[LevelData.leveltexs].Select(a => a.Name).ToArray() : null),
				};
				COL.Model.Attach = newmodel.Attach;
                COL.Model.ProcessVertexData();
                Visible = true;
                Solid = true;
                mesh = COL.Model.Attach.CreateD3DMesh();
                return;
            }
			Assimp.AssimpContext context = new Assimp.AssimpContext();
			context.SetConfig(new Assimp.Configs.FBXPreservePivotsConfig(false));
			Assimp.Scene scene = context.ImportFile(filePath, Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.JoinIdenticalVertices | Assimp.PostProcessSteps.FlipUVs);
			newmodel = SAEditorCommon.Import.AssimpStuff.AssimpImport(scene, scene.RootNode, ModelFormat.BasicDX, LevelData.TextureBitmaps[LevelData.leveltexs].Select(a => a.Name).ToArray(), true);
			COL.Model.Attach = newmodel.Attach;
			COL.Model.ProcessVertexData();
			Visible = true;
			Solid = true;
			mesh = COL.Model.Attach.CreateD3DMesh();
		}

		[Browsable(true)]
		[DisplayName("Edit Materials")]
		public void EditMaterials()
		{
			BMPInfo[] textures;
			if (LevelData.leveltexs == null || LevelData.TextureBitmaps.Count == 0) textures = null; else textures = LevelData.TextureBitmaps[LevelData.leveltexs];
			switch (COL.Model.Attach)
			{
				case BasicAttach attach:
					{
						using (MaterialEditor pw = new MaterialEditor(attach.Material, textures, attach.MaterialName))
						{
							pw.FormUpdated += pw_FormUpdated;
							pw.ShowDialog();
						}
					}
					break;
				case ChunkAttach:
					{
						using (MaterialEditor pw = new MaterialEditor(COL.Model.Attach.MeshInfo.Select(a => a.Material).ToList(), textures))
						{
							pw.FormUpdated += pw_FormUpdated;
							pw.ShowDialog();
						}
					}
					break;
				case GC.GCAttach:
					{
						using (GCMaterialEditor pw = new GCMaterialEditor(COL.Model.Attach.MeshInfo.Select(a => a.Material).ToList(), textures))
						{
							pw.FormUpdated += pw_FormUpdated;
							pw.ShowDialog();
						}
					}
					break;
			}
		}

		[Browsable(true)]
		[DisplayName("Edit Model")]
		public void EditModel()
		{
			BMPInfo[] textures;
			if (LevelData.leveltexs == null || LevelData.TextureBitmaps.Count == 0) textures = null; else textures = LevelData.TextureBitmaps[LevelData.leveltexs];
			switch (COL.Model.Attach)
			{
				case BasicAttach:
				ModelDataEditor me = new ModelDataEditor(COL.Model);
				if (me.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					COL.Model = me.editedHierarchy.Clone();
					COL.Model.ProcessVertexData();
					mesh = COL.Model.Attach.CreateD3DMesh();
					COL.Model.Attach.CalculateBounds();
					LevelData.InvalidateRenderState();
				}
				break;
				case ChunkAttach:
					ChunkModelDataEditor ce = new ChunkModelDataEditor(COL.Model, textures);
					if (ce.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						COL.Model = ce.editedHierarchy.Clone();
						COL.Model.ProcessVertexData();
						mesh = COL.Model.Attach.CreateD3DMesh();
						COL.Model.Attach.CalculateBounds();
						LevelData.InvalidateRenderState();
					}
					break;
				case GC.GCAttach:
					GCModelDataEditor ge = new GCModelDataEditor(COL.Model, textures);
					if (ge.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						COL.Model = ge.editedHierarchy.Clone();
						COL.Model.ProcessVertexData();
						mesh = COL.Model.Attach.CreateD3DMesh();
						COL.Model.Attach.CalculateBounds();
						LevelData.InvalidateRenderState();
					}
					break;
			}
		}

		[Browsable(true)]
		[DisplayName("Calculate Bounds")]
		public void CalculateBounds()
		{
			COL.CalculateBounds();
			COL.Model.Attach.CalculateBounds();
			LevelData.InvalidateRenderState();
		}

		[Browsable(true)]
		[DisplayName("Export Model")]
		public void ExportModel()
		{
            string defaultex;

            switch (COL.Model.GetModelFormat())
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
                FileName = Name + defaultex,
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
                            ModelFile.CreateFile(a.FileName, COL.Model, null, null, null, null, COL.Model.GetModelFormat());
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
					SAEditorCommon.Import.AssimpStuff.AssimpExport(COL.Model, scene, Matrix.Identity, texturePaths.Count > 0 ? texturePaths.ToArray() : null, scene.RootNode);
					context.ExportFile(scene, a.FileName, ftype, Assimp.PostProcessSteps.ValidateDataStructure | Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.FlipUVs);//
				}
			}
		}

		[Category("Common"), DisplayName("Flags"), Description("Surface flags that configure collision and transparency sorting.")]
		public string Flags
		{
			get
			{
				return COL.Flags.ToString("X8");
			}
			set
			{
				COL.Flags = int.Parse(value, NumberStyles.HexNumber);
			}
		}
				
		[Category("Miscellaneous"), DisplayName("Y Width"), Description("A field that can store a floating point value. Only used in some SA1 Autodemo levels.")]
		public string WidthY
		{
			get
			{
				return COL.WidthY.ToC(true);
			}
			set
			{
				COL.WidthY = float.Parse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
			}
		}

		[Category("Miscellaneous"), DisplayName("Z Width"), Description("A field that can store a floating point value. Only used in some SA1 Autodemo levels.")]
		public string WidthZ
		{
			get
			{
				return COL.WidthZ.ToC(true);
			}
			set
			{
				COL.WidthZ = float.Parse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
			}
		}

		[Category("Miscellaneous"), DisplayName("Block Bits"), Description("Controls how the item pops in. Set to 0 to make it pop in gradually. Used for texlist pointers in some SA1 Autodemo levels.")]
		public string BlockBits
		{
			get
			{
				return COL.BlockBits.ToString("X8");
			}
			set
			{
				COL.BlockBits = uint.Parse(value, NumberStyles.HexNumber);
			}
		}

		protected override void GetHandleMatrix()
		{
			position = Position;
			rotation = Rotation;
			scale = Scale;
			base.GetHandleMatrix();
		}

		#region Surface Flag Accessors
		[Category("Flags")]
		public bool Solid
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.Solid) == SA1SurfaceFlags.Solid; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.Solid) | (value ? SA1SurfaceFlags.Solid : 0); }
		}
		[Category("Flags"), Description("Water collision and transparency sorting.")]
		public bool Water
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.Water) == SA1SurfaceFlags.Water; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.Water) | (value ? SA1SurfaceFlags.Water : 0); }
		}
		[Category("Flags")]
		public bool NoFriction
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.NoFriction) == SA1SurfaceFlags.NoFriction; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.NoFriction) | (value ? SA1SurfaceFlags.NoFriction : 0); }
		}
		[Category("Flags")]
		public bool NoAcceleration
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.NoAcceleration) == SA1SurfaceFlags.NoAcceleration; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.NoAcceleration) | (value ? SA1SurfaceFlags.NoAcceleration : 0); }
		}
		[Category("Flags")]
		public bool CannotLand
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.CannotLand) == SA1SurfaceFlags.CannotLand; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.CannotLand) | (value ? SA1SurfaceFlags.CannotLand : 0); }
		}
		[Category("Flags")]
		public bool IncreasedAcceleration
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.IncreasedAcceleration) == SA1SurfaceFlags.IncreasedAcceleration; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.IncreasedAcceleration) | (value ? SA1SurfaceFlags.IncreasedAcceleration : 0); }
		}
		[Category("Flags")]
		public bool Diggable
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.Diggable) == SA1SurfaceFlags.Diggable; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.Diggable) | (value ? SA1SurfaceFlags.Diggable : 0); }
		}
		[Category("Flags")]
		public bool Unclimbable
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.Unclimbable) == SA1SurfaceFlags.Unclimbable; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.Unclimbable) | (value ? SA1SurfaceFlags.Unclimbable : 0); }
		}
		[Category("Flags")]
		public bool Hurt
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.Hurt) == SA1SurfaceFlags.Hurt; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.Hurt) | (value ? SA1SurfaceFlags.Hurt : 0); }
		}
		[Category("Flags")]
		public bool Footprints
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.Footprints) == SA1SurfaceFlags.Footprints; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.Footprints) | (value ? SA1SurfaceFlags.Footprints : 0); }
		}
		[Category("Flags")]
		public bool Visible
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.Visible) == SA1SurfaceFlags.Visible; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.Visible) | (value ? SA1SurfaceFlags.Visible : 0); }
		}
		[Category("Flags")]
		public bool LowAcceleration
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.LowAcceleration) == SA1SurfaceFlags.LowAcceleration; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.LowAcceleration) | (value ? SA1SurfaceFlags.LowAcceleration : 0); }
		}
		[Category("Flags"), Description("Use skybox draw distance instead of level draw distance.")]
		public bool UseSkyDrawDistance
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.UseSkyDrawDistance) == SA1SurfaceFlags.UseSkyDrawDistance; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.UseSkyDrawDistance) | (value ? SA1SurfaceFlags.UseSkyDrawDistance : 0); }
		}
		[Category("Flags")]
		public bool Stairs
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.Stairs) == SA1SurfaceFlags.Stairs; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.Stairs) | (value ? SA1SurfaceFlags.Stairs : 0); }
		}
		[Category("Flags"), Description("Put the model earlier in the draw queue for transparency sorting.")]
		public bool LowDepth
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.LowDepth) == SA1SurfaceFlags.LowDepth; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.LowDepth) | (value ? SA1SurfaceFlags.LowDepth : 0); }
		}
		[Category("Flags"), Description("Water collision without transparency sorting.")]
		public bool WaterCollision
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.WaterCollision) == SA1SurfaceFlags.WaterCollision; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.WaterCollision) | (value ? SA1SurfaceFlags.WaterCollision : 0); }
		}
		[Category("Flags")]
		public bool RotateByGravity
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.RotateByGravity) == SA1SurfaceFlags.RotateByGravity; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.RotateByGravity) | (value ? SA1SurfaceFlags.RotateByGravity : 0); }
		}
		[Category("Flags"), Description("Disable Z Write when rendering the model.")]
		public bool NoZWrite
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.NoZWrite) == SA1SurfaceFlags.NoZWrite; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.NoZWrite) | (value ? SA1SurfaceFlags.NoZWrite : 0); }
		}
		[Category("Flags"), Description("Use per-mesh transparency sorting when rendering this model.")]
		public bool DrawByMesh
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.DrawByMesh) == SA1SurfaceFlags.DrawByMesh; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.DrawByMesh) | (value ? SA1SurfaceFlags.DrawByMesh : 0); }
		}
		[Category("Flags"), Description("Enable model data manipulation by disabling meshset buffers. Useful for UV animations.")]
		public bool EnableManipulation
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.EnableManipulation) == SA1SurfaceFlags.EnableManipulation; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.EnableManipulation) | (value ? SA1SurfaceFlags.EnableManipulation : 0); }
		}
		[Category("Flags")]
		public bool UseRotation
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.UseRotation) == SA1SurfaceFlags.UseRotation; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.UseRotation) | (value ? SA1SurfaceFlags.UseRotation : 0); }
		}
		[Category("Flags"), Description("Force alpha sorting; Disable Z Write when used together with Water; Force disable Z write in all levels except Lost World 2.")]
		public bool Waterfall
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.Waterfall) == SA1SurfaceFlags.Waterfall; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.Waterfall) | (value ? SA1SurfaceFlags.Waterfall : 0); }
		}
		[Category("Flags")]
		public bool DynamicCollision
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.DynamicCollision) == SA1SurfaceFlags.DynamicCollision; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.DynamicCollision) | (value ? SA1SurfaceFlags.DynamicCollision : 0); }
		}
		[Category("Flags")]
		public bool Accelerate
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.Accelerate) == SA1SurfaceFlags.Accelerate; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.Accelerate) | (value ? SA1SurfaceFlags.Accelerate : 0); }
		}

		#endregion

		#region SA2 Surface Flag Accessors
		[Category("SA2 Flags"), DisplayName("Solid")]
		public bool SA2Solid
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.Solid) == SA2SurfaceFlags.Solid; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.Solid) | (value ? SA2SurfaceFlags.Solid : 0); }
		}
		[Category("SA2 Flags"), DisplayName("Water")]
		public bool SA2Water
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.Water) == SA2SurfaceFlags.Water; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.Water) | (value ? SA2SurfaceFlags.Water : 0); }
		}
		[Category("SA2 Flags"), DisplayName("Low Friction")]
		public bool SA2LowFric
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.LowFriction) == SA2SurfaceFlags.LowFriction; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.LowFriction) | (value ? SA2SurfaceFlags.LowFriction : 0); }
		}
		[Category("SA2 Flags"), DisplayName("High Friction")]
		public bool SA2HiFric
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.HighFriction) == SA2SurfaceFlags.HighFriction; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.HighFriction) | (value ? SA2SurfaceFlags.HighFriction : 0); }
		}
		[Category("SA2 Flags"), DisplayName("Medium Friction")]
		public bool SA2MediumFric
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.MediumFriction) == SA2SurfaceFlags.MediumFriction; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.MediumFriction) | (value ? SA2SurfaceFlags.MediumFriction : 0); }
		}
		[Category("SA2 Flags"), DisplayName("Diggable")]
		public bool SA2Diggable
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.Diggable) == SA2SurfaceFlags.Diggable; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.Diggable) | (value ? SA2SurfaceFlags.Diggable : 0); }
		}
		[Category("SA2 Flags"), DisplayName("Unclimbable")]
		public bool SA2Unclimbable
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.Unclimbable) == SA2SurfaceFlags.Unclimbable; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.Unclimbable) | (value ? SA2SurfaceFlags.Unclimbable : 0); }
		}
		[Category("SA2 Flags"), DisplayName("Stairs")]
		public bool SA2Stairs
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.Stairs) == SA2SurfaceFlags.Stairs; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.Stairs) | (value ? SA2SurfaceFlags.Stairs : 0); }
		}
		[Category("SA2 Flags"), DisplayName("Hurt")]
		public bool SA2Hurt
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.Hurt) == SA2SurfaceFlags.Hurt; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.Hurt) | (value ? SA2SurfaceFlags.Hurt : 0); }
		}
		[Category("SA2 Flags"), DisplayName("Footsteps"), Description("This flag draws footstep textures on surfaces in levels that utilize the feature.")]
		public bool SA2Footsteps
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.Footsteps) == SA2SurfaceFlags.Footsteps; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.Footsteps) | (value ? SA2SurfaceFlags.Footsteps : 0); }
		}
		[Category("SA2 Flags"), DisplayName("Cannot Land")]
		public bool SA2CannotLand
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.CannotLand) == SA2SurfaceFlags.CannotLand; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.CannotLand) | (value ? SA2SurfaceFlags.CannotLand : 0); }
		}
		[Category("SA2 Flags"), DisplayName("Slow Water"), Description("Any surface with this flag greatly reduces player speed if it's underwater.")]
		public bool SA2WaterSlow
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.WaterSlowMove) == SA2SurfaceFlags.WaterSlowMove; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.WaterSlowMove) | (value ? SA2SurfaceFlags.WaterSlowMove : 0); }
		}
		[Category("SA2 Flags"), DisplayName("No Shadows"), Description("Drop shadows will not render over surfaces with this flag.")]
		public bool SA2NoShadows
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.NoShadows) == SA2SurfaceFlags.NoShadows; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.NoShadows) | (value ? SA2SurfaceFlags.NoShadows : 0); }
		}
		[Category("SA2 Flags"), DisplayName("High Speed")]
		public bool SA2HighSpeed
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.IncreaseSpeed) == SA2SurfaceFlags.IncreaseSpeed; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.IncreaseSpeed) | (value ? SA2SurfaceFlags.IncreaseSpeed : 0); }
		}
		[Category("SA2 Flags"), DisplayName("High Acceleration")]
		public bool SA2HighAccel
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.IncreaseAccel) == SA2SurfaceFlags.IncreaseAccel; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.IncreaseAccel) | (value ? SA2SurfaceFlags.IncreaseAccel : 0); }
		}
		[Category("SA2 Flags"), DisplayName("No Fog/High Gravity"), Description("Visible surfaces with this flag will not have fog data applied to them. Collision surfaces will increase player gravity.")]
		public bool SA2NoFogHighGrav
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.NoFogHighGravity) == SA2SurfaceFlags.NoFogHighGravity; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.NoFogHighGravity) | (value ? SA2SurfaceFlags.NoFogHighGravity : 0); }
		}
		[Category("SA2 Flags"), DisplayName("Use Max Clip Distance"), Description("Surfaces with this flag will ignore the LandTable's Far Clipping setting and use the value stored in nj_clip instead.")]
		public bool SA2MaxClip
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.MaxClip) == SA2SurfaceFlags.MaxClip; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.MaxClip) | (value ? SA2SurfaceFlags.MaxClip : 0); }
		}
		[Category("SA2 Flags"), DisplayName("Simple Draw"), Description("Surfaces with this flag will use the Simple draw method. Setting this to False will set the surface to use the Easy draw method instead.")]
		public bool SA2SimpleDraw
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.SimpleDraw) == SA2SurfaceFlags.SimpleDraw; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.SimpleDraw) | (value ? SA2SurfaceFlags.SimpleDraw : 0); }
		}
		[Category("SA2 Flags"), DisplayName("Direct Draw"), Description("Surfaces with this flag will use the Direct draw method.")]
		public bool SA2DirectDraw
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.DirectDraw) == SA2SurfaceFlags.DirectDraw; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.DirectDraw) | (value ? SA2SurfaceFlags.DirectDraw : 0); }
		}
		[Category("SA2 Flags"), DisplayName("Do Not Compile"), Description("Surfaces with this flag will not compile model data, allowing for edits like UV manipulations to be applied.")]
		public bool SA2NoCompile
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.NoCompile) == SA2SurfaceFlags.NoCompile; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.NoCompile) | (value ? SA2SurfaceFlags.NoCompile : 0); }
		}
		[Category("SA2 Flags"), DisplayName("Dynamic Collision"), Description("Collision surfaces with this flag are capable of moving around.")]
		public bool SA2DynamicCollision
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.DynamicCollision) == SA2SurfaceFlags.DynamicCollision; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.DynamicCollision) | (value ? SA2SurfaceFlags.DynamicCollision : 0); }
		}
		[Category("SA2 Flags"), DisplayName("No Rotation Collision"), Description("Collision surfaces with this flag will ignore rotations.")]
		public bool SA2NoRotateCollision
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.NoRotateCollision) == SA2SurfaceFlags.NoRotateCollision; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.NoRotateCollision) | (value ? SA2SurfaceFlags.NoRotateCollision : 0); }
		}
		[Category("SA2 Flags"), DisplayName("Small Collision Radius"), Description("Collision surfaces with this flag will have a reduced radius. Enabling this flag and Tiny Collision Radius will shrink the radius even more.")]
		public bool SA2SmallCollision
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.SmallCollisionRad) == SA2SurfaceFlags.SmallCollisionRad; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.SmallCollisionRad) | (value ? SA2SurfaceFlags.SmallCollisionRad : 0); }
		}
		[Category("SA2 Flags"), DisplayName("Tiny Collision Radius"), Description("Collision surfaces with this flag will have a reduced radius. Enabling this flag and Small Collision Radius will shrink the radius even more.")]
		public bool SA2TinyCollision
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.TinyCollisionRad) == SA2SurfaceFlags.TinyCollisionRad; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.TinyCollisionRad) | (value ? SA2SurfaceFlags.TinyCollisionRad : 0); }
		}
		[Category("SA2 Flags"), DisplayName("Visible")]
		public bool SA2Visible
		{
			get { return (COL.SA2SurfaceFlags & SA2SurfaceFlags.Visible) == SA2SurfaceFlags.Visible; }
			set { COL.SA2SurfaceFlags = (COL.SA2SurfaceFlags & ~SA2SurfaceFlags.Visible) | (value ? SA2SurfaceFlags.Visible : 0); }
		}
		#endregion

		public void Save() { COL.CalculateBounds(); }

		// Form property update event method
		void pw_FormUpdated(object sender, EventArgs e)
		{
			COL.Model.ProcessVertexData();
			mesh = COL.Model.Attach.CreateD3DMesh();
			LevelData.InvalidateRenderState();
		}

		public Vertex GetScale()
		{
			return COL.Model.Scale;
		}

		public void SetScale(Vertex scale)
		{
			COL.Model.Scale = scale;
		}
	}
}
