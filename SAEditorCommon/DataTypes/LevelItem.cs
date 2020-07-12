using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D9;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.Direct3D.TextureSystem;
using SonicRetro.SAModel.SAEditorCommon.UI;
using Mesh = SonicRetro.SAModel.Direct3D.Mesh;

namespace SonicRetro.SAModel.SAEditorCommon.DataTypes
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

		private int index = 0;

		/// <summary>
		/// Creates a Levelitem from an external file.
		/// </summary>
		/// <param name="dev">Current Direct3D device.</param>
		/// <param name="filePath">location of the file to use.</param>
		/// <param name="position">Position to place the resulting model (worldspace).</param>
		/// <param name="rotation">Rotation to apply to the model.</param>
		public LevelItem(string filePath, Vertex position, Rotation rotation, int index, EditorItemSelection selectionManager)
			: base(selectionManager)
		{
			this.index = index;
			COL = new COL
			{
				Model = new NJS_OBJECT
				{
					Position = position,
					Rotation = rotation
				}
			};
			ImportModel(filePath);
			COL.CalculateBounds();
			Paste();

			GetHandleMatrix();
		}

		/// <summary>
		/// Creates a LevelItem from an existing COL data.
		/// </summary>
		/// <param name="col"></param>
		/// <param name="dev">Current Direct3d Device.</param>
		public LevelItem(COL col, int index, EditorItemSelection selectionManager)
			: base(selectionManager)
		{
			this.index = index;
			COL = col;
			col.Model.ProcessVertexData();
			Mesh = col.Model.Attach.CreateD3DMesh();

			GetHandleMatrix();
		}

		/// <summary>
		/// Creates a new instance of an existing item with the specified position and rotation.
		/// </summary>
		/// <param name="attach">Attach to use for this levelItem</param>
		/// <param name="position">Position in worldspace to place this LevelItem.</param>
		/// <param name="rotation">Rotation.</param>
		public LevelItem(Attach attach, Vertex position, Rotation rotation, int index, EditorItemSelection selectionManager)
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

		[ReadOnly(true)]
		[ParenthesizePropertyName(true)]
		public string Name
		{
			get
			{
				return COL.Model.Name;
			}
		}

		[ReadOnly(true)]
		[ParenthesizePropertyName(true)]
		public int Index
		{
			get
			{
				return index;
			}
		}

		public override Vertex Position { get { return COL.Model.Position; } set { COL.Model.Position = value; GetHandleMatrix(); } }
		public override Rotation Rotation { get { return COL.Model.Rotation; } set { COL.Model.Rotation = value; GetHandleMatrix(); } }
		public override BoundingSphere Bounds { get { return COL.Bounds; } }

		public override HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View)
		{
			return COL.Model.CheckHit(Near, Far, Viewport, Projection, View, Mesh);
		}

		public override List<RenderInfo> Render(Device dev, EditorCamera camera, MatrixStack transform)
		{
			if (!camera.SphereInFrustum(COL.Bounds)) return EmptyRenderInfo;

			List<RenderInfo> result = new List<RenderInfo>();
			if (!string.IsNullOrEmpty(LevelData.leveltexs) && LevelData.Textures.Count > 0)
				result.AddRange(COL.Model.DrawModel(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, LevelData.Textures[LevelData.leveltexs], Mesh, Visible));
			else
				result.AddRange(COL.Model.DrawModel(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, Mesh, Visible));
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

		public void ImportModel(string filePath)
		{
			COL.Model.Attach = Direct3D.Extensions.obj2nj(filePath, LevelData.TextureBitmaps[LevelData.leveltexs].Select(a => a.Name).ToArray());
			Visible = true;
			Solid = true;

			mesh = COL.Model.Attach.CreateD3DMesh();
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
				BMPInfo[] textures;
				if (LevelData.leveltexs == null || LevelData.TextureBitmaps.Count == 0) textures = null; else textures = LevelData.TextureBitmaps[LevelData.leveltexs];
				using (MaterialEditor pw = new MaterialEditor(((BasicAttach)COL.Model.Attach).Material, textures))
				{
					pw.FormUpdated += pw_FormUpdated;
					pw.ShowDialog();
				}
			}
		}

		[Browsable(true)]
		[DisplayName("Calculate Bounds")]
		public void CalculateBounds()
		{
			COL.CalculateBounds();
			COL.Model.Attach.CalculateBounds();
		}

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

		protected override void GetHandleMatrix()
		{
			position = Position;
			rotation = Rotation;
			base.GetHandleMatrix();
		}

		#region Surface Flag Accessors
		public bool Solid
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.Solid) == SA1SurfaceFlags.Solid; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.Solid) | (value ? SA1SurfaceFlags.Solid : 0); }
		}

		public bool Water
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.Water) == SA1SurfaceFlags.Water; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.Water) | (value ? SA1SurfaceFlags.Water : 0); }
		}

		public bool NoFriction
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.NoFriction) == SA1SurfaceFlags.NoFriction; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.NoFriction) | (value ? SA1SurfaceFlags.NoFriction : 0); }
		}

		public bool NoAcceleration
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.NoAcceleration) == SA1SurfaceFlags.NoAcceleration; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.NoAcceleration) | (value ? SA1SurfaceFlags.NoAcceleration : 0); }
		}

		public bool CannotLand
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.CannotLand) == SA1SurfaceFlags.CannotLand; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.CannotLand) | (value ? SA1SurfaceFlags.CannotLand : 0); }
		}

		public bool IncreasedAcceleration
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.IncreasedAcceleration) == SA1SurfaceFlags.IncreasedAcceleration; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.IncreasedAcceleration) | (value ? SA1SurfaceFlags.IncreasedAcceleration : 0); }
		}

		public bool Diggable
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.Diggable) == SA1SurfaceFlags.Diggable; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.Diggable) | (value ? SA1SurfaceFlags.Diggable : 0); }
		}

		public bool Unclimbable
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.Unclimbable) == SA1SurfaceFlags.Unclimbable; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.Unclimbable) | (value ? SA1SurfaceFlags.Unclimbable : 0); }
		}

		public bool Hurt
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.Hurt) == SA1SurfaceFlags.Hurt; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.Hurt) | (value ? SA1SurfaceFlags.Hurt : 0); }
		}

		public bool Footprints
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.Footprints) == SA1SurfaceFlags.Footprints; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.Footprints) | (value ? SA1SurfaceFlags.Footprints : 0); }
		}

		public bool Visible
		{
			get { return (COL.SurfaceFlags & SA1SurfaceFlags.Visible) == SA1SurfaceFlags.Visible; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SA1SurfaceFlags.Visible) | (value ? SA1SurfaceFlags.Visible : 0); }
		}
		#endregion

		public void Save() { COL.CalculateBounds(); }

		// Form property update event method
		void pw_FormUpdated(object sender, EventArgs e)
		{
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
