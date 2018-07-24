using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.UI;

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
		public LevelItem(Device dev, string filePath, Vertex position, Rotation rotation, int index, EditorItemSelection selectionManager)
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
			ImportModel(filePath, dev);
			COL.CalculateBounds();
			Paste();

            GetHandleMatrix();
		}

		/// <summary>
		/// Creates a LevelItem from an existing COL data.
		/// </summary>
		/// <param name="col"></param>
		/// <param name="dev">Current Direct3d Device.</param>
		public LevelItem(COL col, Device dev, int index, EditorItemSelection selectionManager)
			: base(selectionManager)
		{
			this.index = index;
			COL = col;
			col.Model.ProcessVertexData();
			Mesh = col.Model.Attach.CreateD3DMesh(dev);

            GetHandleMatrix();
		}

		/// <summary>
		/// Creates a new instance of an existing item with the specified position and rotation.
		/// </summary>
		/// <param name="attach">Attach to use for this levelItem</param>
		/// <param name="position">Position in worldspace to place this LevelItem.</param>
		/// <param name="rotation">Rotation.</param>
		public LevelItem(Device dev, Attach attach, Vertex position, Rotation rotation, int index, EditorItemSelection selectionManager)
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
			Mesh = COL.Model.Attach.CreateD3DMesh(dev);
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
			if (!string.IsNullOrEmpty(LevelData.leveltexs))
				result.AddRange(COL.Model.DrawModel(dev, transform, LevelData.Textures[LevelData.leveltexs], Mesh, Visible));
			else
				result.AddRange(COL.Model.DrawModel(dev, transform, null, Mesh, Visible));
			if (Selected)
				result.AddRange(COL.Model.DrawModelInvert(transform, Mesh, Visible));
			return result;
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

		public void RegenerateMesh(Device dev)
		{
			mesh = COL.Model.Attach.CreateD3DMesh(dev);
		}

		public void ImportModel(string filePath, Device dev)
		{
			COL.Model.Attach = Direct3D.Extensions.obj2nj(filePath, LevelData.TextureBitmaps[LevelData.leveltexs].Select(a => a.Name).ToArray());
			Visible = true;
			Solid = true;

			mesh = COL.Model.Attach.CreateD3DMesh(dev);
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
				using (MaterialEditor pw = new MaterialEditor(((BasicAttach)COL.Model.Attach).Material, LevelData.TextureBitmaps[LevelData.leveltexs]))
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
			get { return (COL.SurfaceFlags & SurfaceFlags.Solid) == SurfaceFlags.Solid; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SurfaceFlags.Solid) | (value ? SurfaceFlags.Solid : 0); }
		}

		public bool Water
		{
			get { return (COL.SurfaceFlags & SurfaceFlags.Water) == SurfaceFlags.Water; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SurfaceFlags.Water) | (value ? SurfaceFlags.Water : 0); }
		}

		public bool NoFriction
		{
			get { return (COL.SurfaceFlags & SurfaceFlags.NoFriction) == SurfaceFlags.NoFriction; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SurfaceFlags.NoFriction) | (value ? SurfaceFlags.NoFriction : 0); }
		}

		public bool NoAcceleration
		{
			get { return (COL.SurfaceFlags & SurfaceFlags.NoAcceleration) == SurfaceFlags.NoAcceleration; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SurfaceFlags.NoAcceleration) | (value ? SurfaceFlags.NoAcceleration : 0); }
		}

		public bool CannotLand
		{
			get { return (COL.SurfaceFlags & SurfaceFlags.CannotLand) == SurfaceFlags.CannotLand; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SurfaceFlags.CannotLand) | (value ? SurfaceFlags.CannotLand : 0); }
		}

		public bool IncreasedAcceleration
		{
			get { return (COL.SurfaceFlags & SurfaceFlags.IncreasedAcceleration) == SurfaceFlags.IncreasedAcceleration; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SurfaceFlags.IncreasedAcceleration) | (value ? SurfaceFlags.IncreasedAcceleration : 0); }
		}

		public bool Diggable
		{
			get { return (COL.SurfaceFlags & SurfaceFlags.Diggable) == SurfaceFlags.Diggable; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SurfaceFlags.Diggable) | (value ? SurfaceFlags.Diggable : 0); }
		}

		public bool Unclimbable
		{
			get { return (COL.SurfaceFlags & SurfaceFlags.Unclimbable) == SurfaceFlags.Unclimbable; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SurfaceFlags.Unclimbable) | (value ? SurfaceFlags.Unclimbable : 0); }
		}

		public bool Hurt
		{
			get { return (COL.SurfaceFlags & SurfaceFlags.Hurt) == SurfaceFlags.Hurt; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SurfaceFlags.Hurt) | (value ? SurfaceFlags.Hurt : 0); }
		}

		public bool Footprints
		{
			get { return (COL.SurfaceFlags & SurfaceFlags.Footprints) == SurfaceFlags.Footprints; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SurfaceFlags.Footprints) | (value ? SurfaceFlags.Footprints : 0); }
		}

		public bool Visible
		{
			get { return (COL.SurfaceFlags & SurfaceFlags.Visible) == SurfaceFlags.Visible; }
			set { COL.SurfaceFlags = (COL.SurfaceFlags & ~SurfaceFlags.Visible) | (value ? SurfaceFlags.Visible : 0); }
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
