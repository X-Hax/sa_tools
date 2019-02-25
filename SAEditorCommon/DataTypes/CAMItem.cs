using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D9;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.UI;
using Color = System.Drawing.Color;
using Mesh = SonicRetro.SAModel.Direct3D.Mesh;

namespace SonicRetro.SAModel.SAEditorCommon.DataTypes
{
	[Serializable]
	public class CAMItem : Item, IScaleable
	{
		public enum SADXCamType : byte
		{
			FOLLOW        = 0x00,
			A_FOLLOW      = 0x01,
			C_FOLLOW      = 0x02,
			COL_FOLLOW    = 0x03,
			KNUCKLES      = 0x04,
			A_KNUCKLES    = 0x05,
			C_KNUCKLES    = 0x06,
			COL_KNUCKLES  = 0x07,
			KNUCKLES2     = 0x08,
			A_KNUCKLES2   = 0x09,
			C_KNUCKLES2   = 0x0a,
			COL_KNUCKLES2 = 0x0b,
			MAGONOTE      = 0x0c,
			A_MAGONOTE    = 0x0d,
			C_MAGONOTE    = 0x0e,
			COL_MAGONOTE  = 0x0f,
			SONIC         = 0x10,
			A_SONIC       = 0x11,
			C_SONIC       = 0x12,
			COL_SONIC     = 0x13,
			ASHLAND       = 0x14,
			A_ASHLAND     = 0x15,
			C_ASHLAND     = 0x16,
			ASHLAND_I     = 0x17,
			A_ASHLAND_I   = 0x18,
			C_ASHLAND_I   = 0x19,
			FISHING       = 0x1a,
			A_FISHING     = 0x1b,
			FIXED         = 0x1c,
			A_FIXED       = 0x1d,
			C_FIXED       = 0x1e,
			KLAMATH       = 0x1f,
			A_KLAMATH     = 0x20,
			C_KLAMATH     = 0x21,
			LINE          = 0x22,
			A_LINE        = 0x23,
			NEWFOLLOW     = 0x24,
			A_NEWFOLLOW   = 0x25,
			C_NEWFOLLOW   = 0x26,
			POINT         = 0x27,
			A_POINT       = 0x28,
			C_POINT       = 0x29,
			SONIC_P       = 0x2a,
			A_SONIC_P     = 0x2b,
			C_SONIC_P     = 0x2c,
			ADVERTISE     = 0x2d,
			BACK          = 0x2e,
			BACK2         = 0x2f,
			BUILDING      = 0x30,
			CART          = 0x31,
			CHAOS         = 0x32,
			CHAOS_P       = 0x33,
			CHAOS_STINIT  = 0x34,
			CHAOS_STD     = 0x35,
			CHAOS_W       = 0x36,
			E101R         = 0x37,
			E103          = 0x38,
			EGM3          = 0x39,
			FOLLOW_G      = 0x3a,
			A_FOLLOW_G    = 0x3b,
			LR            = 0x3c,
			COLLI         = 0x3d,
			RuinWaka1     = 0x3e,
			SNOWBOARD     = 0x3f,
			SURVEY        = 0x40,
			TAIHO         = 0x41,
			TORNADE       = 0x42,
			TWO_HARES     = 0x43,
			LEAVE         = 0x44,
			AVOID         = 0x45,
			A_AVOID       = 0x46,
			C_AVOID       = 0x47,
			COL_AVOID     = 0x48,
			EDITOR        = 0x49,
			GURIGURI      = 0x4a,
			PATHCAM       = 0x4b
		}

		#region Camera Data Vars
		public SADXCamType CamType { get; set; }
		public byte CollisionType { get; set; }
		public byte PanSpeed { get; set; }
		public byte Priority { get; set; }
		public Vertex Scale { get; set; }
		//public Int32 NotUsed { get; set; }
		public short ViewAngleX { get; set; }
		public short ViewAngleY { get; set; }
		public Vertex PointA { get; set; }
		public Vertex PointB { get; set; }
		public float Variable { get; set; }

		public override Vertex Position { get { return position; } set { position = value; GetHandleMatrix(); } }
		public override Rotation Rotation
		{
			get { return rotation; }
			set
			{
				if (rotation == null)
				{
					rotation = new Rotation();
				}

				rotation.X = value.X; rotation.Y = value.Y; rotation.Z = 0; GetHandleMatrix();
			}
		}
		public override BoundingSphere Bounds
		{
			get
			{
				float largestScale = Scale.X;
				if (Scale.Y > largestScale) largestScale = Scale.Y;
				if (Scale.Z > largestScale) largestScale = Scale.Z;

				return new BoundingSphere() { Center = new Vertex(Position.X, Position.Y, Position.Z), Radius = (1.5f * largestScale) };
			}
		}

		public Vertex GetScale()
		{
			return Scale;
		}

		public void SetScale(Vertex scale)
		{
			Scale = scale;
		}
		#endregion

		#region Render / Volume Vars
		public static Mesh VolumeMesh { get; set; }
		public static NJS_MATERIAL Material { get; set; }

		public static PointHelper pointHelperA;
		public static PointHelper pointHelperB;
		#endregion

		#region Construction / Initialization
		/// <summary>
		///  Create a new CAM Item from within the editor.
		/// </summary>
		/// <param name="dev">An active Direct3D device for meshing/material/rendering purposes.</param>
		public CAMItem(Vertex position, EditorItemSelection selectionManager)
			: base(selectionManager)
		{
			CamType = (SADXCamType)0x23;

			Position = position;
			Rotation = new Rotation();
			Scale = new Vertex(1, 1, 1);
			Priority = 2;
			PanSpeed = 0x14;

			PointA = new Vertex(position.X + 25, position.Y - 38, position.Z);
			PointB = new Vertex(position.X - 40, position.Y - 38, position.Z);

			Variable = 45f;

			selectionManager.SelectionChanged += selectionManager_SelectionChanged;

			GetHandleMatrix();
		}

		/// <summary>
		/// Creates a new CAM Item from a byte array and offset.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="address"></param>
		public CAMItem(byte[] file, int address, EditorItemSelection selectionManager)
			: base(selectionManager)
		{
			CamType = (SADXCamType)file[address];
			CollisionType = file[address + 1];
			PanSpeed = file[address + 2];
			Priority = file[address + 3];
			Rotation = new Rotation(BitConverter.ToInt16(file, address + 4), BitConverter.ToInt16(file, address + 6), 0);
			Position = new Vertex(file, address + 8);
			Scale = new Vertex(file, address + 20);
			ViewAngleX = BitConverter.ToInt16(file, address + 32);
			ViewAngleY = BitConverter.ToInt16(file, address + 34);
			PointA = new Vertex(file, address + 36);
			PointB = new Vertex(file, address + 48);
			Variable = BitConverter.ToSingle(file, address + 60);

			selectionManager.SelectionChanged += selectionManager_SelectionChanged;

			GetHandleMatrix();
		}

		public static void Init()
		{
			VolumeMesh = Mesh.Box(2f, 2f, 2f);
			Material = new NJS_MATERIAL
			{
				DiffuseColor = Color.FromArgb(200, Color.Purple),
				SpecularColor = Color.Black,
				UseAlpha = true,
				DoubleSided = false,
				Exponent = 10,
				IgnoreSpecular = false,
				UseTexture = false
			};

			pointHelperA = new PointHelper { BoxTexture = Gizmo.ATexture, DrawCube = true };
			pointHelperB = new PointHelper { BoxTexture = Gizmo.BTexture, DrawCube = true };
		}
		#endregion

		#region Saving
		public byte[] GetBytes()
		{
			List<byte> bytes = new List<byte>(0x40) { (byte)CamType, CollisionType, PanSpeed, Priority };

			unchecked
			{
				bytes.AddRange(BitConverter.GetBytes((ushort)Rotation.X));
				bytes.AddRange(BitConverter.GetBytes((ushort)Rotation.Y));
			}

			bytes.AddRange(Position.GetBytes());
			bytes.AddRange(Scale.GetBytes());

			bytes.AddRange(BitConverter.GetBytes(ViewAngleX));
			bytes.AddRange(BitConverter.GetBytes(ViewAngleY));

			bytes.AddRange(PointA.GetBytes());
			bytes.AddRange(PointB.GetBytes());
			bytes.AddRange(BitConverter.GetBytes(Variable));

			return bytes.ToArray();
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
		public override List<RenderInfo> Render(Device dev, EditorCamera camera, MatrixStack transform)
		{
			if (!camera.SphereInFrustum(Bounds))
				return EmptyRenderInfo;

			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(Position);
			transform.NJRotateY(Rotation.Y);
			transform.NJScale((Scale.X), (Scale.Y), (Scale.Z));

			RenderInfo outputInfo = new RenderInfo(VolumeMesh, 0, transform.Top, Material, null, FillMode.Solid, Bounds);

			if (Selected)
			{
				NJS_MATERIAL mat = new NJS_MATERIAL
				{
					DiffuseColor = Color.White,
					IgnoreLighting = true,
					UseAlpha = false
				};
				result.Add(new RenderInfo(VolumeMesh, 0, transform.Top, mat, null, FillMode.Wireframe, Bounds));
			}

			result.Add(outputInfo);

			transform.Pop();
			return result;
		}

		public override HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View)
		{
			if (Scale.X == 0 || Scale.Y == 0 || Scale.Z == 0)
			{
				return HitResult.NoHit;
			}

			MatrixStack transform = new MatrixStack();

			transform.Push();
			transform.NJTranslate(Position);
			transform.NJRotateY(Rotation.Y);
			transform.NJScale((Scale.X), (Scale.Y), (Scale.Z));

			HitResult result = VolumeMesh.CheckHit(Near, Far, Viewport, Projection, View, transform);

			transform.Pop();
			return result;
		}
		#endregion

		#region Selection Changed Methods
		void selectionManager_SelectionChanged(EditorItemSelection sender)
		{
			if (sender.ItemCount != 1)
			{
				pointHelperA.Enabled = false;
				pointHelperB.Enabled = false;
			}
			else
			{
				if (Selected)
				{
					pointHelperA.SetPoint(PointA);
					pointHelperB.SetPoint(PointB);

					pointHelperA.Enabled = true;
					pointHelperB.Enabled = true;
				}
			}
		}
		#endregion
	}
}
