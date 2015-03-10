using System;
using System.Collections.Generic;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using SonicRetro.SAModel.Direct3D;

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
				float largestScale = this.Scale.X;
				if (this.Scale.Y > largestScale) largestScale = this.Scale.Y;
				if (this.Scale.Z > largestScale) largestScale = this.Scale.Z;

				return new BoundingSphere() { Center = new Vertex(this.Position.X, this.Position.Y, this.Position.Z), Radius = (1.5f * largestScale) };
			}
		}
		#endregion

		#region Render / Volume Vars
		public static Microsoft.DirectX.Direct3D.Mesh VolumeMesh { get; set; }
		public static Material Material { get; set; }
		#endregion

		#region Construction / Initialization
		/// <summary>
		///  Create a new CAM Item from within the editor.
		/// </summary>
		/// <param name="dev">An active Direct3D device for meshing/material/rendering purposes.</param>
		public CAMItem(Vertex position, UI.EditorItemSelection selectionManager)
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
		}

		/// <summary>
		/// Creates a new CAM Item from a byte array and offset.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="address"></param>
		public CAMItem(byte[] file, int address, UI.EditorItemSelection selectionManager)
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
		}

		public static void Init(Device dev)
		{
			VolumeMesh = Microsoft.DirectX.Direct3D.Mesh.Box(dev, 2f, 2f, 2f);
			Material = new Material();
			Material.DiffuseColor = Color.FromArgb(200, Color.Purple);
			Material.SpecularColor = Color.Black;
			Material.UseAlpha = true;
			Material.DoubleSided = false;
			Material.Exponent = 10;
			Material.IgnoreSpecular = false;
			Material.UseTexture = false;
		}
		#endregion

		#region Saving
		public byte[] GetBytes()
		{
			List<byte> bytes = new List<byte>(0x40);
			bytes.Add(CamType);
			bytes.Add(Unknown);
			bytes.Add(PanSpeed);
			bytes.Add(Priority);

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
			transform.NJTranslate(this.Position);
			transform.NJRotateY(this.Rotation.Y);
			transform.NJScale((this.Scale.X), (this.Scale.Y), (this.Scale.Z));

			RenderInfo outputInfo = new RenderInfo(VolumeMesh, 0, transform.Top, Material, null, FillMode.Solid, Bounds);

			if (Selected)
			{
				Material mat = new Material
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
			if (this.Scale.X == 0 || this.Scale.Y == 0 || this.Scale.Z == 0)
			{
				return HitResult.NoHit;
			}

			MatrixStack transform = new MatrixStack();

			transform.Push();
			transform.NJTranslate(this.Position);
			transform.NJRotateY(this.Rotation.Y);
			transform.NJScale((this.Scale.X), (this.Scale.Y), (this.Scale.Z));

			HitResult result = VolumeMesh.CheckHit(Near, Far, Viewport, Projection, View, transform);

			transform.Pop();
			return result;
		}
		#endregion
	}
}
