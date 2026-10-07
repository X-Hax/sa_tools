using Assimp;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using SharpDX;
using SharpDX.Direct3D9;
using SplitTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SA2ObjectDefinitions.CityEscape
{
	public class GUNTruck : ObjectDefinition
	{
		protected NJS_OBJECT[] truckmodels;
		protected Mesh[][] truckmeshes;
		protected NJS_TEXLIST[] texarrs;
		protected Texture[][] texs;
		protected NJS_OBJECT points;
		protected Mesh[] pointsmesh;
		protected List<string> texpacks = [];
		protected CarPathData[] truckpaths;
		[NonSerialized]
		protected FVF_PositionColored[] vertices;
		[NonSerialized]
		protected short[] faceIndeces;

		[NonSerialized]
		protected BoundingSphere pathbounds;
		protected Mesh pathmesh;

		public static NJS_MATERIAL SelectedMaterial { get; set; }
		public static NJS_MATERIAL UnSelectedMaterial { get; set; }

		[NonSerialized]
		protected static Mesh vertexHandleMesh;

		//Ripped from SplineData implementation
		public void DefineSpline(int id)
		{
			List<FVF_PositionColored> vertList = new List<FVF_PositionColored>();
			List<short> faceIndexList = new List<short>();

			Vector3 up = new Vector3(0, 1, 0);

			float splineMeshRadius = 5F;

			#region Segment vert/face creation
			short highestFaceIndex = 0;

			for (int i = 0; i < truckpaths[id].Path.Count - 1; i++) // don't process the last knot
			{
				Vector3 thisKnot = new Vector3(truckpaths[id].Path[i].X, truckpaths[id].Path[i].Y, truckpaths[id].Path[i].Z);
				Vector3 nextKnot = new Vector3(truckpaths[id].Path[i + 1].X, truckpaths[id].Path[i + 1].Y, truckpaths[id].Path[i + 1].Z);

				Vector3 directionToNextKnot = Vector3.Normalize(nextKnot - thisKnot);
				Vector3 perpendicularDirection = Vector3.Cross(directionToNextKnot, up);

				// verts for knot 1
				FVF_PositionColored vert1_1; // top vert 1 (0)
				FVF_PositionColored vert1_2; // top vert 2 (1)
				FVF_PositionColored vert1_3; // bottom vert 1 (2)
				FVF_PositionColored vert1_4; // bottom vert 2 (3)

				// verts for knot 2
				FVF_PositionColored vert2_1; // top vert 1 (4)
				FVF_PositionColored vert2_2; // top vert 2 (5)
				FVF_PositionColored vert2_3; // bottom vert 1 (6)
				FVF_PositionColored vert2_4; // bottom vert 2 (7)

				// move top verts
				vert1_1 = new FVF_PositionColored((thisKnot + (perpendicularDirection * splineMeshRadius)), System.Drawing.Color.White);
				vert1_2 = new FVF_PositionColored((thisKnot + (perpendicularDirection * (splineMeshRadius * -1))), System.Drawing.Color.White);

				vert2_1 = new FVF_PositionColored((nextKnot + (perpendicularDirection * splineMeshRadius)), System.Drawing.Color.White);
				vert2_2 = new FVF_PositionColored((nextKnot + (perpendicularDirection * (splineMeshRadius * -1))), System.Drawing.Color.White);

				// move bottom verts
				vert1_3 = new FVF_PositionColored(vert1_1.Position - (up * splineMeshRadius), System.Drawing.Color.White);
				vert1_4 = new FVF_PositionColored(vert1_2.Position - (up * splineMeshRadius), System.Drawing.Color.White);

				vert2_3 = new FVF_PositionColored(vert2_1.Position - (up * splineMeshRadius), System.Drawing.Color.White);
				vert2_4 = new FVF_PositionColored(vert2_2.Position - (up * splineMeshRadius), System.Drawing.Color.White);

				List<short> thisKnotFaceIndexes = new List<short>
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
					thisKnotFaceIndexes[faceIndx] += (short)vertList.Count(); // this is the wrong approach because it's the verts we're indexing, not the faces!
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
			pathbounds = SharpDX.BoundingSphere.FromPoints(vertices.Select(a => a.Position).ToArray()).ToSAModel();

			// build actual mesh from face index array and vbuf
			pathmesh = new Mesh<FVF_PositionColored>(vertices, new short[][] { faceIndeces });

			// create a vertexHandle
			if (vertexHandleMesh == null) vertexHandleMesh = Mesh.Box(1, 1, 1);
		}

		public override void Init(ObjectData data, string name)
		{
			SelectedMaterial = new NJS_MATERIAL
			{
				DiffuseColor = System.Drawing.Color.White,
				SpecularColor = System.Drawing.Color.Black,
				UseAlpha = false,
				DoubleSided = true,
				Exponent = 10,
				IgnoreSpecular = false,
				UseTexture = false,
				IgnoreLighting = true
			};

			UnSelectedMaterial = new NJS_MATERIAL
			{
				DiffuseColor = System.Drawing.Color.Maroon,
				SpecularColor = System.Drawing.Color.Black,
				UseAlpha = false,
				DoubleSided = true,
				Exponent = 10,
				IgnoreSpecular = false,
				UseTexture = false,
				IgnoreLighting = true
			};
			truckmodels = new NJS_OBJECT[5];
			truckmeshes = new Mesh[5][];
			texarrs = new NJS_TEXLIST[5];
			texs = new Texture[5][];
			truckpaths = new CarPathData[2];

			//Despite having a Ginja variant, the truck's body uses the original Chunk model
			truckmodels[0] = ObjectHelper.LoadModel("stg13_cityescape/models/GUNTruck.sa2mdl");
			texarrs[0] = NJS_TEXLIST.Load("stg13_cityescape/tls/GUNTruck.satex");

			truckmodels[1] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/GUNTruckBumper.sa2bmdl");
			texarrs[1] = NJS_TEXLIST.Load("stg13_cityescape/tls/GUNTruckBumper.satex");

			truckmodels[2] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/GUNTruckWindshield.sa2bmdl");
			texarrs[2] = NJS_TEXLIST.Load("stg13_cityescape/tls/GUNTruckWindshield.satex");

			truckmodels[3] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/GUNTruckRWindow.sa2bmdl");
			texarrs[3] = NJS_TEXLIST.Load("stg13_cityescape/tls/GUNTruckRWindow.satex");

			truckmodels[4] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/GUNTruckLWindow.sa2bmdl");
			texarrs[4] = NJS_TEXLIST.Load("stg13_cityescape/tls/GUNTruckLWindow.satex");

			for (int i = 0; i < 5; i++)
			{
				truckmeshes[i] = ObjectHelper.GetMeshes(truckmodels[i]);
			}

			points = ObjectHelper.LoadModel("OBJECT/SmallAnimalBubbleCollision.sa2mdl");
			pointsmesh = ObjectHelper.GetMeshes(points);

			texpacks.Add("objtex_stg13");
			texpacks.Add("landtx13");

			string pathfolder = "stg13_cityescape\\models\\TRUCK\\Paths";
			if (Directory.Exists(pathfolder))
			{
				List<string> pathFiles = new List<string>();

				for (int i = 0; i < int.MaxValue; i++)
				{
					string path = Path.Combine(pathfolder, string.Format("{0}.ini", i));
					if (File.Exists(path))
					{
						pathFiles.Add(path);
					}
					else
						break;
				}

				for (int j = 0; j < pathFiles.Count; j++) // looping through path files
				{
					truckpaths[j] = CarPathData.Load(pathFiles[j]);
				}
			}
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			DefineSpline(0);
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateZYX(item.Rotation);
			for (int i = 0; i < 5; i++)
			{
				result = HitResult.Min(result, truckmodels[i].CheckHit(Near, Far, Viewport, Projection, View, transform, truckmeshes[i]));
			}

			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale(item.Scale.Y / 10F, item.Scale.Y / 10F, item.Scale.Y / 10F);
			result = HitResult.Min(result, points.CheckHit(Near, Far, Viewport, Projection, View, transform, pointsmesh));
			transform.Pop();

			for (int i = 0; i < truckpaths[0].Path.Count; i++)
			{
				transform.Push();
				transform.NJTranslate(truckpaths[0].Path[i]);
				result = HitResult.Min(result, points.CheckHit(Near, Far, Viewport, Projection, View, transform, pointsmesh));
				transform.Pop();
			}
			transform.Push();

			//transform.NJTranslate(splineVertex.Position.X, splineVertex.Position.Y, splineVertex.Position.Z);

			result = HitResult.Min(result, pathmesh.CheckHit(Near, Far, Viewport, Projection, View, transform));

			transform.Pop();
			return result;

		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			for (int i = 0; i < 5; i++)
			{
				if (texs[i] == null)
					texs[i] = ObjectHelper.GetTextures(texpacks, texarrs[i]);
			}
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			for (int i = 0; i < 5; i++)
			{
				result.AddRange(truckmodels[i].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs[i], truckmeshes[i], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(truckmodels[i].DrawModelTreeInvert(transform, truckmeshes[i]));
			}
			transform.Pop();

			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale(item.Scale.Y / 10F, item.Scale.Y / 10F, item.Scale.Y / 10F);
			result.AddRange(points.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs[0], pointsmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(points.DrawModelTreeInvert(transform, pointsmesh));
			transform.Pop();

			DefineSpline(0);

			RenderInfo outputInfo = new RenderInfo(pathmesh, 0, Matrix.Identity, (item.Selected) ? SelectedMaterial : UnSelectedMaterial, null, EditorOptions.RenderFillMode, new BoundingSphere(pathbounds.Center.X, pathbounds.Center.Y,
				pathbounds.Center.Z, 1000F));
			result.Add(outputInfo);
			for (int i = 0; i < truckpaths[0].Path.Count; i++)
			{
				transform.Push();
				transform.NJTranslate(truckpaths[0].Path[i]);
				result.AddRange(points.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs[Math.Min(i, 4)], pointsmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(points.DrawModelTreeInvert(transform, pointsmesh));
				transform.Pop();
			}
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateZYX(item.Rotation);
			for (int i = 0; i < 5; i++)
			{
				result.Add(new ModelTransform(truckmodels[i], transform.Top));
			}
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			DefineSpline(0);
			BoundingSphere completebounds = null;
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateZYX(item.Rotation);
			BoundingSphere mainbounds = ObjectHelper.GetModelBounds(truckmodels[0], transform);
			transform.Pop();
			for (int i = 0; i < truckpaths[0].Path.Count; i++)
			{
				transform.Push();
				transform.NJTranslate(truckpaths[0].Path[i]);
				BoundingSphere pointbounds = ObjectHelper.GetModelBounds(points, transform);
				transform.Pop();
				completebounds = SAModel.Direct3D.Extensions.Merge(pointbounds, mainbounds);
			}
			return SAModel.Direct3D.Extensions.Merge(completebounds, new BoundingSphere(pathbounds.Center.X, pathbounds.Center.Y,
				pathbounds.Center.Z, 10000F));
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);

			return matrix;
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Truck Speed", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Trigger Scale", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
		public override string Name { get { return "GUN Truck"; } }
	}
	
}