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

namespace SA2ObjectDefinitions.PumpkinHill
{
	public class GhostTrain : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] mesh;
		protected NJS_TEXLIST texarr;
		protected Texture[] texs;
		protected NJS_OBJECT points;
		protected Mesh[] pointsmesh;
		protected List<string> texpacks = [];
		protected CarPathData[] trainpaths;
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
		[NonSerialized]
		protected Sprite textSprite;

		//Ripped from SplineData implementation
		public void DefineSpline(int id, Device dev = null)
		{
			List<FVF_PositionColored> vertList = new List<FVF_PositionColored>();
			List<short> faceIndexList = new List<short>();

			float splineMeshRadius = 1F;

			Vector3 up = new Vector3(0, 1, 0);

			#region Segment vert/face creation
			short highestFaceIndex = 0;

			for (int i = 0; i < trainpaths[id].Path.Count - 1; i++) // don't process the last knot
			{
				Vector3 thisKnot = new Vector3(trainpaths[id].Path[i].X, trainpaths[id].Path[i].Y, trainpaths[id].Path[i].Z);
				Vector3 nextKnot = new Vector3(trainpaths[id].Path[i + 1].X, trainpaths[id].Path[i + 1].Y, trainpaths[id].Path[i + 1].Z);

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
			//if (pathmesh == null)
			pathmesh = new Mesh<FVF_PositionColored>(vertices, new short[][] { faceIndeces });

			// create a vertexHandle
			if (vertexHandleMesh == null)
				vertexHandleMesh = Mesh.Box(1, 1, 1);

			if (dev != null)
				textSprite = new Sprite(dev); // todo: do we really have to create this so often? Look into storing a cache list statically?
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
			model = ObjectHelper.LoadModel("stg05_pumpkin/models/GSTRAIN.sa2mdl");
			mesh = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("stg05_pumpkin/tls/GSTRAIN.satex");

			points = ObjectHelper.LoadModel("OBJECT/SmallAnimalBubbleCollision.sa2mdl");
			pointsmesh = ObjectHelper.GetMeshes(points);

			texpacks.Add("objtex_stg05");
			texpacks.Add("landtx05");

			trainpaths = new CarPathData[5];

			string pathfolder = "stg05_pumpkin/models/Paths";
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
					trainpaths[j] = CarPathData.Load(pathFiles[j]);
				}
			}
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			int pathID = (item.Rotation.X & 0xF) % 5;
			DefineSpline(pathID);
			HitResult result = HitResult.NoHit;
			transform.Push();
			if (pathID < 2)
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				transform.NJTranslate(trainpaths[pathID].Path[0]);
			}
			else
			{
				transform.NJTranslate(trainpaths[pathID].Path[0]);
				switch (pathID)
				{
					case 2:
					default:
						break;
					case 3:
						transform.NJRotateY(0xE000);
						break;
					case 4:
						transform.NJRotateY(0xE700);
						break;

				}
			}
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh));
			transform.Pop();

			//transform.NJTranslate(splineVertex.Position.X, splineVertex.Position.Y, splineVertex.Position.Z);
			transform.Push();
			if (pathID < 2)
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
			}
			result = HitResult.Min(result, pathmesh.CheckHit(Near, Far, Viewport, Projection, View, transform));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			result = HitResult.Min(result, points.CheckHit(Near, Far, Viewport, Projection, View, transform, pointsmesh));
			transform.Pop();
			return result;

		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			int pathID = (item.Rotation.X & 0xF) % 5;
			if (texs == null)
				texs = ObjectHelper.GetTextures(texpacks, texarr);
			List<RenderInfo> result = new List<RenderInfo>();

			DefineSpline(pathID, dev);
			transform.Push();
			if (pathID < 2)
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
			}
			RenderInfo outputInfo = new RenderInfo(pathmesh, 0, transform.Top, (item.Selected) ? SelectedMaterial : UnSelectedMaterial, null, EditorOptions.RenderFillMode, pathbounds);
			result.Add(outputInfo);
			textSprite.Begin(SpriteFlags.AlphaBlend);

			Matrix view = camera.ToMatrix();
			Matrix projection = Matrix.PerspectiveFovRH(camera.FOV, camera.Aspect, 1, camera.DrawDistance);
			Viewport viewport = dev.Viewport;

			if (item.Selected)
			{
				for (int vIndx = 0; vIndx < trainpaths[pathID].Path.Count(); vIndx++)
				{
					#region Draw Vertex IDs
					Vector3 screenCoordinates = viewport.Project(new Vector3(trainpaths[pathID].Path[vIndx].X, trainpaths[pathID].Path[vIndx].Y, trainpaths[pathID].Path[vIndx].Z),
						projection, view, transform.Top);

					EditorOptions.OnscreenFont.DrawText(textSprite, vIndx.ToString(), (int)screenCoordinates.X, (int)screenCoordinates.Y, System.Drawing.Color.White.ToRawColorBGRA());
					#endregion

					#region Draw Vertex Handles
					transform.Push();

					transform.NJTranslate(trainpaths[pathID].Path[vIndx].X, trainpaths[pathID].Path[vIndx].Y, trainpaths[pathID].Path[vIndx].Z);

					result.Add(new RenderInfo(vertexHandleMesh, 0, transform.Top, UnSelectedMaterial, null, FillMode.Solid, new BoundingSphere(trainpaths[pathID].Path[vIndx].X,
						trainpaths[pathID].Path[vIndx].Y, trainpaths[pathID].Path[vIndx].Z, 1f)));

					result.Add(new RenderInfo(vertexHandleMesh, 0, transform.Top, SelectedMaterial, null, FillMode.Wireframe, new BoundingSphere(trainpaths[pathID].Path[vIndx].X,
					trainpaths[pathID].Path[vIndx].Y, trainpaths[pathID].Path[vIndx].Z, 1f)));
					transform.Pop();
					#endregion
				}
			}

			textSprite.End();
			transform.Pop();
			transform.Push();
			if (pathID < 2)
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				transform.NJTranslate(trainpaths[pathID].Path[0]);
			}
			else
			{
				transform.NJTranslate(trainpaths[pathID].Path[0]);
				switch (pathID)
				{
					case 2:
					default:
						break;
					case 3:
					transform.NJRotateY(0xE000);
						break;
					case 4:
						transform.NJRotateY(0xE700);
						break;

				}
			}
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, mesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, mesh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			result.AddRange(points.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, pointsmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(points.DrawModelTreeInvert(transform, pointsmesh));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			BoundingSphere modbounds = pathbounds;
			int pathID = (item.Rotation.X & 0xF) % 5;
			DefineSpline(pathID);
			MatrixStack transform = new MatrixStack();
			transform.Push();
			if (pathID < 2)
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				transform.NJTranslate(trainpaths[pathID].Path[0]);
			}
			else
			{
				transform.NJTranslate(trainpaths[pathID].Path[0]);
				switch (pathID)
				{
					case 2:
					default:
						break;
					case 3:
						transform.NJRotateY(0xE000);
						break;
					case 4:
						transform.NJRotateY(0xE700);
						break;

				}
			}
			BoundingSphere mainbounds = ObjectHelper.GetModelBounds(model, transform);
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			mainbounds = SAModel.Direct3D.Extensions.Merge(mainbounds, ObjectHelper.GetModelBounds(points, transform));
			transform.Pop();
			return SAModel.Direct3D.Extensions.Merge(mainbounds, pathbounds);
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
			new PropertySpec("Train Speed", typeof(float), "Extended", "This value is centered around -2.", null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Emerald ID", typeof(float), "Extended", "All Emerald pieces assigned to spawn with the train are of Type 7. A value between 1 and 0 prevents a piece from spawning.", null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Path ID", typeof(int), "Extended", null, null, (o) => (o.Rotation.X & 0xF) % 5, (o, v) => o.Rotation.X = (int)v),
			new PropertySpec("Switch Activation ID", typeof(int), "Extended", "If there is no switch with this ID in the level, the train will always be loaded.", null, (o) => o.Rotation.Z, (o, v) => o.Rotation.Z = (int)v),
			new PropertySpec("Whistle Point/Path Scale", typeof(float), "Extended", "For path ID 0, this scales the path. For all other paths, this is used to determine where the train blows its whistle on the path.", null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Ghost Train"; } }
		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
}