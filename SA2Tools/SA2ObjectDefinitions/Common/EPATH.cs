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
	public class EPATH : ObjectDefinition
	{
		protected NJS_OBJECT[] pathmodels;
		protected Mesh[][] pathmeshes;
		protected NJS_TEXLIST[] texlists;
		protected Texture[][] texs;
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
			pathmodels = new NJS_OBJECT[4];
			pathmeshes = new Mesh[4][];
			robotpaths = new PathData[12];
			texlists = new NJS_TEXLIST[2];
			texs = new Texture[2][];
			pathmodels[0] = ObjectHelper.LoadModel("enemy/path/E_PATH_GUNKUMI.sa2mdl");
			pathmodels[1] = ObjectHelper.LoadModel("enemy/path/E_PATH_BOMBKUMI.sa2mdl");
			pathmodels[2] = ObjectHelper.LoadModel("enemy/path/E_PATH_KYOKO.sa2mdl");
			pathmodels[3] = ObjectHelper.LoadModel("enemy/path/E_PATH_BACKKYOKO.sa2mdl");

			for (int i = 0; i < 4; i++)
			{
				pathmeshes[i] = ObjectHelper.GetMeshes(pathmodels[i]);
			}

			point = ObjectHelper.LoadModel("OBJECT/SmallAnimalBubbleCollision.sa2mdl");
			pointmesh = ObjectHelper.GetMeshes(point);

			texlists[0] = NJS_TEXLIST.Load("enemy/path/tls/PATHKUMI.satex");
			texlists[1] = NJS_TEXLIST.Load("enemy/path/tls/PATHKYOKO.satex");

			string pathfolder = "Enemy/path/Paths";
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
			int robotType = (item.Rotation.X & 0xF) > 3 ? 0 : item.Rotation.X & 0xF;
			int pathType = ((item.Rotation.X >> 12) & 0xF) > 11 ? 0 : (item.Rotation.X >> 12) & 0xF;
			float triggerX = (float)Math.Truncate(item.Scale.X);
			float triggerY = (float)Math.Truncate(item.Scale.Y);
			float triggerZ = (float)Math.Truncate(item.Scale.Z);
			float radius = (item.Scale.X - (float)Math.Truncate(item.Scale.X)) * 400F;
			DefineSpline(pathType);
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y + 0x4000);
			result = HitResult.Min(result, pathmodels[robotType].CheckHit(Near, Far, Viewport, Projection, View, transform, pathmeshes[robotType]));
			result = HitResult.Min(result, pathmesh.CheckHit(Near, Far, Viewport, Projection, View, transform));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X + triggerX, item.Position.Y + triggerY, item.Position.Z + triggerZ);
			transform.NJScale((radius + 10F) / 10F, (radius + 10F) / 10F, (radius + 10F) / 10F);
			result = HitResult.Min(result, point.CheckHit(Near, Far, Viewport, Projection, View, transform, pointmesh));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			if (texs[0] == null)
				texs[0] = ObjectHelper.GetTextures("E_PATHKUMITEX", texlists[0], dev);
			if (texs[1] == null)
				texs[1] = ObjectHelper.GetTextures("E_PATHKYOKOTEX", texlists[1], dev);
			int robotType = (item.Rotation.X & 0xF) > 3 ? 0 : item.Rotation.X & 0xF;
			int pathType = ((item.Rotation.X >> 12) & 0xF) > 11 ? 0 : (item.Rotation.X >> 12) & 0xF;
			int texID = robotType > 1 ? 1 : 0;
			float triggerX = (float)Math.Truncate(item.Scale.X);
			float triggerY = (float)Math.Truncate(item.Scale.Y);
			float triggerZ = (float)Math.Truncate(item.Scale.Z);
			float radius = (item.Scale.X - (float)Math.Truncate(item.Scale.X)) * 400F;
			DefineSpline(pathType, dev);


			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position.X + triggerX, item.Position.Y + triggerY, item.Position.Z + triggerZ);
			transform.NJScale((radius + 10F) / 10F, (radius + 10F) / 10F, (radius + 10F) / 10F);
			result.AddRange(point.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, pointmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(point.DrawModelTreeInvert(transform, pointmesh));
			}
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			result.AddRange(pathmodels[robotType].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs[texID], pathmeshes[robotType], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(pathmodels[robotType].DrawModelTreeInvert(transform, pathmeshes[robotType]));
			}

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
			int robotType = (item.Rotation.X & 0xF) > 3 ? 0 : item.Rotation.X & 0xF;
			float triggerX = (float)Math.Truncate(item.Scale.X);
			float triggerY = (float)Math.Truncate(item.Scale.Y);
			float triggerZ = (float)Math.Truncate(item.Scale.Z);
			float radius = (item.Scale.X - (float)Math.Truncate(item.Scale.X)) * 400F;
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			result.Add(new ModelTransform(pathmodels[robotType], transform.Top));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X + triggerX, item.Position.Y + triggerY, item.Position.Z + triggerZ);
			transform.NJScale((radius + 10F) / 10F, (radius + 10F) / 10F, (radius + 10F) / 10F);
			result.Add(new ModelTransform(point, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			int robotType = (item.Rotation.X & 0xF) > 3 ? 0 : item.Rotation.X & 0xF;
			int pathType = ((item.Rotation.X >> 12) & 0xF) > 11 ? 0 : (item.Rotation.X >> 12) & 0xF;
			float triggerX = (float)Math.Truncate(item.Scale.X);
			float triggerY = (float)Math.Truncate(item.Scale.Y);
			float triggerZ = (float)Math.Truncate(item.Scale.Z);
			float radius = (item.Scale.X - (float)Math.Truncate(item.Scale.X)) * 400F;
			DefineSpline(pathType);
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateY(item.Rotation.Y);
			BoundingSphere robotbounds = ObjectHelper.GetModelBounds(pathmodels[robotType], transform);
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X + triggerX, item.Position.Y + triggerY, item.Position.Z + triggerZ);
			transform.NJScale((radius + 10F) / 10F, (radius + 10F) / 10F, (radius + 10F) / 10F);
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
			
			new PropertySpec("Robot Type", typeof(PathRobotType), "Extended", null, null, (o) => (PathRobotType)((o.Rotation.X & 0xF) > 3 ? 0 : o.Rotation.X & 0xF),
				(o, v) => { o.Rotation.X &= 0xFFF0; o.Rotation.X |= (byte)(PathRobotType)v; }),
			new PropertySpec("Path ID", typeof(byte), "Extended", null, null, (o) => ((o.Rotation.X >> 12) & 0xF) > 11 ? 0 : (o.Rotation.X >> 12) & 0xF,
				(o, v) => { o.Rotation.X &= 0x0FFF; o.Rotation.X |= (byte)v << 12; }),
			new PropertySpec("Behavior Flag", typeof(byte), "Extended", null, null, (o) => (o.Rotation.X >> 4) & 0xF,
				(o, v) => { o.Rotation.X &= 0xFF0F; o.Rotation.X |= (byte)v << 4; }),
			new PropertySpec("Emerald/Item Link ID", typeof(byte), "Extended", null, null, (o) => o.Rotation.Y & 0xFF,
				(o, v) => { o.Rotation.Y &= 0xFF00; o.Rotation.Y |= (byte)v; }),
			new PropertySpec("Trigger X Displacement", typeof(float), "Extended", null, null, (o) => (float)Math.Truncate(o.Scale.X), 
				(o, v) => o.Scale.X = (o.Scale.X > 0 && (float)v > 0)? Math.Abs((float)v) + (Math.Abs(o.Scale.X) - Math.Abs((float)Math.Truncate(o.Scale.X))) : -(Math.Abs((float)v) + (Math.Abs(o.Scale.X) - Math.Abs((float)Math.Truncate(o.Scale.X))))),
			new PropertySpec("Trigger Radius", typeof(float), "Extended", "Value is multiplied by 4 and 10 is added by the game's code. This is tied to the decimal point of X Scale.", null,
				(o) => Math.Abs((o.Scale.X - (float)Math.Truncate(o.Scale.X)) * 100F),
				(o, v) => o.Scale.X = o.Scale.X > 0 ? Math.Abs((float)Math.Truncate(o.Scale.X)) + Math.Abs((float)v / 100F) : -(Math.Abs((float)Math.Truncate(o.Scale.X)) + Math.Abs((float)v / 100F))),
			new PropertySpec("Trigger Y Displacement", typeof(float), "Extended", null, null, (o) => (float)Math.Truncate(o.Scale.Y), 
				(o, v) => o.Scale.Y = (o.Scale.Y > 0 && (float)v > 0)? Math.Abs((float)v) + (Math.Abs(o.Scale.Y) - Math.Abs((float)Math.Truncate(o.Scale.Y))) : -(Math.Abs((float)v) + (Math.Abs(o.Scale.Y) - Math.Abs((float)Math.Truncate(o.Scale.Y))))),
			new PropertySpec("Robot Starting Point", typeof(float), "Extended", "Values between 0 and 15 are valid. All others will cause erratic behavior. This is tied to the decimal point of Y Scale.", null, 
				(o) => Math.Abs((o.Scale.Y - (float)Math.Truncate(o.Scale.Y)) * 100F), 
				(o, v) => o.Scale.Y = o.Scale.Y > 0 ? Math.Abs((float)Math.Truncate(o.Scale.Y)) + Math.Abs((float)v / 100F) : -(Math.Abs((float)Math.Truncate(o.Scale.Y)) + Math.Abs((float)v / 100F))),
			new PropertySpec("Trigger Z Displacement", typeof(float), "Extended", null, null, (o) => (float)Math.Truncate(o.Scale.Z), 
				(o, v) => o.Scale.Z = (o.Scale.Z > 0 && (float)v > 0) ? Math.Abs((float)v) + (Math.Abs(o.Scale.Z) - Math.Abs((float)Math.Truncate(o.Scale.Z))) : -(Math.Abs((float)v) + (Math.Abs(o.Scale.Z) - Math.Abs((float)Math.Truncate(o.Scale.Z))))),
			new PropertySpec("Robot Firing Rate", typeof(float), "Extended", "This is tied to the decimal point of Z Scale.", null, 
				(o) => Math.Abs(o.Scale.Z - (float)Math.Truncate(o.Scale.Z)), 
				(o, v) => o.Scale.Z = o.Scale.Z > 0 ? Math.Abs((float)Math.Truncate(o.Scale.Z)) + Math.Abs((float)v) : -(Math.Abs((float)Math.Truncate(o.Scale.Z)) + Math.Abs((float)v))),
			new PropertySpec("Fire Before First Range Point", typeof(bool), "Extended", null, null, (o) => Convert.ToBoolean((o.Rotation.Z >> 12) & 0xF),
				(o, v) => { o.Rotation.Z &= 0x0FFF; o.Rotation.Z |= Convert.ToByte((bool)v) << 12; }),
			new PropertySpec("Fire After Last Range Point", typeof(bool), "Extended", null, null, (o) => Convert.ToBoolean((o.Rotation.Z >> 4) & 0xF),
				(o, v) => { o.Rotation.Z &= 0xFF0F; o.Rotation.Z |= Convert.ToByte((bool)v) << 4; }),
			new PropertySpec("First Firing Range Point", typeof(byte), "Extended", "Determines the point where the path robot begins to attack the player. Setting the 'Fire Before First Range Point' boolean to True will ignore this data.", null, (o) => (o.Rotation.Z >> 8) & 0xF,
				(o, v) => { o.Rotation.Z &= 0xF0FF; o.Rotation.Z |= (byte)v << 8; }),
			new PropertySpec("Last Firing Range Point", typeof(byte), "Extended", "Determines the point where the path robot stops attacking the player. Setting the 'Fire After Last Range Point' boolean to True will ignore this data.", null, (o) => o.Rotation.Z & 0xF,
				(o, v) => { o.Rotation.Z &= 0xFFF0; o.Rotation.Z |= (byte)v; }),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "GUN Path Robot"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}

	public enum PathRobotType
	{
		GunWing,
		BombWing,
		SkyHawk,
		SkyHawkJet
	}
}