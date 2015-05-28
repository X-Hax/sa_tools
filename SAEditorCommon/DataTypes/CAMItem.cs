using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.UI;

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
		public Vertex Scale { get; set; }
		public Int32 NotUsed { get; set; }
		public Vertex PointA { get; set; }
		public Vertex PointB { get; set; }
		public float Variable { get; set; }

		public override Vertex Position { get; set; }
		public override Rotation Rotation { get; set; }
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
			CamType = 0x23;

			Position = position;
			Rotation = new Rotation();
			Scale = new Vertex(1, 1, 1);
			Priority = 2;
			PanSpeed = 0x14;

			PointA = new Vertex(position.X + 25, position.Y - 38, position.Z);
			PointB = new Vertex(position.X - 40, position.Y - 38, position.Z);

			Variable = 45f;

			selectionManager.SelectionChanged += selectionManager_SelectionChanged;
		}

		/// <summary>
		/// Creates a new CAM Item from a byte array and offset.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="address"></param>
		public CAMItem(byte[] file, int address, EditorItemSelection selectionManager)
			: base(selectionManager)
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

			selectionManager.SelectionChanged += selectionManager_SelectionChanged;

		}

		public static void Init(Device dev)
		{
			VolumeMesh = Mesh.Box(dev, 2f, 2f, 2f);
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
			List<byte> bytes = new List<byte>(0x40) { CamType, Unknown, PanSpeed, Priority };

			unchecked
			{
				bytes.AddRange(BitConverter.GetBytes((ushort)Unknown_2));
				bytes.AddRange(BitConverter.GetBytes((ushort)Rotation.Y));
			}

			bytes.AddRange(Position.GetBytes());
			bytes.AddRange(Scale.GetBytes());

			bytes.AddRange(BitConverter.GetBytes(NotUsed));

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
				result.Add(new RenderInfo(VolumeMesh, 0, transform.Top, mat, null, FillMode.WireFrame, Bounds));
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
