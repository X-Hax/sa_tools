using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

using SonicRetro.SAModel.Direct3D;

namespace SonicRetro.SAModel.SAEditorCommon.DataTypes
{
	/// <summary>
	/// A linear/corner spline knot.
	/// </summary>
	public class Knot
	{
		private Rotation direction;
		public Rotation Direction { get { return direction; } set { Direction = new Rotation(value.X, value.Y, 0); } }
		public Vertex Position { get; set; }
		public float Distance { get; set; }

		public Knot(Vertex Position)
		{
			this.Position = Position;
			this.Distance = 0f;
			this.Direction = new Rotation();
		}

		public Knot(Vertex Position, Rotation Direction, float Distance)
		{
			this.Position = Position;
			this.direction = Direction;
			this.Distance = Distance;
		}
	}

	/// <summary>
	/// A linear (non-bezier) spline object that contains a series of knots.
	/// </summary>
	public class SplineData
	{
		public List<Knot> KnotList { get; set; }
		public float TotalDistance { get { float d = 0; foreach (Knot knot in KnotList) d += knot.Distance; return d; } }
		public int CodeAddress { get; set; }

		public SplineData()
		{
			KnotList = new List<Knot>();
		}

		/// <summary>
		/// Call this after changing a knot's position - it will set the distance of the previous knot.
		/// </summary>
		/// <param name="knotID"></param>
		public void CalcDistance(int knotID)
		{
			if (knotID == 0) return;

			// knot A is the previous knot, and the one we will be applying the distance value to
			Vector3 knotAPos = KnotList[knotID - 1].Position.ToVector3();
			Vector3 knotBPos = KnotList[knotID].Position.ToVector3();

			float distance = knotAPos.Distance(knotBPos);

			KnotList[knotID - 1].Distance = distance;
		}

		/// <summary>
		/// Add a new knot (in-editor)
		/// </summary>
		public void AddKnot()
		{
			Knot newKnot = new Knot(new Vertex(KnotList[KnotList.Count - 1].Position.X + 10, KnotList[KnotList.Count - 1].Position.Y, KnotList[KnotList.Count - 1].Position.Z));
			KnotList.Add(newKnot);
			CalcDistance(KnotList.Count - 1);
		}

		/// <summary>
		/// Add a pre-defined knot.
		/// </summary>
		/// <param name="knot"></param>
		public void AddKnot(Knot knot)
		{
			KnotList.Add(knot);
			CalcDistance(KnotList.Count - 1);
		}

		/// <summary>
		/// Draws the spline in the scene.
		/// </summary>
		/// <param name="d3dDevice"></param>
		public void Draw(Device d3dDevice)
		{
			CustomVertex.PositionColored[] vertices = new CustomVertex.PositionColored[KnotList.Count];

			for (int i = 0; i < KnotList.Count; i++)
			{
				vertices[i].Position = KnotList[i].Position.ToVector3();
				vertices[i].Color = System.Drawing.Color.Turquoise.ToArgb();
			}

			d3dDevice.VertexFormat = CustomVertex.PositionColored.Format;
			d3dDevice.RenderState.Ambient = System.Drawing.Color.White;
			d3dDevice.RenderState.Lighting = false;
			d3dDevice.Transform.World = Matrix.Identity;

			d3dDevice.DrawUserPrimitives(PrimitiveType.LineStrip, KnotList.Count - 1, vertices);
		}
	}
}
