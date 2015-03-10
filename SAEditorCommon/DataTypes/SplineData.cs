using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

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
	[Serializable]
	public class SplineData : Item
	{
		#region Data Vars
		public List<Knot> KnotList { get; set; }
		public float TotalDistance { get { float d = 0; foreach (Knot knot in KnotList) d += knot.Distance; return d; } }
		public int CodeAddress { get; set; }
		#endregion

		#region Rendering Variables
		private CustomVertex.PositionColored[] vertices;
		private UInt16[] faceIndeces;

		private BoundingSphere bounds;
		public override BoundingSphere Bounds { get	{ return bounds; }	}
		private Microsoft.DirectX.Direct3D.Mesh mesh;
		public static Material SelectedMaterial { get; set; }
		public static Material UnSelectedMaterial { get; set; }
		Microsoft.DirectX.Direct3D.Mesh boxMesh;
		Sprite textSprite;
		#endregion

		#region Editor Variables
		private int selectedKnot = -1;
		#endregion

		public static void Init()
		{
			SelectedMaterial = new Material();
			SelectedMaterial.DiffuseColor = Color.White;
			SelectedMaterial.SpecularColor = Color.Black;
			SelectedMaterial.UseAlpha = false;
			SelectedMaterial.DoubleSided = true;
			SelectedMaterial.Exponent = 10;
			SelectedMaterial.IgnoreSpecular = false;
			SelectedMaterial.UseTexture = false;
			SelectedMaterial.IgnoreLighting = true;

			UnSelectedMaterial = new Material();
			UnSelectedMaterial.DiffuseColor = Color.Maroon;
			UnSelectedMaterial.SpecularColor = Color.Black;
			UnSelectedMaterial.UseAlpha = false;
			UnSelectedMaterial.DoubleSided = true;
			UnSelectedMaterial.Exponent = 10;
			UnSelectedMaterial.IgnoreSpecular = false;
			UnSelectedMaterial.UseTexture = false;
			UnSelectedMaterial.IgnoreLighting = true;
		}

		private const float splineMeshRadius = 1f;

		public void RebuildMesh(Device device)
		{
			List<CustomVertex.PositionColored> vertList = new List<CustomVertex.PositionColored>();
			List<UInt16> faceIndexList = new List<UInt16>();

			Vector3 up = new Vector3(0,1,0);

			#region Segment vert/face creation
			UInt16 highestFaceIndex = 0;

			for (int i = 0; i < KnotList.Count - 1; i++) // don't process the last knot
			{
				Vector3 thisKnot = KnotList[i].Position.ToVector3();
				Vector3 nextKnot = KnotList[i+1].Position.ToVector3();

				Vector3 directionToNextKnot = Vector3.Normalize(nextKnot - thisKnot);
				Vector3 perpendicularDirection = Vector3.Cross(directionToNextKnot, up);

				// verts for knot 1
				CustomVertex.PositionColored vert1_1; // top vert 1 (0)
				CustomVertex.PositionColored vert1_2; // top vert 2 (1)
				CustomVertex.PositionColored vert1_3; // bottom vert 1 (2)
				CustomVertex.PositionColored vert1_4; // bottom vert 2 (3)

				// verts for knot 2
				CustomVertex.PositionColored vert2_1; // top vert 1 (4)
				CustomVertex.PositionColored vert2_2; // top vert 2 (5)
				CustomVertex.PositionColored vert2_3; // bottom vert 1 (6)
				CustomVertex.PositionColored vert2_4; // bottom vert 2 (7)

				// move top verts
				vert1_1 = new CustomVertex.PositionColored((thisKnot + (perpendicularDirection * splineMeshRadius)), Color.White.ToArgb());
				vert1_2 = new CustomVertex.PositionColored((thisKnot + (perpendicularDirection * (splineMeshRadius * -1))), Color.White.ToArgb());

				vert2_1 = new CustomVertex.PositionColored((nextKnot + (perpendicularDirection * splineMeshRadius)), Color.White.ToArgb());
				vert2_2 = new CustomVertex.PositionColored((nextKnot + (perpendicularDirection * (splineMeshRadius * -1))), Color.White.ToArgb());

				// move bottom verts
				vert1_3 = new CustomVertex.PositionColored(vert1_1.Position - (up * splineMeshRadius), Color.White.ToArgb());
				vert1_4 = new CustomVertex.PositionColored(vert1_2.Position - (up * splineMeshRadius), Color.White.ToArgb());

				vert2_3 = new CustomVertex.PositionColored(vert2_1.Position - (up * splineMeshRadius), Color.White.ToArgb());
				vert2_4 = new CustomVertex.PositionColored(vert2_2.Position - (up * splineMeshRadius), Color.White.ToArgb());

				List<UInt16> thisKnotFaceIndexes = new List<UInt16> 
				{
					// far side
					4,0,6,
					6,2,0,

					// bottom
					6,2,3,
					3,7,6,

					// our side
					7,3,1,
					7,5,1,

					// top
					1,5,4,
					4,0,1
				};

				for (int faceIndx = 0; faceIndx < thisKnotFaceIndexes.Count(); faceIndx++)
				{
					thisKnotFaceIndexes[faceIndx] += (UInt16)vertList.Count(); // this is the wrong approach because it's the verts we're indexing, not the faces!
					if (thisKnotFaceIndexes[faceIndx] > highestFaceIndex) highestFaceIndex = thisKnotFaceIndexes[faceIndx];
				}

				// add verts to vert list and faces to face list
				vertList.Add(vert1_1);
				vertList.Add(vert1_2);
				vertList.Add(vert1_3);
				vertList.Add(vert1_4);
				vertList.Add(vert2_1);
				vertList.Add(vert2_2);
				vertList.Add(vert2_3);
				vertList.Add(vert2_4);

				faceIndexList.AddRange(thisKnotFaceIndexes);
			}
			#endregion

			vertices = vertList.ToArray();
			faceIndeces = faceIndexList.ToArray();

			// build bounding sphere
			Vector3 center;
			float radius = Geometry.ComputeBoundingSphere(vertices, CustomVertex.PositionColored.Format, out center);
			bounds = new BoundingSphere(center.ToVertex(), radius);

			// build actual mesh from face index array and vbuf
			mesh = new Microsoft.DirectX.Direct3D.Mesh(faceIndexList.Count() / 3, vertList.Count(), MeshFlags.Managed, CustomVertex.PositionColored.Format, device);

			// Apply the buffers
			mesh.SetVertexBufferData(vertices, LockFlags.None);
			mesh.IndexBuffer.SetData(faceIndeces, 0, LockFlags.None);

			// TODO: this is locking up the program
			/*int[] adjacency = new int[3 * mesh.NumberFaces];
			mesh.GenerateAdjacency(0.01f, adjacency);
			mesh = mesh.Optimize(MeshFlags.Managed,adjacency);*/
			
			textSprite = new Sprite(device);
		}

		public SplineData(UI.EditorItemSelection selectionManager)
			: base (selectionManager)
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

		// override implementations go here!
		public override HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View)
		{
			MatrixStack transform = new MatrixStack();

			transform.Push();
			return mesh.CheckHit(Near, Far, Viewport, Projection, View, transform);
		}

		public override List<RenderInfo> Render(Device dev, EditorCamera camera, MatrixStack transform)
		{
			if (!camera.SphereInFrustum(Bounds))
				return EmptyRenderInfo;

			List<RenderInfo> result = new List<RenderInfo>();

			RenderInfo outputInfo = new RenderInfo(mesh, 0, Matrix.Identity, (Selected) ? SelectedMaterial : UnSelectedMaterial, null, EditorOptions.RenderFillMode, Bounds);
			result.Add(outputInfo);

			textSprite.Begin(SpriteFlags.AlphaBlend);

			Matrix view = camera.ToMatrix();
			Matrix projection = Matrix.PerspectiveFovRH(camera.FOV, camera.Aspect, 1, camera.DrawDistance);

			for (int vIndx = 0; vIndx < KnotList.Count(); vIndx++)
			{
				Vector3 screenCoordinates = Vector3.Project(KnotList[vIndx].Position.ToVector3(), dev.Viewport, projection, view, Matrix.Identity);
				Vector3 altScrCoord = Vector3.Project(KnotList[vIndx].Position.ToVector3(), dev.Viewport, dev.Transform.Projection, dev.Transform.View, Matrix.Identity);

				EditorOptions.OnscreenFont.DrawText(textSprite, vIndx.ToString(), new Point((int)(screenCoordinates.X), (int)(screenCoordinates.Y)), Color.White);
			}

			textSprite.End();

			return result;
		}

		public override void Delete()
		{
			throw new NotImplementedException();
		}

		public override void Paste()
		{
			throw new NotImplementedException();
		}

		public override Vertex Position
		{
			get
			{
				return base.Position;
			}
			set
			{
				base.Position = value;
			}
		}

		// todo 
		public override Rotation Rotation
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
	}
}