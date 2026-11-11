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

namespace SA2ObjectDefinitions.Common
{
	public class ESHOUKO : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] mesh;
		protected NJS_TEXLIST texlist;
		protected Texture[] texs;
		protected NJS_OBJECT point;
		protected Mesh[] pointmesh;

		protected PathData[] robotpaths;
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

		public void DefineSpline(int id, Device dev = null)
		{
			List<FVF_PositionColored> vertList = new List<FVF_PositionColored>();
			List<short> faceIndexList = new List<short>();

			float splineMeshRadius = 1F;

			Vector3 up = new Vector3(0, 1, 0);

			#region Segment vert/face creation
			short highestFaceIndex = 0;

			for (int i = 0; i < robotpaths[id].Path.Count - 1; i++) // don't process the last knot
			{
				Vector3 thisKnot = new Vector3(robotpaths[id].Path[i].Position.X, robotpaths[id].Path[i].Position.Y, robotpaths[id].Path[i].Position.Z);
				Vector3 nextKnot = new Vector3(robotpaths[id].Path[i + 1].Position.X, robotpaths[id].Path[i + 1].Position.Y, robotpaths[id].Path[i + 1].Position.Z);

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

			if (vertexHandleMesh == null) vertexHandleMesh = Mesh.Box(1, 1, 1);

			if (dev != null)
			textSprite = new Sprite(dev); // todo: do we really have to create this so often? Look into storing a cache list statically?
		}

		public override void Init(ObjectData data, string name)
		{
			robotpaths = new PathData[2];
			model = ObjectHelper.LoadModel("enemy/shouko/E_SHOUKO.sa2mdl");
			mesh = ObjectHelper.GetMeshes(model);

			point = ObjectHelper.LoadModel("OBJECT/SmallAnimalBubbleCollision.sa2mdl");
			pointmesh = ObjectHelper.GetMeshes(point);

			texlist = NJS_TEXLIST.Load("enemy/shouko/tls/SHOUKO.satex");

			string pathfolder = "Enemy/shouko/Paths";
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
					robotpaths[j] = PathData.Load(pathFiles[j]);
				}
			}
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			int pathType = ((item.Rotation.X >> 4) & 0xF) > 2 ? 0 : (item.Rotation.X >> 4) & 0xF;
			DefineSpline(pathType);
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y + 0x4000);
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh));
			result = HitResult.Min(result, pathmesh.CheckHit(Near, Far, Viewport, Projection, View, transform));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Scale);
			result = HitResult.Min(result, point.CheckHit(Near, Far, Viewport, Projection, View, transform, pointmesh));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			if (texs == null)
				texs = ObjectHelper.GetTextures("E_SHOTEX", texlist, dev);
			int pathType = ((item.Rotation.X >> 4) & 0xF) > 2 ? 0 : (item.Rotation.X >> 4) & 0xF;
			DefineSpline(pathType, dev);

			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Scale);
			result.AddRange(point.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, pointmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(point.DrawModelTreeInvert(transform, pointmesh));
			}
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, mesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(model.DrawModelTreeInvert(transform, mesh));
			}
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			RenderInfo outputInfo = new RenderInfo(pathmesh, 0, transform.Top, (item.Selected) ? SelectedMaterial : UnSelectedMaterial, null, EditorOptions.RenderFillMode, pathbounds);
			result.Add(outputInfo);
			textSprite.Begin(SpriteFlags.AlphaBlend);

			Matrix view = camera.ToMatrix();
			Matrix projection = Matrix.PerspectiveFovRH(camera.FOV, camera.Aspect, 1, camera.DrawDistance);
			Viewport viewport = dev.Viewport;

			if (item.Selected)
			{
				for (int vIndx = 0; vIndx < robotpaths[pathType].Path.Count(); vIndx++)
				{
					#region Draw Vertex IDs
					Vector3 screenCoordinates = viewport.Project(new Vector3(robotpaths[pathType].Path[vIndx].Position.X, robotpaths[pathType].Path[vIndx].Position.Y, robotpaths[pathType].Path[vIndx].Position.Z),
						projection, view, transform.Top);

					EditorOptions.OnscreenFont.DrawText(textSprite, vIndx.ToString(), (int)screenCoordinates.X, (int)screenCoordinates.Y, System.Drawing.Color.White.ToRawColorBGRA());
					#endregion

					#region Draw Vertex Handles
					transform.Push();

					transform.NJTranslate(robotpaths[pathType].Path[vIndx].Position.X, robotpaths[pathType].Path[vIndx].Position.Y, robotpaths[pathType].Path[vIndx].Position.Z);

					result.Add(new RenderInfo(vertexHandleMesh, 0, transform.Top, UnSelectedMaterial, null, FillMode.Solid, new BoundingSphere(robotpaths[pathType].Path[vIndx].Position.X,
						robotpaths[pathType].Path[vIndx].Position.Y, robotpaths[pathType].Path[vIndx].Position.Z, 1f)));

					result.Add(new RenderInfo(vertexHandleMesh, 0, transform.Top, SelectedMaterial, null, FillMode.Wireframe, new BoundingSphere(robotpaths[pathType].Path[vIndx].Position.X,
					robotpaths[pathType].Path[vIndx].Position.Y, robotpaths[pathType].Path[vIndx].Position.Z, 1f)));
					transform.Pop();
					#endregion
				}
			}

			textSprite.End();
			transform.Pop();

			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Scale);
			result.Add(new ModelTransform(point, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			int pathType = ((item.Rotation.X >> 4) & 0xF) > 2 ? 0 : (item.Rotation.X >> 4) & 0xF;
			DefineSpline(pathType);
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateY(item.Rotation.Y);
			BoundingSphere robotbounds = ObjectHelper.GetModelBounds(model, transform);
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Scale);
			BoundingSphere triggerbounds = ObjectHelper.GetModelBounds(point, transform);
			robotbounds = SAModel.Direct3D.Extensions.Merge(robotbounds, triggerbounds);
			transform.Pop();
			return SAModel.Direct3D.Extensions.Merge(robotbounds, pathbounds);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, 0, item.Rotation.Y + 0x4000, item.Rotation.Z);

			return matrix;
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			
			new PropertySpec("Plane Type", typeof(FighterJetType), "Extended", null, null, (o) => (FighterJetType)((o.Rotation.X & 0xF) > 4 ? 0 : o.Rotation.X & 0xF),
				(o, v) => { o.Rotation.X &= 0xFFF0; o.Rotation.X |= (byte)(FighterJetType)v; }),
			new PropertySpec("Bomb Delay", typeof(byte), "Extended", null, null, (o) => ((o.Rotation.X >> 12) & 0xFF),
				(o, v) => { o.Rotation.X &= 0x00FF; o.Rotation.X |= (byte)v << 12; }),
			new PropertySpec("Path ID", typeof(byte), "Extended", null, null, (o) => ((o.Rotation.X >> 4) & 0xF) > 2 ? 0 : (o.Rotation.X >> 4) & 0xF,
				(o, v) => { o.Rotation.X &= 0xFF0F; o.Rotation.X |= (byte)v << 4; }),
			new PropertySpec("Trigger Link ID", typeof(byte), "Extended", null, null, (o) => o.Rotation.Y & 0xF,
				(o, v) => { o.Rotation.Y &= 0xFFF0; o.Rotation.Y |= (byte)v; }),
			new PropertySpec("Bomb Displacement +-", typeof(int), "Extended", "Displacement affects the bombs that are dropped before and after the set location.", null, (o) => o.Rotation.Z,(o, v) => o.Rotation.Z  = (int)v),
			new PropertySpec("Attack X Coordinate", typeof(float), "Extended", "Blue Eagles drop bombs using this coordinate; Vulcan Fighters approach and fire using this coordinate.", null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Attack Y Coordinate", typeof(float), "Extended", "Blue Eagles drop bombs using this coordinate; Vulcan Fighters approach and fire using this coordinate.", null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Attack Z Coordinate", typeof(float), "Extended", "Blue Eagles drop bombs using this coordinate; Vulcan Fighters approach and fire using this coordinate.", null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "GUN Blue Eagle/Vulcan Fighter"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
		public enum FighterJetType
		{
			VulcanFighter,
			BlueEagle3Bomb,
			BlueEagle2Bomb,
			BlueEagle1Bomb
		}
	}
	public class SWSHOUKO : ObjectDefinition
	{
		private NJS_OBJECT[] models;
		private Mesh[][] meshes;

		public override void Init(ObjectData data, string name)
		{
			models = new NJS_OBJECT[2];
			meshes = new Mesh[2][];
			models[0] = ObjectHelper.LoadModel("OBJECT/CubeCollision.sa2mdl");
			models[1] = ObjectHelper.LoadModel("OBJECT/SmallAnimalBubbleCollision.sa2mdl");

			for (int i = 0; i < 2; i++)
				meshes[i] = ObjectHelper.GetMeshes(models[i]);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			var coltype = (item.Rotation.X & 1);
			transform.Push();
			transform.NJTranslate(item.Position);
			if (coltype == 0)
			{
				transform.NJScale((item.Scale.X + 10) * 2, (item.Scale.Y + 10) * 2, (item.Scale.Z + 10) * 2);
				transform.NJRotateZ(item.Rotation.Z);
			}
			if (coltype == 1)
				transform.NJScale((item.Scale.X + 10) / 10f, (item.Scale.Y + 10) / 10f, (item.Scale.Z + 10) / 10f);
			HitResult result = models[coltype].CheckHit(Near, Far, Viewport, Projection, View, transform, meshes[coltype]);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			var coltype = (item.Rotation.X & 1);
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (coltype == 0)
			{
				transform.NJScale((item.Scale.X + 10) * 2, (item.Scale.Y + 10) * 2, (item.Scale.Z + 10) * 2);
				transform.NJRotateZ(item.Rotation.Z);
			}
			if (coltype == 1)
				transform.NJScale((item.Scale.X + 10) / 10f, (item.Scale.Y + 10) / 10f, (item.Scale.Z + 10) / 10f);
			result.AddRange(models[coltype].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, meshes[coltype]));
			if (item.Selected)
				result.AddRange(models[coltype].DrawModelTreeInvert(transform, meshes[coltype]));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			return new List<ModelTransform>();
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			var coltype = (item.Rotation.X & 1);
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			if (coltype == 0)
			{
				transform.NJScale((item.Scale.X + 10) * 2, (item.Scale.Y + 10) * 2, (item.Scale.Z + 10) * 2);
				transform.NJRotateZ(item.Rotation.Z);
			}
			if (coltype == 1)
				transform.NJScale((item.Scale.X + 10) / 10f, (item.Scale.Y + 10) / 10f, (item.Scale.Z + 10) / 10f);
			return ObjectHelper.GetModelBounds(models[coltype], transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Collision Type", typeof(CollisionType), "Extended", null, null, (o) => (CollisionType)(o.Rotation.X & 1),
			(o, v) => { o.Rotation.X &= 0xFFF0; o.Rotation.X |= (byte)v; }),
			new PropertySpec("Fighter Jet ID", typeof(byte), "Extended", null, null, (o) => (o.Rotation.Y & 0xF),
			(o, v) => { o.Rotation.Y &= 0xFFF0; o.Rotation.Y |= (byte)v; }),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "GUN Fighter Jet Trigger"; } }
		public enum CollisionType : byte
		{
			Cube,
			Sphere
		}
	}
}