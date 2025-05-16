using System;
using System.Collections.Generic;
using System.ComponentModel;
using SharpDX;
using SharpDX.Direct3D9;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.UI;
using Color = System.Drawing.Color;
using Mesh = SAModel.Direct3D.Mesh;

namespace SAModel.SAEditorCommon.DataTypes
{
	[Serializable]
	public class CAMItem : Item, IScaleable
	{
		#region Enums
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
			PATHCAM       = 0x4b,
			KOSCAM		  = 0x4c
		}

		public enum SADXCamColType : byte
		{
			CamCol_Sphere = 0,
			CamCol_Plane = 1,
			CamCol_Block = 2,
		}

		public enum SADXCamAdjustType : byte
		{
			CamAdjust_NONE = 0x0,
			CamAdjust_NORMAL = 0x1,
			CamAdjust_NORMAL_S = 0x2,
			CamAdjust_SLOW = 0x3,
			CamAdjust_SLOW_S = 0x4,
			CamAdjust_TIME = 0x5,
			CamAdjust_THREE1 = 0x6,
			CamAdjust_THREE1C = 0x7,
			CamAdjust_THREE2 = 0x8,
			CamAdjust_THREE2C = 0x9,
			CamAdjust_THREE3 = 0xA,
			CamAdjust_THREE3C = 0xB,
			CamAdjust_THREE4 = 0xC,
			CamAdjust_THREE4C = 0xD,
			CamAdjust_THREE5 = 0xE,
			CamAdjust_THREE5C = 0xF,
			CamAdjust_RELATIVE1 = 0x10,
			CamAdjust_RELATIVE1C = 0x11,
			CamAdjust_RELATIVE2 = 0x12,
			CamAdjust_RELATIVE2C = 0x13,
			CamAdjust_RELATIVE3 = 0x14,
			CamAdjust_RELATIVE3C = 0x15,
			CamAdjust_RELATIVE4 = 0x16,
			CamAdjust_RELATIVE4C = 0x17,
			CamAdjust_RELATIVE5 = 0x18,
			CamAdjust_RELATIVE5C = 0x19,
			CamAdjust_RELATIVE6C = 0x1A,
			CamAdjust_FORFREECAMERA = 0x1B,
		}
		#endregion

		#region Camera Variables
		[Category("Info"), DisplayName("Camera Mode"), Description("The Camera Mode to be used.")]
		public SADXCamType CamType { get; set; }
		[Category("Info"), DisplayName("Collision Mode"), Description("The Collision type used by the camera entry.")]
		public SADXCamColType CollisionType { get; set; }
		[Category("Info"), DisplayName("Adjustment Mode"), Description("The method used by the camera to transition into another camera.")]
		public SADXCamAdjustType AdjustType { get; set; }
		[Category("Info"), Description("Priority of the camera compared to other cameras.")]
		public byte Priority { get; set; }
		[Category("Info"), DisplayName("Collision Position"), Description("Position of the Camera Collision.")]
		public override Vertex Position { get { return position; } set { position = value; GetHandleMatrix(); } }
		[Category("Info"), DisplayName("Collision Angle X"), Description("The X Angle of the Collision Mesh")]
		public int AngleX { get { return Rotation.X; } set { Rotation.X = value; } }
		[Category("Info"), DisplayName("Collision Angle Y"), Description("The Y Angle of the Collision Mesh")]
		public int AngleY { get { return Rotation.Y; } set { Rotation.Y = value; } }
		[Browsable(false)]
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
				float largestScale;
				float BoundsScale;
				switch (CollisionType)
				{
					case (SADXCamColType.CamCol_Sphere):
					default:
						BoundsScale = MathF.Sqrt(Scale.Z);
						break;
					case (SADXCamColType.CamCol_Plane):
						largestScale = Scale.X;
						if (Scale.Y > largestScale) largestScale = Scale.Y;
						BoundsScale = largestScale*2.0f;
						break;
					case (SADXCamColType.CamCol_Block):
						largestScale = Scale.X;
						if (Scale.Y > largestScale) largestScale = Scale.Y;
						if (Scale.Z > largestScale) largestScale = Scale.Z;
						BoundsScale = largestScale*2.0f;
						break;
				}
				return new BoundingSphere() { Center = new Vertex(Position.X, Position.Y, Position.Z), Radius = BoundsScale };
			}
		}
		[Category("Info"), DisplayName("Colliion Scale"), Description("Scale of the Collision.\nSpheres - Z\nPlane - X Y\nCube - X Y Z")]
		public Vertex Scale { get; set; }

		[Category("Parameters"), DisplayName("Camera Angle X"), Description("Camera Angle X Property, not always used.")]
		public short CameraAngleX { get; set; }
		[Category("Parameters"), DisplayName("Camera Angle Y"), Description("Camera Angle Y Property, not always used.")]
		public short CameraAngleY { get; set; }
		[Category("Parameters"), DisplayName("Target Position X"), Description("Target Position X, not always used.")]
		public float DirectPositionX { get { return DirectPosition.X; } set { DirectPosition.X = value; } }
		[Category("Parameters"), DisplayName("Target Position Y"), Description("Target Position X, not always used.")]
		public float DirectPositionY { get { return DirectPosition.Y; } set { DirectPosition.Y = value; } }
		[Category("Parameters"), DisplayName("Target Position Z"), Description("Target Position X, not always used.")]
		public float DirectPositionZ { get { return DirectPosition.Z; } set { DirectPosition.Z = value; } }
		[Category("Parameters"), DisplayName("Camera Position X"), Description("Camera Position X, not always used.")]
		public float CameraPositionX { get { return CameraPosition.X; } set { CameraPosition.X = value; } }
		[Category("Parameters"), DisplayName("Camera Position Y"), Description("Camera Position Y, not always used.")]
		public float CameraPositionY { get { return CameraPosition.Y; } set { CameraPosition.Y = value; } }
		[Category("Parameters"), DisplayName("Camera Position Z"), Description("Camera Position Z, not always used.")]
		public float CameraPositionZ { get { return CameraPosition.Z; } set { CameraPosition.Z = value; } }
		[Category("Parameters"), Description("Distance maintained between Camera Position and Target, not always used.")]
		public float Distance { get; set; }

		Vertex DirectPosition = new Vertex();
		Vertex CameraPosition = new Vertex();

		public Vertex GetScale()
		{
			return Scale;
		}

		public void SetScale(Vertex scale)
		{
			Scale = scale;
		}
		#endregion

		#region Render Variables
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
			CamType = (SADXCamType.FOLLOW);

			Position = position;
			Rotation = new Rotation();
			Scale = new Vertex(1, 1, 1);
			Priority = 0;
			AdjustType = (SADXCamAdjustType.CamAdjust_RELATIVE3);
			CollisionType = (SADXCamColType.CamCol_Block);

			DirectPosition = new Vertex(position.X + 25.0f, position.Y + 25.0f, position.Z + 25.0f);
			CameraPosition = new Vertex(position.X, position.Y, position.Z);

			Distance = 45f;

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
			Priority = file[address + 1];
			AdjustType = (SADXCamAdjustType)file[address + 2];
			CollisionType = (SADXCamColType)file[address + 3];
			Rotation = new Rotation(BitConverter.ToInt16(file, address + 4), BitConverter.ToInt16(file, address + 6), 0);
			Position = new Vertex(file, address + 8);
			Scale = new Vertex(file, address + 20);
			CameraAngleX = BitConverter.ToInt16(file, address + 32);
			CameraAngleY = BitConverter.ToInt16(file, address + 34);

			DirectPosition = new Vertex(file, address + 36);
			CameraPosition = new Vertex(file, address + 48);

			Distance = BitConverter.ToSingle(file, address + 60);

			selectionManager.SelectionChanged += selectionManager_SelectionChanged;

			GetHandleMatrix();
		}

		public static void Init()
		{
			pointHelperA = new PointHelper { BoxTexture = Gizmo.ATexture, DrawCube = true };
			pointHelperB = new PointHelper { BoxTexture = Gizmo.BTexture, DrawCube = true };
		}
		#endregion

		#region File Saving
		public byte[] GetBytes()
		{
			List<byte> bytes = new List<byte>(0x40) { (byte)CamType, Priority, (byte)AdjustType, (byte)CollisionType };

			unchecked
			{
				bytes.AddRange(BitConverter.GetBytes((ushort)Rotation.X));
				bytes.AddRange(BitConverter.GetBytes((ushort)Rotation.Y));
			}

			bytes.AddRange(Position.GetBytes());
			bytes.AddRange(Scale.GetBytes());

			bytes.AddRange(BitConverter.GetBytes(CameraAngleX));
			bytes.AddRange(BitConverter.GetBytes(CameraAngleY));

			bytes.AddRange(DirectPosition.GetBytes());
			bytes.AddRange(CameraPosition.GetBytes());
			bytes.AddRange(BitConverter.GetBytes(Distance));

			return bytes.ToArray();
		}

		public static void Save(List<CAMItem> items, string filename, bool bigendian = false) => System.IO.File.WriteAllBytes(filename, Save(items, bigendian));

		public static byte[] Save(List<CAMItem> items, bool bigendian = false)
		{
			List<byte> file = new(items.Count * 0x40 + 0x40);
			bool bigendianbk = ByteConverter.BigEndian;
			ByteConverter.BigEndian = bigendian;
			file.AddRange(ByteConverter.GetBytes(items.Count));
			file.Align(0x40);

			foreach (CAMItem item in items)
				file.AddRange(item.GetBytes());
			ByteConverter.BigEndian = bigendianbk;
			return file.ToArray();
		}

		#endregion

		#region Inheretance
		#region Add / Delete
		public override void Paste()
		{
			LevelData.CAMItems[LevelData.Character].Add(this);
		}

		protected override void DeleteInternal(EditorItemSelection selectionManager)
		{
			selectionManager.SelectionChanged -= selectionManager_SelectionChanged;
			LevelData.CAMItems[LevelData.Character].Remove(this);
		}
		#endregion

		#region Render
		public override List<RenderInfo> Render(Device dev, EditorCamera camera, MatrixStack transform)
		{
			if (!camera.SphereInFrustum(Bounds))
				return EmptyRenderInfo;

			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(Position);
			transform.NJRotateX(Rotation.X);
			transform.NJRotateY(Rotation.Y);

			switch (CollisionType)
			{
				case (SADXCamColType.CamCol_Sphere):
				default:
					VolumeMesh = Mesh.Sphere(1.0f, 12, 6);
					Material = new NJS_MATERIAL
					{
						DiffuseColor = Color.FromArgb(200, Color.Blue),
						SpecularColor = Color.Black,
						UseAlpha = true,
						DoubleSided = false,
						Exponent = 10,
						IgnoreSpecular = false,
						UseTexture = false
					};
					float scale = MathF.Sqrt(Scale.Z);
					transform.NJScale(scale, scale, scale);
					break;
				case (SADXCamColType.CamCol_Plane):
					VolumeMesh = Mesh.Box(1.0f, 1.0f, 0.0f);
					Material = new NJS_MATERIAL
					{
						DiffuseColor = Color.FromArgb(200, Color.Green),
						SpecularColor = Color.Black,
						UseAlpha = true,
						DoubleSided = false,
						Exponent = 10,
						IgnoreSpecular = false,
						UseTexture = false
					};
					transform.NJScale((Scale.X*2.0f), (Scale.Y*2.0f), (1f));
					break;
				case (SADXCamColType.CamCol_Block):
					VolumeMesh = Mesh.Box(1.0f, 1.0f, 1.0f);
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
					transform.NJScale((Scale.X*2.0f), (Scale.Y*2.0f), (Scale.Z*2.0f));
					break;
			}

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
		#endregion

		#region Bounds
		public override HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View)
		{
			if (Scale.X == 0 || Scale.Y == 0 || Scale.Z == 0)
			{
				return HitResult.NoHit;
			}

			MatrixStack transform = new MatrixStack();

			transform.Push();
			transform.NJTranslate(Position);
			transform.NJRotateX(Rotation.X);
			transform.NJRotateY(Rotation.Y);

			switch (CollisionType)
			{
				case (SADXCamColType.CamCol_Sphere):
				default:
					float scale = MathF.Sqrt(Scale.Z);
					transform.NJScale(scale, scale, scale);
					break;
				case (SADXCamColType.CamCol_Plane):
					transform.NJScale((Scale.X*2.0f), (Scale.Y*2.0f), (1f));
					break;
				case (SADXCamColType.CamCol_Block):
					transform.NJScale((Scale.X*2.0f), (Scale.Y*2.0f), (Scale.Z*2.0f));
					break;
			}

			HitResult result = VolumeMesh.CheckHit(Near, Far, Viewport, Projection, View, transform);

			transform.Pop();
			return result;
		}
		#endregion
		#endregion

		#region Selection Manager
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
					switch (CamType)
					{
						case SADXCamType.ASHLAND:
						case SADXCamType.A_ASHLAND:
						case SADXCamType.C_ASHLAND:
						case SADXCamType.ASHLAND_I:
						case SADXCamType.A_ASHLAND_I:
						case SADXCamType.C_ASHLAND_I:
							pointHelperB.SetPoint(CameraPosition);
							pointHelperB.Enabled = true;
							break;
						case SADXCamType.FIXED:
						case SADXCamType.C_FIXED:
						case SADXCamType.A_FIXED:
							pointHelperB.SetPoint(CameraPosition);
							pointHelperB.Enabled = true;
							pointHelperA.SetPoint(DirectPosition);
							pointHelperA.Enabled = true;
							break;
						case SADXCamType.POINT:
						case SADXCamType.A_POINT:
						case SADXCamType.C_POINT:
						case SADXCamType.KLAMATH:
						case SADXCamType.A_KLAMATH:
						case SADXCamType.C_KLAMATH:
						case SADXCamType.TORNADE:
							pointHelperA.SetPoint(DirectPosition);
							pointHelperA.Enabled = true;
							break;
						default:
							pointHelperA.Enabled = false;
							pointHelperB.Enabled = false;
							break;
					}
				}
			}
		}
		#endregion
	}
}
